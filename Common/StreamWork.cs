using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Engine;

namespace Common
{
	public class StreamWork : IDisposable
	{
		public class MyAsyncInfo
		{
			public byte[] ByteArray;

			public Stream MyStream;

			public string Name;

			public MyAsyncInfo(byte[] array, Stream stream, string name)
			{
				ByteArray = array;
				MyStream = stream;
				Name = name;
			}
		}

		public Stream str;

		private Dictionary<string, bool> ready = new Dictionary<string, bool>();

		private Dictionary<string, List<byte>> data = new Dictionary<string, List<byte>>();

		private List<string> toDelete = new List<string>();

		public byte[] LoadData(string filename)
		{
			List<byte> list = new List<byte>();
			if (str == null)
			{
				try
				{
					str = new FileStream(filename, FileMode.Open);
				}
				catch (Exception exception)
				{
					Logger.LogException(exception, "Couldn't load file: " + filename);
					return new byte[0];
				}
			}
			int num;
			do
			{
				num = str.ReadByte();
				if (num != -1)
				{
					list.Add((byte)num);
				}
			}
			while (num != -1);
			Dispose();
			return list.ToArray();
		}

		public virtual void Dispose()
		{
			if (str != null)
			{
				str.Close();
				str.Dispose();
			}
		}

		public byte[] StartAsyncLoad(string filename)
		{
			try
			{
				if (ready.ContainsKey(filename))
				{
					if (ready[filename])
					{
						toDelete.Add(filename);
						return data[filename].ToArray();
					}
					return new byte[0];
				}
				FileStream fileStream = File.OpenRead(filename);
				if (!fileStream.CanRead)
				{
					MessageBox.Show("Cannot read stream.");
					fileStream.Close();
					return new byte[0];
				}
				foreach (string item in toDelete)
				{
					ready.Remove(item);
					data.Remove(item);
				}
				toDelete.Clear();
				byte[] array = new byte[1024];
				ready.Add(filename, value: false);
				data.Add(filename, new List<byte>());
				fileStream.BeginRead(array, 0, array.Length, ReadAsyncCallback, new MyAsyncInfo(array, fileStream, filename));
				return new byte[0];
			}
			catch (Exception exception)
			{
				Logger.LogException(exception);
				return new byte[0];
			}
		}

		private void ReadAsyncCallback(IAsyncResult ar)
		{
			MyAsyncInfo myAsyncInfo = ar.AsyncState as MyAsyncInfo;
			try
			{
				int num = myAsyncInfo.MyStream.EndRead(ar);
				for (int i = 0; i < num; i++)
				{
					data[myAsyncInfo.Name].Add(myAsyncInfo.ByteArray[i]);
				}
				if (myAsyncInfo.MyStream.Position < myAsyncInfo.MyStream.Length)
				{
					myAsyncInfo.MyStream.BeginRead(myAsyncInfo.ByteArray, 0, myAsyncInfo.ByteArray.Length, ReadAsyncCallback, myAsyncInfo);
					return;
				}
				ready[myAsyncInfo.Name] = true;
				myAsyncInfo.MyStream.Close();
			}
			catch (Exception exception)
			{
				Logger.LogException(exception);
				myAsyncInfo.MyStream.Close();
			}
		}
	}
}

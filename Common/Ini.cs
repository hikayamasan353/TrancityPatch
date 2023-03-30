using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
	public class Ini : IDisposable
	{
		private string filename;

		public Ini(string _filename, StreamWorkMode _mode)
		{
			filename = _filename;
		}

		public bool ReadBool(string section, string key, bool default_value)
		{
			if (bool.TryParse(Read(section, key, default_value.ToString()), out var result))
			{
				return result;
			}
			return default_value;
		}

		public double ReadDouble(string section, string key, double default_value)
		{
			if (double.TryParse(Read(section, key, default_value.ToString()), out var result))
			{
				return result;
			}
			return default_value;
		}

		public int ReadInt(string section, string key, int default_value)
		{
			if (int.TryParse(Read(section, key, default_value.ToString()), out var result))
			{
				return result;
			}
			return default_value;
		}

		public string Read(string section, string key, string default_value)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			GetPrivateProfileStringA(section, key, default_value, stringBuilder, 255, filename);
			return stringBuilder.ToString();
		}

		public void Write(string section, string key, int value)
		{
			Write(section, key, value.ToString());
		}

		public void Write(string section, string key, bool value)
		{
			Write(section, key, value.ToString());
		}

		public void Write(string section, string key, string value)
		{
			WritePrivateProfileStringA(section, key, value, filename);
		}

		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileIntA(string section, string key, int default_value, string filename);

		[DllImport("kernel32.dll")]
		private static extern ushort GetPrivateProfileStringA(string section, string key, string default_value, StringBuilder buffer, int char_count, string filename);

		[DllImport("kernel32.dll")]
		private static extern bool WritePrivateProfileStringA(string section, string key, string value, string filename);

		public void Dispose()
		{
		}
	}
}

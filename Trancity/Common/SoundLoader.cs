using System.Collections;
using SlimDX.Multimedia;

namespace Common
{
	public class SoundLoader : StreamWork
	{
		private struct SoundStruct
		{
			public string filename;

			public byte[] bytes;

			public WaveFormat Format;
		}

		public WaveFormat Format;

		public byte[] OutBytes;

		private static ArrayList _soundStructs = new ArrayList();

		public SoundLoader(string filename)
		{
			str = new WaveStream(filename);
			OutBytes = LoadData(filename);
			Format = ((WaveStream)str).Format;
			foreach (SoundStruct soundStruct in _soundStructs)
			{
				if (filename != soundStruct.filename)
				{
					continue;
				}
				OutBytes = null;
				OutBytes = (byte[])soundStruct.bytes.Clone();
				Format = soundStruct.Format;
				goto IL_00dd;
			}
			_soundStructs.Add(new SoundStruct
			{
				filename = filename,
				bytes = OutBytes,
				Format = Format
			});
			goto IL_00dd;
			IL_00dd:
			Dispose();
		}
	}
}

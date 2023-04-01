using Engine.Sound;

namespace Common
{
	public class MyXAudio2
	{
		public static SoundDevice Device;

		public static void Initialize(SoundDeviceType type)
		{
			Device = SoundDevice.CreateDevice(type);
		}

		public static void Free()
		{
			Device.Dispose();
		}
	}
}

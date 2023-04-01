using SlimDX.RawInput;

namespace Common
{
	public class MyRawInput
	{
		private static KeyboardInfo Keyboard = null;

		private static MouseInfo Mouse = null;

		private static RawMouseState _mouseState = new RawMouseState();

		private static bool reset_mouse = false;

		public bool Initialize()
		{
			Device.KeyboardInput += Device_KeyboardInput;
			Device.MouseInput += Device_MouseInput;
			return true;
		}

		private void Device_KeyboardInput(object sender, KeyboardInputEventArgs args)
		{
			_ = args.Device != Keyboard.Handle;
		}

		private void Device_MouseInput(object sender, MouseInputEventArgs args)
		{
			if (args.Device != Mouse.Handle)
			{
				return;
			}
			if (reset_mouse)
			{
				for (int i = 0; i < 5; i++)
				{
					_mouseState.RawMoseButtons[i] = false;
				}
				reset_mouse = false;
			}
			_mouseState.X = args.X;
			_mouseState.Y = args.Y;
			if (args.ButtonFlags != 0)
			{
				switch (args.ButtonFlags)
				{
				case MouseButtonFlags.LeftDown:
					_mouseState.RawMoseButtons[0] = true;
					break;
				case MouseButtonFlags.RightDown:
					_mouseState.RawMoseButtons[1] = true;
					break;
				case MouseButtonFlags.MiddleDown:
					_mouseState.RawMoseButtons[2] = true;
					break;
				case MouseButtonFlags.Button4Down:
					_mouseState.RawMoseButtons[3] = true;
					break;
				case MouseButtonFlags.Button5Down:
					_mouseState.RawMoseButtons[4] = true;
					break;
				case MouseButtonFlags.MouseWheel:
					_mouseState.Z = args.WheelDelta;
					break;
				}
			}
		}

		public RawMouseState GetMouseState()
		{
			reset_mouse = true;
			return _mouseState;
		}
	}
}

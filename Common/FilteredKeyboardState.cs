using System;
using SlimDX.DirectInput;

namespace Common
{
	public class FilteredKeyboardState
	{
		public Keyboard device;

		private KeyboardState InputState;

		public bool[] key_pressed;

		public int[] keyticks;

		public int lasttick;

		public const int Max_Available_Keys = 256;

		public int[] repeat_tick;

		public bool this[Key key]
		{
			get
			{
				return key_pressed[(int)key];
			}
			set
			{
				key_pressed[(int)key] = value;
			}
		}

		public FilteredKeyboardState(Keyboard inputDevice, int ArrowTicks, int OtherTicks)
		{
			device = inputDevice;
			keyticks = new int[256];
			key_pressed = new bool[256];
			lasttick = Environment.TickCount;
			repeat_tick = GenerateDefaultRepeatTicks(ArrowTicks, OtherTicks);
		}

		public static int[] GenerateDefaultRepeatTicks(int ArrowTicks, int OtherTicks)
		{
			int[] array = new int[256];
			for (int i = 0; i < 256; i++)
			{
				array[i] = OtherTicks;
			}
			array[77] = 0;
			array[75] = 0;
			array[78] = 0;
			array[119] = 0;
			array[116] = 0;
			array[120] = 0;
			array[132] = ArrowTicks;
			array[50] = ArrowTicks;
			array[76] = ArrowTicks;
			array[118] = ArrowTicks;
			return array;
		}

		public void Refresh()
		{
			InputState = device.GetCurrentState();
			int num = Environment.TickCount - lasttick;
			lasttick = Environment.TickCount;
			for (int i = 0; i < 256; i++)
			{
				if (InputState.IsPressed((Key)i))
				{
					if (!key_pressed[i] && keyticks[i] == 0)
					{
						key_pressed[i] = true;
						keyticks[i] = 1;
						continue;
					}
					keyticks[i] += num;
					if (keyticks[i] >= repeat_tick[i] && repeat_tick[i] != -1)
					{
						key_pressed[i] = true;
						keyticks[i] = 1;
					}
					else
					{
						key_pressed[i] = false;
					}
				}
				else
				{
					key_pressed[i] = false;
					keyticks[i] = 0;
				}
			}
		}

		public bool IsDirtyPressed(Key key)
		{
			return InputState.IsPressed(key);
		}
	}
}

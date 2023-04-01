using System;
using SlimDX.DirectInput;

namespace Common
{
	public class FilteredJoystickState
	{
		public bool Arrow_Pressed;

		public int Arrow_Repeat_Ticks;

		public int Arrow_State = -1;

		public int Arrow_Tick;

		public Joystick device;

		public JoystickState InputState;

		public bool[] key_pressed;

		public bool[] key_pressed_unfiltered;

		public int[] keyticks;

		public int lasttick;

		public const int Max_Available_Buttons = 16;

		public int[] repeat_tick;

		public bool this[int button]
		{
			get
			{
				return this[button, true];
			}
			set
			{
				key_pressed[button] = value;
			}
		}

		public bool this[int button, bool filter]
		{
			get
			{
				if (filter)
				{
					return key_pressed[button];
				}
				return key_pressed_unfiltered[button];
			}
			set
			{
				key_pressed[button] = value;
			}
		}

		public FilteredJoystickState(Joystick inputDevice, int ArrowTicks, int OtherTicks)
		{
			device = inputDevice;
			keyticks = new int[16];
			key_pressed = new bool[16];
			key_pressed_unfiltered = new bool[16];
			lasttick = Environment.TickCount;
			repeat_tick = GenerateDefaultRepeatTicks(OtherTicks);
			Arrow_Tick = ArrowTicks;
			if (repeat_tick.Length < 16)
			{
				throw new Exception("The repeat tick array lenth must be at least " + 16 + "!");
			}
		}

		public static int[] GenerateDefaultRepeatTicks(int OtherTicks)
		{
			int[] array = new int[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = OtherTicks;
			}
			return array;
		}

		public void Refresh()
		{
			device.Poll();
			InputState = device.GetCurrentState();
			int num = Environment.TickCount - lasttick;
			lasttick = Environment.TickCount;
			for (int i = 0; i < 16; i++)
			{
				if (InputState.GetButtons().Length > i && InputState.GetButtons()[i])
				{
					key_pressed_unfiltered[i] = true;
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
					key_pressed_unfiltered[i] = false;
					key_pressed[i] = false;
					keyticks[i] = 0;
				}
			}
			int num2 = InputState.GetPointOfViewControllers()[0];
			if (num2 != -1)
			{
				if ((!Arrow_Pressed && Arrow_Tick == 0) || num2 != Arrow_State)
				{
					Arrow_Pressed = true;
					Arrow_Tick = 1;
				}
				else
				{
					Arrow_Tick += num;
					if (Arrow_Tick >= Arrow_Repeat_Ticks && Arrow_Repeat_Ticks != -1)
					{
						Arrow_Pressed = true;
						Arrow_Tick = 1;
					}
					else
					{
						Arrow_Pressed = false;
					}
				}
			}
			else
			{
				Arrow_Pressed = false;
				Arrow_Tick = 0;
			}
			Arrow_State = num2;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Engine;
using SlimDX.DirectInput;

namespace Common
{
	public class MyDirectInput
	{
		public static bool alt_f4;

		public const int AxisRange = 1024;

		public static Guid[] DeviceGuids;

		public static string[] DeviceNames;

		public static FilteredJoystickState[] Joystick_FilteredStates;

		public static JoystickState[] Joystick_States;

		public static Joystick[] JoystickDevices;

		public static FilteredKeyboardState Key_State;

		public static Keyboard Keyboard_Device = null;

		public static byte[] last_buttons = new byte[5];

		public static Mouse Mouse_Device = null;

		public static MouseState Mouse_State;

		private static DirectInput dinput;

		public static bool Acquire()
		{
			try
			{
				Keyboard_Device.Acquire();
				Mouse_Device.Acquire();
				for (int i = 0; i < JoystickDevices.Length; i++)
				{
					JoystickDevices[i].Acquire();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool Unacquire()
		{
			try
			{
				Keyboard_Device.Unacquire();
				Mouse_Device.Unacquire();
				for (int i = 0; i < JoystickDevices.Length; i++)
				{
					JoystickDevices[i].Unacquire();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static void EnumerateDevices()
		{
			dinput = new DirectInput();
			List<DeviceInstance> list = new List<DeviceInstance>(dinput.GetDevices(DeviceClass.Keyboard, DeviceEnumerationFlags.AttachedOnly));
			List<DeviceInstance> list2 = new List<DeviceInstance>(dinput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly));
			DeviceGuids = new Guid[list2.Count + 1];
			DeviceNames = new string[list2.Count + 1];
			int num = 0;
			foreach (DeviceInstance item in list2)
			{
				DeviceGuids[num] = item.InstanceGuid;
				DeviceNames[num] = item.InstanceName;
				num++;
			}
			foreach (DeviceInstance item2 in list)
			{
				if (item2.Type == DeviceType.Keyboard)
				{
					DeviceGuids[num] = item2.InstanceGuid;
					DeviceNames[num] = item2.InstanceName;
					num++;
					break;
				}
			}
		}

		public static void Free()
		{
			if (Keyboard_Device != null)
			{
				Keyboard_Device.Unacquire();
				Keyboard_Device.Dispose();
				Keyboard_Device = null;
			}
			if (Mouse_Device != null)
			{
				Mouse_Device.Unacquire();
				Mouse_Device.Dispose();
				Mouse_Device = null;
			}
			for (int i = 0; i < JoystickDevices.Length; i++)
			{
				if (JoystickDevices[i] != null)
				{
					JoystickDevices[i].Unacquire();
					JoystickDevices[i].Dispose();
					JoystickDevices[i] = null;
				}
			}
		}

		public static bool Initialize(Control control)
		{
			return Initialize(control, keyboard_exclusive: false, mouse_exclusive: false);
		}

		public static bool Initialize(Control control, bool keyboard_exclusive, bool mouse_exclusive)
		{
			try
			{
				Keyboard_Device = new Keyboard(dinput);
				if (Key_State == null)
				{
					Key_State = new FilteredKeyboardState(Keyboard_Device, 200, -1);
				}
				else
				{
					Key_State.device = Keyboard_Device;
				}
				Mouse_Device = new Mouse(dinput);
				JoystickDevices = new Joystick[DeviceGuids.Length - 1];
				Joystick_FilteredStates = new FilteredJoystickState[JoystickDevices.Length];
				for (int i = 0; i < JoystickDevices.Length; i++)
				{
					JoystickDevices[i] = new Joystick(dinput, DeviceGuids[i]);
					Joystick_FilteredStates[i] = new FilteredJoystickState(JoystickDevices[i], 200, -1);
				}
				Joystick_States = new JoystickState[JoystickDevices.Length];
				CooperativeLevel cooperativeLevel = CooperativeLevel.Foreground | CooperativeLevel.Exclusive;
				CooperativeLevel cooperativeLevel2 = CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive;
				Keyboard_Device.SetCooperativeLevel(control, keyboard_exclusive ? cooperativeLevel : cooperativeLevel2);
				Mouse_Device.SetCooperativeLevel(control, mouse_exclusive ? cooperativeLevel : cooperativeLevel2);
				for (int j = 0; j < JoystickDevices.Length; j++)
				{
					JoystickDevices[j].SetCooperativeLevel(control, cooperativeLevel);
					JoystickDevices[j].Properties.SetRange(-1024, 1024);
					JoystickDevices[j].Properties.DeadZone = 500;
				}
			}
			catch (DirectInputException exception)
			{
				Logger.LogException(exception);
				Free();
				return false;
			}
			Acquire();
			return true;
		}

		public static bool Process()
		{
			try
			{
				Key_State.Refresh();
				Mouse_State = Mouse_Device.GetCurrentState();
				for (int i = 0; i < JoystickDevices.Length; i++)
				{
					Joystick_FilteredStates[i].Refresh();
					Joystick_States[i] = Joystick_FilteredStates[i].InputState;
				}
			}
			catch
			{
				return false;
			}
			if ((Key_State.IsDirtyPressed(Key.LeftAlt) && Key_State.IsDirtyPressed(Key.F4)) || alt_f4)
			{
				alt_f4 = true;
				Application.Exit();
				return false;
			}
			return true;
		}
	}
}

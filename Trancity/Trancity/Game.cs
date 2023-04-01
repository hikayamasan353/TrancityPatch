using System;
using System.Drawing;
using Common;
using Engine;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace Trancity
{
	public class Game
	{
		private bool[] _lastMouseButtons = new bool[5];

		public bool активна = true;

		public Игрок[] игроки;

		public World мир;

		private int _транспортPosIndex;

		private const int num = 1024;

		public static bool fmouse;

		public MyMenu menu;

		public static int col;

		public static int row;

		public void Process_Input()
		{
			if (активна && мир.транспорты.Count > 0)
			{
				_транспортPosIndex++;
				if (_транспортPosIndex >= мир.транспорты.Count)
				{
					_транспортPosIndex = 0;
				}
				foreach (Положение item in ((Transport)мир.транспорты[_транспортPosIndex]).найденные_положения)
				{
					if (item.Дорога != null)
					{
						item.Дорога.занятыеПоложения.Remove(item);
					}
				}
				((Transport)мир.транспорты[_транспортPosIndex]).НайтиВсеПоложения(мир);
				foreach (Положение item2 in ((Transport)мир.транспорты[_транспортPosIndex]).найденные_положения)
				{
					if (item2.Дорога != null)
					{
						item2.Дорога.занятыеПоложения.Add(item2);
					}
				}
				foreach (Transport item3 in мир.транспорты)
				{
					if (item3.управление.автоматическое)
					{
						item3.АвтоматическиУправлять(мир);
					}
				}
			}
			MouseState mouse_State = MyDirectInput.Mouse_State;
			FilteredKeyboardState key_State = MyDirectInput.Key_State;
			JoystickState[] joystick_States = MyDirectInput.Joystick_States;
			FilteredJoystickState[] joystick_FilteredStates = MyDirectInput.Joystick_FilteredStates;
			Joystick[] joystickDevices = MyDirectInput.JoystickDevices;
			Guid[] deviceGuids = MyDirectInput.DeviceGuids;
			bool[] buttons = mouse_State.GetButtons();
			int x = mouse_State.X;
			int y = mouse_State.Y;
			int z = mouse_State.Z;
			if (MyDirectInput.alt_f4)
			{
				return;
			}
			if (!активна)
			{
				menu.Refresh();
			}
			bool flag = false;
			if (key_State[Key.PageUp])
			{
				MyDirect3D.light_intency = Math.Min(MyDirect3D.light_intency + 0.1f, 1f);
				flag = true;
			}
			if (key_State[Key.PageDown])
			{
				MyDirect3D.light_intency = Math.Max(MyDirect3D.light_intency - 0.1f, 0f);
				flag = true;
			}
			if (flag)
			{
				int num = (int)((float)MyDirect3D.light_color * MyDirect3D.light_intency);
				for (int i = 0; i < 11; i += 2)
				{
					Light light = MyDirect3D.device.GetLight(i);
					light.Diffuse = Color.FromArgb(num, num, num);
					MyDirect3D.device.SetLight(i, light);
				}
			}
			if (key_State[Key.Escape])
			{
				активна = !активна;
			}
			for (int j = 0; j < joystickDevices.Length; j++)
			{
				if (joystick_FilteredStates[j][9])
				{
					активна = !активна;
				}
			}
			if (активна)
			{
				if (key_State[Key.Tab])
				{
					MyDirect3D.вид_сверху = !MyDirect3D.вид_сверху;
				}
				Игрок[] array = игроки;
				foreach (Игрок игрок in array)
				{
					if (игрок.управляемыйОбъект != null && (игрок.управляемыйОбъект.position - игрок.cameraPosition.XZPoint).Modulus > 200.0)
					{
						игрок.управляемыйОбъект.управление = Управление.Автоматическое;
						игрок.управляемыйОбъект = null;
						игрок.объектПривязки = null;
					}
					int num2 = -1;
					for (int l = 0; l < joystickDevices.Length; l++)
					{
						if (игрок.inputGuid == deviceGuids[l])
						{
							num2 = l;
							break;
						}
					}
					if (num2 == -1)
					{
						if (buttons[1] && !_lastMouseButtons[1])
						{
							Привязывать(игрок);
						}
						if (!MyDirect3D.вид_сверху)
						{
							if (!buttons[0])
							{
								игрок.cameraRotationChange.x -= 0.001 * (double)x;
								игрок.cameraRotationChange.y -= 0.001 * (double)y;
							}
							else
							{
								DoublePoint doublePoint = new DoublePoint(игрок.cameraPositionChange.x, игрок.cameraPositionChange.z) / new DoublePoint(игрок.cameraRotation.x);
								doublePoint.x -= 0.01 * (double)y;
								doublePoint.y -= 0.01 * (double)x;
								игрок.cameraPositionChange.x = (doublePoint * new DoublePoint(игрок.cameraRotation.x)).x;
								игрок.cameraPositionChange.z = (doublePoint * new DoublePoint(игрок.cameraRotation.x)).y;
							}
							игрок.cameraPositionChange.y += 0.001 * (double)z;
						}
						else
						{
							MyDirect3D.масштаб += 0.001 * (double)z;
							if (MyDirect3D.масштаб <= 2.5)
							{
								MyDirect3D.масштаб = 2.5;
							}
							if (buttons[0])
							{
								DoublePoint doublePoint2 = new DoublePoint(игрок.cameraPositionChange.x, игрок.cameraPositionChange.z);
								doublePoint2.x += 0.01 * (double)x;
								doublePoint2.y -= 0.01 * (double)y;
								игрок.cameraPositionChange.x = doublePoint2.x;
								игрок.cameraPositionChange.z = doublePoint2.y;
							}
						}
						_lastMouseButtons = buttons;
					}
					else
					{
						FilteredJoystickState filteredJoystickState = joystick_FilteredStates[num2];
						JoystickState joystickState = joystick_States[num2];
						if (filteredJoystickState[8])
						{
							Привязывать(игрок);
						}
						double num3 = 0.05 * (double)joystickState.X / 1024.0;
						double num4 = 0.02 * (double)joystickState.Y / 1024.0;
						double num5 = 0.05 * (double)joystickState.Z / 1024.0;
						num5 = joystickState.GetPointOfViewControllers()[0] switch
						{
							0 => 0.04, 
							18000 => -0.04, 
							_ => 0.0, 
						};
						if (игрок.управляемыйОбъект != null && игрок.управляемыйОбъект is Безрельсовый_Транспорт && игрок.управляемыйОбъект.управление.ручное)
						{
							if (!filteredJoystickState[4, false])
							{
								int button = 6;
								if (((Transport)игрок.управляемыйОбъект).система_управления is Система_управления.Автобусная)
								{
									button = 10;
								}
								if (filteredJoystickState[button, false])
								{
									игрок.cameraRotationChange.x -= num3;
									игрок.cameraRotationChange.y -= num4;
								}
							}
							else
							{
								DoublePoint doublePoint3 = new DoublePoint(игрок.cameraPositionChange.x, игрок.cameraPositionChange.z) / new DoublePoint(игрок.cameraRotation.x);
								doublePoint3.x -= 10.0 * num4;
								doublePoint3.y -= 10.0 * num3;
								игрок.cameraPositionChange.x = (doublePoint3 * new DoublePoint(игрок.cameraRotation.x)).x;
								игрок.cameraPositionChange.z = (doublePoint3 * new DoublePoint(игрок.cameraRotation.x)).y;
								игрок.cameraPositionChange.y += num5;
							}
						}
						else if (!filteredJoystickState[4, false])
						{
							игрок.cameraRotationChange.x -= num3;
							игрок.cameraRotationChange.y -= num4;
						}
						else
						{
							DoublePoint doublePoint4 = new DoublePoint(игрок.cameraPositionChange.x, игрок.cameraPositionChange.z) / new DoublePoint(игрок.cameraRotation.x);
							doublePoint4.x -= 10.0 * num4;
							doublePoint4.y -= 10.0 * num3;
							игрок.cameraPositionChange.x = (doublePoint4 * new DoublePoint(игрок.cameraRotation.x)).x;
							игрок.cameraPositionChange.z = (doublePoint4 * new DoublePoint(игрок.cameraRotation.x)).y;
							игрок.cameraPositionChange.y += num5;
						}
					}
					if (игрок.управляемыйОбъект == null)
					{
						continue;
					}
					Transport transport2 = (Transport)игрок.управляемыйОбъект;
					if (num2 == -1)
					{
						if (key_State[Key.A])
						{
							transport2.управление.автоматическое = !transport2.управление.автоматическое;
							transport2.currentStop = (transport2.nextStop = null);
							transport2.stopIndex = 0;
						}
						if (key_State[Key.M])
						{
							transport2.управление.ручное = !transport2.управление.ручное;
						}
						if (transport2.управление.ручное)
						{
							if (key_State[Key.Y])
							{
								transport2.включен = !transport2.включен;
							}
							if (key_State[Key.S])
							{
								if (!transport2.двери_водителя_закрыты)
								{
									transport2.ОткрытьДвериВодителя(открыть: false);
								}
								else if (!transport2.двери_водителя_открыты)
								{
									transport2.ОткрытьДвериВодителя(открыть: true);
								}
							}
							if (key_State[Key.D])
							{
								if (!transport2.двери_закрыты)
								{
									transport2.ОткрытьДвери(открыть: false);
								}
								else if (!transport2.двери_открыты)
								{
									transport2.ОткрытьДвери(открыть: true);
								}
							}
							if (key_State[Key.D1])
							{
								if (!transport2.ДверьЗакрыта(0))
								{
									transport2.ОткрытьДвери(0, открыть: false);
								}
								else if (!transport2.ДверьОткрыта(0))
								{
									transport2.ОткрытьДвери(0, открыть: true);
								}
							}
							if (key_State[Key.D2])
							{
								if (!transport2.ДверьЗакрыта(1))
								{
									transport2.ОткрытьДвери(1, открыть: false);
								}
								else if (!transport2.ДверьОткрыта(0))
								{
									transport2.ОткрытьДвери(1, открыть: true);
								}
							}
							if (key_State[Key.D3])
							{
								if (!transport2.ДверьЗакрыта(2))
								{
									transport2.ОткрытьДвери(2, открыть: false);
								}
								else if (!transport2.ДверьОткрыта(0))
								{
									transport2.ОткрытьДвери(2, открыть: true);
								}
							}
							if (key_State[Key.D4])
							{
								if (!transport2.ДверьЗакрыта(3))
								{
									transport2.ОткрытьДвери(3, открыть: false);
								}
								else if (!transport2.ДверьОткрыта(0))
								{
									transport2.ОткрытьДвери(3, открыть: true);
								}
							}
							if (key_State[Key.D5])
							{
								if (!transport2.ДверьЗакрыта(4))
								{
									transport2.ОткрытьДвери(4, открыть: false);
								}
								else if (!transport2.ДверьОткрыта(0))
								{
									transport2.ОткрытьДвери(4, открыть: true);
								}
							}
							if (key_State[Key.B])
							{
								transport2.stand_brake = !transport2.stand_brake;
							}
							if (key_State[Key.F])
							{
								transport2.включены_фары = !transport2.включены_фары;
							}
						}
						if (игрок.объектПривязки != null)
						{
							if (key_State[Key.C])
							{
								transport2.SetCamera(0, игрок);
							}
							if (key_State[Key.F2])
							{
								transport2.SetCamera(1, игрок);
							}
							if (key_State[Key.F3])
							{
								transport2.SetCamera(2, игрок);
							}
							if (key_State[Key.F4])
							{
								transport2.SetCamera(3, игрок);
							}
						}
					}
					if (игрок.управляемыйОбъект is Трамвай)
					{
						Трамвай трамвай = (Трамвай)игрок.управляемыйОбъект;
						if (num2 == -1)
						{
							if (трамвай.управление.ручное)
							{
								if (key_State[Key.T])
								{
									if (трамвай.токоприёмник.опущен)
									{
										трамвай.токоприёмник.НайтиПровод(мир.контактныеПровода2);
										if (трамвай.токоприёмник.Провод != null)
										{
											трамвай.токоприёмник.поднимается = true;
										}
									}
									else if (трамвай.токоприёмник.поднят)
									{
										трамвай.токоприёмник.поднимается = false;
									}
								}
								if (key_State[Key.D6])
								{
									if (!трамвай.ДверьЗакрыта(5))
									{
										трамвай.ОткрытьДвери(5, открыть: false);
									}
									else if (!трамвай.ДверьОткрыта(5))
									{
										трамвай.ОткрытьДвери(5, открыть: true);
									}
								}
								if (key_State[Key.D7])
								{
									if (!трамвай.ДверьЗакрыта(6))
									{
										трамвай.ОткрытьДвери(6, открыть: false);
									}
									else if (!трамвай.ДверьОткрыта(6))
									{
										трамвай.ОткрытьДвери(6, открыть: true);
									}
								}
								if (key_State[Key.D8])
								{
									if (!трамвай.ДверьЗакрыта(7))
									{
										трамвай.ОткрытьДвери(7, открыть: false);
									}
									else if (!трамвай.ДверьОткрыта(7))
									{
										трамвай.ОткрытьДвери(7, открыть: true);
									}
								}
								if (key_State[Key.D9])
								{
									if (!трамвай.ДверьЗакрыта(8))
									{
										трамвай.ОткрытьДвери(8, открыть: false);
									}
									else if (!трамвай.ДверьОткрыта(8))
									{
										трамвай.ОткрытьДвери(8, открыть: true);
									}
								}
								if (key_State[Key.D0])
								{
									if (!трамвай.ДверьЗакрыта(9))
									{
										трамвай.ОткрытьДвери(9, открыть: false);
									}
									else if (!трамвай.ДверьОткрыта(9))
									{
										трамвай.ОткрытьДвери(9, открыть: true);
									}
								}
								if (трамвай.система_управления is Система_управления.РКСУ_Трамвай)
								{
									Система_управления.РКСУ_Трамвай рКСУ_Трамвай = (Система_управления.РКСУ_Трамвай)трамвай.система_управления;
									if (key_State[Key.Backspace] && трамвай.скорость == 0.0 && рКСУ_Трамвай.позиция_контроллера == 0)
									{
										рКСУ_Трамвай.позиция_реверсора = -рКСУ_Трамвай.позиция_реверсора;
									}
									if (key_State[Key.DownArrow])
									{
										if (рКСУ_Трамвай.позиция_контроллера > рКСУ_Трамвай.позиция_min)
										{
											рКСУ_Трамвай.позиция_контроллера--;
										}
									}
									else if (key_State[Key.UpArrow] && рКСУ_Трамвай.позиция_контроллера < рКСУ_Трамвай.позиция_max)
									{
										рКСУ_Трамвай.позиция_контроллера++;
									}
								}
								if (!key_State[Key.RightControl])
								{
									if (key_State[Key.LeftArrow])
									{
										if (трамвай.указатель_поворота >= 0)
										{
											трамвай.указатель_поворота = -1;
											трамвай.аварийная_сигнализация = false;
										}
										else
										{
											трамвай.указатель_поворота = 0;
										}
									}
									if (key_State[Key.RightArrow])
									{
										if (трамвай.указатель_поворота <= 0)
										{
											трамвай.указатель_поворота = 1;
											трамвай.аварийная_сигнализация = false;
										}
										else
										{
											трамвай.указатель_поворота = 0;
										}
									}
								}
								else if (трамвай.скорость == 0.0 && трамвай.двери_водителя_открыты && трамвай.передняя_ось.текущий_рельс.следующие_рельсы.Length > 1 && трамвай.передняя_ось.пройденное_расстояние_по_рельсу > трамвай.передняя_ось.текущий_рельс.Длина - 8.0)
								{
									if (key_State[Key.LeftArrow])
									{
										трамвай.передняя_ось.текущий_рельс.следующий_рельс = 0;
									}
									if (key_State[Key.RightArrow])
									{
										трамвай.передняя_ось.текущий_рельс.следующий_рельс = 1;
									}
								}
								if (key_State[Key.Q])
								{
									трамвай.аварийная_сигнализация = !трамвай.аварийная_сигнализация;
								}
							}
						}
						else
						{
							FilteredJoystickState filteredJoystickState2 = joystick_FilteredStates[num2];
							JoystickState joystickState2 = joystick_States[num2];
							int num6 = joystickState2.GetPointOfViewControllers()[0];
							if (num6 >= 0)
							{
								num6 = (int)Math.Round((double)num6 * 1.0 / 4500.0);
							}
							_ = filteredJoystickState2[6];
							if (filteredJoystickState2[5])
							{
								трамвай.управление.автоматическое = !трамвай.управление.автоматическое;
								трамвай.управление.ручное = !трамвай.управление.ручное;
							}
							if (трамвай.управление.ручное)
							{
								if (filteredJoystickState2[11])
								{
									трамвай.включен = !трамвай.включен;
								}
								if (filteredJoystickState2[2])
								{
									if (трамвай.токоприёмник.опущен)
									{
										трамвай.токоприёмник.НайтиПровод(мир.контактныеПровода2);
										if (трамвай.токоприёмник.Провод != null)
										{
											трамвай.токоприёмник.поднимается = true;
										}
									}
									else if (трамвай.токоприёмник.поднят)
									{
										трамвай.токоприёмник.поднимается = false;
									}
								}
								if (filteredJoystickState2[0])
								{
									if (!трамвай.двери_водителя_закрыты)
									{
										трамвай.ОткрытьДвериВодителя(открыть: false);
									}
									else if (!трамвай.двери_водителя_открыты)
									{
										трамвай.ОткрытьДвериВодителя(открыть: true);
									}
								}
								if (filteredJoystickState2[1])
								{
									if (!трамвай.двери_закрыты)
									{
										трамвай.ОткрытьДвери(открыть: false);
									}
									else if (!трамвай.двери_открыты)
									{
										трамвай.ОткрытьДвери(открыть: true);
									}
								}
								if (трамвай.система_управления is Система_управления.РКСУ_Трамвай)
								{
									Система_управления.РКСУ_Трамвай рКСУ_Трамвай2 = (Система_управления.РКСУ_Трамвай)трамвай.система_управления;
									switch (-5 * joystickState2.RotationZ / 1024)
									{
									case -5:
										рКСУ_Трамвай2.позиция_контроллера = -5;
										break;
									case -4:
										if (рКСУ_Трамвай2.позиция_контроллера > -4)
										{
											рКСУ_Трамвай2.позиция_контроллера = -4;
										}
										break;
									case -3:
										if (рКСУ_Трамвай2.позиция_контроллера > -3)
										{
											рКСУ_Трамвай2.позиция_контроллера = -3;
										}
										break;
									case -2:
										if (рКСУ_Трамвай2.позиция_контроллера > -2)
										{
											рКСУ_Трамвай2.позиция_контроллера = -2;
										}
										break;
									case -1:
										if (рКСУ_Трамвай2.позиция_контроллера > -1)
										{
											рКСУ_Трамвай2.позиция_контроллера = -1;
										}
										break;
									case 0:
										if (joystickState2.RotationZ > 0)
										{
											if (рКСУ_Трамвай2.позиция_контроллера > 0)
											{
												рКСУ_Трамвай2.позиция_контроллера = 0;
											}
										}
										else if (joystickState2.RotationZ < 0 && рКСУ_Трамвай2.позиция_контроллера < 0)
										{
											рКСУ_Трамвай2.позиция_контроллера = 0;
										}
										break;
									case 1:
										if (рКСУ_Трамвай2.позиция_контроллера < 1)
										{
											рКСУ_Трамвай2.позиция_контроллера = 1;
										}
										break;
									case 2:
										if (рКСУ_Трамвай2.позиция_контроллера < 1)
										{
											рКСУ_Трамвай2.позиция_контроллера = 1;
										}
										break;
									case 3:
										if (рКСУ_Трамвай2.позиция_контроллера < 2)
										{
											рКСУ_Трамвай2.позиция_контроллера = 2;
										}
										break;
									case 4:
										if (рКСУ_Трамвай2.позиция_контроллера < 3)
										{
											рКСУ_Трамвай2.позиция_контроллера = 3;
										}
										break;
									case 5:
										рКСУ_Трамвай2.позиция_контроллера = 4;
										break;
									}
									if (filteredJoystickState2[7] && трамвай.скорость == 0.0 && рКСУ_Трамвай2.позиция_контроллера == 0)
									{
										рКСУ_Трамвай2.позиция_реверсора = -рКСУ_Трамвай2.позиция_реверсора;
									}
								}
								switch (num6)
								{
								case 0:
									трамвай.указатель_поворота = 0;
									трамвай.аварийная_сигнализация = false;
									break;
								case 2:
									трамвай.указатель_поворота = 1;
									трамвай.аварийная_сигнализация = false;
									break;
								case 4:
									трамвай.указатель_поворота = 0;
									трамвай.аварийная_сигнализация = true;
									break;
								case 6:
									трамвай.указатель_поворота = -1;
									трамвай.аварийная_сигнализация = false;
									break;
								}
							}
						}
					}
					if (!(игрок.управляемыйОбъект is Троллейбус))
					{
						continue;
					}
					Троллейбус троллейбус = (Троллейбус)игрок.управляемыйОбъект;
					if (num2 == -1)
					{
						if (!троллейбус.управление.ручное)
						{
							continue;
						}
						if (троллейбус.штанги.Length == 2 && key_State[Key.T])
						{
							if (троллейбус.штанги[0].Опущена && троллейбус.штанги[1].Опущена)
							{
								троллейбус.штанги[0].НайтиПровод(мир.контактныеПровода);
								if (троллейбус.штанги[0].Провод != null)
								{
									троллейбус.штанги[0].поднимается = true;
								}
								троллейбус.штанги[1].НайтиПровод(мир.контактныеПровода);
								if (троллейбус.штанги[1].Провод != null)
								{
									троллейбус.штанги[1].поднимается = true;
								}
							}
							else
							{
								троллейбус.штанги[0].поднимается = false;
								троллейбус.штанги[1].поднимается = false;
							}
						}
						if (троллейбус.система_управления is Система_управления.РКСУ_Троллейбус)
						{
							Система_управления.РКСУ_Троллейбус рКСУ_Троллейбус = (Система_управления.РКСУ_Троллейбус)троллейбус.система_управления;
							if (key_State[Key.Backspace] && троллейбус.скорость == 0.0 && рКСУ_Троллейбус.позиция_контроллера == 0)
							{
								рКСУ_Троллейбус.позиция_реверсора = -рКСУ_Троллейбус.позиция_реверсора;
							}
							if (key_State.IsDirtyPressed(Key.DownArrow))
							{
								if (рКСУ_Троллейбус.пневматический_тормоз > 0.0 && рКСУ_Троллейбус.пневматический_тормоз < 1.0)
								{
									рКСУ_Троллейбус.пневматический_тормоз += 0.05;
								}
							}
							else if (key_State.IsDirtyPressed(Key.UpArrow) && рКСУ_Троллейбус.пневматический_тормоз > 0.0)
							{
								рКСУ_Троллейбус.пневматический_тормоз -= 0.05;
								if (рКСУ_Троллейбус.пневматический_тормоз < 0.0)
								{
									рКСУ_Троллейбус.пневматический_тормоз = 0.0;
								}
							}
							if (key_State[Key.DownArrow])
							{
								if (рКСУ_Троллейбус.позиция_контроллера > рКСУ_Троллейбус.позиция_min)
								{
									рКСУ_Троллейбус.позиция_контроллера--;
								}
								else if (рКСУ_Троллейбус.пневматический_тормоз == 0.0)
								{
									рКСУ_Троллейбус.пневматический_тормоз = 0.05;
								}
							}
							else if (key_State[Key.UpArrow] && рКСУ_Троллейбус.позиция_контроллера < рКСУ_Троллейбус.позиция_max && рКСУ_Троллейбус.пневматический_тормоз == 0.0)
							{
								рКСУ_Троллейбус.позиция_контроллера++;
							}
							if (key_State[Key.O] && троллейбус.ах != null)
							{
								троллейбус.ах.включён = !троллейбус.ах.включён;
							}
						}
						if (троллейбус.система_управления is Система_управления.КП_Авто)
						{
							Система_управления.КП_Авто кП_Авто = (Система_управления.КП_Авто)троллейбус.система_управления;
							if (key_State[Key.Z] && кП_Авто.режим > 0 && ((кП_Авто.текущий_режим != "R" && кП_Авто.текущий_режим != "N") || (кП_Авто.положение_педалей == -1.0 && троллейбус.скорость == 0.0)))
							{
								кП_Авто.режим--;
							}
							if (key_State[Key.X] && кП_Авто.режим < кП_Авто.режимы.Length - 1 && ((кП_Авто.текущий_режим != "P" && кП_Авто.текущий_режим != "N") || (кП_Авто.положение_педалей == -1.0 && троллейбус.скорость == 0.0)))
							{
								кП_Авто.режим++;
							}
                            /*
							if (key_State[Key.DownArrow])
							{
								if (кП_Авто.положение_педалей == 0.0)
								{
									//кП_Авто.положение_педалей = (0.0 - World.прошлоВремени) * 5.0 / 3.0;
								}
							}
							else if (key_State[Key.UpArrow] && кП_Авто.положение_педалей == 0.0)
							{
								//кП_Авто.положение_педалей = World.прошлоВремени * 5.0 / 3.0;
							}
                            */
							if (key_State.IsDirtyPressed(Key.DownArrow))
							{
								if (кП_Авто.положение_педалей <= 0.0)
								{
									кП_Авто.положение_педалей -= World.прошлоВремени * 5.0 / 3.0;
									if (кП_Авто.положение_педалей < -1.0)
									{
										кП_Авто.положение_педалей = -1.0;
									}
								}
								if (кП_Авто.положение_педалей > 0.0)
								{
									кП_Авто.положение_педалей -= World.прошлоВремени * 5.0 / 3.0;
									if (кП_Авто.положение_педалей < 0.0)
									{
										кП_Авто.положение_педалей = 0.0;
										key_State.keyticks[208] = 1;
									}
								}
							}
							else if (key_State.IsDirtyPressed(Key.UpArrow))
							{
								if (кП_Авто.положение_педалей >= 0.0)
								{
									кП_Авто.положение_педалей += World.прошлоВремени * 5.0 / 3.0;
									if (кП_Авто.положение_педалей > 1.0)
									{
										кП_Авто.положение_педалей = 1.0;
									}
								}
								if (кП_Авто.положение_педалей < 0.0)
								{
									кП_Авто.положение_педалей += World.прошлоВремени * 5.0 / 3.0;
									if (кП_Авто.положение_педалей > 0.0)
									{
										кП_Авто.положение_педалей = 0.0;
										key_State.keyticks[200] = 1;
									}
								}
							}
						}
						if (!fmouse)
						{
							if (key_State.IsDirtyPressed(Key.LeftArrow))
							{
								троллейбус.поворотРуля -= 0.3 * World.прошлоВремени;
							}
							if (key_State.IsDirtyPressed(Key.RightArrow))
							{
								троллейбус.поворотРуля += 0.3 * World.прошлоВремени;
							}
						}
						else if (!buttons[0])
						{
							троллейбус.поворотРуля += (double)x * 0.001;
						}
						if (key_State[Key.Q])
						{
							if (троллейбус.указатель_поворота >= 0)
							{
								троллейбус.указатель_поворота = -1;
								троллейбус.аварийная_сигнализация = false;
							}
							else
							{
								троллейбус.указатель_поворота = 0;
							}
						}
						if (key_State[Key.W])
						{
							if (троллейбус.указатель_поворота <= 0)
							{
								троллейбус.указатель_поворота = 1;
								троллейбус.аварийная_сигнализация = false;
							}
							else
							{
								троллейбус.указатель_поворота = 0;
							}
						}
						if (key_State[Key.E])
						{
							if (троллейбус.аварийная_сигнализация)
							{
								троллейбус.аварийная_сигнализация = false;
								continue;
							}
							троллейбус.аварийная_сигнализация = true;
							троллейбус.указатель_поворота = 0;
						}
						continue;
					}
					FilteredJoystickState filteredJoystickState3 = joystick_FilteredStates[num2];
					JoystickState joystickState3 = joystick_States[num2];
					int num7 = joystickState3.GetPointOfViewControllers()[0];
					if (num7 >= 0)
					{
						num7 = (int)Math.Round((double)num7 * 1.0 / 4500.0);
					}
					if (filteredJoystickState3[5])
					{
						троллейбус.управление.автоматическое = !троллейбус.управление.автоматическое;
						троллейбус.управление.ручное = !троллейбус.управление.ручное;
					}
					if (filteredJoystickState3[3])
					{
						троллейбус.включены_фары = !троллейбус.включены_фары;
					}
					if (!троллейбус.управление.ручное)
					{
						continue;
					}
					if (filteredJoystickState3[11])
					{
						троллейбус.включен = !троллейбус.включен;
					}
					if (filteredJoystickState3[2] && троллейбус.штанги.Length > 1)
					{
						if (троллейбус.штанги[0].Опущена && троллейбус.штанги[1].Опущена)
						{
							троллейбус.штанги[0].НайтиПровод(мир.контактныеПровода);
							if (троллейбус.штанги[0].Провод != null)
							{
								троллейбус.штанги[0].поднимается = true;
							}
							троллейбус.штанги[1].НайтиПровод(мир.контактныеПровода);
							if (троллейбус.штанги[1].Провод != null)
							{
								троллейбус.штанги[1].поднимается = true;
							}
						}
						else
						{
							троллейбус.штанги[0].поднимается = false;
							троллейбус.штанги[1].поднимается = false;
						}
					}
					if (filteredJoystickState3[0])
					{
						if (!троллейбус.двери_водителя_закрыты)
						{
							троллейбус.ОткрытьДвериВодителя(открыть: false);
						}
						else if (!троллейбус.двери_водителя_открыты)
						{
							троллейбус.ОткрытьДвериВодителя(открыть: true);
						}
					}
					if (filteredJoystickState3[1])
					{
						if (!троллейбус.двери_закрыты)
						{
							троллейбус.ОткрытьДвери(открыть: false);
						}
						else if (!троллейбус.двери_открыты)
						{
							троллейбус.ОткрытьДвери(открыть: true);
						}
					}
					if (!filteredJoystickState3[4, false] && !filteredJoystickState3[6, false])
					{
						троллейбус.поворотРуля += 0.5 * World.прошлоВремени * (double)joystickState3.X / 1024.0;
					}
					double num8 = -1.0 * (double)joystickState3.RotationZ / 1024.0;
					if (троллейбус.система_управления is Система_управления.РКСУ_Троллейбус)
					{
						Система_управления.РКСУ_Троллейбус рКСУ_Троллейбус2 = (Система_управления.РКСУ_Троллейбус)троллейбус.система_управления;
						if (num8 >= -0.6)
						{
							рКСУ_Троллейбус2.позиция_контроллера = (int)(4.0 * num8);
							рКСУ_Троллейбус2.пневматический_тормоз = 0.0;
						}
						else
						{
							рКСУ_Троллейбус2.позиция_контроллера = -2;
							рКСУ_Троллейбус2.пневматический_тормоз = (0.0 - (num8 + 0.6)) / 0.4;
						}
						if (filteredJoystickState3[7] && троллейбус.скорость == 0.0 && рКСУ_Троллейбус2.позиция_контроллера == 0)
						{
							рКСУ_Троллейбус2.позиция_реверсора = -рКСУ_Троллейбус2.позиция_реверсора;
						}
					}
					if (троллейбус.система_управления is Система_управления.КП_Авто)
					{
						Система_управления.КП_Авто кП_Авто2 = (Система_управления.КП_Авто)троллейбус.система_управления;
						кП_Авто2.положение_педалей = num8;
						if (filteredJoystickState3[6] && кП_Авто2.режим > 0 && ((кП_Авто2.текущий_режим != "R" && кП_Авто2.текущий_режим != "N") || (кП_Авто2.положение_педалей == -1.0 && троллейбус.скорость == 0.0)))
						{
							кП_Авто2.режим--;
						}
						if (filteredJoystickState3[7] && кП_Авто2.режим < кП_Авто2.режимы.Length - 1 && ((кП_Авто2.текущий_режим != "P" && кП_Авто2.текущий_режим != "N") || (кП_Авто2.положение_педалей == -1.0 && троллейбус.скорость == 0.0)))
						{
							кП_Авто2.режим++;
						}
					}
					switch (num7)
					{
					case 0:
						троллейбус.указатель_поворота = 0;
						троллейбус.аварийная_сигнализация = false;
						break;
					case 2:
						троллейбус.указатель_поворота = 1;
						троллейбус.аварийная_сигнализация = false;
						break;
					case 4:
						троллейбус.указатель_поворота = 0;
						троллейбус.аварийная_сигнализация = true;
						break;
					case 6:
						троллейбус.указатель_поворота = -1;
						троллейбус.аварийная_сигнализация = false;
						break;
					}
				}
			}
			if (key_State[Key.F1])
			{
				MainForm.debug = !MainForm.debug;
			}
			if (key_State[Key.F10])
			{
				MyFeatures.MakeScreenshot(request: true);
			}
		}

		public void Render()
		{
			if (MyDirect3D.device == null)
			{
				return;
			}
			if (!MainForm.in_editor)
			{
				if (MyDirect3D._newDevice.IsDeviceLost)
				{
					return;
				}
				MyDirect3D._newDevice.BeginScene();
				MyDirect3D.ResetViewports(игроки.Length);
				MyDirect3D.SetViewport(-1);
				MyDirect3D.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 0, 1f, 0);
				if (!активна)
				{
					menu.Draw();
					MyDirect3D._newDevice.EndScene();
					return;
				}
			}
			for (int i = 0; i < игроки.Length; i++)
			{
				MyDirect3D.SetViewport(i);
				MyDirect3D.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 46335, 1f, 0);
				игроки[i].cameraPosition.Add(ref игроки[i].cameraPositionChange);
				игроки[i].cameraPositionChange.Divide(3.0);
				игроки[i].cameraRotation.Add(ref игроки[i].cameraRotationChange);
				игроки[i].cameraRotationChange.Divide(3.0);
				if (Math.Abs(игроки[i].cameraRotation.x) > Math.PI)
				{
					игроки[i].cameraRotation.x -= Math.PI * 2.0 * (double)Math.Sign(игроки[i].cameraRotation.x);
				}
				if (Math.Abs(игроки[i].cameraRotation.y) > Math.PI / 2.0)
				{
					игроки[i].cameraRotation.y = Math.PI / 2.0 * (double)Math.Sign(игроки[i].cameraRotation.y);
				}
				MyDirect3D.SetCameraPos(игроки[i].cameraPosition, игроки[i].cameraRotation);
				col = (int)Math.Floor(игроки[i].cameraPosition.x / (double)Ground.grid_size);
				row = (int)Math.Floor(игроки[i].cameraPosition.z / (double)Ground.grid_size);
				MyDirect3D.ComputeFrustum();
				мир.RenderMeshes2();
				мир.RenderMeshes();
				MeshObject.RenderList();
				MyDirect3D.Alpha = true;
				мир.RenderMeshesA();
				MeshObject.RenderListA();
				MyDirect3D.Alpha = false;
				if (игроки[i].управляемыйОбъект != null)
				{
					Transport transport = (Transport)игроки[i].управляемыйОбъект;
					string text = (transport.скорость * 3.6).ToString("###0.00");
					string text2 = "";
					text2 = ((!transport.управление.автоматическое) ? (transport.управление.ручное ? Localization.current_.ctrl_m : "-") : (transport.управление.ручное ? Localization.current_.ctrl_s : Localization.current_.ctrl_a));
					if (MainForm.debug)
					{
						string text3 = "\nCS: " + ((transport.currentStop != null) ? transport.currentStop.название : "") + "\nNS: " + ((transport.nextStop != null) ? transport.nextStop.название : "") + "\nSI: " + transport.stopIndex + "\n\nX: " + transport.Координаты3D.x.ToString("#0.0") + "\nY: " + transport.Координаты3D.y.ToString("#0.0") + "\nZ: " + transport.Координаты3D.z.ToString("#0.0") + "\nrY: " + (transport.direction * 180.0 / Math.PI).ToString("#0.0") + "\nrZ: " + (transport.НаправлениеY * 180.0 / Math.PI).ToString("#0.0");
						MyGUI.default_font.DrawString(null, text3, 420 + MyDirect3D.device.Viewport.X, 15 + MyDirect3D.device.Viewport.Y, Color.Black);
					}
					if (transport is Трамвай)
					{
						Трамвай трамвай = (Трамвай)transport;
						string text4 = "-";
						if (трамвай.система_управления is Система_управления.РКСУ_Трамвай)
						{
							Система_управления.РКСУ_Трамвай рКСУ_Трамвай = (Система_управления.РКСУ_Трамвай)трамвай.система_управления;
							switch (рКСУ_Трамвай.позиция_контроллера)
							{
							case -5:
								text4 = "ТР";
								break;
							case -4:
								text4 = "Т4";
								break;
							case -3:
								text4 = "Т3";
								break;
							case -2:
								text4 = "Т2";
								break;
							case -1:
								text4 = "Т1";
								break;
							case 0:
								text4 = "0";
								break;
							case 1:
								text4 = "М";
								break;
							case 2:
								text4 = "Х1";
								break;
							case 3:
								text4 = "Х2";
								break;
							case 4:
								text4 = "Х3";
								break;
							}
							string text5 = ((рКСУ_Трамвай.позиция_реверсора == 1) ? Localization.current_.forward : ((рКСУ_Трамвай.позиция_реверсора == -1) ? Localization.current_.back : "0"));
							text4 = text4 + "\n" + Localization.current_.reverse + ": " + text5;
						}
						text4 = text4 + "\n" + (трамвай.токоприёмник.поднят ? Localization.current_.tk_on : Localization.current_.tk_off) + "\n" + Localization.current_.parking_brake + " " + (трамвай.stand_brake ? Localization.current_.enable : Localization.current_.disable);
						string text6 = трамвай.маршрут.number;
						if (трамвай.в_парк)
						{
							text6 = text6 + " (" + Localization.current_.route_in_park + ")";
						}
						if (трамвай.наряд != null)
						{
							text6 = text6 + "\n" + Localization.current_.order + ": " + трамвай.наряд.маршрут.number + "/" + трамвай.наряд.номер;
							if (трамвай.рейс != null)
							{
								if (мир.time < трамвай.рейс.время_отправления)
								{
									text6 = text6 + "\n" + Localization.current_.departure_time + ": " + трамвай.рейс.str_время_отправления;
								}
								text6 = text6 + "\n" + Localization.current_.arrival_time + ": " + трамвай.рейс.str_время_прибытия;
								if (трамвай.рейс_index < трамвай.рейс.pathes.Length - 1 && трамвай.передняя_ось.текущий_рельс.следующие_рельсы.Length > 1 && (трамвай.рейс_index > 0 || трамвай.передняя_ось.текущий_рельс == трамвай.рейс.pathes[0]))
								{
									Road road = трамвай.рейс.pathes[трамвай.рейс_index + 1];
									string text7 = Localization.current_.nr_pryamo;
									if (road.кривая)
									{
										if (road.СтепеньПоворота0 > 0.0)
										{
											text7 = Localization.current_.nr_right;
										}
										else if (road.СтепеньПоворота0 < 0.0)
										{
											text7 = Localization.current_.nr_left;
										}
									}
									text6 = text6 + "\n" + Localization.current_.nr + ": " + text7;
								}
							}
						}
						MyGUI.default_font.DrawString(null, Localization.current_.tram_control + ": " + text2 + "\n" + Localization.current_.ctrl_pos + ": " + text4 + "\n" + Localization.current_.speed + ": " + text + " " + Localization.current_.speed_km + "\n" + Localization.current_.route + ": " + text6, 15 + MyDirect3D.device.Viewport.X, 15 + MyDirect3D.device.Viewport.Y, Color.Black);
					}
					if (transport is Троллейбус)
					{
						Троллейбус троллейбус = (Троллейбус)transport;
						string text8 = "-";
						string text9 = "неизвестно чем";
						if (троллейбус.система_управления is Система_управления.РКСУ_Троллейбус)
						{
							text9 = Localization.current_.trol_control;
							text8 = "\n" + Localization.current_.ctrl_pos + ": ";
							Система_управления.РКСУ_Троллейбус рКСУ_Троллейбус = (Система_управления.РКСУ_Троллейбус)троллейбус.система_управления;
							switch (рКСУ_Троллейбус.позиция_контроллера)
							{
							case -2:
								text8 += "Т2";
								break;
							case -1:
								text8 += "Т1";
								break;
							case 0:
								text8 += "0";
								break;
							case 1:
								text8 += "М";
								break;
							case 2:
								text8 += "Х1";
								break;
							case 3:
								text8 += "Х2";
								break;
							case 4:
								text8 += "Х3";
								break;
							}
							text8 = text8 + "\n" + Localization.current_.air_brake + ": " + (рКСУ_Троллейбус.пневматический_тормоз * 100.0).ToString("0") + "%";
							string text10 = ((рКСУ_Троллейбус.позиция_реверсора == 1) ? Localization.current_.forward : ((рКСУ_Троллейбус.позиция_реверсора == -1) ? Localization.current_.back : "0"));
							text8 = text8 + "\n" + Localization.current_.reverse + ": " + text10;
							text8 = text8 + "\n" + (троллейбус.штанги_подняты ? Localization.current_.st_on : Localization.current_.st_off);
							text8 = text8 + "\n" + Localization.current_.trol + " " + (троллейбус.включен ? Localization.current_.enable : Localization.current_.disable);
							if (троллейбус.ах != null)
							{
								text8 = text8 + "\n" + Localization.current_.ax + " " + (троллейбус.ах.включён ? Localization.current_.enable : Localization.current_.disable) + "\n" + Localization.current_.ax_power + ": " + (троллейбус.ах.текущая_ёмкость / троллейбус.ах.полная_ёмкость).ToString("##0%");
							}
						}
						else if (троллейбус.система_управления is Система_управления.КП_Авто)
						{
							text9 = Localization.current_.bus_control;
							Система_управления.КП_Авто кП_Авто = (Система_управления.КП_Авто)троллейбус.система_управления;
							text8 = "\n" + Localization.current_.gmod + ": " + кП_Авто.текущий_режим + "\n" + Localization.current_.cur_pos + ": " + кП_Авто.текущая_передача + "\n" + Localization.current_.pedal_pos + ": ";
							if (кП_Авто.положение_педалей > 0.0)
							{
								text8 = text8 + Localization.current_.gas + " ";
							}
							if (кП_Авто.положение_педалей < 0.0)
							{
								text8 = text8 + Localization.current_.brake + " ";
							}
							text8 = text8 + (Math.Abs(кП_Авто.положение_педалей) * 100.0).ToString("0") + "%\n" + Localization.current_.engine + " " + (троллейбус.включен ? Localization.current_.enable : Localization.current_.disable);
						}
						text8 = text8 + "\n" + Localization.current_.parking_brake + " " + (троллейбус.stand_brake ? Localization.current_.enable : Localization.current_.disable);
						text8 = ((троллейбус.поворотРуля > 0.0) ? (text8 + "\n" + Localization.current_.sterling + ": " + (троллейбус.поворотРуля * 180.0 / Math.PI).ToString("0") + "° " + Localization.current_.ster_r) : ((!(троллейбус.поворотРуля < 0.0)) ? (text8 + "\n" + Localization.current_.sterling + ": " + Localization.current_.nr_pryamo) : (text8 + "\n" + Localization.current_.sterling + ": " + ((0.0 - троллейбус.поворотРуля) * 180.0 / Math.PI).ToString("0") + "° " + Localization.current_.ster_l)));
						string text11 = троллейбус.маршрут.number;
						if (троллейбус.в_парк)
						{
							text11 = text11 + " (" + Localization.current_.route_in_park + ")";
						}
						if (троллейбус.наряд != null)
						{
							string text12 = text11;
							text11 = text12 + "\n" + Localization.current_.order + ": " + троллейбус.наряд.маршрут.number + "/" + троллейбус.наряд.номер;
							if (троллейбус.рейс != null)
							{
								if (мир.time < троллейбус.рейс.время_отправления)
								{
									text11 = text11 + "\n" + Localization.current_.departure_time + ": " + троллейбус.рейс.str_время_отправления;
								}
								text11 = text11 + "\n" + Localization.current_.arrival_time + ": " + троллейбус.рейс.str_время_прибытия;
								if (троллейбус.рейс_index < троллейбус.рейс.pathes.Length - 1 && троллейбус.положение.Дорога != null && троллейбус.положение.Дорога.следующиеДороги.Length > 1 && (троллейбус.рейс_index > 0 || троллейбус.положение.Дорога == троллейбус.рейс.pathes[0]))
								{
									Road road2 = троллейбус.рейс.pathes[троллейбус.рейс_index + 1];
									string text13 = Localization.current_.nr_pryamo;
									if (road2.кривая)
									{
										if (road2.СтепеньПоворота0 > 0.0)
										{
											text13 = Localization.current_.nr_right;
										}
										else if (road2.СтепеньПоворота0 < 0.0)
										{
											text13 = Localization.current_.nr_left;
										}
									}
									text11 = text11 + "\n" + Localization.current_.nr + ": " + text13;
								}
							}
						}
						MyGUI.default_font.DrawString(null, text9 + ": " + text2 + text8 + "\n" + Localization.current_.speed + ": " + text + " " + Localization.current_.speed_km + "\n" + Localization.current_.route + ": " + text11, 15 + MyDirect3D.device.Viewport.X, 15 + MyDirect3D.device.Viewport.Y, Color.Black);
					}
				}
				if (MyDirect3D.device.Viewport.X + MyDirect3D.device.Viewport.Width == MyDirect3D.Window_Width && MyDirect3D.device.Viewport.Y == 0)
				{
					MyGUI.default_font.DrawString(null, ConvertTime.TimeFromSeconds(мир.time % 86400.0), MyDirect3D.Window_Width - 105, 15, Color.Black);
				}
				if (MainForm.debug)
				{
					string text14 = "\ndTmax: " + World.dtmax.ToString("#0.000") + "\nFPS: " + MyDirect3D._newDevice.FPS.ToString("  00") + "\nX: " + MyDirect3D.Camera_Position.x.ToString("#0.0") + "\nY: " + MyDirect3D.Camera_Position.y.ToString("#0.0") + "\nZ: " + MyDirect3D.Camera_Position.z.ToString("#0.0") + "\nrY: " + MyDirect3D.Camera_Rotation.x.ToString("#0.000") + "\nrZ: " + MyDirect3D.Camera_Rotation.y.ToString("#0.000");
					MyGUI.default_font.DrawString(null, text14, new Rectangle(MyDirect3D.Window_Width - 160, 15, 160, 500), DrawTextFormat.Right, Color.Black);
				}
			}
			MyFeatures.MakeScreenshot(request: false);
			MyDirect3D._newDevice.EndScene();
		}

		public void RenderMain()
		{
			for (int i = 0; i < игроки.Length; i++)
			{
				игроки[i].cameraPosition.Add(ref игроки[i].cameraPositionChange);
				игроки[i].cameraPositionChange.Divide(3.0);
				игроки[i].cameraRotation.Add(ref игроки[i].cameraRotationChange);
				if (игроки[i].cameraRotation.x > Math.PI)
				{
					игроки[i].cameraRotation.x -= Math.PI * 2.0;
				}
				else if (игроки[i].cameraRotation.x < -Math.PI)
				{
					игроки[i].cameraRotation.x += Math.PI * 2.0;
				}
				игроки[i].cameraRotation.y = Math.Min(Math.Max(игроки[i].cameraRotation.y, -Math.PI / 2.0), Math.PI / 2.0);
				игроки[i].cameraRotationChange.Divide(3.0);
				игроки[i].excameraPosition = игроки[i].cameraPosition;
				игроки[i].excameraRotation = игроки[i].cameraRotation;
				MyDirect3D.SetCameraPos(игроки[i].cameraPosition, игроки[i].cameraRotation);
				col = (int)Math.Floor(игроки[i].cameraPosition.x / (double)Ground.grid_size);
				row = (int)Math.Floor(игроки[i].cameraPosition.z / (double)Ground.grid_size);
				MyDirect3D.ComputeFrustum();
				мир.RenderMeshes();
				string text = "";
				if (игроки[i].управляемыйОбъект != null)
				{
					Transport transport = (Transport)игроки[i].управляемыйОбъект;
					string text2 = (transport.скорость * 3.6).ToString("###0.00");
					string text3 = "";
					text3 = ((!transport.управление.автоматическое) ? (transport.управление.ручное ? Localization.current_.ctrl_m : "-") : (transport.управление.ручное ? Localization.current_.ctrl_s : Localization.current_.ctrl_a));
					if (MainForm.debug)
					{
						MyGUI.stringlist[4 + i] = "\nCS: " + ((transport.currentStop != null) ? transport.currentStop.название : "") + "\nNS: " + ((transport.nextStop != null) ? transport.nextStop.название : "") + "\nSI: " + transport.stopIndex + "\n\nX: " + transport.Координаты3D.x.ToString("#0.0") + "\nY: " + transport.Координаты3D.y.ToString("#0.0") + "\nZ: " + transport.Координаты3D.z.ToString("#0.0") + "\nrY: " + (transport.direction * 180.0 / Math.PI).ToString("#0.0") + "\nrZ: " + (transport.НаправлениеY * 180.0 / Math.PI).ToString("#0.0");
					}
					if (transport is Трамвай)
					{
						Трамвай трамвай = (Трамвай)transport;
						string text4 = "-";
						if (трамвай.система_управления is Система_управления.РКСУ_Трамвай)
						{
							Система_управления.РКСУ_Трамвай рКСУ_Трамвай = (Система_управления.РКСУ_Трамвай)трамвай.система_управления;
							switch (рКСУ_Трамвай.позиция_контроллера)
							{
							case -5:
								text4 = "ТР";
								break;
							case -4:
								text4 = "Т4";
								break;
							case -3:
								text4 = "Т3";
								break;
							case -2:
								text4 = "Т2";
								break;
							case -1:
								text4 = "Т1";
								break;
							case 0:
								text4 = "0";
								break;
							case 1:
								text4 = "М";
								break;
							case 2:
								text4 = "Х1";
								break;
							case 3:
								text4 = "Х2";
								break;
							case 4:
								text4 = "Х3";
								break;
							}
							string text5 = ((рКСУ_Трамвай.позиция_реверсора == 1) ? Localization.current_.forward : ((рКСУ_Трамвай.позиция_реверсора == -1) ? Localization.current_.back : "0"));
							text4 = text4 + "\n" + Localization.current_.reverse + ": " + text5;
						}
						text4 = text4 + "\n" + (трамвай.токоприёмник.поднят ? Localization.current_.tk_on : Localization.current_.tk_off) + "\n" + Localization.current_.parking_brake + " " + (трамвай.stand_brake ? Localization.current_.enable : Localization.current_.disable);
						string text6 = трамвай.маршрут.number;
						if (трамвай.в_парк)
						{
							text6 = text6 + " (" + Localization.current_.route_in_park + ")";
						}
						if (трамвай.наряд != null)
						{
							text6 = text6 + "\n" + Localization.current_.order + ": " + трамвай.наряд.маршрут.number + "/" + трамвай.наряд.номер;
							if (трамвай.рейс != null)
							{
								if (мир.time < трамвай.рейс.время_отправления)
								{
									text6 = text6 + "\n" + Localization.current_.departure_time + ": " + трамвай.рейс.str_время_отправления;
								}
								text6 = text6 + "\n" + Localization.current_.arrival_time + ": " + трамвай.рейс.str_время_прибытия;
								if (трамвай.рейс_index < трамвай.рейс.pathes.Length - 1 && трамвай.передняя_ось.текущий_рельс.следующие_рельсы.Length > 1 && (трамвай.рейс_index > 0 || трамвай.передняя_ось.текущий_рельс == трамвай.рейс.pathes[0]))
								{
									Road road = трамвай.рейс.pathes[трамвай.рейс_index + 1];
									string text7 = Localization.current_.nr_pryamo;
									if (road.кривая)
									{
										if (road.СтепеньПоворота0 > 0.0)
										{
											text7 = Localization.current_.nr_right;
										}
										else if (road.СтепеньПоворота0 < 0.0)
										{
											text7 = Localization.current_.nr_left;
										}
									}
									text6 = text6 + "\n" + Localization.current_.nr + ": " + text7;
								}
							}
						}
						text = Localization.current_.tram_control + ": " + text3 + "\n" + Localization.current_.ctrl_pos + ": " + text4 + "\n" + Localization.current_.speed + ": " + text2 + " " + Localization.current_.speed_km + "\n" + Localization.current_.route + ": " + text6;
					}
					if (transport is Троллейбус)
					{
						Троллейбус троллейбус = (Троллейбус)transport;
						string text8 = "-";
						string text9 = "неизвестно чем";
						if (троллейбус.система_управления is Система_управления.РКСУ_Троллейбус)
						{
							text9 = Localization.current_.trol_control;
							text8 = "\n" + Localization.current_.ctrl_pos + ": ";
							Система_управления.РКСУ_Троллейбус рКСУ_Троллейбус = (Система_управления.РКСУ_Троллейбус)троллейбус.система_управления;
							switch (рКСУ_Троллейбус.позиция_контроллера)
							{
							case -2:
								text8 += "Т2";
								break;
							case -1:
								text8 += "Т1";
								break;
							case 0:
								text8 += "0";
								break;
							case 1:
								text8 += "М";
								break;
							case 2:
								text8 += "Х1";
								break;
							case 3:
								text8 += "Х2";
								break;
							case 4:
								text8 += "Х3";
								break;
							}
							text8 = text8 + "\n" + Localization.current_.air_brake + ": " + (рКСУ_Троллейбус.пневматический_тормоз * 100.0).ToString("0") + "%";
							string text10 = ((рКСУ_Троллейбус.позиция_реверсора == 1) ? Localization.current_.forward : ((рКСУ_Троллейбус.позиция_реверсора == -1) ? Localization.current_.back : "0"));
							text8 = text8 + "\n" + Localization.current_.reverse + ": " + text10;
							text8 = text8 + "\n" + (троллейбус.штанги_подняты ? Localization.current_.st_on : Localization.current_.st_off);
							text8 = text8 + "\n" + Localization.current_.trol + " " + (троллейбус.включен ? Localization.current_.enable : Localization.current_.disable);
							if (троллейбус.ах != null)
							{
								text8 = text8 + "\n" + Localization.current_.ax + " " + (троллейбус.ах.включён ? Localization.current_.enable : Localization.current_.disable) + "\n" + Localization.current_.ax_power + ": " + (троллейбус.ах.текущая_ёмкость / троллейбус.ах.полная_ёмкость).ToString("##0%");
							}
						}
						else if (троллейбус.система_управления is Система_управления.КП_Авто)
						{
							text9 = Localization.current_.bus_control;
							Система_управления.КП_Авто кП_Авто = (Система_управления.КП_Авто)троллейбус.система_управления;
							text8 = "\n" + Localization.current_.gmod + ": " + кП_Авто.текущий_режим + "\n" + Localization.current_.cur_pos + ": " + кП_Авто.текущая_передача + "\n" + Localization.current_.pedal_pos + ": ";
							if (кП_Авто.положение_педалей > 0.0)
							{
								text8 = text8 + Localization.current_.gas + " ";
							}
							if (кП_Авто.положение_педалей < 0.0)
							{
								text8 = text8 + Localization.current_.brake + " ";
							}
							text8 = text8 + (Math.Abs(кП_Авто.положение_педалей) * 100.0).ToString("0") + "%\n" + Localization.current_.engine + " " + (троллейбус.включен ? Localization.current_.enable : Localization.current_.disable);
						}
						text8 = text8 + "\n" + Localization.current_.parking_brake + " " + (троллейбус.stand_brake ? Localization.current_.enable : Localization.current_.disable);
						text8 = ((троллейбус.поворотРуля > 0.0) ? (text8 + "\n" + Localization.current_.sterling + ": " + (троллейбус.поворотРуля * 180.0 / Math.PI).ToString("0") + "° " + Localization.current_.ster_r) : ((!(троллейбус.поворотРуля < 0.0)) ? (text8 + "\n" + Localization.current_.sterling + ": " + Localization.current_.nr_pryamo) : (text8 + "\n" + Localization.current_.sterling + ": " + ((0.0 - троллейбус.поворотРуля) * 180.0 / Math.PI).ToString("0") + "° " + Localization.current_.ster_l)));
						string text11 = троллейбус.маршрут.number;
						if (троллейбус.в_парк)
						{
							text11 = text11 + " (" + Localization.current_.route_in_park + ")";
						}
						if (троллейбус.наряд != null)
						{
							string text12 = text11;
							text11 = text12 + "\n" + Localization.current_.order + ": " + троллейбус.наряд.маршрут.number + "/" + троллейбус.наряд.номер;
							if (троллейбус.рейс != null)
							{
								if (мир.time < троллейбус.рейс.время_отправления)
								{
									text11 = text11 + "\n" + Localization.current_.departure_time + ": " + троллейбус.рейс.str_время_отправления;
								}
								text11 = text11 + "\n" + Localization.current_.arrival_time + ": " + троллейбус.рейс.str_время_прибытия;
								if (троллейбус.рейс_index < троллейбус.рейс.pathes.Length - 1 && троллейбус.положение.Дорога != null && троллейбус.положение.Дорога.следующиеДороги.Length > 1 && (троллейбус.рейс_index > 0 || троллейбус.положение.Дорога == троллейбус.рейс.pathes[0]))
								{
									Road road2 = троллейбус.рейс.pathes[троллейбус.рейс_index + 1];
									string text13 = Localization.current_.nr_pryamo;
									if (road2.кривая)
									{
										if (road2.СтепеньПоворота0 > 0.0)
										{
											text13 = Localization.current_.nr_right;
										}
										else if (road2.СтепеньПоворота0 < 0.0)
										{
											text13 = Localization.current_.nr_left;
										}
									}
									text11 = text11 + "\n" + Localization.current_.nr + ": " + text13;
								}
							}
						}
						text = text9 + ": " + text3 + text8 + "\n" + Localization.current_.speed + ": " + text2 + " " + Localization.current_.speed_km + "\n" + Localization.current_.route + ": " + text11;
					}
				}
				MyGUI.stringlist[i] = text;
				MyGUI.stringlist[i + 8] = "\ndTmax: " + World.dtmax.ToString("#0.000") + "\nFPS: " + MyDirect3D._newDevice.FPS.ToString("  00") + "\nX: " + MyDirect3D.Camera_Position.x.ToString("#0.0") + "\nY: " + MyDirect3D.Camera_Position.y.ToString("#0.0") + "\nZ: " + MyDirect3D.Camera_Position.z.ToString("#0.0") + "\nrY: " + MyDirect3D.Camera_Rotation.x.ToString("#0.000") + "\nrZ: " + MyDirect3D.Camera_Rotation.y.ToString("#0.000");
			}
			MyGUI.stringlist[12] = ConvertTime.TimeFromSeconds(мир.time % 86400.0);
		}

		public void RenderThread()
		{
			if (MyDirect3D.device == null)
			{
				return;
			}
			if (!MainForm.in_editor)
			{
				MyDirect3D.device.BeginScene();
				MyDirect3D.ResetViewports(игроки.Length);
				MyDirect3D.SetViewport(-1);
				MyDirect3D.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 0, 1f, 0);
				if (!активна)
				{
					menu.Draw();
					MyDirect3D.device.EndScene();
					MyDirect3D.device.Present();
					return;
				}
			}
			for (int i = 0; i < игроки.Length; i++)
			{
				MyDirect3D.SetViewport(i);
				MyDirect3D.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 46335, 1f, 0);
				MyDirect3D.SetCameraPos(игроки[i].excameraPosition, игроки[i].excameraRotation);
				мир.RenderMeshes2();
				MeshObject.RenderList();
				MyDirect3D.Alpha = true;
				мир.RenderMeshesA();
				MeshObject.RenderListA();
				MyDirect3D.Alpha = false;
				if (!string.IsNullOrEmpty(MyGUI.stringlist[i]))
				{
					MyGUI.default_font.DrawString(null, MyGUI.stringlist[i], 15 + MyDirect3D.device.Viewport.X, 15 + MyDirect3D.device.Viewport.Y, Color.Black);
				}
				if (MyDirect3D.device.Viewport.X + MyDirect3D.device.Viewport.Width == MyDirect3D.Window_Width && MyDirect3D.device.Viewport.Y == 0 && !string.IsNullOrEmpty(MyGUI.stringlist[12]))
				{
					MyGUI.default_font.DrawString(null, MyGUI.stringlist[12], MyDirect3D.Window_Width - 105, 15, Color.Black);
				}
				if (MainForm.debug)
				{
					if (!string.IsNullOrEmpty(MyGUI.stringlist[i + 8]))
					{
						MyGUI.default_font.DrawString(null, MyGUI.stringlist[i + 8], new Rectangle(MyDirect3D.Window_Width - 160, 15, 160, 500), DrawTextFormat.Right, Color.Black);
					}
					if (!string.IsNullOrEmpty(MyGUI.stringlist[i + 4]))
					{
						MyGUI.default_font.DrawString(null, MyGUI.stringlist[i + 4], 420 + MyDirect3D.device.Viewport.X, 15 + MyDirect3D.device.Viewport.Y, Color.Black);
						MyGUI.stringlist[i + 4] = string.Empty;
					}
				}
			}
			MyFeatures.MakeScreenshot(request: false);
			MyDirect3D.device.EndScene();
			MyDirect3D.device.Present();
		}

		private void Привязывать(Игрок игрок)
		{
			if (игрок.объектПривязки != null)
			{
				игрок.объектПривязки = null;
				return;
			}
			double num = 200.0;
			IVector объектПривязки = null;
			IControlledObject controlledObject = null;
			foreach (Transport item in мир.транспорты)
			{
				double modulus = (item.position - игрок.cameraPosition.XZPoint).Modulus;
				if (modulus < num)
				{
					num = modulus;
					объектПривязки = item;
					controlledObject = item;
				}
			}
			игрок.объектПривязки = объектПривязки;
			if (игрок.управляемыйОбъект != null && игрок.управляемыйОбъект != controlledObject)
			{
				игрок.управляемыйОбъект.управление = Управление.Автоматическое;
			}
			игрок.управляемыйОбъект = controlledObject;
		}

		public void Сохранить(string filename)
		{
		}
	}
}

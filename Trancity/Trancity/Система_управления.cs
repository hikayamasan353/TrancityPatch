using System;
using Common;
using Engine;
using Engine.Sound;

namespace Trancity
{
	public abstract class Система_управления
	{
		public abstract class Автобусная : Система_управления
		{
		}

		public class КП_Авто : Автобусная
		{
			private int fрежим;

			private Троллейбус автобус;

			public int передача;

			public double положение_педалей = -1.0;

			public string[] режимы = new string[6] { "P", "R", "N", "D", "2", "1" };

			public override int направление => Math.Sign(передача);

			public override bool переключение => false;

			public int режим
			{
				get
				{
					return fрежим;
				}
				set
				{
					fрежим = value;
					string text = текущий_режим;
					if (text == null)
					{
						return;
					}
					if (text != "P" && text != "N")
					{
						if (text != "R")
						{
							if (text != "D")
							{
								if (text != "2")
								{
									if (text == "1")
									{
										передача = 1;
									}
								}
								else
								{
									передача = 2;
								}
							}
							else if (передача <= 0)
							{
								передача = 1;
							}
						}
						else
						{
							передача = -1;
						}
					}
					else
					{
						передача = 0;
					}
				}
			}

			public string текущая_передача
			{
				get
				{
					if (передача > 0)
					{
						return передача.ToString();
					}
					if (передача == 0)
					{
						return "-";
					}
					return "R";
				}
			}

			public string текущий_режим => режимы[fрежим];

			public override double ускорение
			{
				get
				{
					if (режим == 0 || автобус.stand_brake)
					{
						return -2 * Math.Sign(автобус.скорость);
					}
					double num = 0.0;
					double num2 = 0.0;
					double num3 = автобус.скорость * (double)направление;
					if (автобус.включен && режим != 2)
					{
						if (num3 < 2.0)
						{
							num = 1.0;
							if (num3 > 0.4)
							{
								num *= Math.Pow(0.4 / num3, 2.0);
							}
						}
						if (положение_педалей > 0.0)
						{
							double num4 = Math.Abs(передача);
							num = 2.0 * положение_педалей / num4;
							double num5 = (4.3 * num4 - автобус.скорость_abs) / World.прошлоВремени;
							if (num > num5)
							{
								num = num5;
								if (текущий_режим == "D" && передача < 5)
								{
									передача++;
								}
								else if (текущий_режим == "2" && передача < 2)
								{
									передача++;
								}
							}
							if (передача > 1 && автобус.скорость_abs < 4.0 * (double)(передача - 1))
							{
								передача--;
							}
						}
					}
					if (положение_педалей < 0.0)
					{
						num2 += (0.0 - положение_педалей) * 1.8;
						if (передача > 1 && автобус.скорость_abs < 4.0 * (double)(передача - 1))
						{
							передача--;
						}
					}
					if (Math.Abs(автобус.скорость) < 0.1)
					{
						num -= num2;
						if (num < 0.0)
						{
							num = 0.0;
						}
					}
					try
					{
						return num * (double)направление - num2 * (double)Math.Sign(автобус.скорость);
					}
					catch
					{
						return 0.0;
					}
				}
			}

			public override int ход_или_тормоз
			{
				get
				{
					if (режим == 0 || автобус.stand_brake)
					{
						return -1;
					}
					if (положение_педалей < 0.0 && Math.Abs(автобус.скорость) < 2.0)
					{
						return 0;
					}
					return Math.Sign(положение_педалей);
				}
			}

			public КП_Авто(Троллейбус автобус)
			{
				this.автобус = автобус;
			}

			public override void CreateSoundBuffers()
			{
				SoundBuffers = new ISound3D[5];
				for (int i = 0; i < SoundBuffers.Length; i++)
				{
					SoundBuffers[i] = MyXAudio2.Device.CreateEmitter(60f, автобус.основная_папка + "engine.wav");
					SoundBuffers[i].Volume = 1f;
				}
				SoundBuffers[3].Volume = 0.5f;
				SoundBuffers[4].Volume = 0.5f;
			}

			public override void UpdateSound(Игрок[] игроки, bool игра_активна)
			{
				if (PreUpdateSound(автобус, игра_активна))
				{
					int num = Math.Max(1, Math.Abs(передача));
					double num2 = ((ход_или_тормоз > 0) ? (((double)num + 0.5) / 5.0) : 1.0);
					double num3 = 4.3 - (4.3 - автобус.скорость_abs / (double)num) * num2;
					double num4 = 400.0 * автобус.скорость_abs + 1000.0;
					double num5 = 400.0 * num3 + 1000.0;
					if (автобус.скорость_abs < 2.0)
					{
						num4 = Math.Max(1800.0 * автобус.скорость_abs / 2.0, 1.0);
					}
					if (num3 < 2.0)
					{
						num5 = ((положение_педалей > 0.0 || автобус.скорость_abs >= 2.0) ? (90.0 * num3) : 0.0);
					}
					if (передача == 0)
					{
						num5 = 0.0;
					}
					SoundBuffers[0].Frequency = (float)Math.Max(num4 * 2.8, 100.0) / 10000f;
					SoundBuffers[1].Frequency = (float)Math.Max(num4 * 4.2, 100.0) / 10000f;
					SoundBuffers[2].Frequency = (float)((автобус._soundУскоряется || автобус._soundЗамедляется) ? Math.Max(num5 * 6.0, 8000.0) : 8000.0) / 10000f;
					SoundBuffers[3].Frequency = (float)Math.Max(num5 * 10.5, 100.0) / 10000f;
					SoundBuffers[4].Frequency = (float)Math.Max(num5 * 15.0, 100.0) / 10000f;
				}
			}

			public override void автоматически_управлять(double рекомендуемая_скорость, double оставшееся_расстояние, int переключение)
			{
				double num = World.прошлоВремени * 5.0 / 3.0;
				if (режим == 0 || режим == 2)
				{
					if (положение_педалей != -1.0 || автобус.скорость != 0.0)
					{
						if (положение_педалей > -1.0 + num)
						{
							положение_педалей -= num;
						}
						else
						{
							положение_педалей = -1.0;
						}
						return;
					}
					режим = ((!(рекомендуемая_скорость >= 0.0)) ? 1 : 3);
				}
				if (режим > 3)
				{
					режим = 3;
				}
				if (рекомендуемая_скорость * (double)направление < 0.0)
				{
					if (положение_педалей != -1.0 || автобус.скорость != 0.0)
					{
						if (положение_педалей > -1.0 + num)
						{
							положение_педалей -= num;
						}
						else
						{
							положение_педалей = -1.0;
						}
						return;
					}
					режим = ((направление >= 0) ? 1 : 3);
				}
				рекомендуемая_скорость *= (double)направление;
				double num2 = автобус.скорость * (double)направление;
				double num3 = рекомендуемая_скорость - num2;
				if (num2 < 0.0)
				{
					num3 = 0.0 - num3;
				}
				if (рекомендуемая_скорость <= 0.0)
				{
					num3 -= 2.0;
				}
				double num4 = 0.0;
				if (Math.Abs(num3) < 0.5 && num2 > 0.5)
				{
					num4 = 0.0;
					if (num3 > 0.1 && num2 < 4.0)
					{
						num4 = 0.2;
					}
				}
				else if (num2 == 0.0 && рекомендуемая_скорость <= 0.0)
				{
					num4 = -1.0;
				}
				else if (num3 > 0.0)
				{
					if (num2 > 10.0 && положение_педалей == 0.0)
					{
						num4 = 0.0;
					}
					else if (num2 >= 0.0 && num2 < 2.3)
					{
						num4 = 0.4;
					}
					else if (num2 >= 2.3 && num2 < 5.0)
					{
						num4 = 0.6;
					}
					else if (num2 >= 5.0 && num2 < 10.0)
					{
						num4 = 0.8;
					}
					else if (num2 >= 10.0)
					{
						num4 = 1.0;
					}
				}
				else if (num3 < 0.0)
				{
					num4 = (num3 - 3.0) * ((num2 + рекомендуемая_скорость) / 2.0) / Math.Max(оставшееся_расстояние - 5.0, 0.6) / 1.8;
					if (num4 > 0.0)
					{
						num4 = 0.0;
					}
					if (num4 < -1.0 || num2 < 0.3)
					{
						num4 = -1.0;
					}
				}
				if (положение_педалей > num4 + num)
				{
					положение_педалей -= num;
				}
				else if (положение_педалей < num4 - num)
				{
					положение_педалей += num;
				}
				else
				{
					положение_педалей = num4;
				}
			}
		}

		public class РКСУ_Трамвай : Система_управления
		{
			public readonly int позиция_max = 4;

			public readonly int позиция_min = -5;

			public int позиция_контроллера;

			public int позиция_реверсора = 1;

			private readonly Трамвай трамвай;

			public override int ход_или_тормоз
			{
				get
				{
					if (трамвай.stand_brake)
					{
						return -1;
					}
					return Math.Sign(позиция_контроллера);
				}
			}

			public override double ускорение
			{
				get
				{
					double num = 0.0;
					double num2 = трамвай.скорость * (double)позиция_реверсора;
					switch (позиция_контроллера)
					{
					case -5:
						num = -2.1;
						break;
					case -4:
						num = -2.0;
						break;
					case -3:
						num = -1.5;
						break;
					case -2:
						num = -1.1;
						break;
					case -1:
						num = -0.7;
						break;
					case 1:
						num = 0.8;
						if (num2 > 2.0)
						{
							num *= Math.Pow(2.0 / num2, 4.0);
						}
						break;
					case 2:
						num = 1.1;
						if (num2 > 8.0)
						{
							num *= Math.Pow(8.0 / num2, 4.0);
						}
						break;
					case 3:
						num = 1.3;
						if (num2 > 8.0)
						{
							num *= Math.Pow(8.0 / num2, 4.0);
						}
						break;
					case 4:
						num = 1.5;
						if (num2 > 6.0)
						{
							num *= 6.0 / num2;
						}
						if (num2 > 15.0)
						{
							num *= Math.Pow(15.0 / num2, 4.0);
						}
						break;
					}
					if (трамвай.stand_brake)
					{
						num -= 2.1;
					}
					if (позиция_контроллера < 0 || трамвай.stand_brake)
					{
						return num * (double)Math.Sign(трамвай.скорость);
					}
					return num * (double)позиция_реверсора;
				}
			}

			public override int направление => позиция_реверсора;

			public override bool переключение
			{
				get
				{
					if (трамвай.токоприёмник.поднят && трамвай.передняя_ось.текущий_рельс.следующие_рельсы.Length > 1)
					{
						return (трамвай.токоприёмник.position.XZPoint - трамвай.передняя_ось.текущий_рельс.добавочные_провода.координаты).Modulus < 0.5;
					}
					return false;
				}
			}

			public РКСУ_Трамвай(Трамвай трамвай)
			{
				this.трамвай = трамвай;
			}

			public override void CreateSoundBuffers()
			{
				SoundBuffers = new ISound3D[3];
                /*
                 * Старый код rew
				for (int i = 0; i < SoundBuffers.Length; i++)
				{
					SoundBuffers[i] = MyXAudio2.Device.CreateEmitter(60f, трамвай.основная_папка + "Sound 1.wav");
					SoundBuffers[i].Volume = 1f;
				}
                */

                SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(60f, трамвай.основная_папка + "Sound 1.wav");
                SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(60f, трамвай.основная_папка + "Sound 1.wav");
                SoundBuffers[2] = MyXAudio2.Device.CreateEmitter(40f, трамвай.основная_папка + "Sound 1.wav"); //Звук ТЭДа

                for (int i = 0; i < SoundBuffers.Length; i++)
                {
                    SoundBuffers[i].Volume = 1f;
                }


            }

			public override void UpdateSound(Игрок[] игроки, bool игра_активна)
			{

                /*
				if (PreUpdateSound(трамвай, игра_активна))
				{
					SoundBuffers[0].Frequency = ((трамвай.скорость_abs > 1.0) ? ((float)(3000.0 * трамвай.скорость_abs) + 1000f) : 0f) / 10000f;
					SoundBuffers[1].Frequency = SoundBuffers[0].Frequency * 2f;
					SoundBuffers[2].Frequency = SoundBuffers[0].Frequency * 25f / 16f;
					SoundBuffers[0].Volume = ((трамвай._soundУскоряется || трамвай._soundЗамедляется) ? 0.8f : 0.2f);
					SoundBuffers[1].Volume = SoundBuffers[0].Volume;
					SoundBuffers[2].Volume = ((трамвай.скорость_abs < 6.0) ? (((SoundBuffers[0].Volume * 10000f - 10000f + 2000f) * (float)трамвай.скорость_abs / 6f + 8000f) / 10000f) : SoundBuffers[0].Volume);
				}
                */
                if (!PreUpdateSound(трамвай, игра_активна))
                    return;

                /////////
                //// Этот код в системе управления троллейбусом создает фактор регулировки частоты звука
                //// В трамвае он бы пригодился не меньше для более гладкого звука
                float num = 600f * (float)трамвай.скорость_abs + 1000f;
                if (трамвай.скорость_abs < 1.0)
                {
                    num = Math.Max(1600f * (float)трамвай.скорость_abs, 1f);
                }
                num /= 20000f;

                //Если на проводе трамвая есть ток
                if(!трамвай.токоприёмник.Провод.обесточенный)
                {
                    //Регулируем частоту звуков ТЭД
                    //SoundBuffers[0].Frequency = num * 2f;
                    SoundBuffers[0].Frequency = num * ((трамвай.скорость_abs > 1.0) ? ((float)(1000.0 * трамвай.скорость_abs) + 500f) : 0f) / 10000f;
                    SoundBuffers[1].Frequency = num * 1.3f;
                    SoundBuffers[2].Frequency = num * 1.5f;
                }
                else
                {
                    for (int i = 0; i < SoundBuffers.Length - 1; i++)
                    {
                        //Глушим частоту звуков ТЭД
                        SoundBuffers[i].Frequency = 0.01f;
                    }
                }
                //Отключаем звук ТЭД если стоим
                if (трамвай.скорость_abs < 0.1)
                {
                    SoundBuffers[2].Volume = 0f;
                }


                //SoundBuffers[0].Volume = ((трамвай._soundУскоряется || трамвай._soundЗамедляется) ? 0.8f : 0.2f);
                else if (трамвай._soundУскоряется || трамвай._soundЗамедляется)
                {
                    SoundBuffers[2].Volume = Math.Min(SoundBuffers[2].Volume + 0.2f, 1f);
                }
                else
                {
                    SoundBuffers[2].Volume = Math.Max(SoundBuffers[2].Volume - 0.2f, 0f);
                }
                SoundBuffers[0].Volume = 1f;
                SoundBuffers[1].Volume = 1f;


            }

			public override void автоматически_управлять(double рекомендуемаяСкорость, double оставшеесяРасстояние, int переключение)
			{
				double num = трамвай.скорость * (double)позиция_реверсора;
				double num2 = рекомендуемаяСкорость - num;
				if (num + num2 < 0.0)
				{
					num2 -= num + num2;
				}
				if (this.переключение && трамвай.скорость > 0.0)
				{
					if (переключение == 0 != Рельс.стрелки_наоборот)
					{
						if (позиция_контроллера > 0)
						{
							позиция_контроллера = 0;
						}
						else if (позиция_контроллера <= -5)
						{
							позиция_контроллера = -4;
						}
					}
					else if (позиция_контроллера <= 0 && позиция_контроллера > -5)
					{
						позиция_контроллера = 1;
					}
				}
				else if (Math.Abs(num2) < 0.5 && трамвай.скорость_abs > 0.5)
				{
					if (num2 > 0.1 && трамвай.скорость_abs < 2.8)
					{
						позиция_контроллера = 1;
					}
					else
					{
						позиция_контроллера = 0;
					}
				}
				else if (Math.Abs(трамвай.скорость_abs) < 0.1 && рекомендуемаяСкорость <= 0.0)
				{
					позиция_контроллера = 0;
				}
				else if (num2 > 0.0)
				{
					if (трамвай.скорость_abs > 5.0 && позиция_контроллера == 0)
					{
						позиция_контроллера = 0;
					}
					else if (трамвай.скорость_abs >= 0.0 && трамвай.скорость_abs < 2.3)
					{
						позиция_контроллера = 1;
					}
					else if (трамвай.скорость_abs >= 2.3 && трамвай.скорость_abs < 5.0)
					{
						позиция_контроллера = 2;
					}
					else if (трамвай.скорость_abs >= 5.0 && трамвай.скорость_abs < 10.0)
					{
						позиция_контроллера = 3;
					}
					else if (трамвай.скорость_abs >= 10.0)
					{
						позиция_контроллера = 4;
					}
				}
				else
				{
					if (!(num2 < 0.0))
					{
						return;
					}
					позиция_контроллера = -1;
					if (!(оставшеесяРасстояние / ((num + рекомендуемаяСкорость) / 2.0) < num2 / ускорение))
					{
						return;
					}
					позиция_контроллера = -2;
					if (!(оставшеесяРасстояние / ((num + рекомендуемаяСкорость) / 2.0) < num2 / ускорение))
					{
						return;
					}
					позиция_контроллера = -3;
					if (оставшеесяРасстояние / ((num + рекомендуемаяСкорость) / 2.0) < num2 / ускорение)
					{
						позиция_контроллера = -4;
						if (оставшеесяРасстояние / ((num + рекомендуемаяСкорость) / 2.0) < num2 / ускорение)
						{
							позиция_контроллера = -5;
						}
					}
				}
			}
		}

		public class РКСУ_Троллейбус : Система_управления
		{
			public double пневматический_тормоз;

			public readonly int позиция_max = 4;

			public readonly int позиция_min = -2;

			public int позиция_контроллера;

			public int позиция_реверсора = 1;

			private Троллейбус троллейбус;

			public override int направление => позиция_реверсора;

			public override bool переключение => позиция_контроллера > 0;

			public override double ускорение
			{
				get
				{
					double num = 0.0;
					double num2 = троллейбус.скорость * (double)позиция_реверсора;
					double num3 = ((троллейбус.ах != null && троллейбус.ах.включён) ? троллейбус.ах.ускорение : 1.0);
					if (!троллейбус.обесточен)
					{
						switch (позиция_контроллера)
						{
						case -2:
							num = -1.1 * num3;
							break;
						case -1:
							num = -0.7 * num3;
							break;
						case 1:
							num = 0.8 * num3;
							if (num2 > 2.0 * num3)
							{
								num *= Math.Pow(2.0 * num3 / num2, 4.0);
							}
							break;
						case 2:
							num = 1.1 * num3;
							if (num2 > 8.0 * num3)
							{
								num *= Math.Pow(8.0 * num3 / num2, 4.0);
							}
							break;
						case 3:
							num = 1.3 * num3;
							if (num2 > 8.0 * num3)
							{
								num *= Math.Pow(8.0 * num3 / num2, 4.0);
							}
							break;
						case 4:
							num = 1.5;
							if (num2 > 6.0 * num3)
							{
								num *= 6.0 * num3 / num2;
							}
							if (num2 > 15.0 * num3)
							{
								num *= Math.Pow(15.0 * num3 / num2, 4.0);
							}
							break;
						}
					}
					num -= пневматический_тормоз * 1.5;
					if (троллейбус.stand_brake)
					{
						num -= (1.0 - пневматический_тормоз) * 1.8;
					}
					if (позиция_контроллера < 0 || троллейбус.stand_brake)
					{
						return num * (double)Math.Sign(троллейбус.скорость);
					}
					return num * (double)позиция_реверсора;
				}
			}

			public override int ход_или_тормоз
			{
				get
				{
					if (троллейбус.stand_brake)
					{
						return -1;
					}
					return Math.Sign(позиция_контроллера);
				}
			}

			public РКСУ_Троллейбус(Троллейбус троллейбус)
			{
				this.троллейбус = троллейбус;
			}

			public override void CreateSoundBuffers()
			{
				SoundBuffers = new ISound3D[6];
				SoundBuffers[0] = MyXAudio2.Device.CreateEmitter(60f, троллейбус.основная_папка + "Sound 1.wav"); // ТЭД?
				SoundBuffers[1] = MyXAudio2.Device.CreateEmitter(60f, троллейбус.основная_папка + "Sound 1.wav");
				SoundBuffers[2] = MyXAudio2.Device.CreateEmitter(40f, троллейбус.основная_папка + "Sound 1.wav"); //ТЭД?
				SoundBuffers[3] = MyXAudio2.Device.CreateEmitter(60f, троллейбус.основная_папка + "Sound 2.wav");
				SoundBuffers[4] = MyXAudio2.Device.CreateEmitter(40f, троллейбус.основная_папка + "Sound 2.wav"); //Преобразователь???
				SoundBuffers[5] = MyXAudio2.Device.CreateEmitter(30f, троллейбус.основная_папка + "Sound 3.wav"); //Зуммер
				SoundBuffers[5].Volume = 0.5f;
				SoundBuffers[5].Frequency = 0.5f;
			}

			public override void UpdateSound(Игрок[] игроки, bool игра_активна)
			{
				if (!PreUpdateSound(троллейбус, игра_активна))
				{
					return;
				}

                //Множитель частоты
				float num = 600f * (float)троллейбус.скорость_abs + 1000f;
				if (троллейбус.скорость_abs < 1.0)
				{
					num = Math.Max(1600f * (float)троллейбус.скорость_abs, 1f);
				}
				num /= 20000f;

                //Если троллейбус под током
				if (!троллейбус.обесточен)
				{
					SoundBuffers[0].Frequency = num * 2f;
					SoundBuffers[1].Frequency = num * 3f;
					SoundBuffers[2].Frequency = num * 5f; //ТЭД???
					SoundBuffers[3].Frequency = num * 7f;
					SoundBuffers[4].Frequency = num * 10f; //Преобразователь напряжения???
				}
				else
				{
					for (int i = 0; i < SoundBuffers.Length - 1; i++)
					{
						SoundBuffers[i].Frequency = 0.01f;
					}
				}

                //Отключаем звук ТЭД если стоим
				if (Math.Abs(троллейбус.скорость) < 0.1)
				{
					SoundBuffers[2].Volume = 0f;
				}

                //Регулируем громкость звука ТЭД
				else if (троллейбус._soundУскоряется || троллейбус._soundЗамедляется)
				{
					SoundBuffers[2].Volume = Math.Min(SoundBuffers[2].Volume + 0.2f, 1f);
				}
				else
				{
					SoundBuffers[2].Volume = Math.Max(SoundBuffers[2].Volume - 0.2f, 0f);
				}
				SoundBuffers[0].Volume = 1f;
				SoundBuffers[1].Volume = 1f;

                //Преобразователь напряжения???
				SoundBuffers[3].Volume = 0.8f;
				if (троллейбус.скорость_abs > 2.5)
				{
					SoundBuffers[3].Volume = 1f - (float)Math.Pow(2.0, троллейбус.скорость_abs / 180000.0) / 1000f;
				}
				SoundBuffers[4].Volume = SoundBuffers[3].Volume;
				for (int j = 0; j < SoundBuffers.Length; j++)
				{
					if ((double)SoundBuffers[j].Frequency <= 0.05)
					{
						SoundBuffers[j].Volume = 0f;
					}
				}

                //Зуммер если штанги слетели
				if (троллейбус.обесточен && троллейбус.включен)
				{
					SoundBuffers[5].Play();
				}
				else
				{
					SoundBuffers[5].Stop();
				}
			}

			public override void автоматически_управлять(double рекомендуемаяСкорость, double оставшеесяРасстояние, int переключение)
			{
				if (Math.Abs(троллейбус.скорость) < 0.1 && рекомендуемаяСкорость * (double)позиция_реверсора < 0.0)
				{
					позиция_реверсора = -позиция_реверсора;
				}
				рекомендуемаяСкорость *= (double)позиция_реверсора;
				double num = троллейбус.скорость * (double)позиция_реверсора;
				double num2 = рекомендуемаяСкорость - num;
				if (num + num2 < 0.0)
				{
					num2 -= num + num2;
				}
				if (num < 0.0)
				{
					num2 = 0.0 - num2;
				}
				пневматический_тормоз = 0.0;
				if (переключение >= 0 && num > 0.0)
				{
					switch (переключение)
					{
					case 1:
						if (позиция_контроллера > 0)
						{
							позиция_контроллера = 0;
						}
						break;
					case 0:
						if (позиция_контроллера <= 0)
						{
							позиция_контроллера = 1;
						}
						break;
					}
				}
				else if (Math.Abs(num2) < 0.5 && num > 0.5)
				{
					if (num2 > 0.5 && num < 2.8)
					{
						позиция_контроллера = 1;
					}
					else
					{
						позиция_контроллера = 0;
					}
				}
				else if (Math.Abs(num) < 0.1 && рекомендуемаяСкорость <= 0.0)
				{
					позиция_контроллера = 0;
				}
				else if (num2 > 0.0)
				{
					if (num > 2.3 && позиция_контроллера == 0 && num2 < 1.5)
					{
						позиция_контроллера = 0;
					}
					else if (num >= 0.0 && num < 2.3)
					{
						позиция_контроллера = 1;
					}
					else if (num >= 0.0 && num < 5.0)
					{
						позиция_контроллера = 2;
					}
					else if (num >= 5.0 && num < 10.0)
					{
						позиция_контроллера = 3;
					}
					else if (num >= 10.0)
					{
						позиция_контроллера = 4;
					}
				}
				else
				{
					if (!(num2 < 0.0))
					{
						return;
					}
					позиция_контроллера = -1;
					if (!(оставшеесяРасстояние / (num + рекомендуемаяСкорость) < num2 / ускорение))
					{
						return;
					}
					позиция_контроллера = -2;
					if (оставшеесяРасстояние / (num + рекомендуемаяСкорость) < num2 / ускорение)
					{
						пневматический_тормоз = (num2 - 2.0) * ((num + рекомендуемаяСкорость) / 2.0) / Math.Max(оставшеесяРасстояние - 5.0, 0.6) / -1.8;
						if (пневматический_тормоз < 0.0)
						{
							пневматический_тормоз = 0.0;
						}
						if (пневматический_тормоз > 1.0)
						{
							пневматический_тормоз = 1.0;
						}
					}
				}
			}
		}

		protected ISound3D[] SoundBuffers;

		protected int[] SoundBuffers_Volume;

		protected int[] frequency = new int[6];

		protected int[] last_frequency = new int[6];

		protected int[] volume = new int[6];

		protected int volume_muting;

		private bool isPlaying;

		public abstract int направление { get; }

		public abstract bool переключение { get; }

		public abstract double ускорение { get; }

		public abstract int ход_или_тормоз { get; }

		public abstract void CreateSoundBuffers();

		public static Система_управления Parse(string text, Transport transport)
		{
			return text switch
			{
				"РКСУ_Трамвай" => new РКСУ_Трамвай((Трамвай)transport), 
				"РКСУ_Троллейбус" => new РКСУ_Троллейбус((Троллейбус)transport), 
				"КП_Авто" => new КП_Авто((Троллейбус)transport), 
				_ => null, 
			};
		}

		public abstract void UpdateSound(Игрок[] игроки, bool игра_активна);

		public abstract void автоматически_управлять(double рекомендуемая_скорость, double оставшееся_расстояние, int переключение);

		protected bool PreUpdateSound(Transport transport, bool gameActive)
		{
			bool flag = !transport.condition;
			if (gameActive)
			{
				if (flag && !isPlaying)
				{
					ISound3D[] soundBuffers = SoundBuffers;
					for (int i = 0; i < soundBuffers.Length; i++)
					{
						soundBuffers[i].Play();
					}
					isPlaying = true;
				}
				else if (!flag && isPlaying)
				{
					ISound3D[] soundBuffers = SoundBuffers;
					for (int i = 0; i < soundBuffers.Length; i++)
					{
						soundBuffers[i].Stop();
					}
					isPlaying = false;
				}
			}
			else if (isPlaying)
			{
				ISound3D[] soundBuffers = SoundBuffers;
				for (int i = 0; i < soundBuffers.Length; i++)
				{
					soundBuffers[i].Stop();
				}
				isPlaying = false;
			}
			if (isPlaying)
			{
				Double3DPoint position = transport.Координаты3D;
				ISound3D[] soundBuffers = SoundBuffers;
				for (int i = 0; i < soundBuffers.Length; i++)
				{
					soundBuffers[i].Update(ref position);
				}
			}
			return isPlaying;
		}
	}
}

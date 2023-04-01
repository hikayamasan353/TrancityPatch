using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Common;
using Engine;

namespace Trancity
{
	public class World
	{
		public string filename;

		public MyList listДороги = new MyList(typeof(Road), typeof(Рельс));

		public double time;

		public Ground земля = new Ground();

		public Контактный_провод[] контактныеПровода = new Контактный_провод[0];

		public Трамвайный_контактный_провод[] контактныеПровода2 = new Трамвайный_контактный_провод[0];

		public Route[] маршруты = new Route[0];

		public List<Stop> остановки = new List<Stop>();

		public Парк[] парки = new Парк[1]
		{
			new Парк("Парк")
		};

		public static double прошлоВремени;

		public Светофорная_система[] светофорныеСистемы = new Светофорная_система[0];

		public Сигнальная_система[] сигнальныеСистемы = new Сигнальная_система[0];

		public double системноеВремя;

		public MyList транспорты = new MyList();

		public List<Объект> объекты = new List<Объект>();

		public double time_speed = 1000.0;

		public SkyBox skybox = new SkyBox();

		public static double dtmax;

		public Road[] ВсеДороги => (Road[])listДороги.ToArray(typeof(Road));

		private List<Order> ВсеНаряды
		{
			get
			{
				List<Order> list = new List<Order>();
				Route[] array = маршруты;
				foreach (Route route in array)
				{
					list.AddRange(route.orders);
				}
				return list;
			}
		}

		public Road[] Дороги => listДороги.Get_array<Road>();

		public Рельс[] Рельсы => listДороги.Get_array<Рельс>();

		public void Create_Meshes()
		{
			MyGUI.status_string = Localization.current_.load_models;
			MyGUI.Splash();
			for (int i = 0; i < ВсеДороги.Length; i++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num = MyGUI.load_max / 20 * i / ВсеДороги.Length;
					if (num > MyGUI.load_status)
					{
						MyGUI.load_status = num;
						MyGUI.Splash();
					}
				}
				ВсеДороги[i].CreateMesh();
				if (ВсеДороги[i] is Рельс && (MainForm.in_editor || ((Рельс)ВсеДороги[i]).следующие_рельсы.Length > 1))
				{
					((Рельс)ВсеДороги[i]).добавочные_провода.CreateMesh();
				}
			}
			for (int j = 0; j < контактныеПровода.Length; j++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num2 = MyGUI.load_max / 20 * j / контактныеПровода.Length + MyGUI.load_max / 20;
					if (num2 > MyGUI.load_status)
					{
						MyGUI.load_status = num2;
						MyGUI.Splash();
					}
				}
				контактныеПровода[j].CreateMesh();
			}
			for (int k = 0; k < контактныеПровода2.Length; k++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num3 = MyGUI.load_max / 20 * k / контактныеПровода2.Length + MyGUI.load_max / 20;
					if (num3 > MyGUI.load_status)
					{
						MyGUI.load_status = num3;
						MyGUI.Splash();
					}
				}
				контактныеПровода2[k].CreateMesh();
			}
			for (int l = 0; l < транспорты.Count; l++)
			{
				int num4 = MyGUI.load_max * 18 / 20 * l / транспорты.Count + MyGUI.load_max / 20;
				if (num4 > MyGUI.load_status)
				{
					MyGUI.load_status = num4;
					MyGUI.Splash();
				}
				((Transport)транспорты[l]).CreateMesh(this);
			}
			MyGUI.load_status = MyGUI.load_max;
			MyGUI.Splash();
			Сигнальная_система[] array = сигнальныеСистемы;
			for (int m = 0; m < array.Length; m++)
			{
				array[m].CreateMesh();
			}
			Светофорная_система[] array2 = светофорныеСистемы;
			for (int m = 0; m < array2.Length; m++)
			{
				array2[m].CreateMesh();
			}
			земля.CreateMesh();
			if (SkyBox.draw && !MainForm.in_editor)
			{
				MyGUI.status_string = Localization.current_.load_shaders;
				MyGUI.Splash();
				skybox.CreateMesh();
			}
			MyGUI.load_status = 0;
			MyGUI.status_string = Localization.current_.load_objects;
			MyGUI.Splash();
			int num5 = ((объекты.Count > 1) ? (объекты.Count - 1) : объекты.Count);
			for (int n = 0; n < объекты.Count; n++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num6 = MyGUI.load_max * n / num5;
					if (num6 > MyGUI.load_status)
					{
						MyGUI.load_status = num6;
						MyGUI.Splash();
					}
				}
				объекты[n].CreateMesh();
			}
			MyGUI.load_status = MyGUI.load_max;
			MyGUI.Splash();
			MyGUI.load_status = 0;
			MyGUI.status_string = Localization.current_.load_stops;
			MyGUI.Splash();
			int num7 = ((остановки.Count > 1) ? (остановки.Count - 1) : остановки.Count);
			for (int num8 = 0; num8 < остановки.Count; num8++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num9 = MyGUI.load_max * num8 / num7;
					if (num9 > MyGUI.load_status)
					{
						MyGUI.load_status = num9;
						MyGUI.Splash();
					}
				}
				остановки[num8].CreateMesh();
				остановки[num8].ОбновитьКартинку();
				if (!MainForm.in_editor)
				{
					остановки[num8].ComputeMatrix();
				}
			}
			MyGUI.load_status = MyGUI.load_max;
			MyGUI.Splash();
		}

		public void CreateSound()
		{
			MyGUI.load_status = 0;
			MyGUI.status_string = Localization.current_.load_sounds;
			MyGUI.Splash();
			int num = ((транспорты.Count > 1) ? (транспорты.Count - 1) : транспорты.Count);
			for (int i = 0; i < транспорты.Count; i++)
			{
				if (Environment.TickCount - MainForm.ticklast > 50)
				{
					int num2 = MyGUI.load_max * i / num;
					if (num2 > MyGUI.load_status)
					{
						MyGUI.load_status = num2;
						MyGUI.Splash();
					}
				}
				((Transport)транспорты[i]).CreateSoundBuffers();
			}
			MyGUI.load_status = MyGUI.load_max;
			MyGUI.Splash();
		}

		public void RenderMeshes()
		{
			Рельс[] рельсы = Рельсы;
			foreach (Рельс рельс in рельсы)
			{
				if (рельс.следующие_рельсы.Length > 1)
				{
					рельс.добавочные_провода.Render();
				}
			}
			Контактный_провод[] array = контактныеПровода;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Render();
			}
			Трамвайный_контактный_провод[] array2 = контактныеПровода2;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Render();
			}
			foreach (Transport item in транспорты)
			{
				item.Render();
			}
			Сигнальная_система[] array3 = сигнальныеСистемы;
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].Render();
			}
			Светофорная_система[] array4 = светофорныеСистемы;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].Render();
			}
			foreach (Stop item2 in остановки)
			{
				item2.CheckCondition();
				item2.Render();
			}
			foreach (Объект item3 in объекты)
			{
				item3.CheckCondition();
				item3.Render();
			}
		}

		public void RenderMeshes2()
		{
			skybox.Render();
			земля.Render();
		}

		public void RenderMeshesA()
		{
			Road[] всеДороги = ВсеДороги;
			for (int i = 0; i < всеДороги.Length; i++)
			{
				всеДороги[i].Render();
			}
		}

		public void UpdateSound(Игрок[] игроки, bool игра_активна)
		{
			MyXAudio2.Device.UpdateListner(ref игроки[0].cameraPosition, ref игроки[0].cameraRotation);
			foreach (Transport item in транспорты)
			{
				item.UpdateSound(игроки, игра_активна);
			}
		}

		public void ДобавитьТранспорт(MainForm.НастройкиЗапуска настройки, Game игра)
		{
			if (игра == null)
			{
				игра = new Game();
			}
			List<Order> всеНаряды = ВсеНаряды;
			for (int i = 0; i < всеНаряды.Count; i++)
			{
				int num = всеНаряды[i].рейсы.Length - 1;
				if (num < 0 || !(всеНаряды[i].рейсы[num].время_прибытия >= time))
				{
					всеНаряды.RemoveAt(i);
					i--;
				}
			}
			List<Transport>[] array = new List<Transport>[парки.Length];
			for (int j = 0; j < парки.Length; j++)
			{
				array[j] = new List<Transport>();
			}
			игра.игроки = new Игрок[настройки.количествоИгроков];
			for (int k = 0; k < настройки.количествоИгроков; k++)
			{
				Transport transport = null;
				MainForm.НастройкиЗапускаИгрока настройкиЗапускаИгрока = настройки.игроки[k];
				игра.игроки[k] = new Игрок();
				игра.игроки[k].cameraPositionChange = Double3DPoint.Zero;
				игра.игроки[k].cameraRotationChange = DoublePoint.Zero;
				игра.игроки[k].inputGuid = настройкиЗапускаИгрока.inputGuid;
				игра.игроки[k].поворачиватьКамеру = настройки.поворачиватьКамеру;
				МодельТранспорта модельТранспорта = null;
				int num2 = -1;
				foreach (МодельТранспорта item in Модели.Трамваи)
				{
					if (!(настройкиЗапускаИгрока.подвижнойСостав != item.name))
					{
						num2 = 0;
						модельТранспорта = item;
					}
				}
				foreach (МодельТранспорта item2 in Модели.Троллейбусы)
				{
					if (!(настройкиЗапускаИгрока.подвижнойСостав != item2.name))
					{
						num2 = 1;
						модельТранспорта = item2;
					}
				}
				foreach (МодельТранспорта item3 in Модели.Автобусы)
				{
					if (!(настройкиЗапускаИгрока.подвижнойСостав != item3.name))
					{
						num2 = 2;
						модельТранспорта = item3;
					}
				}
				if (модельТранспорта == null || num2 == -1)
				{
					Logger.Log("World", "Couldn't find model " + настройкиЗапускаИгрока.подвижнойСостав + " for player " + настройкиЗапускаИгрока.имя);
					continue;
				}
				Управление управление = (настройки.автоматическоеУправление ? Управление.Автоматическое : Управление.Ручное);
				Route route = new Route(num2, "-");
				if (настройкиЗапускаИгрока.маршрут > 0 && маршруты.Length != 0)
				{
					if (настройкиЗапускаИгрока.маршрут > 1 && настройкиЗапускаИгрока.маршрут - 2 < маршруты.Length)
					{
						route = маршруты[настройкиЗапускаИгрока.маршрут - 2];
					}
					else
					{
						try
						{
							List<Route> list = new List<Route>();
							Route[] array2 = маршруты;
							foreach (Route route2 in array2)
							{
								if (route2.typeOfTransport == num2)
								{
									list.Add(route2);
								}
							}
							route = list[Cheats._random.Next(list.Count)];
						}
						catch
						{
							Logger.Log("World", "Random route not selected for player " + настройкиЗапускаИгрока.имя);
						}
					}
				}
				Парк парк = парки[Cheats._random.Next(парки.Length)];
				Order order = new Order(парк, route, "-", настройкиЗапускаИгрока.подвижнойСостав);
				if (настройкиЗапускаИгрока.наряд > 1 && настройкиЗапускаИгрока.наряд - 2 < route.orders.Length)
				{
					order = route.orders[настройкиЗапускаИгрока.наряд - 2];
				}
				else if (настройкиЗапускаИгрока.наряд == 1)
				{
					try
					{
						order = route.orders[Cheats._random.Next(route.orders.Length)];
					}
					catch
					{
						Logger.Log("World", "Random order not selected for player " + настройкиЗапускаИгрока.имя);
					}
				}
				Road дорога;
				if (num2 != 0)
				{
					try
					{
						дорога = Дороги[Cheats._random.Next(Дороги.Length)];
					}
					catch
					{
						дорога = new Road(0.0, 0.0, 20.0, 0.0, 0.0, прямая: true, 1.0, 1.0);
						дорога.следующиеДороги = new Road[1] { дорога };
						дорога.предыдущиеДороги = new Road[1] { дорога };
						дорога.соседниеДороги = new Road[1] { дорога };
					}
				}
				else
				{
					try
					{
						дорога = Рельсы[Cheats._random.Next(Рельсы.Length)];
					}
					catch
					{
						дорога = new Рельс(0.0, 0.0, 20.0, 0.0, 0.0, прямой: true);
						дорога.следующиеДороги = new Road[1] { дорога };
						дорога.предыдущиеДороги = new Road[1] { дорога };
						дорога.соседниеДороги = new Road[1] { дорога };
					}
				}
				double расстояние_по_дороге = Cheats._random.NextDouble() * дорога.Длина;
				Trip рейс = null;
				bool from_park = false;
				if (order != null)
				{
					всеНаряды.Remove(order);
					парк = order.парк;
					Найти_положение_наряда(order, ref рейс, ref дорога, ref расстояние_по_дороге, ref from_park);
				}
				else if (route.trips.Count > 0)
				{
					рейс = route.trips[0];
				}
				игра.игроки[k].cameraPosition = new Double3DPoint(0.0, 2.0, 0.0);
				игра.игроки[k].cameraRotation = new DoublePoint(0.0, -0.1);
				switch (num2)
				{
				case 0:
					if (!(дорога is Рельс))
					{
						Logger.Log("World", "Attempt to place tramway on road for player " + настройкиЗапускаИгрока.имя);
					}
					else
					{
						transport = new Трамвай.ОбычныйТрамвай(модельТранспорта, (Рельс)дорога, расстояние_по_дороге, управление, парк, route, order);
					}
					break;
				case 1:
				case 2:
				{
					Double3DPoint double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = дорога.НайтиКоординаты(расстояние_по_дороге, 0.0);
					double3DPoint.y = дорога.НайтиВысоту(расстояние_по_дороге);
					Double3DPoint координаты = double3DPoint;
					DoublePoint направление = new DoublePoint(дорога.НайтиНаправление(расстояние_по_дороге), дорога.НайтиНаправлениеY(расстояние_по_дороге));
					transport = new Троллейбус.ОбычныйТроллейбус(модельТранспорта, координаты, направление, управление, парк, route, order);
					transport.SetPosition(дорога, расстояние_по_дороге, 0.0, Double3DPoint.Zero, DoublePoint.Zero, this);
					break;
				}
				}
				игра.игроки[k].управляемыйОбъект = transport;
				игра.игроки[k].объектПривязки = transport;
				transport.наряд = order;
				transport.рейс = рейс;
				if (transport is Троллейбус)
				{
					Троллейбус.Штанга[] штанги = ((Троллейбус)transport).штанги;
					foreach (Троллейбус.Штанга штанга in штанги)
					{
						штанга.НайтиПровод(контактныеПровода);
						if (штанга.Провод != null)
						{
							штанга.поднимается = true;
							штанга.Обновить(включенТэд: false);
							штанга.угол = штанга.уголNormal;
						}
					}
				}
				else
				{
					Трамвай.Токоприёмник_new токоприёмник = ((Трамвай)transport).токоприёмник;
					((Трамвай)transport).Обновить(this, new Игрок[0]);
					токоприёмник.НайтиПровод(контактныеПровода2);
					if (токоприёмник.Провод != null)
					{
						токоприёмник.поднимается = true;
						токоприёмник.высота = токоприёмник.обычная_высота_max;
					}
				}
				if (!transport.SetCamera(0, игра.игроки[k]))
				{
					игра.игроки[k].cameraRotation = new DoublePoint(transport.direction, transport.НаправлениеY - 0.1);
					игра.игроки[k].cameraPosition = Double3DPoint.Multiply(new Double3DPoint(8.0, 2.5, 0.0), transport.Координаты3D, игра.игроки[k].cameraRotation);
				}
				транспорты.Add(transport);
				if (!from_park)
				{
					continue;
				}
				for (int m = 0; m < парки.Length; m++)
				{
					if (парк == парки[m])
					{
						array[m].Add(transport);
					}
				}
			}
			for (int n = 0; n < всеНаряды.Count; n++)
			{
				Road дорога2 = null;
				double расстояние_по_дороге2 = 0.0;
				Trip рейс2 = null;
				bool from_park2 = false;
				Найти_положение_наряда(всеНаряды[n], ref рейс2, ref дорога2, ref расстояние_по_дороге2, ref from_park2);
				if (рейс2 == null || дорога2 == null)
				{
					continue;
				}
				Transport transport2 = null;
				switch (всеНаряды[n].маршрут.typeOfTransport)
				{
				case 0:
				{
					if (Модели.Трамваи.Count == 0)
					{
						continue;
					}
					МодельТранспорта модельТранспорта4 = null;
					if (всеНаряды[n].transport == "" || всеНаряды[n].transport == "Случайный")
					{
						модельТранспорта4 = Модели.Трамваи[Cheats._random.Next(0, Модели.Трамваи.Count)];
					}
					else
					{
						модельТранспорта4 = null;
						foreach (МодельТранспорта item4 in Модели.Трамваи)
						{
							if (item4.name == всеНаряды[n].transport)
							{
								модельТранспорта4 = item4;
								break;
							}
						}
						if (модельТранспорта4 == null)
						{
							модельТранспорта4 = Модели.Трамваи[Cheats._random.Next(0, Модели.Трамваи.Count)];
						}
					}
					if (!(дорога2 is Рельс))
					{
						Logger.Log("World", "Attempt to place tramway (route " + всеНаряды[n].номер + ") on road");
					}
					else
					{
						transport2 = new Трамвай.ОбычныйТрамвай(модельТранспорта4, (Рельс)дорога2, расстояние_по_дороге2, Управление.Автоматическое, всеНаряды[n].парк, всеНаряды[n].маршрут, всеНаряды[n]);
					}
					break;
				}
				case 1:
				{
					if (Модели.Троллейбусы.Count == 0)
					{
						continue;
					}
					Double3DPoint double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = дорога2.НайтиКоординаты(расстояние_по_дороге2, 0.0);
					double3DPoint.y = дорога2.НайтиВысоту(расстояние_по_дороге2);
					Double3DPoint координаты3 = double3DPoint;
					DoublePoint направление3 = new DoublePoint(дорога2.НайтиНаправление(расстояние_по_дороге2), дорога2.НайтиНаправлениеY(расстояние_по_дороге2));
					МодельТранспорта модельТранспорта3 = null;
					if (всеНаряды[n].transport == "" || всеНаряды[n].transport == "Случайный")
					{
						модельТранспорта3 = Модели.Троллейбусы[Cheats._random.Next(0, Модели.Троллейбусы.Count)];
					}
					else
					{
						модельТранспорта3 = null;
						foreach (МодельТранспорта item5 in Модели.Троллейбусы)
						{
							if (item5.name == всеНаряды[n].transport)
							{
								модельТранспорта3 = item5;
								break;
							}
						}
						if (модельТранспорта3 == null)
						{
							модельТранспорта3 = Модели.Троллейбусы[Cheats._random.Next(0, Модели.Троллейбусы.Count)];
						}
					}
					transport2 = new Троллейбус.ОбычныйТроллейбус(модельТранспорта3, координаты3, направление3, Управление.Автоматическое, всеНаряды[n].парк, всеНаряды[n].маршрут, всеНаряды[n]);
					transport2.SetPosition(дорога2, расстояние_по_дороге2, 0.0, Double3DPoint.Zero, DoublePoint.Zero, this);
					break;
				}
				default:
				{
					if (всеНаряды[n].маршрут.typeOfTransport != 2)
					{
						throw new Exception("Такого вида транспорта не существует!!!");
					}
					if (Модели.Автобусы.Count == 0)
					{
						continue;
					}
					Double3DPoint double3DPoint = default(Double3DPoint);
					double3DPoint.XZPoint = дорога2.НайтиКоординаты(расстояние_по_дороге2, 0.0);
					double3DPoint.y = дорога2.НайтиВысоту(расстояние_по_дороге2);
					Double3DPoint координаты2 = double3DPoint;
					DoublePoint направление2 = new DoublePoint(дорога2.НайтиНаправление(расстояние_по_дороге2), дорога2.НайтиНаправлениеY(расстояние_по_дороге2));
					МодельТранспорта модельТранспорта2 = null;
					if (всеНаряды[n].transport == "" || всеНаряды[n].transport == "Случайный")
					{
						модельТранспорта2 = Модели.Автобусы[Cheats._random.Next(0, Модели.Автобусы.Count)];
					}
					else
					{
						модельТранспорта2 = null;
						foreach (МодельТранспорта item6 in Модели.Автобусы)
						{
							if (item6.name == всеНаряды[n].transport)
							{
								модельТранспорта2 = item6;
								break;
							}
						}
						if (модельТранспорта2 == null)
						{
							модельТранспорта2 = Модели.Автобусы[Cheats._random.Next(0, Модели.Автобусы.Count)];
						}
					}
					transport2 = new Троллейбус.ОбычныйТроллейбус(модельТранспорта2, координаты2, направление2, Управление.Автоматическое, всеНаряды[n].парк, всеНаряды[n].маршрут, всеНаряды[n]);
					transport2.SetPosition(дорога2, расстояние_по_дороге2, 0.0, Double3DPoint.Zero, DoublePoint.Zero, this);
					break;
				}
				}
				if (transport2 == null)
				{
					continue;
				}
				transport2.наряд = всеНаряды[n];
				transport2.рейс = рейс2;
				if (transport2 is Троллейбус)
				{
					Троллейбус.Штанга[] штанги = ((Троллейбус)transport2).штанги;
					foreach (Троллейбус.Штанга штанга2 in штанги)
					{
						штанга2.НайтиПровод(контактныеПровода);
						if (штанга2.Провод != null)
						{
							штанга2.поднимается = true;
							штанга2.Обновить(включенТэд: false);
							штанга2.угол = штанга2.уголNormal;
						}
					}
				}
				else
				{
					Трамвай.Токоприёмник_new токоприёмник2 = ((Трамвай)transport2).токоприёмник;
					((Трамвай)transport2).Обновить(this, new Игрок[0]);
					токоприёмник2.НайтиПровод(контактныеПровода2);
					if (токоприёмник2.Провод != null)
					{
						токоприёмник2.поднимается = true;
						токоприёмник2.высота = токоприёмник2.обычная_высота_max;
					}
				}
				транспорты.Add(transport2);
				if (!from_park2)
				{
					continue;
				}
				for (int num3 = 0; num3 < парки.Length; num3++)
				{
					if (transport2.парк == парки[num3])
					{
						array[num3].Add(transport2);
					}
				}
			}
			for (int num4 = 0; num4 < парки.Length; num4++)
			{
				if (парки[num4].пути_стоянки.Length == 0)
				{
					continue;
				}
				int num5 = 0;
				double num6 = парки[num4].пути_стоянки[0].Длина;
				while (array[num4].Count > 0)
				{
					Transport transport3 = array[num4][0];
					foreach (Transport item7 in array[num4])
					{
						if (transport3.рейс.время_отправления > item7.рейс.время_отправления)
						{
							transport3 = item7;
						}
					}
					while (num6 - transport3.length0 < -1.0 && num5 < парки[num4].пути_стоянки.Length - 1)
					{
						num5++;
						num6 = парки[num4].пути_стоянки[num5].Длина;
					}
					transport3.SetPosition(парки[num4].пути_стоянки[num5], num6 - transport3.length0 + 1.0, 0.0, Double3DPoint.Zero, DoublePoint.Zero, this);
					num6 -= transport3.length0 + transport3.length1;
					for (int num7 = 0; num7 < игра.игроки.Length; num7++)
					{
						if (игра.игроки[num7].управляемыйОбъект == transport3 && !transport3.SetCamera(0, игра.игроки[num7]))
						{
							игра.игроки[num7].cameraRotation = new DoublePoint(transport3.direction, transport3.НаправлениеY - 0.1);
							игра.игроки[num7].cameraPosition = Double3DPoint.Multiply(new Double3DPoint(8.0, 2.5, 0.0), transport3.Координаты3D, игра.игроки[num7].cameraRotation);
						}
					}
					if (transport3 is Троллейбус)
					{
						Троллейбус.Штанга[] штанги = ((Троллейбус)transport3).штанги;
						foreach (Троллейбус.Штанга штанга3 in штанги)
						{
							штанга3.НайтиПровод(контактныеПровода);
							if (штанга3.Провод != null)
							{
								штанга3.поднимается = true;
								штанга3.Обновить(включенТэд: false);
								штанга3.угол = штанга3.уголNormal;
							}
						}
					}
					else
					{
						Трамвай.Токоприёмник_new токоприёмник3 = ((Трамвай)transport3).токоприёмник;
						((Трамвай)transport3).Обновить(this, new Игрок[0]);
						токоприёмник3.НайтиПровод(контактныеПровода2);
						if (токоприёмник3.Провод != null)
						{
							токоприёмник3.поднимается = true;
							токоприёмник3.высота = токоприёмник3.обычная_высота_max;
						}
					}
					array[num4].Remove(transport3);
				}
			}
		}

		public void ЗагрузитьГород(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return;
			}
			Logger.Log("LoadCity", $"Loading {filename}");
			XmlElement xmlElement = Xml.TryOpenDocument(filename)["City"];
			listДороги.Clear();
			if (xmlElement == null)
			{
				return;
			}
			XmlElement xmlElement2 = xmlElement["Rails"];
			if (xmlElement2 != null)
			{
				for (int i = 0; i < xmlElement2.ChildNodes.Count; i++)
				{
					XmlElement xmlElement3 = xmlElement2["rail" + i];
					listДороги.Add(new Рельс(Xml.GetDouble(xmlElement3["x0"]), Xml.GetDouble(xmlElement3["y0"]), Xml.GetDouble(xmlElement3["x1"]), Xml.GetDouble(xmlElement3["y1"]), Xml.GetDouble(xmlElement3["angle0"]), Xml.GetDouble(xmlElement3["angle1"])));
					if (xmlElement3["height0"] != null && xmlElement3["height1"] != null)
					{
						Рельсы[i].высота[0] = Xml.GetDouble(xmlElement3["height0"]);
						Рельсы[i].высота[1] = Xml.GetDouble(xmlElement3["height1"]);
					}
					Рельсы[i].расстояние_добавочных_проводов = Xml.GetDouble(xmlElement3["d_strel"]);
					Рельсы[i].кривая = Xml.GetDouble(xmlElement3["iskriv"]) != 0.0;
					Рельсы[i].name = Xml.GetString(xmlElement3["name"], "Rails");
				}
			}
			XmlElement xmlElement4 = xmlElement["Roads"];
			if (xmlElement4 != null)
			{
				for (int j = 0; j < xmlElement4.ChildNodes.Count; j++)
				{
					XmlElement xmlElement5 = xmlElement4["road" + j];
					listДороги.Add(new Road(Xml.GetDouble(xmlElement5["x0"]), Xml.GetDouble(xmlElement5["y0"]), Xml.GetDouble(xmlElement5["x1"]), Xml.GetDouble(xmlElement5["y1"]), Xml.GetDouble(xmlElement5["angle0"]), Xml.GetDouble(xmlElement5["angle1"]), Xml.GetDouble(xmlElement5["wide0"]), Xml.GetDouble(xmlElement5["wide1"])));
					if (xmlElement5["height0"] != null && xmlElement5["height1"] != null)
					{
						Дороги[j].высота[0] = Xml.GetDouble(xmlElement5["height0"]);
						Дороги[j].высота[1] = Xml.GetDouble(xmlElement5["height1"]);
					}
					Дороги[j].кривая = Xml.GetDouble(xmlElement5["iskriv"]) != 0.0;
					Дороги[j].name = Xml.GetString(xmlElement5["name"], "Road");
				}
			}
			Road[] всеДороги = ВсеДороги;
			for (int k = 0; k < всеДороги.Length; k++)
			{
				всеДороги[k].ОбновитьСледующиеДороги(ВсеДороги);
			}
			всеДороги = ВсеДороги;
			for (int k = 0; k < всеДороги.Length; k++)
			{
				всеДороги[k].CreateBoundingSphere();
			}
			XmlElement xmlElement6 = xmlElement["Trolleybus_lines"];
			if (xmlElement6 != null)
			{
				контактныеПровода = new Контактный_провод[xmlElement6.ChildNodes.Count];
				for (int l = 0; l < xmlElement6.ChildNodes.Count; l++)
				{
					XmlElement xmlElement7 = xmlElement6["line" + l];
					контактныеПровода[l] = new Контактный_провод(Xml.GetDouble(xmlElement7["x0"]), Xml.GetDouble(xmlElement7["y0"]), Xml.GetDouble(xmlElement7["x1"]), Xml.GetDouble(xmlElement7["y1"]), Xml.GetDouble(xmlElement7["right"]) != 0.0);
					if (xmlElement7["height0"] != null && xmlElement7["height1"] != null)
					{
						контактныеПровода[l].высота[0] = Xml.GetDouble(xmlElement7["height0"]);
						контактныеПровода[l].высота[1] = Xml.GetDouble(xmlElement7["height1"]);
					}
					контактныеПровода[l].обесточенный = Xml.GetDouble(xmlElement7["no_contact"]) != 0.0;
				}
			}
			Контактный_провод[] array = контактныеПровода;
			foreach (Контактный_провод obj in array)
			{
				obj.UpdateNextWires(контактныеПровода);
				obj.ComputeMatrix();
			}
			XmlElement xmlElement8 = xmlElement["Tramway_lines"];
			if (xmlElement8 != null)
			{
				контактныеПровода2 = new Трамвайный_контактный_провод[xmlElement8.ChildNodes.Count];
				for (int m = 0; m < xmlElement8.ChildNodes.Count; m++)
				{
					XmlElement xmlElement9 = xmlElement8["line" + m];
					контактныеПровода2[m] = new Трамвайный_контактный_провод(Xml.GetDouble(xmlElement9["x0"]), Xml.GetDouble(xmlElement9["y0"]), Xml.GetDouble(xmlElement9["x1"]), Xml.GetDouble(xmlElement9["y1"]));
					if (xmlElement9["height0"] != null && xmlElement9["height1"] != null)
					{
						контактныеПровода2[m].высота[0] = Xml.GetDouble(xmlElement9["height0"]);
						контактныеПровода2[m].высота[1] = Xml.GetDouble(xmlElement9["height1"]);
					}
					контактныеПровода2[m].обесточенный = Xml.GetDouble(xmlElement9["no_contact"]) != 0.0;
				}
			}
			Трамвайный_контактный_провод[] array2 = контактныеПровода2;
			foreach (Трамвайный_контактный_провод obj2 in array2)
			{
				obj2.UpdateNextWires(контактныеПровода2);
				obj2.ComputeMatrix();
			}
			XmlElement xmlElement10 = xmlElement["Parks"];
			if (xmlElement10 != null)
			{
				парки = new Парк[xmlElement10.ChildNodes.Count];
				for (int n = 0; n < xmlElement10.ChildNodes.Count; n++)
				{
					XmlElement xmlElement11 = xmlElement10["park" + n];
					парки[n] = new Парк(xmlElement11["name"].InnerText);
					int num = (int)Xml.GetDouble(xmlElement11["in"]);
					if (num >= 0)
					{
						парки[n].въезд = ВсеДороги[num];
					}
					int num2 = (int)Xml.GetDouble(xmlElement11["out"]);
					if (num2 >= 0)
					{
						парки[n].выезд = ВсеДороги[num2];
					}
					XmlElement xmlElement12 = xmlElement11["park_rails"];
					парки[n].пути_стоянки = new Road[xmlElement12.ChildNodes.Count];
					for (int num3 = 0; num3 < xmlElement12.ChildNodes.Count; num3++)
					{
						парки[n].пути_стоянки[num3] = ВсеДороги[(int)Xml.GetDouble(xmlElement12["park_rail" + num3])];
					}
				}
			}
			XmlElement xmlElement13 = xmlElement["Stops"];
			if (xmlElement13 != null)
			{
				остановки = new List<Stop>();
				bool flag = false;
				for (int num4 = 0; num4 < xmlElement13.ChildNodes.Count; num4++)
				{
					XmlElement xmlElement14 = xmlElement13[$"stop{num4}"];
					string innerText = xmlElement14["name"].InnerText;
					try
					{
						остановки.Add(new Stop(Xml.GetString(xmlElement14["model"], "Stop (4 routes)"), new TypeOfTransport(0), ВсеДороги[(int)Xml.GetDouble(xmlElement14["rail"])], Xml.GetDouble(xmlElement14["distance"])));
					}
					catch
					{
						throw new IndexOutOfRangeException("Can't load stop[" + num4 + "] " + innerText);
					}
					XmlElement xmlElement15 = xmlElement14["type"];
					if (xmlElement15 != null)
					{
						if (xmlElement15.InnerText == "0" || xmlElement15.InnerText == "1" || xmlElement15.InnerText == "2")
						{
							if (!flag)
							{
								Logger.Log("LoadCity", "Too old city file (greater than 0.6.2)");
								flag = true;
							}
							остановки[num4].typeOfTransport[(int)Xml.GetDouble(xmlElement14["type"])] = true;
						}
						else
						{
							XmlNodeList childNodes = xmlElement15.ChildNodes;
							остановки[num4].typeOfTransport[0] = ((childNodes[0].InnerText == "True") ? true : false);
							остановки[num4].typeOfTransport[1] = ((childNodes[1].InnerText == "True") ? true : false);
							остановки[num4].typeOfTransport[2] = ((childNodes[2].InnerText == "True") ? true : false);
						}
					}
					остановки[num4].название = innerText;
					XmlElement xmlElement16 = xmlElement14["stop_path"];
					остановки[num4].частьПути = new Road[xmlElement16.ChildNodes.Count];
					for (int num5 = 0; num5 < xmlElement16.ChildNodes.Count; num5++)
					{
						остановки[num4].частьПути[num5] = ВсеДороги[(int)Xml.GetDouble(xmlElement16["stop_rail" + num5])];
					}
					остановки[num4].UpdatePosition(this);
				}
			}
			XmlElement xmlElement17 = xmlElement["Routes"];
			if (xmlElement17 != null)
			{
				маршруты = new Route[xmlElement17.ChildNodes.Count];
				for (int num6 = 0; num6 < xmlElement17.ChildNodes.Count; num6++)
				{
					XmlElement xmlElement18 = null;
					XmlElement xmlElement19 = xmlElement17[$"route{num6}"];
					if (xmlElement19 != null)
					{
						маршруты[num6] = new Route(0, xmlElement19["name"].InnerText);
						if (xmlElement19["type"] != null)
						{
							маршруты[num6].typeOfTransport = (int)Xml.GetDouble(xmlElement19["type"]);
						}
						xmlElement18 = xmlElement19["route_runs"];
					}
					if (xmlElement18 != null)
					{
						for (int num7 = 0; num7 < xmlElement18.ChildNodes.Count; num7++)
						{
							XmlElement xmlElement20 = xmlElement18[$"run{num7}"];
							if (xmlElement20 == null)
							{
								continue;
							}
							Trip trip = new Trip
							{
								route = маршруты[num6],
								время_прибытия = Xml.GetDouble(xmlElement20["time"])
							};
							XmlElement xmlElement21 = xmlElement20["run_rails"];
							if (xmlElement21 != null)
							{
								trip.pathes = new Road[xmlElement21.ChildNodes.Count];
								for (int num8 = 0; num8 < xmlElement21.ChildNodes.Count; num8++)
								{
									trip.pathes[num8] = ВсеДороги[(int)Xml.GetDouble(xmlElement21["run_rail" + num8])];
								}
							}
							XmlElement xmlElement22 = xmlElement20["Stops"];
							if (xmlElement22 != null)
							{
								trip.tripStopList = new List<TripStop>();
								int num9 = 0;
								for (int num10 = 0; num10 < xmlElement22.ChildNodes.Count; num10++)
								{
									XmlNode xmlNode = xmlElement22.ChildNodes[num10];
									num9 = int.Parse(xmlNode.Name.Substring(4));
									if (num9 >= 0)
									{
										Stop stop = остановки[num9];
										trip.tripStopList.Add(new TripStop(stop, xmlNode.InnerText == "Да" || xmlNode.InnerText == "1"));
									}
								}
							}
							else
							{
								trip.InitTripStopList();
							}
							маршруты[num6].trips.Add(trip);
						}
					}
					if (xmlElement19 != null)
					{
						XmlElement xmlElement23 = xmlElement19["park_runs"];
						if (xmlElement23 != null)
						{
							for (int num11 = 0; num11 < xmlElement23.ChildNodes.Count; num11++)
							{
								XmlElement xmlElement24 = xmlElement23["run" + num11];
								if (xmlElement24 == null)
								{
									continue;
								}
								Trip trip2 = new Trip
								{
									route = маршруты[num6],
									inPark = (Xml.GetDouble(xmlElement24["to_park"]) != 0.0),
									inParkIndex = (int)Xml.GetDouble(xmlElement24["to_park_index"]),
									время_прибытия = Xml.GetDouble(xmlElement24["time"])
								};
								XmlElement xmlElement25 = xmlElement24["run_rails"];
								if (xmlElement25 != null)
								{
									trip2.pathes = new Road[xmlElement25.ChildNodes.Count];
									for (int num12 = 0; num12 < xmlElement25.ChildNodes.Count; num12++)
									{
										trip2.pathes[num12] = ВсеДороги[(int)Xml.GetDouble(xmlElement25["run_rail" + num12])];
									}
								}
								XmlElement xmlElement26 = xmlElement24["Stops"];
								if (xmlElement26 != null)
								{
									trip2.tripStopList = new List<TripStop>();
									int num13 = 0;
									for (int num14 = 0; num14 < xmlElement26.ChildNodes.Count; num14++)
									{
										XmlNode xmlNode2 = xmlElement26.ChildNodes[num14];
										num13 = int.Parse(xmlNode2.Name.Substring(4));
										if (num13 >= 0)
										{
											Stop stop2 = остановки[num13];
											trip2.tripStopList.Add(new TripStop(stop2, xmlNode2.InnerText == "Да" || xmlNode2.InnerText == "1"));
										}
									}
								}
								else
								{
									trip2.InitTripStopList();
								}
								маршруты[num6].parkTrips.Add(trip2);
							}
						}
					}
					if (xmlElement19 == null)
					{
						continue;
					}
					XmlElement xmlElement27 = xmlElement19["Narads"];
					if (xmlElement27 == null)
					{
						continue;
					}
					маршруты[num6].orders = new Order[xmlElement27.ChildNodes.Count];
					XmlElement xmlElement28 = null;
					for (int num15 = 0; num15 < xmlElement27.ChildNodes.Count; num15++)
					{
						XmlElement xmlElement29 = xmlElement27[$"narad{num15}"];
						if (xmlElement29 != null)
						{
							маршруты[num6].orders[num15] = new Order(парки[(int)Xml.GetDouble(xmlElement29["park"])], маршруты[num6], xmlElement29["name"].InnerText, Xml.GetString(xmlElement29["transport"]))
							{
								поРабочим = (Xml.GetDouble(xmlElement29["po_rabochim"]) != 0.0),
								поВыходным = (Xml.GetDouble(xmlElement29["po_vihodnim"]) != 0.0)
							};
							xmlElement28 = xmlElement29["runs"];
						}
						if (xmlElement28 == null)
						{
							continue;
						}
						маршруты[num6].orders[num15].рейсы = new Trip[xmlElement28.ChildNodes.Count];
						for (int num16 = 0; num16 < xmlElement28.ChildNodes.Count; num16++)
						{
							XmlElement xmlElement30 = xmlElement28[$"run{num16}"];
							if (xmlElement30 != null)
							{
								int index = (int)Xml.GetDouble(xmlElement30["index"]);
								double @double = Xml.GetDouble(xmlElement30["time"]);
								маршруты[num6].orders[num15].рейсы[num16] = ((Xml.GetDouble(xmlElement30["park"]) == 0.0) ? маршруты[num6].trips[index].Clone(@double) : маршруты[num6].parkTrips[index].Clone(@double));
							}
						}
					}
				}
				if (xmlElement13 != null)
				{
					for (int num17 = 0; num17 < xmlElement13.ChildNodes.Count; num17++)
					{
						остановки[num17].ОбновитьМаршруты(маршруты);
					}
				}
			}
			XmlElement xmlElement31 = xmlElement["Signals"];
			if (xmlElement31 != null)
			{
				Logger.Log("LoadCity", "Old signals construction found!");
				сигнальныеСистемы = new Сигнальная_система[xmlElement31.ChildNodes.Count];
				for (int num18 = 0; num18 < xmlElement31.ChildNodes.Count; num18++)
				{
					XmlElement xmlElement32 = xmlElement31["signal" + num18];
					сигнальныеСистемы[num18] = new Сигнальная_система((int)Xml.GetDouble(xmlElement32["bound"]), (int)Xml.GetDouble(xmlElement32["status"]));
					XmlElement xmlElement33 = xmlElement32["elements"];
					for (int num19 = 0; num19 < xmlElement33.ChildNodes.Count; num19++)
					{
						XmlElement xmlElement34 = xmlElement33["element" + num19];
						Road road = ВсеДороги[(int)Xml.GetDouble(xmlElement34["rail"])];
						double double2 = Xml.GetDouble(xmlElement34["distance"]);
						switch (xmlElement34["type"].InnerText)
						{
						case "Контакт":
						{
							Сигнальная_система.Контакт элемент = new Сигнальная_система.Контакт(сигнальныеСистемы[num18], road, double2, Xml.GetDouble(xmlElement34["minus"]) != 0.0);
							сигнальныеСистемы[num18].Добавить_элемент(элемент);
							break;
						}
						case "Сигнал":
						{
							Visual_Signal visual_Signal = new Visual_Signal(сигнальныеСистемы[num18], Xml.GetString(xmlElement34["model"], "Signal"));
							сигнальныеСистемы[num18].Добавить_сигнал(visual_Signal);
							visual_Signal.road = road;
							visual_Signal.положение.расстояние = double2;
							visual_Signal.положение.отклонение = Xml.GetDouble(xmlElement34["place"], -1.4 - road.НайтиШирину(double2) / 2.0);
							visual_Signal.положение.высота = Xml.GetDouble(xmlElement34["height"]);
							visual_Signal.CreateBoundingSphere();
							break;
						}
						}
					}
				}
			}
			else
			{
				XmlElement xmlElement35 = xmlElement["Signal_systems"];
				сигнальныеСистемы = new Сигнальная_система[xmlElement35.ChildNodes.Count];
				for (int num20 = 0; num20 < xmlElement35.ChildNodes.Count; num20++)
				{
					XmlElement xmlElement36 = xmlElement35["signal_system" + num20];
					сигнальныеСистемы[num20] = new Сигнальная_система((int)Xml.GetDouble(xmlElement36["bound"]), (int)Xml.GetDouble(xmlElement36["status"]));
					XmlElement xmlElement37 = xmlElement36["signals"];
					for (int num21 = 0; num21 < xmlElement37.ChildNodes.Count; num21++)
					{
						XmlElement xmlElement38 = xmlElement37["signal" + num21];
						string @string = Xml.GetString(xmlElement38["model"]);
						Road road2 = ВсеДороги[(int)Xml.GetDouble(xmlElement38["path"])];
						double double3 = Xml.GetDouble(xmlElement38["distance"]);
						double double4 = Xml.GetDouble(xmlElement38["place"]);
						Visual_Signal visual_Signal2 = new Visual_Signal(сигнальныеСистемы[num20], @string);
						сигнальныеСистемы[num20].Добавить_сигнал(visual_Signal2);
						visual_Signal2.road = road2;
						visual_Signal2.положение.расстояние = double3;
						visual_Signal2.положение.отклонение = double4;
						visual_Signal2.положение.высота = Xml.GetDouble(xmlElement38["height"]);
						visual_Signal2.CreateBoundingSphere();
					}
					XmlElement xmlElement39 = xmlElement36["contacts"];
					for (int num22 = 0; num22 < xmlElement39.ChildNodes.Count; num22++)
					{
						XmlElement xmlElement40 = xmlElement39["element" + num22];
						Сигнальная_система.Контакт элемент2 = new Сигнальная_система.Контакт(сигнальныеСистемы[num20], ВсеДороги[(int)Xml.GetDouble(xmlElement40["rail"])], Xml.GetDouble(xmlElement40["distance"]), Xml.GetDouble(xmlElement40["minus"]) != 0.0);
						сигнальныеСистемы[num20].Добавить_элемент(элемент2);
					}
				}
			}
			XmlElement xmlElement41 = xmlElement["Svetofor_systems"];
			if (xmlElement41 != null)
			{
				светофорныеСистемы = new Светофорная_система[xmlElement41.ChildNodes.Count];
				for (int num23 = 0; num23 < xmlElement41.ChildNodes.Count; num23++)
				{
					XmlElement xmlElement42 = xmlElement41["svetofor_system" + num23];
					светофорныеСистемы[num23] = new Светофорная_система();
					светофорныеСистемы[num23].начало_работы = Xml.GetDouble(xmlElement42["begin"]);
					светофорныеСистемы[num23].окончание_работы = Xml.GetDouble(xmlElement42["end"]);
					светофорныеСистемы[num23].цикл = Xml.GetDouble(xmlElement42["cycle"]);
					светофорныеСистемы[num23].время_переключения_на_зелёный = Xml.GetDouble(xmlElement42["time_to_green"]);
					светофорныеСистемы[num23].время_зелёного = Xml.GetDouble(xmlElement42["time_of_green"]);
					XmlElement xmlElement43 = xmlElement42["svetofors"];
					for (int num24 = 0; num24 < xmlElement43.ChildNodes.Count; num24++)
					{
						XmlElement xmlElement44 = xmlElement43["svetofor" + num24];
						bool flag2 = Xml.GetDouble(xmlElement44["arrow"]) != 0.0;
						светофорныеСистемы[num23].светофоры.Add(new Светофор(Xml.GetString(xmlElement44["model"], flag2 ? "Tr. light (arrow)" : "Tr. light")));
						светофорныеСистемы[num23].светофоры[num24].положение.Дорога = ВсеДороги[(int)Xml.GetDouble(xmlElement44["rail"])];
						светофорныеСистемы[num23].светофоры[num24].положение.расстояние = Xml.GetDouble(xmlElement44["distance"]);
						светофорныеСистемы[num23].светофоры[num24].положение.отклонение = Xml.GetDouble(xmlElement44["place"]);
						светофорныеСистемы[num23].светофоры[num24].положение.высота = Xml.GetDouble(xmlElement44["height"]);
						светофорныеСистемы[num23].светофоры[num24].зелёная_стрелка = (int)Xml.GetDouble(xmlElement44["arrow_green"]);
						светофорныеСистемы[num23].светофоры[num24].жёлтая_стрелка = (int)Xml.GetDouble(xmlElement44["arrow_yellow"]);
						светофорныеСистемы[num23].светофоры[num24].красная_стрелка = (int)Xml.GetDouble(xmlElement44["arrow_red"]);
						светофорныеСистемы[num23].светофоры[num24].CreateBoundingSphere();
					}
					XmlElement xmlElement45 = xmlElement42["svetofor_signals"];
					for (int num25 = 0; num25 < xmlElement45.ChildNodes.Count; num25++)
					{
						XmlElement xmlElement46 = xmlElement45["svetofor_signal" + num25];
						Road дорога = ВсеДороги[(int)Xml.GetDouble(xmlElement46["rail"])];
						double double5 = Xml.GetDouble(xmlElement46["distance"]);
						светофорныеСистемы[num23].светофорные_сигналы.Add(new Светофорный_сигнал(дорога, double5));
					}
				}
			}
			XmlElement xmlElement47 = xmlElement["Objects"];
			if (xmlElement47 != null)
			{
				for (int num26 = 0; num26 < xmlElement47.ChildNodes.Count; num26++)
				{
					XmlElement xmlElement48 = xmlElement47["object" + num26];
					объекты.Add(new Объект(xmlElement48["filename"].InnerText, Xml.GetDouble(xmlElement48["x0"]), Xml.GetDouble(xmlElement48["y0"]), Xml.GetDouble(xmlElement48["angle0"]), Xml.GetDouble(xmlElement48["height0"])));
				}
			}
			Logger.Log("LoadCity", "Success!");
			this.filename = filename;
		}

		public void LoadCitySimple(string filename)
		{
			if (filename == string.Empty)
			{
				return;
			}
			Logger.Log("LoadCitySimple", $"Loading {filename}");
			XmlElement xmlElement = Xml.TryOpenDocument(filename)["City"];
			listДороги.Clear();
			if (xmlElement == null)
			{
				return;
			}
			XmlElement xmlElement2 = xmlElement["Rails"];
			if (xmlElement2 != null)
			{
				for (int i = 0; i < xmlElement2.ChildNodes.Count; i++)
				{
					listДороги.Add(new Рельс(0.0, 0.0, 0.0, 0.0, 0.0, 0.0));
				}
			}
			XmlElement xmlElement3 = xmlElement["Roads"];
			if (xmlElement3 != null)
			{
				for (int j = 0; j < xmlElement3.ChildNodes.Count; j++)
				{
					listДороги.Add(new Road(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0));
				}
			}
			XmlElement xmlElement4 = xmlElement["Parks"];
			if (xmlElement4 != null)
			{
				парки = new Парк[xmlElement4.ChildNodes.Count];
				for (int k = 0; k < xmlElement4.ChildNodes.Count; k++)
				{
					XmlElement xmlElement5 = xmlElement4["park" + k];
					парки[k] = new Парк(xmlElement5["name"].InnerText);
					int num = (int)Xml.GetDouble(xmlElement5["out"]);
					if (num >= 0)
					{
						парки[k].выезд = ВсеДороги[num];
					}
				}
			}
			XmlElement xmlElement6 = xmlElement["Routes"];
			if (xmlElement6 != null)
			{
				маршруты = new Route[xmlElement6.ChildNodes.Count];
				for (int l = 0; l < xmlElement6.ChildNodes.Count; l++)
				{
					XmlElement xmlElement7 = null;
					XmlElement xmlElement8 = xmlElement6[$"route{l}"];
					if (xmlElement8 != null)
					{
						маршруты[l] = new Route(0, xmlElement8["name"].InnerText);
						if (xmlElement8["type"] != null)
						{
							маршруты[l].typeOfTransport = (int)Xml.GetDouble(xmlElement8["type"]);
						}
						xmlElement7 = xmlElement8["route_runs"];
					}
					if (xmlElement7 != null)
					{
						for (int m = 0; m < xmlElement7.ChildNodes.Count; m++)
						{
							XmlElement xmlElement9 = xmlElement7[$"run{m}"];
							if (xmlElement9 == null)
							{
								continue;
							}
							Trip trip = new Trip
							{
								route = маршруты[l],
								время_прибытия = Xml.GetDouble(xmlElement9["time"])
							};
							XmlElement xmlElement10 = xmlElement9["run_rails"];
							if (xmlElement10 != null)
							{
								trip.pathes = new Road[Math.Min(xmlElement10.ChildNodes.Count, 1)];
								for (int n = 0; n < trip.pathes.Length; n++)
								{
									trip.pathes[n] = ВсеДороги[(int)Xml.GetDouble(xmlElement10["run_rail" + n])];
								}
							}
							маршруты[l].trips.Add(trip);
						}
					}
					if (xmlElement8 != null)
					{
						XmlElement xmlElement11 = xmlElement8["park_runs"];
						if (xmlElement11 != null)
						{
							for (int num2 = 0; num2 < xmlElement11.ChildNodes.Count; num2++)
							{
								XmlElement xmlElement12 = xmlElement11["run" + num2];
								if (xmlElement12 == null)
								{
									continue;
								}
								Trip trip2 = new Trip
								{
									route = маршруты[l],
									inPark = (Xml.GetDouble(xmlElement12["to_park"]) != 0.0),
									inParkIndex = (int)Xml.GetDouble(xmlElement12["to_park_index"]),
									время_прибытия = Xml.GetDouble(xmlElement12["time"])
								};
								XmlElement xmlElement13 = xmlElement12["run_rails"];
								if (xmlElement13 != null)
								{
									trip2.pathes = new Road[Math.Min(xmlElement13.ChildNodes.Count, 1)];
									for (int num3 = 0; num3 < trip2.pathes.Length; num3++)
									{
										trip2.pathes[num3] = ВсеДороги[(int)Xml.GetDouble(xmlElement13["run_rail" + num3])];
									}
								}
								маршруты[l].parkTrips.Add(trip2);
							}
						}
					}
					if (xmlElement8 == null)
					{
						continue;
					}
					XmlElement xmlElement14 = xmlElement8["Narads"];
					if (xmlElement14 == null)
					{
						continue;
					}
					маршруты[l].orders = new Order[xmlElement14.ChildNodes.Count];
					XmlElement xmlElement15 = null;
					for (int num4 = 0; num4 < xmlElement14.ChildNodes.Count; num4++)
					{
						XmlElement xmlElement16 = xmlElement14[$"narad{num4}"];
						if (xmlElement16 != null)
						{
							маршруты[l].orders[num4] = new Order(парки[(int)Xml.GetDouble(xmlElement16["park"])], маршруты[l], xmlElement16["name"].InnerText, Xml.GetString(xmlElement16["transport"]))
							{
								поРабочим = (Xml.GetDouble(xmlElement16["po_rabochim"]) != 0.0),
								поВыходным = (Xml.GetDouble(xmlElement16["po_vihodnim"]) != 0.0)
							};
							xmlElement15 = xmlElement16["runs"];
						}
						if (xmlElement15 == null)
						{
							continue;
						}
						маршруты[l].orders[num4].рейсы = new Trip[xmlElement15.ChildNodes.Count];
						for (int num5 = 0; num5 < xmlElement15.ChildNodes.Count; num5++)
						{
							XmlElement xmlElement17 = xmlElement15[$"run{num5}"];
							if (xmlElement17 != null)
							{
								int index = (int)Xml.GetDouble(xmlElement17["index"]);
								double @double = Xml.GetDouble(xmlElement17["time"]);
								маршруты[l].orders[num4].рейсы[num5] = ((Xml.GetDouble(xmlElement17["park"]) == 0.0) ? маршруты[l].trips[index].Clone(@double) : маршруты[l].parkTrips[index].Clone(@double));
							}
						}
					}
				}
			}
			Logger.Log("LoadCitySimple", "Success!");
			this.filename = filename;
		}

		public Положение Найти_ближайшее_положение(DoublePoint pos)
		{
			return Найти_ближайшее_положение(pos, ВсеДороги);
		}

		public Положение Найти_ближайшее_положение(DoublePoint pos, Road[] нужные_дороги)
		{
			List<Положение> list = new List<Положение>();
			List<double> list2 = new List<double>();
			DoublePoint point = default(DoublePoint);
			DoublePoint point2 = default(DoublePoint);
			foreach (Road road in нужные_дороги)
			{
				if (road.кривая)
				{
					pos.CopyTo(ref point);
					point.Subtract(ref road.структура.центр0);
					road.концы[0].CopyTo(ref point2);
					point2.Subtract(ref road.структура.центр0);
					double num = ((double)Math.Sign(road.структура.угол0) * (point.Angle - point2.Angle) + Math.PI * 4.0) % (Math.PI * 2.0);
					if (num < Math.Abs(road.структура.угол0))
					{
						list.Add(new Положение(road, road.структура.длина0 * num / Math.Abs(road.структура.угол0), (double)(-Math.Sign(road.структура.угол0)) * (point.Modulus - road.АбсолютныйРадиус)));
						list2.Add(Math.Abs(point.Modulus - road.АбсолютныйРадиус));
					}
					pos.CopyTo(ref point);
					point.Subtract(ref road.структура.центр1);
					road.структура.серединка.CopyTo(ref point2);
					point2.Subtract(ref road.структура.центр1);
					num = ((double)Math.Sign(road.структура.угол1) * (point.Angle - point2.Angle) + Math.PI * 4.0) % (Math.PI * 2.0);
					if (num < Math.Abs(road.структура.угол1))
					{
						list.Add(new Положение(road, road.структура.длина0 + road.структура.длина1 * num / Math.Abs(road.структура.угол1), (double)(-Math.Sign(road.структура.угол1)) * (point.Modulus - road.АбсолютныйРадиус)));
						list2.Add(Math.Abs(point.Modulus - road.АбсолютныйРадиус));
					}
				}
				else
				{
					pos.CopyTo(ref point);
					point.Subtract(ref road.концы[0]);
					point.Angle -= road.направления[0];
					road.концы[1].CopyTo(ref point2);
					point2.Subtract(ref road.концы[0]);
					point2.Angle -= road.направления[0];
					if (point.x >= 0.0 && point.x < point2.x)
					{
						point.y -= point2.y * point.x / point2.x;
						point.x *= road.Длина / point2.x;
						list.Add(new Положение(road, point.x, point.y));
						list2.Add(Math.Abs(point.y));
					}
				}
			}
			double num2 = 4.0;
			Положение result = default(Положение);
			for (int j = 0; j < list.Count; j++)
			{
				if (list2[j] < num2)
				{
					num2 = list2[j];
					result = list[j];
				}
			}
			return result;
		}

		public Положение[] Найти_все_положения(params Double3DPoint[] pos)
		{
			Double3DPoint point = default(Double3DPoint);
			DoublePoint point2 = default(DoublePoint);
			List<Положение> list = new List<Положение>();
			double num = 0.0;
			for (int i = 0; i < pos.Length; i++)
			{
				for (int j = i + 1; j < pos.Length; j++)
				{
					pos[i].CopyTo(ref point);
					point.Subtract(ref pos[j]);
					num = Math.Max(num, point.Modulus);
				}
			}
			Road[] всеДороги = ВсеДороги;
			foreach (Road road in всеДороги)
			{
				double num2 = road.Длина + Math.Max(road.ширина[0], road.ширина[1]);
				pos[0].XZPoint.CopyTo(ref point2);
				point2.Subtract(ref road.концы[0]);
				if (!(point2.Modulus <= num2 + num))
				{
					continue;
				}
				pos[0].XZPoint.CopyTo(ref point2);
				point2.Subtract(ref road.концы[1]);
				if (!(point2.Modulus <= num2 + num))
				{
					continue;
				}
				for (int l = 0; l < pos.Length; l++)
				{
					Положение item = Найти_положение(pos[l], road);
					if (item.Дорога != null)
					{
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}

		public int Найти_индекс(Road дорога)
		{
			return listДороги.IndexOf(дорога);
		}

		public int Найти_индекс(Парк парк)
		{
			for (int i = 0; i < парки.Length; i++)
			{
				if (парк == парки[i])
				{
					return i;
				}
			}
			return -1;
		}

		public int Найти_индекс(Trip рейс, Route маршрут, ref bool парковый)
		{
			for (int i = 0; i < маршрут.trips.Count; i++)
			{
				if (рейс.pathes == маршрут.trips[i].pathes)
				{
					парковый = false;
					return i;
				}
			}
			for (int j = 0; j < маршрут.parkTrips.Count; j++)
			{
				if (рейс.pathes == маршрут.parkTrips[j].pathes)
				{
					парковый = true;
					return j;
				}
			}
			return -1;
		}

		public int Найти_индекс_для_сохранения(Road дорога)
		{
			List<Road> list = new List<Road>(Рельсы);
			list.AddRange(Дороги);
			return list.IndexOf(дорога);
		}

		public Положение Найти_положение(Double3DPoint pos, Road дорога)
		{
			DoublePoint point = default(DoublePoint);
			DoublePoint point2 = default(DoublePoint);
			DoublePoint left = pos.XZPoint;
			double num = дорога.Длина + Math.Max(дорога.ширина[0], дорога.ширина[1]) / 2.0;
			if (DoublePoint.Distance(ref left, ref дорога.концы[0]) <= num && DoublePoint.Distance(ref left, ref дорога.концы[1]) <= num)
			{
				if (дорога.кривая)
				{
					left.CopyTo(ref point);
					point.Subtract(ref дорога.структура.центр0);
					дорога.концы[0].CopyTo(ref point2);
					point2.Subtract(ref дорога.структура.центр0);
					double num2 = ((double)Math.Sign(дорога.структура.угол0) * (point.Angle - point2.Angle) + Math.PI * 4.0) % (Math.PI * 2.0);
					if (num2 < Math.Abs(дорога.структура.угол0))
					{
						double расстояние = дорога.структура.длина0 * num2 / Math.Abs(дорога.структура.угол0);
						double num3 = pos.y - дорога.НайтиВысоту(расстояние);
						if (Math.Abs(point.Modulus - дорога.АбсолютныйРадиус) < дорога.НайтиШирину(расстояние) / 2.0 && num3 >= -1.0 && num3 < 5.0)
						{
							return new Положение(дорога, расстояние, (double)(-Math.Sign(дорога.структура.угол0)) * (point.Modulus - дорога.АбсолютныйРадиус), num3);
						}
					}
					left.CopyTo(ref point);
					point.Subtract(ref дорога.структура.центр1);
					дорога.структура.серединка.CopyTo(ref point2);
					point2.Subtract(ref дорога.структура.центр1);
					num2 = ((double)Math.Sign(дорога.структура.угол1) * (point.Angle - point2.Angle) + Math.PI * 4.0) % (Math.PI * 2.0);
					if (num2 < Math.Abs(дорога.структура.угол1))
					{
						double расстояние2 = дорога.структура.длина0 + дорога.структура.длина1 * num2 / Math.Abs(дорога.структура.угол1);
						double num4 = pos.y - дорога.НайтиВысоту(расстояние2);
						if (Math.Abs(point.Modulus - дорога.АбсолютныйРадиус) < дорога.НайтиШирину(расстояние2) / 2.0 && num4 >= -1.0 && num4 < 5.0)
						{
							return new Положение(дорога, расстояние2, (double)(-Math.Sign(дорога.структура.угол1)) * (point.Modulus - дорога.АбсолютныйРадиус), num4);
						}
					}
				}
				else
				{
					left.CopyTo(ref point);
					point.Subtract(ref дорога.концы[0]);
					point.Angle -= дорога.направления[0];
					дорога.концы[1].CopyTo(ref point2);
					point2.Subtract(ref дорога.концы[0]);
					point2.Angle -= дорога.направления[0];
					if (point.x >= 0.0 && point.x < point2.x)
					{
						point.y -= point2.y * point.x / point2.x;
						point.x *= дорога.Длина / point2.x;
						double num5 = pos.y - дорога.НайтиВысоту(point.x);
						if (Math.Abs(point.y) < дорога.НайтиШирину(point.x) / 2.0 && num5 >= -1.0 && num5 < 5.0)
						{
							return new Положение(дорога, point.x, point.y, num5);
						}
					}
				}
				return default(Положение);
			}
			return default(Положение);
		}

		public Положение Найти_положение(DoublePoint pos, Road дорога)
		{
			return Найти_положение(new Double3DPoint(pos.x, 0.0, pos.y), дорога);
		}

		public void Найти_положение_наряда(Order наряд, ref Trip рейс, ref Road дорога, ref double расстояние_по_дороге, ref bool from_park)
		{
			for (int i = 0; i < наряд.рейсы.Length; i++)
			{
				if (time < наряд.рейсы[i].время_прибытия)
				{
					рейс = наряд.рейсы[i];
					if (time < наряд.рейсы[i].время_отправления)
					{
						if (наряд.рейсы[i].дорога_отправления == наряд.парк.выезд)
						{
							from_park = true;
							дорога = наряд.парк.выезд;
							расстояние_по_дороге = Cheats._random.NextDouble() * Math.Min(Math.Abs(дорога.Длина - 20.0), дорога.Длина);
						}
						else
						{
							дорога = наряд.рейсы[i].дорога_отправления;
							расстояние_по_дороге = Cheats._random.NextDouble() * (дорога.Длина * 0.4);
						}
						break;
					}
					double num = рейс.длина_пути * (time - наряд.рейсы[i].время_отправления) / (наряд.рейсы[i].время_прибытия - наряд.рейсы[i].время_отправления);
					Road[] pathes = рейс.pathes;
					foreach (Road road in pathes)
					{
						if (num < road.Длина)
						{
							дорога = road;
							расстояние_по_дороге = num;
							if (road is Рельс && ((Рельс)road).следующие_рельсы.Length > 1 && расстояние_по_дороге > road.Длина - ((Рельс)road).расстояние_добавочных_проводов)
							{
								расстояние_по_дороге -= ((Рельс)road).расстояние_добавочных_проводов + Cheats._random.NextDouble() * 5.0;
							}
							break;
						}
						num -= road.Длина;
					}
					break;
				}
				if (i == наряд.рейсы.Length - 1)
				{
					рейс = наряд.рейсы[i];
					дорога = наряд.рейсы[i].дорога_прибытия;
					расстояние_по_дороге = Cheats._random.NextDouble() * (дорога.Длина * 0.4);
				}
			}
		}

		public void Обновить(Игрок[] игроки)
		{
			Обновить_время();
			time += прошлоВремени;
			if (time >= 97200.0)
			{
				time -= 86400.0;
			}
			Светофорная_система[] array = светофорныеСистемы;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Обновить(this);
			}
			foreach (Transport item in транспорты)
			{
				item.Обновить(this, игроки);
			}
		}

		public void Обновить_время()
		{
			double num = (double)Environment.TickCount / time_speed;
			if (системноеВремя == 0.0)
			{
				системноеВремя = num;
			}
			прошлоВремени = num - системноеВремя;
			dtmax = Math.Max(dtmax, прошлоВремени);
			if (!MainForm.in_editor)
			{
				прошлоВремени = Math.Min(прошлоВремени, 0.25);
			}
			системноеВремя = num;
		}

		public void Сохранить_город(string filename)
		{
			Logger.Log("SaveCity", "Trying to save current city...");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement parent = Xml.AddElement(xmlDocument, "City");
			XmlElement parent2 = Xml.AddElement(xmlDocument, parent, "Rails");
			for (int i = 0; i < Рельсы.Length; i++)
			{
				XmlElement parent3 = Xml.AddElement(xmlDocument, parent2, "rail" + i);
				Xml.AddElement(xmlDocument, parent3, "x0", Рельсы[i].концы[0].x);
				Xml.AddElement(xmlDocument, parent3, "y0", Рельсы[i].концы[0].y);
				Xml.AddElement(xmlDocument, parent3, "x1", Рельсы[i].концы[1].x);
				Xml.AddElement(xmlDocument, parent3, "y1", Рельсы[i].концы[1].y);
				Xml.AddElement(xmlDocument, parent3, "angle0", Рельсы[i].направления[0]);
				Xml.AddElement(xmlDocument, parent3, "angle1", Рельсы[i].направления[1]);
				Xml.AddElement(xmlDocument, parent3, "height0", Рельсы[i].высота[0]);
				Xml.AddElement(xmlDocument, parent3, "height1", Рельсы[i].высота[1]);
				Xml.AddElement(xmlDocument, parent3, "d_strel", Рельсы[i].расстояние_добавочных_проводов);
				Xml.AddElement(xmlDocument, parent3, "iskriv", Рельсы[i].кривая ? 1.0 : 0.0);
				Xml.AddElement(xmlDocument, parent3, "name", Рельсы[i].name);
			}
			XmlElement parent4 = Xml.AddElement(xmlDocument, parent, "Roads");
			for (int j = 0; j < Дороги.Length; j++)
			{
				XmlElement parent5 = Xml.AddElement(xmlDocument, parent4, "road" + j);
				Xml.AddElement(xmlDocument, parent5, "x0", Дороги[j].концы[0].x);
				Xml.AddElement(xmlDocument, parent5, "y0", Дороги[j].концы[0].y);
				Xml.AddElement(xmlDocument, parent5, "x1", Дороги[j].концы[1].x);
				Xml.AddElement(xmlDocument, parent5, "y1", Дороги[j].концы[1].y);
				Xml.AddElement(xmlDocument, parent5, "angle0", Дороги[j].направления[0]);
				Xml.AddElement(xmlDocument, parent5, "angle1", Дороги[j].направления[1]);
				Xml.AddElement(xmlDocument, parent5, "wide0", Дороги[j].ширина[0]);
				Xml.AddElement(xmlDocument, parent5, "wide1", Дороги[j].ширина[1]);
				Xml.AddElement(xmlDocument, parent5, "height0", Дороги[j].высота[0]);
				Xml.AddElement(xmlDocument, parent5, "height1", Дороги[j].высота[1]);
				Xml.AddElement(xmlDocument, parent5, "iskriv", Дороги[j].кривая ? 1.0 : 0.0);
				Xml.AddElement(xmlDocument, parent5, "name", Дороги[j].name);
			}
			XmlElement parent6 = Xml.AddElement(xmlDocument, parent, "Trolleybus_lines");
			for (int k = 0; k < контактныеПровода.Length; k++)
			{
				XmlElement parent7 = Xml.AddElement(xmlDocument, parent6, "line" + k);
				Xml.AddElement(xmlDocument, parent7, "x0", контактныеПровода[k].начало.x);
				Xml.AddElement(xmlDocument, parent7, "y0", контактныеПровода[k].начало.y);
				Xml.AddElement(xmlDocument, parent7, "x1", контактныеПровода[k].конец.x);
				Xml.AddElement(xmlDocument, parent7, "y1", контактныеПровода[k].конец.y);
				Xml.AddElement(xmlDocument, parent7, "height0", контактныеПровода[k].высота[0]);
				Xml.AddElement(xmlDocument, parent7, "height1", контактныеПровода[k].высота[1]);
				Xml.AddElement(xmlDocument, parent7, "right", контактныеПровода[k].правый ? 1.0 : 0.0);
				Xml.AddElement(xmlDocument, parent7, "no_contact", контактныеПровода[k].обесточенный ? 1.0 : 0.0);
			}
			XmlElement parent8 = Xml.AddElement(xmlDocument, parent, "Tramway_lines");
			for (int l = 0; l < контактныеПровода2.Length; l++)
			{
				XmlElement parent9 = Xml.AddElement(xmlDocument, parent8, "line" + l);
				Xml.AddElement(xmlDocument, parent9, "x0", контактныеПровода2[l].начало.x);
				Xml.AddElement(xmlDocument, parent9, "y0", контактныеПровода2[l].начало.y);
				Xml.AddElement(xmlDocument, parent9, "x1", контактныеПровода2[l].конец.x);
				Xml.AddElement(xmlDocument, parent9, "y1", контактныеПровода2[l].конец.y);
				Xml.AddElement(xmlDocument, parent9, "height0", контактныеПровода2[l].высота[0]);
				Xml.AddElement(xmlDocument, parent9, "height1", контактныеПровода2[l].высота[1]);
				Xml.AddElement(xmlDocument, parent9, "no_contact", контактныеПровода2[l].обесточенный ? 1.0 : 0.0);
			}
			XmlElement parent10 = Xml.AddElement(xmlDocument, parent, "Parks");
			for (int m = 0; m < парки.Length; m++)
			{
				XmlElement parent11 = Xml.AddElement(xmlDocument, parent10, "park" + m);
				Xml.AddElement(xmlDocument, parent11, "name", парки[m].название);
				Xml.AddElement(xmlDocument, parent11, "in", Найти_индекс_для_сохранения(парки[m].въезд));
				Xml.AddElement(xmlDocument, parent11, "out", Найти_индекс_для_сохранения(парки[m].выезд));
				XmlElement parent12 = Xml.AddElement(xmlDocument, parent11, "park_rails");
				int num = 0;
				int num2 = 0;
				while (num < парки[m].пути_стоянки.Length)
				{
					int num3 = Найти_индекс_для_сохранения(парки[m].пути_стоянки[num]);
					if (num3 < 0)
					{
						num2--;
					}
					else
					{
						Xml.AddElement(xmlDocument, parent12, "park_rail" + num2, num3);
					}
					num++;
					num2++;
				}
			}
			XmlElement parent13 = Xml.AddElement(xmlDocument, parent, "Routes");
			for (int n = 0; n < маршруты.Length; n++)
			{
				XmlElement parent14 = Xml.AddElement(xmlDocument, parent13, "route" + n);
				Xml.AddElement(xmlDocument, parent14, "name", маршруты[n].number);
				Xml.AddElement(xmlDocument, parent14, "type", маршруты[n].typeOfTransport);
				XmlElement parent15 = Xml.AddElement(xmlDocument, parent14, "route_runs");
				for (int num4 = 0; num4 < маршруты[n].trips.Count; num4++)
				{
					XmlElement parent16 = Xml.AddElement(xmlDocument, parent15, "run" + num4);
					Xml.AddElement(xmlDocument, parent16, "time", маршруты[n].trips[num4].время_прибытия);
					XmlElement parent17 = Xml.AddElement(xmlDocument, parent16, "run_rails");
					for (int num5 = 0; num5 < маршруты[n].trips[num4].pathes.Length; num5++)
					{
						int num6 = Найти_индекс_для_сохранения(маршруты[n].trips[num4].pathes[num5]);
						if (num6 < 0)
						{
							throw new IndexOutOfRangeException("Маршрут " + маршруты[n].number + " (рейс " + num4 + ") проходит по несуществующему пути!");
						}
						Xml.AddElement(xmlDocument, parent17, "run_rail" + num5, num6);
					}
					XmlElement parent18 = Xml.AddElement(xmlDocument, parent16, "Stops");
					if (маршруты[n].trips[num4].tripStopList == null || маршруты[n].trips[num4].tripStopList.Count <= 0)
					{
						continue;
					}
					for (int num7 = 0; num7 < маршруты[n].trips[num4].tripStopList.Count; num7++)
					{
						TripStop tripStop = маршруты[n].trips[num4].tripStopList[num7];
						if (tripStop.stop.typeOfTransport[маршруты[n].typeOfTransport])
						{
							int num8 = остановки.IndexOf(tripStop.stop);
							if (num8 >= 0)
							{
								Xml.AddElement(xmlDocument, parent18, "Stop" + num8, tripStop.active ? 1.0 : 0.0);
							}
						}
					}
				}
				XmlElement parent19 = Xml.AddElement(xmlDocument, parent14, "park_runs");
				for (int num9 = 0; num9 < маршруты[n].parkTrips.Count; num9++)
				{
					XmlElement parent20 = Xml.AddElement(xmlDocument, parent19, "run" + num9);
					Xml.AddElement(xmlDocument, parent20, "to_park", маршруты[n].parkTrips[num9].inPark ? 1.0 : 0.0);
					Xml.AddElement(xmlDocument, parent20, "to_park_index", маршруты[n].parkTrips[num9].inParkIndex);
					Xml.AddElement(xmlDocument, parent20, "time", маршруты[n].parkTrips[num9].время_прибытия);
					XmlElement parent21 = Xml.AddElement(xmlDocument, parent20, "run_rails");
					for (int num10 = 0; num10 < маршруты[n].parkTrips[num9].pathes.Length; num10++)
					{
						int num11 = Найти_индекс_для_сохранения(маршруты[n].parkTrips[num9].pathes[num10]);
						if (num11 < 0)
						{
							throw new IndexOutOfRangeException("Маршрут " + маршруты[n].number + " (парковый рейс " + num9 + ") проходит по несуществующему пути!");
						}
						Xml.AddElement(xmlDocument, parent21, "run_rail" + num10, num11);
					}
					XmlElement parent22 = Xml.AddElement(xmlDocument, parent20, "Stops");
					if (маршруты[n].parkTrips[num9].tripStopList == null || маршруты[n].parkTrips[num9].tripStopList.Count <= 0)
					{
						continue;
					}
					for (int num12 = 0; num12 < маршруты[n].parkTrips[num9].tripStopList.Count; num12++)
					{
						TripStop tripStop2 = маршруты[n].parkTrips[num9].tripStopList[num12];
						if (tripStop2.stop.typeOfTransport[маршруты[n].typeOfTransport])
						{
							int num13 = остановки.IndexOf(tripStop2.stop);
							if (num13 >= 0)
							{
								Xml.AddElement(xmlDocument, parent22, "Stop" + num13, tripStop2.active ? 1.0 : 0.0);
							}
						}
					}
				}
				XmlElement parent23 = Xml.AddElement(xmlDocument, parent14, "Narads");
				for (int num14 = 0; num14 < маршруты[n].orders.Length; num14++)
				{
					XmlElement parent24 = Xml.AddElement(xmlDocument, parent23, "narad" + num14);
					Xml.AddElement(xmlDocument, parent24, "name", маршруты[n].orders[num14].номер);
					int num15 = Найти_индекс(маршруты[n].orders[num14].парк);
					if (num15 < 0)
					{
						throw new IndexOutOfRangeException("В наряде " + маршруты[n].number + "/" + маршруты[n].orders[num14].номер + " не указан парк!");
					}
					Xml.AddElement(xmlDocument, parent24, "park", num15);
					Xml.AddElement(xmlDocument, parent24, "transport", маршруты[n].orders[num14].transport);
					Xml.AddElement(xmlDocument, parent24, "po_rabochim", маршруты[n].orders[num14].поРабочим ? 1.0 : 0.0);
					Xml.AddElement(xmlDocument, parent24, "po_vihodnim", маршруты[n].orders[num14].поВыходным ? 1.0 : 0.0);
					XmlElement parent25 = Xml.AddElement(xmlDocument, parent24, "runs");
					for (int num16 = 0; num16 < маршруты[n].orders[num14].рейсы.Length; num16++)
					{
						Trip рейс = маршруты[n].orders[num14].рейсы[num16];
						XmlElement parent26 = Xml.AddElement(xmlDocument, parent25, "run" + num16);
						bool парковый = false;
						int num17 = Найти_индекс(рейс, маршруты[n], ref парковый);
						if (num17 < 0)
						{
							num17 = 0;
						}
						Xml.AddElement(xmlDocument, parent26, "park", парковый ? 1.0 : 0.0);
						Xml.AddElement(xmlDocument, parent26, "index", num17);
						Xml.AddElement(xmlDocument, parent26, "time", маршруты[n].orders[num14].рейсы[num16].время_отправления);
					}
				}
			}
			XmlElement parent27 = Xml.AddElement(xmlDocument, parent, "Stops");
			for (int num18 = 0; num18 < остановки.Count; num18++)
			{
				XmlElement parent28 = Xml.AddElement(xmlDocument, parent27, "stop" + num18);
				Xml.AddElement(xmlDocument, parent28, "name", остановки[num18].название);
				Xml.AddElement(xmlDocument, parent28, "model", остановки[num18].name);
				XmlElement parent29 = Xml.AddElement(xmlDocument, parent28, "type");
				Xml.AddElement(xmlDocument, parent29, "Tramway", остановки[num18].typeOfTransport[0].ToString());
				Xml.AddElement(xmlDocument, parent29, "Trolleybus", остановки[num18].typeOfTransport[1].ToString());
				Xml.AddElement(xmlDocument, parent29, "Bus", остановки[num18].typeOfTransport[2].ToString());
				int num19 = Найти_индекс_для_сохранения(остановки[num18].road);
				if (num19 < 0)
				{
					throw new IndexOutOfRangeException("Остановка \"" + остановки[num18].название + "\" находится на несуществующем пути!");
				}
				Xml.AddElement(xmlDocument, parent28, "rail", num19);
				Xml.AddElement(xmlDocument, parent28, "distance", остановки[num18].distance);
				XmlElement parent30 = Xml.AddElement(xmlDocument, parent28, "stop_path");
				for (int num20 = 0; num20 < остановки[num18].частьПути.Length; num20++)
				{
					int num21 = Найти_индекс_для_сохранения(остановки[num18].частьПути[num20]);
					if (num21 < 0)
					{
						break;
					}
					Xml.AddElement(xmlDocument, parent30, "stop_rail" + num20, num21);
				}
			}
			XmlElement parent31 = Xml.AddElement(xmlDocument, parent, "Signals");
			for (int num22 = 0; num22 < сигнальныеСистемы.Length; num22++)
			{
				XmlElement parent32 = Xml.AddElement(xmlDocument, parent31, "signal" + num22);
				Xml.AddElement(xmlDocument, parent32, "status", сигнальныеСистемы[num22].состояние);
				Xml.AddElement(xmlDocument, parent32, "bound", сигнальныеСистемы[num22].граница_переключения);
				XmlElement parent33 = Xml.AddElement(xmlDocument, parent32, "elements");
				for (int num23 = 0; num23 < сигнальныеСистемы[num22].vsignals.Count; num23++)
				{
					XmlElement parent34 = Xml.AddElement(xmlDocument, parent33, "element" + num23);
					Xml.AddElement(xmlDocument, parent34, "type", "Сигнал");
					Xml.AddElement(xmlDocument, parent34, "model", сигнальныеСистемы[num22].vsignals[num23].name);
					int num24 = Найти_индекс_для_сохранения(сигнальныеСистемы[num22].vsignals[num23].положение.Дорога);
					if (num24 < 0)
					{
						throw new IndexOutOfRangeException("Сигнал №" + num23 + " сигнальной системы №" + num22 + " находится на несуществующем пути!");
					}
					Xml.AddElement(xmlDocument, parent34, "rail", num24);
					Xml.AddElement(xmlDocument, parent34, "distance", сигнальныеСистемы[num22].vsignals[num23].положение.расстояние);
					Xml.AddElement(xmlDocument, parent34, "place", сигнальныеСистемы[num22].vsignals[num23].положение.отклонение);
					Xml.AddElement(xmlDocument, parent34, "height", сигнальныеСистемы[num22].vsignals[num23].положение.высота);
				}
				for (int num25 = 0; num25 < сигнальныеСистемы[num22].элементы.Count; num25++)
				{
					XmlElement parent35 = Xml.AddElement(xmlDocument, parent33, "element" + (num25 + сигнальныеСистемы[num22].vsignals.Count));
					Xml.AddElement(xmlDocument, parent35, "type", сигнальныеСистемы[num22].элементы[num25].GetType().Name);
					int num26 = Найти_индекс_для_сохранения(сигнальныеСистемы[num22].элементы[num25].дорога);
					if (num26 < 0)
					{
						throw new IndexOutOfRangeException("Элемент №" + num25 + " сигнальной системы №" + num22 + " находится на несуществующем пути!");
					}
					Xml.AddElement(xmlDocument, parent35, "rail", num26);
					Xml.AddElement(xmlDocument, parent35, "distance", сигнальныеСистемы[num22].элементы[num25].расстояние);
					Xml.AddElement(xmlDocument, parent35, "minus", сигнальныеСистемы[num22].элементы[num25].минус ? 1.0 : 0.0);
				}
			}
			XmlElement parent36 = Xml.AddElement(xmlDocument, parent, "Svetofor_systems");
			for (int num27 = 0; num27 < светофорныеСистемы.Length; num27++)
			{
				XmlElement parent37 = Xml.AddElement(xmlDocument, parent36, "svetofor_system" + num27);
				Xml.AddElement(xmlDocument, parent37, "begin", светофорныеСистемы[num27].начало_работы);
				Xml.AddElement(xmlDocument, parent37, "end", светофорныеСистемы[num27].окончание_работы);
				Xml.AddElement(xmlDocument, parent37, "cycle", светофорныеСистемы[num27].цикл);
				Xml.AddElement(xmlDocument, parent37, "time_to_green", светофорныеСистемы[num27].время_переключения_на_зелёный);
				Xml.AddElement(xmlDocument, parent37, "time_of_green", светофорныеСистемы[num27].время_зелёного);
				XmlElement parent38 = Xml.AddElement(xmlDocument, parent37, "svetofors");
				for (int num28 = 0; num28 < светофорныеСистемы[num27].светофоры.Count; num28++)
				{
					XmlElement parent39 = Xml.AddElement(xmlDocument, parent38, "svetofor" + num28);
					int num29 = Найти_индекс_для_сохранения(светофорныеСистемы[num27].светофоры[num28].положение.Дорога);
					if (num29 < 0)
					{
						throw new IndexOutOfRangeException("Светофор №" + num28 + " светофорной системы №" + num27 + " находится на несуществующем пути!");
					}
					Xml.AddElement(xmlDocument, parent39, "model", светофорныеСистемы[num27].светофоры[num28].name);
					Xml.AddElement(xmlDocument, parent39, "rail", num29);
					Xml.AddElement(xmlDocument, parent39, "distance", светофорныеСистемы[num27].светофоры[num28].положение.расстояние);
					Xml.AddElement(xmlDocument, parent39, "place", светофорныеСистемы[num27].светофоры[num28].положение.отклонение);
					Xml.AddElement(xmlDocument, parent39, "height", светофорныеСистемы[num27].светофоры[num28].положение.высота);
					Xml.AddElement(xmlDocument, parent39, "arrow_green", светофорныеСистемы[num27].светофоры[num28].зелёная_стрелка);
					Xml.AddElement(xmlDocument, parent39, "arrow_yellow", светофорныеСистемы[num27].светофоры[num28].жёлтая_стрелка);
					Xml.AddElement(xmlDocument, parent39, "arrow_red", светофорныеСистемы[num27].светофоры[num28].красная_стрелка);
				}
				XmlElement parent40 = Xml.AddElement(xmlDocument, parent37, "svetofor_signals");
				for (int num30 = 0; num30 < светофорныеСистемы[num27].светофорные_сигналы.Count; num30++)
				{
					XmlElement parent41 = Xml.AddElement(xmlDocument, parent40, "svetofor_signal" + num30);
					int num31 = Найти_индекс_для_сохранения(светофорныеСистемы[num27].светофорные_сигналы[num30].дорога);
					if (num31 < 0)
					{
						throw new IndexOutOfRangeException("Сигнал №" + num30 + " светофорной системы №" + num27 + " находится на несуществующем пути!");
					}
					Xml.AddElement(xmlDocument, parent41, "rail", num31);
					Xml.AddElement(xmlDocument, parent41, "distance", светофорныеСистемы[num27].светофорные_сигналы[num30].расстояние);
				}
			}
			XmlElement parent42 = Xml.AddElement(xmlDocument, parent, "Objects");
			for (int num32 = 0; num32 < объекты.Count; num32++)
			{
				XmlElement parent43 = Xml.AddElement(xmlDocument, parent42, "object" + num32);
				Xml.AddElement(xmlDocument, parent43, "filename", объекты[num32].filename);
				Xml.AddElement(xmlDocument, parent43, "x0", объекты[num32].x0);
				Xml.AddElement(xmlDocument, parent43, "y0", объекты[num32].y0);
				Xml.AddElement(xmlDocument, parent43, "angle0", объекты[num32].angle0);
				Xml.AddElement(xmlDocument, parent43, "height0", объекты[num32].height0);
			}
			xmlDocument.Save(filename);
			stopwatch.Stop();
			Logger.Log("SaveCity", "City saved to file " + filename);
			Logger.Log("SaveCity", "Elapsed time : " + stopwatch.Elapsed.ToString());
		}

		public double GetHeight(DoublePoint pos)
		{
			return земля.GetHeight(pos);
		}
	}
}

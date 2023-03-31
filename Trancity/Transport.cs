using System;
using System.Collections.Generic;
using Engine;

namespace Trancity
{
	public abstract class Transport : IControlledObject, IVector, IОбъектПривязки3D
	{
		public enum Тип_дополнения
		{
			фары,
			влево,
			вправо,
			тормоз,
			назад
		}

		private bool faultAlarm;

		private Route route;

		private int fуказатель_поворота;

		private Управление fуправление;

		private bool nosound;

		public bool включен = true;

		public bool включены_фары;

		public List<Положение> найденные_положения = new List<Положение>();

		public Order наряд;

		public string основная_папка = "";

		public double осталось_стоять;

		public Парк парк;

		public Trip рейс;

		public int рейс_index;

		public Система_управления система_управления;

		public double скорость;

		protected bool стоим_с_закрытыми_дверями;

		public bool _soundЗамедляется;

		public bool _soundУскоряется;

		public УказательНаряда указатель_наряда;

		public ТабличкаВПарк табличка_в_парк;

		public МодельТранспорта модель;

		protected BaseStop базоваяОстановка;

		public Stop nextStop;

		public Stop currentStop;

		public int stopIndex;

		protected Двери[] _двери;

		protected int _количествоДверей = 1;

		protected double времяПоворотников;

		protected double времяПоворотниковMax = 1.0;

		protected double времяПоворотниковВыкл = 0.5;

		protected int _бывшийУказательПоворота;

		protected bool _былаАварийнаяСигнализация;

		public bool stand_brake;

		public double length0;

		public double length1;

		public double width;

		public MyCamera[] cameras = new MyCamera[0];

		public bool аварийная_сигнализация
		{
			get
			{
				return faultAlarm;
			}
			set
			{
				faultAlarm = value;
			}
		}

		public bool в_парк
		{
			get
			{
				if (рейс != null && рейс.inPark)
				{
					return рейс_index >= рейс.inParkIndex;
				}
				return false;
			}
		}

		public abstract DoublePoint position { get; }

		public abstract Double3DPoint Координаты3D { get; }

		public Route маршрут
		{
			get
			{
				return route;
			}
			protected set
			{
				if (route != value)
				{
					route = value;
					ОбновитьМаршрутныеУказатели();
				}
			}
		}

		public abstract double direction { get; }

		public abstract double НаправлениеY { get; }

		public double скорость_abs
		{
			get
			{
				return Math.Abs(скорость);
			}
			set
			{
				if (value <= 0.0)
				{
					скорость = 0.0;
					return;
				}
				if (скорость == 0.0)
				{
					скорость = value;
					return;
				}
				try
				{
					скорость = (double)Math.Sign(скорость) * value;
				}
				catch
				{
				}
			}
		}

		public abstract Положение текущее_положение { get; }

		public int указатель_поворота
		{
			get
			{
				return fуказатель_поворота;
			}
			set
			{
				fуказатель_поворота = value;
			}
		}

		public Управление управление
		{
			get
			{
				return fуправление;
			}
			set
			{
				fуправление = value;
			}
		}

		public abstract double ускорение { get; }

		public bool двери_водителя_закрыты
		{
			get
			{
				Двери[] двери = _двери;
				foreach (Двери двери2 in двери)
				{
					if (двери2.дверьВодителя && !двери2.Закрыты)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool двери_водителя_открыты
		{
			get
			{
				Двери[] двери = _двери;
				foreach (Двери двери2 in двери)
				{
					if (двери2.дверьВодителя && !двери2.Открыты)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool двери_закрыты
		{
			get
			{
				Двери[] двери = _двери;
				foreach (Двери двери2 in двери)
				{
					if (двери2.номер >= 0 && !двери2.Закрыты)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool двери_открыты
		{
			get
			{
				Двери[] двери = _двери;
				foreach (Двери двери2 in двери)
				{
					if (двери2.номер >= 0 && !двери2.Открыты)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool condition
		{
			get
			{
				if (!(Math.Abs(Math.Floor(position.x / (double)Ground.grid_size) - (double)Game.col) > 1.0))
				{
					return Math.Abs(Math.Floor(position.y / (double)Ground.grid_size) - (double)Game.row) > 1.0;
				}
				return true;
			}
		}

		public abstract void CreateMesh(World мир);

		public abstract void Render();

		public abstract void UpdateBoundigBoxes(World world);

		public abstract void АвтоматическиУправлять(World мир);

		public abstract Положение[] НайтиВсеПоложения(World мир);

		public abstract void Обновить(World мир, Игрок[] игроки);

		protected abstract void ОбновитьМаршрутныеУказатели();

		public abstract void SetPosition(Road road, double distance, double shift, Double3DPoint pos, DoublePoint rot, World world);

		protected void ОбновитьРейс()
		{
			if (рейс == null)
			{
				return;
			}
			if (рейс_index != рейс.pathes.Length - 1 || наряд == null)
			{
				while (рейс_index < рейс.pathes.Length && текущее_положение.Дорога != рейс.pathes[рейс_index])
				{
					if (рейс_index > 0 && текущее_положение.Дорога == рейс.pathes[рейс_index - 1])
					{
						рейс_index--;
					}
					else
					{
						рейс_index++;
					}
				}
				if (рейс_index == рейс.pathes.Length)
				{
					рейс_index = 0;
					for (int i = 0; i < рейс.pathes.Length; i++)
					{
						if (текущее_положение.Дорога == рейс.pathes[i])
						{
							рейс_index = i;
							break;
						}
					}
				}
			}
			if (наряд == null || рейс_index != рейс.pathes.Length - 1)
			{
				return;
			}
			for (int j = 0; j < наряд.рейсы.Length - 1; j++)
			{
				if (наряд.рейсы[j] == рейс)
				{
					рейс = наряд.рейсы[j + 1];
					nextStop = null;
					currentStop = null;
					stopIndex = 0;
					рейс_index = 0;
					break;
				}
			}
		}

		public abstract void Передвинуть(double расстояние, World мир);

		public bool ДверьЗакрыта(int номер)
		{
			Двери[] двери = _двери;
			foreach (Двери двери2 in двери)
			{
				if (двери2.номер == номер && !двери2.Закрыты)
				{
					return false;
				}
			}
			return true;
		}

		public bool ДверьОткрыта(int номер)
		{
			Двери[] двери = _двери;
			foreach (Двери двери2 in двери)
			{
				if (двери2.номер == номер && !двери2.Открыты)
				{
					return false;
				}
			}
			return true;
		}

		public void ОткрытьДвери(bool открыть)
		{
			for (int i = 0; i < _количествоДверей; i++)
			{
				ОткрытьДвери(i, открыть);
			}
		}

		public void ОткрытьДвери(int номер, bool открыть)
		{
			Двери[] двери = _двери;
			foreach (Двери двери2 in двери)
			{
				if (двери2.номер == номер)
				{
					двери2.открываются = открыть;
				}
			}
		}

		public void ОткрытьДвериВодителя(bool открыть)
		{
			Двери[] двери = _двери;
			foreach (Двери двери2 in двери)
			{
				if (двери2.дверьВодителя)
				{
					двери2.открываются = открыть;
				}
			}
		}

		protected void UpdateTripStops()
		{
			if (рейс == null || рейс.tripStopList == null || рейс.tripStopList.Count == 0)
			{
				return;
			}
			if (stopIndex < рейс.tripStopList.Count)
			{
				while (nextStop == null && stopIndex < рейс.tripStopList.Count)
				{
					if (!рейс.tripStopList[stopIndex].active)
					{
						stopIndex = Math.Min(stopIndex + 1, рейс.tripStopList.Count);
					}
					else
					{
						nextStop = рейс.tripStopList[stopIndex].stop;
					}
				}
			}
			else
			{
				stopIndex = рейс.tripStopList.Count;
				nextStop = null;
			}
		}

		protected void SearchForCurrentStop(Stop stop)
		{
			if (рейс == null || рейс.tripStopList == null || nextStop == null || stop == nextStop || stopIndex != 0)
			{
				return;
			}
			for (int i = 0; i < рейс.tripStopList.Count; i++)
			{
				if (рейс.tripStopList[i].stop == stop && рейс.tripStopList[i].active)
				{
					stopIndex = i;
					nextStop = рейс.tripStopList[i].stop;
					break;
				}
			}
		}

		public void CreateSoundBuffers()
		{
            //Создаем звуки системы управления
			система_управления.CreateSoundBuffers();
            //Создаем звуки дверей

            for (int i = 0; i < _двери.Length; i++)
            {
                _двери[i].transport = this;
                _двери[i].CreateSoundBuffers();
            }


		}

		public void UpdateSound(Игрок[] игроки, bool игра_активна)
		{
            //Если звук включен
			if (!nosound)
			{
                //Обновляем звуки системы управления
				система_управления.UpdateSound(игроки, игра_активна);
                //Обновляем звуки дверей
                foreach(Двери дверь in _двери)
                {
                    дверь.UpdateSound(игроки, игра_активна);
                }

			}
		}

		public void LoadCameras()
		{
			if (модель.cameras != null)
			{
				cameras = new MyCamera[модель.cameras.Length];
				for (int i = 0; i < cameras.Length; i++)
				{
					cameras[i] = new MyCamera();
					cameras[i].position = модель.cameras[i].pos;
					cameras[i].rotation = модель.cameras[i].rot;
				}
			}
		}

		public bool SetCamera(int index, Игрок player)
		{
			if (index >= cameras.Length)
			{
				return false;
			}
			player.cameraPositionChange = Double3DPoint.Zero;
			player.cameraRotationChange = DoublePoint.Zero;
			player.cameraPosition = Double3DPoint.Multiply(direction: new DoublePoint(direction, НаправлениеY), _point: cameras[index].position, body: Координаты3D);
			player.cameraRotation.x = direction + cameras[index].rotation.x;
			player.cameraRotation.y = НаправлениеY + cameras[index].rotation.y;
			return true;
		}

		protected abstract void CheckCondition();
	}
}

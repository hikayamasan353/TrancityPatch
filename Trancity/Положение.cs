using Engine;

namespace Trancity
{
	public struct Положение
	{
		private IObjectContainer _container;

		public double расстояние;

		public double отклонение;

		public double высота;

		public object comment;

		public Road Дорога
		{
			get
			{
				if (_container != null && _container is Road)
				{
					return (Road)_container;
				}
				return null;
			}
			set
			{
				_container = value;
			}
		}

		public Рельс Рельс
		{
			get
			{
				if (_container != null && _container is Рельс)
				{
					return (Рельс)_container;
				}
				return null;
			}
		}

		public Double3DPoint Координаты
		{
			get
			{
				if (Дорога != null)
				{
					DoublePoint doublePoint = Дорога.НайтиКоординаты(расстояние, отклонение);
					return new Double3DPoint(doublePoint.x, Дорога.НайтиВысоту(расстояние), doublePoint.y);
				}
				return Double3DPoint.Zero;
			}
		}

		public double Направление
		{
			get
			{
				if (Дорога == null)
				{
					return 0.0;
				}
				return Дорога.НайтиНаправление(расстояние);
			}
		}

		public Положение(IObjectContainer container, double расстояние)
			: this(container, расстояние, 0.0, 0.0)
		{
		}

		public Положение(IObjectContainer container, double расстояние, double отклонение)
			: this(container, расстояние, отклонение, 0.0)
		{
		}

		public Положение(IObjectContainer container, double расстояние, double отклонение, double высота)
		{
			_container = container;
			this.расстояние = расстояние;
			this.отклонение = отклонение;
			this.высота = высота;
			comment = null;
		}
	}
}

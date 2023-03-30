using System;
using System.Collections.Generic;

namespace Trancity
{
	public class Сигнальная_система
	{
		public class Контакт
		{
			private Road _дорога;

			public double расстояние;

			public Сигнальная_система система;

			public bool минус;

			public Road дорога
			{
				get
				{
					return _дорога;
				}
				set
				{
					if (_дорога != null)
					{
						_дорога.objects.Remove(this);
					}
					_дорога = value;
					if (value != null)
					{
						_дорога.objects.Add(this);
					}
				}
			}

			public Контакт(Сигнальная_система система, Road дорога, double расстояние, bool минус)
			{
				this.система = система;
				this.дорога = дорога;
				this.расстояние = расстояние;
				this.минус = минус;
			}
		}

		private int fсостояние;

		public int граница_переключения;

		public List<Контакт> элементы = new List<Контакт>();

		public List<Visual_Signal> vsignals = new List<Visual_Signal>();

		public Сигналы сигнал
		{
			get
			{
				состояние = Math.Max(0, состояние);
				if (состояние < граница_переключения)
				{
					return Сигналы.Зелёный;
				}
				return Сигналы.Красный;
			}
		}

		public int состояние
		{
			get
			{
				return fсостояние;
			}
			set
			{
				value = Math.Max(0, value);
				fсостояние = value;
			}
		}

		public Сигнальная_система(int граница_переключения, int состояние)
		{
			this.граница_переключения = граница_переключения;
			this.состояние = состояние;
		}

		public void CreateMesh()
		{
			foreach (Visual_Signal vsignal in vsignals)
			{
				vsignal.CreateMesh();
			}
		}

		public void Render()
		{
			foreach (Visual_Signal vsignal in vsignals)
			{
				vsignal.Render();
			}
		}

		public void Добавить_элемент(Контакт элемент)
		{
			элементы.Add(элемент);
		}

		public void Убрать_элемент(Контакт элемент)
		{
			элемент.дорога = null;
			элементы.Remove(элемент);
		}

		public int Индекс_элемента(Контакт элемент)
		{
			return элементы.IndexOf(элемент);
		}

		public void Вставить_элемент(int idx, Контакт элемент)
		{
			элементы.Insert(idx, элемент);
		}

		public void Добавить_сигнал(Visual_Signal элемент)
		{
			vsignals.Add(элемент);
		}

		public void Убрать_сигнал(Visual_Signal элемент)
		{
			элемент.road = null;
			vsignals.Remove(элемент);
		}

		public int Индекс_сигнала(Visual_Signal элемент)
		{
			return vsignals.IndexOf(элемент);
		}

		public void Вставить_сигнал(int idx, Visual_Signal элемент)
		{
			vsignals.Insert(idx, элемент);
		}
	}
}

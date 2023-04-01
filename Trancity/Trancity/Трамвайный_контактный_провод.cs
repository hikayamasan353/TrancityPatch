using System.Collections.Generic;
using Engine;

namespace Trancity
{
	public class Трамвайный_контактный_провод : Контактный_провод
	{
		public Трамвайный_контактный_провод[] следующие_провода2 = new Трамвайный_контактный_провод[0];

		public Трамвайный_контактный_провод[] предыдущие_провода2 = new Трамвайный_контактный_провод[0];

		private double tan_z => (высота[1] - высота[0]) / base.длина;

		public Трамвайный_контактный_провод(double начало_x, double начало_y, double конец_x, double конец_y)
			: base(начало_x, начало_y, конец_x, конец_y, правый: false)
		{
		}

		public override void UpdateNextWires(Контактный_провод[] провода)
		{
			List<Трамвайный_контактный_провод> list = new List<Трамвайный_контактный_провод>();
			List<Трамвайный_контактный_провод> list2 = new List<Трамвайный_контактный_провод>();
			foreach (Контактный_провод контактный_провод in провода)
			{
				if (контактный_провод == this || !(контактный_провод is Трамвайный_контактный_провод))
				{
					continue;
				}
				Трамвайный_контактный_провод трамвайный_контактный_провод = (Трамвайный_контактный_провод)контактный_провод;
				if ((трамвайный_контактный_провод.начало - конец).Modulus < 0.01)
				{
					int num = 0;
					while (true)
					{
						if (num < list.Count)
						{
							DoublePoint doublePoint = new DoublePoint(трамвайный_контактный_провод.tan_z - tan_z);
							DoublePoint doublePoint2 = new DoublePoint(list[num].tan_z - tan_z);
							if (doublePoint.Angle < doublePoint2.Angle)
							{
								list.Insert(num, трамвайный_контактный_провод);
								break;
							}
							num++;
							continue;
						}
						list.Add(трамвайный_контактный_провод);
						break;
					}
				}
				if (!((трамвайный_контактный_провод.конец - начало).Modulus < 0.01))
				{
					continue;
				}
				int num2 = 0;
				while (true)
				{
					if (num2 < list2.Count)
					{
						DoublePoint doublePoint3 = new DoublePoint(трамвайный_контактный_провод.tan_z - tan_z);
						DoublePoint doublePoint4 = new DoublePoint(list2[num2].tan_z - tan_z);
						if (doublePoint3.Angle < doublePoint4.Angle)
						{
							list2.Insert(num2, трамвайный_контактный_провод);
							break;
						}
						num2++;
						continue;
					}
					list2.Add(трамвайный_контактный_провод);
					break;
				}
			}
			следующие_провода2 = list.ToArray();
			предыдущие_провода2 = list2.ToArray();
		}
	}
}

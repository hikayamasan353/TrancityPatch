using System;
using System.Collections;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public class Рельс : Road
	{
		public class Добавочные_провода : MeshObject, IFromFile, IMatrixObject
		{
			private Рельс рельс;

			public string Filename => "box.x";

			public int MatricesCount
			{
				get
				{
					if (рельс != null && рельс.MatricesCount > 0)
					{
						return 1;
					}
					return 0;
				}
			}

			public DoublePoint координаты => рельс.НайтиКоординаты(расстояние, 0.0);

			public double направление => рельс.НайтиНаправление(расстояние);

			private double расстояние => Math.Max(рельс.Длина - рельс.расстояние_добавочных_проводов, 0.1);

			public Добавочные_провода(Рельс рельс)
			{
				this.рельс = рельс;
			}

			public Matrix GetMatrix(int index)
			{
				return Matrix.Scaling(0.5f, 0.5f, 0.5f) * Matrix.RotationY(0f - (float)направление) * Matrix.Translation((float)координаты.x, (float)высота_контактной_сети + 0.05f, (float)координаты.y);
			}
		}

		private int mesh_num_parts;

		public static double высота_контактной_сети = 5.7;

		public static double длина_стрелки = 3.0;

		public Добавочные_провода добавочные_провода;

		public static double качество_рельсов = 4.0;

		public int предыдущий_рельс;

		public double расстояние_добавочных_проводов;

		public static double расстояние_между_путями = 3.2;

		public int следующий_рельс;

		public static double смещение_стрелки = 0.15;

		public static bool стрелки_наоборот;

		public override int MatricesCount
		{
			get
			{
				if (matrices_count != -1 && (Math.Abs(col - Game.col) > 1 || Math.Abs(row - Game.row) > 1))
				{
					return 0;
				}
				if (bounding_sphere != null && !MyDirect3D.SphereInFrustum(bounding_sphere))
				{
					return 0;
				}
				if (matrices_count != -1)
				{
					return matrices_count;
				}
				if (кривая)
				{
					mesh_num_parts = (int)(base.Длина / base.АбсолютныйРадиус * 22.0 / качество_рельсов);
					if (mesh_num_parts > 200)
					{
						mesh_num_parts = 200;
					}
					if (mesh_num_parts < 5)
					{
						mesh_num_parts = 5;
					}
				}
				else if (высота[0] != высота[1])
				{
					mesh_num_parts = (int)(base.Длина * 2.0 / 10.0 / качество_рельсов) * 2 + 1;
				}
				else
				{
					mesh_num_parts = 1;
					if (стрелка_пошёрстная)
					{
						mesh_num_parts++;
					}
					if (стрелка_противошёрстная)
					{
						mesh_num_parts++;
					}
				}
				return mesh_num_parts * 2;
			}
		}

		public Рельс[] предыдущие_рельсы => (Рельс[])предыдущиеДороги;

		public Рельс[] следующие_рельсы => (Рельс[])следующиеДороги;

		public Рельс[] соседние_рельсы => (Рельс[])соседниеДороги;

		public bool стрелка_пошёрстная
		{
			get
			{
				if (предыдущие_рельсы.Length != 0)
				{
					return предыдущие_рельсы[предыдущий_рельс].следующие_рельсы.Length > 1;
				}
				return false;
			}
		}

		public bool стрелка_противошёрстная
		{
			get
			{
				if (следующие_рельсы.Length != 0)
				{
					return следующие_рельсы[следующий_рельс].предыдущие_рельсы.Length > 1;
				}
				return false;
			}
		}

		public Рельс(double начало_x, double начало_y, double конец_x, double конец_y, double направление, bool прямой)
			: base(начало_x, начало_y, конец_x, конец_y, направление, прямой, расстояние_между_путями, расстояние_между_путями)
		{
			расстояние_добавочных_проводов = 14.0;
			mesh_num_parts = 1;
			добавочные_провода = new Добавочные_провода(this);
		}

		public Рельс(double начало_x, double начало_y, double конец_x, double конец_y, double направление_0, double направление_1)
			: base(начало_x, начало_y, конец_x, конец_y, направление_0, направление_1, расстояние_между_путями, расстояние_между_путями)
		{
			расстояние_добавочных_проводов = 14.0;
			mesh_num_parts = 1;
			добавочные_провода = new Добавочные_провода(this);
		}

		public override Matrix GetMatrix(int index)
		{
			if (!MainForm.in_editor && matrixes[index] != MyMatrix.Zero)
			{
				return matrixes[index];
			}
			bool flag = false;
			if (index >= mesh_num_parts)
			{
				index -= mesh_num_parts;
				flag = true;
			}
			double num = (flag ? (-0.762) : 0.762);
			double отклонение = (flag ? (-0.762) : 0.762);
			double num2 = 0.05;
			double num3 = num2;
			double num4 = 1.5;
			if (MyDirect3D.карта)
			{
				num4 *= 10.0;
			}
			double расстояние = base.Длина * (double)index / (double)mesh_num_parts;
			double num5 = base.Длина * (double)(index + 1) / (double)mesh_num_parts;
			if (высота[0] != высота[1] && !кривая)
			{
				расстояние = ((index > mesh_num_parts / 2) ? (base.Длина * 8.0 / 10.0 + base.Длина * 2.0 / 10.0 * (double)(index - mesh_num_parts / 2 - 1) / (double)(mesh_num_parts / 2)) : (base.Длина * 2.0 / 10.0 * (double)index / (double)(mesh_num_parts / 2)));
				num5 = ((index + 1 > mesh_num_parts / 2) ? (base.Длина * 8.0 / 10.0 + base.Длина * 2.0 / 10.0 * (double)(index - mesh_num_parts / 2) / (double)(mesh_num_parts / 2)) : (base.Длина * 2.0 / 10.0 * (double)(index + 1) / (double)(mesh_num_parts / 2)));
			}
			num2 += НайтиВысоту(расстояние);
			num3 += НайтиВысоту(num5);
			if (стрелка_пошёрстная && index == 0)
			{
				num5 = длина_стрелки;
				if (flag && this == предыдущие_рельсы[предыдущий_рельс].следующие_рельсы[0] && предыдущие_рельсы[предыдущий_рельс].следующий_рельс != 0 && предыдущие_рельсы[предыдущий_рельс].следующие_рельсы[1].кривая)
				{
					num += смещение_стрелки;
				}
				if (!flag && this == предыдущие_рельсы[предыдущий_рельс].следующие_рельсы[1] && предыдущие_рельсы[предыдущий_рельс].следующий_рельс != 1 && предыдущие_рельсы[предыдущий_рельс].следующие_рельсы[0].кривая)
				{
					num -= смещение_стрелки;
				}
			}
			else if (стрелка_противошёрстная && index == mesh_num_parts - 1)
			{
				расстояние = num5 - длина_стрелки;
			}
			else
			{
				if (стрелка_пошёрстная && !стрелка_противошёрстная)
				{
					расстояние = длина_стрелки + (base.Длина - длина_стрелки) * (double)(index - 1) / (double)(mesh_num_parts - 1);
					num5 = длина_стрелки + (base.Длина - длина_стрелки) * (double)index / (double)(mesh_num_parts - 1);
				}
				if (!стрелка_пошёрстная && стрелка_противошёрстная)
				{
					расстояние = (base.Длина - длина_стрелки) * (double)index / (double)(mesh_num_parts - 1);
					num5 = (base.Длина - длина_стрелки) * (double)(index + 1) / (double)(mesh_num_parts - 1);
				}
				if (стрелка_пошёрстная && стрелка_противошёрстная)
				{
					расстояние = длина_стрелки + (base.Длина - 2.0 * длина_стрелки) * (double)(index - 1) / (double)(mesh_num_parts - 2);
					num5 = длина_стрелки + (base.Длина - 2.0 * длина_стрелки) * (double)index / (double)(mesh_num_parts - 2);
				}
			}
			DoublePoint doublePoint = НайтиКоординаты(расстояние, num);
			DoublePoint doublePoint2 = НайтиКоординаты(num5, отклонение) - doublePoint;
			double modulus = doublePoint2.Modulus;
			double angle = doublePoint2.Angle;
			float num6 = (float)num4 / 2f;
			Matrix matrix = default(Matrix);
			matrix.M11 = 1f;
			matrix.M12 = (float)(num3 - num2) / num6;
			matrix.M22 = 1f;
			matrix.M33 = 1f;
			matrix.M44 = 1f;
			return matrix * Matrix.Scaling((float)modulus, num6, (float)num4) * Matrix.RotationY(0f - (float)angle) * Matrix.Translation((float)doublePoint.x, (float)num2, (float)doublePoint.y);
		}

		public override void ОбновитьСледующиеДороги(Road[] дороги)
		{
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			ArrayList arrayList3 = new ArrayList();
			foreach (Road road in дороги)
			{
				if (road == this || !(road is Рельс))
				{
					continue;
				}
				if ((road.концы[0] - концы[1]).Modulus < 0.01 && (Math.Abs(road.направления[0] - направления[1] + Math.PI) < 0.0001 || Math.Abs(road.направления[0] - направления[1] - Math.PI) < 0.0001))
				{
					if (arrayList.Count > 0 && road.СтепеньПоворота0 < ((Рельс)arrayList[0]).СтепеньПоворота0)
					{
						arrayList.Insert(0, road);
					}
					else
					{
						arrayList.Add(road);
					}
				}
				if ((road.концы[1] - концы[0]).Modulus < 0.01 && (Math.Abs(road.направления[1] - направления[0] + Math.PI) < 0.0001 || Math.Abs(road.направления[1] - направления[0] - Math.PI) < 0.0001))
				{
					if (arrayList2.Count > 0 && road.СтепеньПоворота1 > ((Рельс)arrayList2[0]).СтепеньПоворота1)
					{
						arrayList2.Insert(0, road);
					}
					else
					{
						arrayList2.Add(road);
					}
				}
				if ((road.концы[1] - концы[1]).Modulus < 0.01 && (Math.Abs(road.направления[1] - направления[1]) < 0.0001 || Math.Abs(road.направления[1] - направления[1] + Math.PI * 2.0) < 0.0001 || Math.Abs(road.направления[1] - направления[1] - Math.PI * 2.0) < 0.0001))
				{
					arrayList3.Add(road);
				}
			}
			следующиеДороги = (Рельс[])arrayList.ToArray(typeof(Рельс));
			предыдущиеДороги = (Рельс[])arrayList2.ToArray(typeof(Рельс));
			соседниеДороги = (Рельс[])arrayList3.ToArray(typeof(Рельс));
		}
	}
}

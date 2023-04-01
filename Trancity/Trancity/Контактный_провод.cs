using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public class Контактный_провод : MeshObject, MeshObject.IFromFile, IMatrixObject, IObjectContainer, ITest
	{
		private ArrayList fобъекты = new ArrayList();

		public double[] высота = new double[2];

		public DoublePoint конец;

		public DoublePoint начало;

		public bool обесточенный;

		public bool правый;

		public Контактный_провод[] предыдущие_провода = new Контактный_провод[0];

		public static double расстояние_между_проводами = 0.65;

		public Контактный_провод[] следующие_провода = new Контактный_провод[0];

		public int color
		{
			get
			{
				if (_meshMaterials != null && _meshMaterials.Length != 0)
				{
					return _meshMaterials[0].Diffuse.ToArgb();
				}
				return 0;
			}
			set
			{
				if (_meshMaterials != null && _meshMaterials.Length != 0)
				{
					_meshMaterials[0].Diffuse = Color.FromArgb(value | -16777216);
					_meshMaterials[0].Ambient = Color.FromArgb(value | -16777216);
				}
			}
		}

		public string Filename => "wire.x";

		public int MatricesCount
		{
			get
			{
				if (!MyDirect3D.карта)
				{
					DoublePoint doublePoint = FindCoords(0.0, 0.0);
					DoublePoint doublePoint2 = FindCoords(длина, 0.0);
					if (new DoublePoint(doublePoint.x - MyDirect3D.Camera_Position.x, doublePoint.y - MyDirect3D.Camera_Position.z).Modulus > 250.0 + длина && new DoublePoint(doublePoint2.x - MyDirect3D.Camera_Position.x, doublePoint2.y - MyDirect3D.Camera_Position.z).Modulus > 250.0 + длина)
					{
						return 0;
					}
				}
				return 1;
			}
		}

		public static double высота_контактной_сети => Рельс.высота_контактной_сети;

		public double длина => (конец - начало).Modulus;

		public double направление => (конец - начало).Angle;

		public ArrayList objects => fобъекты;

		public Контактный_провод(double начало_x, double начало_y, double конец_x, double конец_y, bool правый)
		{
			начало = new DoublePoint(начало_x, начало_y);
			конец = new DoublePoint(конец_x, конец_y);
			this.правый = правый;
		}

		public Matrix GetMatrix(int index)
		{
			if (last_matrix != MyMatrix.Zero)
			{
				return last_matrix;
			}
			float num = (MyDirect3D.вид_сверху ? 1.5f : 0.5f);
			if (MyDirect3D.карта)
			{
				num *= 10f;
			}
			DoublePoint doublePoint = конец - начало;
			double modulus = doublePoint.Modulus;
			double angle = doublePoint.Angle;
			double num2 = высота[0] + высота_контактной_сети;
			float num3 = num / 2f;
			Matrix matrix = default(Matrix);
			matrix.M11 = 1f;
			matrix.M22 = 1f;
			matrix.M33 = 1f;
			matrix.M44 = 1f;
			matrix.M12 = (float)(высота[1] - высота[0]) / num3;
			return matrix * Matrix.Scaling((float)modulus, num3, num) * Matrix.RotationY(0f - (float)angle) * Matrix.Translation((float)начало.x, (float)num2, (float)начало.y);
		}

		public void ComputeMatrix()
		{
			if (!MainForm.in_editor)
			{
				last_matrix = GetMatrix(0);
			}
		}

		public double FindHeight(double расстояние)
		{
			return высота[0] + (высота[1] - высота[0]) * расстояние / длина;
		}

		public DoublePoint FindCoords(double расстояние, double отклонение)
		{
			DoublePoint point = конец - начало;
			DoublePoint doublePoint = начало + point * (расстояние / длина);
			point.Angle += Math.PI / 2.0;
			point.Modulus = отклонение;
			return doublePoint.Add(ref point);
		}

		public virtual void UpdateNextWires(Контактный_провод[] провода)
		{
			List<Контактный_провод> list = new List<Контактный_провод>();
			List<Контактный_провод> list2 = new List<Контактный_провод>();
			foreach (Контактный_провод контактный_провод in провода)
			{
				if (контактный_провод == this || контактный_провод is Трамвайный_контактный_провод)
				{
					continue;
				}
				if ((контактный_провод.начало - конец).Modulus < 0.01)
				{
					if (list.Count > 0)
					{
						DoublePoint doublePoint = new DoublePoint(контактный_провод.направление - направление);
						DoublePoint doublePoint2 = new DoublePoint(list[0].направление - направление);
						if (doublePoint.Angle > doublePoint2.Angle)
						{
							list.Insert(0, контактный_провод);
							goto IL_00af;
						}
					}
					list.Add(контактный_провод);
				}
				goto IL_00af;
				IL_00af:
				if (!((контактный_провод.конец - начало).Modulus < 0.01))
				{
					continue;
				}
				if (list2.Count > 0)
				{
					DoublePoint doublePoint3 = new DoublePoint(контактный_провод.направление - направление);
					DoublePoint doublePoint4 = new DoublePoint(list2[0].направление - направление);
					if (doublePoint3.Angle > doublePoint4.Angle)
					{
						list2.Insert(0, контактный_провод);
						continue;
					}
				}
				list2.Add(контактный_провод);
			}
			следующие_провода = list.ToArray();
			предыдущие_провода = list2.ToArray();
		}
	}
}

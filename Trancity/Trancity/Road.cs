using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Common;
using Engine;
using SlimDX;
using SlimDX.Direct3D9;

namespace Trancity
{
	public class Road : Spline, IObjectContainer
	{
		public struct Структура
		{
			public double радиус;

			public double угол0;

			public double угол1;

			public DoublePoint центр0;

			public DoublePoint центр1;

			public DoublePoint серединка;

			public double длина0;

			public double длина1;
		}

		private int _defaultAmbient;

		private int _defaultDiffuse;

		private int _meshNumParts;

		public readonly double[] высота;

		public List<Положение> занятыеПоложения;

		public static double качествоДороги = 1.0;

		public readonly DoublePoint[] концы;

		public bool кривая;

		public readonly double[] направления;

		public Road[] предыдущиеДороги;

		public Road[] следующиеДороги;

		public Road[] соседниеДороги;

		public Структура структура;

		public double[] ширина;

		public static double ширинаПолосы = 5.0;

		public const double uklon_koef = 2.2;

		protected Matrix[] matrixes;

		protected int matrices_count = -1;

		protected int col;

		protected int row;

		public int Color
		{
			get
			{
				if (_meshMaterials != null && _meshMaterials.Length != 0)
				{
					return _meshMaterials[0].Diffuse.ToArgb() & 0xFFFFFF;
				}
				return 0;
			}
			set
			{
				if (_meshMaterials != null && _meshMaterials.Length != 0)
				{
					if (_defaultDiffuse == -1)
					{
						_defaultDiffuse = _meshMaterials[0].Diffuse.ToArgb();
						_defaultAmbient = _meshMaterials[0].Ambient.ToArgb();
					}
					if (value == 0)
					{
						_meshMaterials[0].Diffuse = System.Drawing.Color.FromArgb(_defaultDiffuse);
						_meshMaterials[0].Ambient = System.Drawing.Color.FromArgb(_defaultAmbient);
					}
					else
					{
						_meshMaterials[0].Diffuse = System.Drawing.Color.FromArgb(value | -16777216);
						_meshMaterials[0].Ambient = System.Drawing.Color.FromArgb(value | -16777216);
					}
				}
			}
		}

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
					_meshNumParts = (int)Math.Floor(Длина / АбсолютныйРадиус * 22.0 / качествоДороги);
					if (_meshNumParts > 200)
					{
						_meshNumParts = 200;
					}
					if (_meshNumParts < 5)
					{
						_meshNumParts = 5;
					}
				}
				else if (высота[0] != высота[1])
				{
					_meshNumParts = (int)(Длина * 2.0 / 10.0 / качествоДороги) * 2 + 1;
				}
				else
				{
					_meshNumParts = 1;
				}
				return _meshNumParts * 2;
			}
		}

		public double Длина
		{
			get
			{
				if (кривая)
				{
					return структура.длина0 + структура.длина1;
				}
				return DoublePoint.Distance(ref концы[1], ref концы[0]);
			}
		}

		public ArrayList objects { get; private set; }

		public bool ОпределитьКривой => Радиус != 0.0;

		public double Радиус => структура.радиус;

		public double АбсолютныйРадиус => Math.Abs(структура.радиус);

		public double СтепеньПоворота0
		{
			get
			{
				DoublePoint doublePoint = НайтиКоординаты(5.0, 0.0).Subtract(ref концы[0]);
				doublePoint.Angle -= направления[0];
				return 0.0 - doublePoint.y;
			}
		}

		public double СтепеньПоворота1
		{
			get
			{
				DoublePoint doublePoint = НайтиКоординаты(Длина - 5.0, 0.0).Subtract(ref концы[1]);
				doublePoint.Angle -= направления[1];
				return 0.0 - doublePoint.y;
			}
		}

		public Road(double началоX, double началоY, double конецX, double конецY, double направление, bool прямая, double ширина0, double ширина1)
		{
			занятыеПоложения = new List<Положение>();
			концы = new DoublePoint[2];
			направления = new double[2];
			ширина = new double[2];
			высота = new double[2];
			предыдущиеДороги = new Road[0];
			следующиеДороги = new Road[0];
			соседниеДороги = new Road[0];
			_defaultDiffuse = -1;
			_defaultAmbient = -1;
			objects = new ArrayList();
			_meshNumParts = 1;
			концы[0] = new DoublePoint(началоX, началоY);
			концы[1] = new DoublePoint(конецX, конецY);
			направления[0] = направление;
			if (прямая)
			{
				направления[1] = направление + Math.PI;
			}
			else
			{
				DoublePoint doublePoint = концы[0] - концы[1];
				DoublePoint doublePoint2 = концы[1] - концы[0];
				направления[1] = doublePoint.Angle - (направления[0] - doublePoint2.Angle);
			}
			кривая = !прямая;
			ширина[0] = ширина0;
			ширина[1] = ширина1;
			ОбновитьСтруктуру();
			while (направления[0] > Math.PI)
			{
				направления[0] -= Math.PI * 2.0;
			}
			while (направления[0] <= -Math.PI)
			{
				направления[0] += Math.PI * 2.0;
			}
			while (направления[1] > Math.PI)
			{
				направления[1] -= Math.PI * 2.0;
			}
			while (направления[1] <= -Math.PI)
			{
				направления[1] += Math.PI * 2.0;
			}
		}

		public Road(double началоX, double началоY, double конецX, double конецY, double направление0, double направление1, double ширина0, double ширина1)
		{
			занятыеПоложения = new List<Положение>();
			концы = new DoublePoint[2];
			направления = new double[2];
			ширина = new double[2];
			высота = new double[2];
			предыдущиеДороги = new Road[0];
			следующиеДороги = new Road[0];
			соседниеДороги = new Road[0];
			_defaultDiffuse = -1;
			_defaultAmbient = -1;
			objects = new ArrayList();
			_meshNumParts = 1;
			концы[0] = new DoublePoint(началоX, началоY);
			концы[1] = new DoublePoint(конецX, конецY);
			направления[0] = направление0;
			направления[1] = направление1;
			ширина[0] = ширина0;
			ширина[1] = ширина1;
			ОбновитьСтруктуру();
			кривая = ОпределитьКривой;
			while (направления[0] > Math.PI)
			{
				направления[0] -= Math.PI * 2.0;
			}
			while (направления[0] <= -Math.PI)
			{
				направления[0] += Math.PI * 2.0;
			}
			while (направления[1] > Math.PI)
			{
				направления[1] -= Math.PI * 2.0;
			}
			while (направления[1] <= -Math.PI)
			{
				направления[1] += Math.PI * 2.0;
			}
		}

		public override void CreateCustomMesh()
		{
			matrices_count = MatricesCount;
			int num = 0;
			vertexes = new MeshVertex[matrices_count * model.points.Length];
			for (int i = 0; i < matrices_count; i++)
			{
				double num2 = Длина * (double)i / (double)(matrices_count - 1);
				double num3 = НайтиШирину(num2) / 2.0;
				НайтиПозициюПоВысоте(num2, out var нужнаяВысота, out var направлениеY);
				Vector3 normal = MyFeatures.ToVector3(new Double3DPoint(НайтиНаправление(num2), направлениеY + Math.PI / 2.0));
				normal.Normalize();
				for (int j = 0; j < model.points.Length; j++)
				{
					DoublePoint doublePoint = НайтиКоординаты(num2, model.noscale ? model.points[j].x : (model.points[j].x * num3)).Subtract(ref структура.серединка);
					vertexes[i * model.points.Length + j].Position = new Vector3((float)doublePoint.x, (float)(нужнаяВысота + Math.Cos(направлениеY) * model.points[j].y), (float)doublePoint.y);
					vertexes[i * model.points.Length + j].Normal = normal;
					vertexes[i * model.points.Length + j].texcoord = new Vector2((float)(num2 / num3), (float)model.points[j].z);
				}
			}
			poly_count = (model.points.Length - 1) * (matrices_count - 1) * 2;
			indexes = new int[poly_count * 3];
			for (int k = 0; k < matrices_count - 1; k++)
			{
				for (int l = 0; l < model.points.Length - 1; l++)
				{
					num = ((model.points.Length - 1) * k + l) * 6;
					indexes[num] = k * model.points.Length + l;
					indexes[num + 2] = indexes[num] + 1;
					indexes[num + 1] = indexes[num] + model.points.Length;
					indexes[num + 3] = indexes[num + 2];
					indexes[num + 5] = indexes[num + 1] + 1;
					indexes[num + 4] = indexes[num + 1];
				}
			}
			_meshMaterials = new Material[1];
			_meshMaterials[0] = default(Material);
			_meshMaterials[0].Diffuse = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Specular = new Color4(1f, 1f, 1f, 1f);
			_meshMaterials[0].Ambient = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Emissive = new Color4(1f, 0f, 0f, 0f);
			_meshMaterials[0].Power = 0f;
			_meshTextures = new Texture[1];
			LoadTexture(0, meshDir + model.texture_filename);
			matrices_count = 1;
			matrixes = new Matrix[1];
			matrixes[0] = Matrix.Translation((float)структура.серединка.x, 0f, (float)структура.серединка.y);
		}

		public override Matrix GetMatrix(int index)
		{
			if (!MainForm.in_editor && matrixes[index] != MyMatrix.Zero)
			{
				return matrixes[index];
			}
			bool flag = false;
			if (index >= _meshNumParts)
			{
				index -= _meshNumParts;
				flag = true;
			}
			double длина = Длина;
			double расстояние = длина * (double)index / (double)_meshNumParts;
			double расстояние2 = длина * (double)(index + 1) / (double)_meshNumParts;
			if (высота[0] != высота[1] && !кривая)
			{
				расстояние = ((index > _meshNumParts / 2) ? (длина * 8.0 / 10.0 + длина * 2.0 / 10.0 * (double)(index - _meshNumParts / 2 - 1) / (double)(_meshNumParts / 2)) : (длина * 2.0 / 10.0 * (double)index / (double)(_meshNumParts / 2)));
				расстояние2 = ((index + 1 > _meshNumParts / 2) ? (длина * 8.0 / 10.0 + длина * 2.0 / 10.0 * (double)(index - _meshNumParts / 2) / (double)(_meshNumParts / 2)) : (длина * 2.0 / 10.0 * (double)(index + 1) / (double)(_meshNumParts / 2)));
			}
			double num = НайтиШирину(расстояние);
			double num2 = НайтиШирину(расстояние2);
			Matrix matrix = default(Matrix);
			double num3;
			DoublePoint doublePoint;
			double num5;
			if (!flag)
			{
				num3 = НайтиВысоту(расстояние);
				double num4 = НайтиВысоту(расстояние2) - num3;
				doublePoint = НайтиКоординаты(расстояние, num / 2.0);
				DoublePoint doublePoint2 = НайтиКоординаты(расстояние2, num2 / 2.0);
				num5 = НайтиНаправление(расстояние);
				DoublePoint doublePoint3 = doublePoint2 - doublePoint;
				doublePoint3.Angle -= num5;
				matrix.M11 = (float)doublePoint3.x;
				matrix.M12 = (float)num4;
				matrix.M13 = (float)doublePoint3.y;
				matrix.M33 = (float)num;
			}
			else
			{
				num3 = НайтиВысоту(расстояние2);
				double num4 = НайтиВысоту(расстояние) - num3;
				doublePoint = НайтиКоординаты(расстояние2, (0.0 - num2) / 2.0);
				DoublePoint doublePoint4 = НайтиКоординаты(расстояние, (0.0 - num) / 2.0);
				num5 = НайтиНаправление(расстояние2) + Math.PI;
				DoublePoint doublePoint5 = doublePoint4 - doublePoint;
				doublePoint5.Angle -= num5;
				matrix.M11 = (float)doublePoint5.x;
				matrix.M12 = (float)num4;
				matrix.M13 = (float)doublePoint5.y;
				matrix.M33 = (float)num2;
			}
			matrix.M22 = 1f;
			matrix.M44 = 1f;
			return matrix * Matrix.RotationY(0f - (float)num5) * Matrix.Translation((float)doublePoint.x, (float)num3, (float)doublePoint.y);
		}

		public double НайтиВысоту(double расстояние)
		{
			НайтиПозициюПоВысоте(расстояние, out var нужнаяВысота, out var _);
			return нужнаяВысота;
		}

		public DoublePoint НайтиКоординаты(double расстояние, double отклонение)
		{
			НайтиПозицию(расстояние, out var координаты, out var направление);
			DoublePoint point = new DoublePoint(направление += Math.PI / 2.0).Multyply(отклонение);
			return координаты.Add(ref point);
		}

		public double НайтиНаправление(double расстояние)
		{
			НайтиПозицию(расстояние, out var _, out var направление);
			return направление;
		}

		public double НайтиНаправлениеY(double расстояние)
		{
			НайтиПозициюПоВысоте(расстояние, out var _, out var направлениеY);
			return направлениеY;
		}

		public void НайтиПозицию(double расстояние, out DoublePoint координаты, out double направление)
		{
			if (кривая)
			{
				if (расстояние < структура.длина0)
				{
					DoublePoint doublePoint = концы[0] - структура.центр0;
					double num = структура.угол0 * расстояние / структура.длина0;
					doublePoint.Angle += num;
					координаты = структура.центр0 + doublePoint;
					if (структура.радиус > 0.0)
					{
						направление = doublePoint.Angle + Math.PI / 2.0;
					}
					else
					{
						направление = doublePoint.Angle - Math.PI / 2.0;
					}
				}
				else
				{
					DoublePoint doublePoint2 = структура.серединка - структура.центр1;
					double num2 = структура.угол1 * (расстояние - структура.длина0) / структура.длина1;
					doublePoint2.Angle += num2;
					координаты = структура.центр1 + doublePoint2;
					if (структура.радиус < 0.0)
					{
						направление = doublePoint2.Angle + Math.PI / 2.0;
					}
					else
					{
						направление = doublePoint2.Angle - Math.PI / 2.0;
					}
				}
			}
			else
			{
				double value = расстояние / Длина;
				DoublePoint doublePoint3 = концы[1] - концы[0];
				координаты = концы[0] + doublePoint3.Multyply(value);
				направление = направления[0];
			}
		}

		public void НайтиПозициюПоВысоте(double расстояние, out double нужнаяВысота, out double направлениеY)
		{
			if (высота[0] == высота[1])
			{
				нужнаяВысота = высота[0];
				направлениеY = 0.0;
				return;
			}
			double длина = Длина;
			double num = длина / 10.0;
			double num2 = (длина - 2.0 * num) / new DoublePoint(длина - 2.0 * num, высота[1] - высота[0]).Modulus;
			double num3 = num / Math.Tan(Math.Acos(num2) / 2.0);
			int num4 = ((высота[1] > высота[0]) ? 1 : (-1));
			if (расстояние < num + num * num2)
			{
				if (расстояние < 0.0)
				{
					расстояние = 0.0;
				}
				нужнаяВысота = высота[0] + (double)num4 * (num3 - Math.Sqrt(num3 * num3 - расстояние * расстояние));
				направлениеY = (double)num4 * Math.Asin(расстояние / num3);
			}
			else if (расстояние > длина - num - num * num2)
			{
				if (расстояние > длина)
				{
					расстояние = длина;
				}
				нужнаяВысота = высота[1] - (double)num4 * (num3 - Math.Sqrt(num3 * num3 - (длина - расстояние) * (длина - расстояние)));
				направлениеY = (double)num4 * Math.Asin((длина - расстояние) / num3);
			}
			else
			{
				нужнаяВысота = высота[0] + (высота[1] - высота[0]) * (расстояние - num) / (длина - 2.0 * num);
				направлениеY = Math.Atan((высота[1] - высота[0]) / (длина - 2.0 * num));
			}
		}

		public double НайтиШирину(double расстояние)
		{
			return ширина[0] + (ширина[1] - ширина[0]) * расстояние / Длина;
		}

		public virtual void ОбновитьСледующиеДороги(Road[] дороги)
		{
			List<Road> list = new List<Road>();
			List<Road> list2 = new List<Road>();
			List<Road> list3 = new List<Road>();
			foreach (Road road in дороги)
			{
				if (road == this || road is Рельс)
				{
					continue;
				}
				if ((road.концы[0] - концы[1]).Modulus < 0.01 && (Math.Abs(road.направления[0] - направления[1] + Math.PI) < 0.0001 || Math.Abs(road.направления[0] - направления[1] - Math.PI) < 0.0001))
				{
					if (list.Count > 0 && road.СтепеньПоворота0 < list[0].СтепеньПоворота0)
					{
						list.Insert(0, road);
					}
					else
					{
						list.Add(road);
					}
				}
				if ((road.концы[1] - концы[0]).Modulus < 0.01 && (Math.Abs(road.направления[1] - направления[0] + Math.PI) < 0.0001 || Math.Abs(road.направления[1] - направления[0] - Math.PI) < 0.0001))
				{
					if (list2.Count > 0 && road.СтепеньПоворота1 > list2[0].СтепеньПоворота1)
					{
						list2.Insert(0, road);
					}
					else
					{
						list2.Add(road);
					}
				}
				if ((road.концы[1] - концы[1]).Modulus < 0.01 && (Math.Abs(road.направления[1] - направления[1]) < 0.0001 || Math.Abs(road.направления[1] - направления[1] + Math.PI * 2.0) < 0.0001 || Math.Abs(road.направления[1] - направления[1] - Math.PI * 2.0) < 0.0001))
				{
					list3.Add(road);
				}
			}
			следующиеДороги = list.ToArray();
			предыдущиеДороги = list2.ToArray();
			соседниеДороги = list3.ToArray();
		}

		public void ОбновитьСтруктуру()
		{
			Структура структура = ОпределитьСтруктуру(index: false);
			Структура структура2 = ОпределитьСтруктуру(index: true);
			if (структура2.радиус == 0.0)
			{
				this.структура = структура;
			}
			else if (структура.радиус == 0.0 || структура.длина0 + структура.длина1 > структура2.длина0 + структура2.длина1)
			{
				this.структура = структура2;
			}
			else
			{
				this.структура = структура;
			}
		}

		public Структура ОпределитьСтруктуру(bool index)
		{
			Структура result = default(Структура);
			DoublePoint doublePoint = концы[0];
			DoublePoint doublePoint2 = концы[1];
			double angle = направления[0] + Math.PI / 2.0;
			double angle2 = направления[1] + Math.PI / 2.0;
			DoublePoint point = new DoublePoint(angle2);
			DoublePoint doublePoint3 = new DoublePoint(angle).Subtract(ref point).Divide(2.0);
			DoublePoint doublePoint4 = (doublePoint - doublePoint2).Divide(2.0);
			point.CopyFromAngle(doublePoint4.Angle);
			doublePoint3.Divide(ref point);
			double num = Math.Cos(Math.Asin(doublePoint3.y));
			DoublePoint point2 = new DoublePoint((index ? num : (0.0 - num)) - doublePoint3.x, 0.0).Multyply(ref point);
			if (point2.Modulus >= 1E-06)
			{
				point.CopyFromAngle(doublePoint4.Angle);
				doublePoint3.Multyply(ref point);
				point2 = doublePoint4.Divide(ref point2);
				result.радиус = point2.x;
				result.центр0 = doublePoint + new DoublePoint(angle).Multyply(ref point2);
				result.центр1 = doublePoint2 + new DoublePoint(angle2).Multyply(ref point2);
				result.серединка = (result.центр0 + result.центр1).Divide(2.0);
				DoublePoint point3 = doublePoint - result.центр0;
				result.угол0 = ((result.серединка - result.центр0).Divide(ref point3).Angle + Math.PI * 2.0) % (Math.PI * 2.0);
				point3 = result.серединка - result.центр1;
				result.угол1 = ((doublePoint2 - result.центр1).Divide(ref point3).Angle + Math.PI * 2.0) % (Math.PI * 2.0);
				if (result.угол0 > 6.283184307179586)
				{
					result.угол0 -= Math.PI * 2.0;
				}
				if (result.угол1 > 6.283184307179586)
				{
					result.угол1 -= Math.PI * 2.0;
				}
				if (point2.x < 0.0 && result.угол0 > 1E-06)
				{
					result.угол0 -= Math.PI * 2.0;
				}
				if (point2.x > 0.0 && result.угол1 > 1E-06)
				{
					result.угол1 -= Math.PI * 2.0;
				}
				result.длина0 = Math.Abs(result.угол0 * result.радиус);
				result.длина1 = Math.Abs(result.угол1 * result.радиус);
			}
			return result;
		}

		public void CreateBoundingSphere()
		{
			try
			{
				double num = Длина / 2.0;
				double y = НайтиВысоту(num);
				DoublePoint doublePoint = НайтиКоординаты(num, 0.0);
				Double3DPoint position = new Double3DPoint(doublePoint.x, y, doublePoint.y);
				bounding_sphere = new Sphere(Double3DPoint.Zero, num + 5.0);
				bounding_sphere.Update(position, DoublePoint.Zero);
				col = (int)Math.Floor(doublePoint.x / (double)Ground.grid_size);
				row = (int)Math.Floor(doublePoint.y / (double)Ground.grid_size);
			}
			catch
			{
			}
		}
	}
}

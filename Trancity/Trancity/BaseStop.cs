using System;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public abstract class BaseStop : MeshObject, MeshObject.IFromFile, IMatrixObject, ITest, ITest2
	{
		public TypeOfTransport typeOfTransport = new TypeOfTransport();

		private Road froad;

		private Положение position;

		private Double3DPoint point_position;

		private double vector;

		public double distance;

		public bool serviceStop;

		public ObjectModel model;

		public string name;

		public virtual string Filename => model.filename;

		public int MatricesCount
		{
			get
			{
				if (MyDirect3D.SphereInFrustum(bounding_sphere) && !serviceStop)
				{
					return 1;
				}
				return 0;
			}
		}

		public Road road
		{
			get
			{
				return froad;
			}
			set
			{
				if (froad != null)
				{
					froad.objects.Remove(this);
				}
				froad = value;
				if (value != null)
				{
					froad.objects.Add(this);
				}
			}
		}

		protected BaseStop(string _name)
		{
			name = _name;
			ObjectLoader.FindModel(1, _name, ref model, ref meshDir);
			if (model != null)
			{
				meshDir = model.dir;
			}
		}

		public Matrix GetMatrix(int index)
		{
			if (!(last_matrix != MyMatrix.Zero))
			{
				return Matrix.RotationY(0f - (float)vector) * Matrix.Translation((float)point_position.x, (float)point_position.y, (float)point_position.z);
			}
			return last_matrix;
		}

		public void ComputeMatrix()
		{
			if (!MainForm.in_editor)
			{
				last_matrix = GetMatrix(0);
			}
		}

		public void UpdatePosition(World мир)
		{
			if (road == null)
			{
				return;
			}
			position = new Положение(road, distance, (0.0 - road.НайтиШирину(distance)) / 2.0 - 2.4);
			if (road is Рельс)
			{
				Road[] дороги = мир.Дороги;
				foreach (Road дорога in дороги)
				{
					Положение положение = мир.Найти_положение(road.НайтиКоординаты(distance, 0.0), дорога);
					if (положение.Дорога != null && положение.отклонение > 0.0)
					{
						double val = -2.4 - положение.отклонение - положение.Дорога.НайтиШирину(положение.расстояние) / 2.0;
						position.отклонение = Math.Min(position.отклонение, val);
					}
				}
			}
			point_position.XZPoint = road.НайтиКоординаты(distance, position.отклонение);
			point_position.y = road.НайтиВысоту(distance);
			vector = road.НайтиНаправление(distance);
			if (bounding_sphere == null)
			{
				bounding_sphere = new Sphere(new Double3DPoint(2.0125, 1.5, -6.9875), 21.0);
			}
			bounding_sphere.Update(new Double3DPoint(point_position.x, point_position.y, point_position.z), new DoublePoint(vector));
		}

		public void CheckCondition()
		{
			if (!MainForm.in_editor)
			{
				base.IsNear = Math.Abs(Math.Floor(point_position.x / (double)Ground.grid_size) - (double)Game.col) < 1.1 && Math.Abs(Math.Floor(point_position.z / (double)Ground.grid_size) - (double)Game.row) < 1.1;
			}
		}
	}
}

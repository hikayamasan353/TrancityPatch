using System;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public class Объект : MeshObject, IMatrixObject, MeshObject.IFromFile, IVector, ITest, ITest2
	{
		public string filename;

		public double x0;

		public double y0;

		public double angle0;

		public double height0;

		public ObjectModel model;

		private int col;

		private int row;

		public string Filename => model.filename;

		public int MatricesCount
		{
			get
			{
				if (Math.Abs(col - Game.col) > 1 || Math.Abs(row - Game.row) > 1)
				{
					return 0;
				}
				if (!MyDirect3D.SphereInFrustum(bounding_sphere))
				{
					return 0;
				}
				return 1;
			}
		}

		public DoublePoint position
		{
			get
			{
				return new DoublePoint(x0, y0);
			}
			set
			{
				x0 = value.x;
				y0 = value.y;
				OnPositionUpdate();
			}
		}

		public Double3DPoint Position3D
		{
			get
			{
				return new Double3DPoint(x0, height0, y0);
			}
			set
			{
				x0 = value.x;
				y0 = value.z;
				height0 = value.y;
				OnPositionUpdate();
			}
		}

		public DoublePoint direction2 => new DoublePoint(angle0);

		public double direction => angle0;

		public Объект(string filename, double x0, double y0, double angle0, double height0)
		{
			this.filename = filename;
			this.x0 = x0;
			this.y0 = y0;
			this.angle0 = angle0;
			this.height0 = height0;
		}

		public override void CreateMesh()
		{
			ObjectLoader.FindModel(0, filename, ref model, ref meshDir);
			if (model != null)
			{
				ComputeMatrix();
				CheckCondition();
				base.CreateMesh();
				bounding_sphere = new Sphere(model.bsphere.pos, model.bsphere.radius);
				bounding_sphere.Update(Position3D, direction2);
			}
		}

		private void OnPositionUpdate()
		{
			bounding_sphere.Update(Position3D, direction2);
			ComputeMatrix();
		}

		public Matrix GetMatrix(int index)
		{
			if (!(last_matrix != MyMatrix.Zero))
			{
				return Matrix.RotationY(0f - (float)angle0) * Matrix.Translation((float)x0, (float)height0, (float)y0);
			}
			return last_matrix;
		}

		public void ComputeMatrix()
		{
			if (!MainForm.in_editor)
			{
				last_matrix = GetMatrix(0);
			}
			col = (int)Math.Floor(x0 / (double)Ground.grid_size);
			row = (int)Math.Floor(y0 / (double)Ground.grid_size);
		}

		public void CheckCondition()
		{
			if (!MainForm.in_editor)
			{
				base.IsNear = (double)Math.Abs(col - Game.col) < 1.1 && (double)Math.Abs(row - Game.row) < 1.1;
			}
		}
	}
}

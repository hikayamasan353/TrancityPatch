using System;
using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public class Ground : MeshObject, MeshObject.IFromFile, IMatrixObject
	{
		public static int grid_step = 21;

		public static int grid_size = 300;

		public string Filename => "ground.x";

		public int MatricesCount
		{
			get
			{
				if (MyDirect3D.карта)
				{
					return 0;
				}
				return 4;
			}
		}

		public Matrix GetMatrix(int index)
		{
			DoublePoint xZPoint = MyDirect3D.Camera_Position.XZPoint;
			DoublePoint doublePoint = new DoublePoint(Math.Floor(xZPoint.x / 500.0) * 500.0 + (double)(index % 2) * 500.0, Math.Floor(xZPoint.y / 500.0) * 500.0 + (double)(index / 2) * 500.0);
			return Matrix.Scaling(0.5f, 1f, 0.5f) * Matrix.Translation((float)doublePoint.x, -0.1f, (float)doublePoint.y);
		}

		public double GetHeight(DoublePoint pos)
		{
			return -0.1;
		}
	}
}

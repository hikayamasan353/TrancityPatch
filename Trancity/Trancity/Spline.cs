using Common;
using Engine;
using SlimDX;
using SlimDX.Direct3D9;

namespace Trancity
{
	public abstract class Spline : MeshObject, MeshObject.IFromFile, IMatrixObject, MeshObject.ICustomCreation
	{
		public SplineModel model;

		protected MeshVertex[] vertexes;

		protected int[] indexes;

		protected int poly_count;

		public string name;

		public virtual string Filename => model.mesh_filename;

		public virtual int MatricesCount => 0;

		public override void CreateMesh()
		{
			if (string.IsNullOrEmpty(name))
			{
				name = ((this is Рельс) ? "Rails" : "Road");
			}
			if (model == null)
			{
				foreach (SplineModel spline in SplineLoader.splines)
				{
					if (spline.name == name)
					{
						model = spline;
						break;
					}
				}
				if (model == null)
				{
					Logger.Log("SplineLoader", "Spline " + name + " not found!");
					return;
				}
			}
			meshDir = model.dir;
			if (MainForm.in_editor)
			{
				base.CreateMesh();
			}
			else
			{
				CreateCustomMesh();
			}
		}

		public abstract void CreateCustomMesh();

		public virtual Matrix GetMatrix(int index)
		{
			return MyMatrix.Zero;
		}

		public virtual void CustomRender()
		{
			MyDirect3D.device.SetTransform(TransformState.World, ((IMatrixObject)this).GetMatrix(0));
			MyDirect3D.device.Material = _meshMaterials[0];
			MyDirect3D.device.SetTexture(0, _meshTextures[0]);
			MyDirect3D.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, 0, 0, vertexes.Length, poly_count, indexes, Format.Index32, vertexes, 32);
		}
	}
}

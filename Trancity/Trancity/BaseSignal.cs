using Common;
using Engine;
using SlimDX;

namespace Trancity
{
	public abstract class BaseSignal : MeshObject, MeshObject.IFromFile, IMatrixObject
	{
		public ObjectModel model;

		public string name;

		public Положение положение;

		public string Filename => model.filename;

		public virtual int MatricesCount
		{
			get
			{
				if (model == null)
				{
					return 0;
				}
				if (MainForm.in_editor && (положение.Координаты - MyDirect3D.Camera_Position).Modulus > 250.0)
				{
					return 0;
				}
				if (bounding_sphere != null && !MyDirect3D.SphereInFrustum(bounding_sphere))
				{
					return 0;
				}
				return 1;
			}
		}

		protected BaseSignal(string _name, byte type)
		{
			положение = default(Положение);
			name = _name;
			ObjectLoader.FindModel(type, _name, ref model, ref meshDir);
		}

		public Matrix GetMatrix(int index)
		{
			Double3DPoint координаты = положение.Координаты;
			return Matrix.RotationY(0f - (float)положение.Направление) * Matrix.Translation((float)координаты.x, (float)(координаты.y + положение.высота), (float)координаты.z);
		}

		public void CreateBoundingSphere()
		{
			if (model != null)
			{
				bounding_sphere = new Sphere(model.bsphere.pos, model.bsphere.radius);
				Double3DPoint координаты = положение.Координаты;
				координаты.y += положение.высота;
				bounding_sphere.Update(координаты, new DoublePoint(положение.Направление));
			}
		}
	}
}

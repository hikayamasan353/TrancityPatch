using SlimDX;

namespace Trancity
{
	public class Visual_Signal : BaseSignal
	{
		public Сигнальная_система система;

		private int green_mtrl;

		private int red_mtrl;

		public Road road
		{
			get
			{
				return положение.Дорога;
			}
			set
			{
				if (положение.Дорога != null)
				{
					положение.Дорога.objects.Remove(this);
				}
				положение.Дорога = value;
				if (value != null)
				{
					положение.Дорога.objects.Add(this);
				}
			}
		}

		public override int MatricesCount
		{
			get
			{
				int matricesCount = base.MatricesCount;
				if (matricesCount > 0)
				{
					Обновить_материалы();
				}
				return matricesCount;
			}
		}

		public Visual_Signal(Сигнальная_система _signal_system, string _name)
			: base(_name, 3)
		{
			система = _signal_system;
			green_mtrl = model.FindNumericArg("green_mtrl", -1);
			red_mtrl = model.FindNumericArg("red_mtrl", -1);
		}

		public override void CreateMesh()
		{
			if (model != null)
			{
				meshDir = model.dir;
				base.CreateMesh();
				if (green_mtrl >= _meshMaterials.Length)
				{
					green_mtrl = -1;
				}
				if (red_mtrl >= _meshMaterials.Length)
				{
					red_mtrl = -1;
				}
				Обновить_материалы();
			}
		}

		public void Обновить_материалы()
		{
			if (_meshMaterials != null)
			{
				bool flag = система.сигнал == Сигналы.Зелёный;
				if (green_mtrl >= 0)
				{
					Color4 emissive = _meshMaterials[green_mtrl].Emissive;
					emissive.Green = (flag ? 1f : 0f);
					_meshMaterials[green_mtrl].Emissive = emissive;
				}
				if (red_mtrl >= 0)
				{
					Color4 emissive2 = _meshMaterials[red_mtrl].Emissive;
					emissive2.Red = (flag ? 0f : 1f);
					_meshMaterials[red_mtrl].Emissive = emissive2;
				}
			}
		}
	}
}

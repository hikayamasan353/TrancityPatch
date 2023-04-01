using SlimDX;

namespace Trancity
{
	public class Светофор : BaseSignal
	{
		public int жёлтая_стрелка;

		public int зелёная_стрелка;

		public int красная_стрелка;

		private int green_mtrl;

		private int red_mtrl;

		private int yellow_mtrl;

		public int tex_count;

		public Светофор(string name)
			: base(name, 2)
		{
			green_mtrl = model.FindNumericArg("green_mtrl", -1);
			red_mtrl = model.FindNumericArg("red_mtrl", -1);
			yellow_mtrl = model.FindNumericArg("yellow_mtrl", -1);
			tex_count = model.FindNumericArg("custom_tex_count", 0);
		}

		public override void CreateMesh()
		{
			if (model == null)
			{
				return;
			}
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
			if (yellow_mtrl >= _meshMaterials.Length)
			{
				yellow_mtrl = -1;
			}
			if (red_mtrl >= 0 && красная_стрелка != 0)
			{
				string text = model.FindStringArg("tex" + (красная_стрелка - 1), string.Empty);
				if (text != string.Empty)
				{
					LoadTexture(red_mtrl, meshDir + text);
				}
			}
			if (yellow_mtrl >= 0 && жёлтая_стрелка != 0)
			{
				string text2 = model.FindStringArg("tex" + (жёлтая_стрелка - 1), string.Empty);
				if (text2 != string.Empty)
				{
					LoadTexture(yellow_mtrl, meshDir + text2);
				}
			}
			if (green_mtrl >= 0 && зелёная_стрелка != 0)
			{
				string text3 = model.FindStringArg("tex" + (зелёная_стрелка - 1), string.Empty);
				if (text3 != string.Empty)
				{
					LoadTexture(green_mtrl, meshDir + text3);
				}
			}
		}

		public void Custom_render(bool нужен_зелёный, bool нужен_жёлтый, bool нужен_красный)
		{
			if (MatricesCount == 0 && _meshMaterials == null)
			{
				return;
			}
			if (red_mtrl >= 0)
			{
				Color4 emissive = _meshMaterials[red_mtrl].Emissive;
				if (нужен_красный && emissive.Red < 1f)
				{
					emissive.Red += 0.2f;
				}
				if (!нужен_красный && emissive.Red > 0.0001f)
				{
					emissive.Red -= 0.2f;
				}
				_meshMaterials[red_mtrl].Emissive = emissive;
			}
			if (yellow_mtrl >= 0)
			{
				Color4 emissive2 = _meshMaterials[yellow_mtrl].Emissive;
				if (нужен_жёлтый && emissive2.Green < 1f)
				{
					emissive2.Red += 0.2f;
					emissive2.Green += 0.2f;
				}
				if (!нужен_жёлтый && emissive2.Green > 0.0001f)
				{
					emissive2.Red -= 0.2f;
					emissive2.Green -= 0.2f;
				}
				_meshMaterials[yellow_mtrl].Emissive = emissive2;
			}
			if (green_mtrl >= 0)
			{
				Color4 emissive3 = _meshMaterials[green_mtrl].Emissive;
				if (нужен_зелёный && emissive3.Green < 1f)
				{
					emissive3.Green += 0.2f;
				}
				if (!нужен_зелёный && emissive3.Green > 0.0001f)
				{
					emissive3.Green -= 0.2f;
				}
				_meshMaterials[green_mtrl].Emissive = emissive3;
			}
			Render();
		}
	}
}

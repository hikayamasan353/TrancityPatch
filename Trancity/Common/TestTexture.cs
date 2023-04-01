using System.Drawing;
using SlimDX.Direct3D9;

namespace Common
{
	public class TestTexture
	{
		public Texture texture;

		public Size size;

		public TestTexture(string filename)
		{
			try
			{
				texture = Texture.FromFile(MyDirect3D.device, filename);
				using Image image = Image.FromFile(filename);
				size = image.Size;
			}
			catch
			{
				texture = new Texture(MyDirect3D.device, 1, 1, 0, Usage.Dynamic, Format.A8R8G8B8, Pool.Default);
				size = new Size(1, 1);
			}
		}
	}
}

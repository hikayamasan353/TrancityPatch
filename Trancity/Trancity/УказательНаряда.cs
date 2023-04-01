using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Common;
using SlimDX;

namespace Trancity
{
	public class УказательНаряда : MeshObject, MeshObject.IFromFile, IMatrixObject
	{
		public Matrix matrix;

		public string Filename => "order.x";

		public int MatricesCount => 1;

		public Matrix GetMatrix(int index)
		{
			return matrix;
		}

		public void ОбновитьКартинку(Order наряд)
		{
			Font font = new Font("Verdana", 14f);
			Brush brush = new SolidBrush(Color.Black);
			for (int i = 0; i < _meshTextures.Length; i++)
			{
				if (!string.IsNullOrEmpty(_meshTextureFilenames[i]) && !(_meshTextureFilenames[i].ToLower() != "Order.PNG".ToLower()))
				{
					Bitmap bitmap = new Bitmap("Order.PNG");
					Graphics graphics = Graphics.FromImage(bitmap);
					string number = наряд.маршрут.number;
					string номер = наряд.номер;
					graphics.DrawString(number, font, brush, 10f - 0.5f * graphics.MeasureString(number, font).Width, 0f);
					graphics.DrawString(номер, font, brush, 10f - 0.5f * graphics.MeasureString(номер, font).Width, 21f);
					Stream stream = new MemoryStream();
					bitmap.Save(stream, ImageFormat.Bmp);
					stream.Seek(0L, SeekOrigin.Begin);
					LoadTextureFromStream(i, stream);
				}
			}
		}
	}
}

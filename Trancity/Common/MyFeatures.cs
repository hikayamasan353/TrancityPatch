using System;
using System.IO;
using System.Windows.Forms;
using Engine;
using SlimDX;
using SlimDX.Direct3D9;
using Trancity;

namespace Common
{
	public class MyFeatures
	{
		private static bool screenshot_requested;

		public static double Lerp(double a, double b, double t)
		{
			return a + t * (b - a);
		}

		public static int[] GetPos(ref DoublePoint pos)
		{
			int[] array = new int[2]
			{
				(int)Math.Floor((pos.x + (double)Ground.grid_size / 2.0) / (double)Ground.grid_size),
				(int)Math.Floor((pos.y + (double)Ground.grid_size / 2.0) / (double)Ground.grid_size)
			};
			pos.x -= array[0] * Ground.grid_size;
			pos.y -= array[1] * Ground.grid_size;
			return array;
		}

		public static void CheckFolders(string startup_path)
		{
			TryToCreateFolder(startup_path + "\\Cities\\");
			TryToCreateFolder(startup_path + "\\Data\\Splines\\");
			TryToCreateFolder(startup_path + "\\Data\\Skybox\\");
			TryToCreateFolder(startup_path + "\\Data\\Localization\\");
			TryToCreateFolder(startup_path + "\\Data\\Transport\\");
			TryToCreateFolder(startup_path + "\\Data\\Objects\\");
			TryToCreateFolder(startup_path + "\\Screenshots\\");
		}

		private static void TryToCreateFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (Exception exception)
				{
					Logger.LogException(exception);
				}
			}
		}

		public static void MakeScreenshot(bool request)
		{
			if (!screenshot_requested)
			{
				screenshot_requested = request;
				return;
			}
			screenshot_requested = request;
			DateTime now = DateTime.Now;
			string text = Application.StartupPath + "\\Screenshots\\";
			string fileName = $"{text}\\Trancity {now.Day:00}-{now.Month:00}-{now.Year} {now.Hour:00}-{now.Minute:00}-{now.Second:00}-{now.Millisecond:000}.png";
			using Surface surface = MyDirect3D.device.GetBackBuffer(0, 0);
			using Surface surface2 = Surface.CreateOffscreenPlain(MyDirect3D.device, surface.Description.Width, surface.Description.Height, Format.X8R8G8B8, Pool.Scratch);
			Surface.FromSurface(surface2, surface, Filter.Default, 0);
			Surface.ToFile(surface2, fileName, ImageFileFormat.Png);
		}

		public static Vector3 ToVector3(Double3DPoint a)
		{
			return new Vector3((float)a.x, (float)a.y, (float)a.z);
		}

		public static Double3DPoint ToDouble3DPoint(Vector3 a)
		{
			return new Double3DPoint(a.X, a.Y, a.Z);
		}
	}
}

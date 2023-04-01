using System.Runtime.InteropServices;
using Engine;

namespace Trancity
{
	[StructLayout(LayoutKind.Sequential)]
	public class SplineModel
	{
		public string dir;

		public string name;

		public bool noscale;

		public double length;

		public string texture_filename;

		public Double3DPoint[] points = new Double3DPoint[0];

		public string mesh_filename;

		public int type;
	}
}

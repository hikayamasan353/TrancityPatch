namespace Common
{
	public class RawMouseState
	{
		public bool[] RawMoseButtons = new bool[5];

		public int X;

		public int Y;

		public int Z;

		public RawMouseState()
		{
			X = 0;
			Y = 0;
			Z = 0;
		}
	}
}

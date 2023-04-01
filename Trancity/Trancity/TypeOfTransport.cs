using System;

namespace Trancity
{
	public class TypeOfTransport
	{
		public const int Tramway = 0;

		public const int Trolleybus = 1;

		public const int Bus = 2;

		private bool[] type = new bool[3];

		public bool this[int index]
		{
			get
			{
				if (index < 0 && index > 2)
				{
					throw new IndexOutOfRangeException("Invalid type of transport");
				}
				return type[index];
			}
			set
			{
				if (index < 0 && index > 2)
				{
					throw new IndexOutOfRangeException("Invalid type of transport");
				}
				switch (index)
				{
				case 0:
					type[0] = value;
					type[1] = !value;
					type[2] = !value;
					break;
				case 1:
					type[0] = !value && !type[2];
					type[1] = value;
					break;
				case 2:
					type[0] = !value && !type[1];
					type[2] = value;
					break;
				}
			}
		}

		public TypeOfTransport()
			: this(0)
		{
		}

		public TypeOfTransport(int p)
		{
			this[p] = true;
		}
	}
}

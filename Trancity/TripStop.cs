namespace Trancity
{
	public class TripStop
	{
		public class Editable
		{
			private Stop stop;

			public bool ShouldStop { get; set; }

			public string StopName
			{
				get
				{
					if (stop != null)
					{
						return stop.название;
					}
					return "null";
				}
			}

			public static Editable FromTripStop(TripStop tripStop)
			{
				return new Editable
				{
					ShouldStop = tripStop.active,
					stop = tripStop.stop
				};
			}

			public TripStop ToTripStop()
			{
				return new TripStop(stop, ShouldStop);
			}
		}

		public readonly Stop stop;

		public readonly bool active;

		public TripStop(Stop stop, bool active)
		{
			this.stop = stop;
			this.active = active;
		}
	}
}

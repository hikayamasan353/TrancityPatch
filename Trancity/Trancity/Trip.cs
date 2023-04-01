using System.Collections.Generic;

namespace Trancity
{
	public class Trip
	{
		public bool inPark;

		public int inParkIndex;

		public double время_отправления;

		public double время_прибытия;

		public Road[] pathes = new Road[0];

		public List<TripStop> tripStopList;

		public Route route;

		public string str_время_отправления
		{
			get
			{
				int num = (int)время_отправления / 3600 % 24;
				int num2 = (int)время_отправления / 60 % 60;
				return num.ToString("0") + ":" + num2.ToString("00");
			}
		}

		public string str_время_прибытия
		{
			get
			{
				int num = (int)время_прибытия / 3600 % 24;
				int num2 = (int)время_прибытия / 60 % 60;
				return num.ToString("0") + ":" + num2.ToString("00");
			}
		}

		public double длина_пути
		{
			get
			{
				double num = 0.0;
				Road[] array = pathes;
				foreach (Road road in array)
				{
					num += road.Длина;
				}
				return num;
			}
		}

		public Road дорога_отправления
		{
			get
			{
				if (pathes.Length != 0)
				{
					return pathes[0];
				}
				return null;
			}
		}

		public Road дорога_прибытия
		{
			get
			{
				if (pathes.Length != 0)
				{
					return pathes[pathes.Length - 1];
				}
				return null;
			}
		}

		public Trip Clone(double время_отправления)
		{
			Trip trip = new Trip();
			trip.pathes = pathes;
			trip.inPark = inPark;
			trip.inParkIndex = inParkIndex;
			trip.pathes = pathes;
			trip.время_отправления = время_отправления;
			trip.время_прибытия = время_отправления + время_прибытия;
			trip.tripStopList = tripStopList;
			trip.route = route;
			return trip;
		}

		public void InitTripStopList()
		{
			tripStopList = AddToTripStopList();
		}

		public List<TripStop> AddToTripStopList()
		{
			List<TripStop> list = new List<TripStop>();
			bool flag = false;
			Road[] array = pathes;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (object @object in array[i].objects)
				{
					if (!(@object is Stop stop) || !stop.typeOfTransport[route.typeOfTransport])
					{
						continue;
					}
					if (flag)
					{
						int j;
						for (j = 0; (j < list.Count - 1 || (list.Count == 1 && j == 0)) && list[list.Count - 1 - j].stop.distance > stop.distance && list[list.Count - 1 - j].stop.road == stop.road; j++)
						{
						}
						list.Insert(list.Count - j, new TripStop(stop, active: true));
					}
					else
					{
						list.Add(new TripStop(stop, active: true));
						flag = true;
					}
				}
				flag = false;
			}
			return list;
		}
	}
}

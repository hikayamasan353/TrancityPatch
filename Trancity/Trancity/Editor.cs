using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common;
using Engine;
using Engine.Controls;
using SlimDX.Direct3D9;

namespace Trancity
{
	public class Editor : Form
	{
		private class AddRoadsAction : EditorAction
		{
			private Road[] _roads;

			public AddRoadsAction(params Road[] roads)
			{
				_roads = roads;
			}

			public override void Do()
			{
				for (int i = 0; i < _roads.Length; i++)
				{
					_parent.мир.listДороги.Add(_roads[i]);
				}
				Road[] всеДороги = _parent.мир.ВсеДороги;
				for (int j = 0; j < всеДороги.Length; j++)
				{
					всеДороги[j].ОбновитьСледующиеДороги(_parent.мир.ВсеДороги);
				}
				_parent.UpdateSplinesList();
			}

			public override void Undo()
			{
				for (int num = _roads.Length - 1; num >= 0; num--)
				{
					_parent.мир.listДороги.Remove(_roads[num]);
				}
				Road[] всеДороги = _parent.мир.ВсеДороги;
				for (int i = 0; i < всеДороги.Length; i++)
				{
					всеДороги[i].ОбновитьСледующиеДороги(_parent.мир.ВсеДороги);
				}
				_parent.UpdateSplinesList();
			}
		}

		private class RemoveRoadAction : EditorAction
		{
			private int _index;

			private Road _road;

			public RemoveRoadAction(Road road)
			{
				_road = road;
			}

			public RemoveRoadAction(int index)
			{
				_index = index;
			}

			public override void Do()
			{
				if (_road != null)
				{
					_index = _parent.мир.listДороги.IndexOf(_road);
				}
				else
				{
					_road = (Road)_parent.мир.listДороги[_index];
				}
				_parent.мир.listДороги.RemoveAt(_index);
				Road[] всеДороги = _parent.мир.ВсеДороги;
				for (int i = 0; i < всеДороги.Length; i++)
				{
					всеДороги[i].ОбновитьСледующиеДороги(_parent.мир.ВсеДороги);
				}
				_parent.UpdateSplinesList();
			}

			public override void Undo()
			{
				_parent.мир.listДороги.Insert(_index, _road);
				Road[] всеДороги = _parent.мир.ВсеДороги;
				for (int i = 0; i < всеДороги.Length; i++)
				{
					всеДороги[i].ОбновитьСледующиеДороги(_parent.мир.ВсеДороги);
				}
				_parent.UpdateSplinesList();
			}
		}

		private class AddWiresAction : EditorAction
		{
			private Контактный_провод[] _wires;

			public AddWiresAction(params Контактный_провод[] wires)
			{
				_wires = wires;
			}

			public override void Do()
			{
				List<Контактный_провод> list = new List<Контактный_провод>(_parent.мир.контактныеПровода);
				list.AddRange(_wires);
				_parent.мир.контактныеПровода = list.ToArray();
			}

			public override void Undo()
			{
				List<Контактный_провод> list = new List<Контактный_провод>(_parent.мир.контактныеПровода);
				Контактный_провод[] wires = _wires;
				foreach (Контактный_провод item in wires)
				{
					list.Remove(item);
				}
				_parent.мир.контактныеПровода = list.ToArray();
			}
		}

		private class AddTramWireAction : EditorAction
		{
			private Трамвайный_контактный_провод _wire;

			public AddTramWireAction(Трамвайный_контактный_провод wire)
			{
				_wire = wire;
			}

			public override void Do()
			{
				List<Трамвайный_контактный_провод> list = new List<Трамвайный_контактный_провод>(_parent.мир.контактныеПровода2);
				list.Add(_wire);
				_parent.мир.контактныеПровода2 = list.ToArray();
			}

			public override void Undo()
			{
				List<Трамвайный_контактный_провод> list = new List<Трамвайный_контактный_провод>(_parent.мир.контактныеПровода2);
				list.Remove(_wire);
				_parent.мир.контактныеПровода2 = list.ToArray();
			}
		}

		private class RemoveWiresAction : EditorAction
		{
			private Контактный_провод[] _wires;

			public RemoveWiresAction(params Контактный_провод[] wires)
			{
				_wires = wires;
			}

			public override void Do()
			{
				List<Контактный_провод> list = new List<Контактный_провод>(_parent.мир.контактныеПровода);
				Контактный_провод[] wires = _wires;
				foreach (Контактный_провод item in wires)
				{
					list.Remove(item);
				}
				_parent.мир.контактныеПровода = list.ToArray();
			}

			public override void Undo()
			{
				List<Контактный_провод> list = new List<Контактный_провод>(_parent.мир.контактныеПровода);
				list.AddRange(_wires);
				_parent.мир.контактныеПровода = list.ToArray();
			}
		}

		private class RemoveTramWireAction : EditorAction
		{
			private Трамвайный_контактный_провод _wire;

			public RemoveTramWireAction(Трамвайный_контактный_провод wire)
			{
				_wire = wire;
			}

			public override void Do()
			{
				List<Трамвайный_контактный_провод> list = new List<Трамвайный_контактный_провод>(_parent.мир.контактныеПровода2);
				list.Remove(_wire);
				_parent.мир.контактныеПровода2 = list.ToArray();
			}

			public override void Undo()
			{
				List<Трамвайный_контактный_провод> list = new List<Трамвайный_контактный_провод>(_parent.мир.контактныеПровода2);
				list.Add(_wire);
				_parent.мир.контактныеПровода2 = list.ToArray();
			}
		}

		private class AddStopAction : EditorAction
		{
			private Stop _stop;

			public AddStopAction(Stop stop)
			{
				_stop = stop;
			}

			public override void Do()
			{
				_parent.мир.остановки.Add(_stop);
				_parent.UpdateStopsList();
				_parent.Stops_Box.SelectedIndex = _parent.Stops_Box.Items.Count - 1;
			}

			public override void Undo()
			{
				int num = _parent.мир.остановки.IndexOf(_stop);
				if (num != _parent.мир.остановки.Count - 1)
				{
					throw new InvalidOperationException("Stops array does not ends with added stop");
				}
				_parent.мир.остановки.RemoveAt(num);
				_parent.UpdateStopsList();
			}
		}

		private class MoveStopAction : EditorAction
		{
			private Stop _stop;

			private Road _initialSpline;

			private Road _newSpline;

			private double _initialDist;

			private double _newDist;

			public MoveStopAction(Stop obj)
			{
				_stop = obj;
				_initialSpline = obj.road;
				_initialDist = obj.distance;
			}

			public override void Do()
			{
				if (_newSpline == null)
				{
					_newSpline = _stop.road;
					_newDist = _stop.distance;
				}
				else
				{
					_stop.road = _newSpline;
					_stop.distance = _newDist;
					_stop.UpdatePosition(_parent.мир);
				}
			}

			public override void Undo()
			{
				_stop.road = _initialSpline;
				_stop.distance = _initialDist;
				_stop.UpdatePosition(_parent.мир);
			}
		}

		private class RemoveStopAction : EditorAction
		{
			private int _index;

			private Stop _object;

			public RemoveStopAction(int index)
			{
				_index = index;
			}

			public override void Do()
			{
				_object = _parent.мир.остановки[_index];
				_parent.мир.остановки.RemoveAt(_index);
				_parent.UpdateStopsList();
			}

			public override void Undo()
			{
				_parent.мир.остановки.Insert(_index, _object);
				_parent.UpdateStopsList();
			}
		}

		private class SetTripStopListAction : EditorAction
		{
			private Trip _trip;

			private List<TripStop> _oldTripStops;

			private List<TripStop> _newTripStops;

			public SetTripStopListAction(Trip trip, List<TripStop> newTripStop)
			{
				_trip = trip;
				_newTripStops = newTripStop;
			}

			public override void Do()
			{
				_oldTripStops = _trip.tripStopList;
				_trip.tripStopList = _newTripStops;
			}

			public override void Undo()
			{
				_trip.tripStopList = _oldTripStops;
			}
		}

		private class AddDepotAction : EditorAction
		{
			private Парк _depot;

			public AddDepotAction(Парк depot)
			{
				_depot = depot;
			}

			public override void Do()
			{
				List<Парк> list = new List<Парк>(_parent.мир.парки);
				list.Add(_depot);
				_parent.мир.парки = list.ToArray();
				_parent.UpdateParksList();
				_parent.Park_Box.SelectedIndex = _parent.Park_Box.Items.Count - 1;
				_parent.Narad_Box_SelectedIndexChanged(null, null);
			}

			public override void Undo()
			{
				int num = _parent.Park_Box.SelectedIndex;
				List<Парк> list = new List<Парк>(_parent.мир.парки);
				list.Remove(_depot);
				_parent.мир.парки = list.ToArray();
				_parent.UpdateParksList();
				if (num >= list.Count)
				{
					num--;
				}
				_parent.Park_Box.SelectedIndex = num;
				_parent.Narad_Box_SelectedIndexChanged(null, null);
			}
		}

		private class RemoveDepotAction : EditorAction
		{
			private int _index;

			private Парк _depot;

			public RemoveDepotAction(int index)
			{
				_index = index;
			}

			public override void Do()
			{
				List<Парк> list = new List<Парк>(_parent.мир.парки);
				_depot = _depot ?? list[_index];
				list.RemoveAt(_index);
				_parent.мир.парки = list.ToArray();
				_parent.UpdateParksList();
				_parent.Park_Box.SelectedIndex = _index - 1;
				_parent.Narad_Box_SelectedIndexChanged(null, null);
			}

			public override void Undo()
			{
				List<Парк> list = new List<Парк>(_parent.мир.парки);
				list.Insert(_index, _depot);
				_parent.мир.парки = list.ToArray();
				_parent.UpdateParksList();
				_parent.Park_Box.SelectedIndex = _index;
				_parent.Narad_Box_SelectedIndexChanged(null, null);
			}
		}

		private class SetDepotEnterAction : EditorAction
		{
			private Парк depot;

			private Road oldEnter;

			private Road newEnter;

			public SetDepotEnterAction(Парк depot, Road newEnter)
			{
				if (depot == null)
				{
					throw new ArgumentException("depot can not be null");
				}
				this.depot = depot;
				this.newEnter = newEnter;
				oldEnter = depot.въезд;
			}

			public override void Do()
			{
				depot.въезд = newEnter;
				_parent.ОбновитьРаскрашенныеСплайны();
			}

			public override void Undo()
			{
				depot.въезд = oldEnter;
				_parent.ОбновитьРаскрашенныеСплайны();
			}
		}

		private class SetDepotExitAction : EditorAction
		{
			private Парк depot;

			private Road oldExit;

			private Road newExit;

			public SetDepotExitAction(Парк depot, Road newExit)
			{
				if (depot == null)
				{
					throw new ArgumentException("depot can not be null");
				}
				this.depot = depot;
				this.newExit = newExit;
				oldExit = depot.выезд;
			}

			public override void Do()
			{
				depot.выезд = newExit;
				_parent.ОбновитьРаскрашенныеСплайны();
			}

			public override void Undo()
			{
				depot.выезд = oldExit;
				_parent.ОбновитьРаскрашенныеСплайны();
			}
		}

		private class ToggleDepotPathAction : EditorAction
		{
			private Парк depot;

			private Road road;

			private int roadIndex = -1;

			public ToggleDepotPathAction(Парк depot, Road road)
			{
				if (depot == null)
				{
					throw new ArgumentException("depot can not be null");
				}
				this.depot = depot;
				this.road = road;
			}

			public override void Do()
			{
				roadIndex = -1;
				for (int i = 0; i < depot.пути_стоянки.Length; i++)
				{
					if (depot.пути_стоянки[i] == road)
					{
						roadIndex = i;
						break;
					}
				}
				List<Road> list = new List<Road>(depot.пути_стоянки);
				if (roadIndex >= 0)
				{
					list.RemoveAt(roadIndex);
				}
				else
				{
					list.Add(road);
				}
				depot.пути_стоянки = list.ToArray();
				_parent.ОбновитьРаскрашенныеСплайны();
			}

			public override void Undo()
			{
				List<Road> list = new List<Road>(depot.пути_стоянки);
				if (roadIndex < 0)
				{
					if (list.Count == 0)
					{
						throw new InvalidOperationException("there is no depot roads, nothing to remove");
					}
					list.Remove(road);
				}
				else
				{
					list.Insert(roadIndex, road);
				}
				depot.пути_стоянки = list.ToArray();
				_parent.ОбновитьРаскрашенныеСплайны();
			}
		}

		private class AddRouteAction : EditorAction
		{
			private Route route;

			public AddRouteAction(Route route)
			{
				this.route = route;
			}

			public override void Do()
			{
				List<Route> list = new List<Route>(_parent.мир.маршруты);
				list.Add(route);
				_parent.мир.маршруты = list.ToArray();
				_parent.Route_Box.Items.Add(route.number);
				_parent.Route_Box.SelectedIndex = _parent.Route_Box.Items.Count - 1;
			}

			public override void Undo()
			{
				List<Route> list = new List<Route>(_parent.мир.маршруты);
				int index = list.Count - 1;
				if (list[index] != route)
				{
					throw new InvalidOperationException("last route in list is not lastly added");
				}
				list.RemoveAt(index);
				_parent.мир.маршруты = list.ToArray();
				_parent.Route_Box.Items.RemoveAt(index);
				_parent.RouteBoxSelectedIndexChanged(null, new EventArgs());
			}
		}

		private class RemoveRouteAction : EditorAction
		{
			private Route route;

			private int idx;

			public RemoveRouteAction(int idx)
			{
				this.idx = idx;
			}

			public override void Do()
			{
				route = _parent.мир.маршруты[idx];
				List<Route> list = new List<Route>(_parent.мир.маршруты);
				list.RemoveAt(idx);
				_parent.мир.маршруты = list.ToArray();
				_parent.Route_Box.Items.RemoveAt(idx);
				_parent.RouteBoxSelectedIndexChanged(null, new EventArgs());
			}

			public override void Undo()
			{
				if (route == null)
				{
					throw new InvalidOperationException("Undo called for unknown route");
				}
				List<Route> list = new List<Route>(_parent.мир.маршруты);
				list.Add(route);
				_parent.мир.маршруты = list.ToArray();
				_parent.Route_Box.Items.Add(route.number);
				_parent.Route_Box.SelectedIndex = _parent.Route_Box.Items.Count - 1;
			}
		}

		private class AddRouteTripAction : EditorAction
		{
			private Route route;

			private Trip trip;

			public AddRouteTripAction(Route route, Trip trip)
			{
				this.route = route;
				this.trip = trip;
			}

			public override void Do()
			{
				route.trips.Add(trip);
				int num = route.trips.Count - 1;
				int num2 = num + 1;
				if (_parent.выбранный_маршрут == route)
				{
					_parent.Route_Runs_Box.Items.Insert(num, "Рейс " + num2);
					_parent.Route_Runs_Box.SelectedIndex = num;
					_parent.Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}

			public override void Undo()
			{
				int index = route.trips.Count - 1;
				if (route.trips[index] != trip)
				{
					throw new InvalidOperationException("last trip in list is not previously added one");
				}
				route.trips.RemoveAt(index);
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Route_Runs_Box.Items.RemoveAt(index);
					_parent.Route_Runs_Box_SelectedIndexChanged(null, new EventArgs());
					_parent.Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}
		}

		private class RemoveRouteTripAction : EditorAction
		{
			private Route route;

			private Trip trip;

			private int tripIdx;

			private object tripName;

			public RemoveRouteTripAction(Route route, int tripIdx)
			{
				this.route = route;
				this.tripIdx = tripIdx;
			}

			public override void Do()
			{
				if (tripIdx < route.trips.Count)
				{
					trip = route.trips[tripIdx];
					route.trips.RemoveAt(tripIdx);
				}
				else
				{
					trip = route.parkTrips[tripIdx - route.trips.Count];
					route.parkTrips.RemoveAt(tripIdx - route.trips.Count);
				}
				if (route == _parent.выбранный_маршрут)
				{
					tripName = _parent.Route_Runs_Box.Items[tripIdx];
					_parent.Route_Runs_Box.Items.RemoveAt(tripIdx);
					_parent.Route_Runs_Box_SelectedIndexChanged(null, new EventArgs());
					_parent.Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}

			public override void Undo()
			{
				if (trip.inPark)
				{
					route.parkTrips.Insert(tripIdx - route.trips.Count, trip);
				}
				else
				{
					route.trips.Insert(tripIdx, trip);
				}
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Route_Runs_Box.Items.Insert(tripIdx, tripName);
				}
			}
		}

		private class ChangeRouteTripTypeAction : EditorAction
		{
			private Route route;

			private Trip trip;

			private int idx;

			public ChangeRouteTripTypeAction(Route route, Trip trip)
			{
				this.route = route;
				this.trip = trip;
			}

			private void toggleTripType()
			{
				idx = route.AllTrips.IndexOf(trip);
				if (_parent.выбранный_маршрут == route)
				{
					_parent.Route_Runs_Box.Items.RemoveAt(idx);
				}
				if (idx < route.trips.Count)
				{
					route.trips.Remove(trip);
					route.parkTrips.Add(trip);
					if (_parent.выбранный_маршрут == route)
					{
						_parent.Route_Runs_Box.Items.Add("Парковый рейс " + route.parkTrips.Count);
						_parent.Route_Runs_Box.SelectedIndex = _parent.Route_Runs_Box.Items.Count - 1;
					}
					return;
				}
				route.parkTrips.Remove(trip);
				route.trips.Add(trip);
				trip.inPark = false;
				if (_parent.выбранный_маршрут == route)
				{
					int num = route.trips.Count - 1;
					_parent.Route_Runs_Box.Items.Insert(num, "Рейс " + (num + 1));
					_parent.Route_Runs_Box.SelectedIndex = num;
				}
			}

			public override void Do()
			{
				toggleTripType();
			}

			public override void Undo()
			{
				toggleTripType();
			}
		}

		private class AddRouteOrderAction : EditorAction
		{
			private Route route;

			private Order order;

			public AddRouteOrderAction(Route route, Order order)
			{
				this.route = route;
				this.order = order;
			}

			public override void Do()
			{
				List<Order> list = new List<Order>(route.orders);
				list.Add(order);
				route.orders = list.ToArray();
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Narad_Box.Items.Add(order.номер);
					_parent.Narad_Box.SelectedIndex = _parent.Narad_Box.Items.Count - 1;
				}
			}

			public override void Undo()
			{
				if (route.orders[route.orders.Length - 1] != order)
				{
					throw new InvalidOperationException("unexppected last added route order");
				}
				List<Order> list = new List<Order>(route.orders);
				int index = list.Count - 1;
				list.RemoveAt(index);
				route.orders = list.ToArray();
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Narad_Box.Items.RemoveAt(index);
					_parent.Narad_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}
		}

		private class RemoveRouteOrderAction : EditorAction
		{
			private Route route;

			private Order order;

			private int idx;

			public RemoveRouteOrderAction(Route route, int idx)
			{
				this.route = route;
				this.idx = idx;
			}

			public override void Do()
			{
				List<Order> list = new List<Order>(route.orders);
				order = list[idx];
				list.RemoveAt(idx);
				route.orders = list.ToArray();
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Narad_Box.Items.RemoveAt(idx);
					_parent.Narad_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}

			public override void Undo()
			{
				List<Order> list = new List<Order>(route.orders);
				list.Insert(idx, order);
				route.orders = list.ToArray();
				if (route == _parent.выбранный_маршрут)
				{
					_parent.Narad_Box.Items.Insert(idx, order.номер);
					_parent.Narad_Box.SelectedIndex = idx;
				}
			}
		}

		private class AddOrderTripAction : EditorAction
		{
			private Order order;

			private Trip trip;

			private int idx;

			public AddOrderTripAction(Order order, Trip trip)
			{
				this.order = order;
				this.trip = trip;
			}

			public override void Do()
			{
				List<Trip> list = new List<Trip>(order.рейсы);
				list.Add(trip);
				order.рейсы = list.ToArray();
				idx = order.рейсы.Length - 1;
				if (order == _parent.выбранный_наряд)
				{
					_parent.Narad_Runs_Box.Items.Add("Рейс " + (idx + 1));
					_parent.Narad_Runs_Box.SelectedIndex = _parent.Narad_Runs_Box.Items.Count - 1;
				}
			}

			public override void Undo()
			{
				List<Trip> list = new List<Trip>(order.рейсы);
				list.RemoveAt(idx);
				order.рейсы = list.ToArray();
				if (order == _parent.выбранный_наряд)
				{
					_parent.Narad_Runs_Box.Items.RemoveAt(idx);
					_parent.Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}
		}

		private class RemoveOrderTripAction : EditorAction
		{
			private Order order;

			private Trip trip;

			private int idx;

			public RemoveOrderTripAction(Order order, int idx)
			{
				this.order = order;
				this.idx = idx;
			}

			public override void Do()
			{
				List<Trip> list = new List<Trip>(order.рейсы);
				trip = list[idx];
				list.RemoveAt(idx);
				order.рейсы = list.ToArray();
				if (order == _parent.выбранный_наряд)
				{
					_parent.Narad_Runs_Box.Items.RemoveAt(idx);
					_parent.Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				}
			}

			public override void Undo()
			{
				List<Trip> list = new List<Trip>(order.рейсы);
				list.Insert(idx, trip);
				order.рейсы = list.ToArray();
				if (order == _parent.выбранный_наряд)
				{
					_parent.Narad_Runs_Box.Items.Insert(idx, "Рейс " + (idx + 1));
					_parent.Narad_Runs_Box.SelectedIndex = idx;
				}
			}
		}

		private class AddSignalSystemAction : EditorAction
		{
			private Сигнальная_система signalSystem;

			public AddSignalSystemAction(Сигнальная_система s)
			{
				signalSystem = s;
			}

			public override void Do()
			{
				List<Сигнальная_система> list = new List<Сигнальная_система>(_parent.мир.сигнальныеСистемы);
				list.Add(signalSystem);
				_parent.мир.сигнальныеСистемы = list.ToArray();
				_parent.Signals_Box.Items.Add("Система " + list.Count);
				_parent.Signals_Box.SelectedIndex = _parent.Signals_Box.Items.Count - 1;
			}

			public override void Undo()
			{
				int num = _parent.мир.сигнальныеСистемы.Length - 1;
				if (_parent.мир.сигнальныеСистемы[num] != signalSystem)
				{
					throw new InvalidOperationException("last known added signal system is wrong");
				}
				List<Сигнальная_система> list = new List<Сигнальная_система>(_parent.мир.сигнальныеСистемы);
				list.RemoveAt(num);
				_parent.мир.сигнальныеСистемы = list.ToArray();
				_parent.Signals_Box.Items.RemoveAt(num);
				_parent.Signals_Box_SelectedIndexChanged(null, null);
			}
		}

		private class RemoveSignalSystemAction : EditorAction
		{
			private int idx;

			private Сигнальная_система signalSystem;

			public RemoveSignalSystemAction(int idx)
			{
				this.idx = idx;
			}

			public override void Do()
			{
				signalSystem = _parent.мир.сигнальныеСистемы[idx];
				List<Сигнальная_система> list = new List<Сигнальная_система>(_parent.мир.сигнальныеСистемы);
				list.RemoveAt(idx);
				_parent.мир.сигнальныеСистемы = list.ToArray();
				_parent.Signals_Box.Items.RemoveAt(idx);
				_parent.Signals_Box_SelectedIndexChanged(null, null);
			}

			public override void Undo()
			{
				List<Сигнальная_система> list = new List<Сигнальная_система>(_parent.мир.сигнальныеСистемы);
				list.Insert(idx, signalSystem);
				_parent.мир.сигнальныеСистемы = list.ToArray();
				_parent.Signals_Box.Items.Add("Система " + (idx + 1));
				_parent.Signals_Box.SelectedIndex = idx;
			}
		}

		private class AddSignalSystemContactAction : EditorAction
		{
			private Сигнальная_система signals;

			private Сигнальная_система.Контакт contact;

			public AddSignalSystemContactAction(Сигнальная_система s, Сигнальная_система.Контакт c)
			{
				signals = s;
				contact = c;
			}

			public override void Do()
			{
				signals.Добавить_элемент(contact);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = _parent.Signals_Element_Box.Items.Count - 1;
				}
			}

			public override void Undo()
			{
				signals.Убрать_элемент(contact);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = _parent.Signals_Element_Box.Items.Count - 1;
				}
			}
		}

		private class AddSignalSystemLightAction : EditorAction
		{
			private Сигнальная_система signals;

			private Visual_Signal light;

			public AddSignalSystemLightAction(Сигнальная_система s, Visual_Signal l)
			{
				signals = s;
				light = l;
			}

			public override void Do()
			{
				signals.Добавить_сигнал(light);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = signals.vsignals.Count - 1;
				}
			}

			public override void Undo()
			{
				signals.Убрать_сигнал(light);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = signals.vsignals.Count - 1;
				}
			}
		}

		private class MoveSignalSystemContactAction : EditorAction
		{
			private Сигнальная_система.Контакт contact;

			private Road initialSpline;

			private Road newSpline;

			private double initialDist;

			private double newDist;

			public MoveSignalSystemContactAction(Сигнальная_система.Контакт c)
			{
				contact = c;
				initialSpline = c.дорога;
				initialDist = c.расстояние;
			}

			public override void Do()
			{
				if (newSpline == null)
				{
					newSpline = contact.дорога;
					newDist = contact.расстояние;
				}
				else
				{
					contact.дорога = newSpline;
					contact.расстояние = newDist;
				}
			}

			public override void Undo()
			{
				contact.дорога = initialSpline;
				contact.расстояние = initialDist;
			}
		}

		private class MoveSignalSystemLightAction : EditorAction
		{
			private Visual_Signal light;

			private Road initialSpline;

			private Road newSpline;

			private Положение initialPlacement;

			private Положение newPlacement;

			public MoveSignalSystemLightAction(Visual_Signal c)
			{
				light = c;
				initialSpline = c.road;
				initialPlacement = c.положение;
			}

			public override void Do()
			{
				if (newSpline == null)
				{
					newSpline = light.road;
					newPlacement = light.положение;
				}
				else
				{
					light.road = newSpline;
					light.положение = newPlacement;
				}
			}

			public override void Undo()
			{
				light.road = initialSpline;
				light.положение = initialPlacement;
			}
		}

		private class RemoveSignalSystemContactAction : EditorAction
		{
			private Road road;

			private Сигнальная_система signals;

			private Сигнальная_система.Контакт contact;

			private int idx;

			private int ridx;

			public RemoveSignalSystemContactAction(Сигнальная_система s, Сигнальная_система.Контакт c, int i)
			{
				signals = s;
				contact = c;
				idx = i;
			}

			public override void Do()
			{
				road = contact.дорога;
				ridx = signals.Индекс_элемента(contact);
				signals.Убрать_элемент(contact);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = idx - 1;
				}
			}

			public override void Undo()
			{
				signals.Вставить_элемент(ridx, contact);
				contact.дорога = road;
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = idx;
				}
			}
		}

		private class RemoveSignalSystemLightAction : EditorAction
		{
			private Road road;

			private Сигнальная_система signals;

			private Visual_Signal light;

			private int idx;

			private int ridx;

			public RemoveSignalSystemLightAction(Сигнальная_система s, Visual_Signal l, int i)
			{
				signals = s;
				light = l;
				idx = i;
			}

			public override void Do()
			{
				road = light.road;
				ridx = signals.Индекс_сигнала(light);
				signals.Убрать_сигнал(light);
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = idx - 1;
				}
			}

			public override void Undo()
			{
				signals.Вставить_сигнал(ridx, light);
				light.road = road;
				if (_parent.выбранная_сигнальная_система == signals)
				{
					_parent.UpdateSignalsControls(signals);
					_parent.Signals_Element_Box.SelectedIndex = idx;
				}
			}
		}

		private class AddLightSystemAction : EditorAction
		{
			private Светофорная_система lightSystem;

			public AddLightSystemAction(Светофорная_система s)
			{
				lightSystem = s;
			}

			public override void Do()
			{
				List<Светофорная_система> list = new List<Светофорная_система>(_parent.мир.светофорныеСистемы);
				list.Add(lightSystem);
				_parent.мир.светофорныеСистемы = list.ToArray();
				_parent.Svetofor_Box.Items.Add("Система " + list.Count);
				_parent.Svetofor_Box.SelectedIndex = _parent.Svetofor_Box.Items.Count - 1;
			}

			public override void Undo()
			{
				int num = _parent.мир.светофорныеСистемы.Length - 1;
				if (_parent.мир.светофорныеСистемы[num] != lightSystem)
				{
					throw new InvalidOperationException("last known added light system is wrong");
				}
				List<Светофорная_система> list = new List<Светофорная_система>(_parent.мир.светофорныеСистемы);
				list.RemoveAt(num);
				_parent.мир.светофорныеСистемы = list.ToArray();
				_parent.Svetofor_Box.Items.RemoveAt(num);
				_parent.Svetofor_Box_SelectedIndexChanged(null, new EventArgs());
			}
		}

		private class RemoveLightSystemAction : EditorAction
		{
			private int idx;

			private Светофорная_система lightSystem;

			public RemoveLightSystemAction(int idx)
			{
				this.idx = idx;
			}

			public override void Do()
			{
				lightSystem = _parent.мир.светофорныеСистемы[idx];
				List<Светофорная_система> list = new List<Светофорная_система>(_parent.мир.светофорныеСистемы);
				list.RemoveAt(idx);
				_parent.мир.светофорныеСистемы = list.ToArray();
				_parent.Svetofor_Box.Items.RemoveAt(idx);
				_parent.Svetofor_Box_SelectedIndexChanged(null, new EventArgs());
			}

			public override void Undo()
			{
				List<Светофорная_система> list = new List<Светофорная_система>(_parent.мир.светофорныеСистемы);
				list.Insert(idx, lightSystem);
				_parent.мир.светофорныеСистемы = list.ToArray();
				_parent.Svetofor_Box.Items.Add("Система " + (idx + 1));
				_parent.Svetofor_Box.SelectedIndex = idx;
			}
		}

		private class AddLightSystemLightAction : EditorAction
		{
			private Светофорная_система lights;

			private Светофор light;

			public AddLightSystemLightAction(Светофорная_система ls, Светофор l)
			{
				lights = ls;
				light = l;
			}

			public override void Do()
			{
				lights.светофоры.Add(light);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = lights.светофоры.Count - 1;
				}
			}

			public override void Undo()
			{
				lights.светофоры.Remove(light);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = lights.светофоры.Count - 1;
				}
			}
		}

		private class AddLightSystemSignalAction : EditorAction
		{
			private Светофорная_система lights;

			private Светофорный_сигнал signal;

			public AddLightSystemSignalAction(Светофорная_система ls, Светофорный_сигнал s)
			{
				lights = ls;
				signal = s;
			}

			public override void Do()
			{
				lights.светофорные_сигналы.Add(signal);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = _parent.Svetofor_Element_Box.Items.Count - 1;
				}
			}

			public override void Undo()
			{
				lights.светофорные_сигналы.Remove(signal);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = _parent.Svetofor_Element_Box.Items.Count - 1;
				}
			}
		}

		private class MoveLightSystemLightAction : EditorAction
		{
			private Светофор light;

			private Положение initialPlacement;

			private Положение newPlacement;

			private bool firstDo = true;

			public MoveLightSystemLightAction(Светофор l)
			{
				light = l;
				initialPlacement = l.положение;
			}

			public override void Do()
			{
				if (firstDo)
				{
					newPlacement = light.положение;
					firstDo = false;
				}
				else
				{
					light.положение = newPlacement;
				}
			}

			public override void Undo()
			{
				light.положение = initialPlacement;
			}
		}

		private class MoveLightSystemSignalAction : EditorAction
		{
			private Светофорный_сигнал signal;

			private Road initialSpline;

			private Road newSpline;

			private double initialDist;

			private double newDist;

			public MoveLightSystemSignalAction(Светофорный_сигнал s)
			{
				signal = s;
				initialSpline = s.дорога;
				initialDist = s.расстояние;
			}

			public override void Do()
			{
				if (newSpline == null)
				{
					newSpline = signal.дорога;
					newDist = signal.расстояние;
				}
				else
				{
					signal.дорога = newSpline;
					signal.расстояние = newDist;
				}
			}

			public override void Undo()
			{
				signal.дорога = initialSpline;
				signal.расстояние = initialDist;
			}
		}

		private class RemoveLightSystemLightAction : EditorAction
		{
			private Светофорная_система lights;

			private Светофор signal;

			private int idx;

			private int ridx;

			public RemoveLightSystemLightAction(Светофорная_система ls, int ri, int i)
			{
				lights = ls;
				ridx = ri;
				idx = i;
			}

			public override void Do()
			{
				signal = lights.светофоры[ridx];
				lights.светофоры.RemoveAt(ridx);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = idx - 1;
				}
			}

			public override void Undo()
			{
				lights.светофоры.Insert(ridx, signal);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = idx;
				}
			}
		}

		private class RemoveLightSystemSignalAction : EditorAction
		{
			private Road road;

			private Светофорная_система lights;

			private Светофорный_сигнал signal;

			private int idx;

			private int ridx;

			public RemoveLightSystemSignalAction(Светофорная_система ls, int ri, int i)
			{
				lights = ls;
				ridx = ri;
				idx = i;
			}

			public override void Do()
			{
				signal = lights.светофорные_сигналы[ridx];
				road = signal.дорога;
				lights.светофорные_сигналы.RemoveAt(ridx);
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = idx - 1;
				}
			}

			public override void Undo()
			{
				lights.светофорные_сигналы.Insert(ridx, signal);
				signal.дорога = road;
				if (_parent.выбранная_светофорная_система == lights)
				{
					_parent.UpdateSvetoforControls(lights);
					_parent.Svetofor_Element_Box.SelectedIndex = idx;
				}
			}
		}

		private class AddObjectAction : EditorAction
		{
			private Объект _object;

			public AddObjectAction(Объект obj)
			{
				_object = obj;
			}

			public override void Do()
			{
				_parent.мир.объекты.Add(_object);
				_parent.UpdateObjectsList();
				_parent.Objects_Instance_Box.SelectedIndex = _parent.Objects_Instance_Box.Items.Count - 1;
			}

			public override void Undo()
			{
				int num = _parent.мир.объекты.IndexOf(_object);
				if (num != _parent.мир.объекты.Count - 1)
				{
					throw new InvalidOperationException("Objects array does not ends with added object");
				}
				_parent.мир.объекты.RemoveAt(num);
				_parent.UpdateObjectsList();
			}
		}

		private class MoveObjectAction : EditorAction
		{
			private Объект _object;

			private Double3DPoint _initialPos;

			private Double3DPoint _newPos;

			private double _initialAngle;

			private double _newAngle;

			private bool _isDone;

			public MoveObjectAction(Объект obj)
			{
				_object = obj;
				_initialPos = obj.Position3D;
				_initialAngle = obj.direction;
			}

			public override void Do()
			{
				if (_isDone)
				{
					_object.Position3D = _newPos;
					_object.angle0 = _newAngle;
				}
				else
				{
					_newPos = _object.Position3D;
					_newAngle = _object.direction;
					_isDone = true;
				}
			}

			public override void Undo()
			{
				_object.Position3D = _initialPos;
				_object.angle0 = _initialAngle;
			}
		}

		private class RemoveObjectAction : EditorAction
		{
			private int _index;

			private Объект _object;

			public RemoveObjectAction(int index)
			{
				_index = index;
			}

			public override void Do()
			{
				_object = _parent.мир.объекты[_index];
				_parent.мир.объекты.RemoveAt(_index);
				_parent.UpdateObjectsList();
			}

			public override void Undo()
			{
				_parent.мир.объекты.Insert(_index, _object);
				_parent.UpdateObjectsList();
			}
		}

		private abstract class EditorAction
		{
			protected Editor _parent;

			public Editor Parent
			{
				set
				{
					_parent = value;
				}
			}

			public abstract void Do();

			public abstract void Undo();
		}

		private enum Стадия_стоительства
		{
			Нет,
			Второй_конец,
			Первый_конец
		}

		private class Строящиеся_провода
		{
			public double[] высота = new double[2];

			public DoublePoint[] конец;

			public DoublePoint[] концы = new DoublePoint[2];

			public double направление;

			public double[] направления = new double[2];

			public DoublePoint[] начало;

			public Контактный_провод[] провода;

			public Стадия_стоительства стадия;

			public Строящиеся_провода()
			{
				провода = new Контактный_провод[4];
				Create();
			}

			public virtual void Обновить()
			{
				направление = Math.Round(направление * 36.0 / Math.PI) * Math.PI / 36.0;
				направления[0] = Math.Round(направления[0] * 72.0 / Math.PI) * Math.PI / 72.0;
				направления[1] = Math.Round(направления[1] * 72.0 / Math.PI) * Math.PI / 72.0;
				концы[0].Angle -= направление;
				концы[1].Angle -= направление;
				направления[0] -= направление;
				направления[1] -= направление;
				double num = Контактный_провод.расстояние_между_проводами / 2.0;
				провода[0].начало.x = концы[0].x + num * Math.Tan(направления[0]);
				провода[0].начало.y = концы[0].y - num;
				провода[1].начало.x = концы[0].x - num * Math.Tan(направления[0]);
				провода[1].начало.y = концы[0].y + num;
				провода[0].конец.x = концы[1].x + num * Math.Tan(направления[1]);
				провода[0].конец.y = концы[1].y - num;
				провода[1].конец.x = концы[1].x - num * Math.Tan(направления[1]);
				провода[1].конец.y = концы[1].y + num;
				if (стадия == Стадия_стоительства.Первый_конец)
				{
					провода[2].начало = провода[0].начало - new DoublePoint(2.0 * направления[0]) * 20.0;
					провода[2].конец = провода[0].начало;
					провода[3].начало = провода[1].начало - new DoublePoint(2.0 * направления[0]) * 20.0;
					провода[3].конец = провода[1].начало;
				}
				else
				{
					провода[2].начало = провода[0].конец;
					провода[2].конец = провода[0].конец + new DoublePoint(2.0 * направления[1]) * 20.0;
					провода[3].начало = провода[1].конец;
					провода[3].конец = провода[1].конец + new DoublePoint(2.0 * направления[1]) * 20.0;
				}
				провода[0].высота[0] = высота[0];
				провода[0].высота[1] = высота[1];
				провода[1].высота[0] = высота[0];
				провода[1].высота[1] = высота[1];
				провода[2].высота[0] = высота[1];
				провода[2].высота[1] = высота[1];
				провода[3].высота[0] = высота[1];
				провода[3].высота[1] = высота[1];
				провода[0].начало.Angle += направление;
				провода[0].конец.Angle += направление;
				провода[1].начало.Angle += направление;
				провода[1].конец.Angle += направление;
				провода[2].начало.Angle += направление;
				провода[2].конец.Angle += направление;
				провода[3].начало.Angle += направление;
				провода[3].конец.Angle += направление;
				концы[0].Angle += направление;
				концы[1].Angle += направление;
				направления[0] += направление;
				направления[1] += направление;
				if (начало != null)
				{
					провода[0].начало = начало[0];
					провода[1].начало = начало[1];
				}
				if (конец != null)
				{
					провода[0].конец = конец[0];
					провода[1].конец = конец[1];
				}
			}

			protected virtual void Create()
			{
				for (int i = 0; i < провода.Length; i++)
				{
					провода[i] = new Контактный_провод(0.0, 0.0, 20.0, 0.0, i % 2 == 0);
					провода[i].CreateMesh();
					провода[i].color = ((i < 2) ? 255 : 4144959);
				}
			}
		}

		private class Строящиеся_трамвайные_провода : Строящиеся_провода
		{
			public bool flag;

			public Строящиеся_трамвайные_провода()
			{
				провода = new Контактный_провод[2];
				Create();
			}

			public override void Обновить()
			{
				направление = Math.Round(направление * 36.0 / Math.PI) * Math.PI / 36.0;
				направления[0] = Math.Round(направления[0] * 72.0 / Math.PI) * Math.PI / 72.0;
				направления[1] = Math.Round(направления[1] * 72.0 / Math.PI) * Math.PI / 72.0;
				концы[0].Angle -= направление;
				концы[1].Angle -= направление;
				направления[0] -= направление;
				направления[1] -= направление;
				провода[0].начало.x = концы[0].x;
				провода[0].начало.y = концы[0].y;
				провода[1].начало.x = концы[0].x;
				провода[1].начало.y = концы[0].y;
				провода[0].конец.x = концы[1].x;
				провода[0].конец.y = концы[1].y;
				провода[1].конец.x = концы[1].x;
				провода[1].конец.y = концы[1].y;
				if (стадия == Стадия_стоительства.Первый_конец)
				{
					провода[1].начало = провода[0].начало - new DoublePoint(2.0 * направления[0]) * 20.0;
					провода[1].конец = провода[0].начало;
				}
				else
				{
					провода[1].начало = провода[0].конец;
					провода[1].конец = провода[0].конец + new DoublePoint(2.0 * направления[1]) * 20.0;
				}
				провода[0].высота[0] = высота[0];
				провода[0].высота[1] = высота[1];
				провода[1].высота[0] = высота[1];
				провода[1].высота[1] = высота[1];
				провода[0].начало.Angle += направление;
				провода[0].конец.Angle += направление;
				провода[1].начало.Angle += направление;
				провода[1].конец.Angle += направление;
				концы[0].Angle += направление;
				концы[1].Angle += направление;
				направления[0] += направление;
				направления[1] += направление;
				if (начало != null)
				{
					провода[0].начало = начало[0];
				}
				if (конец != null)
				{
					провода[0].конец = конец[0];
				}
			}

			protected override void Create()
			{
				for (int i = 0; i < провода.Length; i++)
				{
					провода[i] = new Трамвайный_контактный_провод(0.0, 0.0, 20.0, 0.0);
					провода[i].CreateMesh();
					провода[i].color = ((i < 1) ? 255 : 4144959);
				}
			}
		}

		private RestrictedStack<EditorAction> _undoStack = new RestrictedStack<EditorAction>(1024);

		private RestrictedStack<EditorAction> _redoStack = new RestrictedStack<EditorAction>(1024);

		private EditorAction _pendingAction;

		private bool _pendingActionApplied;

		private bool alt;

		private StatusBarPanel Angle_Status;

		private StatusBarPanel Angle1_Status;

		private StatusBarPanel Angle2_Status;

		private MenuItem Check_Joints_Item;

		private MenuItem City_Item;

		private MenuItem Edit_Item;

		private MenuItem ComputeAllTime_Item;

		private StatusBarPanel Coord_x1_Status;

		private StatusBarPanel Coord_x2_Status;

		private StatusBarPanel Coord_y1_Status;

		private StatusBarPanel Coord_y2_Status;

		private bool ctrl;

		private StatusBarPanel Cursor_x_Status;

		private StatusBarPanel Cursor_y_Status;

		private Point drag_point;

		private bool dragging;

		private ToolBarButton Edit_Button;

		private Panel edit_panel;

		private MenuItem Exit_Item;

		private string filename;

		private MenuItem Find_MinRadius_Item;

		private StatusBarPanel Height0_Status;

		private StatusBarPanel Height1_Status;

		private ImageList imageList;

		private StatusBarPanel Length_Status;

		private MainMenu mainMenu;

		private bool modified;

		private MouseEventArgs mouse_args = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

		private Button Narad_Add_Button;

		private ComboBox Narad_Box;

		private Button Narad_ChangeName_Button;

		private Label Narad_label;

		private TextBox Narad_Name_Box;

		private Label Narad_Name_label;

		private Panel narad_panel;

		private ComboBox Narad_Park_Box;

		private Label Narad_Park_label;

		private Button Narad_Remove_Button;

		private Button Narad_Runs_Add_Button;

		private ComboBox Narad_Runs_Box;

		private Label Narad_Runs_label;

		private Button Narad_Runs_Remove_Button;

		private ComboBox Narad_Runs_Run_Box;

		private Label Narad_Runs_Run_label;

		private TimeBox Narad_Runs_Time1_Box;

		private Label Narad_Runs_Time1_label;

		private TimeBox Narad_Runs_Time2_Box;

		private Label Narad_Runs_Time2_label;

		private MenuItem New_Item;

		private MenuItem Undo_Item;

		private MenuItem Redo_Item;

		private Panel old_panel;

		private ToolBarButton New_Button;

		private ToolBarButton Open_Button;

		private MenuItem Open_Item;

		private OpenFileDialog openFileDialog;

		private RenderPanel renderPanel;

		private Button Park_Add_Button;

		private ComboBox Park_Box;

		private ToolBarButton Park_Button;

		private Button Park_ChangeName_Button;

		private ToolBarButton Park_Edit_Button;

		private ToolBarButton Park_In_Button;

		private Label Park_label;

		private TextBox Park_Name_Box;

		private Label Park_Name_label;

		private ToolBarButton Park_Out_Button;

		private Panel park_panel;

		private ToolBarButton Park_Rails_Button;

		private Button Park_Remove_Button;

		private ToolBarButton Play_Button;

		private StatusBarPanel Radius_Status;

		private ToolBarButton Rail_Build_Direct_Button;

		private ToolBarButton Rail_Button;

		private ToolBarButton Rail_Edit_Button;

		private MenuItem Refresh_All_TripStop_Lists_Item;

		private Timer Refresh_Timer;

		private ToolBarButton Road_Button;

		private Button Route_Add_Button;

		private ComboBox Route_Box;

		private ToolBarButton Route_Button;

		private Button Route_ChangeName_Button;

		private Label Route_label;

		private TextBox Route_Name_Box;

		private Label Route_Name_label;

		private Panel route_panel;

		private Button Route_Remove_Button;

		private Button Route_Runs_Add_Button;

		private ComboBox Route_Runs_Box;

		private Button Route_Runs_ComputeTime_Button;

		private Label Route_Runs_label;

		private CheckBox Route_Runs_Park_Box;

		private Button Route_Runs_Remove_Button;

		private TimeBox Route_Runs_Time_Box;

		private Label Route_Runs_Time_label;

		private CheckBox Route_Runs_ToPark_Box;

		private Label Route_Runs_ToParkIndex_label;

		private NumericUpDown Route_Runs_ToParkIndex_UpDown;

		private CheckBox Route_ShowNarads_Box;

		private ComboBox Route_TransportType_Box;

		private Label Route_TransportType_label;

		private ToolBarButton Run_Button;

		private MenuItem Run_Item;

		private ToolBarButton Save_Button;

		private MenuItem Save_Item;

		private MenuItem SaveAs_Item;

		private SaveFileDialog saveFileDialog;

		private ToolBarButton SeparatorButton1;

		private ToolBarButton SeparatorButton2;

		private ToolBarButton SeparatorButton3;

		private ToolBarButton SeparatorButton4;

		private ToolBarButton SeparatorButton5;

		private ToolBarButton SeparatorButton6;

		private MenuItem SeparatorItem1;

		private MenuItem SeparatorItem2;

		private StatusBarPanel SeparatorPanel1;

		private StatusBarPanel SeparatorPanel2;

		private StatusBarPanel SeparatorPanel3;

		private bool shift;

		private Button Signals_Add_Button;

		private Label Signals_Bound_label;

		private NumericUpDown Signals_Bound_UpDown;

		private ComboBox Signals_Box;

		private ToolBarButton Signals_Button;

		private Button Signals_Element_AddContact_Button;

		private Button Signals_Element_AddSignal_Button;

		private ComboBox Signals_Element_Box;

		private Button Signals_Element_EditLocation_Button;

		private Label Signals_Element_label;

		private Label Signals_Element_Location_label;

		private CheckBox Signals_Element_Minus_Box;

		private Button Signals_Element_Remove_Button;

		private Button Signals_Element_ShowLocation_Button;

		private Label Signals_label;

		private Panel signals_panel;

		private Button Signals_Remove_Button;

		private Panel Sizable_Panel;

		private StatusBar statusBar;

		private Button Stops_Add_Button;

		private ComboBox Stops_Box;

		private ToolBarButton Stops_Button;

		private Button Stops_ChangeName_Button;

		private Button Stops_EditLocation_Button;

		private Label Stops_label;

		private Label Stops_Location_label;

		private TextBox Stops_Name_Box;

		private Label Stops_Name_label;

		private Panel stops_panel;

		private Button Stops_Remove_Button;

		private Button Stops_ShowLocation_Button;

		private Button Svetofor_Add_Button;

		private TimeBox Svetofor_Begin_Box;

		private ComboBox Svetofor_Box;

		private ToolBarButton Svetofor_Button;

		private TimeBox Svetofor_Cycle_Box;

		private Label Svetofor_Cycle_label;

		private ComboBox Svetofor_Element_Box;

		private Button Svetofor_Element_EditLocation_Button;

		private Label Svetofor_Element_label;

		private Label Svetofor_Element_Location_label;

		private Button Svetofor_Element_Remove_Button;

		private Button Svetofor_Element_ShowLocation_Button;

		private TimeBox Svetofor_End_Box;

		private Label Svetofor_Green_label;

		private Label Svetofor_label;

		private TimeBox Svetofor_OfGreen_Box;

		private Panel svetofor_panel;

		private Button Svetofor_Remove_Button;

		private Button Svetofor_Signal_Add_Button;

		private Button Svetofor_Svetofor_Add_Button;

		private ComboBox Svetofor_Svetofor_ArrowGreen_Box;

		private Label Svetofor_Svetofor_ArrowGreen_label;

		private ComboBox Svetofor_Svetofor_ArrowRed_Box;

		private Label Svetofor_Svetofor_ArrowRed_label;

		private ComboBox Svetofor_Svetofor_ArrowYellow_Box;

		private Label Svetofor_Svetofor_ArrowYellow_label;

		private TimeBox Svetofor_ToGreen_Box;

		private Label Svetofor_Work_label;

		private ToolBar toolBar;

		private ToolBarButton Troll_lines_Button;

		private ToolBarButton Troll_lines_Draw_Button;

		private ToolBarButton Troll_lines_Edit_Button;

		private ToolBarButton Troll_lines_Flag_Button;

		private StatusBarPanel Wide0_Status;

		private StatusBarPanel Wide1_Status;

		private Игрок[] без_игроков;

		private Game игра;

		private Игрок игрок;

		private Игрок[] игроки;

		private World мир;

		private bool раскрашены_провода;

		private bool раскрашены_рельсы;

		private Стадия_стоительства строительство_дороги;

		private Road строящаяся_дорога;

		private Stop строящаяся_остановка;

		private Строящиеся_провода строящиеся_провода;

		private Светофор строящийся_светофор;

		private Светофорный_сигнал строящийся_светофорный_сигнал;

		private Сигнальная_система.Контакт строящийся_элемент_сигнальной_системы;

		private Visual_Signal строящийся_сигнал_сигнальной_системы;

		private ToolBarButton Object_Button;

		private Объект строящийся_объект;

		private bool splines_aviable;

		private double time_color;

		private IContainer components;

		private Label Rail_Box_dist_Label;

		private NumericBox Rail_Box_NumericBox;

		private ToolBarButton Rail_Build_Curve_Button;

		private Label Stops_Model_label;

		private ComboBox Stops_Model_Box;

		private Label Svetofor_Model_label;

		private ComboBox Svetofor_Model_Box;

		private Label Signals_Model_label;

		private ComboBox Signals_Model_Box;

		private CheckBox Spline_Select_mode_Box;

		private Label Splines_label;

		private ComboBox Splines_Models_Box;

		private Button Splines_Remove_Button;

		private Button Splines_ChangeModel_Button;

		private Label Splines_Location_label;

		private Button Splines_ShowLocation_Button;

		private Label Splines_Instance_label;

		private ComboBox Splines_Instance_Box;

		private Panel splines_panel;

		private Panel object_panel;

		private Label Objects_label;

		private ComboBox Objects_Box;

		private Button Objects_Add_Button;

		private Label Objects_Location_label;

		private Button Objects_Remove_Button;

		private Button Objects_EditLocation_Button;

		private Button Objects_ShowLocation_Button;

		private ComboBox Objects_Instance_Box;

		private Label Objects_Instance_label;

		private ComboBox RollingStockBox;

		private Label Transport_label;

		private StatusBarPanel SeparatorPanel4;

		private StatusBarPanel Maschtab;

		private StatusBarPanel SeparatorPanel5;

		private StatusBarPanel Ugol;

		private ToolBarButton SeparatorButton8;

		private ToolBarButton Rail_Build_попутки_Button;

		private ToolBarButton Rail_Build_попутки1_Button;

		private ToolBarButton Rail_Build_попутки2_Button;

		private ToolBarButton Rail_Build_попутки3_Button;

		private ToolBarButton Rail_Build_встречки_Button;

		private ToolBarButton Rail_Build_встречки1_Button;

		private ToolBarButton Rail_Build_встречки2_Button;

		private ToolBarButton Rail_Build_встречки3_Button;

		private ToolBarButton toolBarButton3;

		private GroupBox TypeOfTransportBox;

		private CheckBox BusBox;

		private CheckBox TrolleybusBox;

		private CheckBox TramwayBox;

		private Button StopsButton;

		public DoublePoint cursor_pos
		{
			get
			{
				int num = (base.Size.Width - base.ClientSize.Width) / 2;
				int num2 = base.Size.Height - base.ClientSize.Height - num;
				double num3 = (double)(Control.MousePosition.X - (2 * (base.Location.X + num) + Sizable_Panel.Left + Sizable_Panel.Right) / 2) / MyDirect3D.масштаб;
				double num4 = (double)(Control.MousePosition.Y - (2 * (base.Location.Y + num2) + Sizable_Panel.Top + Sizable_Panel.Bottom) / 2) / MyDirect3D.масштаб;
				return new DoublePoint(игрок.cameraPosition.x + num3, игрок.cameraPosition.z - num4);
			}
		}

		public Контактный_провод[] ближайшие_провода
		{
			get
			{
				DoublePoint doublePoint = cursor_pos;
				List<Контактный_провод> list = new List<Контактный_провод>();
				List<int> list2 = new List<int>();
				for (int i = 0; i < мир.контактныеПровода.Length; i++)
				{
					Контактный_провод контактный_провод = мир.контактныеПровода[i];
					DoublePoint doublePoint2 = (doublePoint - контактный_провод.начало) / (контактный_провод.конец - контактный_провод.начало);
					if (doublePoint2.x >= 0.0 && doublePoint2.x < 1.0 && Math.Abs(контактный_провод.длина * doublePoint2.y) < 0.5)
					{
						list.Add(контактный_провод);
						list2.Add(i);
					}
				}
				if (list.Count == 2 && list2[0] / 2 == list2[1] / 2 && list[0].правый != list[1].правый)
				{
					return list.ToArray();
				}
				return null;
			}
		}

		public Трамвайный_контактный_провод ближайший_провод
		{
			get
			{
				DoublePoint doublePoint = cursor_pos;
				for (int i = 0; i < мир.контактныеПровода2.Length; i++)
				{
					Трамвайный_контактный_провод трамвайный_контактный_провод = мир.контактныеПровода2[i];
					DoublePoint doublePoint2 = (doublePoint - трамвайный_контактный_провод.начало) / (трамвайный_контактный_провод.конец - трамвайный_контактный_провод.начало);
					if (doublePoint2.x >= 0.0 && doublePoint2.x < 1.0 && Math.Abs(трамвайный_контактный_провод.длина * doublePoint2.y) < 0.5)
					{
						return трамвайный_контактный_провод;
					}
				}
				return null;
			}
		}

		private Светофорная_система выбранная_светофорная_система
		{
			get
			{
				if (Svetofor_Box.SelectedIndex >= 0)
				{
					return мир.светофорныеСистемы[Svetofor_Box.SelectedIndex];
				}
				return new Светофорная_система();
			}
		}

		private Сигнальная_система выбранная_сигнальная_система
		{
			get
			{
				if (Signals_Box.SelectedIndex >= 0)
				{
					return мир.сигнальныеСистемы[Signals_Box.SelectedIndex];
				}
				return new Сигнальная_система(0, 0);
			}
		}

		public Route выбранный_маршрут
		{
			get
			{
				if (Route_Box.SelectedIndex >= 0)
				{
					return мир.маршруты[Route_Box.SelectedIndex];
				}
				return new Route(0, "-");
			}
		}

		private Order выбранный_наряд
		{
			get
			{
				if (Narad_Box.SelectedIndex >= 0 && Narad_Box.SelectedIndex < выбранный_маршрут.orders.Length)
				{
					return выбранный_маршрут.orders[Narad_Box.SelectedIndex];
				}
				return new Order(new Парк(""), new Route(0, ""), "", "");
			}
		}

		private Trip выбранный_рейс
		{
			get
			{
				if (Route_Runs_Box.SelectedIndex >= выбранный_маршрут.trips.Count)
				{
					return выбранный_маршрут.parkTrips[Route_Runs_Box.SelectedIndex - выбранный_маршрут.trips.Count];
				}
				if (Route_Runs_Box.SelectedIndex >= 0)
				{
					return выбранный_маршрут.trips[Route_Runs_Box.SelectedIndex];
				}
				return null;
			}
		}

		public Road строящаяся_дорога1
		{
			get
			{
				if (строящаяся_дорога == null)
				{
					return null;
				}
				try
				{
					double num = строящаяся_дорога.ширина[0];
					double num2 = строящаяся_дорога.ширина[1];
					DoublePoint doublePoint = строящаяся_дорога.НайтиКоординаты(0.0, 0.0 - num);
					DoublePoint doublePoint2 = строящаяся_дорога.НайтиКоординаты(строящаяся_дорога.Длина, 0.0 - num2);
					double num3 = строящаяся_дорога.направления[0];
					double num4 = строящаяся_дорога.направления[1];
					Road road = ((!(строящаяся_дорога is Рельс)) ? new Road(doublePoint.x, doublePoint.y, doublePoint2.x, doublePoint2.y, num3, num4, num, num2) : new Рельс(doublePoint.x, doublePoint.y, doublePoint2.x, doublePoint2.y, num3, num4));
					road.высота[0] = строящаяся_дорога.высота[0];
					road.высота[1] = строящаяся_дорога.высота[1];
					road.кривая = строящаяся_дорога.кривая;
					road.ОбновитьСледующиеДороги(мир.ВсеДороги);
					road.name = строящаяся_дорога.name;
					road.CreateMesh();
					if (строящаяся_дорога.Color == 255)
					{
						road.Color = 16711680;
					}
					return road;
				}
				catch
				{
					return null;
				}
			}
		}

		public Road строящаяся_дорога2
		{
			get
			{
				if (строящаяся_дорога == null)
				{
					return null;
				}
				try
				{
					double num = строящаяся_дорога.ширина[0];
					double num2 = строящаяся_дорога.ширина[1];
					DoublePoint doublePoint = строящаяся_дорога.НайтиКоординаты(0.0, -2.0 * num);
					DoublePoint doublePoint2 = строящаяся_дорога.НайтиКоординаты(строящаяся_дорога.Длина, -2.0 * num2);
					double num3 = строящаяся_дорога.направления[0];
					double num4 = строящаяся_дорога.направления[1];
					Road road = ((!(строящаяся_дорога is Рельс)) ? new Road(doublePoint.x, doublePoint.y, doublePoint2.x, doublePoint2.y, num3, num4, num, num2) : new Рельс(doublePoint.x, doublePoint.y, doublePoint2.x, doublePoint2.y, num3, num4));
					road.высота[0] = строящаяся_дорога.высота[0];
					road.высота[1] = строящаяся_дорога.высота[1];
					road.кривая = строящаяся_дорога.кривая;
					road.ОбновитьСледующиеДороги(мир.ВсеДороги);
					road.name = строящаяся_дорога.name;
					road.CreateMesh();
					if (строящаяся_дорога.Color == 255)
					{
						road.Color = 16711680;
					}
					return road;
				}
				catch
				{
					return null;
				}
			}
		}

		public Road строящаяся_обратная_дорога
		{
			get
			{
				if (строящаяся_дорога == null)
				{
					return null;
				}
				try
				{
					double num = строящаяся_дорога.ширина[0];
					double num2 = строящаяся_дорога.ширина[1];
					DoublePoint doublePoint = строящаяся_дорога.НайтиКоординаты(0.0, num);
					DoublePoint doublePoint2 = строящаяся_дорога.НайтиКоординаты(строящаяся_дорога.Длина, num2);
					double num3 = строящаяся_дорога.направления[0];
					double num4 = строящаяся_дорога.направления[1];
					Road road = ((!(строящаяся_дорога is Рельс)) ? new Road(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3, num2, num) : new Рельс(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3));
					road.высота[0] = строящаяся_дорога.высота[1];
					road.высота[1] = строящаяся_дорога.высота[0];
					road.кривая = строящаяся_дорога.кривая;
					road.ОбновитьСледующиеДороги(мир.ВсеДороги);
					road.name = строящаяся_дорога.name;
					road.CreateMesh();
					if (строящаяся_дорога.Color == 255)
					{
						road.Color = 16776960;
					}
					return road;
				}
				catch
				{
					return null;
				}
			}
		}

		public Road строящаяся_обратная_дорога1
		{
			get
			{
				if (строящаяся_дорога == null)
				{
					return null;
				}
				try
				{
					double num = строящаяся_дорога.ширина[0];
					double num2 = строящаяся_дорога.ширина[1];
					DoublePoint doublePoint = строящаяся_дорога.НайтиКоординаты(0.0, 2.0 * num);
					DoublePoint doublePoint2 = строящаяся_дорога.НайтиКоординаты(строящаяся_дорога.Длина, 2.0 * num2);
					double num3 = строящаяся_дорога.направления[0];
					double num4 = строящаяся_дорога.направления[1];
					Road road = ((!(строящаяся_дорога is Рельс)) ? new Road(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3, num2, num) : new Рельс(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3));
					road.высота[0] = строящаяся_дорога.высота[1];
					road.высота[1] = строящаяся_дорога.высота[0];
					road.кривая = строящаяся_дорога.кривая;
					road.ОбновитьСледующиеДороги(мир.ВсеДороги);
					road.name = строящаяся_дорога.name;
					road.CreateMesh();
					if (строящаяся_дорога.Color == 255)
					{
						road.Color = 16776960;
					}
					return road;
				}
				catch
				{
					return null;
				}
			}
		}

		public Road строящаяся_обратная_дорога2
		{
			get
			{
				if (строящаяся_дорога == null)
				{
					return null;
				}
				try
				{
					double num = строящаяся_дорога.ширина[0];
					double num2 = строящаяся_дорога.ширина[1];
					DoublePoint doublePoint = строящаяся_дорога.НайтиКоординаты(0.0, 3.0 * num);
					DoublePoint doublePoint2 = строящаяся_дорога.НайтиКоординаты(строящаяся_дорога.Длина, 3.0 * num2);
					double num3 = строящаяся_дорога.направления[0];
					double num4 = строящаяся_дорога.направления[1];
					Road road = ((!(строящаяся_дорога is Рельс)) ? new Road(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3, num2, num) : new Рельс(doublePoint2.x, doublePoint2.y, doublePoint.x, doublePoint.y, num4, num3));
					road.высота[0] = строящаяся_дорога.высота[1];
					road.высота[1] = строящаяся_дорога.высота[0];
					road.кривая = строящаяся_дорога.кривая;
					road.ОбновитьСледующиеДороги(мир.ВсеДороги);
					road.name = строящаяся_дорога.name;
					road.CreateMesh();
					if (строящаяся_дорога.Color == 255)
					{
						road.Color = 16776960;
					}
					return road;
				}
				catch
				{
					return null;
				}
			}
		}

		private void DoRegisterAction(EditorAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action", "IEditorAction can not be null");
			}
			action.Parent = this;
			_undoStack.Push(action);
			_redoStack.Clear();
			action.Do();
			UpdateUndoRedoState();
		}

		private void UpdateUndoRedoState()
		{
			Undo_Item.Enabled = _undoStack.Count > 0;
			Redo_Item.Enabled = _redoStack.Count > 0;
		}

		private void UndoAction()
		{
			if (_undoStack.Count == 0)
			{
				throw new InvalidOperationException("Can not undo from empty buffer");
			}
			EditorAction editorAction = _undoStack.Pop();
			_redoStack.Push(editorAction);
			editorAction.Undo();
		}

		private void RedoAction()
		{
			if (_redoStack.Count == 0)
			{
				throw new InvalidOperationException("Can not redo from empty buffer");
			}
			EditorAction editorAction = _redoStack.Pop();
			_undoStack.Push(editorAction);
			editorAction.Do();
		}

		private void RegisterPendingAction(EditorAction action, bool doNow = false)
		{
			if (_pendingAction != null)
			{
				throw new InvalidOperationException("Pending action is not null");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action", "IEditorAction can not be null");
			}
			action.Parent = this;
			if (doNow)
			{
				action.Do();
			}
			_pendingAction = action;
			_pendingActionApplied = doNow;
		}

		private void DoPendingAction()
		{
			if (_pendingAction == null)
			{
				throw new InvalidOperationException("Pending action is null");
			}
			_undoStack.Push(_pendingAction);
			_redoStack.Clear();
			if (!_pendingActionApplied)
			{
				_pendingAction.Do();
			}
			_pendingAction = null;
			UpdateUndoRedoState();
		}

		private void ClearPendingAction()
		{
			if (_pendingAction == null)
			{
				throw new InvalidOperationException("Pending action is null");
			}
			if (_pendingActionApplied)
			{
				_pendingAction.Undo();
			}
			_pendingAction = null;
		}

		public Editor()
		{
			InitializeComponent();
		}

		private void ApplyLocalization()
		{
			Localization.ApplyLocalization(this);
			Localization.ApplyLocalizationToolBar(toolBar);
		}

		private void Check_стыки_Item_Click(object sender, EventArgs e)
		{
			int num = 0;
			int num2 = 0;
			Road[] всеДороги = мир.ВсеДороги;
			foreach (Road road in всеДороги)
			{
				road.ОбновитьСледующиеДороги(мир.ВсеДороги);
				if (road.предыдущиеДороги.Length == 0)
				{
					num++;
					игрок.cameraPosition.XZPoint = road.концы[0];
				}
				if (road.следующиеДороги.Length == 0)
				{
					num2++;
					игрок.cameraPosition.XZPoint = road.концы[1];
				}
			}
			if (num > 0 && num2 > 0)
			{
				MessageBox.Show(this, string.Format(Localization.current_.joints_begin_end, num.ToString(), num2.ToString()), "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (num > 0)
			{
				MessageBox.Show(this, string.Format(Localization.current_.joints_begin, num.ToString()), "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (num2 > 0)
			{
				MessageBox.Show(this, string.Format(Localization.current_.joints_end, num2.ToString()), "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				MessageBox.Show(this, Localization.current_.joints_checked, "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private void ComputeAllTime_Item_Click(object sender, EventArgs e)
		{
			List<Trip> list = new List<Trip>();
			List<int> list2 = new List<int>();
			Route[] маршруты = мир.маршруты;
			foreach (Route route in маршруты)
			{
				list.AddRange(route.AllTrips);
				for (int j = 0; j < route.AllTrips.Count; j++)
				{
					list2.Add(route.typeOfTransport);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].время_прибытия != 0.0)
				{
					list.RemoveAt(k);
					list2.RemoveAt(k);
					k--;
				}
			}
			if (list.Count == 0)
			{
				MessageBox.Show(this, Localization.current_.routes_computed, "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}
			Refresh_Timer.Enabled = false;
			for (int l = 0; l < list.Count; l++)
			{
				if (list[l].pathes.Length == 0)
				{
					MessageBox.Show(Localization.current_.route_failed, "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					continue;
				}
				мир.time = 0.0;
				ComputeTimeDialog computeTimeDialog = new ComputeTimeDialog(мир, list2[l], list[l], игрок);
				computeTimeDialog.Text = $"{computeTimeDialog.Text} ({l + 1} {Localization.current_.of} {list.Count})";
				if (computeTimeDialog.ShowDialog(this) == DialogResult.Cancel)
				{
					break;
				}
				list[l].время_прибытия = мир.time;
			}
			мир.time = 0.0;
			Refresh_Timer.Enabled = true;
			Route_Runs_Box_SelectedIndexChanged(null, new EventArgs());
			modified = true;
		}

		private void Editor_Form_Closing(object sender, CancelEventArgs e)
		{
			if (modified)
			{
				switch (MessageBox.Show(Localization.current_.save_quit, "Transedit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					Save_Item_Click(sender, e);
					break;
				}
			}
		}

		private void Editor_Form_KeyDown(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = !edit_panel.Enabled;
			if (e.KeyCode == Keys.W && e.Control)
			{
				MyDirect3D.карта = !MyDirect3D.карта;
				MyDirect3D.масштаб = (MyDirect3D.карта ? 1.0 : 10.0);
			}
			if (e.KeyCode == Keys.Add && e.Control && MyDirect3D.масштаб < 50.0)
			{
				MyDirect3D.масштаб += 1.0;
			}
			if (e.KeyCode == Keys.Subtract && e.Control && MyDirect3D.масштаб > 1.0)
			{
				MyDirect3D.масштаб -= 1.0;
			}
			if (e.KeyCode == Keys.Menu)
			{
				alt = true;
			}
			if (e.KeyCode == Keys.ShiftKey)
			{
				shift = true;
			}
			if (e.KeyCode == Keys.ControlKey)
			{
				ctrl = true;
			}
			if (строящаяся_дорога != null && (строящаяся_дорога.кривая || строительство_дороги == Стадия_стоительства.Нет))
			{
				if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Z)
				{
					switch (строительство_дороги)
					{
					case Стадия_стоительства.Нет:
					case Стадия_стоительства.Первый_конец:
						строящаяся_дорога.направления[0] -= Math.PI / 36.0;
						if (строящаяся_дорога.направления[0] <= -Math.PI)
						{
							строящаяся_дорога.направления[0] += Math.PI * 2.0;
						}
						break;
					case Стадия_стоительства.Второй_конец:
						строящаяся_дорога.направления[1] -= Math.PI / 36.0;
						if (строящаяся_дорога.направления[1] <= -Math.PI)
						{
							строящаяся_дорога.направления[1] += Math.PI * 2.0;
						}
						break;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Up || e.KeyCode == Keys.A)
				{
					switch (строительство_дороги)
					{
					case Стадия_стоительства.Нет:
					case Стадия_стоительства.Первый_конец:
						строящаяся_дорога.направления[0] += Math.PI / 36.0;
						if (строящаяся_дорога.направления[0] > Math.PI)
						{
							строящаяся_дорога.направления[0] -= Math.PI * 2.0;
						}
						break;
					case Стадия_стоительства.Второй_конец:
						строящаяся_дорога.направления[1] += Math.PI / 36.0;
						if (строящаяся_дорога.направления[1] > Math.PI)
						{
							строящаяся_дорога.направления[1] -= Math.PI * 2.0;
						}
						break;
					}
					process_mouse(mouse_args, click: false);
				}
			}
			if (строящаяся_дорога != null)
			{
				if (e.KeyCode == Keys.Left || e.KeyCode == Keys.X)
				{
					if (строительство_дороги == Стадия_стоительства.Первый_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.ширина[0] -= 0.5;
						if (строящаяся_дорога.ширина[0] < 0.5)
						{
							строящаяся_дорога.ширина[0] = 0.5;
						}
					}
					if (строительство_дороги == Стадия_стоительства.Второй_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.ширина[1] -= 0.5;
						if (строящаяся_дорога.ширина[1] < 0.5)
						{
							строящаяся_дорога.ширина[1] = 0.5;
						}
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Right || e.KeyCode == Keys.S)
				{
					if (строительство_дороги == Стадия_стоительства.Первый_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.ширина[0] += 0.5;
					}
					if (строительство_дороги == Стадия_стоительства.Второй_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.ширина[1] += 0.5;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Next || e.KeyCode == Keys.C)
				{
					if (строительство_дороги == Стадия_стоительства.Первый_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.высота[0] -= 0.5;
						if (строящаяся_дорога.высота[0] < 0.0)
						{
							строящаяся_дорога.высота[0] = 0.0;
						}
					}
					if (строительство_дороги == Стадия_стоительства.Второй_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.высота[1] -= 0.5;
						if (строящаяся_дорога.высота[1] < 0.0)
						{
							строящаяся_дорога.высота[1] = 0.0;
						}
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Prior || e.KeyCode == Keys.D)
				{
					if (строительство_дороги == Стадия_стоительства.Первый_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.высота[0] += 0.5;
					}
					if (строительство_дороги == Стадия_стоительства.Второй_конец || строительство_дороги == Стадия_стоительства.Нет)
					{
						строящаяся_дорога.высота[1] += 0.5;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Escape && строительство_дороги != 0)
				{
					строительство_дороги = Стадия_стоительства.Нет;
				}
			}
			if (строящиеся_провода != null)
			{
				if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Z)
				{
					switch (строящиеся_провода.стадия)
					{
					case Стадия_стоительства.Нет:
						строящиеся_провода.направление -= Math.PI / 36.0;
						строящиеся_провода.направления[0] -= Math.PI / 36.0;
						строящиеся_провода.направления[1] -= Math.PI / 36.0;
						break;
					case Стадия_стоительства.Второй_конец:
						строящиеся_провода.направления[1] -= Math.PI / 72.0;
						break;
					case Стадия_стоительства.Первый_конец:
						строящиеся_провода.направления[0] -= Math.PI / 72.0;
						break;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Up || e.KeyCode == Keys.A)
				{
					switch (строящиеся_провода.стадия)
					{
					case Стадия_стоительства.Нет:
						строящиеся_провода.направление += Math.PI / 36.0;
						строящиеся_провода.направления[0] += Math.PI / 36.0;
						строящиеся_провода.направления[1] += Math.PI / 36.0;
						break;
					case Стадия_стоительства.Второй_конец:
						строящиеся_провода.направления[1] += Math.PI / 72.0;
						break;
					case Стадия_стоительства.Первый_конец:
						строящиеся_провода.направления[0] += Math.PI / 72.0;
						break;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Next || e.KeyCode == Keys.C)
				{
					if (строящиеся_провода.стадия == Стадия_стоительства.Первый_конец || строящиеся_провода.стадия == Стадия_стоительства.Нет)
					{
						строящиеся_провода.высота[0] -= 0.5;
						if (строящиеся_провода.высота[0] < 0.0)
						{
							строящиеся_провода.высота[0] = 0.0;
						}
					}
					if (строящиеся_провода.стадия == Стадия_стоительства.Второй_конец || строящиеся_провода.стадия == Стадия_стоительства.Нет)
					{
						строящиеся_провода.высота[1] -= 0.5;
						if (строящиеся_провода.высота[1] < 0.0)
						{
							строящиеся_провода.высота[1] = 0.0;
						}
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Prior || e.KeyCode == Keys.D)
				{
					if (строящиеся_провода.стадия == Стадия_стоительства.Первый_конец || строящиеся_провода.стадия == Стадия_стоительства.Нет)
					{
						строящиеся_провода.высота[0] += 0.5;
					}
					if (строящиеся_провода.стадия == Стадия_стоительства.Второй_конец || строящиеся_провода.стадия == Стадия_стоительства.Нет)
					{
						строящиеся_провода.высота[1] += 0.5;
					}
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Escape && строящиеся_провода.стадия != 0)
				{
					строящиеся_провода.стадия = Стадия_стоительства.Нет;
				}
			}
			if (строящаяся_остановка != null && e.KeyCode == Keys.Escape)
			{
				ClearPendingAction();
				строящаяся_остановка = null;
				EnableControls(value: true);
				return;
			}
			if (строящийся_сигнал_сигнальной_системы != null)
			{
				if (e.KeyCode == Keys.Left || e.KeyCode == Keys.X)
				{
					строящийся_сигнал_сигнальной_системы.положение.отклонение = Math.Round(строящийся_сигнал_сигнальной_системы.положение.отклонение + 0.2, 1);
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Right || e.KeyCode == Keys.S)
				{
					строящийся_сигнал_сигнальной_системы.положение.отклонение = Math.Round(строящийся_сигнал_сигнальной_системы.положение.отклонение - 0.2, 1);
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Up || e.KeyCode == Keys.D)
				{
					строящийся_сигнал_сигнальной_системы.положение.высота += 0.2;
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Down || e.KeyCode == Keys.C)
				{
					строящийся_сигнал_сигнальной_системы.положение.высота = Math.Max(строящийся_сигнал_сигнальной_системы.положение.высота - 0.2, 0.0);
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Escape)
				{
					ClearPendingAction();
					строящийся_сигнал_сигнальной_системы = null;
					EnableControls(value: true);
					return;
				}
			}
			if (строящийся_элемент_сигнальной_системы != null && e.KeyCode == Keys.Escape)
			{
				ClearPendingAction();
				строящийся_элемент_сигнальной_системы = null;
				EnableControls(value: true);
				return;
			}
			if (строящийся_светофор != null)
			{
				if (e.KeyCode == Keys.Left || e.KeyCode == Keys.X)
				{
					строящийся_светофор.положение.отклонение += 0.2;
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Right || e.KeyCode == Keys.S)
				{
					строящийся_светофор.положение.отклонение -= 0.2;
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Up || e.KeyCode == Keys.D)
				{
					строящийся_светофор.положение.высота += 0.2;
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Down || e.KeyCode == Keys.C)
				{
					строящийся_светофор.положение.высота = Math.Max(строящийся_светофор.положение.высота - 0.2, 0.0);
					process_mouse(mouse_args, click: false);
				}
				if (e.KeyCode == Keys.Escape)
				{
					ClearPendingAction();
					строящийся_светофор = null;
					EnableControls(value: true);
					return;
				}
			}
			if (строящийся_светофорный_сигнал != null && e.KeyCode == Keys.Escape)
			{
				ClearPendingAction();
				строящийся_светофорный_сигнал = null;
				EnableControls(value: true);
				return;
			}
			if (строящийся_объект != null)
			{
				switch (e.KeyCode)
				{
				case Keys.Z:
					строящийся_объект.angle0 -= Math.PI / 36.0;
					break;
				case Keys.A:
					строящийся_объект.angle0 += Math.PI / 36.0;
					break;
				case Keys.D:
					строящийся_объект.height0 += 0.5;
					break;
				case Keys.C:
					строящийся_объект.height0 = Math.Max(строящийся_объект.height0 - 0.5, 0.0);
					break;
				case Keys.Escape:
					строящийся_объект = null;
					ClearPendingAction();
					EnableControls(value: true);
					return;
				}
				if (строящийся_объект.angle0 > Math.PI)
				{
					строящийся_объект.angle0 -= Math.PI * 2.0;
				}
				else if (строящийся_объект.angle0 < -Math.PI)
				{
					строящийся_объект.angle0 += Math.PI * 2.0;
				}
				UpdateStatusBar();
			}
			if (e.KeyCode != Keys.Z || !Route_Button.Pushed || Route_Box.SelectedIndex < 0 || Route_Runs_Box.SelectedIndex < 0)
			{
				return;
			}
			Trip trip = выбранный_рейс;
			List<Road> list = new List<Road>(trip.pathes);
			if (list.Count <= 0)
			{
				return;
			}
			while (list[list.Count - 1].следующиеДороги.Length == 1)
			{
				list.Add(list[list.Count - 1].следующиеДороги[0]);
				if (list[list.Count - 1] == trip.дорога_прибытия)
				{
					break;
				}
			}
			trip.pathes = list.ToArray();
			игрок.cameraPosition.XZPoint = trip.дорога_прибытия.концы[1];
			Route_Runs_ToParkIndex_UpDown.Maximum = выбранный_рейс.pathes.Length;
			ОбновитьРаскрашенныеСплайны();
			modified = true;
		}

		private void Editor_Form_KeyUp(object sender, KeyEventArgs e)
		{
			e.SuppressKeyPress = false;
			if (e.KeyCode == Keys.Menu)
			{
				alt = false;
			}
			if (e.KeyCode == Keys.ShiftKey)
			{
				shift = false;
			}
			if (e.KeyCode == Keys.ControlKey)
			{
				ctrl = false;
			}
		}

		private void Editor_Form_Load(object sender, EventArgs e)
		{
			Directory.SetCurrentDirectory(Application.StartupPath + "\\Data");
			DeviceOptions deviceOptions = default(DeviceOptions);
			deviceOptions.vSync = false;
			deviceOptions.windowed = true;
			deviceOptions.windowedX = 1600;
			deviceOptions.windowedY = 1200;
			deviceOptions.vertexProcessingMode = 1;
			DeviceOptions dialog = deviceOptions;
			if (!MyDirect3D.InitializeWOpt(renderPanel, dialog))
			{
				MessageBox.Show("Could not initialize Direct3D.", "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Close();
				return;
			}
			MyGUI.splash_title = "Transedit";
			MainForm.in_editor = true;
			MyDirect3D.вид_сверху = true;
			Road.качествоДороги = 5.0;
			ApplyLocalization();
			RefreshPanelSize(sender, e);
			toolBar_ButtonClick(this, new ToolBarButtonClickEventArgs(Edit_Button));
			Stop.неЗагружаемКартинки = true;
			Reset_World();
			без_игроков = new Игрок[0];
		}

		private void EnableControls(bool value)
		{
			edit_panel.Enabled = value;
			toolBar.Enabled = value;
			foreach (MenuItem menuItem in base.Menu.MenuItems)
			{
				menuItem.Enabled = value;
			}
		}

		private void Exit_Item_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Undo_Item_Click(object sender, EventArgs e)
		{
			UndoAction();
			UpdateUndoRedoState();
		}

		private void Redo_Item_Click(object sender, EventArgs e)
		{
			RedoAction();
			UpdateUndoRedoState();
		}

		private void Find_MinRadius_Item_Click(object sender, EventArgs e)
		{
			double num = 0.0;
			Road road = null;
			Road[] всеДороги = мир.ВсеДороги;
			foreach (Road road2 in всеДороги)
			{
				if (road2.кривая && (num == 0.0 || road2.АбсолютныйРадиус < num))
				{
					road = road2;
					num = road2.АбсолютныйРадиус;
				}
			}
			if (road != null)
			{
				игрок.cameraPosition.XZPoint = road.концы[0];
				MessageBox.Show(this, string.Format(Localization.current_.min_radius, num.ToString("0.00")), "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				MessageBox.Show(this, Localization.current_.no_curves, "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private void Narad_Add_Button_Click(object sender, EventArgs e)
		{
			string номер = (выбранный_маршрут.orders.Length + 1).ToString();
			Order order = new Order(new Парк(""), выбранный_маршрут, номер, "");
			DoRegisterAction(new AddRouteOrderAction(выбранный_маршрут, order));
			modified = true;
		}

		private void Narad_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Narad_Box.SelectedIndex >= 0 && Route_Box.SelectedIndex >= 0;
			Narad_Remove_Button.Enabled = flag;
			Narad_Name_label.Enabled = flag;
			Narad_Name_Box.Enabled = flag;
			Narad_Name_Box.Text = выбранный_наряд.номер;
			Narad_Name_Box.Modified = false;
			Narad_ChangeName_Button.Enabled = false;
			Narad_Park_label.Enabled = flag;
			Narad_Park_Box.Enabled = flag;
			Transport_label.Enabled = flag;
			RollingStockBox.Enabled = flag;
			Narad_Park_Box.SelectedIndex = new List<Парк>(мир.парки).IndexOf(выбранный_наряд.парк);
			Narad_Runs_label.Enabled = flag;
			Narad_Runs_Box.Enabled = flag;
			Narad_Runs_Add_Button.Enabled = flag;
			UpdateNaradControls(flag ? выбранный_наряд : null);
			RollingStockUpdate(выбранный_наряд);
		}

		private void Narad_ChangeName_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Narad_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				выбранный_маршрут.orders[selectedIndex].номер = Narad_Name_Box.Text;
				Narad_Box.Items[selectedIndex] = Narad_Name_Box.Text;
				Narad_Name_Box.Modified = false;
				modified = true;
			}
		}

		private void Narad_Name_Box_ModifiedChanged(object sender, EventArgs e)
		{
			Narad_ChangeName_Button.Enabled = Narad_Name_Box.Modified;
		}

		private void Narad_Park_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Narad_Park_Box.SelectedIndex >= 0 && Narad_Park_Box.SelectedIndex < мир.парки.Length)
			{
				выбранный_наряд.парк = мир.парки[Narad_Park_Box.SelectedIndex];
			}
		}

		private void Narad_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Narad_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveRouteOrderAction(выбранный_маршрут, selectedIndex));
				modified = true;
			}
		}

		private void Narad_Runs_Add_Button_Click(object sender, EventArgs e)
		{
			Trip trip = new Trip
			{
				route = выбранный_маршрут
			};
			DoRegisterAction(new AddOrderTripAction(выбранный_наряд, trip));
			modified = true;
		}

		private void Narad_Runs_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Narad_Runs_Box.SelectedIndex >= 0 && Narad_Box.SelectedIndex >= 0 && Route_Box.SelectedIndex >= 0;
			int selectedIndex = Narad_Runs_Box.SelectedIndex;
			Narad_Runs_Remove_Button.Enabled = flag;
			Narad_Runs_Run_label.Enabled = flag;
			Narad_Runs_Run_Box.Enabled = flag;
			Narad_Runs_Run_Box.Items.Clear();
			Narad_Runs_Run_Box.SelectedIndexChanged -= Narad_Runs_Run_Box_SelectedIndexChanged;
			int selectedIndex2 = ((!flag || выбранный_маршрут.AllTrips.Count <= 0) ? (-1) : 0);
			if (flag)
			{
				for (int i = 0; i < выбранный_маршрут.trips.Count; i++)
				{
					Narad_Runs_Run_Box.Items.Add("Рейс " + (i + 1));
					if (selectedIndex >= 0 && выбранный_маршрут.trips[i].pathes == выбранный_наряд.рейсы[selectedIndex].pathes)
					{
						selectedIndex2 = i;
					}
				}
				for (int j = 0; j < выбранный_маршрут.parkTrips.Count; j++)
				{
					Narad_Runs_Run_Box.Items.Add("Парковый рейс " + (j + 1));
					if (selectedIndex >= 0 && выбранный_маршрут.parkTrips[j].pathes == выбранный_наряд.рейсы[selectedIndex].pathes)
					{
						selectedIndex2 = j + выбранный_маршрут.trips.Count;
					}
				}
			}
			Narad_Runs_Run_Box.SelectedIndex = selectedIndex2;
			Narad_Runs_Run_Box.SelectedIndexChanged += Narad_Runs_Run_Box_SelectedIndexChanged;
			Narad_Runs_Time1_label.Enabled = flag;
			Narad_Runs_Time1_Box.Enabled = flag;
			if (flag)
			{
				Narad_Runs_Time1_Box.Time_Seconds = (int)выбранный_наряд.рейсы[selectedIndex].время_отправления;
				Narad_Runs_Time1_Box_TimeChanged(sender, e);
			}
			else
			{
				Narad_Runs_Time1_Box.Time_Seconds = 0;
				Narad_Runs_Time2_Box.Time_Seconds = 0;
			}
			Narad_Runs_Time2_label.Enabled = flag;
			Narad_Runs_Run_Box_SelectedIndexChanged(sender, e);
		}

		private void Narad_Runs_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Narad_Runs_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveOrderTripAction(выбранный_наряд, selectedIndex));
				modified = true;
			}
		}

		private void Narad_Runs_Run_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = Narad_Runs_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				выбранный_наряд.рейсы[selectedIndex].pathes = выбранный_маршрут.AllTrips[Narad_Runs_Run_Box.SelectedIndex].pathes;
				Narad_Runs_Time1_Box_TimeChanged(sender, e);
			}
		}

		private void Narad_Runs_Time1_Box_TimeChanged(object sender, EventArgs e)
		{
			int selectedIndex = Narad_Runs_Box.SelectedIndex;
			if (selectedIndex < 0)
			{
				return;
			}
			выбранный_наряд.рейсы[selectedIndex].время_отправления = Narad_Runs_Time1_Box.Time_Seconds;
			if (выбранный_наряд.рейсы[selectedIndex].время_отправления < 10800.0)
			{
				выбранный_наряд.рейсы[selectedIndex].время_отправления += 86400.0;
			}
			double num = 0.0;
			foreach (Trip allTrip in выбранный_маршрут.AllTrips)
			{
				if (allTrip.pathes == выбранный_наряд.рейсы[selectedIndex].pathes)
				{
					num = allTrip.время_прибытия;
					break;
				}
			}
			выбранный_наряд.рейсы[selectedIndex].время_прибытия = выбранный_наряд.рейсы[selectedIndex].время_отправления + num;
			Narad_Runs_Time2_Box.Time_Seconds = (int)выбранный_наряд.рейсы[selectedIndex].время_прибытия;
		}

		private void New_Item_Click(object sender, EventArgs e)
		{
			if (Set_New_File(saveFileDialog))
			{
				Reset_World();
			}
		}

		private void Open_Item_Click(object sender, EventArgs e)
		{
			if (Set_New_File(openFileDialog))
			{
				мир = new World();
				игрок = new Игрок();
				игроки = new Игрок[1] { игрок };
				игра = new Game();
				игра.мир = мир;
				игра.игроки = игроки;
				мир.ЗагрузитьГород(filename);
				мир.Create_Meshes();
				UpdatePanels();
			}
		}

		private void Reset_World()
		{
			мир = new World();
			мир.Create_Meshes();
			игрок = new Игрок();
			игроки = new Игрок[1] { игрок };
			игра = new Game();
			игра.мир = мир;
			игра.игроки = игроки;
			UpdatePanels();
		}

		private void panel_MouseDown(object sender, MouseEventArgs e)
		{
			mouse_args = e;
			if (e.Button == MouseButtons.Right)
			{
				dragging = true;
				drag_point = new Point(e.X, e.Y);
			}
			process_mouse(e, click: true);
		}

		private void panel_MouseLeave(object sender, EventArgs e)
		{
			Cursor_x_Status.Text = "";
			Cursor_y_Status.Text = "";
		}

		private void panel_MouseMove(object sender, MouseEventArgs e)
		{
			mouse_args = e;
			if (dragging)
			{
				DoublePoint doublePoint = new DoublePoint(-e.X + drag_point.X, e.Y - drag_point.Y);
				doublePoint /= MyDirect3D.масштаб;
				игрок.cameraPosition.XZPoint += doublePoint;
				drag_point = new Point(e.X, e.Y);
			}
			process_mouse(e, click: false);
			Cursor_x_Status.Text = "x: " + (cursor_pos.x / 1000.0).ToString("0.000") + " км";
			Cursor_y_Status.Text = "y: " + (cursor_pos.y / 1000.0).ToString("0.000") + " км";
		}

		private void panel_MouseUp(object sender, MouseEventArgs e)
		{
			mouse_args = e;
			if (e.Button == MouseButtons.Right)
			{
				dragging = false;
			}
		}

		private void Park_Add_Button_Click(object sender, EventArgs e)
		{
			DoRegisterAction(new AddDepotAction(new Парк("Парк")));
			modified = true;
		}

		private void Park_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			Check_All_Park_Boxes();
		}

		private void Check_All_Park_Boxes()
		{
			bool flag = Park_Box.SelectedIndex >= 0;
			Park_In_Button.Enabled = flag;
			Park_Out_Button.Enabled = flag;
			Park_Rails_Button.Enabled = flag;
			Park_Remove_Button.Enabled = flag;
			Park_Name_label.Enabled = flag;
			Park_Name_Box.Enabled = flag;
			Park_Name_Box.Text = (flag ? мир.парки[Park_Box.SelectedIndex].название : string.Empty);
			Park_Name_Box.Modified = false;
			Park_ChangeName_Button.Enabled = false;
			ОбновитьРаскрашенныеСплайны();
		}

		private void Park_ChangeName_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Park_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.парки[selectedIndex].название = Park_Name_Box.Text;
				UpdateParksList();
				Park_Box.SelectedIndex = selectedIndex;
				Park_Name_Box.Modified = false;
				Narad_Box_SelectedIndexChanged(sender, e);
				modified = true;
			}
		}

		private void Park_Name_Box_ModifiedChanged(object sender, EventArgs e)
		{
			Park_ChangeName_Button.Enabled = Park_Name_Box.Modified;
		}

		private void Park_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Park_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveDepotAction(selectedIndex));
				modified = true;
			}
		}

		private void process_mouse(MouseEventArgs e, bool click)
		{
			Положение положение = мир.Найти_ближайшее_положение(cursor_pos);
			if (строящаяся_дорога == null)
			{
				if (Rail_Button.Pushed && click && e.Button == MouseButtons.Left && положение.Дорога != null)
				{
					if (!Spline_Select_mode_Box.Checked)
					{
						DoRegisterAction(new RemoveRoadAction(положение.Дорога));
						modified = true;
					}
					else
					{
						Splines_Instance_Box.SelectedIndex = мир.listДороги.IndexOf(положение.Дорога);
						time_color = 2.0;
						ОбновитьРаскрашенныеСплайны();
					}
				}
			}
			else
			{
				строящаяся_дорога.кривая = Rail_Build_Curve_Button.Pushed;
				DoublePoint point = new DoublePoint(20.0, строящаяся_дорога.кривая ? 10.0 : 0.0);
				DoublePoint point2 = new DoublePoint(-20.0, строящаяся_дорога.кривая ? 10.0 : 0.0);
				DoublePoint doublePoint = (shift ? cursor_pos.RoundPoint : cursor_pos);
				if (ctrl && строящаяся_дорога.кривая && строительство_дороги != 0)
				{
					строящаяся_дорога.ОбновитьСтруктуру();
					if (строительство_дороги == Стадия_стоительства.Второй_конец)
					{
						DoublePoint doublePoint2 = строящаяся_дорога.концы[0] + new DoublePoint(строящаяся_дорога.направления[0] + Math.PI / 2.0);
						DoublePoint point3 = строящаяся_дорога.концы[0] - doublePoint2;
						DoublePoint point4 = new DoublePoint(Math.PI + строящаяся_дорога.направления[1] - строящаяся_дорога.направления[0]);
						point3.Multyply(ref point4);
						DoublePoint doublePoint3 = doublePoint2.Add(ref point3);
						DoublePoint doublePoint4 = doublePoint - строящаяся_дорога.концы[0];
						DoublePoint point5 = new DoublePoint(doublePoint3.Subtract(ref строящаяся_дорога.концы[0]).Angle);
						doublePoint4.Divide(ref point5);
						doublePoint4.y = 0.0;
						doublePoint4.Multyply(ref point5);
						doublePoint = doublePoint4.Add(ref строящаяся_дорога.концы[0]);
					}
					if (строительство_дороги == Стадия_стоительства.Первый_конец)
					{
						DoublePoint doublePoint5 = строящаяся_дорога.концы[1] + new DoublePoint(строящаяся_дорога.направления[1] + Math.PI / 2.0);
						DoublePoint point3 = строящаяся_дорога.концы[1] - doublePoint5;
						DoublePoint point4 = new DoublePoint(Math.PI + строящаяся_дорога.направления[0] - строящаяся_дорога.направления[1]);
						point3.Multyply(ref point4);
						DoublePoint doublePoint6 = doublePoint5.Add(ref point3);
						DoublePoint doublePoint7 = doublePoint - строящаяся_дорога.концы[1];
						DoublePoint point6 = new DoublePoint(doublePoint6.Subtract(ref строящаяся_дорога.концы[1]).Angle);
						doublePoint7.Divide(ref point6);
						doublePoint7.y = 0.0;
						doublePoint7.Multyply(ref point6);
						doublePoint = doublePoint7.Add(ref строящаяся_дорога.концы[1]);
					}
				}
				switch (строительство_дороги)
				{
				case Стадия_стоительства.Нет:
					строящаяся_дорога.концы[0] = doublePoint;
					строящаяся_дорога.концы[1] = doublePoint + point;
					break;
				case Стадия_стоительства.Второй_конец:
					строящаяся_дорога.концы[1] = doublePoint;
					break;
				case Стадия_стоительства.Первый_конец:
					строящаяся_дорога.концы[0] = doublePoint;
					break;
				}
				bool flag = false;
				bool flag2 = false;
				if ((строящаяся_дорога.концы[1] - строящаяся_дорога.концы[0]).Modulus > 1.0)
				{
					Road[] array = мир.Дороги;
					if (строящаяся_дорога is Рельс)
					{
						array = мир.Рельсы;
					}
					Road[] array2 = array;
					foreach (Road road in array2)
					{
						if ((строительство_дороги == Стадия_стоительства.Нет || строительство_дороги == Стадия_стоительства.Первый_конец) && (строящаяся_дорога.концы[0] - road.концы[1]).Modulus < 1.0)
						{
							flag2 = true;
							строящаяся_дорога.концы[0] = road.концы[1];
							if (строящаяся_дорога.кривая || строительство_дороги == Стадия_стоительства.Нет)
							{
								строящаяся_дорога.направления[0] = road.направления[1] + Math.PI;
								if (!строящаяся_дорога.кривая)
								{
									строящаяся_дорога.направления[1] = строящаяся_дорога.направления[0] - Math.PI;
								}
							}
							строящаяся_дорога.ширина[0] = road.ширина[1];
							строящаяся_дорога.высота[0] = road.высота[1];
							if (строительство_дороги == Стадия_стоительства.Нет)
							{
								строящаяся_дорога.ширина[1] = road.ширина[1];
								строящаяся_дорога.высота[1] = road.высота[1];
								строящаяся_дорога.концы[1] = строящаяся_дорога.концы[0] + new DoublePoint(строящаяся_дорога.направления[0]).Multyply(ref point);
							}
							break;
						}
						if ((строительство_дороги != 0 && строительство_дороги != Стадия_стоительства.Второй_конец) || !((строящаяся_дорога.концы[1] - road.концы[0]).Modulus < 1.0))
						{
							continue;
						}
						flag = true;
						строящаяся_дорога.концы[1] = road.концы[0];
						if (строящаяся_дорога.кривая || строительство_дороги == Стадия_стоительства.Нет)
						{
							строящаяся_дорога.направления[1] = road.направления[0] + Math.PI;
							if (!строящаяся_дорога.кривая)
							{
								строящаяся_дорога.направления[0] = строящаяся_дорога.направления[1] - Math.PI;
							}
						}
						строящаяся_дорога.ширина[1] = road.ширина[0];
						строящаяся_дорога.высота[1] = road.высота[0];
						if (строительство_дороги == Стадия_стоительства.Нет)
						{
							строящаяся_дорога.высота[0] = road.высота[0];
							строящаяся_дорога.ширина[0] = road.ширина[0];
							строящаяся_дорога.концы[0] = строящаяся_дорога.концы[1] + new DoublePoint(road.направления[0]).Multyply(ref point2);
						}
						break;
					}
				}
				if (!строящаяся_дорога.кривая)
				{
					double num = Math.Abs(строящаяся_дорога.ширина[0] - строящаяся_дорога.ширина[1]) / 2.0;
					DoublePoint point7 = default(DoublePoint);
					if (строительство_дороги == Стадия_стоительства.Нет)
					{
						if (!flag2 && !flag)
						{
							строящаяся_дорога.концы[1] = строящаяся_дорога.концы[0] + new DoublePoint(строящаяся_дорога.направления[0]).Multyply(ref point);
							строящаяся_дорога.направления[1] = строящаяся_дорога.направления[0] + Math.PI;
							if (строящаяся_дорога.направления[1] > Math.PI)
							{
								строящаяся_дорога.направления[1] -= Math.PI * 2.0;
							}
						}
					}
					else if (строительство_дороги == Стадия_стоительства.Второй_конец)
					{
						DoublePoint doublePoint8 = строящаяся_дорога.концы[1] - строящаяся_дорога.концы[0];
						point7.CopyFromAngle(строящаяся_дорога.направления[0]);
						doublePoint8.Divide(ref point7);
						if (ctrl)
						{
							doublePoint8.y = 0.0;
						}
						else if (Math.Abs(doublePoint8.y) > num)
						{
							doublePoint8.y = (double)Math.Sign(doublePoint8.y) * num;
						}
						if (doublePoint8.x < 1.0)
						{
							doublePoint8.x = 1.0;
						}
						doublePoint8.Multyply(ref point7);
						строящаяся_дорога.концы[1] = doublePoint8.Add(ref строящаяся_дорога.концы[0]);
					}
					else if (строительство_дороги == Стадия_стоительства.Первый_конец)
					{
						DoublePoint doublePoint9 = строящаяся_дорога.концы[0] - строящаяся_дорога.концы[1];
						point7.CopyFromAngle(строящаяся_дорога.направления[1]);
						doublePoint9.Divide(ref point7);
						if (ctrl)
						{
							doublePoint9.y = 0.0;
						}
						else if (Math.Abs(doublePoint9.y) > num)
						{
							doublePoint9.y = (double)Math.Sign(doublePoint9.y) * num;
						}
						if (doublePoint9.x < 1.0)
						{
							doublePoint9.x = 1.0;
						}
						doublePoint9.Multyply(ref point7);
						строящаяся_дорога.концы[0] = doublePoint9.Add(ref строящаяся_дорога.концы[1]);
					}
				}
				строящаяся_дорога.ОбновитьСледующиеДороги(мир.ВсеДороги);
				if (click && e.Button == MouseButtons.Left)
				{
					switch (строительство_дороги)
					{
					case Стадия_стоительства.Нет:
						if (!flag2)
						{
							if (flag)
							{
								строительство_дороги = Стадия_стоительства.Первый_конец;
							}
							else
							{
								строительство_дороги = Стадия_стоительства.Второй_конец;
							}
						}
						else
						{
							строительство_дороги = Стадия_стоительства.Второй_конец;
						}
						break;
					case Стадия_стоительства.Второй_конец:
					case Стадия_стоительства.Первый_конец:
					{
						строительство_дороги = Стадия_стоительства.Нет;
						строящаяся_дорога.Color = 0;
						AddRoadsAction addRoadsAction = null;
						if (Rail_Build_попутки_Button.Pushed && Rail_Build_попутки2_Button.Pushed)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога, строящаяся_дорога1);
						}
						if (Rail_Build_попутки_Button.Pushed && Rail_Build_попутки3_Button.Pushed)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога, строящаяся_дорога1, строящаяся_дорога2);
							мир.listДороги.Add(строящаяся_дорога1);
							мир.listДороги.Add(строящаяся_дорога2);
						}
						if (Rail_Build_встречки_Button.Pushed && Rail_Build_встречки1_Button.Pushed)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога, строящаяся_обратная_дорога);
						}
						if (Rail_Build_встречки_Button.Pushed && Rail_Build_встречки2_Button.Pushed)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога, строящаяся_обратная_дорога, строящаяся_обратная_дорога1);
						}
						if (Rail_Build_встречки_Button.Pushed && Rail_Build_встречки3_Button.Pushed)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога, строящаяся_обратная_дорога, строящаяся_обратная_дорога1, строящаяся_обратная_дорога2);
						}
						if (addRoadsAction == null)
						{
							addRoadsAction = new AddRoadsAction(строящаяся_дорога);
						}
						DoRegisterAction(addRoadsAction);
						modified = true;
						строящаяся_дорога = null;
						toolBar_ButtonClick(null, new ToolBarButtonClickEventArgs(null));
						break;
					}
					}
				}
			}
			if (строящиеся_провода == null)
			{
				if (Troll_lines_Button.Pushed && click && e.Button == MouseButtons.Left)
				{
					if (ближайшие_провода != null)
					{
						DoRegisterAction(new RemoveWiresAction(ближайшие_провода));
						modified = true;
					}
					else if (ближайший_провод != null)
					{
						DoRegisterAction(new RemoveTramWireAction(ближайший_провод));
						modified = true;
					}
				}
			}
			else
			{
				DoublePoint point8 = new DoublePoint(20.0, 0.0);
				DoublePoint point9 = new DoublePoint(-20.0, 0.0);
				DoublePoint doublePoint10 = (shift ? cursor_pos.RoundPoint : cursor_pos);
				switch (строящиеся_провода.стадия)
				{
				case Стадия_стоительства.Нет:
					строящиеся_провода.концы[0] = doublePoint10;
					строящиеся_провода.концы[1] = doublePoint10 + point8;
					break;
				case Стадия_стоительства.Второй_конец:
					строящиеся_провода.концы[1] = doublePoint10;
					break;
				case Стадия_стоительства.Первый_конец:
					строящиеся_провода.концы[0] = doublePoint10;
					break;
				}
				bool flag3 = false;
				bool flag4 = false;
				bool flag5 = строящиеся_провода is Строящиеся_трамвайные_провода;
				if ((строящиеся_провода.концы[1] - строящиеся_провода.концы[0]).Modulus > 1.0)
				{
					if (!flag5)
					{
						for (int j = 0; j < мир.контактныеПровода.Length - 1; j += 2)
						{
							Контактный_провод контактный_провод = мир.контактныеПровода[j];
							Контактный_провод контактный_провод2 = мир.контактныеПровода[j + 1];
							DoublePoint[] начало;
							if ((строящиеся_провода.стадия == Стадия_стоительства.Нет || строящиеся_провода.стадия == Стадия_стоительства.Первый_конец) && (строящиеся_провода.концы[0] - (контактный_провод.конец + контактный_провод2.конец) / 2.0).Modulus < 1.0)
							{
								flag4 = true;
								строящиеся_провода.концы[0] = (контактный_провод.конец + контактный_провод2.конец) / 2.0;
								начало = new DoublePoint[2] { контактный_провод.конец, контактный_провод2.конец };
								строящиеся_провода.начало = начало;
								double num2 = (контактный_провод2.конец - контактный_провод.конец).Angle - Math.PI / 2.0;
								строящиеся_провода.направления[0] = num2;
								строящиеся_провода.высота[0] = контактный_провод.высота[1];
								if (строящиеся_провода.стадия == Стадия_стоительства.Нет)
								{
									строящиеся_провода.высота[1] = контактный_провод.высота[1];
									DoublePoint doublePoint11 = контактный_провод.конец - контактный_провод2.конец;
									if (Math.Abs(Контактный_провод.расстояние_между_проводами - doublePoint11.Modulus) < 0.001)
									{
										строящиеся_провода.направление = num2;
									}
									else
									{
										строящиеся_провода.направление = 2.0 * num2 - контактный_провод.направление;
									}
									строящиеся_провода.направления[1] = строящиеся_провода.направление;
									строящиеся_провода.концы[1] = строящиеся_провода.концы[0] + new DoublePoint(строящиеся_провода.направление).Multyply(ref point8);
								}
								break;
							}
							if ((строящиеся_провода.стадия != 0 && строящиеся_провода.стадия != Стадия_стоительства.Второй_конец) || !((строящиеся_провода.концы[1] - (контактный_провод.начало + контактный_провод2.начало) / 2.0).Modulus < 1.0))
							{
								continue;
							}
							flag3 = true;
							строящиеся_провода.концы[1] = (контактный_провод.начало + контактный_провод2.начало) / 2.0;
							начало = new DoublePoint[2] { контактный_провод.начало, контактный_провод2.начало };
							строящиеся_провода.конец = начало;
							double num3 = (контактный_провод2.начало - контактный_провод.начало).Angle - Math.PI / 2.0;
							строящиеся_провода.направления[1] = num3;
							строящиеся_провода.высота[1] = контактный_провод.высота[0];
							if (строящиеся_провода.стадия == Стадия_стоительства.Нет)
							{
								строящиеся_провода.высота[0] = контактный_провод.высота[0];
								DoublePoint doublePoint11 = контактный_провод.начало - контактный_провод2.начало;
								if (Math.Abs(Контактный_провод.расстояние_между_проводами - doublePoint11.Modulus) < 0.001)
								{
									строящиеся_провода.направление = num3;
								}
								else
								{
									строящиеся_провода.направление = 2.0 * num3 - контактный_провод.направление;
								}
								строящиеся_провода.направления[0] = строящиеся_провода.направление;
								строящиеся_провода.концы[0] = строящиеся_провода.концы[1] + new DoublePoint(строящиеся_провода.направление).Multyply(ref point9);
							}
							break;
						}
					}
					else
					{
						for (int k = 0; k < мир.контактныеПровода2.Length; k++)
						{
							Трамвайный_контактный_провод трамвайный_контактный_провод = мир.контактныеПровода2[k];
							if ((строящиеся_провода.стадия == Стадия_стоительства.Нет || строящиеся_провода.стадия == Стадия_стоительства.Первый_конец) && (строящиеся_провода.концы[0] - трамвайный_контактный_провод.конец).Modulus < 1.0)
							{
								flag4 = true;
								строящиеся_провода.концы[0] = трамвайный_контактный_провод.конец;
								строящиеся_провода.начало = new DoublePoint[1] { трамвайный_контактный_провод.конец };
								if (!((Строящиеся_трамвайные_провода)строящиеся_провода).flag)
								{
									строящиеся_провода.направление = трамвайный_контактный_провод.направление;
									((Строящиеся_трамвайные_провода)строящиеся_провода).flag = true;
								}
								строящиеся_провода.высота[0] = трамвайный_контактный_провод.высота[1];
								if (строящиеся_провода.стадия == Стадия_стоительства.Нет)
								{
									строящиеся_провода.высота[1] = трамвайный_контактный_провод.высота[1];
									строящиеся_провода.направления[1] = строящиеся_провода.направление;
									строящиеся_провода.концы[1] = строящиеся_провода.концы[0] + new DoublePoint(строящиеся_провода.направление).Multyply(ref point8);
								}
								break;
							}
							if ((строящиеся_провода.стадия == Стадия_стоительства.Нет || строящиеся_провода.стадия == Стадия_стоительства.Второй_конец) && (строящиеся_провода.концы[1] - трамвайный_контактный_провод.начало).Modulus < 1.0)
							{
								flag3 = true;
								строящиеся_провода.концы[1] = трамвайный_контактный_провод.начало;
								строящиеся_провода.конец = new DoublePoint[1] { трамвайный_контактный_провод.начало };
								строящиеся_провода.высота[1] = трамвайный_контактный_провод.высота[0];
								if (строящиеся_провода.стадия == Стадия_стоительства.Нет)
								{
									строящиеся_провода.высота[0] = трамвайный_контактный_провод.высота[0];
									строящиеся_провода.направления[0] = строящиеся_провода.направление;
									строящиеся_провода.концы[0] = строящиеся_провода.концы[1] + new DoublePoint(строящиеся_провода.направление).Multyply(ref point9);
								}
								break;
							}
						}
					}
				}
				if (строящиеся_провода.стадия == Стадия_стоительства.Нет)
				{
					if (!flag4 && !flag3)
					{
						строящиеся_провода.концы[1] = строящиеся_провода.концы[0] + new DoublePoint(строящиеся_провода.направление).Multyply(ref point8);
						строящиеся_провода.начало = null;
						строящиеся_провода.конец = null;
						if (flag5)
						{
							((Строящиеся_трамвайные_провода)строящиеся_провода).flag = false;
						}
					}
				}
				else if (строящиеся_провода.стадия == Стадия_стоительства.Второй_конец)
				{
					DoublePoint doublePoint12 = строящиеся_провода.концы[1] - строящиеся_провода.концы[0];
					DoublePoint point10 = new DoublePoint(строящиеся_провода.направление);
					doublePoint12.Divide(ref point10);
					if (ctrl)
					{
						doublePoint12.y = 0.0;
					}
					if (doublePoint12.x < 1.0)
					{
						doublePoint12.x = 1.0;
					}
					doublePoint12.Multyply(ref point10);
					строящиеся_провода.концы[1] = doublePoint12.Add(ref строящиеся_провода.концы[0]);
					if (!flag3)
					{
						строящиеся_провода.конец = null;
					}
				}
				else if (строящиеся_провода.стадия == Стадия_стоительства.Первый_конец)
				{
					DoublePoint doublePoint13 = строящиеся_провода.концы[0] - строящиеся_провода.концы[1];
					DoublePoint point11 = new DoublePoint(строящиеся_провода.направление + Math.PI);
					doublePoint13.Divide(ref point11);
					if (ctrl)
					{
						doublePoint13.y = 0.0;
					}
					if (doublePoint13.x < 1.0)
					{
						doublePoint13.x = 1.0;
					}
					doublePoint13.Multyply(ref point11);
					строящиеся_провода.концы[0] = doublePoint13.Add(ref строящиеся_провода.концы[1]);
					if (!flag4)
					{
						строящиеся_провода.начало = null;
					}
				}
				строящиеся_провода.Обновить();
				if (click && e.Button == MouseButtons.Left)
				{
					switch (строящиеся_провода.стадия)
					{
					case Стадия_стоительства.Нет:
						if (!flag4)
						{
							if (flag3)
							{
								строящиеся_провода.стадия = Стадия_стоительства.Первый_конец;
							}
							else
							{
								строящиеся_провода.стадия = Стадия_стоительства.Второй_конец;
							}
						}
						else
						{
							строящиеся_провода.стадия = Стадия_стоительства.Второй_конец;
						}
						break;
					case Стадия_стоительства.Второй_конец:
					case Стадия_стоительства.Первый_конец:
						строящиеся_провода.стадия = Стадия_стоительства.Нет;
						строящиеся_провода.провода[0].color = 0;
						if (!flag5)
						{
							строящиеся_провода.провода[1].color = 0;
						}
						if (flag5)
						{
							строящиеся_провода.провода[0].обесточенный = false;
							Контактный_провод[] контактныеПровода = мир.контактныеПровода;
							foreach (Контактный_провод контактный_провод3 in контактныеПровода)
							{
								if (строящиеся_провода.провода[0].обесточенный)
								{
									break;
								}
								DoublePoint doublePoint14 = контактный_провод3.конец - контактный_провод3.начало;
								if (контактный_провод3.высота[0] != строящиеся_провода.провода[0].высота[0] || контактный_провод3.высота[1] != строящиеся_провода.провода[0].высота[1])
								{
									continue;
								}
								DoublePoint doublePoint15 = строящиеся_провода.провода[0].начало - контактный_провод3.начало;
								DoublePoint doublePoint16 = строящиеся_провода.провода[0].конец - контактный_провод3.начало;
								doublePoint15.Angle -= doublePoint14.Angle;
								doublePoint16.Angle -= doublePoint14.Angle;
								if (Math.Sign(doublePoint15.y) != Math.Sign(doublePoint16.y))
								{
									double num4 = doublePoint15.x + (doublePoint16.x - doublePoint15.x) * (0.0 - doublePoint15.y) / (doublePoint16.y - doublePoint15.y);
									if (num4 > 0.001 && num4 < doublePoint14.Modulus - 0.001)
									{
										DoublePoint doublePoint11 = new DoublePoint(контактный_провод3.направление - строящиеся_провода.провода[0].направление);
										строящиеся_провода.провода[0].обесточенный = doublePoint11.Angle != 0.0;
									}
								}
							}
						}
						else
						{
							Трамвайный_контактный_провод[] контактныеПровода2 = мир.контактныеПровода2;
							foreach (Контактный_провод контактный_провод4 in контактныеПровода2)
							{
								DoublePoint doublePoint17 = контактный_провод4.конец - контактный_провод4.начало;
								for (int l = 0; l < 2; l++)
								{
									if (контактный_провод4.высота[0] != строящиеся_провода.провода[l].высота[0] || контактный_провод4.высота[1] != строящиеся_провода.провода[l].высота[1])
									{
										continue;
									}
									DoublePoint doublePoint18 = строящиеся_провода.провода[l].начало - контактный_провод4.начало;
									DoublePoint doublePoint19 = строящиеся_провода.провода[l].конец - контактный_провод4.начало;
									doublePoint18.Angle -= doublePoint17.Angle;
									doublePoint19.Angle -= doublePoint17.Angle;
									if (Math.Sign(doublePoint18.y) != Math.Sign(doublePoint19.y))
									{
										double num5 = doublePoint18.x + (doublePoint18.x - doublePoint19.x) * doublePoint18.y / (doublePoint19.y - doublePoint18.y);
										if (num5 > 0.001 && num5 < doublePoint17.Modulus - 0.001)
										{
											контактный_провод4.обесточенный = new DoublePoint(контактный_провод4.направление - строящиеся_провода.провода[l].направление).Angle != 0.0;
										}
									}
								}
							}
							Контактный_провод[] контактныеПровода = мир.контактныеПровода;
							foreach (Контактный_провод контактный_провод5 in контактныеПровода)
							{
								DoublePoint doublePoint20 = контактный_провод5.конец - контактный_провод5.начало;
								for (int m = 0; m < 2; m++)
								{
									if (контактный_провод5.высота[0] != строящиеся_провода.провода[m].высота[0] || контактный_провод5.высота[1] != строящиеся_провода.провода[m].высота[1])
									{
										continue;
									}
									DoublePoint doublePoint21 = строящиеся_провода.провода[m].начало - контактный_провод5.начало;
									DoublePoint doublePoint22 = строящиеся_провода.провода[m].конец - контактный_провод5.начало;
									doublePoint21.Angle -= doublePoint20.Angle;
									doublePoint22.Angle -= doublePoint20.Angle;
									if (Math.Sign(doublePoint21.y) == Math.Sign(doublePoint22.y))
									{
										continue;
									}
									double num6 = doublePoint21.x + (doublePoint21.x - doublePoint22.x) * doublePoint21.y / (doublePoint22.y - doublePoint21.y);
									if (num6 > 0.001 && num6 < doublePoint20.Modulus - 0.001)
									{
										if (контактный_провод5.правый == строящиеся_провода.провода[m].правый)
										{
											контактный_провод5.обесточенный = true;
											строящиеся_провода.провода[m].обесточенный = true;
										}
										else if (new DoublePoint(контактный_провод5.направление - строящиеся_провода.провода[m].направление).Angle < 0.0)
										{
											контактный_провод5.обесточенный = true;
										}
										else
										{
											строящиеся_провода.провода[m].обесточенный = true;
										}
									}
								}
							}
						}
						DoPendingAction();
						modified = true;
						строящиеся_провода = null;
						toolBar_ButtonClick(null, new ToolBarButtonClickEventArgs(null));
						break;
					}
				}
			}
			UpdateStatusBar();
			if (строящаяся_остановка != null && положение.Дорога != null)
			{
				строящаяся_остановка.road = положение.Дорога;
				строящаяся_остановка.distance = положение.расстояние;
				строящаяся_остановка.UpdatePosition(мир);
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящаяся_остановка = null;
					EnableControls(value: true);
				}
			}
			if (строящийся_элемент_сигнальной_системы != null && положение.Дорога != null)
			{
				строящийся_элемент_сигнальной_системы.дорога = положение.Дорога;
				строящийся_элемент_сигнальной_системы.расстояние = положение.расстояние;
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящийся_элемент_сигнальной_системы = null;
					EnableControls(value: true);
				}
			}
			if (строящийся_сигнал_сигнальной_системы != null && положение.Дорога != null)
			{
				строящийся_сигнал_сигнальной_системы.road = положение.Дорога;
				строящийся_сигнал_сигнальной_системы.положение.расстояние = Math.Round(положение.расстояние);
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящийся_сигнал_сигнальной_системы = null;
					EnableControls(value: true);
				}
			}
			if (строящийся_светофор != null && положение.Дорога != null)
			{
				строящийся_светофор.положение.Дорога = положение.Дорога;
				строящийся_светофор.положение.расстояние = Math.Round(положение.расстояние);
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящийся_светофор = null;
					EnableControls(value: true);
				}
			}
			if (строящийся_светофорный_сигнал != null && положение.Дорога != null)
			{
				строящийся_светофорный_сигнал.дорога = положение.Дорога;
				строящийся_светофорный_сигнал.расстояние = положение.расстояние;
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящийся_светофорный_сигнал = null;
					EnableControls(value: true);
				}
				ОбновитьРаскрашенныеСплайны();
			}
			if (click && e.Button == MouseButtons.Left && положение.Дорога != null)
			{
				if (Park_Button.Pushed && Park_Box.SelectedIndex >= 0)
				{
					Парк depot = мир.парки[Park_Box.SelectedIndex];
					if (Park_In_Button.Pushed)
					{
						DoRegisterAction(new SetDepotEnterAction(depot, положение.Дорога));
						modified = true;
					}
					if (Park_Out_Button.Pushed)
					{
						DoRegisterAction(new SetDepotExitAction(depot, положение.Дорога));
						modified = true;
					}
					if (Park_Rails_Button.Pushed)
					{
						DoRegisterAction(new ToggleDepotPathAction(depot, положение.Дорога));
						modified = true;
					}
				}
				if (Stops_Button.Pushed && Stops_Box.SelectedIndex >= 0)
				{
					Stop stop = мир.остановки[Stops_Box.SelectedIndex];
					int num7 = -1;
					for (int n = 0; n < stop.частьПути.Length; n++)
					{
						if (stop.частьПути[n] == положение.Дорога)
						{
							num7 = n;
							break;
						}
					}
					List<Road> list = new List<Road>(stop.частьПути);
					if (num7 >= 0)
					{
						list.RemoveAt(num7);
					}
					else
					{
						list.Add(положение.Дорога);
					}
					stop.частьПути = list.ToArray();
					ОбновитьРаскрашенныеСплайны();
					modified = true;
				}
				if (Route_Button.Pushed && Route_Box.SelectedIndex >= 0 && Route_Runs_Box.SelectedIndex >= 0)
				{
					Trip trip = выбранный_рейс;
					List<Road> list2 = new List<Road>(trip.pathes);
					if (trip.дорога_прибытия == положение.Дорога)
					{
						list2.RemoveAt(trip.pathes.Length - 1);
					}
					else if (trip.pathes.Length == 0 || new List<Road>(trip.дорога_прибытия.следующиеДороги).Contains(положение.Дорога))
					{
						list2.Add(положение.Дорога);
					}
					trip.pathes = list2.ToArray();
					Route_Runs_ToParkIndex_UpDown.Maximum = выбранный_рейс.pathes.Length;
					ОбновитьРаскрашенныеСплайны();
					modified = true;
				}
			}
			if (строящийся_объект != null)
			{
				строящийся_объект.position = cursor_pos;
				if (click && e.Button == MouseButtons.Left)
				{
					DoPendingAction();
					строящийся_объект = null;
					EnableControls(value: true);
					modified = true;
				}
			}
			else
			{
				if (!Object_Button.Pushed || мир.объекты.Count <= 0 || !click || e.Button != MouseButtons.Left)
				{
					return;
				}
				double num8 = 250.0;
				int num9 = -1;
				for (int num10 = 0; num10 < мир.объекты.Count; num10++)
				{
					double modulus = (мир.объекты[num10].bounding_sphere.position.XZPoint - cursor_pos).Modulus;
					if (modulus < num8)
					{
						num8 = modulus;
						num9 = num10;
					}
				}
				if (num9 >= 0)
				{
					Objects_Instance_Box.SelectedIndex = num9;
				}
			}
		}

		private void UpdateStatusBar()
		{
			Coord_x1_Status.Text = "";
			Coord_y1_Status.Text = "";
			Angle1_Status.Text = "";
			Coord_x2_Status.Text = "";
			Coord_y2_Status.Text = "";
			Radius_Status.Text = "";
			Angle_Status.Text = "";
			Angle2_Status.Text = "";
			Length_Status.Text = "";
			Wide0_Status.Text = "";
			Wide1_Status.Text = "";
			Height0_Status.Text = "";
			Height1_Status.Text = "";
			Maschtab.Text = "масштаб: " + MyDirect3D.масштаб.ToString("0.0");
			if (строящаяся_дорога != null)
			{
				строящаяся_дорога.ОбновитьСтруктуру();
				double num = строящаяся_дорога.концы[0].x / 1000.0;
				Coord_x1_Status.Text = "x: " + num.ToString("0.000") + " км";
				num = строящаяся_дорога.концы[0].y / 1000.0;
				Coord_y1_Status.Text = "y: " + num.ToString("0.000") + " км";
				num = строящаяся_дорога.направления[0] * 180.0 / Math.PI;
				Angle1_Status.Text = num.ToString("0") + "°";
				num = строящаяся_дорога.концы[1].x / 1000.0;
				Coord_x2_Status.Text = "x: " + num.ToString("0.000") + " км";
				num = строящаяся_дорога.концы[1].y / 1000.0;
				Coord_y2_Status.Text = "y: " + num.ToString("0.000") + " км";
				num = строящаяся_дорога.направления[1] * 180.0 / Math.PI;
				Angle2_Status.Text = num.ToString("0") + "°";
				Length_Status.Text = "l: " + строящаяся_дорога.Длина.ToString("0.0") + " м";
				Wide0_Status.Text = "w1: " + строящаяся_дорога.ширина[0].ToString("0.0") + " м";
				Wide1_Status.Text = "w2: " + строящаяся_дорога.ширина[1].ToString("0.0") + " м";
				Height0_Status.Text = "h1: " + строящаяся_дорога.высота[0].ToString("0.0") + " м";
				Height1_Status.Text = "h2: " + строящаяся_дорога.высота[1].ToString("0.0") + " м";
				if (строящаяся_дорога.кривая)
				{
					Radius_Status.Text = "r: " + строящаяся_дорога.Радиус.ToString("0.0") + " м";
					num = (строящаяся_дорога.структура.угол0 + строящаяся_дорога.структура.угол1) * 180.0 / Math.PI;
					Angle_Status.Text = num.ToString("0") + "°";
				}
			}
			else if (строящиеся_провода != null)
			{
				double num = строящиеся_провода.концы[0].x / 1000.0;
				Coord_x1_Status.Text = "x: " + num.ToString("0.000") + " км";
				num = строящиеся_провода.концы[0].y / 1000.0;
				Coord_y1_Status.Text = "y: " + num.ToString("0.000") + " км";
				num = строящиеся_провода.направление * 180.0 / Math.PI;
				Angle1_Status.Text = num.ToString("0") + "°";
				num = строящиеся_провода.концы[1].x / 1000.0;
				Coord_x2_Status.Text = "x: " + num.ToString("0.000") + " км";
				num = строящиеся_провода.концы[1].y / 1000.0;
				Coord_y2_Status.Text = "y: " + num.ToString("0.000") + " км";
				num = (строящиеся_провода.направление + 2.0 * (строящиеся_провода.направления[1] - строящиеся_провода.направление)) * 180.0 / Math.PI;
				Angle2_Status.Text = num.ToString("0") + "°";
				DoublePoint doublePoint = строящиеся_провода.концы[1] - строящиеся_провода.концы[0];
				Length_Status.Text = "l: " + doublePoint.Modulus.ToString("0.0") + " м";
				Height0_Status.Text = "h1: " + строящиеся_провода.высота[0].ToString("0.0") + " м";
				Height1_Status.Text = "h2: " + строящиеся_провода.высота[1].ToString("0.0") + " м";
				Angle_Status.Text = (2.0 * (строящиеся_провода.направления[1] - строящиеся_провода.направление) * 180.0 / Math.PI).ToString("0") + "°";
			}
			else if (Stops_Button.Pushed)
			{
				double num = 11.0;
				if (строящаяся_остановка != null)
				{
					Length_Status.Text = "d: " + строящаяся_остановка.distance.ToString("0.0") + " м";
					num = строящаяся_остановка.distance;
				}
				else if (Stops_Box.SelectedIndex >= 0)
				{
					Length_Status.Text = "d: " + мир.остановки[Stops_Box.SelectedIndex].distance.ToString("0.0") + " м";
					num = мир.остановки[Stops_Box.SelectedIndex].distance;
				}
				if (num < 10.0)
				{
					Radius_Status.Text = "critical d!";
				}
			}
			else if (строящийся_светофор != null)
			{
				Length_Status.Text = "d: " + строящийся_светофор.положение.расстояние.ToString("0.0") + " м";
				Wide0_Status.Text = "offset:";
				Wide1_Status.Text = строящийся_светофор.положение.отклонение.ToString("0.0") + " м";
				Height0_Status.Text = "h: " + строящийся_светофор.положение.высота.ToString("0.0") + " м";
			}
			else if (строящийся_светофорный_сигнал != null)
			{
				Length_Status.Text = "d: " + строящийся_светофорный_сигнал.расстояние.ToString("0.0") + " м";
			}
			else if (строящийся_сигнал_сигнальной_системы != null)
			{
				Length_Status.Text = "d: " + строящийся_сигнал_сигнальной_системы.положение.расстояние.ToString("0.0") + " м";
				Wide0_Status.Text = "offset:";
				Wide1_Status.Text = строящийся_сигнал_сигнальной_системы.положение.отклонение.ToString("0.0") + " м";
				Height0_Status.Text = "h: " + строящийся_сигнал_сигнальной_системы.положение.высота.ToString("0.0") + " м";
			}
			else if (строящийся_элемент_сигнальной_системы != null)
			{
				Length_Status.Text = "d: " + строящийся_элемент_сигнальной_системы.расстояние.ToString("0.0") + " м";
			}
			else if (строящийся_объект != null)
			{
				Radius_Status.Text = "angle:";
				Angle_Status.Text = (строящийся_объект.angle0 * 180.0 / Math.PI).ToString("0") + "°";
				Height0_Status.Text = "h: " + строящийся_объект.height0.ToString("0.0") + " м";
			}
		}

		private void Refresh_All_TripStop_Lists_Item_Click(object sender, EventArgs e)
		{
			мир.остановки.Sort((IComparer<Stop>)null);
			Route[] маршруты = мир.маршруты;
			foreach (Route route in маршруты)
			{
				for (int j = 0; j < route.AllTrips.Count; j++)
				{
					route.AllTrips[j].InitTripStopList();
				}
			}
			modified = true;
		}

		private void Refresh_Timer_Tick(object sender, EventArgs e)
		{
			if (!Refresh_Timer.Enabled)
			{
				return;
			}
			Refresh_Timer.Enabled = false;
			мир.Обновить_время();
			MyDirect3D.device.BeginScene();
			MyDirect3D.ResetViewports(игроки.Length);
			MyDirect3D.SetViewport(-1);
			MyDirect3D.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, 0, 1f, 0);
			if (строящаяся_дорога != null)
			{
				if (Rail_Build_попутки_Button.Pushed)
				{
					Road road = строящаяся_дорога1;
					Road road2 = строящаяся_дорога2;
					if (Rail_Build_попутки2_Button.Pushed)
					{
						road?.Render();
					}
					else if (Rail_Build_попутки3_Button.Pushed)
					{
						road?.Render();
						road2?.Render();
					}
				}
				if (Rail_Build_встречки_Button.Pushed)
				{
					Road road3 = строящаяся_обратная_дорога;
					Road road4 = строящаяся_обратная_дорога1;
					Road road5 = строящаяся_обратная_дорога2;
					if (Rail_Build_встречки1_Button.Pushed)
					{
						road3?.Render();
					}
					else if (Rail_Build_встречки2_Button.Pushed)
					{
						road3?.Render();
						road4?.Render();
					}
					else if (Rail_Build_встречки3_Button.Pushed)
					{
						road3?.Render();
						road4?.Render();
						road5?.Render();
					}
				}
				строящаяся_дорога.Render();
			}
			else if (строящиеся_провода != null)
			{
				int num = ((строящиеся_провода.стадия == Стадия_стоительства.Второй_конец) ? строящиеся_провода.провода.Length : (строящиеся_провода.провода.Length / 2));
				for (int i = 0; i < num; i++)
				{
					строящиеся_провода.провода[i].Render();
				}
			}
			else if (строящаяся_остановка != null)
			{
				строящаяся_остановка.Render();
			}
			else if (строящийся_объект != null)
			{
				строящийся_объект.Render();
			}
			игра.мир.RenderMeshesA();
			игра.Render();
			Refresh_Timer.Enabled = true;
			if (time_color > 0.0)
			{
				time_color -= (double)Refresh_Timer.Interval / 500.0;
			}
			if (time_color <= 0.0 && раскрашены_рельсы)
			{
				time_color = 0.0;
				ОбновитьРаскрашенныеСплайны();
			}
		}

		private void RefreshPanelSize(object sender, EventArgs e)
		{
			MyDirect3D.Window_Width = Sizable_Panel.ClientSize.Width;
			MyDirect3D.Window_Height = Sizable_Panel.ClientSize.Height;
			MyDirect3D.viewport_x = null;
		}

		private void Route_Add_Button_Click(object sender, EventArgs e)
		{
			string number = (мир.маршруты.Length + 1).ToString();
			DoRegisterAction(new AddRouteAction(new Route(0, number)));
			modified = true;
		}

		private void RouteBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Route_Box.SelectedIndex >= 0;
			Route_Remove_Button.Enabled = flag;
			Route_Name_label.Enabled = flag;
			Route_Name_Box.Enabled = flag;
			Route_Name_Box.Text = (flag ? мир.маршруты[Route_Box.SelectedIndex].number : "");
			Route_Name_Box.Modified = false;
			Route_ChangeName_Button.Enabled = false;
			Route_TransportType_label.Enabled = flag;
			Route_TransportType_Box.Enabled = flag;
			Route_TransportType_Box.SelectedIndex = (flag ? мир.маршруты[Route_Box.SelectedIndex].typeOfTransport : (-1));
			Route_Runs_label.Enabled = flag;
			Route_Runs_Box.Enabled = flag;
			Route_Runs_Add_Button.Enabled = flag;
			StopsButton.Enabled = flag;
			Narad_label.Enabled = flag;
			Narad_Box.Enabled = flag;
			Narad_Add_Button.Enabled = flag;
			UpdateRouteControls(flag ? мир.маршруты[Route_Box.SelectedIndex] : null);
			ОбновитьРаскрашенныеСплайны();
		}

		private void RouteChangeNameButtonClick(object sender, EventArgs e)
		{
			int selectedIndex = Route_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.маршруты[selectedIndex].number = Route_Name_Box.Text;
				Route_Box.Items[selectedIndex] = Route_Name_Box.Text;
				Route_Name_Box.Modified = false;
				modified = true;
			}
		}

		private void RouteNameBoxModifiedChanged(object sender, EventArgs e)
		{
			Route_ChangeName_Button.Enabled = Route_Name_Box.Modified;
		}

		private void RouteRemoveButtonClick(object sender, EventArgs e)
		{
			int selectedIndex = Route_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveRouteAction(selectedIndex));
				modified = true;
			}
		}

		private void RouteRunsAddButtonClick(object sender, EventArgs e)
		{
			Trip trip = new Trip
			{
				route = выбранный_маршрут
			};
			DoRegisterAction(new AddRouteTripAction(выбранный_маршрут, trip));
			modified = true;
		}

		private void Route_Runs_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Route_Runs_Box.SelectedIndex >= 0 && Route_Box.SelectedIndex >= 0;
			Route_Runs_Remove_Button.Enabled = flag;
			Route_Runs_Park_Box.Enabled = flag;
			Route_Runs_Park_Box.Checked = flag && Route_Runs_Box.SelectedIndex >= выбранный_маршрут.trips.Count;
			Route_Runs_ToPark_Box.Enabled = flag && Route_Runs_Park_Box.Checked;
			Route_Runs_ToPark_Box.Checked = flag && выбранный_рейс.inPark;
			Route_Runs_ToParkIndex_label.Enabled = flag && Route_Runs_ToPark_Box.Checked;
			Route_Runs_ToParkIndex_UpDown.Enabled = flag && Route_Runs_ToPark_Box.Checked;
			Route_Runs_ToParkIndex_UpDown.Maximum = (flag ? Math.Max(Route_Runs_ToParkIndex_UpDown.Value, выбранный_рейс.pathes.Length) : 0m);
			Route_Runs_ToParkIndex_UpDown.Value = (flag ? выбранный_рейс.inParkIndex : 0);
			Route_Runs_ToParkIndex_UpDown.Maximum = (flag ? выбранный_рейс.pathes.Length : 0);
			Route_Runs_Time_label.Enabled = flag;
			Route_Runs_Time_Box.Enabled = flag;
			Route_Runs_Time_Box.Time_Seconds = (flag ? ((int)выбранный_рейс.время_прибытия) : 0);
			Route_Runs_ComputeTime_Button.Enabled = flag;
			ОбновитьРаскрашенныеСплайны();
		}

		private void Route_Runs_ComputeTime_Button_Click(object sender, EventArgs e)
		{
			мир.time = 0.0;
			if (выбранный_рейс.tripStopList == null)
			{
				выбранный_рейс.InitTripStopList();
			}
			ComputeTimeDialog computeTimeDialog = new ComputeTimeDialog(мир, выбранный_маршрут.typeOfTransport, выбранный_рейс, игрок);
			Refresh_Timer.Enabled = false;
			if (computeTimeDialog.ShowDialog(this) != DialogResult.Cancel)
			{
				выбранный_рейс.время_прибытия = мир.time;
				Route_Runs_Box_SelectedIndexChanged(null, new EventArgs());
				modified = true;
			}
			мир.time = 0.0;
			Refresh_Timer.Enabled = true;
		}

		private void Route_Runs_Park_Box_CheckedChanged(object sender, EventArgs e)
		{
			Trip trip = выбранный_рейс;
			if (trip != null)
			{
				bool @checked = Route_Runs_Park_Box.Checked;
				if (trip.inPark != @checked)
				{
					DoRegisterAction(new ChangeRouteTripTypeAction(выбранный_маршрут, trip));
					modified = true;
				}
			}
		}

		private void Route_Runs_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Route_Runs_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveRouteTripAction(выбранный_маршрут, selectedIndex));
				modified = true;
			}
		}

		private void RouteRunsTimeBoxTimeChanged(object sender, EventArgs e)
		{
			выбранный_рейс.время_прибытия = Route_Runs_Time_Box.Time_Seconds;
			Narad_Runs_Box_SelectedIndexChanged(sender, e);
			modified = true;
		}

		private void RouteRunsToParkBoxCheckedChanged(object sender, EventArgs e)
		{
			if (выбранный_рейс != null)
			{
				выбранный_рейс.inPark = Route_Runs_ToPark_Box.Checked;
				Route_Runs_ToParkIndex_label.Enabled = Route_Runs_ToPark_Box.Enabled && Route_Runs_ToPark_Box.Checked;
				Route_Runs_ToParkIndex_UpDown.Enabled = Route_Runs_ToPark_Box.Enabled && Route_Runs_ToPark_Box.Checked;
				ОбновитьРаскрашенныеСплайны();
				modified = true;
			}
		}

		private void RouteRunsToParkIndexUpDownValueChanged(object sender, EventArgs e)
		{
			выбранный_рейс.inParkIndex = (int)Route_Runs_ToParkIndex_UpDown.Value;
			ОбновитьРаскрашенныеСплайны();
			modified = true;
		}

		private void RouteShowNaradsBoxCheckedChanged(object sender, EventArgs e)
		{
			narad_panel.Visible = Route_ShowNarads_Box.Checked;
		}

		private void RouteTransportTypeBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (Route_TransportType_Box.SelectedIndex >= 0)
			{
				выбранный_маршрут.typeOfTransport = Route_TransportType_Box.SelectedIndex;
				RollingStockUpdate(выбранный_наряд);
				modified = true;
			}
		}

		private void RunItemClick(object sender, EventArgs e)
		{
			if (modified || filename == null)
			{
				MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
				if (filename == null)
				{
					buttons = MessageBoxButtons.OKCancel;
				}
				switch (MessageBox.Show(Localization.current_.save_run, "Transedit", buttons, MessageBoxIcon.Exclamation))
				{
				case DialogResult.Cancel:
					return;
				default:
					Save_Item_Click(this, new EventArgs());
					break;
				case DialogResult.No:
					break;
				}
			}
			base.WindowState = FormWindowState.Minimized;
			Process.Start(Application.ExecutablePath, $"\"{filename}\" -nolog");
		}

		private void Save_Item_Click(object sender, EventArgs e)
		{
			if (filename == null)
			{
				if (saveFileDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				filename = saveFileDialog.FileName;
			}
			try
			{
				мир.Сохранить_город(filename);
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, Localization.current_.save_failed + ":\n" + ex.Message, "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			modified = false;
		}

		private void SaveAs_Item_Click(object sender, EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				filename = saveFileDialog.FileName;
				Save_Item_Click(sender, e);
			}
		}

		private bool Set_New_File(FileDialog dialog)
		{
			if (modified)
			{
				switch (MessageBox.Show(Localization.current_.save_only, "Transedit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
				{
				case DialogResult.Cancel:
					return false;
				case DialogResult.Yes:
					Save_Item_Click(this, new EventArgs());
					break;
				}
			}
			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return false;
			}
			filename = dialog.FileName;
			modified = false;
			return true;
		}

		private void SetPanelControls(Panel new_panel)
		{
			if (old_panel == new_panel)
			{
				return;
			}
			if (old_panel != null)
			{
				while (edit_panel.Controls.Count > 0)
				{
					old_panel.Controls.Add(edit_panel.Controls[0]);
				}
			}
			edit_panel.Controls.Clear();
			if (new_panel != null)
			{
				while (new_panel.Controls.Count > 0)
				{
					edit_panel.Controls.Add(new_panel.Controls[0]);
				}
			}
			old_panel = new_panel;
		}

		private void Signals_Add_Button_Click(object sender, EventArgs e)
		{
			Сигнальная_система сигнальная_система = new Сигнальная_система(1, 0);
			сигнальная_система.CreateMesh();
			DoRegisterAction(new AddSignalSystemAction(сигнальная_система));
			modified = true;
		}

		private void Signals_Bound_UpDown_ValueChanged(object sender, EventArgs e)
		{
			выбранная_сигнальная_система.граница_переключения = (int)Signals_Bound_UpDown.Value;
			modified = true;
		}

		private void Signals_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Signals_Box.SelectedIndex >= 0;
			Signals_Remove_Button.Enabled = flag;
			Signals_Bound_label.Enabled = flag;
			Signals_Bound_UpDown.Enabled = flag;
			Signals_Bound_UpDown.Value = (flag ? выбранная_сигнальная_система.граница_переключения : 0);
			Signals_Element_label.Enabled = flag;
			Signals_Element_Box.Enabled = flag;
			Signals_Element_AddContact_Button.Enabled = flag;
			Signals_Element_AddSignal_Button.Enabled = flag && Signals_Model_Box.Items.Count > 0;
			UpdateSignalsControls(flag ? мир.сигнальныеСистемы[Signals_Box.SelectedIndex] : null);
		}

		private void Signals_Element_AddContact_Button_Click(object sender, EventArgs e)
		{
			if (мир.ВсеДороги.Length != 0)
			{
				строящийся_элемент_сигнальной_системы = new Сигнальная_система.Контакт(выбранная_сигнальная_система, мир.ВсеДороги[0], 0.0, минус: false);
				RegisterPendingAction(new AddSignalSystemContactAction(выбранная_сигнальная_система, строящийся_элемент_сигнальной_системы));
				EnableControls(value: false);
			}
		}

		private void Signals_Element_AddSignal_Button_Click(object sender, EventArgs e)
		{
			if (мир.ВсеДороги.Length != 0)
			{
				строящийся_сигнал_сигнальной_системы = new Visual_Signal(выбранная_сигнальная_система, Signals_Model_Box.Items[Signals_Model_Box.SelectedIndex].ToString());
				строящийся_сигнал_сигнальной_системы.CreateMesh();
				RegisterPendingAction(new AddSignalSystemLightAction(выбранная_сигнальная_система, строящийся_сигнал_сигнальной_системы));
				EnableControls(value: false);
			}
		}

		private void Signals_Element_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Signals_Element_Box.SelectedIndex >= 0 && Signals_Box.SelectedIndex >= 0;
			Signals_Element_Remove_Button.Enabled = flag;
			Signals_Element_Minus_Box.Enabled = flag;
			if (flag && Signals_Element_Box.SelectedIndex >= выбранная_сигнальная_система.vsignals.Count)
			{
				Signals_Element_Minus_Box.Visible = true;
				Signals_Element_Minus_Box.Checked = выбранная_сигнальная_система.элементы[Signals_Element_Box.SelectedIndex - выбранная_сигнальная_система.vsignals.Count].минус;
			}
			else
			{
				Signals_Element_Minus_Box.Visible = false;
			}
			Signals_Element_Location_label.Enabled = flag;
			Signals_Element_ShowLocation_Button.Enabled = flag;
			Signals_Element_EditLocation_Button.Enabled = flag;
		}

		private void Signals_Element_EditLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Signals_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_сигнальная_система.vsignals.Count)
				{
					строящийся_сигнал_сигнальной_системы = выбранная_сигнальная_система.vsignals[selectedIndex];
					RegisterPendingAction(new MoveSignalSystemLightAction(строящийся_сигнал_сигнальной_системы));
				}
				else
				{
					строящийся_элемент_сигнальной_системы = выбранная_сигнальная_система.элементы[selectedIndex - выбранная_сигнальная_система.vsignals.Count];
					RegisterPendingAction(new MoveSignalSystemContactAction(строящийся_элемент_сигнальной_системы));
				}
				EnableControls(value: false);
			}
		}

		private void Signals_Element_Minus_Box_CheckedChanged(object sender, EventArgs e)
		{
			int selectedIndex = Signals_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				выбранная_сигнальная_система.элементы[selectedIndex - выбранная_сигнальная_система.vsignals.Count].минус = Signals_Element_Minus_Box.Checked;
				modified = true;
			}
		}

		private void Signals_Element_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Signals_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_сигнальная_система.vsignals.Count)
				{
					DoRegisterAction(new RemoveSignalSystemLightAction(выбранная_сигнальная_система, выбранная_сигнальная_система.vsignals[selectedIndex], selectedIndex));
				}
				else
				{
					DoRegisterAction(new RemoveSignalSystemContactAction(выбранная_сигнальная_система, выбранная_сигнальная_система.элементы[selectedIndex - выбранная_сигнальная_система.vsignals.Count], selectedIndex - выбранная_сигнальная_система.vsignals.Count));
				}
				modified = true;
			}
		}

		private void Signals_Element_ShowLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Signals_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_сигнальная_система.vsignals.Count)
				{
					игрок.cameraPosition.XZPoint = выбранная_сигнальная_система.vsignals[selectedIndex].road.НайтиКоординаты(выбранная_сигнальная_система.vsignals[selectedIndex].положение.расстояние, 0.0);
				}
				else
				{
					игрок.cameraPosition.XZPoint = выбранная_сигнальная_система.элементы[selectedIndex - выбранная_сигнальная_система.vsignals.Count].дорога.НайтиКоординаты(выбранная_сигнальная_система.элементы[selectedIndex - выбранная_сигнальная_система.vsignals.Count].расстояние, 0.0);
				}
			}
		}

		private void Signals_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Signals_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveSignalSystemAction(selectedIndex));
				modified = true;
			}
		}

		private void Stops_Add_Button_Click(object sender, EventArgs e)
		{
			if (мир.ВсеДороги.Length != 0)
			{
				строящаяся_остановка = new Stop(Stops_Model_Box.Items[Stops_Model_Box.SelectedIndex].ToString(), new TypeOfTransport(0), "Остановка", мир.ВсеДороги[0], 0.0);
				строящаяся_остановка.CreateMesh();
				RegisterPendingAction(new AddStopAction(строящаяся_остановка));
				EnableControls(value: false);
			}
		}

		private void Stops_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			Check_All_Stops_Boxes();
		}

		private void Check_All_Stops_Boxes()
		{
			bool flag = Stops_Box.SelectedIndex >= 0;
			Stops_Remove_Button.Enabled = flag;
			Stops_Name_label.Enabled = flag;
			Stops_Name_Box.Enabled = flag;
			Stops_Name_Box.Text = string.Empty;
			Stops_Name_Box.Modified = false;
			Stops_ChangeName_Button.Enabled = false;
			TypeOfTransportBox.Enabled = flag;
			Stops_Location_label.Enabled = flag;
			Stops_ShowLocation_Button.Enabled = flag;
			Stops_EditLocation_Button.Enabled = flag;
			ОбновитьРаскрашенныеСплайны();
			UpdateStatusBar();
			if (flag)
			{
				Stop stop = мир.остановки[Stops_Box.SelectedIndex];
				Stops_Name_Box.Text = stop.название;
				TramwayBox.Checked = stop.typeOfTransport[0];
				TrolleybusBox.Checked = stop.typeOfTransport[1];
				BusBox.Checked = stop.typeOfTransport[2];
			}
		}

		private void Stops_ChangeName_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.остановки[selectedIndex].название = Stops_Name_Box.Text;
				Stops_Box.Items[selectedIndex] = Stops_Name_Box.Text;
				Stops_Name_Box.Modified = false;
				modified = true;
			}
		}

		private void Stops_EditLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				строящаяся_остановка = мир.остановки[selectedIndex];
				RegisterPendingAction(new MoveStopAction(строящаяся_остановка), doNow: true);
				EnableControls(value: false);
				modified = true;
			}
		}

		private void Stops_Name_Box_ModifiedChanged(object sender, EventArgs e)
		{
			Stops_ChangeName_Button.Enabled = Stops_Name_Box.Modified;
		}

		private void Stops_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveStopAction(selectedIndex));
				modified = true;
			}
		}

		private void Stops_ShowLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				игрок.cameraPosition.XZPoint = мир.остановки[selectedIndex].road.НайтиКоординаты(мир.остановки[selectedIndex].distance, 0.0);
			}
		}

		private void Svetofor_Add_Button_Click(object sender, EventArgs e)
		{
			Светофорная_система s = new Светофорная_система();
			DoRegisterAction(new AddLightSystemAction(s));
			modified = true;
		}

		private void Svetofor_Begin_Box_TimeChanged(object sender, EventArgs e)
		{
			выбранная_светофорная_система.начало_работы = Svetofor_Begin_Box.Time_Seconds;
			modified = true;
		}

		private void Svetofor_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Svetofor_Box.SelectedIndex >= 0;
			Svetofor_Remove_Button.Enabled = flag;
			Svetofor_Work_label.Enabled = flag;
			Svetofor_Begin_Box.Enabled = flag;
			Svetofor_Begin_Box.Time_Seconds = (int)выбранная_светофорная_система.начало_работы;
			Svetofor_End_Box.Enabled = flag;
			Svetofor_End_Box.Time_Seconds = (int)выбранная_светофорная_система.окончание_работы;
			Svetofor_Cycle_label.Enabled = flag;
			Svetofor_Cycle_Box.Enabled = flag;
			Svetofor_Cycle_Box.Time_Seconds = (int)выбранная_светофорная_система.цикл;
			Svetofor_Green_label.Enabled = flag;
			Svetofor_ToGreen_Box.Enabled = flag;
			Svetofor_ToGreen_Box.Time_Seconds = (int)выбранная_светофорная_система.время_переключения_на_зелёный;
			Svetofor_OfGreen_Box.Enabled = flag;
			Svetofor_OfGreen_Box.Time_Seconds = (int)выбранная_светофорная_система.время_зелёного;
			Svetofor_Element_label.Enabled = flag;
			Svetofor_Element_Box.Enabled = flag;
			Svetofor_Svetofor_Add_Button.Enabled = flag && Svetofor_Model_Box.Items.Count > 0;
			Svetofor_Signal_Add_Button.Enabled = flag;
			UpdateSvetoforControls(flag ? мир.светофорныеСистемы[Svetofor_Box.SelectedIndex] : null);
		}

		private void Svetofor_Cycle_Box_TimeChanged(object sender, EventArgs e)
		{
			выбранная_светофорная_система.цикл = Svetofor_Cycle_Box.Time_Seconds;
			modified = true;
		}

		private void Svetofor_Element_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool flag = Svetofor_Element_Box.SelectedIndex >= 0 && Svetofor_Box.SelectedIndex >= 0;
			bool flag2 = flag && Svetofor_Element_Box.SelectedIndex < выбранная_светофорная_система.светофоры.Count;
			Svetofor_Element_Remove_Button.Enabled = flag;
			Svetofor_Svetofor_ArrowGreen_label.Enabled = flag2;
			Svetofor_Svetofor_ArrowGreen_Box.Enabled = flag2;
			Svetofor_Svetofor_ArrowYellow_label.Enabled = flag2;
			Svetofor_Svetofor_ArrowYellow_Box.Enabled = flag2;
			Svetofor_Svetofor_ArrowRed_label.Enabled = flag2;
			Svetofor_Svetofor_ArrowRed_Box.Enabled = flag2;
			Svetofor_Svetofor_ArrowRed_Box.Items.Clear();
			Svetofor_Svetofor_ArrowYellow_Box.Items.Clear();
			Svetofor_Svetofor_ArrowGreen_Box.Items.Clear();
			if (flag2)
			{
				Svetofor_Svetofor_ArrowRed_Box.Items.Add(Localization.current_.empty);
				Svetofor_Svetofor_ArrowYellow_Box.Items.Add(Localization.current_.empty);
				Svetofor_Svetofor_ArrowGreen_Box.Items.Add(Localization.current_.empty);
				for (int i = 0; i < выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].tex_count; i++)
				{
					Svetofor_Svetofor_ArrowRed_Box.Items.Add(выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].model.FindStringArg("tex" + i, string.Empty));
					Svetofor_Svetofor_ArrowYellow_Box.Items.Add(выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].model.FindStringArg("tex" + i, string.Empty));
					Svetofor_Svetofor_ArrowGreen_Box.Items.Add(выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].model.FindStringArg("tex" + i, string.Empty));
				}
			}
			Svetofor_Svetofor_ArrowGreen_Box.SelectedIndex = (flag2 ? выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].зелёная_стрелка : (-1));
			Svetofor_Svetofor_ArrowYellow_Box.SelectedIndex = (flag2 ? выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].жёлтая_стрелка : (-1));
			Svetofor_Svetofor_ArrowRed_Box.SelectedIndex = (flag2 ? выбранная_светофорная_система.светофоры[Svetofor_Element_Box.SelectedIndex].красная_стрелка : (-1));
			Svetofor_Element_Location_label.Enabled = flag;
			Svetofor_Element_ShowLocation_Button.Enabled = flag;
			Svetofor_Element_EditLocation_Button.Enabled = flag;
			ОбновитьРаскрашенныеСплайны();
		}

		private void Svetofor_Element_EditLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_светофорная_система.светофоры.Count)
				{
					строящийся_светофор = выбранная_светофорная_система.светофоры[selectedIndex];
					RegisterPendingAction(new MoveLightSystemLightAction(строящийся_светофор));
				}
				else
				{
					selectedIndex -= выбранная_светофорная_система.светофоры.Count;
					строящийся_светофорный_сигнал = выбранная_светофорная_система.светофорные_сигналы[selectedIndex];
					RegisterPendingAction(new MoveLightSystemSignalAction(строящийся_светофорный_сигнал));
				}
				EnableControls(value: false);
			}
		}

		private void Svetofor_Element_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_светофорная_система.светофоры.Count)
				{
					DoRegisterAction(new RemoveLightSystemLightAction(выбранная_светофорная_система, selectedIndex, selectedIndex));
				}
				else
				{
					DoRegisterAction(new RemoveLightSystemSignalAction(выбранная_светофорная_система, selectedIndex - выбранная_светофорная_система.светофоры.Count, selectedIndex));
				}
				modified = true;
			}
		}

		private void Svetofor_Element_ShowLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				if (selectedIndex < выбранная_светофорная_система.светофоры.Count)
				{
					игрок.cameraPosition.XZPoint = выбранная_светофорная_система.светофоры[selectedIndex].положение.Координаты.XZPoint;
					return;
				}
				selectedIndex -= выбранная_светофорная_система.светофоры.Count;
				игрок.cameraPosition.XZPoint = выбранная_светофорная_система.светофорные_сигналы[selectedIndex].дорога.НайтиКоординаты(выбранная_светофорная_система.светофорные_сигналы[selectedIndex].расстояние, 0.0);
			}
		}

		private void Svetofor_End_Box_TimeChanged(object sender, EventArgs e)
		{
			выбранная_светофорная_система.окончание_работы = Svetofor_End_Box.Time_Seconds;
			modified = true;
		}

		private void Svetofor_OfGreen_Box_TimeChanged(object sender, EventArgs e)
		{
			выбранная_светофорная_система.время_зелёного = Svetofor_OfGreen_Box.Time_Seconds;
			modified = true;
		}

		private void Svetofor_Remove_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				DoRegisterAction(new RemoveLightSystemAction(selectedIndex));
				modified = true;
			}
		}

		private void Svetofor_Signal_Add_Button_Click(object sender, EventArgs e)
		{
			if (мир.ВсеДороги.Length != 0)
			{
				строящийся_светофорный_сигнал = new Светофорный_сигнал(мир.ВсеДороги[0], 0.0);
				RegisterPendingAction(new AddLightSystemSignalAction(выбранная_светофорная_система, строящийся_светофорный_сигнал));
				EnableControls(value: false);
			}
		}

		private void Svetofor_Svetofor_Add_Button_Click(object sender, EventArgs e)
		{
			if (мир.ВсеДороги.Length != 0)
			{
				строящийся_светофор = new Светофор(Svetofor_Model_Box.Items[Svetofor_Model_Box.SelectedIndex].ToString());
				строящийся_светофор.CreateMesh();
				RegisterPendingAction(new AddLightSystemLightAction(выбранная_светофорная_система, строящийся_светофор));
				EnableControls(value: false);
			}
		}

		private void Svetofor_Svetofor_ArrowGreen_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < выбранная_светофорная_система.светофоры.Count && Svetofor_Svetofor_ArrowGreen_Box.SelectedIndex >= 0)
			{
				выбранная_светофорная_система.светофоры[selectedIndex].зелёная_стрелка = Svetofor_Svetofor_ArrowGreen_Box.SelectedIndex;
				modified = true;
			}
		}

		private void Svetofor_Svetofor_ArrowRed_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < выбранная_светофорная_система.светофоры.Count && Svetofor_Svetofor_ArrowRed_Box.SelectedIndex >= 0)
			{
				выбранная_светофорная_система.светофоры[selectedIndex].красная_стрелка = Svetofor_Svetofor_ArrowRed_Box.SelectedIndex;
				modified = true;
			}
		}

		private void Svetofor_Svetofor_ArrowYellow_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = Svetofor_Element_Box.SelectedIndex;
			if (selectedIndex >= 0 && selectedIndex < выбранная_светофорная_система.светофоры.Count && Svetofor_Svetofor_ArrowYellow_Box.SelectedIndex >= 0)
			{
				выбранная_светофорная_система.светофоры[selectedIndex].жёлтая_стрелка = Svetofor_Svetofor_ArrowYellow_Box.SelectedIndex;
				modified = true;
			}
		}

		private void Svetofor_ToGreen_Box_TimeChanged(object sender, EventArgs e)
		{
			выбранная_светофорная_система.время_переключения_на_зелёный = Svetofor_ToGreen_Box.Time_Seconds;
			modified = true;
		}

		private void toolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == New_Button)
			{
				New_Item_Click(sender, e);
			}
			if (e.Button == Open_Button)
			{
				Open_Item_Click(sender, e);
			}
			if (e.Button == Save_Button)
			{
				Save_Item_Click(sender, e);
			}
			if (e.Button == Run_Button)
			{
				RunItemClick(sender, e);
			}
			if (e.Button == Edit_Button)
			{
				edit_panel.Visible = false;
				if (Edit_Button.Pushed)
				{
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Edit_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Button)
			{
				if (Rail_Button.Pushed)
				{
					SetPanelControls(splines_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Rail_Button.Pushed = true;
				}
			}
			if (e.Button == Troll_lines_Button)
			{
				edit_panel.Visible = false;
				if (Troll_lines_Button.Pushed)
				{
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Troll_lines_Button.Pushed = true;
				}
			}
			if (e.Button == Stops_Button)
			{
				if (Stops_Button.Pushed)
				{
					SetPanelControls(stops_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Stops_Button.Pushed = true;
				}
			}
			if (e.Button == Park_Button)
			{
				if (Park_Button.Pushed)
				{
					SetPanelControls(park_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Park_Button.Pushed = true;
				}
			}
			if (e.Button == Route_Button)
			{
				if (Route_Button.Pushed)
				{
					SetPanelControls(route_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Route_Button.Pushed = true;
				}
			}
			if (e.Button == Signals_Button)
			{
				if (Signals_Button.Pushed)
				{
					SetPanelControls(signals_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Signals_Button.Pushed = true;
				}
			}
			if (e.Button == Svetofor_Button)
			{
				if (Svetofor_Button.Pushed)
				{
					SetPanelControls(svetofor_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Object_Button.Pushed = false;
				}
				else
				{
					Svetofor_Button.Pushed = true;
				}
			}
			if (e.Button == Object_Button)
			{
				if (Object_Button.Pushed)
				{
					SetPanelControls(object_panel);
					edit_panel.Visible = true;
					Edit_Button.Pushed = false;
					Rail_Button.Pushed = false;
					Troll_lines_Button.Pushed = false;
					Stops_Button.Pushed = false;
					Park_Button.Pushed = false;
					Route_Button.Pushed = false;
					Signals_Button.Pushed = false;
					Svetofor_Button.Pushed = false;
				}
				else
				{
					Object_Button.Pushed = true;
				}
			}
			if (Rail_Button.Pushed)
			{
				Rail_Edit_Button.Visible = true;
				Rail_Build_Direct_Button.Visible = splines_aviable;
				Rail_Build_Curve_Button.Visible = splines_aviable;
				Rail_Build_попутки_Button.Visible = splines_aviable;
				Rail_Build_встречки_Button.Visible = splines_aviable;
				Road_Button.Visible = splines_aviable;
			}
			else
			{
				Rail_Edit_Button.Visible = false;
				Rail_Build_Direct_Button.Visible = false;
				Rail_Build_Curve_Button.Visible = false;
				Rail_Build_попутки_Button.Visible = false;
				Rail_Build_встречки_Button.Visible = false;
				Road_Button.Visible = false;
				Rail_Edit_Button.Pushed = true;
				Rail_Build_Direct_Button.Pushed = false;
				Rail_Build_Curve_Button.Pushed = false;
				Rail_Build_встречки_Button.Pushed = false;
				Rail_Build_попутки_Button.Pushed = false;
			}
			if (Rail_Build_встречки_Button.Pushed)
			{
				Rail_Build_встречки1_Button.Visible = true;
				Rail_Build_встречки2_Button.Visible = true;
				Rail_Build_встречки3_Button.Visible = true;
			}
			else
			{
				Rail_Build_встречки1_Button.Visible = false;
				Rail_Build_встречки2_Button.Visible = false;
				Rail_Build_встречки3_Button.Visible = false;
				Rail_Build_встречки1_Button.Pushed = true;
				Rail_Build_встречки2_Button.Pushed = false;
				Rail_Build_встречки3_Button.Pushed = false;
			}
			if (Rail_Build_попутки_Button.Pushed)
			{
				Rail_Build_попутки1_Button.Visible = true;
				Rail_Build_попутки2_Button.Visible = true;
				Rail_Build_попутки3_Button.Visible = true;
			}
			else
			{
				Rail_Build_попутки1_Button.Visible = false;
				Rail_Build_попутки2_Button.Visible = false;
				Rail_Build_попутки3_Button.Visible = false;
				Rail_Build_попутки1_Button.Pushed = true;
				Rail_Build_попутки2_Button.Pushed = false;
				Rail_Build_попутки3_Button.Pushed = false;
			}
			if (e.Button == Rail_Build_попутки1_Button)
			{
				if (Rail_Build_попутки1_Button.Pushed)
				{
					Rail_Build_попутки2_Button.Pushed = false;
					Rail_Build_попутки3_Button.Pushed = false;
				}
				else
				{
					Rail_Build_попутки1_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Build_попутки2_Button)
			{
				if (Rail_Build_попутки2_Button.Pushed)
				{
					Rail_Build_попутки1_Button.Pushed = false;
					Rail_Build_попутки3_Button.Pushed = false;
				}
				else
				{
					Rail_Build_попутки2_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Build_попутки3_Button)
			{
				if (Rail_Build_попутки3_Button.Pushed)
				{
					Rail_Build_попутки1_Button.Pushed = false;
					Rail_Build_попутки2_Button.Pushed = false;
				}
				else
				{
					Rail_Build_попутки3_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Build_встречки1_Button)
			{
				if (Rail_Build_встречки1_Button.Pushed)
				{
					Rail_Build_встречки2_Button.Pushed = false;
					Rail_Build_встречки3_Button.Pushed = false;
				}
				else
				{
					Rail_Build_встречки1_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Build_встречки2_Button)
			{
				if (Rail_Build_встречки2_Button.Pushed)
				{
					Rail_Build_встречки1_Button.Pushed = false;
					Rail_Build_встречки3_Button.Pushed = false;
				}
				else
				{
					Rail_Build_встречки2_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Build_встречки3_Button)
			{
				if (Rail_Build_встречки3_Button.Pushed)
				{
					Rail_Build_встречки1_Button.Pushed = false;
					Rail_Build_встречки2_Button.Pushed = false;
				}
				else
				{
					Rail_Build_встречки3_Button.Pushed = true;
				}
			}
			if (e.Button == Rail_Edit_Button)
			{
				if (Rail_Edit_Button.Pushed)
				{
					Rail_Build_Direct_Button.Pushed = false;
					Rail_Build_Curve_Button.Pushed = false;
				}
				else
				{
					Rail_Edit_Button.Pushed = true;
				}
			}
			else if (e.Button == Rail_Build_Direct_Button)
			{
				if (Rail_Build_Direct_Button.Pushed)
				{
					Rail_Edit_Button.Pushed = false;
					Rail_Build_Curve_Button.Pushed = false;
				}
				else
				{
					Rail_Build_Direct_Button.Pushed = true;
				}
			}
			else if (e.Button == Rail_Build_Curve_Button)
			{
				if (Rail_Build_Curve_Button.Pushed)
				{
					Rail_Edit_Button.Pushed = false;
					Rail_Build_Direct_Button.Pushed = false;
				}
				else
				{
					Rail_Build_Curve_Button.Pushed = true;
				}
			}
			if (e.Button == Road_Button)
			{
				Road_Button.Pushed = !Road_Button.Pushed;
			}
			if (Rail_Build_Direct_Button.Pushed || Rail_Build_Curve_Button.Pushed)
			{
				if (строящаяся_дорога == null)
				{
					if (Road_Button.Pushed)
					{
						строящаяся_дорога = new Road(0.0, 0.0, 20.0, 0.0, 0.0, прямая: true, 5.0, 5.0);
						строящаяся_дорога.ОбновитьСледующиеДороги(мир.Дороги);
					}
					else
					{
						строящаяся_дорога = new Рельс(0.0, 0.0, 20.0, 0.0, 0.0, прямой: true);
						строящаяся_дорога.ОбновитьСледующиеДороги(мир.Рельсы);
					}
					строящаяся_дорога.name = Splines_Models_Box.Text;
					строящаяся_дорога.CreateMesh();
					строящаяся_дорога.Color = 255;
					if (строящаяся_дорога is Рельс)
					{
						((Рельс)строящаяся_дорога).добавочные_провода.CreateMesh();
					}
					edit_panel.Enabled = false;
				}
			}
			else if (строящаяся_дорога != null)
			{
				строящаяся_дорога = null;
				edit_panel.Enabled = true;
			}
			if (Troll_lines_Button.Pushed)
			{
				Troll_lines_Edit_Button.Visible = true;
				Troll_lines_Draw_Button.Visible = true;
				Troll_lines_Flag_Button.Visible = true;
			}
			else
			{
				Troll_lines_Edit_Button.Visible = false;
				Troll_lines_Draw_Button.Visible = false;
				Troll_lines_Flag_Button.Visible = false;
				Troll_lines_Edit_Button.Pushed = true;
				Troll_lines_Draw_Button.Pushed = false;
				Troll_lines_Flag_Button.Pushed = false;
			}
			if (e.Button == Troll_lines_Edit_Button)
			{
				if (Troll_lines_Edit_Button.Pushed)
				{
					Troll_lines_Draw_Button.Pushed = false;
				}
				else
				{
					Troll_lines_Edit_Button.Pushed = true;
				}
			}
			else if (e.Button == Troll_lines_Draw_Button)
			{
				if (Troll_lines_Draw_Button.Pushed)
				{
					Troll_lines_Edit_Button.Pushed = false;
				}
				else
				{
					Troll_lines_Draw_Button.Pushed = true;
				}
			}
			if (Troll_lines_Draw_Button.Pushed)
			{
				if (строящиеся_провода == null || e.Button == Troll_lines_Flag_Button)
				{
					if (строящиеся_провода != null)
					{
						ClearPendingAction();
					}
					if (!Troll_lines_Flag_Button.Pushed)
					{
						строящиеся_провода = new Строящиеся_провода();
						RegisterPendingAction(new AddWiresAction(строящиеся_провода.провода[0], строящиеся_провода.провода[1]));
					}
					else
					{
						строящиеся_провода = new Строящиеся_трамвайные_провода();
						RegisterPendingAction(new AddTramWireAction(строящиеся_провода.провода[0] as Трамвайный_контактный_провод));
					}
				}
			}
			else if (строящиеся_провода != null)
			{
				ClearPendingAction();
				строящиеся_провода = null;
			}
			if (Park_Button.Pushed)
			{
				Park_Edit_Button.Visible = true;
				Park_In_Button.Visible = true;
				Park_Out_Button.Visible = true;
				Park_Rails_Button.Visible = true;
			}
			else
			{
				Park_Edit_Button.Visible = false;
				Park_In_Button.Visible = false;
				Park_Out_Button.Visible = false;
				Park_Rails_Button.Visible = false;
				Park_Edit_Button.Pushed = true;
				Park_In_Button.Pushed = false;
				Park_Out_Button.Pushed = false;
				Park_Rails_Button.Pushed = false;
			}
			if (e.Button == Park_Edit_Button)
			{
				if (Park_Edit_Button.Pushed)
				{
					Park_In_Button.Pushed = false;
					Park_Out_Button.Pushed = false;
					Park_Rails_Button.Pushed = false;
				}
				else
				{
					Park_Edit_Button.Pushed = true;
				}
			}
			else if (e.Button == Park_In_Button)
			{
				if (Park_In_Button.Pushed)
				{
					Park_Edit_Button.Pushed = false;
					Park_Out_Button.Pushed = false;
					Park_Rails_Button.Pushed = false;
				}
				else
				{
					Park_In_Button.Pushed = true;
				}
			}
			else if (e.Button == Park_Out_Button)
			{
				if (Park_Out_Button.Pushed)
				{
					Park_Edit_Button.Pushed = false;
					Park_In_Button.Pushed = false;
					Park_Rails_Button.Pushed = false;
				}
				else
				{
					Park_Out_Button.Pushed = true;
				}
			}
			else if (e.Button == Park_Rails_Button)
			{
				if (Park_Rails_Button.Pushed)
				{
					Park_Edit_Button.Pushed = false;
					Park_In_Button.Pushed = false;
					Park_Out_Button.Pushed = false;
				}
				else
				{
					Park_Rails_Button.Pushed = true;
				}
			}
			narad_panel.Visible = Route_Button.Pushed && Route_ShowNarads_Box.Checked;
			ОбновитьРаскрашенныеСплайны();
			ОбновитьРаскрашенныеПровода();
			Check_All_Splines_Boxes();
		}

		private void UpdateNaradControls(Order наряд)
		{
			Narad_Runs_Box.Items.Clear();
			if (наряд != null)
			{
				for (int i = 0; i < наряд.рейсы.Length; i++)
				{
					int num = i + 1;
					Narad_Runs_Box.Items.Add("Рейс " + num);
				}
			}
			Narad_Runs_Box_SelectedIndexChanged(null, new EventArgs());
		}

		private void UpdatePanels()
		{
			UpdateUndoRedoState();
			UpdateParksList();
			UpdateStopsList();
			Route_Box.Items.Clear();
			Route[] маршруты = мир.маршруты;
			foreach (Route route in маршруты)
			{
				Route_Box.Items.Add(route.number);
			}
			RouteBoxSelectedIndexChanged(null, new EventArgs());
			Signals_Box.Items.Clear();
			for (int j = 1; j < мир.сигнальныеСистемы.Length + 1; j++)
			{
				Signals_Box.Items.Add("Система " + j);
			}
			Signals_Box_SelectedIndexChanged(null, new EventArgs());
			Svetofor_Box.Items.Clear();
			for (int k = 1; k < мир.светофорныеСистемы.Length + 1; k++)
			{
				Svetofor_Box.Items.Add("Система " + k);
			}
			Svetofor_Box_SelectedIndexChanged(null, new EventArgs());
			Objects_Box.Items.Clear();
			Stops_Model_Box.Items.Clear();
			Svetofor_Model_Box.Items.Clear();
			Signals_Model_Box.Items.Clear();
			try
			{
				foreach (ObjectModel item in ObjectLoader.objects[0])
				{
					Objects_Box.Items.Add(item.name);
				}
				Objects_Box.SelectedIndex = ((Objects_Box.Items.Count <= 0) ? (-1) : 0);
				foreach (ObjectModel item2 in ObjectLoader.objects[1])
				{
					Stops_Model_Box.Items.Add(item2.name);
				}
				Stops_Model_Box.SelectedIndex = ((Stops_Model_Box.Items.Count <= 0) ? (-1) : 0);
				Stops_Add_Button.Enabled = Stops_Model_Box.Items.Count > 0;
				foreach (ObjectModel item3 in ObjectLoader.objects[2])
				{
					Svetofor_Model_Box.Items.Add(item3.name);
				}
				Svetofor_Model_Box.SelectedIndex = ((Svetofor_Model_Box.Items.Count <= 0) ? (-1) : 0);
				foreach (ObjectModel item4 in ObjectLoader.objects[3])
				{
					Signals_Model_Box.Items.Add(item4.name);
				}
				Signals_Model_Box.SelectedIndex = ((Signals_Model_Box.Items.Count <= 0) ? (-1) : 0);
			}
			catch (DirectoryNotFoundException ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Objects_Box.Enabled = false;
				Objects_Instance_Box.Enabled = false;
				Objects_Add_Button.Enabled = false;
				Objects_Remove_Button.Enabled = false;
				Objects_ShowLocation_Button.Enabled = false;
				Objects_EditLocation_Button.Enabled = false;
			}
			UpdateObjectsList();
			Splines_Models_Box.Items.Clear();
			foreach (SplineModel spline in SplineLoader.splines)
			{
				Splines_Models_Box.Items.Add(spline.name);
			}
			splines_aviable = Splines_Models_Box.Items.Count > 0;
			Splines_Models_Box.Enabled = splines_aviable;
			Splines_Models_Box.SelectedIndex = ((!splines_aviable) ? (-1) : 0);
			UpdateSplinesList();
		}

		private void UpdateObjectsList()
		{
			Objects_Instance_Box.Items.Clear();
			for (int i = 0; i < мир.объекты.Count; i++)
			{
				if (мир.объекты[i] != null)
				{
					Objects_Instance_Box.Items.Add("Obj " + (i + 1) + ", " + ((мир.объекты[i].model != null) ? мир.объекты[i].model.name : "???"));
				}
			}
			if (Objects_Instance_Box.Items.Count > 0)
			{
				Objects_Instance_Box.SelectedIndex = 0;
			}
			Check_All_Objects_Boxes();
		}

		private void UpdateSplinesList()
		{
			Splines_Instance_Box.Items.Clear();
			Рельс[] рельсы = мир.Рельсы;
			for (int i = 1; i < рельсы.Length + 1; i++)
			{
				Splines_Instance_Box.Items.Add("Rail " + i + ", " + рельсы[i - 1].name);
			}
			Road[] дороги = мир.Дороги;
			for (int j = 1; j < дороги.Length + 1; j++)
			{
				Splines_Instance_Box.Items.Add("Road " + j + ", " + дороги[j - 1].name);
			}
			if (Splines_Instance_Box.Items.Count > 0)
			{
				Splines_Instance_Box.SelectedIndex = 0;
			}
			Check_All_Splines_Boxes();
		}

		private void UpdateStopsList()
		{
			Stops_Box.Items.Clear();
			foreach (Stop item in мир.остановки)
			{
				Stops_Box.Items.Add(item.название);
			}
			if (Stops_Box.Items.Count > 0)
			{
				Stops_Box.SelectedIndex = 0;
			}
			else
			{
				Check_All_Stops_Boxes();
			}
		}

		private void UpdateRouteControls(Route маршрут)
		{
			Route_Runs_Box.Items.Clear();
			if (маршрут != null)
			{
				for (int i = 0; i < маршрут.trips.Count; i++)
				{
					int num = i + 1;
					if (Route_Runs_Box != null)
					{
						Route_Runs_Box.Items.Add("Рейс " + num);
					}
				}
				for (int j = 0; j < маршрут.parkTrips.Count; j++)
				{
					int num2 = j + 1;
					if (Route_Runs_Box != null)
					{
						Route_Runs_Box.Items.Add("Парковый рейс " + num2);
					}
				}
				if (Route_Runs_Box.Items.Count > 0)
				{
					Route_Runs_Box.SelectedIndex = 0;
				}
			}
			Route_Runs_Box_SelectedIndexChanged(null, new EventArgs());
			Narad_Box.Items.Clear();
			if (маршрут != null)
			{
				for (int k = 0; k < маршрут.orders.Length; k++)
				{
					Narad_Box.Items.Add(маршрут.orders[k].номер);
				}
			}
			Narad_Box_SelectedIndexChanged(null, new EventArgs());
		}

		private void UpdateSignalsControls(Сигнальная_система система)
		{
			Signals_Element_Box.Items.Clear();
			if (система != null)
			{
				for (int i = 0; i < система.vsignals.Count; i++)
				{
					Signals_Element_Box.Items.Add("Сигнал " + (i + 1));
				}
				for (int j = 0; j < система.элементы.Count; j++)
				{
					Signals_Element_Box.Items.Add("Контакт " + (j + 1));
				}
				if (система.элементы.Count > 0)
				{
					Signals_Element_Box.SelectedIndex = система.vsignals.Count;
				}
			}
			Signals_Element_Box_SelectedIndexChanged(null, new EventArgs());
		}

		private void UpdateSvetoforControls(Светофорная_система система)
		{
			Svetofor_Element_Box.Items.Clear();
			if (система != null)
			{
				for (int i = 0; i < система.светофоры.Count; i++)
				{
					int num = i + 1;
					Svetofor_Element_Box.Items.Add("Светофор " + num);
				}
				for (int j = 0; j < система.светофорные_сигналы.Count; j++)
				{
					int num2 = j + 1;
					Svetofor_Element_Box.Items.Add("Сигнал " + num2);
				}
				if (система.светофорные_сигналы.Count > 0)
				{
					Svetofor_Element_Box.SelectedIndex = система.светофоры.Count;
				}
			}
			Svetofor_Element_Box_SelectedIndexChanged(null, new EventArgs());
		}

		private void UpdateParksList()
		{
			Park_Box.Items.Clear();
			Парк[] парки = мир.парки;
			foreach (Парк парк in парки)
			{
				Park_Box.Items.Add(парк.название);
			}
			if (мир.парки.Length != 0)
			{
				Park_Box.SelectedIndex = 0;
			}
			else
			{
				Check_All_Park_Boxes();
			}
			Narad_Park_Box.Items.Clear();
			парки = мир.парки;
			foreach (Парк парк2 in парки)
			{
				Narad_Park_Box.Items.Add(парк2.название);
			}
		}

		private void ОбновитьРаскрашенныеПровода()
		{
			Контактный_провод[] контактныеПровода;
			Трамвайный_контактный_провод[] контактныеПровода2;
			if (раскрашены_провода)
			{
				контактныеПровода = мир.контактныеПровода;
				for (int i = 0; i < контактныеПровода.Length; i++)
				{
					контактныеПровода[i].color = 0;
				}
				контактныеПровода2 = мир.контактныеПровода2;
				for (int i = 0; i < контактныеПровода2.Length; i++)
				{
					контактныеПровода2[i].color = 0;
				}
				раскрашены_провода = false;
			}
			if (!Troll_lines_Button.Pushed)
			{
				return;
			}
			контактныеПровода = мир.контактныеПровода;
			foreach (Контактный_провод контактный_провод in контактныеПровода)
			{
				if (контактный_провод.обесточенный)
				{
					контактный_провод.color = 16711680;
				}
			}
			контактныеПровода2 = мир.контактныеПровода2;
			foreach (Трамвайный_контактный_провод трамвайный_контактный_провод in контактныеПровода2)
			{
				if (трамвайный_контактный_провод.обесточенный)
				{
					трамвайный_контактный_провод.color = 16711680;
				}
			}
			раскрашены_провода = true;
		}

		private void ОбновитьРаскрашенныеСплайны()
		{
			if (раскрашены_рельсы)
			{
				Road[] всеДороги = мир.ВсеДороги;
				for (int i = 0; i < всеДороги.Length; i++)
				{
					всеДороги[i].Color = 0;
				}
				раскрашены_рельсы = false;
			}
			try
			{
				if (Rail_Button.Pushed && Splines_Instance_Box.SelectedIndex >= 0 && time_color > 0.0)
				{
					if (Splines_Instance_Box.SelectedIndex < мир.Рельсы.Length)
					{
						мир.Рельсы[Splines_Instance_Box.SelectedIndex].Color = 255;
					}
					else
					{
						мир.Дороги[Splines_Instance_Box.SelectedIndex - мир.Рельсы.Length].Color = 255;
					}
					раскрашены_рельсы = true;
				}
				if (Park_Button.Pushed && Park_Box.SelectedIndex >= 0)
				{
					Парк парк = мир.парки[Park_Box.SelectedIndex];
					if (парк.въезд != null)
					{
						парк.въезд.Color = 65280;
					}
					if (парк.выезд != null)
					{
						парк.выезд.Color = 16711680;
					}
					Road[] всеДороги = парк.пути_стоянки;
					for (int i = 0; i < всеДороги.Length; i++)
					{
						всеДороги[i].Color = 255;
					}
					раскрашены_рельсы = true;
				}
				if (Stops_Button.Pushed && Stops_Box.SelectedIndex >= 0)
				{
					Road[] всеДороги = мир.остановки[Stops_Box.SelectedIndex].частьПути;
					for (int i = 0; i < всеДороги.Length; i++)
					{
						всеДороги[i].Color = 255;
					}
					раскрашены_рельсы = true;
				}
				if (Route_Button.Pushed && Route_Box.SelectedIndex >= 0 && Route_Runs_Box.SelectedIndex >= 0)
				{
					Trip trip = мир.маршруты[Route_Box.SelectedIndex].AllTrips[Route_Runs_Box.SelectedIndex];
					for (int j = 0; j < trip.pathes.Length; j++)
					{
						if (j == 0)
						{
							trip.pathes[j].Color = 16711680;
						}
						else if (j == trip.pathes.Length - 1)
						{
							trip.pathes[j].Color = 65280;
						}
						else if (trip.inPark && j >= trip.inParkIndex)
						{
							trip.pathes[j].Color = 16776960;
						}
						else
						{
							trip.pathes[j].Color = 255;
						}
					}
					раскрашены_рельсы = true;
				}
				if (Svetofor_Button.Pushed && Svetofor_Box.SelectedIndex >= 0 && Svetofor_Element_Box.SelectedIndex >= 0 && Svetofor_Element_Box.SelectedIndex >= выбранная_светофорная_система.светофоры.Count)
				{
					int index = Svetofor_Element_Box.SelectedIndex - выбранная_светофорная_система.светофоры.Count;
					if (выбранная_светофорная_система.светофорные_сигналы[index].дорога != null)
					{
						выбранная_светофорная_система.светофорные_сигналы[index].дорога.Color = 255;
					}
					раскрашены_рельсы = true;
				}
			}
			catch
			{
			}
		}

		private void Objects_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			Check_All_Objects_Boxes();
		}

		private void Check_All_Objects_Boxes()
		{
			Objects_Instance_Box.Enabled = Objects_Instance_Box.Items.Count != 0;
			Objects_Remove_Button.Enabled = Objects_Instance_Box.SelectedIndex >= 0;
			Objects_Location_label.Enabled = Objects_Instance_Box.SelectedIndex >= 0;
			Objects_ShowLocation_Button.Enabled = Objects_Instance_Box.SelectedIndex >= 0;
			Objects_EditLocation_Button.Enabled = Objects_Instance_Box.SelectedIndex >= 0;
		}

		private void Objects_Add_Button_Click(object sender, EventArgs e)
		{
			строящийся_объект = new Объект(Objects_Box.Text, cursor_pos.x, cursor_pos.y, 0.0, 0.0);
			строящийся_объект.CreateMesh();
			RegisterPendingAction(new AddObjectAction(строящийся_объект));
			EnableControls(value: false);
		}

		private void Object_Instance_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			Check_All_Objects_Boxes();
		}

		private void ObjectsRemoveButtonClick(object sender, EventArgs e)
		{
			int selectedIndex = Objects_Instance_Box.SelectedIndex;
			DoRegisterAction(new RemoveObjectAction(selectedIndex));
			if (selectedIndex > 0)
			{
				Objects_Instance_Box.SelectedIndex = selectedIndex - 1;
			}
			modified = true;
		}

		private void RollingStockUpdate(Order наряд)
		{
			RollingStockBox.Items.Clear();
			if (Narad_Box.Items.Count <= 0 || Narad_Box.SelectedIndex < 0)
			{
				return;
			}
			RollingStockBox.Items.Add("Случайный");
			switch (наряд.маршрут.typeOfTransport)
			{
			case 0:
				foreach (МодельТранспорта item in Модели.Трамваи)
				{
					RollingStockBox.Items.Add(item.name);
				}
				break;
			case 1:
				foreach (МодельТранспорта item2 in Модели.Троллейбусы)
				{
					RollingStockBox.Items.Add(item2.name);
				}
				break;
			case 2:
				foreach (МодельТранспорта item3 in Модели.Автобусы)
				{
					RollingStockBox.Items.Add(item3.name);
				}
				break;
			}
			RollingStockBox.Text = (string)((наряд.transport != "" || наряд.transport != "Случайный") ? наряд.transport : RollingStockBox.Items[0]);
			if (!RollingStockBox.Items.Contains(RollingStockBox.Text))
			{
				RollingStockBox.Text = (string)RollingStockBox.Items[0];
			}
		}

		private void RollingStockBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			выбранный_наряд.transport = RollingStockBox.Text;
		}

		private void Objects_EditLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Objects_Instance_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				строящийся_объект = мир.объекты[selectedIndex];
				RegisterPendingAction(new MoveObjectAction(строящийся_объект), doNow: true);
				EnableControls(value: false);
			}
		}

		private void Objects_ShowLocation_Button_Click(object sender, EventArgs e)
		{
			int selectedIndex = Objects_Instance_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				игрок.cameraPosition.XZPoint = мир.объекты[selectedIndex].position;
			}
		}

		private void Route_Runs_Time_Box_Validated(object sender, EventArgs e)
		{
			выбранный_рейс.время_прибытия = Route_Runs_Time_Box.Time_Seconds;
		}

		private void TramwayBox_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.остановки[selectedIndex].typeOfTransport[0] = TramwayBox.Checked;
				TrolleybusBox.Checked = мир.остановки[selectedIndex].typeOfTransport[1];
				BusBox.Checked = мир.остановки[selectedIndex].typeOfTransport[2];
			}
		}

		private void TrolleybusBox_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.остановки[selectedIndex].typeOfTransport[1] = TrolleybusBox.Checked;
				TramwayBox.Checked = мир.остановки[selectedIndex].typeOfTransport[0];
				BusBox.Checked = мир.остановки[selectedIndex].typeOfTransport[2];
			}
		}

		private void BusBox_Click(object sender, EventArgs e)
		{
			int selectedIndex = Stops_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				мир.остановки[selectedIndex].typeOfTransport[2] = BusBox.Checked;
				TramwayBox.Checked = мир.остановки[selectedIndex].typeOfTransport[0];
				TrolleybusBox.Checked = мир.остановки[selectedIndex].typeOfTransport[1];
			}
		}

		private void StopsButton_Click(object sender, EventArgs e)
		{
			using StopListForm stopListForm = new StopListForm(выбранный_рейс);
			DialogResult dialogResult = stopListForm.ShowDialog();
			if (dialogResult == DialogResult.OK && stopListForm.Changed)
			{
				DoRegisterAction(new SetTripStopListAction(выбранный_рейс, stopListForm.TripStopList));
				modified = true;
			}
		}

		private void Splines_Remove_ButtonClick(object sender, EventArgs e)
		{
			if (Splines_Instance_Box.SelectedIndex >= 0)
			{
				int selectedIndex = Splines_Instance_Box.SelectedIndex;
				DoRegisterAction(new RemoveRoadAction(selectedIndex));
				if (selectedIndex > 0)
				{
					Splines_Instance_Box.SelectedIndex = selectedIndex - 1;
				}
				modified = true;
			}
		}

		private void Check_All_Splines_Boxes()
		{
			Splines_Models_Box.Enabled = !Rail_Build_Curve_Button.Pushed && !Rail_Build_Direct_Button.Pushed && splines_aviable;
			Splines_Instance_Box.Enabled = !Rail_Build_Curve_Button.Pushed && !Rail_Build_Direct_Button.Pushed && Splines_Instance_Box.Items.Count != 0;
			Splines_Remove_Button.Enabled = !Rail_Build_Curve_Button.Pushed && !Rail_Build_Direct_Button.Pushed && Splines_Instance_Box.SelectedIndex >= 0;
			Splines_ChangeModel_Button.Enabled = Splines_Remove_Button.Enabled;
			Splines_ShowLocation_Button.Enabled = Splines_Remove_Button.Enabled;
			Splines_Location_label.Enabled = Splines_Remove_Button.Enabled;
			Splines_Instance_BoxSelectedIndexChanged(null, null);
		}

		private void Splines_Models_BoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (Splines_Models_Box.SelectedIndex < 0)
			{
				return;
			}
			foreach (SplineModel spline in SplineLoader.splines)
			{
				if (!(spline.name != Splines_Models_Box.Text))
				{
					Road_Button.Pushed = spline.type == 1;
					break;
				}
			}
		}

		private void Splines_ChangeModel_ButtonClick(object sender, EventArgs e)
		{
			foreach (SplineModel spline in SplineLoader.splines)
			{
				if (!(spline.name != Splines_Models_Box.Text))
				{
					if (spline.type == мир.ВсеДороги[Splines_Instance_Box.SelectedIndex].model.type)
					{
						Road obj = ((Splines_Instance_Box.SelectedIndex < мир.Рельсы.Length) ? мир.Рельсы[Splines_Instance_Box.SelectedIndex] : мир.Дороги[Splines_Instance_Box.SelectedIndex - мир.Рельсы.Length]);
						obj.name = spline.name;
						obj.model = null;
						obj.CreateMesh();
						modified = true;
					}
					break;
				}
			}
		}

		private void Splines_ShowLocation_ButtonClick(object sender, EventArgs e)
		{
			int selectedIndex = Splines_Instance_Box.SelectedIndex;
			if (selectedIndex >= 0)
			{
				Road obj = ((selectedIndex < мир.Рельсы.Length) ? мир.Рельсы[selectedIndex] : мир.Дороги[selectedIndex - мир.Рельсы.Length]);
				DoublePoint doublePoint = obj.НайтиКоординаты(obj.Длина / 2.0, 0.0);
				игрок.cameraPosition.XZPoint = new DoublePoint(doublePoint.x, doublePoint.y);
				time_color = 2.0;
				ОбновитьРаскрашенныеСплайны();
			}
		}

		private void Rails_NumericBoxEnterPressed(object sender, EventArgs e)
		{
			((Рельс)мир.ВсеДороги[Splines_Instance_Box.SelectedIndex]).расстояние_добавочных_проводов = Rail_Box_NumericBox.Value;
		}

		private void Splines_Instance_BoxSelectedIndexChanged(object sender, EventArgs e)
		{
			Rail_Box_NumericBox.Text = string.Empty;
			int selectedIndex = Splines_Instance_Box.SelectedIndex;
			Rail_Box_NumericBox.Enabled = Splines_Instance_Box.Enabled && selectedIndex >= 0 && selectedIndex < мир.Рельсы.Length && мир.Рельсы[selectedIndex].следующие_рельсы.Length > 1;
			if (Rail_Box_NumericBox.Enabled)
			{
				Rail_Box_NumericBox.Value = ((Рельс)мир.ВсеДороги[selectedIndex]).расстояние_добавочных_проводов;
			}
		}

		private void EditorDeactivate(object sender, EventArgs e)
		{
			Refresh_Timer.Enabled = false;
		}

		private void EditorActivated(object sender, EventArgs e)
		{
			if (!Refresh_Timer.Enabled)
			{
				Refresh_Timer.Enabled = true;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Trancity.Editor));
			this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
			this.City_Item = new System.Windows.Forms.MenuItem();
			this.New_Item = new System.Windows.Forms.MenuItem();
			this.Open_Item = new System.Windows.Forms.MenuItem();
			this.Save_Item = new System.Windows.Forms.MenuItem();
			this.SaveAs_Item = new System.Windows.Forms.MenuItem();
			this.SeparatorItem1 = new System.Windows.Forms.MenuItem();
			this.Refresh_All_TripStop_Lists_Item = new System.Windows.Forms.MenuItem();
			this.Check_Joints_Item = new System.Windows.Forms.MenuItem();
			this.Find_MinRadius_Item = new System.Windows.Forms.MenuItem();
			this.ComputeAllTime_Item = new System.Windows.Forms.MenuItem();
			this.Run_Item = new System.Windows.Forms.MenuItem();
			this.SeparatorItem2 = new System.Windows.Forms.MenuItem();
			this.Exit_Item = new System.Windows.Forms.MenuItem();
			this.Edit_Item = new System.Windows.Forms.MenuItem();
			this.Undo_Item = new System.Windows.Forms.MenuItem();
			this.Redo_Item = new System.Windows.Forms.MenuItem();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.Cursor_x_Status = new System.Windows.Forms.StatusBarPanel();
			this.Cursor_y_Status = new System.Windows.Forms.StatusBarPanel();
			this.SeparatorPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.Coord_x1_Status = new System.Windows.Forms.StatusBarPanel();
			this.Coord_y1_Status = new System.Windows.Forms.StatusBarPanel();
			this.Angle1_Status = new System.Windows.Forms.StatusBarPanel();
			this.SeparatorPanel2 = new System.Windows.Forms.StatusBarPanel();
			this.Coord_x2_Status = new System.Windows.Forms.StatusBarPanel();
			this.Coord_y2_Status = new System.Windows.Forms.StatusBarPanel();
			this.Angle2_Status = new System.Windows.Forms.StatusBarPanel();
			this.SeparatorPanel3 = new System.Windows.Forms.StatusBarPanel();
			this.Length_Status = new System.Windows.Forms.StatusBarPanel();
			this.Radius_Status = new System.Windows.Forms.StatusBarPanel();
			this.Angle_Status = new System.Windows.Forms.StatusBarPanel();
			this.Wide0_Status = new System.Windows.Forms.StatusBarPanel();
			this.Wide1_Status = new System.Windows.Forms.StatusBarPanel();
			this.Height0_Status = new System.Windows.Forms.StatusBarPanel();
			this.Height1_Status = new System.Windows.Forms.StatusBarPanel();
			this.SeparatorPanel4 = new System.Windows.Forms.StatusBarPanel();
			this.Maschtab = new System.Windows.Forms.StatusBarPanel();
			this.SeparatorPanel5 = new System.Windows.Forms.StatusBarPanel();
			this.Ugol = new System.Windows.Forms.StatusBarPanel();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.New_Button = new System.Windows.Forms.ToolBarButton();
			this.Open_Button = new System.Windows.Forms.ToolBarButton();
			this.Save_Button = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton1 = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton2 = new System.Windows.Forms.ToolBarButton();
			this.Run_Button = new System.Windows.Forms.ToolBarButton();
			this.Play_Button = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton3 = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton4 = new System.Windows.Forms.ToolBarButton();
			this.Edit_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Button = new System.Windows.Forms.ToolBarButton();
			this.Troll_lines_Button = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton5 = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton6 = new System.Windows.Forms.ToolBarButton();
			this.Stops_Button = new System.Windows.Forms.ToolBarButton();
			this.Park_Button = new System.Windows.Forms.ToolBarButton();
			this.Route_Button = new System.Windows.Forms.ToolBarButton();
			this.Signals_Button = new System.Windows.Forms.ToolBarButton();
			this.Svetofor_Button = new System.Windows.Forms.ToolBarButton();
			this.Object_Button = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.SeparatorButton8 = new System.Windows.Forms.ToolBarButton();
			this.Rail_Edit_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_Direct_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_Curve_Button = new System.Windows.Forms.ToolBarButton();
			this.Road_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_попутки_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_попутки1_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_попутки2_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_попутки3_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_встречки_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_встречки1_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_встречки2_Button = new System.Windows.Forms.ToolBarButton();
			this.Rail_Build_встречки3_Button = new System.Windows.Forms.ToolBarButton();
			this.Park_Edit_Button = new System.Windows.Forms.ToolBarButton();
			this.Park_In_Button = new System.Windows.Forms.ToolBarButton();
			this.Park_Out_Button = new System.Windows.Forms.ToolBarButton();
			this.Park_Rails_Button = new System.Windows.Forms.ToolBarButton();
			this.Troll_lines_Edit_Button = new System.Windows.Forms.ToolBarButton();
			this.Troll_lines_Draw_Button = new System.Windows.Forms.ToolBarButton();
			this.Troll_lines_Flag_Button = new System.Windows.Forms.ToolBarButton();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.renderPanel = new Engine.Controls.RenderPanel();
			this.object_panel = new System.Windows.Forms.Panel();
			this.Objects_Instance_Box = new System.Windows.Forms.ComboBox();
			this.Objects_Instance_label = new System.Windows.Forms.Label();
			this.Objects_EditLocation_Button = new System.Windows.Forms.Button();
			this.Objects_ShowLocation_Button = new System.Windows.Forms.Button();
			this.Objects_Location_label = new System.Windows.Forms.Label();
			this.Objects_Remove_Button = new System.Windows.Forms.Button();
			this.Objects_Add_Button = new System.Windows.Forms.Button();
			this.Objects_Box = new System.Windows.Forms.ComboBox();
			this.Objects_label = new System.Windows.Forms.Label();
			this.route_panel = new System.Windows.Forms.Panel();
			this.StopsButton = new System.Windows.Forms.Button();
			this.Route_TransportType_Box = new System.Windows.Forms.ComboBox();
			this.Route_TransportType_label = new System.Windows.Forms.Label();
			this.Route_ShowNarads_Box = new System.Windows.Forms.CheckBox();
			this.Route_Runs_ComputeTime_Button = new System.Windows.Forms.Button();
			this.Route_Runs_Time_Box = new Trancity.TimeBox();
			this.Route_Runs_ToParkIndex_UpDown = new System.Windows.Forms.NumericUpDown();
			this.Route_Runs_ToPark_Box = new System.Windows.Forms.CheckBox();
			this.Route_Runs_Park_Box = new System.Windows.Forms.CheckBox();
			this.Route_Runs_Time_label = new System.Windows.Forms.Label();
			this.Route_Runs_ToParkIndex_label = new System.Windows.Forms.Label();
			this.Route_Runs_label = new System.Windows.Forms.Label();
			this.Route_Name_label = new System.Windows.Forms.Label();
			this.Route_Name_Box = new System.Windows.Forms.TextBox();
			this.Route_Runs_Remove_Button = new System.Windows.Forms.Button();
			this.Route_Remove_Button = new System.Windows.Forms.Button();
			this.Route_Runs_Add_Button = new System.Windows.Forms.Button();
			this.Route_Add_Button = new System.Windows.Forms.Button();
			this.Route_ChangeName_Button = new System.Windows.Forms.Button();
			this.Route_label = new System.Windows.Forms.Label();
			this.Route_Runs_Box = new System.Windows.Forms.ComboBox();
			this.Route_Box = new System.Windows.Forms.ComboBox();
			this.svetofor_panel = new System.Windows.Forms.Panel();
			this.Svetofor_Model_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Model_label = new System.Windows.Forms.Label();
			this.Svetofor_Svetofor_ArrowRed_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Svetofor_ArrowYellow_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Svetofor_ArrowGreen_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Cycle_Box = new Trancity.TimeBox();
			this.Svetofor_OfGreen_Box = new Trancity.TimeBox();
			this.Svetofor_End_Box = new Trancity.TimeBox();
			this.Svetofor_ToGreen_Box = new Trancity.TimeBox();
			this.Svetofor_Begin_Box = new Trancity.TimeBox();
			this.Svetofor_Element_Location_label = new System.Windows.Forms.Label();
			this.Svetofor_Cycle_label = new System.Windows.Forms.Label();
			this.Svetofor_Green_label = new System.Windows.Forms.Label();
			this.Svetofor_Work_label = new System.Windows.Forms.Label();
			this.Svetofor_Remove_Button = new System.Windows.Forms.Button();
			this.Svetofor_Svetofor_ArrowRed_label = new System.Windows.Forms.Label();
			this.Svetofor_Svetofor_ArrowYellow_label = new System.Windows.Forms.Label();
			this.Svetofor_Svetofor_ArrowGreen_label = new System.Windows.Forms.Label();
			this.Svetofor_Element_label = new System.Windows.Forms.Label();
			this.Svetofor_Add_Button = new System.Windows.Forms.Button();
			this.Svetofor_Element_EditLocation_Button = new System.Windows.Forms.Button();
			this.Svetofor_Element_ShowLocation_Button = new System.Windows.Forms.Button();
			this.Svetofor_label = new System.Windows.Forms.Label();
			this.Svetofor_Element_Remove_Button = new System.Windows.Forms.Button();
			this.Svetofor_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Element_Box = new System.Windows.Forms.ComboBox();
			this.Svetofor_Signal_Add_Button = new System.Windows.Forms.Button();
			this.Svetofor_Svetofor_Add_Button = new System.Windows.Forms.Button();
			this.splines_panel = new System.Windows.Forms.Panel();
			this.Rail_Box_NumericBox = new Common.NumericBox();
			this.Spline_Select_mode_Box = new System.Windows.Forms.CheckBox();
			this.Splines_Instance_Box = new System.Windows.Forms.ComboBox();
			this.Splines_Instance_label = new System.Windows.Forms.Label();
			this.Splines_ShowLocation_Button = new System.Windows.Forms.Button();
			this.Rail_Box_dist_Label = new System.Windows.Forms.Label();
			this.Splines_Location_label = new System.Windows.Forms.Label();
			this.Splines_ChangeModel_Button = new System.Windows.Forms.Button();
			this.Splines_Remove_Button = new System.Windows.Forms.Button();
			this.Splines_Models_Box = new System.Windows.Forms.ComboBox();
			this.Splines_label = new System.Windows.Forms.Label();
			this.signals_panel = new System.Windows.Forms.Panel();
			this.Signals_Model_Box = new System.Windows.Forms.ComboBox();
			this.Signals_Model_label = new System.Windows.Forms.Label();
			this.Signals_Element_Minus_Box = new System.Windows.Forms.CheckBox();
			this.Signals_Bound_UpDown = new System.Windows.Forms.NumericUpDown();
			this.Signals_Element_Location_label = new System.Windows.Forms.Label();
			this.Signals_Bound_label = new System.Windows.Forms.Label();
			this.Signals_Remove_Button = new System.Windows.Forms.Button();
			this.Signals_Element_label = new System.Windows.Forms.Label();
			this.Signals_Add_Button = new System.Windows.Forms.Button();
			this.Signals_Element_EditLocation_Button = new System.Windows.Forms.Button();
			this.Signals_Element_ShowLocation_Button = new System.Windows.Forms.Button();
			this.Signals_label = new System.Windows.Forms.Label();
			this.Signals_Element_Remove_Button = new System.Windows.Forms.Button();
			this.Signals_Box = new System.Windows.Forms.ComboBox();
			this.Signals_Element_Box = new System.Windows.Forms.ComboBox();
			this.Signals_Element_AddSignal_Button = new System.Windows.Forms.Button();
			this.Signals_Element_AddContact_Button = new System.Windows.Forms.Button();
			this.stops_panel = new System.Windows.Forms.Panel();
			this.Stops_Model_Box = new System.Windows.Forms.ComboBox();
			this.Stops_Model_label = new System.Windows.Forms.Label();
			this.TypeOfTransportBox = new System.Windows.Forms.GroupBox();
			this.BusBox = new System.Windows.Forms.CheckBox();
			this.TrolleybusBox = new System.Windows.Forms.CheckBox();
			this.TramwayBox = new System.Windows.Forms.CheckBox();
			this.Stops_Location_label = new System.Windows.Forms.Label();
			this.Stops_Name_label = new System.Windows.Forms.Label();
			this.Stops_Name_Box = new System.Windows.Forms.TextBox();
			this.Stops_Remove_Button = new System.Windows.Forms.Button();
			this.Stops_Add_Button = new System.Windows.Forms.Button();
			this.Stops_EditLocation_Button = new System.Windows.Forms.Button();
			this.Stops_ShowLocation_Button = new System.Windows.Forms.Button();
			this.Stops_ChangeName_Button = new System.Windows.Forms.Button();
			this.Stops_label = new System.Windows.Forms.Label();
			this.Stops_Box = new System.Windows.Forms.ComboBox();
			this.park_panel = new System.Windows.Forms.Panel();
			this.Park_Name_label = new System.Windows.Forms.Label();
			this.Park_Name_Box = new System.Windows.Forms.TextBox();
			this.Park_Remove_Button = new System.Windows.Forms.Button();
			this.Park_ChangeName_Button = new System.Windows.Forms.Button();
			this.Park_Add_Button = new System.Windows.Forms.Button();
			this.Park_label = new System.Windows.Forms.Label();
			this.Park_Box = new System.Windows.Forms.ComboBox();
			this.Refresh_Timer = new System.Windows.Forms.Timer(this.components);
			this.Sizable_Panel = new System.Windows.Forms.Panel();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.edit_panel = new System.Windows.Forms.Panel();
			this.narad_panel = new System.Windows.Forms.Panel();
			this.RollingStockBox = new System.Windows.Forms.ComboBox();
			this.Transport_label = new System.Windows.Forms.Label();
			this.Narad_Runs_Time2_Box = new Trancity.TimeBox();
			this.Narad_Runs_Time1_Box = new Trancity.TimeBox();
			this.Narad_Runs_Time2_label = new System.Windows.Forms.Label();
			this.Narad_Runs_Time1_label = new System.Windows.Forms.Label();
			this.Narad_Runs_label = new System.Windows.Forms.Label();
			this.Narad_Park_label = new System.Windows.Forms.Label();
			this.Narad_Runs_Run_label = new System.Windows.Forms.Label();
			this.Narad_Name_label = new System.Windows.Forms.Label();
			this.Narad_Name_Box = new System.Windows.Forms.TextBox();
			this.Narad_Runs_Remove_Button = new System.Windows.Forms.Button();
			this.Narad_Remove_Button = new System.Windows.Forms.Button();
			this.Narad_Runs_Add_Button = new System.Windows.Forms.Button();
			this.Narad_Add_Button = new System.Windows.Forms.Button();
			this.Narad_ChangeName_Button = new System.Windows.Forms.Button();
			this.Narad_label = new System.Windows.Forms.Label();
			this.Narad_Park_Box = new System.Windows.Forms.ComboBox();
			this.Narad_Runs_Run_Box = new System.Windows.Forms.ComboBox();
			this.Narad_Runs_Box = new System.Windows.Forms.ComboBox();
			this.Narad_Box = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)this.Cursor_x_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Cursor_y_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel1).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_x1_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_y1_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Angle1_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel2).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_x2_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_y2_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Angle2_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel3).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Length_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Radius_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Angle_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Wide0_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Wide1_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Height0_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Height1_Status).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel4).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Maschtab).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel5).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Ugol).BeginInit();
			this.object_panel.SuspendLayout();
			this.route_panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Route_Runs_ToParkIndex_UpDown).BeginInit();
			this.svetofor_panel.SuspendLayout();
			this.splines_panel.SuspendLayout();
			this.signals_panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.Signals_Bound_UpDown).BeginInit();
			this.stops_panel.SuspendLayout();
			this.TypeOfTransportBox.SuspendLayout();
			this.park_panel.SuspendLayout();
			this.Sizable_Panel.SuspendLayout();
			this.narad_panel.SuspendLayout();
			base.SuspendLayout();
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[2] { this.City_Item, this.Edit_Item });
			this.City_Item.Index = 0;
			this.City_Item.MenuItems.AddRange(new System.Windows.Forms.MenuItem[12]
			{
				this.New_Item, this.Open_Item, this.Save_Item, this.SaveAs_Item, this.SeparatorItem1, this.Refresh_All_TripStop_Lists_Item, this.Check_Joints_Item, this.Find_MinRadius_Item, this.ComputeAllTime_Item, this.Run_Item,
				this.SeparatorItem2, this.Exit_Item
			});
			this.City_Item.Text = "Город";
			this.City_Item.Name = "City_Item";
			this.New_Item.Index = 0;
			this.New_Item.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.New_Item.Text = "Новый";
			this.New_Item.Click += new System.EventHandler(New_Item_Click);
			this.New_Item.Name = "New_Item";
			this.Open_Item.Index = 1;
			this.Open_Item.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.Open_Item.Text = "Открыть...";
			this.Open_Item.Click += new System.EventHandler(Open_Item_Click);
			this.Open_Item.Name = "Open_Item";
			this.Save_Item.Index = 2;
			this.Save_Item.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.Save_Item.Text = "Сохранить";
			this.Save_Item.Click += new System.EventHandler(Save_Item_Click);
			this.Save_Item.Name = "Save_Item";
			this.SaveAs_Item.Index = 3;
			this.SaveAs_Item.Text = "Сохранить как...";
			this.SaveAs_Item.Click += new System.EventHandler(SaveAs_Item_Click);
			this.SaveAs_Item.Name = "SaveAs_Item";
			this.SeparatorItem1.Index = 4;
			this.SeparatorItem1.Text = "-";
			this.Refresh_All_TripStop_Lists_Item.Index = 5;
			this.Refresh_All_TripStop_Lists_Item.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.Refresh_All_TripStop_Lists_Item.Text = "Обновить все остановки рейсов...";
			this.Refresh_All_TripStop_Lists_Item.Click += new System.EventHandler(Refresh_All_TripStop_Lists_Item_Click);
			this.Refresh_All_TripStop_Lists_Item.Name = "Refresh_All_TripStop_Lists_Item";
			this.Check_Joints_Item.Index = 6;
			this.Check_Joints_Item.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.Check_Joints_Item.Text = "Проверить стыки...";
			this.Check_Joints_Item.Click += new System.EventHandler(Check_стыки_Item_Click);
			this.Check_Joints_Item.Name = "Check_Joints_Item";
			this.Find_MinRadius_Item.Index = 7;
			this.Find_MinRadius_Item.Text = "Найти минимальный радиус кривой...";
			this.Find_MinRadius_Item.Click += new System.EventHandler(Find_MinRadius_Item_Click);
			this.Find_MinRadius_Item.Name = "Find_MinRadius_Item";
			this.ComputeAllTime_Item.Index = 8;
			this.ComputeAllTime_Item.Text = "Посчитать время всех рейсов...";
			this.ComputeAllTime_Item.Click += new System.EventHandler(ComputeAllTime_Item_Click);
			this.ComputeAllTime_Item.Name = "ComputeAllTime_Item";
			this.Run_Item.Index = 9;
			this.Run_Item.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.Run_Item.Text = "Запустить";
			this.Run_Item.Click += new System.EventHandler(RunItemClick);
			this.Run_Item.Name = "Run_Item";
			this.SeparatorItem2.Index = 10;
			this.SeparatorItem2.Text = "-";
			this.Exit_Item.Index = 11;
			this.Exit_Item.Shortcut = System.Windows.Forms.Shortcut.AltF4;
			this.Exit_Item.Text = "Выход";
			this.Exit_Item.Click += new System.EventHandler(Exit_Item_Click);
			this.Exit_Item.Name = "Exit_Item";
			this.Edit_Item.Index = 1;
			this.Edit_Item.MenuItems.AddRange(new System.Windows.Forms.MenuItem[2] { this.Undo_Item, this.Redo_Item });
			this.Edit_Item.Text = "Правка";
			this.Edit_Item.Name = "Edit_Item";
			this.Undo_Item.Index = 0;
			this.Undo_Item.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.Undo_Item.Text = "Отменить";
			this.Undo_Item.Click += new System.EventHandler(Undo_Item_Click);
			this.Undo_Item.Name = "Undo_Item";
			this.Redo_Item.Index = 1;
			this.Redo_Item.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftZ;
			this.Redo_Item.Text = "Повторить";
			this.Redo_Item.Click += new System.EventHandler(Redo_Item_Click);
			this.Redo_Item.Name = "Redo_Item";
			this.statusBar.Location = new System.Drawing.Point(0, 1040);
			this.statusBar.Name = "statusBar";
			this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[22]
			{
				this.Cursor_x_Status, this.Cursor_y_Status, this.SeparatorPanel1, this.Coord_x1_Status, this.Coord_y1_Status, this.Angle1_Status, this.SeparatorPanel2, this.Coord_x2_Status, this.Coord_y2_Status, this.Angle2_Status,
				this.SeparatorPanel3, this.Length_Status, this.Radius_Status, this.Angle_Status, this.Wide0_Status, this.Wide1_Status, this.Height0_Status, this.Height1_Status, this.SeparatorPanel4, this.Maschtab,
				this.SeparatorPanel5, this.Ugol
			});
			this.statusBar.ShowPanels = true;
			this.statusBar.Size = new System.Drawing.Size(692, 22);
			this.statusBar.TabIndex = 0;
			this.Cursor_x_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Cursor_x_Status.Name = "Cursor_x_Status";
			this.Cursor_x_Status.Width = 92;
			this.Cursor_y_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Cursor_y_Status.Name = "Cursor_y_Status";
			this.Cursor_y_Status.Width = 92;
			this.SeparatorPanel1.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.SeparatorPanel1.Name = "SeparatorPanel1";
			this.SeparatorPanel1.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.SeparatorPanel1.Width = 24;
			this.Coord_x1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Coord_x1_Status.Name = "Coord_x1_Status";
			this.Coord_x1_Status.Width = 92;
			this.Coord_y1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Coord_y1_Status.Name = "Coord_y1_Status";
			this.Coord_y1_Status.Width = 92;
			this.Angle1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Angle1_Status.Name = "Angle1_Status";
			this.Angle1_Status.Width = 42;
			this.SeparatorPanel2.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.SeparatorPanel2.Name = "SeparatorPanel2";
			this.SeparatorPanel2.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.SeparatorPanel2.Width = 24;
			this.Coord_x2_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Coord_x2_Status.Name = "Coord_x2_Status";
			this.Coord_x2_Status.Width = 92;
			this.Coord_y2_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Coord_y2_Status.Name = "Coord_y2_Status";
			this.Coord_y2_Status.Width = 92;
			this.Angle2_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Angle2_Status.Name = "Angle2_Status";
			this.Angle2_Status.Width = 42;
			this.SeparatorPanel3.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.SeparatorPanel3.Name = "SeparatorPanel3";
			this.SeparatorPanel3.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.SeparatorPanel3.Width = 24;
			this.Length_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Length_Status.Name = "Length_Status";
			this.Length_Status.Width = 80;
			this.Radius_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Radius_Status.Name = "Radius_Status";
			this.Radius_Status.Width = 80;
			this.Angle_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Angle_Status.Name = "Angle_Status";
			this.Angle_Status.Width = 42;
			this.Wide0_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Wide0_Status.Name = "Wide0_Status";
			this.Wide0_Status.Width = 80;
			this.Wide1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Wide1_Status.Name = "Wide1_Status";
			this.Wide1_Status.Width = 80;
			this.Height0_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Height0_Status.Name = "Height0_Status";
			this.Height0_Status.Width = 80;
			this.Height1_Status.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Height1_Status.Name = "Height1_Status";
			this.Height1_Status.Width = 80;
			this.SeparatorPanel4.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.SeparatorPanel4.Name = "SeparatorPanel4";
			this.SeparatorPanel4.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.SeparatorPanel4.Width = 25;
			this.Maschtab.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Maschtab.Name = "Maschtab";
			this.Maschtab.Text = "Выбранный масштаб: 10.0";
			this.Maschtab.Width = 170;
			this.SeparatorPanel5.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
			this.SeparatorPanel5.Name = "SeparatorPanel5";
			this.SeparatorPanel5.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
			this.SeparatorPanel5.Width = 25;
			this.Ugol.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.Ugol.Name = "Ugol";
			this.Ugol.Text = "Угол: 0°";
			this.Ugol.Width = 120;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[41]
			{
				this.New_Button, this.Open_Button, this.Save_Button, this.SeparatorButton1, this.SeparatorButton2, this.Run_Button, this.Play_Button, this.SeparatorButton3, this.SeparatorButton4, this.Edit_Button,
				this.Rail_Button, this.Troll_lines_Button, this.SeparatorButton5, this.SeparatorButton6, this.Stops_Button, this.Park_Button, this.Route_Button, this.Signals_Button, this.Svetofor_Button, this.Object_Button,
				this.toolBarButton3, this.SeparatorButton8, this.Rail_Edit_Button, this.Rail_Build_Direct_Button, this.Rail_Build_Curve_Button, this.Road_Button, this.Rail_Build_попутки_Button, this.Rail_Build_попутки1_Button, this.Rail_Build_попутки2_Button, this.Rail_Build_попутки3_Button,
				this.Rail_Build_встречки_Button, this.Rail_Build_встречки1_Button, this.Rail_Build_встречки2_Button, this.Rail_Build_встречки3_Button, this.Park_Edit_Button, this.Park_In_Button, this.Park_Out_Button, this.Park_Rails_Button, this.Troll_lines_Edit_Button, this.Troll_lines_Draw_Button,
				this.Troll_lines_Flag_Button
			});
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(1008, 28);
			this.toolBar.TabIndex = 1;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(toolBar_ButtonClick);
			this.New_Button.ImageIndex = 2;
			this.New_Button.Name = "New_Button";
			this.New_Button.ToolTipText = "Новый город";
			this.Open_Button.ImageIndex = 0;
			this.Open_Button.Name = "Open_Button";
			this.Open_Button.ToolTipText = "Открыть город";
			this.Save_Button.ImageIndex = 1;
			this.Save_Button.Name = "Save_Button";
			this.Save_Button.ToolTipText = "Сохранить город";
			this.SeparatorButton1.Name = "SeparatorButton1";
			this.SeparatorButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.SeparatorButton2.Name = "SeparatorButton2";
			this.SeparatorButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.Run_Button.ImageIndex = 3;
			this.Run_Button.Name = "Run_Button";
			this.Run_Button.ToolTipText = "Запустить игру в городе";
			this.Play_Button.Enabled = false;
			this.Play_Button.ImageIndex = 3;
			this.Play_Button.Name = "Play_Button";
			this.Play_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Play_Button.ToolTipText = "Play";
			this.Play_Button.Visible = false;
			this.SeparatorButton3.Name = "SeparatorButton3";
			this.SeparatorButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.SeparatorButton4.Name = "SeparatorButton4";
			this.SeparatorButton4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.Edit_Button.ImageIndex = 4;
			this.Edit_Button.Name = "Edit_Button";
			this.Edit_Button.Pushed = true;
			this.Edit_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Button.ImageIndex = 5;
			this.Rail_Button.Name = "Rail_Button";
			this.Rail_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Button.ToolTipText = "Рельсы и дороги";
			this.Troll_lines_Button.ImageIndex = 6;
			this.Troll_lines_Button.Name = "Troll_lines_Button";
			this.Troll_lines_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Troll_lines_Button.ToolTipText = "Контактные провода троллейбуса";
			this.SeparatorButton5.Name = "SeparatorButton5";
			this.SeparatorButton5.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.SeparatorButton6.Name = "SeparatorButton6";
			this.SeparatorButton6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.Stops_Button.ImageIndex = 7;
			this.Stops_Button.Name = "Stops_Button";
			this.Stops_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Stops_Button.ToolTipText = "Остановки";
			this.Park_Button.ImageIndex = 8;
			this.Park_Button.Name = "Park_Button";
			this.Park_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_Button.ToolTipText = "Парки";
			this.Route_Button.ImageIndex = 9;
			this.Route_Button.Name = "Route_Button";
			this.Route_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Route_Button.ToolTipText = "Маршруты";
			this.Signals_Button.ImageIndex = 10;
			this.Signals_Button.Name = "Signals_Button";
			this.Signals_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Signals_Button.ToolTipText = "Сигнальные системы";
			this.Svetofor_Button.ImageIndex = 11;
			this.Svetofor_Button.Name = "Svetofor_Button";
			this.Svetofor_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Svetofor_Button.ToolTipText = "Светофоры";
			this.Object_Button.ImageIndex = 12;
			this.Object_Button.Name = "Object_Button";
			this.Object_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Object_Button.ToolTipText = "Объекты";
			this.toolBarButton3.Name = "toolBarButton3";
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.SeparatorButton8.Name = "SeparatorButton8";
			this.SeparatorButton8.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.Rail_Edit_Button.ImageIndex = 4;
			this.Rail_Edit_Button.Name = "Rail_Edit_Button";
			this.Rail_Edit_Button.Pushed = true;
			this.Rail_Edit_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Edit_Button.ToolTipText = "Редактировать";
			this.Rail_Build_Direct_Button.ImageIndex = 13;
			this.Rail_Build_Direct_Button.Name = "Rail_Build_Direct_Button";
			this.Rail_Build_Direct_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_Direct_Button.ToolTipText = "Строить прямые участки";
			this.Rail_Build_Curve_Button.ImageIndex = 14;
			this.Rail_Build_Curve_Button.Name = "Rail_Build_Curve_Button";
			this.Rail_Build_Curve_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_Curve_Button.ToolTipText = "Строить поворотные участки";
			this.Road_Button.ImageIndex = 16;
			this.Road_Button.Name = "Road_Button";
			this.Road_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Road_Button.ToolTipText = "Дороги";
			this.Rail_Build_попутки_Button.ImageIndex = 20;
			this.Rail_Build_попутки_Button.Name = "Rail_Build_попутки_Button";
			this.Rail_Build_попутки_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_попутки1_Button.ImageIndex = 22;
			this.Rail_Build_попутки1_Button.Name = "Rail_Build_попутки1_Button";
			this.Rail_Build_попутки1_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_попутки2_Button.ImageIndex = 23;
			this.Rail_Build_попутки2_Button.Name = "Rail_Build_попутки2_Button";
			this.Rail_Build_попутки2_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_попутки3_Button.ImageIndex = 24;
			this.Rail_Build_попутки3_Button.Name = "Rail_Build_попутки3_Button";
			this.Rail_Build_попутки3_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_встречки_Button.ImageIndex = 21;
			this.Rail_Build_встречки_Button.Name = "Rail_Build_встречки_Button";
			this.Rail_Build_встречки_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_встречки1_Button.ImageIndex = 22;
			this.Rail_Build_встречки1_Button.Name = "Rail_Build_встречки1_Button";
			this.Rail_Build_встречки1_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_встречки2_Button.ImageIndex = 23;
			this.Rail_Build_встречки2_Button.Name = "Rail_Build_встречки2_Button";
			this.Rail_Build_встречки2_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Rail_Build_встречки3_Button.ImageIndex = 24;
			this.Rail_Build_встречки3_Button.Name = "Rail_Build_встречки3_Button";
			this.Rail_Build_встречки3_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_Edit_Button.ImageIndex = 4;
			this.Park_Edit_Button.Name = "Park_Edit_Button";
			this.Park_Edit_Button.Pushed = true;
			this.Park_Edit_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_In_Button.ImageIndex = 17;
			this.Park_In_Button.Name = "Park_In_Button";
			this.Park_In_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_In_Button.ToolTipText = "Въезд";
			this.Park_Out_Button.ImageIndex = 18;
			this.Park_Out_Button.Name = "Park_Out_Button";
			this.Park_Out_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_Out_Button.ToolTipText = "Выезд";
			this.Park_Rails_Button.ImageIndex = 19;
			this.Park_Rails_Button.Name = "Park_Rails_Button";
			this.Park_Rails_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Park_Rails_Button.ToolTipText = "Пути стоянки";
			this.Troll_lines_Edit_Button.ImageIndex = 4;
			this.Troll_lines_Edit_Button.Name = "Troll_lines_Edit_Button";
			this.Troll_lines_Edit_Button.Pushed = true;
			this.Troll_lines_Edit_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Troll_lines_Draw_Button.ImageIndex = 6;
			this.Troll_lines_Draw_Button.Name = "Troll_lines_Draw_Button";
			this.Troll_lines_Draw_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Troll_lines_Flag_Button.ImageIndex = 22;
			this.Troll_lines_Flag_Button.Name = "Troll_lines_Flag_Button";
			this.Troll_lines_Flag_Button.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.Troll_lines_Flag_Button.ToolTipText = "Трамвайная КС";
			this.imageList.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList.ImageStream");
			this.imageList.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList.Images.SetKeyName(0, "Open_Button.PNG");
			this.imageList.Images.SetKeyName(1, "Save_Button.PNG");
			this.imageList.Images.SetKeyName(2, "New_Button.PNG");
			this.imageList.Images.SetKeyName(3, "Run_Button.PNG");
			this.imageList.Images.SetKeyName(4, "Arrow_Button.PNG");
			this.imageList.Images.SetKeyName(5, "Rail_Button.PNG");
			this.imageList.Images.SetKeyName(6, "Contact_wire_Button.PNG");
			this.imageList.Images.SetKeyName(7, "Stop_Button.PNG");
			this.imageList.Images.SetKeyName(8, "Park_Button.PNG");
			this.imageList.Images.SetKeyName(9, "Route_Button.PNG");
			this.imageList.Images.SetKeyName(10, "Signal_Button.PNG");
			this.imageList.Images.SetKeyName(11, "Svetofor_Button.PNG");
			this.imageList.Images.SetKeyName(12, "Object_Button.PNG");
			this.imageList.Images.SetKeyName(13, "Direct_Button.PNG");
			this.imageList.Images.SetKeyName(14, "Curve_Button.PNG");
			this.imageList.Images.SetKeyName(15, "Both_Button.PNG");
			this.imageList.Images.SetKeyName(16, "Road_Button.PNG");
			this.imageList.Images.SetKeyName(17, "In_Button.PNG");
			this.imageList.Images.SetKeyName(18, "Out_Button.PNG");
			this.imageList.Images.SetKeyName(19, "Path_Button.PNG");
			this.imageList.Images.SetKeyName(20, "Duoble1.PNG");
			this.imageList.Images.SetKeyName(21, "Duoble0.PNG");
			this.imageList.Images.SetKeyName(22, "1.PNG");
			this.imageList.Images.SetKeyName(23, "2.PNG");
			this.imageList.Images.SetKeyName(24, "3.PNG");
			this.renderPanel.Location = new System.Drawing.Point(0, 0);
			this.renderPanel.Name = "renderPanel";
			this.renderPanel.Size = new System.Drawing.Size(1600, 1200);
			this.renderPanel.TabIndex = 2;
			this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(panel_MouseDown);
			this.renderPanel.MouseLeave += new System.EventHandler(panel_MouseLeave);
			this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(panel_MouseMove);
			this.renderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(panel_MouseUp);
			this.object_panel.Controls.Add(this.Objects_Instance_Box);
			this.object_panel.Controls.Add(this.Objects_Instance_label);
			this.object_panel.Controls.Add(this.Objects_EditLocation_Button);
			this.object_panel.Controls.Add(this.Objects_ShowLocation_Button);
			this.object_panel.Controls.Add(this.Objects_Location_label);
			this.object_panel.Controls.Add(this.Objects_Remove_Button);
			this.object_panel.Controls.Add(this.Objects_Add_Button);
			this.object_panel.Controls.Add(this.Objects_Box);
			this.object_panel.Controls.Add(this.Objects_label);
			this.object_panel.Location = new System.Drawing.Point(3, 637);
			this.object_panel.Name = "object_panel";
			this.object_panel.Size = new System.Drawing.Size(158, 291);
			this.object_panel.TabIndex = 6;
			this.object_panel.Visible = false;
			this.Objects_Instance_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Objects_Instance_Box.FormattingEnabled = true;
			this.Objects_Instance_Box.Location = new System.Drawing.Point(11, 79);
			this.Objects_Instance_Box.Name = "Objects_Instance_Box";
			this.Objects_Instance_Box.Size = new System.Drawing.Size(136, 21);
			this.Objects_Instance_Box.TabIndex = 8;
			this.Objects_Instance_Box.SelectedIndexChanged += new System.EventHandler(Object_Instance_Box_SelectedIndexChanged);
			this.Objects_Instance_label.AutoSize = true;
			this.Objects_Instance_label.Location = new System.Drawing.Point(11, 63);
			this.Objects_Instance_label.Name = "Objects_Instance_label";
			this.Objects_Instance_label.Size = new System.Drawing.Size(75, 13);
			this.Objects_Instance_label.TabIndex = 7;
			this.Objects_Instance_label.Text = "Экземпляры:";
			this.Objects_EditLocation_Button.Location = new System.Drawing.Point(11, 238);
			this.Objects_EditLocation_Button.Name = "Objects_EditLocation_Button";
			this.Objects_EditLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Objects_EditLocation_Button.TabIndex = 6;
			this.Objects_EditLocation_Button.Text = "Изменить";
			this.Objects_EditLocation_Button.UseVisualStyleBackColor = true;
			this.Objects_EditLocation_Button.Click += new System.EventHandler(Objects_EditLocation_Button_Click);
			this.Objects_ShowLocation_Button.Location = new System.Drawing.Point(11, 209);
			this.Objects_ShowLocation_Button.Name = "Objects_ShowLocation_Button";
			this.Objects_ShowLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Objects_ShowLocation_Button.TabIndex = 5;
			this.Objects_ShowLocation_Button.Text = "Показать";
			this.Objects_ShowLocation_Button.UseVisualStyleBackColor = true;
			this.Objects_ShowLocation_Button.Click += new System.EventHandler(Objects_ShowLocation_Button_Click);
			this.Objects_Location_label.AutoSize = true;
			this.Objects_Location_label.Location = new System.Drawing.Point(11, 193);
			this.Objects_Location_label.Name = "Objects_Location_label";
			this.Objects_Location_label.Size = new System.Drawing.Size(98, 13);
			this.Objects_Location_label.TabIndex = 4;
			this.Objects_Location_label.Text = "Местоположение:";
			this.Objects_Remove_Button.Location = new System.Drawing.Point(11, 135);
			this.Objects_Remove_Button.Name = "Objects_Remove_Button";
			this.Objects_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Objects_Remove_Button.TabIndex = 3;
			this.Objects_Remove_Button.Text = "Удалить объект";
			this.Objects_Remove_Button.UseVisualStyleBackColor = true;
			this.Objects_Remove_Button.Click += new System.EventHandler(ObjectsRemoveButtonClick);
			this.Objects_Add_Button.Location = new System.Drawing.Point(11, 106);
			this.Objects_Add_Button.Name = "Objects_Add_Button";
			this.Objects_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Objects_Add_Button.TabIndex = 2;
			this.Objects_Add_Button.Text = "Добавить объект";
			this.Objects_Add_Button.UseVisualStyleBackColor = true;
			this.Objects_Add_Button.Click += new System.EventHandler(Objects_Add_Button_Click);
			this.Objects_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Objects_Box.FormattingEnabled = true;
			this.Objects_Box.Location = new System.Drawing.Point(11, 36);
			this.Objects_Box.Name = "Objects_Box";
			this.Objects_Box.Size = new System.Drawing.Size(136, 21);
			this.Objects_Box.TabIndex = 1;
			this.Objects_Box.SelectedIndexChanged += new System.EventHandler(Objects_Box_SelectedIndexChanged);
			this.Objects_label.AutoSize = true;
			this.Objects_label.Location = new System.Drawing.Point(11, 20);
			this.Objects_label.Name = "Objects_label";
			this.Objects_label.Size = new System.Drawing.Size(56, 13);
			this.Objects_label.TabIndex = 0;
			this.Objects_label.Text = "Объекты:";
			this.route_panel.Controls.Add(this.StopsButton);
			this.route_panel.Controls.Add(this.Route_TransportType_Box);
			this.route_panel.Controls.Add(this.Route_TransportType_label);
			this.route_panel.Controls.Add(this.Route_ShowNarads_Box);
			this.route_panel.Controls.Add(this.Route_Runs_ComputeTime_Button);
			this.route_panel.Controls.Add(this.Route_Runs_Time_Box);
			this.route_panel.Controls.Add(this.Route_Runs_ToParkIndex_UpDown);
			this.route_panel.Controls.Add(this.Route_Runs_ToPark_Box);
			this.route_panel.Controls.Add(this.Route_Runs_Park_Box);
			this.route_panel.Controls.Add(this.Route_Runs_Time_label);
			this.route_panel.Controls.Add(this.Route_Runs_ToParkIndex_label);
			this.route_panel.Controls.Add(this.Route_Runs_label);
			this.route_panel.Controls.Add(this.Route_Name_label);
			this.route_panel.Controls.Add(this.Route_Name_Box);
			this.route_panel.Controls.Add(this.Route_Runs_Remove_Button);
			this.route_panel.Controls.Add(this.Route_Remove_Button);
			this.route_panel.Controls.Add(this.Route_Runs_Add_Button);
			this.route_panel.Controls.Add(this.Route_Add_Button);
			this.route_panel.Controls.Add(this.Route_ChangeName_Button);
			this.route_panel.Controls.Add(this.Route_label);
			this.route_panel.Controls.Add(this.Route_Runs_Box);
			this.route_panel.Controls.Add(this.Route_Box);
			this.route_panel.Location = new System.Drawing.Point(528, 11);
			this.route_panel.Name = "route_panel";
			this.route_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.route_panel.Size = new System.Drawing.Size(158, 641);
			this.route_panel.TabIndex = 5;
			this.route_panel.Visible = false;
			this.StopsButton.Location = new System.Drawing.Point(11, 549);
			this.StopsButton.Name = "StopsButton";
			this.StopsButton.Size = new System.Drawing.Size(136, 23);
			this.StopsButton.TabIndex = 14;
			this.StopsButton.Text = "Остановки...";
			this.StopsButton.UseVisualStyleBackColor = true;
			this.StopsButton.Click += new System.EventHandler(StopsButton_Click);
			this.Route_TransportType_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Route_TransportType_Box.FormattingEnabled = true;
			this.Route_TransportType_Box.Items.AddRange(new object[3] { "Трамвай", "Троллейбус", "Автобус" });
			this.Route_TransportType_Box.Location = new System.Drawing.Point(11, 233);
			this.Route_TransportType_Box.Name = "Route_TransportType_Box";
			this.Route_TransportType_Box.Size = new System.Drawing.Size(136, 21);
			this.Route_TransportType_Box.TabIndex = 11;
			this.Route_TransportType_Box.SelectedIndexChanged += new System.EventHandler(RouteTransportTypeBoxSelectedIndexChanged);
			this.Route_TransportType_label.AutoSize = true;
			this.Route_TransportType_label.Location = new System.Drawing.Point(11, 217);
			this.Route_TransportType_label.Name = "Route_TransportType_label";
			this.Route_TransportType_label.Size = new System.Drawing.Size(90, 13);
			this.Route_TransportType_label.TabIndex = 10;
			this.Route_TransportType_label.Text = "Вид транспорта:";
			this.Route_ShowNarads_Box.AutoSize = true;
			this.Route_ShowNarads_Box.Location = new System.Drawing.Point(11, 578);
			this.Route_ShowNarads_Box.Name = "Route_ShowNarads_Box";
			this.Route_ShowNarads_Box.Size = new System.Drawing.Size(116, 17);
			this.Route_ShowNarads_Box.TabIndex = 9;
			this.Route_ShowNarads_Box.Text = "Показать наряды";
			this.Route_ShowNarads_Box.UseVisualStyleBackColor = true;
			this.Route_ShowNarads_Box.CheckedChanged += new System.EventHandler(RouteShowNaradsBoxCheckedChanged);
			this.Route_Runs_ComputeTime_Button.Location = new System.Drawing.Point(11, 520);
			this.Route_Runs_ComputeTime_Button.Name = "Route_Runs_ComputeTime_Button";
			this.Route_Runs_ComputeTime_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_Runs_ComputeTime_Button.TabIndex = 8;
			this.Route_Runs_ComputeTime_Button.Text = "Посчитать время...";
			this.Route_Runs_ComputeTime_Button.UseVisualStyleBackColor = true;
			this.Route_Runs_ComputeTime_Button.Click += new System.EventHandler(Route_Runs_ComputeTime_Button_Click);
			this.Route_Runs_Time_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Route_Runs_Time_Box.Hours = 0;
			this.Route_Runs_Time_Box.Location = new System.Drawing.Point(11, 493);
			this.Route_Runs_Time_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Route_Runs_Time_Box.Minutes = 0;
			this.Route_Runs_Time_Box.Name = "Route_Runs_Time_Box";
			this.Route_Runs_Time_Box.Seconds = 0;
			this.Route_Runs_Time_Box.Size = new System.Drawing.Size(136, 21);
			this.Route_Runs_Time_Box.TabIndex = 7;
			this.Route_Runs_Time_Box.Time = new System.DateTime(0L);
			this.Route_Runs_Time_Box.Time_Minutes = 0;
			this.Route_Runs_Time_Box.Time_Seconds = 0;
			this.Route_Runs_Time_Box.ViewSeconds = true;
			this.Route_Runs_Time_Box.Validated += new System.EventHandler(Route_Runs_Time_Box_Validated);
			this.Route_Runs_ToParkIndex_UpDown.Location = new System.Drawing.Point(11, 453);
			this.Route_Runs_ToParkIndex_UpDown.Name = "Route_Runs_ToParkIndex_UpDown";
			this.Route_Runs_ToParkIndex_UpDown.Size = new System.Drawing.Size(136, 20);
			this.Route_Runs_ToParkIndex_UpDown.TabIndex = 6;
			this.Route_Runs_ToParkIndex_UpDown.ValueChanged += new System.EventHandler(RouteRunsToParkIndexUpDownValueChanged);
			this.Route_Runs_ToPark_Box.AutoSize = true;
			this.Route_Runs_ToPark_Box.Location = new System.Drawing.Point(11, 404);
			this.Route_Runs_ToPark_Box.Name = "Route_Runs_ToPark_Box";
			this.Route_Runs_ToPark_Box.Size = new System.Drawing.Size(60, 17);
			this.Route_Runs_ToPark_Box.TabIndex = 5;
			this.Route_Runs_ToPark_Box.Text = "В парк";
			this.Route_Runs_ToPark_Box.UseVisualStyleBackColor = true;
			this.Route_Runs_ToPark_Box.CheckedChanged += new System.EventHandler(RouteRunsToParkBoxCheckedChanged);
			this.Route_Runs_Park_Box.AutoSize = true;
			this.Route_Runs_Park_Box.Location = new System.Drawing.Point(11, 381);
			this.Route_Runs_Park_Box.Name = "Route_Runs_Park_Box";
			this.Route_Runs_Park_Box.Size = new System.Drawing.Size(105, 17);
			this.Route_Runs_Park_Box.TabIndex = 5;
			this.Route_Runs_Park_Box.Text = "Парковый рейс";
			this.Route_Runs_Park_Box.UseVisualStyleBackColor = true;
			this.Route_Runs_Park_Box.CheckedChanged += new System.EventHandler(Route_Runs_Park_Box_CheckedChanged);
			this.Route_Runs_Time_label.AutoSize = true;
			this.Route_Runs_Time_label.Location = new System.Drawing.Point(11, 477);
			this.Route_Runs_Time_label.Name = "Route_Runs_Time_label";
			this.Route_Runs_Time_label.Size = new System.Drawing.Size(77, 13);
			this.Route_Runs_Time_label.TabIndex = 4;
			this.Route_Runs_Time_label.Text = "Время в пути:";
			this.Route_Runs_ToParkIndex_label.AutoSize = true;
			this.Route_Runs_ToParkIndex_label.Location = new System.Drawing.Point(11, 424);
			this.Route_Runs_ToParkIndex_label.Name = "Route_Runs_ToParkIndex_label";
			this.Route_Runs_ToParkIndex_label.Size = new System.Drawing.Size(113, 26);
			this.Route_Runs_ToParkIndex_label.TabIndex = 4;
			this.Route_Runs_ToParkIndex_label.Text = "Номер пути, начиная\r\nс которого в парк:";
			this.Route_Runs_label.AutoSize = true;
			this.Route_Runs_label.Location = new System.Drawing.Point(11, 280);
			this.Route_Runs_label.Name = "Route_Runs_label";
			this.Route_Runs_label.Size = new System.Drawing.Size(102, 13);
			this.Route_Runs_label.TabIndex = 4;
			this.Route_Runs_label.Text = "Трассы маршрута:";
			this.Route_Name_label.AutoSize = true;
			this.Route_Name_label.Location = new System.Drawing.Point(11, 131);
			this.Route_Name_label.Name = "Route_Name_label";
			this.Route_Name_label.Size = new System.Drawing.Size(113, 13);
			this.Route_Name_label.TabIndex = 4;
			this.Route_Name_label.Text = "Название маршрута:";
			this.Route_Name_Box.Location = new System.Drawing.Point(11, 147);
			this.Route_Name_Box.Name = "Route_Name_Box";
			this.Route_Name_Box.Size = new System.Drawing.Size(136, 20);
			this.Route_Name_Box.TabIndex = 3;
			this.Route_Name_Box.ModifiedChanged += new System.EventHandler(RouteNameBoxModifiedChanged);
			this.Route_Runs_Remove_Button.Location = new System.Drawing.Point(11, 352);
			this.Route_Runs_Remove_Button.Name = "Route_Runs_Remove_Button";
			this.Route_Runs_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_Runs_Remove_Button.TabIndex = 2;
			this.Route_Runs_Remove_Button.Text = "Удалить трассу";
			this.Route_Runs_Remove_Button.UseVisualStyleBackColor = true;
			this.Route_Runs_Remove_Button.Click += new System.EventHandler(Route_Runs_Remove_Button_Click);
			this.Route_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Route_Remove_Button.Name = "Route_Remove_Button";
			this.Route_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_Remove_Button.TabIndex = 2;
			this.Route_Remove_Button.Text = "Удалить маршрут";
			this.Route_Remove_Button.UseVisualStyleBackColor = true;
			this.Route_Remove_Button.Click += new System.EventHandler(RouteRemoveButtonClick);
			this.Route_Runs_Add_Button.Location = new System.Drawing.Point(11, 323);
			this.Route_Runs_Add_Button.Name = "Route_Runs_Add_Button";
			this.Route_Runs_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_Runs_Add_Button.TabIndex = 2;
			this.Route_Runs_Add_Button.Text = "Добавить трассу";
			this.Route_Runs_Add_Button.UseVisualStyleBackColor = true;
			this.Route_Runs_Add_Button.Click += new System.EventHandler(RouteRunsAddButtonClick);
			this.Route_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Route_Add_Button.Name = "Route_Add_Button";
			this.Route_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_Add_Button.TabIndex = 2;
			this.Route_Add_Button.Text = "Добавить маршрут";
			this.Route_Add_Button.UseVisualStyleBackColor = true;
			this.Route_Add_Button.Click += new System.EventHandler(Route_Add_Button_Click);
			this.Route_ChangeName_Button.Location = new System.Drawing.Point(11, 174);
			this.Route_ChangeName_Button.Name = "Route_ChangeName_Button";
			this.Route_ChangeName_Button.Size = new System.Drawing.Size(136, 23);
			this.Route_ChangeName_Button.TabIndex = 2;
			this.Route_ChangeName_Button.Text = "Изменить название";
			this.Route_ChangeName_Button.UseVisualStyleBackColor = true;
			this.Route_ChangeName_Button.Click += new System.EventHandler(RouteChangeNameButtonClick);
			this.Route_label.AutoSize = true;
			this.Route_label.Location = new System.Drawing.Point(11, 20);
			this.Route_label.Name = "Route_label";
			this.Route_label.Size = new System.Drawing.Size(63, 13);
			this.Route_label.TabIndex = 1;
			this.Route_label.Text = "Маршруты:";
			this.Route_Runs_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Route_Runs_Box.FormattingEnabled = true;
			this.Route_Runs_Box.Location = new System.Drawing.Point(11, 296);
			this.Route_Runs_Box.Name = "Route_Runs_Box";
			this.Route_Runs_Box.Size = new System.Drawing.Size(136, 21);
			this.Route_Runs_Box.TabIndex = 0;
			this.Route_Runs_Box.SelectedIndexChanged += new System.EventHandler(Route_Runs_Box_SelectedIndexChanged);
			this.Route_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Route_Box.FormattingEnabled = true;
			this.Route_Box.Location = new System.Drawing.Point(11, 36);
			this.Route_Box.Name = "Route_Box";
			this.Route_Box.Size = new System.Drawing.Size(136, 21);
			this.Route_Box.TabIndex = 0;
			this.Route_Box.SelectedIndexChanged += new System.EventHandler(RouteBoxSelectedIndexChanged);
			this.svetofor_panel.Controls.Add(this.Svetofor_Model_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Model_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowRed_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowYellow_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowGreen_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Cycle_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_OfGreen_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_End_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_ToGreen_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Begin_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_Location_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Cycle_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Green_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Work_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Remove_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowRed_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowYellow_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_ArrowGreen_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Add_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_EditLocation_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_ShowLocation_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_label);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_Remove_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Element_Box);
			this.svetofor_panel.Controls.Add(this.Svetofor_Signal_Add_Button);
			this.svetofor_panel.Controls.Add(this.Svetofor_Svetofor_Add_Button);
			this.svetofor_panel.Location = new System.Drawing.Point(183, 11);
			this.svetofor_panel.Name = "svetofor_panel";
			this.svetofor_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.svetofor_panel.Size = new System.Drawing.Size(158, 796);
			this.svetofor_panel.TabIndex = 5;
			this.svetofor_panel.Visible = false;
			this.Svetofor_Model_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Model_Box.FormattingEnabled = true;
			this.Svetofor_Model_Box.Location = new System.Drawing.Point(11, 653);
			this.Svetofor_Model_Box.Name = "Svetofor_Model_Box";
			this.Svetofor_Model_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Model_Box.TabIndex = 11;
			this.Svetofor_Model_label.AutoSize = true;
			this.Svetofor_Model_label.Location = new System.Drawing.Point(11, 637);
			this.Svetofor_Model_label.Name = "Svetofor_Model_label";
			this.Svetofor_Model_label.Size = new System.Drawing.Size(49, 13);
			this.Svetofor_Model_label.TabIndex = 10;
			this.Svetofor_Model_label.Text = "Модель:";
			this.Svetofor_Svetofor_ArrowRed_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Svetofor_ArrowRed_Box.FormattingEnabled = true;
			this.Svetofor_Svetofor_ArrowRed_Box.Location = new System.Drawing.Point(11, 511);
			this.Svetofor_Svetofor_ArrowRed_Box.Name = "Svetofor_Svetofor_ArrowRed_Box";
			this.Svetofor_Svetofor_ArrowRed_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Svetofor_ArrowRed_Box.TabIndex = 8;
			this.Svetofor_Svetofor_ArrowRed_Box.SelectedIndexChanged += new System.EventHandler(Svetofor_Svetofor_ArrowRed_Box_SelectedIndexChanged);
			this.Svetofor_Svetofor_ArrowYellow_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Svetofor_ArrowYellow_Box.FormattingEnabled = true;
			this.Svetofor_Svetofor_ArrowYellow_Box.Location = new System.Drawing.Point(11, 469);
			this.Svetofor_Svetofor_ArrowYellow_Box.Name = "Svetofor_Svetofor_ArrowYellow_Box";
			this.Svetofor_Svetofor_ArrowYellow_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Svetofor_ArrowYellow_Box.TabIndex = 8;
			this.Svetofor_Svetofor_ArrowYellow_Box.SelectedIndexChanged += new System.EventHandler(Svetofor_Svetofor_ArrowYellow_Box_SelectedIndexChanged);
			this.Svetofor_Svetofor_ArrowGreen_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Svetofor_ArrowGreen_Box.FormattingEnabled = true;
			this.Svetofor_Svetofor_ArrowGreen_Box.Location = new System.Drawing.Point(11, 429);
			this.Svetofor_Svetofor_ArrowGreen_Box.Name = "Svetofor_Svetofor_ArrowGreen_Box";
			this.Svetofor_Svetofor_ArrowGreen_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Svetofor_ArrowGreen_Box.TabIndex = 8;
			this.Svetofor_Svetofor_ArrowGreen_Box.SelectedIndexChanged += new System.EventHandler(Svetofor_Svetofor_ArrowGreen_Box_SelectedIndexChanged);
			this.Svetofor_Cycle_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Svetofor_Cycle_Box.Hours = 0;
			this.Svetofor_Cycle_Box.Location = new System.Drawing.Point(11, 187);
			this.Svetofor_Cycle_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Svetofor_Cycle_Box.Minutes = 0;
			this.Svetofor_Cycle_Box.Name = "Svetofor_Cycle_Box";
			this.Svetofor_Cycle_Box.Seconds = 0;
			this.Svetofor_Cycle_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Cycle_Box.TabIndex = 7;
			this.Svetofor_Cycle_Box.Time = new System.DateTime(0L);
			this.Svetofor_Cycle_Box.Time_Minutes = 0;
			this.Svetofor_Cycle_Box.Time_Seconds = 0;
			this.Svetofor_Cycle_Box.ViewSeconds = true;
			this.Svetofor_Cycle_Box.TimeChanged += new System.EventHandler(Svetofor_Cycle_Box_TimeChanged);
			this.Svetofor_OfGreen_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Svetofor_OfGreen_Box.Hours = 0;
			this.Svetofor_OfGreen_Box.Location = new System.Drawing.Point(82, 240);
			this.Svetofor_OfGreen_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Svetofor_OfGreen_Box.Minutes = 0;
			this.Svetofor_OfGreen_Box.Name = "Svetofor_OfGreen_Box";
			this.Svetofor_OfGreen_Box.Seconds = 0;
			this.Svetofor_OfGreen_Box.Size = new System.Drawing.Size(65, 21);
			this.Svetofor_OfGreen_Box.TabIndex = 7;
			this.Svetofor_OfGreen_Box.Time = new System.DateTime(0L);
			this.Svetofor_OfGreen_Box.Time_Minutes = 0;
			this.Svetofor_OfGreen_Box.Time_Seconds = 0;
			this.Svetofor_OfGreen_Box.ViewSeconds = true;
			this.Svetofor_OfGreen_Box.TimeChanged += new System.EventHandler(Svetofor_OfGreen_Box_TimeChanged);
			this.Svetofor_End_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Svetofor_End_Box.Hours = 0;
			this.Svetofor_End_Box.Location = new System.Drawing.Point(82, 147);
			this.Svetofor_End_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Svetofor_End_Box.Minutes = 0;
			this.Svetofor_End_Box.Name = "Svetofor_End_Box";
			this.Svetofor_End_Box.Seconds = 0;
			this.Svetofor_End_Box.Size = new System.Drawing.Size(65, 21);
			this.Svetofor_End_Box.TabIndex = 7;
			this.Svetofor_End_Box.Time = new System.DateTime(0L);
			this.Svetofor_End_Box.Time_Minutes = 0;
			this.Svetofor_End_Box.Time_Seconds = 0;
			this.Svetofor_End_Box.ViewSeconds = true;
			this.Svetofor_End_Box.TimeChanged += new System.EventHandler(Svetofor_End_Box_TimeChanged);
			this.Svetofor_ToGreen_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Svetofor_ToGreen_Box.Hours = 0;
			this.Svetofor_ToGreen_Box.Location = new System.Drawing.Point(11, 240);
			this.Svetofor_ToGreen_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Svetofor_ToGreen_Box.Minutes = 0;
			this.Svetofor_ToGreen_Box.Name = "Svetofor_ToGreen_Box";
			this.Svetofor_ToGreen_Box.Seconds = 0;
			this.Svetofor_ToGreen_Box.Size = new System.Drawing.Size(65, 21);
			this.Svetofor_ToGreen_Box.TabIndex = 7;
			this.Svetofor_ToGreen_Box.Time = new System.DateTime(0L);
			this.Svetofor_ToGreen_Box.Time_Minutes = 0;
			this.Svetofor_ToGreen_Box.Time_Seconds = 0;
			this.Svetofor_ToGreen_Box.ViewSeconds = true;
			this.Svetofor_ToGreen_Box.TimeChanged += new System.EventHandler(Svetofor_ToGreen_Box_TimeChanged);
			this.Svetofor_Begin_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Svetofor_Begin_Box.Hours = 0;
			this.Svetofor_Begin_Box.Location = new System.Drawing.Point(11, 147);
			this.Svetofor_Begin_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Svetofor_Begin_Box.Minutes = 0;
			this.Svetofor_Begin_Box.Name = "Svetofor_Begin_Box";
			this.Svetofor_Begin_Box.Seconds = 0;
			this.Svetofor_Begin_Box.Size = new System.Drawing.Size(65, 21);
			this.Svetofor_Begin_Box.TabIndex = 7;
			this.Svetofor_Begin_Box.Time = new System.DateTime(0L);
			this.Svetofor_Begin_Box.Time_Minutes = 0;
			this.Svetofor_Begin_Box.Time_Seconds = 0;
			this.Svetofor_Begin_Box.ViewSeconds = true;
			this.Svetofor_Begin_Box.TimeChanged += new System.EventHandler(Svetofor_Begin_Box_TimeChanged);
			this.Svetofor_Element_Location_label.AutoSize = true;
			this.Svetofor_Element_Location_label.Location = new System.Drawing.Point(11, 548);
			this.Svetofor_Element_Location_label.Name = "Svetofor_Element_Location_label";
			this.Svetofor_Element_Location_label.Size = new System.Drawing.Size(98, 13);
			this.Svetofor_Element_Location_label.TabIndex = 4;
			this.Svetofor_Element_Location_label.Text = "Местоположение:";
			this.Svetofor_Cycle_label.AutoSize = true;
			this.Svetofor_Cycle_label.Location = new System.Drawing.Point(11, 171);
			this.Svetofor_Cycle_label.Name = "Svetofor_Cycle_label";
			this.Svetofor_Cycle_label.Size = new System.Drawing.Size(107, 13);
			this.Svetofor_Cycle_label.TabIndex = 4;
			this.Svetofor_Cycle_label.Text = "Светофорный цикл:";
			this.Svetofor_Green_label.AutoSize = true;
			this.Svetofor_Green_label.Location = new System.Drawing.Point(11, 211);
			this.Svetofor_Green_label.Name = "Svetofor_Green_label";
			this.Svetofor_Green_label.Size = new System.Drawing.Size(122, 26);
			this.Svetofor_Green_label.TabIndex = 4;
			this.Svetofor_Green_label.Text = "Время и длительность\r\nзелёного сигнала:";
			this.Svetofor_Work_label.AutoSize = true;
			this.Svetofor_Work_label.Location = new System.Drawing.Point(11, 131);
			this.Svetofor_Work_label.Name = "Svetofor_Work_label";
			this.Svetofor_Work_label.Size = new System.Drawing.Size(83, 13);
			this.Svetofor_Work_label.TabIndex = 4;
			this.Svetofor_Work_label.Text = "Время работы:";
			this.Svetofor_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Svetofor_Remove_Button.Name = "Svetofor_Remove_Button";
			this.Svetofor_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Remove_Button.TabIndex = 2;
			this.Svetofor_Remove_Button.Text = "Удалить систему";
			this.Svetofor_Remove_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Remove_Button.Click += new System.EventHandler(Svetofor_Remove_Button_Click);
			this.Svetofor_Svetofor_ArrowRed_label.AutoSize = true;
			this.Svetofor_Svetofor_ArrowRed_label.Location = new System.Drawing.Point(11, 495);
			this.Svetofor_Svetofor_ArrowRed_label.Name = "Svetofor_Svetofor_ArrowRed_label";
			this.Svetofor_Svetofor_ArrowRed_label.Size = new System.Drawing.Size(97, 13);
			this.Svetofor_Svetofor_ArrowRed_label.TabIndex = 4;
			this.Svetofor_Svetofor_ArrowRed_label.Text = "Красная стрелка:";
			this.Svetofor_Svetofor_ArrowYellow_label.AutoSize = true;
			this.Svetofor_Svetofor_ArrowYellow_label.Location = new System.Drawing.Point(11, 453);
			this.Svetofor_Svetofor_ArrowYellow_label.Name = "Svetofor_Svetofor_ArrowYellow_label";
			this.Svetofor_Svetofor_ArrowYellow_label.Size = new System.Drawing.Size(94, 13);
			this.Svetofor_Svetofor_ArrowYellow_label.TabIndex = 4;
			this.Svetofor_Svetofor_ArrowYellow_label.Text = "Жёлтая стрелка:";
			this.Svetofor_Svetofor_ArrowGreen_label.AutoSize = true;
			this.Svetofor_Svetofor_ArrowGreen_label.Location = new System.Drawing.Point(11, 413);
			this.Svetofor_Svetofor_ArrowGreen_label.Name = "Svetofor_Svetofor_ArrowGreen_label";
			this.Svetofor_Svetofor_ArrowGreen_label.Size = new System.Drawing.Size(97, 13);
			this.Svetofor_Svetofor_ArrowGreen_label.TabIndex = 4;
			this.Svetofor_Svetofor_ArrowGreen_label.Text = "Зелёная стрелка:";
			this.Svetofor_Element_label.AutoSize = true;
			this.Svetofor_Element_label.Location = new System.Drawing.Point(11, 276);
			this.Svetofor_Element_label.Name = "Svetofor_Element_label";
			this.Svetofor_Element_label.Size = new System.Drawing.Size(62, 13);
			this.Svetofor_Element_label.TabIndex = 4;
			this.Svetofor_Element_label.Text = "Элементы:";
			this.Svetofor_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Svetofor_Add_Button.Name = "Svetofor_Add_Button";
			this.Svetofor_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Add_Button.TabIndex = 2;
			this.Svetofor_Add_Button.Text = "Добавить систему";
			this.Svetofor_Add_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Add_Button.Click += new System.EventHandler(Svetofor_Add_Button_Click);
			this.Svetofor_Element_EditLocation_Button.Location = new System.Drawing.Point(11, 593);
			this.Svetofor_Element_EditLocation_Button.Name = "Svetofor_Element_EditLocation_Button";
			this.Svetofor_Element_EditLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Element_EditLocation_Button.TabIndex = 2;
			this.Svetofor_Element_EditLocation_Button.Text = "Изменить";
			this.Svetofor_Element_EditLocation_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Element_EditLocation_Button.Click += new System.EventHandler(Svetofor_Element_EditLocation_Button_Click);
			this.Svetofor_Element_ShowLocation_Button.Location = new System.Drawing.Point(11, 564);
			this.Svetofor_Element_ShowLocation_Button.Name = "Svetofor_Element_ShowLocation_Button";
			this.Svetofor_Element_ShowLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Element_ShowLocation_Button.TabIndex = 2;
			this.Svetofor_Element_ShowLocation_Button.Text = "Показать";
			this.Svetofor_Element_ShowLocation_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Element_ShowLocation_Button.Click += new System.EventHandler(Svetofor_Element_ShowLocation_Button_Click);
			this.Svetofor_label.AutoSize = true;
			this.Svetofor_label.Location = new System.Drawing.Point(11, 20);
			this.Svetofor_label.Name = "Svetofor_label";
			this.Svetofor_label.Size = new System.Drawing.Size(128, 13);
			this.Svetofor_label.TabIndex = 1;
			this.Svetofor_label.Text = "Светофорные системы:";
			this.Svetofor_Element_Remove_Button.Location = new System.Drawing.Point(11, 377);
			this.Svetofor_Element_Remove_Button.Name = "Svetofor_Element_Remove_Button";
			this.Svetofor_Element_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Element_Remove_Button.TabIndex = 2;
			this.Svetofor_Element_Remove_Button.Text = "Удалить элемент";
			this.Svetofor_Element_Remove_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Element_Remove_Button.Click += new System.EventHandler(Svetofor_Element_Remove_Button_Click);
			this.Svetofor_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Box.FormattingEnabled = true;
			this.Svetofor_Box.Location = new System.Drawing.Point(11, 36);
			this.Svetofor_Box.Name = "Svetofor_Box";
			this.Svetofor_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Box.TabIndex = 0;
			this.Svetofor_Box.SelectedIndexChanged += new System.EventHandler(Svetofor_Box_SelectedIndexChanged);
			this.Svetofor_Element_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Svetofor_Element_Box.FormattingEnabled = true;
			this.Svetofor_Element_Box.Location = new System.Drawing.Point(11, 292);
			this.Svetofor_Element_Box.Name = "Svetofor_Element_Box";
			this.Svetofor_Element_Box.Size = new System.Drawing.Size(136, 21);
			this.Svetofor_Element_Box.TabIndex = 0;
			this.Svetofor_Element_Box.SelectedIndexChanged += new System.EventHandler(Svetofor_Element_Box_SelectedIndexChanged);
			this.Svetofor_Signal_Add_Button.Location = new System.Drawing.Point(11, 348);
			this.Svetofor_Signal_Add_Button.Name = "Svetofor_Signal_Add_Button";
			this.Svetofor_Signal_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Signal_Add_Button.TabIndex = 2;
			this.Svetofor_Signal_Add_Button.Text = "Добавить сигнал";
			this.Svetofor_Signal_Add_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Signal_Add_Button.Click += new System.EventHandler(Svetofor_Signal_Add_Button_Click);
			this.Svetofor_Svetofor_Add_Button.Location = new System.Drawing.Point(11, 319);
			this.Svetofor_Svetofor_Add_Button.Name = "Svetofor_Svetofor_Add_Button";
			this.Svetofor_Svetofor_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Svetofor_Svetofor_Add_Button.TabIndex = 2;
			this.Svetofor_Svetofor_Add_Button.Text = "Добавить светофор";
			this.Svetofor_Svetofor_Add_Button.UseVisualStyleBackColor = true;
			this.Svetofor_Svetofor_Add_Button.Click += new System.EventHandler(Svetofor_Svetofor_Add_Button_Click);
			this.splines_panel.Controls.Add(this.Rail_Box_NumericBox);
			this.splines_panel.Controls.Add(this.Spline_Select_mode_Box);
			this.splines_panel.Controls.Add(this.Splines_Instance_Box);
			this.splines_panel.Controls.Add(this.Splines_Instance_label);
			this.splines_panel.Controls.Add(this.Splines_ShowLocation_Button);
			this.splines_panel.Controls.Add(this.Rail_Box_dist_Label);
			this.splines_panel.Controls.Add(this.Splines_Location_label);
			this.splines_panel.Controls.Add(this.Splines_ChangeModel_Button);
			this.splines_panel.Controls.Add(this.Splines_Remove_Button);
			this.splines_panel.Controls.Add(this.Splines_Models_Box);
			this.splines_panel.Controls.Add(this.Splines_label);
			this.splines_panel.Location = new System.Drawing.Point(0, 98);
			this.splines_panel.Name = "splines_panel";
			this.splines_panel.Size = new System.Drawing.Size(158, 309);
			this.splines_panel.TabIndex = 9;
			this.splines_panel.Visible = false;
			this.Rail_Box_NumericBox.Location = new System.Drawing.Point(11, 252);
			this.Rail_Box_NumericBox.Name = "Rail_Box_NumericBox";
			this.Rail_Box_NumericBox.Size = new System.Drawing.Size(136, 20);
			this.Rail_Box_NumericBox.TabIndex = 10;
			this.Rail_Box_NumericBox.Text = "0";
			this.Rail_Box_NumericBox.Value = 0.0;
			this.Rail_Box_NumericBox.EnterPressed += new System.EventHandler(Rails_NumericBoxEnterPressed);
			this.Spline_Select_mode_Box.Location = new System.Drawing.Point(12, 282);
			this.Spline_Select_mode_Box.Name = "Spline_Select_mode_Box";
			this.Spline_Select_mode_Box.Size = new System.Drawing.Size(135, 24);
			this.Spline_Select_mode_Box.TabIndex = 9;
			this.Spline_Select_mode_Box.Text = "Режим выбора";
			this.Spline_Select_mode_Box.UseVisualStyleBackColor = true;
			this.Splines_Instance_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Splines_Instance_Box.FormattingEnabled = true;
			this.Splines_Instance_Box.Location = new System.Drawing.Point(11, 79);
			this.Splines_Instance_Box.Name = "Splines_Instance_Box";
			this.Splines_Instance_Box.Size = new System.Drawing.Size(136, 21);
			this.Splines_Instance_Box.TabIndex = 8;
			this.Splines_Instance_Box.SelectedIndexChanged += new System.EventHandler(Splines_Instance_BoxSelectedIndexChanged);
			this.Splines_Instance_label.AutoSize = true;
			this.Splines_Instance_label.Location = new System.Drawing.Point(11, 63);
			this.Splines_Instance_label.Name = "Splines_Instance_label";
			this.Splines_Instance_label.Size = new System.Drawing.Size(75, 13);
			this.Splines_Instance_label.TabIndex = 7;
			this.Splines_Instance_label.Text = "Экземпляры:";
			this.Splines_ShowLocation_Button.Location = new System.Drawing.Point(11, 209);
			this.Splines_ShowLocation_Button.Name = "Splines_ShowLocation_Button";
			this.Splines_ShowLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Splines_ShowLocation_Button.TabIndex = 5;
			this.Splines_ShowLocation_Button.Text = "Показать";
			this.Splines_ShowLocation_Button.UseVisualStyleBackColor = true;
			this.Splines_ShowLocation_Button.Click += new System.EventHandler(Splines_ShowLocation_ButtonClick);
			this.Rail_Box_dist_Label.AutoSize = true;
			this.Rail_Box_dist_Label.Location = new System.Drawing.Point(11, 237);
			this.Rail_Box_dist_Label.Name = "Rail_Box_dist_Label";
			this.Rail_Box_dist_Label.Size = new System.Drawing.Size(115, 13);
			this.Rail_Box_dist_Label.TabIndex = 4;
			this.Rail_Box_dist_Label.Text = "Расстояние коробки:";
			this.Splines_Location_label.AutoSize = true;
			this.Splines_Location_label.Location = new System.Drawing.Point(11, 193);
			this.Splines_Location_label.Name = "Splines_Location_label";
			this.Splines_Location_label.Size = new System.Drawing.Size(98, 13);
			this.Splines_Location_label.TabIndex = 4;
			this.Splines_Location_label.Text = "Местоположение:";
			this.Splines_ChangeModel_Button.Location = new System.Drawing.Point(11, 135);
			this.Splines_ChangeModel_Button.Name = "Splines_ChangeModel_Button";
			this.Splines_ChangeModel_Button.Size = new System.Drawing.Size(136, 23);
			this.Splines_ChangeModel_Button.TabIndex = 3;
			this.Splines_ChangeModel_Button.Text = "Сменить модель";
			this.Splines_ChangeModel_Button.UseVisualStyleBackColor = true;
			this.Splines_ChangeModel_Button.Click += new System.EventHandler(Splines_ChangeModel_ButtonClick);
			this.Splines_Remove_Button.Location = new System.Drawing.Point(11, 106);
			this.Splines_Remove_Button.Name = "Splines_Remove_Button";
			this.Splines_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Splines_Remove_Button.TabIndex = 2;
			this.Splines_Remove_Button.Text = "Удалить сплайн";
			this.Splines_Remove_Button.UseVisualStyleBackColor = true;
			this.Splines_Remove_Button.Click += new System.EventHandler(Splines_Remove_ButtonClick);
			this.Splines_Models_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Splines_Models_Box.FormattingEnabled = true;
			this.Splines_Models_Box.Location = new System.Drawing.Point(11, 36);
			this.Splines_Models_Box.Name = "Splines_Models_Box";
			this.Splines_Models_Box.Size = new System.Drawing.Size(136, 21);
			this.Splines_Models_Box.TabIndex = 1;
			this.Splines_Models_Box.SelectedIndexChanged += new System.EventHandler(Splines_Models_BoxSelectedIndexChanged);
			this.Splines_label.AutoSize = true;
			this.Splines_label.Location = new System.Drawing.Point(11, 20);
			this.Splines_label.Name = "Splines_label";
			this.Splines_label.Size = new System.Drawing.Size(55, 13);
			this.Splines_label.TabIndex = 0;
			this.Splines_label.Text = "Сплайны:";
			this.signals_panel.Controls.Add(this.Signals_Model_Box);
			this.signals_panel.Controls.Add(this.Signals_Model_label);
			this.signals_panel.Controls.Add(this.Signals_Element_Minus_Box);
			this.signals_panel.Controls.Add(this.Signals_Bound_UpDown);
			this.signals_panel.Controls.Add(this.Signals_Element_Location_label);
			this.signals_panel.Controls.Add(this.Signals_Bound_label);
			this.signals_panel.Controls.Add(this.Signals_Remove_Button);
			this.signals_panel.Controls.Add(this.Signals_Element_label);
			this.signals_panel.Controls.Add(this.Signals_Add_Button);
			this.signals_panel.Controls.Add(this.Signals_Element_EditLocation_Button);
			this.signals_panel.Controls.Add(this.Signals_Element_ShowLocation_Button);
			this.signals_panel.Controls.Add(this.Signals_label);
			this.signals_panel.Controls.Add(this.Signals_Element_Remove_Button);
			this.signals_panel.Controls.Add(this.Signals_Box);
			this.signals_panel.Controls.Add(this.Signals_Element_Box);
			this.signals_panel.Controls.Add(this.Signals_Element_AddSignal_Button);
			this.signals_panel.Controls.Add(this.Signals_Element_AddContact_Button);
			this.signals_panel.Location = new System.Drawing.Point(3, 3);
			this.signals_panel.Name = "signals_panel";
			this.signals_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.signals_panel.Size = new System.Drawing.Size(158, 614);
			this.signals_panel.TabIndex = 5;
			this.signals_panel.Visible = false;
			this.Signals_Model_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Signals_Model_Box.FormattingEnabled = true;
			this.Signals_Model_Box.Location = new System.Drawing.Point(11, 501);
			this.Signals_Model_Box.Name = "Signals_Model_Box";
			this.Signals_Model_Box.Size = new System.Drawing.Size(136, 21);
			this.Signals_Model_Box.TabIndex = 8;
			this.Signals_Model_label.AutoSize = true;
			this.Signals_Model_label.Location = new System.Drawing.Point(11, 485);
			this.Signals_Model_label.Name = "Signals_Model_label";
			this.Signals_Model_label.Size = new System.Drawing.Size(49, 13);
			this.Signals_Model_label.TabIndex = 7;
			this.Signals_Model_label.Text = "Модель:";
			this.Signals_Element_Minus_Box.AutoSize = true;
			this.Signals_Element_Minus_Box.Location = new System.Drawing.Point(11, 357);
			this.Signals_Element_Minus_Box.Name = "Signals_Element_Minus_Box";
			this.Signals_Element_Minus_Box.Size = new System.Drawing.Size(125, 17);
			this.Signals_Element_Minus_Box.TabIndex = 6;
			this.Signals_Element_Minus_Box.Text = "Минусовой контакт";
			this.Signals_Element_Minus_Box.UseVisualStyleBackColor = true;
			this.Signals_Element_Minus_Box.Visible = false;
			this.Signals_Element_Minus_Box.CheckedChanged += new System.EventHandler(Signals_Element_Minus_Box_CheckedChanged);
			this.Signals_Bound_UpDown.Location = new System.Drawing.Point(11, 147);
			this.Signals_Bound_UpDown.Name = "Signals_Bound_UpDown";
			this.Signals_Bound_UpDown.Size = new System.Drawing.Size(136, 20);
			this.Signals_Bound_UpDown.TabIndex = 5;
			this.Signals_Bound_UpDown.ValueChanged += new System.EventHandler(Signals_Bound_UpDown_ValueChanged);
			this.Signals_Element_Location_label.AutoSize = true;
			this.Signals_Element_Location_label.Location = new System.Drawing.Point(11, 397);
			this.Signals_Element_Location_label.Name = "Signals_Element_Location_label";
			this.Signals_Element_Location_label.Size = new System.Drawing.Size(98, 13);
			this.Signals_Element_Location_label.TabIndex = 4;
			this.Signals_Element_Location_label.Text = "Местоположение:";
			this.Signals_Bound_label.AutoSize = true;
			this.Signals_Bound_label.Location = new System.Drawing.Point(11, 131);
			this.Signals_Bound_label.Name = "Signals_Bound_label";
			this.Signals_Bound_label.Size = new System.Drawing.Size(126, 13);
			this.Signals_Bound_label.TabIndex = 4;
			this.Signals_Bound_label.Text = "Красный при значении:";
			this.Signals_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Signals_Remove_Button.Name = "Signals_Remove_Button";
			this.Signals_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Remove_Button.TabIndex = 2;
			this.Signals_Remove_Button.Text = "Удалить систему";
			this.Signals_Remove_Button.UseVisualStyleBackColor = true;
			this.Signals_Remove_Button.Click += new System.EventHandler(Signals_Remove_Button_Click);
			this.Signals_Element_label.AutoSize = true;
			this.Signals_Element_label.Location = new System.Drawing.Point(11, 204);
			this.Signals_Element_label.Name = "Signals_Element_label";
			this.Signals_Element_label.Size = new System.Drawing.Size(62, 13);
			this.Signals_Element_label.TabIndex = 4;
			this.Signals_Element_label.Text = "Элементы:";
			this.Signals_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Signals_Add_Button.Name = "Signals_Add_Button";
			this.Signals_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Add_Button.TabIndex = 2;
			this.Signals_Add_Button.Text = "Добавить систему";
			this.Signals_Add_Button.UseVisualStyleBackColor = true;
			this.Signals_Add_Button.Click += new System.EventHandler(Signals_Add_Button_Click);
			this.Signals_Element_EditLocation_Button.Location = new System.Drawing.Point(11, 442);
			this.Signals_Element_EditLocation_Button.Name = "Signals_Element_EditLocation_Button";
			this.Signals_Element_EditLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Element_EditLocation_Button.TabIndex = 2;
			this.Signals_Element_EditLocation_Button.Text = "Изменить";
			this.Signals_Element_EditLocation_Button.UseVisualStyleBackColor = true;
			this.Signals_Element_EditLocation_Button.Click += new System.EventHandler(Signals_Element_EditLocation_Button_Click);
			this.Signals_Element_ShowLocation_Button.Location = new System.Drawing.Point(11, 413);
			this.Signals_Element_ShowLocation_Button.Name = "Signals_Element_ShowLocation_Button";
			this.Signals_Element_ShowLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Element_ShowLocation_Button.TabIndex = 2;
			this.Signals_Element_ShowLocation_Button.Text = "Показать";
			this.Signals_Element_ShowLocation_Button.UseVisualStyleBackColor = true;
			this.Signals_Element_ShowLocation_Button.Click += new System.EventHandler(Signals_Element_ShowLocation_Button_Click);
			this.Signals_label.AutoSize = true;
			this.Signals_label.Location = new System.Drawing.Point(11, 20);
			this.Signals_label.Name = "Signals_label";
			this.Signals_label.Size = new System.Drawing.Size(120, 13);
			this.Signals_label.TabIndex = 1;
			this.Signals_label.Text = "Сигнальные системы:";
			this.Signals_Element_Remove_Button.Location = new System.Drawing.Point(11, 305);
			this.Signals_Element_Remove_Button.Name = "Signals_Element_Remove_Button";
			this.Signals_Element_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Element_Remove_Button.TabIndex = 2;
			this.Signals_Element_Remove_Button.Text = "Удалить элемент";
			this.Signals_Element_Remove_Button.UseVisualStyleBackColor = true;
			this.Signals_Element_Remove_Button.Click += new System.EventHandler(Signals_Element_Remove_Button_Click);
			this.Signals_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Signals_Box.FormattingEnabled = true;
			this.Signals_Box.Location = new System.Drawing.Point(11, 36);
			this.Signals_Box.Name = "Signals_Box";
			this.Signals_Box.Size = new System.Drawing.Size(136, 21);
			this.Signals_Box.TabIndex = 0;
			this.Signals_Box.SelectedIndexChanged += new System.EventHandler(Signals_Box_SelectedIndexChanged);
			this.Signals_Element_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Signals_Element_Box.FormattingEnabled = true;
			this.Signals_Element_Box.Location = new System.Drawing.Point(11, 220);
			this.Signals_Element_Box.Name = "Signals_Element_Box";
			this.Signals_Element_Box.Size = new System.Drawing.Size(136, 21);
			this.Signals_Element_Box.TabIndex = 0;
			this.Signals_Element_Box.SelectedIndexChanged += new System.EventHandler(Signals_Element_Box_SelectedIndexChanged);
			this.Signals_Element_AddSignal_Button.Location = new System.Drawing.Point(11, 276);
			this.Signals_Element_AddSignal_Button.Name = "Signals_Element_AddSignal_Button";
			this.Signals_Element_AddSignal_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Element_AddSignal_Button.TabIndex = 2;
			this.Signals_Element_AddSignal_Button.Text = "Добавить сигнал";
			this.Signals_Element_AddSignal_Button.UseVisualStyleBackColor = true;
			this.Signals_Element_AddSignal_Button.Click += new System.EventHandler(Signals_Element_AddSignal_Button_Click);
			this.Signals_Element_AddContact_Button.Location = new System.Drawing.Point(11, 247);
			this.Signals_Element_AddContact_Button.Name = "Signals_Element_AddContact_Button";
			this.Signals_Element_AddContact_Button.Size = new System.Drawing.Size(136, 23);
			this.Signals_Element_AddContact_Button.TabIndex = 2;
			this.Signals_Element_AddContact_Button.Text = "Добавить контакт";
			this.Signals_Element_AddContact_Button.UseVisualStyleBackColor = true;
			this.Signals_Element_AddContact_Button.Click += new System.EventHandler(Signals_Element_AddContact_Button_Click);
			this.stops_panel.Controls.Add(this.Stops_Model_Box);
			this.stops_panel.Controls.Add(this.Stops_Model_label);
			this.stops_panel.Controls.Add(this.TypeOfTransportBox);
			this.stops_panel.Controls.Add(this.Stops_Location_label);
			this.stops_panel.Controls.Add(this.Stops_Name_label);
			this.stops_panel.Controls.Add(this.Stops_Name_Box);
			this.stops_panel.Controls.Add(this.Stops_Remove_Button);
			this.stops_panel.Controls.Add(this.Stops_Add_Button);
			this.stops_panel.Controls.Add(this.Stops_EditLocation_Button);
			this.stops_panel.Controls.Add(this.Stops_ShowLocation_Button);
			this.stops_panel.Controls.Add(this.Stops_ChangeName_Button);
			this.stops_panel.Controls.Add(this.Stops_label);
			this.stops_panel.Controls.Add(this.Stops_Box);
			this.stops_panel.Location = new System.Drawing.Point(364, 233);
			this.stops_panel.Name = "stops_panel";
			this.stops_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.stops_panel.Size = new System.Drawing.Size(158, 562);
			this.stops_panel.TabIndex = 5;
			this.stops_panel.Visible = false;
			this.Stops_Model_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Stops_Model_Box.FormattingEnabled = true;
			this.Stops_Model_Box.Location = new System.Drawing.Point(11, 431);
			this.Stops_Model_Box.Name = "Stops_Model_Box";
			this.Stops_Model_Box.Size = new System.Drawing.Size(136, 21);
			this.Stops_Model_Box.TabIndex = 13;
			this.Stops_Model_label.AutoSize = true;
			this.Stops_Model_label.Location = new System.Drawing.Point(11, 415);
			this.Stops_Model_label.Name = "Stops_Model_label";
			this.Stops_Model_label.Size = new System.Drawing.Size(49, 13);
			this.Stops_Model_label.TabIndex = 12;
			this.Stops_Model_label.Text = "Модель:";
			this.TypeOfTransportBox.Controls.Add(this.BusBox);
			this.TypeOfTransportBox.Controls.Add(this.TrolleybusBox);
			this.TypeOfTransportBox.Controls.Add(this.TramwayBox);
			this.TypeOfTransportBox.Location = new System.Drawing.Point(11, 213);
			this.TypeOfTransportBox.Name = "TypeOfTransportBox";
			this.TypeOfTransportBox.Size = new System.Drawing.Size(136, 98);
			this.TypeOfTransportBox.TabIndex = 6;
			this.TypeOfTransportBox.TabStop = false;
			this.TypeOfTransportBox.Text = "Вид транспорта";
			this.BusBox.AutoSize = true;
			this.BusBox.Location = new System.Drawing.Point(15, 67);
			this.BusBox.Name = "BusBox";
			this.BusBox.Size = new System.Drawing.Size(67, 17);
			this.BusBox.TabIndex = 2;
			this.BusBox.Text = "Автобус";
			this.BusBox.UseVisualStyleBackColor = true;
			this.BusBox.Click += new System.EventHandler(BusBox_Click);
			this.TrolleybusBox.AutoSize = true;
			this.TrolleybusBox.Location = new System.Drawing.Point(15, 44);
			this.TrolleybusBox.Name = "TrolleybusBox";
			this.TrolleybusBox.Size = new System.Drawing.Size(86, 17);
			this.TrolleybusBox.TabIndex = 1;
			this.TrolleybusBox.Text = "Троллейбус";
			this.TrolleybusBox.UseVisualStyleBackColor = true;
			this.TrolleybusBox.Click += new System.EventHandler(TrolleybusBox_Click);
			this.TramwayBox.AutoSize = true;
			this.TramwayBox.Location = new System.Drawing.Point(15, 21);
			this.TramwayBox.Name = "TramwayBox";
			this.TramwayBox.Size = new System.Drawing.Size(71, 17);
			this.TramwayBox.TabIndex = 0;
			this.TramwayBox.Text = "Трамвай";
			this.TramwayBox.UseVisualStyleBackColor = true;
			this.TramwayBox.Click += new System.EventHandler(TramwayBox_Click);
			this.Stops_Location_label.AutoSize = true;
			this.Stops_Location_label.Location = new System.Drawing.Point(11, 327);
			this.Stops_Location_label.Name = "Stops_Location_label";
			this.Stops_Location_label.Size = new System.Drawing.Size(98, 13);
			this.Stops_Location_label.TabIndex = 4;
			this.Stops_Location_label.Text = "Местоположение:";
			this.Stops_Name_label.AutoSize = true;
			this.Stops_Name_label.Location = new System.Drawing.Point(11, 131);
			this.Stops_Name_label.Name = "Stops_Name_label";
			this.Stops_Name_label.Size = new System.Drawing.Size(116, 13);
			this.Stops_Name_label.TabIndex = 4;
			this.Stops_Name_label.Text = "Название остановки:";
			this.Stops_Name_Box.Location = new System.Drawing.Point(11, 147);
			this.Stops_Name_Box.Name = "Stops_Name_Box";
			this.Stops_Name_Box.Size = new System.Drawing.Size(136, 20);
			this.Stops_Name_Box.TabIndex = 3;
			this.Stops_Name_Box.ModifiedChanged += new System.EventHandler(Stops_Name_Box_ModifiedChanged);
			this.Stops_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Stops_Remove_Button.Name = "Stops_Remove_Button";
			this.Stops_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Stops_Remove_Button.TabIndex = 2;
			this.Stops_Remove_Button.Text = "Удалить остановку";
			this.Stops_Remove_Button.UseVisualStyleBackColor = true;
			this.Stops_Remove_Button.Click += new System.EventHandler(Stops_Remove_Button_Click);
			this.Stops_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Stops_Add_Button.Name = "Stops_Add_Button";
			this.Stops_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Stops_Add_Button.TabIndex = 2;
			this.Stops_Add_Button.Text = "Добавить остановку";
			this.Stops_Add_Button.UseVisualStyleBackColor = true;
			this.Stops_Add_Button.Click += new System.EventHandler(Stops_Add_Button_Click);
			this.Stops_EditLocation_Button.Location = new System.Drawing.Point(11, 372);
			this.Stops_EditLocation_Button.Name = "Stops_EditLocation_Button";
			this.Stops_EditLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Stops_EditLocation_Button.TabIndex = 2;
			this.Stops_EditLocation_Button.Text = "Изменить";
			this.Stops_EditLocation_Button.UseVisualStyleBackColor = true;
			this.Stops_EditLocation_Button.Click += new System.EventHandler(Stops_EditLocation_Button_Click);
			this.Stops_ShowLocation_Button.Location = new System.Drawing.Point(11, 343);
			this.Stops_ShowLocation_Button.Name = "Stops_ShowLocation_Button";
			this.Stops_ShowLocation_Button.Size = new System.Drawing.Size(136, 23);
			this.Stops_ShowLocation_Button.TabIndex = 2;
			this.Stops_ShowLocation_Button.Text = "Показать";
			this.Stops_ShowLocation_Button.UseVisualStyleBackColor = true;
			this.Stops_ShowLocation_Button.Click += new System.EventHandler(Stops_ShowLocation_Button_Click);
			this.Stops_ChangeName_Button.Location = new System.Drawing.Point(11, 174);
			this.Stops_ChangeName_Button.Name = "Stops_ChangeName_Button";
			this.Stops_ChangeName_Button.Size = new System.Drawing.Size(136, 23);
			this.Stops_ChangeName_Button.TabIndex = 2;
			this.Stops_ChangeName_Button.Text = "Изменить название";
			this.Stops_ChangeName_Button.UseVisualStyleBackColor = true;
			this.Stops_ChangeName_Button.Click += new System.EventHandler(Stops_ChangeName_Button_Click);
			this.Stops_label.AutoSize = true;
			this.Stops_label.Location = new System.Drawing.Point(11, 20);
			this.Stops_label.Name = "Stops_label";
			this.Stops_label.Size = new System.Drawing.Size(65, 13);
			this.Stops_label.TabIndex = 1;
			this.Stops_label.Text = "Остановки:";
			this.Stops_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Stops_Box.FormattingEnabled = true;
			this.Stops_Box.Location = new System.Drawing.Point(11, 36);
			this.Stops_Box.Name = "Stops_Box";
			this.Stops_Box.Size = new System.Drawing.Size(136, 21);
			this.Stops_Box.TabIndex = 0;
			this.Stops_Box.SelectedIndexChanged += new System.EventHandler(Stops_Box_SelectedIndexChanged);
			this.park_panel.Controls.Add(this.Park_Name_label);
			this.park_panel.Controls.Add(this.Park_Name_Box);
			this.park_panel.Controls.Add(this.Park_Remove_Button);
			this.park_panel.Controls.Add(this.Park_ChangeName_Button);
			this.park_panel.Controls.Add(this.Park_Add_Button);
			this.park_panel.Controls.Add(this.Park_label);
			this.park_panel.Controls.Add(this.Park_Box);
			this.park_panel.Location = new System.Drawing.Point(364, 11);
			this.park_panel.Name = "park_panel";
			this.park_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.park_panel.Size = new System.Drawing.Size(158, 217);
			this.park_panel.TabIndex = 5;
			this.park_panel.Visible = false;
			this.Park_Name_label.AutoSize = true;
			this.Park_Name_label.Location = new System.Drawing.Point(11, 131);
			this.Park_Name_label.Name = "Park_Name_label";
			this.Park_Name_label.Size = new System.Drawing.Size(93, 13);
			this.Park_Name_label.TabIndex = 4;
			this.Park_Name_label.Text = "Название парка:";
			this.Park_Name_Box.Location = new System.Drawing.Point(11, 147);
			this.Park_Name_Box.Name = "Park_Name_Box";
			this.Park_Name_Box.Size = new System.Drawing.Size(136, 20);
			this.Park_Name_Box.TabIndex = 3;
			this.Park_Name_Box.ModifiedChanged += new System.EventHandler(Park_Name_Box_ModifiedChanged);
			this.Park_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Park_Remove_Button.Name = "Park_Remove_Button";
			this.Park_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Park_Remove_Button.TabIndex = 2;
			this.Park_Remove_Button.Text = "Удалить парк";
			this.Park_Remove_Button.UseVisualStyleBackColor = true;
			this.Park_Remove_Button.Click += new System.EventHandler(Park_Remove_Button_Click);
			this.Park_ChangeName_Button.Location = new System.Drawing.Point(11, 174);
			this.Park_ChangeName_Button.Name = "Park_ChangeName_Button";
			this.Park_ChangeName_Button.Size = new System.Drawing.Size(136, 23);
			this.Park_ChangeName_Button.TabIndex = 2;
			this.Park_ChangeName_Button.Text = "Изменить название";
			this.Park_ChangeName_Button.UseVisualStyleBackColor = true;
			this.Park_ChangeName_Button.Click += new System.EventHandler(Park_ChangeName_Button_Click);
			this.Park_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Park_Add_Button.Name = "Park_Add_Button";
			this.Park_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Park_Add_Button.TabIndex = 2;
			this.Park_Add_Button.Text = "Добавить парк";
			this.Park_Add_Button.UseVisualStyleBackColor = true;
			this.Park_Add_Button.Click += new System.EventHandler(Park_Add_Button_Click);
			this.Park_label.AutoSize = true;
			this.Park_label.Location = new System.Drawing.Point(11, 20);
			this.Park_label.Name = "Park_label";
			this.Park_label.Size = new System.Drawing.Size(96, 13);
			this.Park_label.TabIndex = 1;
			this.Park_label.Text = "Выбранный парк:";
			this.Park_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Park_Box.FormattingEnabled = true;
			this.Park_Box.Location = new System.Drawing.Point(11, 36);
			this.Park_Box.Name = "Park_Box";
			this.Park_Box.Size = new System.Drawing.Size(136, 21);
			this.Park_Box.TabIndex = 0;
			this.Park_Box.SelectedIndexChanged += new System.EventHandler(Park_Box_SelectedIndexChanged);
			this.Refresh_Timer.Enabled = true;
			this.Refresh_Timer.Interval = 30;
			this.Refresh_Timer.Tick += new System.EventHandler(Refresh_Timer_Tick);
			this.Sizable_Panel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.Sizable_Panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.Sizable_Panel.Controls.Add(this.renderPanel);
			this.Sizable_Panel.Location = new System.Drawing.Point(0, 28);
			this.Sizable_Panel.Name = "Sizable_Panel";
			this.Sizable_Panel.Size = new System.Drawing.Size(1012, 1073);
			this.Sizable_Panel.TabIndex = 3;
			this.Sizable_Panel.SizeChanged += new System.EventHandler(RefreshPanelSize);
			this.openFileDialog.DefaultExt = "city";
			this.openFileDialog.Filter = "Города Trancity (*.city)|*.city|Все файлы (*.*)|*.*";
			this.openFileDialog.InitialDirectory = "..\\Cities";
			this.openFileDialog.Title = "Открыть город";
			this.saveFileDialog.DefaultExt = "city";
			this.saveFileDialog.Filter = "Города Trancity (*.city)|*.city";
			this.saveFileDialog.InitialDirectory = "..\\Cities";
			this.saveFileDialog.Title = "Сохранить город";
			this.edit_panel.Dock = System.Windows.Forms.DockStyle.Right;
			this.edit_panel.Location = new System.Drawing.Point(850, 28);
			this.edit_panel.Name = "edit_panel";
			this.edit_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.edit_panel.Size = new System.Drawing.Size(158, 1034);
			this.edit_panel.TabIndex = 4;
			this.narad_panel.Controls.Add(this.RollingStockBox);
			this.narad_panel.Controls.Add(this.Transport_label);
			this.narad_panel.Controls.Add(this.Narad_Runs_Time2_Box);
			this.narad_panel.Controls.Add(this.Narad_Runs_Time1_Box);
			this.narad_panel.Controls.Add(this.Narad_Runs_Time2_label);
			this.narad_panel.Controls.Add(this.Narad_Runs_Time1_label);
			this.narad_panel.Controls.Add(this.Narad_Runs_label);
			this.narad_panel.Controls.Add(this.Narad_Park_label);
			this.narad_panel.Controls.Add(this.Narad_Runs_Run_label);
			this.narad_panel.Controls.Add(this.Narad_Name_label);
			this.narad_panel.Controls.Add(this.Narad_Name_Box);
			this.narad_panel.Controls.Add(this.Narad_Runs_Remove_Button);
			this.narad_panel.Controls.Add(this.Narad_Remove_Button);
			this.narad_panel.Controls.Add(this.Narad_Runs_Add_Button);
			this.narad_panel.Controls.Add(this.Narad_Add_Button);
			this.narad_panel.Controls.Add(this.Narad_ChangeName_Button);
			this.narad_panel.Controls.Add(this.Narad_label);
			this.narad_panel.Controls.Add(this.Narad_Park_Box);
			this.narad_panel.Controls.Add(this.Narad_Runs_Run_Box);
			this.narad_panel.Controls.Add(this.Narad_Runs_Box);
			this.narad_panel.Controls.Add(this.Narad_Box);
			this.narad_panel.Dock = System.Windows.Forms.DockStyle.Right;
			this.narad_panel.Location = new System.Drawing.Point(692, 28);
			this.narad_panel.Name = "narad_panel";
			this.narad_panel.Padding = new System.Windows.Forms.Padding(8, 20, 8, 0);
			this.narad_panel.Size = new System.Drawing.Size(158, 1034);
			this.narad_panel.TabIndex = 5;
			this.narad_panel.Visible = false;
			this.RollingStockBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RollingStockBox.FormattingEnabled = true;
			this.RollingStockBox.Location = new System.Drawing.Point(11, 550);
			this.RollingStockBox.Name = "RollingStockBox";
			this.RollingStockBox.Size = new System.Drawing.Size(136, 21);
			this.RollingStockBox.TabIndex = 9;
			this.RollingStockBox.SelectedIndexChanged += new System.EventHandler(RollingStockBox_SelectedIndexChanged);
			this.Transport_label.AutoSize = true;
			this.Transport_label.Location = new System.Drawing.Point(11, 534);
			this.Transport_label.Name = "Transport_label";
			this.Transport_label.Size = new System.Drawing.Size(121, 13);
			this.Transport_label.TabIndex = 8;
			this.Transport_label.Text = "Подвижной состав:";
			this.Narad_Runs_Time2_Box.Enabled = false;
			this.Narad_Runs_Time2_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Narad_Runs_Time2_Box.Hours = 0;
			this.Narad_Runs_Time2_Box.Location = new System.Drawing.Point(11, 495);
			this.Narad_Runs_Time2_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Narad_Runs_Time2_Box.Minutes = 0;
			this.Narad_Runs_Time2_Box.Name = "Narad_Runs_Time2_Box";
			this.Narad_Runs_Time2_Box.Seconds = 0;
			this.Narad_Runs_Time2_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Runs_Time2_Box.TabIndex = 7;
			this.Narad_Runs_Time2_Box.Time = new System.DateTime(0L);
			this.Narad_Runs_Time2_Box.Time_Minutes = 0;
			this.Narad_Runs_Time2_Box.Time_Seconds = 0;
			this.Narad_Runs_Time2_Box.ViewSeconds = false;
			this.Narad_Runs_Time1_Box.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.Narad_Runs_Time1_Box.Hours = 0;
			this.Narad_Runs_Time1_Box.Location = new System.Drawing.Point(11, 455);
			this.Narad_Runs_Time1_Box.MinimumSize = new System.Drawing.Size(40, 21);
			this.Narad_Runs_Time1_Box.Minutes = 0;
			this.Narad_Runs_Time1_Box.Name = "Narad_Runs_Time1_Box";
			this.Narad_Runs_Time1_Box.Seconds = 0;
			this.Narad_Runs_Time1_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Runs_Time1_Box.TabIndex = 7;
			this.Narad_Runs_Time1_Box.Time = new System.DateTime(0L);
			this.Narad_Runs_Time1_Box.Time_Minutes = 0;
			this.Narad_Runs_Time1_Box.Time_Seconds = 0;
			this.Narad_Runs_Time1_Box.ViewSeconds = false;
			this.Narad_Runs_Time1_Box.TimeChanged += new System.EventHandler(Narad_Runs_Time1_Box_TimeChanged);
			this.Narad_Runs_Time2_label.AutoSize = true;
			this.Narad_Runs_Time2_label.Location = new System.Drawing.Point(11, 479);
			this.Narad_Runs_Time2_label.Name = "Narad_Runs_Time2_label";
			this.Narad_Runs_Time2_label.Size = new System.Drawing.Size(108, 13);
			this.Narad_Runs_Time2_label.TabIndex = 4;
			this.Narad_Runs_Time2_label.Text = "Время прибытия:";
			this.Narad_Runs_Time1_label.AutoSize = true;
			this.Narad_Runs_Time1_label.Location = new System.Drawing.Point(11, 439);
			this.Narad_Runs_Time1_label.Name = "Narad_Runs_Time1_label";
			this.Narad_Runs_Time1_label.Size = new System.Drawing.Size(127, 13);
			this.Narad_Runs_Time1_label.TabIndex = 4;
			this.Narad_Runs_Time1_label.Text = "Время отправления:";
			this.Narad_Runs_label.AutoSize = true;
			this.Narad_Runs_label.Location = new System.Drawing.Point(11, 280);
			this.Narad_Runs_label.Name = "Narad_Runs_label";
			this.Narad_Runs_label.Size = new System.Drawing.Size(48, 13);
			this.Narad_Runs_label.TabIndex = 4;
			this.Narad_Runs_label.Text = "Рейсы:";
			this.Narad_Park_label.AutoSize = true;
			this.Narad_Park_label.Location = new System.Drawing.Point(11, 217);
			this.Narad_Park_label.Name = "Narad_Park_label";
			this.Narad_Park_label.Size = new System.Drawing.Size(41, 13);
			this.Narad_Park_label.TabIndex = 4;
			this.Narad_Park_label.Text = "Парк:";
			this.Narad_Runs_Run_label.AutoSize = true;
			this.Narad_Runs_Run_label.Location = new System.Drawing.Point(11, 391);
			this.Narad_Runs_Run_label.Name = "Narad_Runs_Run_label";
			this.Narad_Runs_Run_label.Size = new System.Drawing.Size(90, 13);
			this.Narad_Runs_Run_label.TabIndex = 4;
			this.Narad_Runs_Run_label.Text = "Трасса рейса:";
			this.Narad_Name_label.AutoSize = true;
			this.Narad_Name_label.Location = new System.Drawing.Point(11, 131);
			this.Narad_Name_label.Name = "Narad_Name_label";
			this.Narad_Name_label.Size = new System.Drawing.Size(115, 13);
			this.Narad_Name_label.TabIndex = 4;
			this.Narad_Name_label.Text = "Название наряда:";
			this.Narad_Name_Box.Location = new System.Drawing.Point(11, 147);
			this.Narad_Name_Box.Name = "Narad_Name_Box";
			this.Narad_Name_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Name_Box.TabIndex = 3;
			this.Narad_Name_Box.ModifiedChanged += new System.EventHandler(Narad_Name_Box_ModifiedChanged);
			this.Narad_Runs_Remove_Button.Location = new System.Drawing.Point(11, 352);
			this.Narad_Runs_Remove_Button.Name = "Narad_Runs_Remove_Button";
			this.Narad_Runs_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Narad_Runs_Remove_Button.TabIndex = 2;
			this.Narad_Runs_Remove_Button.Text = "Удалить рейс";
			this.Narad_Runs_Remove_Button.UseVisualStyleBackColor = true;
			this.Narad_Runs_Remove_Button.Click += new System.EventHandler(Narad_Runs_Remove_Button_Click);
			this.Narad_Remove_Button.Location = new System.Drawing.Point(11, 92);
			this.Narad_Remove_Button.Name = "Narad_Remove_Button";
			this.Narad_Remove_Button.Size = new System.Drawing.Size(136, 23);
			this.Narad_Remove_Button.TabIndex = 2;
			this.Narad_Remove_Button.Text = "Удалить наряд";
			this.Narad_Remove_Button.UseVisualStyleBackColor = true;
			this.Narad_Remove_Button.Click += new System.EventHandler(Narad_Remove_Button_Click);
			this.Narad_Runs_Add_Button.Location = new System.Drawing.Point(11, 323);
			this.Narad_Runs_Add_Button.Name = "Narad_Runs_Add_Button";
			this.Narad_Runs_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Narad_Runs_Add_Button.TabIndex = 2;
			this.Narad_Runs_Add_Button.Text = "Добавить рейс";
			this.Narad_Runs_Add_Button.UseVisualStyleBackColor = true;
			this.Narad_Runs_Add_Button.Click += new System.EventHandler(Narad_Runs_Add_Button_Click);
			this.Narad_Add_Button.Location = new System.Drawing.Point(11, 63);
			this.Narad_Add_Button.Name = "Narad_Add_Button";
			this.Narad_Add_Button.Size = new System.Drawing.Size(136, 23);
			this.Narad_Add_Button.TabIndex = 2;
			this.Narad_Add_Button.Text = "Добавить наряд";
			this.Narad_Add_Button.UseVisualStyleBackColor = true;
			this.Narad_Add_Button.Click += new System.EventHandler(Narad_Add_Button_Click);
			this.Narad_ChangeName_Button.Location = new System.Drawing.Point(11, 174);
			this.Narad_ChangeName_Button.Name = "Narad_ChangeName_Button";
			this.Narad_ChangeName_Button.Size = new System.Drawing.Size(136, 23);
			this.Narad_ChangeName_Button.TabIndex = 2;
			this.Narad_ChangeName_Button.Text = "Изменить название";
			this.Narad_ChangeName_Button.UseVisualStyleBackColor = true;
			this.Narad_ChangeName_Button.Click += new System.EventHandler(Narad_ChangeName_Button_Click);
			this.Narad_label.AutoSize = true;
			this.Narad_label.Location = new System.Drawing.Point(11, 20);
			this.Narad_label.Name = "Narad_label";
			this.Narad_label.Size = new System.Drawing.Size(58, 13);
			this.Narad_label.TabIndex = 1;
			this.Narad_label.Text = "Наряды:";
			this.Narad_Park_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Narad_Park_Box.FormattingEnabled = true;
			this.Narad_Park_Box.Location = new System.Drawing.Point(11, 233);
			this.Narad_Park_Box.Name = "Narad_Park_Box";
			this.Narad_Park_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Park_Box.TabIndex = 0;
			this.Narad_Park_Box.SelectedIndexChanged += new System.EventHandler(Narad_Park_Box_SelectedIndexChanged);
			this.Narad_Runs_Run_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Narad_Runs_Run_Box.FormattingEnabled = true;
			this.Narad_Runs_Run_Box.Location = new System.Drawing.Point(11, 407);
			this.Narad_Runs_Run_Box.Name = "Narad_Runs_Run_Box";
			this.Narad_Runs_Run_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Runs_Run_Box.TabIndex = 0;
			this.Narad_Runs_Run_Box.SelectedIndexChanged += new System.EventHandler(Narad_Runs_Run_Box_SelectedIndexChanged);
			this.Narad_Runs_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Narad_Runs_Box.FormattingEnabled = true;
			this.Narad_Runs_Box.Location = new System.Drawing.Point(11, 296);
			this.Narad_Runs_Box.Name = "Narad_Runs_Box";
			this.Narad_Runs_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Runs_Box.TabIndex = 0;
			this.Narad_Runs_Box.SelectedIndexChanged += new System.EventHandler(Narad_Runs_Box_SelectedIndexChanged);
			this.Narad_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Narad_Box.FormattingEnabled = true;
			this.Narad_Box.Location = new System.Drawing.Point(11, 36);
			this.Narad_Box.Name = "Narad_Box";
			this.Narad_Box.Size = new System.Drawing.Size(136, 21);
			this.Narad_Box.TabIndex = 0;
			this.Narad_Box.SelectedIndexChanged += new System.EventHandler(Narad_Box_SelectedIndexChanged);
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			base.ClientSize = new System.Drawing.Size(1008, 1062);
			base.Controls.Add(this.statusBar);
			base.Controls.Add(this.narad_panel);
			base.Controls.Add(this.park_panel);
			base.Controls.Add(this.route_panel);
			base.Controls.Add(this.splines_panel);
			base.Controls.Add(this.object_panel);
			base.Controls.Add(this.stops_panel);
			base.Controls.Add(this.signals_panel);
			base.Controls.Add(this.svetofor_panel);
			base.Controls.Add(this.edit_panel);
			base.Controls.Add(this.Sizable_Panel);
			base.Controls.Add(this.toolBar);
			this.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			base.KeyPreview = true;
			base.Menu = this.mainMenu;
			base.Name = "Editor";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Transedit";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.Activated += new System.EventHandler(EditorActivated);
			base.Closing += new System.ComponentModel.CancelEventHandler(Editor_Form_Closing);
			base.Deactivate += new System.EventHandler(EditorDeactivate);
			base.Load += new System.EventHandler(Editor_Form_Load);
			base.KeyDown += new System.Windows.Forms.KeyEventHandler(Editor_Form_KeyDown);
			base.KeyUp += new System.Windows.Forms.KeyEventHandler(Editor_Form_KeyUp);
			((System.ComponentModel.ISupportInitialize)this.Cursor_x_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Cursor_y_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel1).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_x1_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_y1_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Angle1_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel2).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_x2_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Coord_y2_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Angle2_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel3).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Length_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Radius_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Angle_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Wide0_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Wide1_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Height0_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Height1_Status).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel4).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Maschtab).EndInit();
			((System.ComponentModel.ISupportInitialize)this.SeparatorPanel5).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Ugol).EndInit();
			this.object_panel.ResumeLayout(false);
			this.object_panel.PerformLayout();
			this.route_panel.ResumeLayout(false);
			this.route_panel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.Route_Runs_ToParkIndex_UpDown).EndInit();
			this.svetofor_panel.ResumeLayout(false);
			this.svetofor_panel.PerformLayout();
			this.splines_panel.ResumeLayout(false);
			this.splines_panel.PerformLayout();
			this.signals_panel.ResumeLayout(false);
			this.signals_panel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.Signals_Bound_UpDown).EndInit();
			this.stops_panel.ResumeLayout(false);
			this.stops_panel.PerformLayout();
			this.TypeOfTransportBox.ResumeLayout(false);
			this.TypeOfTransportBox.PerformLayout();
			this.park_panel.ResumeLayout(false);
			this.park_panel.PerformLayout();
			this.Sizable_Panel.ResumeLayout(false);
			this.narad_panel.ResumeLayout(false);
			this.narad_panel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}

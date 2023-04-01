namespace Trancity
{
	public class Order
	{
		public Route маршрут;

		public string номер;

		public Парк парк;

		public bool поВыходным = true;

		public bool поРабочим = true;

		public Trip[] рейсы = new Trip[0];

		public string transport;

		public Order(Парк парк, Route маршрут, string номер, string transport)
		{
			this.парк = парк;
			this.маршрут = маршрут;
			this.номер = номер;
			this.transport = transport;
		}
	}
}

namespace Trancity
{
	public class Светофорный_сигнал
	{
		private Road froad;

		public double расстояние;

		public Сигналы сигнал;

		public Road дорога
		{
			get
			{
				return froad;
			}
			set
			{
				if (froad != null)
				{
					froad.objects.Remove(this);
				}
				froad = value;
				if (value != null)
				{
					froad.objects.Add(this);
				}
			}
		}

		public Светофорный_сигнал(Road дорога, double расстояние)
		{
			this.дорога = дорога;
			this.расстояние = расстояние;
		}
	}
}

using System.Collections.Generic;

namespace Trancity
{
	public class Светофорная_система
	{
		public double время_зелёного;

		public double время_переключения_на_зелёный;

		public double начало_работы;

		public double окончание_работы;

		public List<Светофорный_сигнал> светофорные_сигналы = new List<Светофорный_сигнал>();

		public List<Светофор> светофоры = new List<Светофор>();

		public double цикл = 1.0;

		private bool green;

		private bool yellow;

		private bool red;

		public void CreateMesh()
		{
			foreach (Светофор item in светофоры)
			{
				item.CreateMesh();
			}
		}

		public void Render()
		{
			foreach (Светофор item in светофоры)
			{
				item.Custom_render(green, yellow, red);
			}
		}

		public void Обновить(World мир)
		{
			green = false;
			yellow = false;
			red = false;
			Сигналы сигнал = Сигналы.Нет;
			if (начало_работы == 0.0 && окончание_работы == 86400.0)
			{
				начало_работы = 10800.0;
				окончание_работы = 97200.0;
			}
			if (мир.time < начало_работы || мир.time >= окончание_работы)
			{
				yellow = мир.time % 1.0 < 0.5;
			}
			else
			{
				double num = (мир.time - начало_работы - время_переключения_на_зелёный) % цикл;
				if (num < время_зелёного)
				{
					сигнал = Сигналы.Зелёный;
					green = num < время_зелёного - 4.0 || num % 1.0 >= 0.5;
				}
				else if (num < время_зелёного + 2.0)
				{
					сигнал = Сигналы.Жёлтый;
					yellow = true;
				}
				else
				{
					сигнал = Сигналы.Красный;
					red = true;
					if (num >= цикл - 2.0)
					{
						yellow = true;
					}
				}
			}
			foreach (Светофорный_сигнал item in светофорные_сигналы)
			{
				item.сигнал = сигнал;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Trancity
{
	public class Stop : BaseStop, IComparable<Stop>
	{
		public class Расписание
		{
			public int[] поВыходным = new int[0];

			public int[] поРабочим = new int[0];

			public bool Ежедневное
			{
				get
				{
					if (поРабочим.Length != поВыходным.Length)
					{
						return false;
					}
					for (int i = 0; i < поРабочим.Length; i++)
					{
						if (поРабочим[i] != поВыходным[i])
						{
							return false;
						}
					}
					return true;
				}
			}

			public string StrПоВыходным(int index)
			{
				int num = поВыходным[index] / 3600 % 24;
				int num2 = поВыходным[index] / 60 % 60;
				return num + ":" + num2.ToString("00");
			}

			public string StrПоРабочим(int index)
			{
				int num = поРабочим[index] / 3600 % 24;
				int num2 = поРабочим[index] / 60 % 60;
				return num + ":" + num2.ToString("00");
			}

			public string ИнтервалыПоВыходным(int timeIndex)
			{
				int[] array = new int[5];
				array[4] = поВыходным.Length - 1;
				array[0] = 0;
				array[1] = array[4];
				array[2] = array[4];
				array[3] = array[4];
				for (int i = 0; i < поВыходным.Length; i++)
				{
					if (array[1] == array[4] && поВыходным[i] >= 36000)
					{
						array[1] = i;
					}
					if (array[2] == array[4] && поВыходным[i] >= 57600)
					{
						array[2] = i;
					}
					if (array[3] == array[4] && поВыходным[i] >= 72000)
					{
						array[3] = i;
					}
				}
				int[] array2 = new int[4];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[j + 1] - array[j];
				}
				if (array2[timeIndex] == 0)
				{
					return "—";
				}
				int num = array[timeIndex + 1];
				if (timeIndex + 2 < 5 && array[timeIndex + 2] == num)
				{
					num--;
					if (array2[timeIndex] > 1)
					{
						array2[timeIndex]--;
					}
				}
				return (int)Math.Ceiling((double)(поВыходным[num] - поВыходным[array[timeIndex]]) / 60.0 / (double)array2[timeIndex]) + " мин.";
			}

			public string ИнтервалыПоРабочим(int timeIndex)
			{
				int[] array = new int[5];
				array[4] = поРабочим.Length - 1;
				array[0] = 0;
				array[1] = array[4];
				array[2] = array[4];
				array[3] = array[4];
				for (int i = 0; i < поРабочим.Length; i++)
				{
					if (array[1] == array[4] && поРабочим[i] >= 36000)
					{
						array[1] = i;
					}
					if (array[2] == array[4] && поРабочим[i] >= 57600)
					{
						array[2] = i;
					}
					if (array[3] == array[4] && поРабочим[i] >= 72000)
					{
						array[3] = i;
					}
				}
				int[] array2 = new int[4];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array[j + 1] - array[j];
				}
				if (array2[timeIndex] == 0)
				{
					return "—";
				}
				int num = array[timeIndex + 1];
				if (timeIndex + 2 < 5 && array[timeIndex + 2] == num)
				{
					num--;
					if (array2[timeIndex] > 1)
					{
						array2[timeIndex]--;
					}
				}
				return (int)Math.Ceiling((double)(поРабочим[num] - поРабочим[array[timeIndex]]) / 60.0 / (double)array2[timeIndex]) + " мин.";
			}
		}

		private Route[] _маршруты;

		public string название;

		public static bool неЗагружаемКартинки;

		private Расписание[] _расписания;

		public Road[] частьПути;

		public Stop(string mname, TypeOfTransport видТранспорта, Road дорога, double расстояние)
			: base(mname)
		{
			частьПути = new Road[0];
			название = "";
			_маршруты = new Route[0];
			_расписания = new Расписание[0];
			typeOfTransport = видТранспорта;
			base.road = дорога;
			distance = расстояние;
		}

		public Stop(string mname, TypeOfTransport видТранспорта, string название, Road дорога, double расстояние)
			: this(mname, видТранспорта, дорога, расстояние)
		{
			this.название = название;
		}

		public Stop(string mname, TypeOfTransport видТранспорта, Road дорога, double расстояние, bool служебная)
			: this(mname, видТранспорта, дорога, расстояние)
		{
			serviceStop = служебная;
		}

		private void DrawCenteredString(Graphics graphics, string s, Font font, Brush brush, float x, float y)
		{
			graphics.DrawString(s, font, brush, x - 0.5f * graphics.MeasureString(s, font).Width, y);
		}

		public void ОбновитьКартинку()
		{
			string text = model.FindStringArg("tram_tex", string.Empty);
			string text2 = "";
			if (typeOfTransport[1])
			{
				text2 = model.FindStringArg("trol_tex", string.Empty);
			}
			if (typeOfTransport[2])
			{
				text2 = model.FindStringArg("bus_tex", string.Empty);
			}
			if (typeOfTransport[1] && typeOfTransport[2])
			{
				text2 = model.FindStringArg("trol_bus_tex", string.Empty);
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			if (неЗагружаемКартинки)
			{
				if (!(text2 != text))
				{
					return;
				}
				for (int i = 0; i < _meshTextures.Length; i++)
				{
					if (!string.IsNullOrEmpty(_meshTextureFilenames[i]) && !(_meshTextureFilenames[i].ToLower() != text.ToLower()))
					{
						LoadTexture(i, meshDir + text2);
					}
				}
				return;
			}
			List<Route>[] array = new List<Route>[Math.Max(1, (_маршруты.Length + 3) / 4)];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new List<Route>();
			}
			Route[] маршруты = _маршруты;
			foreach (Route item in маршруты)
			{
				int num;
				do
				{
					num = Cheats._random.Next(array.Length);
				}
				while (array[num].Count >= 4);
				array[num].Add(item);
			}
			string[] array2 = new string[4 * array.Length];
			Route[] array3 = new Route[4 * array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				for (int m = 0; m < array[l].Count; m++)
				{
					int num2;
					do
					{
						num2 = Cheats._random.Next(4 * l, 4 * l + 4);
					}
					while (array2[num2] != null);
					array2[num2] = array[l][m].number;
				}
				if (array[l].Count <= 2)
				{
					for (int n = 0; n < array[l].Count; n++)
					{
						int num3;
						do
						{
							num3 = Cheats._random.Next(4 * l, 4 * l + 2);
						}
						while (array3[num3] != null);
						array3[num3] = array[l][n];
					}
					for (int num4 = 0; num4 < array[l].Count; num4++)
					{
						int num5;
						do
						{
							num5 = Cheats._random.Next(4 * l + 2, 4 * l + 4);
						}
						while (array3[num5] != null);
						array3[num5] = array[l][num4];
					}
					continue;
				}
				for (int num6 = 0; num6 < array[l].Count; num6++)
				{
					int num7;
					do
					{
						num7 = Cheats._random.Next(4 * l, 4 * l + 4);
					}
					while (array3[num7] != null);
					array3[num7] = array[l][num6];
				}
			}
			for (int num8 = 0; num8 < array2.Length; num8++)
			{
				if (array2[num8] == null)
				{
					array2[num8] = "";
				}
			}
			Font font = new Font("Verdana", 7f);
			Font font2 = new Font("Verdana", 12f, FontStyle.Bold);
			Font font3 = new Font("Verdana", 8f, FontStyle.Bold);
			Font font4 = new Font("Verdana", 5.5f);
			Font font5 = new Font("Verdana", 4f);
			Brush brush = new SolidBrush(Color.Black);
			Brush brush2 = new SolidBrush(Color.FromArgb(-13749870));
			Brush brush3 = new SolidBrush(Color.FromArgb(-1237980));
			Pen pen = new Pen(Color.Black);
			int num9 = 0;
			int num10 = 0;
			while (num9 < _meshTextures.Length)
			{
				if (string.IsNullOrEmpty(_meshTextureFilenames[num9]) || _meshTextureFilenames[num9].ToLower() != (meshDir + text).ToLower())
				{
					num10--;
				}
				else
				{
					Image image = new Bitmap(meshDir + text2);
					Graphics graphics = Graphics.FromImage(image);
					DrawCenteredString(graphics, название, font, brush, 187f, 30f);
					DrawCenteredString(graphics, array2[num10 / 2 * 4], font2, brush, 230f, 70f);
					DrawCenteredString(graphics, array2[num10 / 2 * 4 + 1], font2, brush, 310f, 70f);
					DrawCenteredString(graphics, array2[num10 / 2 * 4 + 2], font2, brush, 230f, 110f);
					DrawCenteredString(graphics, array2[num10 / 2 * 4 + 3], font2, brush, 310f, 110f);
					for (int num11 = 0; num11 < 2; num11++)
					{
						Route route = array3[num10 * 2 + num11];
						if (route == null)
						{
							continue;
						}
						DrawCenteredString(graphics, route.number, font3, brush, (float)(186 * num11) + 33.5f, 179f);
						int num12 = 0;
						for (int num13 = 0; num13 < _маршруты.Length; num13++)
						{
							if (_маршруты[num13] == route)
							{
								num12 = num13;
								break;
							}
						}
						Расписание расписание = _расписания[num12];
						int num14 = 0;
						num14 += (int)Math.Ceiling((double)расписание.поРабочим.Length / 10.0);
						if (!расписание.Ежедневное)
						{
							num14 += (int)Math.Ceiling((double)расписание.поВыходным.Length / 10.0);
						}
						if (num14 <= 5)
						{
							DrawCenteredString(graphics, "Расписание", font4, brush, (float)(186 * num11) + 126.5f, 183f);
							if (!расписание.Ежедневное)
							{
								if (расписание.поВыходным.Length == 0)
								{
									graphics.DrawString("только по будн.", font5, brush2, 186 * num11 + 5, 209f);
								}
								else if (расписание.поРабочим.Length != 0)
								{
									graphics.DrawString("по будн.", font5, brush2, 186 * num11 + 5, 209f);
								}
							}
							else
							{
								graphics.DrawString("ежедневно", font5, brush, 186 * num11 + 5, 209f);
							}
							for (int num15 = 0; num15 < расписание.поРабочим.Length; num15++)
							{
								DrawCenteredString(graphics, расписание.StrПоРабочим(num15), font5, brush, 186 * num11 + 18 + 37 * (num15 / 10), 222 + 12 * (num15 % 10));
							}
							if (расписание.Ежедневное || расписание.поВыходным.Length == 0)
							{
								continue;
							}
							if (расписание.поРабочим.Length == 0)
							{
								graphics.DrawString("только по вых.", font5, brush3, 186 * num11 + 5, 209f);
								for (int num16 = 0; num16 < расписание.поВыходным.Length; num16++)
								{
									DrawCenteredString(graphics, расписание.StrПоВыходным(num16), font5, brush, 186 * num11 + 18 + 37 * (num16 / 10), 222 + 12 * (num16 % 10));
								}
								continue;
							}
							int num17 = 0;
							if (расписание.поРабочим.Length <= 20 && расписание.поВыходным.Length <= 20)
							{
								num17 = 93;
							}
							else
							{
								int num18 = (int)Math.Ceiling((double)расписание.поРабочим.Length / 10.0);
								if (num18 == 1 && расписание.поВыходным.Length <= 30)
								{
									num18 = 2;
								}
								num17 = 37 * num18;
							}
							graphics.DrawLine(pen, 186 * num11 + num17 - 1, 209, 186 * num11 + num17 - 1, 341);
							graphics.DrawString("по вых.", font5, brush3, 186 * num11 + num17 + 5, 209f);
							for (int num19 = 0; num19 < расписание.поВыходным.Length; num19++)
							{
								DrawCenteredString(graphics, расписание.StrПоВыходным(num19), font5, brush, 186 * num11 + num17 + 18 + 37 * (num19 / 10), 222 + 12 * (num19 % 10));
							}
							continue;
						}
						DrawCenteredString(graphics, "Режим работы", font4, brush, (float)(186 * num11) + 126.5f, 183f);
						graphics.DrawLine(pen, 186 * num11 + 67, 209, 186 * num11 + 67, 341);
						graphics.DrawLine(pen, 186 * num11, 252, 186 * num11 + 184, 252);
						graphics.DrawString("нач.движ.", font5, brush, 186 * num11, 225f);
						graphics.DrawString("оконч.движ.", font5, brush, 186 * num11, 237f);
						graphics.DrawString("06:00-10:00", font5, brush, 186 * num11, 285f);
						graphics.DrawString("10:00-16:00", font5, brush, 186 * num11, 297f);
						graphics.DrawString("16:00-20:00", font5, brush, 186 * num11, 309f);
						graphics.DrawString("20:00-00:00", font5, brush, 186 * num11, 321f);
						DrawCenteredString(graphics, "интервал", font5, brush, (float)(186 * num11) + 126.5f, 253f);
						DrawCenteredString(graphics, "движения", font5, brush, (float)(186 * num11) + 126.5f, 265f);
						if (!расписание.Ежедневное && расписание.поРабочим.Length != 0 && расписание.поВыходным.Length != 0)
						{
							DrawCenteredString(graphics, "по будн.", font5, brush2, 186 * num11 + 97, 209f);
							DrawCenteredString(graphics, расписание.StrПоРабочим(0), font5, brush, 186 * num11 + 97, 225f);
							DrawCenteredString(graphics, расписание.StrПоРабочим(расписание.поРабочим.Length - 1), font5, brush, 186 * num11 + 97, 237f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(0), font5, brush, 186 * num11 + 97, 285f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(1), font5, brush, 186 * num11 + 97, 297f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(2), font5, brush, 186 * num11 + 97, 309f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(3), font5, brush, 186 * num11 + 97, 321f);
							DrawCenteredString(graphics, "по вых.", font5, brush3, 186 * num11 + 156, 209f);
							DrawCenteredString(graphics, расписание.StrПоВыходным(0), font5, brush, 186 * num11 + 156, 225f);
							DrawCenteredString(graphics, расписание.StrПоВыходным(расписание.поВыходным.Length - 1), font5, brush, 186 * num11 + 156, 237f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(0), font5, brush, 186 * num11 + 156, 285f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(1), font5, brush, 186 * num11 + 156, 297f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(2), font5, brush, 186 * num11 + 156, 309f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(3), font5, brush, 186 * num11 + 156, 321f);
						}
						else if (расписание.Ежедневное || расписание.поВыходным.Length == 0)
						{
							DrawCenteredString(graphics, расписание.Ежедневное ? "ежедневно" : "только по будн.", font5, brush, (float)(186 * num11) + 126.5f, 209f);
							DrawCenteredString(graphics, расписание.StrПоРабочим(0), font5, brush, (float)(186 * num11) + 126.5f, 225f);
							DrawCenteredString(graphics, расписание.StrПоРабочим(расписание.поРабочим.Length - 1), font5, brush, (float)(186 * num11) + 126.5f, 237f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(0), font5, brush, (float)(186 * num11) + 126.5f, 285f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(1), font5, brush, (float)(186 * num11) + 126.5f, 297f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(2), font5, brush, (float)(186 * num11) + 126.5f, 309f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоРабочим(3), font5, brush, (float)(186 * num11) + 126.5f, 321f);
						}
						else
						{
							DrawCenteredString(graphics, "только по вых.", font5, brush3, (float)(186 * num11) + 126.5f, 209f);
							DrawCenteredString(graphics, расписание.StrПоВыходным(0), font5, brush, (float)(186 * num11) + 126.5f, 225f);
							DrawCenteredString(graphics, расписание.StrПоВыходным(расписание.поВыходным.Length - 1), font5, brush, (float)(186 * num11) + 126.5f, 237f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(0), font5, brush, (float)(186 * num11) + 126.5f, 285f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(1), font5, brush, (float)(186 * num11) + 126.5f, 297f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(2), font5, brush, (float)(186 * num11) + 126.5f, 309f);
							DrawCenteredString(graphics, расписание.ИнтервалыПоВыходным(3), font5, brush, (float)(186 * num11) + 126.5f, 321f);
						}
					}
					Stream stream = new MemoryStream();
					image.Save(stream, ImageFormat.Bmp);
					stream.Seek(0L, SeekOrigin.Begin);
					LoadTextureFromStream(num9, stream);
				}
				num9++;
				num10++;
			}
		}

		public void ОбновитьМаршруты(Route[] всеМаршруты)
		{
			List<Route> list = new List<Route>();
			foreach (Route route in всеМаршруты)
			{
				if (!typeOfTransport[route.typeOfTransport])
				{
					continue;
				}
				foreach (Trip trip2 in route.trips)
				{
					if (ПутьПодходит(trip2.pathes))
					{
						list.Add(route);
						break;
					}
				}
			}
			_маршруты = list.ToArray();
			_расписания = new Расписание[_маршруты.Length];
			for (int j = 0; j < _маршруты.Length; j++)
			{
				_расписания[j] = new Расписание();
				List<int> list2 = new List<int>();
				List<int> list3 = new List<int>();
				Order[] orders = _маршруты[j].orders;
				foreach (Order order in orders)
				{
					Trip[] рейсы = order.рейсы;
					foreach (Trip trip in рейсы)
					{
						Road[] array = trip.pathes;
						if (trip.inPark)
						{
							array = new Road[trip.inParkIndex];
							for (int l = 0; l < array.Length; l++)
							{
								array[l] = trip.pathes[l];
							}
						}
						if (!ПутьПодходит(array))
						{
							continue;
						}
						double num = 0.0;
						Road[] array2 = array;
						foreach (Road road in array2)
						{
							num += road.Длина;
						}
						double num2 = 0.0;
						for (int n = 0; n < array.Length - 1; n++)
						{
							if (array[n] == base.road)
							{
								int item = (int)(trip.время_отправления + (trip.время_прибытия - trip.время_отправления) * (num2 + distance) / num);
								if (order.поРабочим)
								{
									list2.Add(item);
								}
								if (order.поВыходным)
								{
									list3.Add(item);
								}
							}
							num2 += array[n].Длина;
						}
					}
				}
				list2.Sort();
				list3.Sort();
				_расписания[j].поРабочим = list2.ToArray();
				_расписания[j].поВыходным = list3.ToArray();
			}
		}

		public bool ПутьПодходит(Road[] arrayПуть)
		{
			List<Road> list = new List<Road>(arrayПуть);
			if (list.Contains(base.road))
			{
				if (частьПути.Length == 0)
				{
					return true;
				}
				if (частьПути.Length == 1)
				{
					if (list.Contains(частьПути[0]))
					{
						return true;
					}
				}
				else
				{
					List<int> list2 = new List<int>();
					int index = 0;
					while (list.IndexOf(частьПути[0], index) >= 0)
					{
						int num = list.IndexOf(частьПути[0], index);
						list2.Add(num);
						index = num + 1;
					}
					foreach (int item in list2)
					{
						bool flag = true;
						for (int i = 0; i < частьПути.Length; i++)
						{
							if (item + i >= list.Count || list[item + i] != частьПути[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public int CompareTo(Stop stop)
		{
			if (stop.road != base.road)
			{
				return 0;
			}
			return Math.Sign(distance - stop.distance);
		}
	}
}

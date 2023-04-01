using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Engine;

namespace Trancity
{
	public static class Модели
	{
		public static readonly List<МодельТранспорта> Автобусы;

		public static readonly List<МодельТранспорта> Троллейбусы;

		public static readonly List<МодельТранспорта> Трамваи;

		static Модели()
		{
			Автобусы = new List<МодельТранспорта>();
			Троллейбусы = new List<МодельТранспорта>();
			Трамваи = new List<МодельТранспорта>();
			XmlDocument xmlDocument = new XmlDocument();
			string[] directories = Directory.GetDirectories(Application.StartupPath + "\\Data\\Transport\\");
			for (int i = 0; i < directories.Length; i++)
			{
				string text = directories[i] + "\\";
				try
				{
					xmlDocument.Load(text + "model.xml");
				}
				catch (Exception)
				{
					Logger.Log("ModelLoader", "model.xml not found in directory " + text);
					continue;
				}
				XmlElement xmlElement = xmlDocument["Trancity"];
				if (xmlElement == null)
				{
					continue;
				}
				XmlElement xmlElement2 = xmlElement["Doors"];
				if (xmlElement2 == null)
				{
					continue;
				}
				МодельТранспорта модельТранспорта = new МодельТранспорта();
				for (int j = 0; j < xmlElement2.ChildNodes.Count; j++)
				{
					XmlNode xmlNode = xmlElement2.ChildNodes[j];
					if (xmlNode != null)
					{
						МодельДверей.Тип тип = (МодельДверей.Тип)Xml.GetDouble(xmlNode["type"]);
						string dir = text + xmlNode["dir"].InnerText;
						string innerText = xmlNode["filename"].InnerText;
						double @double = Xml.GetDouble(xmlNode["length"]);
						double double2 = Xml.GetDouble(xmlNode["height"]);
						double double3 = Xml.GetDouble(xmlNode["width"]);
						if (модельТранспорта.модельДверей != null)
						{
							модельТранспорта.модельДверей.Add(new МодельДверей(тип, dir, innerText, @double, double2, double3));
						}
					}
				}
				XmlElement xmlElement3 = xmlElement["Model"];
				if (xmlElement3 == null)
				{
					continue;
				}
				XmlNode firstChild = xmlElement3.FirstChild;
				if (firstChild == null)
				{
					continue;
				}
				try
				{
					switch (firstChild.Name.ToLower())
					{
					case "trolleybus":
						LoadTrolleybus(модельТранспорта, text, firstChild);
						Троллейбусы.Add(модельТранспорта);
						break;
					case "bus":
						LoadTrolleybus(модельТранспорта, text, firstChild);
						Автобусы.Add(модельТранспорта);
						break;
					case "tramway":
						LoadTramway(модельТранспорта, text, firstChild);
						Трамваи.Add(модельТранспорта);
						break;
					}
				}
				catch (Exception)
				{
					Logger.Log("ModelLoader", "Error in " + text + "model.xml");
				}
			}
		}

		private static void LoadTrolleybus(МодельТранспорта транспорт, string directory1, XmlNode element3)
		{
			транспорт.name = element3["name"].InnerText;
			транспорт.dir = directory1 + element3["dir"].InnerText;
			транспорт.filename = element3["filename"].InnerText;
			транспорт.хвостFilename = LoadStrings(element3["tail_filename"]);
			транспорт.хвостDist1 = LoadDoubles(element3["tail_dist_1"]);
			транспорт.хвостDist2 = LoadDoubles(element3["tail_dist_2"]);
			транспорт.сочленениеFilename = LoadStrings(element3["middle_filename"]);
			транспорт.дополнения = LoadAdditions(element3["additions"]);
			транспорт.количествоДверей = (int)Xml.GetDouble(element3["door_count"]);
			транспорт.двери = LoadDoors(транспорт, element3["doors"]);
			транспорт.радиусКолёс = Xml.GetDouble(element3["wheel_radius"]);
			транспорт.колёсныеПары = LoadWheels(directory1, element3["wheels"]);
			транспорт.штангиDir = directory1 + element3["shtangi_dir"].InnerText;
			транспорт.штангиFilename = element3["shtangi_filename"].InnerText;
			транспорт.штангиПолнаяДлина = Xml.GetDouble(element3["shtangi_full_length"]);
			транспорт.штангиУголMin = Xml.GetDouble(element3["shtangi_angle_min"]);
			транспорт.штанги = LoadShtangi(element3["shtangi"]);
			транспорт.руль = LoadSteering(directory1, element3["steering"]);
			транспорт.ах = LoadStandaloneMotion(element3["standalone_motion"]);
			транспорт.табличка = Load_VPark_Tabl(element3["tabl_v_park"]);
			транспорт.нарядPos = LoadDouble_3DPoint(element3["narad_pos"]);
			транспорт.cameras = LoadCameras(element3["cameras"]);
			try
			{
				транспорт.bsphere = LoadBSphere(element3["bounding_sphere"]);
				транспорт.tails_bsphere = LoadBSpheres(element3["tails_bounding_sphere"]);
			}
			catch
			{
				транспорт.hasnt_bbox = true;
				Logger.Log("ModelLoader", "Bounding spheres not loaded for " + транспорт.name);
			}
			транспорт.занятыеПоложения = LoadDoublePoints(element3["occupied_locations"]);
			транспорт.занятыеПоложенияХвостов = LoadArrayOfDoublePoints(element3["tails_occupied_locations"]);
			транспорт.системаУправления = element3["control_system"].InnerText;
		}

		private static void LoadTramway(МодельТранспорта трамвай, string directory1, XmlNode element3)
		{
			трамвай.name = element3["name"].InnerText;
			трамвай.dir = directory1 + element3["dir"].InnerText;
			трамвай.filename = element3["filename"].InnerText;
			трамвай.tails = LoadTails(element3["tails"]);
			try
			{
				трамвай.сочленения = LoadNewMiddles(element3["middles"]);
			}
			catch
			{
			}
			трамвай.дополнения = LoadAdditions(element3["additions"]);
			трамвай.количествоДверей = (int)Xml.GetDouble(element3["door_count"]);
			трамвай.двери = LoadDoors(трамвай, element3["doors"]);
			трамвай.axisfilename = element3["axis_filename"].InnerText;
			трамвай.telegafilename = element3["telegi_filename"].InnerText;
			трамвай.расстояние_между_осями = Xml.GetDouble(element3["axis_dist"]);
			трамвай.axis_radius = Xml.GetDouble(element3["axis_radius"]);
			трамвай.тележки = LoadBoogies(element3["telegi"]);
			трамвай.пантограф = LoadPantograph(directory1, element3["pantograph"]);
			трамвай.табличка = Load_VPark_Tabl(element3["tabl_v_park"]);
			трамвай.нарядPos = LoadDouble_3DPoint(element3["narad_pos"]);
			трамвай.cameras = LoadCameras(element3["cameras"]);
			try
			{
				трамвай.bsphere = LoadBSphere(element3["bounding_sphere"]);
				трамвай.tails_bsphere = LoadBSpheres(element3["tails_bounding_sphere"]);
			}
			catch
			{
				трамвай.hasnt_bbox = true;
				Logger.Log("ModelLoader", "Bounding spheres not loaded for " + трамвай.name);
			}
			трамвай.занятыеПоложения = LoadDoublePoints(element3["occupied_locations"]);
			трамвай.занятыеПоложенияХвостов = LoadArrayOfDoublePoints(element3["tails_occupied_locations"]);
			трамвай.системаУправления = element3["control_system"].InnerText;
		}

		private static МодельТранспорта.Руль LoadSteering(string directory, XmlNode node)
		{
			if (node != null)
			{
				string d = directory + node["dir"].InnerText;
				string innerText = node["filename"].InnerText;
				double @double = Xml.GetDouble(node["x"]);
				double double2 = Xml.GetDouble(node["y"]);
				double double3 = Xml.GetDouble(node["z"]);
				double double4 = Xml.GetDouble(node["angle"]);
				return new МодельТранспорта.Руль(d, innerText, @double, double2, double3, double4);
			}
			return null;
		}

		private static МодельТранспорта.АХ LoadStandaloneMotion(XmlNode node)
		{
			if (node != null)
			{
				double @double = Xml.GetDouble(node["battery_power"]);
				double double2 = Xml.GetDouble(node["acceleration"]);
				return new МодельТранспорта.АХ(@double, double2);
			}
			return null;
		}

		private static МодельТранспорта.Дополнение[] LoadAdditions(XmlNode items)
		{
			МодельТранспорта.Дополнение[] array = new МодельТранспорта.Дополнение[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				int часть = (int)Xml.GetDouble(xmlNode["part"]);
				string innerText = xmlNode["filename"].InnerText;
				int тип = (int)Xml.GetDouble(xmlNode["type"]);
				array[i] = new МодельТранспорта.Дополнение(часть, innerText, (Transport.Тип_дополнения)тип);
			}
			return array;
		}

		private static DoublePoint[][] LoadArrayOfDoublePoints(XmlNode items)
		{
			DoublePoint[][] array = new DoublePoint[items.ChildNodes.Count][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadDoublePoints(items.ChildNodes[i]);
			}
			return array;
		}

		private static Double3DPoint[][] LoadArrayOfDouble_3DPoints(XmlNode items)
		{
			Double3DPoint[][] array = new Double3DPoint[items.ChildNodes.Count][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadDouble_3DPoints(items.ChildNodes[i]);
			}
			return array;
		}

		private static МодельТранспорта.Дверь[] LoadDoors(МодельТранспорта троллейбус, XmlNode items)
		{
			МодельТранспорта.Дверь[] array = new МодельТранспорта.Дверь[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				int index = (int)Xml.GetDouble(xmlNode["model"]);
				МодельДверей модель = троллейбус.модельДверей[index];
				int часть = (int)Xml.GetDouble(xmlNode["part"]);
				double @double = Xml.GetDouble(xmlNode["x0"]);
				double double2 = Xml.GetDouble(xmlNode["x1"]);
				double double3 = Xml.GetDouble(xmlNode["y0"]);
				double double4 = Xml.GetDouble(xmlNode["y1"]);
				double double5 = Xml.GetDouble(xmlNode["z0"]);
				double double6 = Xml.GetDouble(xmlNode["z1"]);
				bool правые = Xml.GetDouble(xmlNode["right"]) != 0.0;
				bool дверьВодителя = Xml.GetDouble(xmlNode["driver"]) != 0.0;
				int номер = (int)Xml.GetDouble(xmlNode["index"]);
				array[i] = new МодельТранспорта.Дверь(модель, часть, @double, double5, double2, double6, double3, double4, правые, дверьВодителя, номер);
			}
			return array;
		}

		private static Double3DPoint LoadDouble_3DPoint(XmlNode items)
		{
			return new Double3DPoint(Xml.GetDouble(items["x"]), Xml.GetDouble(items["y"]), Xml.GetDouble(items["z"]));
		}

		private static Double3DPoint[] LoadDouble_3DPoints(XmlNode items)
		{
			Double3DPoint[] array = new Double3DPoint[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadDouble_3DPoint(items.ChildNodes[i]);
			}
			return array;
		}

		private static DoublePoint LoadDoublePoint(XmlNode items)
		{
			return new DoublePoint(Xml.GetDouble(items["x"]), Xml.GetDouble(items["y"]));
		}

		private static DoublePoint[] LoadDoublePoints(XmlNode items)
		{
			DoublePoint[] array = new DoublePoint[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadDoublePoint(items.ChildNodes[i]);
			}
			return array;
		}

		private static double[] LoadDoubles(XmlNode items)
		{
			double[] array = new double[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Xml.GetDouble(items.ChildNodes[i]);
			}
			return array;
		}

		private static МодельТранспорта.Штанга[] LoadShtangi(XmlNode items)
		{
			МодельТранспорта.Штанга[] array = new МодельТранспорта.Штанга[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				double @double = Xml.GetDouble(xmlNode["x"]);
				double double2 = Xml.GetDouble(xmlNode["y"]);
				double double3 = Xml.GetDouble(xmlNode["z"]);
				array[i] = new МодельТранспорта.Штанга(@double, double2, double3);
			}
			return array;
		}

		private static string[] LoadStrings(XmlNode items)
		{
			if (items != null)
			{
				string[] array = new string[items.ChildNodes.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = items.ChildNodes[i].InnerText;
				}
				return array;
			}
			return null;
		}

		private static МодельТранспорта.КолёснаяПара[] LoadWheels(string directory, XmlNode items)
		{
			МодельТранспорта.КолёснаяПара[] array = new МодельТранспорта.КолёснаяПара[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				string dir = directory + xmlNode["dir"].InnerText;
				string innerText = xmlNode["filename"].InnerText;
				int часть = (int)Xml.GetDouble(xmlNode["part"]);
				double @double = Xml.GetDouble(xmlNode["x"]);
				double double2 = Xml.GetDouble(xmlNode["y"]);
				array[i] = new МодельТранспорта.КолёснаяПара(dir, innerText, часть, @double, double2);
			}
			return array;
		}

		private static МодельТранспорта.Хвост[] LoadTails(XmlNode items)
		{
			МодельТранспорта.Хвост[] array = new МодельТранспорта.Хвост[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				double ds = Xml.GetDouble(xmlNode["distance"]);
				string innerText = xmlNode["filename"].InnerText;
				array[i] = new МодельТранспорта.Хвост(ds, innerText);
			}
			return array;
		}

		private static МодельТранспорта.Сочленение_new[] LoadNewMiddles(XmlNode items)
		{
			МодельТранспорта.Сочленение_new[] array = new МодельТранспорта.Сочленение_new[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = items.ChildNodes[i];
				int ind = (int)Xml.GetDouble(xmlNode["index"]);
				int att = (int)Xml.GetDouble(xmlNode["target"]);
				double ds = Xml.GetDouble(xmlNode["distance"]);
				string innerText = xmlNode["filename"].InnerText;
				array[i] = new МодельТранспорта.Сочленение_new(ds, innerText, ind, att);
			}
			return array;
		}

		private static МодельТранспорта.Пантограф LoadPantograph(string directory, XmlNode node)
		{
			string d = directory + node["dir"].InnerText;
			double @double = Xml.GetDouble(node["x"]);
			double double2 = Xml.GetDouble(node["y"]);
			double double3 = Xml.GetDouble(node["z"]);
			double double4 = Xml.GetDouble(node["min_height"]);
			double double5 = Xml.GetDouble(node["max_height"]);
			double dist = Math.Abs(Xml.GetDouble(node["dist"]));
			return new МодельТранспорта.Пантограф(d, @double, double2, double3, double4, double5, dist, LoadPantographsParts(node["parts"]));
		}

		private static МодельТранспорта.Часть_пантографа[] LoadPantographsParts(XmlNode node)
		{
			МодельТранспорта.Часть_пантографа[] array = new МодельТранспорта.Часть_пантографа[node.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = node.ChildNodes[i];
				double @double = Xml.GetDouble(xmlNode["height"]);
				string innerText = xmlNode["filename"].InnerText;
				double double2 = Xml.GetDouble(xmlNode["width"]);
				double double3 = Xml.GetDouble(xmlNode["length"]);
				double double4 = Xml.GetDouble(xmlNode["norm_angel"]);
				array[i] = new МодельТранспорта.Часть_пантографа(innerText, @double, double2, double3, double4);
			}
			return array;
		}

		private static МодельТранспорта.Табличка Load_VPark_Tabl(XmlNode node)
		{
			if (node != null)
			{
				string innerText = node["filename"].InnerText;
				double @double = Xml.GetDouble(node["x"]);
				double double2 = Xml.GetDouble(node["y"]);
				double double3 = Xml.GetDouble(node["z"]);
				return new МодельТранспорта.Табличка(innerText, @double, double2, double3);
			}
			return null;
		}

		private static МодельТранспорта.SphereModel LoadBSphere(XmlNode node)
		{
			double @double = Xml.GetDouble(node["r"]);
			double double2 = Xml.GetDouble(node["x"]);
			double double3 = Xml.GetDouble(node["y"]);
			double double4 = Xml.GetDouble(node["z"]);
			return new МодельТранспорта.SphereModel(@double, double2, double3, double4);
		}

		private static МодельТранспорта.SphereModel[] LoadBSpheres(XmlNode node)
		{
			МодельТранспорта.SphereModel[] array = new МодельТранспорта.SphereModel[node.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadBSphere(node.ChildNodes[i]);
			}
			return array;
		}

		private static МодельТранспорта.Тележка[] LoadBoogies(XmlNode node)
		{
			МодельТранспорта.Тележка[] array = new МодельТранспорта.Тележка[node.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XmlNode xmlNode = node.ChildNodes[i];
				int index = (int)Xml.GetDouble(xmlNode["index"]);
				string @string = Xml.GetString(xmlNode["filename"]);
				double @double = Xml.GetDouble(xmlNode["dist"]);
				array[i] = new МодельТранспорта.Тележка(index, @double, @string);
			}
			return array;
		}

		private static МодельТранспорта.Camera[] LoadCameras(XmlNode node)
		{
			if (node != null)
			{
				МодельТранспорта.Camera[] array = new МодельТранспорта.Camera[node.ChildNodes.Count];
				for (int i = 0; i < array.Length; i++)
				{
					XmlNode xmlNode = node.ChildNodes[i];
					double @double = Xml.GetDouble(xmlNode["x"]);
					double double2 = Xml.GetDouble(xmlNode["y"]);
					double double3 = Xml.GetDouble(xmlNode["z"]);
					double double4 = Xml.GetDouble(xmlNode["rot_y"]);
					double double5 = Xml.GetDouble(xmlNode["rot_z"]);
					array[i] = new МодельТранспорта.Camera(@double, double2, double3, double4, double5);
				}
				return array;
			}
			return null;
		}
	}
}

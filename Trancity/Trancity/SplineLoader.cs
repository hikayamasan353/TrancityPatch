using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Engine;

namespace Trancity
{
	public static class SplineLoader
	{
		public static readonly List<SplineModel> splines;

		static SplineLoader()
		{
			splines = new List<SplineModel>();
			XmlDocument xmlDocument = new XmlDocument();
			string text = Application.StartupPath + "\\Data\\Splines\\";
			if (!Directory.Exists(text))
			{
				Logger.Log("SplineLoader", "Directory " + text + " not found!");
				return;
			}
			string[] directories = Directory.GetDirectories(text);
			for (int i = 0; i < directories.Length; i++)
			{
				string text2 = directories[i] + "\\";
				try
				{
					xmlDocument.Load(text2 + "spline.xml");
				}
				catch (Exception)
				{
					Logger.Log("SplineLoader", "spline.xml not found in directory " + text2);
					continue;
				}
				XmlElement xmlElement = xmlDocument["Trancity"];
				if (xmlElement == null)
				{
					continue;
				}
				XmlElement xmlElement2 = xmlElement["Spline"];
				if (xmlElement2 != null)
				{
					SplineModel splineModel = new SplineModel();
					try
					{
						splineModel.dir = text2 + xmlElement2["dir"].InnerText;
						splineModel.name = xmlElement2["name"].InnerText;
						splineModel.type = (int)Xml.GetDouble(xmlElement2["type"]);
						splineModel.noscale = Xml.GetDouble(xmlElement2["noscale"]) != 0.0;
						splineModel.length = Xml.GetDouble(xmlElement2["length"]);
						splineModel.texture_filename = xmlElement2["texture_filename"].InnerText;
						splineModel.points = LoadSplinePoints(xmlElement2["points"]);
						splineModel.mesh_filename = xmlElement2["mesh_filename"].InnerText;
						splines.Add(splineModel);
					}
					catch (Exception)
					{
						Logger.Log("SplineLoader", "Error in " + text2 + "spline.xml");
					}
				}
			}
		}

		private static Double3DPoint[] LoadSplinePoints(XmlNode items)
		{
			Double3DPoint[] array = new Double3DPoint[items.ChildNodes.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LoadSplinePoint(items.ChildNodes[i]);
			}
			return array;
		}

		private static Double3DPoint LoadSplinePoint(XmlNode items)
		{
			return new Double3DPoint(Xml.GetDouble(items["x"]), Xml.GetDouble(items["y"]), Xml.GetDouble(items["texv"]));
		}
	}
}

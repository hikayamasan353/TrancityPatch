using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Engine;

namespace Trancity
{
	public static class ObjectLoader
	{
		public static readonly List<ObjectModel>[] objects;

		static ObjectLoader()
		{
			objects = new List<ObjectModel>[4];
			XmlDocument xmlDocument = new XmlDocument();
			string path = Application.StartupPath + "\\Data\\Objects\\";
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i] = new List<ObjectModel>();
			}
			string[] directories = Directory.GetDirectories(path);
			for (int j = 0; j < directories.Length; j++)
			{
				string text = directories[j] + "\\";
				try
				{
					xmlDocument.Load(text + "object.xml");
				}
				catch (Exception)
				{
					Logger.Log("ObjectLoader", "object.xml not found in directory " + text);
					continue;
				}
				XmlElement xmlElement = xmlDocument["Trancity"];
				if (xmlElement == null)
				{
					continue;
				}
				XmlElement xmlElement2 = xmlElement["Object"];
				if (xmlElement2 == null)
				{
					continue;
				}
				ObjectModel objectModel = new ObjectModel();
				try
				{
					objectModel.dir = text + xmlElement2["dir"].InnerText;
					objectModel.name = xmlElement2["name"].InnerText;
					objectModel.filename = xmlElement2["filename"].InnerText;
					objectModel.bsphere = LoadBSphere(xmlElement2["bounding_sphere"]);
					int num = (int)Xml.GetDouble(xmlElement2["type"]);
					try
					{
						objectModel.args = new Dictionary<string, string>();
						foreach (XmlNode childNode in xmlElement2["args"].ChildNodes)
						{
							objectModel.args.Add(childNode.LocalName, childNode.InnerText);
						}
					}
					catch
					{
					}
					objects[num].Add(objectModel);
				}
				catch (Exception)
				{
					Logger.Log("ObjectLoader", "Error in " + text + "object.xml");
				}
			}
		}

		private static void ParseArgs(XmlNode node, ref Dictionary<string, string> args)
		{
			try
			{
				args = new Dictionary<string, string>();
				foreach (XmlNode childNode in node.ChildNodes)
				{
					args.Add(childNode.LocalName, childNode.InnerText);
				}
			}
			catch
			{
			}
		}

		private static ObjectModel.SphereModel LoadBSphere(XmlNode node)
		{
			double @double = Xml.GetDouble(node["r"]);
			double double2 = Xml.GetDouble(node["x"]);
			double double3 = Xml.GetDouble(node["y"]);
			double double4 = Xml.GetDouble(node["z"]);
			return new ObjectModel.SphereModel(@double, double2, double3, double4);
		}

		public static void FindModel(byte index, string name, ref ObjectModel model, ref string dir)
		{
			try
			{
				model = null;
				foreach (ObjectModel item in objects[index])
				{
					if (item.name == name)
					{
						model = item;
						break;
					}
				}
				dir = model.dir;
			}
			catch
			{
				Logger.Log("ObjectLoader", "Object " + name + " not found!");
			}
		}
	}
}

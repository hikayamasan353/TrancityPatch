using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Common;
using Engine;

namespace Trancity
{
	public static class Localization
	{
		public static ПсевдоЛокализация current_;

		public static readonly List<ПсевдоЛокализация> localizations;

		static Localization()
		{
			localizations = new List<ПсевдоЛокализация>();
			string text = Application.StartupPath + "\\Data\\Localization\\";
			if (!Directory.Exists(text))
			{
				Logger.Log("Localization", "Directory " + text + " not found!");
				return;
			}
			string[] files = Directory.GetFiles(text);
			foreach (string text2 in files)
			{
				if (Path.GetExtension(text2) != ".xml")
				{
					continue;
				}
				try
				{
					XmlDocument xmlDocument = Xml.TryOpenDocument(text2);
					ПсевдоЛокализация item = new ПсевдоЛокализация
					{
						name = Path.GetFileNameWithoutExtension(text2),
						controllist = new List<TextListStruct>(),
						menulist = new List<TextListStruct>(),
						tipslist = new List<TextListStruct>()
					};
					XmlElement xmlElement = xmlDocument["Trancity"]["Localization"];
					XmlElement xmlElement2 = xmlElement["Reserve"];
					XmlElement xmlElement3 = xmlElement["Messages"];
					XmlElement xmlElement4 = xmlElement["Forms"];
					XmlElement xmlElement5 = xmlElement["Menu_Editor"];
					XmlElement xmlElement6 = xmlElement["Tips"];
					XmlElement xmlElement7 = xmlElement["General"];
					XmlElement xmlElement8 = xmlElement["Tramway"];
					XmlElement xmlElement9 = xmlElement["Trolleybus"];
					XmlElement xmlElement10 = xmlElement["Bus"];
					item.windowed = Xml.GetString(xmlElement2["Windowed"], "в окне");
					item.empty = Xml.GetString(xmlElement2["Empty"], "Нет");
					item.random = Xml.GetString(xmlElement2["Random"], "Случайный");
					item.shtangi_loosed = Xml.GetString(xmlElement2["Shtangi_loosed"], "штанги слетели!");
					item.of = Xml.GetString(xmlElement2["Of"], "из");
					item.edit = Xml.GetString(xmlElement2["Edit"], "Настроить...");
					item.load_city = Xml.GetString(xmlElement2["Load_city"], "Загрузка города...");
					item.load_models = Xml.GetString(xmlElement2["Load_models"], "Загрузка моделей...");
					item.load_shaders = Xml.GetString(xmlElement2["Load_shaders"], "Загрузка шейдеров...");
					item.load_objects = Xml.GetString(xmlElement2["Load_objects"], "Загрузка объектов...");
					item.load_stops = Xml.GetString(xmlElement2["Load_stops"], "Загрузка остановок...");
					item.load_sounds = Xml.GetString(xmlElement2["Load_sounds"], "Загрузка звуков...");
					item.save_city = Xml.GetString(xmlElement2["Save_city"], "Сохранене города...");
					item.joints_begin_end = Xml.GetString(xmlElement3["Joints_no_begin_no_end"]);
					item.joints_begin = Xml.GetString(xmlElement3["Joints_no_begin"]);
					item.joints_end = Xml.GetString(xmlElement3["Joints_no_end"]);
					item.joints_checked = Xml.GetString(xmlElement3["Joints_checked"]);
					item.routes_computed = Xml.GetString(xmlElement3["Routes_already_computed"]);
					item.route_failed = Xml.GetString(xmlElement3["Route_failed"]);
					item.save_quit = Xml.GetString(xmlElement3["Save_city_quit"]);
					item.save_run = Xml.GetString(xmlElement3["Save_city_run"]);
					item.save_only = Xml.GetString(xmlElement3["Save_city"]);
					item.save_failed = Xml.GetString(xmlElement3["Save_city_failed"]);
					item.min_radius = Xml.GetString(xmlElement3["Min_curve_radius"]);
					item.no_curves = Xml.GetString(xmlElement3["Curve_not_found"]);
					if (xmlElement4 != null)
					{
						foreach (XmlElement childNode in xmlElement4.ChildNodes)
						{
							item.controllist.Add(new TextListStruct
							{
								name = childNode.Name,
								text = childNode.InnerText
							});
						}
					}
					if (xmlElement5 != null)
					{
						foreach (XmlElement childNode2 in xmlElement5.ChildNodes)
						{
							item.menulist.Add(new TextListStruct
							{
								name = childNode2.Name,
								text = childNode2.InnerText
							});
						}
					}
					if (xmlElement6 != null)
					{
						foreach (XmlElement childNode3 in xmlElement6.ChildNodes)
						{
							item.tipslist.Add(new TextListStruct
							{
								name = childNode3.Name,
								text = childNode3.InnerText
							});
						}
					}
					item.ctrl_a = Xml.GetString(xmlElement7["Control_auto"]);
					item.ctrl_s = Xml.GetString(xmlElement7["Control_semiauto"]);
					item.ctrl_m = Xml.GetString(xmlElement7["Control_manual"]);
					item.ctrl_pos = Xml.GetString(xmlElement7["Controller_position"]);
					item.reverse = Xml.GetString(xmlElement7["Reverse"]);
					item.parking_brake = Xml.GetString(xmlElement7["Parking_brake"]);
					item.speed = Xml.GetString(xmlElement7["Speed"]);
					item.speed_km = Xml.GetString(xmlElement7["Speed_kmh"]);
					item.route = Xml.GetString(xmlElement7["Route"]);
					item.order = Xml.GetString(xmlElement7["Order"]);
					item.route_in_park = Xml.GetString(xmlElement7["To_park"]);
					item.nr = Xml.GetString(xmlElement7["Next_road"]);
					item.nr_pryamo = Xml.GetString(xmlElement7["Nr_forward"]);
					item.nr_right = Xml.GetString(xmlElement7["Nr_right"]);
					item.nr_left = Xml.GetString(xmlElement7["Nr_left"]);
					item.departure_time = Xml.GetString(xmlElement7["Departure_time"]);
					item.arrival_time = Xml.GetString(xmlElement7["Arrival_time"]);
					item.sterling = Xml.GetString(xmlElement7["Steering"]);
					item.ster_l = Xml.GetString(xmlElement7["Steering_left"]);
					item.ster_r = Xml.GetString(xmlElement7["Steering_right"]);
					item.forward = Xml.GetString(xmlElement7["Forward"]);
					item.back = Xml.GetString(xmlElement7["Back"]);
					item.enable = Xml.GetString(xmlElement7["Enabled"]);
					item.disable = Xml.GetString(xmlElement7["Disabled"]);
					item.tram = Xml.GetString(xmlElement8["Tramway"]);
					item.tram_control = Xml.GetString(xmlElement8["Control"]);
					item.tk_on = Xml.GetString(xmlElement8["Pantograph_raised"]);
					item.tk_off = Xml.GetString(xmlElement8["Pantograph_omitted"]);
					item.trol = Xml.GetString(xmlElement9["Trolleybus"]);
					item.trol_control = Xml.GetString(xmlElement9["Control"]);
					item.st_on = Xml.GetString(xmlElement9["Shtangi_raised"]);
					item.st_off = Xml.GetString(xmlElement9["Shtangi_omitted"]);
					item.air_brake = Xml.GetString(xmlElement9["Air_brake"]);
					item.ax = Xml.GetString(xmlElement9["Standalone_motion"]);
					item.ax_power = Xml.GetString(xmlElement9["Battery_power"]);
					item.engine = Xml.GetString(xmlElement10["Engine"]);
					item.bus_control = Xml.GetString(xmlElement10["Control"]);
					item.gmod = Xml.GetString(xmlElement10["Gearbox_mode"]);
					item.cur_pos = Xml.GetString(xmlElement10["Current_gear"]);
					item.pedal_pos = Xml.GetString(xmlElement10["Pedals_position"]);
					item.gas = Xml.GetString(xmlElement10["Gas"]);
					item.brake = Xml.GetString(xmlElement10["Brake"]);
					localizations.Add(item);
				}
				catch (Exception exception)
				{
					Logger.LogException(exception, "Localization");
					Logger.Log("Localization", "Error in file " + text2);
				}
			}
		}

		public static void ApplyLocalization(Control basecontrol)
		{
			new List<Control>();
			foreach (TextListStruct item in current_.controllist)
			{
				foreach (Control item2 in MyGUI.FindControl(basecontrol, item.name))
				{
					item2.Text = item.text;
				}
			}
			if (((Form)basecontrol).Menu == null)
			{
				return;
			}
			MainMenu menu = ((Form)basecontrol).Menu;
			foreach (TextListStruct item3 in current_.menulist)
			{
				foreach (MenuItem item4 in MyGUI.FindMenuItems(menu, item3.name))
				{
					item4.Text = item3.text;
				}
			}
		}

		public static void ApplyLocalizationToolBar(ToolBar toolbar)
		{
			foreach (TextListStruct item in current_.tipslist)
			{
				foreach (ToolBarButton button in toolbar.Buttons)
				{
					if (button.Name == item.name)
					{
						button.ToolTipText = item.text;
					}
				}
			}
		}
	}
}

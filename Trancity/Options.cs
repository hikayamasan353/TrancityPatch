using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Engine;
using Common;

namespace Trancity
{
	public class Options : Form
	{
		private Dictionary<string, int> _маршруты = new Dictionary<string, int>();

		private Dictionary<int, string> _routeFromIndex = new Dictionary<int, string>();

		private Dictionary<string, int> _транспорт = new Dictionary<string, int>();

		private int _видТранспорта;

		private bool city_ready;

		private Button Add_Button;

		private MainForm.НастройкиЗапуска настройки;

		private CheckBox AutoControl_Box;

		private Label City_label;

		private Label City_Name_label;

		private int current_player = -1;

		private DeviceOptionsDialog dialog;

		private TabPage DirectX_Page;

		private Button Editor_Button;

		private GroupBox DirectX_Box;

		private Button Exit_Button;

		private Label Transport_label;

		private Label Order_label;

		private Label Route_label;

		private Label Control_label;

		private Button Launch_Buttton;

		private Button LoadCity_Button;

		private OpenFileDialog LoadCity_Dialog;

		private TextBox Name_Box;

		private CheckBox NonExclusiveKeyboard_Box;

		private CheckBox NonExclusiveMouse_Box;

		private CheckBox NoStops_Box;

		private GroupBox Options_Group;

		private TabPage Options_Page;

		private GroupBox Players_Box;

		private CheckedListBox Players_List;

		private TabPage Players_Page;

		private TrackBar Rail_Box;

		private Button Remove_Button;

		private TimeBox StartTime_Box;

		private TabControl Tab_Control;

		private Label Time_label;

		private Label Vertex_label;

		private ComboBox VertexProcessing_Box;

		private ComboBox Маршрут_Box;

		private World world;

		private ComboBox Наряд_Box;

		private ComboBox ПодвижнойСостав_Box;

		private ComboBox Управление_Box;

		private CheckBox EnableShaders_Box;

		private Label Langugage_label;

		private ComboBox Lang_Box;

		private CheckBox InvArrows_Box;

		private CheckBox RotateCamera_Box;

		private Label OnRouteCount_label;

		private Label TransportCount_label;

		private Label InParkCount_label;

		private Label Compute_TCount_label;

		private Label Screen_Size_label;

		private Label Splines_Cond_label;

		private IContainer components;

		private TrackBar Volume_TrackBar;

		private CheckBox EnableSound_Box;

		private Button Control_button;
        private Button Configure_Controls_Button;
        private Button Screen_Button;

		public MainForm.НастройкиЗапуска Настройки => настройки;

		private int RouteIndex
		{
			set
			{
				if (value < 2 && Маршрут_Box.Items.Count >= 2)
				{
					Маршрут_Box.SelectedIndex = value;
					return;
				}
				int key = value - 2;
				if (!_routeFromIndex.ContainsKey(key))
				{
					Маршрут_Box.SelectedIndex = 0;
					return;
				}
				string selectedItem = _routeFromIndex[key];
				Маршрут_Box.SelectedItem = selectedItem;
			}
		}

		private void Add_Button_Click(object sender, EventArgs e)
		{
			MainForm.НастройкиЗапускаИгрока[] игроки = настройки.игроки;
			настройки.игроки = new MainForm.НастройкиЗапускаИгрока[игроки.Length + 1];
			настройки.количествоИгроков = игроки.Length + 1;
			for (int i = 0; i < игроки.Length; i++)
			{
				настройки.игроки[i] = игроки[i];
			}
			настройки.игроки[игроки.Length].inputGuid = Guid.Empty;
			настройки.игроки[игроки.Length].вИгре = true;
			настройки.игроки[игроки.Length].имя = Name_Box.Text;
			настройки.игроки[игроки.Length].маршрут = 0;
			настройки.игроки[игроки.Length].наряд = 0;
			настройки.игроки[игроки.Length].подвижнойСостав = "";
			Name_Box.Clear();
			Players_List.Items.Clear();
			for (int j = 0; j < настройки.количествоИгроков; j++)
			{
				Players_List.Items.Add(настройки.игроки[j].имя, настройки.игроки[j].вИгре);
			}
			UpdatePlayers(sender, e);
		}

		public Options(MainForm.НастройкиЗапуска настройки)
		{
			this.настройки = настройки;
			InitializeComponent();
		}

		private void LoadCity_Button_Click(object sender, EventArgs e)
		{
			if (LoadCity_Dialog.ShowDialog(this) == DialogResult.OK)
			{
				настройки.cityFilename = LoadCity_Dialog.FileName;
				UpdateCity();
				UpdatePlayers(sender, e);
			}
		}

		private void Name_Box_Enter(object sender, EventArgs e)
		{
			base.AcceptButton = Add_Button;
		}

		private void Name_Box_Leave(object sender, EventArgs e)
		{
			base.AcceptButton = Launch_Buttton;
		}

		private void Options_Form_Closing(object sender, CancelEventArgs e)
		{
			настройки.начальноеВремя = StartTime_Box.Time_Seconds;
			настройки.качествоРельсов = (double)(Rail_Box.Maximum + Rail_Box.Minimum - Rail_Box.Value) / 100.0;
			настройки.количествоИгроков = Players_List.Items.Count;
			настройки.автоматическоеУправление = AutoControl_Box.Checked;
			настройки.поворачиватьКамеру = RotateCamera_Box.Checked;
			настройки.стрелкиНаоборот = InvArrows_Box.Checked;
			настройки.noSound = !EnableSound_Box.Checked;
			настройки.soundVolume = Volume_TrackBar.Value;
			настройки.noStops = NoStops_Box.Checked;
			настройки.nonExclusiveKeyboard = NonExclusiveKeyboard_Box.Checked;
			настройки.nonExclusiveMouse = NonExclusiveMouse_Box.Checked;
			настройки.enableShaders = EnableShaders_Box.Checked;
			for (int i = 0; i < настройки.количествоИгроков; i++)
			{
				настройки.игроки[i].вИгре = Players_List.CheckedIndices.Contains(i);
			}
			UpdatePlayers(sender, e);
		}

		private void Options_Form_Load(object sender, EventArgs e)
		{
            //Если нет файла настроек то создаем настройки по умолчанию.
            if(!File.Exists("Data\\DeviceOptions.xml"))
            {
                //Сказать что нет файла настроек
                MessageBox.Show("Device options settings not found.\nDefault settings will be applied", "Trancity", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Создаем настройки по умолчанию
                XmlDocument newoptions = new XmlDocument();
                XmlElement node_options = Xml.AddElement(newoptions, "Options");
                XmlElement node_adapterid = Xml.AddElement(newoptions, node_options, "adapter_id",0);
                XmlElement node_device_type = Xml.AddElement(newoptions, node_options, "device_type", 0);
                XmlElement node_vertex_processing = Xml.AddElement(newoptions, node_options, "vertexProcessingMode", 1);
                XmlElement node_vsync = Xml.AddElement(newoptions, node_options, "vSync", 0);
                XmlElement node_fullscreen_x = Xml.AddElement(newoptions, node_options, "fullscreen_x", 800);
                XmlElement node_fullscreen_y = Xml.AddElement(newoptions, node_options, "fullscreen_y", 600);
                XmlElement node_windowed_x = Xml.AddElement(newoptions, node_options, "windowed_x", 800);
                XmlElement node_windowed_y = Xml.AddElement(newoptions, node_options, "windowed_y", 600);
                XmlElement node_windowed = Xml.AddElement(newoptions, node_options, "windowed", 1);
                //Сохраняем их в файл
                newoptions.Save("Data\\DeviceOptions.xml");
            }
            
            dialog = new DeviceOptionsDialog("Data\\DeviceOptions.xml");
            //////////////////////////////////////////////////
			if (components == null)
			{
				components = new Container();
			}
			components.Add(dialog);
			UpdateLocalization();
			StartTime_Box.Time_Seconds = настройки.начальноеВремя;
			if (настройки.качествоРельсов > 0.0)
			{
				Rail_Box.Value = Rail_Box.Maximum + Rail_Box.Minimum - (int)(настройки.качествоРельсов * 100.0);
			}
			AutoControl_Box.Checked = настройки.автоматическоеУправление;
			RotateCamera_Box.Checked = настройки.поворачиватьКамеру;
			InvArrows_Box.Checked = настройки.стрелкиНаоборот;
			EnableSound_Box.Checked = !настройки.noSound;
			Volume_TrackBar.Value = настройки.soundVolume;
			NoStops_Box.Checked = настройки.noStops;
			NonExclusiveKeyboard_Box.Checked = настройки.nonExclusiveKeyboard;
			NonExclusiveMouse_Box.Checked = настройки.nonExclusiveMouse;
			EnableShaders_Box.Checked = настройки.enableShaders;
			Name_Box.Clear();
			Players_List.Items.Clear();
			for (int i = 0; i < настройки.количествоИгроков; i++)
			{
				Players_List.Items.Add(настройки.игроки[i].имя, настройки.игроки[i].вИгре);
			}
			Управление_Box.Items.Clear();
			string[] deviceNames = MyDirectInput.DeviceNames;
			foreach (string item in deviceNames)
			{
				Управление_Box.Items.Add(item);
			}
			foreach (МодельТранспорта item2 in Модели.Трамваи)
			{
				ПодвижнойСостав_Box.Items.Add(item2.name);
				_транспорт[item2.name] = 0;
			}
			foreach (МодельТранспорта item3 in Модели.Троллейбусы)
			{
				ПодвижнойСостав_Box.Items.Add(item3.name);
				_транспорт[item3.name] = 1;
			}
			foreach (МодельТранспорта item4 in Модели.Автобусы)
			{
				ПодвижнойСостав_Box.Items.Add(item4.name);
				_транспорт[item4.name] = 2;
			}
			UpdateCity();
			if (ПодвижнойСостав_Box.SelectedIndex == -1 && ПодвижнойСостав_Box.Items.Count > 0)
			{
				ПодвижнойСостав_Box.SelectedIndex = 0;
			}
			UpdatePlayers(sender, e);
		}

		private void RefreshScreenOptions()
		{
			if (dialog.subj.windowed)
			{
				Screen_Button.Text = $"{dialog.subj.windowedX}x{dialog.subj.windowedY}, {Localization.current_.windowed}";
			}
			else
			{
				Screen_Button.Text = $"{dialog.subj.fullscreenX}x{dialog.subj.fullscreenY}";
			}
		}

		private void Remove_Button_Click(object sender, EventArgs e)
		{
			if (current_player < 0)
			{
				return;
			}
			MainForm.НастройкиЗапускаИгрока[] игроки = настройки.игроки;
			настройки.игроки = new MainForm.НастройкиЗапускаИгрока[игроки.Length - 1];
			настройки.количествоИгроков = игроки.Length - 1;
			int i = 0;
			int num = 0;
			for (; i < игроки.Length; i++)
			{
				if (i != current_player)
				{
					настройки.игроки[num] = игроки[i];
					num++;
				}
			}
			Players_List.Items.Clear();
			for (int j = 0; j < настройки.количествоИгроков; j++)
			{
				Players_List.Items.Add(настройки.игроки[j].имя, настройки.игроки[j].вИгре);
			}
			current_player = -1;
			UpdatePlayers(sender, e);
		}

		private void Screen_ButtonClick(object sender, EventArgs e)
		{
			dialog.ShowDialog(this);
			RefreshScreenOptions();
		}

		private void StartTime_Box_TimeChanged(object sender, EventArgs e)
		{
			UpdateTramsCount();
		}

		private void UpdateCity()
		{
			world = new World();
			if (!string.IsNullOrEmpty(настройки.cityFilename) && File.Exists(настройки.cityFilename))
			{
				City_Name_label.Text = Path.GetFileNameWithoutExtension(настройки.cityFilename);
				world.LoadCitySimple(настройки.cityFilename);
				city_ready = true;
			}
			else
			{
				MessageBox.Show("Current city wasn't found!\nPress OK and select another city.", "Trancity", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				city_ready = false;
			}
			UpdateRoutes(_видТранспорта);
			UpdateМаршрутBox();
			UpdateTramsCount();
		}

		private void UpdatePlayers(object sender, EventArgs e)
		{
			Remove_Button.Enabled = Players_List.SelectedIndex >= 0;
			Add_Button.Enabled = Name_Box.Text.Length > 0;
			Launch_Buttton.Enabled = city_ready && Players_List.CheckedIndices.Count >= 1 && Players_List.CheckedIndices.Count <= 4;
			if (current_player >= 0)
			{
				MainForm.НастройкиЗапускаИгрока[] игроки = настройки.игроки;
				if (Управление_Box.SelectedIndex >= 0)
				{
					игроки[current_player].inputGuid = MyDirectInput.DeviceGuids[Управление_Box.SelectedIndex];
				}
				игроки[current_player].подвижнойСостав = (string)ПодвижнойСостав_Box.SelectedItem;
				if (игроки[current_player].подвижнойСостав == null)
				{
					игроки[current_player].подвижнойСостав = "";
				}
				switch (Маршрут_Box.SelectedIndex)
				{
				case 0:
					игроки[current_player].маршрут = 0;
					break;
				case 1:
					игроки[current_player].маршрут = 1;
					break;
				default:
					игроки[current_player].маршрут = _маршруты[(string)Маршрут_Box.SelectedItem] + 2;
					break;
				}
				игроки[current_player].наряд = Наряд_Box.SelectedIndex;
			}
			current_player = Players_List.SelectedIndex;
			if (current_player < 0)
			{
				Players_Box.Visible = false;
				return;
			}
			MainForm.НастройкиЗапускаИгрока[] игроки2 = настройки.игроки;
			Players_Box.Visible = true;
			int selectedIndex = -1;
			for (int i = 0; i < MyDirectInput.DeviceGuids.Length; i++)
			{
				if (!(игроки2[current_player].inputGuid != MyDirectInput.DeviceGuids[i]))
				{
					selectedIndex = i;
					break;
				}
			}
			Управление_Box.SelectedIndex = selectedIndex;
			if (игроки2[current_player].подвижнойСостав != "")
			{
				ПодвижнойСостав_Box.SelectedItem = игроки2[current_player].подвижнойСостав;
			}
			RouteIndex = игроки2[current_player].маршрут;
			Наряд_Box.SelectedIndex = ((игроки2[current_player].наряд < Наряд_Box.Items.Count) ? игроки2[current_player].наряд : 0);
		}

		private void UpdateTramsCount()
		{
			if (world == null)
			{
				return;
			}
			int num = StartTime_Box.Time_Seconds;
			if (num < 10800)
			{
				num += 86400;
			}
			int num2 = 0;
			int num3 = 0;
			Route[] маршруты = world.маршруты;
			for (int i = 0; i < маршруты.Length; i++)
			{
				Order[] orders = маршруты[i].orders;
				foreach (Order order in orders)
				{
					if (order.рейсы.Length == 0 || order.рейсы[order.рейсы.Length - 1].время_прибытия <= (double)num)
					{
						continue;
					}
					bool flag = false;
					for (int k = 0; k < order.рейсы.Length; k++)
					{
						if (!((double)num >= order.рейсы[k].время_прибытия))
						{
							flag = (double)num < order.рейсы[k].время_отправления && order.рейсы[k].дорога_отправления == order.парк.выезд;
							break;
						}
					}
					if (flag)
					{
						num3++;
					}
					else
					{
						num2++;
					}
				}
			}
			Compute_TCount_label.Text = num2 + "\n" + num3;
		}

		private void Маршрут_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			Наряд_Box.Items.Clear();
			Наряд_Box.Items.Add(Localization.current_.empty);
			if (Маршрут_Box.SelectedIndex > 0)
			{
				if (Маршрут_Box.SelectedIndex == 1)
				{
					Наряд_Box.Items.Add(Localization.current_.random);
				}
				else if (world.маршруты[_маршруты[(string)Маршрут_Box.SelectedItem]].orders.Length != 0)
				{
					Наряд_Box.Items.Add(Localization.current_.random);
					Order[] orders = world.маршруты[_маршруты[(string)Маршрут_Box.SelectedItem]].orders;
					foreach (Order order in orders)
					{
						string text = order.номер + " (";
						if (order.рейсы.Length != 0)
						{
							string text2 = text;
							text = text2 + order.рейсы[0].str_время_отправления + " - " + order.рейсы[order.рейсы.Length - 1].str_время_прибытия + ", ";
						}
						text = text + order.парк.название + ")";
						Наряд_Box.Items.Add(text);
					}
				}
			}
			Наряд_Box.SelectedIndex = 0;
		}

		private void ПодвижнойСостав_Box_SelectedIndexChanged(object sender, EventArgs e)
		{
			_видТранспорта = _транспорт[(string)ПодвижнойСостав_Box.SelectedItem];
			UpdateRoutes(_видТранспорта);
			UpdateМаршрутBox();
		}

		private void UpdateМаршрутBox()
		{
			Маршрут_Box.Items.Clear();
			Маршрут_Box.Items.Add(Localization.current_.empty);
			foreach (string key in _маршруты.Keys)
			{
				Маршрут_Box.Items.Add(key);
			}
			if (Маршрут_Box.Items.Count > 1)
			{
				Маршрут_Box.Items.Insert(1, Localization.current_.random);
			}
			Маршрут_Box.SelectedIndex = 0;
		}

		private void UpdateRoutes(int видТранспорта)
		{
			_маршруты.Clear();
			_routeFromIndex.Clear();
			for (int i = 0; i < world.маршруты.Length; i++)
			{
				Route route = world.маршруты[i];
				if (route.typeOfTransport == видТранспорта)
				{
					_маршруты[route.number] = i;
					_routeFromIndex[i] = route.number;
				}
			}
		}

		private void UpdateLocalization()
		{
			Lang_Box.Items.Clear();
			Lang_Box.SelectedIndex = -1;
			for (int i = 0; i < Localization.localizations.Count; i++)
			{
				Lang_Box.Items.Add(Localization.localizations[i].name);
				if (Localization.localizations[i].name == настройки.language)
				{
					Lang_Box.SelectedIndex = i;
				}
			}
			if (Localization.localizations.Count > 0 && Lang_Box.SelectedIndex == -1)
			{
				Lang_Box.SelectedIndex = 0;
			}
		}

		private void Lang_BoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (Lang_Box.SelectedIndex < 0)
			{
				return;
			}
			Localization.current_ = Localization.localizations[Lang_Box.SelectedIndex];
			настройки.language = Localization.current_.name;
			Localization.ApplyLocalization(this);
			if (dialog != null)
			{
				Localization.ApplyLocalization(dialog);
				RefreshScreenOptions();
			}
			if (Маршрут_Box.Items.Count > 0)
			{
				Маршрут_Box.Items[0] = Localization.current_.empty;
				if (Маршрут_Box.Items.Count > 2)
				{
					Маршрут_Box.Items[1] = Localization.current_.random;
				}
			}
		}

		private void Control_buttonClick(object sender, EventArgs e)
		{
			UserControlForm userControlForm = new UserControlForm();
			userControlForm.ShowDialog();
			_ = 1;
			userControlForm.Dispose();
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
            this.Launch_Buttton = new System.Windows.Forms.Button();
            this.Exit_Button = new System.Windows.Forms.Button();
            this.AutoControl_Box = new System.Windows.Forms.CheckBox();
            this.Time_label = new System.Windows.Forms.Label();
            this.TransportCount_label = new System.Windows.Forms.Label();
            this.RotateCamera_Box = new System.Windows.Forms.CheckBox();
            this.Tab_Control = new System.Windows.Forms.TabControl();
            this.Options_Page = new System.Windows.Forms.TabPage();
            this.Options_Group = new System.Windows.Forms.GroupBox();
            this.Lang_Box = new System.Windows.Forms.ComboBox();
            this.Compute_TCount_label = new System.Windows.Forms.Label();
            this.InParkCount_label = new System.Windows.Forms.Label();
            this.OnRouteCount_label = new System.Windows.Forms.Label();
            this.City_Name_label = new System.Windows.Forms.Label();
            this.LoadCity_Button = new System.Windows.Forms.Button();
            this.StartTime_Box = new Trancity.TimeBox();
            this.InvArrows_Box = new System.Windows.Forms.CheckBox();
            this.City_label = new System.Windows.Forms.Label();
            this.Langugage_label = new System.Windows.Forms.Label();
            this.Players_Page = new System.Windows.Forms.TabPage();
            this.Players_Box = new System.Windows.Forms.GroupBox();
            this.Управление_Box = new System.Windows.Forms.ComboBox();
            this.Control_label = new System.Windows.Forms.Label();
            this.Order_label = new System.Windows.Forms.Label();
            this.Route_label = new System.Windows.Forms.Label();
            this.Наряд_Box = new System.Windows.Forms.ComboBox();
            this.Маршрут_Box = new System.Windows.Forms.ComboBox();
            this.ПодвижнойСостав_Box = new System.Windows.Forms.ComboBox();
            this.Transport_label = new System.Windows.Forms.Label();
            this.Remove_Button = new System.Windows.Forms.Button();
            this.Name_Box = new System.Windows.Forms.TextBox();
            this.Add_Button = new System.Windows.Forms.Button();
            this.Players_List = new System.Windows.Forms.CheckedListBox();
            this.DirectX_Page = new System.Windows.Forms.TabPage();
            this.DirectX_Box = new System.Windows.Forms.GroupBox();
            this.Screen_Button = new System.Windows.Forms.Button();
            this.Control_button = new System.Windows.Forms.Button();
            this.EnableShaders_Box = new System.Windows.Forms.CheckBox();
            this.Screen_Size_label = new System.Windows.Forms.Label();
            this.NonExclusiveMouse_Box = new System.Windows.Forms.CheckBox();
            this.Splines_Cond_label = new System.Windows.Forms.Label();
            this.NonExclusiveKeyboard_Box = new System.Windows.Forms.CheckBox();
            this.Vertex_label = new System.Windows.Forms.Label();
            this.NoStops_Box = new System.Windows.Forms.CheckBox();
            this.VertexProcessing_Box = new System.Windows.Forms.ComboBox();
            this.EnableSound_Box = new System.Windows.Forms.CheckBox();
            this.Volume_TrackBar = new System.Windows.Forms.TrackBar();
            this.Rail_Box = new System.Windows.Forms.TrackBar();
            this.Editor_Button = new System.Windows.Forms.Button();
            this.LoadCity_Dialog = new System.Windows.Forms.OpenFileDialog();
            this.Configure_Controls_Button = new System.Windows.Forms.Button();
            this.Tab_Control.SuspendLayout();
            this.Options_Page.SuspendLayout();
            this.Options_Group.SuspendLayout();
            this.Players_Page.SuspendLayout();
            this.Players_Box.SuspendLayout();
            this.DirectX_Page.SuspendLayout();
            this.DirectX_Box.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Volume_TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rail_Box)).BeginInit();
            this.SuspendLayout();
            // 
            // Launch_Buttton
            // 
            this.Launch_Buttton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Launch_Buttton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Launch_Buttton.Location = new System.Drawing.Point(60, 389);
            this.Launch_Buttton.Name = "Launch_Buttton";
            this.Launch_Buttton.Size = new System.Drawing.Size(153, 43);
            this.Launch_Buttton.TabIndex = 0;
            this.Launch_Buttton.Text = "Запустить!";
            // 
            // Exit_Button
            // 
            this.Exit_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Exit_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Exit_Button.Location = new System.Drawing.Point(427, 389);
            this.Exit_Button.Name = "Exit_Button";
            this.Exit_Button.Size = new System.Drawing.Size(153, 43);
            this.Exit_Button.TabIndex = 1;
            this.Exit_Button.Text = "Выход";
            // 
            // AutoControl_Box
            // 
            this.AutoControl_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoControl_Box.Location = new System.Drawing.Point(40, 169);
            this.AutoControl_Box.Name = "AutoControl_Box";
            this.AutoControl_Box.Size = new System.Drawing.Size(271, 21);
            this.AutoControl_Box.TabIndex = 3;
            this.AutoControl_Box.Text = "Начать с автоматическим управлением";
            // 
            // Time_label
            // 
            this.Time_label.AutoSize = true;
            this.Time_label.Location = new System.Drawing.Point(40, 79);
            this.Time_label.Name = "Time_label";
            this.Time_label.Size = new System.Drawing.Size(139, 17);
            this.Time_label.TabIndex = 0;
            this.Time_label.Text = "Начальное время:";
            // 
            // TransportCount_label
            // 
            this.TransportCount_label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TransportCount_label.AutoSize = true;
            this.TransportCount_label.Location = new System.Drawing.Point(40, 23);
            this.TransportCount_label.Name = "TransportCount_label";
            this.TransportCount_label.Size = new System.Drawing.Size(124, 17);
            this.TransportCount_label.TabIndex = 0;
            this.TransportCount_label.Text = "Количество ПС:";
            // 
            // RotateCamera_Box
            // 
            this.RotateCamera_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RotateCamera_Box.Location = new System.Drawing.Point(40, 206);
            this.RotateCamera_Box.Name = "RotateCamera_Box";
            this.RotateCamera_Box.Size = new System.Drawing.Size(271, 20);
            this.RotateCamera_Box.TabIndex = 3;
            this.RotateCamera_Box.Text = "Поворачивать камеру вместе с ПС";
            // 
            // Tab_Control
            // 
            this.Tab_Control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Tab_Control.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.Tab_Control.Controls.Add(this.Options_Page);
            this.Tab_Control.Controls.Add(this.Players_Page);
            this.Tab_Control.Controls.Add(this.DirectX_Page);
            this.Tab_Control.ItemSize = new System.Drawing.Size(131, 21);
            this.Tab_Control.Location = new System.Drawing.Point(47, 21);
            this.Tab_Control.Name = "Tab_Control";
            this.Tab_Control.SelectedIndex = 0;
            this.Tab_Control.Size = new System.Drawing.Size(545, 346);
            this.Tab_Control.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.Tab_Control.TabIndex = 3;
            // 
            // Options_Page
            // 
            this.Options_Page.Controls.Add(this.Options_Group);
            this.Options_Page.Location = new System.Drawing.Point(4, 25);
            this.Options_Page.Name = "Options_Page";
            this.Options_Page.Size = new System.Drawing.Size(374, 273);
            this.Options_Page.TabIndex = 0;
            this.Options_Page.Text = "Общие настройки";
            // 
            // Options_Group
            // 
            this.Options_Group.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Options_Group.BackColor = System.Drawing.Color.Transparent;
            this.Options_Group.Controls.Add(this.Lang_Box);
            this.Options_Group.Controls.Add(this.Compute_TCount_label);
            this.Options_Group.Controls.Add(this.InParkCount_label);
            this.Options_Group.Controls.Add(this.OnRouteCount_label);
            this.Options_Group.Controls.Add(this.City_Name_label);
            this.Options_Group.Controls.Add(this.LoadCity_Button);
            this.Options_Group.Controls.Add(this.StartTime_Box);
            this.Options_Group.Controls.Add(this.InvArrows_Box);
            this.Options_Group.Controls.Add(this.AutoControl_Box);
            this.Options_Group.Controls.Add(this.City_label);
            this.Options_Group.Controls.Add(this.Langugage_label);
            this.Options_Group.Controls.Add(this.Time_label);
            this.Options_Group.Controls.Add(this.TransportCount_label);
            this.Options_Group.Controls.Add(this.RotateCamera_Box);
            this.Options_Group.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Options_Group.Location = new System.Drawing.Point(8, 5);
            this.Options_Group.Name = "Options_Group";
            this.Options_Group.Size = new System.Drawing.Size(357, 254);
            this.Options_Group.TabIndex = 2;
            this.Options_Group.TabStop = false;
            // 
            // Lang_Box
            // 
            this.Lang_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Lang_Box.FormattingEnabled = true;
            this.Lang_Box.Location = new System.Drawing.Point(289, 200);
            this.Lang_Box.Name = "Lang_Box";
            this.Lang_Box.Size = new System.Drawing.Size(184, 25);
            this.Lang_Box.TabIndex = 11;
            this.Lang_Box.SelectedIndexChanged += new System.EventHandler(this.Lang_BoxSelectedIndexChanged);
            // 
            // Compute_TCount_label
            // 
            this.Compute_TCount_label.Location = new System.Drawing.Point(411, 107);
            this.Compute_TCount_label.Name = "Compute_TCount_label";
            this.Compute_TCount_label.Size = new System.Drawing.Size(56, 31);
            this.Compute_TCount_label.TabIndex = 10;
            this.Compute_TCount_label.Text = "0";
            this.Compute_TCount_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InParkCount_label
            // 
            this.InParkCount_label.Location = new System.Drawing.Point(289, 123);
            this.InParkCount_label.Name = "InParkCount_label";
            this.InParkCount_label.Size = new System.Drawing.Size(114, 15);
            this.InParkCount_label.TabIndex = 10;
            this.InParkCount_label.Text = "в парке:";
            this.InParkCount_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OnRouteCount_label
            // 
            this.OnRouteCount_label.Location = new System.Drawing.Point(289, 107);
            this.OnRouteCount_label.Name = "OnRouteCount_label";
            this.OnRouteCount_label.Size = new System.Drawing.Size(114, 16);
            this.OnRouteCount_label.TabIndex = 10;
            this.OnRouteCount_label.Text = "на линии:";
            this.OnRouteCount_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // City_Name_label
            // 
            this.City_Name_label.AutoSize = true;
            this.City_Name_label.Location = new System.Drawing.Point(111, 43);
            this.City_Name_label.Name = "City_Name_label";
            this.City_Name_label.Size = new System.Drawing.Size(97, 17);
            this.City_Name_label.TabIndex = 9;
            this.City_Name_label.Text = "не загружен";
            // 
            // LoadCity_Button
            // 
            this.LoadCity_Button.Location = new System.Drawing.Point(289, 36);
            this.LoadCity_Button.Name = "LoadCity_Button";
            this.LoadCity_Button.Size = new System.Drawing.Size(184, 28);
            this.LoadCity_Button.TabIndex = 8;
            this.LoadCity_Button.Text = "Загрузить...";
            this.LoadCity_Button.UseVisualStyleBackColor = true;
            this.LoadCity_Button.Click += new System.EventHandler(this.LoadCity_Button_Click);
            // 
            // StartTime_Box
            // 
            this.StartTime_Box.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StartTime_Box.Hours = 0;
            this.StartTime_Box.Location = new System.Drawing.Point(289, 75);
            this.StartTime_Box.MinimumSize = new System.Drawing.Size(40, 21);
            this.StartTime_Box.Minutes = 0;
            this.StartTime_Box.Name = "StartTime_Box";
            this.StartTime_Box.Seconds = 0;
            this.StartTime_Box.Size = new System.Drawing.Size(184, 26);
            this.StartTime_Box.TabIndex = 6;
            this.StartTime_Box.Time = new System.DateTime(((long)(0)));
            this.StartTime_Box.Time_Minutes = 0;
            this.StartTime_Box.Time_Seconds = 0;
            this.StartTime_Box.ViewSeconds = true;
            this.StartTime_Box.TimeChanged += new System.EventHandler(this.StartTime_Box_TimeChanged);
            // 
            // InvArrows_Box
            // 
            this.InvArrows_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InvArrows_Box.Location = new System.Drawing.Point(40, 60);
            this.InvArrows_Box.Name = "InvArrows_Box";
            this.InvArrows_Box.Size = new System.Drawing.Size(271, 20);
            this.InvArrows_Box.TabIndex = 3;
            this.InvArrows_Box.Text = "Переключение стрелок под током налево";
            // 
            // City_label
            // 
            this.City_label.AutoSize = true;
            this.City_label.Location = new System.Drawing.Point(40, 43);
            this.City_label.Name = "City_label";
            this.City_label.Size = new System.Drawing.Size(58, 17);
            this.City_label.TabIndex = 0;
            this.City_label.Text = "Город:";
            // 
            // Langugage_label
            // 
            this.Langugage_label.AutoSize = true;
            this.Langugage_label.Location = new System.Drawing.Point(40, 204);
            this.Langugage_label.Name = "Langugage_label";
            this.Langugage_label.Size = new System.Drawing.Size(50, 17);
            this.Langugage_label.TabIndex = 0;
            this.Langugage_label.Text = "Язык:";
            // 
            // Players_Page
            // 
            this.Players_Page.BackColor = System.Drawing.Color.Transparent;
            this.Players_Page.Controls.Add(this.Players_Box);
            this.Players_Page.Controls.Add(this.Remove_Button);
            this.Players_Page.Controls.Add(this.Name_Box);
            this.Players_Page.Controls.Add(this.Add_Button);
            this.Players_Page.Controls.Add(this.Players_List);
            this.Players_Page.Location = new System.Drawing.Point(4, 25);
            this.Players_Page.Name = "Players_Page";
            this.Players_Page.Size = new System.Drawing.Size(537, 317);
            this.Players_Page.TabIndex = 1;
            this.Players_Page.Text = "Настройки игроков";
            // 
            // Players_Box
            // 
            this.Players_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Players_Box.Controls.Add(this.Configure_Controls_Button);
            this.Players_Box.Controls.Add(this.Управление_Box);
            this.Players_Box.Controls.Add(this.Control_label);
            this.Players_Box.Controls.Add(this.Order_label);
            this.Players_Box.Controls.Add(this.Route_label);
            this.Players_Box.Controls.Add(this.Наряд_Box);
            this.Players_Box.Controls.Add(this.Маршрут_Box);
            this.Players_Box.Controls.Add(this.ПодвижнойСостав_Box);
            this.Players_Box.Controls.Add(this.Transport_label);
            this.Players_Box.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Players_Box.Location = new System.Drawing.Point(8, 72);
            this.Players_Box.Name = "Players_Box";
            this.Players_Box.Size = new System.Drawing.Size(520, 231);
            this.Players_Box.TabIndex = 3;
            this.Players_Box.TabStop = false;
            // 
            // Управление_Box
            // 
            this.Управление_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Управление_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Управление_Box.Location = new System.Drawing.Point(228, 39);
            this.Управление_Box.Name = "Управление_Box";
            this.Управление_Box.Size = new System.Drawing.Size(252, 25);
            this.Управление_Box.TabIndex = 1;
            // 
            // Control_label
            // 
            this.Control_label.AutoSize = true;
            this.Control_label.Location = new System.Drawing.Point(40, 43);
            this.Control_label.Name = "Control_label";
            this.Control_label.Size = new System.Drawing.Size(100, 17);
            this.Control_label.TabIndex = 0;
            this.Control_label.Text = "Управление:";
            // 
            // Order_label
            // 
            this.Order_label.AutoSize = true;
            this.Order_label.Location = new System.Drawing.Point(40, 190);
            this.Order_label.Name = "Order_label";
            this.Order_label.Size = new System.Drawing.Size(58, 17);
            this.Order_label.TabIndex = 0;
            this.Order_label.Text = "Наряд:";
            // 
            // Route_label
            // 
            this.Route_label.AutoSize = true;
            this.Route_label.Location = new System.Drawing.Point(40, 153);
            this.Route_label.Name = "Route_label";
            this.Route_label.Size = new System.Drawing.Size(77, 17);
            this.Route_label.TabIndex = 0;
            this.Route_label.Text = "Маршрут:";
            // 
            // Наряд_Box
            // 
            this.Наряд_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Наряд_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Наряд_Box.Items.AddRange(new object[] {
            "Нет",
            "Случайный",
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.Наряд_Box.Location = new System.Drawing.Point(228, 186);
            this.Наряд_Box.Name = "Наряд_Box";
            this.Наряд_Box.Size = new System.Drawing.Size(252, 25);
            this.Наряд_Box.TabIndex = 1;
            // 
            // Маршрут_Box
            // 
            this.Маршрут_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Маршрут_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Маршрут_Box.Items.AddRange(new object[] {
            "Нет",
            "Случайный",
            "А",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9"});
            this.Маршрут_Box.Location = new System.Drawing.Point(228, 150);
            this.Маршрут_Box.Name = "Маршрут_Box";
            this.Маршрут_Box.Size = new System.Drawing.Size(252, 25);
            this.Маршрут_Box.TabIndex = 1;
            this.Маршрут_Box.SelectedIndexChanged += new System.EventHandler(this.Маршрут_Box_SelectedIndexChanged);
            // 
            // ПодвижнойСостав_Box
            // 
            this.ПодвижнойСостав_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ПодвижнойСостав_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ПодвижнойСостав_Box.Location = new System.Drawing.Point(228, 113);
            this.ПодвижнойСостав_Box.Name = "ПодвижнойСостав_Box";
            this.ПодвижнойСостав_Box.Size = new System.Drawing.Size(252, 25);
            this.ПодвижнойСостав_Box.TabIndex = 1;
            this.ПодвижнойСостав_Box.SelectedIndexChanged += new System.EventHandler(this.ПодвижнойСостав_Box_SelectedIndexChanged);
            // 
            // Transport_label
            // 
            this.Transport_label.AutoSize = true;
            this.Transport_label.Location = new System.Drawing.Point(40, 117);
            this.Transport_label.Name = "Transport_label";
            this.Transport_label.Size = new System.Drawing.Size(150, 17);
            this.Transport_label.TabIndex = 0;
            this.Transport_label.Text = "Подвижной состав:";
            // 
            // Remove_Button
            // 
            this.Remove_Button.Location = new System.Drawing.Point(267, 30);
            this.Remove_Button.Name = "Remove_Button";
            this.Remove_Button.Size = new System.Drawing.Size(128, 33);
            this.Remove_Button.TabIndex = 2;
            this.Remove_Button.Text = "Удалить";
            this.Remove_Button.Click += new System.EventHandler(this.Remove_Button_Click);
            // 
            // Name_Box
            // 
            this.Name_Box.Location = new System.Drawing.Point(267, 0);
            this.Name_Box.Name = "Name_Box";
            this.Name_Box.Size = new System.Drawing.Size(261, 24);
            this.Name_Box.TabIndex = 1;
            this.Name_Box.Text = "Игрок 3";
            this.Name_Box.TextChanged += new System.EventHandler(this.UpdatePlayers);
            this.Name_Box.Enter += new System.EventHandler(this.Name_Box_Enter);
            this.Name_Box.Leave += new System.EventHandler(this.Name_Box_Leave);
            // 
            // Add_Button
            // 
            this.Add_Button.Location = new System.Drawing.Point(400, 30);
            this.Add_Button.Name = "Add_Button";
            this.Add_Button.Size = new System.Drawing.Size(128, 33);
            this.Add_Button.TabIndex = 2;
            this.Add_Button.Text = "Добавить";
            this.Add_Button.Click += new System.EventHandler(this.Add_Button_Click);
            // 
            // Players_List
            // 
            this.Players_List.Items.AddRange(new object[] {
            "Игрок 1",
            "Игрок 2"});
            this.Players_List.Location = new System.Drawing.Point(0, 0);
            this.Players_List.Name = "Players_List";
            this.Players_List.ScrollAlwaysVisible = true;
            this.Players_List.Size = new System.Drawing.Size(261, 42);
            this.Players_List.TabIndex = 2;
            this.Players_List.ThreeDCheckBoxes = true;
            this.Players_List.SelectedIndexChanged += new System.EventHandler(this.UpdatePlayers);
            this.Players_List.DoubleClick += new System.EventHandler(this.UpdatePlayers);
            // 
            // DirectX_Page
            // 
            this.DirectX_Page.Controls.Add(this.DirectX_Box);
            this.DirectX_Page.Location = new System.Drawing.Point(4, 25);
            this.DirectX_Page.Name = "DirectX_Page";
            this.DirectX_Page.Padding = new System.Windows.Forms.Padding(3);
            this.DirectX_Page.Size = new System.Drawing.Size(374, 273);
            this.DirectX_Page.TabIndex = 2;
            this.DirectX_Page.Text = "Настройки DirectX";
            this.DirectX_Page.UseVisualStyleBackColor = true;
            // 
            // DirectX_Box
            // 
            this.DirectX_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DirectX_Box.Controls.Add(this.Screen_Button);
            this.DirectX_Box.Controls.Add(this.Control_button);
            this.DirectX_Box.Controls.Add(this.EnableShaders_Box);
            this.DirectX_Box.Controls.Add(this.Screen_Size_label);
            this.DirectX_Box.Controls.Add(this.NonExclusiveMouse_Box);
            this.DirectX_Box.Controls.Add(this.Splines_Cond_label);
            this.DirectX_Box.Controls.Add(this.NonExclusiveKeyboard_Box);
            this.DirectX_Box.Controls.Add(this.Vertex_label);
            this.DirectX_Box.Controls.Add(this.NoStops_Box);
            this.DirectX_Box.Controls.Add(this.VertexProcessing_Box);
            this.DirectX_Box.Controls.Add(this.EnableSound_Box);
            this.DirectX_Box.Controls.Add(this.Volume_TrackBar);
            this.DirectX_Box.Controls.Add(this.Rail_Box);
            this.DirectX_Box.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DirectX_Box.Location = new System.Drawing.Point(8, 5);
            this.DirectX_Box.Name = "DirectX_Box";
            this.DirectX_Box.Size = new System.Drawing.Size(357, 254);
            this.DirectX_Box.TabIndex = 12;
            this.DirectX_Box.TabStop = false;
            // 
            // Screen_Button
            // 
            this.Screen_Button.Location = new System.Drawing.Point(289, 40);
            this.Screen_Button.Name = "Screen_Button";
            this.Screen_Button.Size = new System.Drawing.Size(184, 28);
            this.Screen_Button.TabIndex = 14;
            this.Screen_Button.Text = "<Screen>";
            this.Screen_Button.UseVisualStyleBackColor = true;
            this.Screen_Button.Click += new System.EventHandler(this.Screen_ButtonClick);
            // 
            // Control_button
            // 
            this.Control_button.Location = new System.Drawing.Point(289, 293);
            this.Control_button.Name = "Control_button";
            this.Control_button.Size = new System.Drawing.Size(184, 28);
            this.Control_button.TabIndex = 13;
            this.Control_button.Text = "Управление";
            this.Control_button.UseVisualStyleBackColor = true;
            this.Control_button.Visible = false;
            this.Control_button.Click += new System.EventHandler(this.Control_buttonClick);
            // 
            // EnableShaders_Box
            // 
            this.EnableShaders_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EnableShaders_Box.Location = new System.Drawing.Point(40, 206);
            this.EnableShaders_Box.Name = "EnableShaders_Box";
            this.EnableShaders_Box.Size = new System.Drawing.Size(271, 20);
            this.EnableShaders_Box.TabIndex = 12;
            this.EnableShaders_Box.Text = "Рисовать скайбокс";
            // 
            // Screen_Size_label
            // 
            this.Screen_Size_label.AutoSize = true;
            this.Screen_Size_label.Location = new System.Drawing.Point(40, 43);
            this.Screen_Size_label.Name = "Screen_Size_label";
            this.Screen_Size_label.Size = new System.Drawing.Size(154, 17);
            this.Screen_Size_label.TabIndex = 7;
            this.Screen_Size_label.Text = "Разрешение экрана:";
            // 
            // NonExclusiveMouse_Box
            // 
            this.NonExclusiveMouse_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NonExclusiveMouse_Box.Location = new System.Drawing.Point(40, 169);
            this.NonExclusiveMouse_Box.Name = "NonExclusiveMouse_Box";
            this.NonExclusiveMouse_Box.Size = new System.Drawing.Size(271, 21);
            this.NonExclusiveMouse_Box.TabIndex = 11;
            this.NonExclusiveMouse_Box.Text = "Не захватывать мышь";
            // 
            // Splines_Cond_label
            // 
            this.Splines_Cond_label.AutoSize = true;
            this.Splines_Cond_label.Location = new System.Drawing.Point(40, 115);
            this.Splines_Cond_label.Name = "Splines_Cond_label";
            this.Splines_Cond_label.Size = new System.Drawing.Size(203, 17);
            this.Splines_Cond_label.TabIndex = 6;
            this.Splines_Cond_label.Text = "Качество кривых рельсов:";
            // 
            // NonExclusiveKeyboard_Box
            // 
            this.NonExclusiveKeyboard_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NonExclusiveKeyboard_Box.Location = new System.Drawing.Point(40, 133);
            this.NonExclusiveKeyboard_Box.Name = "NonExclusiveKeyboard_Box";
            this.NonExclusiveKeyboard_Box.Size = new System.Drawing.Size(271, 20);
            this.NonExclusiveKeyboard_Box.TabIndex = 11;
            this.NonExclusiveKeyboard_Box.Text = "Не захватывать клавиатуру";
            // 
            // Vertex_label
            // 
            this.Vertex_label.AutoSize = true;
            this.Vertex_label.Location = new System.Drawing.Point(40, 79);
            this.Vertex_label.Name = "Vertex_label";
            this.Vertex_label.Size = new System.Drawing.Size(141, 17);
            this.Vertex_label.TabIndex = 5;
            this.Vertex_label.Text = "Vertex processing:";
            this.Vertex_label.Visible = false;
            // 
            // NoStops_Box
            // 
            this.NoStops_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NoStops_Box.Location = new System.Drawing.Point(40, 96);
            this.NoStops_Box.Name = "NoStops_Box";
            this.NoStops_Box.Size = new System.Drawing.Size(271, 21);
            this.NoStops_Box.TabIndex = 11;
            this.NoStops_Box.Text = "Не рисовать текст на значках остановок";
            // 
            // VertexProcessing_Box
            // 
            this.VertexProcessing_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.VertexProcessing_Box.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VertexProcessing_Box.Location = new System.Drawing.Point(133, 75);
            this.VertexProcessing_Box.Name = "VertexProcessing_Box";
            this.VertexProcessing_Box.Size = new System.Drawing.Size(184, 25);
            this.VertexProcessing_Box.TabIndex = 9;
            this.VertexProcessing_Box.Visible = false;
            // 
            // EnableSound_Box
            // 
            this.EnableSound_Box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EnableSound_Box.Location = new System.Drawing.Point(40, 60);
            this.EnableSound_Box.Name = "EnableSound_Box";
            this.EnableSound_Box.Size = new System.Drawing.Size(65, 20);
            this.EnableSound_Box.TabIndex = 11;
            this.EnableSound_Box.Text = "Звук";
            // 
            // Volume_TrackBar
            // 
            this.Volume_TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Volume_TrackBar.AutoSize = false;
            this.Volume_TrackBar.LargeChange = 10;
            this.Volume_TrackBar.Location = new System.Drawing.Point(133, 147);
            this.Volume_TrackBar.Maximum = 100;
            this.Volume_TrackBar.Name = "Volume_TrackBar";
            this.Volume_TrackBar.Size = new System.Drawing.Size(184, 29);
            this.Volume_TrackBar.SmallChange = 5;
            this.Volume_TrackBar.TabIndex = 10;
            this.Volume_TrackBar.Value = 80;
            // 
            // Rail_Box
            // 
            this.Rail_Box.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Rail_Box.AutoSize = false;
            this.Rail_Box.Location = new System.Drawing.Point(133, 111);
            this.Rail_Box.Maximum = 400;
            this.Rail_Box.Minimum = 100;
            this.Rail_Box.Name = "Rail_Box";
            this.Rail_Box.Size = new System.Drawing.Size(184, 29);
            this.Rail_Box.TabIndex = 10;
            this.Rail_Box.Value = 400;
            // 
            // Editor_Button
            // 
            this.Editor_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Editor_Button.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Editor_Button.Location = new System.Drawing.Point(260, 389);
            this.Editor_Button.Name = "Editor_Button";
            this.Editor_Button.Size = new System.Drawing.Size(153, 43);
            this.Editor_Button.TabIndex = 1;
            this.Editor_Button.Text = "Редактор";
            // 
            // LoadCity_Dialog
            // 
            this.LoadCity_Dialog.DefaultExt = "city";
            this.LoadCity_Dialog.Filter = "Города Trancity (*.city)|*.city|Все файлы (*.*)|*.*";
            this.LoadCity_Dialog.InitialDirectory = "..\\Cities";
            this.LoadCity_Dialog.Title = "Загрузить город";
            // 
            // Configure_Controls_Button
            // 
            this.Configure_Controls_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Configure_Controls_Button.Location = new System.Drawing.Point(368, 71);
            this.Configure_Controls_Button.Name = "Configure_Controls_Button";
            this.Configure_Controls_Button.Size = new System.Drawing.Size(111, 36);
            this.Configure_Controls_Button.TabIndex = 2;
            this.Configure_Controls_Button.Text = "Настроить...";
            this.Configure_Controls_Button.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 17);
            this.ClientSize = new System.Drawing.Size(632, 471);
            this.Controls.Add(this.Tab_Control);
            this.Controls.Add(this.Exit_Button);
            this.Controls.Add(this.Launch_Buttton);
            this.Controls.Add(this.Editor_Button);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trancity";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Options_Form_Closing);
            this.Load += new System.EventHandler(this.Options_Form_Load);
            this.Tab_Control.ResumeLayout(false);
            this.Options_Page.ResumeLayout(false);
            this.Options_Group.ResumeLayout(false);
            this.Options_Group.PerformLayout();
            this.Players_Page.ResumeLayout(false);
            this.Players_Page.PerformLayout();
            this.Players_Box.ResumeLayout(false);
            this.Players_Box.PerformLayout();
            this.DirectX_Page.ResumeLayout(false);
            this.DirectX_Box.ResumeLayout(false);
            this.DirectX_Box.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Volume_TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rail_Box)).EndInit();
            this.ResumeLayout(false);

		}
	}
}

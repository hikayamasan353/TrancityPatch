using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Common;
using Engine;
using Engine.Controls;
using Engine.Sound;
using ODE_Test;

namespace Trancity
{
	public class MainForm : RenderForm
	{
		public struct НастройкиЗапуска
		{
			public int начальноеВремя;

			public bool автоматическоеУправление;

			public bool поворачиватьКамеру;

			public int количествоИгроков;

			public double качествоРельсов;

			public НастройкиЗапускаИгрока[] игроки;

			public string cityFilename;

			public bool стрелкиНаоборот;

			public bool noSound;

			public int soundVolume;

			public bool noStops;

			public bool nonExclusiveKeyboard;

			public bool nonExclusiveMouse;

			public bool enableShaders;

			public string language;
		}

		public struct НастройкиЗапускаИгрока
		{
			public string имя;

			public Guid inputGuid;

			public string подвижнойСостав;

			public int маршрут;

			public int наряд;

			public bool вИгре;
		}

		public static int ticklast;

		public Game _игра;

		public НастройкиЗапуска настройки;

		public static bool debug;

		public static bool thread_test;

		public static bool in_editor;

		public MainForm()
		{
			InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.ClientSize = new System.Drawing.Size(346, 345);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "MainForm";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Trancity";
			base.ResumeLayout(false);
		}

		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += OnUIThreadException;
			AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
			bool flag = false;
			bool flag2 = false;
			MyFeatures.CheckFolders(Application.StartupPath);
			MainForm mainForm = new MainForm
			{
				настройки = default(НастройкиЗапуска)
			};
			mainForm.ЗагрузитьНастройки();
			foreach (string text in args)
			{
				switch (text)
				{
				case "-limited":
					Cheats.limited = true;
					break;
				case "-debug_strings":
					debug = true;
					break;
				case "-thread_test":
					thread_test = true;
					Logger.Log("Additional render thread enabled!");
					break;
				case "-nolog":
					flag2 = true;
					break;
				default:
					mainForm.настройки.cityFilename = text;
					break;
				}
			}
			if (!flag2)
			{
				Logger.Initialize(Assembly.GetExecutingAssembly());
			}
			MyDirectInput.EnumerateDevices();
			using (Options options = new Options(mainForm.настройки))
			{
				DialogResult dialogResult = options.ShowDialog();
				if (dialogResult != DialogResult.OK)
				{
					mainForm.Close();
					if (dialogResult == DialogResult.Ignore)
					{
						Application.Run(new Editor());
					}
					return;
				}
				mainForm.настройки = options.Настройки;
			}
			mainForm.СохранитьНастройки();
			if (mainForm.настройки.игроки.Length > 1)
			{
				List<НастройкиЗапускаИгрока> list = new List<НастройкиЗапускаИгрока>();
				НастройкиЗапускаИгрока[] игроки = mainForm.настройки.игроки;
				for (int i = 0; i < игроки.Length; i++)
				{
					НастройкиЗапускаИгрока item = игроки[i];
					if (item.вИгре)
					{
						list.Add(item);
					}
				}
				mainForm.настройки.игроки = list.ToArray();
				mainForm.настройки.количествоИгроков = mainForm.настройки.игроки.Length;
			}
			flag = !mainForm.настройки.noSound;
			Рельс.качество_рельсов = mainForm.настройки.качествоРельсов;
			Road.качествоДороги = mainForm.настройки.качествоРельсов;
			Рельс.стрелки_наоборот = mainForm.настройки.стрелкиНаоборот;
			Stop.неЗагружаемКартинки = mainForm.настройки.noStops;
			SkyBox.draw = mainForm.настройки.enableShaders;
			Directory.SetCurrentDirectory(Application.StartupPath + "\\Data");
			if (!MyDirect3D.Initialize(mainForm))
			{
				throw new Exception("Could not initialize Direct3D.");
			}
			mainForm.Show();
			MyGUI.Splash();
			if (flag)
			{
				MyXAudio2.Initialize(SoundDeviceType.XAudio2);
				MyXAudio2.Device.MasterVolume = (float)mainForm.настройки.soundVolume / 100f;
			}
			if (!MyDirectInput.Initialize(mainForm, !mainForm.настройки.nonExclusiveKeyboard, !mainForm.настройки.nonExclusiveMouse))
			{
				throw new Exception("Could not initialize DirectInput.");
			}
			mainForm._игра = new Game();
			MyGUI.splash_title = "Trancity";
			MyGUI.Splash();
			MyGUI.load_status = 0;
			MyGUI.status_string = Localization.current_.load_city;
			MyGUI.Splash();
			mainForm._игра.menu = new MyMenu();
			mainForm._игра.мир = new World();
			mainForm._игра.мир.ЗагрузитьГород(mainForm.настройки.cityFilename);
			mainForm._игра.мир.time = mainForm.настройки.начальноеВремя;
			if (mainForm._игра.мир.time < 10800.0)
			{
				mainForm._игра.мир.time += 86400.0;
			}
			mainForm._игра.мир.ДобавитьТранспорт(mainForm.настройки, mainForm._игра);
			mainForm._игра.мир.Create_Meshes();
			if (flag)
			{
				mainForm._игра.мир.CreateSound();
			}
			else
			{
				MyDirectInput.Acquire();
			}
			Logger.Log(flag ? "Sound enabled" : "Sound disabled");
			Logger.Log("Game started");
			if (thread_test)
			{
				ThreadPoolTest.RunGameProcess(mainForm._игра, flag);
				mainForm.Close();
				return;
			}
			while (mainForm.Created)
			{
				if (!MyDirectInput.Process() && MyDirectInput.alt_f4)
				{
					MyDirectInput.Free();
					mainForm.Close();
					break;
				}
				mainForm._игра.Process_Input();
				if (mainForm._игра.активна)
				{
					mainForm._игра.мир.Обновить(mainForm._игра.игроки);
				}
				else
				{
					mainForm._игра.мир.Обновить_время();
				}
				if (flag)
				{
					mainForm._игра.мир.UpdateSound(mainForm._игра.игроки, mainForm._игра.активна);
				}
				mainForm._игра.Render();
				Application.DoEvents();
			}
		}

		protected override void OnDeactivate(EventArgs e)
		{
			MyDirectInput.Unacquire();
			if (base.WindowState == FormWindowState.Minimized)
			{
				base.ShowInTaskbar = false;
				base.Visible = false;
			}
			base.OnDeactivate(e);
		}

		protected override void OnActivated(EventArgs e)
		{
			MyDirectInput.Acquire();
			if (base.WindowState == FormWindowState.Minimized)
			{
				base.ShowInTaskbar = true;
				base.Visible = true;
				base.WindowState = FormWindowState.Normal;
			}
			base.OnActivated(e);
		}

		private static void OnUIThreadException(object sender, ThreadExceptionEventArgs t)
		{
			Exception exception = t.Exception;
			Logger.LogException(exception, "UI thread");
			if (ExceptionHandlerForm.ShowHandlerDialog(exception, "UI thread", aborted: false) == DialogResult.Abort)
			{
				Environment.Exit(0);
			}
		}

		private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception obj = (Exception)e.ExceptionObject;
			Logger.LogException(obj, "CurrentDomain.UnhandledException");
			if (ExceptionHandlerForm.ShowHandlerDialog(obj, "CurrentDomain.UnhandledException event", e.IsTerminating) != DialogResult.Retry)
			{
				Environment.Exit(0);
			}
		}

		private void ЗагрузитьНастройки()
		{
			Directory.SetCurrentDirectory(Application.StartupPath);
			using Ini ini = new Ini(".\\options.ini", StreamWorkMode.Read);
			настройки.начальноеВремя = ini.ReadInt("Common", "startupTime", 25200);
			настройки.автоматическоеУправление = ini.ReadBool("Common", "autoControl", default_value: false);
			настройки.поворачиватьКамеру = ini.ReadBool("Common", "rotateCam", default_value: true);
			настройки.качествоРельсов = ini.ReadDouble("Common", "splinesQuality", 4.0);
			настройки.cityFilename = ini.Read("Common", "cityFilename", string.Empty);
			настройки.стрелкиНаоборот = ini.ReadBool("Common", "invRailArrows", default_value: false);
			настройки.noSound = ini.ReadBool("Common", "noSound", default_value: false);
			настройки.soundVolume = ini.ReadInt("Common", "soundVolume", 80);
			настройки.noStops = ini.ReadBool("Common", "noStops", default_value: false);
			настройки.nonExclusiveKeyboard = ini.ReadBool("Common", "nonExclusiveKeyboard", default_value: false);
			настройки.nonExclusiveMouse = ini.ReadBool("Common", "nonExclusiveMouse", default_value: false);
			настройки.enableShaders = ini.ReadBool("Common", "enableShaders", default_value: false);
			настройки.language = ini.Read("Common", "language", "Russian");
			настройки.количествоИгроков = ini.ReadInt("Common", "playersCount", 0);
			настройки.игроки = new НастройкиЗапускаИгрока[настройки.количествоИгроков];
			for (int i = 0; i < настройки.количествоИгроков; i++)
			{
				string section = $"Player {i}";
				настройки.игроки[i].имя = ini.Read(section, "name", "Игрок " + i);
				настройки.игроки[i].inputGuid = new Guid(ini.Read(section, "inputGuid", string.Empty));
				настройки.игроки[i].подвижнойСостав = ini.Read(section, "transport", "");
				настройки.игроки[i].маршрут = ini.ReadInt(section, "route", 0);
				настройки.игроки[i].наряд = ini.ReadInt(section, "order", 0);
				настройки.игроки[i].вИгре = ini.ReadBool(section, "inGame", default_value: false);
			}
		}

		private void СохранитьНастройки()
		{
			Directory.SetCurrentDirectory(Application.StartupPath);
			using Ini ini = new Ini(".\\options.ini", StreamWorkMode.Write);
			ini.Write("Common", "startupTime", настройки.начальноеВремя.ToString());
			ini.Write("Common", "autoControl", настройки.автоматическоеУправление.ToString());
			ini.Write("Common", "rotateCam", настройки.поворачиватьКамеру.ToString());
			ini.Write("Common", "splinesQuality", настройки.качествоРельсов.ToString());
			ini.Write("Common", "cityFilename", (настройки.cityFilename != null) ? настройки.cityFilename : "");
			ini.Write("Common", "invRailArrows", настройки.стрелкиНаоборот.ToString());
			ini.Write("Common", "noSound", настройки.noSound.ToString());
			ini.Write("Common", "soundVolume", настройки.soundVolume.ToString());
			ini.Write("Common", "noStops", настройки.noStops.ToString());
			ini.Write("Common", "nonExclusiveKeyboard", настройки.nonExclusiveKeyboard.ToString());
			ini.Write("Common", "nonExclusiveMouse", настройки.nonExclusiveMouse.ToString());
			ini.Write("Common", "enableShaders", настройки.enableShaders.ToString());
			ini.Write("Common", "language", настройки.language.ToString());
			ini.Write("Common", "playersCount", настройки.количествоИгроков.ToString());
			for (int i = 0; i < настройки.количествоИгроков; i++)
			{
				string section = $"Player {i}";
				ini.Write(section, "name", настройки.игроки[i].имя);
				ini.Write(section, "inputGuid", настройки.игроки[i].inputGuid.ToString());
				ini.Write(section, "transport", настройки.игроки[i].подвижнойСостав);
				ini.Write(section, "route", настройки.игроки[i].маршрут.ToString());
				ini.Write(section, "order", настройки.игроки[i].наряд.ToString());
				ini.Write(section, "inGame", настройки.игроки[i].вИгре.ToString());
			}
		}
	}
}

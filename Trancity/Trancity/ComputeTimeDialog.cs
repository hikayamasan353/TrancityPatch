using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Engine;

namespace Trancity
{
	public class ComputeTimeDialog : Form
	{
		private Button Cancel_Button;

		private ProgressBar _progressBar;

		private bool _refreshing;

		private string _text;

		private Timer _timer;

		private Игрок[] _игроки;

		private Игрок _игрок;

		private World _мир;

		private Trip _рейс;

		private Transport _транспорт;

		private bool b1;

		private IContainer components;

		public ComputeTimeDialog(World мир, int typeOfTransport, Trip рейс, Игрок игрок_камера)
		{
			InitializeComponent();
			Localization.ApplyLocalization(this);
			_мир = мир;
			_рейс = рейс;
			_игроки = new Игрок[0];
			_игрок = игрок_камера;
			Парк парк = new Парк("");
			Route route = new Route(typeOfTransport, "");
			route.orders = new Order[1]
			{
				new Order(парк, route, "", "")
			};
			route.orders[0].рейсы = new Trip[1] { рейс };
			рейс.InitTripStopList();
			if ((typeOfTransport == 2 && Модели.Автобусы.Count == 0) || (typeOfTransport == 1 && Модели.Троллейбусы.Count == 0) || (typeOfTransport == 0 && Модели.Трамваи.Count == 0))
			{
				MessageBox.Show(this, $"No models type {typeOfTransport.ToString()} found!", "Transedit", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			switch (typeOfTransport)
			{
			case 0:
				_транспорт = new Трамвай.ОбычныйТрамвай(Модели.Трамваи[Cheats._random.Next(0, Модели.Трамваи.Count)], (Рельс)рейс.дорога_отправления, 0.0, Управление.Автоматическое, парк, route, null);
				break;
			case 1:
			{
				Double3DPoint double3DPoint = new Double3DPoint
				{
					XZPoint = рейс.дорога_отправления.концы[0],
					y = рейс.дорога_отправления.высота[0]
				};
				_транспорт = new Троллейбус.ОбычныйТроллейбус(Модели.Троллейбусы[Cheats._random.Next(0, Модели.Троллейбусы.Count)], double3DPoint, new DoublePoint(рейс.дорога_отправления.направления[0], 0.0), Управление.Автоматическое, парк, route, null);
				_транспорт.SetPosition(рейс.дорога_отправления, 0.0, 0.0, double3DPoint, new DoublePoint(рейс.дорога_отправления.направления[0], 0.0), _мир);
				break;
			}
			case 2:
			{
				Double3DPoint координаты = new Double3DPoint
				{
					XZPoint = рейс.дорога_отправления.концы[0],
					y = рейс.дорога_отправления.высота[0]
				};
				_транспорт = new Троллейбус.ОбычныйТроллейбус(Модели.Автобусы[Cheats._random.Next(0, Модели.Автобусы.Count)], координаты, new DoublePoint(рейс.дорога_отправления.направления[0], 0.0), Управление.Автоматическое, парк, route, null);
				break;
			}
			}
			_транспорт.наряд = route.orders[0];
			_транспорт.рейс = рейс;
			мир.транспорты.Add(_транспорт);
			_progressBar.Maximum = (int)Math.Ceiling(рейс.длина_пути - рейс.дорога_прибытия.Длина);
			_timer.Enabled = true;
		}

		private void ComputeTime_Dialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			_timer.Enabled = false;
			_мир.транспорты.Remove(_транспорт);
			foreach (Положение item in _транспорт.найденные_положения)
			{
				if (item.Дорога != null)
				{
					item.Дорога.занятыеПоложения.Remove(item);
				}
			}
			if (_транспорт is Трамвай)
			{
				Трамвай.Ось[] все_оси = ((Трамвай)_транспорт).все_оси;
				foreach (Трамвай.Ось ось in все_оси)
				{
					ось.текущий_рельс.objects.Remove(ось);
				}
				Трамвай.Токоприёмник_new токоприёмник = ((Трамвай)_транспорт).токоприёмник;
				if (токоприёмник.Провод != null)
				{
					токоприёмник.Провод.objects.Remove(токоприёмник);
				}
			}
			else
			{
				if (!(_транспорт is Троллейбус))
				{
					return;
				}
				Троллейбус.Штанга[] штанги = ((Троллейбус)_транспорт).штанги;
				foreach (Троллейбус.Штанга штанга in штанги)
				{
					if (штанга.Провод != null)
					{
						штанга.Провод.objects.Remove(штанга);
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._timer = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(12, 32);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(745, 23);
			this.Cancel_Button.TabIndex = 0;
			this.Cancel_Button.Text = "Отмена";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			this._progressBar.Location = new System.Drawing.Point(12, 12);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(745, 14);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 1;
			this._timer.Interval = 1;
			this._timer.Tick += new System.EventHandler(timer_Tick);
			base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.Cancel_Button;
			base.ClientSize = new System.Drawing.Size(769, 67);
			base.ControlBox = false;
			base.Controls.Add(this._progressBar);
			base.Controls.Add(this.Cancel_Button);
			this.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ComputeTimeDialog";
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Вычисление времени рейса";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ComputeTime_Dialog_FormClosing);
			base.ResumeLayout(false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if (_refreshing)
			{
				return;
			}
			_refreshing = true;
			if (_транспорт.рейс_index == _рейс.pathes.Length - 1)
			{
				_timer.Enabled = false;
				base.DialogResult = DialogResult.OK;
				Close();
				return;
			}
			_транспорт.АвтоматическиУправлять(_мир);
			if (_транспорт.осталось_стоять > 0.0 && _транспорт.скорость == 0.0 && _транспорт.ускорение == 0.0)
			{
				_мир.системноеВремя -= _транспорт.осталось_стоять;
			}
			bool flag = false;
			if (_транспорт is Трамвай)
			{
				Рельс текущий_рельс = ((Трамвай)_транспорт).передняя_ось.текущий_рельс;
				if (текущий_рельс.следующие_рельсы.Length > 1 && _рейс.pathes[_транспорт.рейс_index] == текущий_рельс)
				{
					if (_рейс.pathes[_транспорт.рейс_index + 1] == текущий_рельс.следующие_рельсы[0])
					{
						текущий_рельс.следующий_рельс = 0;
					}
					else if (_рейс.pathes[_транспорт.рейс_index + 1] == текущий_рельс.следующие_рельсы[1])
					{
						текущий_рельс.следующий_рельс = 1;
					}
				}
			}
			else if (_транспорт is Троллейбус)
			{
				Троллейбус.Штанга[] штанги = ((Троллейбус)_транспорт).штанги;
				foreach (Троллейбус.Штанга штанга in штанги)
				{
					if (!штанга.Поднята)
					{
						штанга.НайтиПровод(_мир.контактныеПровода);
						штанга.поднимается = true;
						штанга.угол = штанга.уголNormal;
						if (!штанга.Поднята)
						{
							flag = true;
						}
					}
					else
					{
						b1 = true;
					}
				}
			}
			_мир.системноеВремя -= 0.275;
			_мир.Обновить(_игроки);
			string text = string.Format(". {0}: {1} {2}", Localization.current_.speed, (_транспорт.скорость * 3.6).ToString("0.00"), Localization.current_.speed_km);
			if (flag)
			{
				text = text + " (" + Localization.current_.shtangi_loosed + ")";
				if (b1)
				{
					_игрок.cameraPosition.XZPoint = _транспорт.position;
					b1 = false;
				}
			}
			if (_text == null)
			{
				_text = Text;
			}
			Text = _text + text;
			double num = 0.0;
			for (int j = 0; j < _транспорт.рейс_index; j++)
			{
				num += _рейс.pathes[j].Длина;
			}
			if (_транспорт.рейс_index > 0 || _транспорт.текущее_положение.Дорога == _рейс.дорога_отправления)
			{
				num += _транспорт.текущее_положение.расстояние;
			}
			_progressBar.Value = Math.Min((int)Math.Round(num), _progressBar.Maximum);
			_refreshing = false;
		}
	}
}

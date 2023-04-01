using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Trancity
{
	public class TimeBox : UserControl
	{
		private IContainer components;

		private int h;

		private int m;

		private int pos;

		private int s;

		private TextBox text_box;

		private bool view_seconds;

		public int Hours
		{
			get
			{
				return h;
			}
			set
			{
				while (value < 0)
				{
					value += 24;
				}
				h = value % 24;
				Redraw();
			}
		}

		public int Minutes
		{
			get
			{
				return m;
			}
			set
			{
				while (value < 0)
				{
					value += 60;
				}
				m = value % 60;
				Redraw();
			}
		}

		public int Seconds
		{
			get
			{
				return s;
			}
			set
			{
				while (value < 0)
				{
					value += 60;
				}
				s = value % 60;
				Redraw();
			}
		}

		public DateTime Time
		{
			get
			{
				return new DateTime(1, 1, 1, h, m, s);
			}
			set
			{
				h = value.Hour;
				m = value.Minute;
				s = value.Second;
				Redraw();
			}
		}

		public int Time_Minutes
		{
			get
			{
				return m + 60 * h;
			}
			set
			{
				while (value < 0)
				{
					value += 1440;
				}
				s = 0;
				m = value % 60;
				h = value / 60 % 24;
				Redraw();
			}
		}

		public int Time_Seconds
		{
			get
			{
				return s + 60 * (m + 60 * h);
			}
			set
			{
				while (value < 0)
				{
					value += 86400;
				}
				s = value % 60;
				m = value / 60 % 60;
				h = value / 3600 % 24;
				Redraw();
			}
		}

		public bool ViewSeconds
		{
			get
			{
				return view_seconds;
			}
			set
			{
				view_seconds = value;
				if (!value && pos >= 4)
				{
					pos = 2;
				}
				Redraw();
			}
		}

		public event EventHandler TimeChanged;

		public TimeBox()
		{
			InitializeComponent();
			base.EnabledChanged += TimeBox_EnabledChanged;
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
			this.text_box = new System.Windows.Forms.TextBox();
			base.SuspendLayout();
			this.text_box.BackColor = System.Drawing.SystemColors.Window;
			this.text_box.Dock = System.Windows.Forms.DockStyle.Top;
			this.text_box.Location = new System.Drawing.Point(0, 0);
			this.text_box.Name = "text_box";
			this.text_box.ReadOnly = true;
			this.text_box.Size = new System.Drawing.Size(100, 21);
			this.text_box.TabIndex = 0;
			this.text_box.Text = "00:00";
			this.text_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.text_box.DoubleClick += new System.EventHandler(text_box_DoubleClick);
			this.text_box.Click += new System.EventHandler(text_box_Click);
			this.text_box.Leave += new System.EventHandler(text_box_Leave);
			this.text_box.KeyPress += new System.Windows.Forms.KeyPressEventHandler(text_box_KeyPress);
			this.text_box.KeyDown += new System.Windows.Forms.KeyEventHandler(text_box_KeyDown);
			base.Controls.Add(this.text_box);
			this.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			this.MinimumSize = new System.Drawing.Size(40, 21);
			base.Name = "TimeBox";
			base.Size = new System.Drawing.Size(100, 21);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void Redraw()
		{
			text_box.Text = h.ToString("00") + ":" + m.ToString("00");
			if (view_seconds)
			{
				text_box.Text = text_box.Text + ":" + s.ToString("00");
			}
			text_box.SelectionStart = pos / 2 * 3;
			text_box.SelectionLength = 2;
		}

		private void text_box_Click(object sender, EventArgs e)
		{
			pos = text_box.SelectionStart / 3 * 2;
			Redraw();
		}

		private void text_box_DoubleClick(object sender, EventArgs e)
		{
			Redraw();
		}

		private void text_box_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
			{
				int num = ((e.KeyCode == Keys.Up) ? 1 : (-1));
				pos = pos / 2 * 2;
				switch (pos)
				{
				case 0:
					Hours += num;
					break;
				case 2:
					Minutes += num;
					break;
				case 4:
					Seconds += num;
					break;
				}
				if (this.TimeChanged != null)
				{
					this.TimeChanged(this, new EventArgs());
				}
			}
			if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
			{
				int num2 = ((e.KeyCode == Keys.Right) ? 1 : (-1));
				pos = (pos / 2 + num2) * 2;
				if (pos >= 6 || (pos >= 4 && !view_seconds))
				{
					pos = 0;
				}
				if (pos < 0)
				{
					pos = (view_seconds ? 4 : 2);
				}
			}
			Redraw();
		}

		private void text_box_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar < '0' || e.KeyChar > '9')
			{
				return;
			}
			int num = e.KeyChar - 48;
			switch (pos)
			{
			case 0:
				h = num;
				break;
			case 1:
				h = 10 * h + num;
				if (h > 23)
				{
					h = 23;
				}
				break;
			case 2:
				m = num;
				break;
			case 3:
				m = 10 * m + num;
				if (m > 59)
				{
					m = 59;
				}
				break;
			case 4:
				s = num;
				break;
			case 5:
				s = 10 * s + num;
				if (s > 59)
				{
					s = 59;
				}
				break;
			}
			pos++;
			if (pos >= 6 || (pos >= 4 && !view_seconds))
			{
				pos = 0;
			}
			Redraw();
			if (this.TimeChanged != null)
			{
				this.TimeChanged(this, new EventArgs());
			}
		}

		private void text_box_Leave(object sender, EventArgs e)
		{
			pos = pos / 2 * 2;
		}

		private void TimeBox_EnabledChanged(object sender, EventArgs e)
		{
			text_box.Enabled = base.Enabled;
			if (base.Enabled)
			{
				text_box.BackColor = SystemColors.Window;
				text_box.ForeColor = SystemColors.WindowText;
			}
			else
			{
				text_box.BackColor = SystemColors.Control;
				text_box.ForeColor = SystemColors.ControlDark;
			}
		}
	}
}

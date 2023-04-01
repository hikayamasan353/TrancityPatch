using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Trancity
{
	public class UserControlForm : Form
	{
		private IContainer components;

		private Button Reset_Button;

		private ListBox KeysListBox;

		private Button Close_Button;

		private Button Save_Button;

		public UserControlForm()
		{
			InitializeComponent();
			Localization.ApplyLocalization(this);
		}

		private void UpdateListBox()
		{
		}

		private void Save_ButtonClick(object sender, EventArgs e)
		{
		}

		private void Close_ButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		private void Reset_ButtonClick(object sender, EventArgs e)
		{
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
			this.Save_Button = new System.Windows.Forms.Button();
			this.Close_Button = new System.Windows.Forms.Button();
			this.KeysListBox = new System.Windows.Forms.ListBox();
			this.Reset_Button = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.Save_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Save_Button.Location = new System.Drawing.Point(12, 380);
			this.Save_Button.Name = "Save_Button";
			this.Save_Button.Size = new System.Drawing.Size(115, 35);
			this.Save_Button.TabIndex = 0;
			this.Save_Button.Text = "Сохранить";
			this.Save_Button.UseVisualStyleBackColor = true;
			this.Save_Button.Click += new System.EventHandler(Save_ButtonClick);
			this.Close_Button.Location = new System.Drawing.Point(342, 380);
			this.Close_Button.Name = "Close_Button";
			this.Close_Button.Size = new System.Drawing.Size(115, 35);
			this.Close_Button.TabIndex = 0;
			this.Close_Button.Text = "Закрыть";
			this.Close_Button.UseVisualStyleBackColor = true;
			this.Close_Button.Click += new System.EventHandler(Close_ButtonClick);
			this.KeysListBox.FormattingEnabled = true;
			this.KeysListBox.Location = new System.Drawing.Point(12, 12);
			this.KeysListBox.Name = "KeysListBox";
			this.KeysListBox.Size = new System.Drawing.Size(445, 355);
			this.KeysListBox.TabIndex = 1;
			this.Reset_Button.Location = new System.Drawing.Point(133, 391);
			this.Reset_Button.Name = "Reset_Button";
			this.Reset_Button.Size = new System.Drawing.Size(88, 24);
			this.Reset_Button.TabIndex = 0;
			this.Reset_Button.Text = "Восстановить";
			this.Reset_Button.UseVisualStyleBackColor = true;
			this.Reset_Button.Click += new System.EventHandler(Reset_ButtonClick);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(469, 427);
			base.Controls.Add(this.KeysListBox);
			base.Controls.Add(this.Reset_Button);
			base.Controls.Add(this.Close_Button);
			base.Controls.Add(this.Save_Button);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UserControlForm";
			base.ShowIcon = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Управление";
			base.ResumeLayout(false);
		}
	}
}

/*
 * Created by SharpDevelop.
 * User: sergey
 * Date: 12.04.2015
 * Time: 14:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Common
{
	partial class DeviceOptionsDialog
	{
		private void InitializeComponent()
        {
            this.Fullscreen_Radio = new System.Windows.Forms.RadioButton();
            this.Windowed_Radio = new System.Windows.Forms.RadioButton();
            this.ChooseDevice_Label = new System.Windows.Forms.Label();
            this.AdapterMode_ComboBox = new System.Windows.Forms.ComboBox();
            this.Size_x_UpDown = new System.Windows.Forms.NumericUpDown();
            this.Size_y_UpDown = new System.Windows.Forms.NumericUpDown();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.VSync_CheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Size_x_UpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Size_y_UpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Fullscreen_Radio
            // 
            this.Fullscreen_Radio.Location = new System.Drawing.Point(23, 61);
            this.Fullscreen_Radio.Name = "Fullscreen_Radio";
            this.Fullscreen_Radio.Size = new System.Drawing.Size(180, 25);
            this.Fullscreen_Radio.TabIndex = 0;
            this.Fullscreen_Radio.Text = "Полноэранный";
            this.Fullscreen_Radio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // Windowed_Radio
            // 
            this.Windowed_Radio.Location = new System.Drawing.Point(23, 92);
            this.Windowed_Radio.Name = "Windowed_Radio";
            this.Windowed_Radio.Size = new System.Drawing.Size(180, 26);
            this.Windowed_Radio.TabIndex = 1;
            this.Windowed_Radio.Text = "Оконный";
            this.Windowed_Radio.CheckedChanged += new System.EventHandler(this.Radio_CheckedChanged);
            // 
            // ChooseDevice_Label
            // 
            this.ChooseDevice_Label.AutoSize = true;
            this.ChooseDevice_Label.Location = new System.Drawing.Point(19, 21);
            this.ChooseDevice_Label.Name = "ChooseDevice_Label";
            this.ChooseDevice_Label.Size = new System.Drawing.Size(137, 17);
            this.ChooseDevice_Label.TabIndex = 2;
            this.ChooseDevice_Label.Text = "Выберите режим:";
            // 
            // AdapterMode_ComboBox
            // 
            this.AdapterMode_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AdapterMode_ComboBox.Location = new System.Drawing.Point(211, 62);
            this.AdapterMode_ComboBox.Name = "AdapterMode_ComboBox";
            this.AdapterMode_ComboBox.Size = new System.Drawing.Size(170, 25);
            this.AdapterMode_ComboBox.TabIndex = 3;
            this.AdapterMode_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Check_OK_Available);
            // 
            // Size_x_UpDown
            // 
            this.Size_x_UpDown.Increment = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.Size_x_UpDown.Location = new System.Drawing.Point(211, 95);
            this.Size_x_UpDown.Maximum = new decimal(new int[] {
            4000,
            0,
            0,
            0});
            this.Size_x_UpDown.Minimum = new decimal(new int[] {
            320,
            0,
            0,
            0});
            this.Size_x_UpDown.Name = "Size_x_UpDown";
            this.Size_x_UpDown.Size = new System.Drawing.Size(81, 24);
            this.Size_x_UpDown.TabIndex = 6;
            this.Size_x_UpDown.Value = new decimal(new int[] {
            320,
            0,
            0,
            0});
            // 
            // Size_y_UpDown
            // 
            this.Size_y_UpDown.Increment = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.Size_y_UpDown.Location = new System.Drawing.Point(300, 95);
            this.Size_y_UpDown.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.Size_y_UpDown.Minimum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this.Size_y_UpDown.Name = "Size_y_UpDown";
            this.Size_y_UpDown.Size = new System.Drawing.Size(81, 24);
            this.Size_y_UpDown.TabIndex = 7;
            this.Size_y_UpDown.Value = new decimal(new int[] {
            240,
            0,
            0,
            0});
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK_Button.Location = new System.Drawing.Point(62, 125);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(100, 28);
            this.OK_Button.TabIndex = 8;
            this.OK_Button.Text = "OK";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(179, 125);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(100, 28);
            this.Cancel_Button.TabIndex = 8;
            this.Cancel_Button.Text = "Cancel";
            // 
            // VSync_CheckBox
            // 
            this.VSync_CheckBox.Location = new System.Drawing.Point(23, 128);
            this.VSync_CheckBox.Name = "VSync_CheckBox";
            this.VSync_CheckBox.Size = new System.Drawing.Size(180, 29);
            this.VSync_CheckBox.TabIndex = 9;
            this.VSync_CheckBox.Text = "VSync";
            this.VSync_CheckBox.UseVisualStyleBackColor = true;
            // 
            // DeviceOptionsDialog
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 17);
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(306, 178);
            this.ControlBox = false;
            this.Controls.Add(this.VSync_CheckBox);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.Size_y_UpDown);
            this.Controls.Add(this.Size_x_UpDown);
            this.Controls.Add(this.AdapterMode_ComboBox);
            this.Controls.Add(this.ChooseDevice_Label);
            this.Controls.Add(this.Windowed_Radio);
            this.Controls.Add(this.Fullscreen_Radio);
            this.Controls.Add(this.Cancel_Button);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DeviceOptionsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки оборудования";
            ((System.ComponentModel.ISupportInitialize)(this.Size_x_UpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Size_y_UpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		private System.Windows.Forms.CheckBox VSync_CheckBox;
		private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Label ChooseDevice_Label;
        private System.Windows.Forms.RadioButton Fullscreen_Radio;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.ComboBox AdapterMode_ComboBox;
        private System.Windows.Forms.NumericUpDown Size_x_UpDown;
        private System.Windows.Forms.NumericUpDown Size_y_UpDown;
        private System.Windows.Forms.RadioButton Windowed_Radio;
	}
}

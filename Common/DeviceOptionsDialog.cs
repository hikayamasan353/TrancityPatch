using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Engine;

namespace Common
{
	public class DeviceOptionsDialog : Form
	{
		private CheckBox VSync_CheckBox;

		private Button Cancel_Button;

		private Label ChooseDevice_Label;

		private RadioButton Fullscreen_Radio;

		private Button OK_Button;

		private ComboBox AdapterMode_ComboBox;

		private NumericUpDown Size_x_UpDown;

		private NumericUpDown Size_y_UpDown;

		private RadioButton Windowed_Radio;

		private BindingSource ScreenMode_BindingSource;

		private IContainer components;

		private ComboBox Devices_ComboBox;

		private BindingSource Devices_BindingSource;

		private ComboBox VertexProcessing_ComboBox;

		private Label VertexProcessing_Label;

		private BindingSource VertexProcessing_BindingSource;

		public string filename;

		public DeviceOptions subj;

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Fullscreen_Radio = new System.Windows.Forms.RadioButton();
			this.Windowed_Radio = new System.Windows.Forms.RadioButton();
			this.ChooseDevice_Label = new System.Windows.Forms.Label();
			this.AdapterMode_ComboBox = new System.Windows.Forms.ComboBox();
			this.ScreenMode_BindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.Size_x_UpDown = new System.Windows.Forms.NumericUpDown();
			this.Size_y_UpDown = new System.Windows.Forms.NumericUpDown();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.VSync_CheckBox = new System.Windows.Forms.CheckBox();
			this.Devices_ComboBox = new System.Windows.Forms.ComboBox();
			this.Devices_BindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.VertexProcessing_ComboBox = new System.Windows.Forms.ComboBox();
			this.VertexProcessing_Label = new System.Windows.Forms.Label();
			this.VertexProcessing_BindingSource = new System.Windows.Forms.BindingSource(this.components);
			((System.ComponentModel.ISupportInitialize)this.ScreenMode_BindingSource).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Size_x_UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Size_y_UpDown).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.Devices_BindingSource).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.VertexProcessing_BindingSource).BeginInit();
			base.SuspendLayout();
			this.Fullscreen_Radio.Location = new System.Drawing.Point(17, 50);
			this.Fullscreen_Radio.Name = "Fullscreen_Radio";
			this.Fullscreen_Radio.Size = new System.Drawing.Size(135, 21);
			this.Fullscreen_Radio.TabIndex = 0;
			this.Fullscreen_Radio.Text = "Fullscreen";
			this.Fullscreen_Radio.CheckedChanged += new System.EventHandler(Radio_CheckedChanged);
			this.Windowed_Radio.Location = new System.Drawing.Point(17, 76);
			this.Windowed_Radio.Name = "Windowed_Radio";
			this.Windowed_Radio.Size = new System.Drawing.Size(135, 21);
			this.Windowed_Radio.TabIndex = 1;
			this.Windowed_Radio.Text = "Windowed";
			this.Windowed_Radio.CheckedChanged += new System.EventHandler(Radio_CheckedChanged);
			this.ChooseDevice_Label.AutoSize = true;
			this.ChooseDevice_Label.Location = new System.Drawing.Point(14, 17);
			this.ChooseDevice_Label.Name = "ChooseDevice_Label";
			this.ChooseDevice_Label.Size = new System.Drawing.Size(98, 13);
			this.ChooseDevice_Label.TabIndex = 2;
			this.ChooseDevice_Label.Text = "Choose Device:";
			this.AdapterMode_ComboBox.DataSource = this.ScreenMode_BindingSource;
			this.AdapterMode_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.AdapterMode_ComboBox.Location = new System.Drawing.Point(158, 51);
			this.AdapterMode_ComboBox.Name = "AdapterMode_ComboBox";
			this.AdapterMode_ComboBox.Size = new System.Drawing.Size(128, 21);
			this.AdapterMode_ComboBox.TabIndex = 3;
			this.AdapterMode_ComboBox.SelectedIndexChanged += new System.EventHandler(Check_OK_Available);
			this.Size_x_UpDown.Increment = new decimal(new int[4] { 40, 0, 0, 0 });
			this.Size_x_UpDown.Location = new System.Drawing.Point(158, 78);
			this.Size_x_UpDown.Maximum = new decimal(new int[4] { 4000, 0, 0, 0 });
			this.Size_x_UpDown.Minimum = new decimal(new int[4] { 320, 0, 0, 0 });
			this.Size_x_UpDown.Name = "Size_x_UpDown";
			this.Size_x_UpDown.Size = new System.Drawing.Size(61, 21);
			this.Size_x_UpDown.TabIndex = 6;
			this.Size_x_UpDown.Value = new decimal(new int[4] { 320, 0, 0, 0 });
			this.Size_y_UpDown.Increment = new decimal(new int[4] { 30, 0, 0, 0 });
			this.Size_y_UpDown.Location = new System.Drawing.Point(225, 78);
			this.Size_y_UpDown.Maximum = new decimal(new int[4] { 3000, 0, 0, 0 });
			this.Size_y_UpDown.Minimum = new decimal(new int[4] { 240, 0, 0, 0 });
			this.Size_y_UpDown.Name = "Size_y_UpDown";
			this.Size_y_UpDown.Size = new System.Drawing.Size(61, 21);
			this.Size_y_UpDown.TabIndex = 7;
			this.Size_y_UpDown.Value = new decimal(new int[4] { 240, 0, 0, 0 });
			this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Location = new System.Drawing.Point(123, 148);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(75, 23);
			this.OK_Button.TabIndex = 8;
			this.OK_Button.Text = "OK";
			this.OK_Button.Click += new System.EventHandler(OK_Button_Click);
			this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(211, 148);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
			this.Cancel_Button.TabIndex = 8;
			this.Cancel_Button.Text = "Cancel";
			this.VSync_CheckBox.Location = new System.Drawing.Point(14, 124);
			this.VSync_CheckBox.Name = "VSync_CheckBox";
			this.VSync_CheckBox.Size = new System.Drawing.Size(135, 24);
			this.VSync_CheckBox.TabIndex = 9;
			this.VSync_CheckBox.Text = "VSync";
			this.VSync_CheckBox.UseVisualStyleBackColor = true;
			this.Devices_ComboBox.DataSource = this.Devices_BindingSource;
			this.Devices_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Devices_ComboBox.Location = new System.Drawing.Point(158, 14);
			this.Devices_ComboBox.Name = "Devices_ComboBox";
			this.Devices_ComboBox.Size = new System.Drawing.Size(128, 21);
			this.Devices_ComboBox.TabIndex = 10;
			this.Devices_BindingSource.CurrentChanged += new System.EventHandler(Devices_BindingSourceCurrentChanged);
			this.VertexProcessing_ComboBox.DataSource = this.VertexProcessing_BindingSource;
			this.VertexProcessing_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.VertexProcessing_ComboBox.Location = new System.Drawing.Point(158, 105);
			this.VertexProcessing_ComboBox.Name = "VertexProcessing_ComboBox";
			this.VertexProcessing_ComboBox.Size = new System.Drawing.Size(128, 21);
			this.VertexProcessing_ComboBox.TabIndex = 12;
			this.VertexProcessing_Label.AutoSize = true;
			this.VertexProcessing_Label.Location = new System.Drawing.Point(14, 108);
			this.VertexProcessing_Label.Name = "VertexProcessing_Label";
			this.VertexProcessing_Label.Size = new System.Drawing.Size(115, 13);
			this.VertexProcessing_Label.TabIndex = 11;
			this.VertexProcessing_Label.Text = "Vertex processing:";
			this.VertexProcessing_BindingSource.AllowNew = false;
			base.AcceptButton = this.OK_Button;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			base.CancelButton = this.Cancel_Button;
			base.ClientSize = new System.Drawing.Size(306, 192);
			base.ControlBox = false;
			base.Controls.Add(this.VertexProcessing_ComboBox);
			base.Controls.Add(this.VertexProcessing_Label);
			base.Controls.Add(this.Devices_ComboBox);
			base.Controls.Add(this.VSync_CheckBox);
			base.Controls.Add(this.OK_Button);
			base.Controls.Add(this.Size_y_UpDown);
			base.Controls.Add(this.Size_x_UpDown);
			base.Controls.Add(this.AdapterMode_ComboBox);
			base.Controls.Add(this.ChooseDevice_Label);
			base.Controls.Add(this.Windowed_Radio);
			base.Controls.Add(this.Fullscreen_Radio);
			base.Controls.Add(this.Cancel_Button);
			this.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "DeviceOptionsDialog";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Device Options";
			((System.ComponentModel.ISupportInitialize)this.ScreenMode_BindingSource).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Size_x_UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Size_y_UpDown).EndInit();
			((System.ComponentModel.ISupportInitialize)this.Devices_BindingSource).EndInit();
			((System.ComponentModel.ISupportInitialize)this.VertexProcessing_BindingSource).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public DeviceOptionsDialog(string filename)
		{
			InitializeComponent();
			this.filename = filename;
			subj = LoadDeviceOptions(filename);
			Devices_BindingSource.DataSource = RenderSystem.EnumerateDevices();
			AdapterMode_ComboBox.SelectedIndex = 0;
			Size_x_UpDown.Value = subj.windowedX;
			Size_y_UpDown.Value = subj.windowedY;
			Windowed_Radio.Checked = subj.windowed;
			Fullscreen_Radio.Checked = !subj.windowed;
			VSync_CheckBox.Checked = subj.vSync;
			Check_OK_Available(this, new EventArgs());
		}

		public static DeviceOptions LoadDeviceOptions(string filename)
		{
			DeviceOptions result = default(DeviceOptions);
			XmlElement child = Xml.GetChild(Xml.TryOpenDocument(filename), "Options");
			result.adapterID = (int)Xml.GetDouble(Xml.GetChild(child, "adapter_id"), 0.0);
			result.deviceType = (int)Xml.GetDouble(Xml.GetChild(child, "device_type"), 0.0);
			result.vertexProcessingMode = (int)Xml.GetDouble(Xml.GetChild(child, "vertexProcessingMode"), 0.0);
			result.vSync = Xml.GetDouble(Xml.GetChild(child, "vSync"), 0.0) > 0.0;
			result.fullscreenX = (int)Xml.GetDouble(Xml.GetChild(child, "fullscreen_x"), 640.0);
			result.fullscreenY = (int)Xml.GetDouble(Xml.GetChild(child, "fullscreen_y"), 480.0);
			result.windowedX = (int)Xml.GetDouble(Xml.GetChild(child, "windowed_x"), 640.0);
			result.windowedY = (int)Xml.GetDouble(Xml.GetChild(child, "windowed_y"), 480.0);
			result.windowed = Xml.GetDouble(Xml.GetChild(child, "windowed")) > 0.0;
			return result;
		}

		public static void SaveDeviceOptions(DeviceOptions options, string filename)
		{
			XmlDocument document = new XmlDocument();
			XmlElement xmlElement = Xml.AddElement(document, "Options");
			Xml.AddElement(document, xmlElement, "adapter_id", options.adapterID);
			Xml.AddElement(document, xmlElement, "device_type", options.deviceType);
			Xml.AddElement(document, xmlElement, "vertexProcessingMode", options.vertexProcessingMode);
			Xml.AddElement(document, xmlElement, "vSync", options.vSync ? 1 : 0);
			Xml.AddElement(document, xmlElement, "fullscreen_x", options.fullscreenX);
			Xml.AddElement(document, xmlElement, "fullscreen_y", options.fullscreenY);
			Xml.AddElement(document, xmlElement, "windowed_x", options.windowedX);
			Xml.AddElement(document, xmlElement, "windowed_y", options.windowedY);
			Xml.AddElement(document, xmlElement, "windowed", options.windowed ? 1 : 0);
			Xml.TrySaveDocument(document, filename);
		}

		private void Check_OK_Available(object sender, EventArgs e)
		{
			OK_Button.Enabled = !Fullscreen_Radio.Checked || AdapterMode_ComboBox.SelectedIndex >= 0;
		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			ScreenMode screenMode = (ScreenMode)ScreenMode_BindingSource.Current;
			VertexProcessingMode vertexProcessingMode = (VertexProcessingMode)VertexProcessing_BindingSource.Current;
			subj.adapterID = Devices_BindingSource.Position;
			subj.fullscreenX = screenMode.Width;
			subj.fullscreenY = screenMode.Height;
			subj.windowedX = (int)Size_x_UpDown.Value;
			subj.windowedY = (int)Size_y_UpDown.Value;
			subj.windowed = Windowed_Radio.Checked;
			subj.vSync = VSync_CheckBox.Checked;
			subj.vertexProcessingMode = (int)vertexProcessingMode;
			SaveDeviceOptions(subj, filename);
		}

		private void Radio_CheckedChanged(object sender, EventArgs e)
		{
			AdapterMode_ComboBox.Enabled = Fullscreen_Radio.Checked;
			Size_x_UpDown.Enabled = Windowed_Radio.Checked;
			Size_y_UpDown.Enabled = Windowed_Radio.Checked;
			Check_OK_Available(sender, e);
		}

		private void Devices_BindingSourceCurrentChanged(object sender, EventArgs e)
		{
			DeviceInfo device = (DeviceInfo)(sender as BindingSource).Current;
			RefreshScreenModes(device);
			RefreshVertexProcessingTypes(device);
		}

		private void RefreshScreenModes(DeviceInfo device)
		{
			ScreenMode[] screenModes = device.ScreenModes;
			ScreenMode_BindingSource.DataSource = screenModes;
			ScreenMode screenMode = new ScreenMode(subj.fullscreenX, subj.fullscreenY);
			for (int i = 0; i < screenModes.Length; i++)
			{
				if (screenMode.Equals(screenModes[i]))
				{
					ScreenMode_BindingSource.Position = i;
					return;
				}
			}
			ScreenMode_BindingSource.Position = screenModes.Length - 1;
		}

		private void RefreshVertexProcessingTypes(DeviceInfo device)
		{
			VertexProcessingMode[] vertexProcessingModes = device.VertexProcessingModes;
			VertexProcessing_BindingSource.DataSource = vertexProcessingModes;
			VertexProcessingMode vertexProcessingMode;
			try
			{
				vertexProcessingMode = (VertexProcessingMode)subj.vertexProcessingMode;
			}
			catch
			{
				vertexProcessingMode = VertexProcessingMode.Hardware;
			}
			for (int i = 0; i < vertexProcessingModes.Length; i++)
			{
				if (vertexProcessingMode == vertexProcessingModes[i])
				{
					VertexProcessing_BindingSource.Position = i;
					break;
				}
			}
		}
	}
}

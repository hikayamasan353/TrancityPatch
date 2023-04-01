using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Trancity
{
	public class StopListForm : Form
	{
		private class TripStopBindingList : BindingList<TripStop.Editable>
		{
			public TripStopBindingList()
			{
			}

			public TripStopBindingList(List<TripStop> tripStopList)
			{
				if (tripStopList != null)
				{
					foreach (TripStop tripStop in tripStopList)
					{
						Add(TripStop.Editable.FromTripStop(tripStop));
					}
				}
				base.AllowNew = false;
				base.AllowRemove = false;
			}

			public List<TripStop> ToList()
			{
				List<TripStop> list = new List<TripStop>(base.Count);
				foreach (TripStop.Editable item in base.Items)
				{
					list.Add(item.ToTripStop());
				}
				return list;
			}
		}

		private Trip trip;

		private bool changed;

		private IContainer components;

		private Button Update_Button;

		private Button Clear_Button;

		private Button OK_Button;

		private Button Cancel_Button;

		private BindingSource TripStopList_BindingSource;

		private DataGridView TripStopList_DataGridView;

		private DataGridViewCheckBoxColumn ShouldStop;

		private DataGridViewTextBoxColumn StopName;

		private Trip Trip
		{
			set
			{
				trip = value;
				List<TripStop> list = value.tripStopList;
				if (list == null)
				{
					list = trip.AddToTripStopList();
					changed = true;
				}
				TripStopList_BindingSource.DataSource = new TripStopBindingList(list);
				TripStopList_BindingSource.ListChanged += TripStopList_BindingSourceListChanged;
			}
		}

		public List<TripStop> TripStopList => ((TripStopBindingList)TripStopList_BindingSource.DataSource).ToList();

		public bool Changed => changed;

		public StopListForm(Trip _trip)
		{
			InitializeComponent();
			Localization.ApplyLocalization(this);
			Trip = _trip;
			TripStopList_DataGridView.DataSource = TripStopList_BindingSource;
		}

		private void UpdateClick(object sender, EventArgs e)
		{
			TripStopList_BindingSource.DataSource = new TripStopBindingList(trip.AddToTripStopList());
			changed = true;
		}

		private void ClearClick(object sender, EventArgs e)
		{
			TripStopList_BindingSource.DataSource = new TripStopBindingList();
			changed = true;
		}

		private void TripStopList_BindingSourceListChanged(object sender, ListChangedEventArgs e)
		{
			changed = true;
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
			this.components = new System.ComponentModel.Container();
			this.Update_Button = new System.Windows.Forms.Button();
			this.Clear_Button = new System.Windows.Forms.Button();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.TripStopList_BindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.TripStopList_DataGridView = new System.Windows.Forms.DataGridView();
			this.ShouldStop = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.StopName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)this.TripStopList_BindingSource).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.TripStopList_DataGridView).BeginInit();
			base.SuspendLayout();
			this.Update_Button.Location = new System.Drawing.Point(11, 368);
			this.Update_Button.Name = "Update_Button";
			this.Update_Button.Size = new System.Drawing.Size(136, 23);
			this.Update_Button.TabIndex = 1;
			this.Update_Button.Text = "Обновить";
			this.Update_Button.UseVisualStyleBackColor = true;
			this.Update_Button.Click += new System.EventHandler(UpdateClick);
			this.Clear_Button.Location = new System.Drawing.Point(147, 368);
			this.Clear_Button.Name = "Clear_Button";
			this.Clear_Button.Size = new System.Drawing.Size(136, 23);
			this.Clear_Button.TabIndex = 2;
			this.Clear_Button.Text = "Очистить";
			this.Clear_Button.UseVisualStyleBackColor = true;
			this.Clear_Button.Click += new System.EventHandler(ClearClick);
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Location = new System.Drawing.Point(11, 397);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(136, 23);
			this.OK_Button.TabIndex = 3;
			this.OK_Button.Text = "OK";
			this.OK_Button.UseVisualStyleBackColor = true;
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(147, 397);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(136, 23);
			this.Cancel_Button.TabIndex = 4;
			this.Cancel_Button.Text = "Cancel";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			this.TripStopList_BindingSource.AllowNew = false;
			this.TripStopList_DataGridView.AllowUserToResizeColumns = false;
			this.TripStopList_DataGridView.AllowUserToResizeRows = false;
			this.TripStopList_DataGridView.AutoGenerateColumns = false;
			this.TripStopList_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.TripStopList_DataGridView.Columns.AddRange(this.ShouldStop, this.StopName);
			this.TripStopList_DataGridView.DataSource = this.TripStopList_BindingSource;
			this.TripStopList_DataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.TripStopList_DataGridView.Location = new System.Drawing.Point(11, 11);
			this.TripStopList_DataGridView.MultiSelect = false;
			this.TripStopList_DataGridView.Name = "TripStopList_DataGridView";
			this.TripStopList_DataGridView.RowHeadersVisible = false;
			this.TripStopList_DataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.TripStopList_DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.TripStopList_DataGridView.ShowEditingIcon = false;
			this.TripStopList_DataGridView.Size = new System.Drawing.Size(272, 351);
			this.TripStopList_DataGridView.TabIndex = 0;
			this.ShouldStop.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ShouldStop.DataPropertyName = "ShouldStop";
			this.ShouldStop.HeaderText = "";
			this.ShouldStop.Name = "ShouldStop";
			this.ShouldStop.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ShouldStop.Width = 50;
			this.StopName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.StopName.DataPropertyName = "StopName";
			this.StopName.HeaderText = "";
			this.StopName.Name = "StopName";
			this.StopName.ReadOnly = true;
			this.StopName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.AllowDrop = true;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(294, 425);
			base.Controls.Add(this.TripStopList_DataGridView);
			base.Controls.Add(this.Cancel_Button);
			base.Controls.Add(this.OK_Button);
			base.Controls.Add(this.Clear_Button);
			base.Controls.Add(this.Update_Button);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "StopListForm";
			base.Padding = new System.Windows.Forms.Padding(8);
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Остановки";
			((System.ComponentModel.ISupportInitialize)this.TripStopList_BindingSource).EndInit();
			((System.ComponentModel.ISupportInitialize)this.TripStopList_DataGridView).EndInit();
			base.ResumeLayout(false);
		}
	}
}

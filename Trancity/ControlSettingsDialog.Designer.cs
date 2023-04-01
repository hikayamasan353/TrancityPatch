namespace Trancity
{
    partial class ControlSettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Управление трамваем", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Управление троллейбусом", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Управление автобусом", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Руль влево",
            "Стрелка влево"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Руль вправо");
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Газ");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Тормоз");
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Руль влево");
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Руль вправо");
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Педаль хода");
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem("Педаль тормоза");
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem("Ход");
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem("Тормоз");
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem("Токоприемник");
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem("Штанги");
            this.ControlsConfig_ListView = new System.Windows.Forms.ListView();
            this.columnHeader_Action = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_Key = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // ControlsConfig_ListView
            // 
            this.ControlsConfig_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_Action,
            this.columnHeader_Key});
            listViewGroup1.Header = "Управление трамваем";
            listViewGroup1.Name = "listViewGroup_Tram";
            listViewGroup2.Header = "Управление троллейбусом";
            listViewGroup2.Name = "listViewGroup_Trolleybus";
            listViewGroup3.Header = "Управление автобусом";
            listViewGroup3.Name = "listViewGroup_Bus";
            this.ControlsConfig_ListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.ControlsConfig_ListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ControlsConfig_ListView.HideSelection = false;
            listViewItem1.Group = listViewGroup3;
            listViewItem2.Group = listViewGroup3;
            listViewItem3.Group = listViewGroup3;
            listViewItem4.Group = listViewGroup3;
            listViewItem5.Group = listViewGroup2;
            listViewItem6.Group = listViewGroup2;
            listViewItem7.Group = listViewGroup2;
            listViewItem8.Group = listViewGroup2;
            listViewItem9.Group = listViewGroup1;
            listViewItem10.Group = listViewGroup1;
            listViewItem11.Group = listViewGroup1;
            listViewItem12.Group = listViewGroup2;
            this.ControlsConfig_ListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12});
            this.ControlsConfig_ListView.Location = new System.Drawing.Point(13, 13);
            this.ControlsConfig_ListView.MultiSelect = false;
            this.ControlsConfig_ListView.Name = "ControlsConfig_ListView";
            this.ControlsConfig_ListView.Size = new System.Drawing.Size(515, 425);
            this.ControlsConfig_ListView.TabIndex = 0;
            this.ControlsConfig_ListView.UseCompatibleStateImageBehavior = false;
            this.ControlsConfig_ListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_Action
            // 
            this.columnHeader_Action.Text = "Действие";
            this.columnHeader_Action.Width = 140;
            // 
            // columnHeader_Key
            // 
            this.columnHeader_Key.Text = "Клавиша";
            this.columnHeader_Key.Width = 132;
            // 
            // ControlSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 465);
            this.Controls.Add(this.ControlsConfig_ListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ControlSettingsDialog";
            this.Text = "Настройки управления";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ControlsConfig_ListView;
        private System.Windows.Forms.ColumnHeader columnHeader_Action;
        private System.Windows.Forms.ColumnHeader columnHeader_Key;
    }
}

namespace Game
{
    partial class FormSelectUnidentifiedItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectUnidentifiedItem));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listViewEquip = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(303, 172);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(82, 22);
            this.buttonCancel.TabIndex = 37;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // listViewEquip
            // 
            this.listViewEquip.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName});
            this.listViewEquip.FullRowSelect = true;
            this.listViewEquip.GridLines = true;
            this.listViewEquip.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewEquip.Location = new System.Drawing.Point(12, 11);
            this.listViewEquip.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listViewEquip.MultiSelect = false;
            this.listViewEquip.Name = "listViewEquip";
            this.listViewEquip.Size = new System.Drawing.Size(373, 158);
            this.listViewEquip.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewEquip.TabIndex = 36;
            this.listViewEquip.UseCompatibleStateImageBehavior = false;
            this.listViewEquip.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 320;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(216, 172);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(82, 22);
            this.buttonOk.TabIndex = 35;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FormSelectUnidentifiedItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 204);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listViewEquip);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectUnidentifiedItem";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Unidentified Items";
            this.Load += new System.EventHandler(this.FormSelectUnidentifiedItem_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView listViewEquip;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.Button buttonOk;
    }
}
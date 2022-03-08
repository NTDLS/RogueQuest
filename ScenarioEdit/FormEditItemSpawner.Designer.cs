
namespace ScenarioEdit
{
    partial class FormEditItemSpawner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditItemSpawner));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.listViewContainer = new System.Windows.Forms.ListView();
            this.columnHeaderSubType = new System.Windows.Forms.ColumnHeader();
            this.buttonNone = new System.Windows.Forms.Button();
            this.buttonAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Spawn Types";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(187, 408);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(86, 31);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(94, 409);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(86, 31);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // listViewContainer
            // 
            this.listViewContainer.AllowDrop = true;
            this.listViewContainer.CheckBoxes = true;
            this.listViewContainer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderSubType});
            this.listViewContainer.FullRowSelect = true;
            this.listViewContainer.GridLines = true;
            this.listViewContainer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewContainer.HideSelection = false;
            this.listViewContainer.Location = new System.Drawing.Point(14, 36);
            this.listViewContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listViewContainer.MultiSelect = false;
            this.listViewContainer.Name = "listViewContainer";
            this.listViewContainer.Size = new System.Drawing.Size(259, 364);
            this.listViewContainer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewContainer.TabIndex = 8;
            this.listViewContainer.UseCompatibleStateImageBehavior = false;
            this.listViewContainer.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderSubType
            // 
            this.columnHeaderSubType.Text = "SubType";
            this.columnHeaderSubType.Width = 250;
            // 
            // buttonNone
            // 
            this.buttonNone.Location = new System.Drawing.Point(279, 75);
            this.buttonNone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonNone.Name = "buttonNone";
            this.buttonNone.Size = new System.Drawing.Size(59, 31);
            this.buttonNone.TabIndex = 10;
            this.buttonNone.Text = "None";
            this.buttonNone.UseVisualStyleBackColor = true;
            this.buttonNone.Click += new System.EventHandler(this.buttonNone_Click);
            // 
            // buttonAll
            // 
            this.buttonAll.Location = new System.Drawing.Point(279, 36);
            this.buttonAll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonAll.Name = "buttonAll";
            this.buttonAll.Size = new System.Drawing.Size(59, 31);
            this.buttonAll.TabIndex = 9;
            this.buttonAll.Text = "All";
            this.buttonAll.UseVisualStyleBackColor = true;
            this.buttonAll.Click += new System.EventHandler(this.buttonAll_Click);
            // 
            // FormEditSpawner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 452);
            this.Controls.Add(this.buttonNone);
            this.Controls.Add(this.buttonAll);
            this.Controls.Add(this.listViewContainer);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditSpawner";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Spawner SubType";
            this.Load += new System.EventHandler(this.FormEditSpawner_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ListView listViewContainer;
        private System.Windows.Forms.ColumnHeader columnHeaderSubType;
        private System.Windows.Forms.Button buttonNone;
        private System.Windows.Forms.Button buttonAll;
    }
}
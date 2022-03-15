
namespace ScenarioEdit
{
    partial class FormEditEffects
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditEffects));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.listViewContainer = new System.Windows.Forms.ListView();
            this.columnHeaderEffect = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderValue = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(260, 385);
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
            this.buttonSave.Location = new System.Drawing.Point(168, 385);
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
            this.listViewContainer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderEffect,
            this.columnHeaderValue});
            this.listViewContainer.FullRowSelect = true;
            this.listViewContainer.GridLines = true;
            this.listViewContainer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewContainer.HideSelection = false;
            this.listViewContainer.Location = new System.Drawing.Point(12, 13);
            this.listViewContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listViewContainer.MultiSelect = false;
            this.listViewContainer.Name = "listViewContainer";
            this.listViewContainer.Size = new System.Drawing.Size(334, 364);
            this.listViewContainer.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewContainer.TabIndex = 8;
            this.listViewContainer.UseCompatibleStateImageBehavior = false;
            this.listViewContainer.View = System.Windows.Forms.View.Details;
            this.listViewContainer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewContainer_MouseDoubleClick);
            // 
            // columnHeaderEffect
            // 
            this.columnHeaderEffect.Text = "Effect";
            this.columnHeaderEffect.Width = 200;
            // 
            // columnHeaderValue
            // 
            this.columnHeaderValue.Text = "Value";
            this.columnHeaderValue.Width = 100;
            // 
            // FormEditEffects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 452);
            this.Controls.Add(this.listViewContainer);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditEffects";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Item Effects";
            this.Load += new System.EventHandler(this.FormEditSpawner_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ListView listViewContainer;
        private System.Windows.Forms.ColumnHeader columnHeaderEffect;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
    }
}
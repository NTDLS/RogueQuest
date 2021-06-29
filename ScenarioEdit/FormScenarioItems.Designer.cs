
namespace ScenarioEdit
{
    partial class FormScenarioItems
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
            this.listViewItems = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderContainer = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLevelName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLevelIndex = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewItems
            // 
            this.listViewItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderContainer,
            this.columnHeaderLevelName,
            this.columnHeaderLevelIndex});
            this.listViewItems.FullRowSelect = true;
            this.listViewItems.GridLines = true;
            this.listViewItems.HideSelection = false;
            this.listViewItems.Location = new System.Drawing.Point(12, 12);
            this.listViewItems.MultiSelect = false;
            this.listViewItems.Name = "listViewItems";
            this.listViewItems.Size = new System.Drawing.Size(594, 337);
            this.listViewItems.TabIndex = 6;
            this.listViewItems.UseCompatibleStateImageBehavior = false;
            this.listViewItems.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 150;
            // 
            // columnHeaderContainer
            // 
            this.columnHeaderContainer.Text = "Container";
            this.columnHeaderContainer.Width = 200;
            // 
            // columnHeaderLevelName
            // 
            this.columnHeaderLevelName.Text = "Level";
            this.columnHeaderLevelName.Width = 100;
            // 
            // columnHeaderLevelIndex
            // 
            this.columnHeaderLevelIndex.Text = "Level #";
            // 
            // FormScenarioItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 361);
            this.Controls.Add(this.listViewItems);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScenarioItems";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scenario Items";
            this.Load += new System.EventHandler(this.FormWorldItems_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listViewItems;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderContainer;
        private System.Windows.Forms.ColumnHeader columnHeaderLevelName;
        private System.Windows.Forms.ColumnHeader columnHeaderLevelIndex;
    }
}

namespace ScenarioEdit
{
    partial class FormEditSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditSelection));
            this.treeViewTiles = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonMoveTilesUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveTilesDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteTiles = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCopyToClipboard = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewTiles
            // 
            this.treeViewTiles.CheckBoxes = true;
            this.treeViewTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewTiles.Location = new System.Drawing.Point(0, 25);
            this.treeViewTiles.Name = "treeViewTiles";
            this.treeViewTiles.Size = new System.Drawing.Size(623, 352);
            this.treeViewTiles.TabIndex = 1;
            this.treeViewTiles.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeViewTiles_AfterCheck);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonMoveTilesUp,
            this.toolStripButtonMoveTilesDown,
            this.toolStripButtonDeleteTiles,
            this.toolStripButtonCopyToClipboard});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(623, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonMoveTilesUp
            // 
            this.toolStripButtonMoveTilesUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveTilesUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveTilesUp.Image")));
            this.toolStripButtonMoveTilesUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveTilesUp.Name = "toolStripButtonMoveTilesUp";
            this.toolStripButtonMoveTilesUp.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveTilesUp.Text = "Move Tiles Up";
            this.toolStripButtonMoveTilesUp.Click += new System.EventHandler(this.toolStripButtonMoveTilesUp_Click);
            // 
            // toolStripButtonMoveTilesDown
            // 
            this.toolStripButtonMoveTilesDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveTilesDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveTilesDown.Image")));
            this.toolStripButtonMoveTilesDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveTilesDown.Name = "toolStripButtonMoveTilesDown";
            this.toolStripButtonMoveTilesDown.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveTilesDown.Text = "Move Tiles Down";
            this.toolStripButtonMoveTilesDown.Click += new System.EventHandler(this.toolStripButtonMoveTilesDown_Click);
            // 
            // toolStripButtonDeleteTiles
            // 
            this.toolStripButtonDeleteTiles.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteTiles.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteTiles.Image")));
            this.toolStripButtonDeleteTiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteTiles.Name = "toolStripButtonDeleteTiles";
            this.toolStripButtonDeleteTiles.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteTiles.Text = "Delete Tiles";
            this.toolStripButtonDeleteTiles.Click += new System.EventHandler(this.toolStripButtonDeleteTiles_Click);
            // 
            // toolStripButtonCopyToClipboard
            // 
            this.toolStripButtonCopyToClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCopyToClipboard.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopyToClipboard.Image")));
            this.toolStripButtonCopyToClipboard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopyToClipboard.Name = "toolStripButtonCopyToClipboard";
            this.toolStripButtonCopyToClipboard.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCopyToClipboard.Text = "Copy to Clipboard";
            this.toolStripButtonCopyToClipboard.Click += new System.EventHandler(this.toolStripButtonCopyToClipboard_Click);
            // 
            // FormEditSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 377);
            this.Controls.Add(this.treeViewTiles);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditSelection";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Selection Picker";
            this.Load += new System.EventHandler(this.FormEditSelection_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TreeView treeViewTiles;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveTilesUp;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveTilesDown;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteTiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopyToClipboard;
    }
}
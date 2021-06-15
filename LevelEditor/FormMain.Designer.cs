
namespace LevelEditor
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerBody = new System.Windows.Forms.SplitContainer();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.splitContainerTools = new System.Windows.Forms.SplitContainer();
            this.listViewTiles = new System.Windows.Forms.ListView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabelMouseXY = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelHoverObject = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).BeginInit();
            this.splitContainerBody.Panel1.SuspendLayout();
            this.splitContainerBody.Panel2.SuspendLayout();
            this.splitContainerBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTools)).BeginInit();
            this.splitContainerTools.Panel1.SuspendLayout();
            this.splitContainerTools.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerBody
            // 
            this.splitContainerBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerBody.IsSplitterFixed = true;
            this.splitContainerBody.Location = new System.Drawing.Point(0, 0);
            this.splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel1
            // 
            this.splitContainerBody.Panel1.Controls.Add(this.pictureBox);
            // 
            // splitContainerBody.Panel2
            // 
            this.splitContainerBody.Panel2.Controls.Add(this.splitContainerTools);
            this.splitContainerBody.Size = new System.Drawing.Size(784, 561);
            this.splitContainerBody.SplitterDistance = 500;
            this.splitContainerBody.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(500, 561);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            // 
            // splitContainerTools
            // 
            this.splitContainerTools.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerTools.Location = new System.Drawing.Point(0, 0);
            this.splitContainerTools.Name = "splitContainerTools";
            this.splitContainerTools.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerTools.Panel1
            // 
            this.splitContainerTools.Panel1.Controls.Add(this.listViewTiles);
            this.splitContainerTools.Size = new System.Drawing.Size(280, 561);
            this.splitContainerTools.SplitterDistance = 280;
            this.splitContainerTools.TabIndex = 1;
            // 
            // listViewTiles
            // 
            this.listViewTiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewTiles.HideSelection = false;
            this.listViewTiles.Location = new System.Drawing.Point(0, 0);
            this.listViewTiles.Name = "listViewTiles";
            this.listViewTiles.Size = new System.Drawing.Size(280, 280);
            this.listViewTiles.TabIndex = 0;
            this.listViewTiles.UseCompatibleStateImageBehavior = false;
            this.listViewTiles.View = System.Windows.Forms.View.List;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelMouseXY,
            this.toolStripStatusLabelHoverObject});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabelMouseXY
            // 
            this.statusLabelMouseXY.Name = "statusLabelMouseXY";
            this.statusLabelMouseXY.Size = new System.Drawing.Size(43, 17);
            this.statusLabelMouseXY.Text = "coords";
            // 
            // toolStripStatusLabelHoverObject
            // 
            this.toolStripStatusLabelHoverObject.Name = "toolStripStatusLabelHoverObject";
            this.toolStripStatusLabelHoverObject.Size = new System.Drawing.Size(72, 17);
            this.toolStripStatusLabelHoverObject.Text = "hoverObject";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainerBody);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RougueQuest : LevelEditor";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.splitContainerBody.Panel1.ResumeLayout(false);
            this.splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBody)).EndInit();
            this.splitContainerBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.splitContainerTools.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerTools)).EndInit();
            this.splitContainerTools.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.ListView listViewTiles;
        private System.Windows.Forms.SplitContainer splitContainerTools;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelMouseXY;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHoverObject;
    }
}



using Game.Controls;

namespace Game
{
    partial class FormUseAction
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUseAction));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonUse = new System.Windows.Forms.Button();
            this.buttonSplit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelItem = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(203, 86);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(89, 31);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonUse
            // 
            this.buttonUse.Location = new System.Drawing.Point(13, 86);
            this.buttonUse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonUse.Name = "buttonUse";
            this.buttonUse.Size = new System.Drawing.Size(89, 31);
            this.buttonUse.TabIndex = 2;
            this.buttonUse.Text = "Use";
            this.buttonUse.UseVisualStyleBackColor = true;
            this.buttonUse.Click += new System.EventHandler(this.buttonUse_Click);
            // 
            // buttonSplit
            // 
            this.buttonSplit.Location = new System.Drawing.Point(108, 86);
            this.buttonSplit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonSplit.Name = "buttonSplit";
            this.buttonSplit.Size = new System.Drawing.Size(89, 31);
            this.buttonSplit.TabIndex = 1;
            this.buttonSplit.Text = "Split";
            this.buttonSplit.UseVisualStyleBackColor = true;
            this.buttonSplit.Click += new System.EventHandler(this.buttonSplit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(219, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "What would you like to do with:";
            // 
            // labelItem
            // 
            this.labelItem.AutoSize = true;
            this.labelItem.Location = new System.Drawing.Point(35, 43);
            this.labelItem.Name = "labelItem";
            this.labelItem.Size = new System.Drawing.Size(59, 20);
            this.labelItem.TabIndex = 14;
            this.labelItem.Text = "<item>";
            // 
            // FormUseAction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 144);
            this.Controls.Add(this.labelItem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSplit);
            this.Controls.Add(this.buttonUse);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormUseAction";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Use / Action";
            this.Load += new System.EventHandler(this.FormUseAction_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonUse;
        private System.Windows.Forms.Button buttonSplit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelItem;
    }
}
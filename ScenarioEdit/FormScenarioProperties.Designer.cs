
namespace ScenarioEdit
{
    partial class FormScenarioProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScenarioProperties));
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxCreatedBy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxModifiedDate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxCreatedDate = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxLevels = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Description";
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(96, 157);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(212, 114);
            this.textBoxDescription.TabIndex = 5;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(152, 277);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(233, 277);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxCreatedBy
            // 
            this.textBoxCreatedBy.Location = new System.Drawing.Point(96, 41);
            this.textBoxCreatedBy.Name = "textBoxCreatedBy";
            this.textBoxCreatedBy.Size = new System.Drawing.Size(212, 23);
            this.textBoxCreatedBy.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Created By";
            // 
            // textBoxModifiedDate
            // 
            this.textBoxModifiedDate.Location = new System.Drawing.Point(96, 99);
            this.textBoxModifiedDate.Name = "textBoxModifiedDate";
            this.textBoxModifiedDate.ReadOnly = true;
            this.textBoxModifiedDate.Size = new System.Drawing.Size(212, 23);
            this.textBoxModifiedDate.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Modified Date";
            // 
            // textBoxCreatedDate
            // 
            this.textBoxCreatedDate.Location = new System.Drawing.Point(96, 70);
            this.textBoxCreatedDate.Name = "textBoxCreatedDate";
            this.textBoxCreatedDate.ReadOnly = true;
            this.textBoxCreatedDate.Size = new System.Drawing.Size(212, 23);
            this.textBoxCreatedDate.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Created Date";
            // 
            // textBoxLevels
            // 
            this.textBoxLevels.Location = new System.Drawing.Point(96, 128);
            this.textBoxLevels.Name = "textBoxLevels";
            this.textBoxLevels.ReadOnly = true;
            this.textBoxLevels.Size = new System.Drawing.Size(212, 23);
            this.textBoxLevels.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "Levels";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(96, 12);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(212, 23);
            this.textBoxName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 14;
            this.label1.Text = "Name";
            // 
            // FormScenarioProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 317);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxLevels);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxModifiedDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxCreatedDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxCreatedBy);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.label3);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormScenarioProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scenario Properties";
            this.Load += new System.EventHandler(this.FormScenarioProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxCreatedBy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxModifiedDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxCreatedDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxLevels;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
    }
}
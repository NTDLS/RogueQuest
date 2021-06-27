
using Game.Controls;

namespace Game
{
    partial class FormNewCharacter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewCharacter));
            this.progressBarAvailable = new Game.Controls.SmoothProgressBar();
            this.labelAvailable = new System.Windows.Forms.Label();
            this.labelStrength = new System.Windows.Forms.Label();
            this.progressBarStrength = new Game.Controls.SmoothProgressBar();
            this.labelIntelligence = new System.Windows.Forms.Label();
            this.progressBarIntelligence = new Game.Controls.SmoothProgressBar();
            this.labelConstitution = new System.Windows.Forms.Label();
            this.labelDexterity = new System.Windows.Forms.Label();
            this.progressBarDexterity = new Game.Controls.SmoothProgressBar();
            this.progressBarConstitution = new Game.Controls.SmoothProgressBar();
            this.buttonStrengthUp = new System.Windows.Forms.Button();
            this.buttonStrengthDown = new System.Windows.Forms.Button();
            this.buttonIntelligenceDown = new System.Windows.Forms.Button();
            this.buttonIntelligenceUp = new System.Windows.Forms.Button();
            this.buttonConstitutionDown = new System.Windows.Forms.Button();
            this.buttonConstitutionUp = new System.Windows.Forms.Button();
            this.buttonDexterityDown = new System.Windows.Forms.Button();
            this.buttonDexterityUp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarAvailable
            // 
            this.progressBarAvailable.Location = new System.Drawing.Point(6, 40);
            this.progressBarAvailable.Maximum = 100;
            this.progressBarAvailable.Minimum = 0;
            this.progressBarAvailable.Name = "progressBarAvailable";
            this.progressBarAvailable.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarAvailable.Size = new System.Drawing.Size(298, 23);
            this.progressBarAvailable.TabIndex = 0;
            this.progressBarAvailable.Value = 0;
            // 
            // labelAvailable
            // 
            this.labelAvailable.AutoSize = true;
            this.labelAvailable.Location = new System.Drawing.Point(6, 19);
            this.labelAvailable.Name = "labelAvailable";
            this.labelAvailable.Size = new System.Drawing.Size(55, 15);
            this.labelAvailable.TabIndex = 1;
            this.labelAvailable.Text = "Available";
            // 
            // labelStrength
            // 
            this.labelStrength.AutoSize = true;
            this.labelStrength.Location = new System.Drawing.Point(6, 66);
            this.labelStrength.Name = "labelStrength";
            this.labelStrength.Size = new System.Drawing.Size(52, 15);
            this.labelStrength.TabIndex = 3;
            this.labelStrength.Text = "Strength";
            // 
            // progressBarStrength
            // 
            this.progressBarStrength.Location = new System.Drawing.Point(6, 84);
            this.progressBarStrength.Maximum = 100;
            this.progressBarStrength.Minimum = 0;
            this.progressBarStrength.Name = "progressBarStrength";
            this.progressBarStrength.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarStrength.Size = new System.Drawing.Size(298, 23);
            this.progressBarStrength.TabIndex = 2;
            this.progressBarStrength.Value = 0;
            // 
            // labelIntelligence
            // 
            this.labelIntelligence.AutoSize = true;
            this.labelIntelligence.Location = new System.Drawing.Point(6, 110);
            this.labelIntelligence.Name = "labelIntelligence";
            this.labelIntelligence.Size = new System.Drawing.Size(68, 15);
            this.labelIntelligence.TabIndex = 5;
            this.labelIntelligence.Text = "Intelligence";
            // 
            // progressBarIntelligence
            // 
            this.progressBarIntelligence.Location = new System.Drawing.Point(6, 128);
            this.progressBarIntelligence.Maximum = 100;
            this.progressBarIntelligence.Minimum = 0;
            this.progressBarIntelligence.Name = "progressBarIntelligence";
            this.progressBarIntelligence.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarIntelligence.Size = new System.Drawing.Size(298, 23);
            this.progressBarIntelligence.TabIndex = 4;
            this.progressBarIntelligence.Value = 0;
            // 
            // labelConstitution
            // 
            this.labelConstitution.AutoSize = true;
            this.labelConstitution.Location = new System.Drawing.Point(6, 154);
            this.labelConstitution.Name = "labelConstitution";
            this.labelConstitution.Size = new System.Drawing.Size(73, 15);
            this.labelConstitution.TabIndex = 7;
            this.labelConstitution.Text = "Constitution";
            // 
            // labelDexterity
            // 
            this.labelDexterity.AutoSize = true;
            this.labelDexterity.Location = new System.Drawing.Point(6, 198);
            this.labelDexterity.Name = "labelDexterity";
            this.labelDexterity.Size = new System.Drawing.Size(54, 15);
            this.labelDexterity.TabIndex = 9;
            this.labelDexterity.Text = "Dexterity";
            // 
            // progressBarDexterity
            // 
            this.progressBarDexterity.Location = new System.Drawing.Point(6, 216);
            this.progressBarDexterity.Maximum = 100;
            this.progressBarDexterity.Minimum = 0;
            this.progressBarDexterity.Name = "progressBarDexterity";
            this.progressBarDexterity.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarDexterity.Size = new System.Drawing.Size(298, 23);
            this.progressBarDexterity.TabIndex = 8;
            this.progressBarDexterity.Value = 0;
            // 
            // progressBarConstitution
            // 
            this.progressBarConstitution.Location = new System.Drawing.Point(6, 172);
            this.progressBarConstitution.Maximum = 100;
            this.progressBarConstitution.Minimum = 0;
            this.progressBarConstitution.Name = "progressBarConstitution";
            this.progressBarConstitution.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarConstitution.Size = new System.Drawing.Size(298, 23);
            this.progressBarConstitution.TabIndex = 14;
            this.progressBarConstitution.Value = 0;
            // 
            // buttonStrengthUp
            // 
            this.buttonStrengthUp.Location = new System.Drawing.Point(340, 84);
            this.buttonStrengthUp.Name = "buttonStrengthUp";
            this.buttonStrengthUp.Size = new System.Drawing.Size(24, 23);
            this.buttonStrengthUp.TabIndex = 2;
            this.buttonStrengthUp.Text = "+";
            this.buttonStrengthUp.UseVisualStyleBackColor = true;
            this.buttonStrengthUp.Click += new System.EventHandler(this.buttonStrengthUp_Click);
            // 
            // buttonStrengthDown
            // 
            this.buttonStrengthDown.Location = new System.Drawing.Point(310, 84);
            this.buttonStrengthDown.Name = "buttonStrengthDown";
            this.buttonStrengthDown.Size = new System.Drawing.Size(24, 23);
            this.buttonStrengthDown.TabIndex = 1;
            this.buttonStrengthDown.Text = "-";
            this.buttonStrengthDown.UseVisualStyleBackColor = true;
            this.buttonStrengthDown.Click += new System.EventHandler(this.buttonStrengthDown_Click);
            // 
            // buttonIntelligenceDown
            // 
            this.buttonIntelligenceDown.Location = new System.Drawing.Point(310, 128);
            this.buttonIntelligenceDown.Name = "buttonIntelligenceDown";
            this.buttonIntelligenceDown.Size = new System.Drawing.Size(24, 23);
            this.buttonIntelligenceDown.TabIndex = 3;
            this.buttonIntelligenceDown.Text = "-";
            this.buttonIntelligenceDown.UseVisualStyleBackColor = true;
            this.buttonIntelligenceDown.Click += new System.EventHandler(this.buttonIntelligenceDown_Click);
            // 
            // buttonIntelligenceUp
            // 
            this.buttonIntelligenceUp.Location = new System.Drawing.Point(340, 128);
            this.buttonIntelligenceUp.Name = "buttonIntelligenceUp";
            this.buttonIntelligenceUp.Size = new System.Drawing.Size(24, 23);
            this.buttonIntelligenceUp.TabIndex = 4;
            this.buttonIntelligenceUp.Text = "+";
            this.buttonIntelligenceUp.UseVisualStyleBackColor = true;
            this.buttonIntelligenceUp.Click += new System.EventHandler(this.buttonIntelligenceUp_Click);
            // 
            // buttonConstitutionDown
            // 
            this.buttonConstitutionDown.Location = new System.Drawing.Point(310, 172);
            this.buttonConstitutionDown.Name = "buttonConstitutionDown";
            this.buttonConstitutionDown.Size = new System.Drawing.Size(24, 23);
            this.buttonConstitutionDown.TabIndex = 5;
            this.buttonConstitutionDown.Text = "-";
            this.buttonConstitutionDown.UseVisualStyleBackColor = true;
            this.buttonConstitutionDown.Click += new System.EventHandler(this.buttonConstitutionDown_Click);
            // 
            // buttonConstitutionUp
            // 
            this.buttonConstitutionUp.Location = new System.Drawing.Point(340, 172);
            this.buttonConstitutionUp.Name = "buttonConstitutionUp";
            this.buttonConstitutionUp.Size = new System.Drawing.Size(24, 23);
            this.buttonConstitutionUp.TabIndex = 6;
            this.buttonConstitutionUp.Text = "+";
            this.buttonConstitutionUp.UseVisualStyleBackColor = true;
            this.buttonConstitutionUp.Click += new System.EventHandler(this.buttonConstitutionUp_Click);
            // 
            // buttonDexterityDown
            // 
            this.buttonDexterityDown.Location = new System.Drawing.Point(310, 216);
            this.buttonDexterityDown.Name = "buttonDexterityDown";
            this.buttonDexterityDown.Size = new System.Drawing.Size(24, 23);
            this.buttonDexterityDown.TabIndex = 7;
            this.buttonDexterityDown.Text = "-";
            this.buttonDexterityDown.UseVisualStyleBackColor = true;
            this.buttonDexterityDown.Click += new System.EventHandler(this.buttonDexterityDown_Click);
            // 
            // buttonDexterityUp
            // 
            this.buttonDexterityUp.Location = new System.Drawing.Point(340, 216);
            this.buttonDexterityUp.Name = "buttonDexterityUp";
            this.buttonDexterityUp.Size = new System.Drawing.Size(24, 23);
            this.buttonDexterityUp.TabIndex = 8;
            this.buttonDexterityUp.Text = "+";
            this.buttonDexterityUp.UseVisualStyleBackColor = true;
            this.buttonDexterityUp.Click += new System.EventHandler(this.buttonDexterityUp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBarAvailable);
            this.groupBox1.Controls.Add(this.buttonDexterityDown);
            this.groupBox1.Controls.Add(this.labelAvailable);
            this.groupBox1.Controls.Add(this.buttonDexterityUp);
            this.groupBox1.Controls.Add(this.progressBarStrength);
            this.groupBox1.Controls.Add(this.buttonConstitutionDown);
            this.groupBox1.Controls.Add(this.labelStrength);
            this.groupBox1.Controls.Add(this.buttonConstitutionUp);
            this.groupBox1.Controls.Add(this.progressBarIntelligence);
            this.groupBox1.Controls.Add(this.buttonIntelligenceDown);
            this.groupBox1.Controls.Add(this.labelIntelligence);
            this.groupBox1.Controls.Add(this.buttonIntelligenceUp);
            this.groupBox1.Controls.Add(this.labelConstitution);
            this.groupBox1.Controls.Add(this.buttonStrengthDown);
            this.groupBox1.Controls.Add(this.progressBarDexterity);
            this.groupBox1.Controls.Add(this.buttonStrengthUp);
            this.groupBox1.Controls.Add(this.labelDexterity);
            this.groupBox1.Controls.Add(this.progressBarConstitution);
            this.groupBox1.Location = new System.Drawing.Point(12, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 264);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stats";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(39, 15);
            this.labelName.TabIndex = 23;
            this.labelName.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(12, 27);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(248, 23);
            this.textBoxName.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(324, 331);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(243, 331);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // FormNewCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 362);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormNewCharacter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Character";
            this.Load += new System.EventHandler(this.FormNewCharacter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SmoothProgressBar progressBarAvailable;
        private System.Windows.Forms.Label labelAvailable;
        private System.Windows.Forms.Label labelStrength;
        private SmoothProgressBar progressBarStrength;
        private System.Windows.Forms.Label labelIntelligence;
        private SmoothProgressBar progressBarIntelligence;
        private System.Windows.Forms.Label labelConstitution;
        private System.Windows.Forms.Label labelDexterity;
        private SmoothProgressBar progressBarDexterity;
        private SmoothProgressBar progressBarConstitution;
        private System.Windows.Forms.Button buttonStrengthUp;
        private System.Windows.Forms.Button buttonStrengthDown;
        private System.Windows.Forms.Button buttonIntelligenceDown;
        private System.Windows.Forms.Button buttonIntelligenceUp;
        private System.Windows.Forms.Button buttonConstitutionDown;
        private System.Windows.Forms.Button buttonConstitutionUp;
        private System.Windows.Forms.Button buttonDexterityDown;
        private System.Windows.Forms.Button buttonDexterityUp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}
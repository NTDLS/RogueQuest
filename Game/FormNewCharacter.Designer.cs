
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
            this.pictureBoxPlayer1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPlayer2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPlayer3 = new System.Windows.Forms.PictureBox();
            this.pictureBoxPlayer4 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRandom = new System.Windows.Forms.Button();
            this.comboBoxScenario = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer4)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressBarAvailable
            // 
            this.progressBarAvailable.Location = new System.Drawing.Point(6, 40);
            this.progressBarAvailable.Maximum = 100;
            this.progressBarAvailable.Minimum = 0;
            this.progressBarAvailable.Name = "progressBarAvailable";
            this.progressBarAvailable.ProgressBarColor = System.Drawing.Color.Blue;
            this.progressBarAvailable.Size = new System.Drawing.Size(298, 12);
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
            this.labelStrength.Location = new System.Drawing.Point(6, 56);
            this.labelStrength.Name = "labelStrength";
            this.labelStrength.Size = new System.Drawing.Size(52, 15);
            this.labelStrength.TabIndex = 3;
            this.labelStrength.Text = "Strength";
            // 
            // progressBarStrength
            // 
            this.progressBarStrength.Location = new System.Drawing.Point(6, 74);
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
            this.labelIntelligence.Location = new System.Drawing.Point(6, 100);
            this.labelIntelligence.Name = "labelIntelligence";
            this.labelIntelligence.Size = new System.Drawing.Size(68, 15);
            this.labelIntelligence.TabIndex = 5;
            this.labelIntelligence.Text = "Intelligence";
            // 
            // progressBarIntelligence
            // 
            this.progressBarIntelligence.Location = new System.Drawing.Point(6, 118);
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
            this.labelConstitution.Location = new System.Drawing.Point(6, 144);
            this.labelConstitution.Name = "labelConstitution";
            this.labelConstitution.Size = new System.Drawing.Size(73, 15);
            this.labelConstitution.TabIndex = 7;
            this.labelConstitution.Text = "Constitution";
            // 
            // labelDexterity
            // 
            this.labelDexterity.AutoSize = true;
            this.labelDexterity.Location = new System.Drawing.Point(6, 188);
            this.labelDexterity.Name = "labelDexterity";
            this.labelDexterity.Size = new System.Drawing.Size(54, 15);
            this.labelDexterity.TabIndex = 9;
            this.labelDexterity.Text = "Dexterity";
            // 
            // progressBarDexterity
            // 
            this.progressBarDexterity.Location = new System.Drawing.Point(6, 206);
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
            this.progressBarConstitution.Location = new System.Drawing.Point(6, 162);
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
            this.buttonStrengthUp.Location = new System.Drawing.Point(340, 74);
            this.buttonStrengthUp.Name = "buttonStrengthUp";
            this.buttonStrengthUp.Size = new System.Drawing.Size(24, 23);
            this.buttonStrengthUp.TabIndex = 4;
            this.buttonStrengthUp.Text = "+";
            this.buttonStrengthUp.UseVisualStyleBackColor = true;
            this.buttonStrengthUp.Click += new System.EventHandler(this.buttonStrengthUp_Click);
            // 
            // buttonStrengthDown
            // 
            this.buttonStrengthDown.Location = new System.Drawing.Point(310, 74);
            this.buttonStrengthDown.Name = "buttonStrengthDown";
            this.buttonStrengthDown.Size = new System.Drawing.Size(24, 23);
            this.buttonStrengthDown.TabIndex = 3;
            this.buttonStrengthDown.Text = "-";
            this.buttonStrengthDown.UseVisualStyleBackColor = true;
            this.buttonStrengthDown.Click += new System.EventHandler(this.buttonStrengthDown_Click);
            // 
            // buttonIntelligenceDown
            // 
            this.buttonIntelligenceDown.Location = new System.Drawing.Point(310, 118);
            this.buttonIntelligenceDown.Name = "buttonIntelligenceDown";
            this.buttonIntelligenceDown.Size = new System.Drawing.Size(24, 23);
            this.buttonIntelligenceDown.TabIndex = 5;
            this.buttonIntelligenceDown.Text = "-";
            this.buttonIntelligenceDown.UseVisualStyleBackColor = true;
            this.buttonIntelligenceDown.Click += new System.EventHandler(this.buttonIntelligenceDown_Click);
            // 
            // buttonIntelligenceUp
            // 
            this.buttonIntelligenceUp.Location = new System.Drawing.Point(340, 118);
            this.buttonIntelligenceUp.Name = "buttonIntelligenceUp";
            this.buttonIntelligenceUp.Size = new System.Drawing.Size(24, 23);
            this.buttonIntelligenceUp.TabIndex = 6;
            this.buttonIntelligenceUp.Text = "+";
            this.buttonIntelligenceUp.UseVisualStyleBackColor = true;
            this.buttonIntelligenceUp.Click += new System.EventHandler(this.buttonIntelligenceUp_Click);
            // 
            // buttonConstitutionDown
            // 
            this.buttonConstitutionDown.Location = new System.Drawing.Point(310, 162);
            this.buttonConstitutionDown.Name = "buttonConstitutionDown";
            this.buttonConstitutionDown.Size = new System.Drawing.Size(24, 23);
            this.buttonConstitutionDown.TabIndex = 7;
            this.buttonConstitutionDown.Text = "-";
            this.buttonConstitutionDown.UseVisualStyleBackColor = true;
            this.buttonConstitutionDown.Click += new System.EventHandler(this.buttonConstitutionDown_Click);
            // 
            // buttonConstitutionUp
            // 
            this.buttonConstitutionUp.Location = new System.Drawing.Point(340, 162);
            this.buttonConstitutionUp.Name = "buttonConstitutionUp";
            this.buttonConstitutionUp.Size = new System.Drawing.Size(24, 23);
            this.buttonConstitutionUp.TabIndex = 8;
            this.buttonConstitutionUp.Text = "+";
            this.buttonConstitutionUp.UseVisualStyleBackColor = true;
            this.buttonConstitutionUp.Click += new System.EventHandler(this.buttonConstitutionUp_Click);
            // 
            // buttonDexterityDown
            // 
            this.buttonDexterityDown.Location = new System.Drawing.Point(310, 206);
            this.buttonDexterityDown.Name = "buttonDexterityDown";
            this.buttonDexterityDown.Size = new System.Drawing.Size(24, 23);
            this.buttonDexterityDown.TabIndex = 9;
            this.buttonDexterityDown.Text = "-";
            this.buttonDexterityDown.UseVisualStyleBackColor = true;
            this.buttonDexterityDown.Click += new System.EventHandler(this.buttonDexterityDown_Click);
            // 
            // buttonDexterityUp
            // 
            this.buttonDexterityUp.Location = new System.Drawing.Point(340, 206);
            this.buttonDexterityUp.Name = "buttonDexterityUp";
            this.buttonDexterityUp.Size = new System.Drawing.Size(24, 23);
            this.buttonDexterityUp.TabIndex = 10;
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
            this.groupBox1.Location = new System.Drawing.Point(12, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 245);
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
            this.buttonCancel.Location = new System.Drawing.Point(324, 435);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(243, 435);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 11;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // pictureBoxPlayer1
            // 
            this.pictureBoxPlayer1.Location = new System.Drawing.Point(6, 21);
            this.pictureBoxPlayer1.Name = "pictureBoxPlayer1";
            this.pictureBoxPlayer1.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxPlayer1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPlayer1.TabIndex = 24;
            this.pictureBoxPlayer1.TabStop = false;
            // 
            // pictureBoxPlayer2
            // 
            this.pictureBoxPlayer2.Location = new System.Drawing.Point(42, 21);
            this.pictureBoxPlayer2.Name = "pictureBoxPlayer2";
            this.pictureBoxPlayer2.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxPlayer2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPlayer2.TabIndex = 25;
            this.pictureBoxPlayer2.TabStop = false;
            // 
            // pictureBoxPlayer3
            // 
            this.pictureBoxPlayer3.Location = new System.Drawing.Point(78, 21);
            this.pictureBoxPlayer3.Name = "pictureBoxPlayer3";
            this.pictureBoxPlayer3.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxPlayer3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPlayer3.TabIndex = 26;
            this.pictureBoxPlayer3.TabStop = false;
            // 
            // pictureBoxPlayer4
            // 
            this.pictureBoxPlayer4.Location = new System.Drawing.Point(114, 21);
            this.pictureBoxPlayer4.Name = "pictureBoxPlayer4";
            this.pictureBoxPlayer4.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxPlayer4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxPlayer4.TabIndex = 27;
            this.pictureBoxPlayer4.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBoxPlayer3);
            this.groupBox2.Controls.Add(this.pictureBoxPlayer4);
            this.groupBox2.Controls.Add(this.pictureBoxPlayer1);
            this.groupBox2.Controls.Add(this.pictureBoxPlayer2);
            this.groupBox2.Location = new System.Drawing.Point(12, 56);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 66);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Avatar";
            // 
            // buttonRandom
            // 
            this.buttonRandom.Location = new System.Drawing.Point(309, 56);
            this.buttonRandom.Name = "buttonRandom";
            this.buttonRandom.Size = new System.Drawing.Size(67, 64);
            this.buttonRandom.TabIndex = 1;
            this.buttonRandom.Text = "Random";
            this.buttonRandom.UseVisualStyleBackColor = true;
            this.buttonRandom.Click += new System.EventHandler(this.buttonRandom_Click);
            // 
            // comboBoxScenario
            // 
            this.comboBoxScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScenario.FormattingEnabled = true;
            this.comboBoxScenario.Location = new System.Drawing.Point(12, 143);
            this.comboBoxScenario.Name = "comboBoxScenario";
            this.comboBoxScenario.Size = new System.Drawing.Size(387, 23);
            this.comboBoxScenario.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 31;
            this.label1.Text = "Scenario";
            // 
            // FormNewCharacter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 470);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxScenario);
            this.Controls.Add(this.buttonRandom);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormNewCharacter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Character";
            this.Load += new System.EventHandler(this.FormNewCharacter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayer4)).EndInit();
            this.groupBox2.ResumeLayout(false);
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
        private System.Windows.Forms.PictureBox pictureBoxPlayer1;
        private System.Windows.Forms.PictureBox pictureBoxPlayer2;
        private System.Windows.Forms.PictureBox pictureBoxPlayer3;
        private System.Windows.Forms.PictureBox pictureBoxPlayer4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonRandom;
        private System.Windows.Forms.ComboBox comboBoxScenario;
        private System.Windows.Forms.Label label1;
    }
}

namespace Game
{
    partial class FormPickNewSpell
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.listViewSpells = new System.Windows.Forms.ListView();
            this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLevel = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderMana = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(344, 227);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(94, 29);
            this.buttonOk.TabIndex = 32;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // listViewSpells
            // 
            this.listViewSpells.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderLevel,
            this.columnHeaderMana});
            this.listViewSpells.FullRowSelect = true;
            this.listViewSpells.GridLines = true;
            this.listViewSpells.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSpells.HideSelection = false;
            this.listViewSpells.Location = new System.Drawing.Point(12, 12);
            this.listViewSpells.MultiSelect = false;
            this.listViewSpells.Name = "listViewSpells";
            this.listViewSpells.Size = new System.Drawing.Size(426, 209);
            this.listViewSpells.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewSpells.TabIndex = 33;
            this.listViewSpells.UseCompatibleStateImageBehavior = false;
            this.listViewSpells.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 280;
            // 
            // columnHeaderLevel
            // 
            this.columnHeaderLevel.Text = "Level";
            // 
            // columnHeaderMana
            // 
            this.columnHeaderMana.Text = "Mana";
            // 
            // FormPickNewSpell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 262);
            this.Controls.Add(this.listViewSpells);
            this.Controls.Add(this.buttonOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPickNewSpell";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pick a New Spell";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPickNewSpell_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ListView listViewSpells;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderLevel;
        private System.Windows.Forms.ColumnHeader columnHeaderMana;
    }
}
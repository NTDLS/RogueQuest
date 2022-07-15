
using Game.Controls;

namespace Game
{
    partial class FormStoreQuantity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStoreQuantity));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonBuy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelStoreQuantity = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelItem = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTotalPrice = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxBuyAmount = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxBuyAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(166, 163);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(89, 31);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonBuy
            // 
            this.buttonBuy.Location = new System.Drawing.Point(71, 163);
            this.buttonBuy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonBuy.Name = "buttonBuy";
            this.buttonBuy.Size = new System.Drawing.Size(89, 31);
            this.buttonBuy.TabIndex = 1;
            this.buttonBuy.Text = "Buy";
            this.buttonBuy.UseVisualStyleBackColor = true;
            this.buttonBuy.Click += new System.EventHandler(this.buttonBuy_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Store Has";
            // 
            // labelStoreQuantity
            // 
            this.labelStoreQuantity.AutoSize = true;
            this.labelStoreQuantity.Location = new System.Drawing.Point(130, 52);
            this.labelStoreQuantity.Name = "labelStoreQuantity";
            this.labelStoreQuantity.Size = new System.Drawing.Size(57, 20);
            this.labelStoreQuantity.TabIndex = 13;
            this.labelStoreQuantity.Text = "000000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Buy Amount";
            // 
            // labelItem
            // 
            this.labelItem.AutoSize = true;
            this.labelItem.Location = new System.Drawing.Point(130, 21);
            this.labelItem.Name = "labelItem";
            this.labelItem.Size = new System.Drawing.Size(59, 20);
            this.labelItem.TabIndex = 17;
            this.labelItem.Text = "<item>";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 20);
            this.label4.TabIndex = 16;
            this.label4.Text = "Item";
            // 
            // labelTotalPrice
            // 
            this.labelTotalPrice.AutoSize = true;
            this.labelTotalPrice.Location = new System.Drawing.Point(130, 117);
            this.labelTotalPrice.Name = "labelTotalPrice";
            this.labelTotalPrice.Size = new System.Drawing.Size(57, 20);
            this.labelTotalPrice.TabIndex = 19;
            this.labelTotalPrice.Text = "000000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 20);
            this.label5.TabIndex = 18;
            this.label5.Text = "Total Price";
            // 
            // textBoxBuyAmount
            // 
            this.textBoxBuyAmount.Location = new System.Drawing.Point(130, 83);
            this.textBoxBuyAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.textBoxBuyAmount.Name = "textBoxBuyAmount";
            this.textBoxBuyAmount.Size = new System.Drawing.Size(125, 27);
            this.textBoxBuyAmount.TabIndex = 20;
            this.textBoxBuyAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // FormStoreQuantity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 214);
            this.Controls.Add(this.textBoxBuyAmount);
            this.Controls.Add(this.labelTotalPrice);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelItem);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelStoreQuantity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonBuy);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStoreQuantity";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buy how many?";
            this.Load += new System.EventHandler(this.FormStoreQuantity_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textBoxBuyAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonBuy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelStoreQuantity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelTotalPrice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown textBoxBuyAmount;
    }
}
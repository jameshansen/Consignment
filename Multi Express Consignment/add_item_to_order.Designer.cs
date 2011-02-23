namespace Multi_Express_Consignment
{
    partial class add_item_to_order
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(add_item_to_order));
            this.label1 = new System.Windows.Forms.Label();
            this.item_upc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.item_description = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.item_consignment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.item_date_expiry = new System.Windows.Forms.TextBox();
            this.item_price_suggested = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.item_price_minimum = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.input_price = new System.Windows.Forms.MaskedTextBox();
            this.button_price3 = new System.Windows.Forms.Button();
            this.button_price2 = new System.Windows.Forms.Button();
            this.button_price1 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "UPC";
            // 
            // item_upc
            // 
            this.item_upc.Location = new System.Drawing.Point(78, 6);
            this.item_upc.Name = "item_upc";
            this.item_upc.ReadOnly = true;
            this.item_upc.Size = new System.Drawing.Size(202, 20);
            this.item_upc.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // item_description
            // 
            this.item_description.Location = new System.Drawing.Point(78, 32);
            this.item_description.Name = "item_description";
            this.item_description.ReadOnly = true;
            this.item_description.Size = new System.Drawing.Size(213, 20);
            this.item_description.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.item_consignment);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.item_date_expiry);
            this.groupBox2.Location = new System.Drawing.Point(15, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 105);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Reference";
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(234, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 21);
            this.button1.TabIndex = 10;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Consignment";
            // 
            // item_consignment
            // 
            this.item_consignment.Location = new System.Drawing.Point(9, 71);
            this.item_consignment.Name = "item_consignment";
            this.item_consignment.ReadOnly = true;
            this.item_consignment.Size = new System.Drawing.Size(219, 20);
            this.item_consignment.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Item Expiry Date";
            // 
            // item_date_expiry
            // 
            this.item_date_expiry.Location = new System.Drawing.Point(9, 32);
            this.item_date_expiry.Name = "item_date_expiry";
            this.item_date_expiry.ReadOnly = true;
            this.item_date_expiry.Size = new System.Drawing.Size(252, 20);
            this.item_date_expiry.TabIndex = 6;
            // 
            // item_price_suggested
            // 
            this.item_price_suggested.Location = new System.Drawing.Point(10, 58);
            this.item_price_suggested.Name = "item_price_suggested";
            this.item_price_suggested.ReadOnly = true;
            this.item_price_suggested.Size = new System.Drawing.Size(127, 20);
            this.item_price_suggested.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Minimum Price";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Suggested Price";
            // 
            // item_price_minimum
            // 
            this.item_price_minimum.Location = new System.Drawing.Point(10, 97);
            this.item_price_minimum.Name = "item_price_minimum";
            this.item_price_minimum.ReadOnly = true;
            this.item_price_minimum.Size = new System.Drawing.Size(127, 20);
            this.item_price_minimum.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.input_price);
            this.groupBox1.Controls.Add(this.button_price3);
            this.groupBox1.Controls.Add(this.button_price2);
            this.groupBox1.Controls.Add(this.button_price1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.item_price_suggested);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.item_price_minimum);
            this.groupBox1.Location = new System.Drawing.Point(15, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 137);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enter Selling Price";
            // 
            // input_price
            // 
            this.input_price.Location = new System.Drawing.Point(10, 17);
            this.input_price.Mask = "999999.9999";
            this.input_price.Name = "input_price";
            this.input_price.PromptChar = ' ';
            this.input_price.Size = new System.Drawing.Size(178, 20);
            this.input_price.TabIndex = 8;
            this.input_price.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.input_price_MaskInputRejected);
            this.input_price.KeyDown += new System.Windows.Forms.KeyEventHandler(this.currency_check_KeyDown);
            this.input_price.Leave += new System.EventHandler(this.stringToCurrencyEvt);
            // 
            // button_price3
            // 
            this.button_price3.Location = new System.Drawing.Point(143, 97);
            this.button_price3.Name = "button_price3";
            this.button_price3.Size = new System.Drawing.Size(122, 20);
            this.button_price3.TabIndex = 7;
            this.button_price3.Text = "Set this Price";
            this.button_price3.UseVisualStyleBackColor = true;
            this.button_price3.Click += new System.EventHandler(this.button_price3_Click);
            // 
            // button_price2
            // 
            this.button_price2.Location = new System.Drawing.Point(143, 58);
            this.button_price2.Name = "button_price2";
            this.button_price2.Size = new System.Drawing.Size(122, 20);
            this.button_price2.TabIndex = 6;
            this.button_price2.Text = "Set this Price";
            this.button_price2.UseVisualStyleBackColor = true;
            this.button_price2.Click += new System.EventHandler(this.button_price2_Click);
            // 
            // button_price1
            // 
            this.button_price1.Location = new System.Drawing.Point(196, 18);
            this.button_price1.Name = "button_price1";
            this.button_price1.Size = new System.Drawing.Size(69, 20);
            this.button_price1.TabIndex = 1;
            this.button_price1.Text = "OK";
            this.button_price1.UseVisualStyleBackColor = true;
            this.button_price1.Click += new System.EventHandler(this.button_price1_Click);
            // 
            // add_item_to_order
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 318);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.item_description);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.item_upc);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "add_item_to_order";
            this.Text = "Enter/Select Price for Item";
            this.Load += new System.EventHandler(this.add_item_to_order_Load);
            this.Shown += new System.EventHandler(this.add_item_to_order_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox item_upc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox item_description;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox item_date_expiry;
        private System.Windows.Forms.TextBox item_price_minimum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox item_price_suggested;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox item_consignment;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_price3;
        private System.Windows.Forms.Button button_price2;
        private System.Windows.Forms.Button button_price1;
        private System.Windows.Forms.MaskedTextBox input_price;
    }
}
namespace Multi_Express_Consignment
{
    partial class add_item_to_cpo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(add_item_to_cpo));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.input_date_expiry = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.input_date_received = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.input_share_type = new System.Windows.Forms.ComboBox();
            this.input_price_suggested = new System.Windows.Forms.MaskedTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.input_share = new System.Windows.Forms.MaskedTextBox();
            this.input_price_minimum = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.input_description = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.input_desc_size = new System.Windows.Forms.ComboBox();
            this.input_desc_colour = new System.Windows.Forms.ComboBox();
            this.input_desc_material = new System.Windows.Forms.ComboBox();
            this.input_desc_garment = new System.Windows.Forms.ComboBox();
            this.input_desc_brand = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.input_desc_gender = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.existing_upc = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.input_date_expiry);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.input_date_received);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.input_share_type);
            this.groupBox2.Controls.Add(this.input_price_suggested);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.input_share);
            this.groupBox2.Controls.Add(this.input_price_minimum);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.input_description);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(336, 185);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Item Details";
            // 
            // input_date_expiry
            // 
            this.input_date_expiry.Location = new System.Drawing.Point(125, 149);
            this.input_date_expiry.Name = "input_date_expiry";
            this.input_date_expiry.Size = new System.Drawing.Size(200, 20);
            this.input_date_expiry.TabIndex = 9;
            this.input_date_expiry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(58, 153);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Expiry Date";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_date_received
            // 
            this.input_date_received.Location = new System.Drawing.Point(125, 123);
            this.input_date_received.Name = "input_date_received";
            this.input_date_received.Size = new System.Drawing.Size(200, 20);
            this.input_date_received.TabIndex = 5;
            this.input_date_received.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 127);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(79, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Date Received";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_share_type
            // 
            this.input_share_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.input_share_type.FormattingEnabled = true;
            this.input_share_type.Items.AddRange(new object[] {
            "%",
            "$"});
            this.input_share_type.Location = new System.Drawing.Point(215, 96);
            this.input_share_type.Name = "input_share_type";
            this.input_share_type.Size = new System.Drawing.Size(54, 21);
            this.input_share_type.TabIndex = 4;
            this.input_share_type.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // input_price_suggested
            // 
            this.input_price_suggested.Location = new System.Drawing.Point(125, 71);
            this.input_price_suggested.Mask = "999999.9999";
            this.input_price_suggested.Name = "input_price_suggested";
            this.input_price_suggested.PromptChar = ' ';
            this.input_price_suggested.Size = new System.Drawing.Size(84, 20);
            this.input_price_suggested.TabIndex = 2;
            this.input_price_suggested.Enter += new System.EventHandler(this.highlightContents);
            this.input_price_suggested.KeyDown += new System.Windows.Forms.KeyEventHandler(this.currency_check_KeyDown);
            this.input_price_suggested.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            this.input_price_suggested.Leave += new System.EventHandler(this.stringToCurrencyEvt);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Suggested Sale Price";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Share of Sale Price";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_share
            // 
            this.input_share.Location = new System.Drawing.Point(125, 97);
            this.input_share.Mask = "000000.0000";
            this.input_share.Name = "input_share";
            this.input_share.PromptChar = ' ';
            this.input_share.Size = new System.Drawing.Size(84, 20);
            this.input_share.TabIndex = 3;
            this.input_share.Text = "    5000";
            this.input_share.Enter += new System.EventHandler(this.highlightContents);
            this.input_share.KeyDown += new System.Windows.Forms.KeyEventHandler(this.currency_check_KeyDown);
            this.input_share.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            this.input_share.Leave += new System.EventHandler(this.stringToCurrencyEvt);
            // 
            // input_price_minimum
            // 
            this.input_price_minimum.Location = new System.Drawing.Point(125, 44);
            this.input_price_minimum.Mask = "999999.9999";
            this.input_price_minimum.Name = "input_price_minimum";
            this.input_price_minimum.PromptChar = ' ';
            this.input_price_minimum.Size = new System.Drawing.Size(84, 20);
            this.input_price_minimum.TabIndex = 1;
            this.input_price_minimum.Enter += new System.EventHandler(this.highlightContents);
            this.input_price_minimum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.currency_check_KeyDown);
            this.input_price_minimum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            this.input_price_minimum.Leave += new System.EventHandler(this.stringToCurrencyEvt);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimum Sale Price";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Description";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_description
            // 
            this.input_description.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_description.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.input_description.Location = new System.Drawing.Point(74, 19);
            this.input_description.Name = "input_description";
            this.input_description.Size = new System.Drawing.Size(251, 20);
            this.input_description.TabIndex = 0;
            this.input_description.Enter += new System.EventHandler(this.highlightContents);
            this.input_description.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_description_KeyDown);
            this.input_description.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.input_desc_size);
            this.groupBox1.Controls.Add(this.input_desc_colour);
            this.groupBox1.Controls.Add(this.input_desc_material);
            this.groupBox1.Controls.Add(this.input_desc_garment);
            this.groupBox1.Controls.Add(this.input_desc_brand);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.input_desc_gender);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 203);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 200);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Matrix Details";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(55, 157);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "Size";
            this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_desc_size
            // 
            this.input_desc_size.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_desc_size.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.input_desc_size.FormattingEnabled = true;
            this.input_desc_size.Location = new System.Drawing.Point(88, 154);
            this.input_desc_size.Name = "input_desc_size";
            this.input_desc_size.Size = new System.Drawing.Size(237, 21);
            this.input_desc_size.TabIndex = 13;
            this.input_desc_size.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_desc_size_KeyDown);
            // 
            // input_desc_colour
            // 
            this.input_desc_colour.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_desc_colour.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.input_desc_colour.FormattingEnabled = true;
            this.input_desc_colour.Location = new System.Drawing.Point(88, 127);
            this.input_desc_colour.Name = "input_desc_colour";
            this.input_desc_colour.Size = new System.Drawing.Size(237, 21);
            this.input_desc_colour.TabIndex = 10;
            this.input_desc_colour.Enter += new System.EventHandler(this.highlightContents);
            this.input_desc_colour.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_desc_colour_KeyDown);
            this.input_desc_colour.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // input_desc_material
            // 
            this.input_desc_material.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_desc_material.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.input_desc_material.FormattingEnabled = true;
            this.input_desc_material.Location = new System.Drawing.Point(88, 100);
            this.input_desc_material.Name = "input_desc_material";
            this.input_desc_material.Size = new System.Drawing.Size(237, 21);
            this.input_desc_material.TabIndex = 9;
            this.input_desc_material.Enter += new System.EventHandler(this.highlightContents);
            this.input_desc_material.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_desc_material_KeyDown);
            this.input_desc_material.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // input_desc_garment
            // 
            this.input_desc_garment.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_desc_garment.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.input_desc_garment.FormattingEnabled = true;
            this.input_desc_garment.Location = new System.Drawing.Point(88, 73);
            this.input_desc_garment.Name = "input_desc_garment";
            this.input_desc_garment.Size = new System.Drawing.Size(237, 21);
            this.input_desc_garment.TabIndex = 8;
            this.input_desc_garment.Enter += new System.EventHandler(this.highlightContents);
            this.input_desc_garment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_desc_garment_KeyDown);
            this.input_desc_garment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // input_desc_brand
            // 
            this.input_desc_brand.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.input_desc_brand.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.input_desc_brand.FormattingEnabled = true;
            this.input_desc_brand.Location = new System.Drawing.Point(88, 19);
            this.input_desc_brand.Name = "input_desc_brand";
            this.input_desc_brand.Size = new System.Drawing.Size(237, 21);
            this.input_desc_brand.TabIndex = 6;
            this.input_desc_brand.Enter += new System.EventHandler(this.highlightContents);
            this.input_desc_brand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_desc_brand_KeyDown);
            this.input_desc_brand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(45, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Colour";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Material";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // input_desc_gender
            // 
            this.input_desc_gender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.input_desc_gender.FormattingEnabled = true;
            this.input_desc_gender.Items.AddRange(new object[] {
            " ",
            "Male",
            "Female",
            "Unisex"});
            this.input_desc_gender.Location = new System.Drawing.Point(88, 46);
            this.input_desc_gender.Name = "input_desc_gender";
            this.input_desc_gender.Size = new System.Drawing.Size(96, 21);
            this.input_desc_gender.TabIndex = 7;
            this.input_desc_gender.Enter += new System.EventHandler(this.highlightContents);
            this.input_desc_gender.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nextBox);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Garment Type";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(40, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(42, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Gender";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(47, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Brand";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(273, 409);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.TabStop = false;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(153, 409);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Add Item";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.existing_upc);
            this.groupBox3.Location = new System.Drawing.Point(12, 438);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(336, 65);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Existing UPC";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 16);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(286, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "If this item has a UPC enter it below, otherwise leave blank.";
            // 
            // existing_upc
            // 
            this.existing_upc.Location = new System.Drawing.Point(11, 32);
            this.existing_upc.Name = "existing_upc";
            this.existing_upc.Size = new System.Drawing.Size(314, 20);
            this.existing_upc.TabIndex = 0;
            // 
            // add_item_to_cpo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 517);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "add_item_to_cpo";
            this.Text = "Add Item to Consignment Order";
            this.Load += new System.EventHandler(this.add_item_to_cpo_Load);
            this.Shown += new System.EventHandler(this.add_item_to_cpo_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox input_description;
        private System.Windows.Forms.MaskedTextBox input_price_minimum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox input_share;
        private System.Windows.Forms.ComboBox input_share_type;
        private System.Windows.Forms.MaskedTextBox input_price_suggested;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox input_desc_colour;
        private System.Windows.Forms.ComboBox input_desc_material;
        private System.Windows.Forms.ComboBox input_desc_garment;
        private System.Windows.Forms.ComboBox input_desc_brand;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox input_desc_gender;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker input_date_received;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker input_date_expiry;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox input_desc_size;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox existing_upc;

    }
}
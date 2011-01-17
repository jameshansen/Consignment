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
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
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
            this.groupBox2.Size = new System.Drawing.Size(336, 132);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Item Details";
            // 
            // input_share_type
            // 
            this.input_share_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.input_share_type.FormattingEnabled = true;
            this.input_share_type.Items.AddRange(new object[] {
            "Value",
            "Percentage"});
            this.input_share_type.Location = new System.Drawing.Point(215, 97);
            this.input_share_type.Name = "input_share_type";
            this.input_share_type.Size = new System.Drawing.Size(110, 21);
            this.input_share_type.TabIndex = 4;
            // 
            // input_price_suggested
            // 
            this.input_price_suggested.Location = new System.Drawing.Point(125, 71);
            this.input_price_suggested.Mask = "000000.0000";
            this.input_price_suggested.Name = "input_price_suggested";
            this.input_price_suggested.Size = new System.Drawing.Size(84, 20);
            this.input_price_suggested.TabIndex = 2;
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
            this.input_share.Size = new System.Drawing.Size(84, 20);
            this.input_share.TabIndex = 3;
            // 
            // input_price_minimum
            // 
            this.input_price_minimum.Location = new System.Drawing.Point(125, 44);
            this.input_price_minimum.Mask = "000000.0000";
            this.input_price_minimum.Name = "input_price_minimum";
            this.input_price_minimum.Size = new System.Drawing.Size(84, 20);
            this.input_price_minimum.TabIndex = 1;
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
            this.input_description.Location = new System.Drawing.Point(74, 19);
            this.input_description.Name = "input_description";
            this.input_description.Size = new System.Drawing.Size(251, 20);
            this.input_description.TabIndex = 0;
            // 
            // groupBox1
            // 
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
            this.groupBox1.Location = new System.Drawing.Point(12, 152);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 162);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Matrix Details";
            // 
            // input_desc_colour
            // 
            this.input_desc_colour.FormattingEnabled = true;
            this.input_desc_colour.Location = new System.Drawing.Point(88, 127);
            this.input_desc_colour.Name = "input_desc_colour";
            this.input_desc_colour.Size = new System.Drawing.Size(237, 21);
            this.input_desc_colour.TabIndex = 9;
            // 
            // input_desc_material
            // 
            this.input_desc_material.FormattingEnabled = true;
            this.input_desc_material.Location = new System.Drawing.Point(88, 100);
            this.input_desc_material.Name = "input_desc_material";
            this.input_desc_material.Size = new System.Drawing.Size(237, 21);
            this.input_desc_material.TabIndex = 8;
            // 
            // input_desc_garment
            // 
            this.input_desc_garment.FormattingEnabled = true;
            this.input_desc_garment.Location = new System.Drawing.Point(88, 73);
            this.input_desc_garment.Name = "input_desc_garment";
            this.input_desc_garment.Size = new System.Drawing.Size(237, 21);
            this.input_desc_garment.TabIndex = 7;
            // 
            // input_desc_brand
            // 
            this.input_desc_brand.FormattingEnabled = true;
            this.input_desc_brand.Location = new System.Drawing.Point(88, 19);
            this.input_desc_brand.Name = "input_desc_brand";
            this.input_desc_brand.Size = new System.Drawing.Size(237, 21);
            this.input_desc_brand.TabIndex = 5;
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
            "Female"});
            this.input_desc_gender.Location = new System.Drawing.Point(88, 46);
            this.input_desc_gender.Name = "input_desc_gender";
            this.input_desc_gender.Size = new System.Drawing.Size(96, 21);
            this.input_desc_gender.TabIndex = 6;
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
            this.button1.Location = new System.Drawing.Point(273, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.TabStop = false;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(192, 324);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // add_item_to_cpo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 360);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
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

    }
}
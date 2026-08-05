namespace Multi_Express_Consignment
{
    partial class tax_code_entry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(tax_code_entry));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.input_code = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.input_desc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.input_rate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.input_icon = new System.Windows.Forms.ComboBox();
            this.preview = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).BeginInit();
            this.SuspendLayout();
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.preview);
            this.groupBox1.Controls.Add(this.input_icon);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.input_rate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.input_desc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.input_code);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 135);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tax Code";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Code";
            //
            // input_code
            //
            this.input_code.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.input_code.Location = new System.Drawing.Point(74, 22);
            this.input_code.MaxLength = 2;
            this.input_code.Name = "input_code";
            this.input_code.Size = new System.Drawing.Size(40, 20);
            this.input_code.TabIndex = 0;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Description";
            //
            // input_desc
            //
            this.input_desc.Location = new System.Drawing.Point(74, 48);
            this.input_desc.MaxLength = 30;
            this.input_desc.Name = "input_desc";
            this.input_desc.Size = new System.Drawing.Size(228, 20);
            this.input_desc.TabIndex = 1;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Rate %";
            //
            // input_rate
            //
            this.input_rate.Location = new System.Drawing.Point(74, 74);
            this.input_rate.MaxLength = 8;
            this.input_rate.Name = "input_rate";
            this.input_rate.Size = new System.Drawing.Size(70, 20);
            this.input_rate.TabIndex = 2;
            this.input_rate.Text = "0.0000";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Icon";
            //
            // input_icon
            //
            this.input_icon.FormattingEnabled = true;
            this.input_icon.Location = new System.Drawing.Point(74, 100);
            this.input_icon.MaxLength = 8;
            this.input_icon.Name = "input_icon";
            this.input_icon.Size = new System.Drawing.Size(150, 21);
            this.input_icon.TabIndex = 3;
            this.input_icon.TextChanged += new System.EventHandler(this.input_icon_TextChanged);
            //
            // preview
            //
            this.preview.Location = new System.Drawing.Point(234, 102);
            this.preview.Name = "preview";
            this.preview.Size = new System.Drawing.Size(16, 16);
            this.preview.TabIndex = 12;
            this.preview.TabStop = false;
            //
            // button1
            //
            this.button1.Location = new System.Drawing.Point(257, 153);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // button2
            //
            this.button2.Location = new System.Drawing.Point(137, 153);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(114, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Add Tax Code";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            //
            // tax_code_entry
            //
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(344, 188);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "tax_code_entry";
            this.Text = "Add Tax Code";
            this.Load += new System.EventHandler(this.tax_code_entry_Load);
            this.Shown += new System.EventHandler(this.tax_code_entry_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.preview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox input_code;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox input_desc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox input_rate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox input_icon;
        private System.Windows.Forms.PictureBox preview;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

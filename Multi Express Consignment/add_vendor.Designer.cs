namespace Multi_Express_Consignment
{
    partial class add_vendor
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
            this.input_CMCUCODE = new System.Windows.Forms.TextBox();
            this.input_CMNAMESUR = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.input_CMNAME1ST = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.input_CMCUNAME = new System.Windows.Forms.TextBox();
            this.input_CMPHONE_a = new System.Windows.Forms.MaskedTextBox();
            this.input_CMPHONE_b = new System.Windows.Forms.MaskedTextBox();
            this.input_CMPHONE_c = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // input_CMCUCODE
            // 
            this.input_CMCUCODE.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.input_CMCUCODE.Location = new System.Drawing.Point(101, 123);
            this.input_CMCUCODE.Name = "input_CMCUCODE";
            this.input_CMCUCODE.Size = new System.Drawing.Size(170, 20);
            this.input_CMCUCODE.TabIndex = 5;
            this.input_CMCUCODE.TextChanged += new System.EventHandler(this.input_CMCUCODE_TextChanged);
            this.input_CMCUCODE.Enter += new System.EventHandler(this.input_CMCUCODE_Enter);
            this.input_CMCUCODE.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMCUCODE_KeyDown);
            // 
            // input_CMNAMESUR
            // 
            this.input_CMNAMESUR.Location = new System.Drawing.Point(100, 32);
            this.input_CMNAMESUR.Name = "input_CMNAMESUR";
            this.input_CMNAMESUR.Size = new System.Drawing.Size(214, 20);
            this.input_CMNAMESUR.TabIndex = 1;
            this.input_CMNAMESUR.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMNAMESUR_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(60, 126);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Code";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Last Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Telephone #";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(157, 163);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(239, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.TabStop = false;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // input_CMNAME1ST
            // 
            this.input_CMNAME1ST.Location = new System.Drawing.Point(100, 6);
            this.input_CMNAME1ST.Name = "input_CMNAME1ST";
            this.input_CMNAME1ST.Size = new System.Drawing.Size(214, 20);
            this.input_CMNAME1ST.TabIndex = 0;
            this.input_CMNAME1ST.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMNAME1ST_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "First Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Company Name";
            // 
            // input_CMCUNAME
            // 
            this.input_CMCUNAME.Location = new System.Drawing.Point(100, 83);
            this.input_CMCUNAME.Name = "input_CMCUNAME";
            this.input_CMCUNAME.Size = new System.Drawing.Size(214, 20);
            this.input_CMCUNAME.TabIndex = 4;
            this.input_CMCUNAME.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMCUNAME_KeyDown);
            // 
            // input_CMPHONE_a
            // 
            this.input_CMPHONE_a.Location = new System.Drawing.Point(100, 58);
            this.input_CMPHONE_a.Mask = "9999";
            this.input_CMPHONE_a.Name = "input_CMPHONE_a";
            this.input_CMPHONE_a.PromptChar = ' ';
            this.input_CMPHONE_a.Size = new System.Drawing.Size(36, 20);
            this.input_CMPHONE_a.TabIndex = 2;
            this.input_CMPHONE_a.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMPHONE_a_KeyDown);
            // 
            // input_CMPHONE_b
            // 
            this.input_CMPHONE_b.Location = new System.Drawing.Point(142, 58);
            this.input_CMPHONE_b.Mask = "999999999999";
            this.input_CMPHONE_b.Name = "input_CMPHONE_b";
            this.input_CMPHONE_b.PromptChar = ' ';
            this.input_CMPHONE_b.Size = new System.Drawing.Size(172, 20);
            this.input_CMPHONE_b.TabIndex = 3;
            this.input_CMPHONE_b.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_CMPHONE_b_KeyDown);
            // 
            // input_CMPHONE_c
            // 
            this.input_CMPHONE_c.Location = new System.Drawing.Point(9, 103);
            this.input_CMPHONE_c.Mask = "AAAAAAAA";
            this.input_CMPHONE_c.Name = "input_CMPHONE_c";
            this.input_CMPHONE_c.PromptChar = ' ';
            this.input_CMPHONE_c.Size = new System.Drawing.Size(59, 20);
            this.input_CMPHONE_c.TabIndex = 0;
            this.input_CMPHONE_c.TabStop = false;
            this.input_CMPHONE_c.Visible = false;
            // 
            // add_vendor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 199);
            this.Controls.Add(this.input_CMPHONE_c);
            this.Controls.Add(this.input_CMPHONE_b);
            this.Controls.Add(this.input_CMPHONE_a);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.input_CMCUNAME);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.input_CMNAME1ST);
            this.Controls.Add(this.input_CMNAMESUR);
            this.Controls.Add(this.input_CMCUCODE);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "add_vendor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create New";
            this.Load += new System.EventHandler(this.add_vendor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input_CMCUCODE;
        private System.Windows.Forms.TextBox input_CMNAMESUR;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox input_CMNAME1ST;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox input_CMCUNAME;
        private System.Windows.Forms.MaskedTextBox input_CMPHONE_a;
        private System.Windows.Forms.MaskedTextBox input_CMPHONE_b;
        private System.Windows.Forms.MaskedTextBox input_CMPHONE_c;
    }
}
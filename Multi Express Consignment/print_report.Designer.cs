namespace Multi_Express_Consignment
{
    partial class print_report
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(print_report));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.form_list = new System.Windows.Forms.ListBox();
            this.b_printer = new System.Windows.Forms.Button();
            this.b_screen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.form_list);
            this.groupBox2.Location = new System.Drawing.Point(181, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(393, 142);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Form";
            // 
            // form_list
            // 
            this.form_list.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.form_list.FormattingEnabled = true;
            this.form_list.ItemHeight = 15;
            this.form_list.Items.AddRange(new object[] {
            "C-Consignment Agreement",
            "C-Consignment Barcode Item Labels",
            "C-Consignment Invoice",
            "S-Sales Order",
            "S-Sales Receipt"});
            this.form_list.Location = new System.Drawing.Point(6, 19);
            this.form_list.Name = "form_list";
            this.form_list.Size = new System.Drawing.Size(381, 109);
            this.form_list.TabIndex = 0;
            // 
            // b_printer
            // 
            this.b_printer.Image = global::Multi_Express_Consignment.Properties.Resources.lg_print;
            this.b_printer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.b_printer.Location = new System.Drawing.Point(13, 19);
            this.b_printer.Name = "b_printer";
            this.b_printer.Size = new System.Drawing.Size(135, 49);
            this.b_printer.TabIndex = 2;
            this.b_printer.Text = "Print to &Printer   ";
            this.b_printer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.b_printer.UseVisualStyleBackColor = true;
            this.b_printer.Click += new System.EventHandler(this.process);
            // 
            // b_screen
            // 
            this.b_screen.Image = ((System.Drawing.Image)(resources.GetObject("b_screen.Image")));
            this.b_screen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.b_screen.Location = new System.Drawing.Point(13, 76);
            this.b_screen.Name = "b_screen";
            this.b_screen.Size = new System.Drawing.Size(135, 49);
            this.b_screen.TabIndex = 3;
            this.b_screen.Text = "View on &Screen   ";
            this.b_screen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.b_screen.UseVisualStyleBackColor = true;
            this.b_screen.Click += new System.EventHandler(this.process);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.b_printer);
            this.groupBox1.Controls.Add(this.b_screen);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(163, 142);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(499, 160);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // print_report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 189);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "print_report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print a Consignment Transaction";
            this.Load += new System.EventHandler(this.print_report_Load);
            this.Shown += new System.EventHandler(this.print_report_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button b_printer;
        private System.Windows.Forms.ListBox form_list;
        private System.Windows.Forms.Button b_screen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}
namespace Multi_Express_Consignment
{
    partial class print_reports_options
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonScreen = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.total_grand = new System.Windows.Forms.CheckBox();
            this.total_daily = new System.Windows.Forms.CheckBox();
            this.total_monthly = new System.Windows.Forms.CheckBox();
            this.report_detailed = new System.Windows.Forms.RadioButton();
            this.report_summary = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.sMonthBox = new System.Windows.Forms.ComboBox();
            this.sMonthCheck = new System.Windows.Forms.RadioButton();
            this.sDateBox = new System.Windows.Forms.DateTimePicker();
            this.sDateCheck = new System.Windows.Forms.RadioButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.input_enddate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.input_startdate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dailysalesreport_options = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.expiryUpto = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.consignorBox = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.dailysalesreport_options.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(201, 366);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(291, 366);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonScreen);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 49);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // radioButtonScreen
            // 
            this.radioButtonScreen.AutoSize = true;
            this.radioButtonScreen.Location = new System.Drawing.Point(108, 19);
            this.radioButtonScreen.Name = "radioButtonScreen";
            this.radioButtonScreen.Size = new System.Drawing.Size(100, 17);
            this.radioButtonScreen.TabIndex = 1;
            this.radioButtonScreen.Text = "View on Screen";
            this.radioButtonScreen.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(11, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(91, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Print to Printer";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // total_grand
            // 
            this.total_grand.AutoSize = true;
            this.total_grand.Checked = true;
            this.total_grand.CheckState = System.Windows.Forms.CheckState.Checked;
            this.total_grand.Location = new System.Drawing.Point(11, 65);
            this.total_grand.Name = "total_grand";
            this.total_grand.Size = new System.Drawing.Size(82, 17);
            this.total_grand.TabIndex = 0;
            this.total_grand.Text = "Grand Total";
            this.total_grand.UseVisualStyleBackColor = true;
            this.total_grand.Visible = false;
            // 
            // total_daily
            // 
            this.total_daily.AutoSize = true;
            this.total_daily.Checked = true;
            this.total_daily.CheckState = System.Windows.Forms.CheckState.Checked;
            this.total_daily.Location = new System.Drawing.Point(11, 19);
            this.total_daily.Name = "total_daily";
            this.total_daily.Size = new System.Drawing.Size(76, 17);
            this.total_daily.TabIndex = 1;
            this.total_daily.Text = "Daily Total";
            this.total_daily.UseVisualStyleBackColor = true;
            // 
            // total_monthly
            // 
            this.total_monthly.AutoSize = true;
            this.total_monthly.Location = new System.Drawing.Point(11, 42);
            this.total_monthly.Name = "total_monthly";
            this.total_monthly.Size = new System.Drawing.Size(90, 17);
            this.total_monthly.TabIndex = 2;
            this.total_monthly.Text = "Monthly Total";
            this.total_monthly.UseVisualStyleBackColor = true;
            // 
            // report_detailed
            // 
            this.report_detailed.AutoSize = true;
            this.report_detailed.Checked = true;
            this.report_detailed.Location = new System.Drawing.Point(11, 88);
            this.report_detailed.Name = "report_detailed";
            this.report_detailed.Size = new System.Drawing.Size(99, 17);
            this.report_detailed.TabIndex = 3;
            this.report_detailed.TabStop = true;
            this.report_detailed.Text = "Detailed Report";
            this.report_detailed.UseVisualStyleBackColor = true;
            // 
            // report_summary
            // 
            this.report_summary.AutoSize = true;
            this.report_summary.Location = new System.Drawing.Point(11, 111);
            this.report_summary.Name = "report_summary";
            this.report_summary.Size = new System.Drawing.Size(103, 17);
            this.report_summary.TabIndex = 4;
            this.report_summary.Text = "Summary Report";
            this.report_summary.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(11, 134);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(179, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Show Individual Item Sale Dates";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 67);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(354, 128);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.sMonthBox);
            this.tabPage2.Controls.Add(this.sMonthCheck);
            this.tabPage2.Controls.Add(this.sDateBox);
            this.tabPage2.Controls.Add(this.sDateCheck);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(346, 102);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Single Month or Date";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // sMonthBox
            // 
            this.sMonthBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sMonthBox.FormattingEnabled = true;
            this.sMonthBox.Location = new System.Drawing.Point(6, 67);
            this.sMonthBox.Name = "sMonthBox";
            this.sMonthBox.Size = new System.Drawing.Size(197, 21);
            this.sMonthBox.TabIndex = 8;
            this.sMonthBox.SelectedValueChanged += new System.EventHandler(this.sMonthBox_SelectedValueChanged);
            this.sMonthBox.VisibleChanged += new System.EventHandler(this.sMonthBox_VisibleChanged);
            this.sMonthBox.Click += new System.EventHandler(this.sMonthBox_Click);
            // 
            // sMonthCheck
            // 
            this.sMonthCheck.AutoSize = true;
            this.sMonthCheck.Location = new System.Drawing.Point(7, 49);
            this.sMonthCheck.Name = "sMonthCheck";
            this.sMonthCheck.Size = new System.Drawing.Size(87, 17);
            this.sMonthCheck.TabIndex = 7;
            this.sMonthCheck.Text = "Single Month";
            this.sMonthCheck.UseVisualStyleBackColor = true;
            // 
            // sDateBox
            // 
            this.sDateBox.Location = new System.Drawing.Point(7, 24);
            this.sDateBox.Name = "sDateBox";
            this.sDateBox.Size = new System.Drawing.Size(197, 20);
            this.sDateBox.TabIndex = 6;
            this.sDateBox.ValueChanged += new System.EventHandler(this.sDateBox_ValueChanged);
            // 
            // sDateCheck
            // 
            this.sDateCheck.AutoSize = true;
            this.sDateCheck.Checked = true;
            this.sDateCheck.Location = new System.Drawing.Point(7, 6);
            this.sDateCheck.Name = "sDateCheck";
            this.sDateCheck.Size = new System.Drawing.Size(80, 17);
            this.sDateCheck.TabIndex = 0;
            this.sDateCheck.TabStop = true;
            this.sDateCheck.Text = "Single Date";
            this.sDateCheck.UseVisualStyleBackColor = true;
            this.sDateCheck.Click += new System.EventHandler(this.sDateCheck_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.input_enddate);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.input_startdate);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(346, 102);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Date Range";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // input_enddate
            // 
            this.input_enddate.Location = new System.Drawing.Point(9, 63);
            this.input_enddate.Name = "input_enddate";
            this.input_enddate.Size = new System.Drawing.Size(197, 20);
            this.input_enddate.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "End Date";
            // 
            // input_startdate
            // 
            this.input_startdate.Location = new System.Drawing.Point(7, 24);
            this.input_startdate.Name = "input_startdate";
            this.input_startdate.Size = new System.Drawing.Size(197, 20);
            this.input_startdate.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Start Date";
            // 
            // dailysalesreport_options
            // 
            this.dailysalesreport_options.Controls.Add(this.checkBox1);
            this.dailysalesreport_options.Controls.Add(this.report_summary);
            this.dailysalesreport_options.Controls.Add(this.report_detailed);
            this.dailysalesreport_options.Controls.Add(this.total_monthly);
            this.dailysalesreport_options.Controls.Add(this.total_daily);
            this.dailysalesreport_options.Controls.Add(this.total_grand);
            this.dailysalesreport_options.Location = new System.Drawing.Point(12, 201);
            this.dailysalesreport_options.Name = "dailysalesreport_options";
            this.dailysalesreport_options.Size = new System.Drawing.Size(354, 159);
            this.dailysalesreport_options.TabIndex = 4;
            this.dailysalesreport_options.TabStop = false;
            this.dailysalesreport_options.Text = "Report Options";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.expiryUpto);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.consignorBox);
            this.groupBox1.Location = new System.Drawing.Point(385, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 128);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Consolidated Consignor Report Options";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(230, 34);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 20);
            this.button3.TabIndex = 4;
            this.button3.Text = "Select";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Expiry Date to Print up to:";
            // 
            // expiryUpto
            // 
            this.expiryUpto.Location = new System.Drawing.Point(9, 83);
            this.expiryUpto.Name = "expiryUpto";
            this.expiryUpto.Size = new System.Drawing.Size(200, 20);
            this.expiryUpto.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Consignor Code:";
            // 
            // consignorBox
            // 
            this.consignorBox.Location = new System.Drawing.Point(9, 34);
            this.consignorBox.Name = "consignorBox";
            this.consignorBox.Size = new System.Drawing.Size(215, 20);
            this.consignorBox.TabIndex = 0;
            // 
            // print_reports_options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 401);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.dailysalesreport_options);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "print_reports_options";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report Options";
            this.Load += new System.EventHandler(this.print_reports_options_Load);
            this.Shown += new System.EventHandler(this.print_reports_options_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.dailysalesreport_options.ResumeLayout(false);
            this.dailysalesreport_options.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonScreen;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox total_grand;
        private System.Windows.Forms.CheckBox total_daily;
        private System.Windows.Forms.CheckBox total_monthly;
        private System.Windows.Forms.RadioButton report_detailed;
        private System.Windows.Forms.RadioButton report_summary;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DateTimePicker input_enddate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker input_startdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox dailysalesreport_options;
        private System.Windows.Forms.DateTimePicker sDateBox;
        private System.Windows.Forms.RadioButton sDateCheck;
        private System.Windows.Forms.RadioButton sMonthCheck;
        private System.Windows.Forms.ComboBox sMonthBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker expiryUpto;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox consignorBox;
    }
}
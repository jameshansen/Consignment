namespace Multi_Express_Consignment
{
    partial class purge_consignments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(purge_consignments));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.dateGroup = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.input_dateFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.input_dateTo = new System.Windows.Forms.DateTimePicker();
            this.numberGroup = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.input_numFrom = new System.Windows.Forms.TextBox();
            this.input_numTo = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxSold = new System.Windows.Forms.CheckBox();
            this.checkBoxUnsold = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.progressGroup = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.currentConsignment = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.dateGroup.SuspendLayout();
            this.numberGroup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.progressGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 71);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Purge by";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(10, 19);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(154, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Consignment Creation Date";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(10, 42);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(161, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Consignment Number Range";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // dateGroup
            // 
            this.dateGroup.Controls.Add(this.input_dateTo);
            this.dateGroup.Controls.Add(this.label2);
            this.dateGroup.Controls.Add(this.input_dateFrom);
            this.dateGroup.Controls.Add(this.label1);
            this.dateGroup.Location = new System.Drawing.Point(12, 89);
            this.dateGroup.Name = "dateGroup";
            this.dateGroup.Size = new System.Drawing.Size(236, 104);
            this.dateGroup.TabIndex = 1;
            this.dateGroup.TabStop = false;
            this.dateGroup.Text = "Date Range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Delete Consignments from";
            // 
            // input_dateFrom
            // 
            this.input_dateFrom.Location = new System.Drawing.Point(10, 32);
            this.input_dateFrom.Name = "input_dateFrom";
            this.input_dateFrom.Size = new System.Drawing.Size(219, 20);
            this.input_dateFrom.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "up to and including";
            // 
            // input_dateTo
            // 
            this.input_dateTo.Location = new System.Drawing.Point(10, 71);
            this.input_dateTo.Name = "input_dateTo";
            this.input_dateTo.Size = new System.Drawing.Size(219, 20);
            this.input_dateTo.TabIndex = 3;
            // 
            // numberGroup
            // 
            this.numberGroup.Controls.Add(this.input_numTo);
            this.numberGroup.Controls.Add(this.input_numFrom);
            this.numberGroup.Controls.Add(this.label3);
            this.numberGroup.Controls.Add(this.label4);
            this.numberGroup.Location = new System.Drawing.Point(277, 89);
            this.numberGroup.Name = "numberGroup";
            this.numberGroup.Size = new System.Drawing.Size(236, 104);
            this.numberGroup.TabIndex = 2;
            this.numberGroup.TabStop = false;
            this.numberGroup.Text = "Number Range";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "up to and including";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Delete Consignments from";
            // 
            // input_numFrom
            // 
            this.input_numFrom.Location = new System.Drawing.Point(11, 32);
            this.input_numFrom.Name = "input_numFrom";
            this.input_numFrom.Size = new System.Drawing.Size(217, 20);
            this.input_numFrom.TabIndex = 3;
            this.input_numFrom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // input_numTo
            // 
            this.input_numTo.Location = new System.Drawing.Point(11, 71);
            this.input_numTo.Name = "input_numTo";
            this.input_numTo.Size = new System.Drawing.Size(217, 20);
            this.input_numTo.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.checkBoxUnsold);
            this.groupBox4.Controls.Add(this.checkBoxSold);
            this.groupBox4.Location = new System.Drawing.Point(12, 199);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(236, 108);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Status Options";
            // 
            // checkBoxSold
            // 
            this.checkBoxSold.AutoSize = true;
            this.checkBoxSold.Checked = true;
            this.checkBoxSold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSold.Location = new System.Drawing.Point(11, 19);
            this.checkBoxSold.Name = "checkBoxSold";
            this.checkBoxSold.Size = new System.Drawing.Size(150, 17);
            this.checkBoxSold.TabIndex = 0;
            this.checkBoxSold.Text = "Delete Sold Consignments";
            this.checkBoxSold.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnsold
            // 
            this.checkBoxUnsold.AutoSize = true;
            this.checkBoxUnsold.Checked = true;
            this.checkBoxUnsold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUnsold.Location = new System.Drawing.Point(11, 42);
            this.checkBoxUnsold.Name = "checkBoxUnsold";
            this.checkBoxUnsold.Size = new System.Drawing.Size(162, 17);
            this.checkBoxUnsold.TabIndex = 1;
            this.checkBoxUnsold.Text = "Delete Unsold Consignments";
            this.checkBoxUnsold.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(173, 313);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 313);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(155, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Begin Purge";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // progressGroup
            // 
            this.progressGroup.Controls.Add(this.currentConsignment);
            this.progressGroup.Controls.Add(this.progressBar1);
            this.progressGroup.Location = new System.Drawing.Point(277, 12);
            this.progressGroup.Name = "progressGroup";
            this.progressGroup.Size = new System.Drawing.Size(236, 71);
            this.progressGroup.TabIndex = 6;
            this.progressGroup.TabStop = false;
            this.progressGroup.Text = "Purging in Progress";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(217, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 0;
            // 
            // currentConsignment
            // 
            this.currentConsignment.AutoSize = true;
            this.currentConsignment.Location = new System.Drawing.Point(8, 46);
            this.currentConsignment.Name = "currentConsignment";
            this.currentConsignment.Size = new System.Drawing.Size(105, 13);
            this.currentConsignment.TabIndex = 1;
            this.currentConsignment.Text = "Current Consignment";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 62);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(221, 35);
            this.label5.TabIndex = 2;
            this.label5.Text = "If the consignment is partially sold it won\'t be removed unless both boxes are ch" +
                "ecked.";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // purge_consignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 350);
            this.Controls.Add(this.progressGroup);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.numberGroup);
            this.Controls.Add(this.dateGroup);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "purge_consignments";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purge Consignments";
            this.Load += new System.EventHandler(this.purge_consignments_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dateGroup.ResumeLayout(false);
            this.dateGroup.PerformLayout();
            this.numberGroup.ResumeLayout(false);
            this.numberGroup.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.progressGroup.ResumeLayout(false);
            this.progressGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox dateGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker input_dateTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker input_dateFrom;
        private System.Windows.Forms.GroupBox numberGroup;
        private System.Windows.Forms.TextBox input_numFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox input_numTo;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxUnsold;
        private System.Windows.Forms.CheckBox checkBoxSold;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox progressGroup;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label currentConsignment;
        private System.Windows.Forms.Label label5;
    }
}
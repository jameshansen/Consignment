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
            this.dateGroup = new System.Windows.Forms.GroupBox();
            this.input_dateTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.input_dateFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.deletePayments = new System.Windows.Forms.CheckBox();
            this.checkBoxUnsold = new System.Windows.Forms.CheckBox();
            this.checkBoxSold = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.progressGroup = new System.Windows.Forms.GroupBox();
            this.currentConsignment = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.dateGroup.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.progressGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            this.radioButton1.Location = new System.Drawing.Point(11, 25);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(102, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Item Expiry Date";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
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
            // input_dateTo
            // 
            this.input_dateTo.Location = new System.Drawing.Point(10, 71);
            this.input_dateTo.Name = "input_dateTo";
            this.input_dateTo.Size = new System.Drawing.Size(219, 20);
            this.input_dateTo.TabIndex = 3;
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
            // input_dateFrom
            // 
            this.input_dateFrom.Location = new System.Drawing.Point(10, 32);
            this.input_dateFrom.Name = "input_dateFrom";
            this.input_dateFrom.Size = new System.Drawing.Size(219, 20);
            this.input_dateFrom.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Delete Items that Expired from";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.deletePayments);
            this.groupBox4.Controls.Add(this.checkBoxUnsold);
            this.groupBox4.Controls.Add(this.checkBoxSold);
            this.groupBox4.Location = new System.Drawing.Point(12, 199);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(236, 119);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Purge Options";
            // 
            // deletePayments
            // 
            this.deletePayments.AutoSize = true;
            this.deletePayments.Checked = true;
            this.deletePayments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.deletePayments.Location = new System.Drawing.Point(11, 65);
            this.deletePayments.Name = "deletePayments";
            this.deletePayments.Size = new System.Drawing.Size(192, 17);
            this.deletePayments.TabIndex = 2;
            this.deletePayments.Text = "Delete Payments Attached to Items";
            this.deletePayments.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnsold
            // 
            this.checkBoxUnsold.AutoSize = true;
            this.checkBoxUnsold.Checked = true;
            this.checkBoxUnsold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUnsold.Location = new System.Drawing.Point(11, 42);
            this.checkBoxUnsold.Name = "checkBoxUnsold";
            this.checkBoxUnsold.Size = new System.Drawing.Size(121, 17);
            this.checkBoxUnsold.TabIndex = 1;
            this.checkBoxUnsold.Text = "Delete Unsold Items";
            this.checkBoxUnsold.UseVisualStyleBackColor = true;
            // 
            // checkBoxSold
            // 
            this.checkBoxSold.AutoSize = true;
            this.checkBoxSold.Checked = true;
            this.checkBoxSold.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSold.Location = new System.Drawing.Point(11, 19);
            this.checkBoxSold.Name = "checkBoxSold";
            this.checkBoxSold.Size = new System.Drawing.Size(109, 17);
            this.checkBoxSold.TabIndex = 0;
            this.checkBoxSold.Text = "Delete Sold Items";
            this.checkBoxSold.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(173, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 324);
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
            // currentConsignment
            // 
            this.currentConsignment.AutoSize = true;
            this.currentConsignment.Location = new System.Drawing.Point(8, 46);
            this.currentConsignment.Name = "currentConsignment";
            this.currentConsignment.Size = new System.Drawing.Size(105, 13);
            this.currentConsignment.TabIndex = 1;
            this.currentConsignment.Text = "Current Consignment";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 19);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(217, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "(Only if Consignment is Empty)";
            // 
            // purge_consignments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 359);
            this.Controls.Add(this.progressGroup);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.dateGroup);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "purge_consignments";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purge Items";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.purge_consignments_FormClosing);
            this.Load += new System.EventHandler(this.purge_consignments_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.dateGroup.ResumeLayout(false);
            this.dateGroup.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.progressGroup.ResumeLayout(false);
            this.progressGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox dateGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker input_dateTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker input_dateFrom;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxUnsold;
        private System.Windows.Forms.CheckBox checkBoxSold;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox progressGroup;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label currentConsignment;
        private System.Windows.Forms.CheckBox deletePayments;
        private System.Windows.Forms.Label label3;
    }
}
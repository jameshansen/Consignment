namespace Multi_Express_Consignment
{
    partial class change_status_cpo
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radio_cancelled = new System.Windows.Forms.RadioButton();
            this.radio_invoiced = new System.Windows.Forms.RadioButton();
            this.radio_systemdef2 = new System.Windows.Forms.RadioButton();
            this.radio_systemdef1 = new System.Windows.Forms.RadioButton();
            this.radio_pending = new System.Windows.Forms.RadioButton();
            this.radio_workcompleted = new System.Windows.Forms.RadioButton();
            this.radio_inprogress = new System.Windows.Forms.RadioButton();
            this.radio_open = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.radio_cancelled);
            this.groupBox1.Controls.Add(this.radio_invoiced);
            this.groupBox1.Controls.Add(this.radio_systemdef2);
            this.groupBox1.Controls.Add(this.radio_systemdef1);
            this.groupBox1.Controls.Add(this.radio_pending);
            this.groupBox1.Controls.Add(this.radio_workcompleted);
            this.groupBox1.Controls.Add(this.radio_inprogress);
            this.groupBox1.Controls.Add(this.radio_open);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 276);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status Options";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(2, 222);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(266, 4);
            this.panel2.TabIndex = 18;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(1, 167);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(266, 4);
            this.panel1.TabIndex = 17;
            // 
            // radio_cancelled
            // 
            this.radio_cancelled.AutoSize = true;
            this.radio_cancelled.Location = new System.Drawing.Point(12, 242);
            this.radio_cancelled.Name = "radio_cancelled";
            this.radio_cancelled.Size = new System.Drawing.Size(72, 17);
            this.radio_cancelled.TabIndex = 16;
            this.radio_cancelled.Text = "Cancelled";
            this.radio_cancelled.UseVisualStyleBackColor = true;
            this.radio_cancelled.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_invoiced
            // 
            this.radio_invoiced.AutoSize = true;
            this.radio_invoiced.Location = new System.Drawing.Point(12, 187);
            this.radio_invoiced.Name = "radio_invoiced";
            this.radio_invoiced.Size = new System.Drawing.Size(66, 17);
            this.radio_invoiced.TabIndex = 15;
            this.radio_invoiced.Text = "Invoiced";
            this.radio_invoiced.UseVisualStyleBackColor = true;
            this.radio_invoiced.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_systemdef2
            // 
            this.radio_systemdef2.AutoSize = true;
            this.radio_systemdef2.Location = new System.Drawing.Point(12, 134);
            this.radio_systemdef2.Name = "radio_systemdef2";
            this.radio_systemdef2.Size = new System.Drawing.Size(104, 17);
            this.radio_systemdef2.TabIndex = 14;
            this.radio_systemdef2.TabStop = true;
            this.radio_systemdef2.Text = "system defined 2";
            this.radio_systemdef2.UseVisualStyleBackColor = true;
            this.radio_systemdef2.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_systemdef1
            // 
            this.radio_systemdef1.AutoSize = true;
            this.radio_systemdef1.Location = new System.Drawing.Point(12, 111);
            this.radio_systemdef1.Name = "radio_systemdef1";
            this.radio_systemdef1.Size = new System.Drawing.Size(104, 17);
            this.radio_systemdef1.TabIndex = 13;
            this.radio_systemdef1.TabStop = true;
            this.radio_systemdef1.Text = "system defined 1";
            this.radio_systemdef1.UseVisualStyleBackColor = true;
            this.radio_systemdef1.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_pending
            // 
            this.radio_pending.AutoSize = true;
            this.radio_pending.Location = new System.Drawing.Point(12, 88);
            this.radio_pending.Name = "radio_pending";
            this.radio_pending.Size = new System.Drawing.Size(64, 17);
            this.radio_pending.TabIndex = 11;
            this.radio_pending.TabStop = true;
            this.radio_pending.Text = "Pending";
            this.radio_pending.UseVisualStyleBackColor = true;
            this.radio_pending.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_workcompleted
            // 
            this.radio_workcompleted.AutoSize = true;
            this.radio_workcompleted.Location = new System.Drawing.Point(12, 65);
            this.radio_workcompleted.Name = "radio_workcompleted";
            this.radio_workcompleted.Size = new System.Drawing.Size(104, 17);
            this.radio_workcompleted.TabIndex = 10;
            this.radio_workcompleted.TabStop = true;
            this.radio_workcompleted.Text = "Work Completed";
            this.radio_workcompleted.UseVisualStyleBackColor = true;
            this.radio_workcompleted.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_inprogress
            // 
            this.radio_inprogress.AutoSize = true;
            this.radio_inprogress.Location = new System.Drawing.Point(12, 42);
            this.radio_inprogress.Name = "radio_inprogress";
            this.radio_inprogress.Size = new System.Drawing.Size(78, 17);
            this.radio_inprogress.TabIndex = 9;
            this.radio_inprogress.TabStop = true;
            this.radio_inprogress.Text = "In Progress";
            this.radio_inprogress.UseVisualStyleBackColor = true;
            this.radio_inprogress.CheckedChanged += new System.EventHandler(this.checkedChanged);
            // 
            // radio_open
            // 
            this.radio_open.AutoSize = true;
            this.radio_open.Checked = true;
            this.radio_open.Location = new System.Drawing.Point(12, 19);
            this.radio_open.Name = "radio_open";
            this.radio_open.Size = new System.Drawing.Size(51, 17);
            this.radio_open.TabIndex = 8;
            this.radio_open.TabStop = true;
            this.radio_open.Text = "Open";
            this.radio_open.UseVisualStyleBackColor = true;
            this.radio_open.CheckedChanged += new System.EventHandler(this.checkedChanged);
            this.radio_open.Click += new System.EventHandler(this.checkedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(205, 294);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(124, 294);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // change_status_cpo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 327);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "change_status_cpo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Change Consignment Order Status";
            this.Load += new System.EventHandler(this.change_status_cpo_Load);
            this.Shown += new System.EventHandler(this.change_status_cpo_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton radio_systemdef1;
        private System.Windows.Forms.RadioButton radio_pending;
        private System.Windows.Forms.RadioButton radio_workcompleted;
        private System.Windows.Forms.RadioButton radio_inprogress;
        private System.Windows.Forms.RadioButton radio_open;
        private System.Windows.Forms.RadioButton radio_systemdef2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radio_cancelled;
        private System.Windows.Forms.RadioButton radio_invoiced;
    }
}
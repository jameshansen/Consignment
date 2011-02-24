namespace Multi_Express_Consignment
{
    partial class payment_entry
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(payment_entry));
            this.label1 = new System.Windows.Forms.Label();
            this.pe_code = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.payment_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payment_description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel_step2 = new System.Windows.Forms.Panel();
            this.paymentNotes = new System.Windows.Forms.Label();
            this.pe_expiry = new System.Windows.Forms.MaskedTextBox();
            this.labelexpiry = new System.Windows.Forms.Label();
            this.pe_payment_total = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pe_vendor_name = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pe_vendor_code = new System.Windows.Forms.TextBox();
            this.pe_reference = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pe_description = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.selectalldelay = new System.Windows.Forms.Timer(this.components);
            this.panel_step3 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.ltex_totalcost = new System.Windows.Forms.Label();
            this.ltex_amountpaid = new System.Windows.Forms.Label();
            this.ltex_outstanding = new System.Windows.Forms.Label();
            this.lval_outstanding = new System.Windows.Forms.Label();
            this.lval_amountpaid = new System.Windows.Forms.Label();
            this.lval_totalcost = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel_step2.SuspendLayout();
            this.panel_step3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Payment Code";
            // 
            // pe_code
            // 
            this.pe_code.Location = new System.Drawing.Point(143, 6);
            this.pe_code.Name = "pe_code";
            this.pe_code.Size = new System.Drawing.Size(117, 20);
            this.pe_code.TabIndex = 1;
            this.pe_code.TabStop = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.payment_code,
            this.payment_description});
            this.dataGridView1.Location = new System.Drawing.Point(15, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(369, 213);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // payment_code
            // 
            this.payment_code.HeaderText = "Payment Code";
            this.payment_code.Name = "payment_code";
            this.payment_code.ReadOnly = true;
            this.payment_code.Width = 163;
            // 
            // payment_description
            // 
            this.payment_description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.payment_description.HeaderText = "Description";
            this.payment_description.Name = "payment_description";
            this.payment_description.ReadOnly = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(143, 251);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Select";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(309, 251);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.TabStop = false;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel_step2
            // 
            this.panel_step2.Controls.Add(this.paymentNotes);
            this.panel_step2.Controls.Add(this.pe_expiry);
            this.panel_step2.Controls.Add(this.labelexpiry);
            this.panel_step2.Controls.Add(this.pe_payment_total);
            this.panel_step2.Controls.Add(this.label5);
            this.panel_step2.Controls.Add(this.pe_vendor_name);
            this.panel_step2.Controls.Add(this.label4);
            this.panel_step2.Controls.Add(this.pe_vendor_code);
            this.panel_step2.Controls.Add(this.pe_reference);
            this.panel_step2.Controls.Add(this.label3);
            this.panel_step2.Controls.Add(this.pe_description);
            this.panel_step2.Controls.Add(this.label2);
            this.panel_step2.Location = new System.Drawing.Point(403, 32);
            this.panel_step2.Name = "panel_step2";
            this.panel_step2.Size = new System.Drawing.Size(369, 213);
            this.panel_step2.TabIndex = 13;
            // 
            // paymentNotes
            // 
            this.paymentNotes.AutoSize = true;
            this.paymentNotes.Location = new System.Drawing.Point(125, 179);
            this.paymentNotes.Name = "paymentNotes";
            this.paymentNotes.Size = new System.Drawing.Size(0, 13);
            this.paymentNotes.TabIndex = 14;
            // 
            // pe_expiry
            // 
            this.pe_expiry.Location = new System.Drawing.Point(128, 52);
            this.pe_expiry.Mask = "99/99";
            this.pe_expiry.Name = "pe_expiry";
            this.pe_expiry.Size = new System.Drawing.Size(69, 20);
            this.pe_expiry.TabIndex = 2;
            // 
            // labelexpiry
            // 
            this.labelexpiry.AutoSize = true;
            this.labelexpiry.Location = new System.Drawing.Point(-3, 55);
            this.labelexpiry.Name = "labelexpiry";
            this.labelexpiry.Size = new System.Drawing.Size(107, 13);
            this.labelexpiry.TabIndex = 13;
            this.labelexpiry.Text = "Expiry Date (MM/YY)";
            // 
            // pe_payment_total
            // 
            this.pe_payment_total.Location = new System.Drawing.Point(128, 156);
            this.pe_payment_total.Mask = "999999.9999";
            this.pe_payment_total.Name = "pe_payment_total";
            this.pe_payment_total.PromptChar = ' ';
            this.pe_payment_total.Size = new System.Drawing.Size(117, 20);
            this.pe_payment_total.TabIndex = 3;
            this.pe_payment_total.Enter += new System.EventHandler(this.pe_payment_total_Enter);
            this.pe_payment_total.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pe_payment_total_KeyDown);
            this.pe_payment_total.Leave += new System.EventHandler(this.stringToCurrencyEvt);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Payment Amount";
            // 
            // pe_vendor_name
            // 
            this.pe_vendor_name.Location = new System.Drawing.Point(128, 104);
            this.pe_vendor_name.Name = "pe_vendor_name";
            this.pe_vendor_name.ReadOnly = true;
            this.pe_vendor_name.Size = new System.Drawing.Size(241, 20);
            this.pe_vendor_name.TabIndex = 7;
            this.pe_vendor_name.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Consignee";
            this.label4.Visible = false;
            // 
            // pe_vendor_code
            // 
            this.pe_vendor_code.Location = new System.Drawing.Point(128, 78);
            this.pe_vendor_code.Name = "pe_vendor_code";
            this.pe_vendor_code.ReadOnly = true;
            this.pe_vendor_code.Size = new System.Drawing.Size(241, 20);
            this.pe_vendor_code.TabIndex = 5;
            this.pe_vendor_code.TabStop = false;
            // 
            // pe_reference
            // 
            this.pe_reference.Location = new System.Drawing.Point(128, 26);
            this.pe_reference.Name = "pe_reference";
            this.pe_reference.Size = new System.Drawing.Size(241, 20);
            this.pe_reference.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Card Number / Reference";
            // 
            // pe_description
            // 
            this.pe_description.Location = new System.Drawing.Point(128, 0);
            this.pe_description.Name = "pe_description";
            this.pe_description.Size = new System.Drawing.Size(241, 20);
            this.pe_description.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Description";
            // 
            // selectalldelay
            // 
            this.selectalldelay.Tick += new System.EventHandler(this.selectalldelay_Tick);
            // 
            // panel_step3
            // 
            this.panel_step3.Controls.Add(this.lval_totalcost);
            this.panel_step3.Controls.Add(this.lval_amountpaid);
            this.panel_step3.Controls.Add(this.lval_outstanding);
            this.panel_step3.Controls.Add(this.ltex_outstanding);
            this.panel_step3.Controls.Add(this.ltex_amountpaid);
            this.panel_step3.Controls.Add(this.ltex_totalcost);
            this.panel_step3.Controls.Add(this.label6);
            this.panel_step3.Location = new System.Drawing.Point(801, 32);
            this.panel_step3.Name = "panel_step3";
            this.panel_step3.Size = new System.Drawing.Size(369, 213);
            this.panel_step3.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(154, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "Payment Overview";
            // 
            // ltex_totalcost
            // 
            this.ltex_totalcost.AutoSize = true;
            this.ltex_totalcost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltex_totalcost.Location = new System.Drawing.Point(89, 41);
            this.ltex_totalcost.Name = "ltex_totalcost";
            this.ltex_totalcost.Size = new System.Drawing.Size(104, 20);
            this.ltex_totalcost.TabIndex = 1;
            this.ltex_totalcost.Text = "Order Total:";
            // 
            // ltex_amountpaid
            // 
            this.ltex_amountpaid.AutoSize = true;
            this.ltex_amountpaid.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltex_amountpaid.Location = new System.Drawing.Point(77, 61);
            this.ltex_amountpaid.Name = "ltex_amountpaid";
            this.ltex_amountpaid.Size = new System.Drawing.Size(116, 20);
            this.ltex_amountpaid.TabIndex = 2;
            this.ltex_amountpaid.Text = "Amount Paid:";
            // 
            // ltex_outstanding
            // 
            this.ltex_outstanding.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltex_outstanding.Location = new System.Drawing.Point(3, 81);
            this.ltex_outstanding.Name = "ltex_outstanding";
            this.ltex_outstanding.Size = new System.Drawing.Size(190, 20);
            this.ltex_outstanding.TabIndex = 4;
            this.ltex_outstanding.Text = "Amount Outstanding:";
            this.ltex_outstanding.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lval_outstanding
            // 
            this.lval_outstanding.AutoSize = true;
            this.lval_outstanding.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lval_outstanding.ForeColor = System.Drawing.Color.Red;
            this.lval_outstanding.Location = new System.Drawing.Point(199, 81);
            this.lval_outstanding.Name = "lval_outstanding";
            this.lval_outstanding.Size = new System.Drawing.Size(54, 20);
            this.lval_outstanding.TabIndex = 5;
            this.lval_outstanding.Text = "$0.00";
            // 
            // lval_amountpaid
            // 
            this.lval_amountpaid.AutoSize = true;
            this.lval_amountpaid.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lval_amountpaid.ForeColor = System.Drawing.Color.Black;
            this.lval_amountpaid.Location = new System.Drawing.Point(199, 61);
            this.lval_amountpaid.Name = "lval_amountpaid";
            this.lval_amountpaid.Size = new System.Drawing.Size(54, 20);
            this.lval_amountpaid.TabIndex = 7;
            this.lval_amountpaid.Text = "$0.00";
            // 
            // lval_totalcost
            // 
            this.lval_totalcost.AutoSize = true;
            this.lval_totalcost.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lval_totalcost.ForeColor = System.Drawing.Color.Black;
            this.lval_totalcost.Location = new System.Drawing.Point(199, 41);
            this.lval_totalcost.Name = "lval_totalcost";
            this.lval_totalcost.Size = new System.Drawing.Size(54, 20);
            this.lval_totalcost.TabIndex = 8;
            this.lval_totalcost.Text = "$0.00";
            // 
            // payment_entry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1207, 286);
            this.Controls.Add(this.panel_step3);
            this.Controls.Add(this.panel_step2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.pe_code);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "payment_entry";
            this.Text = "Select Payment";
            this.Load += new System.EventHandler(this.payment_entry_Load);
            this.Shown += new System.EventHandler(this.payment_entry_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel_step2.ResumeLayout(false);
            this.panel_step2.PerformLayout();
            this.panel_step3.ResumeLayout(false);
            this.panel_step3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pe_code;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn payment_description;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel_step2;
        private System.Windows.Forms.TextBox pe_description;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pe_reference;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox pe_vendor_code;
        private System.Windows.Forms.TextBox pe_vendor_name;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox pe_payment_total;
        private System.Windows.Forms.MaskedTextBox pe_expiry;
        private System.Windows.Forms.Label labelexpiry;
        private System.Windows.Forms.Timer selectalldelay;
        private System.Windows.Forms.Label paymentNotes;
        private System.Windows.Forms.Panel panel_step3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label ltex_amountpaid;
        private System.Windows.Forms.Label ltex_totalcost;
        private System.Windows.Forms.Label lval_totalcost;
        private System.Windows.Forms.Label lval_amountpaid;
        private System.Windows.Forms.Label lval_outstanding;
        private System.Windows.Forms.Label ltex_outstanding;
    }
}
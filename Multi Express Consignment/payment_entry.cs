using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Multi_Express_Consignment
{
    public partial class payment_entry : Form
    {
        public static string vendor_code;
        public static string vendor_name;
        public static string total_cost;

        public payment_entry(Form calledBy, string inVendor_code, string inVendor_name, string inTotal_cost)
        {
            InitializeComponent();
            m_parent = calledBy;

            vendor_code = inVendor_code;
            vendor_name = inVendor_name;
            total_cost = inTotal_cost;
        }

        private static Form m_parent = null;

        private void payment_entry_Shown(object sender, EventArgs e)
        {
            // Lock window size
            this.MaximumSize = new System.Drawing.Size(this.Width, this.Height);
            this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            DataGridViewRow outputRow = null;
            // Create Payment Types
            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);       
            outputRow.Cells[0].Value = "A/R"; outputRow.Cells[1].Value = "Accounts Receivable"; dataGridView1.Rows.Add(outputRow);
            pe_code.Text = "A/R";

            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);       
            outputRow.Cells[0].Value = "AMEX"; outputRow.Cells[1].Value = "American Express"; dataGridView1.Rows.Add(outputRow);
            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);  
            outputRow.Cells[0].Value = "CASH"; outputRow.Cells[1].Value = "CASH"; dataGridView1.Rows.Add(outputRow);
            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);  
            outputRow.Cells[0].Value = "CHEQUE"; outputRow.Cells[1].Value = "Cheque"; dataGridView1.Rows.Add(outputRow);
            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);  
            outputRow.Cells[0].Value = "MASTERCARD"; outputRow.Cells[1].Value = "Master Card"; dataGridView1.Rows.Add(outputRow);
            outputRow = new DataGridViewRow(); outputRow.CreateCells(dataGridView1);  
            outputRow.Cells[0].Value = "VISA"; outputRow.Cells[1].Value = "VISA"; dataGridView1.Rows.Add(outputRow);


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            pe_code.Text = Convert.ToString(dataGridView1.SelectedRows[0].Cells[0].Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void payment_entry_Load(object sender, EventArgs e)
        {
            this.Width = 404; // Hide Second Screen
            this.Top = m_parent.Top;
            this.Left = m_parent.Left + m_parent.Width + 10;
        }

        private string stringToCurrency(string input)
        {
            string output = "";

            if (input.IndexOf(".") == -1)
            {
                input = input + ".00";
            }

            int spaces = 5 - input.IndexOf(".");
            if (spaces < 1)
            {
                output = input;
            }
            else
            {
                for (int i = 0; i <= spaces; i++) output = output + " ";
                output = output + input;
            }

            return output;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Select")
            {
                if (pe_code.Text == "CASH")
                {
                    button2.Text = "Calculate Change and Add Payment";
                }
                else
                {
                    button2.Text = "Add Payment";
                }
                pe_code.Text = Convert.ToString(dataGridView1.SelectedRows[0].Cells[0].Value);
                pe_description.Text = Convert.ToString(dataGridView1.SelectedRows[0].Cells[1].Value);

                pe_vendor_code.Text = vendor_code;
                pe_vendor_name.Text = vendor_name;
                pe_payment_total.Text = stringToCurrency(total_cost);

                if (pe_code.Text == "VISA" || pe_code.Text == "MASTERCARD" || pe_code.Text == "AMEX")
                {
                    pe_expiry.Visible = true;
                    labelexpiry.Visible = true;
                }
                else
                {
                    pe_expiry.Visible = false;
                    labelexpiry.Visible = false;
                }

                pe_code.ReadOnly = true;
                dataGridView1.Visible = false;
                panel_step2.Left = dataGridView1.Left;

            }

            /* Cash Only */
            if (button2.Text == "Calculate Change and Add Payment")
            {
                lval_totalcost.Text = cg.price(total_cost);
                lval_amountpaid.Text = cg.price(pe_payment_total.Text);

                decimal difference = Convert.ToDecimal(lval_amountpaid.Text) - Convert.ToDecimal(ltex_totalcost);

                if (difference > 0)
                {
                    // Change
                    ltex_outstanding.Text = "Change:";
                    lval_outstanding.Text = cg.price(difference);
                }
                else
                {
                    // Amount Outstanding
                    ltex_outstanding.Text = "Amount Outstanding:";
                    lval_outstanding.Text = cg.price(difference * -1);
                }



            }
            /* End of Cash Only Clause */

            if (button2.Text == "Add Payment")
            {
                if (m_parent.Name == "consignment_purchase_order")
                {
                    ((consignment_purchase_order)m_parent).addPayment(pe_code.Text, pe_description.Text, pe_reference.Text, pe_expiry.Text, pe_vendor_code.Text, pe_vendor_name.Text, cg.price(pe_payment_total.Text));
                }
                if (m_parent.Name == "consignment_sale_order")
                {
                    ((consignment_sale_order)m_parent).addPayment(pe_code.Text, pe_description.Text, pe_reference.Text, pe_expiry.Text, pe_vendor_code.Text, pe_vendor_name.Text, cg.price(pe_payment_total.Text));
                }
                
                this.Close();
            }
        }

        private string currencyAlign(string input)
        {
            string output = null;
            if (input.IndexOf(" .") != -1)
            {
                // Remove Dot and Trim
                input = input.Replace(".", " ");
                input = input.Trim();
                output = input.PadLeft(6);
            }
            else
            {
                output = input;
            }
            return output;
        }

        private void pe_payment_total_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ((Control)sender).Text = currencyAlign(((Control)sender).Text);
            }
        }

        private void pe_payment_total_Enter(object sender, EventArgs e)
        {
            selectalldelay.Enabled = true;
        }

        private void selectalldelay_Tick(object sender, EventArgs e)
        {
            pe_payment_total.SelectAll();
            selectalldelay.Enabled = false;
        }

        private void stringToCurrencyEvt(object sender, EventArgs e)
        {
            (sender as MaskedTextBox).Text = stringToCurrency(cg.price((sender as MaskedTextBox).Text));
        }

    }
}

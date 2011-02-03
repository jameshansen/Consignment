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
    public partial class add_item_to_order : Form
    {
        public consignment_sale_order m_parent = null;

        public string m_upc;
        public string m_description;
        public string m_price_suggested;
        public string m_price_minimum;
        public string m_date_expiry;
        public string m_consignment;

        public add_item_to_order(consignment_sale_order calledBy, string upc, string description, string price_suggested, string price_minimum, string date_expiry, string consignment)
        {
            InitializeComponent();
            m_parent = calledBy;

            m_upc = upc;
            m_description = description;
            m_price_suggested = price_suggested;
            m_price_minimum = price_minimum;
            m_date_expiry = date_expiry;
            m_consignment = consignment;
        }

        private void add_item_to_order_Load(object sender, EventArgs e)
        {
            this.Top = m_parent.Top;
            this.Left = m_parent.Left + m_parent.Width + 10;
        }

        private void add_item_to_order_Shown(object sender, EventArgs e)
        {
            /* Load up Data */
            item_upc.Text = m_upc;
            item_description.Text = m_description;
            input_price.Text = stringToCurrency(m_price_suggested);
            item_price_suggested.Text = m_price_suggested;
            item_price_minimum.Text = m_price_minimum;
            item_date_expiry.Text = m_date_expiry;
            item_consignment.Text = m_consignment;


            /* Focus and Highlight Price Input */
            input_price.Focus();
            input_price.SelectAll();
        }

        public string final_price
        {
            get
            {
                return cg.price(input_price.Text);
            }
        }

        private void button_price1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private string stringToCurrency(string input)
        {
            string output = "";

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

        private void currency_check_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ((Control)sender).Text = currencyAlign(((Control)sender).Text);
            }

            if (e.KeyCode == Keys.Enter)
            {
                button_price1_Click(null, null);
            }
        }

        private void button_price2_Click(object sender, EventArgs e)
        {
            input_price.Text = stringToCurrency(item_price_suggested.Text);
            input_price.Focus();
            input_price.SelectAll();
        }

        private void button_price3_Click(object sender, EventArgs e)
        {
            input_price.Text = stringToCurrency(item_price_minimum.Text);
            input_price.Focus();
            input_price.SelectAll();
        }


    }
}

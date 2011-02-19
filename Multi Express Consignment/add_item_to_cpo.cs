using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace Multi_Express_Consignment
{
    public partial class add_item_to_cpo : Form
    {

        private static consignment_purchase_order m_parent = null;
        private static int m_rowIndex = -1;

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

        public add_item_to_cpo(consignment_purchase_order calledBy, int rowIndex)
        {
            InitializeComponent();
            m_parent = calledBy;
            m_rowIndex = rowIndex;
        }

        private void add_item_to_cpo_Shown(object sender, EventArgs e)
        {

            // Drop Down Share Type Default
            input_share_type.SelectedIndex = input_share_type.FindString(@"%");

            // Clear existing UPC since no two items would have the same
            existing_upc.Text = "";

            // Expiry Date Default
            // Date 1: 31st January
            // Date 2: 30th June

            if (DateTime.Now.Month < 6)
            {
                // June Expiry
                input_date_expiry.Value = new DateTime(DateTime.Now.Year, 6, 30);
            }
            else
            {
                // Jan Year + 1 Expiry
                input_date_expiry.Value = new DateTime(DateTime.Now.Year + 1, 1, 31);
            }

            // Load up Row Data if in Edit Mode
            if (m_rowIndex >= 0)
            {
                this.Text = "Edit Item in Consignment Order";
                button2.Text = "Save Changes";
                input_description.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[2].Value);
                input_price_minimum.Text = stringToCurrency(Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[3].Value));
                input_price_suggested.Text = stringToCurrency(Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[4].Value));
                input_share.Text = stringToCurrency(Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[6].Value));
                input_share_type.SelectedIndex = input_share_type.FindString(Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[7].Value));

                // Load Received and Expiry Dates
                input_date_received.Value = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(m_parent.dataGridView1.Rows[m_rowIndex].Cells[8].Value));
                input_date_expiry.Value = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(m_parent.dataGridView1.Rows[m_rowIndex].Cells[9].Value));

                input_desc_brand.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[13].Value);
                input_desc_gender.SelectedIndex = input_desc_gender.FindString(Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[14].Value));
                input_desc_garment.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[15].Value);
                input_desc_material.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[16].Value);
                input_desc_colour.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[17].Value);
                input_desc_size.Text = Convert.ToString(m_parent.dataGridView1.Rows[m_rowIndex].Cells[18].Value);
            }
            else
            {
                this.Text = "Add Item to Consignment Order";
            }

            // Consignment Code on Title            
            this.Text = this.Text + " #" + m_parent.getconsignment_code();



            // Compile matrix lists
            string strSQL = "SELECT * FROM `CSTITEM`";
            MySqlCommand mysqlCmd = new MySqlCommand(strSQL, mysqlglobal.mysqlCon);
            MySqlDataAdapter myDA = new MySqlDataAdapter(mysqlCmd);
            DataSet item_file = new DataSet();
            myDA.Fill(item_file, "CSTITEM");



            foreach (DataRow row in item_file.Tables["CSTITEM"].Rows)
            {
                if (input_desc_brand.Items.IndexOf(row["desc_brand"]) == -1) input_desc_brand.Items.Add(row["desc_brand"]);
                if (input_desc_garment.Items.IndexOf(row["desc_garment"]) == -1) input_desc_garment.Items.Add(row["desc_garment"]);
                if (input_desc_material.Items.IndexOf(row["desc_material"]) == -1) input_desc_material.Items.Add(row["desc_material"]);
                if (input_desc_colour.Items.IndexOf(row["desc_colour"]) == -1) input_desc_colour.Items.Add(row["desc_colour"]);
                if (input_desc_size.Items.IndexOf(row["desc_size"]) == -1) input_desc_size.Items.Add(row["desc_size"]);
                if (input_description.AutoCompleteCustomSource.IndexOf(Convert.ToString(row["description"])) == -1) input_description.AutoCompleteCustomSource.Add(Convert.ToString(row["description"]));
            }

            // Focus on Description
            input_description.Focus();
            input_description.SelectAll();

        }
     

        private void button2_Click(object sender, EventArgs e)
        {
            /* Validation */
            string error_list = "";

            if (input_description.Text == "")
            {
                error_list += "* Item Description cannot be blank" + Environment.NewLine;
            }

            if (input_price_minimum.Text.Replace(" ", "") == ".")
            {
                error_list += "* Minimum price cannot be blank" + Environment.NewLine;
            }

            if (input_price_suggested.Text.Replace(" ", "") == ".")
            {
                error_list += "* Suggested price cannot be blank" + Environment.NewLine;
            }

            if (error_list != "")
            {
                MessageBox.Show("The following errors were found:" + Environment.NewLine + error_list, "Alert");
                return;
            }

            /* Add Item */
            m_parent.addItem(input_description.Text, cg.price(input_price_minimum.Text), cg.price(input_price_suggested.Text), cg.price(input_share.Text), input_share_type.Text, input_desc_brand.Text, input_desc_gender.Text, input_desc_garment.Text, input_desc_material.Text, input_desc_colour.Text, input_desc_size.Text, input_date_received.Value, input_date_expiry.Value, existing_upc.Text, m_rowIndex);
            this.Close();
        }

        private void add_item_to_cpo_Load(object sender, EventArgs e)
        {
            this.Top = m_parent.Top;
            this.Left = m_parent.Left + m_parent.Width + 10;
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

        private void currency_check_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ((Control)sender).Text = currencyAlign(((Control)sender).Text);
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void highlightContents(object sender, EventArgs e)
        {
            if (sender is TextBox) 
            {
                ((TextBox)sender).SelectionStart = 0;
                ((TextBox)sender).SelectionLength = ((TextBox)sender).Text.Length;
            }

            if(sender is MaskedTextBox)
            {
                this.BeginInvoke((MethodInvoker)delegate()
                {
                    ((MaskedTextBox)sender).SelectAll();
                });
            }

        }

        private void nextBox(object sender, KeyPressEventArgs e)
        {
        if (e.KeyChar == '\r')
            {
            e.Handled = true;
            System.Windows.Forms.SendKeys.Send("{TAB}");
            }
        }

        private void input_description_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_price_minimum.Focus();
            }
        }

        private void input_desc_brand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_desc_gender.Focus();
            }
        }

        private void input_desc_garment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_desc_material.Focus();
            }
        }

        private void input_desc_colour_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_desc_size.Focus();
            }
        }

        private void input_desc_size_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2.Focus();
            }
        }

        private void input_desc_material_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_desc_colour.Focus();
            }
        }

}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using System.Data.OleDb;
using System.Xml.Serialization;

namespace Multi_Express_Consignment
{
    public partial class select_vendor_or_customer : Form
    {

        public string m_mode = "";

        public select_vendor_or_customer(string mode)
        {
            InitializeComponent();
            m_mode = mode;
        }

        public void gotoList(string search_term)
        {
            // Highlist Record
            int best_match = searchglobal.findRow(search_term, dataGridView1, 0);
            dataGridView1.CurrentCell = this.dataGridView1[0, best_match];

        }

        public void loadList()
        {
            string dbf = "";
            if (m_mode == "vendor") dbf = "PSVEMAST";
            if (m_mode == "customer") dbf = "SFCUMAST";

            string query = "SELECT * FROM " + dbf + " ORDER BY CMCUCODE ASC";
            OleDbCommand accesscommand = new OleDbCommand(query, dbfglobal.dbfCon); // Query -> Result
            OleDbDataAdapter vendorAdaptor = new OleDbDataAdapter(accesscommand); // Result -> Adaptor

            dbfglobal.dbfCon.Open();
            DataSet vendor_data = new DataSet();
            vendorAdaptor.Fill(vendor_data, dbf); // Adaptor -> vendor_data
            dbfglobal.dbfCon.Close();

            dataGridView1.Rows.Clear(); // Empty Data Grid

            foreach (DataRow vendor_row in vendor_data.Tables[dbf].Rows)
            {
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);

                outputRow.Cells[0].Value = vendor_row["CMCUCODE"]; // Vendor Code
                outputRow.Cells[1].Value = vendor_row["CMCUNAME"]; // Vendor Company Name
                outputRow.Cells[2].Value = vendor_row["CMPHONE"]; // Vendor Phone Number
                outputRow.Cells[3].Value = vendor_row["CMNAMESUR"]; // Vendor Last Name
                outputRow.Cells[4].Value = vendor_row["CMNAME1ST"]; // Vendor First Name

                dataGridView1.Rows.Add(outputRow);

            }
        }

        private void select_vendor_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();

            /* Set Text Fields Based on Mode */
            if (m_mode == "customer")
            {
                this.Text = "Select a Customer";
                label1.Text = "Search for Customer Code";
                vendor_code.HeaderText = "Customer Code";
            }

            loadList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedID = Convert.ToString(dataGridView1.SelectedRows[0].Cells[0].Value);
            this.Close();

            if (m_mode == "vendor")
            {
                //MessageBox.Show(selectedVendor);
                consignment_purchase_order cpo = new consignment_purchase_order(null, selectedID, null);
                cpo.Show();
            }

            if (m_mode == "customer")
            {
                //MessageBox.Show(selectedVendor);
                consignment_sale_order cpo = new consignment_sale_order(null, selectedID, null);
                cpo.Show();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            add_vendor av = new add_vendor(this, m_mode);
            av.ShowDialog(this);
            if (av.new_entry != "cancelled")
            {
                gotoList(av.new_entry);
                button1_Click(null, null);
            }
            
        }

        private void select_vendor_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text != "")
                {
                    // Assume they've found who they're looking for
                    button1_Click(null, null);
                }
                else
                {
                    // Nothing Typed, they probably want to create a new person
                    button2_Click(null, null);
                }

            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            gotoList(textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Selected Row #" + Convert.ToString(dataGridView1.SelectedRows[0].Index) + " out of " + Convert.ToString(dataGridView1.RowCount));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(null, null); // Trigger Select Button Routine
        }
    }
}

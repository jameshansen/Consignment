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
        public bool m_return_selection = false;
        public int searchCellIndex = 0;


        public select_vendor_or_customer(string mode, bool return_selection = false)
        {
            InitializeComponent();
            m_mode = mode;
            m_return_selection = return_selection;
        }

        public void gotoList(string search_term)
        {
            // Highlist Record
            int best_match = searchglobal.findRow(search_term, dataGridView1, searchCellIndex);
            dataGridView1.CurrentCell = this.dataGridView1[searchCellIndex, best_match];

        }

        public void loadList()
        {
            // Position Search Box
            textBox1.Left = label1.Left + label1.Width + 10; 

            string dbf = "";
            if (m_mode == "vendor") dbf = "PSVEMAST";
            if (m_mode == "customer") dbf = "SFCUMAST";

            string query = "SELECT * FROM " + dbf + " ORDER BY CMCUCODE ASC";
            DataSet vendor_data = mysqlglobal.executeDataSetQuery(query, dbf, this);

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

            // Sort by search key (new)
            dataGridView1.Sort(dataGridView1.Columns[searchCellIndex], ListSortDirection.Ascending);

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
            else
            {
                /* Make Vendor First Name Default */
                toolStripButton3_Click(null, null);
            }

            loadList();
        }

        public string return_var = "";

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedID = Convert.ToString(dataGridView1.SelectedRows[0].Cells[0].Value);
            this.Close();

            if (m_return_selection)
            {
                return_var = selectedID;
                return;
            }

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

            if (m_mode == "vendor")
            {
                //MessageBox.Show(selectedVendor);
                consignment_purchase_order cpo = null;
                av.ShowDialog(cpo);
                if (av.new_entry != "cancelled")
                {
                    cpo = new consignment_purchase_order(null, av.new_entry, null);
                    cpo.Show();
                    this.Close();
                    cpo.Focus();
                }
            }

            if (m_mode == "customer")
            {
                //MessageBox.Show(selectedVendor);
                consignment_sale_order cpo = null;
                av.ShowDialog(cpo);
                if (av.new_entry != "cancelled")
                {
                    cpo = new consignment_sale_order(null, av.new_entry, null);
                    cpo.Show();
                    this.Close();
                    cpo.Focus();
                }
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


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button1_Click(null, null); // Trigger Select Button Routine
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Code
            if (m_mode == "customer")
            {
                label1.Text = "Search for Customer Code";
            }
            else
            {
                label1.Text = "Search for Vendor Code";
            }
            searchCellIndex = 0;
            loadList();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            label1.Text = "Search for Phone Number";
            searchCellIndex = 2;
            loadList();
        }


        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            label1.Text = "Search for Last Name";
            searchCellIndex = 3;
            loadList();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            label1.Text = "Search for First Name";
            searchCellIndex = 4;
            loadList();
        }

    }
}

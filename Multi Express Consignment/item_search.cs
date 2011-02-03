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
    public partial class item_search : Form
    {
        public string i_search_key;
        public string i_search_value;
        public static Form m_parent;
        public string m_mode;
        public string added_items;

        public item_search(string initial_search_key, string initial_search_value, Form calledBy, string mode, string addedItems)
        {
            InitializeComponent();

            i_search_key = initial_search_key;
            i_search_value = initial_search_value;
            m_parent = calledBy;
            m_mode = mode;
            added_items = addedItems; // item1,item2,item3
        }

        public Dictionary<string, string> key2db = new Dictionary<string, string>()
        {
            {"UPC", "upc"},
            {"Description", "description"},
            {"Brand", "desc_brand"},
            {"Gender", "desc_gender"},
            {"Garment", "desc_garment"},
            {"Material", "desc_material"},
            {"Colour", "desc_colour"},
            {"Size", "desc_size"}
        };

        private Dictionary<string, string> getSearchRow(int index) {
            Dictionary<string, string> output = new Dictionary<string, string>();

            string indexStr = Convert.ToString(index);

            output.Add("field", searchpanel.Controls.Find("field_" + indexStr, true)[0].Text);
            output.Add("condition", searchpanel.Controls.Find("cond_" + indexStr, true)[0].Text);
            output.Add("value", searchpanel.Controls.Find("value_" + indexStr, true)[0].Text);

            return output;
        }

        private void item_search_Shown(object sender, EventArgs e)
        {
            /* Populate Drop Downs */
            for (int i = 1; i <= 5; i++)
            {
                string indexStr = Convert.ToString(i);
                Control field = searchpanel.Controls.Find("field_" + indexStr, true)[0];
                Control cond = searchpanel.Controls.Find("cond_" + indexStr, true)[0];

                ((ComboBox)cond).Items.Add("");
                ((ComboBox)cond).Items.Add("starts with");
                ((ComboBox)cond).Items.Add("contains");
                ((ComboBox)cond).Items.Add("does not contain");

                ((ComboBox)field).Items.Add("");
                foreach (var entry in key2db)
                {
                    ((ComboBox)field).Items.Add(entry.Key);
                }

            }

            /* Populate first row if key provided */

            if (i_search_key != null)
            {
                field_1.SelectedIndex = field_1.FindString(i_search_key);
                cond_1.SelectedIndex = cond_1.FindString("starts with");
                value_1.Text = i_search_value;
                value_1.Focus();
                value_1.SelectAll();

                /* Auto Search */
                searchRoutine();
            }
            else
            {
                /* Default Search Description */
                field_1.SelectedIndex = field_1.FindString("Description");
                cond_1.SelectedIndex = cond_1.FindString("contains");
                value_1.Text = "";
                value_1.Focus();
                value_1.SelectAll();
            }


            



            /* Switch Search Box Mode */
            if (m_mode == "consignment_sale_order")
            {
                openSale.Text = "Add Item to Sale";
                openConsignment.Visible = false;
                openSale.Enabled = true;
            }

            

        }

        

        public void consignment_sale_order_add()
        {
            this.Hide();
            this.Close();
            Application.DoEvents();
            consignment_sale_order parent = (m_parent as consignment_sale_order);
            
            parent.addItem(s_upc); // Stapler
        }

        public consignment_purchase_order cpo = null;
        public consignment_sale_order cso = null;

        private void openSale_Click(object sender, EventArgs e)
        {
            checkSelectedRowCO();

            if (m_mode == "consignment_sale_order")
            {
                consignment_sale_order_add();
                return;
            }

            // Else, open up the Sale
            cso = new consignment_sale_order(s_order.ToString(), null, s_upc);
            cso.Show();
        }

        private int findColumn(DataGridView dgv, string search)
        {
            int output = -1;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (dgv.Columns[i].Name == search)
                {
                    output = i;
                    break;
                }
            }
            return output;
        }

        private void searchRoutine()
        {
            string query = "";
            bool andAtTheEnd = false;
            bool whereAtTheEnd = false;

            query += "SELECT * FROM `CSTITEM` WHERE ";
            whereAtTheEnd = true;

            for (int i = 1; i <= 5; i++)
            {
                string indexStr = Convert.ToString(i);
                Control field = searchpanel.Controls.Find("field_" + indexStr, true)[0];
                Control cond = searchpanel.Controls.Find("cond_" + indexStr, true)[0];
                Control value = searchpanel.Controls.Find("value_" + indexStr, true)[0];

                if (field.Text != "" && cond.Text != "" && value.Text != "")
                {
                    string q_field = key2db[field.Text];
                    string q_cond = "";
                    string q_value = mysqlglobal.escapeString(value.Text);

                    if (cond.Text == "starts with")
                    {
                        q_cond = "LIKE";
                        q_value = q_value + "%";
                    }

                    if (cond.Text == "contains")
                    {
                        q_cond = "LIKE";
                        q_value = "%" + q_value + "%";
                    }

                    if (cond.Text == "does not contain")
                    {
                        q_cond = "NOT LIKE";
                        q_value = "%" + q_value + "%";
                    }

                    query += "`" + q_field + "` " + q_cond + " \"" + q_value + "\" AND";
                    andAtTheEnd = true;
                    whereAtTheEnd = false;
                }
            }

            if (m_mode == "consignment_sale_order")
            {
                /* Only UnSold Items */
                query += "`status` = \"unsold\"";
                andAtTheEnd = false;
                whereAtTheEnd = false;
            
                if (added_items != "")
                {
                    // item1,item2,item3 -> `upc` != "item1" AND `upc` != "item2"
                    string additional = " AND `upc` != " + added_items.Replace(",", " AND `upc` != ");

                    query += additional;
                    andAtTheEnd = false;
                    whereAtTheEnd = false;
                }
            }

            if (andAtTheEnd == true)
            {
                query = query.Substring(0, query.Length - 4); // Remove Last " AND"
            }
            if (whereAtTheEnd == true)
            {
                query = query.Substring(0, query.Length - 6); // Remove "WHERE "
            }

            // Debug:
            //MessageBox.Show(query);

            DataSet results = mysqlglobal.executeDataSetQuery(query, "CSTITEM", this);

            /* Clear and Populate DataGrid */
            dataGridView1.Rows.Clear();

            foreach(DataRow resultRow in results.Tables["CSTITEM"].Rows) {
                /* Experimental Auto-Mapping */
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);

                /* Build Output Row */
                foreach (DataColumn col in results.Tables["CSTITEM"].Columns)
                {                                     
                    string dgvColumnName = "sg_" + col.ColumnName;
                    int columnIndex = findColumn(dataGridView1, dgvColumnName);

                    if (columnIndex != -1)
                    {
                        /* It Exists! Map It! */
                        outputRow.Cells[columnIndex].Value = resultRow[col.ColumnName];
                    }

                }
                /* End Build Output Row */

                /* Add Row to Results */
                dataGridView1.Rows.Add(outputRow);
            }

            /* Check Selected Item */
            checkSelectedRowCO();



        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchRoutine();
        }

        private void field_1_TextChanged(object sender, EventArgs e)
        {
            // Choose Contains
            cond_1.SelectedIndex = cond_1.FindString("contains");
        }

        private void field_2_TextChanged(object sender, EventArgs e)
        {
            // Choose Contains
            cond_2.SelectedIndex = cond_2.FindString("contains");
        }

        private void field_3_TextChanged(object sender, EventArgs e)
        {
            // Choose Contains
            cond_3.SelectedIndex = cond_3.FindString("contains");
        }

        private void field_4_TextChanged(object sender, EventArgs e)
        {
            // Choose Contains
            cond_4.SelectedIndex = cond_4.FindString("contains");
        }

        private void field_5_TextChanged(object sender, EventArgs e)
        {
            // Choose Contains
            cond_5.SelectedIndex = cond_5.FindString("contains");
        }

        public int s_consign = 0;
        public int s_order = 0;
        public string s_upc = "";
        private void checkSelectedRowCO()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                s_consign = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[sg_consignment_code.Index].Value);
                s_order = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[sg_order_number.Index].Value);
                s_upc = Convert.ToString(dataGridView1.SelectedRows[0].Cells[sg_upc.Index].Value);

                //MessageBox.Show("Consignment #" + Convert.ToString(s_consign) + " Order #" + Convert.ToString(s_order));

                if (m_mode == null)
                {
                    if (s_consign > 0)
                    {
                        openConsignment.Enabled = true;
                    }
                    else
                    {
                        openConsignment.Enabled = false;
                    }

                    if (s_order > 0)
                    {
                        openSale.Enabled = true;
                    }
                    else
                    {
                        openSale.Enabled = false;
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            checkSelectedRowCO();
        }

        private void enterTriggerSearch(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                searchRoutine();       
                e.Handled = true;         
            }
        }

        private void openConsignment_Click(object sender, EventArgs e)
        {
            // Open up the Consignment
            cpo = new consignment_purchase_order(s_consign.ToString(), null, s_upc);
            cpo.Show();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_mode == "consignment_sale_order")
            {
                openSale_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Please Choose to open the Sale or Consignment for this item using the buttons below.", "Alert");
            }
        }

        private void clearRow_Click(object sender, EventArgs e)
        {
            string indexStr = ((Control)sender).Name.Substring(6);
            Control field = searchpanel.Controls.Find("field_" + indexStr, true)[0];
            Control cond = searchpanel.Controls.Find("cond_" + indexStr, true)[0];
            Control value = searchpanel.Controls.Find("value_" + indexStr, true)[0];

            field.Text = "";
            cond.Text = "";
            value.Text = "";

        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            checkSelectedRowCO();
        }




    }
}

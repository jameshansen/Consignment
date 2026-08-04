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

using System.Data.OleDb;
using System.Xml.Serialization;

namespace Multi_Express_Consignment
{
    public partial class consignment_sale_desktop : Form
    {
        public consignment_sale_desktop()
        {
            InitializeComponent();

            // Bugfix (2026): Keep the record buttons greyed out until there is a record to work on
            dataGridView1.SelectionChanged += rowSelectionChanged;
            rowSelectionChanged(null, null);
        }

        // Bugfix (2026): Nothing selected (or nothing listed) means nothing to open, print or scroll to
        private void rowSelectionChanged(object sender, EventArgs e)
        {
            bool rowSelected = dataGridView1.SelectedRows.Count > 0;
            bool anyRows = dataGridView1.RowCount > 0;

            button2.Enabled = rowSelected; // Update
            button4.Enabled = rowSelected; // Print
            toolStripButton3.Enabled = rowSelected; // Update
            toolStripButton5.Enabled = rowSelected; // Print

            toolStripButton6.Enabled = anyRows; // First record
            toolStripButton7.Enabled = anyRows; // Up 10
            toolStripButton8.Enabled = anyRows; // Up 1
            toolStripButton9.Enabled = anyRows; // Down 1
            toolStripButton10.Enabled = anyRows; // Down 10
            toolStripButton11.Enabled = anyRows; // Last record
        }

        private void windowProportions()
        {
            var formWidth = this.Width;
            var formHeight = this.Height;

            var topDistance = 53;
            var bottomDistance = 134;
            var leftDistance = 12;
            var rightDistance = 20;

            dataGridView1.Width = formWidth - leftDistance - rightDistance;
            dataGridView1.Height = formHeight - topDistance - bottomDistance;

            panel1.Top = formHeight - 37 - panel1.Height;
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        private void consignment_sale_desktop_Resize(object sender, EventArgs e)
        {
            // Adjust Proportions of Data Area
            windowProportions();
        }

        public static OleDbDataAdapter customerAdaptor = null;

        public static OleDbCommand accesscommand = null;
        public static string query = null;

        public static string search_key = "order_number";
        public static string listMode = "orders";

        public void loadOrders()
        {
            // Clear DataGridView
            dataGridView1.Rows.Clear();

            // Load MySQL data
            //MySqlCommand mysqlCmd = null;

            // Check options
            bool displayInvoice = checkBoxInvoice.Checked;
            bool displayOrder = checkBoxOrder.Checked;

            string whereQuery = "";

            if (listMode == "orders")
            {

                if (displayInvoice) whereQuery = "WHERE `status` = \"invoice\"";
                if (displayOrder) whereQuery = "WHERE `status` = \"order\"";
            }


            if (displayInvoice && displayOrder) whereQuery = "WHERE `status` = \"invoice\" OR `status` = \"order\"";

            string strSQL = "SELECT * FROM `CSTORDER` " + whereQuery + " ORDER BY `order_number` ASC";
            DataSet item_file = mysqlglobal.executeDataSetQuery(strSQL, "CSTORDER", null);

            // Hide all Columns
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Visible = false; // Hide the column
            }

            fillercolumn.Visible = true;

            // If in Consignment Mode
            if (listMode == "orders")
            {
                // Enable Needed Columni
                og_icon.Visible = true;
                og_order_number.Visible = true;
                og_customer_code.Visible = true;
                og_customer_first_name.Visible = true;
                og_customer_last_name.Visible = true;
                og_phone.Visible = true;
                og_total_items.Visible = true;
                og_total_amount.Visible = true;
                og_status.Visible = true;

                string status = "";
                foreach (DataRow row in item_file.Tables["CSTORDER"].Rows)
                {

                    // Fetch Data on Customer
                    query = "SELECT * FROM SFCUMAST WHERE CMCUCODE = \"" + row["customer_code"] + "\"";
                    DataSet customer_data = mysqlglobal.executeDataSetQuery(query, "SFCUMAST", this);
                    DataRow customer_row = customer_data.Tables["SFCUMAST"].Rows[0]; // customer_data -> customer_row

                    // End customer Data Fetch

                        DataGridViewRow outputRow = new DataGridViewRow();
                        outputRow.CreateCells(dataGridView1);

                        status = row["status"].ToString();
                        
                        if(status == "invoice") outputRow.Cells[og_icon.Index].Value = imageList1.Images[1];
                        if(status == "order") outputRow.Cells[og_icon.Index].Value = imageList1.Images[0];

                        outputRow.Cells[og_order_number.Index].Value = row["order_number"];
                        outputRow.Cells[og_customer_code.Index].Value = row["customer_code"];

                        outputRow.Cells[og_customer_first_name.Index].Value = customer_row["CMNAME1ST"]; // Vendor First Name
                        outputRow.Cells[og_customer_last_name.Index].Value = customer_row["CMNAMESUR"]; // Vendor Last Name
                        outputRow.Cells[og_phone.Index].Value = customer_row["CMPHONE"]; // Vendor Last Name

                        outputRow.Cells[og_total_items.Index].Value = row["items"];
                        outputRow.Cells[og_total_amount.Index].Value = cg.price(row["total"]);

                        if (imageList2.Images.IndexOfKey(row["order_status"].ToString()) != -1)
                        {
                            outputRow.Cells[og_status.Index].Value = imageList2.Images[row["order_status"].ToString()];
                        }
                        else
                        {
                            outputRow.Cells[og_status.Index].Value = imageList2.Images[0];
                        }


                        dataGridView1.Rows.Add(outputRow);
                   
                }
            }

            // Move Cursor to Bottom
            if(dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1[0, dataGridView1.Rows.Count - 1];

            rowSelectionChanged(null, null); // Bugfix (2026): List has changed, re-check the buttons

            /*
            if (listMode == "items")
            {
                // Enable Needed Columni
                cg_icon.Visible = true;
                cg_upc.Visible = true;
                cg_description.Visible = true;

                cg_price_minimum.Visible = true;
                cg_price_sale.Visible = true;
                cg_price_suggested.Visible = true;

                cg_consignment_code.Visible = true;
                cg_vendor_code.Visible = true;

                cg_date_received.Visible = true;
                cg_date_expiry.Visible = true;
                cg_date_sold.Visible = true;
                cg_date_paid.Visible = true;

                cg_status.Visible = false;

                string status = "";
                foreach (DataRow row in item_file.Tables["CSTITEM"].Rows)
                {
                    DataGridViewRow outputRow = new DataGridViewRow();
                    outputRow.CreateCells(dataGridView1);

                    status = row["status"].ToString();

                    if (status == "sold") outputRow.Cells[cg_icon.Index].Value = imageList1.Images[1];
                    if (status == "unsold") outputRow.Cells[cg_icon.Index].Value = imageList1.Images[0];

                    outputRow.Cells[cg_upc.Index].Value = row["upc"];
                    outputRow.Cells[cg_description.Index].Value = row["description"];

                    outputRow.Cells[cg_price_minimum.Index].Value = row["price_minimum"];
                    outputRow.Cells[cg_price_sale.Index].Value = row["price_sale"];
                    outputRow.Cells[cg_price_suggested.Index].Value = row["price_suggested"];

                    outputRow.Cells[cg_consignment_code.Index].Value = row["consignment_code"];
                    outputRow.Cells[cg_vendor_code.Index].Value = row["vendor_code"];

                    outputRow.Cells[cg_date_received.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_received"])));
                    outputRow.Cells[cg_date_expiry.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_expiry"])));
                    outputRow.Cells[cg_date_sold.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_sold"])));
                    outputRow.Cells[cg_date_paid.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_paid"])));

                    dataGridView1.Rows.Add(outputRow);
                }
            }
             * */
            toolStripButton13_Click(null, null);
        }

        private void consignment_sale_desktop_Shown(object sender, EventArgs e)
        {
            windowProportions();

            loadOrders();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Browse selected Order
            string selectedOrder = "";
            try
            {
                selectedOrder = Convert.ToString(dataGridView1.SelectedRows[0].Cells[og_order_number.Index].Value);
            }
            catch
            {
                MessageBox.Show("Please select an Order", "No Record Selected");
                return;
            }
            string selectedUPC = null;
            if (og_upc.Visible == true)
            {
                selectedUPC = Convert.ToString(dataGridView1.SelectedRows[0].Cells[og_upc.Index].Value);
            }

            consignment_sale_order cpo = new consignment_sale_order(selectedOrder, null, selectedUPC);
            cpo.Show();
        }

        private void consignment_sale_desktop_Load(object sender, EventArgs e)
        {
           // Position in top left.
            this.Top = 10;
            this.Left = 10;
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            loadOrders();
        }

        


        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an Order", "No Record Selected"); // Bugfix (2026): Was indexing an empty selection
                return;
            }

            // Print Reports
            string selectedOrder = Convert.ToString(dataGridView1.SelectedRows[0].Cells[og_order_number.Index].Value);
            print_report prt = new print_report(null, selectedOrder, null, "Order Receipt");
            prt.ShowDialog(this);
           
        }

        public select_vendor_or_customer select_vendor = null;

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (select_vendor == null)
            {
                select_vendor = new select_vendor_or_customer("customer");
            }
            else
            {
                select_vendor.Dispose();
                select_vendor = new select_vendor_or_customer("customer");
            }
            select_vendor.MdiParent = this.MdiParent;
            select_vendor.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            button2_Click(null, null);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            button4_Click(null, null);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            // Up to top
            //dataGridView1.Rows[dataGridView1.RowCount].Selected = true;
            dataGridView1.CurrentCell = this.dataGridView1[0, 0];
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            // Up 10 records
            int goToRow = Math.Max(dataGridView1.CurrentCell.RowIndex - 10,0);
            dataGridView1.CurrentCell = this.dataGridView1[0, goToRow];
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // Up 1 record
            int goToRow = Math.Max(dataGridView1.CurrentCell.RowIndex - 1, 0);
            dataGridView1.CurrentCell = this.dataGridView1[0, goToRow];
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Double Clicked on entry
            button2_Click(null, null);
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            // Down 1 record
            int goToRow = Math.Min(dataGridView1.CurrentCell.RowIndex + 1, (dataGridView1.RowCount - 1));
            dataGridView1.CurrentCell = this.dataGridView1[0, goToRow];
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            // Down 10 records
            int goToRow = Math.Min(dataGridView1.CurrentCell.RowIndex + 10, (dataGridView1.RowCount - 1));
            dataGridView1.CurrentCell = this.dataGridView1[0, goToRow];
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            // Last record
            dataGridView1.CurrentCell = this.dataGridView1[0, dataGridView1.RowCount - 1];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Open Record in Read Only Mode

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            button3_Click(null, null);
        }

        private void SoldUnsoldCheckedChanged(object sender, EventArgs e)
        {
            loadOrders();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void toolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            int cellIndex = 0;
            if (search_key == "order_number") cellIndex = og_order_number.Index;
            
            if (search_key == "upc")   cellIndex = og_upc.Index;

            if (search_key == "customer_last_name") cellIndex = og_customer_last_name.Index;
            if (search_key == "customer_first_name") cellIndex = og_customer_first_name.Index;

            if (dataGridView1.RowCount == 0) return; // Bugfix (2026): Nothing listed to jump to

            int best_match = searchglobal.findRow(toolStripTextBox1.Text, dataGridView1, cellIndex);
            dataGridView1.CurrentCell = this.dataGridView1[0, best_match];
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            // By Order Number
            listMode = "orders";
            search_key = "order_number";
            label_searchkey.Text = "Search Key: By Order Number";
            //loadOrders();
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            // Weirdly hacked Item Search Form

            if ((this.MdiParent as Form1).item_search_form == null || (this.MdiParent as Form1).item_search_form.IsDisposed == true)
            {
                (this.MdiParent as Form1).item_search_form = new item_search(null, null, null, null, null);
            }
            (this.MdiParent as Form1).item_search_form.MdiParent = this.MdiParent;
            (this.MdiParent as Form1).item_search_form.Show();
        }


        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            search_key = "customer_last_name";
            label_searchkey.Text = "Search Key: By Last Name";
            dataGridView1.Sort(dataGridView1.Columns[og_customer_last_name.Index],ListSortDirection.Ascending);
            
            //loadOrders();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            search_key = "customer_first_name";
            label_searchkey.Text = "Search Key: By First Name";
            dataGridView1.Sort(dataGridView1.Columns[og_customer_first_name.Index], ListSortDirection.Ascending);
            //loadOrders();
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

    }
}

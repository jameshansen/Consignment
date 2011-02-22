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
    public partial class consignment_purchase_desktop : Form
    {
        public consignment_purchase_desktop()
        {
            InitializeComponent();
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

        private void consignment_purchase_desktop_Resize(object sender, EventArgs e)
        {
            // Adjust Proportions of Data Area
            windowProportions();
        }

        public static OleDbDataAdapter vendorAdaptor = null;

        public static OleDbCommand accesscommand = null;
        public static string query = null;

        public static string search_key = "consignment_code";

        public void loadConsignments()
        {
            // Clear DataGridView
            dataGridView1.Rows.Clear();

            // Load MySQL data
            MySqlCommand mysqlCmd = null;

            // Check options
            bool displaySold = checkBoxSold.Checked;
            bool displayUnsold = checkBoxUnsold.Checked;

            string whereQuery = "";

            if (listMode == "consignment")
            {

                if (displaySold) whereQuery =   "WHERE `consignment_code` IN (SELECT `consignment_code` FROM `CSTITEM` WHERE `status` = \"sold\")";
                if (displayUnsold) whereQuery = "WHERE `consignment_code` IN (SELECT `consignment_code` FROM `CSTITEM` WHERE `status` = \"unsold\")";
                /* Explanation of the above subquery
                 * SELECT * FROM  `CSTITEM` 
                 * WHERE  `consignment_code` 
                 * IN (
                 * SELECT  `consignment_code` 
                 * FROM  `CSTITEM`
                 * WHERE  `status` =  "sold"
                 * )
                 * 
                 * This query in brackets is ran, this selects all the Consignments where 'status' is sold. The parent query selects all the rows with these consignment numbers. This means
                 * *ALL* the items, not just sold/unsold items, in partially sold consignments are returned.
                 * */
            }
            if (listMode == "items")
            {
                if (displaySold) whereQuery = "WHERE `status` = \"sold\"";
                if (displayUnsold) whereQuery = "WHERE `status` = \"unsold\"";
            }

            if (displaySold && displayUnsold) whereQuery = "WHERE `status` = \"unsold\" OR `status` = \"sold\"";

            string strSQL = "SELECT * FROM `CSTITEM` " + whereQuery + " ORDER BY `" + search_key + "` ASC";
            DataSet item_file = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", null);

            // Hide all Columns
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.Visible = false; // Hide the column
            }

            fillercolumn.Visible = true;

            // If in Consignment Mode
            if (listMode == "consignment")
            {
                // Enable Needed Columni
                cg_icon.Visible = true;
                cg_consignment_code.Visible = true;
                cg_vendor_code.Visible = true;
                cg_vendor_first_name.Visible = true;
                cg_vendor_last_name.Visible = true;
                cg_phone.Visible = true;
                cg_total_items.Visible = true;
                cg_status.Visible = true;

                string prev_consignment_code = null;
                string status = "";
                string shareprefix = "";
                foreach (DataRow row in item_file.Tables["CSTITEM"].Rows)
                {

                    // Fetch Data on Vendor
                    query = "SELECT * FROM PSVEMAST WHERE CMCUCODE = \"" + row["vendor_code"] + "\"";
                    accesscommand = new OleDbCommand(query, dbfglobal.dbfCon); // Query -> Result
                    vendorAdaptor = new OleDbDataAdapter(accesscommand); // Result -> Adaptor

                    dbfglobal.dbfCon.Open();
                    DataSet vendor_data = new DataSet();
                    vendorAdaptor.Fill(vendor_data, "PSVEMAST"); // Adaptor -> vendor_data
                    dbfglobal.dbfCon.Close();
                    DataRow vendor_row = vendor_data.Tables["PSVEMAST"].Rows[0]; // vendor_data -> vendor_row

                    // End Vendor Data Fetch



                    if (prev_consignment_code != Convert.ToString(row["consignment_code"]))
                    {
                        DataGridViewRow outputRow = new DataGridViewRow();
                        outputRow.CreateCells(dataGridView1);

                        status = row["status"].ToString();
                        
                        if(status == "sold") outputRow.Cells[cg_icon.Index].Value = imageList1.Images[1];
                        if(status == "unsold") outputRow.Cells[cg_icon.Index].Value = imageList1.Images[0];

                        outputRow.Cells[cg_consignment_code.Index].Value = row["consignment_code"];
                        outputRow.Cells[cg_vendor_code.Index].Value = row["vendor_code"];

                        outputRow.Cells[cg_vendor_first_name.Index].Value = vendor_row["CMNAME1ST"]; // Vendor First Name
                        outputRow.Cells[cg_vendor_last_name.Index].Value = vendor_row["CMNAMESUR"]; // Vendor Last Name
                        outputRow.Cells[cg_phone.Index].Value = vendor_row["CMPHONE"]; // Vendor Last Name

                        outputRow.Cells[cg_total_items.Index].Value = 1;

                        if (imageList2.Images.IndexOfKey(row["consignment_status"].ToString()) != -1)
                        {
                            outputRow.Cells[cg_status.Index].Value = imageList2.Images[row["consignment_status"].ToString()];
                        }
                        else
                        {
                            outputRow.Cells[cg_status.Index].Value = imageList2.Images[0];
                        }


                        dataGridView1.Rows.Add(outputRow);
                    }
                    else
                    {
                        var lastRow = dataGridView1.Rows.Count - 1;
                        dataGridView1.Rows[lastRow].Cells[cg_total_items.Index].Value = Convert.ToString(Convert.ToInt32(dataGridView1.Rows[lastRow].Cells[cg_total_items.Index].Value) + 1);


                        if (row["status"].ToString() != status && status != "partial")
                        {
                            //MessageBox.Show("First Status in Consignment " + row["consignment_code"] + ": '" + status + "' new status: '" + row["status"].ToString() + "'");
                            status = "partial";
                            dataGridView1.Rows[lastRow].Cells[cg_icon.Index].Value = imageList1.Images[2];
                        }
                    }


                    prev_consignment_code = Convert.ToString(row["consignment_code"]);
                }
            }
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

                    outputRow.Cells[cg_price_minimum.Index].Value = cg.price(row["price_minimum"]);
                    outputRow.Cells[cg_price_sale.Index].Value = cg.price(row["price_sale"]);
                    outputRow.Cells[cg_price_suggested.Index].Value = cg.price(row["price_suggested"]);

                    outputRow.Cells[cg_consignment_code.Index].Value = row["consignment_code"];
                    outputRow.Cells[cg_vendor_code.Index].Value = row["vendor_code"];

                    outputRow.Cells[cg_date_received.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_received"])));
                    outputRow.Cells[cg_date_expiry.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_expiry"])));
                    outputRow.Cells[cg_date_sold.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_sold"])));
                    outputRow.Cells[cg_date_paid.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_paid"])));

                    dataGridView1.Rows.Add(outputRow);
                }
            }

        }

        private void consignment_purchase_desktop_Shown(object sender, EventArgs e)
        {
            windowProportions();

            loadConsignments();

        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Browse selected Purchase Desktop
            string selectedConsignment = "";
            try
            {
                selectedConsignment = Convert.ToString(dataGridView1.SelectedRows[0].Cells[cg_consignment_code.Index].Value);
            }
            catch
            {
                MessageBox.Show("Please select a Consignment", "No Record Selected");
                return;
            }
            string selectedUPC = null;
            if (cg_upc.Visible == true)
            {
                selectedUPC = Convert.ToString(dataGridView1.SelectedRows[0].Cells[cg_upc.Index].Value);
            }

            consignment_purchase_order cpo = new consignment_purchase_order(selectedConsignment, null, selectedUPC);
            cpo.Show();
        }

        private void consignment_purchase_desktop_Load(object sender, EventArgs e)
        {
           
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            loadConsignments();
        }

        public static string listMode = "consignment";

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            listMode = "items";
            search_key = "upc";
            label_searchkey.Text = "Search Key: By UPC";

            modeDisplay.Text = "Item Mode";
            modeDisplay.BackColor = Color.Aqua;

            loadConsignments();
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            listMode = "consignment";
            search_key = "consignment_code";
            label_searchkey.Text = "Search Key: By Consignment Code";

            modeDisplay.Text = "Consignment Mode";
            modeDisplay.BackColor = Color.FromArgb(255, 138, 0);
            
            loadConsignments();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Print Reports
            string selectedConsignment = Convert.ToString(dataGridView1.SelectedRows[0].Cells[cg_consignment_code.Index].Value);
            print_report prt = new print_report(selectedConsignment, null, "Consignment Agreement");
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
                select_vendor = new select_vendor_or_customer("vendor");
            }
            else
            {
                select_vendor.Dispose();
                select_vendor = new select_vendor_or_customer("vendor");
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
            loadConsignments();
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
            if (search_key == "consignment_code")
            {
                cellIndex = cg_consignment_code.Index;
            }
            if (search_key == "upc")
            {
                cellIndex = cg_upc.Index;
            } 
            

            int best_match = searchglobal.findRow(toolStripTextBox1.Text, dataGridView1, cellIndex);
            dataGridView1.CurrentCell = this.dataGridView1[0, best_match];
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            purge_consignments purgeForm = new purge_consignments();
            //purgeForm.MdiParent = this.MdiParent;
            purgeForm.ShowDialog(this);
        }


    }
}

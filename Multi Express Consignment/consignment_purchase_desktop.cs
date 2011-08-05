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

using System.Runtime.InteropServices;

namespace Multi_Express_Consignment
{
    public partial class consignment_purchase_desktop : Form
    {
        public int searchCellIndex = 0;

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

        public void gotoList(string search_term)
        {
            if (search_key == "consignment_code") searchCellIndex = 2;
            if (search_key == "upc") searchCellIndex = 1;
            if (search_key == "vendor_code") searchCellIndex = 4;
            if (search_key == "CMPHONE") searchCellIndex = 14;
            if (search_key == "CMNAME1ST") searchCellIndex = 12;
            if (search_key == "CMNAMESUR") searchCellIndex = 13;
             
            int best_match = searchglobal.findRow(search_term, dataGridView1, searchCellIndex);
            dataGridView1.CurrentCell = this.dataGridView1[searchCellIndex, best_match];

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

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool bEnable);

        public void loadConsignments(string jump_to_consignment = null)
        {

            // Lock Window
            EnableWindow(this.Handle, false);
            loadingPanel.Left = dataGridView1.Left + (dataGridView1.Width / 2) - (loadingPanel.Width / 2);
            loadingPanel.Visible = true;
            

            // Clear DataGridView
            dataGridView1.Rows.Clear();

            // Load MySQL data
            MySqlCommand mysqlCmd = null;

            // Check options
            bool displaySold = checkBoxSold.Checked;
            bool displayUnsold = checkBoxUnsold.Checked;

            string whereQuery = "";

            string strSQL = "";
            if (listMode == "consignment")
            {

                //if (displaySold) whereQuery = " AND c.`status` = \"sold\" ";
                //if (displayUnsold) whereQuery = " AND (GROUP_CONCAT(DISTINCT status) = \"unsold\" OR GROUP_CONCAT(DISTINCT status) = \"sold,unsold\" OR GROUP_CONCAT(DISTINCT status) = \"unsold,sold\") ";
                //if (displaySold && displayUnsold)
                whereQuery = "";
                strSQL = "SELECT c.* , COUNT(*) AS items,GROUP_CONCAT(DISTINCT status) AS all_status, p.cmphone, p.CMNAME1ST, p.CMNAMESUR  FROM `CSTITEM` AS c, `PSVEMAST` AS p WHERE c.vendor_code=p.cmcucode " + whereQuery + " GROUP BY c.consignment_code";


                //if (displaySold) whereQuery = "WHERE c.vendor_code=p.cmcucode AND `consignment_code` IN (SELECT `consignment_code` FROM `CSTITEM` WHERE `status` = \"sold\")";
                //if (displayUnsold) whereQuery = "WHERE c.vendor_code=p.cmcucode AND `consignment_code` IN (SELECT `consignment_code` FROM `CSTITEM` WHERE `status` = \"unsold\")";
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
                if (displaySold) whereQuery = "WHERE c.vendor_code=p.cmcucode AND `status` = \"sold\"";
                if (displayUnsold) whereQuery = "WHERE c.vendor_code=p.cmcucode AND `status` = \"unsold\"";
                if (displaySold && displayUnsold) whereQuery = "WHERE c.vendor_code=p.cmcucode AND (`status` = \"unsold\" OR `status` = \"sold\")";
                strSQL = "SELECT c.*, p.cmphone, p.CMNAME1ST, p.CMNAMESUR FROM `CSTITEM` AS c, `PSVEMAST` AS p " + whereQuery + " ORDER BY `upc` ASC";
            }

           

            //string strSQL = "SELECT c.*, p.cmphone, p.CMNAME1ST, p.CMNAMESUR FROM `CSTITEM` AS c, `PSVEMAST` AS p " + whereQuery + " ORDER BY `" + search_key + "` ASC";

            //strSQL = "SELECT c.*,c.COUNT(*) GROUP BY c.consignment_code";

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
                    Application.DoEvents();
                    // Fetch Data on Vendor
                    query = "SELECT * FROM PSVEMAST WHERE CMCUCODE = \"" + row["vendor_code"] + "\"";
                    DataSet vendor_data = mysqlglobal.executeDataSetQuery(query, "PSVEMAST", this); // Adaptor -> vendor_data
                    DataRow vendor_row = vendor_data.Tables["PSVEMAST"].Rows[0]; // vendor_data -> vendor_row

                    // End Vendor Data Fetch



                    if (prev_consignment_code != Convert.ToString(row["consignment_code"]))
                    {
                        DataGridViewRow outputRow = new DataGridViewRow();
                        outputRow.CreateCells(dataGridView1);

                        status = row["all_status"].ToString();

                        int numeric_status = 0;
                        if(status == "sold")  numeric_status = 1;
                        if(status == "unsold")  numeric_status = 0;
                        if (status == "sold,unsold") numeric_status = 2;
                        if (status == "unsold,sold") numeric_status = 2;

                        outputRow.Cells[cg_icon.Index].Value = imageList1.Images[numeric_status];

                        if (!displayUnsold)
                        {
                            if (numeric_status == 0)
                            {
                                continue;
                            }
                        }

                        if (!displaySold)
                        {
                            if (numeric_status == 1)
                            {
                                continue;
                            }
                        }

                        outputRow.Cells[cg_consignment_code.Index].Value = row["consignment_code"];
                        outputRow.Cells[cg_vendor_code.Index].Value = row["vendor_code"];

                        outputRow.Cells[cg_vendor_first_name.Index].Value = vendor_row["CMNAME1ST"]; // Vendor First Name
                        outputRow.Cells[cg_vendor_last_name.Index].Value = vendor_row["CMNAMESUR"]; // Vendor Last Name
                        outputRow.Cells[cg_phone.Index].Value = vendor_row["CMPHONE"]; // Vendor Phone

                        outputRow.Cells[cg_total_items.Index].Value = row["items"];

                        string all_status = row["all_status"].ToString();



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

                cg_phone.Visible = true;
                cg_vendor_first_name.Visible = true;
                cg_vendor_last_name.Visible = true;

                cg_status.Visible = false;

                string status = "";
                foreach (DataRow row in item_file.Tables["CSTITEM"].Rows)
                {
                    Application.DoEvents();
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
                    //// only set when search by phone #
                    //if (search_key2 == "phone#") outputRow.Cells[cg_phone.Index].Value = mysqlglobal.executeScalarQuery("SELECT cmphone FROM PSVEMAST WHERE CMCUCODE = \"" + row["vendor_code"] + "\"", this);
                    outputRow.Cells[cg_phone.Index].Value = row["CMPHONE"];
                    outputRow.Cells[cg_vendor_first_name.Index].Value = row["CMNAME1ST"];
                    outputRow.Cells[cg_vendor_last_name.Index].Value = row["CMNAMESUR"];
                    dataGridView1.Rows.Add(outputRow);
                }
            }

            // Move Cursor to Bottom
            if (jump_to_consignment == null || jump_to_consignment == "NEW")
            {
                if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1[0, Math.Max(dataGridView1.Rows.Count - 1, 0)];
            }
            else
            {
                // Move cursor to updated consignment
                int best_match = searchglobal.findRow(jump_to_consignment, dataGridView1, 2);
                //MessageBox.Show("Best Match: " + best_match + ". Searched for '" + jump_to_consignment + "'");
                dataGridView1.CurrentCell = this.dataGridView1[2, best_match];
            }
            // Unlock Window
            EnableWindow(this.Handle, true);
            loadingPanel.Visible = false;
            Application.DoEvents();
            dataGridView1.Refresh();


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
            // Position in top left.
            this.Top = 10;
            this.Left = 10;
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            loadConsignments();
        }

        public static string listMode = "consignment";

        public bool changeMode(string newMode) {
            if(listMode == newMode) return false;

            listMode = newMode;

            if (listMode == "items")
            {
                modeDisplay.Text = "Display Item"; modeDisplay.BackColor = Color.Aqua;
                dataGridView1.MultiSelect = true;
            }
            if (listMode == "consignment")
            {
                modeDisplay.Text = "Display Consignment";
                modeDisplay.BackColor = Color.FromArgb(255, 138, 0);
                dataGridView1.MultiSelect = false;
            }

            return true;

        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            changeMode("items");
            search_key = "upc";
            label_searchkey.Text = "Search Key: By UPC";

            dataGridView1.MultiSelect = true;
            loadConsignments();
            // clear search text
            toolStripTextBox1.Text = "";
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            changeMode("consignment");
            search_key = "consignment_code";
            label_searchkey.Text = "Search Key: By Consignment Code";

            modeDisplay.Text = "Display Consignment";
            modeDisplay.BackColor = Color.FromArgb(255, 138, 0);

            dataGridView1.MultiSelect = false;
            loadConsignments();
            // clear search text
            toolStripTextBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Print Reports
            if (listMode == "consignment")
            {
                string selectedConsignment = Convert.ToString(dataGridView1.SelectedRows[0].Cells[cg_consignment_code.Index].Value);
                print_report prt = new print_report(selectedConsignment, null, null, "Consignment Agreement");
                prt.ShowDialog(this);
            }
            else
            {
                //Enumerate Selected Rows
                string selectedItems = "";
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    selectedItems += "," + Convert.ToString(dataGridView1.SelectedRows[i].Cells[cg_upc.Index].Value);
                }
                selectedItems = selectedItems.Substring(1); // Trim First ,
                print_report prt = new print_report(null, null, selectedItems, "Print Barcode Item Label(s) for Selected Item(s)");
                prt.ShowDialog(this);
            }
           
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
            Application.DoEvents();
            loadConsignments();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (toolStripTextBox1.Text != "")
                {
                    // start to search
                    toolStripButton14_Click(null, null);
                }
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            purge_consignments purgeForm = new purge_consignments();
            //purgeForm.MdiParent = this.MdiParent;
            purgeForm.ShowDialog(this);
        }

        private void toolStripButton17_Click(object sender, EventArgs e)
        {

            // Weirdly hacked Item Search Form

            if ((this.MdiParent as Form1).item_search_form == null || (this.MdiParent as Form1).item_search_form.IsDisposed == true)
            {
                (this.MdiParent as Form1).item_search_form = new item_search(null, null, null, null, null);
            }
            (this.MdiParent as Form1).item_search_form.MdiParent = this.MdiParent;
            (this.MdiParent as Form1).item_search_form.Show();
            
            
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Name != "toolStripButton18")
            {
                // Item Mode
                if (changeMode("items")) loadConsignments();
            }
            else
            {
                if (changeMode("consignment")) loadConsignments();
            }

            search_key = "vendor_code";
            label_searchkey.Text = "Search Key: By Vendor Code";


            dataGridView1.MultiSelect = false; // JH: Should this be removed?
    
            

            // Sort by search key (new)
            dataGridView1.Sort(dataGridView1.Columns[cg_vendor_code.Index], ListSortDirection.Ascending);
            if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.SelectedCells[0];

            // clear search text
            toolStripTextBox1.Text = "";
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            // start to search
            gotoList(toolStripTextBox1.Text);
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Name != "toolStripButton19")
            {
                // Item Mode
                if (changeMode("items")) loadConsignments();
            }
            else
            {
                if (changeMode("consignment")) loadConsignments();
            }
            
            search_key = "CMPHONE";
            label_searchkey.Text = "Search Key: By Phone #";
            dataGridView1.MultiSelect = false; // JH: Should this be removed?


            // Sort by search key (new)
            dataGridView1.Sort(dataGridView1.Columns[cg_phone.Index], ListSortDirection.Ascending);
            if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.SelectedCells[0];

            // clear search text
            toolStripTextBox1.Text = "";
        }

        private void toolStripButton20_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Name != "toolStripButton20")
            {
                // Item Mode
                if (changeMode("items")) loadConsignments();
            }
            else
            {
                if (changeMode("consignment")) loadConsignments();
            }
            search_key = "CMNAMESUR";
            label_searchkey.Text = "Search Key: By Last/First Name";
            dataGridView1.MultiSelect = false; // JH: Should this be removed?

            // Sort by search key (new)
            dataGridView1.Sort(dataGridView1.Columns[cg_vendor_last_name.Index], ListSortDirection.Ascending);
            if (dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.SelectedCells[0];

            // clear search text
            toolStripTextBox1.Text = "";
        }

        private void toolStripButton21_Click(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Name != "toolStripButton21")
            {
                // Item Mode
                if (changeMode("items")) loadConsignments();
            }
            else
            {
                if (changeMode("consignment")) loadConsignments();
            }
            search_key = "CMNAME1ST";
            label_searchkey.Text = "Search Key: By First/Last Name";
            dataGridView1.MultiSelect = false; // JH: Should this be removed?

            // Sort by search key (new)
            dataGridView1.Sort(dataGridView1.Columns[cg_vendor_first_name.Index], ListSortDirection.Ascending);
            if(dataGridView1.Rows.Count > 0) dataGridView1.CurrentCell = dataGridView1.SelectedCells[0];

            // clear search text
            toolStripTextBox1.Text = "";
        }


    }
}

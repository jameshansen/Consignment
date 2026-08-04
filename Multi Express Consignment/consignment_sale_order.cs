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

using CrystalDecisions;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Multi_Express_Consignment
{
    public partial class consignment_sale_order : Form
    {

        public string order_number;
        public string customer_code;

        public string selected_upc; // If in Item Mode

        public string mode;

        public bool customerModified = false;
        public bool read_only = false;

        public DateTime old_date;

        public string order_status = null;

        public string getorder_number()
        {
            return order_number;
        }

        public consignment_sale_order(string orderNumber, string vendorCode, string upc)
        {
            InitializeComponent();

            // Bugfix (2026): Keep the row buttons greyed out until they have a row to work on
            dataGridView1.SelectionChanged += rowSelectionChanged;
            dataGridView2.SelectionChanged += rowSelectionChanged;
            rowSelectionChanged(null, null);

            order_number = orderNumber;
            mode = "edit";
            if (vendorCode != null)
            {
                customer_code = vendorCode;
                mode = "new";
            }
                
            selected_upc = upc;

        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public bool loaded = false;

        public void setOrderStatus(string status)
        {
            order_status = status;
            statusButton.Text = status;

            statusButton.BackColor = Color.White;
            statusButton.ForeColor = Color.Black;

            status = UppercaseFirst(status);

            if (status == "Open")
            {
                statusButton.BackColor = Color.White;
            }
            if (status == "In Progress")
            {
                statusButton.BackColor = Color.Yellow;
            }
            if (status == "Work Completed")
            {
                statusButton.BackColor = Color.Yellow;
            }
            if (status == "Pending")
            {
                statusButton.BackColor = Color.Red;
                statusButton.ForeColor = Color.White; // Readability
            }
            if (status == "Invoiced")
            {
                statusButton.BackColor = Color.Aqua;
            }
            if (status == "Cancelled")
            {
                statusButton.BackColor = Color.Lime;
                statusButton.ForeColor = Color.Red; // Readability
            }


        }

        private void consignment_sale_order_Shown(object sender, EventArgs e)
        {
            input_item.Focus();
            if (mode == "new")
            {
                order_number_textbox.Text = "NEW";
                customer_code_textbox.Text = customer_code;
                setOrderStatus(UppercaseFirst("Open"));
                // AUTO-NONREADONLY TEXT BOXES
            }
            // EDIT MODE
            if (mode == "edit")
            {
                order_number_textbox.Text = order_number;

                // Load up Order Header Row
                string strSQL = "SELECT * FROM `CSTORDER` WHERE `order_number` = \"" + order_number + "\"";
                DataRow order_row = mysqlglobal.executeDataSetQuery(strSQL, "CSTORDER", this).Tables["CSTORDER"].Rows[0];

                customer_code = Convert.ToString(order_row["customer_code"]);
                order_status = Convert.ToString(order_row["order_status"]);

                old_date = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToInt64(order_row["date_order"])); // New 2011-08-08
                date_order.Value = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToInt64(order_row["date_order"]));

                setOrderStatus(order_status); // Blank Status Button Fix

                read_only = false;
                if (order_status == "Invoice")
                {
                    read_only = true;
                }

                // Load up Consignment
                strSQL = "SELECT * FROM `CSTITEM` WHERE `order_number` = \"" + order_number + "\" ORDER BY `upc`";
                DataSet consignment_file = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this);

                int jumpToIndex = 0;

                foreach (DataRow row in consignment_file.Tables["CSTITEM"].Rows)
                {
                    DataGridViewRow outputRow = new DataGridViewRow();
                    outputRow.CreateCells(dataGridView1);

                    outputRow.Cells[ig_upc.Index].Value = row["upc"];
                    outputRow.Cells[ig_description.Index].Value = row["description"];
                    outputRow.Cells[ig_date_sold.Index].Value = row["date_sold"];
                    outputRow.Cells[ig_display_date_sold.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_sold"])));
                    outputRow.Cells[ig_price_sale.Index].Value = cg.price(row["price_sale"]);

                    dataGridView1.Rows.Add(outputRow);

                    if (selected_upc != null && row["upc"].ToString() == selected_upc)
                    {
                        jumpToIndex = dataGridView1.RowCount - 1;
                    }

                }

                if (selected_upc != null)
                {
                    dataGridView1.CurrentCell = this.dataGridView1[0, jumpToIndex];
                }
                

                customer_code_textbox.Text = customer_code;
                setOrderStatus(order_status);

                // LOAD UP PAYMENT FILE
                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `order_number` = \"" + order_number + "\" AND `deleted` = false ORDER BY `date` DESC";
                DataSet payment_file = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this);
                foreach (DataRow row in payment_file.Tables["CSTPAYMENT"].Rows)
                {
                    DataGridViewRow outputRow = new DataGridViewRow();
                    outputRow.CreateCells(dataGridView2);

                    outputRow.Cells[pg_id.Index].Value = row["id"];
                    outputRow.Cells[pg_type.Index].Value = row["type"];
                    outputRow.Cells[pg_desc.Index].Value = row["description"];
                    outputRow.Cells[pg_cn.Index].Value = row["cn"];
                    outputRow.Cells[pg_expiry.Index].Value = row["expiry"];
                    string payment_date = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date"])));
                    outputRow.Cells[pg_display_date.Index].Value = payment_date;
                    outputRow.Cells[pg_date.Index].Value = row["date"];
                    outputRow.Cells[pg_amount.Index].Value = cg.price(row["amount"]);
                    outputRow.Cells[pg_vendor_code.Index].Value = row["customer_code"];
                    outputRow.Cells[pg_vendor_name.Index].Value = row["vendor_name"];

                    dataGridView2.Rows.Add(outputRow);
                }
            }

            // LOAD UP CUSTOMER FILE
            string query = "SELECT * FROM SFCUMAST WHERE CMCUCODE = \"" + customer_code + "\"";

            DataSet customer_data = mysqlglobal.executeDataSetQuery(query, "SFCUMAST", this);

            DataRow customer_row = customer_data.Tables["SFCUMAST"].Rows[0]; // customer_data -> customer_row

            input_CMADD1.Text = customer_row["CMADD1"].ToString();
            input_CMADD2.Text = customer_row["CMADD2"].ToString();
            input_CMCITY.Text = customer_row["CMCITY"].ToString();
            input_CMCOUNTRY.Text = customer_row["CMCOUNTRY"].ToString();
            input_CMCUNAME.Text = customer_row["CMCUNAME"].ToString();
            input_CMFAX1.Text = customer_row["CMFAX1"].ToString();
            input_CMNAME1ST.Text = customer_row["CMNAME1ST"].ToString();
            input_CMNAMESUR.Text = customer_row["CMNAMESUR"].ToString();
            input_CMPHONE.Text = customer_row["CMPHONE"].ToString();
            input_CMSTATE.Text = customer_row["CMSTATE"].ToString();

            calcTotals();

            if (read_only == true)
            {
                label13.Visible = false;
                input_item.Visible = false;
                button2.Visible = false;
                button4.Visible = false;
                button13.Visible = false; // Bugfix (2026): Editing an invoiced item was allowed but silently discarded on save
                groupBox4.Text = "View Invoiced Items";
                dataGridView1.Top = 19;
                dataGridView1.Height = 181;


                /*
                // Button to Close
                button8.Left = 499;
                button7.Visible = false;
                button8.Text = "Close";

                // Address Changes
                button5.Visible = false;
                groupBox2.Height = 184;
                 */
            }

            loaded = true;

        }

        private static add_item_to_cpo additem = null;

        private void button4_Click(object sender, EventArgs e)
        {
            if (additem == null || additem.IsDisposed == true)
            {
             //   additem = new add_item_to_cpo(this, -1);
            }
            additem.ShowDialog(this);
        }

        public string nextConsignmentCode()
        {
            string output = "";
            MySqlCommand mysqlCmd = new MySqlCommand("SELECT `order_number` FROM `CSTORDER` ORDER BY `order_number` DESC", mysqlglobal.mysqlCon);
            try
            {
                string currentCode = mysqlCmd.ExecuteScalar().ToString();
                output = Convert.ToString(Convert.ToInt32(currentCode) + 1);
            }
            catch
            {
                /* Read Start Code */
                output = iniglobal.ini.IniReadValue("startcodes", "order");
            }
            return output;
        }

        public void saveItems()
        {
            calcTotals();

            string status = button1.Text.ToLower(); // Order or invoice
            // order_status is global, Open, In Progress etc.
            // order_number is global
            // customer_code
            string customer_first_name = mysqlglobal.escapeString(input_CMNAME1ST.Text);
            string customer_last_name = mysqlglobal.escapeString(input_CMNAMESUR.Text);
            string date_order_str = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(date_order.Value)); // Now the order is placed
            string items = Convert.ToString(dataGridView1.DisplayedRowCount(true)); // Visible Row Count! Deleted items are invisible
            string total = output_totalowed.Text; // Must do CalcTotals beforehand

            if (order_status == "Invoiced")
            {
                status = "invoice";
            }


            // If New
            string ohquery = "";

            if (mode != "edit")
            {
                /* Set next ORDER to lowest avaliable */
                string resetUPC = "ALTER TABLE `CSTORDER` AUTO_INCREMENT = " + iniglobal.ini.IniReadValue("startcodes", "order");
                mysqlglobal.executeNonQuery(resetUPC, this);

                // Insert Header into Orders DB

                ohquery =
                    @"INSERT INTO `CSTORDER` (
                    `status`,
                    `order_status`,
                    `customer_code`,
                    `customer_first_name`,
                    `customer_last_name`,
                    `date_order`,
                    `items`,
                    `total`
                    ) VALUES (
                    """ + status + @""",
                    """ + order_status + @""",
                    """ + customer_code + @""",
                    """ + customer_first_name + @""",
                    """ + customer_last_name + @""",
                    """ + date_order_str + @""",
                    """ + items + @""",
                    """ + total + @""")";
            }
            else
            {
                // Update Header with status, order_status, items, and total
                ohquery =
                    @"UPDATE `CSTORDER` SET
                    `status` = """ + status + @""",
                    `order_status` = """ + order_status + @""",
                    `customer_first_name` = """ + customer_first_name + @""",
                    `customer_last_name` = """ + customer_last_name + @""",
                    `date_order` = """ + date_order_str + @""",
                    `items` = """ + items + @""",
                    `total` = """ + total + @""" WHERE `order_number` = '" + order_number + "'";                
            }

            /* Run Query */
            mysqlglobal.executeNonQuery(ohquery, this);

            if (mode != "edit")
            {
                /* Get Order Number */
                MySqlCommand mysqlCmd = new MySqlCommand("SELECT `order_number` FROM `CSTORDER` ORDER BY `order_number` DESC", mysqlglobal.mysqlCon);
                order_number = Convert.ToString(mysqlCmd.ExecuteScalar()); // Get order number
                mysqlCmd.Dispose();
            }

            int records = dataGridView1.RowCount;
            if (read_only == false)
            {
                // Update Items with Sold Date and Order Number
                // JH: New 2 passes, so that deleted are removed, then new are saved.
                for (int i = records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
                {
                    string upc = Convert.ToString(dataGridView1.Rows[i].Cells[ig_upc.Index].Value);
                    string date_sold = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_sold.Index].Value);
                    string price_sale = Convert.ToString(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value);
                    string item_status = Convert.ToString(dataGridView1.Rows[i].Cells[ig_status.Index].Value);

                    string strSQL;

                    
                    if (item_status == "Deleted") 
                    {
                        strSQL =
                               @"UPDATE `CSTITEM` SET
                    `order_number` = '0',
                    `customer_code` = '',
                    `price_sale` = '',
                    `date_sold` = '',
                    `status` = 'unsold' WHERE `upc` = '" + upc + "'";
                     mysqlglobal.executeNonQuery(strSQL, this);
                    }
                    
                }

                for (int i = records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
                {
                    string upc = Convert.ToString(dataGridView1.Rows[i].Cells[ig_upc.Index].Value);
                    string date_sold = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_sold.Index].Value);
                    string price_sale = Convert.ToString(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value);
                    string item_status = Convert.ToString(dataGridView1.Rows[i].Cells[ig_status.Index].Value);

                    string strSQL;


                    if (item_status != "Deleted")
                    {
                        strSQL =
                               @"UPDATE `CSTITEM` SET
                    `order_number` = '" + order_number + @"',
                    `customer_code` = '" + customer_code + @"',
                    `price_sale` = '" + price_sale + @"',
                    `date_sold` = '" + date_sold + @"',
                    `status` = 'sold' WHERE `upc` = '" + upc + "'";
                        mysqlglobal.executeNonQuery(strSQL, this);
                    }

                }

            }
            // If Vendor has been Modified
            if (customerModified == true)
            {
                string CMADD1 = mysqlglobal.escapeString(input_CMADD1.Text);
                string CMADD2 = mysqlglobal.escapeString(input_CMADD2.Text);
                string CMCITY = mysqlglobal.escapeString(input_CMCITY.Text);
                string CMCOUNTRY = mysqlglobal.escapeString(input_CMCOUNTRY.Text);
                string CMCUNAME = mysqlglobal.escapeString(input_CMCUNAME.Text);
                string CMFAX1 = mysqlglobal.escapeString(input_CMFAX1.Text);
                string CMNAME1ST = mysqlglobal.escapeString(input_CMNAME1ST.Text);
                string CMNAMESUR = mysqlglobal.escapeString(input_CMNAMESUR.Text);
                string CMPHONE = mysqlglobal.escapeString(input_CMPHONE.Text);
                string CMSTATE = mysqlglobal.escapeString(input_CMSTATE.Text);

                string query = 
                @"UPDATE SFCUMAST SET
                `CMADD1` = """ + CMADD1 + @""",
                `CMADD2` = """ + CMADD2 + @""",
                `CMCITY` = """ + CMCITY + @""",
                `CMCOUNTRY` = """ + CMCOUNTRY + @""",
                `CMCUNAME` = """ + CMCUNAME + @""",
                `CMFAX1` = """ + CMFAX1 + @""",
                `CMNAME1ST` = """ + CMNAME1ST + @""",
                `CMNAMESUR` = """ + CMNAMESUR + @""",
                `CMPHONE` = """ + CMPHONE + @""",
                `CMSTATE` = """ + CMSTATE + @"""
                WHERE CMCUCODE = """ + customer_code + @"""";

                mysqlglobal.executeNonQuery(query, this);
            }


            // Insert new Payments into DB
            records = dataGridView2.RowCount;
            for (int i = records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                // order_number = global
                // customer_code = global

                string payment_id = Convert.ToString(dataGridView2.Rows[i].Cells[pg_id.Index].Value);
                string payment_code = Convert.ToString(dataGridView2.Rows[i].Cells[pg_type.Index].Value);
                string payment_description = Convert.ToString(dataGridView2.Rows[i].Cells[pg_desc.Index].Value);
                string payment_reference = Convert.ToString(dataGridView2.Rows[i].Cells[pg_cn.Index].Value);
                string payment_expiry = Convert.ToString(dataGridView2.Rows[i].Cells[pg_expiry.Index].Value);
                string payment_date = Convert.ToString(dataGridView2.Rows[i].Cells[pg_date.Index].Value);
                string payment_amount = Convert.ToString(dataGridView2.Rows[i].Cells[pg_amount.Index].Value);
                string payment_vendor_name = Convert.ToString(dataGridView2.Rows[i].Cells[pg_vendor_name.Index].Value);

                string payment_status = Convert.ToString(dataGridView2.Rows[i].Cells[pg_status.Index].Value);
                string strSQL = "";
                if (payment_status != "Deleted")
                {

                    if (payment_id == "")
                    {
                        // Insert into Database
                            strSQL =
                            @"INSERT INTO `CSTPAYMENT` (
                            `order_number`,
                            `type`,
                            `description`,
                            `cn`,
                            `expiry`,
                            `date`,
                            `amount`,
                            `customer_code`,
                            `customer_name`
                            ) VALUES (
                            '" + order_number + @"',
                            '" + payment_code + @"',
                            '" + payment_description + @"',
                            '" + payment_reference + @"',
                            '" + payment_expiry + @"',
                            " + payment_date + @",
                            '" + payment_amount + @"',
                            '" + customer_code + @"',
                            '" + payment_vendor_name + @"');";

                    }
                    else
                    {
                        strSQL =
                        @"UPDATE `CSTPAYMENT` SET
                    `order_number` = '" + order_number + @"',
                    `type` = '" + payment_code + @"',
                    `description` = '" + payment_description + @"',
                    `cn` = '" + payment_reference + @"',
                    `expiry` = '" + payment_expiry + @"',
                    `date` = '" + payment_date + @"',
                    `amount` = '" + payment_amount + @"',
                    `customer_code` = '" + customer_code + @"',
                    `customer_name` = '" + payment_vendor_name + @"' WHERE `id` = '" + payment_id + "'";
                    }
                }
                else
                {
                    // Remove row
                    strSQL =
                        @"DELETE FROM `CSTPAYMENT` WHERE `id` = '" + payment_id + "'";
                }
                mysqlglobal.executeNonQuery(strSQL, this);
            }

        }

        public void addPayment(string payment_code, string payment_description, string payment_reference, string payment_expiry, string payment_customer_code, string payment_vendor_name, string payment_amount)
        {
            DataGridViewRow outputRow = new DataGridViewRow();
            outputRow.CreateCells(dataGridView2);
            outputRow.Cells[pg_type.Index].Value = payment_code;
            outputRow.Cells[pg_desc.Index].Value = payment_description;
            outputRow.Cells[pg_cn.Index].Value = payment_reference;
            outputRow.Cells[pg_expiry.Index].Value = payment_expiry;
            if (date_order.Enabled && dataGridView2.DisplayedRowCount(true) == 0) // ONLY FIRST OR SINGLE PAYMENT CAN BE BACKDATED
            {
                if (MessageBox.Show("Would you like to Backdate this Payment to the Specified Order Date?", "Backdate Payment", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    outputRow.Cells[pg_display_date.Index].Value = mysqlglobal.formatDate(date_order.Value);
                    // Forgot to add UNIXTIME DATE!
                    outputRow.Cells[pg_date.Index].Value = mysqlglobal.ConvertToUnixTimestamp(date_order.Value).ToString();

                }
                else
                {
                    outputRow.Cells[pg_display_date.Index].Value = mysqlglobal.formatDate(DateTime.Now);
                    outputRow.Cells[pg_date.Index].Value = mysqlglobal.ConvertToUnixTimestamp(DateTime.Now).ToString();
                }
            }
            else
            {
                outputRow.Cells[pg_display_date.Index].Value = mysqlglobal.formatDate(DateTime.Now);
                outputRow.Cells[pg_date.Index].Value = mysqlglobal.ConvertToUnixTimestamp(DateTime.Now).ToString();
            }

            if (dataGridView2.DisplayedRowCount(true) > 0)
            {                
                // New, Warn about Adding Payment IF Date is different
                if (MessageBox.Show("Adding an additional payment will change the date of this order, is this OK?", "Backdate Payment", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return; // Don't add payment
                }

                old_date = DateTime.Now;
                date_order.Value = old_date;

            }

            //outputRow.Cells[pg_date.Index].Value = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(DateTime.Now));
            outputRow.Cells[pg_amount.Index].Value = cg.price(payment_amount);

            outputRow.Cells[pg_vendor_code.Index].Value = payment_customer_code;
            outputRow.Cells[pg_vendor_name.Index].Value = payment_vendor_name;
            dataGridView2.Rows.Add(outputRow);

            payment_entry_form.Dispose();

            // ReCalc Totals
            calcTotals();
        }


        private void consignment_sale_order_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            customerModified = true;
            // Activate all the Fields
            input_CMADD1.ReadOnly = false;
            input_CMADD2.ReadOnly = false;
            input_CMCITY.ReadOnly = false;
            input_CMCOUNTRY.ReadOnly = false;
            input_CMCUNAME.ReadOnly = false;
            input_CMFAX1.ReadOnly = false;
            input_CMNAME1ST.ReadOnly = false;
            input_CMNAMESUR.ReadOnly = false;
            input_CMPHONE.ReadOnly = false;
            input_CMSTATE.ReadOnly = false;
        }

        private void calcTotals()
        {
            decimal share_out = 0;

            decimal totalBeforeTax = 0;
            decimal totalOwed = 0;
            decimal totalPaid = 0;
            decimal totalOutstanding = 0;


            // Cycle through Records
            int item_records = dataGridView1.RowCount;
            for (int i = item_records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                /* Fixed Totalling Problem if Item Deleted 2011-07-08 */
                if (dataGridView1.Rows[i].Visible == true)
                {
                    decimal sold_row = Convert.ToDecimal(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value);
                    totalBeforeTax = totalBeforeTax + sold_row;
                }
            }

            output_totalbeforetax.Text = cg.price(totalBeforeTax);

            totalOwed = totalBeforeTax * (decimal)1.12; // + 12% HST

            totalOwed = Math.Round(totalOwed, 2, MidpointRounding.AwayFromZero); // Round up

            output_totalowed.Text   = cg.price(totalOwed);

            int payment_records = dataGridView2.RowCount;
            for (int i = payment_records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                if (Convert.ToString(dataGridView2.Rows[i].Cells[pg_status.Index].Value) != "Deleted")
                {
                    totalPaid = totalPaid + Convert.ToDecimal(dataGridView2.Rows[i].Cells[pg_amount.Index].Value);
                }
            }

            output_totalpaid.Text = cg.price(totalPaid);

            output_totaloutstanding.Text = cg.price(totalOwed - totalPaid);


        }

        public payment_entry payment_entry_form = null;

        private void button6_Click(object sender, EventArgs e)
        {
            /* JH: Limited to ONE payment per order.
             * This is so the Daily Sales Report has no Problem.
             * 
             * If this is removed, the logic in the Daily Sales Report should be fixed, otherwise multiple dates will mess up daily/monthly total.
             * 
             * TODO
             */

            

            // Open enter Payment window
            string vendor_name = input_CMNAME1ST.Text + " " + input_CMNAMESUR.Text;
            string totalcost = output_totaloutstanding.Text;

            if (payment_entry_form == null)
            {
                payment_entry_form = new payment_entry(this,customer_code,vendor_name,totalcost);
            }
            else
            {
                payment_entry_form.Dispose();
                payment_entry_form = new payment_entry(this, customer_code, vendor_name, totalcost);
            }
            payment_entry_form.ShowDialog(this);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < dataGridView1.Columns.Count; i++) {
                dataGridView1.Columns[i].Visible = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            if (Convert.ToDecimal(output_totaloutstanding.Text) > 0)
            {
                if (MessageBox.Show("There is $" + output_totaloutstanding.Text + " outstanding for this order. Do you wish to return to the order to complete Payment?","Payment Incomplete",MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
                    return;
                }

            }
            

            if (Convert.ToDecimal(output_totaloutstanding.Text) <= 0 && Convert.ToDecimal(output_totalowed.Text) > 0 && statusButton.Text.ToLower() != "invoiced")
            {
                if (MessageBox.Show("Payment has been received, mark this order as invoice?", "Mark as Invoice", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    setOrderStatus("Invoiced");
                }
            }
            

            saveItems();
            //MessageBox.Show("Order Number:" + order_number);

            if (MessageBox.Show("Do you wish to Print Sales Order or Receipt Now?", "Print Order or Receipt", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //printRptConsignment();
                print_report prt = null;
                if (order_status == "Invoiced")
                {

                    prt = new print_report(null, order_number, null, "Sales Receipt");
                }
                else
                {
                    prt = new print_report(null, order_number, null, "Sales Order");
                }
                prt.ShowDialog(this);
            }
            
            // Update Sale Desktop
            this.Close();

            foreach (Form form_search in Application.OpenForms)
            {
                if (form_search.Name == "consignment_sale_desktop")
                {
                    (form_search as consignment_sale_desktop).loadOrders();
                }
            }

            this.Dispose();
        }
        

        private void button12_Click(object sender, EventArgs e)
        {
            // Hide Row
            dataGridView1.SelectedRows[0].Visible = false;

            // Mark Row as Deleted
            dataGridView1.SelectedRows[0].Cells[ig_status.Index].Value = "Deleted";

            calcTotals();
        }

        private void button11_Click(object sender, EventArgs e)
        {

            //if(additem != null || additem.IsDisposed == false) additem.Dispose();
            int editRow = dataGridView1.SelectedRows[0].Index;
            if (Convert.ToString(dataGridView1.Rows[editRow].Cells[ig_status.Index].Value) == "Unsold")
            {
                //additem = new add_item_to_cpo(this, editRow);
                additem.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Sorry, but you cannot edit sold items.", "Cannot Edit");
            }
        }

        // Bugfix (2026): A row button can only work on a row that is selected and not already deleted (hidden)
        private void rowSelectionChanged(object sender, EventArgs e)
        {
            bool itemSelected = dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Visible;
            bool paymentSelected = dataGridView2.SelectedRows.Count > 0 && dataGridView2.SelectedRows[0].Visible;

            button13.Enabled = itemSelected; // Edit Item
            button4.Enabled = itemSelected; // Remove Item
            button9.Enabled = paymentSelected; // Delete Payment
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Hide Row
            dataGridView2.SelectedRows[0].Visible = false;

            // Mark Row as Deleted
            dataGridView2.SelectedRows[0].Cells[pg_status.Index].Value = "Deleted";

            // If Status is Invoiced, Change it to Open
            if (statusButton.Text == "Invoiced")
            {
                setOrderStatus("Open");
            }

            // Calc Totals
            calcTotals();

            rowSelectionChanged(null, null); // Bugfix (2026): Row is gone, re-check the buttons
        }

        private void button3_Click(object sender, EventArgs e)
        {
            change_status_cpo statusWin = new change_status_cpo(this,statusButton.Text,output_totalowed.Text,output_totalpaid.Text);
            statusWin.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        public item_search item_search_form = null;
        public add_item_to_order add_item_to_order_form = null;

        public void addItem(string upc)
        {
            bool notFound = true;

            /* Look for item in current DataGridView (2011-07-08) */
            
            for (int a = 0; a < dataGridView1.Rows.Count; a++)
            {
                if (dataGridView1.Rows[a].Visible == true)/* Flail. Should SKIP deleted (hidden) items (2011-08-05) */
                {
                    string upcC = dataGridView1.Rows[a].Cells[ig_upc.Index].Value.ToString();
                    if (upcC == upc)
                    {
                        // A1
                        MessageBox.Show("This item has already been added to this order.", "Item Already Added");
                        return;
                    }
                }    

            }

            /* Look for exact Match */
            string strSQL = "SELECT * FROM `CSTITEM` WHERE `upc` = \"" + upc + "\"";
            DataSet results = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this);
            if (results.Tables["CSTITEM"].Rows.Count > 0)
            {
                notFound = false;
                DataRow itemRow = results.Tables["CSTITEM"].Rows[0];

                /* Check if Item Already Sold */
                if (itemRow["status"].ToString() == "sold")
                {
                    // Check if it's THIS order. If it got this far, it must have been deleted (see A1)
                    if (itemRow["order_number"].ToString() != order_number)
                    {
                        // It's a different order, so warn the user.
                        MessageBox.Show("This item has already been sold in Order #" + itemRow["order_number"].ToString(), "Item Already Sold");
                        return;
                    }
                }

                /* Prompt for Price */
                string formattedExpiry = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(itemRow["date_expiry"].ToString())));
                add_item_to_order_form = new add_item_to_order(this, itemRow["upc"].ToString(), itemRow["description"].ToString(), itemRow["price_suggested"].ToString(), itemRow["price_minimum"].ToString(), formattedExpiry, itemRow["consignment_code"].ToString());

                add_item_to_order_form.ShowDialog(this);

                if (add_item_to_order_form.DialogResult == DialogResult.OK)
                {
                    string final_price = add_item_to_order_form.final_price.Trim();

                    /* Add Item Details to DataGridView */
                    
                    DataGridViewRow outputRow = new DataGridViewRow();
                    outputRow.CreateCells(dataGridView1);

                    outputRow.Cells[ig_upc.Index].Value = itemRow["upc"];
                    outputRow.Cells[ig_description.Index].Value = itemRow["description"];

                    outputRow.Cells[ig_price_sale.Index].Value = final_price;

                    outputRow.Cells[ig_date_sold.Index].Value = mysqlglobal.ConvertToUnixTimestamp(DateTime.Now);
                    outputRow.Cells[ig_display_date_sold.Index].Value = mysqlglobal.formatDate(DateTime.Now);
                    
                    dataGridView1.Rows.Add(outputRow);
                    calcTotals();

                    input_item.Text = "";
                    input_item.Focus();
                }
            }

            /* If Not Found, Open Search */
            if (notFound)
            {
                /* Compile Added Items */
                string added_items = "";
                int item_count = dataGridView1.Rows.Count;
                for (int i = 0; i < item_count; i++)
                {
                    if (dataGridView1.Rows[i].Visible)
                    {
                        added_items += "," + Convert.ToString(dataGridView1.Rows[i].Cells[ig_upc.Index].Value);
                    }
                }
                
                if (added_items.Length > 0) added_items = added_items.Substring(1); // Remove first comma.

                item_search_form = new item_search("UPC", upc, this, "consignment_sale_order", added_items);
                item_search_form.ShowDialog(this);
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            addItem(input_item.Text);
        }

        private void input_item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                addItem(input_item.Text);                
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // Hide Row
            dataGridView1.SelectedRows[0].Visible = false;

            // Mark Row as Deleted
            dataGridView1.SelectedRows[0].Cells[ig_status.Index].Value = "Deleted";

            // Recalc totals!
            calcTotals();

            rowSelectionChanged(null, null); // Bugfix (2026): Row is gone, re-check the buttons
        }


        private void button11_Click_1(object sender, EventArgs e)
        {
            date_order.Enabled = true;
        }

        

        private void date_order_CloseUp(object sender, EventArgs e)
        {
            // Will Update Order Header On Save

            if (date_order.Value != old_date)
            {
                // 2011-08-19 If more than two payments, force following payment dates to change.
                if (dataGridView2.DisplayedRowCount(true) > 1)
                {
                    if (MessageBox.Show(this, "Changing the Order Date will Update all Payment Dates Except the First Payment to Selected Date, is this OK?", "Update all Items?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        return;
                    }

                    /* Update Dates */
                    for (int a = 1; a < dataGridView2.Rows.Count; a++)
                    {
                        if (dataGridView2.Rows[a].Visible == false) continue; // Skip
                        dataGridView2.Rows[a].Cells[pg_date.Index].Value = mysqlglobal.ConvertToUnixTimestamp(date_order.Value);
                        dataGridView2.Rows[a].Cells[pg_display_date.Index].Value = mysqlglobal.formatDate(date_order.Value);
                    }
                }

                
                old_date = date_order.Value;

                // Would you like to update all items with the new date?
                if (MessageBox.Show(this, "Would you like to update all items with the new date?", "Update all Items?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    update_items_with_date(date_order.Value);
                }


                
                
            }
        }

        private void update_items_with_date(DateTime date)
        {
            int item_count = dataGridView1.Rows.Count;
            for (int i = 0; i < item_count; i++)
            {
                dataGridView1.Rows[i].Cells[ig_date_sold.Index].Value = mysqlglobal.ConvertToUnixTimestamp(date);
                dataGridView1.Rows[i].Cells[ig_display_date_sold.Index].Value = mysqlglobal.formatDate(date);

            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            update_items_with_date(date_order.Value);
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            // Edit Item. New Function 2011-08-05
                DataGridViewCellCollection itemRowD = dataGridView1.SelectedRows[0].Cells; 
                /* Prompt for Price */
                string strSQL = "SELECT * FROM `CSTITEM` WHERE `upc` = \"" + itemRowD[ig_upc.Index].Value.ToString() + "\"";
                //MessageBox.Show(strSQL);
                DataSet results = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this);
                DataRow itemRow = results.Tables["CSTITEM"].Rows[0];    
                string formattedExpiry = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(itemRow["date_expiry"].ToString())));

                add_item_to_order_form = new add_item_to_order(this, itemRowD[ig_upc.Index].Value.ToString(), itemRowD[ig_description.Index].Value.ToString(), itemRow["price_suggested"].ToString(), itemRow["price_minimum"].ToString(), formattedExpiry, itemRow["consignment_code"].ToString(), itemRowD[ig_price_sale.Index].Value.ToString());

                add_item_to_order_form.ShowDialog(this);

                if (add_item_to_order_form.DialogResult == DialogResult.OK)
                {
                    string final_price = add_item_to_order_form.final_price.Trim();
                    dataGridView1.SelectedRows[0].Cells[ig_price_sale.Index].Value = final_price;
                    calcTotals();
                }

        }

    }
}

    


    


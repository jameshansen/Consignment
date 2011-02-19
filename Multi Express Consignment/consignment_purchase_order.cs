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
    public partial class consignment_purchase_order : Form
    {

        public static string consignment_code;
        public static string vendor_code;

        public static string selected_upc; // If in Item Mode

        public static string mode;

        public static bool vendorModified = false;

        public static string consignment_status = null;

        public string getconsignment_code()
        {
            return consignment_code;
        }

        public consignment_purchase_order(string consignmentCode, string vendorCode, string upc)
        {
            InitializeComponent();
          

            consignment_code = consignmentCode;
            mode = "edit";
            if (vendorCode != null)
            {
                vendor_code = vendorCode;
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

        public void setConsignmentStatus(string status)
        {
            consignment_status = status;
            statusButton.Text = status;

            statusButton.BackColor = Color.White;
            statusButton.ForeColor = Color.Black;
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

        private void consignment_purchase_order_Shown(object sender, EventArgs e)
        {
            if (mode == "new")
            {
                consignment_code_textbox.Text = "NEW";
                vendor_code_textbox.Text = vendor_code;
                // AUTO-NONREADONLY TEXT BOXES
            }
            // EDIT MODE
            if (mode == "edit")
            {
                consignment_code_textbox.Text = consignment_code;

                // Load up Consignment
                MySqlCommand mysqlCmd = null;
                MySqlDataReader mysqlReader;

                string strSQL = "SELECT * FROM `CSTITEM` WHERE `consignment_code` = \"" + consignment_code + "\" ORDER BY `upc`";
                DataSet consignment_file = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this);

                int jumpToIndex = 0;

                foreach (DataRow row in consignment_file.Tables["CSTITEM"].Rows)
                {
                    DataGridViewRow outputRow = new DataGridViewRow();
                    outputRow.CreateCells(dataGridView1);

                    outputRow.Cells[ig_status.Index].Value = UppercaseFirst(row["status"].ToString());
                    outputRow.Cells[ig_upc.Index].Value = row["upc"];
                    outputRow.Cells[ig_description.Index].Value = row["description"];

                    outputRow.Cells[ig_price_minimum.Index].Value = cg.price(row["price_minimum"]);
                    outputRow.Cells[ig_price_suggested.Index].Value = cg.price(row["price_suggested"]);
                    outputRow.Cells[ig_price_sale.Index].Value = cg.price(row["price_sale"]);
                    outputRow.Cells[ig_share.Index].Value = cg.price(row["share"]);
                    if (row["share_type"].ToString() == "value") outputRow.Cells[ig_share_type.Index].Value = "$";
                    if (row["share_type"].ToString() == "percentage") outputRow.Cells[ig_share_type.Index].Value = "%";
                    outputRow.Cells[ig_date_received.Index].Value = row["date_received"];
                    outputRow.Cells[ig_date_expiry.Index].Value = row["date_expiry"];
                    outputRow.Cells[ig_display_date_expiry.Index].Value = mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(row["date_expiry"])));

                    outputRow.Cells[ig_desc_brand.Index].Value = row["desc_brand"];
                    outputRow.Cells[ig_desc_gender.Index].Value = row["desc_gender"];
                    outputRow.Cells[ig_desc_garment.Index].Value = row["desc_garment"];
                    outputRow.Cells[ig_desc_material.Index].Value = row["desc_material"];
                    outputRow.Cells[ig_desc_colour.Index].Value = row["desc_colour"];
                    outputRow.Cells[ig_desc_size.Index].Value = row["desc_size"];

                    vendor_code = Convert.ToString(row["vendor_code"]);
                    consignment_status = Convert.ToString(row["consignment_status"]);

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
                

                vendor_code_textbox.Text = vendor_code;
                setConsignmentStatus(consignment_status);

                // LOAD UP PAYMENT FILE
                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `consignment_code` = \"" + consignment_code + "\" AND `deleted` = false ORDER BY `date` DESC";
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
                    outputRow.Cells[pg_vendor_code.Index].Value = row["vendor_code"];
                    outputRow.Cells[pg_vendor_name.Index].Value = row["vendor_name"];

                    dataGridView2.Rows.Add(outputRow);
                }
            }

            // LOAD UP VENDOR FILE
            string query = "SELECT * FROM PSVEMAST WHERE CMCUCODE = \"" + vendor_code + "\"";
            OleDbCommand accesscommand = new OleDbCommand(query, dbfglobal.dbfCon); // Query -> Result
            OleDbDataAdapter vendorAdaptor = new OleDbDataAdapter(accesscommand); // Result -> Adaptor

            dbfglobal.dbfCon.Open();
            DataSet vendor_data = new DataSet();
            vendorAdaptor.Fill(vendor_data, "PSVEMAST"); // Adaptor -> vendor_data
            dbfglobal.dbfCon.Close();
            DataRow vendor_row = vendor_data.Tables["PSVEMAST"].Rows[0]; // vendor_data -> vendor_row

            input_CMADD1.Text = vendor_row["CMADD1"].ToString();
            input_CMADD2.Text = vendor_row["CMADD2"].ToString();
            input_CMCITY.Text = vendor_row["CMCITY"].ToString();
            input_CMCOUNTRY.Text = vendor_row["CMCOUNTRY"].ToString();
            input_CMCUNAME.Text = vendor_row["CMCUNAME"].ToString();
            input_CMFAX1.Text = vendor_row["CMFAX1"].ToString();
            input_CMNAME1ST.Text = vendor_row["CMNAME1ST"].ToString();
            input_CMNAMESUR.Text = vendor_row["CMNAMESUR"].ToString();
            input_CMPHONE.Text = vendor_row["CMPHONE"].ToString();
            input_CMSTATE.Text = vendor_row["CMSTATE"].ToString();

            calcTotals();

            loaded = true;

        }

        private static add_item_to_cpo additem = null;

        private void button4_Click(object sender, EventArgs e)
        {
            additem = new add_item_to_cpo(this, -1);

            additem.ShowDialog(this);
        }

        public string nextConsignmentCode()
        {
            string output = "";
            MySqlCommand mysqlCmd = new MySqlCommand("SELECT `consignment_code` FROM `CSTITEM` ORDER BY `consignment_code` DESC", mysqlglobal.mysqlCon);
            try
            {
                string currentCode = mysqlCmd.ExecuteScalar().ToString();
                output = Convert.ToString(Convert.ToInt32(currentCode) + 1);
            }
            catch
            {
                /* Read Start Code */
                output = iniglobal.ini.IniReadValue("startcodes", "consignment");
            }
            return output;
        }

        public void saveItems()
        {
            calcTotals();

            // If New
            if (mode != "edit")
            {
                // Get next consignment code
                consignment_code = nextConsignmentCode();
            }

            // If Vendor has been Modified
            if (vendorModified == true)
            {               
                string CMADD1 = dbfglobal.escapeString(input_CMADD1.Text);
                string CMADD2 = dbfglobal.escapeString(input_CMADD2.Text);
                string CMCITY = dbfglobal.escapeString(input_CMCITY.Text);
                string CMCOUNTRY = dbfglobal.escapeString(input_CMCOUNTRY.Text);
                string CMCUNAME = dbfglobal.escapeString(input_CMCUNAME.Text);
                string CMFAX1 = dbfglobal.escapeString(input_CMFAX1.Text);
                string CMNAME1ST = dbfglobal.escapeString(input_CMNAME1ST.Text);
                string CMNAMESUR = dbfglobal.escapeString(input_CMNAMESUR.Text);
                string CMPHONE = dbfglobal.escapeString(input_CMPHONE.Text);
                string CMSTATE = dbfglobal.escapeString(input_CMSTATE.Text);

                string query = 
                @"UPDATE PSVEMAST SET
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
                WHERE CMCUCODE = """ + vendor_code + @"""";

                //MessageBox.Show(query);

                OleDbCommand accesscommand = new OleDbCommand(query, dbfglobal.dbfCon); // Query
                dbfglobal.dbfCon.Open();
                accesscommand.ExecuteNonQuery();
                dbfglobal.dbfCon.Close();
            }


            // Insert new items into DB
            int records = dataGridView1.RowCount;
            for (int i = records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                // consignment_code = global
                // vendor_code = global
                    string upc = Convert.ToString(dataGridView1.Rows[i].Cells[ig_upc.Index].Value);
                    string description = mysqlglobal.escapeString(Convert.ToString(dataGridView1.Rows[i].Cells[ig_description.Index].Value));
                    string price_minimum = Convert.ToString(dataGridView1.Rows[i].Cells[ig_price_minimum.Index].Value);
                    string price_suggested = Convert.ToString(dataGridView1.Rows[i].Cells[ig_price_suggested.Index].Value);
                    string price_sale = Convert.ToString(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value);
                    string share = Convert.ToString(dataGridView1.Rows[i].Cells[ig_share.Index].Value);
                    string share_type = Convert.ToString(dataGridView1.Rows[i].Cells[ig_share_type.Index].Value); // Process
                    string status = Convert.ToString(dataGridView1.Rows[i].Cells[ig_status.Index].Value);

                    string date_received = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_received.Index].Value);
                    string date_expiry = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_expiry.Index].Value);
                    string date_sold = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_sold.Index].Value);
                    string date_paid = Convert.ToString(dataGridView1.Rows[i].Cells[ig_date_paid.Index].Value);

                    string desc_brand = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_brand.Index].Value);
                    string desc_gender = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_gender.Index].Value);
                    string desc_garment = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_garment.Index].Value);
                    string desc_material = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_material.Index].Value);
                    string desc_colour = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_colour.Index].Value);
                    string desc_size = Convert.ToString(dataGridView1.Rows[i].Cells[ig_desc_size.Index].Value);

                    if (share_type == "$") share_type = "value";
                    if (share_type == "%") share_type = "percentage";

                string strSQL = "";
                if (status != "Deleted")
                {
                    if (status == "New Item")
                    {
                        /* Set next UPC to lowest avaliable */
                        string resetUPC = "ALTER TABLE `CSTITEM` AUTO_INCREMENT = " + iniglobal.ini.IniReadValue("startcodes", "upc");
                        mysqlglobal.executeNonQuery(resetUPC, this);

                        string existing_upc = "null";
                        if (upc == "") existing_upc = "null";
                        if (upc != "") existing_upc = "\"" + upc + "\"";

                        strSQL =
                        @"INSERT INTO `CSTITEM` (
                    `upc`,
                    `consignment_code`,
                    `vendor_code`,
                    `description`,
                    `price_minimum`,
                    `price_suggested`,
                    `price_sale`,
                    `share`,
                    `share_type`,
                    `status`,
                    `consignment_status`,
                    `date_received`,
                    `date_expiry`,
                    `date_sold`,
                    `date_paid`,
                    `desc_brand`,
                    `desc_gender`,
                    `desc_garment`,
                    `desc_material`,
                    `desc_colour`,
                    `desc_size`
                    ) VALUES (
                    " + existing_upc + @",
                    '" + consignment_code + @"',
                    '" + vendor_code + @"',
                    """ + description + @""",
                    '" + price_minimum + @"',
                    '" + price_suggested + @"',
                    '" + price_sale + @"',
                    '" + share + @"',
                    '" + share_type + @"',
                    'unsold',
                    '" + consignment_status + @"',
                    '" + date_received + @"',
                    '" + date_expiry + @"',
                    '" + date_sold + @"',
                    '" + date_paid + @"',
                    '" + desc_brand + @"',
                    '" + desc_gender + @"',
                    '" + desc_garment + @"',
                    '" + desc_material + @"',
                    '" + desc_colour + @"',
                    '" + desc_size + "');";
                    }
                    else
                    {
                        strSQL =
                        @"UPDATE `CSTITEM` SET
                    `description` = """ + description + @""",
                    `price_minimum` = '" + price_minimum + @"',
                    `price_suggested` = '" + price_suggested + @"',
                    `price_sale` = '" + price_sale + @"',
                    `share` = '" + share + @"',
                    `share_type` = '" + share_type + @"',
                    `status` = '" + status + @"',
                    `consignment_status` = '" + consignment_status + @"',
                    `date_received` = '" + date_received + @"',
                    `date_expiry` = '" + date_expiry + @"',
                    `date_sold` = '" + date_sold + @"',
                    `date_paid` = '" + date_paid + @"',
                    `desc_brand` = '" + desc_brand + @"',
                    `desc_gender` = '" + desc_gender + @"',
                    `desc_garment` = '" + desc_garment + @"',
                    `desc_material` = '" + desc_material + @"',
                    `desc_colour`= '" + desc_colour + @"',
                    `desc_size`= '" + desc_size + "' WHERE `upc` = '" + upc + "'";
                    }
                }
                else
                {
                    // Remove row
                    strSQL =
                        @"DELETE FROM `CSTITEM` WHERE `upc` = '" + upc + "'";
                }
                mysqlglobal.executeNonQuery(strSQL, this);              
            }

            // Insert new Payments into DB
            records = dataGridView2.RowCount;
            for (int i = records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                // consignment_code = global
                // vendor_code = global

                string payment_id = Convert.ToString(dataGridView2.Rows[i].Cells[pg_id.Index].Value);
                string payment_code = Convert.ToString(dataGridView2.Rows[i].Cells[pg_type.Index].Value);
                string payment_description = Convert.ToString(dataGridView2.Rows[i].Cells[pg_desc.Index].Value);
                string payment_reference = Convert.ToString(dataGridView2.Rows[i].Cells[pg_cn.Index].Value);
                string payment_expiry = Convert.ToString(dataGridView2.Rows[i].Cells[pg_expiry.Index].Value);
                string payment_date = Convert.ToString(dataGridView2.Rows[i].Cells[pg_date.Index].Value);
                string payment_amount = Convert.ToString(dataGridView2.Rows[i].Cells[pg_amount.Index].Value);
                string payment_vendor_name = Convert.ToString(dataGridView2.Rows[i].Cells[pg_vendor_name.Index].Value);

                string status = Convert.ToString(dataGridView2.Rows[i].Cells[pg_status.Index].Value);
                string strSQL = "";
                if (status != "Deleted")
                {
                    if (payment_id == "")
                    {
                        // Insert into Database
                            strSQL =
                            @"INSERT INTO `CSTPAYMENT` (
                            `consignment_code`,
                            `type`,
                            `description`,
                            `cn`,
                            `expiry`,
                            `date`,
                            `amount`,
                            `vendor_code`,
                            `vendor_name`
                            ) VALUES (
                            '" + consignment_code + @"',
                            '" + payment_code + @"',
                            '" + payment_description + @"',
                            '" + payment_reference + @"',
                            '" + payment_expiry + @"',
                            " + payment_date + @",
                            '" + payment_amount + @"',
                            '" + vendor_code + @"',
                            '" + payment_vendor_name + @"');";

                    }
                    else
                    {
                        strSQL =
                        @"UPDATE `CSTPAYMENT` SET
                    `consignment_code` = '" + consignment_code + @"',
                    `type` = '" + payment_code + @"',
                    `description` = '" + payment_description + @"',
                    `cn` = '" + payment_reference + @"',
                    `expiry` = '" + payment_expiry + @"',
                    `date` = '" + payment_date + @"',
                    `amount` = '" + payment_amount + @"',
                    `vendor_code` = '" + vendor_code + @"',
                    `vendor_name` = '" + payment_vendor_name + @"' WHERE `id` = '" + payment_id + "'";
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

        public void addPayment(string payment_code, string payment_description, string payment_reference, string payment_expiry, string payment_vendor_code, string payment_vendor_name, string payment_amount)
        {
            DataGridViewRow outputRow = new DataGridViewRow();
            outputRow.CreateCells(dataGridView2);
            outputRow.Cells[pg_type.Index].Value = payment_code;
            outputRow.Cells[pg_desc.Index].Value = payment_description;
            outputRow.Cells[pg_cn.Index].Value = payment_reference;
            outputRow.Cells[pg_expiry.Index].Value = payment_expiry;
            outputRow.Cells[pg_display_date.Index].Value = mysqlglobal.formatDate(DateTime.Now);
            outputRow.Cells[pg_date.Index].Value = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(DateTime.Now));
            outputRow.Cells[pg_amount.Index].Value = payment_amount;

            outputRow.Cells[pg_vendor_code.Index].Value = payment_vendor_code;
            outputRow.Cells[pg_vendor_name.Index].Value = payment_vendor_name;
            dataGridView2.Rows.Add(outputRow);

            payment_entry_form.Dispose();

            // ReCalc Totals
            calcTotals();
        }

        public void addItem(string description, string price_minimum, string price_suggested, string share, string share_type, string desc_brand, string desc_gender, string desc_garment, string desc_material, string desc_colour, string desc_size, DateTime input_date_received, DateTime input_date_expiry, string existing_upc, int rowIndex)
        {
            // Convert Date Received
            //string date_received = mysqlglobal.formatDate(input_date_received); // yyyy-MM-dd
            string date_received = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(input_date_received));
            string date_expiry = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(input_date_expiry));

            string display_date_expiry = mysqlglobal.formatDate(input_date_expiry);
            // Get UPC
            /*
            mysqlCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", mysqlglobal.mysqlCon);
            string upc = mysqlCmd.ExecuteScalar().ToString();
            mysqlCmd.Dispose();

            mysqlCmd = new MySqlCommand("SELECT * FROM `CSTITEM` WHERE `consignment_code` = \"" + consignment_code + "\" AND upc = \"" + upc + "\"", mysqlglobal.mysqlCon);
            MySqlDataAdapter myDA = new MySqlDataAdapter(mysqlCmd); DataSet item_result = new DataSet(); myDA.Fill(item_result, "CSTITEM");
            mysqlCmd.Dispose();
            
            DataRow item_row = item_result.Tables["CSTITEM"].Rows[0];
            */

            if (rowIndex == -1)
            {
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);

                outputRow.Cells[ig_status.Index].Value = "New Item"; // Status
                outputRow.Cells[ig_upc.Index].Value = existing_upc;
                outputRow.Cells[ig_description.Index].Value = description;

                outputRow.Cells[ig_price_minimum.Index].Value = price_minimum;
                outputRow.Cells[ig_price_suggested.Index].Value = price_suggested;
                outputRow.Cells[ig_price_sale.Index].Value = "0.00"; // Selling price
                outputRow.Cells[ig_share.Index].Value = share;
                outputRow.Cells[ig_share_type.Index].Value = share_type;
                outputRow.Cells[ig_date_received.Index].Value = date_received;
                outputRow.Cells[ig_date_expiry.Index].Value = date_expiry;

                outputRow.Cells[ig_display_date_expiry.Index].Value = display_date_expiry;

                outputRow.Cells[ig_desc_brand.Index].Value = desc_brand;
                outputRow.Cells[ig_desc_gender.Index].Value = desc_gender;
                outputRow.Cells[ig_desc_garment.Index].Value = desc_garment;
                outputRow.Cells[ig_desc_material.Index].Value = desc_material;
                outputRow.Cells[ig_desc_colour.Index].Value = desc_colour;
                outputRow.Cells[ig_desc_size.Index].Value = desc_size;

                dataGridView1.Rows.Add(outputRow);
                // If Success
                additem.Hide();
            }
            else
            {
                dataGridView1.Rows[rowIndex].Cells[ig_description.Index].Value = description;
                dataGridView1.Rows[rowIndex].Cells[ig_price_minimum.Index].Value = price_minimum;
                dataGridView1.Rows[rowIndex].Cells[ig_price_suggested.Index].Value = price_suggested;
                dataGridView1.Rows[rowIndex].Cells[ig_share.Index].Value = share;
                dataGridView1.Rows[rowIndex].Cells[ig_share_type.Index].Value = share_type;
                dataGridView1.Rows[rowIndex].Cells[ig_date_received.Index].Value = date_received;
                dataGridView1.Rows[rowIndex].Cells[ig_date_expiry.Index].Value = date_expiry;

                dataGridView1.Rows[rowIndex].Cells[ig_display_date_expiry.Index].Value = display_date_expiry;

                dataGridView1.Rows[rowIndex].Cells[ig_desc_brand.Index].Value = desc_brand;
                dataGridView1.Rows[rowIndex].Cells[ig_desc_gender.Index].Value = desc_gender;
                dataGridView1.Rows[rowIndex].Cells[ig_desc_garment.Index].Value = desc_garment;
                dataGridView1.Rows[rowIndex].Cells[ig_desc_material.Index].Value = desc_material;
                dataGridView1.Rows[rowIndex].Cells[ig_desc_colour.Index].Value = desc_colour;
                dataGridView1.Rows[rowIndex].Cells[ig_desc_size.Index].Value = desc_size;

                additem.Dispose(); // Remove all traces
            }

        }

        private void consignment_purchase_order_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            vendorModified = true;
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
            
            decimal totalOwed = 0;
            decimal totalPaid = 0;
            decimal totalOutstanding = 0;

            decimal totalProfit = 0;

            decimal totalMinPrice = 0;
            decimal totalSugPrice = 0;


            // Cycle through Records
            int item_records = dataGridView1.RowCount;
            for (int i = item_records - 1; i >= 0; i--) // Has to do a backwards for loop, last row is row 0.
            {
                if( Convert.ToString(dataGridView1.Rows[i].Cells[ig_status.Index].Value) == "Sold") {
                    // Calculate Share if Percentage
                    if(Convert.ToString(dataGridView1.Rows[i].Cells[ig_share_type.Index].Value) == "%") {
                        share_out = Convert.ToDecimal(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value) * ((Convert.ToDecimal(dataGridView1.Rows[i].Cells[ig_share.Index].Value) / 100)); // 5 - Sale Price. 6 - Share
                    } else {
                        share_out = Convert.ToDecimal(dataGridView1.Rows[i].Cells[ig_share.Index].Value); // 6 - Share
                    }

                    totalOwed = totalOwed + share_out;
                    totalProfit = totalProfit + (Convert.ToDecimal(dataGridView1.Rows[i].Cells[ig_price_sale.Index].Value) - share_out);
                }
            }

            output_totalowed.Text = cg.price(totalOwed);

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
            // Open enter Payment window
            string vendor_name = input_CMCUNAME.Text;
            string totalcost = output_totaloutstanding.Text;

            if (payment_entry_form == null)
            {
                payment_entry_form = new payment_entry(this,vendor_code,vendor_name,totalcost);
            }
            else
            {
                payment_entry_form.Dispose();
                payment_entry_form = new payment_entry(this, vendor_code, vendor_name, totalcost);
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
            saveItems();
            if (MessageBox.Show("Do you wish to Print a Consignment Agreement Now?", "Print Consignment Agreement", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //printRptConsignment();
                print_report prt = new print_report(consignment_code, null, "Consignment Agreement");
                prt.ShowDialog(this);
            }

            timer1.Enabled = true; // Z-Order Fix

            if (MessageBox.Show("Do you wish to Print Item Labels Now?", "Print Consignment Agreement", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //printRptConsignment();
                print_report prt = new print_report(consignment_code, null, "Consignment Barcode Item Labels");
                prt.ShowDialog(this);
            }

            // Update Purchase Desktop
            foreach (Form form_search in Application.OpenForms)
            {
                if (form_search.Name == "consignment_purchase_desktop")
                {
                    (form_search as consignment_purchase_desktop).loadConsignments();
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
        }

        private void button11_Click(object sender, EventArgs e)
        {

            //if(additem != null || additem.IsDisposed == false) additem.Dispose();
            int editRow = dataGridView1.SelectedRows[0].Index;
            if (Convert.ToString(dataGridView1.Rows[editRow].Cells[ig_status.Index].Value) != "Sold")
            {
                additem = new add_item_to_cpo(this, editRow);
                additem.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Sorry, but you cannot edit sold items.", "Cannot Edit");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Hide Row
            dataGridView2.SelectedRows[0].Visible = false;

            // Mark Row as Deleted
            dataGridView2.SelectedRows[0].Cells[pg_status.Index].Value = "Deleted";

            // Calc Totals
            calcTotals();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            change_status_cpo statusWin = new change_status_cpo(this,consignment_status);
            statusWin.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /* More Z-order issues */
            this.TopMost = true;
            timer1.Enabled = false;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button11_Click(null, null);
        }


    }
}

    


    


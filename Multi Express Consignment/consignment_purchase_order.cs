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
    public partial class consignment_purchase_order : Form
    {

        public static string consignment_code;
        public static string vendor_code;

        public string getconsignment_code()
        {
            return consignment_code;
        }

        public consignment_purchase_order(string consignmentCode)
        {
            InitializeComponent();
            consignment_code = consignmentCode;
        }

        private void consignment_purchase_order_Shown(object sender, EventArgs e)
        {
            consignment_code_textbox.Text = consignment_code;

            // Load up Consignment
            MySqlCommand mysqlCmd = null;
            MySqlDataReader mysqlReader;

            string strSQL = "SELECT * FROM `CSTITEM` WHERE `consignment_code` = \"" + consignment_code + "\"";
            mysqlCmd = new MySqlCommand(strSQL, mysqlglobal.mysqlCon);
            MySqlDataAdapter myDA = new MySqlDataAdapter(mysqlCmd);
            DataSet consignment_file = new DataSet();
            myDA.Fill(consignment_file, "CSTITEM");

            foreach (DataRow row in consignment_file.Tables["CSTITEM"].Rows)
            {
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);

                outputRow.Cells[0].Value = row["upc"];
                outputRow.Cells[1].Value = row["description"];

                outputRow.Cells[2].Value = row["price_minimum"];
                outputRow.Cells[3].Value = row["price_sale"];
                outputRow.Cells[4].Value = row["share"];

                vendor_code = Convert.ToString(row["vendor_code"]);

                dataGridView1.Rows.Add(outputRow);
            }
            vendor_code_textbox.Text = vendor_code;
        }

        private static add_item_to_cpo additem = null;

        private void button4_Click(object sender, EventArgs e)
        {
            if (additem == null)
            {
                additem = new add_item_to_cpo(this);
            }
            additem.Show();
        }

        public void addItem(string description, string price_minimum, string price_suggested, string share, string share_type, string desc_brand, string desc_gender, string desc_garment, string desc_material, string desc_colour)
        {

            // Non set vars
            string status = "unsold";
            string date_received = "NOW()";

            // Insert into DB
            string strSQL = 
            @"INSERT INTO `CSTITEM` (
            `consignment_code`,
            `vendor_code`,
            `description`,
            `price_minimum`,
            `price_suggested`,
            `share`,
            `share_type`,
            `status`,
            `date_received`,
            `desc_brand`,
            `desc_gender`,
            `desc_garment`,
            `desc_material`,
            `desc_colour`
            ) VALUES (
            '" + consignment_code + @"',
            '" + vendor_code + @"',
            '" + description + @"',
            '" + price_minimum + @"',
            '" + price_suggested + @"',
            '" + share + @"',
            '" + share_type + @"',
            '" + status + @"',
            " + date_received + @",
            '" + desc_brand + @"',
            '" + desc_gender + @"',
            '" + desc_garment + @"',
            '" + desc_material + @"',
            '" + desc_colour + "');";
            MySqlCommand mysqlCmd = new MySqlCommand(strSQL, mysqlglobal.mysqlCon);
            mysqlCmd.ExecuteNonQuery();
            mysqlCmd.Dispose();

            // Get UPC
            mysqlCmd = new MySqlCommand("SELECT LAST_INSERT_ID();", mysqlglobal.mysqlCon);
            string upc = mysqlCmd.ExecuteScalar().ToString();
            mysqlCmd.Dispose();

            mysqlCmd = new MySqlCommand("SELECT * FROM `CSTITEM` WHERE `consignment_code` = \"" + consignment_code + "\" AND upc = \"" + upc + "\"", mysqlglobal.mysqlCon);
            MySqlDataAdapter myDA = new MySqlDataAdapter(mysqlCmd); DataSet item_result = new DataSet(); myDA.Fill(item_result, "CSTITEM");
            mysqlCmd.Dispose();

            DataRow item_row = item_result.Tables["CSTITEM"].Rows[0];


            DataGridViewRow outputRow = new DataGridViewRow();
            outputRow.CreateCells(dataGridView1);

            outputRow.Cells[0].Value = item_row["upc"];
            outputRow.Cells[1].Value = item_row["description"];

            outputRow.Cells[2].Value = item_row["price_minimum"];
            outputRow.Cells[3].Value = item_row["price_sale"];
            outputRow.Cells[4].Value = item_row["share"];

            dataGridView1.Rows.Add(outputRow);
            
            // If Success
            additem.Hide();
        }

        private void consignment_purchase_order_Load(object sender, EventArgs e)
        {

        }


    }
}

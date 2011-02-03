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

        private void consignment_purchase_desktop_Shown(object sender, EventArgs e)
        {
            windowProportions();
            
            // Load MySQL data
            MySqlCommand mysqlCmd = null;
            MySqlDataReader mysqlReader;



            string strSQL = "SELECT * FROM `CSTITEM` ORDER BY `consignment_code` ASC";
            mysqlCmd = new MySqlCommand(strSQL, mysqlglobal.mysqlCon);
            MySqlDataAdapter myDA = new MySqlDataAdapter(mysqlCmd);
            DataSet item_file = new DataSet();
            myDA.Fill(item_file, "CSTITEM");

            // If in Consignment Mode
            string prev_consignment_code = null;
            foreach (DataRow row in item_file.Tables["CSTITEM"].Rows)
            {

                // Fetch Data on Vendor
                query = "SELECT * FROM PSVEMAST WHERE CMCUCODE = \"" + row["vendor_code"] +"\"";
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

                    outputRow.Cells[0].Value = imageList1.Images[0];
                    outputRow.Cells[1].Value = row["consignment_code"];
                    outputRow.Cells[2].Value = row["vendor_code"];

                    outputRow.Cells[3].Value = vendor_row["CMNAME1ST"]; // Vendor First Name
                    outputRow.Cells[4].Value = vendor_row["CMNAMESUR"]; // Vendor Last Name
                    outputRow.Cells[5].Value = vendor_row["CMPHONE"]; // Vendor Last Name

                    outputRow.Cells[7].Value = row["share"];
                    outputRow.Cells[8].Value = 1;

                    dataGridView1.Rows.Add(outputRow);
                }
                else
                {
                    var lastRow = dataGridView1.Rows.Count - 1;
                    dataGridView1.Rows[lastRow].Cells[7].Value = Convert.ToInt32(dataGridView1.Rows[lastRow].Cells[7].Value) + 1;
                    dataGridView1.Rows[lastRow].Cells[6].Value = Convert.ToDecimal(dataGridView1.Rows[lastRow].Cells[6].Value) + Convert.ToDecimal(row["share"]);
                }





                prev_consignment_code = Convert.ToString(row["consignment_code"]);
            }

        }

        private void openPurchaseOrder(string consignmentCode)
        {
            consignment_purchase_order cpo = new consignment_purchase_order(consignmentCode);
            cpo.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Browse selected Purchase Desktop
            string selectedConsignment = Convert.ToString(dataGridView1.SelectedRows[0].Cells[1].Value);
            //MessageBox.Show(selectedConsignment);
            openPurchaseOrder(selectedConsignment);
        }

        private void consignment_purchase_desktop_Load(object sender, EventArgs e)
        {
           
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

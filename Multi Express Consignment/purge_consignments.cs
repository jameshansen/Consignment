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
    public partial class purge_consignments : Form
    {
        public purge_consignments()
        {
            InitializeComponent();
        }

        public bool dontclose = false;

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Only accept ints
            int isNumber = 0;
            e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
        }

        private void purge_consignments_Load(object sender, EventArgs e)
        {
            // Set width
            this.Width = 266;

            // Get first consignment date and number
            DataSet results = mysqlglobal.executeDataSetQuery("SELECT * FROM `CSTITEM` ORDER BY `consignment_code` ASC LIMIT 1", "CSTITEM", this);

            if (results.Tables["CSTITEM"].Rows.Count > 0) // Bugfix (2026): No items in the database to date from
            {
                DataRow result = results.Tables["CSTITEM"].Rows[0];

                input_dateFrom.Value = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(result["date_received"]));
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Show Date Group
            dateGroup.Left = 12;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Safe Purge, purge each record one at a time.
            string query = "SELECT * FROM `CSTITEM` WHERE";

            if (radioButton1.Checked == true)
            {
                // Date Based
                string dateFromUnixtime = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(input_dateFrom.Value));
                string dateToUnixtime   = Convert.ToString(mysqlglobal.ConvertToUnixTimestamp(input_dateTo.Value));

                query += " date_expiry >= " + dateFromUnixtime + " AND date_expiry <= " + dateToUnixtime + " ";
            }

            string qpart = "";
            if (checkBoxSold.Checked)
            {
                qpart = "status=\"sold\"";
            }
            if (checkBoxUnsold.Checked)
            {
                qpart = "status=\"unsold\"";
            }
            if (checkBoxSold.Checked && checkBoxUnsold.Checked)
            {
                qpart = "";
            }

            query += qpart;


            query += " ORDER BY `upc` ASC";

            int consignment_count = 0;

            DataSet recordsToDelete = mysqlglobal.executeDataSetQuery(query, "CSTITEM", this);
            consignment_count = recordsToDelete.Tables["CSTITEM"].Rows.Count;           

            // If Zero
            if (consignment_count == 0)
            {
                MessageBox.Show("No items matching given parameters were found.", "Empty Set");
                return;
            }

            // Confirm after getting result count
            string sOrNo = "s";
            if (consignment_count == 1) sOrNo = "";

            if (MessageBox.Show("Are you sure you wish to purge the selected " + consignment_count.ToString() + " items" + sOrNo + "?", "Purge?", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return; // Cancel               
            }

            dontclose = true;
            
            Application.DoEvents();
            // Otherwise, begin purge
            groupBox1.Visible = false;
            dateGroup.Visible = false;

            progressGroup.Left = 12;
            progressBar1.Maximum = consignment_count;

            this.Height = 121;

            foreach (DataRow record in recordsToDelete.Tables["CSTITEM"].Rows)
            {
                currentConsignment.Text = "Purging item " + Convert.ToString(record["upc"]);

                mysqlglobal.executeNonQuery("DELETE FROM `CSTITEM` WHERE `upc` = " + Convert.ToString(record["upc"]), this);

                if (deletePayments.Checked)
                {
                    // Check to see if any items remain in consignment
                    int rCount = Convert.ToInt32(mysqlglobal.executeScalarQuery("SELECT COUNT(*) FROM `CSTITEM` WHERE `consignment_code` = \"" + record["consignment_code"].ToString() + "\"",this));
                    if (rCount == 0)
                    {
                        // No records. Delete Payments for this Consignment.
                        mysqlglobal.executeNonQuery("DELETE FROM `CSTPAYMENT` WHERE `consignment_code` = " + Convert.ToString(record["consignment_code"]), this);

                    }
                }

                progressBar1.PerformStep();
                Application.DoEvents();
            }
            dontclose = false;

            this.Close();

            MessageBox.Show("Purge Complete", "Completed");

            // Update Purchase Desktop
            foreach (Form form_search in Application.OpenForms)
            {
                if (form_search.Name == "consignment_purchase_desktop")
                {
                    (form_search as consignment_purchase_desktop).loadConsignments();
                }
            }
            

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void purge_consignments_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dontclose == true)
            {
                e.Cancel = true;
            }

        }
    }
}

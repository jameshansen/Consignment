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

            DataRow result = results.Tables["CSTITEM"].Rows[0];

            input_dateFrom.Value = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(result["date_received"]));
            input_numFrom.Text = Convert.ToString(result["consignment_code"]);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Show Date Group
            dateGroup.Left = 12;
            numberGroup.Left = 277; // Hide off-form
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // Show Number Group
            numberGroup.Left = 12;
            dateGroup.Left = 277; // Hide off-form
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

                query += " date_received >= " + dateFromUnixtime + " AND date_received <= " + dateToUnixtime + " ";
            }

            if (radioButton2.Checked == true)
            {
                // Number Based
                string numberFrom = input_numFrom.Text;
                string numberTo = input_numTo.Text;
                query += " consignment_code >= " + numberFrom + " AND consignment_code <=" + numberTo + " ";
            }

            

            query += "GROUP BY consignment_code ORDER BY `consignment_code` ASC";

            int consignment_count = 0;

            DataSet recordsToDelete = mysqlglobal.executeDataSetQuery(query, "CSTITEM", this);
            consignment_count = recordsToDelete.Tables["CSTITEM"].Rows.Count;

            // Check each consignment against Sold / Unsold Checkboxes. If the consignment is partial it won't be removed unless both are checked.
            if (checkBoxSold.Checked == false || checkBoxUnsold.Checked == false)
            {
                for (int i = consignment_count - 1; i >= 0; i--)
                {
                    string consignment_code = Convert.ToString(recordsToDelete.Tables["CSTITEM"].Rows[i]["consignment_code"]);
                    DataSet consignmentItems = mysqlglobal.executeDataSetQuery("SELECT * FROM `CSTITEM` WHERE consignment_code = " + consignment_code, "CSTITEM", this);
                    bool deleteThisRow = false;

                    // Scan Consignment
                    foreach (DataRow itemRow in consignmentItems.Tables["CSTITEM"].Rows)
                    {
                        if (Convert.ToString(itemRow["status"]) == "sold" && checkBoxSold.Checked == false)
                        {
                            deleteThisRow = true;
                        }

                        if (Convert.ToString(itemRow["status"]) == "unsold" && checkBoxUnsold.Checked == false)
                        {
                            deleteThisRow = true;
                        }
                    }

                    // If unmatching item is contained, remove Consignment from Purge list.
                    if (deleteThisRow)
                    {
                        recordsToDelete.Tables["CSTITEM"].Rows.RemoveAt(i);
                    }
                }
            }

            // Recount Rows
            consignment_count = recordsToDelete.Tables["CSTITEM"].Rows.Count;

            // If Zero
            if (consignment_count == 0)
            {
                MessageBox.Show("No Consignments Matching the Perameters were Found.", "Empty Set");
                return;
            }

            // Confirm after getting result count
            string sOrNo = "s";
            if (consignment_count == 1) sOrNo = "";

            if (MessageBox.Show("Are you sure you wish to purge the selected " + consignment_count.ToString() + " consignment" + sOrNo + "?", "Purge?", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return; // Cancel               
            }

            // Otherwise, begin purge
            groupBox1.Visible = false;
            dateGroup.Visible = false;
            numberGroup.Visible = false;

            progressGroup.Left = 12;
            progressBar1.Maximum = consignment_count;

            this.Height = 121;

            foreach (DataRow record in recordsToDelete.Tables["CSTITEM"].Rows)
            {
                currentConsignment.Text = "Purging consignment " + Convert.ToString(record["consignment_code"]);

                mysqlglobal.executeNonQuery("DELETE FROM `CSTITEM` WHERE `consignment_code` = " + Convert.ToString(record["consignment_code"]), this);

                progressBar1.PerformStep();
            }

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
    }
}

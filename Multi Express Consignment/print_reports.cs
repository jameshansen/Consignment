using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CrystalDecisions;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using System.Data.OleDb;
using System.Xml.Serialization;

namespace Multi_Express_Consignment
{
    public partial class print_reports : Form
    {
        // Readonly Obfuscates the String for Hardcoded Addresses
        readonly string b1A = "3629 West 4th Ave., Vancouver, B.C. V6R 1P2";
        readonly string b1P = "(604) 730-9638";


        /* Init Output Window */
        public static crystalreportglobal rptwin = new crystalreportglobal();
        public static ReportDocument cryRpt = new ReportDocument();

        /* Init DataSet */
        public DataSet consignment_db_var = new consignment_db(); // Use DataSet XML Template (DataSet1.xsd)

        public print_reports()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* Consignment Payment Out Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Consignment Payment Out");
            print_reports_options_form.ShowDialog(this);
        }

        private void standard_headers()
        {
            /* Load Header Details from INI File [company] section */
            string companyName = iniglobal.ini.IniReadValue("company", "name");
            
            string branch1Address = b1A;
            string branch1Telephone = b1P;

            /* Now hardcoded */
            /*
            string branch1Address = iniglobal.ini.IniReadValue("company", "branch1Address");
            string branch1Telephone = iniglobal.ini.IniReadValue("company", "branch1Telephone");
            */
             
            string dateFormat = iniglobal.ini.IniReadValue("company", "dateFormat");

            /* Input Header Details into Report */
            crystalreportglobal.SetFormulaFieldString(cryRpt, "companyName", companyName);           // Company Name
            crystalreportglobal.SetFormulaFieldString(cryRpt, "branch1Address", branch1Address);     // Branch 1 Address
            crystalreportglobal.SetFormulaFieldString(cryRpt, "branch1Telephone", branch1Telephone); // Branch 1 Telephone

            /* Date Format */
            crystalreportglobal.SetFormulaFieldString(cryRpt, "dateFormat", dateFormat); // Branch 1 Telephone
        }

        public void print_report_method(string reportName, double start_unixtime, double end_unixtime, string output, string totals_shown = "", string report_type = "", string consignor_code = "", double upto_unixtime = 0)
        {
            // Dispose of it, memory problem workaround (one of the many worarounds) for Crystal Reports(tm)
            cryRpt.Dispose();
            rptwin.Dispose();

            // JH: 2011-11-16 Clear the DataSet (BUG 3)
            consignment_db_var.Clear();

            rptwin = new crystalreportglobal();
            cryRpt = new ReportDocument();

            string conditions = ""; // Set conditions for report.
            string strSQL = ""; // Used for SQL Queries.

            bool otherReport = true;

            consignment_db_var.Clear();

            if (reportName == "Consolidated Consignor Report")
            {
                strSQL = "SELECT * FROM `CSTITEM` WHERE `vendor_code` = \"" + consignor_code + "\" AND `date_expiry` <= " + upto_unixtime.ToString() + " ORDER BY `consignment_code`,`upc`";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this, consignment_db_var); // Fill consignment_db_var CSTITEM File

                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `vendor_code` = \"" + consignor_code + "\"";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db_var); // Fill consignment_db_var CSTPAYMENT File

               
                Double totalPaid = 0;
                double totalPaidToday = 0;
                //string consignment_code = "";
                foreach (DataRow a in consignment_db_var.Tables["CSTPAYMENT"].Rows)
                {
                    //consignment_code = a["consignment_code"].ToString();
                    //consignment_db_var.Tables["CSTITEM"].
                    
                    totalPaid += Convert.ToDouble(a["amount"]);
                    DateTime paydate = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(a["date"]));
                    paydate = paydate.Date; // Set to date

                    if (paydate == DateTime.Now.Date)
                    {
                        // Same date.
                        totalPaidToday += Convert.ToDouble(a["amount"]);
                    }
                }
                

                DataRow vendorRow;
                try
                {
                    vendorRow = mysqlglobal.executeDataSetQuery("SELECT * FROM `PSVEMAST` WHERE `CMCUCODE` = \"" + consignor_code + "\"", "PSVEMAST", this).Tables["PSVEMAST"].Rows[0];
                }
                catch
                {
                    MessageBox.Show("Consignor code not found.");
                    return;
                }

                cryRpt.Load("report_consolidated_consignor_report.rpt");

                crystalreportglobal.SetFormulaFieldString(cryRpt, "paid_amt", cg.price(totalPaid));
                crystalreportglobal.SetFormulaFieldString(cryRpt, "paid_today", cg.price(totalPaidToday));

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_code", consignor_code);
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_name", vendorRow["CMNAME1ST"].ToString() + " " + vendorRow["CMNAMESUR"].ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_company", vendorRow["CMCUNAME"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_phone", vendorRow["CMPHONE"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_fax", vendorRow["CMFAX1"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_address", vendorRow["CMADD1"].ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "expiry_upto", upto_unixtime.ToString());

                otherReport = false;
            }

            if (reportName == "Order Detail Report")
            {
                /* Old Daily Sales Report - 2011-05-12 */

                /* Load Sales Data */
                
                strSQL = "SELECT * FROM `CSTORDER` WHERE `date_order` >= " + Convert.ToString(start_unixtime) + " AND `date_order` <= " + Convert.ToString(end_unixtime) + " ORDER BY `date_order` ASC";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTORDER", this, consignment_db_var); // Fill consignment_db_var CSTORDER File
                strSQL = "SELECT * FROM `CSTITEM` WHERE `order_number` != \"\"";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this, consignment_db_var); // Fill consignment_db_var CSTITEM File
                strSQL = "SELECT * FROM `CSTPAYMENT` ORDER BY `date` ASC";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db_var); // Fill consignment_db_var CSTPAYMENT File

                cryRpt.Load("report_order_detail_report.rpt");

                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_from", start_unixtime.ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_to", end_unixtime.ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "totals_shown", totals_shown);
                crystalreportglobal.SetFormulaFieldString(cryRpt, "report_type", report_type);
                otherReport = false;
            }

            if (reportName == "Cash Log Report")
            {
                /* Daily Sales Report - 2011-07-07 */

                /* Load Payment Data */

                // Orders
                strSQL = "(SELECT p.*,o.date_order,o.total FROM `CSTPAYMENT` AS p, `CSTORDER` AS o WHERE p.`order_number` = o.`order_number` AND ( p.`date` >= " + Convert.ToString(start_unixtime) + " AND p.`date` <= " + Convert.ToString(end_unixtime) + " AND p.`order_number` != \"\") ORDER BY p.`date` ASC)";

                strSQL += " UNION ";

                // Consignments
                strSQL += "(SELECT p.*,0,0 FROM `CSTPAYMENT` AS p WHERE  p.`date` >= " + Convert.ToString(start_unixtime) + " AND p.`date` <= " + Convert.ToString(end_unixtime) + " AND p.`consignment_code` != \"\" ORDER BY p.`date` ASC)";

                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db_var); // Fill consignment_db_var CSTPAYMENT File

                //strSQL = "SELECT * FROM `CSTORDER` WHERE `date_order` >= " + Convert.ToString(start_unixtime) + " ORDER BY `order_number` ASC";
                //consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTORDER", this, consignment_db_var); // Fill consignment_db_var CSTORDER File

                strSQL = "SELECT * FROM `CSTITEM` WHERE `order_number` != \"\"";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this, consignment_db_var); // Fill consignment_db_var CSTITEM File

                /* Populate Totals Data for Report */
                //consignment_db_var.Tables.Add("CSTTOTALS_V");
                //consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("type", typeof(String));
                // consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("description", typeof(String));
                //consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("total", typeof(decimal));

                Dictionary<string, decimal> totals_in = new Dictionary<string, decimal>();
                Dictionary<string, decimal> totals_out = new Dictionary<string, decimal>();

                foreach (DataRow row in consignment_db_var.Tables["CSTPAYMENT"].Rows)
                {
                    if (row["order_number"].ToString() != "")
                    {
                        if (totals_in.ContainsKey(row["type"].ToString()))
                        {
                            totals_in[row["type"].ToString()] += Convert.ToDecimal(row["amount"]);
                        }
                        else
                        {
                            totals_in.Add(row["type"].ToString(), Convert.ToDecimal(row["amount"]));
                        }
                    }
                    else
                    {
                        if (totals_out.ContainsKey(row["type"].ToString()))
                        {
                            totals_out[row["type"].ToString()] += Convert.ToDecimal(row["amount"]);
                        }
                        else
                        {
                            totals_out.Add(row["type"].ToString(), Convert.ToDecimal(row["amount"]));
                        }
                    }
                }



                string totals_type = "";
                decimal totals_amt = 0;

                foreach (KeyValuePair<string, decimal> pair in totals_in)
                {
                    totals_type = pair.Key;
                    totals_amt = pair.Value;
                    DataRow outputRow = consignment_db_var.Tables["CSTTOTALS_V"].NewRow();
                    outputRow["type"] = totals_type;
                    outputRow["total"] = totals_amt;
                    consignment_db_var.Tables["CSTTOTALS_V"].Rows.Add(outputRow);
                }

                foreach (KeyValuePair<string, decimal> pair in totals_out)
                {
                    totals_type = pair.Key;
                    totals_amt = pair.Value;
                    DataRow outputRow = consignment_db_var.Tables["CSTTOTALSOUT_V"].NewRow();
                    outputRow["type"] = totals_type;
                    outputRow["total"] = totals_amt;
                    consignment_db_var.Tables["CSTTOTALSOUT_V"].Rows.Add(outputRow);
                }


                cryRpt.Load("report_cash_log_report.rpt");

                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_from", start_unixtime.ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_to", end_unixtime.ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "totals_shown", totals_shown);
                crystalreportglobal.SetFormulaFieldString(cryRpt, "report_type", report_type);
                otherReport = false;
            }

            if (reportName == "Daily Sales Report")
            {
                /* Daily Sales Report - 2011-07-07 */

                /* Load Payment Data */

                // Show Invoices Only 2011-08-31
                strSQL = "SELECT p.*,o.date_order FROM `CSTPAYMENT` AS p, `CSTORDER` AS o WHERE p.`order_number` = o.`order_number` AND ( o.`date_order` >= " + Convert.ToString(start_unixtime) + " AND o.`date_order` <= " + Convert.ToString(end_unixtime) + " AND p.`order_number` != \"\" AND `status` = \"invoice\") ORDER BY o.`date_order` ASC";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db_var); // Fill consignment_db_var CSTPAYMENT File

                //strSQL = "SELECT * FROM `CSTORDER` WHERE `date_order` >= " + Convert.ToString(start_unixtime) + " ORDER BY `order_number` ASC";
                //consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTORDER", this, consignment_db_var); // Fill consignment_db_var CSTORDER File

                strSQL = "SELECT * FROM `CSTITEM` WHERE `order_number` != \"\"";
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this, consignment_db_var); // Fill consignment_db_var CSTITEM File

                /* Populate Totals Data for Report */
                //consignment_db_var.Tables.Add("CSTTOTALS_V");
                //consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("type", typeof(String));
               // consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("description", typeof(String));
                //consignment_db_var.Tables["CSTTOTALS_V"].Columns.Add("total", typeof(decimal));

                Dictionary<string, decimal> totals = new Dictionary<string, decimal>();

                foreach (DataRow row in consignment_db_var.Tables["CSTPAYMENT"].Rows) {
                    if (totals.ContainsKey(row["type"].ToString()))
                    {
                        totals[row["type"].ToString()] += Convert.ToDecimal(row["amount"]);
                    }
                    else
                    {
                        totals.Add(row["type"].ToString(), Convert.ToDecimal(row["amount"]));
                    }
                }

                
                
                string totals_type = "";
                decimal totals_amt = 0;

                foreach (KeyValuePair<string, decimal> pair in totals)
                {
                    totals_type = pair.Key;
                    totals_amt = pair.Value;
                    DataRow outputRow = consignment_db_var.Tables["CSTTOTALS_V"].NewRow();
                    outputRow["type"] = totals_type;
                    outputRow["total"] = totals_amt;
                    consignment_db_var.Tables["CSTTOTALS_V"].Rows.Add(outputRow);
                }


                cryRpt.Load("report_daily_sales_report.rpt");

                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_from", start_unixtime.ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "date_to", end_unixtime.ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "totals_shown", totals_shown);
                crystalreportglobal.SetFormulaFieldString(cryRpt, "report_type", report_type);
                otherReport = false;
            }
            
            if(otherReport == true)
            {

                if (reportName == "Consignment Payment Out")
                {
                    conditions = "AND `consignment_code` > 0";
                }

                if (reportName == "Sales Payment In")
                {
                    conditions = "AND `order_number` > 0";
                }

                /* Load Payment Data */
                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `date` >= " + Convert.ToString(start_unixtime) + " AND `date` <= " + Convert.ToString(end_unixtime) + " " + conditions;
                consignment_db_var = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this); // Fill consignment_db_var CSTPAYMENT File

                if (reportName != "Net Payment-In/Out Report")
                {
                    cryRpt.Load("report_payment_reports.rpt");
                }

                {
                    cryRpt.Load("report_net_payment_report.rpt");
                }

            }

            /* Standard Headers */
            standard_headers();

            /* Additional Headers */

            crystalreportglobal.SetFormulaFieldString(cryRpt, "report_name", reportName);

            /* Connect Report to DataSet */
            cryRpt.SetDataSource(consignment_db_var);
            rptwin.viewer.ReportSource = cryRpt;
            rptwin.viewer.Refresh();

            if (output == "screen")
            {
                rptwin.Show();
                rptwin.TopMost = true;
            }
            else
            {
                // Call Print Method
                rptwin.viewer.PrintReport();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            /* Sales Payment In Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Sales Payment In");
            print_reports_options_form.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /* Net Payment-In/Out Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Net Payment-In/Out Report");
            //print_reports_options print_reports_options_form = new print_reports_options(this, "Cash Log Report");
            print_reports_options_form.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /* Daily Sales Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Daily Sales Report");
            print_reports_options_form.ShowDialog(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            /* Order Detail Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Order Detail Report");
            print_reports_options_form.ShowDialog(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            /* Consolidated Consignor Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Consolidated Consignor Report");
            print_reports_options_form.ShowDialog(this);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            /* Cash Log Report */
            print_reports_options print_reports_options_form = new print_reports_options(this, "Cash Log Report");
            print_reports_options_form.ShowDialog(this);
        }

    }
}

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
        public DataSet consignment_db = new consignment_db(); // Use DataSet XML Template (DataSet1.xsd)

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

        public void print_report_method(string reportName, double start_unixtime, double end_unixtime, string output)
        {
            // Dispose of it, memory problem workaround (one of the many worarounds) for Crystal Reports(tm)
            cryRpt.Dispose();
            rptwin.Dispose();

            rptwin = new crystalreportglobal();
            cryRpt = new ReportDocument();

            string conditions = ""; // Set conditions for report.

            if (reportName == "Consignment Payment Out")
            {
                conditions = "AND `consignment_code` > 0";
            }

            if (reportName == "Sales Payment In")
            {
                conditions = "AND `order_number` > 0";
            }

            /* Load Payment Data */
            string strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `date` >= " + Convert.ToString(start_unixtime) + " AND `date` <= " + Convert.ToString(end_unixtime) + " " + conditions;
            consignment_db = mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this); // Fill Consignment_DB CSTPAYMENT File
            Clipboard.SetText(strSQL);

            if (reportName != "Net Payment-In/Out Report")
            {
                cryRpt.Load("report_payment_reports.rpt");
            }
            else
            {
                cryRpt.Load("report_net_payment_report.rpt");
            }

            /* Standard Headers */
            standard_headers();

            /* Additional Headers */

            crystalreportglobal.SetFormulaFieldString(cryRpt, "report_name", reportName);

            /* Connect Report to DataSet */
            cryRpt.SetDataSource(consignment_db);
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
            print_reports_options_form.ShowDialog(this);
        }

    }
}

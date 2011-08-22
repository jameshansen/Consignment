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
    public partial class print_reports_options : Form
    {
        public print_reports m_parent = null;
        public string report_name;

        public print_reports_options(print_reports calledBy, string reportName)
        {
            InitializeComponent();
            m_parent = calledBy;
            report_name = reportName;
        }

        private void print_reports_options_Load(object sender, EventArgs e)
        {
            this.Width = 384;

            if (report_name == "Consolidated Consignor Report")
            {
                tabControl1.Visible = false;
                groupBox1.Left = tabControl1.Left;
            }

            /* If Report Name is different to Daily Sales Report, Hide Extra Options*/
            if (report_name != "Daily Sales Report" && report_name != "Order Detail Report")
            {
                button1.Top = 202;
                button2.Top = 202;
                this.Height = 262;
                dailysalesreport_options.Visible = false;
            }

            if (report_name == "Daily Sales Report")
            {
                report_detailed.Visible = false;
                report_summary.Visible = false;
            }

            /* Populate Month Dropdown */

            // Get first entry from database
            string firstDateDB = mysqlglobal.executeScalarQuery("SELECT `date_order` FROM `cstorder` ORDER BY `date_order` ASC LIMIT 1", this).ToString();

            DateTime firstDate = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(firstDateDB));

            DateTime curDate = firstDate;
            int a = 0;
            int final = Convert.ToInt32(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString());
            while (a < final)
            {
                a = Convert.ToInt32(curDate.Year.ToString() + curDate.Month.ToString());
                sMonthBox.Items.Add(curDate.ToString("MMMM yyyy"));
                //MessageBox.Show(@"a: " + a + ". final: " + final + ". Datestring: " + curDate.ToString("MMMM yyyy"));
                curDate = curDate.AddMonths(1);
            }
            curDate = curDate.AddMonths(-1);
            sMonthBox.Text = curDate.ToString("MMMM yyyy");
        }


        private void print_reports_options_Shown(object sender, EventArgs e)
        {
            /* Set Default Start Date to One Month Ago */
            //input_startdate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);

            // Set Date Range Boxes
            input_startdate.Value = sDateBox.Value;
            input_enddate.Value = sDateBox.Value;


        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false; // Stop accidental double clicking 2011-08-08
            Application.DoEvents();

            /* OK Clicked, Do Your Magic :) */
            input_startdate.Value = input_startdate.Value.Date; // Set to Midnight 2011-08-08
            
            TimeSpan input_endtime = new TimeSpan(23, 59, 59);
            input_enddate.Value = input_enddate.Value.Date + input_endtime; // Set to 11:59:59 2011-08-08

            double start_unixtime = mysqlglobal.ConvertToUnixTimestamp(input_startdate.Value);
            double end_unixtime   = mysqlglobal.ConvertToUnixTimestamp(input_enddate.Value); // Since it's to 00:00 UTC

            /*
            Clipboard.SetText(start_unixtime.ToString());
            MessageBox.Show(@"Set Clipboard to Start Unixtime: " + start_unixtime);

            Clipboard.SetText(end_unixtime.ToString());
            MessageBox.Show(@"Set Clipboard to End Unixtime: " + end_unixtime);
            */

            string output = "printer";
            if (radioButtonScreen.Checked == true) output = "screen";

            string totals_shown = "";
            if (total_daily.Checked)
            {
                totals_shown = ", Daily";
            }
            if (total_monthly.Checked)
            {
                totals_shown += ", Monthly";
            }
            if (total_grand.Checked)
            {
                totals_shown += ", Grand";
            }

            totals_shown = totals_shown.Substring(2) + ".";

            string report_type = "";
            if (report_detailed.Checked) report_type = "Detailed.";
            if (report_summary.Checked) report_type = "Summary.";

            string consignor_code = consignorBox.Text;
            double upto_unixtime = mysqlglobal.ConvertToUnixTimestamp(expiryUpto.Value);


            m_parent.print_report_method(report_name, start_unixtime, end_unixtime, output, totals_shown, report_type, consignor_code, upto_unixtime);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sMonthBox_Click(object sender, EventArgs e)
        {
            sDateCheck.Checked = false;
            sMonthCheck.Checked = true;
            sMonthBox_SelectedValueChanged(null, null);
            total_monthly.Checked = true;
        }

        private void sDateCheck_Click(object sender, EventArgs e)
        {

        }

        private void sDateBox_ValueChanged(object sender, EventArgs e)
        {

            sDateCheck.Checked = true;
            sMonthCheck.Checked = false;
            total_monthly.Checked = false;
            // Set Date Range Boxes
            input_startdate.Value = sDateBox.Value;
            input_enddate.Value = sDateBox.Value;
        }

        private void sMonthBox_VisibleChanged(object sender, EventArgs e)
        {


        }

        private void sMonthBox_SelectedValueChanged(object sender, EventArgs e)
        {


            // Convert Selected Date into DateTime
            DateTime monthStart = DateTime.Parse(sMonthBox.Text);
            DateTime monthEnd = monthStart.AddMonths(1);
            monthEnd = monthEnd.AddDays(-1);
            // Set Date Range Boxes
            input_startdate.Value = monthStart;
            input_enddate.Value = monthEnd;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            select_vendor_or_customer select_vendor_var = new select_vendor_or_customer("vendor",true);
            select_vendor_var.ShowDialog();
            string consignor = select_vendor_var.return_var;
            consignorBox.Text = consignor;

        }


    }
}

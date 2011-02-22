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

        private void print_reports_options_Shown(object sender, EventArgs e)
        {
            /* Set Default Start Date to One Month Ago */
            input_startdate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.DoEvents();

            /* OK Clicked, Do Your Magic :) */
            double start_unixtime = mysqlglobal.ConvertToUnixTimestamp(input_startdate.Value);
            double end_unixtime   = mysqlglobal.ConvertToUnixTimestamp(input_enddate.Value);
            string output = "printer";
            if (radioButtonScreen.Checked == true) output = "screen";

            m_parent.print_report_method(report_name, start_unixtime, end_unixtime, output);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void print_reports_options_Load(object sender, EventArgs e)
        {

        }
    }
}

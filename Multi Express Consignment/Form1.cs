using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ini;
using System.IO;

using System.Data.OleDb;
using System.Xml.Serialization;
using System.Threading;

namespace Multi_Express_Consignment
{
    public partial class Form1 : Form
    {

        public Form consignment_purchase_desktop_form = null;
        public Form consignment_sale_desktop_form = null;
        public Form select_vendor = null;
        public Form select_customer = null;
        public Form print_reports_form = null;
        public Form item_search_form = null;

        public Form1()
        {
            InitializeComponent();
        }


        public void UnhandledThreadExceptionHandler(object sender, ThreadExceptionEventArgs e) {
            this.HandleUnhandledException(e.Exception);
        }

        public void HandleUnhandledException(Exception e) {
            error_dialog ed = new error_dialog(e);
            ed.ShowDialog(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void onConnect()
        {
            status.Text = "MySQL Connected.";
            // Connect DBF
            string dataSource = iniglobal.ini.IniReadValue("dbase", "path");
            string connectionVar = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataSource + ";Extended Properties=dBASE IV;User ID=Admin;Password=";
            dbfglobal.dbfCon = new OleDbConnection(connectionVar); // Establish Global Connection
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if(consignment_purchase_desktop_form == null || consignment_purchase_desktop_form.IsDisposed == true) {
                consignment_purchase_desktop_form = new consignment_purchase_desktop();
            }
            consignment_purchase_desktop_form.MdiParent = this;
            consignment_purchase_desktop_form.Show();
            consignment_purchase_desktop_form.Focus();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // Ini File
            string iniFileName = Directory.GetCurrentDirectory() + "/settings.ini";
            if (!File.Exists(iniFileName))
            {
                System.IO.File.WriteAllLines(iniFileName, new string[0]);
                // TODO: MySQL Config
            }
            iniglobal.ini = new IniFile(iniFileName);

            // Log Window
            logglobal.log = new program_log();
            logglobal.log.MdiParent = this;
            
            logglobal.log.Show();
            logglobal.log.Visible = false;


            // Establish MySQL Connection
            MysqlConnectForm mc = new MysqlConnectForm(this); // Pass MySQL Connect Form the handle of this Form.
            mc.ShowDialog(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Test Scaling
            float scaleX = ((float)Screen.PrimaryScreen.WorkingArea.Width / 1024); float scaleY = ((float)Screen.PrimaryScreen.WorkingArea.Height / 768); SizeF aSf = new SizeF(scaleX, scaleY); this.Scale(aSf);

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (select_vendor == null)
            {
                select_vendor = new select_vendor_or_customer("vendor");
            }
            else
            {
                select_vendor.Dispose();
                select_vendor = new select_vendor_or_customer("vendor");
            }
            select_vendor.MdiParent = this;
            select_vendor.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (select_vendor == null)
            {
                select_vendor = new select_vendor_or_customer("customer");
            }
            else
            {
                select_vendor.Dispose();
                select_vendor = new select_vendor_or_customer("customer");
            }
            select_vendor.MdiParent = this;
            select_vendor.Show();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (print_reports_form == null)
            {
                print_reports_form = new print_reports();
            } else {
                print_reports_form.Dispose();
                print_reports_form = new print_reports();
            }
            
            print_reports_form.MdiParent = this;
            print_reports_form.Show();
        }



        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            logglobal.log.Visible = true;
            logglobal.log.Show();
        }



        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (consignment_sale_desktop_form == null || consignment_sale_desktop_form.IsDisposed == true)
            {
                consignment_sale_desktop_form = new consignment_sale_desktop();
            }
            consignment_sale_desktop_form.MdiParent = this;
            consignment_sale_desktop_form.Show();
            consignment_sale_desktop_form.Focus();
        }


        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (item_search_form == null || item_search_form.IsDisposed == true)
            {
                item_search_form = new item_search(null,null,null,null,null);
            }
            item_search_form.MdiParent = this;
            item_search_form.Show();
        }



    }
}

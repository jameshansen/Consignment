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

namespace Multi_Express_Consignment
{
    public partial class Form1 : Form
    {

        public Form consignment_purchase_desktop_form = null;

        public Form1()
        {
            InitializeComponent();
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
            if(consignment_purchase_desktop_form == null) {
                consignment_purchase_desktop_form = new consignment_purchase_desktop();
            }
            consignment_purchase_desktop_form.MdiParent = this;
            consignment_purchase_desktop_form.Show();
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


            // Establish MySQL Connection
            MysqlConnectForm mc = new MysqlConnectForm(this); // Pass MySQL Connect Form the handle of this Form.
            mc.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Test Scaling
            float scaleX = ((float)Screen.PrimaryScreen.WorkingArea.Width / 1024); float scaleY = ((float)Screen.PrimaryScreen.WorkingArea.Height / 768); SizeF aSf = new SizeF(scaleX, scaleY); this.Scale(aSf);

        }



    }
}

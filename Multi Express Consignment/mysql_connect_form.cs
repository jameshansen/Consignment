using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using System.Data.OleDb;
using System.Xml.Serialization;

using System.IO;
using System.Diagnostics;
using Ini;


namespace Multi_Express_Consignment
{
    public partial class MysqlConnectForm : Form
    {

        private Form1 m_parent;
        private int isConnected = 0;
        private int countdown;

        public MysqlConnectForm(Form1 frm1)
        {
            InitializeComponent();
            m_parent = frm1;
            
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }


        private void MysqlConnectForm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Check if Connected
            if (isConnected == 1)
            {
                /* Now Connected, Close Self and Notify Form 1 */
                timer1.Enabled = false;
                this.Close(); // Close Self
                if (m_parent != null) m_parent.onConnect(); // Trigger on Connect Routine                              
                this.Dispose();
            }

            // 2 = error
            if (isConnected == 2)
            {
                // Countdown
                countdown = countdown - 1;               
                label1.Text = "Connection Failure... Retrying in " + Convert.ToString(countdown) + " seconds...";

                if (countdown == 0)
                {
                    isConnected = 0;
                    label1.Text = "Establishing Connection to Database...";
                    backgroundWorker1.RunWorkerAsync(); // Retry Connection
                }
            }

            
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // MySQL Connect
            /* Load Ini File Settings */
            string mysqlServer = iniglobal.ini.IniReadValue("mysql", "server");
            string mysqlDatabase = iniglobal.ini.IniReadValue("mysql", "database");
            string mysqlUser = iniglobal.ini.IniReadValue("mysql", "user");
            string mysqlPassword = iniglobal.ini.IniReadValue("mysql", "password");

            /* Connect to MySQL database */
            string strProvider = "Data Source=" + mysqlServer + ";Database=" + mysqlDatabase + ";User ID=" + mysqlUser + ";Password=" + mysqlPassword;
            try
            {
                mysqlglobal.mysqlCon = new MySqlConnection(strProvider);
                mysqlglobal.mysqlCon.Open();
                isConnected = 1;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                label1.Text = "Connection Failure... Retrying in 5 seconds...";
                countdown = 5; // Retry in five seconds
                isConnected = 2;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MysqlConnectForm_Shown(object sender, EventArgs e)
        {

        }
    }
}

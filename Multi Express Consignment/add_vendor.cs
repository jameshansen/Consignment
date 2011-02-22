using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using System.Data.OleDb;
using System.Xml.Serialization;

namespace Multi_Express_Consignment
{
    public partial class add_vendor : Form
    {
        select_vendor_or_customer m_parent = null;
        string m_mode = "";
        public string new_entry_val = "cancelled";


        public string new_entry
        {
            get
            {
                return new_entry_val;
            }
        }

        public add_vendor(select_vendor_or_customer parent, string mode)
        {
            InitializeComponent();
            m_parent = parent;
            m_mode = mode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            fillVendorCode();

            // Get Fields
            string CMCUCODE = mysqlglobal.escapeString(input_CMCUCODE.Text);
            string CMCUNAME = mysqlglobal.escapeString(input_CMCUNAME.Text);
            string CMNAMESUR = mysqlglobal.escapeString(input_CMNAMESUR.Text);
            string CMNAME1ST = mysqlglobal.escapeString(input_CMNAME1ST.Text);
            string CMPHONE = mysqlglobal.escapeString(input_CMPHONE_a.Text).PadRight(4) + mysqlglobal.escapeString(input_CMPHONE_b.Text);
            

            string errorMsg = "";
            if (CMCUCODE == "") errorMsg += "Vendor Code cannot be blank.\n";

            if (errorMsg != "")
            {
                errorMsg = "Please fill out all the required fields:\n" + errorMsg;
                MessageBox.Show(errorMsg, "Cannot Add Vendor");
                return;
            }


            // Insert Vendor or Customer
            string dbf = "";
            if (m_mode == "vendor") dbf = "PSVEMAST";
            if (m_mode == "customer") dbf = "SFCUMAST";
            string query = "INSERT INTO " + dbf + " (CMCUCODE, CMCUNAME, CMNAMESUR, CMNAME1ST, CMPHONE) VALUES (\"" + CMCUCODE + "\", \"" + CMCUNAME + "\", \"" + CMNAMESUR + "\", \"" + CMNAME1ST + "\", \"" + CMPHONE + "\")";
            mysqlglobal.executeNonQuery(query, this);

            new_entry_val = input_CMCUCODE.Text;

            m_parent.loadList();
            m_parent.gotoList(input_CMCUCODE.Text);
            this.Close();
        }

        public void fillVendorCode()
        {
            if (input_CMCUCODE.Text == "" && input_CMCUNAME.Text != "")
            {
                // Generate Code based on Company Name
                Random random = new Random();
                input_CMCUCODE.Text = input_CMCUNAME.Text.ToUpper().Substring(0, 3) + "-" + Convert.ToString(random.Next(1000, 9999));
            }

            if (input_CMCUCODE.Text == "" && input_CMCUNAME.Text == "" && input_CMNAMESUR.Text != "")
            {
                // Generate Code based on First + Surname
                Random random = new Random();
                input_CMCUCODE.Text = input_CMNAME1ST.Text.ToUpper().Substring(0, Math.Min(3, input_CMNAME1ST.Text.Length)) + "-" + input_CMNAMESUR.Text.ToUpper().Substring(0, Math.Min(3, input_CMNAMESUR.Text.Length)) + "-" + Convert.ToString(random.Next(1000, 9999));
            }
        }

        private void input_CMCUCODE_Enter(object sender, EventArgs e)
        {
            fillVendorCode();
        }

        private void add_vendor_Load(object sender, EventArgs e)
        {
            // Set Window Location (Doesn't work)
            //this.Top = m_parent.Top;
            //this.Left = m_parent.Left + m_parent.Width + 10;
        }





        private void input_CMNAME1ST_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_CMNAMESUR.Focus();
            }

        }

        private void input_CMNAMESUR_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_CMPHONE_a.Focus();
            }

        }

        private void input_CMPHONE_a_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_CMPHONE_b.Focus();
            }

        }

        private void input_CMPHONE_b_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_CMCUNAME.Focus();
            }

        }

        private void input_CMCUNAME_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                input_CMCUCODE.Focus();
            }

        }

        private void input_CMCUCODE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2_Click(null, null);  
            }

        }

        private void input_CMCUCODE_TextChanged(object sender, EventArgs e)
        {

        }






    }
}

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
    public partial class change_status_cpo : Form
    {
        public static Form m_parent = null;
        public static string currentStatus;
        public static string newStatus;

        public change_status_cpo(Form calledBy, string status)
        {
            InitializeComponent();
            m_parent = calledBy;
            currentStatus = status;
            newStatus = status;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void change_status_cpo_Load(object sender, EventArgs e)
        {
            radio_systemdef1.Text = iniglobal.ini.IniReadValue("company", "status1");
            radio_systemdef2.Text = iniglobal.ini.IniReadValue("company", "status2");



            this.Top = m_parent.Top;
            this.Left = m_parent.Left + m_parent.Width + 10;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (m_parent.Name == "consignment_purchase_order")
            {
                ((consignment_purchase_order)m_parent).setConsignmentStatus(newStatus);
            }
            if (m_parent.Name == "consignment_sale_order")
            {
                ((consignment_sale_order)m_parent).setOrderStatus(newStatus);
            }
          this.Dispose();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void checkedChanged(object sender, EventArgs e)
        {
            newStatus = ((RadioButton)sender).Text;
        }

        private void change_status_cpo_Shown(object sender, EventArgs e)
        {
            foreach (Control c in groupBox1.Controls)
            {
                if (c is RadioButton)
                {
                    try
                    {
                        RadioButton rb = (c as RadioButton);
                        //MessageBox.Show("'" + rb.Text + "' = '" + currentStatus + "'");
                        if (rb.Text.ToLower() == currentStatus.ToLower())
                        {
                            rb.Checked = true;
                            break;
                        }
                    }
                    catch
                    {
                        // Skip
                    }
                }
            }
        }

        
    }
}

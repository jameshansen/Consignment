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
    public partial class error_dialog : Form
    {
        public Exception global_e;

        public error_dialog(Exception e)
        {
            InitializeComponent();
            global_e = e;
        }

        private void error_dialog_Load(object sender, EventArgs e)
        {
            windowglobal.centre(this); // (2026)

            errorMsg.Text = global_e.Message;
            errorMsg.Text += Environment.NewLine;
            errorMsg.Text += Environment.NewLine;
            errorMsg.Text += global_e.StackTrace;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

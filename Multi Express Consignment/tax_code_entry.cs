using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Multi_Express_Consignment
{
    /* Add or edit one tax code */
    public partial class tax_code_entry : Form
    {
        private tax_codes m_parent = null;
        private int m_rowIndex = -1;

        public tax_code_entry(tax_codes calledBy, int rowIndex)
        {
            InitializeComponent();
            m_parent = calledBy;
            m_rowIndex = rowIndex;
        }

        private void tax_code_entry_Load(object sender, EventArgs e)
        {
            windowglobal.centre(this); // (2026)
        }

        private void tax_code_entry_Shown(object sender, EventArgs e)
        {
            loadIcons();

            if (m_rowIndex >= 0)
            {
                this.Text = "Edit Tax Code";
                button2.Text = "Save Changes";

                input_code.Text = m_parent.cell(m_rowIndex, m_parent.tg_code);
                input_desc.Text = m_parent.cell(m_rowIndex, m_parent.tg_desc);
                input_rate.Text = m_parent.cell(m_rowIndex, m_parent.tg_rate);
                input_icon.Text = m_parent.cell(m_rowIndex, m_parent.tg_icon);
            }

            input_code.Focus();
            input_code.SelectAll();
        }

        /* The bitmaps sitting in the icon folder, so the name does not have to be typed */
        private void loadIcons()
        {
            input_icon.Items.Clear();
            input_icon.Items.Add("");

            try
            {
                string folder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), taxglobal.iconFolder);
                foreach (string file in Directory.GetFiles(folder, "*.BMP"))
                {
                    input_icon.Items.Add(Path.GetFileNameWithoutExtension(file).ToUpper());
                }
            }
            catch
            {
                /* No icon folder, the name can still be typed */
            }
        }

        private void input_icon_TextChanged(object sender, EventArgs e)
        {
            preview.Image = taxglobal.iconFile(input_icon.Text);
        }

        /* OK (button2) */
        private void button2_Click(object sender, EventArgs e)
        {
            string code = input_code.Text.Trim().ToUpper();
            string rate = input_rate.Text.Trim();
            decimal rate_value = 0;

            string error_list = "";

            if (code == "")
            {
                error_list += "* Tax code cannot be blank" + Environment.NewLine;
            }

            if (code.Length > 2)
            {
                error_list += "* Tax code can only be one or two characters" + Environment.NewLine;
            }

            if (code != "" && m_parent.hasCode(code, m_rowIndex))
            {
                error_list += "* Tax code " + code + " is already in the list" + Environment.NewLine;
            }

            if (decimal.TryParse(rate, out rate_value) == false || rate_value < 0 || rate_value > 100)
            {
                error_list += "* Rate must be a percentage between 0 and 100" + Environment.NewLine;
            }

            if (error_list != "")
            {
                MessageBox.Show("The following errors were found:" + Environment.NewLine + error_list, "Alert");
                return;
            }

            m_parent.addTaxCode(code, input_desc.Text.Trim(), rate_value.ToString("#0.0000"), input_icon.Text.Trim().ToUpper(), m_rowIndex);
            this.Close();
        }

        /* Cancel (button1) */
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

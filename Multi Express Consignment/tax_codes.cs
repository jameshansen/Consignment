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
    /* Tax codes, rates and icons */
    public partial class tax_codes : Form
    {
        public tax_codes()
        {
            InitializeComponent();
        }

        private void tax_codes_Shown(object sender, EventArgs e)
        {
            loadCodes();
        }

        public void loadCodes()
        {
            dataGridView1.Rows.Clear();

            DataSet tax_file = mysqlglobal.executeDataSetQuery("SELECT * FROM `" + taxglobal.table + "` ORDER BY `tax_code`", taxglobal.table, this);

            foreach (DataRow row in tax_file.Tables[taxglobal.table].Rows)
            {
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);

                outputRow.Cells[tg_code.Index].Value = Convert.ToString(row["tax_code"]).Trim();
                outputRow.Cells[tg_desc.Index].Value = Convert.ToString(row["tax_desc"]).Trim();
                outputRow.Cells[tg_rate.Index].Value = Convert.ToDecimal(row["tax_rate"]).ToString("#0.0000");
                outputRow.Cells[tg_icon.Index].Value = mysqlglobal.field(row, "tax_icon", "").Trim();
                outputRow.Cells[tg_preview.Index].Value = taxglobal.iconFile(outputRow.Cells[tg_icon.Index].Value);
                outputRow.Cells[tg_saved_code.Index].Value = Convert.ToString(row["tax_code"]).Trim(); // To find the row again if the code is edited

                dataGridView1.Rows.Add(outputRow);
            }

            defaultLabel.Text = "Items with no code of their own are taxed at the default code: " + taxglobal.defaultCode();
            rowSelectionChanged(null, null);
        }

        public string cell(int rowIndex, DataGridViewColumn column)
        {
            return Convert.ToString(dataGridView1.Rows[rowIndex].Cells[column.Index].Value).Trim();
        }

        /* Is this code already in the list, ignoring the row being edited */
        public bool hasCode(string code, int exceptRow)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (i == exceptRow) continue;
                if (cell(i, tg_code).ToUpper() == code.ToUpper()) return true;
            }
            return false;
        }

        /* Called back by the entry window. rowIndex of -1 adds a row. */
        public void addTaxCode(string code, string description, string rate, string icon, int rowIndex)
        {
            if (rowIndex == -1)
            {
                DataGridViewRow outputRow = new DataGridViewRow();
                outputRow.CreateCells(dataGridView1);
                rowIndex = dataGridView1.Rows.Add(outputRow);
            }

            dataGridView1.Rows[rowIndex].Cells[tg_code.Index].Value = code;
            dataGridView1.Rows[rowIndex].Cells[tg_desc.Index].Value = description;
            dataGridView1.Rows[rowIndex].Cells[tg_rate.Index].Value = rate;
            dataGridView1.Rows[rowIndex].Cells[tg_icon.Index].Value = icon;
            dataGridView1.Rows[rowIndex].Cells[tg_preview.Index].Value = taxglobal.iconFile(icon);
        }

        /* Add (button4) */
        private void button4_Click(object sender, EventArgs e)
        {
            tax_code_entry entry = new tax_code_entry(this, -1);
            entry.ShowDialog(this);
        }

        /* Edit (button5) */
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select the tax code you wish to edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tax_code_entry entry = new tax_code_entry(this, dataGridView1.SelectedRows[0].Index);
            entry.ShowDialog(this);
        }

        /* Save (button1) */
        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string code = cell(i, tg_code).ToUpper();
                string saved_code = cell(i, tg_saved_code);

                // The code is the key, so a renamed code leaves its old row behind
                if (saved_code != "" && saved_code != code)
                {
                    mysqlglobal.executeNonQuery("DELETE FROM `" + taxglobal.table + "` WHERE `tax_code` = '" + mysqlglobal.escapeString(saved_code) + "'", this);
                }

                mysqlglobal.executeNonQuery(
                    "REPLACE INTO `" + taxglobal.table + "` (`tax_code`, `tax_desc`, `tax_rate`, `tax_icon`) VALUES ('"
                    + mysqlglobal.escapeString(code) + "', '"
                    + mysqlglobal.escapeString(cell(i, tg_desc)) + "', '"
                    + cell(i, tg_rate) + "', '"
                    + mysqlglobal.escapeString(cell(i, tg_icon)) + "')", this);
            }

            taxglobal.reload();
            loadCodes();

            MessageBox.Show("Tax codes saved.", "Saved");
        }

        /* Delete (button2) */
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("You must select the tax code you wish to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string code = Convert.ToString(dataGridView1.SelectedRows[0].Cells[tg_saved_code.Index].Value).Trim();
            string in_use = "";

            if (code != "")
            {
                string items = Convert.ToString(mysqlglobal.executeScalarQuery("SELECT COUNT(*) FROM `CSTITEM` WHERE `tax_code` = '" + mysqlglobal.escapeString(code) + "'", this));
                if (items != "" && items != "0")
                {
                    in_use = Environment.NewLine + Environment.NewLine + items + " item(s) use this code and will fall back to the default code " + taxglobal.defaultCode() + ".";
                }
            }

            if (MessageBox.Show("Are you sure you want to delete tax code '" + cell(dataGridView1.SelectedRows[0].Index, tg_code) + "'?" + in_use, "Delete Tax Code", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }

            if (code != "")
            {
                mysqlglobal.executeNonQuery("DELETE FROM `" + taxglobal.table + "` WHERE `tax_code` = '" + mysqlglobal.escapeString(code) + "'", this);
                taxglobal.reload();
            }

            dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);
            rowSelectionChanged(null, null);
        }

        /* Close (button3) */
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rowSelectionChanged(object sender, EventArgs e)
        {
            bool rowSelected = dataGridView1.SelectedRows.Count > 0;

            button5.Enabled = rowSelected; // Edit
            button2.Enabled = rowSelected; // Delete
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button5_Click(null, null);
        }
    }
}

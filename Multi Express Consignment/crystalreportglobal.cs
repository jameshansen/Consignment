using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CrystalDecisions;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Multi_Express_Consignment
{
    public partial class crystalreportglobal : Form
    {
        public crystalreportglobal()
        {
            InitializeComponent();
        }

        public static void SetFormulaFieldString(ReportDocument report, String fieldName, String fieldValue)
        {                       
            /* String Escaping */
            string doublequote = Convert.ToString((char)34);
            string doublequotedoublequote = Convert.ToString((char)34) + Convert.ToString((char)34);
            fieldValue = fieldValue.Replace(doublequote, doublequotedoublequote);

            /* New Lines */
            fieldValue = fieldValue.Replace(Environment.NewLine, "\" + chr(13) + \""); 

            /* Set Formula Field */
            try
            {
                report.DataDefinition.FormulaFields[fieldName].Text = "\"" + fieldValue + "\"";
            }
            catch (Exception e)
            {
                if (e.Message.Substring(0, 14) == "Invalid index.")
                {
                    MessageBox.Show(null, "Formula field \"" + fieldName + "\" not found.", "Field not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /* Sets a formula to raw Crystal syntax, unquoted so it can do arithmetic. Subreport name optional. (2026) */
        public static void SetFormulaFieldText(ReportDocument report, String subreportName, String fieldName, String formula)
        {
            try
            {
                ReportDocument target = (subreportName == null) ? report : report.Subreports[subreportName];
                target.DataDefinition.FormulaFields[fieldName].Text = formula;
            }
            catch
            {
                /* Report does not have that formula, leave it as the report author wrote it */
            }
        }

        /* Rewrites one of the report's own captions, by its designer name (2026) */
        public static void SetTextObject(ReportDocument report, String objectName, String text)
        {
            foreach (Section section in report.ReportDefinition.Sections)
            {
                foreach (ReportObject item in section.ReportObjects)
                {
                    if (item.Name == objectName && item.Kind == ReportObjectKind.TextObject)
                    {
                        ((TextObject)item).Text = text;
                        return;
                    }
                }
            }
        }

        private void crystalreportglobal_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            timer1.Enabled = true; // Start Timer. This will wait 500ms after report is shown to disable Always on top. This fixes issues with ShowDialog messing up Z-Order.
        }

        private void crystalreportglobal_SizeChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Workaround for ShowDialog Z-Order issues
            this.TopMost = false;

            timer1.Enabled = false;
        }
    }
}

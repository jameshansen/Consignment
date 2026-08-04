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

using CrystalDecisions;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;


using System.Data.OleDb;
using System.Xml.Serialization;
using System.IO;

namespace Multi_Express_Consignment
{
    public partial class print_report : Form
    {

        /* Global Vars */

        public string consignment_code = null;
        public string order_number = null;
        public string item_number = null;
        public string report_code = null;
        public string vendor_code = null;
        public string customer_code = null;

        public DataRow vendor_detail_row = null;
        public DataRow customer_detail_row = null;
        public DataSet consignment_db = new consignment_db(); // Use DataSet XML Template (DataSet1.xsd)
        public DataRow order_header_row = null;

        /* Init Output Window */
        public static crystalreportglobal rptwin = new crystalreportglobal();
        public static ReportDocument cryRpt = new ReportDocument();


        public print_report(string thisConsignment, string thisOrder, string thisItem, string reportCode)
        {
            InitializeComponent();
            consignment_code = thisConsignment;
            order_number = thisOrder;
            item_number = thisItem;
            report_code = reportCode;
        }

        private void print_report_Load(object sender, EventArgs e)
        {
            /* Set Current Working Folder to Application Path */
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            // JH: 2011-11-16 Clear the DataSet (BUG 3)
            consignment_db.Clear();

            /* Item Data */
            if (consignment_code != null)
            {
                /* Load Item Data based on Consignment */
                string strSQL = "SELECT * FROM `CSTITEM` WHERE `consignment_code` = \"" + consignment_code + "\" ORDER BY `upc`";
                consignment_db = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this); // Fill Consignment_DB CSTITEM File

                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `consignment_code` = \"" + consignment_code + "\" ORDER BY `date`";
                mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db); // Fill Consignment_DB CSTPAYMENT File

                /* Load Vendor Information */
                try
                {
                    vendor_code = Convert.ToString(consignment_db.Tables["CSTITEM"].Rows[0]["vendor_code"]);
                }
                catch
                {
                    MessageBox.Show("No Items Found in Order", "No Items Found");
                    this.Close();
                    return;
                }
                string query = "SELECT * FROM PSVEMAST WHERE CMCUCODE = \"" + vendor_code + "\"";
                DataSet vendor_data = mysqlglobal.executeDataSetQuery(query, "PSVEMAST", this);

                /* Put Vendor Data into Global DataRow */
                // Fix for missing record bug (2026): A deleted vendor prints blank details rather than crashing
                vendor_detail_row = vendor_data.Tables["PSVEMAST"].Rows.Count > 0 ? vendor_data.Tables["PSVEMAST"].Rows[0] : vendor_data.Tables["PSVEMAST"].NewRow();

            }

            if (order_number != null)
            {
                /* Load Order Header Row */
                string strOH = "SELECT * FROM `CSTORDER` WHERE `order_number` = \"" + order_number + "\"";
                DataTable order_header_table = mysqlglobal.executeDataSetQuery(strOH, "CSTORDER", this).Tables["CSTORDER"];

                if (order_header_table.Rows.Count == 0) // Fix for missing record bug (2026): Nothing to report on
                {
                    MessageBox.Show("Order #" + order_number + " could not be found.", "Order Not Found");
                    this.Close();
                    return;
                }

                order_header_row = order_header_table.Rows[0];

                /* Load Item Data based on Sale */
                string strSQL = "SELECT * FROM `CSTITEM` WHERE `order_number` = \"" + order_number + "\" ORDER BY `upc`";
                mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this, consignment_db); // Fill Consignment_DB CSTITEM File

                strSQL = "SELECT * FROM `CSTPAYMENT` WHERE `order_number` = \"" + order_number + "\" ORDER BY `date`";
                mysqlglobal.executeDataSetQuery(strSQL, "CSTPAYMENT", this, consignment_db); // Fill Consignment_DB CSTPAYMENT File

                /* Load Customer Information */
                MySqlCommand mysqlCmd = new MySqlCommand("SELECT `customer_code` FROM `CSTORDER` WHERE `order_number` = \"" + order_number + "\"",  mysqlglobal.mysqlCon);
            
                try
                {
                    customer_code = mysqlCmd.ExecuteScalar().ToString();
                }
                catch
                {
                    customer_code = "";
                }


                string query = "SELECT * FROM SFCUMAST WHERE CMCUCODE = \"" + customer_code + "\"";

                DataSet customer_data = mysqlglobal.executeDataSetQuery(query, "SFCUMAST", this);

                /* Put Customer Data into Global DataRow */
                // Fix for missing record bug (2026): A deleted customer prints blank details rather than crashing
                customer_detail_row = customer_data.Tables["SFCUMAST"].Rows.Count > 0 ? customer_data.Tables["SFCUMAST"].Rows[0] : customer_data.Tables["SFCUMAST"].NewRow();
            }         
   
            /* Item Data for Selected Items */
            if (item_number != null)
            {
                string itemQ = item_number.Replace(",", "\" OR `upc` = \""); // Multi Select

                /* Load Item Data based on Consignment */
                string strSQL = "SELECT * FROM `CSTITEM` WHERE `upc` = \"" + itemQ + "\"";

                consignment_db = mysqlglobal.executeDataSetQuery(strSQL, "CSTITEM", this); // Fill Consignment_DB CSTITEM File
            }

            /* Default Printer */
            //default_printer.Text = rptwin.viewer.Prin

        }
        
        private void print_report_Shown(object sender, EventArgs e)
        {
            bool foundFirst = false;

            /* Filter Form_List */

            // JH: New Method, Load All Into Dictionary, Delete All, Filter, Write Out from Dictionary
            Dictionary<string, bool> reportList = new Dictionary<string, bool>();
            
            string key = "";
            bool show = true;
            for (int i = 0; i < form_list.Items.Count; i++)
            {
                key = form_list.Items[i].ToString();
                show = true;

                if (consignment_code != null)
                {
                    // Remove All that Don't Start with C-
                    if (key.Substring(0, 2) != "C-")
                    {
                        show = false; // false = Do not reinsert this record
                    }
                }
            
                if (order_number != null)
                {
                    // Remove All that Don't Start with S-
                    if (key.Substring(0, 2) != "S-")
                    {
                        show = false; // false = Do not reinsert this record
                    }
                }
                
                if (item_number != null)
                {
                    // Remove All that Don't Start with I-
                    if (key.Substring(0, 2) != "I-")
                    {
                        show = false; // false = Do not reinsert this record
                    }
                }

                reportList.Add(key, show);

            }

            form_list.Items.Clear();
      
            /* Remove Prefixes and Insert Qualified Records into List */
            foreach (var pair in reportList)
            {
                if(pair.Value) {
                    form_list.Items.Add(pair.Key.Substring(2));
                }
            }

            /* Select Report in List From Report Code */            
            try
            {
                if (report_code != null)
                {
                    form_list.SelectedIndex = form_list.FindString(report_code);
                }
            }
            catch
            {
                // Do nothing.
            }

        }

        private void processPayments()
        {
            // Process payments on CSTITEM database
            for (int a = 0; a < consignment_db.Tables["CSTITEM"].Rows.Count; a++)
            {
                consignment_db.Tables["CSTITEM"].Rows[a]["date_paid"] = fetchPaymentDate(consignment_db.Tables["CSTITEM"].Rows[a]["date_paid"]);
            }

        }

        public Int64 fetchPaymentDate(object id)
        {
            Int64 output = 0;

            output = Convert.ToInt64(mysqlglobal.executeScalarQuery("SELECT `date` FROM `CSTPAYMENT` WHERE `id` = " + id.ToString(), this));

            return output;
        }



        private void standard_headers()
        {
            /* Load Header Details from INI File [company] section */
            string companyName = iniglobal.ini.IniReadValue("company", "name");
            string branch1Address = iniglobal.ini.IniReadValue("company", "branch1Address");
            string branch1Telephone = iniglobal.ini.IniReadValue("company", "branch1Telephone");

            string dateFormat = iniglobal.ini.IniReadValue("company", "dateFormat");

            /* Input Header Details into Report */
            crystalreportglobal.SetFormulaFieldString(cryRpt, "companyName", companyName);           // Company Name
            crystalreportglobal.SetFormulaFieldString(cryRpt, "branch1Address", branch1Address);     // Branch 1 Address
            crystalreportglobal.SetFormulaFieldString(cryRpt, "branch1Telephone", branch1Telephone); // Branch 1 Telephone

            /* Date Format */
            crystalreportglobal.SetFormulaFieldString(cryRpt, "dateFormat", dateFormat); // Branch 1 Telephone
        }

        private void process(object sender, EventArgs e)
        {
            report_code = form_list.Text;

            /* Process Payment Dates */
            processPayments();

            /* Recreate Report Stuff */
            cryRpt.Dispose();
            rptwin.Dispose();

            rptwin = new crystalreportglobal();
            cryRpt = new ReportDocument();

            /* CloseThis Window */
            this.Close();
            Application.DoEvents();

            /* OK Button Clicked, compile report */

            // 1 - CONSIGNMENT AGREEMENT
            if (report_code == "Consignment Agreement")
            {
                cryRpt.Load("report_consignment_agreement.rpt");

                /* Standard Headers */
                standard_headers();

                /* Additional Headers */

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_code", vendor_code);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_name", vendor_detail_row["CMNAME1ST"].ToString() + " " + vendor_detail_row["CMNAMESUR"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_company", vendor_detail_row["CMCUNAME"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_address", vendor_detail_row["CMADD1"].ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "consignment_code", consignment_code);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_phone", vendor_detail_row["CMPHONE"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_fax", vendor_detail_row["CMFAX1"].ToString());


                /* Consignment Text */
                //string consignment_agreement_text = System.IO.File.ReadAllText("consignment_agreement_text.txt");

                //crystalreportglobal.SetFormulaFieldString(cryRpt, "consignment_agreement_text", consignment_agreement_text);

                /* Connect Report to DataSet */
                cryRpt.SetDataSource(consignment_db);

                rptwin.viewer.ReportSource = cryRpt;
                rptwin.viewer.Refresh();
            }

            bool labels = false;

            // 2 - BARCODE LABELS
            if (report_code == "Consignment Barcode Item Labels")
            {
                cryRpt.Load("report_barcode_item_labels.rpt");

                /* Specify Form Size */
                /* 3.5 x 1.0 */
                //System.Drawing.Printing.PaperSize paperLabel = new System.Drawing.Printing.PaperSize("3.5 x 1.0", 350, 100);
                /* Connect Report to DataSet */
                cryRpt.SetDataSource(consignment_db);
                rptwin.viewer.ReportSource = cryRpt;
                rptwin.viewer.Refresh();

                labels = true;

                //cryRpt.PrintOptions.PaperSize = PaperSize.PaperLegal;
                
            }

            // 3 - CONSIGNMENT INVOICE
            if (report_code == "Consignment Invoice")
            {
                cryRpt.Load("report_consignment_invoice.rpt");

                /* Standard Headers */
                standard_headers();

                /* Additional Headers */

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_code", vendor_code);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_name", vendor_detail_row["CMNAME1ST"].ToString() + " " + vendor_detail_row["CMNAMESUR"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_company", vendor_detail_row["CMCUNAME"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_address", vendor_detail_row["CMADD1"].ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "consignment_code", consignment_code);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_phone", vendor_detail_row["CMPHONE"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "vendor_fax", vendor_detail_row["CMFAX1"].ToString());

                /* Fetch Payment for Payment Totals on Report */

                double totalPaid = 0;
                double totalPaidToday = 0;

                //MessageBox.Show(@"Rows in CSTPAYMENT: " + consignment_db.Tables["cstpayment"].Rows.Count);

                /*
                 *             input_startdate.Value = input_startdate.Value.Date; // Set to Midnight 2011-08-08
            
            TimeSpan input_endtime = new TimeSpan(23, 59, 59);
            input_enddate.Value = input_enddate.Value.Date + input_endtime; // Set to 11:59:59 2011-08-08

            double start_unixtime = mysqlglobal.ConvertToUnixTimestamp(input_startdate.Value);
            double end_unixtime   = mysqlglobal.ConvertToUnixTimestamp(input_enddate.Value); // Since it's to 00:00 UTC
                 */

                foreach (DataRow a in consignment_db.Tables["CSTPAYMENT"].Rows)
                {
                    totalPaid += Convert.ToDouble(a["amount"]);
                    DateTime paydate = mysqlglobal.ConvertFromUnixTimestamp(Convert.ToDouble(a["date"]));
                    paydate = paydate.Date; // Set to date

                    if (paydate == DateTime.Now.Date)
                    {
                        // Same date.
                        totalPaidToday += Convert.ToDouble(a["amount"]);
                    }
                }
                crystalreportglobal.SetFormulaFieldString(cryRpt, "paid_today", cg.price(totalPaidToday));
                

                crystalreportglobal.SetFormulaFieldString(cryRpt, "paid_amt", cg.price(totalPaid));

                /* Consignment Text */
                //string consignment_agreement_text = System.IO.File.ReadAllText("consignment_agreement_text.txt");

                //crystalreportglobal.SetFormulaFieldString(cryRpt, "consignment_agreement_text", consignment_agreement_text);

                /* Connect Report to DataSet */
                cryRpt.SetDataSource(consignment_db);
                rptwin.viewer.ReportSource = cryRpt;
                rptwin.viewer.Refresh();

            }

            // 4 - ORDER RECEIPT && 5 - ORDER INVOICE
            if (report_code == "Sales Order" || report_code == "Sales Receipt")
            {
                cryRpt.Load("report_sale_receipt.rpt");

                /* Standard Headers */
                standard_headers();

                /* Form Name */
                crystalreportglobal.SetFormulaFieldString(cryRpt, "form_name", report_code);

                /* Additional Headers */

                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_code", customer_code);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_name", customer_detail_row["CMNAME1ST"].ToString() + " " + customer_detail_row["CMNAMESUR"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_company", customer_detail_row["CMCUNAME"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_address", customer_detail_row["CMADD1"].ToString());

                crystalreportglobal.SetFormulaFieldString(cryRpt, "order_number", order_number);

                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_phone", customer_detail_row["CMPHONE"].ToString());
                crystalreportglobal.SetFormulaFieldString(cryRpt, "customer_fax", customer_detail_row["CMFAX1"].ToString());

                double order_date = Convert.ToDouble(order_header_row["date_order"]);
                crystalreportglobal.SetFormulaFieldString(cryRpt, "order_date", mysqlglobal.formatDate(mysqlglobal.ConvertFromUnixTimestamp(order_date)));

                crystalreportglobal.SetFormulaFieldString(cryRpt, "order_header_total", cg.price(order_header_row["total"]));

                /* Connect Report to DataSet */
                cryRpt.SetDataSource(consignment_db);
                rptwin.viewer.ReportSource = cryRpt;
                rptwin.viewer.Refresh();
            }

            // 6 - Single Barcode Label
            if (report_code == "Print Barcode Item Label(s) for Selected Item(s)")
            {
                cryRpt.Load("report_barcode_item_labels.rpt");

                /* Specify Form Size */
                /* 3.5 x 1.0 */
                //System.Drawing.Printing.PaperSize paperLabel = new System.Drawing.Printing.PaperSize("3.5 x 1.0", 350, 100);
                /* Connect Report to DataSet */
                cryRpt.SetDataSource(consignment_db);
                rptwin.viewer.ReportSource = cryRpt;
                rptwin.viewer.Refresh();

                labels = true;
            }


            /* Final Step */           
            if (((Control)sender).Name == "b_screen")
            {
                if (labels == true)
                {
                    MessageBox.Show("If you decide to print labels from the Report Viewer, please be advised that the paper size may be incorrect and will require correction before printing.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                rptwin.Show();

                rptwin.TopMost = true; // Always on top.
            }

            if (((Control)sender).Name == "b_printer")
            {
               // rptwin.viewer.PrintReport();
                this.printDialog1.Document = this.printDocument1;

                if (labels == true) // Set default to Label Printer
                {
                    this.printDocument1.PrinterSettings.PrinterName = iniglobal.ini.IniReadValue("printers", "labelPrinter"); // Read printer from Ini File
                }

                DialogResult dr = this.printDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    //Get the Copy times

                    int nCopy = this.printDocument1.PrinterSettings.Copies;
                    //Get the number of Start Page

                    int sPage = this.printDocument1.PrinterSettings.FromPage;
                    //Get the number of End Page

                    int ePage = this.printDocument1.PrinterSettings.ToPage;
                    //Get the printer name

                    string PrinterName = this.printDocument1.PrinterSettings.PrinterName;


                    try
                    {
                        //Set the printer name to print the report to. By default the sample

                        //report does not have a defult printer specified. This will tell the

                        //engine to use the specified printer to print the report. Print out 

                        //a test page (from Printer properties) to get the correct value.

                        if (labels == false)
                        {
                            cryRpt.PrintOptions.PrinterName = PrinterName;
                        }
                        else
                        {
                            cryRpt.PrintOptions.PrinterName = PrinterName;
                            cryRpt.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;                            
                            cryRpt.PrintOptions.PaperSize = PaperSize.DefaultPaperSize;
                        }


                        //Start the printing process. Provide details of the print job

                        //using the arguments.

                        cryRpt.PrintToPrinter(nCopy, false, sPage, ePage);

                        //Let the user know that the print job is completed

                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.ToString());
                    }
                } 


            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }


    }
}

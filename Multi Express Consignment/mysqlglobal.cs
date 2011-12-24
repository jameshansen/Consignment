using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using System.Data.OleDb;
using System.IO;

using Ini;

namespace Multi_Express_Consignment
{
    public class mysqlglobal
    {
        public static MySqlConnection mysqlCon = null;

        public static void checkConnection(Form freezeForm)
        {
            /* Connection Check */
            bool connected = false;
            try
            {
                connected = mysqlglobal.mysqlCon.Ping();
            }
            catch
            {
                connected = false;
            }
            if (connected == false)
            {

                // MySQL Connection Drop
                logglobal.log.logListbox.Items.Insert(0, "-------MySQL Connection Drop at " + DateTime.Now + "------");

                // Establish MySQL Connection
                MysqlConnectForm mc = new MysqlConnectForm(null); // Pass MySQL Connect Form the handle of this Form.
                mc.ShowDialog(freezeForm);

                // Wait until Reconnect
                while (mc.Visible == true)
                {
                    Application.DoEvents();
                }
            }
        }

        public static object executeScalarQuery(string query, Form sender)
        {
            object o = "";
            /* Connection Check */
            checkConnection(sender);
            
            /* Build MySqlCommand Function for Query */
            MySqlCommand mysqlCmd = new MySqlCommand(query, mysqlglobal.mysqlCon);

            /* Execute Query */
            try
            {
                o = mysqlCmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                // Error With Query, Log
                logglobal.log.logListbox.Items.Insert(0, "MySQL Error at " + DateTime.Now + ": " + Convert.ToString(e.Message));
            }
            mysqlCmd.Dispose();
            return o;
        }

        public static int executeNonQuery(string query, Form sender, bool get_insert_id = false)
        {
            int output = 0;

            /* Connection Check */
            checkConnection(sender);

            /* Add LAST_INSERT_ID() to Query (2011-12-23) */
            if (get_insert_id)
            {
                if (query.EndsWith(";") == false) query += ";";
                query += " SELECT last_insert_id()";
            }

            /* Build MySqlCommand Function for Query */
            MySqlCommand mysqlCmd = new MySqlCommand(query, mysqlglobal.mysqlCon);

            /* Execute Query */
            try
            {
                if (get_insert_id)
                {                    
                    object temp = mysqlCmd.ExecuteScalar();
                    output = Convert.ToInt32(temp);
                }
                else
                {
                    mysqlCmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                // Error With Query, Log
                logglobal.log.logListbox.Items.Insert(0, "MySQL Error at " + DateTime.Now + ": " + Convert.ToString(e.Message));
            }
            mysqlCmd.Dispose();
            return output;
        }

        public static DataSet executeDataSetQuery(string query, string table_name, Form sender, DataSet mysqlDS = null)
        {
            /* Connection Check */
            checkConnection(sender);

            /* Build MySqlCommand Function for Query */
            MySqlCommand mysqlCmd = new MySqlCommand(query, mysqlglobal.mysqlCon);

            /* Execute Query */
            MySqlDataAdapter mysqlDA = null;
            if (mysqlDS == null)
            {
                mysqlDS = new DataSet();
                mysqlDS.Clear(); // New Dataset.                    
            }

            try
            {
                mysqlDA = new MySqlDataAdapter(mysqlCmd);               
                mysqlDA.Fill(mysqlDS, table_name);
            }
            catch (Exception e)
            {
                //MessageBox.Show("Error!");
                // Error With Query, Log
                logglobal.log.Visible = true;
                logglobal.log.Show();
                logglobal.log.logListbox.Items.Insert(0, "MySQL Error at " + DateTime.Now + ": " + Convert.ToString(e.Message));
                mysqlDS.Tables.Add(table_name); // Dummy Table to prevent errors (genius~!)
                return mysqlDS; // Which will be empty
            }

            /* Success ? */
            mysqlCmd.Dispose();
            return mysqlDS;

        }

        public static string escapeString(string input)
        {
            string output = input;

            string one_backslash = Convert.ToString((char) 92);
            string two_backslash = Convert.ToString((char) 92) + Convert.ToString((char) 92);

            string doublequote = Convert.ToString((char)34);
            string backslashdoublequote = Convert.ToString((char)92) + Convert.ToString((char)34);

            output = output.Replace(one_backslash, two_backslash);
            output = output.Replace(doublequote, backslashdoublequote);

            return output;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string formatDate(DateTime date)
        {
            if (date.Year == 1970)
            {
                // Yeah.
                return "<none>";
            }

            string dateFormat = iniglobal.ini.IniReadValue("company","dateFormat");
            return String.Format("{0:" + dateFormat + "}", date);
        }
    }

    public class dbfglobal
    {
        public static OleDbConnection dbfCon = null;

        public static string escapeString(string input)
        {
            string output = input;

            string doublequote = Convert.ToString((char)34);
            string doublequotedoublequote = Convert.ToString((char)34) + Convert.ToString((char)34);

            output = output.Replace(doublequote, doublequotedoublequote);

            return output;
        }
    }

    public class logglobal
    {
        public static program_log log = null;
    }

    public class iniglobal
    {
        public static IniFile ini = null;
    }

    public class cg
    {
        public static string price(object input)
        {
            string output = "";
            decimal decimal_input;

            if (input is string)
            {
                string string_input = ((string)input);               
                // Remove all Spaces
                string_input = string_input.Replace(" ", "");
                // If ending in dot, remove dot
                if (string_input.Length > 0)
                {
                    if (string_input.Substring(string_input.Length - 1, 1) == ".")
                    {
                        string_input = string_input.Substring(0, string_input.Length - 1); // Remove Dot
                    }
                }

                // If Result Blank, Set to 0.
                if (string_input == "") string_input = "0";

                // General Conversion
                decimal_input = Convert.ToDecimal(string_input);
            }
            else
            {
                decimal_input = Convert.ToDecimal(input);
            }

            output = decimal_input.ToString("#0.00");
            if (output == "") output = "0.00";
            return output;
        }

        public static string quantity(object input)
        {
            string output = "";
            decimal decimal_input;

            if (input is string)
            {
                decimal_input = Convert.ToDecimal(((string)input));
            }
            else
            {
                decimal_input = Convert.ToDecimal(input); 
            }

            output = Convert.ToString(Math.Floor(decimal_input));
            return output;
        }
    }

    public class searchglobal
    {
        public static int findRow(string search_term, DataGridView datagridview, int column_index)
        {
            int records = datagridview.RowCount;

            search_term = search_term.ToUpper();

            string cell_value = "";
            int[] match_score;
            match_score = new int[records];

            int best_match = 0;

            //for (int i = 1; i <= records; i++) {
            for (int i = records - 1; i >= 0; i--) // Has to do a backwards search, last row is row 0.
            {
                cell_value = Convert.ToString(datagridview.Rows[i].Cells[column_index].Value).ToUpper(); // Load Key to Search
                int sum = 0;
                for (int j = 0; j < Math.Min(search_term.Length, cell_value.Length); j++)
                {
                    if (search_term[j] == cell_value[j]) sum++; // Check each Letter to produce a Score.
                    if (search_term[j] != cell_value[j]) break; // Stop searching when incompatible char is reached.
                }
                match_score[i] = sum; // Store score.

                if (match_score.Max() == sum) best_match = i; // Check against Previous Scores. If this is the highest, this is the best_match so far.
            }

            // Highlist Record
            return best_match;
        }

    }


}

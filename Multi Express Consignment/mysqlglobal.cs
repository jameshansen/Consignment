using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

using System.Data.OleDb;
using System.Drawing;
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

        /* Reads a column that may not exist yet on an older database. */
        public static string field(DataRow row, string column, string fallback)
        {
            if (row.Table.Columns.Contains(column) == false) return fallback;
            return Convert.ToString(row[column]);
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

    public class windowglobal
    {
        /* Opens a window centred on the one that opened it, so it lands on the same monitor (2026) */
        public static void centre(Form window)
        {
            Form on = window.Owner;
            if (on == null && Application.OpenForms.Count > 0) on = Application.OpenForms[0]; // Shown without an owner, use the main window
            if (on == null) return;

            Rectangle area = on.RectangleToScreen(on.ClientRectangle);

            window.StartPosition = FormStartPosition.Manual;
            window.Left = area.Left + ((area.Width - window.Width) / 2);
            window.Top = area.Top + ((area.Height - window.Height) / 2);
        }
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

    /* Tax codes and rates */
    public class taxglobal
    {
        public const string table = "CSTTBLTAX";

        public const string iconFolder = @"icons\tax"; // Beside the executable, drop in your own bitmaps

        private static Dictionary<string, decimal> rates = null; // Small lookup, cached until the tax window changes it
        private static Dictionary<string, string> descriptions = null;
        private static Dictionary<string, string> icons = null;
        private static Dictionary<string, Image> loaded_icons = new Dictionary<string, Image>();

        /* Used by items with no code of their own */
        public static string defaultCode()
        {
            string code = "";
            try
            {
                code = iniglobal.ini.IniReadValue("company", "taxcode");
            }
            catch
            {
                /* Not set */
            }
            if (code == "") code = "PG"; // PST and GST
            return code.Trim().ToUpper();
        }

        public static void reload()
        {
            rates = null;
            descriptions = null;
            icons = null;
            loaded_icons.Clear(); // Pick up a replaced bitmap file too
        }

        public static Dictionary<string, decimal> load()
        {
            if (rates != null) return rates;

            rates = new Dictionary<string, decimal>();
            descriptions = new Dictionary<string, string>();
            icons = new Dictionary<string, string>();

            DataSet tax_file = mysqlglobal.executeDataSetQuery("SELECT * FROM `" + table + "` ORDER BY `tax_code`", table, null);
            foreach (DataRow row in tax_file.Tables[table].Rows)
            {
                string code = Convert.ToString(row["tax_code"]).Trim().ToUpper();
                rates[code] = Convert.ToDecimal(row["tax_rate"]);
                descriptions[code] = Convert.ToString(row["tax_desc"]).Trim();
                icons[code] = mysqlglobal.field(row, "tax_icon", "").Trim();
            }

            return rates;
        }

        /* Bitmap name held against the code */
        public static string iconName(object tax_code)
        {
            load();

            string code = Convert.ToString(tax_code).Trim().ToUpper();
            return icons.ContainsKey(code) ? icons[code] : "";
        }

        /* The bitmap held against a code, or null if there is none */
        public static Image icon(object tax_code)
        {
            return iconFile(iconName(tax_code));
        }

        /* A bitmap from the icon folder, by name */
        public static Image iconFile(object icon_name)
        {
            string name = Convert.ToString(icon_name).Trim().ToUpper();
            if (name == "") return null;

            if (loaded_icons.ContainsKey(name) == false)
            {
                Image image = null;
                try
                {
                    string file = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), iconFolder), name + ".BMP");

                    // Copied into memory so the file is not held open
                    if (File.Exists(file)) using (Image onDisk = Image.FromFile(file)) image = new Bitmap(onDisk);
                }
                catch
                {
                    /* Unreadable bitmap, show nothing */
                }
                loaded_icons[name] = image;
            }

            return loaded_icons[name];
        }

        /* Puts the icon in a grid cell, with the code, description and rate as its tooltip */
        public static void showIcon(DataGridViewCell cell, object tax_code)
        {
            string code = Convert.ToString(tax_code).Trim().ToUpper();
            if (load().ContainsKey(code) == false) code = defaultCode(); // Blank or unknown, the rate falls back too

            cell.Value = icon(code);
            cell.ToolTipText = label(code);
        }

        /* Code, description and rate as one line for a drop down. */
        public static string label(string tax_code)
        {
            load();

            string code = Convert.ToString(tax_code).Trim().ToUpper();
            string description = descriptions.ContainsKey(code) ? descriptions[code] : "";

            if (description != "") description = " " + description;

            return code + " -" + description + " (" + rate(code).ToString("#0.##") + "%)";
        }

        /* Percentage for a code, unknown or blank falls back to the default code */
        public static decimal rate(object tax_code)
        {
            Dictionary<string, decimal> table_rates = load();

            string code = Convert.ToString(tax_code).Trim().ToUpper();
            if (code != "" && table_rates.ContainsKey(code)) return table_rates[code];

            string fallback = defaultCode();
            if (table_rates.ContainsKey(fallback)) return table_rates[fallback];

            return 12; // No tax table
        }

        /* Creates the tax table and columns so an existing database needs no manual SQL */
        public static void ensureSchema()
        {
            mysqlglobal.executeNonQuery(
            @"CREATE TABLE IF NOT EXISTS `" + table + @"` (
              `tax_code` varchar(2) NOT NULL,
              `tax_desc` varchar(30) NOT NULL DEFAULT '',
              `tax_rate` decimal(7,4) NOT NULL DEFAULT '0.0000',
              `tax_icon` varchar(8) NOT NULL DEFAULT '',
              PRIMARY KEY (`tax_code`)
            ) ENGINE=MyISAM DEFAULT CHARSET=latin1", null);

            /* One time column rename */
            if (Convert.ToString(mysqlglobal.executeScalarQuery("SHOW COLUMNS FROM `" + table + "` LIKE 'txcode'", null)) != "")
            {
                mysqlglobal.executeNonQuery(
                @"ALTER TABLE `" + table + @"`
                  CHANGE `txcode` `tax_code` varchar(2) NOT NULL,
                  CHANGE `txdesc` `tax_desc` varchar(30) NOT NULL DEFAULT '',
                  CHANGE `txrate` `tax_rate` decimal(7,4) NOT NULL DEFAULT '0.0000'", null);
            }

            if (Convert.ToString(mysqlglobal.executeScalarQuery("SHOW COLUMNS FROM `" + table + "` LIKE 'tax_icon'", null)) == "")
            {
                mysqlglobal.executeNonQuery("ALTER TABLE `" + table + "` ADD COLUMN `tax_icon` varchar(8) NOT NULL DEFAULT ''", null);

                /* Default icons */
                mysqlglobal.executeNonQuery(
                @"UPDATE `" + table + @"` SET `tax_icon` = CASE `tax_code`
                  WHEN 'PG' THEN 'METAX1'
                  WHEN 'P' THEN 'METAX2'
                  WHEN 'G' THEN 'METAX3'
                  WHEN 'NO' THEN 'METAX4'
                  WHEN 'H' THEN 'METAX9'
                  ELSE '' END
                  WHERE `tax_icon` = ''", null);
            }

            /* Seed the default codes, once */
            if (Convert.ToString(mysqlglobal.executeScalarQuery("SELECT COUNT(*) FROM `" + table + "`", null)) == "0")
            {
                mysqlglobal.executeNonQuery(
                @"INSERT INTO `" + table + @"` (`tax_code`, `tax_desc`, `tax_rate`, `tax_icon`) VALUES
                  ('PG', 'PST AND GST', 12.0000, 'METAX1'),
                  ('P', 'PST ONLY', 7.0000, 'METAX2'),
                  ('G', 'GST ONLY', 5.0000, 'METAX3'),
                  ('NO', 'NO TAX', 0.0000, 'METAX4'),
                  ('H', 'HST', 12.0000, 'METAX9')", null);
            }

            /* Items carry the tax code they are sold under. */
            if (Convert.ToString(mysqlglobal.executeScalarQuery("SHOW COLUMNS FROM `CSTITEM` LIKE 'tax_code'", null)) == "")
            {
                mysqlglobal.executeNonQuery("ALTER TABLE `CSTITEM` ADD COLUMN `tax_code` varchar(2) NOT NULL DEFAULT ''", null);
            }

            /* And the rate they were sold at, so a later rate change cannot rewrite a past sale */
            if (Convert.ToString(mysqlglobal.executeScalarQuery("SHOW COLUMNS FROM `CSTITEM` LIKE 'tax_rate'", null)) == "")
            {
                mysqlglobal.executeNonQuery("ALTER TABLE `CSTITEM` ADD COLUMN `tax_rate` decimal(7,4) NOT NULL DEFAULT '0.0000'", null);

                /* Anything already sold was charged 12% */
                mysqlglobal.executeNonQuery("UPDATE `CSTITEM` SET `tax_rate` = 12.0000 WHERE `status` = 'sold'", null);
            }

            reload();
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

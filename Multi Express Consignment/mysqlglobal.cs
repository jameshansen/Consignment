using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }

    public class dbfglobal
    {
        public static OleDbConnection dbfCon = null;
    }

    public class logglobal
    {
        public static TextWriter logFile = null;
    }

    public class iniglobal
    {
        public static IniFile ini = null;
    }
}

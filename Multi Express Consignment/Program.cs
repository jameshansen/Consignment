using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace Multi_Express_Consignment
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = new Form1();
            // The form's designer size (1279x1010) is larger than the 1024x768
            // demo screen, so start it maximized. Cleaner than maximizing it from
            // the outside once it has shown.
            form1.WindowState = FormWindowState.Maximized;
            Application.ThreadException += new ThreadExceptionEventHandler(form1.UnhandledThreadExceptionHandler);
            Application.Run(form1);
        }
    }
}

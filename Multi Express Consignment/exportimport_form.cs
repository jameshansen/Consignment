using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Multi_Express_Consignment
{
    public partial class exportimport_form : Form
    {
        public string m_mode = "export";
        public exportimport_form(string mode)
        {
            InitializeComponent();
            m_mode = mode;
        }

        private void exportimport_form_Load(object sender, EventArgs e)
        {
            windowglobal.centre(this); // (2026)

            // Switch Mode if Specified
            if (m_mode.Substring(0,6) == "import")
            {
                this.Text = "Import Data";
                groupBox1.Text = "Import File Location";
                button2.Text = "Start Data Import";
            }

            // Fill in textbox if specified
            if (m_mode.Length > 6)
            {
                textBox1.Text = m_mode.Substring(7); // e.g. export:C:\File or import:C:\File
                saveFileDialog1.FileName = m_mode.Substring(7);
                openFileDialog1.FileName = "";
            }
        }

        void process_Exited(object sender, EventArgs e)
        {
            var process = (Process)sender;
            using (var f = File.CreateText(textBox1.Text))
            {
                f.WriteLine(process.StandardOutput.ReadToEnd());
            }
            process.Kill(); // Finish the job
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // MySQL Details
            string mysqlServer = iniglobal.ini.IniReadValue("mysql", "server");
            string mysqlDatabase = iniglobal.ini.IniReadValue("mysql", "database");
            string mysqlUser = iniglobal.ini.IniReadValue("mysql", "user");
            string mysqlPassword = iniglobal.ini.IniReadValue("mysql", "password");

            // Lock everything
            textBox1.Enabled = false;
            button2.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // Export Mode
            if (m_mode.Substring(0, 6) == "export")
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(@"mysqldump.exe");

                startInfo.Arguments = "--user=" + mysqlUser + " --password=" + mysqlPassword + " --host=" + mysqlServer + " " + mysqlDatabase;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.WindowStyle = ProcessWindowStyle.Minimized;
                
                Process p = new Process();
                p.StartInfo = startInfo;
                //p.EnableRaisingEvents = true;
                //p.Exited += process_Exited;
                p.Start(); // Start Process with info in StartInfo

                string output = p.StandardOutput.ReadToEnd();

                if (output == "")
                {
                    MessageBox.Show("Export Failed. Please contact the software vendor for support.", "Export Failed");
                    this.Close();
                    return;
                }
                System.IO.File.WriteAllText(textBox1.Text, output);

                MessageBox.Show("Export Complete. This workstation will now be locked for editing until import.", "Export Complete");

                // Update INI
                iniglobal.ini.IniWriteValue("mysql", "exportpath", Path.GetDirectoryName(textBox1.Text));

                // Lock workstation
                System.IO.File.WriteAllText(Path.GetDirectoryName(Application.ExecutablePath) + @"\exported.flag", "Exported data at " + DateTime.Now.ToLongDateString());

                this.Close();
            }

            // Import Mode
            if (m_mode.Substring(0, 6) == "import")
            {
                StreamReader file = new StreamReader(textBox1.Text);
                string input = file.ReadToEnd();
                file.Close();


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysql";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = false;
                psi.Arguments = "--user=" + mysqlUser + " --password=" + mysqlPassword + " --host=" + mysqlServer + " " + mysqlDatabase;
                psi.UseShellExecute = false;


                Process process = Process.Start(psi);
                process.StandardInput.WriteLine(input);
                process.StandardInput.Close();
                process.WaitForExit();
                process.Close();


                MessageBox.Show("Import Complete. Any Export Locks will now be removed.", "Import Complete");

                // Unlock workstation
                try
                {
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + @"\exported.flag");
                }
                catch
                {
                    // File Doesn't Exist
                }

                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_mode.Substring(0, 6) == "export")
            {
                saveFileDialog1.FileName = textBox1.Text;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = saveFileDialog1.FileName;
                }
            }
            if (m_mode.Substring(0, 6) == "import")
            {
                openFileDialog1.InitialDirectory =  textBox1.Text;                
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog1.FileName;
                }
            }
        }

        private void exportimport_form_Shown(object sender, EventArgs e)
        {
            if (m_mode.Substring(0, 6) == "import")
            {
                button1_Click(null, null);
            }
        }

    }
}

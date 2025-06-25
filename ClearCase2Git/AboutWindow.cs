using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Microsoft.Win32;

namespace ClearCase2Git
{
    public partial class AboutWindow : Form
    {
        string userid = "";
        string username = "";
        string usermail = "";
        string userpassword = "";
        string originalPath = "";
        string currVersion = "1.0.3";
        string releaseDate = "2025-03-05";
        RegistryKey rkSend;

        #region support functions
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                    throw;
            }
        }
        #endregion

        public AboutWindow(string id, string name, string mail, string password, string op)
        {
            InitializeComponent();
            infoRchTxtBx.Text = "This tool provides a platform for pinpointed or serial transfer of ClearCase VOBs to Git repositories.\n" +
                                "\n" +
                                "Version: " + currVersion + ", " + releaseDate + "\n" +
                                "Author : Sam Bergåse, sam.bergase@saabgroup.com";

            userid = id;
            username = name;
            usermail = mail;
            userpassword = password;

            originalPath = op;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Clients\\Mail"))
            {
                bool mailExists = false;
                string[] keyValues = key.GetValueNames(); // "" & "PreFirstRun"
                foreach (string kv in keyValues)
                {
                    if ((kv != "") && (kv != "PreFirstRun"))
                        mailExists = true;
                }
                if (mailExists)
                {
                    rkSend = key.OpenSubKey(@"Software\Clients\Mail\Microsoft Outlook\protocolsmailto\shell\open\command", false); // null
                    if (rkSend != null)
                        contactBtn.Enabled = true;
                    else
                        contactBtn.Enabled = false;
                }
                else
                    contactBtn.Enabled = false;
            }
        }

        private void contactBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This function is not finished", "Incomplete function", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            // Process "mailto:sam.bergase@saabgroup.com?subject=ClearCase2Git support";
        }
        private void docBtn_Click(object sender, EventArgs e)
        {
            OpenUrl(originalPath + "\\CC2Git_Info.html");
        }
        private void changeBtn_Click(object sender, EventArgs e)
        {
            OpenUrl(originalPath + "\\ChangeManagement.html");
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

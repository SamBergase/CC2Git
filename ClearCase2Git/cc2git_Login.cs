using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
//using Microsoft.SharePoint.Client;
using System.Security.Cryptography;

namespace ClearCase2Git
{
    public struct GitRepoData
    {
        public string pathName;
        public string Name;
        public string Type;
    }
    public struct GitProjectData
    {
        public string Name;
    }
    public struct GitUrlData
    {
        public string Name;
        public string Path;
    }
    public partial class cc2git_Login : Form
    {
        #region variables
        public string user_id = "";
        public string user_pwd = "";
        public string user_name = "";
        public string user_email = "";
        private bool evaluated = false;
        public List<string> binTargets = new List<string>() { "No separation", "Artifactory", "IFS" };
        public List<string> docTargets = new List<string>() { "No separation", "Doc. Sollution", "DOORS", "IFS" };
        public List<string> Available_CC_Srv = new List<string>();
        public int setCCServ = -1;
        // --- GitRepoData Handling ---
        private static int maxNoOfStorages = 165;
        public int noOfStorages = 0;
        public GitRepoData[] userGitStorages = new GitRepoData[maxNoOfStorages];
        public string lastSetGitRepo = "";
        // --- GitProjectData Handling ---
        private static int maxNoOfProjects = 16;
        public int noOfProjects = 0;
        public GitProjectData[] userGitProjects = new GitProjectData[maxNoOfProjects];
        int SetGitProj = 0;
        private static int maxNoOfURLs = 16;
        public int noOfURSs = 0;
        public GitUrlData[] userGitURLs = new GitUrlData[maxNoOfURLs];
        int SetGitUrl = 0;
        // --- Substitute encryption handling ---
        char[] sehEncode = { 'p', 'f', '4', 'k', 'H', '0', '$', '£', '%', 'l', 'y', 'x', 't', 'n', '5', '@', '9', '#', '€', '§', '2', 'a', 'b', '3', '8', '"', '¤', '&', 'o', 'i', 'K', 'V', 'm', '7', '6', 'h', '?', 'P', 'F', 'Q', '^', '=', ')', '{', '+', '/', '(', '}', '1', ']', 'A', 'v', '*', '-', 'q', 'r', 'g', 'B', 'Y', 'd', 'C', 'O', 'z', '¨', '~', 'G', 'z', 'u', 's', 'c', 'U', 'X', 'R', 'T', 'M', 'D', 'J', 'N' };
        char[] sehDecode = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '§', '!', '#', '¤', '%', '&', '/', '(', ')', '=', '?', '@', '£', '$', '€', '~', '^', '*' };
        #endregion

        #region supportFunctions
        private bool CheckIsActive(string server)
        {
            try
            {
                var ping = new Ping();
                var reply = ping.Send(server, 60 * 100);
                if ((reply.Status != IPStatus.Success) && (!reply.ToString().Contains("not find")))
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((ActiveControl == UserLoginPasswordTxtBx) && (keyData == Keys.Return) && (LoginBtn.Text == "Check"))
            {
                EventArgs ea = new EventArgs();
                LoginBtn_Click(this, ea);
                return true;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        private byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }
            return buffer;
        }
        private string EncryptString(string plainText)
        {
            string result = "";

            if (!String.IsNullOrEmpty(plainText))
            {
                foreach (char ch in plainText)
                {
                    int i = 0;
                    bool found = false;
                    while ((!found) && (i < sehDecode.Length))
                    {
                        if (ch == sehDecode[i])
                        {
                            result = result + sehEncode[i];
                            found = true;
                        }
                        i++;
                    }
                }
            }
            return result;
        }
        private string DecryptString(string cipherText)
        {
            string result = "";
            if (!string.IsNullOrEmpty(cipherText))
            {
                foreach (char ch in cipherText)
                {
                    int i = 0;
                    bool found = false;
                    while ((!found) && (i < sehEncode.Length))
                    {
                        if (ch == sehEncode[i])
                        {
                            result = result + sehDecode[i];
                            found = true;
                        }
                        i++;
                    }
                }
            }
            return result;
        }
        #endregion

        public cc2git_Login()
        {
            InitializeComponent();

            gitUrlCmbBx.Items.Clear();
            gitUrlCmbBx.Items.Add("Select");
            gitUrlCmbBx.Items.Add("Add Git Web...");
            gitUrlCmbBx.SelectedIndex = 0;
            CCSrvCmbBx.Items.Clear();
            CCSrvCmbBx.Items.Add("Select");
            CCSrvCmbBx.Items.Add("Add server...");
            CCSrvCmbBx.SelectedIndex = 0;
            ProjectCmbBx.Items.Clear();
            ProjectCmbBx.Items.Add("Select");
            ProjectCmbBx.Items.Add("Add Project...");
            ProjectCmbBx.SelectedIndex = 0;
            ProjectTxtBx.Enabled = false;
            ProjectTxtBx.Visible = false;
            ProjectCmbBx.Visible = true;
            CCSrvCmbBx.Enabled = true;
            CCSrvCmbBx.Visible = true;
            //ProjectCmbBx.Enabled = true;
        }
        #region public methods
        public bool validateUser()
        {
            return evaluated;
        }
        public string getUserId() { return user_id; }
        public string getUserPwd() { return user_pwd; }
        public string getUserName() { return user_name; }
        public string getUserEMail() { return user_email; }
        public int getNoOfUserGitStorages() { return noOfStorages; }
        public string getUserGitStorageName(int nr) { return userGitStorages[nr].Name; }
        public string getUserGitStoragePath(int nr) { return userGitStorages[nr].pathName; }
        public string getUserGitStorageType(int nr) { return userGitStorages[nr].Type; }
        public bool updateUserGitStorages()
        {
            if (System.IO.Directory.Exists("C:\\Users\\" + user_id + "\\source\\repos\\"))
            {
                List<string> tempStorageList = System.IO.Directory.GetDirectories("C:\\Users\\" + user_id + "\\source\\repos\\").ToList();
                foreach (var stge in tempStorageList)
                {
                    bool foundStorage = false;
                    for (int i = 0; i < noOfStorages; i++)
                    {
                        if (userGitStorages[i].pathName == stge)
                            foundStorage = true;
                    }
                    if (!foundStorage)
                    {
                        userGitStorages[noOfStorages].pathName = stge;
                        int dp = stge.LastIndexOf('\\');
                        userGitStorages[noOfStorages].Name = stge.Substring(dp + 1);
                        if (LibGit2Sharp.Repository.IsValid(stge))// (System.IO.Directory.Exists(stge + "\\.git"))
                            userGitStorages[noOfStorages].Type = "Git Repo";
                        else
                            userGitStorages[noOfStorages].Type = "Normal Repo";
                        noOfStorages++;
                    }
                }
            }
            if (System.IO.Directory.Exists("H:\\source\\repos\\"))
            {
                List<string> tempStorageList = System.IO.Directory.GetDirectories("H:\\source\\repos\\").ToList();
                foreach (var stge in tempStorageList)
                {
                    bool foundStorage = false;
                    for (int i = 0; i < noOfStorages; i++)
                    {
                        if (userGitStorages[i].pathName == stge)
                            foundStorage = true;
                    }
                    if (!foundStorage)
                    {
                        userGitStorages[noOfStorages].pathName = stge;
                        int dp = stge.LastIndexOf('\\');
                        userGitStorages[noOfStorages].Name = stge.Substring(dp + 1);
                        if (LibGit2Sharp.Repository.IsValid(stge))// (System.IO.Directory.Exists(stge + "\\.git"))
                            userGitStorages[noOfStorages].Type = "Git Repo";
                        else
                            userGitStorages[noOfStorages].Type = "Normal Repo";
                        noOfStorages++;
                    }
                }
            }
            if (noOfStorages > 0)
                return true;
            return false;

        }
        public bool addUserGitStorage(string name, string path)
        {
            if (noOfStorages < maxNoOfStorages)
            {
                bool foundStorage = false;
                for (int i = 0; i < noOfStorages; i++)
                {
                    if ((userGitStorages[i].pathName == path) && (userGitStorages[i].Name == name))
                        foundStorage = true;
                }
                if (!foundStorage)
                {
                    userGitStorages[noOfStorages].Name = name;
                    userGitStorages[noOfStorages].pathName = path;
                    noOfStorages++;
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
        public int getNoOfProjects() { return noOfProjects; }
        public string getProjectName(int nr) { return userGitProjects[nr].Name; }
        public int getNoOfURLs() { return noOfURSs; }
        public string getURLName(int nr) { return userGitURLs[nr].Name; }
        public string getURLPath(int nr) { return userGitURLs[nr].Path; }
        public int getSetGitURL() { return SetGitUrl; }
        public void setLastGitRepo(string str) { lastSetGitRepo = str; }
        public int getSetGitRepo() { return SetGitProj; }
        public string getLastGitRepo() { return lastSetGitRepo; }
        public int getNoOfCCServers() { return Available_CC_Srv.Count; }
        public string getSetCCServer()
        {
            if (CheckIsActive(Available_CC_Srv[CCSrvCmbBx.SelectedIndex].ToString()))
                return Available_CC_Srv[setCCServ].ToString();
            else
                return "";
        }
        #endregion
        #region private methods
        public bool saveUserData()
        {
            if (!System.IO.Directory.Exists("H:\\ClearCase\\"))
                System.IO.Directory.CreateDirectory("H:\\ClearCase");

            if (System.IO.File.Exists("H:\\ClearCase\\" + user_id + "_data.txt"))
                System.IO.File.Delete("H:\\ClearCase\\" + user_id + "_data.txt");

            using (System.IO.FileStream efs = System.IO.File.Create("H:\\ClearCase\\" + user_id + "_data.txt"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(efs))
                {
                    sw.WriteLine("setUserId : " + user_id);
                    string encPwd = EncryptString(user_pwd);
                    sw.WriteLine("setUserPwd : " + encPwd);
                    sw.WriteLine("setUserName : " + user_name);
                    sw.WriteLine("setUserEmail : " + user_email);
                    foreach (var userGitProject in userGitProjects)
                    {
                        sw.WriteLine("setUserProjects : " + userGitProject.Name);
                    }
                    foreach (var userGitStorage in userGitStorages)
                    {
                        sw.WriteLine("setUserGitStorageName : " + userGitStorage.Name);
                        sw.WriteLine("setUserGitStoragePath : " + userGitStorage.pathName);
                        sw.WriteLine("setUserGitStorageType : " + userGitStorage.Type);
                    }
                    foreach (var userGitURL in userGitURLs)
                    {
                        sw.WriteLine("setUserGitUrlName : " + userGitURL.Name);
                        sw.WriteLine("setUserGitUrlPath : " + userGitURL.Path);
                    }
                    sw.WriteLine("setGitWeb : " + SetGitUrl.ToString());
                    sw.WriteLine("lastSetGigRepo : " + lastSetGitRepo);
                    foreach (var wrsrv in Available_CC_Srv)
                    {
                        sw.WriteLine("SetAvCCSrv : " + wrsrv);
                    }
                    sw.WriteLine("SelectedCCSrv : " + setCCServ.ToString());
                }
                efs.Close();
            }
            return true;
        }
        private bool EvaluateUser(string uid, string pwd)
        {
            if ((uid == user_id) && (pwd == user_pwd))
            {
                evaluated = true;
                return true;
            }
            return false;
        }
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void LoginUserTxtBx_TextChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handledd in LoginBtn
        }
        private void UserLoginPasswordTxtBx_TextChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in LoginBtn.
            if (LoginBtn.Text == "Save data")
                LoginBtn.Enabled = true;
        }
        private void FullNameTxtBx_TextChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in the LoginBtn handling.
            LoginBtn.Enabled = true;
        }
        private void EmailTxtBx_TextChanged(object sender, EventArgs e)
        {
            if ((!EmailTxtBx.Text.Contains('@')) || (!EmailTxtBx.Text.Contains('.')))
                EmailTxtBx.BackColor = Color.PaleVioletRed;
            else
                EmailTxtBx.BackColor = Color.White;
            LoginBtn.Enabled = true;
        }
        private void CCSrvCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selIdx = CCSrvCmbBx.SelectedIndex;
            if ((selIdx > 0) && (selIdx < CCSrvCmbBx.Items.Count - 1) && (CheckIsActive(Available_CC_Srv[selIdx - 1].ToString())))
            {
                setCCServ = selIdx - 1;
                AddProjBtn.Text = "Remove CC Srv";
                AddProjBtn.Enabled = true;
                ProjectCmbBx.Enabled = true;
                if ((gitUrlCmbBx.SelectedIndex != 0) || (LoginBtn.Text == "Save data"))
                    LoginBtn.Enabled = true;
            }
            if (CCSrvCmbBx.SelectedItem.ToString() == "Add server...")
            {
                CCSrvCmbBx.Visible = false;
                AddCCSrvTxtBx.Enabled = true;
                AddCCSrvTxtBx.Visible = true;
                AddProjBtn.Text = "Add CC Server";
                AddProjBtn.Enabled = true;
            }
            else
                setCCServ = -1;
        }
        private void AddProjBtn_Click(object sender, EventArgs e)
        {
            if ((noOfProjects < maxNoOfProjects) && (AddProjBtn.Text == "Add Project"))
            {
                userGitProjects[noOfProjects++].Name = ProjectTxtBx.Text;
                ProjectTxtBx.Text = "";
                ProjectTxtBx.Enabled = false;
                ProjectTxtBx.Visible = false;
                ProjectCmbBx.Items.Clear();
                ProjectCmbBx.Items.Add("Select");
                for (int i = 0; i < noOfProjects; i++)
                    ProjectCmbBx.Items.Add(userGitProjects[i].Name);
                ProjectCmbBx.Items.Add("Add Project...");
                ProjectCmbBx.SelectedIndex = 0;
                ProjectCmbBx.Visible = true;
                ProjectCmbBx.Enabled = true;
            }
            else if ((noOfProjects > 0) && (AddProjBtn.Text == "Remove Proj."))
            {
                int selItm = ProjectCmbBx.SelectedIndex;
                if ((selItm > 0) && (selItm < ProjectCmbBx.Items.Count - 1))
                {
                    for (int i = selItm - 1; i < noOfProjects; i++)
                        userGitProjects[i].Name = userGitProjects[i + 1].Name;
                    userGitProjects[noOfProjects].Name = "";
                    ProjectCmbBx.Items.RemoveAt(selItm - 1);
                    noOfProjects--;
                }
            }
            else if (AddProjBtn.Text == "Add CC Server")
            {
                Available_CC_Srv.Add(AddCCSrvTxtBx.Text);
                AddCCSrvTxtBx.Text = "";
                AddCCSrvTxtBx.Enabled = false;
                AddCCSrvTxtBx.Visible = false;
                CCSrvCmbBx.Items.Clear();
                CCSrvCmbBx.Items.Add("Select");
                foreach (var resrv in Available_CC_Srv)
                {
                    if (CheckIsActive(resrv))
                        CCSrvCmbBx.Items.Add(resrv);
                    else
                        CCSrvCmbBx.Items.Add("(" + resrv + ")");
                }
                CCSrvCmbBx.Items.Add("Add server...");
                CCSrvCmbBx.SelectedIndex = 0;
                CCSrvCmbBx.Enabled = true;
                CCSrvCmbBx.Visible = true;
            }
            else if (AddProjBtn.Text == "Remove CC Srv")
            {
                Available_CC_Srv.Remove(CCSrvCmbBx.SelectedItem.ToString());
                CCSrvCmbBx.Enabled = false;
                CCSrvCmbBx.Visible = false;
                CCSrvCmbBx.Items.Clear();
                CCSrvCmbBx.Items.Add("Select");
                foreach (var resrv in Available_CC_Srv)
                {
                    if (CheckIsActive(resrv))
                        CCSrvCmbBx.Items.Add(resrv);
                }
                CCSrvCmbBx.Items.Add("Add server...");
                CCSrvCmbBx.SelectedIndex = 0;
                CCSrvCmbBx.Enabled = true;
                CCSrvCmbBx.Visible = true;
            }
            else if (AddProjBtn.Text == "Add Git Web")
            {
                string workString = GitUrlTxtBx.Text;
                int dpk = workString.IndexOf(':');
                int dpp = workString.IndexOf('.');
                if (((dpk > 0) && (dpk < workString.Length - 1)) && ((dpp > 0) && (dpp < workString.Length - 1)) && (dpk < dpp))
                {
                    userGitURLs[noOfURSs].Name = workString.Substring(dpk + 3, dpp - dpk - 3);
                    userGitURLs[noOfURSs++].Path = workString;
                }
                else
                {
                    userGitURLs[noOfURSs].Name = "Unknown";
                    userGitURLs[noOfURSs++].Path = workString;
                }
                GitUrlTxtBx.Text = "";
                GitUrlTxtBx.Enabled = false;
                GitUrlTxtBx.Visible = false;
                gitUrlCmbBx.Items.Clear();
                gitUrlCmbBx.Items.Add("Select");
                for (int i = 0; i < noOfURSs; i++)
                {
                    int dpC = userGitURLs[i].Path.IndexOf(':');
                    string cutPath = userGitURLs[i].Path.Substring(dpC + 3);
                    dpC = cutPath.IndexOf('/');
                    cutPath = cutPath.Substring(0, dpC);
                    if (CheckIsActive(cutPath))
                        gitUrlCmbBx.Items.Add(userGitURLs[i].Path);
                    else
                        gitUrlCmbBx.Items.Add("(" + userGitURLs[i].Path + ")");
                }
                gitUrlCmbBx.Items.Add("Add Git Web...");
                gitUrlCmbBx.SelectedIndex = 0;
                gitUrlCmbBx.Enabled = true;
                gitUrlCmbBx.Visible = true;
            }
            else if (AddProjBtn.Text == "Remove Git Web")
            {
                int selItm = gitUrlCmbBx.SelectedIndex;
                if ((selItm > 0) && (selItm - 1 < noOfURSs))
                {
                    for (int i = selItm - 1; i < noOfURSs; i++)
                    {
                        userGitURLs[i].Name = userGitURLs[i + 1].Name;
                        userGitURLs[i].Path = userGitURLs[i + 1].Path;
                    }
                    userGitURLs[noOfURSs].Name = "";
                    userGitURLs[noOfURSs].Path = "";
                    gitUrlCmbBx.Items.RemoveAt(selItm - 1);
                    noOfURSs--;
                }
            }
            AddProjBtn.Enabled = false;
        }
        private void ProjectTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (noOfProjects < maxNoOfProjects)
            {
                AddProjBtn.Text = "Add Project";
                AddProjBtn.Enabled = true;
            }
            else
                AddProjBtn.Enabled = false;
        }
        private void ProjectCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            string setItem = ProjectCmbBx.SelectedItem.ToString();
            if ((setItem != "Select") && (setItem == "Add Project...") && (noOfProjects < maxNoOfProjects))
            {
                SetGitProj = -1;
                ProjectCmbBx.Enabled = false;
                ProjectCmbBx.Visible = false;
                ProjectTxtBx.Visible = true;
                ProjectTxtBx.Enabled = true;
            }
            else if ((setItem != "Select") && (setItem != "Add Project...") && (noOfProjects > 0))
            {
                SetGitProj = ProjectCmbBx.SelectedIndex - 1;
                AddProjBtn.Text = "Remove Proj.";
                AddProjBtn.Enabled = true;
            }
            else
            {
                SetGitProj = -1;
                AddProjBtn.Text = "";
                AddProjBtn.Enabled = false;
            }
        }
        private void GitUrlTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (noOfURSs < maxNoOfURLs)
            {
                AddProjBtn.Text = "Add Git Web";
                AddProjBtn.Enabled = true;
            }
            else
                AddProjBtn.Enabled = false;
        }
        private void gitUrlCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selItem = gitUrlCmbBx.SelectedItem.ToString();
            if ((selItem != "Select") && (selItem == "Add Git Web...") && (noOfURSs < maxNoOfURLs))
            {
                SetGitUrl = 0;
                gitUrlCmbBx.Enabled = false;
                gitUrlCmbBx.Visible = false;
                GitUrlTxtBx.Enabled = true;
                GitUrlTxtBx.Visible = true;
            }
            else if ((selItem != "Select") && (selItem != "Add Git Web...") && (noOfURSs > 0))
            {
                SetGitUrl = gitUrlCmbBx.SelectedIndex - 1;
                AddProjBtn.Text = "Remove Git Web";
                AddProjBtn.Enabled = true;
                if ((CCSrvCmbBx.SelectedIndex != 0) || (LoginBtn.Text == "Save data"))
                    LoginBtn.Enabled = true;
            }
            else
            {
                SetGitUrl = 0;
                AddProjBtn.Text = "";
                AddProjBtn.Enabled = false;
            }
        }
        private void NewUserChkBx_CheckedChanged(object sender, EventArgs e)
        {
            if ((NewUserChkBx.Checked) && (NewUserChkBx.Text == "Edit user"))
            {
                LoginBtn.Text = "Save data";
                UserLoginPasswordTxtBx.Enabled = true;
                FullNameTxtBx.Enabled = true;
                EmailTxtBx.Enabled = true;
                gitUrlCmbBx.Enabled = true;
                CCSrvCmbBx.Enabled = true;
                ProjectCmbBx.Enabled = true;
            }
            else if ((!NewUserChkBx.Checked) && (NewUserChkBx.Text == "Edit user"))
            {
                NewUserChkBx.Text = "New user";
                LoginBtn.Text = "Check";
            }
        }
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string set_user_id = LoginUserTxtBx.Text;
            string set_user_pwd = UserLoginPasswordTxtBx.Text;
            user_name = FullNameTxtBx.Text;
            user_email = EmailTxtBx.Text;
            bool newUserChecked = (NewUserChkBx.Checked && (NewUserChkBx.Text == "New user"));
            bool editUserChecked = (NewUserChkBx.Checked && (NewUserChkBx.Text == "Edit user"));
            bool dataFileExists = System.IO.File.Exists("H:\\ClearCase\\" + set_user_id + "_data.txt");
            if ((newUserChecked) || (!dataFileExists))
            {
                NewUserChkBx.Checked = true;
                LoginBtn.Text = "Create";
                FullNameTxtBx.Focus();
                LoginUserTxtBx.Enabled = true;
                UserLoginPasswordTxtBx.Enabled = true;
                FullNameTxtBx.Enabled = true;
                EmailTxtBx.Enabled = true;
                gitUrlCmbBx.Enabled = true;
                CCSrvCmbBx.Enabled = true;
                ProjectCmbBx.Enabled = true;
            }
            else
            {
                if (editUserChecked)
                {
                    user_id = set_user_id;
                    user_pwd = set_user_pwd;
                    ProjectCmbBx.Items.Clear();
                    ProjectCmbBx.Items.Add("Select");
                    LoginUserTxtBx.Enabled = true;
                    UserLoginPasswordTxtBx.Enabled = true;
                    FullNameTxtBx.Enabled = true;
                    EmailTxtBx.Enabled = true;
                    gitUrlCmbBx.Enabled = true;
                    CCSrvCmbBx.Enabled = true;
                    ProjectCmbBx.Enabled = true;
                    updateUserGitStorages();
                    saveUserData();
                    evaluated = true;
                }
                else if ((!(NewUserChkBx.Checked)) && (LoginBtn.Text != "Login"))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ProjectCmbBx.Items.Clear();
                    ProjectCmbBx.Items.Add("Select");
                    CCSrvCmbBx.Items.Clear();
                    CCSrvCmbBx.Items.Add("Select");
                    gitUrlCmbBx.Items.Clear();
                    gitUrlCmbBx.Items.Add("Select");
                    foreach (string line in System.IO.File.ReadLines("H:\\ClearCase\\" + set_user_id + "_data.txt"))
                    {
                        if (line != "-1")
                        {
                            int dp = line.IndexOf(':');
                            string dataTag = line.Substring(0, dp - 1);
                            string dataInfo = line.Substring(dp + 2);
                            switch (dataTag)
                            {
                                case "setUserId":
                                    {
                                        user_id = dataInfo;
                                    }
                                    break;
                                case "setUserPwd":
                                    {
                                        user_pwd = DecryptString(dataInfo);
                                    }
                                    break;
                                case "setUserName":
                                    {
                                        user_name = dataInfo;
                                        FullNameTxtBx.Text = dataInfo;
                                    }
                                    break;
                                case "setUserEmail":
                                    {
                                        user_email = dataInfo;
                                        EmailTxtBx.Text = dataInfo;
                                    }
                                    break;
                                case "setUserProjects":
                                    {
                                        // TODO - Change this to handle the actual data.
                                        if (noOfProjects < maxNoOfProjects)
                                        {
                                            userGitProjects[noOfProjects++].Name = dataInfo;
                                            ProjectCmbBx.Items.Add(dataInfo);
                                        }
                                    }
                                    break;
                                case "setUserGitUrlPath":
                                    {
                                        userGitURLs[noOfURSs].Path = dataInfo;
                                        int dpC = dataInfo.IndexOf(':');
                                        string cutUrl = dataInfo.Substring(dpC + 3);
                                        dpC = cutUrl.IndexOf('/');
                                        cutUrl = cutUrl.Substring(0, dpC);
                                        if (CheckIsActive(cutUrl))
                                            gitUrlCmbBx.Items.Add(dataInfo);
                                        else
                                            gitUrlCmbBx.Items.Add("(" + dataInfo + ")");
                                        int dpk = dataInfo.IndexOf(':');
                                        int dpp = dataInfo.IndexOf('.');
                                        if (((dpk > 0) && (dpk < dataInfo.Length - 1)) && ((dpp > 0) && (dpp < dataInfo.Length - 1)) && (dpk < dpp))
                                            dataInfo = dataInfo.Substring(dpk + 3, dpp - 3 - dpk);
                                        userGitURLs[noOfURSs++].Name = dataInfo;
                                    }
                                    break;
                                case "setGitWeb":
                                    {
                                        int.TryParse(dataInfo, out SetGitUrl);
                                        SetGitUrl++;
                                        gitUrlCmbBx.SelectedIndex = SetGitUrl;
                                    }
                                    break;
                                case "lastSetGigRepo":
                                    {
                                        lastSetGitRepo = dataInfo;
                                    }
                                    break;
                                case "SetAvCCSrv":
                                    {
                                        if (CheckIsActive(dataInfo))
                                        {
                                            Available_CC_Srv.Add(dataInfo);
                                            CCSrvCmbBx.Items.Add(dataInfo);
                                        }
                                        else
                                        {
                                            Available_CC_Srv.Add(dataInfo);
                                            CCSrvCmbBx.Items.Add("(" + dataInfo + ")");
                                        }
                                    }
                                    break;
                                case "SelectedCCSrv":
                                    {
                                        int setNo = -1;
                                        if ((dataInfo != "-1") && (int.TryParse(dataInfo, out setNo)))
                                        {
                                            if ((setNo <= CCSrvCmbBx.Items.Count) && (CheckIsActive(Available_CC_Srv[setNo])))
                                            {
                                                setCCServ = setNo;
                                                CCSrvCmbBx.SelectedIndex = setNo + 1;
                                            }
                                            else
                                            {
                                                setCCServ = -1;
                                                CCSrvCmbBx.SelectedIndex = 0;
                                            }
                                        }
                                        else
                                        {
                                            setCCServ = -1;
                                            CCSrvCmbBx.SelectedIndex = 0;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        // Erroneous branch, shall not be called.
                                    }
                                    break;
                            }
                        }
                    }
                    CCSrvCmbBx.Items.Add("Add server...");
                    CCSrvCmbBx.SelectedIndex = 0;
                    CCSrvCmbBx.Visible = true;
                    gitUrlCmbBx.Items.Add("Add Git Web...");
                    gitUrlCmbBx.SelectedIndex = 0;
                    gitUrlCmbBx.Visible = true;
                    ProjectCmbBx.Items.Add("Add Project...");
                    ProjectCmbBx.SelectedIndex = 0;
                    ProjectCmbBx.Visible = true;
                    updateUserGitStorages();
                    ProjectCmbBx.SelectedIndex = 0;
                    if (EvaluateUser(set_user_id, set_user_pwd))
                    {
                        NewUserChkBx.Text = "Edit user";
                        LoginBtn.Enabled = false;
                        LoginUserTxtBx.Enabled = false;
                        UserLoginPasswordTxtBx.Enabled = false;
                        FullNameTxtBx.Enabled = false;
                        EmailTxtBx.Enabled = false;
                        gitUrlCmbBx.Enabled = true;
                        CCSrvCmbBx.Enabled = true;
                        ProjectCmbBx.Enabled = true;
                    }
                    else
                    {
                        LoginBtn.Enabled = true;
                        LoginUserTxtBx.Enabled = true;
                        UserLoginPasswordTxtBx.Enabled = true;
                        FullNameTxtBx.Enabled = false;
                        EmailTxtBx.Enabled = false;
                        gitUrlCmbBx.Enabled = false;
                        GitUrlTxtBx.Enabled = false;
                        CCSrvCmbBx.Enabled = false;
                        AddCCSrvTxtBx.Enabled = false;
                        ProjectCmbBx.Enabled = false;
                        ProjectTxtBx.Enabled = false;
                    }
                    Cursor.Current = Cursors.Arrow;
                }
                
                if (LoginBtn.Text == "Login")
                {
                    this.Hide();
                }
                else
                {
                    LoginBtn.Text = "Login";
                    LoginBtn.Enabled = false;
                }
            }
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            if (UserLoginPasswordTxtBx.UseSystemPasswordChar)
                UserLoginPasswordTxtBx.UseSystemPasswordChar = false;
            else
                UserLoginPasswordTxtBx.UseSystemPasswordChar = true;
        }
    }
}

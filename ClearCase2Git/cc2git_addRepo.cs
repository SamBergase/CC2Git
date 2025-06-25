using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibGit2Sharp;

namespace ClearCase2Git
{
    public partial class cc2git_addRepo : Form
    {
        string ar_setDrive = "";
        string ar_setVOB = "";
        string ar_setGitWeb = "";

        #region support functions
        private void createRepo(string path, string proj)
        {
            bool setGitRepo = GitRepoChkBx.Checked;
            if ((setGitRepo) && (!(LibGit2Sharp.Repository.IsValid(path + "\\" + proj))))
            {
                LibGit2Sharp.Repository.Init(path + "\\" + proj);
            }
            else if (!(System.IO.Directory.Exists(path + "\\" + proj)))
            {
                System.IO.Directory.CreateDirectory(path + "\\" + proj);
            }
        }
        #endregion

        public cc2git_addRepo(string setDrive, string setVOB, string setGitWeb)
        {
            InitializeComponent();
            ar_setDrive = setDrive;
            FolderPathTxtBx.Text = setDrive;
            ar_setVOB = setVOB;
            ar_setGitWeb = setGitWeb;
        }

        private void FolderPathTxtBx_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(FolderPathTxtBx.Text))
            {
                FolderPathTxtBx.BackColor = Color.White;
                if ((ProjectNameTxtBx.Text != "") && (System.IO.Directory.Exists(FolderPathTxtBx.Text)))
                    CreateBtn.Enabled = true;
                else
                    CreateBtn.Enabled = false;
            }
            else
            {
                FolderPathTxtBx.BackColor = Color.PaleVioletRed;
                CreateBtn.Enabled = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // This is the "Navigate" button
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            DialogResult dres = folderBrowserDialog.ShowDialog();
            FolderPathTxtBx.Text = folderBrowserDialog.SelectedPath.ToString();
            folderBrowserDialog.Dispose();
        }
        private void ProjectNameTxtBx_TextChanged(object sender, EventArgs e)
        {
            if ((ProjectNameTxtBx.Text != "") && (System.IO.Directory.Exists(FolderPathTxtBx.Text)))
                CreateBtn.Enabled = true;
            else
                CreateBtn.Enabled = false;
        }
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void CreateBtn_Click(object sender, EventArgs e)
        {
            string setPathName = FolderPathTxtBx.Text;
            string setProjName = ProjectNameTxtBx.Text;
            if (!System.IO.File.Exists(setPathName + "\\" + setProjName))
            {
                //IEnumerable<Reference> ier = LibGit2Sharp.Repository.ListRemoteReferences(ar_setGitWeb + ar_setVOB); // Need credentials too.
                if (LibGit2Sharp.Repository.IsValid(ar_setGitWeb + ar_setVOB + "\\" + setProjName))
                {
                    DialogResult answer = MessageBox.Show("Repository \"" + setProjName + "\" exists remote.\nClone?.", "Repo exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.Yes)
                    {
                        // Clone the repo if the user replies "Yes"
                    }
                    else
                    {
                        createRepo(setPathName, setProjName);
                    }
                }
                else if (LibGit2Sharp.Repository.IsValid(ar_setGitWeb + ar_setVOB + "\\" + setProjName.ToUpper()))
                {
                    DialogResult answer = MessageBox.Show("Repository \"" + setProjName.ToUpper() + "\" exists remote.\nClone?.", "Repo exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.Yes)
                    {
                        // Clone the repo if the user replies "Yes"
                    }
                    else
                    {
                        createRepo(setPathName, setProjName);
                    }
                }
                else if (LibGit2Sharp.Repository.IsValid(ar_setGitWeb + ar_setVOB + "\\" + setProjName.ToLower()))
                {
                    DialogResult answer = MessageBox.Show("Repository \"" + setProjName.ToLower() + "\" exists remote.\nClone?.", "Repo exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.Yes)
                    {
                        // Clone the repo if the user replies "Yes"
                    }
                    else
                    {
                        createRepo(setPathName, setProjName);
                    }
                }
                else if (LibGit2Sharp.Repository.IsValid(ar_setGitWeb + ar_setVOB + "\\" + setProjName.Substring(0, 1).ToUpper() + setProjName.Substring(1).ToLower()))
                {
                    DialogResult answer = MessageBox.Show("Repository \"" + setProjName.Substring(0, 1).ToUpper() + setProjName.Substring(1).ToLower() + "\" exists remote.\nClone?.", "Repo exists", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.Yes)
                    {
                        // Clone the repo if the user replies "Yes"
                    }
                    else
                    {
                        createRepo(setPathName, setProjName);
                    }
                }
                else
                {
                    createRepo(setPathName, setProjName);
                }
                CancelBtn.Text = "Exit";
                Close();
            }
            else
            {
                DialogResult answer = MessageBox.Show("Repository \"" + setProjName + "\" allready exists.", "Repo exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        public string getRootDrive() { return ar_setDrive; }
    }
}

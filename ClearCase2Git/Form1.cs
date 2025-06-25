using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.IO;
using System.Web;
using System.Net.NetworkInformation;
//using System.Web.UI;
//using System.Web.UI.WebControls;
using System.Net;
using System.Threading;
using Microsoft.SqlServer;
using Microsoft.Win32;
using Microsoft.CSharp.RuntimeBinder;
using System.Diagnostics;
using LibGit2Sharp;
using System.Text.RegularExpressions;

using Octokit;
using ReadRepo;
using CCAutomation;
using Chilkat;

using Spire.Pdf;


namespace ClearCase2Git
{
    #region enum declarations
    public enum informationType
    {
        UNDEF = 0,
        INFO = 1,
        ERROR = 2,
        FATAL = 3
    }
    public enum workStepType
    {
        cpyOnly = 0,
        cpyToGitWork = 1,
        addToGitStag = 2,
        commitToGitLocal = 3,
        pushToGitRemote = 4,
    }
    public enum workFlowType
    {
        wft_notSet = 0,
        wft_singluar = 1,
        wft_streamed = 2
    }
    public enum avail_cc_vobs
    {
        adpp12 = 0,
        bud = 1,
        clear3 = 2,
        cologne = 3,
        capp9310 = 4,
        dave = 5,
        peggy = 6,
        schampoo = 7,
        sonja = 8
    }
    #endregion
    #region struct declarations
    public struct ClearCaseProjectData
    {
        public string Name;
    }
    public struct WorkProcessList
    {
        public string Name;
        public string GitTagName;
        public string Type;
    }
    public struct ISshKeyPair
    {
        public string PublicKey;
        public string PrivateKey;
    }
    #endregion

    public partial class Form1 : Form
    {
        #region Private Members
        #region User Handling
        string userid = "";
        string userName = "";
        string userEmail = "";
        public string lastworkdir = "";
        private cc2git_Login linwin;
        string userHomeDrive = "";
        #endregion
        #region General Data Handling
        public string setBaseDrive = "";
        public string loadedVobListFile = "";
        public string loadedViewListFile = "";
        public string logpath = "";
        public string dirpath = "";
        string setReg = "";
        List<string> Available_CC_Srv = new List<string>();
        #endregion
        #region ClearCase Handling
        ClearcaseHelper cchelp = new ClearcaseHelper();
        public string selectedCCVersion = "";
        // --- ClearCase Server ---
        public string setCCServer = "";
        // --- ClearCase Views ---
        public string setCCView = "";
        public List<string> ListOfCCViews;
        // --- ClearCase VOBs ---
        public string setCCVOB = "";
        public List<string> ListOfCCVOBs;
        // --- ClearCase Projects ---
        public string setCCProj = "";
        public const int maxNoOfCCProjsData = 1024;
        public int noOfCCProjsData = 0;
        public ClearCaseProjectData[] ccProjArr = new ClearCaseProjectData[maxNoOfCCProjsData];
        // --- ClearCase Branches ---
        public string setCCBranch = "";
        public List<string> ListOfCCBranches;
        // --- ClearCase Labels ---
        public string setCCLabel = "";
        public List<ClearCase.CCLabelType> ListOfCCLabels;
        string[] Available_CC_VOBS = new string[9];
        #endregion
        #region Git Handling
        GitHub myGitHub = new GitHub();
        LibGit2Sharp.Repository setTargetRepository;
        string repositoryUrl = "";
        string selectedGitVersion = "";
        string selectedGitWebTool = "";
        // --- Git Proj Handling ---
        string setGitProj = "";
        // --- Git Repo Handling ---
        string setGitRepo = "";
        // --- Git Commit Handling ---
        string setGitCommit = "";
        // --- Git Branch Handling ---
        string setGitBranch = "";
        // --- Git Tag Handling ---
        string setGitTag = "";
        // --- Standard Repo Handling ---
        #endregion
        #region Work Process Handling
        public string setDocRepo = "";
        // TODO - Move this to user login data taken from meta-data.
        string[] docItems = { "pdf", "doc", "docx", "xml", "odt", "xls", "xlsx", "txt", "csv" };
        public string setBinRepo = "";
        // TODO - Move this to user login data taken from meta-data.
        string[] binItems = { "exe", "com", "inf", "ipa", "osx", "pif", "run", "wsh", "html", "htm" };
        string[] thirdPtyItems = { "dy4", "tornado_221", "buster" };
        string sebBldDir = "";
        string sepTestDir = "";
        string sepThrdPrtDir = "";
        string sepDevDir = "";
        public List<string> binTargets = new List<string>() { "No separation", "Artifactory", "IFS", "IFS-Surf" };
        public List<string> docTargets = new List<string>() { "No separation", "Doc. Sollution", "DOORS", "IFS", "IFS-Surf", "RedDoc" };
        public const int maxNoOfProcessListContent = 32;
        public int noOfProcessListContent = 0;
        public WorkProcessList[] processItems = new WorkProcessList[maxNoOfProcessListContent];
        public workStepType workProcSteps;
        public workFlowType workFlow;
        ISshKeyPair mySSHkeys;
        public string urlResetString = "";
        public bool isSSH = false;
        string sDocSepFile = "";
        //DataTable dtDocSepList;
        //DataTable dtBinSepList;
        // TODO - Create more DataTables if needed.
        //DataSet dsDocItemSetList = new DataSet();
        #endregion
        #region Help handling
        private AboutWindow abwin;
        string originalPath = "";
        #endregion
        #endregion

        #region SupportFunctions
        public bool saveSessionData(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(logpath))
                {
                    if (linwin.getLastGitRepo() != "") 
                        sw.WriteLine("lastworkdir : " +  lastworkdir);
                    // --- ClearCase Version ---
                    sw.WriteLine("setCCVersion : " + selectedCCVersion);
                    // --- ClearCase Region Combo Box ---
                    for (int i = 1; i < RegionCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedCCRegions : " + RegionCmbBx.Items[i].ToString());
                    // --- ClearCase Drive Combo Box ---
                    for (int i = 1; i < ClearCaseDriveCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedCCDrives : " + ClearCaseDriveCmbBx.Items[i].ToString());
                    // --- ClearCase VOB Combo Box ---
                    for (int i = 1; i < ClearCaseVOBCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedCCVOBs : " + ClearCaseVOBCmbBx.Items[i].ToString());
                    // --- ClearCase View Combo Box ---
                    for (int i = 1; i < ClearCaseViewCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedCCViews : " + ClearCaseViewCmbBx.Items[i].ToString());
                    // --- ClearCase Label Combo Box ---
                    for (int i = 1; i < ClearCaseLabelCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedCCLabels : " + ClearCaseLabelCmbBx.Items[i].ToString());
                    // --- ClearCase Work List ---
                    for (int i = 0; i < WorkList.Items.Count; i++)
                        sw.WriteLine("loadedWorkListItems : " + WorkList.Items[i].ToString());
                    // --- Set Git Version ---
                    sw.WriteLine("setGitVersion : " + selectedGitVersion);
                    // --- Set Git Web Interface ---
                    sw.WriteLine("setGitWebInterface : " + selectedGitWebTool);
                    // --- Git Commit Combo Box ---
                    for (int i = 1; i < GitVersionCommitCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedGitCommits : " + GitVersionCommitCmbBx.Items[i].ToString());
                    // --- Git Branch Combo Box ---
                    for (int i = 1; i < GitVersionBranchCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedGitBranches : " + GitVersionBranchCmbBx.Items[i].ToString());
                    // --- Git Tag Combo Box ---
                    for (int i = 1; i < GitVersionTagCmbBx.Items.Count; i++)
                        sw.WriteLine("loadedGitTags : " + GitVersionTagCmbBx.Items[i].ToString());
                }
                linwin.saveUserData();
                return true;
            }
            catch (Exception err)
            {
                setInformationText(err.ToString(), informationType.ERROR, sender, e);
                return false;
            }
        }
        public void setInformationText(string inString, informationType inInfoType, object sender, EventArgs e)
        {
            switch (inInfoType)
            {
                case informationType.INFO:
                    {
                        string infoText = inString;
                        progInfoTxtBx.Text = infoText;
                        progInfoTxtBx.BackColor = Color.White;
                        infoText = System.DateTime.Now.ToString("yyyy-MM-dd - HH:mm:ss.ff") + " : Information : " + inString;
                        using (StreamWriter sw = File.AppendText(logpath))
                            sw.WriteLine(infoText);
                    }
                    break;
                case informationType.ERROR:
                    {
                        string infoText = inString;
                        progInfoTxtBx.Text = infoText;
                        progInfoTxtBx.BackColor = Color.PaleVioletRed;
                        infoText = System.DateTime.Now.ToString("yyyy-MM-dd - HH:mm:ss.ff") + " : ERROR : " + inString;
                        using (StreamWriter sw = File.AppendText(logpath))
                            sw.WriteLine(infoText);
                    }
                    break;
                case informationType.FATAL:
                    {
                        string infoText = System.DateTime.Now.ToString("yyyy-MM-dd - HH:mm:ss.ff") + "\nFATAL ERROR : " + inString + "\nOrigin : " + sender.ToString();
                        using (StreamWriter sw = File.AppendText(logpath))
                            sw.WriteLine(infoText);
                        if (MessageBox.Show(infoText, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        {
                            Close();
                        }
                    }
                    break;
                default:
                    {
                        string infoText = inString;
                        progInfoTxtBx.Text = infoText;
                        progInfoTxtBx.BackColor = Color.White;
                        infoText = System.DateTime.Now.ToString("yyyy-MM-dd - HH:mm:ss.ff") + " : " + inString;
                        using (StreamWriter sw = File.AppendText(logpath))
                            sw.WriteLine(infoText);
                    }
                    break;
            }
        }
        private void closeThisSession(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setInformationText("Session closed.", informationType.INFO, sender, e);
            saveSessionData(sender, e);
            Cursor.Current = Cursors.Arrow;
            try { Close(); } catch { setInformationText("Cought an exeption when closing the program.", informationType.ERROR, sender, e); }
        }
        public void getCCData(object sender, EventArgs e)
        {
            RegionCmbBx.Items.Clear();
            RegionCmbBx.Items.Add("Select");
            foreach (string line in CCAutomation.ClearcaseHelper.GetAllRegions())
            {
                RegionCmbBx.Items.Add(line);
            }
            RegionCmbBx.SelectedIndex = 0;
            RegionCmbBx.Enabled = true;
        }
        public void getGitData(object sender, EventArgs e)
        {
            GitVersionProjectCmbBx.Items.Clear();
            GitVersionProjectCmbBx.Items.Add("Select");
            int foundNoOfProjs = linwin.getNoOfProjects();
            for (int i = 0; i < foundNoOfProjs; i++)
            {
                string pns = linwin.getProjectName(i).ToString();
                GitVersionProjectCmbBx.Items.Add(pns);
            }
            if (linwin.getSetGitRepo() >= 0)
                GitVersionProjectCmbBx.SelectedIndex = linwin.getSetGitRepo() + 1;
            else
                GitVersionProjectCmbBx.SelectedIndex = 0;
            GitVersionProjectCmbBx.Enabled = true;
            // Git storages from user data.
            GitVersionRepoCmbBx.Items.Clear();
            GitVersionRepoCmbBx.Items.Add("Select");
            int foundNoOfRepos = linwin.getNoOfUserGitStorages();
            for (int i = 0; i < foundNoOfRepos; i++)
            {
                string rns = linwin.getUserGitStorageName(i);
                string rnst = linwin.getUserGitStorageType(i);
                if (rnst == "Git Repo")
                    GitVersionRepoCmbBx.Items.Add("(G) " + rns);
                else if (rnst == "Normal Repo")
                    GitVersionRepoCmbBx.Items.Add("(N) " + rns);
                else
                    GitVersionRepoCmbBx.Items.Add(rns);
            }
            GitVersionRepoCmbBx.SelectedIndex = 0;
            GitVersionRepoCmbBx.Enabled = true;
            // Check if the user has a .ssn lib, if not activate "SSH-Key generation" button.
            if (!(System.IO.Directory.Exists(userHomeDrive + "\\.ssh")))
            //if (!(System.IO.Directory.Exists("H:\\.ssh")))
                SSHkeygenBtn.Enabled = true;
            else
                SSHkeygenBtn.Enabled = false;
            // Enable the Git group box.
            GitVersionGrpBx.Enabled = true;
            AddGitRepoBtn.Enabled = true;
        }
        public ISshKeyPair RsaKeyPair()
        {
            // NOTE - This generates a warning, could be exchanged for nuget package SshKeyGenerator
            // see https://www.nuget.org/packages/SshKeyGenerator
            Chilkat.SshKey key = new Chilkat.SshKey();

            bool success;
            int numBits;
            int exponent;
            numBits = 2048;
            exponent = 65537;
            success = key.GenerateRsaKey(numBits, exponent);
            ISshKeyPair Gen_sshKeyPair = new ISshKeyPair();
            Gen_sshKeyPair.PublicKey = key.ToOpenSshPublicKey();
            Gen_sshKeyPair.PrivateKey = key.ToPuttyPrivateKey(false);
            if (!success) RsaKeyPair();
            return Gen_sshKeyPair;
        }
        public void setCCConfigSpec(int nr, string vtcf, object sender, EventArgs e)
        {
            if (workFlow == workFlowType.wft_streamed)
            {
                var viewToSet = new List<string>();
                if (processItems[nr].Type == "View")
                {
                    viewToSet = CCAutomation.ClearcaseHelper.GetConfigSpec(processItems[nr].Name);
                }
                else if ((processItems[nr].Type == "Label") || (processItems[nr].Type == "Git Tag"))
                {
                    viewToSet.Add("element * CHECKEDOUT");
                    viewToSet.Add("element * " + processItems[nr].Name);
                    viewToSet.Add("element * /main/LATEST");
                    if (vtcf.Contains("\\"))
                        viewToSet.Add("Load " + vtcf);
                    else
                        viewToSet.Add("Load \\" + vtcf);
                }
                CCAutomation.ClearcaseHelper.SetConfigSpec(viewToSet);
            }
            else if (workFlow == workFlowType.wft_singluar)
            {
                string selTagish = GitVersionTagCmbBx.SelectedItem.ToString();
                string selTagType = "";
                int stdp = selTagish.IndexOf('(');
                if ((stdp > 0) && (stdp < selTagish.Length - 1))
                {
                    selTagType = selTagish.Substring(stdp + 1, 1);
                    selTagish = selTagish.Substring(0, stdp);
                }
                else
                {
                    selTagType = "R";
                    stdp = selTagish.LastIndexOf('/');
                    if ((stdp > 0) && (stdp < selTagish.Length - 1))
                        selTagish = selTagish.Substring(stdp + 1);
                    else
                    {
                        setInformationText("Faulty tag-type selected.", informationType.FATAL, sender, e);
                    }
                }
                var viewToSet = new List<string>();
                if (selTagType == "V")
                {
                    //CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                    viewToSet = CCAutomation.ClearcaseHelper.GetConfigSpec(selTagish.Substring(0, stdp));
                }
                else if ((selTagType == "L") || (selTagType == "G") || (selTagType == "R"))
                {
                    viewToSet.Add("element * CHECKEDOUT");
                    viewToSet.Add("element * " + selTagish);
                    viewToSet.Add("element * /main/LATEST");
                    if (vtcf.Contains("\\"))
                        viewToSet.Add("Load " + vtcf);
                    else
                        viewToSet.Add("Load \\" + vtcf);
                }
                CCAutomation.ClearcaseHelper.SetConfigSpec(viewToSet);
            }
        }
        public int runClearingTask(string targetPath, object sender, EventArgs e)
        {
            int noOfDeletedItems = 0;
            string[] sourceFileList = System.IO.Directory.GetFiles(targetPath);
            if (sourceFileList.Length > 0)
            {
                for (int i = 0; i <= sourceFileList.Length - 1; i++)
                {
                    if (!sourceFileList[i].Contains(".git"))
                    {
                        try
                        {
                            File.SetAttributes(sourceFileList[i], FileAttributes.Normal);
                            File.Delete(sourceFileList[i]);
                            noOfDeletedItems++;
                        }
                        catch (Exception err)
                        {
                            setInformationText(err.ToString(), informationType.ERROR, sender, e);
                            throw err;
                        }
                    }
                }
            }
            string[] sourceDirectoryList = System.IO.Directory.GetDirectories(targetPath);
            if (sourceDirectoryList.Length > 0)
            {
                for (int i = 0; i <= sourceDirectoryList.Length - 1; i++)
                {
                    if ((!sourceDirectoryList[i].Contains(".git")) && (!sourceDirectoryList[i].Contains(".vs")))
                    {
                        noOfDeletedItems += runClearingTask(sourceDirectoryList[i], sender, e);
                        try
                        {
                            Directory.Delete(sourceDirectoryList[i]);
                        }
                        catch (Exception err)
                        {
                            setInformationText(err.ToString(), informationType.ERROR, sender, e);
                            throw err;
                        }
                    }
                }
            }
            return noOfDeletedItems;
        }
        public bool ConvertPdfToDocx(string sourcePath, string targetPath)
        {
            bool retVal = false;
            int pdfdp = sourcePath.LastIndexOf('.');
            int tpdp = targetPath.LastIndexOf('\\');
            if (tpdp > targetPath.Length - 2)
                targetPath = targetPath.Substring(0, tpdp);
            if ((pdfdp > 0) && (pdfdp < sourcePath.Length))
            {
                string pdfExt = sourcePath.Substring(0, pdfdp);
                if (pdfExt.ToLower() == "pdf")
                {
                    PdfDocument pdf = new PdfDocument();
                    pdf.LoadFromFile(sourcePath);
                    int pdfdp2 = sourcePath.LastIndexOf('\\');
                    if ((pdfdp2 > 0) && (pdfdp2 < pdfdp))
                    {
                        string filnamn = sourcePath.Substring(pdfdp2 + 1, pdfdp - pdfdp2 - 1);
                        pdf.SaveToFile(targetPath + "\\" + filnamn + ".DOCX");
                        retVal = true;
                    }
                    pdf.Close();
                }
            }
            return retVal;
        }
        #region File Separation Handling
        public bool pushToGDS(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            bool retval = false;
            setInformationText("Document " + sourcePath + " is targeted for GREASE-Document Sollution.", informationType.INFO, sender, e);
            try
            {
                // TODO - Handle push to Grease-Document Sollution correctly.
                string originPath = targetPath;
                int spdp = sourcePath.LastIndexOf('.');
                if (System.IO.File.Exists(targetPath))
                {
                    int ludp = targetPath.LastIndexOf('_');
                    if ((ludp > 0) && (ludp < targetPath.Length - 1))
                    {
                        int lastSubNo = 0;
                        if (int.TryParse(targetPath.Substring(ludp + 1, spdp - ludp - 1), out lastSubNo))
                            targetPath = targetPath.Substring(0, ludp + 1) + lastSubNo.ToString() + targetPath.Substring(spdp);
                        else
                            targetPath = targetPath.Substring(0, spdp) + "_0" + targetPath.Substring(spdp);
                    }
                    else
                        targetPath = targetPath.Substring(0, spdp) + "_1" + targetPath.Substring(spdp);
                }
                System.IO.File.Copy(sourcePath, targetPath, true);
                return retval;
            }
            catch
            {
                return retval;
            }
        }
        public bool pushToDOORS(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            setInformationText("Document " + sourcePath + " is targeted for DOORS.", informationType.INFO, sender, e);
            try
            {
                // TODO - Handle push to DOORS correctly.
                bool retval = false;
                int spdp = sourcePath.LastIndexOf('.');
                if (System.IO.File.Exists(targetPath))
                {
                    int ludp = targetPath.LastIndexOf('_');
                    if ((ludp > 0) && (ludp < targetPath.Length - 1))
                    {
                        int lastSubNo = 0;
                        if (int.TryParse(targetPath.Substring(ludp + 1, spdp - ludp - 1), out lastSubNo))
                            targetPath = targetPath.Substring(0, ludp + 1) + lastSubNo.ToString() + targetPath.Substring(spdp);
                        else
                            targetPath = targetPath.Substring(0, spdp) + "_0" + targetPath.Substring(spdp);
                    }
                    else
                        targetPath = targetPath.Substring(0, spdp) + "_1" + targetPath.Substring(spdp);
                }
                System.IO.File.Copy(sourcePath, targetPath, true);
                return retval;
            }
            catch
            {
                return false;
            }
        }
        public bool pushDocToRedDoc(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            // Note: IFS is really "IFS", "IFS-Surf", and "RedDoc".
            // "IFS" is obsolete and exchanged for "IFS-Surf" that has a built-in handling of this.
            // "RedDoc" needs an excel-spreadcheet with the content listed below.
            // Detect which one from selected target in DocSepCmbBx.SelectedItem.
            // The transfers to "RedDoc" is handled by getting data and putting it into an Excel-spreadsheet.
            // ------------------------------------------
            string targetTool = docSepCmbBx.SelectedItem.ToString();
            if (targetTool == "RedDoc")
                setInformationText("Document " + sourcePath + " is targeted for " + targetTool + ".", informationType.INFO, sender, e);
            else
                return false;
            try
            {
                string sepItemDocNo = "";       // 1 - Document no          : fetch from ClearCase Label.
                string sepItemDocRev = "";      // 2 - Document revision    : fetch from ClearCase Label.
                string sepItemDocClass = "";    // 3 - Document class       : 
                string sepItemDocSheets = "";   // 4 - Document sheets      : 
                string sepItemDocTitle = "";    // 5 - Document title       : Manuell hämtning/se om det går att få ut förslag.
                string sepItemDocResp = "";     // 6 - Responsible person   : Manuell hämtning/se om det går att få ut förslag.
                string sepItemDocStatus = "";   // 7 - Document status      : Om det finns i ClearCase, annars inget; [R = Release, P = Approved, tomt = Preliminary]
                string sepItemDocAccess = "";   // 8 - Document access      : blankt i excel-arket; Utses efter en responsible person, utifrån 10/0123-LXE10401.
                string sepItemDocAccLvl = "";   // 9 - Access level         : blankt i excel-arket; Avgörs av responsible person.
                using (StreamWriter sw = File.AppendText(sDocSepFile))
                {
                    // Get data from the source.
                    if (setCCVOB != null)
                    {
                        List<ClearCase.CCLabelType> ccLblTypeList = CCAutomation.ClearcaseHelper.SearchLabelsBasedOnToken("*", sourcePath);
                        if (ccLblTypeList.Count > 0)
                        {
                            foreach (var lblType in ccLblTypeList)
                            {
                                // Figure out if any label represents any of the formats:
                                // <ProjName>_<ProdNo>_<Rev>|<Rev>|"Rev_"<letter>|<Name1>_<Name2>_<prod-id>|<id>_<no>_<ver>|<edition>_<rev>
                                string pattern = "^[.]+_[.]+";
                                MatchCollection matches = Regex.Matches(lblType.ToString(), pattern);
                                if (matches.Count == 0)
                                {
                                    // We have "<Rev>"
                                    sepItemDocRev = lblType.ToString();
                                }
                                else if (matches.Count == 1)
                                {
                                    // It must be one of
                                    // "Rev_"<letter>||<edition>_<rev>
                                    string revNo = lblType.ToString().Substring(lblType.ToString().IndexOf('_') + 1);
                                    if (revNo.Contains('R'))
                                    {
                                        sepItemDocStatus = "Release";
                                        sepItemDocRev = revNo.Substring(1);
                                    }
                                    else if (revNo.Contains('P'))
                                    {
                                        sepItemDocStatus = "Approved";
                                        sepItemDocRev = revNo.Substring(1);
                                    }
                                    else
                                    {
                                        sepItemDocStatus = "Preliminary";
                                        sepItemDocRev = revNo;
                                    }
                                        
                                }
                                else if (matches.Count == 2)
                                {
                                    // It must be one of
                                    // <ProjName>_<ProdNo>_<Rev>|<Name1>_<Name2>_<prod-id>|<id>_<no>_<ver>
                                    pattern = "^[a-zA-Z]+_[0-9]+_[A-Z]+";
                                    matches = Regex.Matches(lblType.ToString(), pattern);
                                    if (matches.Count > 0)
                                    {
                                        // We must have <ProjName>_<ProdNo>_<Rev>
                                        string workString = lblType.ToString();
                                        workString = workString.Substring(workString.IndexOf('_') + 1);
                                        sepItemDocNo = workString.Substring(0, workString.IndexOf('_'));
                                        sepItemDocRev = workString.Substring(workString.IndexOf('_') + 1);
                                    }
                                    else
                                    {
                                        // Now we are looking at <Name1>_<Name2>_<prod-id> or <id-no>_<no>_<ver>.
                                        pattern = "^[a-zA-Z]+[a-zA-Z]+_[0-9]+";
                                        matches = Regex.Matches(lblType.ToString(), pattern);
                                        if (matches.Count > 0)
                                        {
                                            // We must have <Name1>_<Name2>_<prod-id>
                                            sepItemDocTitle = lblType.ToString().Substring(0, lblType.ToString().LastIndexOf('_'));
                                            sepItemDocNo = lblType.ToString().Substring(lblType.ToString().LastIndexOf('_') + 1);
                                        }
                                        else
                                        {
                                            // We are left with <id-no>_<no>_<ver>
                                            sepItemDocNo = lblType.ToString().Substring(0, lblType.ToString().IndexOf('_'));
                                            sepItemDocRev = lblType.ToString().Substring(lblType.ToString().LastIndexOf('_') + 1);
                                        }
                                    }
                                }
                                // Get the document owner for sepItemDocResp
                                sepItemDocResp = lblType.Owner;
                                string tgtFileName = targetPath.Substring(targetPath.LastIndexOf('\\') + 1);
                                if (tgtFileName.Contains("levdef"))
                                {
                                    // <filnamn>.Contains "levdef" = "Delivery definition "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Delivery definition";
                                    else
                                        sepItemDocTitle = "Delivery definition for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("10960"))
                                {
                                    // <filnamn>.Contains "10960" = "Software unit lists "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Software unit list";
                                    else
                                        sepItemDocTitle = "Software unit list for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("10262"))
                                {
                                    // <filnamn>.Contains "10262" = "Construction specification "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Construction specification";
                                    else
                                        sepItemDocTitle = "Construction specification for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("10264"))
                                {
                                    // <filnamn>.Contains "10264" = "Verification specification "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Verification specification";
                                    else
                                        sepItemDocTitle = "Verification specification for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("10265"))
                                {
                                    // <filnamn>.Contains "10265" = "Verification protocol "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Verification protocol";
                                    else
                                        sepItemDocTitle = "Verification protocol for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("1/12022"))
                                {
                                    // <filnamn>.Contains "1/12022" = "Verification matrix "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Verification matrix";
                                    else
                                        sepItemDocTitle = "Verification matrix for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("DISRS"))
                                {
                                    // <filnamn>.Contains "DISRS" = "Display Interface System Requirement Specification "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Display Interface System Requirement Specification";
                                    else
                                        sepItemDocTitle = "Display Interface System Requirement Specification for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("KD"))
                                {
                                    // <filnamn>.Contains "KD" = "Coordination document "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Coordination document";
                                    else
                                        sepItemDocTitle = "Coordination document for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("OCD"))
                                {
                                    // <filnamn>.Contains "OCD" = "Concept description "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "Concept Description";
                                    else
                                        sepItemDocTitle = "Concept Description for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSDD"))
                                {
                                    // <filnamn>.Contains "SSDD" = "SubSystem Design Description "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Design Description";
                                    else
                                        sepItemDocTitle = "SubSystem Design Description for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSS"))
                                {
                                    // <filnamn>.Contains "SSS" = "SubSystem Specification "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Specification";
                                    else
                                        sepItemDocTitle = "SubSystem Specification for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSTD"))
                                {
                                    // <filnamn>.Contains "SSTD" = "SubSystem Test Description "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Test Description";
                                    else
                                        sepItemDocTitle = "SubSystem Test Description for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSTP"))
                                {
                                    // <filnamn>.Contains "SSTP" = "SubSystem Test Plan "
                                    if (String.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Test Plan";
                                    else
                                        sepItemDocTitle = "SubSystem Test Plan for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSTR"))
                                {
                                    if (string.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Test Report";
                                    else
                                        sepItemDocTitle = "SubSystem Test Report for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("SSVP"))
                                {
                                    // SSVP = "SubSystem Verification Plan"
                                    if (string.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "SubSystem Verification Plan";
                                    else
                                        sepItemDocTitle = "SubSystem Verification Plan for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("STP"))
                                {
                                    // STP = "System Test Plan"
                                    if (string.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "System Test Plan";
                                    else
                                        sepItemDocTitle = "System Test Plan for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("STD"))
                                {
                                    // STD = "System Test Description"
                                    if (string.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "System Test Description";
                                    else
                                        sepItemDocTitle = "System Test Description for " + sepItemDocTitle;
                                }
                                else if (tgtFileName.Contains("STR"))
                                {
                                    // STR = "System Test Report"
                                    if (string.IsNullOrEmpty(sepItemDocTitle))
                                        sepItemDocTitle = "System Test Report";
                                    else
                                        sepItemDocTitle = "System Test Report for " + sepItemDocTitle;
                                }
                                // TODO - Add more versions of filename that indicates content.
                            }
                            sw.WriteLine(sepItemDocNo + "\t" + sepItemDocRev + "\t" + sepItemDocClass + "\t" + sepItemDocSheets + "\t" + sepItemDocTitle + "\t" + sepItemDocResp + "\t" + sepItemDocStatus + "\t" + sepItemDocAccess + "\t" + sepItemDocAccLvl + "\t" + targetPath + "\n");
                        }
                        else
                            sw.WriteLine(sepItemDocNo + "\t" + sepItemDocRev + "\t" + sepItemDocClass + "\t" + sepItemDocSheets + "\t" + sepItemDocTitle + "\t" + sepItemDocResp + "\t" + sepItemDocStatus + "\t" + sepItemDocAccess + "\t" + sepItemDocAccLvl + "\t" + targetPath + "\n");
                    }
                    else
                    {
                        setInformationText("Meta data for document " + sourcePath + " could not be handled.", informationType.ERROR, sender, e);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool pushToIFS(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            // Note: IFS is really "IFS", "IFS-Surf", and "RedDoc".
            // "IFS" is obsolete and exchanged for "IFS-Surf" that has a built-in handling of this.
            // Note: Meeting with Emil Almgren 2025-03-04
            //       - Personal tokens for IFS/IFS-Surf picked out with Corp password.
            //       - Fetch all possible data for each transferred document/binary.
            //       - Create a "card" for the transferred item.
            //       - Each document/binary code is pushed into IFS/IFS-Surf by a series of steps.
            setInformationText("Executable " + sourcePath + " is targeted for IFS/IFS-Surf.", informationType.INFO, sender, e);
            try
            {
                // TODO - Handle push to IFS/IFS-Surf correctly.
                bool retval = false;
                int spdp = sourcePath.LastIndexOf('.');
                if (System.IO.File.Exists(targetPath))
                {
                    int ludp = targetPath.LastIndexOf('_');
                    if ((ludp > 0) && (ludp < targetPath.Length - 1))
                    {
                        int lastSubNo = 0;
                        if (int.TryParse(targetPath.Substring(ludp + 1, spdp - ludp - 1), out lastSubNo))
                        {
                            lastSubNo++;
                            targetPath = targetPath.Substring(0, ludp + 1) + lastSubNo.ToString() + targetPath.Substring(spdp);
                        }
                        else
                            targetPath = targetPath.Substring(0, spdp) + "_0" + targetPath.Substring(spdp);
                    }
                    else
                        targetPath = targetPath.Substring(0, spdp) + "_1" + targetPath.Substring(spdp);
                }
                System.IO.File.Copy(sourcePath, targetPath, true);
                return retval;
            }
            catch
            {
                return false;
            }
        }
        public bool pushToArtifactory(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            setInformationText("Executable " + sourcePath + " is targeted for GREASE-Artifactory.", informationType.INFO, sender, e);
            try
            {
                // TODO - Handle push to Artifactory correctly.
                bool retval = false;
                int spdp = sourcePath.LastIndexOf('.');
                if (System.IO.File.Exists(targetPath))
                {
                    int ludp = targetPath.LastIndexOf('_');
                    if ((ludp > 0) && (ludp < targetPath.Length - 1))
                    {
                        int lastSubNo = 0;
                        if (int.TryParse(targetPath.Substring(ludp + 1, spdp - ludp - 1), out lastSubNo))
                            targetPath = targetPath.Substring(0, ludp + 1) + lastSubNo.ToString() + targetPath.Substring(spdp);
                        else
                            targetPath = targetPath.Substring(0, spdp) + "_0" + targetPath.Substring(spdp);
                    }
                    else
                        targetPath = targetPath.Substring(0, spdp) + "_1" + targetPath.Substring(spdp);
                }
                System.IO.File.Copy(sourcePath, targetPath, true);
                return retval;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        public int runCopyTask(string sourcePath, string targetPath, object sender, EventArgs e)
        {
            int noOfCopiedItems = 0;
            int noOfSeparatedItems = 0;
            string[] sourceFileList = System.IO.Directory.GetFiles(sourcePath);
            if (sourceFileList.Length > 0)
            {
                for (int i = 0; i <= sourceFileList.Length - 1; i++)
                {
                    bool prohibMove = false;
                    string filename = sourceFileList[i].Substring(sourceFileList[i].LastIndexOf('\\'));
                    // --- Separation handling ---
                    string selBinSep = binSepCmbBx.SelectedItem.ToString(); // Binary files.
                    string selDocSep = docSepCmbBx.SelectedItem.ToString(); // Document files.
                    string srcFle = sourceFileList[i].ToString();
                    int dpp = filename.LastIndexOf('.');

                    if (BldScrChkBx.Checked)
                    {
                        // Build related files should be separatated. This can be files stored under a catalog
                        //    called "scripts" and configuration files with the extention ".cnf".
                        setInformationText("Separation of build related files Not fully implemented.", informationType.INFO, sender, e);
                        if ((srcFle.Contains("script")) || (srcFle.Substring(dpp + 1).ToLower() == "cnf"))
                        {
                            System.IO.File.Copy(srcFle, sebBldDir + "\\" + filename, true);
                            noOfCopiedItems++;
                            noOfSeparatedItems++;
                        }
                        prohibMove = true;
                    }
                    if (TstScrSepChkBx.Checked)
                    {
                        // Test related files should be separated. This is files stored under a catalog called
                        //    "test" and with the extention "*.scr" or Jenkins scripts.
                        setInformationText("Separation of test related files Not fully implemented.", informationType.INFO, sender, e);
                        if ((srcFle.ToLower().Contains("test")) || (srcFle.Substring(dpp + 1).ToLower() == "scr") || (srcFle.ToLower().Contains("jenkins")))
                        {
                            System.IO.File.Copy(srcFle, sepTestDir + "\\" + filename, true);
                            noOfCopiedItems++;
                            noOfSeparatedItems++;
                        }
                        prohibMove = true;
                    }
                    if (thrdPrtChkBx.Checked)
                    {
                        // Third party files should be separated, these files are sorted under directories
                        //    with names according to the provider, for example dy4, tornado_221, buster, etc.
                        setInformationText("Separation of third party related files Not fully implemented.", informationType.INFO, sender, e);
                        foreach (string itm in thirdPtyItems)
                        {
                            if (srcFle.Contains(itm))
                            {
                                System.IO.File.Copy(srcFle, sepThrdPrtDir + "\\" + filename, true);
                                noOfSeparatedItems++;
                                noOfCopiedItems++;
                            }
                        }
                        prohibMove = true;
                    }
                    if (devChkBx.Checked)
                    {
                        // TODO - implement separation of development related items.
                        // Development related files should be separated, this is files necessary for measuring the code or
                        //    linking the code to other projects, or IDEs.
                        setInformationText("Separation of development related files Not implemented.", informationType.INFO, sender, e);
                        // använd katalog "sepDevDir"
                        prohibMove = false;
                    }
                    if (selBinSep != "Select")
                    {
                        // Binary file separation selected.
                        if (binItems.Contains<string>(filename.Substring(dpp + 1).ToLower()))
                        {
                            // Current file is a binary file.
                            switch (selBinSep)
                            {
                                case "Artifactory":
                                    {
                                        if (pushToArtifactory(sourceFileList[i].ToString(), setBinRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to Artifactory.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                case "IFS-Surf":
                                case "IFS":
                                    {
                                        if (pushToIFS(sourceFileList[i].ToString(), setBinRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to IFS.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        setInformationText("Faulty document separation choice.", informationType.ERROR, sender, e);
                                        prohibMove = false;
                                    }
                                    break;
                                // TODO - Additional binary target tools?
                            }
                        }
                    }
                    if (selDocSep != "Select")
                    {
                        // Document file separation selected.
                        if (docItems.Contains<string>(filename.Substring(dpp + 1).ToLower()))
                        {
                            // Current file is a document file.
                            switch (selDocSep)
                            {
                                case "Doc. Sollution":
                                    {
                                        if (pushToGDS(sourceFileList[i].ToString(), setDocRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to GREASE-Document Sollution.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                case "DOORS":
                                    {
                                        if (pushToDOORS(sourceFileList[i].ToString(), setDocRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to DOORS.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                case "IFS-Surf":
                                case "IFS":
                                    {
                                        if (pushToIFS(sourceFileList[i].ToString(), setDocRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to IFS.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                case "RedDoc":
                                    {
                                        if (pushDocToRedDoc(sourceFileList[i].ToString(), setDocRepo + "\\" + filename, sender, e))
                                        {
                                            prohibMove = true;
                                            noOfSeparatedItems++;
                                            noOfCopiedItems++;
                                        }
                                        else
                                        {
                                            setInformationText("Faulty move to RedDoc.", informationType.ERROR, sender, e);
                                            prohibMove = false;
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        setInformationText("Faulty document separation choice.", informationType.ERROR, sender, e);
                                        prohibMove = false;
                                    }
                                    break;
                                // TODO - Additional Document target tools?
                            }
                        }
                    }

                    if (!prohibMove)
                    {
                        System.IO.File.Copy(sourceFileList[i].ToString(), targetPath + filename, true);
                        noOfCopiedItems++;
                    }
                }
            }
            string[] sourceDirectoryList = System.IO.Directory.GetDirectories(sourcePath);
            if (sourceDirectoryList.Length > 0)
            {
                for (int i = 0; i <= sourceDirectoryList.Length - 1; i++)
                {
                    string newtargetPath = targetPath + sourceDirectoryList[i].Substring(sourceDirectoryList[i].LastIndexOf('\\'));
                    System.IO.Directory.CreateDirectory(newtargetPath);
                    string newSourcePath = sourceDirectoryList[i];
                    noOfCopiedItems += runCopyTask(newSourcePath, newtargetPath, sender, e);
                }
            }
            return noOfCopiedItems;
        }
        public bool addAllToGit(LibGit2Sharp.Repository GitLocalRepository, string originalPath, string sourcePath)
        {
            bool retVal = false;
            try
            {
                string[] sourceFileList = System.IO.Directory.GetFiles(sourcePath);
                if (sourceFileList.Length > 0)
                {
                    for (int i = 0; i < sourceFileList.Length; i++)
                    {
                        string workFileName = sourceFileList[i].Substring(originalPath.Length + 1);
                        GitLocalRepository.Index.Add(workFileName);
                        GitLocalRepository.Index.Write();
                        retVal = true;
                    }
                }
                string[] sourceDirectoryList = System.IO.Directory.GetDirectories(sourcePath);
                if (sourceDirectoryList.Length > 0)
                {
                    for (int i = 0; i < sourceDirectoryList.Length; i++)
                    {
                        if (!sourceDirectoryList[i].ToString().Contains(".git"))
                        {
                            retVal = addAllToGit(GitLocalRepository, originalPath, sourceDirectoryList[i].ToString());
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return retVal;
        }
        public bool checkIfRepoEqual(LibGit2Sharp.Repository repo, string originalPath, string sourcePath)
        {
            bool retVal = true;
            string[] sourceFileList = System.IO.Directory.GetFiles(sourcePath);
            if (sourceFileList.Length > 0)
            {
                for (int i = 0; i < sourceFileList.Length; i++)
                {
                    string workFileName = sourceFileList[i].Substring(originalPath.Length + 1);
                    retVal = retVal && !repo.Equals(workFileName);
                }
            }
            string[] sourceDirectoryList = System.IO.Directory.GetDirectories(sourcePath);
            if (sourceDirectoryList.Length > 0)
            {
                for (int i = 0; i < sourceDirectoryList.Length; i++)
                {
                    if (!sourceDirectoryList[i].ToString().Contains(".git"))
                    {
                        retVal = retVal && checkIfRepoEqual(repo, originalPath, sourceDirectoryList[i].ToString());
                    }
                }
            }
            return retVal;
        }
        private void setupProcessComboBox(object sender, EventArgs e)
        {
            if ((ClearCaseDriveCmbBx.SelectedIndex > 0) && (ClearCaseVOBCmbBx.SelectedIndex > 0) && (GitVersionRepoCmbBx.SelectedIndex != 0))
            {
                ProcessCmbBx.Items.Clear();
                ProcessCmbBx.Items.Add("Select");
                if (GitVersionTagCmbBx.SelectedItem.ToString() == "Select")
                {
                    workFlow = workFlowType.wft_streamed;
                    setInformationText("Streamed work processes instantiated.", informationType.INFO, sender, e);
                }
                else
                {
                    workFlow = workFlowType.wft_singluar;
                    setInformationText("Singular work processes instantiated.", informationType.INFO, sender, e);
                }

                if (workFlow == workFlowType.wft_singluar)
                {
                    ProcessCmbBx.Items.Add("Copy to Folder");
                    ProcessCmbBx.Items.Add("Copy & Add");
                }
                ProcessCmbBx.Items.Add("Copy, Add, & Commit");
                ProcessCmbBx.Items.Add("Copy, Add, Commit, & Push");

                ProcessCmbBx.SelectedIndex = 0;
                ProcessCmbBx.Enabled = true;
                binSepCmbBx.Enabled = true;
                docSepCmbBx.Enabled = true;
            }
            else
            {
                ProcessCmbBx.Enabled = false;
            }
        }
        private LibGit2Sharp.Credentials CreateUserNamePasswordCredentials()
        {
            return new UsernamePasswordCredentials
            {
                Username = linwin.getUserId(),
                Password = linwin.getUserPwd(),
            };
        }
        private string getGitRepoUrl(string targetFile)
        {
            if (File.Exists(targetFile))
            {
                foreach (string line in File.ReadLines(targetFile))
                {
                    if ((line != null) && (line != "-1") && (line != ""))
                    {
                        if (line.Contains("url"))
                        {
                            if (isSSH)
                            {
                                int dp = line.IndexOf('=');
                                string workStr = "";
                                if ((dp > 0) && (dp < line.Length - 1))
                                    workStr = line.Substring(dp + 2);
                                return workStr;
                            }
                            else
                            {
                                // ssh://git@uxc005.corp.saab.se:2222/project/spk39/frans.git 
                                // borde bli https://uxc005.corp.saab.se/gitlab/project/spk39/frans.git
                                int dp = line.IndexOf('@');
                                string workStr = "";
                                if ((dp > 0) && (dp < line.Length - 1))
                                    workStr = line.Substring(dp + 1);
                                dp = workStr.IndexOf(':');
                                int dps = workStr.IndexOf('/');
                                workStr = "https://" + workStr.Substring(0, dp) + "/gitlab" + workStr.Substring(dps);
                                return workStr;
                            }
                        }
                    }
                }
            }
            return "";
        }
        private bool changeGitRepoUrl (string urlToSet, string targetFile)
        {
            bool retVal = true;
            List<string> changedCont = new List<string>();
            if (File.Exists(targetFile))
            {
                foreach (string line in File.ReadLines(targetFile))
                {
                    if (line != "-1")
                    {
                        if (line.Contains("url"))
                        {
                            int dp = line.LastIndexOf(':');
                            if ((dp > 0) && (dp < line.Length - 1))
                            {
                                urlResetString = line;
                                string strToSet = line.Substring(dp);
                                dp = strToSet.IndexOf('/');
                                strToSet = strToSet.Substring(dp + 1);
                                string toPutBefore = linwin.getURLPath(linwin.getSetGitURL());
                                // borde bli "url = https://uxc005.corp.saab.se/gitlab/project/spk39/frans.git"
                                strToSet = "\turl = " + toPutBefore + strToSet;
                                changedCont.Add(strToSet);
                            }
                        }
                        else
                            changedCont.Add(line);
                    }
                }
                using (FileStream efs = File.Create(targetFile))
                {
                    using (StreamWriter sw = new StreamWriter(efs))
                    {
                        foreach (string s in changedCont)
                            sw.WriteLine(s);
                    }
                }
            }
            else
                retVal = false;
            return retVal;
        }
        private bool resetGitRepoUrl (string targetFile)
        {
            bool retVal = true;
            List<string> changedCont = new List<string>();
            if (File.Exists(targetFile))
            {
                foreach (string line in File.ReadLines(targetFile))
                {
                    if (line != "-1")
                    {
                        if ((urlResetString != "") && (line.Contains("url")))
                            changedCont.Add(urlResetString);
                        else
                            changedCont.Add(line);
                    }
                }
                using (FileStream efs = File.Create(targetFile))
                {
                    using (StreamWriter sw = new StreamWriter(efs))
                    {
                        foreach (string s in changedCont)
                            sw.WriteLine(s);
                    }
                }
            }
            else
                retVal = false;
            return retVal;
        }
        private int checkForChanges (LibGit2Sharp.Repository repo, string targetRepo)
        {
            int retVal = 0;
            try
            {
                string[] sourceFileList = System.IO.Directory.GetFiles(targetRepo);
                for (int i = 0; i < sourceFileList.Length; i++)
                {
                    FileStatus fs = repo.RetrieveStatus(sourceFileList[i].ToString());
                    if ((fs.ToString() != "Unaltered") && (fs.ToString() != "Nonexistent"))
                        retVal++;
                }
                string[] sourceDirectoryList = System.IO.Directory.GetDirectories(targetRepo);
                for (int i = 0; i < sourceDirectoryList.Length; i++)
                {
                    if (!sourceDirectoryList[i].Contains(".git"))
                        retVal += checkForChanges(repo, sourceDirectoryList[i].ToString());
                }
            }
            catch
            {
                throw;
            }
            return retVal;
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
            if (!cchelp.CheckRegionExist())
            {
                DialogResult topAnswer = MessageBox.Show("ClearCase does not exist\nor isn't online in this environment.", "No ClearCase", MessageBoxButtons.OK, MessageBoxIcon.Error);
                clearCaseToolStripMenuItem.Enabled = false;
                progInfoTxtBx.Text = "ClearCase server is not available!";
            }
            else
            {
                // --- Login handling ---
                loginWinOpen();

                userName = linwin.getUserName();
                userEmail = linwin.getUserEMail();
                userid = linwin.getUserId();

                // --- File separation handling ---
                // Handle binary separation combo box content.
                binSepCmbBx.Items.Clear();
                for (int i = 0; i < binTargets.Count; i++)
                    binSepCmbBx.Items.Add(binTargets.ElementAt<string>(i));
                binSepCmbBx.SelectedIndex = 0;
                // Handle document separation combo box content.
                docSepCmbBx.Items.Clear();
                for (int i = 0; i < docTargets.Count; i++)
                    docSepCmbBx.Items.Add(docTargets.ElementAt<string>(i));
                docSepCmbBx.SelectedIndex = 0;

                // --- Root path handling ---
                DirectoryInfo di = new DirectoryInfo(".");
                originalPath = di.FullName.ToString();
            }

            if (linwin.validateUser())
                repositoryUrl = linwin.getURLPath(linwin.getSetGitURL());

            if (userid == "")
            {
                userid = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                int idxPos = userid.IndexOf("\\");
                if ((idxPos > 0) && (idxPos < userid.Length))
                    userid = userid.Substring(idxPos + 1);
            }
            progInfoTxtBx.Text = "User \"" + userid + "\" logged in.";

            userHomeDrive = System.Environment.GetEnvironmentVariable("HOMEDRIVE");
            dirpath = userHomeDrive + "\\ClearCase";

            try
            {
                if (!System.IO.Directory.Exists(dirpath))
                    System.IO.Directory.CreateDirectory(dirpath);
            }
            catch (Exception err)
            {
                string reply_string = "";
                if (err.ToString().ToLower().Contains("kerberos"))
                    reply_string = "Your home drive \"" + userHomeDrive + "\" does not exist or is not online.";
                DialogResult reply = MessageBox.Show(reply_string, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            logpath = dirpath + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            using (StreamWriter sw = File.AppendText(logpath))
                sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd - HH:mm:ss.ff") + "User \"" + userid + "\" logged in.");

            GitVersionTagCmbBx.Items.Clear();
            GitVersionTagCmbBx.Items.Add("Select");
            GitVersionTagCmbBx.SelectedIndex = 0;
        }
        #region FileMenu
        private void openSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = dirpath;
                ofd.Filter = "Text files (*.txt)|*.txt|" +
                             "All files (*.*)|*.*";
                ofd.FilterIndex = 2;
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string loadedFileName = ofd.FileName;
                    if (System.IO.File.Exists(loadedFileName))
                    {
                        RegionCmbBx.Items.Clear();
                        RegionCmbBx.Items.Add("Select");
                        ClearCaseDriveCmbBx.Items.Clear();
                        ClearCaseDriveCmbBx.Items.Add("Select");
                        ClearCaseVOBCmbBx.Items.Clear();
                        ClearCaseVOBCmbBx.Items.Add("Select");
                        ClearCaseViewCmbBx.Items.Clear();
                        ClearCaseViewCmbBx.Items.Add("Select");
                        ClearCaseLabelCmbBx.Items.Clear();
                        ClearCaseLabelCmbBx.Items.Add("Select");
                        WorkList.Items.Clear();
                        GitVersionCommitCmbBx.Items.Clear();
                        GitVersionCommitCmbBx.Items.Add("Select");
                        GitVersionBranchCmbBx.Items.Clear();
                        GitVersionBranchCmbBx.Items.Add("Select");
                        GitVersionTagCmbBx.Items.Clear();
                        GitVersionTagCmbBx.Items.Add("Select");
                        foreach (string line in System.IO.File.ReadLines(loadedFileName))
                        {
                            if (line != "-1")
                            {
                                int dp;
                                dp = line.IndexOf(":");
                                if ((dp > 0) && (dp < line.Length - 2))
                                {
                                    string dataTag = line.Substring(0, dp - 1).ToLower();
                                    string dataInfo = line.Substring(dp + 1);
                                    switch (dataTag)
                                    {
                                        case "lastwordir":
                                            {
                                                linwin.setLastGitRepo(dataInfo);
                                                lastworkdir = dataInfo;
                                                //setInformationText("Last work dir and Git repo was loaded.", informationType.INFO, sender, e);
                                            }
                                            break;
                                        case "setccversion":
                                            {
                                                selectedCCVersion = dataInfo;
                                                ClearCaseGrpBx.Enabled = true;
                                                ClearCaseGrpBx.Text = "ClearCase " + selectedCCVersion;
                                                //setInformationText("Previous ClearCase Version setting " + selectedCCVersion + " was loaded.", informationType.INFO, sender, e);
                                            }
                                            break;
                                        case "loadedccregions":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < RegionCmbBx.Items.Count; i++)
                                                {
                                                    if (RegionCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    RegionCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedccdrives":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < ClearCaseDriveCmbBx.Items.Count; i++)
                                                {
                                                    if (ClearCaseDriveCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    ClearCaseDriveCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedccvobs":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < ClearCaseVOBCmbBx.Items.Count; i++)
                                                {
                                                    if (ClearCaseVOBCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    ClearCaseVOBCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedccviews":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < ClearCaseViewCmbBx.Items.Count; i++)
                                                {
                                                    if (ClearCaseViewCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    ClearCaseViewCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedcclabels":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < ClearCaseLabelCmbBx.Items.Count; i++)
                                                {
                                                    if (ClearCaseLabelCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    ClearCaseLabelCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedworklistitems":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < WorkList.Items.Count; i++)
                                                {
                                                    if (WorkList.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                {
                                                    WorkList.Items.Add(dataInfo);
                                                    int dp2 = dataInfo.IndexOf('(');
                                                    int dp3 = dataInfo.IndexOf(')');
                                                    processItems[noOfProcessListContent].Name = dataInfo.Substring(0, dp2 - 1);
                                                    processItems[noOfProcessListContent].GitTagName = dataInfo.Substring(0, dp2 - 1);
                                                    processItems[noOfProcessListContent++].Type = dataInfo.Substring(dp2 + 1, dp3 - dp2 - 1);
                                                }
                                            }
                                            break;
                                        case "setgitversion":
                                            {
                                                selectedGitVersion = dataInfo;
                                                if (selectedGitWebTool != "")
                                                    GitVersionGrpBx.Enabled = true;
                                                //setInformationText("Previous Git Version " + selectedGitVersion + " was set.", informationType.INFO, sender, e);
                                            }
                                            break;
                                        case "setgitwebinterface":
                                            {
                                                selectedGitWebTool = dataInfo;
                                                if (selectedGitVersion != "")
                                                    GitVersionGrpBx.Enabled = true;
                                                //setInformationText("Previous Git Webtool " + selectedGitWebTool + " was set.", informationType.INFO, sender, e);
                                            }
                                            break;
                                        case "loadedgitcommits":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < GitVersionCommitCmbBx.Items.Count; i++)
                                                {
                                                    if (GitVersionCommitCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    GitVersionCommitCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedgitbranches":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < GitVersionBranchCmbBx.Items.Count; i++)
                                                {
                                                    if (GitVersionBranchCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if (!foundItem)
                                                    GitVersionBranchCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                        case "loadedgittags":
                                            {
                                                bool foundItem = false;
                                                for (int i = 0; i < GitVersionTagCmbBx.Items.Count; i++)
                                                {
                                                    if (GitVersionTagCmbBx.Items[i].ToString() == dataInfo)
                                                        foundItem = true;
                                                }
                                                if ((!foundItem) && (!dataInfo.Contains("/refs/tags/")))
                                                    GitVersionTagCmbBx.Items.Add(dataInfo);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        linwin.saveUserData();
                        if (RegionCmbBx.Items.Count > 1)
                        {
                            RegionCmbBx.SelectedIndex = 0;
                            RegionCmbBx.Enabled = true;
                        }
                        else
                            RegionCmbBx.Enabled = false;
                        if (ClearCaseDriveCmbBx.Items.Count > 1)
                        {
                            ClearCaseDriveCmbBx.SelectedIndex = 0;
                            ClearCaseDriveCmbBx.Enabled = true;
                        }
                        else
                            ClearCaseDriveCmbBx.Enabled = false;
                        if (ClearCaseVOBCmbBx.Items.Count > 1)
                        {
                            ClearCaseVOBCmbBx.SelectedIndex = 0;
                            ClearCaseVOBCmbBx.Enabled = true;
                        }
                        else
                            ClearCaseVOBCmbBx.Enabled = false;
                        if (ClearCaseViewCmbBx.Items.Count > 1)
                        {
                            ClearCaseViewCmbBx.SelectedIndex = 0;
                            ClearCaseViewCmbBx.Enabled = true;
                        }
                        else
                            ClearCaseViewCmbBx.Enabled = false;
                        if (ClearCaseLabelCmbBx.Items.Count > 1)
                        {
                            ClearCaseLabelCmbBx.SelectedIndex = 0;
                            ClearCaseLabelCmbBx.Enabled = true;
                        }
                        else
                            ClearCaseLabelCmbBx.Enabled = false;
                        if (WorkList.Items.Count > 1)
                        {
                            WorkList.SelectedIndex = 0;
                            WorkList.Enabled = true;
                        }
                        else
                            WorkList.Enabled = false;
                        if (GitVersionCommitCmbBx.Items.Count > 1)
                        {
                            GitVersionCommitCmbBx.SelectedIndex = 0;
                            GitVersionCommitCmbBx.Enabled = true;
                        }
                        else
                            GitVersionCommitCmbBx.Enabled = false;
                        if (GitVersionBranchCmbBx.Items.Count > 1)
                        {
                            GitVersionBranchCmbBx.SelectedIndex = 0;
                            GitVersionBranchCmbBx.Enabled = true;
                        }
                        else
                            GitVersionBranchCmbBx.Enabled = false;
                        if (GitVersionTagCmbBx.Items.Count > 1)
                        {
                            GitVersionTagCmbBx.SelectedIndex = 0;
                            GitVersionTagCmbBx.Enabled = true;
                        }
                        else
                            GitVersionTagCmbBx.Enabled = false;
                    }
                }
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void saveSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                UseWaitCursor = true;
                saveSessionData(sender, e);
                UseWaitCursor = false;
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void closeSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeThisSession(sender, e);
        }
        #endregion
        #region ClearCaseMenu
        private void version80ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedCCVersion = "8.0";
                ClearCaseGrpBx.Enabled = true;
                ClearCaseGrpBx.Text = "ClearCase 8.0";
                setInformationText("ClearCase version " + selectedCCVersion + " was set.", informationType.INFO, sender, e);
                getCCData(sender, e);
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version90ToolStripMenuItem_Click(object sender, EventArgs e)
        {if (linwin.validateUser())
            {
                selectedCCVersion = "9.0";
                ClearCaseGrpBx.Enabled = true;
                ClearCaseGrpBx.Text = "ClearCase 9.0";
                setInformationText("ClearCase version " + selectedCCVersion + " was set.", informationType.INFO, sender, e);
                getCCData(sender, e);
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedCCVersion = "10.0";
                ClearCaseGrpBx.Enabled = true;
                ClearCaseGrpBx.Text = "ClearCase 10.0";
                setInformationText("ClearCase version " + selectedCCVersion + " was set.", informationType.INFO, sender, e);
                getCCData(sender, e);
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version110ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedCCVersion = "11.0";
                ClearCaseGrpBx.Enabled = true;
                ClearCaseGrpBx.Text = "ClearCase 11.0";
                setInformationText("ClearCase version " + selectedCCVersion + " was set.", informationType.INFO, sender, e);
                getCCData(sender, e);
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        #endregion
        #region GitVersionMenu
        private void version244ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedGitVersion = "2.44";
                setInformationText("Git version " + selectedGitVersion + " was set.", informationType.INFO, sender, e);
                //if (selectedGitWebTool != "")
                //{
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion;// + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                //}
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version245ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedGitVersion = "2.45";
                setInformationText("Git version " + selectedGitVersion + " was set.", informationType.INFO, sender, e);
                // if (selectedGitWebTool != "")
                //{
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion;// + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                //}
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version246ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedGitVersion = "2.46";
                setInformationText("Git version " + selectedGitVersion + " was set.", informationType.INFO, sender, e);
                //if (selectedGitWebTool != "")
                //{
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion; // + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                //}
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void version247ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedGitVersion = "2.47";
                setInformationText("Git version " + selectedGitVersion + " was set.", informationType.INFO, sender, e);
                //if (selectedGitWebTool != "")
                //{
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion;//  + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                //}
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void gitLabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linwin.validateUser())
            {
                selectedGitWebTool = "GitLab";
                setInformationText("Git interface version " + selectedGitWebTool + " was set.", informationType.INFO, sender, e);
                if (selectedGitVersion != "")
                {
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                }
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void bitbucketToolStripMenuItem_Click(object sender, EventArgs e)
        {if (linwin.validateUser())
            {
                selectedGitWebTool = "Bitbucket";
                setInformationText("Git interface version " + selectedGitWebTool + " was set.", informationType.INFO, sender, e);
                if (selectedGitVersion != "")
                {
                    GitVersionGrpBx.Text = "Git Version " + selectedGitVersion + " - " + selectedGitWebTool;
                    getGitData(sender, e);
                }
            }
            else
                setInformationText("User validation needed.", informationType.ERROR, sender, e);
        }
        private void sSHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isSSH = true;
        }
        private void hTTPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isSSH = false;
        }
        #endregion
        #region HelpMenu
        private void documentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string tempurl = originalPath + "\\CC2Git_Info.html";
            try
            {
                Process.Start(tempurl);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    tempurl = tempurl.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(tempurl) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", tempurl);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", tempurl);
                }
                else
                    throw;
            }

        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            abwin = new AboutWindow(userid, userName, userEmail, linwin.getUserPwd(), originalPath);
            abwin.Show();
        }
        #endregion
        #region ClearCaseSelectionSetting
        private void RegionCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setReg = RegionCmbBx.SelectedItem.ToString();
            if (setReg != "Select")
            {
                /* ---
                ListOfCCVOBs = CCAutomation.ClearcaseHelper.GetAllVOBs(setReg);
                if (ListOfCCVOBs.Count > 0)
                {
                    ClearCaseVOBCmbBx.Items.Clear();
                    ClearCaseVOBCmbBx.Items.Add("Select");
                    foreach (string cv in ListOfCCVOBs)
                        ClearCaseVOBCmbBx.Items.Add(cv);
                    ClearCaseVOBCmbBx.SelectedIndex = 0;
                    ClearCaseVOBCmbBx.Enabled = true;
                }
                   --- */

                DriveInfo[] di = System.IO.DriveInfo.GetDrives();
                ClearCaseDriveCmbBx.Items.Clear();
                ClearCaseDriveCmbBx.Items.Add("Select");
                int foundCCDrives = 0;
                for (int i = 0; i < di.Length; i++)
                {
                    try
                    {
                        if (di[i].VolumeLabel == "CCase")
                        {
                            ClearCaseDriveCmbBx.Items.Add(di[i].Name);
                            foundCCDrives++;
                        }
                    }
                    catch (Exception err)
                    {
                        setInformationText(err.ToString(), informationType.INFO, sender, e);
                    }
                }
                if (foundCCDrives > 0)
                {
                    ClearCaseDriveCmbBx.SelectedIndex = 0;
                    ClearCaseDriveCmbBx.Enabled = true;
                }
                RegionCmbBx.Enabled = false;
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void ClearCaseDriveCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setBaseDrive = ClearCaseDriveCmbBx.SelectedItem.ToString();
            if (setBaseDrive != "Select")
            {
                string sbd = setBaseDrive.Substring(0, 2);
                CCAutomation.ClearcaseHelper.BaseDrive = sbd;

                string[] vobsInDrive = System.IO.Directory.GetDirectories(setBaseDrive);
                if (vobsInDrive.Count() > 0)
                {
                    ClearCaseVOBCmbBx.Items.Clear();
                    ClearCaseVOBCmbBx.Items.Add("Select");
                    for (int i = 0; i < vobsInDrive.Count(); i++)
                    {
                        string setString = vobsInDrive[i].ToString().Substring(vobsInDrive[i].ToString().IndexOf('\\') + 1);
                        ClearCaseVOBCmbBx.Items.Add("\\" + setString);
                    }
                    ClearCaseVOBCmbBx.SelectedIndex = 0;
                    ClearCaseVOBCmbBx.Enabled = true;
                }
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void ClearCaseVOBCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setCCVOB = ClearCaseVOBCmbBx.SelectedItem.ToString();
            if (setCCVOB != "Select")
            {
                ClearcaseHelper.MountVob(setCCVOB);
                // --- Find and list all existing Views ---
                ListOfCCViews = CCAutomation.ClearcaseHelper.GetAllViews(setReg);
                if (ListOfCCViews.Count > 0)
                {
                    ClearCaseViewCmbBx.Items.Clear();
                    ClearCaseViewCmbBx.Items.Add("Select");
                    foreach (string cv in ListOfCCViews)
                        ClearCaseViewCmbBx.Items.Add(cv);
                    ClearCaseViewCmbBx.SelectedIndex = 0;
                    ClearCaseViewCmbBx.Enabled = true;
                }
                AddEditViewBtn.Text = "Create View";
                AddEditViewBtn.Enabled = true;
                // --- Find and list the existing labels in the selected VOB ---
                try
                {
                    ListOfCCLabels = ClearcaseHelper.searchLabels(setCCVOB);
                }
                catch (Exception err)
                {
                    setInformationText(err.ToString(), informationType.ERROR, sender, e);
                }
                if ((ListOfCCLabels != null) && (ListOfCCLabels.Count > 0))
                {
                    ClearCaseLabelCmbBx.Items.Clear();
                    ClearCaseLabelCmbBx.Items.Add("Select");
                    foreach (var cl in ListOfCCLabels)
                    {
                        if (!((cl.ToString().ToLower().Contains("BACKSTOP")) || (cl.ToString().ToLower().Contains("CHECKEDOUT")) || (cl.ToString().ToLower().Contains("ClearCase2Git"))))
                            ClearCaseLabelCmbBx.Items.Add(cl.Name.ToString());
                    }
                    ClearCaseLabelCmbBx.SelectedIndex = 0;
                    ClearCaseLabelCmbBx.Enabled = true;
                }
                else
                {
                    ClearCaseLabelCmbBx.BackColor = Color.Red;
                    ClearCaseLabelCmbBx.Enabled = false;
                }
                gitRelatedToolStripMenuItem.Enabled = true;
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void ClearCaseViewCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setCCView = ClearCaseViewCmbBx.SelectedItem.ToString();
            if (setCCView != "Select")
            {
                AddEditViewBtn.Text = "Edit View";
                AddLabelBtn.Text = "Add view";
                AddLabelBtn.Enabled = true;
                if (ListOfCCLabels.Count() == 0)
                {
                    try
                    {
                        ListOfCCLabels = ClearcaseHelper.searchLabels(setCCVOB);
                    }
                    catch (Exception err)
                    {
                        setInformationText(err.ToString(), informationType.ERROR, sender, e);
                    }
                    if ((ListOfCCLabels != null) && (ListOfCCLabels.Count > 0))
                    {
                        ClearCaseLabelCmbBx.Items.Clear();
                        ClearCaseLabelCmbBx.Items.Add("Select");
                        foreach (var cl in ListOfCCLabels)
                            ClearCaseLabelCmbBx.Items.Add(cl.Name.ToString());
                        ClearCaseLabelCmbBx.SelectedIndex = 0;
                        ClearCaseLabelCmbBx.Enabled = true;
                    }
                    else
                    {
                        ClearCaseLabelCmbBx.BackColor = Color.Red;
                        ClearCaseLabelCmbBx.Enabled = false;
                    }
                }
            }
            else
            {
                AddEditViewBtn.Text = "Create View";
                if (AddLabelBtn.Text == "Add view")
                    AddLabelBtn.Enabled = false;
            }
            AddEditViewBtn.Enabled = true;
            Cursor.Current = Cursors.Arrow;
        }
        private void AddEditViewBtn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string setViewAction = AddEditViewBtn.Text;
            if (setViewAction == "Create View")
            {
                // TODO - Start a window for adding a view.
                setInformationText("\"Create View\" has not been implemented yet.", informationType.INFO, sender, e);
            }
            else
            {
                // TODO - Start a window for editing the selected view.
                setInformationText("\"Edit View\" has not been implemented yet.", informationType.INFO, sender, e);
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void ClearCaseLabelCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setCCLabel = ClearCaseLabelCmbBx.SelectedItem.ToString();
            if (setCCLabel != "Select")
            {
                AddLabelBtn.Text = "Add label";
                AddLabelBtn.Enabled = true;
            }
            else if (AddLabelBtn.Text == "Add label")
                AddLabelBtn.Enabled = false;
            Cursor.Current = Cursors.Arrow;
        }
        private void AddLabelBtn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (AddLabelBtn.Text == "Add view")
            {
                processItems[noOfProcessListContent].Name = ClearCaseViewCmbBx.SelectedItem.ToString();
                processItems[noOfProcessListContent].GitTagName = "Unknown";
                processItems[noOfProcessListContent].Type = "View";
                noOfProcessListContent++;
                WorkList.Items.Add(ClearCaseViewCmbBx.SelectedItem.ToString() + "(View)\n");
            }
            else if (AddLabelBtn.Text == "Add label")
            {
                processItems[noOfProcessListContent].Name = ClearCaseLabelCmbBx.SelectedItem.ToString();
                processItems[noOfProcessListContent].GitTagName = ClearCaseLabelCmbBx.SelectedItem.ToString();
                processItems[noOfProcessListContent].Type = "Label";
                noOfProcessListContent++;
                WorkList.Items.Add(ClearCaseLabelCmbBx.SelectedItem.ToString() + "(L)\n");
            }
            AddLabelBtn.Enabled = false;
            if (WorkList.Items.Count > 0)
            {
                WorkList.Enabled = true;
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void WorkList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkList.SelectedItem != null)
            {
                RemoveLabelBtn.Enabled = true;
            }
        }
        private void RemoveLabelBtn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if ((noOfProcessListContent > 0) && (WorkList.Items.Count > 0))
            {
                int itemToRemove = WorkList.SelectedIndex;
                for (int i = itemToRemove; i < noOfProcessListContent; i++)
                {
                    processItems[i].Name = processItems[i + 1].Name;
                    processItems[i].GitTagName = processItems[i + 1].GitTagName;
                    processItems[i].Type = processItems[i + 1].Type;
                }
                processItems[noOfProcessListContent - 1].Name = processItems[noOfProcessListContent].Name;
                processItems[noOfProcessListContent - 1].GitTagName = processItems[noOfProcessListContent].GitTagName;
                processItems[noOfProcessListContent - 1].Type = processItems[noOfProcessListContent].Type;
                noOfProcessListContent--;
                WorkList.Items.RemoveAt(itemToRemove);
                RemoveLabelBtn.Enabled = false;
                if (WorkList.Items.Count == 0)
                {
                    RemoveLabelBtn.Enabled = false;
                    WorkList.Enabled = false;
                }

                if (GitVersionTagCmbBx.Enabled == true)
                {
                    GitVersionTagCmbBx.Enabled = false;
                    GitVersionAddTagBtn.Enabled = false;
                    GitVersionTagCmbBx.Items.Clear();
                    GitVersionTagCmbBx.Items.Add("Select");
                    for (int i = 0; i < noOfProcessListContent; i++)
                        GitVersionTagCmbBx.Items.Add(processItems[i].Name);
                    GitVersionTagCmbBx.SelectedIndex = 0;
                    GitVersionTagCmbBx.Enabled = true;
                    GitVersionAddTagBtn.Enabled = true;
                }
            }
            else
                RemoveLabelBtn.Enabled = false;
            Cursor.Current = Cursors.Arrow;
        }
        #endregion
        #region GitVersionSetting
        private void AddGitProjectBtn_Click(object sender, EventArgs e)
        {
            // TODO - Start a "Add Git Project" window.
        }
        private void GitVersionProjectCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setGitProj = GitVersionProjectCmbBx.SelectedItem.ToString();
            if ((setGitProj != "Select") && (!GitVersionRepoCmbBx.Enabled))
            {
                if ((ClearCaseVOBCmbBx.SelectedItem.ToString() != "Select") && (ClearCaseDriveCmbBx.SelectedItem.ToString() != "Select"))
                    AddGitRepoBtn.Enabled = true;
                else
                    AddGitRepoBtn.Enabled = false;
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void AddGitRepoBtn_Click(object sender, EventArgs e)
        {
            if (AddGitRepoBtn.Text == "Add Repo")
            {
                string drivetouse = "";
                if (linwin.getLastGitRepo() != "")
                    drivetouse = linwin.getLastGitRepo();
                // TODO - Handle the last drive settings.
                //else
                //    drivetouse = ClearCaseDriveCmbBx.SelectedItem.ToString();
                string subVobs = GitVersionProjectCmbBx.SelectedItem.ToString().ToLower() + "//" + ClearCaseVOBCmbBx.SelectedItem.ToString().ToLower().Substring(1);
                cc2git_addRepo addis = new cc2git_addRepo(drivetouse, subVobs, linwin.getURLPath(linwin.getSetGitURL()));
                addis.Show();
                AddGitRepoBtn.Text = "Update Repos";
                AddGitRepoBtn.BackColor = Color.LightCyan;
            }
            else if (AddGitRepoBtn.Text == "Update Repos")
            {
                bool thisIsRun = false;
                if (linwin.updateUserGitStorages())
                {
                    if ((linwin.getNoOfUserGitStorages() > 0) && (!thisIsRun))
                    {
                        GitVersionRepoCmbBx.Items.Clear();
                        GitVersionRepoCmbBx.Items.Add("Select");
                        for (int i = 0; i < linwin.getNoOfUserGitStorages(); i++)
                        {
                            if (linwin.getUserGitStorageType(i) == "Git Repo")
                                GitVersionRepoCmbBx.Items.Add("(G) " + linwin.getUserGitStorageName(i));
                            else if (linwin.getUserGitStorageType(i) == "Normal Repo")
                                GitVersionRepoCmbBx.Items.Add("(N) " + linwin.getUserGitStorageName(i));
                            else
                                GitVersionRepoCmbBx.Items.Add(linwin.getUserGitStorageName(i));
                        }
                        GitVersionRepoCmbBx.SelectedIndex = 0;
                        GitVersionRepoCmbBx.Enabled = true;
                        thisIsRun = true;
                    }
                    AddGitRepoBtn.Enabled = true;
                    AddGitRepoBtn.Text = "Add Repo";
                    AddGitRepoBtn.BackColor = Color.LightGray;
                }
            }
        }
        private void GitVersionRepoCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            int selectedGitRepoIndex = GitVersionRepoCmbBx.SelectedIndex;
            if (selectedGitRepoIndex > 0)
            {
                setGitRepo = linwin.getUserGitStoragePath(selectedGitRepoIndex - 1);
                string nyTstString = myGitHub.GetStatus(setGitRepo);
                // --- Set up setTargetRepository ---
                string newTgtRepo = ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setGitRepo;
                int dp = newTgtRepo.IndexOf(':');
                if((dp > 0) && (dp < newTgtRepo.Length - 1))
                    newTgtRepo = newTgtRepo.Substring(dp - 1);
                try
                {
                    setTargetRepository = new LibGit2Sharp.Repository(newTgtRepo);
                }
                catch (Exception err)
                {
                    setInformationText("Faulty target repository: " + err.ToString(), informationType.ERROR, sender, e);
                    throw;
                }
                // --- Handle Commit combo box ---
                IQueryableCommitLog tgtCommits = setTargetRepository.Commits;
                int noOfCommits = tgtCommits.Count<LibGit2Sharp.Commit>();
                if (noOfCommits > 0)
                {
                    GitVersionCommitCmbBx.Items.Clear();
                    GitVersionCommitCmbBx.Items.Add("Select");
                    LibGit2Sharp.Commit[] commitArray = tgtCommits.ToArray<LibGit2Sharp.Commit>();
                    for (int i = 0; i < noOfCommits; i++)
                    {
                        GitVersionCommitCmbBx.Items.Add(commitArray[i].ToString());
                    }
                    GitVersionCommitCmbBx.SelectedIndex = 0;
                    GitVersionCommitCmbBx.Enabled = true;
                }
                else
                    GitVersionCommitCmbBx.Enabled = false;
                // --- Handle Branch combo box ---
                LibGit2Sharp.BranchCollection tgtBranchColl = setTargetRepository.Branches;
                int noOfBranches = tgtBranchColl.Count<LibGit2Sharp.Branch>();
                if (noOfBranches > 0)
                {
                    GitVersionBranchCmbBx.Items.Clear();
                    GitVersionBranchCmbBx.Items.Add("Select");
                    for (int i = 0; i < noOfBranches; i++)
                    {
                        LibGit2Sharp.Branch currBranch = tgtBranchColl.ElementAt<LibGit2Sharp.Branch>(i);
                        if (!currBranch.ToString().Contains("remotes"))
                            GitVersionBranchCmbBx.Items.Add(currBranch.ToString());
                    }
                    GitVersionBranchCmbBx.SelectedIndex = 0;
                    GitVersionBranchCmbBx.Enabled = true;
                }
                else
                    GitVersionBranchCmbBx.Enabled = false;
                // --- Tag Combo Box handling ---
                GitVersionTagTxtBx.Visible = false;
                GitVersionTagCmbBx.Items.Clear();
                GitVersionTagCmbBx.Items.Add("Select");
                LibGit2Sharp.TagCollection tgtTags = setTargetRepository.Tags;
                int noOfTags = tgtTags.Count<LibGit2Sharp.Tag>();
                if (noOfTags > 0)
                {
                    for (int i = 0; i < noOfTags; i++)
                    {
                        if (!tgtTags.ElementAt<LibGit2Sharp.Tag>(i).ToString().Contains("refs/tags/"))
                            GitVersionTagCmbBx.Items.Add(tgtTags.ElementAt<LibGit2Sharp.Tag>(i).ToString());
                    }
                }
                if (WorkList.Items.Count > 0)
                {
                    for (int i = 0; i < WorkList.Items.Count; i++)
                    {
                        string workStr = WorkList.Items[i].ToString();
                        int dpws1 = workStr.IndexOf('(');
                        int dpws2 = workStr.IndexOf(')');
                        string workTag = workStr.Substring(0, dpws1);
                        string workType = workStr.Substring(dpws1 + 1, dpws2 - dpws1 - 1);
                        if (!workTag.Contains("refs/tags/"))
                        {
                            if (workType == "L")
                                GitVersionTagCmbBx.Items.Add(workTag + "(L)");
                            else if (workType == "View")
                                GitVersionTagCmbBx.Items.Add(workTag + "(V)");
                            else
                                GitVersionTagCmbBx.Items.Add(workTag);
                        }
                    }
                }
                GitVersionTagCmbBx.SelectedIndex = 0;
                GitVersionTagCmbBx.Enabled = true;
                GitVersionTagCmbBx.Visible = true;
                // --- Process Combo Box handling ---
                setupProcessComboBox(sender, e);
            }
            Cursor.Current = Cursors.Arrow;
        }
        private void GitVersionCommitCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setGitCommit = GitVersionCommitCmbBx.SelectedItem.ToString();
            setupProcessComboBox(sender, e);
            Cursor.Current = Cursors.Arrow;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setGitBranch = GitVersionBranchCmbBx.SelectedItem.ToString();
            setupProcessComboBox(sender, e);
            Cursor.Current = Cursors.Arrow;
        }
        private void GitVersionTagCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            setGitTag = GitVersionTagCmbBx.SelectedItem.ToString();
            if (setGitTag != "Select")
            {
                GitVersionAddTagBtn.Text = "Change to Git Tag";
                GitVersionAddTagBtn.Enabled = true;
            }
            setupProcessComboBox(sender, e);
            Cursor.Current = Cursors.Arrow;
        }
        private void GitVersionTagTxtBx_TextChanged(object sender, EventArgs e)
        {
            // Do nothing
        }
        private void GitVersionAddTagBtn_Click(object sender, EventArgs e)
        {
            if ((GitVersionTagTxtBx.Enabled == true) && (GitVersionAddTagBtn.Text == "Edit item to Git Tag"))
            {
                // --- Handle Label change setting ---
                processItems[GitVersionTagCmbBx.SelectedIndex - 1].GitTagName = GitVersionTagTxtBx.Text;
                processItems[GitVersionTagCmbBx.SelectedIndex - 1].Type = "Git Tag";
                // --- Reset GitVersionTagCmbBx ---
                GitVersionAddTagBtn.Enabled = false;
                GitVersionTagCmbBx.Items.Clear();
                GitVersionTagCmbBx.Items.Add("Select");
                for (int i = 0; i < noOfProcessListContent; i++)
                {
                    if ((processItems[i].GitTagName != null) && (processItems[i].GitTagName != ""))
                        GitVersionTagCmbBx.Items.Add(processItems[i].GitTagName);
                    else
                        GitVersionTagCmbBx.Items.Add(processItems[i].Name);
                }
                GitVersionTagCmbBx.SelectedIndex = 0;
                GitVersionTagCmbBx.Visible = true;
                GitVersionTagCmbBx.Enabled = true;
                GitVersionTagCmbBx.Focus();
                // --- Remove GitVErsionTagTxtBx ---
                GitVersionTagTxtBx.Text = "";
                GitVersionTagTxtBx.Enabled = false;
                GitVersionTagTxtBx.Visible = false;
                GitVersionAddTagBtn.Text = "Change to Git Tag";
                GitVersionAddTagBtn.Enabled = false;
            }
            else if (GitVersionAddTagBtn.Text == "Change to Git Tag")
            {
                GitVersionAddTagBtn.Text = "Edit item to Git Tag";
                string tempstr = GitVersionTagCmbBx.SelectedItem.ToString();
                GitVersionTagCmbBx.Visible = false;
                GitVersionTagTxtBx.Enabled = true;
                GitVersionTagTxtBx.Visible = true;
                GitVersionTagTxtBx.Text = tempstr;
                GitVersionTagTxtBx.Focus();
            }
        }
        private void SSHkeygenBtn_Click(object sender, EventArgs e)
        {
            mySSHkeys = RsaKeyPair();
        }
        #endregion

        #region ProgramHandling
        private void ProcessCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            string setWorkStep = ProcessCmbBx.SelectedItem.ToString();
            if (setWorkStep != "Select")
            {
                switch (setWorkStep)
                {
                    case "Copy to Folder":
                        {
                            workProcSteps = workStepType.cpyOnly;
                            if (GitVersionTagCmbBx.SelectedIndex > 0)
                            {
                                int setTagOrViewNo = GitVersionTagCmbBx.SelectedIndex - 1;
                                if (processItems[setTagOrViewNo].Type == "View")
                                {
                                    setInformationText("Selected copy of files in View \"" + processItems[setTagOrViewNo].Name + "\" to a standard folder.", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Label")
                                {
                                    setInformationText("Selected copy of files with Label \"" + processItems[setTagOrViewNo].Name + "\" to a standard folder, no Git Tag set.", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Git Tag")
                                {
                                    setInformationText("Selected copy of files with Label \"" + processItems[setTagOrViewNo].Name + "\" to a standard folder, Git Tag \"" + processItems[setTagOrViewNo].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                }
                                else
                                    setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                            }
                            else
                            {
                                for (int i = 0; i < noOfProcessListContent; i++)
                                {
                                    if (processItems[i].Type == "View")
                                    {
                                        setInformationText("Streamed copy of files in ClearCase View \"" + processItems[i].Name + "\" to a standard folder.", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Label")
                                    {
                                        setInformationText("Streamed copy of files with ClearCase Label \"" + processItems[i].Name + "\" to a standard folder, no Git Tag set.", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Git Tag")
                                    {
                                        setInformationText("Streamed copy of files with ClearCase Label \"" + processItems[i].Name + "\" to a standard folder, Git Tag \"" + processItems[i].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                    }
                                    else
                                        setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                                }
                            }
                        }
                        break;
                    case "Copy & Add":
                        {
                            workProcSteps = workStepType.addToGitStag;
                            if (GitVersionTagCmbBx.SelectedIndex > 0)
                            {
                                int setTagOrViewNo = GitVersionTagCmbBx.SelectedIndex - 1;
                                if (processItems[setTagOrViewNo].Type == "View")
                                {
                                    setInformationText("Selected copy and Add of files in View \"" + processItems[setTagOrViewNo].Name + "\".", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Label")
                                {
                                    setInformationText("Selected copy and Add of files with Label \"" + processItems[setTagOrViewNo].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Git Tag")
                                {
                                    setInformationText("Selected copy and Add of files with Label \"" + processItems[setTagOrViewNo].Name + "\", Git Tag \"" + processItems[setTagOrViewNo].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                }
                                else
                                    setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                            }
                            else
                            {
                                for (int i = 0; i < noOfProcessListContent; i++)
                                {
                                    if (processItems[i].Type == "View")
                                    {
                                        setInformationText("Streamed copy and Add of files in ClearCase View \"" + processItems[i].Name + "\".", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Label")
                                    {
                                        setInformationText("Streamed copy and Add of files with ClearCase Label \"" + processItems[i].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Git Tag")
                                    {
                                        setInformationText("Streamed copy and Add of files with ClearCase Label \"" + processItems[i].Name + "\", Git Tag \"" + processItems[i].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                    }
                                    else
                                        setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                                }
                            }
                        }
                        break;
                    case "Copy, Add, & Commit":
                        {
                            workProcSteps = workStepType.commitToGitLocal;
                            if (GitVersionTagCmbBx.SelectedIndex > 0)
                            {
                                int setTagOrViewNo = GitVersionTagCmbBx.SelectedIndex - 1;
                                if (processItems[setTagOrViewNo].Type == "View")
                                {
                                    setInformationText("Selected copy, Add, and Commit of files in View \"" + processItems[setTagOrViewNo].Name + "\".", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Label")
                                {
                                    setInformationText("Selected copy, Add, and Commit of files with Label \"" + processItems[setTagOrViewNo].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Git Tag")
                                {
                                    setInformationText("Selected copy, Add, and Commit of files with Label \"" + processItems[setTagOrViewNo].Name + "\", Git Tag \"" + processItems[setTagOrViewNo].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                }
                                else
                                    setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                            }
                            else
                            {
                                for (int i = 0; i < noOfProcessListContent; i++)
                                {
                                    if (processItems[i].Type == "View")
                                    {
                                        setInformationText("Streamed copy, Add, and Add of files in ClearCase View \"" + processItems[i].Name + "\".", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Label")
                                    {
                                        setInformationText("Streamed copy, Add, and Commit of files with ClearCase Label \"" + processItems[i].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Git Tag")
                                    {
                                        setInformationText("Streamed copy, Add, and Commit of files with ClearCase Label \"" + processItems[i].Name + "\", Git Tag \"" + processItems[i].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                    }
                                    else
                                        setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                                }
                            }
                        }
                        break;
                    case "Copy, Add, Commit, & Push":
                        {
                            workProcSteps = workStepType.pushToGitRemote;
                            if (GitVersionTagCmbBx.SelectedIndex > 0)
                            {
                                int setTagOrViewNo = GitVersionTagCmbBx.SelectedIndex - 1;
                                if (processItems[setTagOrViewNo].Type == "View")
                                {
                                    setInformationText("Selected copy, Add, Commit, and Push of files in View \"" + processItems[setTagOrViewNo].Name + "\".", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Label")
                                {
                                    setInformationText("Selected copy, Add, Commit, and Push of files with Label \"" + processItems[setTagOrViewNo].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                }
                                else if (processItems[setTagOrViewNo].Type == "Git Tag")
                                {
                                    setInformationText("Selected copy, Add, Commit, and Push of files with Label \"" + processItems[setTagOrViewNo].Name + "\", Git Tag \"" + processItems[setTagOrViewNo].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                }
                                else
                                    setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                            }
                            else
                            {
                                for (int i = 0; i < noOfProcessListContent; i++)
                                {
                                    if (processItems[i].Type == "View")
                                    {
                                        setInformationText("Streamed copy, Add, Commit, and Push of files in ClearCase View \"" + processItems[i].Name + "\".", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Label")
                                    {
                                        setInformationText("Streamed copy, Add, Commit, and Push of files with ClearCase Label \"" + processItems[i].Name + "\", no Git Tag set.", informationType.INFO, sender, e);
                                    }
                                    else if (processItems[i].Type == "Git Tag")
                                    {
                                        setInformationText("Streamed copy, Add, Commit, and Push of files with ClearCase Label \"" + processItems[i].Name + "\", Git Tag \"" + processItems[i].GitTagName + "\" is not set.", informationType.INFO, sender, e);
                                    }
                                    else
                                        setInformationText("Erroneous transfer type!", informationType.ERROR, sender, e);
                                }
                            }
                        }
                        break;
                    default:
                        {
                            setInformationText("Illegal work process set!", informationType.FATAL, sender, e);
                            closeThisSession(sender, e);
                        }
                        break;
                }
                ExecuteBtn.Enabled = true;
            }
            else
                ExecuteBtn.Enabled = false;
        }
        private void CancelSessionBtn_Click(object sender, EventArgs e)
        {
            closeThisSession(sender, e);
        }
        private void BldScrChkBx_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in copy.
        }
        private void TstScrSepChkBx_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in copy.
        }
        private void thrdPrtChkBx_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in copy.
        }
        private void devChkBx_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in copy.
        }
        private void binSepCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in the copy process.
        }
        private void docSepCmbBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing, this is handled in the copy process.
        }
        private void ExecuteBtn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string DriveToCopyFrom = ClearCaseDriveCmbBx.SelectedItem.ToString();
            DriveToCopyFrom = DriveToCopyFrom.Substring(0, DriveToCopyFrom.LastIndexOf('\\'));
            string VobToCopyFrom = ClearCaseVOBCmbBx.SelectedItem.ToString();
            string RepoToCopyFrom = GitVersionRepoCmbBx.SelectedItem.ToString();
            RepoToCopyFrom = RepoToCopyFrom.Substring(RepoToCopyFrom.LastIndexOf(' ') + 1);
            string SourceRepo = DriveToCopyFrom + VobToCopyFrom + "\\" + RepoToCopyFrom;
            string TargetRepo = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1);
            if (BldScrChkBx.Checked)
            {
                sebBldDir = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_BuildRelated";
                System.IO.Directory.CreateDirectory(sebBldDir);
            }
            if (TstScrSepChkBx.Checked)
            {
                sepTestDir = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_TestRelated";
                System.IO.Directory.CreateDirectory(sepTestDir);
            }
            if (thrdPrtChkBx.Checked)
            {
                sepThrdPrtDir = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_ThirdParty";
                System.IO.Directory.CreateDirectory(sepThrdPrtDir);
            }
            if (devChkBx.Checked)
            {
                sepDevDir = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_Development";
                System.IO.Directory.CreateDirectory(sepDevDir);
            }
            if (docSepCmbBx.SelectedIndex > 0)
            {
                if (docSepCmbBx.SelectedItem.ToString() == "RedDoc")
                {
                    setDocRepo = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_Documents";
                    System.IO.Directory.CreateDirectory(setDocRepo);
                    sDocSepFile = setDocRepo + "\\RedDocMetadata" + DateTimeOffset.UtcNow.ToString() + ".txt";
                    Boolean doWriteOver = true;
                    if (System.IO.File.Exists(sDocSepFile))
                    {
                        DialogResult overwriteAnswer = MessageBox.Show("A metadata file with this name allready exists!\nOverwrite?", "File exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (overwriteAnswer == DialogResult.Yes)
                            doWriteOver = true;
                        else
                            doWriteOver = false;
                    }
                    if (doWriteOver)
                    {
                        // Create header line
                        using (StreamWriter sw = File.AppendText(sDocSepFile))
                        {
                            //"Document no."/t"Document rev."/t"Document class"/t"Document sheets"/t"Document title"/t"Responsible"/t"Document status"/t"Document access"/t"Access level"
                            sw.WriteLine("Document no./tDocument rev./tDocument class/tDocument sheets/tDocument title/tResponsible/tDocument status/tDocument access/tAccess level/tFile Path Name\n");
                        }
                        // Handle the data in the move function.
                    }
                }
            }
            if (binSepCmbBx.SelectedIndex > 0)
            {
                setBinRepo = linwin.getUserGitStoragePath(GitVersionRepoCmbBx.SelectedIndex - 1) + "_Binaries";
                System.IO.Directory.CreateDirectory(setBinRepo);
            }
            // Handling the selected process.
            switch (workProcSteps)
            {
                case workStepType.cpyOnly:
                    {
                        // Tested OK.
                        // Merely a copy of files to a folder.
                        if (workFlow == workFlowType.wft_streamed)
                        {
                            setInformationText("Steamed copy is not relevant in this work process.", informationType.ERROR, sender, e);
                        }
                        else if (workFlow == workFlowType.wft_singluar)
                        {
                            setInformationText("Process: Pinpointed copying of files.", informationType.INFO, sender, e);
                            CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                            setCCConfigSpec(0, VobToCopyFrom, sender, e);
                            #region Setting label in ClearCase about the move.
                            string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                            int srdp = setRepo.IndexOf(" ");
                            if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                setRepo = setRepo.Substring(srdp + 1);
                            string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + "\\" + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                            if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                setInformationText("ClearCase label NOT set.", informationType.INFO, sender, e);
                            int noDeletions = runClearingTask(TargetRepo, sender , e);
                            setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                            int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                            setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);
                            #endregion
                        }
                    }
                    break;
                case workStepType.addToGitStag:
                    {
                        // Copy files to a Git repository and add them to staging area.
                        if (workFlow == workFlowType.wft_streamed)
                        {
                            setInformationText("Steamed copy is not relevant in this work process.", informationType.ERROR, sender, e);
                        }
                        else if (workFlow == workFlowType.wft_singluar)
                        {
                            if (LibGit2Sharp.Repository.IsValid(TargetRepo))
                            {
                                setInformationText("Process: Pinpointed copy and add of files.", informationType.INFO, sender, e);
                                CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                                var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo);
                                setCCConfigSpec(0, VobToCopyFrom, sender, e);
                                #region Setting label in ClearCase about the move.
                                string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                                int srdp = setRepo.IndexOf(" ");
                                if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                    setRepo = setRepo.Substring(srdp + 1);
                                string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                                if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                    setInformationText("ClearCase label NOT set.", informationType.INFO, sender, e);
                                // --- Check if repo is clean ---
                                RepositoryStatus currStatus = GitLocalRepository.RetrieveStatus();
                                if (currStatus.IsDirty)
                                {
                                    DialogResult answer = MessageBox.Show("The target repository is NOT clean!\n" +
                                                    currStatus.Staged.Count().ToString() + " staged changes exists.\n" +
                                                    "Reset?", "Dirty target!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    if (answer == DialogResult.Yes)
                                    {
                                        // Clean the target
                                        GitLocalRepository.Reset(ResetMode.Hard);
                                    }
                                }
                                // --- Copy files from VOB ---
                                int noDeletions = runClearingTask(TargetRepo, sender, e);
                                setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                                int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                                setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);
                                // --- Add all files ---
                                if (!addAllToGit(GitLocalRepository, TargetRepo, TargetRepo))
                                    setInformationText("Git add failed.", informationType.ERROR, sender, e);
                                #endregion
                                GitLocalRepository.Dispose();
                            }
                            else
                                setInformationText("Target repo \"" + TargetRepo + "\" is NOT a Git repo.", informationType.ERROR, sender, e);
                        }
                    }
                    break;
                case workStepType.commitToGitLocal:
                    {
                        bool saidNoAllready = false;
                        if (workFlow == workFlowType.wft_streamed)
                        {
                            setInformationText("Process: Streamed copy, add, and commit.", informationType.INFO, sender, e);
                            CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                            for (int i = 0; i < noOfProcessListContent; i++)
                            {
                                if (LibGit2Sharp.Repository.IsValid(TargetRepo))
                                {
                                    var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo);
                                    setCCConfigSpec(i, VobToCopyFrom, sender, e);
                                    #region Setting label in ClearCase about the move.
                                    string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                                    int srdp = setRepo.IndexOf(" ");
                                    if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                        setRepo = setRepo.Substring(srdp + 1);
                                    string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                                    if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                        setInformationText("ClearCase label NOT set.", informationType.INFO, sender, e);
                                    // --- Check if repo is clean ---
                                    if (!saidNoAllready)
                                    {
                                        RepositoryStatus currStatus = GitLocalRepository.RetrieveStatus();
                                        if (currStatus.IsDirty)
                                        {
                                            DialogResult answer = MessageBox.Show("The target repository is NOT clean!\n" +
                                                            "Reset?", "Dirty target!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                            if (answer == DialogResult.Yes)
                                            {
                                                // Clean the target
                                                GitLocalRepository.Reset(ResetMode.Hard);
                                            }
                                            else
                                                saidNoAllready = true;
                                        }
                                    }
                                    #endregion
                                    #region cleaning and copying files
                                    setInformationText("Process: Streamed copying of files.", informationType.ERROR, sender, e);
                                    int noDeletions = runClearingTask(TargetRepo, sender, e);
                                    setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                                    int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                                    setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);
                                    #endregion
                                    if (noOfCreations == 0)
                                        setInformationText("Copying did not work properly.", informationType.ERROR, sender, e);
                                    else
                                    {
                                        try
                                        {
                                            var signature = new LibGit2Sharp.Signature(linwin.getUserId(), linwin.getUserEMail(), DateTimeOffset.UtcNow);
                                            if (!addAllToGit(GitLocalRepository, TargetRepo, TargetRepo))
                                                setInformationText("Git add failed.", informationType.ERROR, sender, e);
                                            // --- Commit changes to Local Repository ---
                                            if (checkForChanges(GitLocalRepository, TargetRepo) > 0)
                                            {
                                                try
                                                {
                                                    var commit = GitLocalRepository.Commit(processItems[i].GitTagName, signature, signature);
                                                }
                                                catch (Exception err)
                                                {
                                                    MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    setInformationText("Commit error: " + err.ToString(), informationType.ERROR, sender, e);
                                                    throw err;
                                                }
                                            }
                                            else
                                            {
                                                GitLocalRepository.ApplyTag(processItems[i].GitTagName);
                                            }
                                            GitLocalRepository.Dispose();
                                        }
                                        catch (Exception err)
                                        {
                                            MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            throw err;
                                        }
                                    }
                                }
                                else
                                    setInformationText("Target repo \"" + TargetRepo + "\" is NOt a Git repo.", informationType.ERROR, sender, e);
                            }
                        }
                        else if (workFlow == workFlowType.wft_singluar)
                        {
                            // Kopiera filer och återskapa katalogstrukturer
                            if (LibGit2Sharp.Repository.IsValid(TargetRepo))
                            {
                                setInformationText("Process: Pinpointed copying of files.", informationType.ERROR, sender, e);
                                // --- Create ClearCase Config. Spec. ---
                                CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                                var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo);
                                setCCConfigSpec(0, VobToCopyFrom, sender, e);
                                // --- Setting label in ClearCase about the move. ---
                                string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                                int srdp = setRepo.IndexOf(" ");
                                if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                    setRepo = setRepo.Substring(srdp + 1);
                                string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                                if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                    setInformationText("ClearCase label NOT set.", informationType.INFO, sender, e);
                                RepositoryStatus currStatus = GitLocalRepository.RetrieveStatus();
                                if (currStatus.IsDirty)
                                {
                                    DialogResult answer = MessageBox.Show("The target repository is NOT clean!\n" +
                                                    currStatus.Staged.Count().ToString() + " staged changes exists.\n" +
                                                    "Reset?", "Dirty target!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    if (answer == DialogResult.Yes)
                                    {
                                        // Clean the target
                                        GitLocalRepository.Reset(ResetMode.Hard);
                                    }
                                }
                                // --- Remove and add all files in target repo ---
                                int noDeletions = runClearingTask(TargetRepo, sender, e);
                                setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                                int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                                setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);
                                // --- Add all changes to the local repository ---
                                if (!addAllToGit(GitLocalRepository, TargetRepo, TargetRepo))
                                    setInformationText("Git add failed.", informationType.ERROR, sender, e);
                                // --- Create a Git tag from a CC Label ---
                                string userSelTag = "";
                                if (GitVersionTagCmbBx.SelectedIndex > 0)
                                {
                                    userSelTag = GitVersionTagCmbBx.SelectedItem.ToString();
                                    int ustdp = userSelTag.IndexOf('(');
                                    if ((ustdp > 0) && (ustdp < userSelTag.Length))
                                        userSelTag = userSelTag.Substring(0, ustdp);
                                    else
                                    {
                                        ustdp = userSelTag.LastIndexOf('/');
                                        if ((ustdp > 0) && (ustdp < userSelTag.Length - 1))
                                        {
                                            userSelTag = userSelTag.Substring(ustdp + 1);
                                        }
                                    }
                                }
                                else if (WorkList.Items.Count > 0)
                                {
                                    userSelTag = WorkList.Items[1].ToString();
                                }
                                // --- Commit changes to Local Repository ---
                                var signature = new LibGit2Sharp.Signature(linwin.getUserId(), linwin.getUserEMail(), DateTimeOffset.UtcNow);
                                //   - Check if the commit allready exists -
                                if (checkForChanges(GitLocalRepository, TargetRepo) > 0)
                                {
                                    try
                                    {
                                        var commit = GitLocalRepository.Commit(GitVersionTagCmbBx.SelectedItem.ToString(), signature, signature);
                                    }
                                    catch (Exception err)
                                    {
                                        MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        setInformationText("Commit error: \n" + err.ToString(), informationType.ERROR, sender, e);
                                        throw err;
                                    }
                                }
                                else
                                {
                                    DialogResult answer = MessageBox.Show("Nothing to commit.\nForce Label?", "Commit error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                    if (answer == DialogResult.Yes)
                                    {
                                        bool tagFound = false;
                                        foreach (Tag t in GitLocalRepository.Tags)
                                        {
                                            string sfn = t.FriendlyName;
                                            if (sfn == userSelTag)
                                                tagFound = true;
                                        }
                                        if (!tagFound)
                                            GitLocalRepository.ApplyTag(userSelTag);
                                    }
                                }
                                GitLocalRepository.Dispose();
                            }
                            else
                                setInformationText("Target repo \"" + TargetRepo + "\" is NOT a Git repo.", informationType.ERROR, sender, e);
                        }
                    }
                    break;
                case workStepType.pushToGitRemote:
                    {
                        bool saidNoAllready = false;
                        if (workFlow == workFlowType.wft_streamed)
                        {
                            if (LibGit2Sharp.Repository.IsValid(TargetRepo))
                            {
                                string GRU = "";
                                CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                                for (int i = 0; i < noOfProcessListContent; i++)
                                {
                                    setInformationText("Streamed copy, add, commit, and push of " + processItems[i].Name, informationType.INFO, sender, e);
                                    var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo);
                                    setCCConfigSpec(i, VobToCopyFrom, sender, e);
                                    #region Setting label in ClearCase about the move.
                                    string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                                    int srdp = setRepo.IndexOf(" ");
                                    if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                        setRepo = setRepo.Substring(srdp + 1);
                                    string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                                    if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                        setInformationText("ClearCase label NOT set.", informationType.INFO, sender, e);
                                    // --- Check if repo is clean ---
                                    if (!saidNoAllready)
                                    {
                                        RepositoryStatus currStatus = GitLocalRepository.RetrieveStatus();
                                        if (currStatus.IsDirty)
                                        {
                                            DialogResult answer = MessageBox.Show("The target repository is NOT clean!\n" +
                                                            "Reset?", "Dirty target!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                            if (answer == DialogResult.Yes)
                                            {
                                                // Clean the target
                                                GitLocalRepository.Reset(ResetMode.Hard);
                                            }
                                            else
                                                saidNoAllready = true;
                                        }
                                    }
                                    #endregion
                                    #region cleaning and copying files
                                    setInformationText("Process: Streamed copying of files.", informationType.ERROR, sender, e);
                                    int noDeletions = runClearingTask(TargetRepo, sender, e);
                                    setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                                    int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                                    setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);
                                    #endregion
                                    if (noOfCreations == 0)
                                        setInformationText("Copying did not work properly.", informationType.ERROR, sender, e);
                                    else
                                    {
                                        try
                                        {
                                            var signature = new LibGit2Sharp.Signature(linwin.getUserId(), linwin.getUserEMail(), DateTimeOffset.UtcNow);
                                            if (!addAllToGit(GitLocalRepository, TargetRepo, TargetRepo))
                                                setInformationText("Git add failed.", informationType.ERROR, sender, e);
                                            // --- Commit changes to Local Repository ---
                                            if (checkForChanges(GitLocalRepository, TargetRepo) > 0)
                                            {
                                                try
                                                {
                                                    var commit = GitLocalRepository.Commit(processItems[i].GitTagName, signature, signature);
                                                }
                                                catch (Exception err)
                                                {
                                                    MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                    setInformationText("Commit error: " + err.ToString(), informationType.ERROR, sender, e);
                                                    throw err;
                                                }
                                            }
                                            else
                                            {
                                                GitLocalRepository.ApplyTag(processItems[i].GitTagName);
                                            }
                                            if (linwin.validateUser())
                                            {
                                                try
                                                {
                                                    if (GRU == "")
                                                        GRU = getGitRepoUrl(TargetRepo + "\\.git\\config");
                                                    var remote = GitLocalRepository.Network.Remotes["origin"];//[selBranchDirection];
                                                    if (remote != null)
                                                    {
                                                        GitLocalRepository.Network.Remotes.Remove("origin");
                                                    }
                                                    GitLocalRepository.Network.Remotes.Add("origin", GRU);
                                                    remote = GitLocalRepository.Network.Remotes["origin"];
                                                    if (remote == null)
                                                    {
                                                        setInformationText("Could not set network remotes.", informationType.ERROR, sender, e);
                                                        return;
                                                    }
                                                    string setBranch = GitVersionBranchCmbBx.SelectedItem.ToString();
                                                    if (setBranch == "Select")
                                                        setBranch = "main";
                                                    var localBranch = GitLocalRepository.Branches[setBranch];
                                                    if (localBranch == null)
                                                    {
                                                        setInformationText("Could not set local branch.", informationType.ERROR, sender, e);
                                                        return;
                                                    }
                                                    GitLocalRepository.Branches.Update(localBranch,
                                                        b => b.Remote = remote.Name,
                                                        b => b.UpstreamBranch = localBranch.CanonicalName);
                                                    var pushOptions = new PushOptions
                                                    {
                                                        CredentialsProvider = (url, usernameFromUrl, types) =>
                                                            new UsernamePasswordCredentials
                                                            {
                                                                Username = linwin.getUserId(),
                                                                Password = linwin.getUserPwd()
                                                            }
                                                    };
                                                    pushOptions.ProxyOptions.Url = GRU;
                                                    GitLocalRepository.Network.Push(localBranch, pushOptions);
                                                    remote.Dispose();
                                                }
                                                catch (LibGit2SharpException err)
                                                {
                                                    setInformationText("Commit error", informationType.ERROR, sender, e);
                                                    throw err;
                                                }
                                            }
                                            GitLocalRepository.Dispose();
                                        }
                                        catch (Exception err)
                                        {
                                            MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            throw err;
                                        }
                                    }
                                }
                            }
                            else
                                setInformationText("Target repo \"" + TargetRepo + "\" is Not a Git repo.", informationType.ERROR, sender, e);
                        }
                        else if (workFlow == workFlowType.wft_singluar)
                        {
                            // Kopiera filer och återskapa katalogstrukturer
                            if (LibGit2Sharp.Repository.IsValid(TargetRepo))
                            {
                                setInformationText("Process: Pinpointed copying of files.", informationType.ERROR, sender, e);
                                CCAutomation.ClearcaseHelper.MountVob(setCCVOB);
                                // --- Create ClearCase Config. Spec. ---
                                setCCConfigSpec(0, VobToCopyFrom, sender, e);
                                //var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo);
                                // --- Setting label in ClearCase about the move. ---
                                string setRepo = GitVersionRepoCmbBx.SelectedItem.ToString();
                                int srdp = setRepo.IndexOf(" ");
                                if ((srdp > 0) && (srdp < setRepo.Length - 1))
                                    setRepo = setRepo.Substring(srdp + 1);
                                string directPath = ClearCaseDriveCmbBx.SelectedItem.ToString() + ClearCaseVOBCmbBx.SelectedItem.ToString() + "\\" + setRepo;
                                if (!CCAutomation.ClearcaseHelper.CreateLabel("ClearCase2Git", "Extracted by " + linwin.getUserId() + " at " + DateTime.Now.ToString("yyyy-MM-dd"), directPath))
                                    setInformationText("ClearCase label NOT set", informationType.INFO, sender, e);

                                using (var GitLocalRepository = new LibGit2Sharp.Repository(TargetRepo))
                                {
                                    // --- Check repo status ---
                                    RepositoryStatus currStatus = GitLocalRepository.RetrieveStatus();
                                    if (currStatus.IsDirty)
                                    {
                                        DialogResult answer = MessageBox.Show("The target repository is NOT clean!\n" +
                                                        "Reset?", "Dirty target!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                        if (answer == DialogResult.Yes)
                                        {
                                            // Clean the target
                                            GitLocalRepository.Reset(ResetMode.Hard);
                                        }
                                    }
                                    // --- Remove and add all files in the target repo ---
                                    int noDeletions = runClearingTask(TargetRepo, sender, e);
                                    setInformationText("Deleted " + noDeletions.ToString() + " items in target repo.", informationType.INFO, sender, e);
                                    int noOfCreations = runCopyTask(SourceRepo, TargetRepo, sender, e);
                                    setInformationText("Created " + noOfCreations + " items in target repo.", informationType.INFO, sender, e);

                                    var signature = new LibGit2Sharp.Signature(linwin.getUserId(), linwin.getUserEMail(), DateTimeOffset.UtcNow);

                                    // --- Add all changes to the local repository ---
                                    if (!addAllToGit(GitLocalRepository, TargetRepo, TargetRepo))
                                        setInformationText("Git add failed.", informationType.ERROR, sender, e);
                                    // --- Create a Git tag from a CC Label ---
                                    string userSelTag = "";
                                    if (GitVersionTagCmbBx.SelectedIndex > 0)
                                    {
                                        userSelTag = GitVersionTagCmbBx.SelectedItem.ToString();
                                        int ustdp = userSelTag.IndexOf('(');
                                        if ((ustdp > 0) && (ustdp < userSelTag.Length))
                                            userSelTag = userSelTag.Substring(0, ustdp);
                                        else
                                        {
                                            ustdp = userSelTag.LastIndexOf('/');
                                            if ((ustdp > 0) && (ustdp < userSelTag.Length - 1))
                                            {
                                                userSelTag = userSelTag.Substring(ustdp + 1);
                                            }
                                            ustdp = userSelTag.IndexOf('_');
                                            if ((ustdp > 0) && (ustdp < userSelTag.Length - 1))
                                            {
                                                userSelTag = userSelTag.Substring(0, ustdp);
                                                userSelTag = userSelTag[0].ToString().ToUpper() + userSelTag.Substring(1).ToLower();
                                            }
                                        }
                                    }
                                    else if (WorkList.Items.Count > 0)
                                    {
                                        userSelTag = WorkList.Items[1].ToString();
                                    }
                                    // --- Commit changes to Local Repository ---
                                    //   - Check if the commit allready exists -
                                    if (checkForChanges(GitLocalRepository, TargetRepo) > 0)
                                    {
                                        try
                                        {
                                            int dp = GitVersionTagCmbBx.SelectedItem.ToString().IndexOf('(');
                                            var commit = GitLocalRepository.Commit(GitVersionTagCmbBx.SelectedItem.ToString().Substring(0, dp), signature, signature);
                                        }
                                        catch (Exception err)
                                        {
                                            MessageBox.Show(err.ToString(), "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            setInformationText("Commit error: " + err.ToString(), informationType.ERROR, sender, e);
                                            throw err;
                                        }
                                        // --- Push changes to Remote Repository ---
                                        if (linwin.validateUser())
                                        {
                                            try
                                            {
                                                string GRU = getGitRepoUrl(TargetRepo + "\\.git\\config");
                                                var remote = GitLocalRepository.Network.Remotes["origin"];//[selBranchDirection];
                                                if (remote != null)
                                                {
                                                    GitLocalRepository.Network.Remotes.Remove("origin");
                                                }
                                                GitLocalRepository.Network.Remotes.Add("origin", GRU);
                                                remote = GitLocalRepository.Network.Remotes["origin"];
                                                if (remote == null)
                                                {
                                                    setInformationText("Could not set network remotes.", informationType.ERROR, sender, e);
                                                    return;
                                                }
                                                string setBranch = GitVersionBranchCmbBx.SelectedItem.ToString();
                                                if (setBranch == "Select")
                                                    setBranch = "main";
                                                var localBranch = GitLocalRepository.Branches[setBranch];
                                                if (localBranch == null)
                                                {
                                                    setInformationText("Could not set local branch.", informationType.ERROR, sender, e);
                                                    return;
                                                }
                                                GitLocalRepository.Branches.Update(localBranch,
                                                    b => b.Remote = remote.Name,
                                                    b => b.UpstreamBranch = localBranch.CanonicalName);
                                                var pushOptions = new PushOptions
                                                {
                                                    CredentialsProvider = (url, usernameFromUrl, types) =>
                                                        new UsernamePasswordCredentials
                                                        {
                                                            Username = linwin.getUserId(),
                                                            Password = linwin.getUserPwd()
                                                        }
                                                };
                                                pushOptions.ProxyOptions.Url = GRU;
                                                GitLocalRepository.Network.Push(localBranch, pushOptions);
                                                remote.Dispose();
                                            }
                                            catch (LibGit2SharpException err)
                                            {
                                                setInformationText("Commit error", informationType.ERROR, sender, e);
                                                throw err;
                                            }
                                        }
                                        GitLocalRepository.Dispose();
                                    }
                                    else
                                    {
                                        DialogResult answer = MessageBox.Show("Nothing to commit.\nForce Label?", "Commit error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                                        if (answer == DialogResult.Yes)
                                        {
                                            bool tagFound = false;
                                            foreach (Tag t in GitLocalRepository.Tags)
                                            {
                                                string sfn = t.FriendlyName;
                                                if (sfn == userSelTag)
                                                    tagFound = true;
                                            }
                                            if (!tagFound)
                                            {
                                                GitLocalRepository.ApplyTag(userSelTag);
                                                setInformationText("Git Tag \"" + userSelTag + "\" force set.", informationType.INFO, sender, e);
                                            }
                                        }
                                        GitLocalRepository.Dispose();
                                    }
                                }
                            }
                            else
                                setInformationText("Target repo \"" + TargetRepo + "\" is NOT a Git repo.", informationType.ERROR, sender, e);
                        }
                    }
                    break;
                default:
                    {
                        setInformationText("Erroneous work process selected.", informationType.ERROR, sender, e);
                    }
                    break;
            }
            Cursor.Current = Cursors.Arrow;
        }
        #endregion
        public void loginWinOpen()
        {
            if (linwin == null)
            {
                linwin = new cc2git_Login();
            }
            linwin.Show();
        }
    }
}

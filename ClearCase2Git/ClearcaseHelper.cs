using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace CCAutomation
{
    public class ClearcaseHelper
    {
        #region Private Members

        /// <summary>
        /// Clearcase application
        /// </summary>
        private static ClearCase.Application ccApp;
        /// <summary>
        /// Clearcase tool
        /// </summary>
        private static ClearCase.ClearTool ct;
        /// <summary>
        /// Network Drive where the view is mounted
        /// </summary>
        private static string m_BaseDrive = string.Empty;
        private static bool recursSetLabels(string comment, string label, string path)
        {
            try
            {
                bool retVal = false;
                if (!System.IO.Directory.Exists(path))
                    return false;

                string[] filesAtDirectory = System.IO.Directory.GetFiles(path);
                foreach (var file in filesAtDirectory)
                {
                    ct.CmdExec("mklabel -rep -silent -c \"" + comment + "\" " + label + " " + file.ToString());
                    retVal = retVal && true;
                }
                string[] dirsAtDirectory = System.IO.Directory.GetDirectories(path);
                foreach (var dir in dirsAtDirectory)
                {
                    retVal = recursSetLabels(comment, label, dir);
                }
                return retVal;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// BaseDrive where the View is mounted.
        /// </summary>
        /// <exception cref="ArgumentNullException">Raised if an invalid value is provided.</exception>
        public static string BaseDrive
        {
            get
            {
                return m_BaseDrive;
            }
            set
            {
                // Set the Current Directory of the application to the given value.
                try
                {
                    m_BaseDrive = value;
                    Directory.SetCurrentDirectory(BaseDrive);
                }
                catch (Exception e)
                {
                    throw new ArgumentNullException("Invalid value provided for BaseDrive property!", e);
                }
            }
        }

        #endregion

        #region Construction/Initialization

        /// <summary>
        /// Static Ctor
        /// </summary>
        static ClearcaseHelper()
        {
            ccApp = new ClearCase.Application();
            ct = new ClearCase.ClearTool();
        }
        /// <summary>
        /// Mounts the identified VOB if it is not mounted.
        /// </summary>
        /// <param name="vob_path"></param>
        public static void MountVob(string vob_path)
        {
            if (vob_path != "")
            {
                try
                {
                    ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                    if ((vob != null) && (!vob.IsMounted))
                    {
                        ct.CmdExec("mount " + vob_path);
                    }
                }
                catch (Exception err)
                {
                    EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.MountVob()\n" + err.ToString(), EventLogEntryType.Error);
                    throw err;
                }
            }
        }
        public bool CheckRegionExist()
        {
            try
            {
                string regions = ct.CmdExec("lsregion -s");
                if (regions.Contains("Error"))
                    return false;
                return true;
            }
            catch // (Exception)// err)
            {
                return false;
                //throw err;
            }
        }
        #endregion

        #region APIs

        /// <summary>
        /// Get the Current Clearcase Region
        /// </summary>
        /// <param name="region">Current Region name (ref)</param>
        /// <returns>true if successful, else false</returns>
        public static bool GetCurrentRegion(ref string region)
        {
            bool regionFound = false;

            try
            {
                // For 32 Bit OS, the Region value is stored at:
                // HKEY_LOCAL_MACHINE\SOFTWARE\Atria\ClearCase\CurrentVersion
                // For 64 Bit OS, the Region value is stored at:
                // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Atria\ClearCase\CurrentVersion
                // We are not sure if the current OS is 32/64 bit, so check at both locations.
                RegistryKey theKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Atria\ClearCase\CurrentVersion\Region");


                if (theKey == null)
                    theKey = Registry.LocalMachine.OpenSubKey(@"\SOFTWARE\Wow6432Node\Atria\ClearCase\CurrentVersion");

                if (theKey != null)
                {
                    region = theKey.GetValue("Region") as string;

                    if (!string.IsNullOrEmpty(region))
                        regionFound = true;
                }

                return regionFound;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetCurrentRegion()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get all the Clearcase Regions
        /// </summary>
        /// <returns>List consisting of all the Clearcase Regions</returns>
        public static List<string> GetAllRegions()
        {
            try
            {
                if (ct != null)
                {
                    string regions = ct.CmdExec("lsregion -s");
                    return new List<string>(regions.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetAllRegions()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get the current View Name.
        /// NOTE: BaseDrive property should be set before calling this API.
        /// </summary>
        /// <exception cref="ArgumentNullException">Raised if BaseDrive property is not set.</exception>
        /// <returns>View Name</returns>
        public static string GetCurrentView()
        {
            try
            {
                // Check if BaseDrive property is set
                if (String.IsNullOrEmpty(BaseDrive))
                    throw new ArgumentNullException("BaseDrive", "BaseDrive is not set!");

                string currView = string.Empty;

                if ((ccApp != null) && (ct != null))
                {
                    currView = ct.CmdExec("lsview -cview -s").Trim();
                }

                return currView;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetCurrentView()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get all the views available for this Clearcase Region
        /// </summary>
        /// <returns>List of View names</returns>
        public static List<string> GetAllViews()
        {
            try
            {
                string region = string.Empty;
                // Get the list of Views for the current Clearcase Region
                if (GetCurrentRegion(ref region))
                {
                    return GetAllViews(region);
                }

                // if Region is not found write to EventLog and throw an ArgumentNullException
                EventLog.WriteEntry("CCAutomation", "Error during ClearcaseHelper.GetAllViews()\nNo Clearcase Region found!", EventLogEntryType.Error);
                throw new ArgumentNullException("Region", "Clearcase Region not Found!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get all the views available for this Clearcase Region
        /// </summary>
        /// <param name="views_ref">List of Views (ref)</param>
        public static void GetAllViews(ref List<ClearCase.CCView> views_ref)
        {
            try
            {
                string region = string.Empty;
                // Get the list of Views for the current Clearcase Region
                if (GetCurrentRegion(ref region))
                {
                    GetAllViews(region, ref views_ref);
                }

                // if Region is not found write to EventLog and throw an ArgumentNullException
                EventLog.WriteEntry("CCAutomation", "Error during ClearcaseHelper.GetAllViews()\nNo Clearcase Region found!", EventLogEntryType.Error);
                throw new ArgumentNullException("Region", "Clearcase Region not Found!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get all the view available for the given Clearcase Region
        /// </summary>
        /// <param name="region_in">Clearcase Region</param>
        /// <returns>List of View names</returns>
        public static List<string> GetAllViews(string region_in)
        {
            try
            {
                // Check if BaseDrive property is set
                if (String.IsNullOrEmpty(BaseDrive))
                    throw new ArgumentNullException("BaseDrive", "BaseDrive is not set!");


                string currView = string.Empty;

                if (ccApp != null)
                {
                    // Get the list of Views for the given Clearcase Region
                    ClearCase.CCViews views = ccApp.get_Views(false, region_in);

                    List<string> viewList = new List<string>();
                    foreach (ClearCase.CCView view in views)
                    {
                        viewList.Add(view.TagName);
                    }

                    return viewList;
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetAllViews()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get all the view available for the given Clearcase Region
        /// </summary>
        /// <param name="region_in">Clearcase Region</param>
        /// <param name="views_ref">List of Views (ref)</param>
        public static void GetAllViews(string region_in, ref List<ClearCase.CCView> views_ref)
        {
            try
            {
                // Check if BaseDrive property is set
                if (String.IsNullOrEmpty(BaseDrive))
                    throw new ArgumentNullException("BaseDrive", "BaseDrive is not set!");


                string currView = string.Empty;

                if (ccApp != null)
                {
                    // Get the list of Views for the given Clearcase Region
                    ClearCase.CCViews views = ccApp.get_Views(false, region_in);

                    views_ref = new List<ClearCase.CCView>();
                    foreach (ClearCase.CCView view in views)
                    {
                        views_ref.Add(view);
                    }
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetAllViews()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get all the VOBs for the current Region
        /// </summary>
        /// <exception cref="ArgumentNullException">Raised if Current Region is not found.</exception>
        /// <returns>List of VOBs</returns>
        public static List<string> GetAllVOBs()
        {
            try
            {
                string region = string.Empty;
                // Get the list of VOBs for the current Region
                if (GetCurrentRegion(ref region))
                {
                    return GetAllVOBs(region);
                }

                // if Region is not found write to EventLog and throw an ArgumentNullException
                EventLog.WriteEntry("CCAutomation", "Error during ClearcaseHelper.GetAllVOBs()\nNo Clearcase Region found!", EventLogEntryType.Error);
                throw new ArgumentNullException("Region", "Clearcase Region not Found!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get all the VOBs for the current Region
        /// </summary>
        /// <param name="vobs_ref">List of VOBs (ref)</param>
        /// <exception cref="ArgumentNullException">Raised if Current Region is not found.</exception>
        public static void GetAllVOBs(ref List<ClearCase.CCVOB> vobs_ref)
        {
            try
            {
                string region = string.Empty;
                // Get the list of VOBs for the current Region
                if (GetCurrentRegion(ref region))
                {
                    GetAllVOBs(region, ref vobs_ref);
                }

                // if Region is not found write to EventLog and throw an ArgumentNullException
                EventLog.WriteEntry("CCAutomation", "Error during ClearcaseHelper.GetAllVOBs()\nNo Clearcase Region found!", EventLogEntryType.Error);
                throw new ArgumentNullException("Region", "Clearcase Region not Found!");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get the list of VOBs for the given Region
        /// </summary>
        /// <param name="region_in">Clearcase Region</param>
        /// <returns>List of VOBs</returns>
        public static List<string> GetAllVOBs(string region_in)
        {
            try
            {
                if (String.IsNullOrEmpty(region_in))
                    return null;

                if (ccApp != null)
                {
                    ClearCase.CCVOBs vobs = ccApp.get_VOBs(false, region_in);

                    List<string> vobList = new List<string>();
                    foreach (ClearCase.CCVOB vob in vobs)
                    {
                        vobList.Add(vob.TagName);
                    }

                    return vobList;
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetAllVOBs()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get List of VOBs for the given Clearcase Region
        /// </summary>
        /// <param name="region_in">Clearcase Region</param>
        /// <param name="vobs_ref">List of VOBs (ref)</param>
        public static void GetAllVOBs(string region_in, ref List<ClearCase.CCVOB> vobs_ref)
        {
            try
            {
                if (String.IsNullOrEmpty(region_in))
                    return;

                if (ccApp != null)
                {
                    ClearCase.CCVOBs vobs = ccApp.get_VOBs(false, region_in);

                    vobs_ref = new List<ClearCase.CCVOB>();
                    foreach (ClearCase.CCVOB vob in vobs)
                    {
                        vobs_ref.Add(vob);
                    }
                }
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetAllVOBs()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get the config spec for the current view
        /// </summary>
        /// <returns>List constisting of the Config spec</returns>
        public static List<string> GetConfigSpec()
        {
            try
            {
                return GetConfigSpec(GetCurrentView());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get the config spec for the given view
        /// </summary>
        /// <param name="view_in">View name</param>
        /// <returns>List constisting of the Config spec</returns>
        public static List<string> GetConfigSpec(string view_in)
        {
            try
            {
                if ((ccApp != null) && (!String.IsNullOrEmpty(view_in)))
                {
                    ClearCase.CCView view = ccApp.get_View(view_in);

                    if (view != null)
                    {
                        string cfgSpec = view.ConfigSpec;
                        return new List<string>(cfgSpec.Split(new string[] { "\n" }, StringSplitOptions.None));
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.GetConfigSpec()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Set the Config Spec for the current view.
        /// </summary>
        /// <param name="configSpec">List consisting of the Config Spec</param>
        public static void SetConfigSpec(List<string> configSpec)
        {
            try
            {
                if (configSpec == null)
                    return;

                SetConfigSpec(configSpec, GetCurrentView());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Set the Config Spec for the given view.
        /// </summary>
        /// <param name="configSpec">List consisting of the Config Spec</param>
        /// <param name="view_in">View name</param>
        /// <returns>true if config spec was set successfully, else false</returns>        
        public static bool SetConfigSpec(List<string> configSpec, string view_in)
        {
            try
            {
                if (configSpec == null)
                    return false;

                if (ccApp != null)
                {
                    // Join the array into a single string
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in configSpec)
                    {
                        sb.Append(s);
                        sb.Append("\n");
                    }

                    // Remove the last "\n"
                    sb.Remove(sb.Length - 1, 1);

                    string cs = sb.ToString();

                    ClearCase.CCView view = ccApp.get_View(view_in);
                    if (view != null)
                    {
                        view.ConfigSpec = cs;
                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.SetConfigSpec()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        public static void findBranchesInView(string vob_in, string view_in)
        {
            try
            {
                //List<string> resultat;
                //string delres = ct.CmdExec("lsstream -long W:");
                //string delres = ct.CmdExec("lsstream -long BKRB_Sim@\\esbberg_view");
                //" + view_in + "@" + vob_in);
                //if (delres != "")
                //{
                    // Blä blä blä ... I don't say that!
                //}
                //List<ClearCase.CCBranchType> resultat = ct.CmdExec("lsstream -cview");// + vob_path);
                //return resultat;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary> Search for Labels.</summary>
        /// <param name="vob_path"></param>
        /// <returns>List of ClearCaseLabelType</returns>
        public static List<ClearCase.CCLabelType> searchLabels(string vob_path)
        {
            try
            {
                if (string.IsNullOrEmpty(vob_path))
                    return null;

                if (ccApp != null)
                {
                    ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                    if ((vob != null) && (vob.IsMounted))
                    {
                        ClearCase.CCLabelTypes labels = vob.get_LabelTypes(true, true);
                        List<ClearCase.CCLabelType> result = new List<ClearCase.CCLabelType>();
                        foreach (ClearCase.CCLabelType label in labels)
                        {
                            result.Add(label);
                        }
                        return result;
                    }
                }
                return null;
            }
            catch (Exception err)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearCaseHelper.SearchLabels()\n" + err.ToString(), EventLogEntryType.Error);
                throw err;
            }
        }

        /// <summary>
        /// Search for Labels containing the given token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="vob_path">VOB where to search for labels</param>
        /// <returns></returns>
        public static List<ClearCase.CCLabelType> SearchLabelsBasedOnToken(string token, string vob_path)
        {
            try
            {
                if ((string.IsNullOrEmpty(token)) || (string.IsNullOrEmpty(vob_path)))
                    return null;

                if (ccApp != null)
                {
                    //Get the VOB
                    ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                    if ((vob != null) && (vob.IsMounted))
                    {
                        // Create the label
                        ClearCase.CCLabelTypes labels = vob.get_LabelTypes(true, true);

                        List<ClearCase.CCLabelType> result = new List<ClearCase.CCLabelType>();

                        foreach (ClearCase.CCLabelType label in labels)
                        {
                            if (label.Name.IndexOf(token) != -1)
                            {
                                result.Add(label);
                            }
                        }

                        return result;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.SearchLabelsBasedOnToken()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Get the list of Checked out files in the given VOB
        /// </summary>
        /// <param name="vob_path">VOB path</param>
        /// <returns></returns>
        public static List<ClearCase.CCCheckedOutFile> GetCheckOuts(string vob_path)
        {
            try
            {
                if (string.IsNullOrEmpty(vob_path))
                    return null;

                if (ccApp != null)
                {
                    //Get the VOB
                    ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                    ClearCase.CCCheckedOutFileQuery coQuery = ccApp.CreateCheckedOutFileQuery();

                    object[] arr = new object[] { vob_path };
                    coQuery.PathArray = arr;
                    coQuery.PathSelects = ClearCase.CCPath_Selection.ccSelection_AllInVOB;
                    ClearCase.CCCheckedOutFiles coFiles = coQuery.Apply();

                    List<ClearCase.CCCheckedOutFile> files = new List<ClearCase.CCCheckedOutFile>();
                    foreach (ClearCase.CCCheckedOutFile file in coFiles)
                        files.Add(file);

                    return files;
                }

                return null;
            }
            catch (Exception e)
            {
                EventLog.WriteEntry("CCAutomation", "Exception during ClearcaseHelper.SearchLabelsBasedOnDateRange()\n" + e.ToString(), EventLogEntryType.Error);
                throw e;
            }
        }

        /// <summary>
        /// Create a label with the given label comments in the given VOB.
        /// </summary>
        /// <param name="label_in">Label Name</param>
        /// <param name="comments_in">Label Comments</param>
        /// <param name="vob_path">VOB path where the label has to be created</param>
        /// <returns></returns>
        public static bool CreateLabel(string label_in, string comments_in, string vob_path)
        {
            bool testRun = true;
            if (testRun)
            {
                bool result = false;

                if (string.IsNullOrEmpty(label_in))
                    return false;

                if (ccApp != null)
                {
                    //Get the VOB
                    if (System.IO.Directory.Exists(vob_path))
                    {
                        ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                        if ((vob != null) && (vob.IsMounted))
                        {
                            try
                            {
                                bool labelTypeExists = false;

                                ClearCase.CCLabelTypes labelTypes = vob.LabelTypes[true, true];
                                ClearCase.CCLabelType cclt;
                                foreach (ClearCase.CCLabelType label in labelTypes)
                                {
                                    if (label.Name == label_in)
                                        labelTypeExists = true;
                                }

                                if (!labelTypeExists)
                                    cclt = vob.CreateLabelType(label_in, "");

                                labelTypeExists = false;
                                labelTypes = vob.LabelTypes[true, true];
                                foreach (ClearCase.CCLabelType label in labelTypes)
                                {
                                    if (label.Name == label_in)
                                        labelTypeExists = true;
                                }

                                if (labelTypeExists)
                                {
                                    result = recursSetLabels(comments_in, label_in, vob_path);
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                            catch (Exception err)
                            {
                                throw err;
                            }
                        }
                    }
                    else
                        result = false;
                }
                return result;
            }
            else
            {
                try
                {
                    bool result = false;

                    if (string.IsNullOrEmpty(label_in))
                        return false;

                    if (ccApp != null)
                    {
                        //Get the VOB
                        if (System.IO.Directory.Exists(vob_path))
                        {
                            ClearCase.CCVOB vob = ccApp.get_VOB(vob_path);

                            if ((vob != null) && (vob.IsMounted))
                            {
                                ClearCase.CCLabelType cclt = vob.CreateLabelType(label_in, "");

                                ct.CmdExec("mklabel -replace -r -silent -c \"" + comments_in + "\" " + label_in);

                                List<ClearCase.CCLabelType> exiLbls = searchLabels(vob_path);
                                foreach (var lt in exiLbls)
                                {
                                    if (lt.ToString() == label_in)
                                        result = true;
                                }
                            }
                        }
                        else
                            result = false;
                    }
                    return result;
                }
                catch// (Exception e)
                {
                    return false;
                }
            }
        }

        #endregion
    }
}

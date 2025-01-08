using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.MVVM.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class BridgeViewModel : ObservableObject
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties
        // Private member for currentUserInfo
        private UserInfo currentUserInfo;

        // Locally accessible public object with user info and group assignments
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // List of AD group strings from ConfigurationProperties for IAM authorization
        private static  List<string> FactoolCompletionRole = new();

        // List of AD group strings from ConfigurationProperties for IAM authorization
        private static  List<string> FacToolMinRole = new();

        // List of AD group strings from ConfigurationProperties for IAM authorization
        private static  List<string> IAMGroups = new();

        // UserRole AD group string from ConfigurationProperties for authorization
        private static  String UnixRole = "";

        // List of Devs authorized for Special Functions
        private static  List<string> DevList = new();

        public BridgeModel CurrentBridge = new();

        // Progress Bar Tracker
        private int currentProgress;
        public int CurrentProgress
        {
            get { return currentProgress; }
            set { currentProgress = value; OnPropertyChanged(); }
        }

        // Binds text Version Number
        private string versionNumber;
        public string VersionNumber
        {
            get { return versionNumber; }
            set { if (versionNumber != value) { versionNumber = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public BridgeViewModel()
        {
            VersionNumber = "Version: ";
            CurrentUserInfo = new UserInfo();
            CurrentBridge = new()
            {
                CfyAdminRole = new(),
                CfyProvisioningRole = new(),
                FacToolAdminRole = new(),
                FactoolCompletionRole = new(),
                FacToolMgrRole = new(),
                FacToolMinRole = new(),
                DevList = new(),
                FactoolUserRole = new(),
                IAMGroups = new(),
                MgrRole = "",
                UnixRole = "",
                ServiceCatalogUserRole = new(),
                ServiceCatalogMgrRole = new(),
                ServiceCatalogUserRoleList = new(),
                ServiceCatalogAdminRole = new(),
                AccessNowUserRoleList = new(),
                AccessNowMgrRole = new(),
                AccessNowUserRole = new(),
                AccessNowClosingRole = new()
            };
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadVM()
        {
            try
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                VersionNumber = "Version: " + version;
                BuildPermissionLists();
                SetUser();
                App.GlobalUserInfo = CurrentUserInfo;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
        
        // Fetch Permission Security Group Configuration from SQL Server
        public void BuildPermissionLists()
        {
            try
            {
                DataTable ConfigurationTable = DBConn.GetTable(ConfigurationProperties.IAMHConfig);
                foreach (DataRow DR in ConfigurationTable.Rows)
                {
                    if ("FactoolCompletionRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.FactoolCompletionRole.Add(trim);
                            CurrentBridge.ServiceCatalogUserRoleList.Add(trim);
                        }
                    }

                    if ("FacToolMinRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.FacToolMinRole.Add(trim);
                        }
                    }

                    if ("IAMGroups" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.IAMGroups.Add(trim);

                        }
                        CurrentBridge.UnixRole = ConfigurationProperties.UnixMembers;
                    }

                    if ("CfyProvisioningRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.CfyProvisioningRole.Add(trim);

                        }

                        CurrentBridge.CfyProvisioningRole = ConfigurationProperties.CfyProvisioningRole;
                    }

                    if ("FactoolUserRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.FactoolUserRole.Add(trim);

                        }
                    }

                    if ("FacToolMgrRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.FacToolMgrRole.Add(trim);
                        }
                    }

                    if ("FacToolAdminRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.FacToolAdminRole.Add(trim);
                        }
                    }

                    if ("ServiceCatalogAdminRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.ServiceCatalogAdminRole.Add(trim);
                        }
                    }

                    if ("CfyAdminRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.CfyAdminRole.Add(trim);
                        }
                    }

                    if ("ServiceCatalogUserRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.ServiceCatalogUserRole.Add(trim);
                        }
                    }

                    if ("ServiceCatalogMgrRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.ServiceCatalogMgrRole.Add(trim);
                        }
                    }

                    if ("AccessNowUserRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.AccessNowUserRoleList.Add(trim);
                            CurrentBridge.AccessNowUserRole.Add(trim);
                        }
                    }

                    if ("AccessNowMgrRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.AccessNowMgrRole.Add(trim);
                        }
                    }

                    if ("AccessNowClosingRole" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.AccessNowClosingRole.Add(trim);
                        }
                    }


                    if ("DevList" == DR[1].ToString())
                    {
                        string role = DR[2].ToString();
                        var splitString = role.Split(";");
                        foreach (string s in splitString)
                        {
                            string trim = s;
                            trim = trim.Trim();
                            CurrentBridge.DevList.Add(trim);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Initialize User Information off AD Windows User Logon
        private void SetUser()
        {
            try
            {
                List<string> userGroups = new();
                string userName = WindowsIdentity.GetCurrent().Name;
                CurrentUserInfo.IsDev = false;
                CurrentUserInfo.FacToolMinRole = false;
                CurrentUserInfo.FactoolCompletionRole = false;
                CurrentUserInfo.Envrionment = "PROD";
                CurrentUserInfo.ConnectionString = Program.ConnectionString;

                try
                {
                    // Sets Primary AD Information for Current User 
                    UserPrincipal userPrincipal = UserPrincipal.Current;
                    if (userPrincipal.Sid != null) CurrentUserInfo.CurrentUserDomain = userPrincipal.Sid.Translate(typeof(NTAccount)).ToString().Split('\\')[0];
                    if (userPrincipal.SamAccountName != null) CurrentUserInfo.SamAccountName = userPrincipal.SamAccountName;
                    if (userPrincipal.Name != null) CurrentUserInfo.DisplayName = userPrincipal.Name;
                    if (userPrincipal.DistinguishedName != null) { CurrentUserInfo.DistinguishedName = userPrincipal.DistinguishedName; }
                    if (userPrincipal != null)
                    {
                        PrincipalSearchResult<Principal> groups = userPrincipal.GetGroups();
                        foreach (Principal p in groups)
                        {
                            userGroups.Add(p.ToString());
                        }
                        
                    }

                    String givenName = "";
                    String surname = "";
                    if (userPrincipal.GivenName != null) { givenName = userPrincipal.GivenName; }
                    if (userPrincipal.Surname != null) { surname = userPrincipal.Surname; }
                    String initial1 = givenName.Substring(0, 1);
                    String initial2 = surname.Substring(0, 1);
                    CurrentUserInfo.UserInitials = initial1 + initial2;
                    // Set Current Groups based on Key Groups being found in AD Security Groups
                    GroupListSet(userGroups);
                    if (userName == "Company\\c334741") { CurrentUserInfo.CurrentUserRole = UnixRole; CurrentUserInfo.CurrentUserGroups.Add(UnixRole); CurrentUserInfo.DisplayName = "Kumar, Virender"; }
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }

                for (int i = 0; i < 6; i++)
                {
                    int attempts = 1;
                    // On failure to load tries 4 more times. After which times out to failure.
                    if (String.IsNullOrEmpty(CurrentUserInfo.CurrentUserRole))
                    {
                        attempts++;
                        Thread.Sleep(1000);
                        if (userGroups.Count != 0 && string.IsNullOrEmpty(CurrentUserInfo.CurrentUserRole))
                        {
                            // Set Current Groups based on Key Groups being found in AD Security Groups
                            GroupListSet(userGroups);
                            if (userName == "Company\\c334741") { CurrentUserInfo.CurrentUserRole = UnixRole; CurrentUserInfo.CurrentUserGroups.Add(UnixRole); CurrentUserInfo.DisplayName = "Kumar, Virender"; }
                        }
                    }

                    if (i == 4)
                    {
                        // Dev Check based off App.Config List
                        foreach (string s in CurrentBridge.DevList)
                        {
                            if (CurrentUserInfo.DisplayName == s || userName == s || CurrentUserInfo.SamAccountName.ToString() == s)
                            {
                                CurrentUserInfo.IsDev = true;
                            }
                        }

                    }

                    // On Failure Notifies Read Only Permission to App
                    if (i == 5 && String.IsNullOrEmpty(CurrentUserInfo.CurrentUserRole))
                    {
                        const string message = "Unauthorized User, Read Only Permissions Granted.";
                        const string caption = "IAMHeimdall.exe - Application Read Only";
                        var result = MessageBox.Show(message, caption, MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                Environment.Exit(-1);
            }
        }

        // Determines Group Permissions assigning Access according to AD Security Groups
        public void GroupListSet(List<string> passedList)
        {
            // Set Current Groups based on Key Groups being found in AD Security Groups
            List<string> TempList = new();

            // Factool Update Permissions
            if ( passedList.Any(a => CurrentBridge.FactoolCompletionRole.Contains(a))) 
            { 
                CurrentUserInfo.FactoolCompletionRole = true; 
                TempList = passedList.Intersect(CurrentBridge.FactoolCompletionRole).ToList(); 
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.FacToolMinRole.Contains(a)))
            {
                CurrentUserInfo.FacToolMinRole = true;
                TempList = passedList.Intersect(CurrentBridge.FacToolMinRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            // Factool Faciilitator Permissions
            if (passedList.Any(a => CurrentBridge.FactoolUserRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.FactoolUserRole[0];
                TempList = passedList.Intersect(CurrentBridge.FactoolUserRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
                CurrentUserInfo.FacToolMinRole = true;
            }

            if (passedList.Any(a => CurrentBridge.FacToolMgrRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.FacToolMgrRole[0];
                TempList = passedList.Intersect(CurrentBridge.FacToolMgrRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.FactoolUserRole[0]);
                TempList.Clear();
                CurrentUserInfo.FacToolMinRole = true;
            }

            if (passedList.Any(a => CurrentBridge.FacToolAdminRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.FacToolAdminRole[0];
                TempList = passedList.Intersect(CurrentBridge.FacToolAdminRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.FacToolMgrRole[0]);
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.FactoolUserRole[0]);
                TempList.Clear();
                CurrentUserInfo.FacToolMinRole = true;
            }

            // Centrify Provisioning Permissions
            if (passedList.Any(a => CurrentBridge.CfyProvisioningRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.CfyProvisioningRole[0];
                TempList = passedList.Intersect(CurrentBridge.CfyProvisioningRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.CfyAdminRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.CfyAdminRole[0];
                TempList = passedList.Intersect(CurrentBridge.CfyAdminRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }


            // Service Catalog Permissions
            if (passedList.Any(a => CurrentBridge.ServiceCatalogAdminRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.ServiceCatalogAdminRole[0];
                TempList = passedList.Intersect(CurrentBridge.ServiceCatalogAdminRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.ServiceCatalogUserRoleList.Contains(a)))
            {
                CurrentUserInfo.ServiceCatalogRole = true;
                CurrentUserInfo.CurrentUserRole = CurrentBridge.ServiceCatalogUserRoleList[0];
                TempList = passedList.Intersect(CurrentBridge.ServiceCatalogUserRoleList).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.ServiceCatalogUserRole.Contains(a)))
            {
                CurrentUserInfo.ServiceCatalogRole = true;
                CurrentUserInfo.CurrentUserRole = CurrentBridge.ServiceCatalogUserRole[0];
                TempList = passedList.Intersect(CurrentBridge.ServiceCatalogUserRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.ServiceCatalogMgrRole.Contains(a)))
            {
                CurrentUserInfo.ServiceCatalogRole = true;
                CurrentUserInfo.CurrentUserRole = CurrentBridge.ServiceCatalogMgrRole[0];
                TempList = passedList.Intersect(CurrentBridge.ServiceCatalogMgrRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.ServiceCatalogUserRole[0]);
                TempList.Clear();
            }

            // Access Now Data Permissions
            if (passedList.Any(a => CurrentBridge.AccessNowUserRoleList.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.AccessNowUserRole[0];
                TempList = passedList.Intersect(CurrentBridge.AccessNowUserRoleList).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.AccessNowMgrRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.AccessNowMgrRole[0];
                TempList = passedList.Intersect(CurrentBridge.AccessNowMgrRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.AccessNowUserRole[0]);
                TempList.Clear();
            }

            if (passedList.Any(a => CurrentBridge.AccessNowClosingRole.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.AccessNowClosingRole[0];
                TempList = passedList.Intersect(CurrentBridge.AccessNowClosingRole).ToList();
                foreach (string s in TempList) { CurrentUserInfo.CurrentUserGroups.Add(s); }
                TempList.Clear();
            }

            // Unix Table Write Permissions
            if (passedList.Any(a => CurrentBridge.IAMGroups.Contains(a)))
            {
                CurrentUserInfo.CurrentUserRole = CurrentBridge.UnixRole;
                CurrentUserInfo.CurrentUserGroups.Add(CurrentBridge.UnixRole);
            }
        }
        #endregion
    }
}

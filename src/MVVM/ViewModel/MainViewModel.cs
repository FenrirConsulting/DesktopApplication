using IAMHeimdall.Core;
using System;
using System.Reflection;
using System.Windows.Input;
using IAMHeimdall.Resources.Commands;
using System.Data;
using IAMHeimdall.MVVM.Model;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;
using System.Linq;
using System.IO;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel 
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();

        // Relay Commands & View Model Classes for UserControl Element
        public RelayCommand UIDViewCommand { get; set; }
        public ICommand UpdateViewCommand { get; set; }
        public RelayCommand CloseContextMenusCommand { get; set; }
        public RelayCommand VersionUpdateCommand { get; set; }
        #endregion

        #region Properties
        // Holds BridgeModel instance containing permissions configuration from SQL Table
        public BridgeModel CurrentBridge = new();

        // Current UserControl View
        private BaseViewModel _selectedViewModel;
        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { _selectedViewModel = value; OnPropertyChanged(); }
        }

        // Static DataTable Objects To hold Data between Page Views
        public static DataTable GroupsTable { get; set; }
        public static DataTable UsersTable { get; set; }
        public static DataTable KerberosServerTable { get; set; }
        public static DataTable KerberosGroupsTable { get; set; }
        public static DataTable KerberosUsersTable { get; set; }
        public static DataTable ServiceCatalogTable { get; set; }
        public static DataTable FactoolExpectedTable { get; set; }

        // Stored User Info 
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Global Permissions visible to other Pages
        public static bool UnixTablePermissionGlobal { get; set; }
        public static bool GlobalIAMToolPermission { get; set; }
        public static bool GlobalIAMToolAdminPermission { get; set; }
        public static bool GlobalFacToolPermission { get; set; }
        public static bool GlobalFacToolMgrPermission { get; set; }
        public static bool GlobalFacToolAdminPermission { get; set; }
        public static bool GlobalDevEnabled { get; set; }
        public static bool GlobalServiceCatalogEnabled { get; set; }
        public static bool GlobalServiceCatalogMgrEnabled { get; set; }
        public static bool GlobalServiceCatalogAdminEnabled { get; set; }
        public static bool GlobalAccessNowUserRole { get; set; }
        public static bool GlobalAccessNowMgrRole { get; set; }
        public static bool GlobalAccessNowClosingRole { get; set; }

        // Enable Write Permissions For Data Tables on User Group Found
        private bool unixPermissionCheck;
        public bool UnixPermissionCheck
        {
            get { return unixPermissionCheck; }
            set { if (unixPermissionCheck != value) { unixPermissionCheck = value; OnPropertyChanged(); } }
        }

        // Enable AccessNow Tools Context Menu
        private bool accessNowToolsPermission;
        public bool AccessNowToolsPermission
        {
            get { return accessNowToolsPermission; }
            set { if (accessNowToolsPermission != value) { accessNowToolsPermission = value; OnPropertyChanged(); } }
        }

        // Enable Permissions to view the Service Catalog Page
        private bool serviceCatalogPermissionsCheck;
        public bool ServiceCatalogPermissionsCheck
        {
            get { return serviceCatalogPermissionsCheck; }
            set { if (serviceCatalogPermissionsCheck != value) { serviceCatalogPermissionsCheck = value; OnPropertyChanged(); } }
        }

        // Enable Dev Tools Button on Dev Group Found
        private bool devEnabled;
        public bool DevEnabled
        {
            get { return devEnabled; }
            set { if (devEnabled != value) { devEnabled = value; OnPropertyChanged(); } }
        }

        // Enable Provisioning Tools on Centrify Security Group(s) Found
        private bool iAMToolPermission;
        public bool IAMToolPermission
        {
            get { return iAMToolPermission; }
            set { if (iAMToolPermission != value) { iAMToolPermission = value; OnPropertyChanged(); } }
        }

        // Enable Factool Permissions on AIM_FACTOOL_USER Security Group Found
        private bool facToolPermission;
        public bool FacToolPermission
        {
            get { return facToolPermission; }
            set { if (facToolPermission != value) { facToolPermission = value; OnPropertyChanged(); } }
        }

        // Enable Factool Mgr Permissions on AIM_FACTOOL_MGR Security Group Found
        private bool facToolMgrPermission;
        public bool FacToolMgrPermission
        {
            get { return facToolMgrPermission; }
            set { if (facToolMgrPermission != value) { facToolMgrPermission = value; OnPropertyChanged(); } }
        }

        // Enable Factool Completion Permissions on if any of allowed Groups is found on User Groups
        private bool facToolCompletionPermission;
        public bool FacToolCompletionPermission
        {
            get { return facToolCompletionPermission; }
            set { if (facToolCompletionPermission != value) { facToolCompletionPermission = value; OnPropertyChanged(); } }
        }

        // Binds Version Number Text
        private string versionNumber;
        public string VersionNumber
        {
            get { return versionNumber; }
            set { if (versionNumber != value) { versionNumber = value; OnPropertyChanged(); } }
        }

        // Binds Higher Version Number Text
        private string higherVersionAvailable;
        public string HigherVersionAvailable
        {
            get { return higherVersionAvailable; }
            set { if (higherVersionAvailable != value) { higherVersionAvailable = value; OnPropertyChanged(); } }
        }

        // Holds Bool of If a Higher Version is Found
        private bool higherVersionCheck;
        public bool HigherVersionCheck
        {
            get { return higherVersionCheck; }
            set { if (higherVersionCheck != value) { higherVersionCheck = value; OnPropertyChanged(); } }
        }

        // Label Holding User Logon Name
        private string _LoggedInAs;
        public string LoggedInAs
        {
            get { return _LoggedInAs; }
            set { if (_LoggedInAs != value) { _LoggedInAs = value; OnPropertyChanged(); } }
        }

        // Control Context Menu Open / Close
        private bool contextMenuOpen1;
        public bool ContextMenuOpen1
        {
            get { return contextMenuOpen1; }
            set { if (contextMenuOpen1 != value) { contextMenuOpen1 = value; OnPropertyChanged(); } }
        }

        // Control Access Now Menu Open / Close
        private bool accessNowMenuOpen;
        public bool AccessNowMenuOpen
        {
            get { return accessNowMenuOpen; }
            set { if (accessNowMenuOpen != value) { accessNowMenuOpen = value; OnPropertyChanged(); } }
        }

        // Control Context Menu 2 Open / Close
        private bool contextMenuOpen2;
        public bool ContextMenuOpen2
        {
            get { return contextMenuOpen2; }
            set { if (contextMenuOpen2 != value) { contextMenuOpen2 = value; OnPropertyChanged(); } }
        }

        // Hold Authentication Credentials For Admin Imperssonation Code Functions
        public static string AuthenticatedLogonDomain { get; set; }
        public static string AuthenticatedUsername { get; set; }
        public static string AuthenticatedPassword { get; set; }
        public static SafeAccessTokenHandle SafeAccessTokenHandle { get; set; }

        // User Configuration Settings 
        public static double UserConfigFontScale { get; set; }
        #endregion

        #region Methods
        public MainViewModel()
        {
            // Initialize Properties
            VersionNumber = "Version: ";
            UnixPermissionCheck = false;
            UnixTablePermissionGlobal = false;
            IAMToolPermission = false;
            GlobalIAMToolPermission = false;
            GlobalIAMToolAdminPermission = false;
            FacToolPermission = false;
            FacToolMgrPermission = false;
            GlobalFacToolPermission = false;
            GlobalFacToolMgrPermission = false;
            GlobalFacToolAdminPermission = false;
            GlobalDevEnabled = false;
            GlobalServiceCatalogEnabled = false;
            GlobalServiceCatalogMgrEnabled = false;
            GlobalServiceCatalogAdminEnabled = false;
            DevEnabled = false;
            HigherVersionCheck = false;
            ServiceCatalogPermissionsCheck = false;
            AccessNowToolsPermission = false;
            CurrentUserInfo = new UserInfo();
            GroupsTable = new();
            UsersTable = new();
            KerberosServerTable = new();
            KerberosGroupsTable = new();
            KerberosUsersTable = new();
            ServiceCatalogTable = new();
            FactoolExpectedTable = new();
            LoggedInAs = "Logged In As : ";

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

            // User Configuration Default Settings
            UserConfigFontScale = 1.0;

            // Initialize Relay Commands
            UpdateViewCommand = new UpdateViewCommand(this);
            CloseContextMenusCommand = new RelayCommand(o => { ClearContextMenus(); });
            VersionUpdateCommand = new RelayCommand(o => { VersionUpdate(); });
        }
        #endregion

        #region Functions
        // On Load Method setting UserInfo and LoggedInAs Label
        public void LoadVM()
        {
            CurrentUserInfo = App.GlobalUserInfo; // Set at Bridge Open
            VersionCheck();
            BuildPermissionLists();
            SetUserPermissions();
            UserConfigurationLoad();
            if (CurrentUserInfo.DisplayName != null)
            {
                LoggedInAs = "Logged In As : " + CurrentUserInfo.DisplayName + Environment.NewLine + CurrentUserInfo.CurrentUserRole;

            }
        }

        // Set App Permissions based on Groups gathered from Bridge
        public void SetUserPermissions()
        {
            
            // General Write Permissions Group to Unix Data Tables
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.IAMGroups.Contains(a))) { UnixPermissionCheck = true; UnixTablePermissionGlobal = true; }

            // Authenticates Basic Centrify User Permission off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.CfyProvisioningRole.Contains(a))) { IAMToolPermission = true; GlobalIAMToolPermission = true; }

            // Authenticates Centrify Admin Permission off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.CfyAdminRole.Contains(a))) { GlobalIAMToolAdminPermission = true; }

            // Authenticates Service Catalog User off User Group
            if (App.GlobalUserInfo.ServiceCatalogRole == true) { GlobalServiceCatalogEnabled = true; ServiceCatalogPermissionsCheck = true; }

            // Authenticates Service Catalog Manager off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.ServiceCatalogMgrRole.Contains(a))) { GlobalServiceCatalogMgrEnabled = true; ServiceCatalogPermissionsCheck = true; }

            // Authenticates Service Catalog Admin off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.ServiceCatalogAdminRole.Contains(a))) { GlobalServiceCatalogAdminEnabled = true; GlobalServiceCatalogMgrEnabled = true;  ServiceCatalogPermissionsCheck = true; }

            // General Access Now Data Permissions
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.AccessNowUserRoleList.Contains(a))) { GlobalAccessNowUserRole = true; AccessNowToolsPermission = true; }

            // Authenticates Access Now Manager Role Permissions
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.AccessNowMgrRole.Contains(a))) { GlobalAccessNowMgrRole = true; GlobalAccessNowUserRole = true; AccessNowToolsPermission = true; }

            // Authenticates Access Now Ticket Closing Role Permissions
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.AccessNowClosingRole.Contains(a))) { GlobalAccessNowClosingRole = true; }

            // Authenticates Basic Factool Permission off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.FactoolUserRole.Contains(a))) { FacToolPermission = true; GlobalFacToolPermission = true; }

            // Authenticates Factool Manager Permission off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.FacToolMgrRole.Contains(a))) { GlobalFacToolMgrPermission = true; FacToolPermission = true; GlobalFacToolPermission = true; FacToolMgrPermission = true; }

            // Authenticates Factool Admin Permission off User Group
            if (App.GlobalUserInfo.CurrentUserGroups.Any(a => CurrentBridge.FacToolAdminRole.Contains(a))) { GlobalFacToolAdminPermission = true; GlobalFacToolMgrPermission = true; FacToolPermission = true; GlobalFacToolPermission = true; }

            if (App.GlobalUserInfo.FactoolCompletionRole == true) { FacToolCompletionPermission = true; }

            // If Dev, Enable all Buttons for Testing
            
            if (App.GlobalUserInfo.IsDev == true) { 
                UnixPermissionCheck = true; 
                IAMToolPermission = true; 
                FacToolPermission = true;
                FacToolMgrPermission = true;
                DevEnabled = true;
                GlobalDevEnabled = true;
                GlobalIAMToolPermission = true;
                GlobalIAMToolAdminPermission = true;
                GlobalFacToolPermission = true;
                GlobalFacToolMgrPermission = true;
                GlobalFacToolAdminPermission = true;
                UnixTablePermissionGlobal = true;
                GlobalServiceCatalogEnabled = true;
                GlobalServiceCatalogMgrEnabled = true;
                GlobalServiceCatalogAdminEnabled = true;
                ServiceCatalogPermissionsCheck = true;
                GlobalAccessNowUserRole = true;
                GlobalAccessNowMgrRole = true;
                GlobalAccessNowClosingRole = true;
                AccessNowToolsPermission = true;
            }
        }

        // Check Current Version Against Latest from SQL Server
        public void VersionCheck()
        {
            try
            {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Version v1 = new(version);
                DataTable configTable = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "Version");
                string versionCheck = configTable.Rows[0][2].ToString();
                Version v2 = new(versionCheck);
                VersionNumber = "Version: " + version;
                if (v2 > v1)
                {
                    HigherVersionCheck = true;
                }
                if (v1 > v2)
                {
                    DBConn.UpdateTableRecord("IAMHConfig", "Version", "Setting", "Value", v1.ToString());
                    HigherVersionCheck = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Button Click Opening Explorer at Setup File Path
        public  void VersionUpdate()
        {
            try
            {
                DataTable ConfigurationTable = DBConn.GetTable(ConfigurationProperties.IAMHConfig);
                string URL = "";
                foreach (DataRow DR in ConfigurationTable.Rows)
                {
                    if ("UpdateURLLink" == DR[1].ToString())
                    {
                        URL = DR[2].ToString();
                    }
                }
                string chromeCheck = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                if (File.Exists(chromeCheck)) 
                {
                    Process process = new();
                    process.StartInfo.FileName = chromeCheck;
                    process.StartInfo.Arguments = URL + " --new-window";
                    process.Start();
                }
                else
                {
                    var msg = "URL Has Been Copied. Paste into Browser to Download.";
                    var body = URL + Environment.NewLine + Environment.NewLine + "Microsoft Edge Cannot Download this Link.";
                    MessageBox.Show(body, msg);
                    Clipboard.SetText(URL);
                }
           
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Controls the Radio Context Sub Menus
        public void ClearContextMenus()
        {
            ContextMenuOpen1 = false;
            ContextMenuOpen2 = false;
            AccessNowMenuOpen = false;
        }

        // Set Configuration Settings on Load
        public void UserConfigurationLoad()
        {
            DataTable userCheck = DBConn.GetSelectedRecord(ConfigurationProperties.UserConfiguration,"Username", CurrentUserInfo.SamAccountName);
            if (userCheck.Rows.Count == 0)
            {
                DBConn.AddUserConfigurationRecord(ConfigurationProperties.UserConfiguration, CurrentUserInfo.SamAccountName, ConfigurationProperties.FontScaleDefault);
            }
            else
            {
                if (userCheck.Rows.Count != 0)
                {
                    bool success = double.TryParse(userCheck.Rows[0]["FontScale"].ToString(), out double fontParse);
                    if (success == true) { UserConfigFontScale = fontParse; }
                }
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
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class ConfigurationProperties
    {
        #region Properties
        // SQL Connection String
        public const string LegacyConnectionString = "";
        public const string DEVConnectionString = "";
        public const string ConnectionString = "";
        public const string QAConnectionString = "";

        // SharePoint App ID Strings
        public const string SPClientID = "";
        public const string SPClientSecret = ";
        public const string SPTitle = "IAMHeimdall App ID";
        public const string SPAppDomain = "www.localhost.com";
        public const string SPRedirect = "https://www.localhost.com";

        // Setup Files Installation Path
        public const string InstallationPath = "";

        // Test of Repository Push

        // SQL Server Table Names
        public const string GroupsTable = "IAMHGroups";
        public const string UsersTable = "IAMHUsers";
        public const string IAMHConfig = "IAMHConfig";
        public const string IAMHLog = "IAMHLog";
        public const string KerberosServersTable = "IAMHKServers";
        public const string KerberosGroupsTable = "IAMHKGroups";
        public const string KerberosUsersTable = "IAMHKUsers";
        //public const string FactoolRequestNumberTable = "IAMHFactoolRequestNumberTest"; 
        //public const string FactoolRequestTable = "IAMHFactoolRequestTestData";
        public const string FactoolRequestTable = "IAMHFactoolRequest";
        public const string FactoolRequestNumberTable = "IAMHFactoolRequestNumber";
        public const string FactoolJoinedView = "IAMHFactoolJoinedView";
        public const string FactoolRequestStatus = "IAMHFactoolRequestStatus";
        public const string FactoolRequestHistory = "IAMHFactoolRequestHistory";
        public const string FactoolRequestSyncHistoryTable = "IAMHFactoolRequestSyncHistory";
        public const string FactoolApplicationHistoryTable = "IAMHFactoolApplicationHistory";
        public const string FactoolConfigDefectReasons = "IAMHFactoolConfigDefectReasons";
        public const string FactoolConfigFormType = "IAMHFactoolConfigFormType";
        public const string FactoolConfigReplyType = "IAMHFactoolConfigReplyType";
        public const string FactoolConfigRequestType = "IAMHFactoolConfigRequestType";
        public const string FactoolConfigSystems = "IAMHFactoolConfigSystems";
        public const string FactoolLOBTypes = "IAMHFactoolLOBTypes";
        public const string FactoolExpectedNumbers = "IAMHFactoolExpectedNumbers";
        public const string FactoolExpectedView = "IAMHFactoolExpectedView";
        public const string UserConfiguration = "IAMHUserConfiguration";
        public const string ServiceCatalogTable = "IAMHServiceCatalog";
        public const string ServiceCatalogSourcesTable = "IAMHServiceCatalogSources";
        public const string ServiceCatalogAssignmentGroupsTable = "IAMHServiceCatalogAssignmentGroups";
        public const string ServiceCatalogEnvironmentsTable = "IAMHServiceCatalogEnvironments";
        public const string AccessNowOperationsDataTest = "IAMHAccessNowOperationsDataTest";
        public const string AccessNowOperationsData = "IAMHAccessNowOperationsView";
        public const string AccessNowData = "IAMHAccessNowData";
        public const string AccessNowHistory = "IAMHAccessNowHistory";
        public const string IAMHSNOWProvisioning = "IAMHSNOWProvisioning";
        public const string IAMHSNOWProvisioningView = "IAMHSNOWProvisioningView";
        public const string IAMHSNOWIncidents = "IAMHSNOWIncidents";
        public const string IAMHSNOWIncidentsView = "IAMHSNOWIncidentsView";
        public const string IAMHSolutionCenterEscalations = "IAMHSolutionCenterEscalations";
        public const string IAMHRMRData = "IAMHRMRDataView";


        // AccessNow Ticket Close Config
        public const string AccessNowClosedTickets = "IAMHAccessNowClosedTickets"; // DB Table Name

        // Production 
        public const string AccessNowAPIURL = "";
        public const string AccessNowClosedAPICONFIRMURL = "";
        public const string AccessNowClosedAPICLOSEURL = "";
        public const string AccessNowClosedAPIUsername = "";
        public const string AccessNowClosedAPIPassword = "";
        public const string AccessNowReportsAPIUsername = "Needed";
        public const string AccessNowReportsAPIPassword = "Needed";

        // Dev Testing
        public const string AccessNowAPITestURL = "";
        public const string AccessNowClosedAPITestCONFIRMURL = "";
        public const string AccessNowClosedAPITestCLOSEURL = "";
        public const string AccessNowClosedAPITestUsername = "";
        public const string AccessNowClosedAPITestPassword = "9";
        public const string AccessNowReportsAPITestUsername = "Needed";
        public const string AccessNowReportsAPITestPassword = "Needed";

        // QA Site Credentials
        public const string AccessNowAPIQAURL = "";
        public const string AccessNowClosedAPIQACONFIRMURL = "";
        public const string AccessNowClosedAPIQACLOSEURL = "";
        public const string AccessNowClosedAPIQAUsername = "";
        public const string AccessNowClosedAPIQAPassword = "";
        public const string AccessNowReportsAPIQAUsername = "";
        public const string AccessNowReportsAPIQAPassword = "";

        // General Access Security Groups
        public const string UnixTablePermission = "AIM_OIM";
        public const string UnixMembers = "AIM_OIM";
        public const string TestMgrRole = "dl_CSTest2";

        // Service Catalog Configuration
        public const string ServiceCatalogSPSite = "";
        public const string SErviceCatalogList = "Service Catalog";


        // List of Groups with Write Permission to User & Group Tables
        public static List<string> IAMGroups = new()
        {
            "AIM_OIM",
            "AIM_Security_Role_RW"
        };

        // AD Lookup Tool Constants
        public const string OmniUsername = "";
        public const string OmniPassword = "";

        // Centrify Tools Security Groups
        public const string CfyAdminRole = "IAM_Tool";

        public static List<string> CfyProvisioningRole = new()
        {
            "AIM_TOOL_CFY_PROVISIONING",
        };

        // Service Catalog Security Groups
        public const string ServiceCatalogUserRole = "AIM_FACTOOL_USER";
        public const string ServiceCatalogMgrRole = "AIM_FACTOOL_MGR";

        // Factool Access Level Groups
        public const string FactoolUserRole = "AIM_FACTOOL_USER";
        public const string FactoolMgrRole = "AIM_FACTOOL_MGR";
        public const string FactoolAdminRole = "AIM_FACTOOL_ADMIN";

        // Factool Configuration 
        public const string SPSiteURL = "https://collab.corp.Company.com/sites/IAM/Team/MailboxRequests/";
        public const string SPList = "CompletionLog";
        public const string MailFrom = "IdentityAccessNotification@Companyhealth.com";
        public const string MailTo = "christopher.olson2@Companyhealth.com";
        public const string SmtpServer = "smtpri.corp.Company.com";
        public const string FactoolAppName = "FactoolRequestSync";
        public enum RequestMode { New, Existing, PreExisting, Complete };
        public static List<string> FacMinRole = new()
        {
            "RBAC_AIM_ADM_TIER1",
            "RBAC_AIM_ADM_TIER2"
        };

        // Factool Update Request Permission Groups
        public static List<string> FactoolCompletionRole = new()
        {
            "AIM_OIM",
            "AIM_Security_Role_R",
            "AIM_Security_Role_RW",
            "RX\\AIM_OIM",
            "RX\\AIM_Security_Role_R",
            "RX\\AIM_Security_Role_RW",
            "CORP\\AIM_OIM",
            "CORP\\AIM_Security_Role_R",
            "CORP\\AIM_Security_Role_RW",
            "Company\\AIM_OIM",
            "Company\\AIM_Security_Role_R",
            "Company\\AIM_Security_Role_RW",
            "DL_ICSLANAdmins",
            "gsecadmin",
            "RX\\gsecadmin",
            "CORP\\gsecadmin",
            "Company\\gsecadmin"
        };

        // Centrify Configuration
        public const string CfyRootZone = "corp.Companyc.com/Centrify/Zones/Global";
        public const string LDAPPath = "LDAP://corp.Company.com/";
        public const string CfyAD = "corp.Company.com";
        public const string CfyComputerOU = "OU=Computers,OU=Centrify,DC=corp,DC=Company,DC=com";
        public static List<string> CfyChildZones = new()
        {
            "Global",
            "Corp",
            "PCI",
            "PCI_AIX",
            "CORP_AIX"
        };
        public static List<string> CfyComputerOUList = new()
        {
            "OU=Computers",
            "OU=Centrify",
            "DC=corp",
            "DC=Company",
            "DC=com"
        };
        public static List<string> GetComputerRolesTypes = new()
        {
            "All",
            "APPL",
            "INFRA",
            "JMPSVR"
        };
        public static List<string> GetComputerRolesEnvironments = new()
        {
            "All",
            "NonProd",
            "PreProd",
            "Prod",
            "OffShore"
        };
        public static List<string> GetComputerRolesPCI = new()
        {
            "All",
            "PCI",
            "NONPCI"
        };

        // Group Query / Add Group Configuration
        public static List<string> GroupQueryAttributes = new()
        {
            "name",
            "objectcategory",
            "samaccountname",
            "employeenumber",
            "useraccountcontrol",
            "mail",
            "distinguishedname",
            "lastlogontimestamp"
        };
        public static List<string> ADDomains = new()
        {
            "corp.Company.com",
            "corp.Company.com",
            "rx.net",
            "procare.pcare.ltd",
            "ridc.pcare.ltd",
            "pharmacare.pcare.ltd",
            "minclinic.local",
            "storz.Company.com"
        };

        // Heimdall Dev User List
        public static List<string> DevList = new()
        {
            "C067460",
            "Olson, Christopher M",
            "A_C067460",
        };


        // User Configuration Defaults
        public static string FontScaleDefault = "1.0";
        public static int SmallFont = 12;
        public static int MediumFont = 14;
        public static int LargeFont = 16;
        #endregion
    }
}

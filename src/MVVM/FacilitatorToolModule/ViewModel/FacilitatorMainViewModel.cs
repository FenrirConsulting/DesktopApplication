using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FacilitatorMainViewModel : BaseViewModel
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties
        // Factool Shared Properties
        public static FacToolRequest CurrentRequest { get; set; }
        public static string ReferenceNumber { get; set; }

        // Object to Move between View Models
        private BaseViewModel _selectedGroupViewModel;
        public BaseViewModel SelectedGroupViewModel
        {
            get { return _selectedGroupViewModel; }
            set { _selectedGroupViewModel = value; OnPropertyChanged(); }
        }

        // Get Facilitator Request View Tab
        private FacilitatorRequestViewModel facilitatorRequestVm;
        public FacilitatorRequestViewModel FacilitatorRequestVm
        {
            get { return facilitatorRequestVm; }
            set { facilitatorRequestVm = value; OnPropertyChanged(); }
        }

        // Get Facilitator Query View Tab
        private FacilitatorQueryViewModel facilitatorQueryVm;
        public FacilitatorQueryViewModel FacilitatorQueryVm
        {
            get { return facilitatorQueryVm; }
            set { facilitatorQueryVm = value; OnPropertyChanged(); }
        }

        // Get Facilitator Maintenance View Tab
        private FacilitatorMaintenanceViewModel facilitatorMaintenanceVm;
        public FacilitatorMaintenanceViewModel FacilitatorMaintenanceVm
        {
            get { return facilitatorMaintenanceVm; }
            set { facilitatorMaintenanceVm = value; OnPropertyChanged(); }
        }

        private bool maintenanceVisibleCheck;
        public bool MaintenanceVisibleCheck
        {
            get { return maintenanceVisibleCheck; }
            set { if (maintenanceVisibleCheck != value) { maintenanceVisibleCheck = value; OnPropertyChanged(); } }
        }

        private bool syncVisibleCheck;
        public bool SyncVisibleCheck
        {
            get { return syncVisibleCheck; }
            set { if (syncVisibleCheck != value) { syncVisibleCheck = value; OnPropertyChanged(); } }
        }

        // Binds Sync Running String
        private string syncRunningString;
        public string SyncRunningString
        {
            get { return syncRunningString; }
            set
            {
                if (syncRunningString != value)
                {
                    syncRunningString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Object to move between View Models
        private int selectedTab;
        public int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; OnPropertyChanged(); }
        }

        // Holds Datatable for SQL Table Select
        public static DataTable FormTypeTable { get; set; }
        public static DataTable QueryRequestTable { get; set; }
        public static DataTable RequestTypesTable { get; set; }
        public static DataTable DefectReasonsTable { get; set; }
        public static DataTable SystemsTable { get; set; }
        public static DataTable ReplyTypesTable { get; set; }
        public static DataTable LOBTypeTable { get; set; }
        public static string HtmlBody { get; set; }
        #endregion

        #region Methods
        public FacilitatorMainViewModel()
        {
            FacilitatorRequestVm = new();
            FacilitatorQueryVm = new();
            FacilitatorMaintenanceVm = new();
            CurrentRequest = new();
            FormTypeTable = new();
            QueryRequestTable = new();
            RequestTypesTable = new();
            DefectReasonsTable = new();
            SystemsTable = new();
            ReplyTypesTable = new();
            SelectedTab = 0;
            MaintenanceVisibleCheck = MainViewModel.GlobalFacToolMgrPermission;
            SyncVisibleCheck = MainViewModel.GlobalFacToolAdminPermission;
            MSG.Messenger.Default.Register<String>(this, TabMessage);
            CurrentRequest = null;
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            try
            {
                LoadTables();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Load Factool Tables from SQL Data
        public void LoadTables()
        {
            FormTypeTable.Clear();
            RequestTypesTable.Clear();
            DefectReasonsTable.Clear();
            SystemsTable.Clear();
            ReplyTypesTable.Clear();

            FormTypeTable = DBConn.GetTable(ConfigurationProperties.FactoolConfigFormType);
            RequestTypesTable = DBConn.GetTable(ConfigurationProperties.FactoolConfigRequestType);
            DefectReasonsTable = DBConn.GetTable(ConfigurationProperties.FactoolConfigDefectReasons);
            SystemsTable = DBConn.GetTable(ConfigurationProperties.FactoolConfigSystems);
            ReplyTypesTable = DBConn.GetTable(ConfigurationProperties.FactoolConfigReplyType);
            LOBTypeTable = DBConn.GetTable(ConfigurationProperties.FactoolLOBTypes);

            MaintenanceVisibleCheck = MainViewModel.GlobalFacToolMgrPermission;
            SyncVisibleCheck = MainViewModel.GlobalFacToolAdminPermission;
        }
        #endregion

        #region Messages
        // Listen to New Record for Database Reload
        public void TabMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                switch (passedMessage)
                {

                    case "Select FacTool Tab 1":
                        SelectedTab = 0;
                        break;

                    case "Select FacTool Tab 2":
                        SelectedTab = 1;
                        break;

                    case "Select FacTool Tab 3":
                        SelectedTab = 2;
                        break;

                    case "Reload Factool Tables":
                        LoadTables();
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}

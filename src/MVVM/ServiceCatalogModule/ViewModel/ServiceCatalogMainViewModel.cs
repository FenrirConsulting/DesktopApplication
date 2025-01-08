using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ServiceCatalogMainViewModel : BaseViewModel
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties
        // Admin Role Bool Binding
        private bool adminRole;
        public bool AdminRole
        {
            get { return adminRole; }
            set { adminRole = value; OnPropertyChanged(); }
        }

        // Holds the Service Item ID to pass between pages
        public static string CurrentServiceItem { get; set; }

       // Holds the Sharepoint Client Id to pass between pages
        public static string SPClientId { get; set; }


        // Holds the Sharepoint Client Secret to pass between pages
        public static string SPClientSecret { get; set; }

        // Object to Move between View Models
        private BaseViewModel _selectedGroupViewModel;
        public BaseViewModel SelectedGroupViewModel
        {
            get { return _selectedGroupViewModel; }
            set { _selectedGroupViewModel = value; OnPropertyChanged(); }
        }

        // Get Service Catalog Lookup View Tab
        private ServiceCatalogLookupViewModel serviceCatalogLookupVm;
        public ServiceCatalogLookupViewModel ServiceCatalogLookupVm
        {
            get { return serviceCatalogLookupVm; }
            set { serviceCatalogLookupVm = value; OnPropertyChanged(); }
        }

        // Get Service Catalog Update View Tab
        private ServiceCatalogUpdateViewModel serviceCatalogUpdateVm;
        public ServiceCatalogUpdateViewModel ServiceCatalogUpdateVm
        {
            get { return serviceCatalogUpdateVm; }
            set { serviceCatalogUpdateVm = value; OnPropertyChanged(); }
        }

        // Get Service Catalog Add View Tab
        private ServiceCatalogAddViewModel serviceCatalogAddVm;
        public ServiceCatalogAddViewModel ServiceCatalogAddVm
        {
            get { return serviceCatalogAddVm; }
            set { serviceCatalogAddVm = value; OnPropertyChanged(); }
        }

        // Get Service Catalog Configuration View Tab
        private ServiceCatalogConfigurationViewModel serviceCatalogConfigVm;
        public ServiceCatalogConfigurationViewModel ServiceCatalogConfigVm
        {
            get { return serviceCatalogConfigVm; }
            set { serviceCatalogConfigVm = value; OnPropertyChanged(); }
        }

        // Object to move between View Models
        private int selectedTab;
        public int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public ServiceCatalogMainViewModel()
        {
            SPClientId = "";
            SPClientSecret = "";
            ServiceCatalogLookupVm = new();
            ServiceCatalogUpdateVm = new();
            ServiceCatalogAddVm = new();
            ServiceCatalogConfigVm = new();
            MSG.Messenger.Default.Register<String>(this, TabMessage);
            AdminRole = false;
        }
        #endregion 

        #region Functions

        public void LoadData()
        {
            AdminRole = MainViewModel.GlobalServiceCatalogAdminEnabled;
            SetSPConfiguration();
        }

        public void SetSPConfiguration()
        {
            DataTable configTable = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "SPClientIDTest"); // SPClientID <- Set to this to the Prod Id
            string SPClientId = configTable.Rows[0][2].ToString();
            configTable = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "SPClientSecretTest"); // SPClientSecret <- Set this to Prod Secret
            string SPClientSecret = configTable.Rows[0][2].ToString();

            ServiceCatalogMainViewModel.SPClientId = SPClientId;
            ServiceCatalogMainViewModel.SPClientSecret = SPClientSecret;
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

                    case "Select Service Catalog Update Tab":
                        SelectedTab = 1;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}

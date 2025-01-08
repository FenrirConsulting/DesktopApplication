using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ServiceCatalogConfigurationViewModel : BaseViewModel
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();
        private readonly SharePointMethods SPMethods = new();

        public RelayCommand UpdateRequestCommand { get; set; }
        public RelayCommand AddRequestCommand { get; set; }
        public RelayCommand DeleteRequestCommand { get; set; }
        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand AddEnvironmentCommand { get; set; }
        public RelayCommand DeleteEnvironmentCommand { get; set; }
        public RelayCommand DeleteGroupCommand { get; set; }
        #endregion

        #region Properties
        // Binds Spinner Loading Bool Value
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ServiceCatalogSourceItemModel> requestSourcesList = new()
        {
            
        };

        // Basis for Search Terms Collection
        public IEnumerable<ServiceCatalogSourceItemModel> RequestSourcesList
        {
            get { return requestSourcesList; }
        }

        // Selected Term for ComboBox
        private ServiceCatalogSourceItemModel selectedRequestSource = new();
        public ServiceCatalogSourceItemModel SelectedRequestSource
        {
            get { return selectedRequestSource; }
            set { 
                selectedRequestSource = value;
                if (requestSourcesList != null && requestSourcesList.Count > 0) { RequestChanged(); }
                OnPropertyChanged(); 
            }
        }

        // Binds URL Update String
        private string uRLUpdateString;
        public string URLUpdateString
        {
            get { return uRLUpdateString; }
            set
            {
                if (uRLUpdateString != value)
                {
                    uRLUpdateString = value;
                    OnPropertyChanged();
                }
            }
        }

       // Binds Request Add String
        private string requestAddString;
        public string RequestAddString
        {
            get { return requestAddString; }
            set
            {
                if (requestAddString != value)
                {
                    requestAddString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds URL Add String
        private string uRLAddString;
        public string URLAddString
        {
            get { return uRLAddString; }
            set
            {
                if (uRLAddString != value)
                {
                    uRLAddString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Assignment Group Add String
        private string assignmentGroupAddString;
        public string AssignmentGroupAddString
        {
            get { return assignmentGroupAddString; }
            set
            {
                if (assignmentGroupAddString != value)
                {
                    assignmentGroupAddString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Envrionment Add String
        private string environmentAddString;
        public string EnvironmentAddString
        {
            get { return environmentAddString; }
            set
            {
                if (environmentAddString != value)
                {
                    environmentAddString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Data for Display Table
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView displayView;
        public DataView DisplayView
        {
            get { return displayView; }
            set { displayView = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table for Environment
        private DataTable displayTableEnvironment;
        public DataTable DisplayTableEnvironment
        {
            get { return displayTableEnvironment; }
            set { displayTableEnvironment = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable for Environment
        private DataView displayViewEnvironment;
        public DataView DisplayViewEnvironment
        {
            get { return displayViewEnvironment; }
            set { displayViewEnvironment = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record for Environment
        private DataGridCellInfo cellInfoEnvironment;
        public DataGridCellInfo CellInfoEnvironment
        {
            get { return cellInfoEnvironment; }
            set { cellInfoEnvironment = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public ServiceCatalogConfigurationViewModel()
        {
            // Initialize Propeties
            DisplayView = new();
            DisplayTable = new();
            CellInfo = new();
            DisplayViewEnvironment = new();
            DisplayTableEnvironment = new();
            CellInfoEnvironment = new();
            requestSourcesList = new();
            SelectedRequestSource.URL = "";
            SelectedRequestSource.RequestSource = "";
            URLAddString = "";
            RequestAddString = "";
            IsLoadBool = false;

            //Initialize Relay Commands
            UpdateRequestCommand = new RelayCommand(o => { UpdateRequest(); });
            AddRequestCommand = new RelayCommand(o => { AddRequest(); });
            DeleteRequestCommand = new RelayCommand(o => { DeleteRequest(); });
            AddGroupCommand = new RelayCommand(o => { AddGroup(); });
            DeleteGroupCommand = new RelayCommand(o => { DeleteGroup(); });
            AddEnvironmentCommand = new RelayCommand(o => { AddEnvironment(); });
            DeleteEnvironmentCommand = new RelayCommand(o => { DeleteEnvironment(); });
        }
        #endregion

        #region Functions
        // Main On Load Function
        public void LoadData()
        {
            try
            {
                requestSourcesList.Clear();
                DataTable RequestTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogSourcesTable);
                DataTable GroupsTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogAssignmentGroupsTable);
                DataTable EnvironmentTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogEnvironmentsTable);

                foreach (DataRow row in RequestTable.Rows)
                {
                    requestSourcesList.Add(new ServiceCatalogSourceItemModel { Id = Int32.Parse(row["Id"].ToString()), RequestSource = row["RequestSource"].ToString(), URL = row["URL"].ToString() });
                }
                SelectedRequestSource = requestSourcesList.ElementAt(0);

                DisplayTable.Clear();
                DisplayView = DisplayTable.DefaultView;
                DisplayTable = GroupsTable.Copy();
                DisplayView = DisplayTable.DefaultView;

                DisplayTableEnvironment.Clear();
                DisplayViewEnvironment = DisplayTableEnvironment.DefaultView;
                DisplayTableEnvironment = EnvironmentTable.Copy();
                DisplayViewEnvironment = DisplayTableEnvironment.DefaultView;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // On Request Source ComboBox Change, Sets URL to Match ID Field
        public void RequestChanged()
        {
            if (SelectedRequestSource.URL != null || SelectedRequestSource.URL != "")
            {
                URLUpdateString = SelectedRequestSource.URL;
            }
        }

        // Function to Update Request List
        public async void UpdateRequest()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Update this Request Source?", "Update Request Source??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Uri uriResult;
                    string mailToSubString = "";
                    if (URLUpdateString.Length > 6) { mailToSubString = URLUpdateString.Substring(0, 7); }
                    bool result = (Uri.TryCreate(URLUpdateString, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) || mailToSubString == "mailto:");

                    if (result == false)
                    {
                        MessageBox.Show("Invalid URL, Please enter a Valid URL", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        IsLoadBool = true;
                        ServiceCatalogSourceItemModel updateModel = new();
                        updateModel.Id = SelectedRequestSource.Id;
                        updateModel.RequestSource = SelectedRequestSource.RequestSource;
                        updateModel.URL = URLUpdateString;

                        await Task.Run(() => { DBConn.ServiceCatalogSourceUpdate(updateModel, ConfigurationProperties.ServiceCatalogSourcesTable); });

                        var UpdatedSources = await DBConn.GetSelectedRecordAsync(ConfigurationProperties.ServiceCatalogTable, "RequestSource", updateModel.RequestSource);
                        DataTable tempTable = UpdatedSources.Copy();

                        await Task.Run(() => 
                        {
                            foreach (DataRow row in tempTable.Rows)
                            {
                               DBConn.UpdateTableRecord(ConfigurationProperties.ServiceCatalogTable, row["ID"].ToString(), "ID", "URL", updateModel.URL);
                            }
                        });

                        var UpdatedSourcesSharepoint = await DBConn.GetSelectedRecordAsync(ConfigurationProperties.ServiceCatalogTable, "RequestSource", updateModel.RequestSource);
                        DataTable tempSPTable = UpdatedSourcesSharepoint.Copy();
                        var waitSharepoint = SPMethods.UpdateAllItems(tempSPTable);

                        LoadData();
                        IsLoadBool = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Add to Request List
        public async void AddRequest()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Create this Request Source?", "Create Request Source??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Uri uriResult;
                    string mailToSubString = "";
                    if (URLAddString.Length > 6) { mailToSubString = URLAddString.Substring(0, 7); }
                    bool result = (Uri.TryCreate(URLAddString, UriKind.Absolute, out uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) || mailToSubString == "mailto:");

                    if (result == false)
                    {
                        MessageBox.Show("Invalid URL, Please enter a Valid URL", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        ServiceCatalogSourceItemModel updateModel = new();
                        updateModel.Id = 1;
                        updateModel.RequestSource = RequestAddString;
                        updateModel.URL = URLAddString;

                        await Task.Run(() => { DBConn.AddServiceCatalogSourceRecord(updateModel, ConfigurationProperties.ServiceCatalogSourcesTable); });

                        RequestAddString = "";
                        URLAddString = "";
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Delete Request from List
        public async void DeleteRequest()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Delete this Request Source?", "Delete Request Source??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string passedID = SelectedRequestSource.Id.ToString();

                    await Task.Run(() => { DBConn.DeleteMatchingRecords(ConfigurationProperties.ServiceCatalogSourcesTable, "Id", passedID); });

                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Add Group to List
        public async void AddGroup()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Create this Assignment Group?", "Create Assignment Group??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string passedGroup = AssignmentGroupAddString;
                    await Task.Run(() => { DBConn.AddServiceCatalogGroupRecord(passedGroup, ConfigurationProperties.ServiceCatalogAssignmentGroupsTable); });
                    AssignmentGroupAddString = "";
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Delete Group from List
        public async void DeleteGroup()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Delete this Assignment Group?", "Delete Assignment Group??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataRowView drv = (DataRowView)CellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            string passedId = row["Id"].ToString();
                            await Task.Run(() => { DBConn.DeleteMatchingRecords(ConfigurationProperties.ServiceCatalogAssignmentGroupsTable, "Id", passedId); });
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Add Envrionment to List
        public async void AddEnvironment()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Create this Environment?", "Create Environment", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string passedEnvironment = EnvironmentAddString;
                    await Task.Run(() => { DBConn.AddServiceCatalogEnvironmentRecord(passedEnvironment, ConfigurationProperties.ServiceCatalogEnvironmentsTable); });
                    EnvironmentAddString = "";
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Delete Envrionment from List
        public async void DeleteEnvironment()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to Delete this Environment?", "Delete Environment??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataRowView drv = (DataRowView)CellInfoEnvironment.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            string passedId = row["Id"].ToString();
                            await Task.Run(() => { DBConn.DeleteMatchingRecords(ConfigurationProperties.ServiceCatalogEnvironmentsTable, "Id", passedId); });
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Builds the Current Service Catalog Item from DB
        public ServiceCatalogItemModel BuildItem(DataRow passedRow, string UpdateURL)
        {
            ServiceCatalogItemModel TemporaryItem = new()
            {
                ID = 0,
                Services = "",
                Description = "",
                RequestSource = "",
                AssignmentGroup = "",
                ExpediteProcess = "",
                SLA = "",
                ArcherAppID = "",
                ITPMAppID = "",
                Hints = "",
                Tasks = "",
                Domain = new(),
                AssignedDomains = new(),
                AvailableDomains = new(),
                Article = "",
                ProvisioningTeam = "",
                URL = "",
                AutomationStatus = ""
            };

            try
            {
                TemporaryItem.ID = Convert.ToInt32(passedRow["ID"]);
                TemporaryItem.Services = passedRow["Services"].ToString();
                TemporaryItem.Description = passedRow["Description"].ToString(); 
                TemporaryItem.RequestSource = passedRow["RequestSource"].ToString(); 
                TemporaryItem.AssignmentGroup = passedRow["AssignmentGroup"].ToString(); 
                TemporaryItem.ExpediteProcess = passedRow["ExpediteProcess"].ToString();
                TemporaryItem.SLA = passedRow["SLA"].ToString(); 
                TemporaryItem.ArcherAppID = passedRow["ArcherAppID"].ToString();
                TemporaryItem.ITPMAppID = passedRow["ITPMAppID"].ToString(); 
                TemporaryItem.Hints = passedRow["Hints"].ToString(); 
                TemporaryItem.Tasks = passedRow["Tasks"].ToString(); 
                string splitDomains = passedRow["Domain"].ToString();
                TemporaryItem.Domain = splitDomains.Split(";").ToList();
                TemporaryItem.Article = passedRow["Article"].ToString(); 
                TemporaryItem.ProvisioningTeam = passedRow["ProvisioningTeam"].ToString(); 
                TemporaryItem.URL = UpdateURL; 
                TemporaryItem.AutomationStatus = passedRow["AutomationStatus"].ToString();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

            return TemporaryItem;
        }
        #endregion
    }
}

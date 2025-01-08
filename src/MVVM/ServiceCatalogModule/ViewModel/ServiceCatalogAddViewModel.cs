using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ServiceCatalogAddViewModel : BaseViewModel
    {
        #region Delegates
        //Access SQL Methods Class
        private readonly SQLMethods DBConn = new();
        private readonly SharePointMethods SPMethods = new();

        //Relay Commands for Buttons
        public RelayCommand AddItemCommand { get; set; }
        public RelayCommand RemoveListCommand { get; set; }
        public RelayCommand AddListCommand { get; set; }
        public RelayCommand SpaceCheckboxCommand { get; set; }
        #endregion

        #region Properties
        // Binds text of Selected Item from Data View Page to Variable
        private string selectedItem;
        public string SelectedItem
        {
            get { return selectedItem; }
            set { if (selectedItem != value) { selectedItem = value; OnPropertyChanged(); } }
        }

        // Binds Item Model Current Instance
        private ServiceCatalogItemModel currentItem;
        public ServiceCatalogItemModel CurrentItem
        {
            get { return currentItem; }
            set { if (currentItem != value) { currentItem = value; OnPropertyChanged(); } }
        }

        // Binds text of Service String from Current Item
        private string serviceString;
        public string ServiceString
        {
            get { return serviceString; }
            set { if (serviceString != value) { serviceString = value; OnPropertyChanged(); } }
        }

        // Binds text of SubSystem String from Current Item
        private string subSystemString;
        public string SubSystemString
        {
            get { return subSystemString; }
            set { if (subSystemString != value) { subSystemString = value; OnPropertyChanged(); } }
        }

        // Binds text of Description String from Current Item
        private string descriptionString;
        public string DescriptionString
        {
            get { return descriptionString; }
            set { if (descriptionString != value) { descriptionString = value; OnPropertyChanged(); } }
        }

        // Binds text of Expedite String from Current Item
        private string expediteString;
        public string ExpediteString
        {
            get { return expediteString; }
            set { if (expediteString != value) { expediteString = value; OnPropertyChanged(); } }
        }

        // Binds text of SLA String from Current Item
        private string sLAString;
        public string SLAString
        {
            get { return sLAString; }
            set { if (sLAString != value) { sLAString = value; OnPropertyChanged(); } }
        }

        // Binds text of Request String from Current Item
        private string requestString;
        public string RequestString
        {
            get { return requestString; }
            set { if (requestString != value) { requestString = value; OnPropertyChanged(); } }
        }

        // Binds text of URL String from Current Item
        private string uRLString;
        public string URLString
        {
            get { return uRLString; }
            set { if (uRLString != value) { uRLString = value; OnPropertyChanged(); } }
        }

        // Binds text of Tasks String from Current Item
        private string tasksString;
        public string TasksString
        {
            get { return tasksString; }
            set { if (tasksString != value) { tasksString = value; OnPropertyChanged(); } }
        }

        // Binds text of Hints String from Current Item
        private string hintsString;
        public string HintsString
        {
            get { return hintsString; }
            set { if (hintsString != value) { hintsString = value; OnPropertyChanged(); } }
        }

        // Binds text of Assignments String from Current Item
        private string assignmentString;
        public string AssignmentString
        {
            get { return assignmentString; }
            set { if (assignmentString != value) { assignmentString = value; OnPropertyChanged(); } }
        }

        // Binds text of Archer String from Current Item
        private string archerString;
        public string ArcherString
        {
            get { return archerString; }
            set { if (archerString != value) { archerString = value; OnPropertyChanged(); } }
        }

        // Binds text of ITPM String from Current Item
        private string iTPMString;
        public string ITPMString
        {
            get { return iTPMString; }
            set { if (iTPMString != value) { iTPMString = value; OnPropertyChanged(); } }
        }

        // Binds text of Provisioning String from Current Item
        private string provisioningString;
        public string ProvisioningString
        {
            get { return provisioningString; }
            set { if (provisioningString != value) { provisioningString = value; OnPropertyChanged(); } }
        }

        // Binds text of Article String from Current Item
        private string articleString;
        public string ArticleString
        {
            get { return articleString; }
            set { if (articleString != value) { articleString = value; OnPropertyChanged(); } }
        }

        // Selected Term for Assigned List Box
        private ServiceCatalogDomainTermModel selectedAssignedDomain = new();
        public ServiceCatalogDomainTermModel SelectedAssignedDomain
        {
            get { return selectedAssignedDomain; }
            set { selectedAssignedDomain = value; OnPropertyChanged(); }
        }

        // Basis for Assigned Terms Collection
        public IEnumerable<ServiceCatalogDomainTermModel> AssignedDomains
        {
            get { return assignedDomains; }
        }

        // Assigned Terms for List Box
        private ObservableCollection<ServiceCatalogDomainTermModel> assignedDomains = new()
        {

        };

        // Selected Term for Assigned List Box
        private ServiceCatalogDomainTermModel selectedAvailableDomain = new();
        public ServiceCatalogDomainTermModel SelectedAvailableDomain
        {
            get { return selectedAvailableDomain; }
            set { selectedAvailableDomain = value; OnPropertyChanged(); }
        }

        // Basis for Assigned Terms Collection
        public IEnumerable<ServiceCatalogDomainTermModel> AvailableDomains
        {
            get { return availableDomains; }
        }

        // Assigned Terms for List Box
        private ObservableCollection<ServiceCatalogDomainTermModel> availableDomains = new()
        {

        };


        // Holds List of Domains Available from SQL Config Table
        private List<string> listOfDomains;
        public List<string> ListOfDomains
        {
            get { return listOfDomains; }
            set
            {
                if (listOfDomains != value)
                {
                    listOfDomains = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds text of Article String from Current Item
        private string statusString;
        public string StatusString
        {
            get { return statusString; }
            set { if (statusString != value) { statusString = value; OnPropertyChanged(); } }
        }

        // Binds text of Article String from Current Item
        private bool newEnabled;
        public bool NewEnabled
        {
            get { return newEnabled; }
            set { if (newEnabled != value) { newEnabled = value; OnPropertyChanged(); } }
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
            set
            {
                selectedRequestSource = value;
                if (requestSourcesList != null && requestSourcesList.Count > 0) { RequestChanged(); }
                OnPropertyChanged();
            }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ServiceCatalogEnvironmentItemModel> requestEnvironmentList = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<ServiceCatalogEnvironmentItemModel> RequestEnvironmentList
        {
            get { return requestEnvironmentList; }
        }

        // Selected Term for ComboBox
        private ServiceCatalogEnvironmentItemModel selectedRequestEnvironment = new();
        public ServiceCatalogEnvironmentItemModel SelectedRequestEnvironment
        {
            get { return selectedRequestEnvironment; }
            set
            {
                selectedRequestEnvironment = value;
                OnPropertyChanged();
            }
        }

        // Binds Automation Bool Status from Current Item
        private bool automationBool;
        public bool AutomationBool
        {
            get { return automationBool; }
            set { if (automationBool != value) { automationBool = value; OnPropertyChanged(); } }
        }

        // Binds text of Automation String from Current Item
        private string automationStatusString;
        public string AutomationStatusString
        {
            get { return automationStatusString; }
            set { if (automationStatusString != value) { automationStatusString = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection Group Assignment ListView
        private readonly ObservableCollection<ServiceCatalogAssignmentGroupItemModel> assignmentGroupsList = new()
        {
        };
        public IEnumerable<ServiceCatalogAssignmentGroupItemModel> AssignmentGroupsList
        {
            get { return assignmentGroupsList; }
        }

        // Selected Term for Group Assignment ListView
        private ServiceCatalogAssignmentGroupItemModel selectedAssignmentGroup = new();
        public ServiceCatalogAssignmentGroupItemModel SelectedAssignmentGroup
        {
            get { return selectedAssignmentGroup; }
            set
            {
                selectedAssignmentGroup = value;
                OnPropertyChanged();
            }
        }

        // Binds AssignmentGroupsStringList
        private List<string> assignmentGroupsStringList;
        public List<string> AssignmentGroupsStringList
        {
            get { return assignmentGroupsStringList; }
            set
            {
                if (assignmentGroupsStringList != value)
                {
                    assignmentGroupsStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds text of Article String from Current Item
        private bool addEnabled;
        public bool AddEnabled
        {
            get { return addEnabled; }
            set { if (addEnabled != value) { addEnabled = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public ServiceCatalogAddViewModel()
        {
            CurrentItem = new()
            {
                ID = 0,
                Services = "",
                SubSystem = "",
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
                Environment = ""
                
            };
            ListOfDomains = new();
            NewEnabled = true;
            AddEnabled = true;
            SPMethods = new();
            RemoveListCommand = new RelayCommand(o => { RemoveList(); });
            AddListCommand = new RelayCommand(o => { AddList(); });
            AddItemCommand = new RelayCommand(o => { AddItem(); });
            SpaceCheckboxCommand = new RelayCommand((box) => { SpaceCheckbox(box.ToString()); });
        }
        #endregion

        #region Functions
        // Primary On-Load Function 
        public void LoadData()
        {
            try
            {
                CurrentItem = new()
                {
                    ID = 0,
                    Services = "",
                    SubSystem = "",
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
                    AutomationStatus = "",
                    Environment = ""
                };

                AutomationBool = false;
                List<double> serviceRecordsDoubleList = DBConn.GetColumnListDouble(ConfigurationProperties.ServiceCatalogTable, "ID");
                double maxRecordDouble = serviceRecordsDoubleList.Max();
                int maxRecord = (int)maxRecordDouble;
                maxRecord++;
                CurrentItem.ID = maxRecord;
                StatusString = "Creating Record " + maxRecord.ToString();
                DataTable fetchList = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "ServiceCatalogDomains");
                string listFetch = fetchList.Rows[0][2].ToString();
                availableDomains.Clear();

                ListOfDomains = listFetch.Split(";").ToList();
                foreach (string s in ListOfDomains)
                {
                    availableDomains.Add(new ServiceCatalogDomainTermModel { DomainTerm = s }); ;
                }

                DataTable SourcesTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogSourcesTable);

                foreach (DataRow row in SourcesTable.Rows)
                {
                    requestSourcesList.Add(new ServiceCatalogSourceItemModel { Id = Int32.Parse(row["Id"].ToString()), RequestSource = row["RequestSource"].ToString(), URL = row["URL"].ToString() });
                }

                assignmentGroupsList.Clear();

                DataTable GroupsTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogAssignmentGroupsTable);

                foreach (DataRow row in GroupsTable.Rows)
                {
                    assignmentGroupsList.Add(new ServiceCatalogAssignmentGroupItemModel { Id = Int32.Parse(row["Id"].ToString()), GroupName = row["GroupName"].ToString(), IsSelected = false });
                }

                DataTable EnvrionmentsTable = DBConn.GetTable(ConfigurationProperties.ServiceCatalogEnvironmentsTable);

                foreach (DataRow row in EnvrionmentsTable.Rows)
                {
                    requestEnvironmentList.Add(new ServiceCatalogEnvironmentItemModel { Id = Int32.Parse(row["Id"].ToString()), RequestEnvironment = row["EnvironmentName"].ToString()});
                }

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Add Item to SharePoint list as Well as SQL Table
        public async void AddItem()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to create this item record?", "Create Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(ServiceString))
                    {
                        MessageBox.Show("Please enter at least one service.");
                    }
                    else if (assignedDomains.Count < 1)
                    {
                        MessageBox.Show("Please assign at least one domain.");
                    }
                    else
                    {
                        string joinedServices = ServiceString.Replace(Environment.NewLine, ";");
                        CurrentItem.Services = joinedServices;
                        CurrentItem.SubSystem = SubSystemString;
                        CurrentItem.Description = DescriptionString;
                        CurrentItem.RequestSource = SelectedRequestSource.RequestSource;

                        CurrentItem.Environment = SelectedRequestEnvironment.RequestEnvironment;

                        List<string> selectedGroups = new();
                        foreach (ServiceCatalogAssignmentGroupItemModel item in assignmentGroupsList)
                        {
                            if (item.IsSelected == true)
                            {
                                selectedGroups.Add(item.GroupName);
                            }
                        }
                        string joinedGroups = string.Join(";", selectedGroups);

                        CurrentItem.AssignmentGroup = joinedGroups;
                        CurrentItem.ExpediteProcess = ExpediteString;
                        CurrentItem.SLA = SLAString;
                        CurrentItem.ArcherAppID = ArcherString;
                        CurrentItem.ITPMAppID = ITPMString;
                        CurrentItem.Hints = HintsString;
                        CurrentItem.Tasks = TasksString;

                        foreach (ServiceCatalogDomainTermModel term in AssignedDomains)
                        {
                            CurrentItem.Domain.Add(term.DomainTerm);
                        }
                        CurrentItem.AssignedDomains = CurrentItem.Domain;
                        CurrentItem.AvailableDomains = CurrentItem.Domain;

                        CurrentItem.Article = ArticleString;
                        CurrentItem.ProvisioningTeam = ProvisioningString;
                        CurrentItem.URL = URLString;
                        if (AutomationBool == true) { CurrentItem.AutomationStatus = "Automated"; } else { CurrentItem.AutomationStatus = "Manual"; }


                        DBConn.AddServiceCatalogRecord(CurrentItem, ConfigurationProperties.ServiceCatalogTable);
                        string updatedRecord = CurrentItem.ID.ToString();

                        ServiceString = "";
                        SubSystemString = "";
                        DescriptionString = "";
                        RequestString = "";
                        AssignmentString = "";
                        ExpediteString = "";
                        SLAString = "";
                        ArcherString = "";
                        ITPMString = "";
                        HintsString = "";
                        TasksString = "";
                        assignedDomains.Clear();
                        ArticleString = "";
                        ProvisioningString = "";
                        URLString = "";
                        AutomationBool = false;

                        DataTable fetchList = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "ServiceCatalogDomains");
                        string listFetch = fetchList.Rows[0][2].ToString();
                        availableDomains.Clear();
                        ListOfDomains = listFetch.Split(";").ToList();

                        foreach (string s in ListOfDomains)
                        {
                            availableDomains.Add(new ServiceCatalogDomainTermModel { DomainTerm = s }); ;
                        }

                        foreach (ServiceCatalogAssignmentGroupItemModel item in assignmentGroupsList)
                        {
                            item.IsSelected = false;
                        }

                        SelectedRequestSource = requestSourcesList.ElementAt(0);
                        SelectedRequestEnvironment = requestEnvironmentList.ElementAt(0); 

                        var awaitTask = await DBConn.GetSelectedRecordAsync(ConfigurationProperties.ServiceCatalogTable, "ID", CurrentItem.ID.ToString());
                        DataTable tempTable = awaitTask;
                        var awaitInsert = await SPMethods.Insert(tempTable, false);

                        int maxRecord = CurrentItem.ID;
                        CurrentItem.ID++;
                        StatusString = "Record #" + maxRecord.ToString() + " Created Succesfully. Now Creating Record #" + CurrentItem.ID.ToString();

                        MSG.Messenger.Default.Send("Reload Service Catalog Table");
                    }

                }
                else
                {
                    //Do Nothing
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Add Record Arrow Button
        public void AddList()
        {
            assignedDomains.Add(this.SelectedAvailableDomain);
            availableDomains.Remove(this.SelectedAvailableDomain);
        }

        // Remove Record Arrow Button
        public void RemoveList()
        {
            availableDomains.Add(this.SelectedAssignedDomain);
            assignedDomains.Remove(this.SelectedAssignedDomain);
        }

        // On Request Source Change Sets URL to matching ID
        public void RequestChanged()
        {
            if (SelectedRequestSource.URL != null || SelectedRequestSource.URL != "")
            {
                URLString = SelectedRequestSource.URL;
            }
        }

        // Uses Checkbox on Space Bar Press
        public void SpaceCheckbox(string ListBox)
        {
            switch (ListBox)
            {
                case "AssignmentGroupType":
                    foreach (ServiceCatalogAssignmentGroupItemModel groupItem in assignmentGroupsList.Where(x => x.Id == SelectedAssignmentGroup.Id))
                    {
                        if (groupItem.IsSelected == true)
                        {
                            groupItem.IsSelected = false;
                        }
                        else
                        {
                            groupItem.IsSelected = true;
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}

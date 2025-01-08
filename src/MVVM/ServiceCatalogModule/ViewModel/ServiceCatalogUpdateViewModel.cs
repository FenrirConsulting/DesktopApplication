using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ServiceCatalogUpdateViewModel : BaseViewModel
    {
        #region Delegates
        //Access SQL Methods Class
        private readonly SQLMethods DBConn = new();
        private readonly SharePointMethods SPMethods = new();

        //Relay Commands for Buttons
        public RelayCommand UpdateItemCommand { get; set; }
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
        private  ObservableCollection<ServiceCatalogDomainTermModel> assignedDomains = new()
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
        private  ObservableCollection<ServiceCatalogDomainTermModel> availableDomains = new()
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

        // Binds text of Article String from Current Item
        private string statusString;
        public string StatusString
        {
            get { return statusString; }
            set { if (statusString != value) { statusString = value; OnPropertyChanged(); } }
        }

        // Binds text of Article String from Current Item
        private bool updateEnabled;
        public bool UpdateEnabled
        {
            get { return updateEnabled; }
            set { if (updateEnabled != value) { updateEnabled = value; OnPropertyChanged(); } }
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
        #endregion

        #region Methods
        public ServiceCatalogUpdateViewModel()
        {
            // Initialize Properties
            requestSourcesList = new();
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
            UpdateEnabled = true;
            SPMethods = new();
            AssignmentGroupsStringList = new();

            //Initialize Relay Commands
            RemoveListCommand = new RelayCommand(o => { RemoveList(); });
            AddListCommand = new RelayCommand(o => { AddList(); });
            UpdateItemCommand = new RelayCommand(o => { UpdateItem(); });
            SpaceCheckboxCommand = new RelayCommand((box) => { SpaceCheckbox(box.ToString()); });
            MSG.Messenger.Default.Register<String>(this, SentMessage);
        }
        #endregion

        #region Functions
        // Main On Load Function
        public void LoadData()
        {
            try
            {
                AutomationBool = false;
                requestSourcesList.Clear();
                StatusString = "";
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
                    requestEnvironmentList.Add(new ServiceCatalogEnvironmentItemModel { Id = Int32.Parse(row["Id"].ToString()), RequestEnvironment = row["EnvironmentName"].ToString() });
                }

                LoadRecord();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
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

        // Primary Load Record Function
        public void LoadRecord()
        {
            try
            {
                SelectedItem = ServiceCatalogMainViewModel.CurrentServiceItem;
                if (SelectedItem != null && SelectedItem != "")
                {
                    DataTable fetchTable = DBConn.GetSelectedRecord(ConfigurationProperties.ServiceCatalogTable, "ID", SelectedItem);
                    if (fetchTable.Rows.Count > 0)
                    {
                        availableDomains.Clear();
                        foreach (string s in ListOfDomains)
                        {
                            availableDomains.Add(new ServiceCatalogDomainTermModel { DomainTerm = s }); ;
                        }
                        assignedDomains.Clear();
                        CurrentItem = BuildItem(fetchTable);
                        if (CurrentItem.AutomationStatus == "Automated") { AutomationBool = true; } 
                        StatusString = "Updating Record #" + CurrentItem.ID;
                    }
                    else
                    {
                        StatusString = "Record not Found.";
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Update Item on DB. Also updates the item on SharePoint
        public async void UpdateItem()
        {
            try
            {
                if (CurrentItem != null )
                {
                    string joinedServices = ServiceString.Replace(Environment.NewLine, ";");
                    CurrentItem.Services = joinedServices;
                    CurrentItem.SubSystem = SubSystemString;
                    CurrentItem.Description = DescriptionString;
                    CurrentItem.RequestSource = SelectedRequestSource.RequestSource;

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
                    CurrentItem.Domain.Clear();
                    foreach (ServiceCatalogDomainTermModel term in assignedDomains)
                    {
                        CurrentItem.Domain.Add(term.DomainTerm);
                    }
                    CurrentItem.Article = ArticleString;
                    CurrentItem.ProvisioningTeam = ProvisioningString;
                    CurrentItem.URL = URLString;
                    CurrentItem.Environment = SelectedRequestEnvironment.RequestEnvironment;
                    if (AutomationBool == true) { CurrentItem.AutomationStatus = "Automated"; } else { CurrentItem.AutomationStatus = "Manual"; }
                    DBConn.ServiceCatalogItemUpdate(CurrentItem, ConfigurationProperties.ServiceCatalogTable);
                    MSG.Messenger.Default.Send("Reload Service Catalog Table");
                    StatusString = "Record #" + CurrentItem.ID + " Updated";
                    var data = await SPMethods.UpdateItem(CurrentItem);
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

        // Builds the Current Service Catalog Item from DB
        public ServiceCatalogItemModel BuildItem(DataTable passedTable)
        {
            ServiceCatalogItemModel TemporaryItem = new()
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

            try
            {
                TemporaryItem.ID = Convert.ToInt32(passedTable.Rows[0]["ID"]);
                TemporaryItem.Services = passedTable.Rows[0]["Services"].ToString();
                TemporaryItem.SubSystem = passedTable.Rows[0]["SubSystem"].ToString(); SubSystemString = TemporaryItem.SubSystem;
                string[] splitServices = TemporaryItem.Services.Split(";");
                string finalServices = "";
                foreach (var word in splitServices)
                {
                    finalServices += word + Environment.NewLine;
                }
                ServiceString = finalServices;
                TemporaryItem.Description = passedTable.Rows[0]["Description"].ToString(); DescriptionString = TemporaryItem.Description;
                TemporaryItem.RequestSource = passedTable.Rows[0]["RequestSource"].ToString(); RequestString = TemporaryItem.RequestSource;
                TemporaryItem.Environment = passedTable.Rows[0]["Environment"].ToString(); 

                if (requestSourcesList.Any(i => i.RequestSource == TemporaryItem.RequestSource)) 
                { 
                    SelectedRequestSource = requestSourcesList.FirstOrDefault(X => X.RequestSource == TemporaryItem.RequestSource);
                }
                else
                {
                    requestSourcesList.Add(new ServiceCatalogSourceItemModel { Id = TemporaryItem.ID, RequestSource = TemporaryItem.RequestSource, URL = TemporaryItem.URL });
                    SelectedRequestSource = requestSourcesList.FirstOrDefault(X => X.Id == TemporaryItem.ID);
                }

                if (requestEnvironmentList.Any(i => i.RequestEnvironment == TemporaryItem.Environment))
                {
                    SelectedRequestEnvironment = requestEnvironmentList.FirstOrDefault(X => X.RequestEnvironment == TemporaryItem.Environment);
                }
                else
                {
                    requestEnvironmentList.Add(new ServiceCatalogEnvironmentItemModel { Id = TemporaryItem.ID, RequestEnvironment = TemporaryItem.Environment});
                    SelectedRequestEnvironment = requestEnvironmentList.FirstOrDefault(X => X.Id == TemporaryItem.ID);
                }

                TemporaryItem.AssignmentGroup = passedTable.Rows[0]["AssignmentGroup"].ToString(); AssignmentString = TemporaryItem.AssignmentGroup;

                AssignmentGroupsStringList.Clear();
                AssignmentGroupsStringList = TemporaryItem.AssignmentGroup.Split(";").ToList();
                foreach (string s in AssignmentGroupsStringList)
                {
                    if (assignmentGroupsList.Any(x => x.GroupName == s)) 
                    { 
                        foreach (ServiceCatalogAssignmentGroupItemModel listModel in assignmentGroupsList)
                        {
                            if (listModel.GroupName == s)
                            {
                                listModel.IsSelected = true;
                            }
                        }
                    }
                    else
                    {
                        assignmentGroupsList.Add(new ServiceCatalogAssignmentGroupItemModel { Id = TemporaryItem.ID, GroupName = s, IsSelected = true});
                    }
                }
                ObservableCollection<ServiceCatalogAssignmentGroupItemModel>  tempList = new ObservableCollection<ServiceCatalogAssignmentGroupItemModel>(assignmentGroupsList.OrderByDescending(i => i.IsSelected == true));
                
                assignmentGroupsList.Clear();
                foreach (ServiceCatalogAssignmentGroupItemModel item in tempList)
                {
                    assignmentGroupsList.Add(item);
                }

                

                TemporaryItem.ExpediteProcess = passedTable.Rows[0]["ExpediteProcess"].ToString(); ExpediteString = TemporaryItem.ExpediteProcess;
                TemporaryItem.SLA = passedTable.Rows[0]["SLA"].ToString(); SLAString = TemporaryItem.SLA;
                TemporaryItem.ArcherAppID = passedTable.Rows[0]["ArcherAppID"].ToString(); ArcherString = TemporaryItem.ArcherAppID;
                TemporaryItem.ITPMAppID = passedTable.Rows[0]["ITPMAppID"].ToString(); ITPMString = TemporaryItem.ITPMAppID;
                TemporaryItem.Hints = passedTable.Rows[0]["Hints"].ToString(); HintsString = TemporaryItem.Hints;
                TemporaryItem.Tasks = passedTable.Rows[0]["Tasks"].ToString(); TasksString = TemporaryItem.Tasks;

                string splitDomains = passedTable.Rows[0]["Domain"].ToString();
                TemporaryItem.Domain = splitDomains.Split(";").ToList();
                foreach (string s in TemporaryItem.Domain)
                {
                    if (ListOfDomains.Contains(s))
                    {
                        assignedDomains.Add(new ServiceCatalogDomainTermModel { DomainTerm = s });
                        availableDomains.Remove(availableDomains.Where(i => i.DomainTerm == s).Single());
                    }
                }

                TemporaryItem.Article = passedTable.Rows[0]["Article"].ToString(); ArticleString = TemporaryItem.Article;
                TemporaryItem.ProvisioningTeam = passedTable.Rows[0]["ProvisioningTeam"].ToString(); ProvisioningString = TemporaryItem.ProvisioningTeam;
                TemporaryItem.URL = passedTable.Rows[0]["URL"].ToString(); URLString = TemporaryItem.URL;
                TemporaryItem.AutomationStatus = passedTable.Rows[0]["AutomationStatus"].ToString();



            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

            return TemporaryItem;
        }

        // Custom DaySpan to be returned by ComputeDaysDifference()
        public struct DaySpan
        {
            public int Days;
            public int Hours;
            public int Minutes;
            public int Seconds;
        }
        #endregion

        #region Messages
        // Listen For Record Select Message
        public void SentMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                switch (passedMessage)
                {

                    case "Select Service Catalog Update Tab":
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}

using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Centrify.DirectControl.API;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;
using SimpleImpersonation;
using IAMHeimdall.Resources.Commands;
using System.Threading;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolGetComputerRolesViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand GetComputersCommand { get; set; }
        public RelayCommand GetUserRolesCommand { get; set; }
        public RelayCommand ClearFormCommand { get; set; }
        public RelayCommand FilterChangeCommand { get; set; }
        public RelayCommand RecordSelectCommand { get; set; }
        public RelayCommand GroupQueryCommand { get; set; }
        #endregion

        #region Properties
        // Binds Computer Name Entry Box
        private string computerNamesEntryBox;
        public string ComputerNamesEntryBox
        {
            get { return computerNamesEntryBox; }
            set
            {
                if (computerNamesEntryBox != value)
                {
                    computerNamesEntryBox = value;
                    if (computerNamesEntryBox.Length > 5)
                    {
                        GetComputersEnabled = true;
                    }
                    else
                    {
                        GetComputersEnabled = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // Holds String to send to Group Query Lookup Box
        public static string SelectedGroupQuery = "";

        // Binds Computer Name Entry Box
        private string statusString;
        public string StatusString
        {
            get { return statusString; }
            set
            {
                if (statusString != value)
                {
                    statusString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Data for Display Table
        private DataTable computersdisplayTable;
        public DataTable ComputersDisplayTable
        {
            get { return computersdisplayTable; }
            set { computersdisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView computersDisplayView;
        public DataView ComputersDisplayView
        {
            get { return computersDisplayView; }
            set { computersDisplayView = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable computersUsersdisplayTable;
        public DataTable ComputersUsersDisplayTable
        {
            get { return computersUsersdisplayTable; }
            set { computersUsersdisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView computersUsersDisplayView;
        public DataView ComputersUsersDisplayView
        {
            get { return computersUsersDisplayView; }
            set { computersUsersDisplayView = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo usercellInfo;
        public DataGridCellInfo UserCellInfo
        {
            get { return usercellInfo; }
            set { usercellInfo = value; OnPropertyChanged(); }
        }

        // Holds Data for Filter Table
        private DataTable filteredTable;
        public DataTable FilteredTable
        {
            get { return filteredTable; }
            set { filteredTable = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> computerSearchTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> ComputerSearchTerms
        {
            get { return computerSearchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedComputerTerm = new();
        public ComboBoxListItem SelectedComputerTerm
        {
            get { return selectedComputerTerm; }
            set { selectedComputerTerm = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> typeSearchTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> TypeSearchTerms
        {
            get { return typeSearchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedTypeTerm = new();
        public ComboBoxListItem SelectedTypeTerm
        {
            get { return selectedTypeTerm; }
            set { selectedTypeTerm = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> environmentSearchTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> EnvironmentSearchTerms
        {
            get { return environmentSearchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedEnvironmentTerm = new();
        public ComboBoxListItem SelectedEnvironmentTerm
        {
            get { return selectedEnvironmentTerm; }
            set { selectedEnvironmentTerm = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> pciSearchTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> PCISearchTerms
        {
            get { return pciSearchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedPCITerm = new();
        public ComboBoxListItem SelectedPCITerm
        {
            get { return selectedPCITerm; }
            set { selectedPCITerm = value; OnPropertyChanged(); }
        }

        // Binds Data Buttons Enabled Bool
        private bool getComputersEnabled;
        public bool GetComputersEnabled
        {
            get { return getComputersEnabled; }
            set { if (getComputersEnabled != value) { getComputersEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Data Buttons Enabled Bool
        private bool getUsersEnabled;
        public bool GetUsersEnabled
        {
            get { return getUsersEnabled; }
            set { if (getUsersEnabled != value) { getUsersEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Data Buttons Enabled Bool
        private bool filtersEnabled;
        public bool FiltersEnabled
        {
            get { return filtersEnabled; }
            set { if (filtersEnabled != value) { filtersEnabled = value; OnPropertyChanged(); } }
        }

        // Bool to starting Loading Animation
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Bool to starting Loading Animation
        private bool isLoadBool2;
        public bool IsLoadBool2
        {
            get { return isLoadBool2; }
            set { if (isLoadBool2 != value) { isLoadBool2 = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public IAMToolGetComputerRolesViewModel()
        {
            // Initialize Relay Commands
            GetComputersCommand = new RelayCommand(o => { GetComputersButton(); });
            GetUserRolesCommand = new RelayCommand(o => { GetUsersButton(null); });
            ClearFormCommand = new RelayCommand(o => { ClearForm(); });
            FilterChangeCommand = new RelayCommand(o => { UserSearchFilter(); });
            RecordSelectCommand = new RelayCommand(o => { GetUserDoubleClick(); });
            GroupQueryCommand = new RelayCommand(o => { SelectGroupQuery(); });

            // Initialize Properties
            IsLoadBool = false;
            GetComputersEnabled = false;
            GetUsersEnabled = false;
            ComputersDisplayView = new();
            ComputersUsersDisplayView = new();
            FilteredTable = new();
            ComputersDisplayTable = new("Temporary Table");
            ComputersDisplayTable.Columns.Add("CDGComputername", typeof(String));
            ComputersDisplayTable.Columns.Add("CDGZone", typeof(String));
            ComputersDisplayTable.Columns.Add("CDGZoneMode", typeof(String));
            ComputersDisplayTable.Columns.Add("CDGADPath", typeof(String));
            ComputersUsersDisplayTable = new("Temporary Second Table");
            ComputersUsersDisplayTable.Columns.Add("CUDGComputername", typeof(String));
            ComputersUsersDisplayTable.Columns.Add("CUDGRole", typeof(String));
            ComputersUsersDisplayTable.Columns.Add("CUDGAssignment", typeof(String));
            computerSearchTerms.Add(new ComboBoxListItem { BoxItem = "All" });
            foreach (string s in ConfigurationProperties.GetComputerRolesTypes)
            {
                typeSearchTerms.Add(new ComboBoxListItem { BoxItem = s });
            }
            foreach (string s in ConfigurationProperties.GetComputerRolesEnvironments)
            {
                environmentSearchTerms.Add(new ComboBoxListItem { BoxItem = s });
            }
            foreach (string s in ConfigurationProperties.GetComputerRolesPCI)
            {
                pciSearchTerms.Add(new ComboBoxListItem { BoxItem = s });
            }
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            try
            {
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Reset Form
        public void ClearForm()
        {
            ComputerNamesEntryBox = "";
            StatusString = "";
            GetUsersEnabled = false;
            GetComputersEnabled = false;
            FiltersEnabled = false;
            ComputersUsersDisplayTable.Clear();
            ComputersDisplayTable.Clear();
            FilteredTable.Clear();
            ComputersDisplayView = ComputersDisplayTable.DefaultView;
            ComputersUsersDisplayView = ComputersUsersDisplayTable.DefaultView;
        }

        // Get Computers Button Function
        public async void GetComputersButton()
        {
            try
            {
                ComputerReturnValues returnInstance = new();
                FiltersEnabled = false;
                ComputersUsersDisplayTable.Clear();
                ComputersDisplayTable.Clear();
                FilteredTable.Clear();
                ComputersUsersDisplayView = ComputersUsersDisplayTable.DefaultView;
                ComputersDisplayView = ComputersDisplayTable.DefaultView;
                IsLoadBool = true;
                StatusString = "Searching...";
                computerSearchTerms.Clear();
                computerSearchTerms.Add(new ComboBoxListItem { BoxItem = "All" });
                await Task.Run(() => {
                    try
                    {
                        returnInstance = GetComputers();
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                        throw;
                    }
                });
                foreach (ComboBoxListItem item in returnInstance.searchTerms )
                {
                    computerSearchTerms.Add(new ComboBoxListItem { BoxItem = item.BoxItem });
                }
                ComputersDisplayTable = returnInstance.table;
                SelectedComputerTerm = ComputerSearchTerms.ElementAt(0);
                if (ComputersDisplayTable.Rows.Count > 0)
                {
                    StatusString = "Found " + ComputersDisplayTable.Rows.Count.ToString() + " Computers in Centrify";
                    GetUsersEnabled = true;
                }
                else
                {
                    StatusString = "No Results Found.";
                }
                IsLoadBool = false;
                ComputersDisplayView = ComputersDisplayTable.DefaultView;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Get Computers Function
        public ComputerReturnValues GetComputers()
        {
            
            ComputerReturnValues returnInstance = new();
            returnInstance.table = new("Temporary Table");
            returnInstance.table.Columns.Add("CDGComputername", typeof(String));
            returnInstance.table.Columns.Add("CDGZone", typeof(String));
            returnInstance.table.Columns.Add("CDGZoneMode", typeof(String));
            returnInstance.table.Columns.Add("CDGADPath", typeof(String));
            returnInstance.searchTerms = new();

            string computerNamesText = ComputerNamesEntryBox;
            string[] inputs = computerNamesText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (var i = 0; i < inputs.Length; i++)
            {
                // If there is a / in group string strips out all characters up to and including it.
                if (inputs[i].ToLower().Contains('\\')) { string str = inputs[i][(inputs[i].IndexOf('\\') + 1)..]; inputs[i] = str; }
            }
            var input = inputs.Distinct();
            var rebuild = string.Join(Environment.NewLine, input);
            ComputerNamesEntryBox = rebuild;
            ICims cims = new Cims();
            IHierarchicalZoneComputer computer ;

            if (input.Any())
            {
                foreach (string c in input)
                {
                    try
                    {
                        if (c != null && c != "")
                        {
                            string dn = ConfigurationProperties.LDAPPath + "CN=" + c + "," + ConfigurationProperties.CfyComputerOU;
                            computer = cims.GetComputerByPath(dn) as IHierarchicalZoneComputer;
                            if (computer != null)
                            {
                                DataRow NewRow;
                                NewRow = returnInstance.table.NewRow();
                                NewRow["CDGComputername"] = computer.Name;
                                NewRow["CDGZone"] = computer.Zone.ToString();
                                NewRow["CDGZoneMode"] = computer.ZoneMode.ToString();
                                NewRow["CDGADPath"] = computer.ADsPath;
                                returnInstance.table.Rows.Add(NewRow);
                                returnInstance.searchTerms.Add(new ComboBoxListItem { BoxItem = computer.Name });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                }
            }
            return returnInstance;
        }

        // Get Users Button Function
        public async void GetUsersButton(string computername)
        {
            try
            {
                FiltersEnabled = true;
                IsLoadBool2 = true;
                ComputersUsersDisplayTable.Clear();
                FilteredTable.Clear();
                ComputersUsersDisplayView = ComputersUsersDisplayTable.DefaultView;
                StatusString = "Searching...";
                await Task.Run(() => {
                    try
                    {
                        Thread.CurrentPrincipal = new System.Security.Principal.WindowsPrincipal(WindowsIdentity.GetCurrent());
                        ComputersUsersDisplayTable = GetUsers(computername);
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                        throw;
                    }
                });
                ComputersUsersDisplayView = ComputersUsersDisplayTable.DefaultView;
                if (ComputersUsersDisplayTable.Rows.Count > 0)
                {
                    StatusString = "Found " + ComputersUsersDisplayTable.Rows.Count.ToString() + " Computer and User Roles in Centrify";
                }
                else
                {
                    StatusString = "No Results Found.";
                }
                IsLoadBool2 = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Get User Double Click Function
        public void GetUserDoubleClick()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["CDGComputername"].ToString();
                        GetUsersButton(record);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Send Selected Group to Group Query Tab
        public void SelectGroupQuery ()
        {
            try
            {
                DataRowView drv = (DataRowView)UserCellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["CUDGAssignment"].ToString();
                        SelectedGroupQuery = record;
                        MSG.Messenger.Default.Send("Select IAM Tool Tab 2");
                        MSG.Messenger.Default.Send("IAM Tool Group Query Selected");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Get Users Function
        public DataTable GetUsers(String computerName)
        {
            DataTable TempTable = new("Temporary Second Table");
            TempTable.Columns.Add("CUDGComputername", typeof(String));
            TempTable.Columns.Add("CUDGRole", typeof(String));
            TempTable.Columns.Add("CUDGAssignment", typeof(String));
            try 
            {
                ICims cims = new Cims();
                IHierarchicalZone objZone = cims.GetZone(ConfigurationProperties.CfyRootZone) as IHierarchicalZone;
                List<string> input = new();

                if (string.IsNullOrEmpty(computerName))
                {
                    foreach (DataRow row in ComputersDisplayTable.Rows)
                    {
                        string cellValue = row["CDGComputername"].ToString();
                        try
                        {
                            if (cellValue != null && cellValue != "")
                            {
                                input.Add(cellValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionOutput.Output(ex.ToString());
                        }
                    }
                }
                else
                {
                    input.Add(computerName);
                }

                foreach (string computer in input)
                {
                    if (computer != null && computer != "")
                    {
                        List<string> groups = GetUserGroups(computer);
                        foreach (string g in groups)
                        {
                            IComputerRole role = objZone.GetComputerRole(g);

                            if (role != null)
                            {
                                IRoleAssignments ras = role.GetRoleAssignments();
                                foreach (IRoleAssignment ra in ras)
                                {
                                    if (ra.TrusteeDn != null)
                                    {
                                        string dn = ra.TrusteeDn.ToString();
                                        string[] sn1 = dn.Split(',');
                                        string[] sn2 = sn1[0].Split('=');
                                        string sn = sn2[1];
                                        DataRow NewRow;
                                        NewRow = TempTable.NewRow();
                                        NewRow["CUDGComputername"] = computer;
                                        NewRow["CUDGRole"] = g;
                                        NewRow["CUDGAssignment"] = sn;
                                        TempTable.Rows.Add(NewRow);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return TempTable;
        }

        // Fetch Group List for User Role Table
        public static List<string> GetUserGroups(string user)
        {
            var userGroupStrings = new List<string>();
            try 
            {
                PrincipalContext domainctx = new(ContextType.Domain, "corp");

                using var userPrincipal = Principal.FindByIdentity(domainctx, user);
                using var userGroups = userPrincipal.GetGroups();
                using var enumerator = userGroups.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    try
                    {
                        userGroupStrings.Add(enumerator.Current.SamAccountName);
                    }
                    catch
                    {
                        continue;
                    }
                }
                return userGroupStrings;
            }
              catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return userGroupStrings;
            }
        }

        // Set Filters on ComboBox Selections
        public void UserSearchFilter()
        {
            try 
            {
                if (ComputersUsersDisplayTable.Rows.Count > 0)
                {
                    string builtFilter = "";
                    string computerFilter = "CUDGComputername Like '*' AND ";
                    string typeFilter = "CUDGRole Like '*' AND ";
                    string environmentFilter = "CUDGRole Like '*' AND ";
                    string pciFilter = "CUDGRole Like '*'";

                    if (SelectedComputerTerm.BoxItem != "All") { computerFilter = "CUDGComputername Like '%" + SelectedComputerTerm.BoxItem.ToString() + "%' AND "; }
                    if (SelectedTypeTerm.BoxItem != "All") { typeFilter = "CUDGRole Like '%" + SelectedTypeTerm.BoxItem.ToString() + "%' AND "; }
                    if (SelectedEnvironmentTerm.BoxItem != "All") { environmentFilter = "CUDGRole Like '%" + SelectedEnvironmentTerm.BoxItem.ToString() + "%' AND "; }
                    if (SelectedPCITerm.BoxItem != "All") { pciFilter = "CUDGRole Like '%" + SelectedPCITerm.BoxItem.ToString() + "%' "; }

                    builtFilter = computerFilter + typeFilter + environmentFilter + pciFilter;
                    string sort = "CUDGComputername";
                    DataView tempView = new(ComputersUsersDisplayTable, builtFilter, sort, DataViewRowState.CurrentRows);
                    DataTable tempTable = tempView.ToTable();
                    FilteredTable.Clear();
                    FilteredTable = tempView.ToTable();
                    if (FilteredTable.Rows.Count == 0) { StatusString = "No Filtered Results"; }
                    else { StatusString = FilteredTable.Rows.Count.ToString() + " Filtered Results"; ComputersUsersDisplayView = FilteredTable.DefaultView; }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Object For Passing Computer DataTable and Computer SearchTerms 
        public struct ComputerReturnValues
        {
            public DataTable table;
            public ObservableCollection<ComboBoxListItem> searchTerms;
        }
        #endregion
    }
}

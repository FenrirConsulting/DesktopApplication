using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class GroupsAddRecordViewModel : BaseViewModel
    {
        #region Delegates
        readonly SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand AddGroupRecordCommand { get; set; }
        #endregion

        #region Properties
        // Get Current User Information
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Logged In Label String
        private string _LoggedInAs;
        public string LoggedInAs
        {
            get { return _LoggedInAs; }
            set { if (_LoggedInAs != value) { _LoggedInAs = value; OnPropertyChanged(); } }
        }

        // Binded GID String Field
        private string gidstring;
        public string GIDString
        {
            get { return gidstring; }
            set { if (gidstring != value) { gidstring = value; OnPropertyChanged(); } }
        }

        // Binded Group Name String Field
        private string groupNameString;
        public string GroupNameString
        {
            get { return groupNameString; }
            set { if (groupNameString != value) { groupNameString = value; OnPropertyChanged(); } }
        }

        // Basis for Search Terms Collection of Groups
        public IEnumerable<ComboBoxListItem> SearchTermsGroups
        {
            get { return searchTermsGroups; }
        }

        // Search Terms for ComboBox of Groups
        private readonly ObservableCollection<ComboBoxListItem> searchTermsGroups = new()
        {
            new ComboBoxListItem { BoxItem = "PBM" },
            new ComboBoxListItem { BoxItem = "Retail" },
            new ComboBoxListItem { BoxItem = "Enterprise" },
        };

        // Selected Term for ComboBox of Groups
        private ComboBoxListItem selectedTermGroup = new();
        public ComboBoxListItem SelectedTermGroup
        {
            get { return selectedTermGroup; }
            set { selectedTermGroup = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public GroupsAddRecordViewModel()
        {
            AddGroupRecordCommand = new RelayCommand(o => { AddGroupRecord(); });
            GIDString = "";
            GroupNameString = "";
        }
        #endregion

        #region Functions
        // View Open Data Load
        public void OnLoad()
        {
            try
            {
                // Auto Increment new GID Record
                List<String> groupRecordsList = DBConn.GetColumnList("dbo.IAMHGroups", "iamhg_gid");
                List<int> groupRecordsIntList = groupRecordsList
                     .Select(s => Int32.TryParse(s, out int n) ? n : (int?)null)
                     .Where(n => n.HasValue)
                     .Select(n => n.Value)
                     .ToList();
                int[] listArray = groupRecordsIntList.ToArray();
                int lowestAvailableUser = SmallestMissingNumber.FindSmallestMissingNumber(listArray, 100000, 299999);
                GIDString = lowestAvailableUser.ToString();

                // Established and displays the user creating the record
                CurrentUserInfo = new UserInfo();
                CurrentUserInfo = App.GlobalUserInfo;
                LoggedInAs = "Creating Record As : ";
                if (CurrentUserInfo.DisplayName != null)
                {
                    LoggedInAs = CurrentUserInfo.DisplayName + Environment.NewLine + "Creating Record on : " + DateTime.Now.ToShortDateString();
                }

                // Set LOB Dropdown Menu
                SelectedTermGroup = SearchTermsGroups.ElementAt(0);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Add Group Confirmation
        public void AddGroupRecord()
        {
            if (MessageBox.Show("Are you certain you want to create this record?", "Create Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                CreateGroupRecord();
            }
            else
            {
                //Do Nothing
            }
        }

        // Method creating the record in SQL Server DB
        public async void CreateGroupRecord()
        {
            try
            {
                // Check new record against GID record list, if no match creates the record.
                bool foundMatch = false;
                List<String> groupRecordsList = DBConn.GetColumnList("dbo.IAMHGroups", "iamhg_gid");
                foreach (string s in groupRecordsList)
                {
                    if (s == GIDString)
                    {
                        foundMatch = true;
                        MessageBox.Show("GID Record Exists, please select another GID#", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (GroupNameString != "")
                {
                    List<String> GroupCheck = DBConn.GetColumnList("dbo.IAMHGroups", "iamhg_group");
                    foreach (string s in GroupCheck)
                    {
                        if (s.ToString() == GroupNameString)
                        {
                            foundMatch = true;
                            MessageBox.Show("Group Record Exists, please select another Group", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                // Creates Record, Updates Local Data Table, & Clears Text Boxes
                if (foundMatch == false)
                {
                    string updateGID = GIDString;
                    string updateGroup = GroupNameString;
                    string updateGroupLob = SelectedTermGroup.BoxItem.ToString();
                    string updateHistory = CurrentUserInfo.DisplayName + " | Creating Record : " + updateGID + " on " + DateTime.Now.ToShortDateString();
                    DBConn.AddGroupsRecord(updateGID, updateGroup, updateGroupLob, updateHistory);
                    // After succesful creation in Server updates the local Table instance
                    
                    DataTable selectNewRow = DBConn.GetSelectedRecord(ConfigurationProperties.GroupsTable, "iamhg_gid", updateGID);
                    string updateGroupID = selectNewRow.Rows[0][0].ToString();
                    
                    if (MainViewModel.GroupsTable.Rows.Count == 0) { MainViewModel.GroupsTable = await DBConn.GetTableAsync(ConfigurationProperties.GroupsTable); }
                    else
                    {
                        DataRow row;
                        row = MainViewModel.GroupsTable.NewRow();
                        row["iamhg_id"] = updateGroupID;
                        row["iamhg_gid"] = updateGID;
                        row["iamhg_group"] = updateGroup;
                        row["iamhg_lob"] = updateGroupLob;
                        row["iamhg_history"] = updateHistory;
                        MainViewModel.GroupsTable.Rows.Add(row);
                    }

                    GIDString = "";
                    GroupNameString = "";
                    MSG.Messenger.Default.Send("Groups Reload");
                    OnLoad();
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

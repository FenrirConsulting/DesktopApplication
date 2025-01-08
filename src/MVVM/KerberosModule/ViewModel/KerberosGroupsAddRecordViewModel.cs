using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using MSG = GalaSoft.MvvmLight.Messaging;
using System.Windows;
using IAMHeimdall.Resources.Commands;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class KerberosGroupsAddRecordViewModel : BaseViewModel
    {
        #region Delegates
        readonly SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand AddGroupRecordCommand { get; set; }
        public RelayCommand RemoveListCommand { get; set; }
        public RelayCommand AddListCommand { get; set; }
        #endregion

        #region Properties
        // Logged In Label String
        private string _LoggedInAs;
        public string LoggedInAs
        {
            get { return _LoggedInAs; }
            set { if (_LoggedInAs != value) { _LoggedInAs = value; OnPropertyChanged(); } }
        }

        // Lists for Holding Server List Information
        private readonly List<String> ServersUpdatedList;
        private List<String> ServersAvailableList;

        // Selected Term for Available Item
        private ServerName selectedAvailableTerm = new();
        public ServerName SelectedAvailableTerm
        {
            get { return selectedAvailableTerm; }
            set { selectedAvailableTerm = value; OnPropertyChanged(); }
        }

        // Basis for SAvailable Terms Collection
        public IEnumerable<ServerName> AvailableServerTerms
        {
            get { return availableServerTerms; }
        }

        // Basis for Available Terms Collection
        private readonly ObservableCollection<ServerName> availableServerTerms = new()
        {

        };

        // Selected Term for Assigned Item
        private ServerName selectedAssignedTerm = new();
        public ServerName SelectedAssignedTerm
        {
            get { return selectedAssignedTerm; }
            set { selectedAssignedTerm = value; OnPropertyChanged(); }
        }

        // Basis for Assigned Terms Collection
        public IEnumerable<ServerName> AssignedServerTerms
        {
            get { return assignedServerTerms; }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ServerName> assignedServerTerms = new()
        {

        };

        // User Information
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Group Properties Model Class
        public KerberosGroupModel GroupModelInstance { get; set; }
        #endregion

        #region Methods
        public KerberosGroupsAddRecordViewModel()
        {
            AddGroupRecordCommand = new RelayCommand(o => { AddGroupRecord(); });
            RemoveListCommand = new RelayCommand(o => { RemoveList(); });
            AddListCommand = new RelayCommand(o => { AddList(); });
            ServersUpdatedList = new();

            GroupModelInstance = new KerberosGroupModel
            {
                GIDRef = "",
                GroupRef = "",
                CommentsRef = "",
                HistoryRef = "",
            };
        }
        #endregion

        #region Functions
        // On Load Event
        public void LoadData()
        {
            try
            {
                assignedServerTerms.Clear();
                GroupModelInstance.CommentsRef = "";
                // Auto Increment new GID Record
                List<double> groupRecordsList = DBConn.GetColumnListDouble("dbo.IAMHKGroups", "iamhkg_gid");
                List<int> groupRecordsIntList = groupRecordsList.ConvertAll(Convert.ToInt32);
                int[] listArray = groupRecordsIntList.ToArray();
                int lowestAvailableUser = SmallestMissingNumber.FindSmallestMissingNumber(listArray, 100000, 199999);
                GroupModelInstance.GIDRef = lowestAvailableUser.ToString();

                ServersAvailableList = DBConn.GetColumnList(ConfigurationProperties.KerberosServersTable, "iamhks_servername");
                availableServerTerms.Clear();
                foreach (var word in ServersAvailableList)
                {
                    availableServerTerms.Add(new ServerName { ServerTerm = word }); ;
                }

                // Established and displays the user creating the record
                CurrentUserInfo = new UserInfo();
                CurrentUserInfo = App.GlobalUserInfo;
                LoggedInAs = "Creating Record As : ";
                if (CurrentUserInfo.DisplayName != null)
                {
                    LoggedInAs = CurrentUserInfo.DisplayName + Environment.NewLine + "Creating Record on : " + DateTime.Now.ToShortDateString();
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
            assignedServerTerms.Add(this.SelectedAvailableTerm);
            availableServerTerms.Remove(this.SelectedAvailableTerm);
        }

        // Remove Record Arrow Button
        public void RemoveList()
        {
            availableServerTerms.Add(this.SelectedAssignedTerm);
            assignedServerTerms.Remove(this.SelectedAssignedTerm);
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
                List<double> groupRecordsList = DBConn.GetColumnListDouble("dbo.IAMHKGroups", "iamhkg_gid");
                List<string> recordsList = new();

                foreach (Double d in groupRecordsList)
                {
                    recordsList.Add(d.ToString());
                }
                foreach (string s in recordsList)
                {
                    if (s == GroupModelInstance.GIDRef)
                    {
                        foundMatch = true;
                        MessageBox.Show("GID Record Exists, please select another GID#", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                if (GroupModelInstance.GroupRef != "")
                {
                    List<String> GroupCheck = DBConn.GetColumnList("dbo.IAMHKGroups", "iamhkg_group");
                    foreach (string s in GroupCheck)
                    {
                        if (s.ToString() == GroupModelInstance.GroupRef)
                        {
                            foundMatch = true;
                            MessageBox.Show("Group Record Exists, please select another Group", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                // Creates Record, Updates Local Data Table, & Clears Text Boxes
                if (foundMatch == false)
                {
                    string updateGID = GroupModelInstance.GIDRef;
                    string updateGroup = GroupModelInstance.GroupRef;
                    string updateComments = GroupModelInstance.CommentsRef;
                    updateComments = updateComments.Replace(Environment.NewLine, ";");
                    string updateHistory = CurrentUserInfo.DisplayName + " | Creating Record : " + updateGID + " on " + DateTime.Now.ToShortDateString();
                    foreach (var server in assignedServerTerms)
                    {
                        ServersUpdatedList.Add(server.ServerTerm);
                    }
                    string updateServers = "";
                    updateServers = String.Join(",", ServersUpdatedList.ToArray());
                    DBConn.AddKerberosGroupRecord(updateGID, updateGroup, updateComments, updateServers, updateHistory);

                    // After succesful creation in Server updates the local Table instance
                    DataTable selectNewRow = DBConn.GetSelectedRecord(ConfigurationProperties.KerberosGroupsTable, "iamhkg_gid", updateGID);
                    string updateGroupID = selectNewRow.Rows[0][0].ToString();

                    if (MainViewModel.KerberosGroupsTable.Rows.Count == 0) { MainViewModel.KerberosGroupsTable = await DBConn.GetTableAsync(ConfigurationProperties.KerberosGroupsTable); }
                    else
                    {
                        DataRow row;
                        row = MainViewModel.KerberosGroupsTable.NewRow();
                        row["iamhkg_id"] = updateGroupID;
                        row["iamhkg_gid"] = updateGID;
                        row["iamhkg_group"] = updateGroup;
                        row["iamhkg_comments"] = updateComments;
                        row["iamhkg_servers"] = updateServers;
                        row["iamhkg_history"] = updateHistory;
                        MainViewModel.KerberosGroupsTable.Rows.Add(row);
                    }

                    GroupModelInstance.GIDRef = "";
                    GroupModelInstance.GroupRef = "";
                    MSG.Messenger.Default.Send("Kerberos Groups Table Reload");
                    LoadData();
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

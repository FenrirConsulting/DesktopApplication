using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using MSG = GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class GroupsUpdateRecordViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand DeleteRecordCommand { get; set; }
        public RelayCommand UpdateRecordCommand { get; set; }
        #endregion

        #region Properties
        // Enable Write Permissions on IAM Write Authenticated
        private bool unixPermissionCheck;
        public bool UnixPermissionCheck
        {
            get { return unixPermissionCheck; }
            set { if (unixPermissionCheck != value) { unixPermissionCheck = value; OnPropertyChanged(); } }
        }

        // Check Group String Before Update
        private string groupCheck;
        public string GroupCheck
        {
            get { return groupCheck; }
            set { if (groupCheck != value) { groupCheck = value; OnPropertyChanged(); } }
        }

        // Check LOB String Before Update
        private string lobCheck;
        public string LOBCheck
        {
            get { return lobCheck; }
            set { if (lobCheck != value) { lobCheck = value; OnPropertyChanged(); } }
        }

        // Sets the Chosen Record from UID View
        private readonly string chosenRecord;

        // Binds GID Label String
        private string gidLabelString;
        public string GidLabelString
        {
            get { return gidLabelString; }
            set { if (gidLabelString != value) { gidLabelString = value; OnPropertyChanged(); } }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> searchTerms = new()
        {
            new ComboBoxListItem { BoxItem = "Enterprise" },
            new ComboBoxListItem { BoxItem = "PBM" },
            new ComboBoxListItem { BoxItem = "Retail" },
            new ComboBoxListItem { BoxItem = "Other" }
        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> SearchTerms
        {
            get { return searchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedTerm = new();
        public ComboBoxListItem SelectedTerm
        {
            get { return selectedTerm; }
            set { selectedTerm = value; OnPropertyChanged(); }
        }

        // Get Current User Information
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Binded GID String Field
        private string idstring;
        public string IDString
        {
            get { return idstring; }
            set { if (idstring != value) { idstring = value; OnPropertyChanged(); } }
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

        // Binded History String Field
        private string historyString;
        public string HistoryString
        {
            get { return historyString; }
            set { if (historyString != value) { historyString = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods

        public GroupsUpdateRecordViewModel()
        {
            // Initialize Properties
            chosenRecord = GroupsTableViewModel.ChosenRecord;
            GroupNameString = "";
            HistoryString = "";
            UnixPermissionCheck = MainViewModel.UnixTablePermissionGlobal;

            //Initialize Relay Commands
            UpdateRecordCommand = new RelayCommand(o => { UpdateRecord(); });
            DeleteRecordCommand = new RelayCommand(o => { DeleteRecord(); });
        }
        #endregion

        #region Functions
        // On Load Event
        public void LoadData()
        {
            // Load in Selected Record information into Text Boxes with formatting
            DataTable foundRecords = DBConn.GetSelectedRecord(ConfigurationProperties.GroupsTable, "iamhg_id", chosenRecord);
            if (foundRecords != null && foundRecords.Rows.Count > 0)
            {
                DataRow selectedRecord = foundRecords.Rows[0];
                string checkEmpty = selectedRecord["iamhg_id"].ToString();

                if (selectedRecord != null && checkEmpty != "")
                {
                    GroupNameString = selectedRecord["iamhg_group"].ToString();  
                    groupCheck = GroupNameString;
                    GIDString = selectedRecord["iamhg_gid"].ToString();
                    LOBCheck = selectedRecord["iamhg_lob"].ToString(); 
                    IDString = selectedRecord["iamhg_id"].ToString();
                    HistoryString = selectedRecord["iamhg_history"].ToString();
                    GidLabelString = GIDString;
                }
                int chosenTerm = LOBCheck switch
                {
                    "Enterprise" => 0,
                    "PBM" => 1,
                    "Retail" => 2,
                    "Other" => 3,
                    _ => 3,
                };
                SelectedTerm = SearchTerms.ElementAt(chosenTerm);
            }
            else
            {
                GroupNameString = "";
                GIDString = "";
                HistoryString = "";
                SelectedTerm = SearchTerms.ElementAt(0);
            }
        }

        // Build SQL Update
        private void UpdateRecord()
        {
            if (MessageBox.Show("Are you certain you want to update this record?", "Update Record??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                try
                {
                    CurrentUserInfo = new UserInfo();
                    CurrentUserInfo = App.GlobalUserInfo;

                    string updateGroup = GroupNameString;
                    string updateGID = GIDString;
                    string updateLOB = SelectedTerm.BoxItem.ToString();
                    string updateHistory = "";
                    string finalHistory = "";
                    // Checks each TextBox against check strings for changes. Logs each change on new line.
                    if (updateGroup != GroupCheck)
                    {
                        updateHistory = updateHistory + "Group Changed From : " + GroupCheck + Environment.NewLine + " To : " + updateGroup + Environment.NewLine;
                    }

                    if (updateLOB != LOBCheck)
                    {
                        updateHistory = updateHistory + "LOB Changed From : " + LOBCheck + Environment.NewLine + " To : " + updateLOB + Environment.NewLine;
                    }

                    if (updateHistory != "")
                    {
                        finalHistory = "Changes Made by " + CurrentUserInfo.DisplayName + " on " + DateTime.Now.ToShortDateString() + " : " + Environment.NewLine + Environment.NewLine + updateHistory + Environment.NewLine + Environment.NewLine;
                    }
                    string passedHistory = finalHistory + HistoryString;
                    DBConn.UpdateTableRecord(ConfigurationProperties.GroupsTable, IDString, "iamhg_id", "iamhg_group", updateGroup);
                    DBConn.UpdateTableRecord(ConfigurationProperties.GroupsTable, IDString, "iamhg_id", "iamhg_gid", updateGID);
                    DBConn.UpdateTableRecord(ConfigurationProperties.GroupsTable, IDString, "iamhg_id", "iamhg_lob", updateLOB);
                    DBConn.UpdateTableRecord(ConfigurationProperties.GroupsTable, IDString, "iamhg_id", "iamhg_history", passedHistory);
                    foreach (DataRow dr in MainViewModel.GroupsTable.Rows)
                    {
                        if (dr["iamhg_id"].ToString() == IDString)
                        {
                            dr["iamhg_group"] = updateGroup;
                            dr["iamhg_gid"] = updateGID;
                            dr["iamhg_lob"] = updateLOB;
                            dr["iamhg_history"] = passedHistory;
                        }
                    }
                    MSG.Messenger.Default.Send("Groups Reload");
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            }
            else
            {
                //Do Nothing
            }
        }

        // Delete SQL Record
        public void DeleteRecord()
        {
            if (MessageBox.Show("Are you certain you want to delete this record?", "Delete Record??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                try
                {
                    string updateRef = chosenRecord;
                    DBConn.DeleteMatchingRecords(ConfigurationProperties.GroupsTable, "iamhg_id", IDString);
                    List<DataRow> rowsToDelete = new();
                    foreach (DataRow dr in MainViewModel.GroupsTable.Rows)
                    {
                        if (dr["iamhg_id"].ToString() == chosenRecord)
                        {
                            rowsToDelete.Add(dr);
                        }
                    }
                    foreach (DataRow dr in rowsToDelete)
                    {
                        MainViewModel.GroupsTable.Rows.Remove(dr);
                    }
                    MainViewModel.GroupsTable.AcceptChanges();
                    MSG.Messenger.Default.Send("Groups Reload");
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            }
            else
            {
                //Do Nothing
            }
        }
        #endregion
    }
}

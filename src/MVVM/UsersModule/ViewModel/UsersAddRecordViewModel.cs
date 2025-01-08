using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System.Windows;
using System.Collections.ObjectModel;
using MSG = GalaSoft.MvvmLight.Messaging;
using IAMHeimdall.Resources.Commands;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class UsersAddRecordViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand AddRecord { get; set; }
        public RelayCommand CloseButtonCommand { get; set; }
        #endregion

        #region Properties
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // UID Properties Model Class
        public UsersModel UsersModelInstance { get; set; }

        // For trackiing duplicate UID Record Creation
        public List<string> recordsList = new();

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> searchTerms = new()
        {
            new ComboBoxListItem { BoxItem = "PBM" },
            new ComboBoxListItem { BoxItem = "Retail" },
            new ComboBoxListItem { BoxItem = "Company" },
            new ComboBoxListItem { BoxItem = "Other" },
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

        // Logged In Label String
        private string _LoggedInAs;
        public string LoggedInAs
        {
            get { return _LoggedInAs; }
            set { if (_LoggedInAs != value) { _LoggedInAs = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public UsersAddRecordViewModel()
        {
            //Initialize UID Model Class
            UsersModelInstance = new UsersModel
            {
                UIDRef = "",
                LogRef = "",
                TypeRef = "",
                AdidRef = "",
                EmpRef = "",
                FirstRef = "",
                LastRef = "",
                CommentsRef = "",
                HistoryRef = ""
            };

            // Initialize Relay Commands
            AddRecord = new RelayCommand(o => { AddRecordMethod(); });
            CloseButtonCommand = new RelayCommand(o => { ReloadWindow(); });
        }
        #endregion

        #region Functions
        // On Load Event creating Records List of existing UID Records
        public void LoadData()
        {
            try
            {
                UsersModelInstance.LogRef = "";
                UsersModelInstance.EmpRef = "";
                UsersModelInstance.AdidRef = "";
                UsersModelInstance.FirstRef = "";
                UsersModelInstance.LastRef = "";
                UsersModelInstance.CommentsRef = "";
                recordsList.Clear();

                // Auto Increment new UID Record
                List<double> userRecordsList = DBConn.GetColumnListDouble(ConfigurationProperties.UsersTable, "iamhu_uid");
                List<int> userRecordsIntList = userRecordsList.ConvertAll(Convert.ToInt32);
                int[] listArray = userRecordsIntList.ToArray();
                int lowestAvailableUser = SmallestMissingNumber.FindSmallestMissingNumber(listArray, 200000, 299999);
                UsersModelInstance.UIDRef = lowestAvailableUser.ToString();

                // Established and displays the user creating the record
                CurrentUserInfo = new UserInfo();
                CurrentUserInfo = App.GlobalUserInfo;
                LoggedInAs = "Creating Record As : ";

                if (CurrentUserInfo.DisplayName != null)
                {
                    LoggedInAs = CurrentUserInfo.DisplayName + Environment.NewLine + "Creating Record on : " + DateTime.Now.ToShortDateString();
                }
                SelectedTerm = SearchTerms.ElementAt(0);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Add Record Command Confirmation
        private void AddRecordMethod()
        {
            if (MessageBox.Show("Are you certain you want to create this record?", "Create Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                CreateRecord();
            }
            else
            {
                //Do Nothing
            }
        }

        // Reload Window and Main Table
        public void ReloadWindow()
        {
            MSG.Messenger.Default.Send("Users Table Reload");
            LoadData();
        }

        // Method creating the record in SQL Server DB
        public void CreateRecord()
        {
            try
            {
                // Check new record against UID record list, if no match creates the record.
                bool foundMatch = false;

                List<double> collectedList = DBConn.GetColumnListDouble(ConfigurationProperties.UsersTable, "iamhu_uid");

                foreach (Double d in collectedList)
                {
                    recordsList.Add(d.ToString());
                }

                foreach (string item in recordsList)
                {
                    if (item == UsersModelInstance.UIDRef)
                    {
                        foundMatch = true;
                        MessageBox.Show("UID Record Exists, please select another UID#", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Creates Record, Updates Local Data Table, & Clears Text Boxes
                if (foundMatch == false)
                {
                    string updateRef = UsersModelInstance.UIDRef;
                    string updateLogID = UsersModelInstance.LogRef;
                    string updateEmpID = UsersModelInstance.UIDRef;
                    string updateAdID = UsersModelInstance.AdidRef;
                    UsersModelInstance.TypeRef = SelectedTerm.BoxItem.ToString();
                    string updateType = UsersModelInstance.TypeRef;
                    string updateFirst = UsersModelInstance.FirstRef;
                    string updateLast = UsersModelInstance.LastRef;
                    string buildTickets = UsersModelInstance.CommentsRef;
                    buildTickets = buildTickets.Replace(Environment.NewLine, ";");
                    string passedName = updateLast + "," + updateFirst;
                    string updateHistory = CurrentUserInfo.DisplayName + " | Creating Record : " + UsersModelInstance.UIDRef + " on " + DateTime.Now.ToShortDateString();
                    string updateQueue = "Enterprise";
                    DBConn.AddUsersRecord(updateRef, updateLogID, updateType, passedName, updateAdID, updateEmpID, buildTickets, updateHistory, updateQueue);

                    // After succesful creation in Server updates the local GlobalUIDTable instance
                    DataTable selectNewRow = DBConn.GetSelectedRecord(ConfigurationProperties.UsersTable, "iamhu_uid", updateRef);
                    string updateUidID = selectNewRow.Rows[0][0].ToString();

                    DataRow row;
                    row = MainViewModel.UsersTable.NewRow();
                    row["iamhu_id"] = updateUidID;
                    row["iamhu_uid"] = updateRef;
                    row["iamhu_logid"] = updateLogID;
                    row["iamhu_adid"] = updateAdID;
                    row["iamhu_lob"] = updateType;
                    row["iamhu_empid"] = updateEmpID;
                    row["iamhu_name"] = passedName;
                    row["iamhu_comments"] = buildTickets;
                    row["iamhu_queue"] = updateQueue;
                    row["iamhu_history"] = updateHistory;
                    MainViewModel.UsersTable.Rows.Add(row);

                    ReloadWindow();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }
        #endregion
    }
}

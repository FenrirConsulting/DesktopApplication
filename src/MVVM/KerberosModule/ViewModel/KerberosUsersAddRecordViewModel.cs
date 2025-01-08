using ClosedXML.Excel;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class KerberosUsersAddRecordViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand AddRecord { get; set; }
        public RelayCommand RemoveListCommand { get; set; }
        public RelayCommand AddListCommand { get; set; }
        #endregion

        #region Properties
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // For trackiing duplicate UID Record Creation
        public List<string> recordsList = new();

        // User Properties Model Class
        public KerberosUserModel UserModelInstance { get; set; }

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

        // Basis for Available Terms Collection
        public IEnumerable<ServerName> AvailableServerTerms
        {
            get { return availableServerTerms; }
        }

        // Basis for Available Terms Collection
        private readonly ObservableCollection<ServerName> availableServerTerms = new()
        {

        };

        //Selected Term for Assigned Item
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

        // Basis for Assigned Terms Collection
        private readonly ObservableCollection<ServerName> assignedServerTerms = new()
        {

        };
        #endregion

        #region Methods
        public KerberosUsersAddRecordViewModel()
        {
            AddRecord = new RelayCommand(o => { AddUserRecord(); });
            RemoveListCommand = new RelayCommand(o => { RemoveList(); });
            AddListCommand = new RelayCommand(o => { AddList(); });
            ServersUpdatedList = new();

            UserModelInstance = new KerberosUserModel
            {
                UIDRef = "",
                LogIDRef = "",
                EmpIDRef = "",
                ADIDRef = "",
                FirstName = "",
                LastName = "",
                CommentsRef = ""
            };
        }
        #endregion

        #region Functions
        // Initial DataLoad on Window Load
        public void LoadData()
        {
            try
            {
                assignedServerTerms.Clear();
                UserModelInstance.LogIDRef = "";
                UserModelInstance.EmpIDRef = "";
                UserModelInstance.ADIDRef = "";
                UserModelInstance.FirstName = "";
                UserModelInstance.LastName = "";
                UserModelInstance.CommentsRef = "";

                // Auto Increment new GID Record
                List<double> userRecordsList = DBConn.GetColumnListDouble("dbo.IAMHKUsers", "iamhku_uid");
                List<int> userRecordsIntList = userRecordsList.ConvertAll(Convert.ToInt32);
                int[] listArray = userRecordsIntList.ToArray();
                int lowestAvailableUser = SmallestMissingNumber.FindSmallestMissingNumber(listArray, 100000, 199999);
                UserModelInstance.UIDRef = lowestAvailableUser.ToString();

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
                throw;
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
        public void AddUserRecord()
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

        // Method creating the record in SQL Server DB
        public void CreateRecord()
        {
            try
            {
                // Check new record against UID record list, if no match creates the record.
                bool foundMatch = false;

                List<double> collectedList = DBConn.GetColumnListDouble(ConfigurationProperties.KerberosUsersTable, "iamhku_uid");

                foreach (Double d in collectedList)
                {
                    recordsList.Add(d.ToString());
                }

                foreach (string item in recordsList)
                {
                    if (item == UserModelInstance.UIDRef)
                    {
                        foundMatch = true;
                        MessageBox.Show("UID Record Exists, please select another UID#", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Creates Record, Updates Local Data Table, & Clears Text Boxes
                if (foundMatch == false)
                {
                    foreach (var server in assignedServerTerms)
                    {
                        ServersUpdatedList.Add(server.ServerTerm);
                    }
                    string updateServers = "";
                    updateServers = String.Join(";", ServersUpdatedList.ToArray());

                    string updateRef = UserModelInstance.UIDRef;
                    string updateLogID = UserModelInstance.LogIDRef;
                    string updateEmpID = UserModelInstance.EmpIDRef;
                    string updateAdID = UserModelInstance.ADIDRef;
                    string updateFirst = UserModelInstance.FirstName;
                    string updateLast = UserModelInstance.LastName;
                    string buildTickets = UserModelInstance.CommentsRef;
                    buildTickets = buildTickets.Replace(Environment.NewLine, ";");
                    string passedName = updateLast + "," + updateFirst;
                    string updateHistory = CurrentUserInfo.DisplayName + " | Creating Record : " + UserModelInstance.UIDRef + " on " + DateTime.Now.ToShortDateString();

                    DBConn.AddKerberosUserRecord(updateRef, updateLogID, updateEmpID, updateAdID, passedName, buildTickets, updateServers, updateHistory);

                    // After succesful creation in Server updates the local GlobalUIDTable instance
                    DataTable selectNewRow = DBConn.GetSelectedRecord(ConfigurationProperties.KerberosUsersTable, "iamhku_uid", updateRef);
                    string updateUidID = selectNewRow.Rows[0][0].ToString();

                    DataRow row;
                    row = MainViewModel.KerberosUsersTable.NewRow();
                    row["iamhku_id"] = updateUidID;
                    row["iamhku_uid"] = updateRef;
                    row["iamhku_logid"] = updateLogID;
                    row["iamhku_empid"] = updateEmpID;
                    row["iamhku_adid"] = updateAdID;
                    row["iamhku_name"] = passedName;
                    row["iamhku_comments"] = buildTickets;
                    row["iamhku_servers"] = updateServers;
                    row["iamhku_history"] = updateHistory;
                    MainViewModel.KerberosUsersTable.Rows.Add(row);

                    MSG.Messenger.Default.Send("Kerberos Users Table Reload");
                    LoadData();
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

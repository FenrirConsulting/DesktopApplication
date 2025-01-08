using ClosedXML.Excel;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
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
    public class KerberosUsersUpdateRecordViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand DeleteRecordCommand { get; set; }
        public RelayCommand UpdateRecordCommand { get; set; }
        public RelayCommand RemoveListCommand { get; set; }
        public RelayCommand AddListCommand { get; set; }
        #endregion

        #region Properties
        // Enable Write Permissions on IAM Write Authenticated
        private bool unixPermissionCheck;
        public bool UnixPermissionCheck
        {
            get { return unixPermissionCheck; }
            set { if (unixPermissionCheck != value) { unixPermissionCheck = value; OnPropertyChanged(); } }
        }

        // Check LOGID String Before Update
        private string logIDCheck;
        public string LogIDCheck
        {
            get { return logIDCheck; }
            set { if (logIDCheck != value) { logIDCheck = value; OnPropertyChanged(); } }
        }

        // Check EMPID String Before Update
        private string empIDCheck;
        public string EmpIDCheck
        {
            get { return empIDCheck; }
            set { if (empIDCheck != value) { empIDCheck = value; OnPropertyChanged(); } }
        }

        // Check EMPID String Before Update
        private string adIDCheck;
        public string ADIDCheck
        {
            get { return adIDCheck; }
            set { if (adIDCheck != value) { adIDCheck = value; OnPropertyChanged(); } }
        }

        // Check First Name String Before Update
        private string firstCheck;
        public string FirstCheck
        {
            get { return firstCheck; }
            set { if (firstCheck != value) { firstCheck = value; OnPropertyChanged(); } }
        }

        // Check First Name String Before Update
        private string lastCheck;
        public string LastCheck
        {
            get { return lastCheck; }
            set { if (lastCheck != value) { lastCheck = value; OnPropertyChanged(); } }
        }

        // Check Last Name String Before Update
        private string commentsCheck;
        public string CommentsCheck
        {
            get { return commentsCheck; }
            set { if (commentsCheck != value) { commentsCheck = value; OnPropertyChanged(); } }
        }

        // Check Servers String Before Update
        private string serversCheck;
        public string ServersCheck
        {
            get { return serversCheck; }
            set { if (serversCheck != value) { serversCheck = value; OnPropertyChanged(); } }
        }

        private readonly List<String> ServersCheckList;
        private readonly List<String> ServersUpdatedList;

        private readonly List<String> ticketsCheckList;
        private List<String> ticketsUpdatedList;

        private List<String> ServersAvailableList;

        // Selected Term for ComboBox
        private ServerName selectedAvailableTerm = new();
        public ServerName SelectedAvailableTerm
        {
            get { return selectedAvailableTerm; }
            set { selectedAvailableTerm = value; OnPropertyChanged(); }
        }

        // Basis for Search Terms Collection
        public IEnumerable<ServerName> AvailableServerTerms
        {
            get { return availableServerTerms; }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ServerName> availableServerTerms = new()
        {

        };

        // Selected Term for ComboBox
        private ServerName selectedAssignedTerm = new();
        public ServerName SelectedAssignedTerm
        {
            get { return selectedAssignedTerm; }
            set { selectedAssignedTerm = value; OnPropertyChanged(); }
        }

        // Basis for Search Terms Collection
        public IEnumerable<ServerName> AssignedServerTerms
        {
            get { return assignedServerTerms; }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ServerName> assignedServerTerms = new()
        {

        };

        // Sets the Chosen Record from UID View
        private string chosenRecord;

        // User Information
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // User Properties Model Class
        public KerberosUserModel UserModelInstance { get; set; }
        #endregion

        #region Methods
        public KerberosUsersUpdateRecordViewModel()
        {
            // Initialize Properties
            chosenRecord = KerberosGroupsViewModel.ChosenRecord;
            ServersCheckList = new List<string>();
            ServersUpdatedList = new List<string>();
            ServersAvailableList = new List<string>();
            ticketsCheckList = new List<string>();
            ticketsUpdatedList = new List<string>();
            UnixPermissionCheck = MainViewModel.UnixTablePermissionGlobal;

            UserModelInstance = new KerberosUserModel
            {
                UIDRef = "",
                LogIDRef = "",
                EmpIDRef = "",
                ADIDRef = "",
                FirstName = "",
                LastName = "",
                CommentsRef = "",
                ServersRef = "",
                HistoryRef = "",
            };

            // Initialize Relay Commands
            UpdateRecordCommand = new RelayCommand(o => { BuildUpdate(); });
            DeleteRecordCommand = new RelayCommand(o => { DeleteRecord(); });
            RemoveListCommand = new RelayCommand(o => { RemoveList(); });
            AddListCommand = new RelayCommand(o => { AddList(); });
        }
        #endregion

        #region Functions
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

        // Initial DataLoad on Window Load
        public void LoadData()
        {
            UserModelInstance.UIDRef = "";
            UserModelInstance.LogIDRef = "";
            UserModelInstance.EmpIDRef = "";
            UserModelInstance.ADIDRef = "";
            UserModelInstance.FirstName = "";
            UserModelInstance.LastName = "";
            UserModelInstance.CommentsRef = "";
            UserModelInstance.ServersRef = "";
            UserModelInstance.HistoryRef = "";
            chosenRecord = KerberosUsersViewModel.ChosenRecord;
            try
            {
                // Load in Selected Record information into Text Boxes with formatting
                DataTable foundRecords = DBConn.GetSelectedRecord(ConfigurationProperties.KerberosUsersTable, "iamhku_id", chosenRecord);
                if (foundRecords != null && foundRecords.Rows.Count > 0)
                {
                    ServersAvailableList = DBConn.GetColumnList(ConfigurationProperties.KerberosServersTable, "iamhks_servername");

                    DataRow selectedRecord = foundRecords.Rows[0];
                    string checkEmpty = selectedRecord["iamhku_id"].ToString();

                    if (selectedRecord != null && checkEmpty != "")
                    {
                        string ticketboxBuilder = selectedRecord["iamhku_comments"].ToString();
                        string[] ticketlines = ticketboxBuilder.Split(";");
                        string finalTicketString = "";
                        foreach (var word in ticketlines)
                        {
                            finalTicketString += word + Environment.NewLine;
                            ticketsCheckList.Add(word);
                        }

                        serversCheck = "";
                        string serverSplit = selectedRecord["iamhku_servers"].ToString();
                        string[] serverLines = serverSplit.Split(";");
                        assignedServerTerms.Clear();
                        foreach (var word in serverLines)
                        {
                            assignedServerTerms.Add(new ServerName { ServerTerm = word }); ;
                            serversCheck += word + Environment.NewLine;
                            ServersCheckList.Add(word);
                        }

                        List<string> removed = ServersAvailableList.Except(serverLines).ToList();
                        availableServerTerms.Clear();
                        foreach (var word in removed)
                        {
                            availableServerTerms.Add(new ServerName { ServerTerm = word }); ;
                        }

                        string nameSplit = selectedRecord["iamhku_name"].ToString();
                        string[] nameLines = nameSplit.Split(",");

                       
                        UserModelInstance.UIDRef = selectedRecord["iamhku_uid"].ToString(); 
                        UserModelInstance.LogIDRef = selectedRecord["iamhku_logid"].ToString(); logIDCheck = selectedRecord["iamhku_logid"].ToString();
                        UserModelInstance.EmpIDRef = selectedRecord["iamhku_empid"].ToString(); empIDCheck = selectedRecord["iamhku_empid"].ToString();
                        UserModelInstance.ADIDRef = selectedRecord["iamhku_adid"].ToString(); adIDCheck = selectedRecord["iamhku_adid"].ToString();
                        UserModelInstance.FirstName = nameLines[1].ToString(); FirstCheck = nameLines[1].ToString();
                        UserModelInstance.LastName = nameLines[0].ToString(); LastCheck = nameLines[0].ToString();
                        UserModelInstance.CommentsRef = finalTicketString; CommentsCheck = finalTicketString;
                        UserModelInstance.HistoryRef = selectedRecord["iamhku_history"].ToString();

                    }
                }
                else
                {
                    UserModelInstance.UIDRef = "";
                    UserModelInstance.LogIDRef = "";
                    UserModelInstance.EmpIDRef = "";
                    UserModelInstance.ADIDRef = "";
                    UserModelInstance.FirstName = "";
                    UserModelInstance.LastName = "";
                    UserModelInstance.CommentsRef = "";
                    UserModelInstance.HistoryRef = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Builds the SQL Update Query
        public void BuildUpdate()
        {
            try
            {
                bool foundMatch = false;


                if (foundMatch == false)
                {
                    if (MessageBox.Show("Are you certain you want to update this record?", "Update Record??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        // Set Current User Information
                        CurrentUserInfo = new UserInfo();
                        CurrentUserInfo = App.GlobalUserInfo;

                        string updateRef = chosenRecord;
                        string updateUID = UserModelInstance.UIDRef;
                        string updateLOGID = UserModelInstance.LogIDRef;
                        string updateEMPID = UserModelInstance.EmpIDRef;
                        string updateADID = UserModelInstance.ADIDRef;
                        string updateFirst = UserModelInstance.FirstName;
                        string updateLast = UserModelInstance.LastName;
                        string updateTickets = UserModelInstance.CommentsRef;
                        string updateHistory = "";
                        string finalHistory = "";
                        string updateServers = "";

                        foreach (var server in assignedServerTerms)
                        {
                            ServersUpdatedList.Add(server.ServerTerm);
                            updateServers += server.ServerTerm + Environment.NewLine;
                        }

                        // Checks each TextBox against check strings for changes. Logs each change on new line.
                        if (updateLOGID != logIDCheck)
                        {
                            updateHistory = updateHistory + "LOGID Changed From : " + logIDCheck + Environment.NewLine + " To : " + updateLOGID + Environment.NewLine;
                        }

                        if (updateEMPID != empIDCheck)
                        {
                            updateHistory = updateHistory + "EmpID Changed From : " + empIDCheck + Environment.NewLine + " To : " + updateEMPID + Environment.NewLine;
                        }

                        if (updateADID != adIDCheck)
                        {
                            updateHistory = updateHistory + "ADID Changed From : " + adIDCheck + Environment.NewLine + " To : " + updateADID + Environment.NewLine;
                        }

                        if (updateFirst != firstCheck)
                        {
                            updateHistory = updateHistory + "First Name Changed From : " + firstCheck + Environment.NewLine + " To : " + updateFirst + Environment.NewLine;
                        }

                        if (updateLast != lastCheck)
                        {
                            updateHistory = updateHistory + "Last Name Changed From : " + lastCheck + Environment.NewLine + " To : " + updateLast + Environment.NewLine;
                        }

                        if (updateServers != serversCheck)
                        {
                            string serverChanges = ServerHistoryBuilder();
                            updateHistory = updateHistory + serverChanges + Environment.NewLine;
                        }

                        if (updateTickets != commentsCheck)
                        {
                            string ticketChanges = TicketHistoryBuilder();
                            updateHistory = updateHistory + ticketChanges + Environment.NewLine;
                        }

                        if (updateHistory != "")
                        {
                            finalHistory = "Changes Made by " + CurrentUserInfo.DisplayName + " on " + DateTime.Now.ToShortDateString() + " : " + Environment.NewLine + Environment.NewLine + updateHistory + Environment.NewLine + Environment.NewLine;
                        }

                        // Final changes to add to record. 
                        string passedHistory = finalHistory + UserModelInstance.HistoryRef;
                        updateTickets = updateTickets.Replace(Environment.NewLine, ";");
                        updateServers = "";
                        updateServers = String.Join(";", ServersUpdatedList.ToArray());
                        string passedName = updateLast + "," + updateFirst;

                        // Final Update Transaction, also updates Local GlobalUIDTable to reflect Changes
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_logid", updateLOGID);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_empid", updateEMPID);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_adid", updateADID);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_name", passedName);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_comments", updateTickets);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_servers", updateServers);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosUsersTable, chosenRecord, "iamhku_id", "iamhku_history", passedHistory);
                        DataRow[] rows = MainViewModel.KerberosUsersTable.Select("iamhku_id =" + chosenRecord);
                        foreach (DataRow dr in rows)
                        {
                            dr["iamhku_logid"] = updateLOGID;
                            dr["iamhku_empid"] = updateEMPID;
                            dr["iamhku_adid"] = updateADID;
                            dr["iamhku_name"] = passedName;
                            dr["iamhku_comments"] = updateTickets;
                            dr["iamhku_servers"] = updateServers;
                            dr["iamhku_history"] = passedHistory;
                        }
                        MSG.Messenger.Default.Send("Kerberos Users Table Reload");
                        LoadData();
                    }
                    else
                    {
                        //Do Nothing
                    }
                }
                else
                {
                    //Do Nothing.
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Builds a History of Ticket Changes
        public string TicketHistoryBuilder()
        {
            ticketsUpdatedList = UserModelInstance.CommentsRef.Split(Environment.NewLine).ToList();
            string removedList = "";
            string addedList = "";

            List<string> removed = ticketsCheckList.Except(ticketsUpdatedList).ToList();
            List<string> added = ticketsUpdatedList.Except(ticketsCheckList).ToList();
            if (removed.Count > 0) { removedList = "Comments Removed: " + Environment.NewLine + string.Join(Environment.NewLine, removed) + Environment.NewLine; }
            if (added.Count > 0) { addedList = "Comments Added: " + Environment.NewLine + string.Join(Environment.NewLine, added) + Environment.NewLine; }

            string builtList = removedList + addedList;
            return builtList;
        }

        // Builds a History of Ticket Changes
        public string ServerHistoryBuilder()
        {
            string removedList = "";
            string addedList = "";

            List<string> added = ServersUpdatedList.Except(ServersCheckList).ToList();
            List<string> removed = ServersCheckList.Except(ServersUpdatedList).ToList();
            if (removed.Count > 0) { removedList = "Servers Removed: " + Environment.NewLine + string.Join(Environment.NewLine, removed) + Environment.NewLine; }
            if (added.Count > 0) { addedList = "Servers Added: " + Environment.NewLine + string.Join(Environment.NewLine, added) + Environment.NewLine; }

            string builtList = removedList + addedList;
            return builtList;
        }

        // Delete SQL Record
        public void DeleteRecord()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to delete this record?", "Delete Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string updateRef = chosenRecord;
                    DBConn.DeleteMatchingRecords(ConfigurationProperties.KerberosUsersTable, "iamhku_id", updateRef);
                    List<DataRow> rowsToDelete = new();
                    foreach (DataRow dr in MainViewModel.KerberosUsersTable.Rows)
                    {
                        if (dr["iamhku_id"].ToString() == chosenRecord)
                        {
                            rowsToDelete.Add(dr);
                        }
                    }
                    foreach (DataRow dr in rowsToDelete)
                    {
                        MainViewModel.KerberosUsersTable.Rows.Remove(dr);
                    }
                    MainViewModel.KerberosUsersTable.AcceptChanges();
                    MSG.Messenger.Default.Send("Kerberos Users Table Reload");
                    LoadData();
                }
                else
                {
                    //Do Nothing
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

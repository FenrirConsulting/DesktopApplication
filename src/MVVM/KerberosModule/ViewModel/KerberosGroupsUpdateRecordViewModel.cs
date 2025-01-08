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
    public class KerberosGroupsUpdateRecordViewModel : BaseViewModel
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

        // Check Group String Before Update
        private string groupCheck;
        public string GroupCheck
        {
            get { return groupCheck; }
            set { if (groupCheck != value) { groupCheck = value; OnPropertyChanged(); } }
        }

        // Check Tickets String Before Update
        private string ticketsCheck;
        public string TicketsCheck
        {
            get { return ticketsCheck; }
            set { if (ticketsCheck != value) { ticketsCheck = value; OnPropertyChanged(); } }
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

        // Group Properties Model Class
        public KerberosGroupModel GroupModelInstance { get; set; }
        #endregion

        #region Methods
        public KerberosGroupsUpdateRecordViewModel() 
        {
            // Initialize Properties
            chosenRecord = KerberosGroupsViewModel.ChosenRecord;
            ServersCheckList = new List<string>();
            ServersUpdatedList = new List<string>();
            ServersAvailableList = new List<string>();
            ticketsCheckList = new List<string>();
            ticketsUpdatedList = new List<string>();
            UnixPermissionCheck = MainViewModel.UnixTablePermissionGlobal ;

            GroupModelInstance = new KerberosGroupModel
            {
                GIDRef = "",
                GroupRef = "",
                CommentsRef = "",
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

        // On Load Event
        public void LoadData()
        {
            GroupModelInstance.GIDRef = "";
            GroupModelInstance.GroupRef = "";
            GroupModelInstance.CommentsRef = "";
            GroupModelInstance.HistoryRef = "";
            chosenRecord = KerberosGroupsViewModel.ChosenRecord;

            try
            {
                // Load in Selected Record information into Text Boxes with formatting
                DataTable foundRecords = DBConn.GetSelectedRecord(ConfigurationProperties.KerberosGroupsTable, "iamhkg_id", chosenRecord);
                if (foundRecords != null && foundRecords.Rows.Count > 0)
                {
                    ServersAvailableList = DBConn.GetColumnList(ConfigurationProperties.KerberosServersTable, "iamhks_servername");

                    DataRow selectedRecord = foundRecords.Rows[0];
                    string checkEmpty = selectedRecord["iamhkg_id"].ToString();

                    if (selectedRecord != null && checkEmpty != "")
                    {
                        string finalTicketString = "";
                        string ticketboxBuilder = selectedRecord["iamhkg_comments"].ToString();
                        string[] ticketlines = ticketboxBuilder.Split(";");
                        foreach (var word in ticketlines)
                        {
                            finalTicketString += word + Environment.NewLine;
                            ticketsCheckList.Add(word);
                        }

                        serversCheck = "";
                        string serverSplit = selectedRecord["iamhkg_servers"].ToString();
                        string[] serverLines = serverSplit.Split(",");
                        assignedServerTerms.Clear();
                        foreach (var word in serverLines)
                        {
                            assignedServerTerms.Add(new ServerName { ServerTerm = word });
                            serversCheck += word + Environment.NewLine;
                            ServersCheckList.Add(word);
                        }

                        List<string> removed = ServersAvailableList.Except(serverLines).ToList();
                        availableServerTerms.Clear();
                        foreach (var word in removed)
                        {
                            availableServerTerms.Add(new ServerName { ServerTerm = word });
                        }

                        GroupModelInstance.GIDRef = selectedRecord["iamhkg_gid"].ToString();
                        GroupModelInstance.GroupRef = selectedRecord["iamhkg_group"].ToString(); GroupCheck = GroupModelInstance.GroupRef;
                        GroupModelInstance.CommentsRef = finalTicketString; ticketsCheck = finalTicketString;
                        GroupModelInstance.HistoryRef = selectedRecord["iamhkg_history"].ToString(); 
                    }
                }
                else
                {
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

                if (GroupCheck != GroupModelInstance.GroupRef)
                {
                    List<String> EmpCheck = DBConn.GetColumnList(ConfigurationProperties.KerberosGroupsTable, "iamhkg_group");
                    foreach (string s in EmpCheck)
                    {
                        if (s.ToString() == GroupModelInstance.GroupRef)
                        {
                            foundMatch = true;
                            MessageBox.Show("Group Record Exists, please select another Group Name.", "Entry Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                if (foundMatch == false)
                {
                    if (MessageBox.Show("Are you certain you want to update this record?", "Update Record??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        // Set Current User Information
                        CurrentUserInfo = new UserInfo();
                        CurrentUserInfo = App.GlobalUserInfo;

                        string updateRef = chosenRecord;
                        string updateGroup = GroupModelInstance.GroupRef;
                        string updateTickets = GroupModelInstance.CommentsRef;
                        string updateHistory = "";
                        string finalHistory = "";
                        string updateServers = "";

                        foreach (var server in assignedServerTerms)
                        {
                            ServersUpdatedList.Add(server.ServerTerm);
                            updateServers += server.ServerTerm + Environment.NewLine;
                        }

                        // Checks each TextBox against check strings for changes. Logs each change on new line.
                        if (updateGroup != GroupCheck)
                        {
                            updateHistory = updateHistory + "Group Changed From : " + GroupCheck + Environment.NewLine + " To : " + updateGroup + Environment.NewLine;
                        }

                        if (updateServers != serversCheck)
                        {
                            string serverChanges = ServerHistoryBuilder();
                            updateHistory = updateHistory + serverChanges + Environment.NewLine;
                        }

                        if (updateTickets != ticketsCheck)
                        {
                            string ticketChanges = TicketHistoryBuilder();
                            updateHistory = updateHistory + ticketChanges + Environment.NewLine;
                        }

                        if (updateHistory != "")
                        {
                            finalHistory = "Changes Made by " + CurrentUserInfo.DisplayName + " on " + DateTime.Now.ToShortDateString() + " : " + Environment.NewLine + Environment.NewLine + updateHistory + Environment.NewLine + Environment.NewLine;
                        }

                        // Final changes to add to record. 
                        string passedHistory = finalHistory + GroupModelInstance.HistoryRef;
                        updateTickets = updateTickets.Replace(Environment.NewLine, ";");
                        updateServers = "";
                        updateServers = String.Join(",", ServersUpdatedList.ToArray());

                        // Final Update Transaction, also updates Local GlobalUIDTable to reflect Changes
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosGroupsTable, chosenRecord, "iamhkg_id", "iamhkg_group", updateGroup);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosGroupsTable, chosenRecord, "iamhkg_id", "iamhkg_comments", updateTickets);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosGroupsTable, chosenRecord, "iamhkg_id", "iamhkg_history", passedHistory);
                        DBConn.UpdateTableRecord(ConfigurationProperties.KerberosGroupsTable, chosenRecord, "iamhkg_id", "iamhkg_servers", updateServers);
                        DataRow[] rows = MainViewModel.KerberosGroupsTable.Select("iamhkg_id =" + chosenRecord);
                        foreach (DataRow dr in rows)
                        {
                            dr["iamhkg_group"] = updateGroup;
                            dr["iamhkg_comments"] = updateTickets;
                            dr["iamhkg_history"] = passedHistory;
                            dr["iamhkg_servers"] = updateServers;
                        }
                        MSG.Messenger.Default.Send("Kerberos Groups Table Reload");
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
            ticketsUpdatedList = GroupModelInstance.CommentsRef.Split(Environment.NewLine).ToList();
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
                    DBConn.DeleteMatchingRecords(ConfigurationProperties.KerberosGroupsTable, "iamhkg_id", updateRef);
                    List<DataRow> rowsToDelete = new();
                    foreach (DataRow dr in MainViewModel.KerberosGroupsTable.Rows)
                    {
                        if (dr["iamhkg_id"].ToString() == chosenRecord)
                        {
                            rowsToDelete.Add(dr);
                        }
                    }
                    foreach (DataRow dr in rowsToDelete)
                    {
                        MainViewModel.KerberosGroupsTable.Rows.Remove(dr);
                    }
                    MainViewModel.KerberosGroupsTable.AcceptChanges();
                    MSG.Messenger.Default.Send("Kerberos Groups Table Reload");
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

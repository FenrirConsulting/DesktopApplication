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
    public class UsersUpdateRecordViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand UpdateRecordCommand { get; set; }
        public RelayCommand LowerChars { get; set; }
        public RelayCommand DeleteRecordCommand { get; set; }
        #endregion

        #region Properties
        // Checks, load initial data to check against submitted changes
        private string logidCheck;
        private string empidCheck;
        private string adidCheck;
        private string typeCheck;
        private string firstnameCheck;
        private string lastnameCheck;
        private string ticketsCheck;
        private readonly List<String> ticketsCheckList;
        private List<String> ticketsUpdatedList;

        // Enable Write Permissions on IAM Write Authenticated
        private bool unixPermissionCheck;
        public bool UnixPermissionCheck
        {
            get { return unixPermissionCheck; }
            set { if (unixPermissionCheck != value) { unixPermissionCheck = value; OnPropertyChanged(); } }
        }

        // Sets the Chosen Record from UID View
        private readonly string chosenRecord;

        // Binds UID Variable Label Number
        private string uidLabelString;
        public string UidLabelString
        {
            get { return uidLabelString; }
            set { if (uidLabelString != value) { uidLabelString = value; OnPropertyChanged(); } }
        }

        // Binds UID Variable Label Number
        private string queueLabelString;
        public string QueueLabelString
        {
            get { return queueLabelString; }
            set { if (queueLabelString != value) { queueLabelString = value; OnPropertyChanged(); } }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> searchTerms = new()
        {
            new ComboBoxListItem { BoxItem = "Enterprise" },
            new ComboBoxListItem { BoxItem = "PBM" },
            new ComboBoxListItem { BoxItem = "Retail" },
            new ComboBoxListItem { BoxItem = "Company" },
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

        // UID Properties Model Class
        public UsersModel UsersModelInstance { get; set; }

        #endregion

        #region Methods
        public UsersUpdateRecordViewModel()
        {
            // Initialize Properties
            chosenRecord = UsersTableViewModel.ChosenRecord;
            ticketsCheckList = new List<string>();
            UnixPermissionCheck = MainViewModel.UnixTablePermissionGlobal;
            ticketsUpdatedList = new List<string>();

            UsersModelInstance = new UsersModel
            {
                UIDRef = "",
                LogRef = "",
                TypeRef = "",
                QueueRef= "",
                AdidRef = "",
                EmpRef = "",
                FirstRef = "",
                LastRef = "",
                CommentsRef = "",
                HistoryRef = ""
            };

            // Initialize Relay Commands
            UpdateRecordCommand = new RelayCommand(o => { BuildUpdate(); });
            DeleteRecordCommand = new RelayCommand(o => { DeleteRecord(); });
        }
        #endregion

        #region Functions

        // On Load Event
        public void LoadData()
        {
            UsersModelInstance.UIDRef = "";
            UsersModelInstance.LogRef = "";
            UsersModelInstance.TypeRef = "";
            UsersModelInstance.QueueRef = "";
            UsersModelInstance.AdidRef = "";
            UsersModelInstance.EmpRef = "";
            UsersModelInstance.FirstRef = "";
            UsersModelInstance.LastRef = "";
            UsersModelInstance.CommentsRef = "";
            UsersModelInstance.HistoryRef = "";

            try
            {
                // Load in Selected Record information into Text Boxes with formatting
                DataTable foundRecords = DBConn.GetSelectedRecord(ConfigurationProperties.UsersTable, "iamhu_id", chosenRecord);
                if (foundRecords != null && foundRecords.Rows.Count > 0)
                {
                    DataRow selectedRecord = foundRecords.Rows[0];
                    string checkEmpty = selectedRecord["iamhu_id"].ToString();

                    if (selectedRecord != null && checkEmpty != "")
                    {
                        char[] delimiterChars = { ';' };
                        string ticketboxBuilder = selectedRecord["iamhu_comments"].ToString();
                        string[] ticketlines = ticketboxBuilder.Split(delimiterChars);
                        string finalTicketString = "";
                        string nameSplit = selectedRecord["iamhu_name"].ToString();
                        string[] nameLines = nameSplit.Split(",");

                        foreach (var word in ticketlines)
                        {
                            finalTicketString += word + Environment.NewLine;
                            ticketsCheckList.Add(word);
                        }
                        UsersModelInstance.UIDRef = selectedRecord["iamhu_uid"].ToString(); UidLabelString = UsersModelInstance.UIDRef;
                        UsersModelInstance.QueueRef = selectedRecord["iamhu_queue"].ToString(); QueueLabelString = UsersModelInstance.QueueRef;
                        UsersModelInstance.LogRef = selectedRecord["iamhu_logid"].ToString(); logidCheck = selectedRecord["iamhu_logid"].ToString();
                        UsersModelInstance.AdidRef = selectedRecord["iamhu_adid"].ToString(); adidCheck = selectedRecord["iamhu_adid"].ToString();
                        UsersModelInstance.TypeRef = selectedRecord["iamhu_lob"].ToString(); typeCheck = selectedRecord["iamhu_lob"].ToString();
                        UsersModelInstance.EmpRef = selectedRecord["iamhu_empid"].ToString(); empidCheck = selectedRecord["iamhu_empid"].ToString();
                        UsersModelInstance.FirstRef = nameLines[1].ToString(); firstnameCheck = nameLines[1].ToString();
                        UsersModelInstance.LastRef = nameLines[0].ToString(); lastnameCheck = nameLines[0].ToString();
                        UsersModelInstance.CommentsRef = finalTicketString; ticketsCheck = finalTicketString;
                        UsersModelInstance.HistoryRef = selectedRecord["iamhu_history"].ToString();
                        
                    }
                    int chosenTerm = UsersModelInstance.TypeRef switch
                    {
                        "Enterprise" => 0,
                        "PBM" => 1,
                        "Retail" => 2,
                        "Company" => 3,
                        "Other" => 4,
                        _ => 4,
                    };
                    SelectedTerm = SearchTerms.ElementAt(chosenTerm);
                }
                else
                {
                    UsersModelInstance.UIDRef = "";
                    UsersModelInstance.LogRef = "";
                    UsersModelInstance.AdidRef = "";
                    UsersModelInstance.TypeRef = "";
                    UsersModelInstance.EmpRef = "";
                    UsersModelInstance.FirstRef = "";
                    UsersModelInstance.LastRef = "";
                    UsersModelInstance.CommentsRef = "";
                    UsersModelInstance.CommentsRef = "";
                    SelectedTerm = SearchTerms.ElementAt(0);
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

                        UsersModelInstance.TypeRef = SelectedTerm.BoxItem.ToString();

                        string updateRef = chosenRecord;
                        string updateUid = UsersModelInstance.UIDRef;
                        string updateLogID = UsersModelInstance.LogRef;
                        string updateEmpID = UsersModelInstance.EmpRef;
                        string updateAdID = UsersModelInstance.AdidRef;
                        string updateType = UsersModelInstance.TypeRef;
                        string updateFirst = UsersModelInstance.FirstRef;
                        string updateLast = UsersModelInstance.LastRef;
                        string updateTickets = UsersModelInstance.CommentsRef;
                        string updateHistory = "";
                        string finalHistory = "";


                        // Checks each TextBox against check strings for changes. Logs each change on new line.
                        if (updateLogID != logidCheck)
                        {
                            updateHistory = updateHistory + "LOGID Changed From : " + logidCheck + Environment.NewLine + " To : " + updateLogID + Environment.NewLine;
                        }

                        if (updateEmpID != empidCheck)
                        {
                            updateHistory = updateHistory + "EmpID Changed From : " + empidCheck + Environment.NewLine + " To : " + updateEmpID + Environment.NewLine;
                        }

                        if (updateAdID != adidCheck)
                        {
                            updateHistory = updateHistory + "ADID Changed From : " + adidCheck + Environment.NewLine + " To : " + updateAdID + Environment.NewLine;
                        }

                        if (updateType != typeCheck)
                        {
                            updateHistory = updateHistory + "Type Changed From : " + typeCheck + Environment.NewLine + " To : " + updateType + Environment.NewLine;
                        }

                        if (updateFirst != firstnameCheck)
                        {
                            updateHistory = updateHistory + "First Name Changed From : " + firstnameCheck + Environment.NewLine + " To : " + updateFirst + Environment.NewLine;
                        }

                        if (updateLast != lastnameCheck)
                        {
                            updateHistory = updateHistory + "Last Name Changed From : " + lastnameCheck + Environment.NewLine + " To : " + updateLast + Environment.NewLine;
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

                        // Final changes history to add to record. 
                        string passedHistory = finalHistory + UsersModelInstance.HistoryRef;
                        updateTickets = updateTickets.Replace(Environment.NewLine, ";");
                        string passedName = updateLast + "," + updateFirst;

                        // Final Update Transaction, also updates Local GlobalUIDTable to reflect Changes
                        DBConn.UsersTableRecordUpdate(updateRef, updateUid, updateLogID, updateType, passedName, updateAdID, updateEmpID, updateTickets, passedHistory);
                        DataRow[] rows = MainViewModel.UsersTable.Select("iamhu_id =" + chosenRecord);
                        foreach (DataRow dr in rows)
                        {
                            dr["iamhu_logid"] = updateLogID;
                            dr["iamhu_adid"] = updateAdID;
                            dr["iamhu_lob"] = updateType;
                            dr["iamhu_empid"] = updateEmpID;
                            dr["iamhu_name"] = passedName;
                            dr["iamhu_comments"] = updateTickets;
                            dr["iamhu_history"] = passedHistory;
                        }
                        MSG.Messenger.Default.Send("Users Table Reload");
                        LoadData();
                    }
                    else
                    {
                        //Do Nothing
                    }
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
            ticketsUpdatedList = UsersModelInstance.CommentsRef.Split(Environment.NewLine).ToList();
            string removedList = "";
            string addedList = "";


            List<string> removed = ticketsCheckList.Except(ticketsUpdatedList).ToList();
            List<string> added = ticketsUpdatedList.Except(ticketsCheckList).ToList();
            if (removed.Count > 0) { removedList = "Comments Removed: " + Environment.NewLine + string.Join(Environment.NewLine, removed) + Environment.NewLine; }
            if (added.Count > 0) { addedList = "Comments Added: " + Environment.NewLine + string.Join(Environment.NewLine, added) + Environment.NewLine; }

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
                    DBConn.DeleteMatchingRecords(ConfigurationProperties.UsersTable, "iamhu_id", updateRef);
                    List<DataRow> rowsToDelete = new();
                    foreach (DataRow dr in MainViewModel.UsersTable.Rows)
                    {
                        if (dr["iamhu_id"].ToString() == chosenRecord)
                        {
                            rowsToDelete.Add(dr);
                        }
                    }
                    foreach (DataRow dr in rowsToDelete)
                    {
                        MainViewModel.UsersTable.Rows.Remove(dr);
                    }
                    MainViewModel.UsersTable.AcceptChanges();
                    MSG.Messenger.Default.Send("Users Table Reload");
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

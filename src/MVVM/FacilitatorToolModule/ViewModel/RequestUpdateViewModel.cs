
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class RequestUpdateViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand GetRequestCommand { get; set; }
        public RelayCommand UpdateRequestCommand { get; set; }
        public FactoolHistoryBuilder HBuilder = new();
        private readonly SQLMethods DBConn = new();

        #endregion

        #region Properties
        // Binds Agents Working Textbox String
        private string agentsWorkingString;
        public string AgentsWorkingString
        {
            get { return agentsWorkingString; }
            set
            {
                if (agentsWorkingString != value)
                {
                    agentsWorkingString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds XRef1 Label String
        private string xref1String;
        public string Xref1String
        {
            get { return xref1String; }
            set
            {
                if (xref1String != value)
                {
                    xref1String = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Systems String
        private string systems;
        public string Systems
        {
            get { return systems; }
            set
            {
                if (systems != value)
                {
                    systems = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Facilitator Comments String
        private string facilitatorComments;
        public string FacilitatorComments
        {
            get { return facilitatorComments; }
            set
            {
                if (facilitatorComments != value)
                {
                    facilitatorComments = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Reference Textbox String
        private string referenceTextBoxString;
        public string ReferenceTextBoxString
        {
            get { return referenceTextBoxString; }
            set
            {
                if (referenceTextBoxString != value)
                {
                    string preFix = value;
                    preFix = preFix.Replace(" ", String.Empty);
                    var afterFix = Regex.Replace(preFix, @"[^0-9a-zA-Z]+", "").Trim();
                    referenceTextBoxString = afterFix;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Object Collection of State Type
        private readonly ObservableCollection<ComboBoxListItem> stateTypes = new()
        {

        };
        public IEnumerable<ComboBoxListItem> StateTypes
        {
            get { return stateTypes; }
        }

        // Selected Term for State Type ComboBox
        private ComboBoxListItem selectedState = new();
        public ComboBoxListItem SelectedState
        {
            get { return selectedState; }
            set
            {
                selectedState = value;
                OnPropertyChanged();
            }
        }

        // Binds Send Back Checkbox
        private bool sendBackChecked;
        public bool SendBackChecked
        {
            get { return sendBackChecked; }
            set { if (sendBackChecked != value) { sendBackChecked = value; OnPropertyChanged(); } }
        }

        // Binds Received Back Checkbox
        private bool receivedBackChecked;
        public bool ReceivedBackChecked
        {
            get { return receivedBackChecked; }
            set { if (receivedBackChecked != value) { receivedBackChecked = value; OnPropertyChanged(); } }
        }

        // Binds Send Back Times Label String
        private string sentBackString;
        public string SentBackString
        {
            get { return sentBackString; }
            set
            {
                if (sentBackString != value)
                {
                    sentBackString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Status Label String
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

        // Binds Date Created Label String
        private string dateCreatedString;
        public string DateCreatedString
        {
            get { return dateCreatedString; }
            set
            {
                if (dateCreatedString != value)
                {
                    dateCreatedString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Date SLA Start Label String
        private string slaStartString;
        public string SLAStartString
        {
            get { return slaStartString; }
            set
            {
                if (slaStartString != value)
                {
                    slaStartString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Completed Date Label String
        private string completedDateString;
        public string CompletedDateString
        {
            get { return completedDateString; }
            set
            {
                if (completedDateString != value)
                {
                    completedDateString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Completion Time Label String
        private string completionTimeString;
        public string CompletionTimeString
        {
            get { return completionTimeString; }
            set
            {
                if (completionTimeString != value)
                {
                    completionTimeString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Add Comments Textbox String
        private string addCommentsString;
        public string AddCommentsString
        {
            get { return addCommentsString; }
            set
            {
                if (addCommentsString != value)
                {
                    addCommentsString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Add Comments Textbox String
        private string commentsString;
        public string CommentsString
        {
            get { return commentsString; }
            set
            {
                if (commentsString != value)
                {
                    commentsString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Agents String List
        private List<string> agentsStringList;
        public List<string> AgentsStringList
        {
            get { return agentsStringList; }
            set
            {
                if (agentsStringList != value)
                {
                    agentsStringList = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Update Button as being Enabled
        private bool updateEnabled;
        public bool UpdateEnabled
        {
            get { return updateEnabled; }
            set { if (updateEnabled != value) { updateEnabled = value; OnPropertyChanged(); } }
        }

        // Holds the Current Request to Compare Against Final Changes for History Tracking
        private FacToolRequest activeRequest;
        public FacToolRequest ActiveRequest
        {
            get { return activeRequest; }
            set { if (activeRequest != value) { activeRequest = value; } }
        }

        // Holds Object Collection of Application History List
        private ObservableCollection<FactoolRequestApplicationHistoryModel> applicationHistoryModels = new()
        {
        };
        public IEnumerable<FactoolRequestApplicationHistoryModel> ApplicationHistoryModels
        {
            get { return applicationHistoryModels; }
        }

        // Selected Term for Current Application History Model
        private FactoolRequestApplicationHistoryModel selectedHistoryModel = new();
        public FactoolRequestApplicationHistoryModel SelectedHistoryModel
        {
            get { return selectedHistoryModel; }
            set
            {
                selectedHistoryModel = value;
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable applicationHistoryTable;
        public DataTable ApplicationHistoryTable
        {
            get { return applicationHistoryTable; }
            set { applicationHistoryTable = value; OnPropertyChanged(); }
        }



        #endregion

        #region Methods
        public RequestUpdateViewModel()
        {
            //Initialize Properties
            ReceivedBackChecked = false;
            SendBackChecked = false;
            AgentsStringList = new();
            UpdateEnabled = true;
            ActiveRequest = new()
            {
                AgentComments = "",
                AgentsWorked = "",
                Comments = "",
                CompletionDate = "",
                CreateDate = "",
                CreateTick = "",
                DefectReason = new string[] { },
                DisplayName = "",
                FormType = "",
                ModifiedDate = "",
                ModifiedTick = "",
                NewRequest = "",
                ReferenceNumber = "",
                ReplyTypes = new string[] { },
                ReqStatusID = 0,
                RequestState = "",
                RequestStatus = "",
                RequestType = new string[] { },
                SamAccount = "",
                SentStatus = "",
                SLACompletionTime = "",
                SLAStart = "",
                Systems = new string[] { },
                TimesReturned = "",
                TotalUsers = "",
                TouchPoints = "",
                XREF1 = "",
                XREF2 = "",
                _id = 0
            };

            //Initialize Relay Commands
            GetRequestCommand = new RelayCommand(o => { GetRequest(); });
            UpdateRequestCommand = new RelayCommand(o => { UpdateRequest(); });
        }
        #endregion

        #region Functions
        // Main User Control Load Function
        public void LoadData()
        {

            SetControls("Pre-Approval");
        }

        // Main Function to Fetch Request in Request Number Textbox
        public void GetRequest()
        {
            try
            {
                ReferenceTextBoxString = string.Join("", ReferenceTextBoxString.Split().Where(w => !Utilities.FilteredWords.Contains(w, StringComparer.InvariantCultureIgnoreCase)));


                FacToolRequest FindRequest = new();
                DataTable RequestTable = new();
                DataTable StatusTable = new();
                DataTable ApplicationHistoryTable = new();
                RequestTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestTable, "ReferenceNumber", ReferenceTextBoxString);
                StatusTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestStatus, "ReferenceNumber", ReferenceTextBoxString);
                ApplicationHistoryTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolApplicationHistoryTable, "ReferenceNumber", ReferenceTextBoxString);



                if (RequestTable.Rows.Count > 0 && StatusTable.Rows.Count > 0)
                {
                    StatusString = "Reference Number " + ReferenceTextBoxString + " found.";

                    FindRequest._id = Convert.ToInt32(RequestTable.Rows[0][0]);
                    FindRequest.CreateDate = RequestTable.Rows[0][1].ToString() ?? "";
                    FindRequest.CreateTick = RequestTable.Rows[0][2].ToString() ?? "";
                    FindRequest.ModifiedDate = RequestTable.Rows[0][3].ToString() ?? "";
                    FindRequest.ModifiedTick = RequestTable.Rows[0][4].ToString() ?? "";
                    FindRequest.SamAccount = RequestTable.Rows[0][5].ToString() ?? "";
                    FindRequest.DisplayName = RequestTable.Rows[0][6].ToString() ?? "";
                    FindRequest.ReferenceNumber = RequestTable.Rows[0][7].ToString() ?? "";
                    FindRequest.NewRequest = RequestTable.Rows[0][8].ToString() ?? "";
                    FindRequest.TotalUsers = RequestTable.Rows[0][9].ToString() ?? "";
                    FindRequest.RequestStatus = RequestTable.Rows[0][10].ToString() ?? "";
                    FindRequest.FormType = RequestTable.Rows[0][11].ToString() ?? "";
                    FindRequest.RequestType = RequestTable.Rows[0][12].ToString().Split(',');
                    FindRequest.DefectReason = RequestTable.Rows[0][13].ToString().Split(',');
                    FindRequest.Systems = RequestTable.Rows[0][14].ToString().Split(',');
                    FindRequest.ReplyTypes = RequestTable.Rows[0][15].ToString().Split(',');
                    FindRequest.Comments = RequestTable.Rows[0][16].ToString() ?? "";
                    FindRequest.XREF1 = RequestTable.Rows[0][17].ToString() ?? "";
                    FindRequest.XREF2 = RequestTable.Rows[0][18].ToString() ?? "";
                    FindRequest.ReqStatusID = Int32.Parse(StatusTable.Rows[0][0].ToString());
                    FindRequest.SentStatus = StatusTable.Rows[0][2].ToString() ?? "";
                    FindRequest.RequestState = StatusTable.Rows[0][3].ToString() ?? "";
                    FindRequest.TouchPoints = StatusTable.Rows[0][4].ToString() ?? "";
                    FindRequest.TimesReturned = StatusTable.Rows[0][5].ToString() ?? "";
                    FindRequest.SLAStart = StatusTable.Rows[0][6].ToString() ?? "";
                    FindRequest.CompletionDate = StatusTable.Rows[0][7].ToString() ?? "";
                    FindRequest.SLACompletionTime = StatusTable.Rows[0][8].ToString() ?? "";
                    FindRequest.AgentComments = StatusTable.Rows[0][9].ToString() ?? "";
                    FindRequest.AgentsWorked = StatusTable.Rows[0][10].ToString() ?? "";

                    SetControls(FindRequest.RequestState);

                    ActiveRequest = FindRequest; // Hold for Update Check Later 

                    AgentsStringList.Clear();
                    string[] nameLines = FindRequest.AgentsWorked.Split(";");
                    string finalAgentList = "";
                    foreach (var word in nameLines)
                    {
                        finalAgentList += word + Environment.NewLine + Environment.NewLine;
                        AgentsStringList.Add(word);
                    }

                    AgentsWorkingString = finalAgentList;

                    // Pass in Request State to Determine which Approval States are Available to Set To


                    if (!string.IsNullOrEmpty(FindRequest.RequestState))
                    {
                        SelectedState = StateTypes.FirstOrDefault(X => X.BoxItem == FindRequest.RequestState);
                    }

                    if (FindRequest.SentStatus == "Sent") { SendBackChecked = true; ReceivedBackChecked = false; } else { ReceivedBackChecked = true; SendBackChecked = false; }
                    SentBackString = FindRequest.TimesReturned;

                    DateTime UTCDateTime = DateTime.Parse(FindRequest.CreateDate);
                    DateTime localTime = UTCDateTime.ToLocalTime();
                    DateCreatedString = localTime.ToString();

                    if (FindRequest.SLAStart == "Pre-Approval")
                    {
                        SLAStartString = FindRequest.SLAStart;
                    }
                    else
                    {
                        UTCDateTime = DateTime.Parse(FindRequest.SLAStart);
                        localTime = UTCDateTime.ToLocalTime();
                        SLAStartString = localTime.ToString();
                    }

                    if (FindRequest.CompletionDate == "Incomplete" || FindRequest.CompletionDate == "Cancelled")
                    {
                        CompletedDateString = FindRequest.CompletionDate;
                    }
                    else
                    {
                        UTCDateTime = DateTime.Parse(FindRequest.CompletionDate);
                        localTime = UTCDateTime.ToLocalTime();
                        CompletedDateString = localTime.ToString();
                    }

                    CompletionTimeString = FindRequest.SLACompletionTime;
                    CommentsString = FindRequest.AgentComments;
                    Xref1String = FindRequest.XREF1;
                    Systems = string.Join(Environment.NewLine, FindRequest.Systems);
                    FacilitatorComments = FindRequest.Comments;

                    foreach (DataRow row in ApplicationHistoryTable.Rows)
                    {
                        applicationHistoryModels.Add(new FactoolRequestApplicationHistoryModel
                        {
                            ReferenceNumber = row["ReferenceNumber"].ToString(),
                            System = row["System"].ToString(),
                            CreatedDate = row["CreatedDate"].ToString(),
                            SLAStart = row["SLAStart"].ToString(),
                            CompletedDate = row["CompletedDate"].ToString(),
                            CompletedBy = row["CompletedBy"].ToString(),
                            CompletedFlag = row["CompletedFlag"].ToString(),
                            Users = row["Users"].ToString()
                        });
                    }

                }
                else
                {
                    StatusString = "Reference Number " + ReferenceTextBoxString + " not found.";
                    applicationHistoryModels.Clear();
                    SetControls("Pre-Approval");
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Update Request Submission Function
        public void UpdateRequest()
        {
            if (MessageBox.Show("Are you certain you want to update this request?", "Update Request?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                try
                {
                    DateTime createDate = DateTime.UtcNow;
                    FacToolRequest PassedRequest = new();

                    // Properties not Changed by Agent
                    PassedRequest._id = ActiveRequest._id;
                    PassedRequest.CreateDate = ActiveRequest.CreateDate;
                    PassedRequest.CreateTick = ActiveRequest.CreateTick;
                    PassedRequest.ModifiedDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    PassedRequest.ModifiedTick = createDate.Ticks.ToString();
                    PassedRequest.SamAccount = ActiveRequest.SamAccount;
                    PassedRequest.DisplayName = ActiveRequest.DisplayName;
                    PassedRequest.ReferenceNumber = ActiveRequest.ReferenceNumber;
                    PassedRequest.NewRequest = ActiveRequest.NewRequest;
                    PassedRequest.TotalUsers = ActiveRequest.TotalUsers;
                    PassedRequest.RequestStatus = ActiveRequest.RequestStatus;
                    PassedRequest.FormType = ActiveRequest.FormType;
                    PassedRequest.RequestType = ActiveRequest.RequestType;
                    PassedRequest.DefectReason = ActiveRequest.DefectReason;
                    PassedRequest.Systems = ActiveRequest.Systems;
                    PassedRequest.ReplyTypes = ActiveRequest.ReplyTypes;
                    PassedRequest.Comments = ActiveRequest.Comments;
                    PassedRequest.XREF1 = ActiveRequest.XREF1;
                    PassedRequest.XREF2 = ActiveRequest.XREF2;
                    PassedRequest.TouchPoints = ActiveRequest.TouchPoints;

                    // Sets Sent to Either Sent or Received if not Checked by Agent
                    if (SendBackChecked == true) { PassedRequest.SentStatus = "Sent"; } else { PassedRequest.SentStatus = "Received"; }
                    PassedRequest.RequestState = SelectedState.BoxItem;

                    // If Sent Back is Checked, Increments the Times Sent by 1
                    int returnTimes = Convert.ToInt32(ActiveRequest.TimesReturned);
                    if (SendBackChecked == true) { returnTimes++; }
                    PassedRequest.TimesReturned = returnTimes.ToString();

                    // Once the State gets past Pre-Approval, SLA Start Time is Set. If it is sent back to Pre-Approval by Facilitators, SLA Time is Reset 
                    PassedRequest.SLAStart = ActiveRequest.SLAStart;
                    if (SLAStartString == "Pre-Approval" && SelectedState.BoxItem != "Pre-Approval") { PassedRequest.SLAStart = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff"); SLAStartString = PassedRequest.SLAStart; }
                    if (SelectedState.BoxItem == "Pre-Approval") { PassedRequest.SLAStart = "Pre-Approval"; }

                    // Upon Request completion, State is set to Complete or Cancelled, as well as Request Status closing out the ticket
                    if (SelectedState.BoxItem != "Completed" && SelectedState.BoxItem != "Cancelled") { PassedRequest.CompletionDate = "Incomplete"; }
                    if (SelectedState.BoxItem == "Completed" || SelectedState.BoxItem == "Cancelled")
                    {
                        if (SelectedState.BoxItem == "Completed") { PassedRequest.CompletionDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff"); CompletedDateString = PassedRequest.CompletionDate; PassedRequest.RequestStatus = "Complete"; }
                        if (SelectedState.BoxItem == "Cancelled") { PassedRequest.RequestState = "Cancelled"; PassedRequest.CompletionDate = "Cancelled"; CompletedDateString = "Cancelled"; }
                    }

                    // Is Start and Completed Date are set, calculates the time span between them to calculate completion time
                    if (PassedRequest.CompletionDate == "Incomplete" || PassedRequest.SLAStart == "Incomplete") { PassedRequest.SLACompletionTime = "Incomplete"; }
                    else
                    {
                        if (PassedRequest.CompletionDate == "Cancelled") { PassedRequest.SLACompletionTime = "Cancelled"; CompletionTimeString = "Cancelled"; }
                        else
                        {
                            PassedRequest.SLACompletionTime = CalculateCompletionTime(PassedRequest); CompletionTimeString = PassedRequest.SLACompletionTime;
                        }
                    }

                    // If Add Comments is not left empty, addes onto Agent Comments with a name and time stamp.
                    string UpdatedComments = BuildComments();
                    if (UpdatedComments != "") { PassedRequest.AgentComments = ActiveRequest.AgentComments + UpdatedComments; }
                    else { PassedRequest.AgentComments = ActiveRequest.AgentComments; CommentsString = PassedRequest.AgentComments; }

                    // If agent is not listed on Worked, added to list of Worked Agents
                    UserPrincipal userPrincipal = UserPrincipal.Current;
                    bool trueInList = AgentsStringList.Contains(userPrincipal.Name);
                    if (trueInList == false) { AgentsStringList.Add(userPrincipal.Name); }
                    PassedRequest.AgentsWorked = string.Join(";", AgentsStringList);
                    AgentsStringList.Clear();
                    string[] nameLines = PassedRequest.AgentsWorked.Split(";");
                    string finalAgentList = "";
                    foreach (var word in nameLines)
                    {
                        finalAgentList += word + Environment.NewLine + Environment.NewLine;
                        AgentsStringList.Add(word);
                    }
                    AgentsWorkingString = finalAgentList;

                    DBConn.FacToolUpdateRequest(PassedRequest, ConfigurationProperties.FactoolRequestTable);
                    DBConn.FacToolUpdateRequestStatus(PassedRequest, ConfigurationProperties.FactoolRequestStatus);
                    FactoolRequestHistory PassedHistory = new();
                    PassedHistory = HBuilder.HistoryBuilder(ActiveRequest, PassedRequest, "Existing"); // Passing Original and Updated Request to Build History
                    DBConn.AddFactoolRequestHistory(PassedHistory, ConfigurationProperties.FactoolRequestHistory);

                    StatusString = "Successfully Updated Reference # " + PassedHistory.ReferenceNumber;

                    GetRequest();
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            }
            else
            {

            }
        }

        // Sets Variables for UI Controls
        public void SetControls(string currentState)
        {
            try
            {
                stateTypes.Clear();

                // Available States determined by Current State of Request
                switch (currentState)
                {
                    case "Pre-Approval":
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Pre-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Post-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "In-Progress"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Cancelled"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Completed"
                        });
                        UpdateEnabled = true;
                        break;

                    case "Post-Approval":
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Post-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "In-Progress"
                        });

                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Cancelled"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Completed"
                        });
                        UpdateEnabled = true;
                        break;

                    case "In-Progress":
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Post-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "In-Progress"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Cancelled"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Completed"
                        });
                        UpdateEnabled = true;
                        break;

                    case "Completed":
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Completed"
                        });
                        UpdateEnabled = false;
                        break;

                    case "Cancelled":
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Cancelled"
                        });
                        UpdateEnabled = false;
                        break;

                    default:
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Pre-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Post-Approval"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "In-Progress"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Cancelled"
                        });
                        stateTypes.Add(new ComboBoxListItem
                        {
                            BoxItem = "Completed"
                        });
                        UpdateEnabled = true;
                        break;
                }
                SelectedState = StateTypes.ElementAt(0);
                SendBackChecked = false;
                ReceivedBackChecked = false;
                DateCreatedString = "";
                SLAStartString = "";
                CompletedDateString = "";
                CompletionTimeString = "";
                SentBackString = "";
                AgentsWorkingString = "";
                AddCommentsString = "";
                CommentsString = "";
                Xref1String = "";
                Systems = "";
                FacilitatorComments = "";
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Builds an Additional Comment to Add to Agent Comments
        public string BuildComments()
        {
            string finalComment = "";
            try
            {
                if (AddCommentsString != "")
                {
                    DateTime createDate = DateTime.UtcNow;
                    finalComment += Environment.NewLine + Environment.NewLine;
                    finalComment += AddCommentsString + Environment.NewLine;
                    finalComment += "Comments Made by " + App.GlobalUserInfo.DisplayName + " on " + createDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return finalComment;
        }

        // Calculate the Completion time between SLA Start and Completion Dates
        public string CalculateCompletionTime(FacToolRequest passedRequest)
        {
            string completionTime = "";
            try
            {
                DateTime StartTime = Convert.ToDateTime(passedRequest.SLAStart);
                DateTime CompletionTime = Convert.ToDateTime(passedRequest.CompletionDate);
                TimespanCalculation.DaySpan DS = TimespanCalculation.ComputeDaysDifference(StartTime, CompletionTime, false, false);
                completionTime = string.Format("{0} days, {1} hours, {2} minutes, {3} seconds", DS.Days.ToString(), DS.Hours.ToString(), DS.Minutes.ToString(), DS.Seconds.ToString());
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return completionTime;
        }
        #endregion
    }
}

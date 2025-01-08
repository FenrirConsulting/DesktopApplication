using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Centrify.DirectControl.API;
using System.Linq;
using System.Data;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Security.Principal;
using MSG = GalaSoft.MvvmLight.Messaging;
using System.Reflection;
using SimpleImpersonation;
using System.DirectoryServices.AccountManagement;
using IAMHeimdall.Resources.Commands;
using System.Threading;
using System.Net;
using Microsoft.Graph;
using AngleSharp.Io;
using DocumentFormat.OpenXml.Drawing;
using PnP.Framework.Diagnostics.Tree;
using System.Windows.Interop;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using Nancy;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Wordprocessing;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class AccessNowCloseTicketViewModel : BaseViewModel
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand GetTicketCommand { get; set; }
        public RelayCommand CloseTicketCommand { get; set; }
        public RelayCommand CloseAllTicketsCommand { get; set; }
        public RelayCommand UpdateCommentsCommand { get; set; }
        #endregion

        #region Properties
        // Holds the Access Now API Connection Credentials
        private string apiUsername;
        private string apiPassword;
        private string apiCloseURL;
        private string apiConfirmURL;
        private string result_post;

        // Binds Ticket Number String
        private string ticketNumber;
        public string TicketNumber
        {
            get { return ticketNumber; }
            set
            {
                if (ticketNumber != value)
                {
                    ticketNumber = value;
                    var fixString = Regex.Replace(ticketNumber, @"\s+", "");
                    ticketNumber = fixString;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Object Collection of State Type
        private readonly ObservableCollection<ComboBoxListItem> stateTypes = new()
        {
            new ComboBoxListItem { BoxItem = "Closed" },
            new ComboBoxListItem { BoxItem = "Provision Manually" },
            new ComboBoxListItem { BoxItem = "Reject" }
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

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
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

        // Binds Dev Settings Label String
        private string devSettingsString;
        public string DevSettingsString
        {
            get { return devSettingsString; }
            set
            {
                if (devSettingsString != value)
                {
                    devSettingsString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Comments Box String
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

        // Binds Bool Value of a Found Ticket
        private bool foundOpenTicket;
        public bool FoundOpenTicket
        {
            get { return foundOpenTicket; }
            set
            {
                if (foundOpenTicket != value)
                {
                    foundOpenTicket = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Bool Value of Update Comments Field
        private bool commentsFieldEnabled;
        public bool CommentsFieldEnabled
        {
            get { return commentsFieldEnabled; }
            set
            {
                if (commentsFieldEnabled != value)
                {
                    commentsFieldEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Bool Value of Development Testing Button
        private bool devTestingTurnedOn;
        public bool DevTestingTurnedOn
        {
            get { return devTestingTurnedOn; }
            set
            {
                if (devTestingTurnedOn != value)
                {
                    devTestingTurnedOn = value;
                    OnPropertyChanged();
                    if (DevTestingTurnedOn == true && QATestingTurnedOn == true)
                    {
                        QATestingTurnedOn = false;
                    }
                    SetEnvironmentConfiguration();
                }
            }
        }

        // Binds Bool Value of QA Testing Button
        private bool qATestingTurnedOn;
        public bool QATestingTurnedOn
        {
            get { return qATestingTurnedOn; }
            set
            {
                if (qATestingTurnedOn != value)
                {
                    qATestingTurnedOn = value;
                    OnPropertyChanged();
                    if (DevTestingTurnedOn == true && QATestingTurnedOn == true)
                    {
                        DevTestingTurnedOn = false;
                    }
                    SetEnvironmentConfiguration();
                }
            }
        }

        // Binds Bool Value of Development Testing Authorization
        private bool devTestingEnabled;
        public bool DevTestingEnabled
        {
            get { return devTestingEnabled; }
            set
            {
                if (devTestingEnabled != value)
                {
                    devTestingEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public struct TicketCheckValues
        {
            public bool ticketExists;
            public string status;
            public string requestId;
            public string requestSummary;
            public string requestedOn;
            public string fulfillmentStatus;
            public string approverName1;
            public string approverStatus1;
            public string approverName2;
            public string approverStatus2;
            public string requestsubmissionid;
            public string mailSentOn;
            public string mailSentTo;
            public List<String> reqestIdChildren;
            public List<AccessNowRelatedTicketsModel> relatedValues;
        }

        // Binds Current Related Tickets Model List
        private List<AccessNowRelatedTicketsModel> trackedValue;
        public List<AccessNowRelatedTicketsModel> TrackedValue
        {
            get { return trackedValue; }
            set
            {
                if (trackedValue != value)
                {
                    trackedValue = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Request Id Label String
        private string requestIdString;
        public string RequestIdString
        {
            get { return requestIdString; }
            set
            {
                if (requestIdString != value)
                {
                    requestIdString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Request Summary Label String
        private string requestSummaryString;
        public string RequestSummaryString
        {
            get { return requestSummaryString; }
            set
            {
                if (requestSummaryString != value)
                {
                    requestSummaryString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Requested On Label String
        private string requestedOnString;
        public string RequestedOnString
        {
            get { return requestedOnString; }
            set
            {
                if (requestedOnString != value)
                {
                    requestedOnString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Fulfillment Status Label String
        private string fullfillmentStatusString;
        public string FullfillmentStatusString
        {
            get { return fullfillmentStatusString; }
            set
            {
                if (fullfillmentStatusString != value)
                {
                    fullfillmentStatusString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Approver 1 Name Label String
        private string approverName1String;
        public string ApproverName1String
        {
            get { return approverName1String; }
            set
            {
                if (approverName1String != value)
                {
                    approverName1String = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Approver 1 Status Label String
        private string approverStatus1String;
        public string ApproverStatus1String
        {
            get { return approverStatus1String; }
            set
            {
                if (approverName1String != value)
                {
                    approverStatus1String = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Approver 2 Name Label String
        private string approverName2String;
        public string ApproverName2String
        {
            get { return approverName2String; }
            set
            {
                if (approverName2String != value)
                {
                    approverName2String = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Approver 2 Status Label String
        private string approverStatus2String;
        public string ApproverStatus2String
        {
            get { return approverStatus2String; }
            set
            {
                if (approverName2String != value)
                {
                    approverStatus2String = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Request Submission ID Label String
        private string requestSubmissionIdString;
        public string RequestSubmissionIdString
        {
            get { return requestSubmissionIdString; }
            set
            {
                if (requestSubmissionIdString != value)
                {
                    requestSubmissionIdString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Mail Sent On Label String
        private string mailSentOnString;
        public string MailSentOnString
        {
            get { return mailSentOnString; }
            set
            {
                if (mailSentOnString != value)
                {
                    mailSentOnString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Mail Sent TO Label String
        private string mailSentToString;
        public string MailSentToString
        {
            get { return mailSentToString; }
            set
            {
                if (mailSentToString != value)
                {
                    mailSentToString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Request Submission ID to a List
        private List<string> requestSubmissionChildrenList;
        public List<string> RequestSubmissionChildrenList
        {
            get { return requestSubmissionChildrenList; }
            set
            {
                if (requestSubmissionChildrenList != value)
                {
                    requestSubmissionChildrenList = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Request Submission Built List TextBox String
        private string requestSubmissionBuiltList;
        public string RequestSubmissionBuiltList
        {
            get { return requestSubmissionBuiltList; }
            set
            {
                if (requestSubmissionBuiltList != value)
                {
                    requestSubmissionBuiltList = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        public AccessNowCloseTicketViewModel()
        {
            //Initialize Properties
            apiUsername = ConfigurationProperties.AccessNowClosedAPIUsername;
            apiPassword = ConfigurationProperties.AccessNowClosedAPIPassword;
            apiConfirmURL = ConfigurationProperties.AccessNowClosedAPICONFIRMURL;
            apiCloseURL = ConfigurationProperties.AccessNowClosedAPICLOSEURL;
            result_post = "";
            FoundOpenTicket = false;
            QATestingTurnedOn = false;
            DevTestingTurnedOn = false;
            CommentsFieldEnabled = false;
            StatusString = "";
            DevTestingEnabled = App.GlobalUserInfo.IsDev;
            RequestIdString = "";
            ApproverName1String = "";
            ApproverStatus1String = "";
            ApproverName2String = "";
            ApproverStatus2String = "";
            RequestSummaryString = "";
            RequestedOnString = "";
            FullfillmentStatusString = "";
            RequestSubmissionIdString = "";
            MailSentOnString = "";
            MailSentToString = "";
            RequestSubmissionChildrenList = new List<string>();
            RequestSubmissionBuiltList = "";
            TrackedValue = new();

            IsLoadBool = false;

            //Initialize Relay Commands
            GetTicketCommand = new RelayCommand(o => { GetTicket(); });
            CloseTicketCommand = new RelayCommand(o => { CloseTicket(); });
            CloseAllTicketsCommand = new RelayCommand(o => { CloseAllRelatedTickets(); });
            UpdateCommentsCommand = new RelayCommand(o => { UpdateComments(); });
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            try
            {
                SetEnvironmentConfiguration();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // On Dev or QA Button Press Changes Environment Configuration
        private void SetEnvironmentConfiguration()
        {
            // Set Development, QA,  or Production Ticket Environment
            if (DevTestingTurnedOn == true)
            {
                apiUsername = ConfigurationProperties.AccessNowClosedAPITestUsername;
                apiPassword = ConfigurationProperties.AccessNowClosedAPITestPassword;
                apiCloseURL = ConfigurationProperties.AccessNowClosedAPICLOSEURL;
                apiConfirmURL = ConfigurationProperties.AccessNowClosedAPICONFIRMURL;
            }
            else if (QATestingTurnedOn == true)
            {
                apiUsername = ConfigurationProperties.AccessNowClosedAPIQAUsername;
                apiPassword = ConfigurationProperties.AccessNowClosedAPIQAPassword;
                apiCloseURL = ConfigurationProperties.AccessNowClosedAPIQACLOSEURL;
                apiConfirmURL = ConfigurationProperties.AccessNowClosedAPIQACONFIRMURL;
            }
            else
            {
                apiUsername = ConfigurationProperties.AccessNowClosedAPIUsername;
                apiPassword = ConfigurationProperties.AccessNowClosedAPIPassword;
                apiCloseURL = ConfigurationProperties.AccessNowClosedAPICLOSEURL;
                apiConfirmURL = ConfigurationProperties.AccessNowClosedAPICONFIRMURL;
            }

            DevSettingsString = apiUsername + " | " + Environment.NewLine + apiPassword + " | " + Environment.NewLine + apiConfirmURL;
        }

        // Lookup if a Ticket Number Exists and is Open
        public async void GetTicket()
        {
            try
            {
                SetEnvironmentConfiguration();
                string uriName = apiConfirmURL;
                TrackedValue.Clear();
                TicketCheckValues passedValues = new()
                {
                    status = "",
                    requestId = "",
                    requestedOn = "",
                    requestSummary = "",
                    approverName1 = "",
                    approverStatus1 = "",
                    approverName2 = "",
                    approverStatus2 = "",
                    fulfillmentStatus = "",
                    requestsubmissionid = "",
                    mailSentOn = "",
                    mailSentTo = "",
                    reqestIdChildren = new List<string>(),
                    ticketExists = false,
                    relatedValues = new()
                };

                IsLoadBool = true;

                passedValues = await TicketURLExists(uriName);

                DevSettingsString = apiUsername + " | " + Environment.NewLine + apiPassword + " | " + Environment.NewLine + apiConfirmURL + " | " + Environment.NewLine + uriName;

                // Switch on if found , and if closed or open
                if (passedValues.ticketExists == true)
                {
                    string buildList = "Request Id        |  Fulfillment Status" + Environment.NewLine;
                    foreach (AccessNowRelatedTicketsModel related in passedValues.relatedValues)
                    {
                        if (related != null)
                        {
                            buildList = buildList + related.RequestSubmissionId + "  |  " + related.RequestSubmissionFulfillmentStatus + Environment.NewLine;
                        }
                    }

                    // Need find Closed Criteria from AccessNow
                    if (passedValues.status == "fulfilled" || passedValues.status == "markformanualfulfillment" || passedValues.status == "rejectedduringfulfillment")
                    {
                        FoundOpenTicket = false;
                        CommentsFieldEnabled = true;
                        AddCommentsString = "";
                        GetComments();
                        string modifier = "";
                        if (passedValues.status == "fulfilled") { modifier = " as Fulfilled."; };
                        if (passedValues.status == "markformanualfulfillment") { modifier = " as Marked for Manual Fulfllment."; };
                        if (passedValues.status == "rejectedduringfulfillment") { modifier = " as Rejected During Fulfillment."; };
                        StatusString = "Ticket is Closed" + modifier;
                        RequestIdString = passedValues.requestId;
                        RequestSummaryString = passedValues.requestSummary;
                        RequestedOnString = passedValues.requestedOn;
                        FullfillmentStatusString = passedValues.fulfillmentStatus;
                        ApproverName1String = passedValues.approverName1;
                        ApproverStatus1String = passedValues.approverStatus1;
                        ApproverName2String = passedValues.approverName2;
                        ApproverStatus2String = passedValues.approverStatus2;
                        RequestSubmissionIdString = passedValues.requestsubmissionid;
                        MailSentToString = passedValues.mailSentTo;
                        MailSentOnString = passedValues.mailSentOn;
                        RequestSubmissionChildrenList = passedValues.reqestIdChildren;
                        RequestSubmissionBuiltList = buildList;
                    }
                    else
                    {
                        FoundOpenTicket = true;
                        CommentsFieldEnabled = true;
                        AddCommentsString = "";
                        GetComments();
                        StatusString = "Open Ticket Found";
                        RequestIdString = passedValues.requestId;
                        RequestSummaryString = passedValues.requestSummary;
                        RequestedOnString = passedValues.requestedOn;
                        FullfillmentStatusString = passedValues.fulfillmentStatus;
                        ApproverName1String = passedValues.approverName1;
                        ApproverStatus1String = passedValues.approverStatus1;
                        ApproverName2String = passedValues.approverName2;
                        ApproverStatus2String = passedValues.approverStatus2;
                        RequestSubmissionIdString = passedValues.requestsubmissionid;
                        MailSentToString = passedValues.mailSentTo;
                        MailSentOnString = passedValues.mailSentOn;
                        RequestSubmissionChildrenList = passedValues.reqestIdChildren;
                        RequestSubmissionBuiltList = buildList;
                    }
                }
                else
                {
                    FoundOpenTicket = false;
                    CommentsFieldEnabled = false;
                    AddCommentsString = "";
                    StatusString = "Ticket Not Found";
                }

                IsLoadBool = false;

            }
            catch (Exception ex)
            {
                StatusString = "Ticket Not Found";


                MessageBox.Show(ex.ToString());
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Checks if Ticket URL Exists
        private async Task<TicketCheckValues> TicketURLExists(string url)
        {
            TicketCheckValues currentValues = new()
            {
                status = "",
                requestId = "",
                requestedOn = "",
                requestSummary = "",
                approverName1 = "",
                approverStatus1 = "",
                approverName2 = "",
                approverStatus2 = "",
                fulfillmentStatus = "",
                requestsubmissionid = "",
                mailSentOn = "",
                mailSentTo = "",
                reqestIdChildren = new List<string>(),
                ticketExists = false
            };

            bool urlCheck = false;
            var uri = new Uri(url);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("{}");
            string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(apiUsername + ":" + apiPassword));

            string json = new JavaScriptSerializer().Serialize(new
            {
                externalId = TicketNumber,
                paginationStartIndex = "1",
                numberOfRecords = "1"
            });

            try
            {
                // Define the API Request Object
                WebRequest requestObj = (HttpWebRequest)WebRequest.Create(uri);
                {
                    requestObj.Method = "POST";
                    requestObj.ContentType = "application/json";
                    requestObj.PreAuthenticate = true;
                    requestObj.Proxy = new WebProxy();
                    requestObj.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    requestObj.ContentLength = json.Length;
                    requestObj.Headers.Add("authorization", "Basic " + encoded);
                };

                using (Stream stream = requestObj.GetRequestStream())
                {
                    StreamWriter sw = new StreamWriter(stream);
                    sw.Write(json);
                    sw.Flush();
                    stream.Close();
                }

                HttpWebResponse responseObjGet = null;
                responseObjGet = (HttpWebResponse)requestObj.GetResponse();
                var statusCode = responseObjGet.StatusCode;

                // Response Object Returns Status Code, as Well as Status of Request 
                using (Stream stream = responseObjGet.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);

                    string resultTest = sr.ReadToEnd();
                    string status = "";
                    string requestId = "";
                    string requestedOn = "";
                    string requestSummary = "";
                    string fulfillmentStatus = "";
                    string approverName1 = "";
                    string approvalStatus1 = "";
                    string approverName2 = "";
                    string approvalStatus2 = "";
                    string requestsubmissionid = "";
                    string mailSentTo = "";
                    string mailSentOn = "";

                    if (resultTest == "[]") { urlCheck = false; }
                    else { urlCheck = true; }

                    if (urlCheck == true)
                    {
                        resultTest = resultTest.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                        var parsedJson = JObject.Parse(resultTest);
                        var token = (string)parsedJson["status"];
                        var requestIdToken = (string)parsedJson["externalId"];
                        var requestedOnToken = (string)parsedJson["requestedOn"];
                        var fulfillmentStatusToken = (string)parsedJson["fulfilledOn"];
                        var requestSummaryToken = (string)parsedJson["requestType"];
                        var requestsubmissionidToken = (string)parsedJson["requestSubmissionId"];

                        status = token;
                        requestId = requestIdToken;
                        requestedOn = requestedOnToken;
                        fulfillmentStatus = fulfillmentStatusToken;
                        requestSummary = requestSummaryToken;
                        requestsubmissionid = requestsubmissionidToken;

                        


                        var additionalDataSummary = parsedJson["additionalData"];
                        if (additionalDataSummary != null && additionalDataSummary.ToString().Contains("fulfillmentData"))
                        {
                            var fullfillmentDataSummary = additionalDataSummary["fulfillmentData"];
                            if (fullfillmentDataSummary != null)
                            {
                                var mailSentToToken = (string)fullfillmentDataSummary["Mail_Sent_To"];
                                if (mailSentToToken != null) { mailSentTo = mailSentToToken; }
                                currentValues.mailSentTo = mailSentTo;

                                var mailSentOnToken = (string)fullfillmentDataSummary["Mail_Sent_On"];
                                if (mailSentOnToken != null) { mailSentOn = mailSentOnToken; }
                                currentValues.mailSentOn = mailSentOn;
                            }
                        }

                        var actionSummaryToken = parsedJson["actionSummary"];
                        if (actionSummaryToken != null)
                        {
                            var approve1 = actionSummaryToken[0]["1"];
                            if (approve1 != null)
                            {
                                var approve1StatusToken = (string)approve1[0]["status"];
                                if (approve1StatusToken != null) { approvalStatus1 = approve1StatusToken; }
                                currentValues.approverStatus1 = approvalStatus1;
                            }

                            var approve2 = actionSummaryToken[0]["2"];
                            if (approve2 != null)
                            {
                                var approve2StatusToken = (string)approve2[0]["status"];
                                if (approve2StatusToken != null) { approvalStatus2 = approve2StatusToken; }
                                currentValues.approverStatus2 = approvalStatus2;
                            }
                        }

                        var approversToken = parsedJson["approvers"];
                        if (approversToken != null)
                        {
                            var approve1 = approversToken["1"];
                            if (approve1 != null)
                            {
                                var approve1NameToken = (string)approve1[0]["approverName"];
                                if (approve1NameToken != null) { approverName1 = approve1NameToken; }
                                currentValues.approverName1 = approverName1;
                            }

                            var approve2 = approversToken["2"];
                            if (approve2 != null)
                            {
                                var approve2NameToken = (string)approve2[0]["approverName"];
                                if (approve2NameToken != null) { approverName2 = approve2NameToken; }
                                currentValues.approverName2 = approverName2;
                            }
                        }
                    }

                    currentValues.requestsubmissionid = requestsubmissionid;
                    currentValues.ticketExists = urlCheck;
                    currentValues.status = status;
                    currentValues.requestId = requestId;
                    currentValues.requestedOn = requestedOn;
                    currentValues.requestSummary = requestSummary;
                    currentValues.fulfillmentStatus = fulfillmentStatus;
                    currentValues.requestsubmissionid = requestsubmissionid;

                    if (currentValues.requestsubmissionid != null && currentValues.requestsubmissionid != "")
                    {
                        currentValues = await Task.Run(() => GetRelatedRequestId(currentValues, url));
                    }
                    sr.Close();
                }

                return currentValues;
            }
            catch (WebException ex)
            {
                string response = "";
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
                ExceptionOutput.Output(ex.ToString());
                return currentValues;
            }
        }

        // Finds Any Associated Related Request ID's to the Passed Request
        private async Task<TicketCheckValues> GetRelatedRequestId(TicketCheckValues passedValues, string url)
        {
            bool urlCheck = false;
            var uri = new Uri(url);
            byte[] data = System.Text.Encoding.UTF8.GetBytes("{}");
            string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(apiUsername + ":" + apiPassword));

            string json = new JavaScriptSerializer().Serialize(new
            {

                requestSubmissionId = passedValues.requestsubmissionid,
                paginationStartIndex = "1",
                numberOfRecords = "100"
            });

            try
            {
                await Task.Run(() => {

                    // Define the API Request Object
                    WebRequest requestObj = (HttpWebRequest)WebRequest.Create(uri);
                    {
                        requestObj.Method = "POST";
                        requestObj.ContentType = "application/json";
                        requestObj.PreAuthenticate = true;
                        requestObj.Proxy = new WebProxy();
                        requestObj.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        requestObj.ContentLength = json.Length;
                        requestObj.Headers.Add("authorization", "Basic " + encoded);
                    };

                    using (Stream stream = requestObj.GetRequestStream())
                    {
                        StreamWriter sw = new StreamWriter(stream);
                        sw.Write(json);
                        sw.Flush();
                        stream.Close();
                    }

                    HttpWebResponse responseObjGet = null;
                    responseObjGet = (HttpWebResponse)requestObj.GetResponse();
                    var statusCode = responseObjGet.StatusCode;

                    // Response Object Returns Status Code, as Well as Status of Request 
                    using (Stream stream = responseObjGet.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream);
                        string resultTest = sr.ReadToEnd();

                        List<string> requestSubmissionIds = new List<string>();
                        List<AccessNowRelatedTicketsModel> relatedTicketsModel = new();


                        if (resultTest == "[]") { urlCheck = false; }
                        else { urlCheck = true; }

                        if (urlCheck == true)
                        {

                            dynamic[] dataArray = JsonConvert.DeserializeObject<dynamic[]>(resultTest);

                            foreach (dynamic returnData in dataArray)
                            {
                                string requestSubmissionId = returnData.externalId;
                                string requestSubmissionFulfillmentStatus = returnData.status;
                                requestSubmissionIds.Add(requestSubmissionId);

                                relatedTicketsModel.Add
                                (
                                    new AccessNowRelatedTicketsModel
                                    {
                                        RequestSubmissionFulfillmentStatus = requestSubmissionFulfillmentStatus,
                                        RequestSubmissionId = requestSubmissionId
                                    }
                                );

                            }
                            sr.Close();
                        }

                        passedValues.reqestIdChildren = requestSubmissionIds;
                        passedValues.relatedValues = relatedTicketsModel;
                        TrackedValue = relatedTicketsModel;
                    }
                });

                return passedValues;
            }
            catch (WebException ex)
            {
                string response = "";
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    response = reader.ReadToEnd();
                }
                ExceptionOutput.Output(ex.ToString());
                return passedValues;
            }
        }

        // If Record Exists Fill Comments Box
        private void GetComments()
        {
            DataTable tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.AccessNowClosedTickets, "TicketNumber", TicketNumber);
            if (tempTable.Rows.Count > 0)
            {
                string comments = tempTable.Rows[0]["Comments"].ToString();
                AddCommentsString = comments;
            }
        }

        // Update Comments Box Only
        private void UpdateComments()
        {
            DataTable tempTable = new();
            tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.AccessNowClosedTickets, "TicketNumber", TicketNumber);
            if (tempTable.Rows.Count > 0)
            {
                DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "Comments", AddCommentsString);
            }
            else
            {
                UserPrincipal userPrincipal = UserPrincipal.Current;
                DateTime createDate = DateTime.UtcNow;
                AccessNowClosedTicketItemModel passedTicket = new()
                {
                    TicketNumber = TicketNumber,
                    FinishedState = SelectedState.BoxItem,
                    Comments = AddCommentsString,
                    ClosedBy = userPrincipal.Name,
                    DateClosed = "Ticket Not Closed."
                };

                DBConn.AddAccessNowCompletedTicket(passedTicket, ConfigurationProperties.AccessNowClosedTickets);
                StatusString = "Created Record";
            }

            string comments = AddCommentsString;
            StatusString = "Comments Updated";
        }

        // Close Ticket with Finished State Type
        public async void CloseTicket()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to update this ticket?", "Update Ticket?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    UserPrincipal userPrincipal = UserPrincipal.Current;
                    DateTime createDate = DateTime.UtcNow;
                    AccessNowClosedTicketItemModel passedTicket = new()
                    {
                        TicketNumber = TicketNumber,
                        FinishedState = SelectedState.BoxItem,
                        Comments = AddCommentsString,
                        ClosedBy = userPrincipal.Name,
                        DateClosed = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
                    };

                    // Set Fulfillment type to Post Request
                    switch (SelectedState.BoxItem)
                    {
                        case "Closed":
                            passedTicket.FinishedState = "fulfilled";
                            break;

                        case "Provision Manually":
                            passedTicket.FinishedState = "markformanualfulfillment";
                            break;

                        case "Reject":
                            passedTicket.FinishedState = "rejectduringfulfillment";
                            break;

                        default:
                            break;
                    }

                    result_post = await SendRequest(passedTicket);
                    if (result_post == "Successfully Closed")
                    {
                        DataTable tempTable = new();
                        tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.AccessNowClosedTickets, "TicketNumber", TicketNumber);
                        if (tempTable.Rows.Count > 0)
                        {
                            DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "FinishedState", passedTicket.FinishedState);
                            DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "Comments", passedTicket.Comments);
                            DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "ClosedBy", passedTicket.ClosedBy);
                            DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "DateClosed", passedTicket.DateClosed);
                        }
                        else
                        {
                            DBConn.AddAccessNowCompletedTicket(passedTicket, ConfigurationProperties.AccessNowClosedTickets);
                        }
                        StatusString = result_post;
                    }
                    else
                    {
                        StatusString = "Error Closing Request";
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Close All Related Requests on a Ticket
        public async void CloseAllRelatedTickets()
        {
            try
            {
                int successCount = 0;
                int failureCount = 0;
                int openRelatedTickets = 0;
                foreach (AccessNowRelatedTicketsModel related in TrackedValue)
                {
                    if (related.RequestSubmissionFulfillmentStatus != "fulfilled" &&
                        related.RequestSubmissionFulfillmentStatus != "rejectduringfulfillment")
                        openRelatedTickets++;
                }

                if (MessageBox.Show("Are you certain you want to update each of these " + openRelatedTickets.ToString() + " tickets?", "Update Tickets?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    IsLoadBool = true;

                    UserPrincipal userPrincipal = UserPrincipal.Current;
                    DateTime createDate = DateTime.UtcNow;

                    foreach (AccessNowRelatedTicketsModel related in TrackedValue)
                    {
                        AccessNowClosedTicketItemModel passedTicket = new()
                        {
                            TicketNumber = related.RequestSubmissionId,
                            FinishedState = SelectedState.BoxItem,
                            Comments = AddCommentsString,
                            ClosedBy = userPrincipal.Name,
                            DateClosed = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff")
                        };

                        // Set Fulfillment type to Post Request
                        switch (SelectedState.BoxItem)
                        {
                            case "Closed":
                                passedTicket.FinishedState = "fulfilled";
                                break;

                            case "Provision Manually":
                                passedTicket.FinishedState = "markformanualfulfillment";
                                break;

                            case "Reject":
                                passedTicket.FinishedState = "rejectduringfulfillment";
                                break;

                            default:
                                break;
                        }

                        if (related.RequestSubmissionFulfillmentStatus != "fulfilled" &&
                            related.RequestSubmissionFulfillmentStatus != "rejectduringfulfillment")
                        {
                            result_post = await SendRequest(passedTicket);
                        }

                        if (result_post == "Successfully Closed")
                        {
                            DataTable tempTable = new();
                            tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.AccessNowClosedTickets, "TicketNumber", TicketNumber);
                            if (tempTable.Rows.Count > 0)
                            {
                                DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "FinishedState", passedTicket.FinishedState);
                                DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "Comments", passedTicket.Comments);
                                DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "ClosedBy", passedTicket.ClosedBy);
                                DBConn.UpdateTableRecord(ConfigurationProperties.AccessNowClosedTickets, TicketNumber, "TicketNumber", "DateClosed", passedTicket.DateClosed);
                            }
                            else
                            {
                                DBConn.AddAccessNowCompletedTicket(passedTicket, ConfigurationProperties.AccessNowClosedTickets);
                            }
                            successCount++;
                        }
                        else if (related.RequestSubmissionFulfillmentStatus != "fulfilled" &&
                                related.RequestSubmissionFulfillmentStatus != "rejectduringfulfillment")
                        {
                            failureCount++;
                        }
                    }

                    IsLoadBool = false;
                    StatusString = "Succesfully Closed " + successCount.ToString() + " out of " + openRelatedTickets + " Requests. " + failureCount.ToString() + " Requests had errors.";
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Send Request 
        public async Task<string> SendRequest(AccessNowClosedTicketItemModel passedTicket)
        {
            try
            {
                string apiURL = apiCloseURL + passedTicket.TicketNumber + "/";
                string postURL = apiURL + passedTicket.FinishedState;
                var uri = new Uri(postURL);
                byte[] data = System.Text.Encoding.UTF8.GetBytes("{}");
                string encoded = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(apiUsername + ":" + apiPassword));

                try
                {
                    // Define the API Request Object
                    WebRequest requestObj = (HttpWebRequest)WebRequest.Create(uri);
                    {
                        requestObj.Method = "POST";
                        requestObj.ContentType = "application/x-www-form-urlencoded";
                        requestObj.PreAuthenticate = true;
                        requestObj.Proxy = new WebProxy();
                        requestObj.Proxy.Credentials = CredentialCache.DefaultCredentials;
                        requestObj.ContentLength = data.Length;
                        requestObj.Headers.Add("authorization", "Basic " + encoded);
                    };

                    using (Stream stream = requestObj.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Close();
                    }

                    HttpWebResponse responseObjGet = null;
                    responseObjGet = (HttpWebResponse)requestObj.GetResponse();
                    var statusCode = responseObjGet.StatusCode;

                    // Get Response from Post Request
                    using (Stream stream = responseObjGet.GetResponseStream())
                    {
                        StreamReader sr = new StreamReader(stream);

                        string resultTest = sr.ReadToEnd();
                        sr.Close();
                        string finalString = "";

                        if (resultTest == "{}") { finalString = "Successfully Closed"; }
                        else
                        {
                            resultTest = resultTest.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

                            var parsedJson = JObject.Parse(resultTest);
                            var token = (string)parsedJson["errorCode"];

                            finalString = token;
                        }

                        return finalString;
                    }

                }
                catch (WebException WebEx)
                {
                    if (WebEx.Message == "The operation has timed out") { return "The operation has timed out"; }
                    else
                    {
                        string resp = new System.IO.StreamReader(WebEx.Response.GetResponseStream()).ReadToEnd();
                        int StartFromHere = resp.IndexOf((char)34 + "errorDescription" + (char)34 + ":" + (char)34);
                        int EndHere = resp.IndexOf((char)34 + ",", StartFromHere);
                        string ErrorIs = resp.Substring(StartFromHere + 20, EndHere - StartFromHere - 20).Trim();

                        return ErrorIs;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                    return "";
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return "";
            }
        }
        #endregion
    }
}
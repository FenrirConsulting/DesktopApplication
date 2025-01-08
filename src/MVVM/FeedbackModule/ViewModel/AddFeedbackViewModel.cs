using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class AddFeedbackViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        //Relay Commands
        public RelayCommand ClearRequest { get; set; }
        public RelayCommand ClearUpdate { get; set; }
        public RelayCommand AddFeedbackButton { get; set; }
        #endregion

        #region Properties
        // Properties of Record Addition 
        private string requestID;
        public string RequestID
        {
            get { return requestID; }
            set { if (requestID != value) { requestID = value; OnPropertyChanged(); } }
        }

        // For Settings next Record #
        public List<double> recordsList = new();

        private string requestor;
        public string Requestor
        {
            get { return requestor; }
            set { if (requestor != value) { requestor = value; OnPropertyChanged(); } }
        }

        private string requestType;
        public string RequestType
        {
            get { return requestType; }
            set { if (requestType != value) { requestType = value; OnPropertyChanged(); } }
        }

        private string requestDate;
        public string RequestDate
        {
            get { return requestDate; }
            set { if (requestDate != value) { requestDate = value; OnPropertyChanged(); } }
        }

        private string updateDate;
        public string UpdateDate
        {
            get { return updateDate; }
            set { if (updateDate != value) { updateDate = value; OnPropertyChanged(); } }
        }

        private string request;
        public string Request
        {
            get { return request; }
            set { if (request != value) { request = value; OnPropertyChanged(); } }
        }

        private string requestUpdate;
        public string RequestUpdate
        {
            get { return requestUpdate; }
            set { if (requestUpdate != value) { requestUpdate = value; OnPropertyChanged(); } }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> searchTerms = new()
        {
            new ComboBoxListItem { BoxItem = "Feature" },
            new ComboBoxListItem { BoxItem = "Bug" },
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

        private readonly Action ParentAction;
        #endregion

        #region Methods
        public AddFeedbackViewModel()
        {

        }

        public AddFeedbackViewModel(Action parentAction)
        {
            // Initialize Properties
            this.ParentAction = parentAction;
            Request = "Enter Feedback Here";
            RequestUpdate = "Additional Notes Here";
            ClearRequest = new RelayCommand(o => { ClearRequestMethod(); });
            ClearUpdate = new RelayCommand(o => { ClearUpdateMethod(); });
            AddFeedbackButton = new RelayCommand(o => { AddRecord(); });
            DateTime date = DateTime.Now.Date;
            RequestDate = date.ToShortDateString();
            UpdateDate = RequestDate;
            Requestor = App.GlobalUserInfo.DisplayName;
            RequestID = "0";
        }
        #endregion

        #region Functions
        // Execute BuildTables on FeedbackView
        public void CallParentAction()
        {
            ParentAction.Invoke();
        }

        // Clear Request Box on Focus
        public void ClearRequestMethod()
        {
            if (Request == "Enter Feedback Here")
            {
                Request = "";
            }
        }

        // Clear Update Box on Focus
        public void ClearUpdateMethod()
        {
            if (RequestUpdate == "Additional Notes Here")
            {
                RequestUpdate = "";
            }
        }

        // On load determines next Record #, and initializes ComboBox Item
        public void OnLoad()
        {
            recordsList = DBConn.GetColumnListDouble("IAMHFeedback", "iamf_reqid");
            if (recordsList.Count > 0)
            {
                var max = recordsList.Max();
                max++;
                RequestID = max.ToString();
            }
            else
            {
                RequestID = "1";
            }
            SelectedTerm = SearchTerms.ElementAt(0);
        }

        public void AddRecord()
        {
            // Validates and then Adds Record using Record Properties
            if (MessageBox.Show("Are you certain you want to Submit this Feedback?", "Submit Feedback?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string iamf_requeststatus = "Open";
                double requestPass = Int64.Parse(RequestID);
                string typePassed = SelectedTerm.BoxItem.ToString();
                if (RequestUpdate == "Additional Notes Here") { RequestUpdate = ""; }
                string passedUpdate = Environment.NewLine + RequestUpdate + Environment.NewLine + Requestor + " left Feedback on " + Environment.NewLine + RequestDate + Environment.NewLine;
                DBConn.AddFeedbackRecord(requestPass, Requestor, typePassed, RequestDate, UpdateDate, Request, passedUpdate, iamf_requeststatus);
                CallParentAction();
            }
            else
            {
                //Do Nothing
            }
        }
        #endregion
    }
}

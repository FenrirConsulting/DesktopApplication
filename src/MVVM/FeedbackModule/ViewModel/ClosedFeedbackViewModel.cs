using IAMHeimdall.Core;
using System;
using System.Data;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ClosedFeedbackViewModel : BaseViewModel
    {
        #region Delegates
        // Relay Commands
        public RelayCommand OpenButtonCommand { get; set; }
        public RelayCommand DeleteButtonCommand { get; set; }

        // SQL Methods Class
        public SQLMethods DBConn = new();
        #endregion

        #region Properties
        // Properties of Record Data Row 
        private string closedRecord;
        public string ClosedRecord
        {
            get { return closedRecord; }
            set { if (closedRecord != value) { closedRecord = value; OnPropertyChanged(); } }
        }

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

        private string selectedRecord;
        public string SelectedRecord
        {
            get { return selectedRecord; }
            set { if (selectedRecord != value) { selectedRecord = value; OnPropertyChanged(); } }
        }

        private string newUpdateDate;
        public string NewUpdateDate
        {
            get { return newUpdateDate; }
            set { if (newUpdateDate != value) { newUpdateDate = value; OnPropertyChanged(); } }
        }

        private bool closedEnabled;
        public bool ClosedEnabled
        {
            get { return closedEnabled; }
            set { if (closedEnabled != value) { closedEnabled = value; OnPropertyChanged(); } }
        }

        private readonly Action ParentAction;
        #endregion

        #region Methods
        public ClosedFeedbackViewModel()
        {

        }

        public ClosedFeedbackViewModel(DataRow passedRow, Action parentAction)
        {
            // Initialize Properties
            this.ParentAction = parentAction;
            ClosedEnabled = false;
            if (App.GlobalUserInfo.IsDev == true) { ClosedEnabled = true; }
            SelectedRecord = passedRow["iamf_reqid"].ToString();
            ClosedRecord = "#" + passedRow["iamf_reqid"].ToString();
            RequestType = passedRow["iamf_requesttype"].ToString();
            Requestor = "By: " + passedRow["iamf_requestor"].ToString();
            RequestDate = "On: " + passedRow["iamf_requestdate"].ToString();
            UpdateDate = "Closed On: " + passedRow["iamf_updatedate"].ToString();
            Request = "Request: " + passedRow["iamf_request"].ToString();
            RequestUpdate = passedRow["iamf_requestupdate"].ToString();
            DateTime date = DateTime.Now.Date;
            NewUpdateDate = date.ToShortDateString();

            // Initialize Relay Commands
            DeleteButtonCommand = new RelayCommand(o => { DeleteRecord(); });
            OpenButtonCommand = new RelayCommand(o => { OpenRecord(); });
        }
        #endregion

        #region Functions
        public void OpenRecord()
        {
            if (MessageBox.Show("Are you certain you want to Open this Feedback?", "Open Feedback?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string requestPass = SelectedRecord;
                DBConn.UpdateTableRecord("IAMHFeedback", requestPass, "iamf_reqid", "iamf_requeststatus", "Open");
                DBConn.UpdateTableRecord("IAMHFeedback", requestPass, "iamf_reqid", "iamf_updatedate", NewUpdateDate);
                CallParentAction();
            }
            else
            {
                //Do Nothing
            }
        }

        public void DeleteRecord()
        {
            if (MessageBox.Show("Are you certain you want to Delete this Feedback?", "Delete Feedback?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string requestPass = SelectedRecord;
                DBConn.DeleteMatchingRecords("IAMHFeedback", "iamf_reqid", requestPass);
                CallParentAction();
            }
            else
            {
                //Do Nothing
            }
        }

        // Execute BuildTables on FeedbackView
        public void CallParentAction()
        {
            ParentAction.Invoke();
        }
        #endregion
    }
}

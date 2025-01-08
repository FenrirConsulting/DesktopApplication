using IAMHeimdall.Core;
using System;
using System.Data;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class UpdateFeedbackViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand UpdateButtonCommand { get; set; }
        public RelayCommand DeleteButtonCommand { get; set; }
        public RelayCommand CloseButtonCommand { get; set; }
        public RelayCommand ClearUpdateButton { get; set; }
        #endregion

        #region Properties
        // Properties of Record Data Row 
        private string updateRecord;
        public string UpdateRecord
        {
            get { return updateRecord; }
            set { if (updateRecord != value) { updateRecord = value; OnPropertyChanged(); } }
        }

        private bool closedEnabled;
        public bool ClosedEnabled
        {
            get { return closedEnabled; }
            set { if (closedEnabled != value) { closedEnabled = value; OnPropertyChanged(); } }
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

        private string updateAddition;
        public string UpdateAddition
        {
            get { return updateAddition; }
            set { if (updateAddition != value) { updateAddition = value; OnPropertyChanged(); } }
        }

        private string newUpdateDate;
        public string NewUpdateDate
        {
            get { return newUpdateDate; }
            set { if (newUpdateDate != value) { newUpdateDate = value; OnPropertyChanged(); } }
        }

        // Sends in Method Action from FeedbackView
        private readonly Action ParentAction;
        #endregion

        #region Methods
        public UpdateFeedbackViewModel()
        {

        }

        public UpdateFeedbackViewModel(DataRow passedRow, Action parentAction)
        {

            // Initialize Properties
            this.ParentAction = parentAction;
            ClosedEnabled = false;
            if (App.GlobalUserInfo.IsDev == true) { ClosedEnabled = true; }
            UpdateRecord = passedRow["iamf_reqid"].ToString();
            SelectedRecord = UpdateRecord;
            UpdateRecord = "#" + UpdateRecord;
            RequestType = passedRow["iamf_requesttype"].ToString();
            Requestor = "By: " + passedRow["iamf_requestor"].ToString();
            RequestDate = "On: " + passedRow["iamf_requestdate"].ToString();
            UpdateDate = "Updated: " + passedRow["iamf_updatedate"].ToString();
            Request = "Request: " + passedRow["iamf_request"].ToString();
            RequestUpdate = passedRow["iamf_requestupdate"].ToString();
            UpdateAddition = "Add Updates";
            DateTime date = DateTime.Now.Date;
            NewUpdateDate = date.ToShortDateString();

            // Initialize Relay Commands
            UpdateButtonCommand = new RelayCommand(o => { RecordUpdateMethod(); });
            DeleteButtonCommand = new RelayCommand(o => { DeleteRecord(); });
            CloseButtonCommand = new RelayCommand(o => { CloseRecord(); });
            ClearUpdateButton = new RelayCommand(o => { ClearUpdateAddition(); });

        }
        #endregion

        #region Functions
        // Execute BuildTables on FeedbackView
        public void CallParentAction()
        {
            ParentAction.Invoke();

        }

        public void RecordUpdateMethod()
        {
            // Validates and then Adds Record using Record Properties
            if (MessageBox.Show("Are you certain you want to Update this Feedback?", "Update Feedback?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string requestPass = SelectedRecord;
                string finalUpdate;
                if (UpdateAddition == "Additional Notes Here") { finalUpdate = ""; }
                else
                {
                    finalUpdate = Environment.NewLine + UpdateAddition + Environment.NewLine + App.GlobalUserInfo.DisplayName + " left Feedback on " + Environment.NewLine + RequestDate + Environment.NewLine;
                }



                string passedUpdate = RequestUpdate + finalUpdate;
                DBConn.UpdateTableRecord("IAMHFeedback", requestPass, "iamf_reqid", "iamf_requestupdate", passedUpdate);
                DBConn.UpdateTableRecord("IAMHFeedback", requestPass, "iamf_reqid", "iamf_updatedate", NewUpdateDate);
                CallParentAction();
            }
            else
            {
                //Do Nothing
            }
        }

        public void CloseRecord()
        {

            if (MessageBox.Show("Are you certain you want to Close this Feedback?", "Close Feedback?", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string requestPass = SelectedRecord;
                DBConn.UpdateTableRecord("IAMHFeedback", requestPass, "iamf_reqid", "iamf_requeststatus", "Closed");
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

        // Clear Update Box on Focus
        public void ClearUpdateAddition()
        {
            if (UpdateAddition == "Add Updates")
            {
                UpdateAddition = "";
            }
        }
        #endregion
    }
}

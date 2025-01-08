using Centrify.DirectControl.API;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FeedbackViewModel : BaseViewModel
    {
        #region Delegates
        readonly SQLMethods DBConn = new();

        // Relay Commands
        public AddFeedbackViewModel AddFeedbackVm { get; set; }
        public UpdateFeedbackViewModel UpdateFeedbackVm { get; set; }
        public ClosedFeedbackViewModel ClosedFeedbackVm { get; set; }
        public RelayCommand AddViewCommand { get; set; }
        public RelayCommand OpenRecordCommand { get; set; }
        public RelayCommand ClosedRecordCommand { get; set; }
        public RelayCommand TestCommand { get; set; }
        #endregion

        #region Properties
        // Private member for currentUserInfo
        private UserInfo currentUserInfo;

        // Locally accessible public object with user info and group assignments
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // Table Used to Hold Open Feedback Data
        private DataTable fetchTable;
        public DataTable FetchTable
        {
            get { return fetchTable; }
            set { fetchTable = value; OnPropertyChanged(); }
        }

        // Dataview for Open Feedback
        private DataView openTable;
        public DataView OpenTable
        {
            get { return openTable; }
            set { openTable = value; OnPropertyChanged(); }
        }

        // Dataview for Closed Feedback
        private DataView closedTable;
        public DataView ClosedTable
        {
            get { return closedTable; }
            set { closedTable = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value for Open Ticket Record
        private DataGridCellInfo openCellInfo;
        public DataGridCellInfo OpenCellInfo
        {
            get { return openCellInfo; }
            set { openCellInfo = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value for Open Ticket Record
        private DataGridCellInfo closedCellInfo;
        public DataGridCellInfo ClosedCellInfo
        {
            get { return closedCellInfo; }
            set { closedCellInfo = value; OnPropertyChanged(); }
        }

        // Current UserControl View
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public FeedbackViewModel()
        {
            // Initialize Properties
            FetchTable = new DataTable();
            OpenTable = new DataView();
            ClosedTable = new DataView();
            CurrentUserInfo = new UserInfo();

            // Relay Command Definitions
            ClosedRecordCommand = new RelayCommand(o => { ClosedRecord(); });
            OpenRecordCommand = new RelayCommand(o => { OpenRecord(); });
            AddViewCommand = new RelayCommand(o => { AddFeedbackVm = new AddFeedbackViewModel(BuildTablesActionMethodWithViewClear); CurrentView = AddFeedbackVm; });
            TestCommand = new RelayCommand(o => { TestFunction(); });

        }
        #endregion

        #region Functions

        public void OnLoad()
        {
            try
            {
                BuildTables();
                LoadAssemblies.RunAssemblies();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void TestFunction()
        {
            List<string> userGroups = new();
            UserPrincipal userPrincipal = UserPrincipal.Current;
            PrincipalSearchResult<Principal> groups = userPrincipal.GetGroups();
            foreach (Principal p in groups)
            {
                userGroups.Add(p.ToString());
            }


            var currentGroups = string.Join(Environment.NewLine, App.GlobalUserInfo.CurrentUserGroups);
            var userGroupsList = string.Join(Environment.NewLine, userGroups);

            MessageBox.Show(currentGroups + Environment.NewLine + Environment.NewLine + userGroupsList);
        }
        private void OpenRecord()
        {
            try
            {
                if (OpenCellInfo.Item != null)
                {
                    DataRowView drv = (DataRowView)OpenCellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            UpdateFeedbackVm = new UpdateFeedbackViewModel(row, BuildTablesActionMethodWithViewClear);
                            CurrentView = UpdateFeedbackVm;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        private void ClosedRecord()
        {

            try
            {
                if (ClosedCellInfo.Item != null)
                {
                    DataRowView drv = (DataRowView)ClosedCellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            ClosedFeedbackVm = new ClosedFeedbackViewModel(row, BuildTablesActionMethodWithViewClear);
                            CurrentView = ClosedFeedbackVm;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void BuildTablesActionMethod()
        {
            BuildTables();
        }

        public void BuildTablesActionMethodWithViewClear()
        {
            BuildTables();
            CurrentView = null;
        }

        public void BuildTables()
        {
            FetchTable.Clear();
            FetchTable = DBConn.GetTable("IAMHFeedback");

            string openFilter = "iamf_requeststatus = 'Open'";
            string closedFilter = "iamf_requeststatus = 'Closed'";
            string sort = "iamf_requestdate";
            OpenTable = new DataView(FetchTable, openFilter, sort, DataViewRowState.CurrentRows); ;
            ClosedTable = new DataView(FetchTable, closedFilter, sort, DataViewRowState.CurrentRows); ;

        }
        #endregion
    }
}
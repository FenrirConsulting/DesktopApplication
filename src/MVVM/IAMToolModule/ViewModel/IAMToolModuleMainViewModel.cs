using IAMHeimdall.Core;
using SimpleImpersonation;
using System;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolModuleMainViewModel : BaseViewModel
    {
        #region Delegates

        #endregion

        #region Properties
        // Object to move between View Models
        private BaseViewModel _selectedGroupViewModel;
        public BaseViewModel SelectedGroupViewModel
        {
            get { return _selectedGroupViewModel; }
            set { _selectedGroupViewModel = value; OnPropertyChanged(); }
        }

        // Get User Tab View
        private IAMToolGetUserViewModel getUserInfoVm;
        public IAMToolGetUserViewModel GetUserInfoVm
        {
            get { return getUserInfoVm; }
            set { getUserInfoVm = value; OnPropertyChanged(); }
        }

        // Add Group Tab View
        private IAMToolAddGroupViewModel addGroupVm;
        public IAMToolAddGroupViewModel AddGroupVm
        {
            get { return addGroupVm; }
            set { addGroupVm = value; OnPropertyChanged(); }
        }

        // Get Computer Roles Tab View
        private IAMToolGetComputerRolesViewModel getComputerRolesVm;
        public IAMToolGetComputerRolesViewModel GetComputerRolesVm
        {
            get { return getComputerRolesVm; }
            set { getComputerRolesVm = value; OnPropertyChanged(); }
        }

        // Get Group Query Tab View
        private IAMToolGroupQueryViewModel getGroupQueryVm;
        public IAMToolGroupQueryViewModel GetGroupQueryVm
        {
            get { return getGroupQueryVm; }
            set { getGroupQueryVm = value; OnPropertyChanged(); }
        }

        // Get Group Query Tab View
        private IAMToolUpdateModelViewModel updateModelVm;
        public IAMToolUpdateModelViewModel UpdateModelVm
        {
            get { return updateModelVm; }
            set { updateModelVm = value; OnPropertyChanged(); }
        }

        // Object to move between View Models
        private int selectedTab;
        public  int SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; OnPropertyChanged(); }
        }

        #endregion

        #region Methods
        public IAMToolModuleMainViewModel()
        {
            GetUserInfoVm = new();
            AddGroupVm = new();
            GetComputerRolesVm = new();
            GetGroupQueryVm = new();
            UpdateModelVm = new();

            MSG.Messenger.Default.Register<String>(this, TabMessage);
        }
        #endregion

        #region Functions
        public void LoadData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }
        #endregion

        #region Messages
        // Listen to New Record for Database Reload
        public void TabMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                switch (passedMessage)
                {

                    case "Select IAM Tool Tab 1":
                        SelectedTab = 0;
                        break;

                    case "Select IAM Tool Tab 2":
                        SelectedTab = 1;
                        break;

                    case "Select IAM Tool Tab 3":
                        SelectedTab = 2;
                        break;

                    case "Select IAM Tool Tab 4":
                        SelectedTab = 3;
                        break;

                    case "Select IAM Tool Tab 5":
                        SelectedTab = 4;
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}

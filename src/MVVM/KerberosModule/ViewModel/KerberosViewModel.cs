using IAMHeimdall.Core;
using System;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class KerberosViewModel : BaseViewModel
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

        // Users Table View
        private KerberosServersViewModel kerberosServerVm;
        public KerberosServersViewModel KerberosServerVm
        {
            get { return kerberosServerVm; }
            set { kerberosServerVm = value; OnPropertyChanged(); }
        }

        // Users Table View
        private KerberosGroupsViewModel kerberosGroupsVm;
        public KerberosGroupsViewModel KerberosGroupsVm
        {
            get { return kerberosGroupsVm; }
            set { kerberosGroupsVm = value; OnPropertyChanged(); }
        }

        // Users Table View
        private KerberosUsersViewModel kerberosUsersVm;
        public KerberosUsersViewModel KerberosUsersVm
        {
            get { return kerberosUsersVm; }
            set { kerberosUsersVm = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public KerberosViewModel()
        {
            KerberosServerVm = new();
            KerberosUsersVm = new();
            KerberosGroupsVm = new();
        }
        #endregion

        #region Functions
        public void OnLoad()
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
    }
}

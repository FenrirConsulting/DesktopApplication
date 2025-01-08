using System;
using IAMHeimdall.MVVM.ViewModel;
using System.Windows.Input;

namespace IAMHeimdall.Resources.Commands
{
    public class UpdateViewCommand : ICommand
    {
        #region Functions
        #pragma warning disable 67
        private readonly MainViewModel viewModel;
        public UpdateViewCommand(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private readonly UsersTableViewModel usersViewModel;

        public event EventHandler CanExecuteChanged;

        public UpdateViewCommand(UsersTableViewModel usersViewModel)
        {
            this.usersViewModel = usersViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            switch (parameter.ToString())
            {

                case "Feedback":
                    viewModel.SelectedViewModel = new FeedbackViewModel();
                    break;

                case "Service Catalog":
                    viewModel.SelectedViewModel = new ServiceCatalogMainViewModel();
                    break;

                case "Logging":
                    viewModel.SelectedViewModel = new LoggingViewModel();
                    break;

                case "Testing":
                    viewModel.SelectedViewModel = new TestingViewModel();
                    break;

                case "Groups":
                    viewModel.SelectedViewModel = new GroupsTableViewModel();
                    break;

                case "Users":
                    viewModel.SelectedViewModel = new UsersTableViewModel();
                    break;

                case "Users Update":
                    viewModel.SelectedViewModel = new UsersUpdateRecordViewModel();
                    break;

                case "Request Update":
                    viewModel.SelectedViewModel = new RequestUpdateViewModel();
                    break;

                case "Users Add":
                    viewModel.SelectedViewModel = new UsersAddRecordViewModel();
                    break;

                case "Kerberos":
                    viewModel.SelectedViewModel = new KerberosViewModel();
                    break;

                case "Provisioning":
                    viewModel.SelectedViewModel = new IAMToolModuleMainViewModel();
                    break;

                case "Facilitator":
                    viewModel.SelectedViewModel = new FacilitatorMainViewModel();
                    break;

                case "Facilitator Expected":
                    viewModel.SelectedViewModel = new FactoolExpectedTableViewModel();
                    break;

                case "ADLookupTools":
                    viewModel.SelectedViewModel = new ADLookupMainViewModel();
                    break;

                case "Settings":
                    viewModel.SelectedViewModel = new UserConfigurationViewModel();
                    break;
                    
                case "Reports":
                    viewModel.SelectedViewModel = new ReportsViewModel();
                    break;

                case "AccessNow Close Ticket":
                    viewModel.SelectedViewModel = new AccessNowCloseTicketViewModel();
                    break;

                case "AccessNow Reports":
                    viewModel.SelectedViewModel = new AccessNowReportsViewModel();
                    break;

                case "Manual Completion":
                    viewModel.SelectedViewModel = new ManualCompletionViewModel();
                    break;

                default:
                    break;
            }
        }
        #pragma warning restore 67
        #endregion
    }
}

using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FactoolExpectedAddViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand AddRecord { get; set; }
        public RelayCommand CloseButtonCommand { get; set; }
        #endregion

        #region Properties
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // UID Properties Model Class
        public FactoolExpectedModel ExpectedModelInstance { get; set; }

        // For tracking duplicate UID Record Creation
        public List<string> recordsList = new();

        // Logged In Label String
        private string _LoggedInAs;
        public string LoggedInAs
        {
            get { return _LoggedInAs; }
            set { if (_LoggedInAs != value) { _LoggedInAs = value; OnPropertyChanged(); } }
        }

        // Id Label String
        private string idRef;
        public string IdRef
        {
            get { return idRef; }
            set { if (idRef != value) { idRef = value; OnPropertyChanged(); } }
        }

        // ClassName Label String
        private string className;
        public string ClassName
        {
            get { return className; }
            set { if (className != value) { className = value; OnPropertyChanged(); } }
        }

        // ClassOwner Label String
        private string classOwner;
        public string ClassOwner
        {
            get { return classOwner; }
            set { if (classOwner != value) { classOwner = value; OnPropertyChanged(); } }
        }

        // N_Number Label String
        private string n_Number;
        public string N_Number
        {
            get { return n_Number; }
            set { if (n_Number != value) { n_Number = value; OnPropertyChanged(); } }
        }

        // Persona Label String
        private string persona;
        public string PERSONA
        {
            get { return persona; }
            set { if (persona != value) { persona = value; OnPropertyChanged(); } }
        }

        // Expected Users Label String
        private string expectedUsers;
        public string ExpectedUsers
        {
            get { return expectedUsers; }
            set { if (expectedUsers != value) 
                { expectedUsers = value;
                  var numeric = Regex.Replace(expectedUsers, @"[^\d]+", "\n").Trim();
                  expectedUsers = numeric;
                  if (expectedApps != null && expectedUsers != null)
                  {
                        ExpectedTouchpoints = (long.Parse(expectedApps) * long.Parse(expectedUsers)).ToString();
                  }
                  OnPropertyChanged();
                } 
                }
        }

        // Expected Apps Label String
        private string expectedApps;
        public string ExpectedApps
        {
            get { return expectedApps; }
            set { if (expectedApps != value) 
                { expectedApps = value;
                  var numeric = Regex.Replace(expectedUsers, @"[^\d]+", "\n").Trim();
                  expectedUsers = numeric;
                  if (expectedApps != null && expectedUsers != null)
                  {
                      ExpectedTouchpoints = (long.Parse(expectedApps) * long.Parse(expectedUsers)).ToString();
                  }
                  OnPropertyChanged();
                } 
                }
        }

        // Expected Touchpoints Label String
        private string expectedTouchpoints;
        public string ExpectedTouchpoints
        {
            get { return expectedTouchpoints; }
            set { if (expectedTouchpoints != value) { expectedTouchpoints = value; OnPropertyChanged(); } }
        }

        private DateTime startDate = DateTime.Now;
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; OnPropertyChanged(); }
        }

        private DateTime endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; OnPropertyChanged(); }
        }

        private DateTime selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set { selectedDate = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public FactoolExpectedAddViewModel()
        {
            //Initialize UID Model Class
            ExpectedModelInstance = new FactoolExpectedModel
            {
                _iD = 0,
                ClassName = "",
                N_Number = "",
                StartDate = "",
                ExpectedUsers = "0",
                ExpectedApps = "0",
                ExpectedTouchpoints = "0",
                ClassOwner = ""
            };

            idRef = "0";
            ClassName = "";
            N_Number = "";
            ExpectedUsers = "0";
            ExpectedApps = "0";
            ExpectedTouchpoints = "0";
            ClassOwner = "";

            StartDate = new DateTime(2019, 01, 01);
            EndDate = new DateTime(2099, 01, 01);
            SelectedDate = DateTime.Now;

            // Initialize Relay Commands
            AddRecord = new RelayCommand(o => { AddRecordMethod(); });
            CloseButtonCommand = new RelayCommand(o => { ReloadWindow(); });

        }
        #endregion

        #region Functions
        public void LoadData()
        {
            try
            {
                idRef = "0";
                ClassName = "";
                PERSONA = "";
                N_Number = "";
                ExpectedUsers = "0";
                ExpectedApps = "0";
                ExpectedTouchpoints = "0";
                StartDate = new DateTime(2019, 01, 01);
                EndDate = new DateTime(2099, 01, 01);
                SelectedDate = DateTime.Now;
                recordsList.Clear();
                // Auto Increment new _iD Record
                List<int> expectedRecordsList = DBConn.GetColumnListInt(ConfigurationProperties.FactoolExpectedNumbers, "_iD");
                int[] listArray = expectedRecordsList.ToArray();
                int maxRecord = listArray.Max();
                maxRecord++;
                idRef = maxRecord.ToString();
                // Established and displays the user creating the record
                CurrentUserInfo = new UserInfo();
                CurrentUserInfo = App.GlobalUserInfo; 
                LoggedInAs = "Creating Record As : ";

                if (CurrentUserInfo.DisplayName != null)
                {
                    LoggedInAs = "Creating Record #" + idRef +  " on : " + DateTime.Now.ToShortDateString();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Add Record Command Confirmation
        private void AddRecordMethod()
        {
            if (MessageBox.Show("Are you certain you want to create this record?", "Create Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                CreateRecord();
            }
            else
            {
                //Do Nothing
            }
        }

        private void CreateRecord()
        {
            try
            {
                ExpectedModelInstance._iD = int.Parse(idRef);
                ExpectedModelInstance.ClassName = ClassName;
                ExpectedModelInstance.N_Number = N_Number;
                ExpectedModelInstance.PERSONA = PERSONA;
                ExpectedModelInstance.ExpectedUsers = ExpectedUsers;
                ExpectedModelInstance.ExpectedApps = ExpectedApps;
                ExpectedModelInstance.ExpectedTouchpoints = ExpectedTouchpoints;
                ExpectedModelInstance.StartDate = SelectedDate.ToString("yyyy-MM-dd");
                ExpectedModelInstance.ClassOwner = ClassOwner;


                DBConn.AddFactoolExpectedRecord(ExpectedModelInstance, ConfigurationProperties.FactoolExpectedNumbers);

                // After succesful creation in Server updates the local GlobalUIDTable instance
                DataTable selectNewRow = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolExpectedNumbers, "_id", idRef);

                string newId = selectNewRow.Rows[0][0].ToString();
                DataRow row;
                row = MainViewModel.FactoolExpectedTable.NewRow();
                row["_id"] = newId;
                row["ClassName"] = ClassName;
                row["N_Number"] = N_Number;
                row["PERSONA"] = PERSONA;
                row["ExpectedUsers"] = ExpectedUsers;
                row["ExpectedApps"] = ExpectedApps;
                row["ExpectedTouchpoints"] = ExpectedTouchpoints;
                row["StartDate"] = ExpectedModelInstance.StartDate;
                row["ClassOwner"] = ExpectedModelInstance.ClassOwner;
                MainViewModel.FactoolExpectedTable.Rows.Add(row);

                ReloadWindow();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Reload Window and Main Table
        public void ReloadWindow()
        {
            MSG.Messenger.Default.Send("Expected Table Reload");
            LoadData();
        }
        #endregion
    }
}

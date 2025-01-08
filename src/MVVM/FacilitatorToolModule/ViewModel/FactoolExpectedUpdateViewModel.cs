using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FactoolExpectedUpdateViewModel : BaseViewModel
    {
        #region Delegates
        // SQL Methods Class
        public SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand UpdateRecordCommand { get; set; }
        #endregion

        #region Properties
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo { get { return currentUserInfo; } set { currentUserInfo = value; } }

        // Factool Expected Properties Model Class
        public FactoolExpectedModel ExpectedModelInstance { get; set; }

        // For tracking duplicate ID Record Creation
        public List<string> recordsList = new();

        // Sets the Chosen Record from Expected Table View
        private readonly string chosenRecord;

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
            set
            {
                if (expectedUsers != value)
                {
                    expectedUsers = value;
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
            set
            {
                if (expectedApps != value)
                {
                    expectedApps = value;
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
        public FactoolExpectedUpdateViewModel()
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

            chosenRecord = FactoolExpectedTableViewModel.ChosenRecord;

            UpdateRecordCommand = new RelayCommand(o => { UpdateRecord(); });
        }
        #endregion

        #region Functions
        public void LoadData()
        {
            try
            {
                // Load in Selected Record information into Text Boxes with formatting
                DataTable foundRecords = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolExpectedNumbers, "_id", chosenRecord);
                if (foundRecords != null && foundRecords.Rows.Count > 0)
                {
                    DataRow selectedRecord = foundRecords.Rows[0];
                    string checkEmpty = selectedRecord["_iD"].ToString();

                    if (selectedRecord != null && checkEmpty != "")
                    {
                        ExpectedModelInstance._iD = int.Parse(selectedRecord["_iD"].ToString()); IdRef = ExpectedModelInstance._iD.ToString();
                        ExpectedModelInstance.ClassName = selectedRecord["ClassName"].ToString(); ClassName = ExpectedModelInstance.ClassName;
                        ExpectedModelInstance.ClassOwner = selectedRecord["ClassOwner"].ToString(); ClassOwner = ExpectedModelInstance.ClassOwner;
                        ExpectedModelInstance.N_Number = selectedRecord["N_Number"].ToString(); N_Number = ExpectedModelInstance.N_Number;
                        ExpectedModelInstance.PERSONA = selectedRecord["PERSONA"].ToString(); PERSONA = ExpectedModelInstance.PERSONA;
                        ExpectedModelInstance.ExpectedUsers = selectedRecord["ExpectedUsers"].ToString(); ExpectedUsers = ExpectedModelInstance.ExpectedUsers;
                        ExpectedModelInstance.ExpectedApps = selectedRecord["ExpectedApps"].ToString(); ExpectedApps = ExpectedModelInstance.ExpectedApps;
                        ExpectedModelInstance.ExpectedTouchpoints = selectedRecord["ExpectedTouchpoints"].ToString(); ExpectedTouchpoints = ExpectedModelInstance.ExpectedTouchpoints;
                        ExpectedModelInstance.StartDate = selectedRecord["StartDate"].ToString(); SelectedDate = DateTime.Parse(ExpectedModelInstance.StartDate);
                    }
                }
                else
                {
                    ExpectedModelInstance._iD = 0;
                    ExpectedModelInstance.ClassName = "";
                    ExpectedModelInstance.ClassOwner = "";
                    ExpectedModelInstance.N_Number = "";
                    ExpectedModelInstance.PERSONA = "";
                    ExpectedModelInstance.ExpectedUsers = "";
                    ExpectedModelInstance.ExpectedApps = "";
                    ExpectedModelInstance.ExpectedTouchpoints = "";
                    ExpectedModelInstance.StartDate = "";
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void UpdateRecord()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to update this record?", "Update Record??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    // Set Current User Information
                    CurrentUserInfo = new UserInfo();
                    CurrentUserInfo = App.GlobalUserInfo;

                    ExpectedModelInstance._iD = int.Parse(idRef);
                    ExpectedModelInstance.ClassName = ClassName;
                    ExpectedModelInstance.ClassOwner = ClassOwner;
                    ExpectedModelInstance.N_Number = N_Number;
                    ExpectedModelInstance.PERSONA = PERSONA;
                    ExpectedModelInstance.ExpectedUsers = ExpectedUsers;
                    ExpectedModelInstance.ExpectedApps = ExpectedApps;
                    ExpectedModelInstance.ExpectedTouchpoints = ExpectedTouchpoints;
                    ExpectedModelInstance.StartDate = SelectedDate.ToString("yyyy-MM-dd");

                    // Final Update Transaction, also updates Local GlobalUIDTable to reflect Changes
                    DBConn.FactoolExpectedTableRecordUpdate(ExpectedModelInstance, ConfigurationProperties.FactoolExpectedNumbers);

                    DataRow[] rows = MainViewModel.FactoolExpectedTable.Select("_iD =" + chosenRecord);
                    foreach (DataRow dr in rows)
                    {
                        dr["_iD"] = idRef;
                        dr["ClassName"] = ClassName;
                        dr["ClassOwner"] = ClassOwner;
                        dr["N_Number"] = N_Number;
                        dr["PERSONA"] = PERSONA;
                        dr["ExpectedUsers"] = ExpectedUsers;
                        dr["ExpectedApps"] = ExpectedApps;
                        dr["ExpectedTouchpoints"] = ExpectedTouchpoints;
                        dr["StartDate"] = ExpectedModelInstance.StartDate;
                    }
                    MSG.Messenger.Default.Send("Expected Table Reload");
                    LoadData();
                }
                else
                {
                    //Do Nothing
                }
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

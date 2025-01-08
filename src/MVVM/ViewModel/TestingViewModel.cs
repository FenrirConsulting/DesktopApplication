using Centrify.DirectControl.API;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using Microsoft.AspNetCore.Mvc;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class TestingViewModel : BaseViewModel
    {
        #region Delegates
        private readonly SQLMethods DBConn = new();
        public RelayCommand RecordSelect { get; set; }
        public RelayCommand UpdateButtonCommand { get; set; }
        public RelayCommand TestButtonCommand { get; set; }
        public RelayCommand SetUserCommand { get; set; }
        public RelayCommand SetEnvironmentCommand { get; set; }
        public RelayCommand CheckConfigCommand { get; set; }
        #endregion

        #region Properties
        // Binds Version Number Text
        private string versionNumber;
        public string VersionNumber
        {
            get { return versionNumber; }
            set { if (versionNumber != value) { versionNumber = value; OnPropertyChanged(); } }
        }

        // Binds Testing Password Text Box
        private string passwordString;
        public string PasswordString
        {
            get { return passwordString; }
            set { if (passwordString != value) { passwordString = value; OnPropertyChanged(); } }
        }

        // Binds recordId String 
        private string recordId;
        public string RecordId
        {
            get { return recordId; }
            set { if (recordId != value) { recordId = value; OnPropertyChanged(); } }
        }

        // Binds recordSetting String 
        private string recordSetting;
        public string RecordSetting
        {
            get { return recordSetting; }
            set { if (recordSetting != value) { recordSetting = value; OnPropertyChanged(); } }
        }

        // Binds recordValue String 
        private string recordValue;
        public string RecordValue
        {
            get { return recordValue; }
            set { if (recordValue != value) { recordValue = value; OnPropertyChanged(); } }
        }

        // Binds Testing Password Text Box
        private string usernameString;
        public string UsernameString
        {
            get { return usernameString; }
            set { if (usernameString != value) { usernameString = value; OnPropertyChanged(); } }
        }

        // Binds Spinner Loading Bool Value
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Table Used to Hold Paged Display Data
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable Paged Data
        private DataView displayView;
        public DataView DisplayView
        {
            get { return displayView; }
            set { displayView = value; OnPropertyChanged(); }
        }

        // Binds Local Table to Global Table
        private DataTable bindTable;
        public DataTable BindTable
        {
            get { return bindTable; }
            set { bindTable = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public TestingViewModel()
        {
            TestButtonCommand = new RelayCommand(o => { TestButton(); });
            RecordSelect = new RelayCommand(o => { RecordSelectMethod(); });
            UpdateButtonCommand = new RelayCommand(o => { UpdateRecord(); });
            SetEnvironmentCommand = new RelayCommand((box) => { SetEnvironment(box.ToString()); });
            CheckConfigCommand = new RelayCommand(o => { CheckConfig(); });
            DisplayTable = new DataTable();
            BindTable = new DataTable();
            DisplayView = new DataView();
        }
        #endregion

        #region Functions
        public async void LoadData()
        {
            try
            {
                


                LoadDatabase();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void TestButton()
        {
            try
            {
                PrincipalSearchResult<Principal> groups = null;
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                UserPrincipal usr = UserPrincipal.FindByIdentity(ctx,
                                                           IdentityType.SamAccountName,
                                                           UsernameString);
                List<String> groupString = new();
                var result = ""; 
                if (usr != null)
                {
                    groups = usr.GetGroups();

                    foreach (Principal p in groups)
                    {
                        groupString.Add(p.ToString());
                    }

                    result = String.Join(Environment.NewLine, groupString.ToArray());
                }
                else
                {
                    result = "User Not Found";
                }
                
                string final = result;
                MessageBox.Show(final);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Manual Data Reload on Button Click
        public async void LoadDatabase()
        {
            try
            {
                IsLoadBool = true;
                var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.IAMHConfig);
                DisplayTable = dataTask;
                DisplayView = DisplayTable.DefaultView;
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Opens Update View with selected Row Record
        private void RecordSelectMethod()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        RecordId = row["Id"].ToString();
                        RecordSetting = row["Setting"].ToString();
                        RecordValue = row["Value"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Updates Value Record
        private void UpdateRecord()
        {
            if (MessageBox.Show("Are you certain you want to update this record?", "Update Record?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                DBConn.UpdateTableRecord(ConfigurationProperties.IAMHConfig,RecordId,"Id","Value",RecordValue);

                LoadDatabase();
                RecordId = "Record Updated";
                RecordValue = "";
                RecordSetting = "";
            }
        }

        // Set Envrionment
        public void SetEnvironment(string ENV)
        {
            try
            {
                switch (ENV)
                {
                    case "PROD":
                        Program.ConnectionString = ConfigurationProperties.ConnectionString;
                        break;

                    case "QA":
                        Program.ConnectionString = ConfigurationProperties.QAConnectionString;
                        break;

                    case "DEV":
                        Program.ConnectionString = ConfigurationProperties.DEVConnectionString;
                        break;

                    default:
                        break;
                }

                MessageBox.Show(ENV);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Check IAMHConfig Setting
        public void CheckConfig()
        {

            try
            {
                DataTable tempTable = new();
                tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "FactoolUpdateCount");

                string checkReturn = tempTable.Rows[0][2].ToString();

                MessageBox.Show(checkReturn);
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

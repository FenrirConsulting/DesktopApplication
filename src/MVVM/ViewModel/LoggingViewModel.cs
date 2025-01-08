    using System;
using IAMHeimdall.Core;
using IAMHeimdall;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using IAMHeimdall.MVVM.Model;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class LoggingViewModel : BaseViewModel
    {
        #region Delegates
        readonly SQLMethods DBConn = new();

        //Relay Commands
        public RelayCommand OpenRecordCommand { get; set; }
        public RelayCommand CloseButtonCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand PopupLostFocusCommand { get; set; }
        public RelayCommand DeleteLogsCommand { get; set; }

        #endregion

        #region Properties
        // Table Used to Hold Logging Data
        private DataTable fetchTable;
        public DataTable FetchTable
        {
            get { return fetchTable; }
            set { fetchTable = value; OnPropertyChanged(); }
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

        // Loading Animation Bool
        private bool recordSelected;
        public bool RecordSelected
        {
            get { return recordSelected; }
            set { recordSelected = value; OnPropertyChanged(); }
        }

        // Binds text Selected Record Text
        private string recordString;
        public string RecordString
        {
            get { return recordString; }
            set { if (recordString != value) { recordString = value; OnPropertyChanged(); } }
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
        public LoggingViewModel()
        {
            //Initialize Properties
            FetchTable = new DataTable();
            RecordSelected = false;

            //Relay Command Initilization
            OpenRecordCommand = new RelayCommand(o => { OpenRecord(); });
            PopupLostFocusCommand = new RelayCommand(o => { RecordString = ""; RecordSelected = false; });
            CloseButtonCommand = new RelayCommand(o => { RecordSelected = false; });
            DeleteCommand = new RelayCommand(o => { DeleteRecord(); });
            DeleteLogsCommand = new RelayCommand(o => { DeleteAllLogs(); });

        }
        #endregion

        #region Functions
        public void OnLoad()
        {
            try
            {
                LoadTable();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void LoadTable()
        {
            try
            {
                FetchTable.Clear();
                FetchTable = DBConn.GetTable("IAMHLog");

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void DeleteRecord()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["Id"].ToString();
                        string deleteRecord = record;
                        DBConn.DeleteMatchingRecords("IAMHLog", "Id", deleteRecord);
                        LoadTable();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void OpenRecord()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["Message"].ToString();
                        RecordString = record;
                    }
                }

                RecordSelected = true;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        private void DeleteAllLogs()
        {
            if (MessageBox.Show("Are you certain you want to delete all records?", "Delete Records??", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                DBConn.DeleteAllTableRecords(ConfigurationProperties.IAMHLog);
                LoadTable();
            }
        }
        #endregion
    }
}

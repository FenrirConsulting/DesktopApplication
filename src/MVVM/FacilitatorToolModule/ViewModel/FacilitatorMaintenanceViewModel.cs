using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FacilitatorMaintenanceViewModel : BaseViewModel
    {
        #region Delegates
        //Access SQL Methods Class
        private readonly SQLMethods DBConn = new();

        //Relay Commands for Buttons
        public RelayCommand UpdateItemsCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand DeleteClickCommand { get; set; }
        public RelayCommand ExportCommand { get; set; }
        public RelayCommand CategoryChangedCommand { get; set; }
        #endregion

        #region Properties
        // Holds Data for Display Table
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // Holds Comapre Data for Modification Check
        private DataTable originalTable;
        public DataTable OriginalTable
        {
            get { return originalTable; }
            set { originalTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView displayView;
        public DataView DisplayView
        {
            get { return displayView; }
            set { displayView = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<ComboBoxListItem> searchTerms = new()
        {
            new ComboBoxListItem { BoxItem = "DefectReasons" },
            new ComboBoxListItem { BoxItem = "FormType" },
            new ComboBoxListItem { BoxItem = "LOBType"},
            new ComboBoxListItem { BoxItem = "ReplyType" },
            new ComboBoxListItem { BoxItem = "RequestType" },
            new ComboBoxListItem { BoxItem = "Systems" }
        };

        // Basis for Search Terms Collection
        public IEnumerable<ComboBoxListItem> SearchTerms
        {
            get { return searchTerms; }
        }

        // Selected Term for ComboBox
        private ComboBoxListItem selectedTerm = new();
        public ComboBoxListItem SelectedTerm
        {
            get { return selectedTerm; }
            set { selectedTerm = value; 
                OnPropertyChanged();
            }
        }

        // Bool to starting Loading Animation
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Holds Data for Display Table
        private DataTable defectReasonsTable;
        public DataTable DefectReasonsTable
        {
            get { return defectReasonsTable; }
            set { defectReasonsTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable formTypesTable;
        public DataTable FormTypesTable
        {
            get { return formTypesTable; }
            set { formTypesTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable lOBTypesTable;
        public DataTable LOBTypesTable
        {
            get { return lOBTypesTable; }
            set { lOBTypesTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable replyTypesTable;
        public DataTable ReplyTypesTable
        {
            get { return replyTypesTable; }
            set { replyTypesTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable requestTypesTable;
        public DataTable RequestTypesTable
        {
            get { return requestTypesTable; }
            set { requestTypesTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable systemsTable;
        public DataTable SystemsTable
        {
            get { return systemsTable; }
            set { systemsTable = value; OnPropertyChanged(); }
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

        private string databaseConnectionString;
        public string DatabaseConnectionString
        {
            get { return databaseConnectionString; }
            set { databaseConnectionString = value; OnPropertyChanged(); }
        }

        private string databaseConfigConnectionString;
        public string DatabaseConfigConnectionString
        {
            get { return databaseConfigConnectionString; }
            set { databaseConfigConnectionString = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public FacilitatorMaintenanceViewModel()
        {
            // Initialize Propeties
            IsLoadBool = false;

            //Initialize Relay Commands
            UpdateItemsCommand = new RelayCommand(o => { UpdateItems(); });
            DeleteCommand = new RelayCommand(o => { DeleteItems(); });
            DeleteClickCommand = new RelayCommand(o => { DeleteClick(); });
            ExportCommand = new RelayCommand(o => { ExportItems(); });
            CategoryChangedCommand = new RelayCommand(o => { CategoryChanged(); });
            DisplayTable = new();
            DisplayView = new();
            OriginalTable = new();
            DefectReasonsTable = new();
            FormTypesTable = new();
            ReplyTypesTable = new();
            RequestTypesTable = new();
            LOBTypesTable = new();
            SystemsTable = new();
            StartDate = new DateTime(2019, 12, 31);
            EndDate = DateTime.Now.AddDays(-5);
            SelectedDate = EndDate;
            DatabaseConnectionString = "Data Source=MAZPIAMHDB01.aeth.Company.com;Initial Catalog=IAMH;\\" + ConfigurationProperties.FactoolRequestTable;
            DatabaseConfigConnectionString = "Data Source=MAZPIAMHDB011.aeth.Company.com;Initial Catalog=IAMH;\\" + Environment.NewLine
                + ConfigurationProperties.FactoolConfigDefectReasons + ";"
                + ConfigurationProperties.FactoolConfigFormType + ";"
                + ConfigurationProperties.FactoolLOBTypes + ";"
                + ConfigurationProperties.FactoolConfigReplyType + ";"
                + ConfigurationProperties.FactoolConfigRequestType + ";"
                + ConfigurationProperties.FactoolConfigSystems;

        }
        #endregion

        #region Functions
        // Main On Load Function
        public void LoadData()
        {
            try
            {
                LoadTables();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // On Category Combobox Change, Changes Display Table to Chosen Category
        public void CategoryChanged()
        {
            try
            {
                OriginalTable.Clear();
                DisplayTable.Clear();
                DisplayView = DisplayTable.DefaultView;
                string SelectedItem = SelectedTerm.BoxItem.ToString();
                switch (SelectedItem)
                {
                    case "DefectReasons":
                        DisplayTable = DefectReasonsTable.Copy();
                        break;

                    case "FormType":
                        DisplayTable = FormTypesTable.Copy(); ;
                        break;

                    case "LOBType":
                        DisplayTable = LOBTypesTable.Copy();
                        break;

                    case "ReplyType":
                        DisplayTable = ReplyTypesTable.Copy(); ;
                        break;

                    case "RequestType":
                        DisplayTable = RequestTypesTable.Copy(); ;
                        break;

                    case "Systems":
                        DisplayTable = SystemsTable.Copy(); ;
                        break;

                    default:
                        break;
                }
                OriginalTable = DisplayTable.Copy();
                DisplayView = DisplayTable.DefaultView;

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Update Items Button Function
        public void UpdateItems()
        {
            try
            {
                DataTable CompareTable = new();
                DataTable TempTable = new();
                DataTable AddTable = new();
                CompareTable = DisplayView.ToTable();
                TempTable = OriginalTable.Clone();
                AddTable = OriginalTable.Clone();
                
                // Loop finding Differences in Modified and Original Table. Creates a Third Differences Table 
                foreach (DataRow orgRow in OriginalTable.Rows)
                {
                    foreach (DataRow comRow in CompareTable.Rows)
                    {
                        if (orgRow["Id"].ToString() == comRow["Id"].ToString())
                        {
                            bool addRow = false;
                            if (orgRow["Name"].ToString() != comRow["Name"].ToString()) { addRow = true; }
                            if (orgRow["Value"].ToString() != comRow["Value"].ToString()) { addRow = true; }
                            if (orgRow["DisplayOrder"].ToString() != comRow["DisplayOrder"].ToString()) { addRow = true; }
                            if (addRow == true) { TempTable.Rows.Add(comRow.ItemArray); }
                        }
                    }
                }

                // Searches Compare Table for Rows that do not Exist in Original Table
                foreach (DataRow comRow in CompareTable.Rows)
                {
                    DataRow[] foundId = OriginalTable.Select("ID= " + comRow["Id"].ToString());
                    if (foundId.Length == 0)
                    {
                        AddTable.Rows.Add(comRow.ItemArray);
                    }
                }

                    // Loop goes through each Differences Row to update on SQL Server. Switch based on Table Chosen in Category Combobox
                    foreach (DataRow row in TempTable.Rows)
                {
                    string table = DetermineTableChosen();
                    string preID = row["Id"].ToString();
                    int id = Int32.Parse(preID);
                    string name = row["Name"].ToString();
                    string value = row["Value"].ToString();
                    string displayorder = row["DisplayOrder"].ToString();
                    string modifiedby = App.GlobalUserInfo.DisplayName;
                    string modified = DateTime.Now.ToString("yyyy/MM/dd h:mm:ss.fff");
                    DBConn.FactoolConfigUpdate(table, id, name, value, displayorder, modifiedby, modified);
                }

                    // Add New Rows to Category Table
                    foreach (DataRow row in AddTable.Rows)
                {
                    string table = DetermineTableChosen();
                    string name = row["Name"].ToString();
                    string value = row["Value"].ToString();
                    string displayorder = row["DisplayOrder"].ToString();
                    string modifiedby = App.GlobalUserInfo.DisplayName;
                    string modified = DateTime.Now.ToString("yyyy/MM/dd h:mm:ss.fff");

                    DBConn.AddFactoolConfig(table, name, value, displayorder, modifiedby, modified);
                }
                MSG.Messenger.Default.Send("Reload Factool Tables");
                LoadTables();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Delete Items Button Function
        public void DeleteItems()
        {
            string chosenTime = SelectedDate.ToLongDateString();

            if (MessageBox.Show("Are you certain you want to delete  records before " + chosenTime + "?", "Delete Records??" , MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                long ticks = SelectedDate.Ticks;
                string passedTicks = ticks.ToString();
                DBConn.DeleteRecordsLessThan(ConfigurationProperties.FactoolRequestTable, "CreateTick", passedTicks);
                DBConn.DeleteFactoolStatusRecords();
                MSG.Messenger.Default.Send("Reload Factool Tables");
                LoadTables();
            }
            else
            { 
                // Do Nothing
            }
        }

        // Delete Item from Table on Right-Click , Delete
        public void DeleteClick()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to delete this record?", "Delete Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataRowView drv = (DataRowView)CellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            string record = row["Id"].ToString();
                            string table = DetermineTableChosen();
                            DBConn.DeleteMatchingRecords(table, "Id", record);

                            MSG.Messenger.Default.Send("Reload Factool Tables");
                            LoadTables();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Export Items Button Function
        public static void ExportItems()
        {

        }

        // Load Config Tables into Local Objects
        public void LoadTables()
        {
            DefectReasonsTable = FacilitatorMainViewModel.DefectReasonsTable.Copy();
            FormTypesTable = FacilitatorMainViewModel.FormTypeTable.Copy();
            ReplyTypesTable = FacilitatorMainViewModel.ReplyTypesTable.Copy();
            RequestTypesTable = FacilitatorMainViewModel.RequestTypesTable.Copy();
            SystemsTable = FacilitatorMainViewModel.SystemsTable.Copy();
            LOBTypesTable = FacilitatorMainViewModel.LOBTypeTable.Copy();
            CategoryChanged();
        }

        // Sets Table to Chosen Term in Drop Down Menu
        public string DetermineTableChosen()
        {
            string table = "";
            string SelectedItem = SelectedTerm.BoxItem.ToString();
            switch (SelectedItem)
            {
                case "DefectReasons":
                    table = ConfigurationProperties.FactoolConfigDefectReasons;
                    break;

                case "FormType":
                    table = ConfigurationProperties.FactoolConfigFormType;
                    break;

                case "LOBType":
                    table = ConfigurationProperties.FactoolLOBTypes;
                    break;

                case "ReplyType":
                    table = ConfigurationProperties.FactoolConfigReplyType;
                    break;

                case "RequestType":
                    table = ConfigurationProperties.FactoolConfigRequestType;
                    break;

                case "Systems":
                    table = ConfigurationProperties.FactoolConfigSystems;
                    break;

                default:
                    break;
            }
            return table;
        }
        #endregion
    }
}

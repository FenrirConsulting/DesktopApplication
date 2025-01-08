using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FacilitatorQueryViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand SearchStatusCommand { get; set; }
        public RelayCommand SearchReferenceCommand { get; set; }
        public RelayCommand QueryAllCommand { get; set; }
        public RelayCommand ClearSearchCommand { get; set; }
        public RelayCommand DoubleClickRecordCommand { get; set; }
        public RelayCommand DeleteClickCommand { get; set; }
        public RelayCommand CSVExportCommand { get; set; }
        public RelayCommand CSVExportNarrowCommand { get; set; }
        public RelayCommand ClipboardExportCommand { get; set; }
        public RelayCommand ClipboardExportNarrowCommand { get; set; }
        public RelayCommand HTMLExportCommand { get; set; }
        public RelayCommand HTMLExportNarrowCommand { get; set; }

        private readonly SQLMethods DBConn = new();
        public FactoolHistoryBuilder HBuilder = new();
        #endregion

        #region Properties
        // Binds Export Stackpanel Enabled
        private bool exportsEnabled;
        public bool ExportsEnabled
        {
            get { return exportsEnabled; }
            set { if (exportsEnabled != value) { exportsEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Datetime for From Date Start Date
        private DateTime fromStartDate = DateTime.Now;
        public DateTime FromStartDate
        {
            get { return fromStartDate; }
            set { fromStartDate = value; OnPropertyChanged(); }
        }

        // Holds Datetime for From Date End Date
        private DateTime fromEndDate = DateTime.Now;
        public DateTime FromEndDate
        {
            get { return fromEndDate; }
            set { fromEndDate = value; OnPropertyChanged(); }
        }

        // Holds Datetime for From Date Select Date
        private DateTime fromSelectedDate = DateTime.Now;
        public DateTime FromSelectedDate
        {
            get { return fromSelectedDate; }
            set { fromSelectedDate = value; OnPropertyChanged(); DateChangedFunction("From"); }
        }

        // Holds Datetime for To Date Start Date
        private DateTime toStartDate = DateTime.Now;
        public DateTime ToStartDate
        {
            get { return toStartDate; }
            set { toStartDate = value; OnPropertyChanged(); }
        }

        // Holds Datetime for To Date From Date
        private DateTime toEndDate = DateTime.Now;
        public DateTime ToEndDate
        {
            get { return toEndDate; }
            set { toEndDate = value; OnPropertyChanged(); }
        }

        // Holds Datetime for To Date Select Date
        private DateTime toSelectedDate = DateTime.Now;
        public DateTime ToSelectedDate
        {
            get { return toSelectedDate; }
            set { toSelectedDate = value; OnPropertyChanged(); DateChangedFunction("To"); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<SearchTerms> statusTerms = new()
        {
            new SearchTerms { SearchTerm = "All" },
            new SearchTerms { SearchTerm = "Complete" },
            new SearchTerms { SearchTerm = "Defected" },
            new SearchTerms { SearchTerm = "Ready to Process" }
        };

        // Basis for Search Terms Collection
        public IEnumerable<SearchTerms> StatusTerms
        {
            get { return statusTerms; }
        }

        // Selected Term for ComboBox
        private SearchTerms selectedStatusTerm = new();
        public SearchTerms SelectedStatusTerm
        {
            get { return selectedStatusTerm; }
            set { selectedStatusTerm = value; OnPropertyChanged(); }
        }

        // Binds Reference Number String
        private string referenceNumberString;
        public string ReferenceNumberString
        {
            get { return referenceNumberString; }
            set
            {
                if (referenceNumberString != value)
                {
                    referenceNumberString = value;
                    if (referenceNumberString.Length >= 9) { ReferenceSearchEnabled = true; } else { ReferenceSearchEnabled = false; }
                    OnPropertyChanged();
                }
            }
        }

        // Binds Reference Search Button as Enabled
        private bool referenceSearchEnabled;
        public bool ReferenceSearchEnabled
        {
            get { return referenceSearchEnabled; }
            set { if (referenceSearchEnabled != value) { referenceSearchEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Show All Columns Checkbox Check Value
        private bool showAllChecked;
        public bool ShowAllChecked
        {
            get { return showAllChecked; }
            set { if (showAllChecked != value) { showAllChecked = value; OnPropertyChanged(); } }
        }

        // Binds Status String 
        private string statusString;
        public string StatusString
        {
            get { return statusString; }
            set
            {
                if (statusString != value)
                {
                    statusString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Data for Display Table
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // Holds Data for Query Table
        private DataTable queryTable;
        public DataTable QueryTable
        {
            get { return queryTable; }
            set { queryTable = value; OnPropertyChanged(); }
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

        // Binds Delete Menu Enabled
        private bool deleteMenuEnabled;
        public bool DeleteMenuEnabled
        {
            get { return deleteMenuEnabled; }
            set { if (deleteMenuEnabled != value) { deleteMenuEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Spinner Loading Bool Value
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Binds Export Spinner Loading
        private bool isLoadBoolExport;
        public bool IsLoadBoolExport
        {
            get { return isLoadBoolExport; }
            set { if (isLoadBoolExport != value) { isLoadBoolExport = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public FacilitatorQueryViewModel()
        {
            // Initialize Propeties
            ShowAllChecked = false;
            DisplayTable = new();
            QueryTable = new();
            DisplayView = new();
            ExportsEnabled = true;
            IsLoadBool = false;
            IsLoadBoolExport = false;
            StatusString = "";
            StatusString = "Ready";
            ReferenceSearchEnabled = false;
            DeleteMenuEnabled = false;
            FromStartDate = new DateTime(2019, 1, 1);
            FromEndDate = DateTime.Now.AddDays(60);
            FromSelectedDate = DateTime.Now;
            ToStartDate = new DateTime(2019, 1, 1);
            ToEndDate = DateTime.Now.AddDays(60);
            ToSelectedDate = DateTime.Now;

            //Initialize Relay Commands
            SearchStatusCommand = new RelayCommand(o => { SearchStatus(); });
            SearchReferenceCommand = new RelayCommand(o => { SearchReference(); });
            QueryAllCommand = new RelayCommand(o => { QueryAll(); });
            ClearSearchCommand = new RelayCommand(o => { ClearSearch(); });
            DoubleClickRecordCommand = new RelayCommand(o => { DoubleClickFunction(); });
            DeleteClickCommand = new RelayCommand(o => { DeleteMenuClick(); });
            CSVExportCommand = new RelayCommand(o => { ExportCSV("All"); });
            CSVExportNarrowCommand = new RelayCommand(o => { ExportCSV("Partial"); });
            ClipboardExportCommand = new RelayCommand(o => { ExportClipboard("All"); });
            ClipboardExportNarrowCommand = new RelayCommand(o => { ExportClipboard("Partial"); });
            HTMLExportCommand = new RelayCommand(o => { HTMLExport("All"); });
            HTMLExportNarrowCommand = new RelayCommand(o => { HTMLExport("Partial"); });
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            try
            {
                DeleteMenuEnabled = MainViewModel.GlobalFacToolAdminPermission; // Checkbox only visibile to Factool Admins
                DisplayTable.Clear();
                BuildTable(DisplayTable);
                DisplayView = DisplayTable.DefaultView;
                ExportsEnabled = MainViewModel.GlobalFacToolMgrPermission; // Enables Export Buttons if Manager or higher level
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Search Status Function, Filtered on Date and Status Selection
        public async void SearchStatus()
        {
            try
            {
                DataTable firstTable = new();
                DataTable secondTable = new();
                DateTime fromDate = Convert.ToDateTime(FromSelectedDate.ToShortDateString());
                DateTime toDate = Convert.ToDateTime(ToSelectedDate.AddDays(1).ToShortDateString());
                string passedFromDate = fromDate.ToString("yyyy/MM/dd");
                string passedToDate = toDate.ToString("yyyy/MM/dd");
                IsLoadBool = true;
                DisplayTable.Clear();
                var dataTask = await DBConn.GetTableWithRange(ConfigurationProperties.FactoolRequestTable,"CreateDate",passedFromDate,passedToDate);
                firstTable = dataTask;

                if (selectedStatusTerm.SearchTerm != "All") 
                {
                    var filteredTable = firstTable.Select("RequestStatus = '" + selectedStatusTerm.SearchTerm + "'"); ;
                    if (filteredTable.Length != 0) { secondTable = filteredTable.CopyToDataTable(); }
                }
                else
                {
                    secondTable = firstTable.Copy();
                }

                DisplayTable = secondTable.Copy();
                DisplayView = DisplayTable.DefaultView;
                QueryTable.Clear();
                QueryTable = DisplayTable.Copy();
                ReferenceNumberString = "";
                StatusString = "Found " + DisplayTable.Rows.Count.ToString() + " requests.";
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Search Reference Function
        public void SearchReference()
        {
            try
            {
                if (ReferenceSearchEnabled == true)
                {
                    string refNumber = ReferenceNumberString.ToUpper().Trim();
                    ReferenceNumberString = refNumber;

                    DataTable tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestTable,"ReferenceNumber", refNumber);

                    if (tempTable.Rows.Count > 0)
                    {
                        DisplayTable.Clear();
                        DisplayTable = tempTable.Copy();
                        DisplayView = DisplayTable.DefaultView;
                        QueryTable.Clear();
                        QueryTable = DisplayTable.Copy();
                        StatusString = "Found request.";
                    }
                    else
                    {
                        StatusString = "Request not Found.";
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Return Entire Request Table
        public async void QueryAll()
        {
            try
            {
                DisplayTable.Clear();
                DisplayView = DisplayTable.DefaultView;
                IsLoadBool = true;
                if (FacilitatorMainViewModel.QueryRequestTable.Rows.Count < 10)
                {
                    FacilitatorMainViewModel.QueryRequestTable.Clear();
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    FacilitatorMainViewModel.QueryRequestTable = dataTask;
                }
                DisplayTable.Clear();
                DisplayTable = FacilitatorMainViewModel.QueryRequestTable.Copy();
                DisplayView = DisplayTable.DefaultView;
                StatusString = "Found " + DisplayTable.Rows.Count.ToString() + " requests.";
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Clear Form and Grid
        public void ClearSearch()
        {
            DisplayTable.Clear();
            DisplayView = DisplayTable.DefaultView;
            FromStartDate = new DateTime(2019, 1, 1);
            FromEndDate = DateTime.Now.AddDays(60);
            FromSelectedDate = DateTime.Now;
            ToStartDate = new DateTime(2019, 1, 1);
            ToEndDate = DateTime.Now.AddDays(60);
            ToSelectedDate = DateTime.Now;
            ReferenceNumberString = "";
            StatusString = "Ready";
        }

        // Function for Datagrid Record Double Click
        public void DoubleClickFunction()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        string record = row["ReferenceNumber"].ToString();
                        ReferenceNumberString = record;
                        FacilitatorMainViewModel.ReferenceNumber = record;
                        MSG.Messenger.Default.Send("Select FacTool Tab 1");
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Admin Only Delete Individual Requests, Records Deletion in History Table
        public void DeleteMenuClick()
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
                            string record = row["ReferenceNumber"].ToString();
                            FacToolRequest HistoryRequest = new();
                            HistoryRequest.ReferenceNumber = row["ReferenceNumber"].ToString();
                            UserPrincipal userPrincipal = UserPrincipal.Current;
                            HistoryRequest.DisplayName = userPrincipal.Name;
                            FactoolRequestHistory PassedHistory = HBuilder.HistoryBuilder(HistoryRequest,HistoryRequest,"Deleted");
                            DBConn.DeleteMatchingRecords(ConfigurationProperties.FactoolRequestTable, "ReferenceNumber", record);
                            DBConn.DeleteMatchingRecords(ConfigurationProperties.FactoolRequestStatus, "ReferenceNumber", record);
                            DBConn.AddFactoolRequestHistory(PassedHistory, ConfigurationProperties.FactoolRequestHistory);

                            List<DataRow> rowsToDelete = new();
                            foreach (DataRow dr in FacilitatorMainViewModel.QueryRequestTable.Rows)
                            {
                                if (dr["_id"].ToString() == record)
                                {
                                    rowsToDelete.Add(dr);
                                }
                            }
                            foreach (DataRow dr in rowsToDelete)
                            {
                                FacilitatorMainViewModel.QueryRequestTable.Rows.Remove(dr);
                            }

                            DisplayTable.Clear();
                            DisplayTable = FacilitatorMainViewModel.QueryRequestTable.Copy();
                            DisplayView = DisplayTable.DefaultView;

                            if (ReferenceNumberString != "")
                            {
                                SearchReference();
                            }
                            else
                            {
                                SearchStatus();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Prevent Date Selection Errors
        public void DateChangedFunction(string switchDate)
        {
            DateTime fromdate = FromSelectedDate;
            DateTime todate = ToSelectedDate;

            if (switchDate == "From")
            {
                if (fromdate > todate)
                {
                    ToSelectedDate = FromSelectedDate;
                }
            }

            if (switchDate == "To")
            {
                if (todate < fromdate)
                {
                    FromSelectedDate = ToSelectedDate;
                }
            }
        }

        // Sets Columns for Passed Display Table
        public static void BuildTable(DataTable SentTable)
        {
            try 
            {
                SentTable.Columns.Clear();
                SentTable.Columns.Add("_id", typeof(int));
                SentTable.Columns.Add("CreateDate", typeof(String));
                SentTable.Columns.Add("CreateTick", typeof(String));
                SentTable.Columns.Add("ModifiedDate", typeof(String));
                SentTable.Columns.Add("ModifiedTick", typeof(String));
                SentTable.Columns.Add("SamAccount", typeof(String));
                SentTable.Columns.Add("DisplayName", typeof(String));
                SentTable.Columns.Add("ReferenceNumber", typeof(String));
                SentTable.Columns.Add("NewRequest", typeof(String));
                SentTable.Columns.Add("TotalUsers", typeof(String));
                SentTable.Columns.Add("RequestStatus", typeof(String));
                SentTable.Columns.Add("FormType", typeof(String));
                SentTable.Columns.Add("RequestType", typeof(String));
                SentTable.Columns.Add("DefectReason", typeof(String));
                SentTable.Columns.Add("Systems", typeof(String));
                SentTable.Columns.Add("ReplyTypes", typeof(String));
                SentTable.Columns.Add("Comments", typeof(String));
                SentTable.Columns.Add("XREF1", typeof(String));
                SentTable.Columns.Add("XREF2", typeof(String));
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Function to Export either Entire Request or Query Table to CSV Format
        public async void ExportCSV(string type)
        {
            try 
            {
                DataTable exportTable = new();
                IsLoadBool = true;
                if (type == "All")
                {
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    exportTable = dataTask.Copy();
                }
                else
                {
                    exportTable = QueryTable.Copy();
                }

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files|*.csv",
                    Title = "Save a CSV File",
                    FileName = "FactoolRequestDatabase.csv"
                };
                StreamWriter writer = null;
                string saveCSV = DataTableToCSV.DataTableCSVConversion(exportTable, '|');
                saveFileDialog.ShowDialog();

                if (!String.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    string filter = saveFileDialog.FileName;
                    using (writer = new StreamWriter(filter))
                    {
                        writer.WriteLine(saveCSV);
                        writer.Close();
                    }
                }
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        //Export DataTable to Clipboard
        public async void ExportClipboard(string type)
        {
            try
            {
                var newline = System.Environment.NewLine;
                var tab = "\t";
                var clipboard_string = new StringBuilder();

                DataTable exportTable = new();
                IsLoadBool = true;
                if (type == "All")
                {
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    exportTable = dataTask.Copy();
                }
                else
                {
                    exportTable = QueryTable.Copy();
                }

                foreach (DataColumn dc in exportTable.Columns)
                {
                    clipboard_string.Append(dc.ColumnName + tab);
                }
                clipboard_string.Append(newline);

                foreach (DataRow row in exportTable.Rows)
                {
                    foreach(DataColumn dc in exportTable.Columns)
                    {
                        clipboard_string.Append(row[dc].ToString() + tab);
                    }

                    clipboard_string.Append(newline);
                }

                Clipboard.SetText(clipboard_string.ToString());
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        //Export HTML Table
        private async void HTMLExport(string type)
        {
            try
            {
                string export = "";
                DataTable ExportTable = new();
                List<String> BuildList = new();
                IsLoadBool = true;
                if (type == "All")
                {
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    ExportTable = dataTask.Copy();
                }
                else
                {
                    ExportTable = QueryTable.Copy();
                }

                string[] columnNames = ExportTable.Columns.Cast<DataColumn>()
                                .Select(x => x.ColumnName)
                                .ToArray();
                foreach (string s in columnNames)
                {
                    BuildList.Add(s);
                }

                string built = string.Join(",", BuildList);
                export = GetHTML(ExportTable, BuildList);
                SaveFileDialog dlg = new();
                dlg.FileName = "RequestExport";
                dlg.DefaultExt = ".html";
                dlg.Filter = "HTML Documents (.html)|*.html";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    File.WriteAllText(dlg.FileName, export);
                }

                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // HTML Table Builder
        private static string GetHTML(DataTable table, List<String> buildList)

        {
            string finalString;
            try
            {
                string tab = "\t";
                List<string> columnList = buildList;
                StringBuilder sb = new();

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<style>");
                sb.AppendLine("body {background-color: #17161b; color: #f0f8ff;}");
                sb.AppendLine("thead {background-color: #17161b; color: #f0f8ff; font-weight: bold;}");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("\t" + "<body>");
                sb.AppendLine("\t\t" + "<table>");
                sb.Append("<table border='2px' solid line black cellpadding='5' cellspacing='0' ");
                sb.Append("style='border: solid 2px #f0f8ff; font-size: medium;'>");

                // headers.
                sb.Append("<thead>");
                sb.Append(tab + tab + tab + "<tr>");

                for (int i = 0; i < columnList.Count; i++)
                {
                    sb.Append("<td>").Append(columnList[i]).Append("</td>");
                }
                sb.AppendLine("</tr>");
                sb.Append("</thead>");

                // data rows
                foreach (DataRow dr in table.Rows)
                {
                    sb.Append("\t\t\t" + "<tr>");

                    foreach (DataColumn dc in table.Columns)
                    {
                        string cellValue = dr[dc] != null ? dr[dc].ToString() : "";
                        sb.AppendFormat("<td>{0}</td>", cellValue);
                    }

                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("\t\t\t" + "</table>");
                sb.AppendLine("\t" + "</body>");
                sb.AppendLine("</html>");
                finalString = sb.ToString();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }

            return finalString;
        }
        #endregion
    }
}

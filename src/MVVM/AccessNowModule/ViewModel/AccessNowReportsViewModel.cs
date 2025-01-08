using AngleSharp.Common;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Nancy;
using Nancy.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class AccessNowReportsViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand LoadReportCommand { get; set; }
        public RelayCommand SpaceCheckboxCommand { get; set; }
        public RelayCommand UpdateColumnsCommand { get; set; }
        public RelayCommand CSVExportCommand { get; set; }
        public RelayCommand HTMLExportCommand { get; set; }
        public RelayCommand ClipboardExportCommand { get; set; }
        public RelayCommand ClearSearch { get; set; }
        public RelayCommand SearchEnterKey { get; set; }
        public RelayCommand SearchQuery { get; set; }
        public RelayCommand ClearQuery { get; set; }
        public RelayCommand EnterCalculation { get; set; }
        public RelayCommand ExcelExport { get; set; }

        private readonly SQLMethods DBConn = new();
        private readonly JSONMethods JMethod = new();
        #endregion

        #region Properties
        // Holds the Access Now API Connection Credentials
        private string apiUsername;
        private string apiPassword;
        private string apiURL;

         // Binds Bool Value of Development Testing Button
        private bool devTestingTurnedOn;
        public bool DevTestingTurnedOn
        {
            get { return devTestingTurnedOn; }
            set
            {
                if (devTestingTurnedOn != value)
                {
                    devTestingTurnedOn = value;
                    OnPropertyChanged();
                    if (DevTestingTurnedOn == true && QATestingTurnedOn == true)
                    {
                        QATestingTurnedOn = false;
                    }
                    SetEnvironmentConfiguration();
                }
            }
        }

        // Binds Bool Value of QA Testing Button
        private bool qATestingTurnedOn;
        public bool QATestingTurnedOn
        {
            get { return qATestingTurnedOn; }
            set
            {
                if (qATestingTurnedOn != value)
                {
                    qATestingTurnedOn = value;
                    OnPropertyChanged();
                    if (DevTestingTurnedOn == true && QATestingTurnedOn == true)
                    {
                        DevTestingTurnedOn = false;
                    }
                    SetEnvironmentConfiguration();
                }
            }
        }

        // Binds Current Local Table 
        private DataTable bindTable;
        public DataTable BindTable
        {
            get { return bindTable; }
            set { bindTable = value; OnPropertyChanged(); }
        }

        // Binds Current Local Export Table 
        private DataTable exportTable;
        public DataTable ExportTable
        {
            get { return exportTable; }
            set { exportTable = value; OnPropertyChanged(); }
        }

        // Table Used to Hold Display Table
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // Table Used to Hold Search Filter Table for Export
        private DataTable filterTable;
        public DataTable FilterTable
        {
            get { return filterTable; }
            set { filterTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable Paged Data
        private DataView displayView;
        public DataView DisplayView
        {
            get { return displayView; }
            set { displayView = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
        }

        // Binds the Currently Loaded Report
        private AccessNowReportsOperationsModel currentReport;
        public AccessNowReportsOperationsModel CurrentReport
        {
            get { return currentReport; }
            set { currentReport = value; OnPropertyChanged(); }
        }

        // Pagination Model Class
        public PaginationModel Pagination { get; set; }

        // Full List of Records Amount
        private List<DataRow> fullList;
        public List<DataRow> FullList
        {
            get { return fullList; }
            set { fullList = value; OnPropertyChanged(); }
        }

        // List of Paged Results
        private ObservableCollection<DataRow> recordsList;
        public ObservableCollection<DataRow> RecordsList
        {
            get { return recordsList; }
            set { recordsList = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<SearchTerms> searchTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<SearchTerms> SearchTerms
        {
            get { return searchTerms; }
        }

        // Selected Term for ComboBox
        private SearchTerms selectedTerm = new();
        public SearchTerms SelectedTerm
        {
            get { return selectedTerm; }
            set { selectedTerm = value; OnPropertyChanged(); }
        }

        // Holds Object Collection of Reports Type
        private readonly ObservableCollection<AccessNowReportsOperationsModel> availableReports = new()
        {

        };
        public IEnumerable<AccessNowReportsOperationsModel> AvailableReports
        {
            get { return availableReports; }
        }

        // Selected Term for Report Combobox
        private AccessNowReportsOperationsModel selectedReport = new();
        public AccessNowReportsOperationsModel SelectedReport
        {
            get { return selectedReport; }
            set
            {
                selectedReport = value;
                OnPropertyChanged();
            }
        }

        // Bool Controlling Visibility of Date Range Selection
        private bool dateVisibileBool;
        public bool DateVisibileBool
        {
            get { return dateVisibileBool; }
            set { dateVisibileBool = value; OnPropertyChanged(); }
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

        // Holds Object Collection of State Type
        private readonly ObservableCollection<ComboBoxListItem> displayColumns = new()
        {

        };
        public IEnumerable<ComboBoxListItem> DisplayColumns
        {
            get { return displayColumns; }
        }

        // Selected Term for Display Columns Listview
        private ComboBoxListItem selectedDisplayColumn = new();
        public ComboBoxListItem SelectedDisplayColumn
        {
            get { return selectedDisplayColumn; }
            set
            {
                selectedDisplayColumn = value;
                OnPropertyChanged();
            }
        }

        // Binds text of Search box to Variable
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { if (searchText != value) { searchText = value; OnPropertyChanged(); } }
        }

        // No Results Found
        private bool noFoundResults;
        public bool NoFoundResults
        {
            get { return noFoundResults; }
            set { noFoundResults = value; OnPropertyChanged(); }
        }

        // Binds Status Label String
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

        // Binds Status Label String
        private string recordCount;
        public string RecordCount
        {
            get { return recordCount; }
            set
            {
                if (recordCount != value)
                {
                    recordCount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods
        public AccessNowReportsViewModel()
        {
            // Initialize Propeties
            DisplayTable = new();
            DisplayView = new();
            ExportTable = new();
            BindTable = new();
            FilterTable = new();
            RecordsList = new ObservableCollection<DataRow>();
            FullList = new List<DataRow>();
            CurrentReport = (new AccessNowReportsOperationsModel
            {
                DisplayName = "",
                TableName = "",
                ColumnList = new(),
                IsSelected = false,
                URL = ""
            });
            DateVisibileBool = true;
            NoFoundResults = false;
            QATestingTurnedOn = false;
            DevTestingTurnedOn = false;
            FromStartDate = new DateTime(2022, 1, 1);
            ToStartDate = new DateTime(2022, 1, 1);

            FromEndDate = DateTime.Now.AddDays(1);
            ToEndDate = DateTime.Now.AddDays(1);

            FromSelectedDate = DateTime.Now.AddDays(-30);

            ToSelectedDate = DateTime.Now;
            SearchText = "Search";
            RecordCount = "";
            apiPassword = "";
            apiUsername = "";
            apiURL = "";
            FromSelectedDate = DateTime.Now.AddDays(-30);
            CSVExportCommand = new RelayCommand(o => { CSVExport(); });
            HTMLExportCommand = new RelayCommand(o => { HTMLExport(); });
            ClipboardExportCommand = new RelayCommand(o => { ClipboardExport(); });
            LoadReportCommand = new RelayCommand(o => { LoadReport(); });
            SpaceCheckboxCommand = new RelayCommand(o => { SpaceCheckbox(); });
            UpdateColumnsCommand = new RelayCommand(o => { UpdateColumns(); });
            ClearSearch = new RelayCommand(o => { ClearSearchMethod(); });
            SearchEnterKey = new RelayCommand(o => { SearchEnterKeyMethod(); });
            SearchQuery = new RelayCommand(o => { SearchFilter(); });
            ClearQuery = new RelayCommand(o => { ClearResults(); });
            EnterCalculation = new RelayCommand(o => { CalculatePagination(); });
            ExcelExport = new RelayCommand(o => { ExcelExportMethod(); });

            //Initializes Paging Parameters
            Pagination = new PaginationModel(200, 10);
            { };
            Pagination.TotalItems = BindTable.Rows.Count;
            Pagination.ItemsPerPage = 100;
            this.Pagination.PropertyChanged += Pagination_PropertyChanged;
        }
        #endregion

        #region Functions
        // Primary On-Load Function for Page
        public void LoadData()
        {
            try
            {
                SetEnvironmentConfiguration();
                LoadTableNames();
                displayColumns.Clear();
                DateVisibileBool = true;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Sets Reports available based on Access Levels
        public void LoadTableNames()
        {
            try
            {
                availableReports.Clear();

                if (MainViewModel.GlobalAccessNowUserRole == true)
                {
                    availableReports.Add(new AccessNowReportsOperationsModel
                    {
                        DisplayName = "AccessNow Operations View",
                        TableName = ConfigurationProperties.AccessNowOperationsData,
                        HasDateColumn = true,
                        DateColumnName = "requested_on",
                        URL = "/api/v1/requests/advancedsearch"
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Loads the Report selected by the Drop-Down Box
        public async void LoadReport()
        {
            try
            {
                IsLoadBool = true;
                if (SelectedReport.HasDateColumn == true)
                {
                    DateVisibileBool = true;
                }
                else
                {
                    DateVisibileBool = false;
                }
                BindTable.Clear();

                DisplayView = BindTable.DefaultView;

                // Searches With Date Filter if a Date Column Exists
                if (SelectedReport.HasDateColumn == true)
                {
                    DateTime fromDate = Convert.ToDateTime(FromSelectedDate.ToShortDateString());
                    DateTime toDate = Convert.ToDateTime(ToSelectedDate.AddDays(1).ToShortDateString());
                    string passedFromDate = "";
                    string passedToDate = "";

                    passedFromDate = fromDate.ToString("yyyy-MM-dd");
                    passedToDate = toDate.ToString("yyyy-MM-dd");

                    var TableFetch = await DBConn.GetTableWithRange(SelectedReport.TableName, SelectedReport.DateColumnName, passedFromDate, passedToDate); BindTable = TableFetch;
                }

                foreach (DataColumn column in BindTable.Columns)
                {
                    string name = column.ColumnName;
                    string modifiedName = Regex.Replace(name, @"(\s+|\.|\/|\\|\(|\))", "");

                    column.ColumnName = modifiedName;
                }

                ExportTable = BindTable.Copy();

                CurrentReport.DisplayName = SelectedReport.DisplayName;
                CurrentReport.TableName = SelectedReport.TableName;
                CurrentReport.ColumnList.Clear();

                foreach (DataColumn column in ExportTable.Columns)
                {
                    CurrentReport.ColumnList.Add(column.ColumnName);
                }
                SetUpdateColumns(CurrentReport);

                TemporaryTable(ExportTable);
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // On Dev or QA Button Press Changes Environment Configuration
        private void SetEnvironmentConfiguration()
        {
            apiPassword = "";
            apiUsername = "";
            apiURL = "";

            // Set Development, QA,  or Production Ticket Environment
            if (DevTestingTurnedOn == true)
            {
                apiUsername = ConfigurationProperties.AccessNowReportsAPITestUsername;
                apiPassword = ConfigurationProperties.AccessNowReportsAPITestPassword;
                apiURL = ConfigurationProperties.AccessNowAPITestURL;
                StatusString = "Dev Mode";
            }
            else if (QATestingTurnedOn == true)
            {
                apiUsername = ConfigurationProperties.AccessNowReportsAPIQAUsername;
                apiPassword = ConfigurationProperties.AccessNowReportsAPIQAPassword;
                apiURL = ConfigurationProperties.AccessNowAPIQAURL;
                StatusString = "QA Mode";
            }
            else
            {
                apiUsername = ConfigurationProperties.AccessNowReportsAPIUsername;
                apiPassword = ConfigurationProperties.AccessNowReportsAPIPassword;
                apiURL = ConfigurationProperties.AccessNowAPIURL;
                StatusString = "Prod Mode";
            }
        }

        // Set the Columns in the Search Term Combo Box
        public void SetSearchColumns(AccessNowReportsOperationsModel PassedModel)
        {
            try
            {
                searchTerms.Clear();
                searchTerms.Add(new SearchTerms
                {
                    SearchTerm = "All"
                });

                foreach (string column in PassedModel.ColumnList)
                {
                    searchTerms.Add(new SearchTerms
                    {
                        SearchTerm = column
                    });
                }

                SelectedTerm = searchTerms.ElementAt(0);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Set the Columns in the Report Term Combo Box
        public void SetUpdateColumns(AccessNowReportsOperationsModel PassedModel)
        {
            try
            {
                displayColumns.Clear();

                foreach (string column in PassedModel.ColumnList)
                {
                    displayColumns.Add(new ComboBoxListItem
                    {
                        BoxItem = column,
                        IsSelected = true
                    });
                }

                SetSearchColumns(PassedModel);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Adjusts Report Display columns to ones selected in Listview
        public void UpdateColumns()
        {
            try
            {
                DataTable tempTable = new();
                tempTable = BindTable.Copy();

                foreach (ComboBoxListItem column in DisplayColumns)
                {
                    if (column.IsSelected == false)
                    {
                        tempTable.Columns.Remove(column.BoxItem);
                    }
                }
                ExportTable.Clear();
                ExportTable = tempTable.Copy();

                CurrentReport.ColumnList.Clear();

                foreach (DataColumn column in ExportTable.Columns)
                {
                    CurrentReport.ColumnList.Add(column.ColumnName);
                }

                SetSearchColumns(CurrentReport);
                TemporaryTable(ExportTable);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Uses Checkbox on Space Bar Press
        public void SpaceCheckbox()
        {
            try
            {
                foreach (ComboBoxListItem displayColumn in displayColumns.Where(x => x.BoxItem == SelectedDisplayColumn.BoxItem))
                {
                    if (displayColumn.IsSelected == true)
                    {
                        displayColumn.IsSelected = false;
                    }
                    else
                    {
                        displayColumn.IsSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Passed in BindTable for Page Filtering to Full List
        private void TemporaryTable(DataTable passedTable)
        {
            try
            {
                if (passedTable.Rows.Count > 0)
                {
                    ClearTables();
                    if (passedTable.Rows.Count > 20) {  /* Pagination.ItemsPerPage = 20; */ }
                    Pagination.TotalItems = passedTable.Rows.Count;
                    CalculatePagination();
                    List<DataRow> newList = new();
                    for (int i = 0; i < Pagination.TotalItems; i++)
                    {
                        if (passedTable.Rows[i] != null) { newList.Add(passedTable.Rows[i]); }
                    }

                    FullList = newList;
                    ProcessPage(null);
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Caculation on how many pages needed to view ItemsPerPage, and then creates PageList given CurrentPage
        public void CalculatePagination()
        {
            try
            {
                if (Pagination.ItemsPerPage == 0) { Pagination.ItemsPerPage = 1; } // Sets a minimum
                if (Pagination.ItemsPerPage > Pagination.TotalItems) { Pagination.ItemsPerPage = Pagination.TotalItems; }
                if (Pagination.TotalItems != 0)
                {

                    int quotient = Math.DivRem(Pagination.TotalItems, Pagination.ItemsPerPage, out int remainder);
                    Pagination.TotalPages = Pagination.TotalItems / Pagination.ItemsPerPage;
                    if (remainder != 0) { Pagination.TotalPages++; } //Increment by 1 to handle extra values
                }
                Pagination.CurrentPage = 1;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // If Current Page checks triggers new Page List calculation
        private void Pagination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pagination.CurrentPage))
            {
                ProcessPage(null);
            }
        }

        // Clear out Tables and Lists for new Page
        private void ClearTables()
        {
            DisplayTable.Clear();
            FullList.Clear();
            RecordsList.Clear();
        }

        //Full List passed in to Set the Display Page Records set to DisplayView
        private void ProcessPage(object param)
        {
            try
            {
                List<DataRow> pageList = new();

                if (FullList.Count > 0)
                {
                    int startingCount = (Pagination.CurrentPage - 1) * Pagination.ItemsPerPage;

                    for (int i = startingCount; i < startingCount + Pagination.ItemsPerPage; i++)
                    {
                        try
                        {
                            if (i < FullList.Count) { pageList.Add(FullList[i]); }
                        }
                        catch (Exception ex)
                        {
                            ExceptionOutput.Output(ex.ToString());
                            throw;
                        }
                    }
                }

                RecordsList = new ObservableCollection<DataRow>(pageList);
                if (RecordsList.Count > 0)
                {
                    DisplayTable = RecordsList.CopyToDataTable();
                    DisplayView = DisplayTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Export to CSV Button
        public void CSVExport()
        {
            try
            {
                if (ExportTable == null || ExportTable.Columns.Count == 0)
                {
                    // Do Nothing, no Table to Export
                }
                else
                {
                    IsLoadBool = true;

                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files|*.csv",
                        Title = "Save a CSV File",
                        FileName = SelectedReport.DisplayName + "Export.csv"
                    };
                    StreamWriter writer = null;
                    string saveCSV = DataTableToCSV.DataTableCSVConversion(ExportTable, '|');
                    saveFileDialog.InitialDirectory = @"C:\Temp\";
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
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Export to HTML Button
        public void HTMLExport()
        {
            try
            {
                if (ExportTable == null || ExportTable.Columns.Count == 0)
                {
                    // Do Nothing, no Table to Export
                }
                else
                {
                    string export = "";
                    List<String> BuildList = new();
                    IsLoadBool = true;

                    string[] columnNames = ExportTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
                    foreach (string s in columnNames)
                    {
                        BuildList.Add(s);
                    }

                    string built = string.Join(",", BuildList);
                    export = GetHTML(DisplayTable, BuildList);
                    SaveFileDialog dlg = new()
                    {
                        FileName = SelectedReport.DisplayName + "Export",
                        DefaultExt = ".html",
                        Filter = "HTML Documents (.html)|*.html",
                        InitialDirectory = @"C:\Temp\"
                    };

                    Nullable<bool> result = dlg.ShowDialog();

                    if (result == true)
                    {
                        File.WriteAllText(dlg.FileName, export);
                    }

                    IsLoadBool = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
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

        // Export to Clipboard Button
        public void ClipboardExport()
        {
            try
            {
                if (ExportTable == null || ExportTable.Columns.Count == 0)
                {
                    // Do Nothing, no Table to Export
                }
                else
                {
                    var newline = System.Environment.NewLine;
                    var tab = "\t";
                    var clipboard_string = new StringBuilder();
                    IsLoadBool = true;

                    foreach (DataColumn dc in ExportTable.Columns)
                    {
                        clipboard_string.Append(dc.ColumnName + tab);
                    }
                    clipboard_string.Append(newline);

                    foreach (DataRow row in ExportTable.Rows)
                    {
                        foreach (DataColumn dc in ExportTable.Columns)
                        {
                            clipboard_string.Append(row[dc].ToString() + tab);
                        }

                        clipboard_string.Append(newline);
                    }
                    Clipboard.SetText(clipboard_string.ToString());
                    IsLoadBool = false;
                }

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Export Datatable to Excel
        private void ExcelExportMethod()
        {
            try
            {
                if (ExportTable == null || ExportTable.Columns.Count == 0)
                {
                    // Do Nothing, no Table to Export
                }
                else
                {
                    IsLoadBool = true;

                    DataTable ExportTable = new();

                    if (FilterTable.Rows.Count > 0) { ExportTable = FilterTable.Copy(); }

                    else { ExportTable = BindTable.Copy(); }
                    ExportTable.PrimaryKey = null;


                    XLWorkbook wb = new();
                    string exportName = "AccessNow_Operations";
                    wb.Worksheets.Add(ExportTable, exportName);
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        Title = "Save an Excel File",
                        FileName = "AccessNow_Operations.xlsx"
                    };
                    saveFileDialog.ShowDialog();

                    if (!String.IsNullOrWhiteSpace(saveFileDialog.FileName))
                        wb.SaveAs(saveFileDialog.FileName);

                    IsLoadBool = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Clear Search Filter
        private void ClearSearchMethod()
        {
            if (SearchText == "Search")
            {
                SearchText = "";
            }
        }

        //Search on Texbox Enter
        private void SearchEnterKeyMethod()
        {
            SearchFilter();
        }

        // Search Filter on Display View
        private void SearchFilter()
        {
            if (SearchText != "Search" && DisplayTable.Rows.Count > 0)
            {
                try
                {
                    NoFoundResults = false;
                    SearchText = string.Join(" ", SearchText.Split().Where(w => !Utilities.FilteredWords.Contains(w, StringComparer.InvariantCultureIgnoreCase)));
                    string selectedFilter = SelectedTerm.SearchTerm.ToString();
                    string builtFilter = "";
                    string selectedColumn = "";
                    List<string> availableColumns = new();
                    foreach (SearchTerms s in searchTerms)
                    {
                        availableColumns.Add(s.SearchTerm);
                    }
                    availableColumns.RemoveAt(0);

                    // Builds a filter for all columns present in the Search Terms combobox
                    if (selectedFilter == "All")
                    {
                        string queryTerms = "";
                        foreach (string s in availableColumns)
                        {
                            queryTerms = queryTerms + s + " + ";
                        }
                        queryTerms = queryTerms.Remove(queryTerms.Length - 2);
                        builtFilter = queryTerms + "Like '%" + SearchText + "%'";
                    }

                    else
                    {
                        selectedColumn = selectedFilter;
                        string tempFilter = string.Format(" Like '%{0}%'", SearchText);
                        builtFilter = string.Format("convert(" + selectedColumn + ", 'System.String') Like '%{0}%' ", SearchText);
                    }

                    string sort = availableColumns.ElementAt(0);
                    DataView tempView = new(ExportTable, builtFilter, sort, DataViewRowState.CurrentRows);
                    DataTable tempTable = tempView.ToTable();
                    if (tempTable.Rows.Count == 0) { NoFoundResults = true; }
                    TemporaryTable(tempTable);
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            }
        }

        // Prevent Date Selection Errors
        public void DateChangedFunction(string switchDate)
        {
            DateTime fromdate = FromSelectedDate;
            DateTime todate = DateTime.Today;

            if (fromdate > todate || fromdate == todate)
            {
                FromSelectedDate = DateTime.Today;
            }
        }

        // Clears Filter on DisplayView
        public void ClearResults()
        {
            DisplayView.RowFilter = string.Empty;
            NoFoundResults = false;
            TemporaryTable(ExportTable);
            SearchText = "Search";
        }
        #endregion
    }
}

using ClosedXML.Excel;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ReportsViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand CSVExportCommand { get; set; }
        public RelayCommand HTMLExportCommand { get; set; }
        public RelayCommand ClipboardExportCommand { get; set; }
        public RelayCommand ExcelExportCommand { get; set; }
        public RelayCommand UpdateColumnsCommand { get; set; }
        public RelayCommand ImportAccessNowCommand { get; set; }
        public RelayCommand LoadReportCommand { get; set; }
        public RelayCommand SearchQuery { get; set; }
        public RelayCommand ClearSearch { get; set; }
        public RelayCommand SearchEnterKey { get; set; }
        public RelayCommand ClearQuery { get; set; }
        public RelayCommand SpaceCheckboxCommand { get; set; }
        public RelayCommand EnterCalculation { get; set; }
        public RelayCommand ChangePage { get; set; }

        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties
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

        // Tracking Selected Record for Update
        public static string ChosenRecord { get; private set; }

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


        // No Results Found
        private bool noFoundResults;
        public bool NoFoundResults
        {
            get { return noFoundResults; }
            set { noFoundResults = value; OnPropertyChanged(); }
        }

        // Bool Controlling Visibility of Date Range Selection
        private bool dateVisibileBool;
        public bool DateVisibileBool
        {
            get { return dateVisibileBool; }
            set { dateVisibileBool = value; OnPropertyChanged(); }
        }

        // Binds text of Search box to Variable
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { if (searchText != value) { searchText = value; OnPropertyChanged(); } }
        }

        // Binds text of Search box to Variable
        private string dateColumnName;
        public string DateColumnName
        {
            get { return dateColumnName; }
            set { if (dateColumnName != value) { dateColumnName = value; OnPropertyChanged(); } }
        }

        // Binds Text of Current Record Count String
        private string currentRecordCountString;
        public string CurrentRecordCountString
        {
            get { return currentRecordCountString; }
            set { if (currentRecordCountString != value) { currentRecordCountString = value; OnPropertyChanged(); } }
        }

        // Binds Text of Total Records Count String
        private string totalRecordCountString;
        public string TotalRecordCountString
        {
            get { return totalRecordCountString; }
            set { if (totalRecordCountString != value) { totalRecordCountString = value; OnPropertyChanged(); } }
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

        // Holds Object Collection of Reports Type
        private readonly ObservableCollection<ReportModel> availableReports = new()
        {

        };
        public IEnumerable<ReportModel> AvailableReports
        {
            get { return availableReports; }
        }

        // Selected Term for Report Combobox
        private ReportModel selectedReport = new();
        public ReportModel SelectedReport
        {
            get { return selectedReport; }
            set
            {
                selectedReport = value;
                ReportChanged();
                OnPropertyChanged();
            }
        }

        // Finds selected Cell Value
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
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

        // Binds DataTable Columns
        private ObservableCollection<DataGridColumn> columnCollection;
        public ObservableCollection<DataGridColumn> ColumnCollection
        {
            get { return columnCollection; }
            set { columnCollection = value; OnPropertyChanged(); }
        }

        // Table Used to Hold Display Table
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

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
        }

        // Access Now Import Button Visbility Binding
        private bool accessNowImportButtonVisible;
        public bool AccessNowImportButtonVisible
        {
            get { return accessNowImportButtonVisible; }
            set { accessNowImportButtonVisible = value; OnPropertyChanged(); }
        }

        // Table Used to Hold Search Filter Table for Export
        private DataTable filterTable;
        public DataTable FilterTable
        {
            get { return filterTable; }
            set { filterTable = value; OnPropertyChanged(); }
        }

        // Binds the Currently Loaded Report
        private ReportModel currentReport;
        public ReportModel CurrentReport
        {
            get { return currentReport; }
            set { currentReport = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public ReportsViewModel()
        {
            // Initialize Propeties
            DisplayTable = new();
            DisplayView = new();
            ExportTable = new();
            BindTable = new();
            FilterTable = new DataTable();
            RecordsList = new ObservableCollection<DataRow>();
            FullList = new List<DataRow>();
            CurrentReport = (new ReportModel
                {
                DisplayName = "",
                TableName = "",
                JoinedTable = "",
                ColumnList = new (),
                IsSelected = false
                });
            NoFoundResults = false;
            DateVisibileBool = true;
            AccessNowImportButtonVisible = false;
            SearchText = "Search";
            FromStartDate = new DateTime(2019, 1, 1);
            FromEndDate = new DateTime(2099, 1, 1);
            ToStartDate = new DateTime(2019, 1, 1);
            ToEndDate = new DateTime(2099, 1, 1);
            FromSelectedDate = DateTime.Now.AddDays(-30);
            ToSelectedDate = DateTime.Now;
            CurrentRecordCountString = "0";
            TotalRecordCountString = "0";

            //Initializes Paging Parameters
            Pagination = new PaginationModel(200, 10);
            { };
            Pagination.TotalItems = BindTable.Rows.Count;
            Pagination.ItemsPerPage = 10;
            this.Pagination.PropertyChanged += Pagination_PropertyChanged;

            //Initialize Relay Commands
            CSVExportCommand = new RelayCommand(o => { CSVExport(); });
            HTMLExportCommand = new RelayCommand(o => { HTMLExport(); });
            ClipboardExportCommand = new RelayCommand(o => { ClipboardExport(); });
            ExcelExportCommand = new RelayCommand(o => { ExcelExport(); });
            UpdateColumnsCommand = new RelayCommand(o => { UpdateColumns(); });
            LoadReportCommand = new RelayCommand(o => { LoadReport(); });
            SpaceCheckboxCommand = new RelayCommand(o => { SpaceCheckbox(); });
            EnterCalculation = new RelayCommand(o => { CalculatePagination(); });
            ChangePage = new RelayCommand((parameter) => ChangePageMethod(parameter));
            SearchEnterKey = new RelayCommand(o => { SearchEnterKeyMethod(); });
            SearchQuery = new RelayCommand(o => { SearchFilter(); });
            ClearQuery = new RelayCommand(o => { ClearResults(); });
            ClearSearch = new RelayCommand(o => { ClearSearchMethod(); });
            ImportAccessNowCommand = new RelayCommand(o => { ImportAccessNow(); });

            

        }
        #endregion

        #region Functions
        // Primary On-Load Function for Page
        public void LoadData()
        {
            try
            {
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

                // Unix Permission Role
                if (MainViewModel.UnixTablePermissionGlobal == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Unix Users Table",
                        TableName = ConfigurationProperties.UsersTable,
                        HasDateColumn = false,
                        DateColumnName = "None"
                    });

                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Unix Groups Table",
                        TableName = ConfigurationProperties.GroupsTable,
                        HasDateColumn = false,
                        DateColumnName = "None"
                    });
                }

                // Basic Factool Permission Role
                if (MainViewModel.GlobalFacToolPermission == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Incidents Table",
                        TableName = ConfigurationProperties.IAMHSNOWIncidentsView,
                        HasDateColumn = true,
                        DateColumnName = "Created"
                    });


                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Provisioning Table",
                        TableName = ConfigurationProperties.IAMHSNOWProvisioningView,
                        HasDateColumn = true,
                        DateColumnName = "sys_created_on"
                    });
                }

                // Access Now Permission on AccessNow User Role, or Factool Mgr Role
                if (MainViewModel.GlobalAccessNowUserRole == true || MainViewModel.GlobalFacToolMgrPermission)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Access Now Table",
                        TableName = ConfigurationProperties.AccessNowOperationsData,
                        HasDateColumn = true,
                        DateColumnName = "requested_on"
                    });
                }

                // AccessNow Mgr Role
                if (MainViewModel.GlobalAccessNowMgrRole == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Access Now History Table",
                        TableName = ConfigurationProperties.AccessNowHistory,
                        HasDateColumn = true,
                        DateColumnName = "ModifiedDate"
                    });
                }

                // Factool Mgr Role
                if (MainViewModel.GlobalFacToolMgrPermission == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Infrastructure Mailbox Table",
                        TableName = ConfigurationProperties.FactoolJoinedView,
                        HasDateColumn = true,
                        DateColumnName = "CreateDate"
                    });

                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Escalation Expedite Table",
                        TableName = ConfigurationProperties.IAMHSolutionCenterEscalations,
                        HasDateColumn = true,
                        DateColumnName = "Created"
                    });

                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Factool Expected Table",
                        TableName = ConfigurationProperties.FactoolExpectedView,
                        HasDateColumn = true,
                        DateColumnName = "StartDate"
                    });
                }

                // Service Catalog User Role
                if (MainViewModel.GlobalServiceCatalogEnabled == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "Service Catalog Table",
                        TableName = ConfigurationProperties.ServiceCatalogTable,
                        HasDateColumn = false,
                        DateColumnName = "None"
                    });
                }

                // Service Catalog Mgr Role
                if (MainViewModel.GlobalServiceCatalogMgrEnabled == true)
                {
                    availableReports.Add(new ReportModel
                    {
                        DisplayName = "RMR Data Table",
                        TableName = ConfigurationProperties.IAMHRMRData,
                        HasDateColumn = true,
                        DateColumnName = "creation_date"
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

                // Searches either the Single Specific Table, or a Joined Table Query
                if (SelectedReport.DisplayName == "Request Joined Table") 
                { 
                    if (SelectedReport.HasDateColumn == true)
                    {
                        DateTime fromDate = Convert.ToDateTime(FromSelectedDate.ToShortDateString());
                        DateTime toDate = Convert.ToDateTime(ToSelectedDate.AddDays(1).ToShortDateString());
                        string passedFromDate = fromDate.ToString("yyyy/MM/dd");
                        string passedToDate = toDate.ToString("yyyy/MM/dd");
                        var TableFetch = await DBConn.GetJoinedRequestTableWithRange(SelectedReport.TableName, SelectedReport.JoinedTable, SelectedReport.DateColumnName, passedFromDate, passedToDate); BindTable = TableFetch;
                    }
                    else
                    {
                        var TableFetch = await DBConn.GetJoinedRequestTable(SelectedReport.TableName, SelectedReport.JoinedTable, ".ReferenceNumber");
                        BindTable = TableFetch;
                    }
                }
                else 
                { 
                    if (SelectedReport.HasDateColumn == true)
                    {
                        DateTime fromDate = Convert.ToDateTime(FromSelectedDate.ToShortDateString());
                        DateTime toDate = Convert.ToDateTime(ToSelectedDate.AddDays(1).ToShortDateString());
                        string passedFromDate = "";
                        string passedToDate = "";
                        if (SelectedReport.DisplayName == "Access Now Table")
                        {
                            passedFromDate = fromDate.ToString("yyyy-MM-dd");
                            passedToDate = toDate.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            passedFromDate = fromDate.ToString("yyyy/MM/dd");
                            passedToDate = toDate.ToString("yyyy/MM/dd");
                        }

                        var TableFetch = await DBConn.GetTableWithRange(SelectedReport.TableName, SelectedReport.DateColumnName, passedFromDate, passedToDate); BindTable = TableFetch;
                    }
                    else
                    {
                        var TableFetch = await DBConn.GetTableAsync(SelectedReport.TableName); BindTable = TableFetch;
                    }
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

        // Set the Columns in the Search Term Combo Box
        public void SetSearchColumns(ReportModel PassedModel)
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
        public void SetUpdateColumns(ReportModel PassedModel)
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

        // Converts a CSV File to a Datatable
        public async Task<DataTable> ConvertCSVToDataTable (string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();

            try
            {
                await Task.Run(() =>
                {
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return dt;
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
                        builtFilter = string.Format("convert(" + selectedColumn + ", 'System.String') Like '%{0}%' ", SearchText) ;
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

        // Export to Excel Button
        public void ExcelExport()
        {
            try
            {
                if (DisplayTable == null || DisplayTable.Columns.Count == 0)
                {
                    // Do Nothing, no Table to Export
                }
                else
                {
                    IsLoadBool = true;
                    XLWorkbook wb = new();
                    string exportName = SelectedReport.DisplayName + "ExportSheet";
                    wb.Worksheets.Add(DisplayTable, exportName);
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        Title = "Save an Excel File",
                        FileName = exportName + ".xlsx"
                    };

                    saveFileDialog.InitialDirectory = @"C:\Temp\";
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
                    SaveFileDialog dlg = new();
                    dlg.FileName = SelectedReport.DisplayName + "Export";
                    dlg.DefaultExt = ".html";
                    dlg.Filter = "HTML Documents (.html)|*.html";
                    dlg.InitialDirectory = @"C:\Temp\";

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

        // Clear out Tables and Lists for new Page
        private void ClearTables()
        {
            DisplayTable.Clear();
            FullList.Clear();
            RecordsList.Clear();
        }

        // Clears Filter on DisplayView
        public void ClearResults()
        {
            DisplayView.RowFilter = string.Empty;
            NoFoundResults = false;
            TemporaryTable(ExportTable);
            SearchText = "Search";
        }

        // Passed in BindTable for Page Filtering to Full List
        private void TemporaryTable(DataTable passedTable)
        {
            try
            {
                if (passedTable.Rows.Count > 0)
                {
                    ClearTables();
                    if (passedTable.Rows.Count > 20) { Pagination.ItemsPerPage = 20; }
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

        //Search on Texbox Enter
        private void SearchEnterKeyMethod()
        {
            SearchFilter();
        }

        // Moving forward or backwards changes PageList
        private void ChangePageMethod(object obj)
        {
            try
            {
                int parameter = int.Parse((string)obj);
                int newpage = Pagination.CurrentPage;
                switch (parameter)
                {
                    case 0:
                        newpage--;
                        if (newpage < 1) { newpage = Pagination.TotalPages; }
                        break;

                    case 1:
                        newpage++;
                        if (newpage > Pagination.TotalPages) { newpage = 1; }
                        break;
                }
                Pagination.CurrentPage = newpage;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
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

        // Clear Search Filter
        private void ClearSearchMethod()
        {
            if (SearchText == "Search")
            {
                SearchText = "";
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

        // On Selected Report Changed, Set Configuration Elements
        public void ReportChanged()
        {
            if (SelectedReport.DisplayName == "Access Now Table")
            {
                if (MainViewModel.GlobalAccessNowMgrRole == true)
                {
                    AccessNowImportButtonVisible = true;
                }
            }
            else
            {
                AccessNowImportButtonVisible = false;
            }
        }

        // Import Access Now Function from CSV File
        public async void ImportAccessNow()
        {
            try
            {
                if (MessageBox.Show("Are you certain you want to update the Access Now Database?", "Update Database??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    OpenFileDialog choofdlog = new OpenFileDialog();
                    choofdlog.Title = "Import Access Now Report";
                    choofdlog.Filter = "CSV files|*.csv";
                    choofdlog.InitialDirectory = @"C:\Temp\";
                    choofdlog.FilterIndex = 1;
                    choofdlog.Multiselect = false;
                    string sFileName = "";

                    if (choofdlog.ShowDialog() == true)
                    {
                        sFileName = choofdlog.FileName;
                    }

                    IsLoadBool = true;

                    var returnTable = await ConvertCSVToDataTable(sFileName);

                    DataTable tempTable = returnTable;
                    bool tableCheck = false;

                    // Determine if Imported Table is Correct Access Now Formatted Columns
                    foreach (DataColumn dc in tempTable.Columns)
                    {
                        if (dc.ColumnName == "request_id")
                        {
                            tableCheck = true;
                        }
                    }

                    AccessNowDataItemModel LastItem = new()
                    {
                        Request_id = 1,
                        Requested_on = "",
                        Request_type = "",
                        Status = "",
                        Closed_on = "",
                        Asset = "",
                        Access = "",
                        Sub_request_type = "",
                        Account_name = "",
                        Requestor_name = "",
                        Requestor_employee_id = "",
                        Ticket_id = "",
                        Ticket_status = "",
                        Mail_to = "",
                        Mail_sent_on = "",
                        Mail_sent_on_date = ""
                    };

                    int updatedRecords = 0;
                    int addedRecords = 0;

                    if (tableCheck == true)
                    {
                        List<long> recordList = new();
                        List<double> doubleList = new();
                        doubleList = DBConn.GetColumnListDouble(ConfigurationProperties.AccessNowData, "request_id");

                        foreach (double d in doubleList)
                        {
                            recordList.Add(Convert.ToInt64(d));
                        }

                        List<AccessNowDataItemModel> NewItems = new();
                        List<AccessNowDataItemModel> UpdateItems = new();

                        int totalRecords = tempTable.Rows.Count;
                        TotalRecordCountString = totalRecords.ToString();
                        int currentRecord = 0;
                        CurrentRecordCountString = currentRecord.ToString();


                        await Task.Run(() =>
                        {
                            foreach (DataRow row in tempTable.Rows)
                            {
                                AccessNowDataItemModel CurrentItem = new()
                                {
                                    Request_id = long.Parse(row["request_id"].ToString()),
                                    Requested_on = row["requested_on"].ToString(),
                                    Request_type = row["request_type"].ToString(),
                                    Status = row["status"].ToString(),
                                    Closed_on = row["closed_on"].ToString(),
                                    Asset = row["asset"].ToString(),
                                    Access = row["access"].ToString(),
                                    Sub_request_type = row["sub_request_type"].ToString(),
                                    Account_name = row["account_name"].ToString(),
                                    Requestor_name = row["requestor_name"].ToString(),
                                    Requestor_employee_id = row["requestor_employee_id"].ToString(),
                                    Ticket_id = row["ticket_id"].ToString(),
                                    Ticket_status = row["ticket_status"].ToString(),
                                    Mail_to = row["mail_to"].ToString(),
                                    Mail_sent_on = row["mail_sent_on"].ToString(),
                                    Mail_sent_on_date = row["mail_sent_on_date"].ToString()
                                };

                                // Check if Request ID Exists, if not Inserts New Record. If so updates the Current Record

                                if (recordList.Contains(CurrentItem.Request_id))
                                {
                                    currentRecord++;
                                    CurrentRecordCountString = currentRecord.ToString();
                                    UpdateItems.Add(CurrentItem);
                                    updatedRecords++;
                                }
                                else
                                {
                                    currentRecord++;
                                    CurrentRecordCountString = currentRecord.ToString();
                                    NewItems.Add(CurrentItem);
                                    addedRecords++;
                                }


                            }
                        });

                        // Primary Bulk SQL Insert 
                        await Task.Run(() =>
                        {
                            DBConn.AddAccessNowDataRecordBulk(NewItems, ConfigurationProperties.AccessNowData);
                        });

                        // Primary Bulk SQL Update 
                        await Task.Run(() =>
                        {
                            DBConn.AccessNowDataUpdateBulk(UpdateItems, ConfigurationProperties.AccessNowData);
                        });


                        UserPrincipal userPrincipal = UserPrincipal.Current;
                        string createdTime = "";
                        DateTime createDate = DateTime.UtcNow; createdTime = createDate.ToString("yyyy/MM/dd h:mm:ss.fff");

                        string ChangedString = "User " + userPrincipal.Name + " on " + createdTime + " Added " + addedRecords.ToString() + " Records, and Updated " + updatedRecords.ToString() + " Records.";

                        AccessNowHistoryItemModel HistoryItem = new()
                        {
                            HistoryID = 0,
                            ModifiedBy = userPrincipal.Name,
                            ModifiedDate = createdTime,
                            Changes = ChangedString
                        };

                        DBConn.AddAccessNowHistory(HistoryItem, ConfigurationProperties.AccessNowHistory);

                        LoadReport();
                    }

                    if (tableCheck == false)
                    {
                        MessageBox.Show("Invalid Access Now Import CSV File. Wrong Format.");
                    }

                    IsLoadBool = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
        #endregion
    }
}

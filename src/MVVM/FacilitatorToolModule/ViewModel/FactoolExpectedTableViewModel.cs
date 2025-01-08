using ClosedXML.Excel;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FactoolExpectedTableViewModel : BaseViewModel
    {
        #region Delegates
        //Access SQL Methods Class
        private readonly SQLMethods DBConn = new();

        //Relay Commands for Buttons
        public RelayCommand LoadTable { get; set; }
        public RelayCommand SearchQuery { get; set; }
        public RelayCommand ClearQuery { get; set; }
        public RelayCommand ClearSearch { get; set; }
        public RelayCommand RecordSelect { get; set; }
        public RelayCommand ChangePage { get; set; }
        public RelayCommand EnterCalculation { get; set; }
        public RelayCommand ExcelExport { get; set; }
        public RelayCommand DeleteClickCommand { get; set; }
        public RelayCommand HTMLExport { get; set; }
        public RelayCommand SearchEnterKey { get; set; }
        public RelayCommand AddRecordCommand { get; set; }
        #endregion

        #region Properties
        // Current UserControl View
        private BaseViewModel _selectedViewModel;
        public BaseViewModel SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { _selectedViewModel = value; OnPropertyChanged(); }
        }

        // Users Table View
        private FactoolExpectedUpdateViewModel factoolTableUpdateVm;
        public FactoolExpectedUpdateViewModel FactoolTableUpdateVm
        {
            get { return factoolTableUpdateVm; }
            set { factoolTableUpdateVm = value; OnPropertyChanged(); }
        }

        // Users Add Record View
        private FactoolExpectedAddViewModel factoolAddRecordVm;
        public FactoolExpectedAddViewModel FactoolAddRecordVm
        {
            get { return factoolAddRecordVm; }
            set { factoolAddRecordVm = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value to open Record
        private DataGridCellInfo cellInfo;
        public DataGridCellInfo CellInfo
        {
            get { return cellInfo; }
            set { cellInfo = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<SearchTerms> searchTerms = new()
        {
            new SearchTerms { SearchTerm = "All" },
            new SearchTerms { SearchTerm = "_iD" },
            new SearchTerms { SearchTerm = "ClassName" },
            new SearchTerms { SearchTerm = "StartDate" },
            new SearchTerms { SearchTerm = "N_Number" },
            new SearchTerms { SearchTerm = "PERSONA" },
            new SearchTerms { SearchTerm = "ExpectedUsers" },
            new SearchTerms { SearchTerm = "ExpectedApps" },
            new SearchTerms { SearchTerm = "ExpectedToucpoints" },
            new SearchTerms { SearchTerm = "ClassOwner" }
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

        public bool filterApplied = false;

        // Binds text of Search box to Variable
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { if (searchText != value) { searchText = value; OnPropertyChanged(); } }
        }

        // Binds Local Table to Global Table
        private DataTable bindTable;
        public DataTable BindTable
        {
            get { return bindTable; }
            set { bindTable = value; OnPropertyChanged(); }
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

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
        }

        // No Results Found
        private bool noFoundResults;
        public bool NoFoundResults
        {
            get { return noFoundResults; }
            set { noFoundResults = value; OnPropertyChanged(); }
        }

        // List of Paged Results
        private ObservableCollection<DataRow> recordsList;
        public ObservableCollection<DataRow> RecordsList
        {
            get { return recordsList; }
            set { recordsList = value; OnPropertyChanged(); }
        }

        // Table Used to Hold Paged Display Data
        private DataTable displayTable;
        public DataTable DisplayTable
        {
            get { return displayTable; }
            set { displayTable = value; OnPropertyChanged(); }
        }

        // Object Holding Export Column Properties
        public ExportColumns ExportColumnList { get; set; }

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
        #endregion

        #region Methods
        public FactoolExpectedTableViewModel()
        {
            // Initialize Properties
            BindTable = new DataTable();
            BindTable = MainViewModel.UsersTable.Copy();
            RecordsList = new ObservableCollection<DataRow>();
            FullList = new List<DataRow>();
            DisplayTable = new DataTable();
            FilterTable = new DataTable();
            DisplayView = new DataView();
            SearchText = "Search";
            IsLoadBool = false;
            NoFoundResults = false;
            MSG.Messenger.Default.Register<String>(this, MessageReload);

            //Initializes Paging Parameters
            Pagination = new PaginationModel(200, 10);
            { };
            Pagination.TotalItems = BindTable.Rows.Count;
            Pagination.ItemsPerPage = 10;
            this.Pagination.PropertyChanged += Pagination_PropertyChanged;

            //Initialize Relay Commands
            RecordSelect = new RelayCommand(o => { RecordSelectMethod(); });
            LoadTable = new RelayCommand(o => { LoadTableMethod(); });
            EnterCalculation = new RelayCommand(o => { CalculatePagination(); });
            ChangePage = new RelayCommand((parameter) => ChangePageMethod(parameter));
            SearchQuery = new RelayCommand(o => { SearchFilter(); });
            ClearQuery = new RelayCommand(o => { ClearResults(); });
            ExcelExport = new RelayCommand(o => { ExcelExportMethod(); });
            DeleteClickCommand = new RelayCommand(o => { DeleteRecordClick(); });
            HTMLExport = new RelayCommand(o => { HtmlExport(); });
            ClearSearch = new RelayCommand(o => { ClearSearchMethod(); });
            SearchEnterKey = new RelayCommand(o => { SearchEnterKeyMethod(); });
            AddRecordCommand = new RelayCommand(o => { AddRecord(); });

            //Initialize Export Column List
            ExportColumnList = new ExportColumns
            {
                Column1 = "_iD",
                Column1IsChecked = true,
                Column2 = "ClassName",
                Column2IsChecked = true,
                Column3 = "Start Date",
                Column3IsChecked = true,
                Column4 = "N_Number",
                Column4IsChecked = true,
                Column5 = "PERSONA",
                Column5IsChecked = true,
                Column6 = "ExpectedUsers",
                Column6IsChecked = true,
                Column7 = "ExpectedApps",
                Column7IsChecked = true,
                Column8 = "ExpectedTouchpoints",
                Column8IsChecked = true,
                Column9 = "ClassOwner",
                Column9IsChecked = true,
                ExportEverything = true
            };
        }
        #endregion

        #region Functions
        public async void LoadData()
        {
            try
            {
                NoFoundResults = false;
                if (MainViewModel.FactoolExpectedTable.Rows.Count < 10)
                {
                    MainViewModel.FactoolExpectedTable.Clear();
                    IsLoadBool = true;
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolExpectedNumbers);
                    MainViewModel.FactoolExpectedTable = dataTask;
                }
                BindTableBuild();
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Copy Global Table to Bind Table
        private void BindTableBuild()
        {
            BindTable.Clear();
            BindTable = MainViewModel.FactoolExpectedTable.Copy();
            TemporaryTable(BindTable);
        }

        // Manual Data Reload on Button Click
        public async void LoadDatabase()
        {
            NoFoundResults = false;
            try
            {
                MainViewModel.FactoolExpectedTable.Clear();
                DisplayView = MainViewModel.FactoolExpectedTable.DefaultView;
                IsLoadBool = true;
                var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolExpectedNumbers);
                MainViewModel.FactoolExpectedTable = dataTask;
                IsLoadBool = false;
                Pagination.ItemsPerPage = 20;
                BindTableBuild();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Reload Database from SQL Server
        private void LoadTableMethod()
        {
            if (MessageBox.Show("Reload Database? This takes time.", "Reload Database?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                BindTable.Clear();
                DisplayView = BindTable.DefaultView;
                LoadDatabase();
            }
            else
            {

            }
        }

        // Search Filter on Display View
        private void SearchFilter()
        {
            if (SearchText != "Search" && DisplayTable.Rows.Count > 0)
            {
                try
                {
                    string selectedFilter = SelectedTerm.SearchTerm.ToString();
                    string builtFilter = "";
                    string selectedColumn = "";
                    NoFoundResults = false;
                    SearchText = string.Join(" ", SearchText.Split().Where(w => !Utilities.FilteredWords.Contains(w, StringComparer.InvariantCultureIgnoreCase)));

                    // Switch based on selected filter in ComboBox, Builds Filter off SearchText TextBox = selectedFilter Category
                    switch (selectedFilter)
                    {
                        case "All":
                            builtFilter = "_iD + ClassName + StartDate + N_Number + PERSONA + ExpectedUsers + ExpectedApps + ExpectedTouchpoints + ClassOwner Like '%" + SearchText + "%'";
                            break;

                        case "_iD":
                            string fixedText = new(SearchText.Where(c => char.IsDigit(c)).ToArray());
                            SearchText = fixedText;

                            selectedColumn = "_iD";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "ClassName":
                            selectedColumn = "ClassName";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "StartDate":
                            selectedColumn = "StartDate";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "N_Number":
                            selectedColumn = "N_Number";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "PERSONA":
                            selectedColumn = "PERSONA";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ExpectedUsers":
                            selectedColumn = "ExpectedUsers";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ExpectedApps":
                            selectedColumn = "ExpectedApps";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ExpectedTouchpoints":
                            selectedColumn = "ExpectedTouchpoints";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ClassOwner":
                            selectedColumn = "ClassOwner";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;
                    }

                    string sort = "_iD";
                    DataView tempView = new(BindTable, builtFilter, sort, DataViewRowState.CurrentRows);
                    DataTable tempTable = tempView.ToTable();
                    FilterTable.Clear();
                    FilterTable = tempView.ToTable();
                    if (tempTable.Rows.Count == 0) { NoFoundResults = true; }
                    TemporaryTable(tempTable);
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
            }
        }

        // Delete User Record From Context Menu  ->  START HERE NEEDS WORK
        public async void DeleteRecordClick()
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
                            string record = row["_iD"].ToString();
                            string UpdateValue = record;
                            DBConn.DeleteMatchingRecords(ConfigurationProperties.FactoolExpectedNumbers, "_id", UpdateValue);
                            List<DataRow> rowsToDelete = new();
                            foreach (DataRow dr in MainViewModel.FactoolExpectedTable.Rows)
                            {
                                if (dr["_iD"].ToString() == UpdateValue)
                                {
                                    rowsToDelete.Add(dr);
                                }
                            }
                            foreach (DataRow dr in rowsToDelete)
                            {
                                MainViewModel.FactoolExpectedTable.Rows.Remove(dr);
                            }
                            await SetSelectedViewModelToNullAsync();
                            BindTableBuild();
                        }
                    }
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
        
        //Search on Texbox Enter
        private void SearchEnterKeyMethod()
        {
            SearchFilter();
        }

        // Clear Search Filter
        private void ClearSearchMethod()
        {
            if (SearchText == "Search")
            {
                SearchText = "";
            }
        }

        // String List holding Columns to Export
        public List<String> BuildExportList()
        {
            List<String> exportList = new();
            if (ExportColumnList.Column1IsChecked == true) { exportList.Add("_iD"); };
            if (ExportColumnList.Column2IsChecked == true) { exportList.Add("ClassName"); };
            if (ExportColumnList.Column3IsChecked == true) { exportList.Add("StartDate"); };
            if (ExportColumnList.Column4IsChecked == true) { exportList.Add("N_Number"); };
            if (ExportColumnList.Column5IsChecked == true) { exportList.Add("PERSONA"); };
            if (ExportColumnList.Column6IsChecked == true) { exportList.Add("ExpectedUsers"); };
            if (ExportColumnList.Column7IsChecked == true) { exportList.Add("ExpectedApps"); };
            if (ExportColumnList.Column8IsChecked == true) { exportList.Add("ExpectedTouchpoints"); };
            if (ExportColumnList.Column9IsChecked == true) { exportList.Add("ClassOwner"); };
            return exportList;
        }

        // String List holding Columns to remove from Export
        public List<String> BuildRemoveExportList()
        {
            List<String> exportRemoveList = new();
            if (ExportColumnList.Column1IsChecked == false) { exportRemoveList.Add("_iD"); };
            if (ExportColumnList.Column2IsChecked == false) { exportRemoveList.Add("ClassName"); };
            if (ExportColumnList.Column3IsChecked == false) { exportRemoveList.Add("StartDate"); };
            if (ExportColumnList.Column4IsChecked == false) { exportRemoveList.Add("N_Number"); };
            if (ExportColumnList.Column5IsChecked == false) { exportRemoveList.Add("PERSONA"); };
            if (ExportColumnList.Column6IsChecked == false) { exportRemoveList.Add("ExpectedUsers"); };
            if (ExportColumnList.Column7IsChecked == false) { exportRemoveList.Add("ExpectedApps"); };
            if (ExportColumnList.Column8IsChecked == false) { exportRemoveList.Add("ExpectedTouchpoints"); };
            if (ExportColumnList.Column9IsChecked == false) { exportRemoveList.Add("ClassOwner"); };
            return exportRemoveList;
        }

        //Export HTML Table
        private void HtmlExport()
        {
            try
            {
                string export = "";
                List<String> RemoveList = BuildRemoveExportList();
                List<String> BuildList = BuildExportList();
                DataTable ExportTable = new();
                if (ExportColumnList.ExportEverything == true) { ExportTable = BindTable.Copy(); }
                else if (FilterTable.Rows.Count > 0) { ExportTable = FilterTable.Copy(); }
                else { ExportTable = BindTable.Copy(); }
                ExportTable.PrimaryKey = null;
                foreach (string s in RemoveList)
                {
                    ExportTable.Columns.Remove(s);
                }
                export = GetHTML(ExportTable, BuildList);
                SaveFileDialog dlg = new();
                dlg.FileName = "FactoolExpectedExport";
                dlg.DefaultExt = ".html";
                dlg.Filter = "HTML Documents (.html)|*.html";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    File.WriteAllText(dlg.FileName, export);
                }
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

        // Export Datatable to Excel
        private void ExcelExportMethod()
        {
            try
            {
                if (BindTable == null || BindTable.Columns.Count == 0)
                {
                    throw new Exception("ExportToExcel: Null or empty input table!\n");
                }
                else
                {
                    List<String> RemoveList = BuildRemoveExportList();
                    DataTable ExportTable = new();

                    if (ExportColumnList.ExportEverything == true) { ExportTable = BindTable.Copy(); }
                    else if (FilterTable.Rows.Count > 0) { ExportTable = FilterTable.Copy(); }
                    else { ExportTable = BindTable.Copy(); }
                    ExportTable.PrimaryKey = null;
                    foreach (string s in RemoveList)
                    {
                        ExportTable.Columns.Remove(s);
                    }

                    XLWorkbook wb = new();
                    string exportName = "FactoolExpectedDatabase";
                    wb.Worksheets.Add(ExportTable, exportName);
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        Title = "Save an Excel File",
                        FileName = "FactoolExpectedExcelBackup.xlsx"
                    };

                    saveFileDialog.ShowDialog();

                    if (!String.IsNullOrWhiteSpace(saveFileDialog.FileName))
                        wb.SaveAs(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Opens Update View with selected Row Record
        private async void RecordSelectMethod()
        {
            try
            {
                DataRowView drv = (DataRowView)CellInfo.Item;
                if (drv != null)
                {
                    DataRow row = drv.Row;
                    if (row != null)
                    {
                        await SetSelectedViewModelToNullAsync();
                        string record = row["_iD"].ToString();
                        FactoolExpectedTableViewModel.ChosenRecord = record;
                        FactoolTableUpdateVm = new FactoolExpectedUpdateViewModel();
                        FactoolTableUpdateVm.LoadData();
                        SelectedViewModel = FactoolTableUpdateVm;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Async Clear View Models 
        public async Task SetSelectedViewModelToNullAsync()
        {
            SelectedViewModel = null;
            FactoolTableUpdateVm = null;
            await Task.Delay(1);
        }

        // Add Record Command, Open Add Record View
        private void AddRecord()
        {
            try
            {
                FactoolAddRecordVm = null;
                SelectedViewModel = null;
                FactoolAddRecordVm = new FactoolExpectedAddViewModel();
                FactoolAddRecordVm.LoadData();
                SelectedViewModel = FactoolAddRecordVm;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Clears Filter on DisplayView
        public void ClearResults()
        {
            DisplayView.RowFilter = string.Empty;
            NoFoundResults = false;
            TemporaryTable(BindTable);
            SearchText = "Search";
        }

        // Clear out Tables and Lists for new Page
        private void ClearTables()
        {
            DisplayTable.Clear();
            FullList.Clear();
            RecordsList.Clear();
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
        #endregion

        #region Messages
        // Listen to New Record for Database Reload
        public async void MessageReload(string passedMessage)
        {
            if (passedMessage == "Expected Table Reload") 
            {
                BindTable.Clear();
                DisplayView = BindTable.DefaultView;
                LoadDatabase();
            }
        }
        #endregion
    }
}

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
using System.Windows;
using System.Windows.Controls;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class GroupsTableViewModel : BaseViewModel
    {
        #region Delegates
        readonly SQLMethods DBConn = new();

        // Relay Commands
        public RelayCommand GroupSelectRecordCommand { get; set; }
        public RelayCommand ClearSearchCommand { get; set; }
        public RelayCommand ClearQuery { get; set; }
        public RelayCommand SearchQuery { get; set; }
        public RelayCommand SearchEnterKey { get; set; }
        public RelayCommand ExcelExport { get; set; }
        public RelayCommand HTMLExport { get; set; }
        public RelayCommand LoadTable { get; set; }
        public RelayCommand DeleteClickCommand { get; set; }
        public RelayCommand ChangePage { get; set; }
        public RelayCommand EnterCalculation { get; set; }
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
        private GroupsUpdateRecordViewModel groupsTableUpdateVm;
        public GroupsUpdateRecordViewModel GroupsTableUpdateVm
        {
            get { return groupsTableUpdateVm; }
            set { groupsTableUpdateVm = value; OnPropertyChanged(); }
        }

        // Users Add Record View
        private GroupsAddRecordViewModel groupsAddRecordVm;
        public GroupsAddRecordViewModel GroupsAddRecordVm
        {
            get { return groupsAddRecordVm; }
            set { groupsAddRecordVm = value; OnPropertyChanged(); }
        }

        private UserInfo currentUserInfo;
        // Get Current User Information
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<SearchTerms> searchFilterTerms = new()
        {
            new SearchTerms { SearchTerm = "All" },
            new SearchTerms { SearchTerm = "GID" },
            new SearchTerms { SearchTerm = "Group" },
            new SearchTerms { SearchTerm = "LOB" }
        };

        // Basis for Search Terms Collection
        public IEnumerable<SearchTerms> SearchFilterTerms
        {
            get { return searchFilterTerms; }
        }

        // Selected Term for ComboBox
        private SearchTerms selectedFilterTerm = new();
        public SearchTerms SelectedFilterTerm
        {
            get { return selectedFilterTerm; }
            set { selectedFilterTerm = value; OnPropertyChanged(); }
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

        // Tracking Selected Record for Update
        public static string ChosenRecord { get; set; }

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

        // Table Used to Hold Search Filter Table for Export
        private DataTable filterTable;
        public DataTable FilterTable
        {
            get { return filterTable; }
            set { filterTable = value; OnPropertyChanged(); }
        }

        // Object Holding Export Column Properties
        public ExportColumns ExportColumnList { get; set; }

        // List of Paged Results
        private ObservableCollection<DataRow> recordsList;
        public ObservableCollection<DataRow> RecordsList
        {
            get { return recordsList; }
            set { recordsList = value; OnPropertyChanged(); }
        }

        // Finds selected Cell Value for Group Ticket Record
        private DataGridCellInfo groupCellInfo;
        public DataGridCellInfo GroupCellInfo
        {
            get { return groupCellInfo; }
            set { groupCellInfo = value; OnPropertyChanged(); }
        }

        // Binds Selected Record to Variable
        private string selectedRecord;
        public string SelectedRecord
        {
            get { return selectedRecord; }
            set { if (selectedRecord != value) { selectedRecord = value; OnPropertyChanged(); } }
        }

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
        }

        // Update Value Reference ID
        private string updateValue;
        public string UpdateValue
        {
            get { return updateValue; }
            set { if (updateValue != value) { updateValue = value; OnPropertyChanged(); } }
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

        // Enable Write Permissions on IAM Write Authenticated
        private bool unixPermissionCheck;
        public bool UnixPermissionCheck
        {
            get { return unixPermissionCheck; }
            set { if (unixPermissionCheck != value) { unixPermissionCheck = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public GroupsTableViewModel()
        {
            // Initialize Relay Commands
            ClearSearchCommand = new RelayCommand(o => { ClearSearchMethod(); });
            SearchEnterKey = new RelayCommand(o => { SearchEnterKeyMethod(); });
            SearchQuery = new RelayCommand(o => { SearchFilter(); });
            ClearQuery = new RelayCommand(o => { ClearResults(); });
            ExcelExport = new RelayCommand(o => { ExcelExportMethod(); });
            HTMLExport = new RelayCommand(o => { HtmlExport(); });
            LoadTable = new RelayCommand(o => { LoadDatabase(); });
            DeleteClickCommand = new RelayCommand(o => { DeleteRecordClick(); });
            EnterCalculation = new RelayCommand(o => { CalculatePagination(); });
            ChangePage = new RelayCommand((parameter) => ChangePageMethod(parameter));
            GroupSelectRecordCommand = new RelayCommand(o => { OpenRecord(); });
            AddRecordCommand = new RelayCommand(o => { AddRecord(); });
            UnixPermissionCheck = MainViewModel.UnixTablePermissionGlobal;

            // Initialize Properties
            DisplayView = new DataView();
            NoFoundResults = false;
            IsLoadBool = true;
            SearchText = "Search";
            MSG.Messenger.Default.Register<String>(this, MessageReloadGroups);
            BindTable = new();
            BindTable = MainViewModel.GroupsTable.Copy();
            RecordsList = new ObservableCollection<DataRow>();
            FullList = new List<DataRow>();
            DisplayTable = new DataTable();
            FilterTable = new DataTable();

            //Initializes Paging Parameters
            Pagination = new PaginationModel(200, 10);
            { };
            Pagination.TotalItems = BindTable.Rows.Count;
            Pagination.ItemsPerPage = 10;
            this.Pagination.PropertyChanged += Pagination_PropertyChanged;

            //Initialize Export Column List
            ExportColumnList = new ExportColumns
            {
                Column1 = "ID",
                Column1IsChecked = true,
                Column2 = "GID",
                Column2IsChecked = true,
                Column3 = "Group",
                Column3IsChecked = true,
                Column4 = "LOB",
                Column4IsChecked = true,
                Column5 = "History",
                Column5IsChecked = true,
                ExportEverything = true
            };
        }
        #endregion

        #region Functions
        // View Open Data Load
        public async void OnLoad()
        {
            try
            {
                IsLoadBool = true;
                UpdateValue = "";
                SearchText = "Search";
                SelectedFilterTerm = SearchFilterTerms.ElementAt(0);
                if (MainViewModel.GroupsTable.Rows.Count < 10)
                {
                    IsLoadBool = true;
                    MainViewModel.GroupsTable.Clear();
                    MainViewModel.GroupsTable = await DBConn.GetTableAsync(ConfigurationProperties.GroupsTable);
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

        private void BindTableBuild() 
        {
            BindTable.Clear();
            BindTable = MainViewModel.GroupsTable.Copy();
            TemporaryTable(BindTable);
        }

        // Manual Database Load
        public async void LoadDatabase()
        {
            try
            {
                IsLoadBool = true;
                UpdateValue = "";
                SearchText = "Search";
                MainViewModel.GroupsTable.Clear();
                DisplayView = MainViewModel.GroupsTable.DefaultView;
                MainViewModel.GroupsTable = await DBConn.GetTableAsync(ConfigurationProperties.GroupsTable);
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

        // Clear out Tables and Lists for new Page
        private void ClearTables()
        {
            DisplayTable.Clear();
            FullList.Clear();
            RecordsList.Clear();
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

        // If Current Page checks triggers new Page List calculation
        private void Pagination_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Pagination.CurrentPage))
            {
                ProcessPage(null);
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

        // Clears Filter on DisplayView
        public void ClearResults()
        {
            // Logic to Clear Filter 
            DisplayView.RowFilter = string.Empty;
            NoFoundResults = false;
            TemporaryTable(BindTable);
            SearchText = "Search";
        }

        // Search Filter on Display View
        private void SearchFilter()
        {
            if (SearchText != "Search" && MainViewModel.GroupsTable.Rows.Count > 0)
            {
                try
                {
                    string selectedFilter = SelectedFilterTerm.SearchTerm.ToString();
                    string selectedColumn = "";
                    string builtFilter = "";
                    SearchText = string.Join(" ", SearchText.Split().Where(w => !Utilities.FilteredWords.Contains(w, StringComparer.InvariantCultureIgnoreCase)));

                    // Switch based on selected filter in ComboBox, Builds Filter off SearchText TextBox = selectedFilter Category
                    switch (selectedFilter)
                    {
                        case "All":
                            builtFilter = "iamhg_group + iamhg_gid + iamhg_lob Like '%" + SearchText + "%'";
                            break;

                        case "GID":
                            string fixedText = new(SearchText.Where(c => char.IsDigit(c)).ToArray());
                            SearchText = fixedText;

                            selectedColumn = "iamhg_gid";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "Group":
                            selectedColumn = "iamhg_group";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "LOB":
                            selectedColumn = "iamhg_lob";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;
                    }

                    string sort = "iamhg_group";
                    DataView tempView = new(MainViewModel.GroupsTable, builtFilter, sort, DataViewRowState.CurrentRows);
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

        // Delete Group Record From Context Menu
        public void DeleteRecordClick()
        {
            if (MessageBox.Show("Are you certain you want to delete this record?", "Delete Record??", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    DataRowView drv = (DataRowView)GroupCellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            string record = row["iamhg_id"].ToString();
                            string UpdateValue = record;
                            DBConn.DeleteMatchingRecords(ConfigurationProperties.GroupsTable, "iamhg_id", UpdateValue);
                            List<DataRow> rowsToDelete = new();
                            foreach (DataRow dr in MainViewModel.GroupsTable.Rows)
                            {
                                if (dr["iamhg_id"].ToString() == UpdateValue)
                                {
                                    rowsToDelete.Add(dr);
                                }
                            }
                            foreach (DataRow dr in rowsToDelete)
                            {
                                MainViewModel.GroupsTable.Rows.Remove(dr);
                            }
                            BindTableBuild();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                    throw;
                }
            }
            else
            {
                //Do Nothing
            }
        }

        // String List holding Columns to Export
        public List<String> BuildExportList()
        {
            List<String> exportList = new();
            if (ExportColumnList.Column1IsChecked == true) { exportList.Add("iamhg_id"); };
            if (ExportColumnList.Column2IsChecked == true) { exportList.Add("iamhg_gid"); };
            if (ExportColumnList.Column3IsChecked == true) { exportList.Add("iamhg_group"); };
            if (ExportColumnList.Column4IsChecked == true) { exportList.Add("iamhg_lob"); };
            if (ExportColumnList.Column5IsChecked == true) { exportList.Add("iamhg_history"); };
            return exportList;
        }

        // String List holding Columns to remove from Export
        public List<String> BuildRemoveExportList()
        {
            List<String> exportRemoveList = new();
            if (ExportColumnList.Column1IsChecked == false) { exportRemoveList.Add("iamhg_id"); };
            if (ExportColumnList.Column2IsChecked == false) { exportRemoveList.Add("iamhg_gid"); };
            if (ExportColumnList.Column3IsChecked == false) { exportRemoveList.Add("iamhg_group"); };
            if (ExportColumnList.Column4IsChecked == false) { exportRemoveList.Add("iamhg_lob"); };
            if (ExportColumnList.Column5IsChecked == false) { exportRemoveList.Add("iamhg_history"); };
            return exportRemoveList;
        }

        // Export Datatable to Excel
        private void ExcelExportMethod()
        {
            try
            {
                if (MainViewModel.GroupsTable == null || MainViewModel.GroupsTable.Columns.Count == 0)
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
                    wb.Worksheets.Add(MainViewModel.GroupsTable, "GroupsDatabase");
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        Title = "Save an Excel File",
                        FileName = "GroupsExcelBackup.xlsx"
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
                dlg.FileName = "GroupsExport";
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

        // Open Record On Double Click
        private void OpenRecord()
        {
            try
            {
                if (GroupCellInfo.Item != null)
                {
                    DataRowView drv = (DataRowView)GroupCellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            GroupsTableUpdateVm = null;
                            SelectedViewModel = null;
                            string record = row["iamhg_id"].ToString();
                            GroupsTableViewModel.ChosenRecord = record;
                            GroupsTableUpdateVm = new GroupsUpdateRecordViewModel();
                            GroupsTableUpdateVm.LoadData();
                            SelectedViewModel = GroupsTableUpdateVm;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Add Record Command, Open Add Record View
        private void AddRecord()
        {
            try
            {
                GroupsAddRecordVm = null;
                SelectedViewModel = null;
                GroupsAddRecordVm = new GroupsAddRecordViewModel();
                GroupsAddRecordVm.OnLoad();
                SelectedViewModel = GroupsAddRecordVm;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
        #endregion

        #region Messages
        // Listen to New Record for Database Reload
        public void MessageReloadGroups(string passedMessage)
        {
            if (passedMessage == "Groups Reload") { BindTableBuild(); }
        }
        #endregion
    }
}

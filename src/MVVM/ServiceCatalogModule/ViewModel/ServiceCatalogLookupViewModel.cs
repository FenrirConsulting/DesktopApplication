using ClosedXML.Excel;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System;
using System.Windows;
using MSG = GalaSoft.MvvmLight.Messaging;
using System.Security;
using System.Data.SqlClient;
using PnP.Framework;
using Microsoft.SharePoint.Client;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ServiceCatalogLookupViewModel : BaseViewModel
    {
        #region Delegates
        //Access SQL Methods Class
        private readonly SQLMethods DBConn = new();
        private readonly SharePointMethods SPMethods = new();

        //Relay Commands for Buttons
        public RelayCommand SearchQuery { get; set; }
        public RelayCommand ClearQuery { get; set; }
        public RelayCommand ClearSearch { get; set; }
        public RelayCommand RecordSelect { get; set; }
        public RelayCommand ChangePage { get; set; }
        public RelayCommand EnterCalculation { get; set; }
        public RelayCommand ExcelExport { get; set; }
        public RelayCommand HTMLExport { get; set; }
        public RelayCommand SearchEnterKey { get; set; }
        public RelayCommand UpdateSharepointCommand { get; set; }
        public RelayCommand DeleteClickCommand { get; set; }
        #endregion

        #region Properties
        // Binds text of Search box to Variable
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { if (searchText != value) { searchText = value; OnPropertyChanged(); } }
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
            new SearchTerms { SearchTerm = "ID" },
            new SearchTerms { SearchTerm = "Service" },
            new SearchTerms { SearchTerm = "SubSystem" },
            new SearchTerms { SearchTerm = "Description" },
            new SearchTerms { SearchTerm = "Request Source" },
            new SearchTerms { SearchTerm = "Assignment Group" },
            new SearchTerms { SearchTerm = "Expedite Process" },
            new SearchTerms { SearchTerm = "SLA" },
            new SearchTerms { SearchTerm = "ArcherAppID" },
            new SearchTerms { SearchTerm = "ITPMAppID" },
            new SearchTerms { SearchTerm = "Hints" },
            new SearchTerms { SearchTerm = "Tasks" },
            new SearchTerms { SearchTerm = "Domain" },
            new SearchTerms { SearchTerm = "Article" },
            new SearchTerms { SearchTerm = "Provisioning Team" },
            new SearchTerms { SearchTerm = "URL" },
            new SearchTerms { SearchTerm = "Automation Status"}
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

        // Binds Local Table to Global Table
        private DataTable bindTable;
        public DataTable BindTable
        {
            get { return bindTable; }
            set { bindTable = value; OnPropertyChanged(); }
        }

        // Full List of Records Amount
        private List<DataRow> fullList;
        public List<DataRow> FullList
        {
            get { return fullList; }
            set { fullList = value; OnPropertyChanged(); }
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

        // List of Paged Results
        private ObservableCollection<DataRow> recordsList;
        public ObservableCollection<DataRow> RecordsList
        {
            get { return recordsList; }
            set { recordsList = value; OnPropertyChanged(); }
        }

        // No Results Found
        private bool noFoundResults;
        public bool NoFoundResults
        {
            get { return noFoundResults; }
            set { noFoundResults = value; OnPropertyChanged(); }
        }

        // Manager Role Bool Binding
        private bool mgrRole;
        public bool MgrRole
        {
            get { return mgrRole; }
            set { mgrRole = value; OnPropertyChanged(); }
        }

        // Admin Role Bool Binding
        private bool adminRole;
        public bool AdminRole
        {
            get { return adminRole; }
            set { adminRole = value; OnPropertyChanged(); }
        }

        // Loading Animation Bool
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { isLoadBool = value; OnPropertyChanged(); }
        }

        // Pagination Model Class
        public PaginationModel Pagination { get; set; }
        #endregion

        #region Methods
        public ServiceCatalogLookupViewModel()
        {
            // Initialize Properties
            BindTable = new();
            BindTable = MainViewModel.ServiceCatalogTable.Copy();
            RecordsList = new();
            FullList = new();
            DisplayTable = new();
            FilterTable = new();
            DisplayView = new();
            SearchText = "Search";
            IsLoadBool = false;
            NoFoundResults = false;
            MgrRole = false;
            AdminRole = false;
            SPMethods = new();
            MSG.Messenger.Default.Register<String>(this, SentMessage);

            //Initializes Paging Parameters
            Pagination = new PaginationModel(200, 10);
            { };
            Pagination.TotalItems = BindTable.Rows.Count;
            Pagination.ItemsPerPage = 10;
            this.Pagination.PropertyChanged += Pagination_PropertyChanged;

            //Initialize Relay Commands
            RecordSelect = new RelayCommand(o => { RecordSelectMethod(); });
            EnterCalculation = new RelayCommand(o => { CalculatePagination(); });
            ChangePage = new RelayCommand((parameter) => ChangePageMethod(parameter));
            SearchQuery = new RelayCommand(o => { SearchFilter(); });
            ClearQuery = new RelayCommand(o => { ClearResults(); });
            ExcelExport = new RelayCommand(o => { ExcelExportMethod(); });
            HTMLExport = new RelayCommand(o => { HtmlExport(); });
            ClearSearch = new RelayCommand(o => { ClearSearchMethod(); });
            SearchEnterKey = new RelayCommand(o => { SearchEnterKeyMethod(); });
            UpdateSharepointCommand = new RelayCommand(o => { UpdateSharePoint(); });
            DeleteClickCommand = new RelayCommand(o => { DeleteRecordClick(); });

            //Initialize Export Column List
            ExportColumnList = new ExportColumns
            {
                Column1 = "ID",
                Column1IsChecked = true,
                Column2 = "Services",
                Column2IsChecked = true,
                Column3 = "SubSystem",
                Column3IsChecked = true,
                Column4 = "Description",
                Column4IsChecked = true,
                Column5 = "RequestSource",
                Column5IsChecked = true,
                Column6 = "AssignmentGroup",
                Column6IsChecked = true,
                Column7 = "ExpediteProcess",
                Column7IsChecked = true,
                Column8 = "SLA",
                Column8IsChecked = true,
                Column9 = "ArcherAppID",
                Column9IsChecked = true,
                Column10 = "ITPMAppID",
                Column10IsChecked = true,
                Column11 = "Hints",
                Column11IsChecked = true,
                Column12 = "Tasks",
                Column12IsChecked = true,
                Column13 = "Domain",
                Column13IsChecked = true,
                Column14 = "Article",
                Column14IsChecked = true,
                Column15 = "ProvisioningTeam",
                Column15IsChecked = true,
                Column16 = "URL",
                Column16IsChecked = true,
                Column17 = "AutomationStatus",
                Column17IsChecked = true,
                Column18 = "Environment",
                Column18IsChecked = true,
                ExportEverything = true
            };
        }
        #endregion

        #region Functions
        // Primary On Load Function
        public async void LoadData()
        {
            try
            {
                MgrRole = MainViewModel.GlobalServiceCatalogMgrEnabled;
                AdminRole = MainViewModel.GlobalServiceCatalogAdminEnabled;
                NoFoundResults = false;
                MainViewModel.ServiceCatalogTable.Clear();
                IsLoadBool = true;
                var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.ServiceCatalogTable);
                MainViewModel.ServiceCatalogTable = dataTask;
                BindTableBuild();
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Reset Display Table
        public async void ReloadTable()
        {
            try
            {
                MainViewModel.ServiceCatalogTable.Clear();
                IsLoadBool = true;
                var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.ServiceCatalogTable);
                MainViewModel.ServiceCatalogTable = dataTask;
                BindTableBuild();
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Updates SharePoint list to Mirror DB Table
        public async void UpdateSharePoint()
        {
            if (MessageBox.Show("Update SharePoint? Are You Sure? Entire List will be Updated.", "Update SharePoint?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    IsLoadBool = true;
                    var sync = await SPMethods.Insert(MainViewModel.ServiceCatalogTable, true);

                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
                IsLoadBool = false;
            }
            else
            {

            }
        }

        // Right-Click Delete Record from SQL and SharePoint
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
                            string record = row["ID"].ToString();
                            DBConn.DeleteMatchingRecords(ConfigurationProperties.ServiceCatalogTable, "ID", record);
                            var awaitDeletion = await SPMethods.DeleteSingleItem(record);
                            ReloadTable();
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

        // Copy Global Table to Bind Table
        private void BindTableBuild()
        {
            try 
            {
                BindTable.Clear();
                BindTable = MainViewModel.ServiceCatalogTable.Copy();
                TemporaryTable(BindTable);
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
            if (MainViewModel.GlobalServiceCatalogMgrEnabled == true)
            {
                try
                {
                    DataRowView drv = (DataRowView)CellInfo.Item;
                    if (drv != null)
                    {
                        DataRow row = drv.Row;
                        if (row != null)
                        {
                            string record = row["ID"].ToString();
                            ServiceCatalogMainViewModel.CurrentServiceItem = record;
                            MSG.Messenger.Default.Send("Select Service Catalog Update Tab");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }
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
                    string selectedFilter = SelectedTerm.SearchTerm.ToString();
                    string builtFilter = "";
                    string selectedColumn = "";
                    NoFoundResults = false;
                    SearchText = string.Join(" ", SearchText.Split().Where(w => !Utilities.FilteredWords.Contains(w, StringComparer.InvariantCultureIgnoreCase)));

                    // Switch based on selected filter in ComboBox, Builds Filter off SearchText TextBox = selectedFilter Category
                    switch (selectedFilter)
                    {
                        case "All":
                            builtFilter = "ID + Services + SubSystem + ProvisioningTeam + Description + ArcherAppID + ITPMAppID + RequestSource + " +
                                "AssignmentGroup + ExpediteProcess + SLA + Hints + Tasks + Domain + Article + URL Like '%" + SearchText + "%'";
                            break;

                        case "ID":
                            selectedColumn = "ID";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "Services":
                            selectedColumn = "Service";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "SubSystem":
                            selectedColumn = "SubSystem";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "Provisioning Team":
                            selectedColumn = "ProvisioningTeam";
                            builtFilter = selectedColumn + " = '" + SearchText + "'";
                            break;

                        case "Description":
                            selectedColumn = "Description";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ArcherAppID":
                            selectedColumn = "ArcherAppID";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "ITPMAppID":
                            selectedColumn = "ITPMAppID";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Request Source":
                            selectedColumn = "RequestSource";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Assignment Group":
                            selectedColumn = "AssignmentGroup";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Expedite Process":
                            selectedColumn = "ExpeditProcess";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "SLA":
                            selectedColumn = "SLA";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Hints":
                            selectedColumn = "Hints";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Tasks":
                            selectedColumn = "Tasks";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Domain":
                            selectedColumn = "Domain";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "Article":
                            selectedColumn = "Article";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "URL":
                            selectedColumn = "URL";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;

                        case "AutomationStatus":
                            selectedColumn = "AutomationStatus";
                            builtFilter = selectedColumn + " Like '%" + SearchText + "%'";
                            break;
                    }

                    string sort = "ID";
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

        // String List holding Columns to Export
        public List<String> BuildExportList()
        {
            List<String> exportList = new();
            if (ExportColumnList.Column1IsChecked == true) { exportList.Add("ID"); };
            if (ExportColumnList.Column2IsChecked == true) { exportList.Add("Services"); };
            if (ExportColumnList.Column3IsChecked == true) { exportList.Add("Subsystem"); };
            if (ExportColumnList.Column4IsChecked == true) { exportList.Add("Description"); };
            if (ExportColumnList.Column5IsChecked == true) { exportList.Add("RequestSource"); };
            if (ExportColumnList.Column6IsChecked == true) { exportList.Add("AssignmentGroup"); };
            if (ExportColumnList.Column7IsChecked == true) { exportList.Add("ExpediteProcess"); };
            if (ExportColumnList.Column8IsChecked == true) { exportList.Add("SLA"); };
            if (ExportColumnList.Column9IsChecked == true) { exportList.Add("ArcherAppID"); };
            if (ExportColumnList.Column10IsChecked == true) { exportList.Add("ITPMAppID"); };
            if (ExportColumnList.Column11IsChecked == true) { exportList.Add("Hints"); };
            if (ExportColumnList.Column12IsChecked == true) { exportList.Add("Tasks"); };
            if (ExportColumnList.Column13IsChecked == true) { exportList.Add("Domain"); };
            if (ExportColumnList.Column14IsChecked == true) { exportList.Add("Article"); }
            if (ExportColumnList.Column15IsChecked == true) { exportList.Add("ProvisioningTeam"); };
            if (ExportColumnList.Column16IsChecked == true) { exportList.Add("URL"); };
            if (ExportColumnList.Column17IsChecked == true) { exportList.Add("AutomationStatus"); };
            if (ExportColumnList.Column18IsChecked == true) { exportList.Add("Environment"); };
            return exportList;
        }

        // String List holding Columns to remove from Export
        public List<String> BuildRemoveExportList()
        {
            List<String> exportRemoveList = new();
            if (ExportColumnList.Column1IsChecked == false) { exportRemoveList.Add("ID"); };
            if (ExportColumnList.Column2IsChecked == false) { exportRemoveList.Add("Services"); };
            if (ExportColumnList.Column3IsChecked == false) { exportRemoveList.Add("SubSystem"); };
            if (ExportColumnList.Column4IsChecked == false) { exportRemoveList.Add("Description"); };
            if (ExportColumnList.Column5IsChecked == false) { exportRemoveList.Add("RequestSource"); };
            if (ExportColumnList.Column6IsChecked == false) { exportRemoveList.Add("AssignmentGroup"); };
            if (ExportColumnList.Column7IsChecked == false) { exportRemoveList.Add("ExpediteProcess"); };
            if (ExportColumnList.Column8IsChecked == false) { exportRemoveList.Add("SLA"); };
            if (ExportColumnList.Column9IsChecked == false) { exportRemoveList.Add("ArcherAppID"); };
            if (ExportColumnList.Column10IsChecked == false) { exportRemoveList.Add("ITPMAppID"); };
            if (ExportColumnList.Column11IsChecked == false) { exportRemoveList.Add("Hints"); };
            if (ExportColumnList.Column12IsChecked == false) { exportRemoveList.Add("Tasks"); };
            if (ExportColumnList.Column13IsChecked == false) { exportRemoveList.Add("Domain"); };
            if (ExportColumnList.Column14IsChecked == false) { exportRemoveList.Add("Article"); };
            if (ExportColumnList.Column15IsChecked == false) { exportRemoveList.Add("ProvisioningTeam"); };
            if (ExportColumnList.Column16IsChecked == false) { exportRemoveList.Add("URL"); };
            if (ExportColumnList.Column17IsChecked == false) { exportRemoveList.Add("AutomationStatus"); };
            if (ExportColumnList.Column18IsChecked == false) { exportRemoveList.Add("Environment"); };
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
                dlg.FileName = "ServiceCatalog";
                dlg.DefaultExt = ".html";
                dlg.Filter = "HTML Documents (.html)|*.html";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    System.IO.File.WriteAllText(dlg.FileName, export);
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
                    string exportName = "ServiceCatalog";
                    wb.Worksheets.Add(ExportTable, exportName);
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        Title = "Save an Excel File",
                        FileName = "ServiceCatalog.xlsx"
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

        // Clears Filter on DisplayView
        public void ClearResults()
        {
            DisplayView.RowFilter = string.Empty;
            NoFoundResults = false;
            TemporaryTable(BindTable);
            SearchText = "Search";
        }

        // Clear Search Filter
        private void ClearSearchMethod()
        {
            if (SearchText == "Search")
            {
                SearchText = "";
            }
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
                    DisplayTable.DefaultView.Sort = "ID";
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
        // Listen For Record Select Message
        public void SentMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                switch (passedMessage)
                {

                    case "Reload Service Catalog Table":
                        ReloadTable();
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}

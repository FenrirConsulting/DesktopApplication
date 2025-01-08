using ClosedXML.Excel;
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MSG = GalaSoft.MvvmLight.Messaging;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class FacilitatorRequestViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand GenerateCommand { get; set; }
        public RelayCommand ClearFormCommand { get; set; }
        public RelayCommand GetRequestCommand { get; set; }
        public RelayCommand RequestChangedCommand { get; set; }
        public RelayCommand RequestModeChangedCommand { get; set; }
        public RelayCommand RequestStatusChangedCommand { get; set; }
        public RelayCommand CSVExportCommand { get; set; }
        public RelayCommand ClipboardExportCommand { get; set; }
        public RelayCommand HTMLExportCommand { get; set; }
        public RelayCommand ToggleSelectParticipantCommand { get; set; }
        public RelayCommand SpaceCheckboxCommand { get; set; }
        public RelayCommand ResetStatusCommand { get; set; }
        public RelayCommand SendChangedCommand { get; set; }

        private readonly SQLMethods DBConn = new();
        public FactoolHistoryBuilder HBuilder = new();
        #endregion Delegates

        #region Properties
        // Bool tracking if the Form is New
        private bool NewForm = false;

        // Holds Datatable Export Button Function
        private DataTable exportDataTable;
        public DataTable ExportDataTable
        {
            get { return exportDataTable; }
            set { exportDataTable = value; OnPropertyChanged(); }
        }

        // Binds Export Stackpanel Enabled
        private bool exportsEnabled;
        public bool ExportsEnabled
        {
            get { return exportsEnabled; }
            set { if (exportsEnabled != value) { exportsEnabled = value; OnPropertyChanged(); } }
        }

          // Binds Reset Status Enabled
        private bool resetStatusEnabled;
        public bool ResetStatusEnabled
        {
            get { return resetStatusEnabled; }
            set { if (resetStatusEnabled != value) { resetStatusEnabled = value; OnPropertyChanged(); } }
        }

        // Binds New Request Enabled
        private bool requestNewEnabled;
        public bool RequestNewEnabled
        {
            get { return requestNewEnabled; }
            set { if (requestNewEnabled != value) { requestNewEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Existing Request Enabled
        private bool requestExistingEnabled;
        public bool RequestExistingEnabled
        {
            get { return requestExistingEnabled; }
            set { if (requestExistingEnabled != value) { requestExistingEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Request Radio Button Options
        private string requestButtonSelectionConverter;
        public string RequestButtonSelectionConverter
        {
            get { return requestButtonSelectionConverter; }
            set
            {
                if (requestButtonSelectionConverter != value)
                {
                    requestButtonSelectionConverter = value;
                    // Set Request Mode Changed Depending on if New or Exisiting
                    if (RequestButtonSelectionConverter == "New") { RequestModeChangedButton(ConfigurationProperties.RequestMode.New); }
                    if (RequestButtonSelectionConverter == "Existing") { RequestModeChangedButton(ConfigurationProperties.RequestMode.Existing); }
                    OnPropertyChanged();
                }
            }
        }

        // Binds Reference Textbox String
        private string referenceTextBoxString;
        public string ReferenceTextBoxString
        {
            get { return referenceTextBoxString; }
            set
            {
                if (referenceTextBoxString != value)
                {
                    referenceTextBoxString = value;
                    if (referenceTextBoxString.Length >= 9) { GetRequestButtonEnabled = true; }
                    OnPropertyChanged();
                }
            }
        }

        // Binds Reference Text Box Enabled
        private bool refTextBoxEnabled;
        public bool RefTextBoxEnabled
        {
            get { return refTextBoxEnabled; }
            set { if (refTextBoxEnabled != value) { refTextBoxEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Get Request Button Enabled
        private bool getRequestButtonEnabled;
        public bool GetRequestButtonEnabled
        {
            get { return getRequestButtonEnabled; }
            set { if (getRequestButtonEnabled != value) { getRequestButtonEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Status Request Defected Enabled
        private bool requestStatusDefectedEnabled;
        public bool RequestStatusDefectedEnabled
        {
            get { return requestStatusDefectedEnabled; }
            set { if (requestStatusDefectedEnabled != value) { requestStatusDefectedEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Status Request Ready Enabled
        private bool requestStatusReadyEnabled;
        public bool RequestStatusReadyEnabled
        {
            get { return requestStatusReadyEnabled; }
            set { if (requestStatusReadyEnabled != value) { requestStatusReadyEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Status Request Complete Enabled
        private bool requestStatusCompleteEnabled;
        public bool RequestStatusCompleteEnabled
        {
            get { return requestStatusCompleteEnabled; }
            set { if (requestStatusCompleteEnabled != value) { requestStatusCompleteEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Request Status Radio Button Options
        private string requestStatusButtonSelectionConverter;
        public string RequestStatusButtonSelectionConverter
        {
            get { return requestStatusButtonSelectionConverter; }
            set
            {
                if (requestStatusButtonSelectionConverter != value)
                {
                    requestStatusButtonSelectionConverter = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Defect List View Enabled
        private bool defectListViewEnabled;
        public bool DefectListViewEnabled
        {
            get { return defectListViewEnabled; }
            set { if (defectListViewEnabled != value) { defectListViewEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Dumpster Visible Button
        private bool dumpsterVisible;
        public bool DumpsterVisible
        {
            get { return dumpsterVisible; }
            set { if (dumpsterVisible != value) { dumpsterVisible = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of Defect Reasons
        private ObservableCollection<FacToolItem> defectReasons = new()
        {
        };
        public IEnumerable<FacToolItem> DefectReasons
        {
            get { return defectReasons; }
        }

        // Selected Term for Reply Listview
        private FacToolItem selectedDefectReason = new();
        public FacToolItem SelectedDefectReason
        {
            get { return selectedDefectReason; }
            set
            {
                selectedDefectReason = value;
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable defectReasonsTable;
        public DataTable DefectReasonsTable
        {
            get { return defectReasonsTable; }
            set { defectReasonsTable = value; OnPropertyChanged(); }
        }

        // Binds Total Users Textbox String
        private string totalUsersString;
        public string TotalUsersString
        {
            get { return totalUsersString; }
            set
            {
                if (totalUsersString != value)
                {
                    var s = value;
                    var stripped = Regex.Replace(s, "[^0-9]", ""); // Removes Non Numeric Characters from Total Users Field
                    totalUsersString = stripped;
                    RequestChanged();
                    OnPropertyChanged();
                }
            }
        }

        // Binds Total Users  Enabled
        private bool totalUsersEnabled;
        public bool TotalUsersEnabled
        {
            get { return totalUsersEnabled; }
            set { if (totalUsersEnabled != value) { totalUsersEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of Form Type
        private ObservableCollection<FacToolItem> formTypes = new()
        {
        };
        public IEnumerable<FacToolItem> FormTypes
        {
            get { return formTypes; }
        }

        // Selected Term for Form Type ComboBox
        private FacToolItem selectedFormTerm = new();
        public FacToolItem SelectedFormTerm
        {
            get { return selectedFormTerm; }
            set
            {
                selectedFormTerm = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable formTypeTable;
        public DataTable FormTypeTable
        {
            get { return formTypeTable; }
            set { formTypeTable = value; OnPropertyChanged(); }
        }

        // Binds Form Types Enabled
        private bool formTypesEnabled;
        public bool FormTypesEnabled
        {
            get { return formTypesEnabled; }
            set { if (formTypesEnabled != value) { formTypesEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of Request Type
        private ObservableCollection<FacToolItem> requestTypes = new()
        {
        };
        public IEnumerable<FacToolItem> RequestTypes
        {
            get { return requestTypes; }
        }


        // Selected Term for Request Type ComboBox
        private FacToolItem selectedRequestTerm = new();
        public FacToolItem SelectedRequestTerm
        {
            get { return selectedRequestTerm; }
            set
            {
                selectedRequestTerm = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Binds Request Types Enabled
        private bool requestTypesEnabled;
        public bool RequestTypesEnabled
        {
            get { return requestTypesEnabled; }
            set { if (requestTypesEnabled != value) { requestTypesEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Datatable for SQL Table Select
        private DataTable requestTypesTable;
        public DataTable RequestTypesTable
        {
            get { return requestTypesTable; }
            set { requestTypesTable = value; OnPropertyChanged(); }
        }

        // Binds XRef 1 Textbox String
        private string xref1String;
        public string Xref1String
        {
            get { return xref1String; }
            set
            {
                if (xref1String != value)
                {
                    xref1String = value;
                    RequestChanged();
                    OnPropertyChanged();
                }
            }
        }

        // Binds XRef1 Box Enabled
        private bool xref1BoxEnabled;
        public bool Xref1BoxEnabled
        {
            get { return xref1BoxEnabled; }
            set { if (xref1BoxEnabled != value) { xref1BoxEnabled = value; OnPropertyChanged(); } }
        }

        // Binds XRef 2 Textbox String
        private string xref2String;
        public string Xref2String
        {
            get { return xref2String; }
            set
            {
                if (xref2String != value)
                {
                    xref2String = value;
                    RequestChanged();
                    OnPropertyChanged();
                }
            }
        }

        // Binds XRef2 Box Enabled
        private bool xref2BoxEnabled;
        public bool Xref2BoxEnabled
        {
            get { return xref2BoxEnabled; }
            set { if (xref2BoxEnabled != value) { xref2BoxEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Systems List View Enabled
        private bool systemsListViewEnabled;
        public bool SystemsListViewEnabled
        {
            get { return systemsListViewEnabled; }
            set { if (systemsListViewEnabled != value) { systemsListViewEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Spinner Loading
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of Systems
        private ObservableCollection<FacToolItem> systems = new()
        {
        };
        public IEnumerable<FacToolItem> Systems
        {
            get { return systems; }
        }

        // Selected Term for System Listview
        private FacToolItem selectedSystemTerm = new();
        public FacToolItem SelectedSystemTerm
        {
            get { return selectedSystemTerm; }
            set
            {
                selectedSystemTerm = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable systemsTable;
        public DataTable SystemsTable
        {
            get { return systemsTable; }
            set { systemsTable = value; OnPropertyChanged(); }
        }

        // Binds Reply List View Enabled
        private bool replyListViewEnabled;
        public bool ReplyListViewEnabled
        {
            get { return replyListViewEnabled; }
            set { if (replyListViewEnabled != value) { replyListViewEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of Reply Type
        private ObservableCollection<FacToolItem> replyTypes = new()
        {
        };
        public IEnumerable<FacToolItem> ReplyTypes
        {
            get { return replyTypes; }
        }

        // Selected Term for Reply Listview
        private FacToolItem selectedReplyTerm = new();
        public FacToolItem SelectedReplyTerm
        {
            get { return selectedReplyTerm; }
            set
            {
                selectedReplyTerm = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable replyTypesTable;
        public DataTable ReplyTypesTable
        {
            get { return replyTypesTable; }
            set { replyTypesTable = value; OnPropertyChanged(); }
        }

        // Binds LOB List View Enabled
        private bool lOBListViewEnabled;
        public bool LOBListViewEnabled
        {
            get { return lOBListViewEnabled; }
            set { if (lOBListViewEnabled != value) { lOBListViewEnabled = value; OnPropertyChanged(); } }
        }

        // Holds Object Collection of LOB Type
        private ObservableCollection<FacToolItem> lOBTypes = new()
        {
        };
        public IEnumerable<FacToolItem> LOBTypes
        {
            get { return lOBTypes; }
        }

        // Selected Term for LOB Listview
        private FacToolItem selectedLOBTerm = new();
        public FacToolItem SelectedLOBTerm
        {
            get { return selectedLOBTerm; }
            set
            {
                selectedLOBTerm = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Holds Datatable for SQL Table Select
        private DataTable lOBTypesTable;
        public DataTable LOBTypesTable
        {
            get { return lOBTypesTable; }
            set { lOBTypesTable = value; OnPropertyChanged(); }
        }

        // Binds Additional Comments String
        private string additionalCommentsTextBoxString;
        public string AdditionalCommentsTextBoxString
        {
            get { return additionalCommentsTextBoxString; }
            set
            {
                if (additionalCommentsTextBoxString != value)
                {
                    additionalCommentsTextBoxString = value;
                    RequestChanged();
                    OnPropertyChanged();
                }
            }
        }

        // Binds Additional Comments Textbox Enabled
        private bool commentsBoxEnabled;
        public bool CommentsBoxEnabled
        {
            get { return commentsBoxEnabled; }
            set
            {
                if (commentsBoxEnabled != value)
                {
                    commentsBoxEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Object Collection of State Type
        private ObservableCollection<ComboBoxListItem> stateTypes = new()
        {

        };
        public IEnumerable<ComboBoxListItem> StateTypes
        {
            get { return stateTypes; }
        }

        // Selected Term for State Type ComboBox
        private ComboBoxListItem selectedState = new();
        public ComboBoxListItem SelectedState
        {
            get { return selectedState; }
            set
            {
                selectedState = value;
                RequestChanged();
                OnPropertyChanged();
            }
        }

        // Binds Selected State Combobox Enabled
        private bool selectedStateEnabled;
        public bool SelectedStateEnabled
        {
            get { return selectedStateEnabled; }
            set { if (selectedStateEnabled != value) { selectedStateEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Sent Back Checkbox Check Value
        private bool sendBackChecked;
        public bool SendBackChecked
        {
            get { return sendBackChecked; }
            set { if (sendBackChecked != value) { sendBackChecked = value; RequestChanged(); OnPropertyChanged(); } }
        }

        // Binds Send Back Checkbox Enabled
        private bool sendBackEnabled;
        public bool SendBackEnabled
        {
            get { return sendBackEnabled; }
            set { if (sendBackEnabled != value) { sendBackEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Received Back Checkbox Check Value
        private bool receivedBackChecked;
        public bool ReceivedBackChecked
        {
            get { return receivedBackChecked; }
            set { if (receivedBackChecked != value) { receivedBackChecked = value; RequestChanged(); OnPropertyChanged(); } }
        }

        // Binds Received Back Checkbox Enabled
        private bool receivedBackEnabled;
        public bool ReceivedBackEnabled
        {
            get { return receivedBackEnabled; }
            set { if (receivedBackEnabled != value) { receivedBackEnabled = value; OnPropertyChanged(); } }
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

        // Binds Generate Textbox String
        private string generateTextBoxString;
        public string GenerateTextBoxString
        {
            get { return generateTextBoxString; }
            set
            {
                if (generateTextBoxString != value)
                {
                    generateTextBoxString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Generate Button Enabled
        private bool generateButtonEnabled;
        public bool GenerateButtonEnabled
        {
            get { return generateButtonEnabled; }
            set { if (generateButtonEnabled != value) { generateButtonEnabled = value; OnPropertyChanged(); } }
        }

        // Binds Generate Button String
        private string generateButtonString;
        public string GenerateButtonString
        {
            get { return generateButtonString; }
            set
            {
                if (generateButtonString != value)
                {
                    generateButtonString = value;
                    OnPropertyChanged();
                }
            }
        }

        // Binds Update Request Checkbox Check Value
        private bool updateRequestChecked;
        public bool UpdateRequestChecked
        {
            get { return updateRequestChecked; }
            set { if (updateRequestChecked != value) { updateRequestChecked = value; OnPropertyChanged(); } }
        }

        // Binds Update Request Checkbox Visibility
        private bool updateRequestCheckVisible;
        public bool UpdateRequestCheckVisible
        {
            get { return updateRequestCheckVisible; }
            set { if (updateRequestCheckVisible != value) { updateRequestCheckVisible = value; OnPropertyChanged(); } }
        }

        // Holds the Current Request to Compare Against Final Changes for History Tracking
        private FacToolRequest activeRequest;
        public FacToolRequest ActiveRequest
        {
            get { return activeRequest; }
            set { if (activeRequest != value) { activeRequest = value; OnPropertyChanged(); } }
        }
        #endregion Properties

        #region Methods
        public FacilitatorRequestViewModel()
        {
            // Initialize Propeties
            FormTypeTable = new();
            RequestTypesTable = new();
            DefectReasonsTable = new();
            SystemsTable = new();
            ReplyTypesTable = new();
            LOBTypesTable = new();
            ExportDataTable = new();
            MSG.Messenger.Default.Register<String>(this, TabMessage);
            ExportsEnabled = MainViewModel.GlobalFacToolMgrPermission;
            IsLoadBool = false;
            ResetStatusEnabled = false;
            SendBackEnabled = true;
            ReceivedBackEnabled = true;
            DumpsterVisible = false;
            LOBListViewEnabled = true;

            //Initialize Relay Commands
            ClearFormCommand = new RelayCommand(o => { ClearFormButton(); });
            GenerateCommand = new RelayCommand(o => { GenerateRequest(); });
            GetRequestCommand = new RelayCommand(o => { GetRequestButton(); });
            RequestChangedCommand = new RelayCommand(o => { RequestChanged(); });
            ResetStatusCommand = new RelayCommand(o => { ResetStatus(); });
            SpaceCheckboxCommand = new RelayCommand((box) => { SpaceCheckbox(box.ToString()); });
            SendChangedCommand = new RelayCommand((box) => { SendChanged(box.ToString()); });

            RequestModeChangedCommand = new RelayCommand(o =>
            {
                // Set Request Mode Changed Depending on if New or Exisiting
                if (RequestButtonSelectionConverter == "New") { RequestModeChangedButton(ConfigurationProperties.RequestMode.New); }
                if (RequestButtonSelectionConverter == "Existing") { RequestModeChangedButton(ConfigurationProperties.RequestMode.Existing); }
            });

            RequestStatusChangedCommand = new RelayCommand(o => { RequestStatusChanged(RequestStatusButtonSelectionConverter); });
            CSVExportCommand = new RelayCommand(o => { CSVExport(); });
            ClipboardExportCommand = new RelayCommand(o => { ExportClipboard("All"); });
            HTMLExportCommand = new RelayCommand(o => { HTMLExport("All"); });
        }
        #endregion Methods

        #region Functions
        // Main Page Load Function
        public void LoadData()
        {
            try
            {
                UpdateRequestCheckVisible = MainViewModel.GlobalFacToolAdminPermission; // Checkbox only visibile to Factool Admins
                ExportsEnabled = MainViewModel.GlobalFacToolMgrPermission; // Enables Export Buttons if Manager or higher level
                SetListViews();
                SetForm(ConfigurationProperties.RequestMode.New);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Switch Send / Receive Checkbox on Check Click
        public void SendChanged(string changedSwitch)
        {
            if (changedSwitch == "Send")
            {
                if (ReceivedBackChecked == true)
                {
                    ReceivedBackChecked = false;
                }
            }
            if (changedSwitch == "Receive")
            {
                if (SendBackChecked == true)
                {
                    SendBackChecked = false;
                }
            }
        }

        // Set the Form for New or Current Request
        public void SetForm(ConfigurationProperties.RequestMode requestMode)
        {
            try
            {
                UpdateRequestChecked = true;
                SendBackEnabled = true;
                if (requestMode == ConfigurationProperties.RequestMode.New)
                {
                    NewForm = true;
                    GenerateButtonString = "Generate";
                    RequestNewEnabled = true;
                    TotalUsersEnabled = true;
                    RequestButtonSelectionConverter = "New";
                    RequestExistingEnabled = true;
                    ReceivedBackChecked = true;
                    SelectedStateEnabled = true;
                }

                if (requestMode == ConfigurationProperties.RequestMode.Existing)
                {
                    NewForm = false;
                    GenerateButtonString = "Save";
                    RequestNewEnabled = false;
                    RequestButtonSelectionConverter = "Existing";
                    SelectedStateEnabled = true;
                }

                // If the Current Request is not Empty, Sets form to Current Request Configuration
                if (FacilitatorMainViewModel.CurrentRequest != null)
                {
                    CurrentRequestSet();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Function to Fill List Views and Combo Boxes off Configuration Tables
        public void SetListViews()
        {
            // Fill Defect Reasons Object from SQL Data Tables
            defectReasons.Clear();
            FacilitatorMainViewModel.DefectReasonsTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.DefectReasonsTable.DefaultView)
            {
                DataRow row = rowView.Row;
                defectReasons.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                    IsCustom = true
                }
                );
                int count = defectReasons.Count;
                // Sets Custom Field Text to be Editable
                if (defectReasons.ElementAt(count - 1).Name.ToString() == "Custom") { defectReasons.ElementAt(count - 1).IsCustom = false; }
            }

            // Fill Form Type Object from SQL Data Tables
            formTypes.Clear();
            FacilitatorMainViewModel.FormTypeTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.FormTypeTable.DefaultView)
            {
                DataRow row = rowView.Row;
                formTypes.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                }
                );
            }

            // Fill LOB Type Object from SQL Data Tables
            lOBTypes.Clear();
            FacilitatorMainViewModel.LOBTypeTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.LOBTypeTable.DefaultView)
            {
                DataRow row = rowView.Row;
                lOBTypes.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                }
                );
            }

            // Fill Request Type Object from SQL Data Tables
            requestTypes.Clear();
            FacilitatorMainViewModel.RequestTypesTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.RequestTypesTable.DefaultView)
            {
                DataRow row = rowView.Row;
                requestTypes.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                    IsCustom = true
                }
                );
                int count = requestTypes.Count;
                // Sets Custom Field Text to be Editable
                if (requestTypes.ElementAt(count - 1).Name.ToString() == "Custom") { requestTypes.ElementAt(count - 1).IsCustom = false; }
            }

            // Fill Systems Object from SQL Data Tables
            systems.Clear();
            FacilitatorMainViewModel.SystemsTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.SystemsTable.DefaultView)
            {
                DataRow row = rowView.Row;
                systems.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                    IsCustom = true
                }
                );
                int count = systems.Count;
                // Sets Custom Field Text to be Editable
                if (systems.ElementAt(count-1).Name.ToString() == "Custom") { systems.ElementAt(count - 1).IsCustom = false; }
            }

            // Fill Reply Type Object from SQL Data Tables
            replyTypes.Clear();
            FacilitatorMainViewModel.ReplyTypesTable.DefaultView.Sort = "DisplayOrder ASC";
            foreach (DataRowView rowView in FacilitatorMainViewModel.ReplyTypesTable.DefaultView)
            {
                DataRow row = rowView.Row;
                replyTypes.Add(new FacToolItem
                {
                    _id = Convert.ToInt32(row[0]),
                    Name = row[1].ToString(),
                    Value = row[2].ToString(),
                    DisplayOrder = row[3].ToString(),
                    ModifiedBy = row[4].ToString(),
                    Modified = row[5].ToString(),
                    IsSelected = false,
                    IsCustom = true
                }
                );
                int count = replyTypes.Count;
                // Sets Custom Field Text to be Editable
                if (replyTypes.ElementAt(count - 1).Name.ToString() == "Custom") { replyTypes.ElementAt(count - 1).IsCustom = false; }
            }

            SelectedFormTerm = formTypes.ElementAt(0);

            SelectedLOBTerm = lOBTypes.ElementAt(0);

            stateTypes.Clear();

            stateTypes.Add(new ComboBoxListItem
            {
                BoxItem = "Pre-Approval"
            });
            stateTypes.Add(new ComboBoxListItem
            {
                BoxItem = "Post-Approval"
            });
            stateTypes.Add(new ComboBoxListItem
            {
                BoxItem = "In-Progress"
            });
            stateTypes.Add(new ComboBoxListItem
            {
                BoxItem = "Cancelled"
            });
            stateTypes.Add(new ComboBoxListItem
            {
                BoxItem = "Completed"
            });

            SelectedState = stateTypes.ElementAt(0);
        }

        // Method for Setting Form for a Current Request
        public void CurrentRequestSet()
        {
            try
            {
                // Completed Request Type Settings
                if (FacilitatorMainViewModel.CurrentRequest.RequestStatus == "Complete" || FacilitatorMainViewModel.CurrentRequest.RequestStatus == "Cancelled")
                {
                    RequestNewEnabled = false;
                    RequestExistingEnabled = false;
                    RequestButtonSelectionConverter = "Existing";
                    RefTextBoxEnabled = false;
                    ReferenceTextBoxString = FacilitatorMainViewModel.CurrentRequest.ReferenceNumber;
                    GetRequestButtonEnabled = false;
                    RequestStatusDefectedEnabled = false;
                    RequestStatusReadyEnabled = false;
                    RequestStatusCompleteEnabled = false;
                    RequestStatusButtonSelectionConverter = "Complete";
                    ResetStatusEnabled = true;

                    // Checks Existing Defect Reasons on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.DefectReason)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in defectReasons.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    if (FacilitatorMainViewModel.CurrentRequest.DefectReason.Length > 2)
                    {
                        DumpsterVisible = true;
                    }
                    DefectListViewEnabled = false;

                    TotalUsersString = FacilitatorMainViewModel.CurrentRequest.TotalUsers;
                    TotalUsersEnabled = false;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.FormType))
                    {
                        SelectedFormTerm = formTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.FormType);
                    }
                    FormTypesEnabled = false;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.LOBType))
                    {
                        SelectedLOBTerm = lOBTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.LOBType);
                    }
                    LOBListViewEnabled = false;

                    // Checks Existing Request Types on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.RequestType)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in requestTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    RequestTypesEnabled = false;

                    Xref1BoxEnabled = false;
                    xref2BoxEnabled = false;
                    Xref1String = FacilitatorMainViewModel.CurrentRequest.XREF1;
                    Xref2String = FacilitatorMainViewModel.CurrentRequest.XREF2;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.RequestState))
                    {
                        SelectedState = stateTypes.FirstOrDefault(X => X.BoxItem == FacilitatorMainViewModel.CurrentRequest.RequestState);
                    }

                    // Checks Existing Systems on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.Systems)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in systems.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    SystemsListViewEnabled = false;

                    // Checks Existing Replys on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.ReplyTypes)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            foreach (FacToolItem facTool in replyTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }

                    if (FacilitatorMainViewModel.CurrentRequest.SentStatus == "Sent")
                    {
                        SendBackChecked = true; ReceivedBackChecked = false;
                    }
                    else
                    {
                        ReceivedBackChecked = true; SendBackChecked = false;
                    }

                    ReplyListViewEnabled = false;
                    SelectedState = StateTypes.ElementAt(3);
                    SendBackEnabled = false;
                    ReceivedBackEnabled = false;
                    SelectedStateEnabled = false;
                    AdditionalCommentsTextBoxString = FacilitatorMainViewModel.CurrentRequest.Comments;
                    CommentsBoxEnabled = false;
                    GenerateButtonString = "Disabled";
                    GenerateButtonEnabled = false;
                    UpdateRequestChecked = false;
                }

                // Ready Request Type Settings
                if (FacilitatorMainViewModel.CurrentRequest.RequestStatus == "Ready to Process")
                {
                    RequestNewEnabled = false;
                    RequestExistingEnabled = true;
                    RequestButtonSelectionConverter = "Existing";
                    RefTextBoxEnabled = false;
                    ReferenceTextBoxString = FacilitatorMainViewModel.CurrentRequest.ReferenceNumber;
                    GetRequestButtonEnabled = false;
                    RequestStatusDefectedEnabled = true;
                    RequestStatusReadyEnabled = true;
                    RequestStatusCompleteEnabled = true;
                    RequestStatusButtonSelectionConverter = "Ready";

                    // Checks Existing Defect Reasons on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.DefectReason)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in defectReasons.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    if (FacilitatorMainViewModel.CurrentRequest.DefectReason.Length > 2)
                    {
                        DumpsterVisible = true;
                    }

                    DefectListViewEnabled = false;

                    TotalUsersString = FacilitatorMainViewModel.CurrentRequest.TotalUsers;
                    TotalUsersEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.FormType))
                    {
                        SelectedFormTerm = formTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.FormType);
                    }
                    FormTypesEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.LOBType))
                    {
                        SelectedLOBTerm = lOBTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.LOBType);
                    }
                    LOBListViewEnabled = true;

                    // Checks Existing Request Types on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.RequestType)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in requestTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    RequestTypesEnabled = true;

                    Xref1String = FacilitatorMainViewModel.CurrentRequest.XREF1;
                    Xref2String = FacilitatorMainViewModel.CurrentRequest.XREF2;
                    Xref1BoxEnabled = true;
                    Xref2BoxEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.RequestState))
                    {
                        SelectedState = stateTypes.FirstOrDefault(X => X.BoxItem == FacilitatorMainViewModel.CurrentRequest.RequestState);
                    }

                    // Checks Existing Systems on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.Systems)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in systems.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    SystemsListViewEnabled = true;

                    // Checks Existing Replys on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.ReplyTypes)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            foreach (FacToolItem facTool in replyTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }

                    if (FacilitatorMainViewModel.CurrentRequest.SentStatus == "Sent")
                    {
                        SendBackChecked = true; ReceivedBackChecked = false;
                    }
                    else
                    {
                        ReceivedBackChecked = true; SendBackChecked = false;
                    }

                    ReplyListViewEnabled = true;
                    SendBackEnabled = true;
                    ReceivedBackEnabled = true;
                    SelectedStateEnabled = true;
                    AdditionalCommentsTextBoxString = FacilitatorMainViewModel.CurrentRequest.Comments;
                    CommentsBoxEnabled = true;
                    GenerateButtonString = "Save";
                    GenerateButtonEnabled = false;
                    UpdateRequestChecked = true;
                }

                // Defected Request Type Settings
                if (FacilitatorMainViewModel.CurrentRequest.RequestStatus == "Defected")
                {
                    RequestNewEnabled = false;
                    RequestExistingEnabled = true;
                    RequestButtonSelectionConverter = "Existing";
                    RefTextBoxEnabled = false;
                    ReferenceTextBoxString = FacilitatorMainViewModel.CurrentRequest.ReferenceNumber;
                    GetRequestButtonEnabled = false;
                    RequestStatusDefectedEnabled = true;
                    RequestStatusReadyEnabled = true;
                    RequestStatusCompleteEnabled = true;
                    RequestStatusButtonSelectionConverter = "Defected";

                    // Checks Existing Defect Reasons on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.DefectReason)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in defectReasons.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    if (FacilitatorMainViewModel.CurrentRequest.DefectReason.Length > 2)
                    {
                        DumpsterVisible = true;
                    }
                    DefectListViewEnabled = true;

                    TotalUsersString = FacilitatorMainViewModel.CurrentRequest.TotalUsers;
                    TotalUsersEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.FormType))
                    {
                        SelectedFormTerm = formTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.FormType);
                    }
                    FormTypesEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.LOBType))
                    {
                        SelectedLOBTerm = lOBTypes.FirstOrDefault(X => X.Name == FacilitatorMainViewModel.CurrentRequest.LOBType);
                    }
                    LOBListViewEnabled = true;

                    // Checks Existing Request Types on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.RequestType)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in requestTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    RequestTypesEnabled = false;

                    Xref1String = FacilitatorMainViewModel.CurrentRequest.XREF1;
                    Xref2String = FacilitatorMainViewModel.CurrentRequest.XREF2;
                    Xref1BoxEnabled = true;
                    Xref2BoxEnabled = true;

                    if (!string.IsNullOrEmpty(FacilitatorMainViewModel.CurrentRequest.RequestState))
                    {
                        SelectedState = stateTypes.FirstOrDefault(X => X.BoxItem == FacilitatorMainViewModel.CurrentRequest.RequestState);
                    }

                    if (FacilitatorMainViewModel.CurrentRequest.SentStatus == "Sent")
                    {
                        SendBackChecked = true; ReceivedBackChecked = false;
                    }
                    else
                    {
                        ReceivedBackChecked = true; SendBackChecked = false;
                    }

                    // Checks Existing Systems on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.Systems)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            //int i = systems.IndexOf(systems.Where(p => p.Name == s).FirstOrDefault());
                            foreach (FacToolItem facTool in systems.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    SystemsListViewEnabled = false;

                    // Checks Existing Replys on Request
                    foreach (string s in FacilitatorMainViewModel.CurrentRequest.ReplyTypes)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            foreach (FacToolItem facTool in replyTypes.Where(x => x.Name == s))
                            {
                                facTool.IsSelected = true;
                            }
                        }
                    }
                    ReplyListViewEnabled = false;

                    SendBackEnabled = true;
                    ReceivedBackEnabled = true;
                    SelectedStateEnabled = true;
                    AdditionalCommentsTextBoxString = FacilitatorMainViewModel.CurrentRequest.Comments;
                    CommentsBoxEnabled = true;
                    GenerateButtonString = "Save";
                    GenerateButtonEnabled = false;
                    UpdateRequestChecked = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Clear Form Function
        public void ClearForm()
        {
            FacilitatorMainViewModel.CurrentRequest = null;

            foreach (FacToolItem fI in defectReasons)
            {
                fI.IsSelected = false;
            }

            foreach (FacToolItem fI in systems)
            {
                fI.IsSelected = false;
            }

            foreach (FacToolItem fI in replyTypes)
            {
                fI.IsSelected = false;
            }

            foreach (FacToolItem fI in requestTypes)
            {
                fI.IsSelected = false;
            }

            NewForm = true;
            SelectedFormTerm = formTypes.ElementAt(0);
            SelectedLOBTerm = lOBTypes.ElementAt(0);
            RequestNewEnabled = true;
            RequestExistingEnabled = true;
            ReferenceTextBoxString = "";
            RefTextBoxEnabled = true;
            RequestStatusDefectedEnabled = true;
            DefectListViewEnabled = false;
            SystemsListViewEnabled = true;
            RequestTypesEnabled = true;
            ReplyListViewEnabled = true;
            TotalUsersString = "1";
            Xref1String = "";
            Xref2String = "";
            Xref1BoxEnabled = true;
            Xref2BoxEnabled = true;
            ResetStatusEnabled = false;
            ReceivedBackChecked = false;
            DumpsterVisible = false;
            SendBackChecked = false;
            AdditionalCommentsTextBoxString = "";
            GenerateTextBoxString = "";
            RequestButtonSelectionConverter = "New";
            StatusString = "Ready";
            RequestStatusButtonSelectionConverter = "Ready";
        }

        // Request Change Function Check if Generate Button is Enabled or Not
        public void RequestChanged()
        {
            try
            {
                // Boolean blnExisting = false;
                bool blnTotalUsers = false;
                bool blnFormType = false;
                bool blnDefectReason = false;
                bool blnDefectReasonSelected = false;
                bool blnSystems = false;
                bool blnSystemsSelected = false;
                bool blnLOBType = false; 

                // TotalUsers required
                if (TotalUsersString != null) { if (TotalUsersString != "") { blnTotalUsers = true; } } 
                else { blnTotalUsers = true; }

                // FormType Required
                if (SelectedFormTerm != null) 
                {  if (SelectedFormTerm.Name != "") { blnFormType = true; } } 
                else { blnFormType = true; }

                // LOBType Returned
                if (SelectedLOBTerm != null)
                { if (SelectedLOBTerm.Name != "") { blnLOBType = true; } }
                else { blnLOBType = true; }

                // DefectReason required if Defective
                if (defectReasons.Count > 0)
                {
                    foreach (FacToolItem item in defectReasons)
                    {
                        if (item.IsSelected == true) { blnDefectReasonSelected = true; break; }
                    }

                    if ( (blnDefectReasonSelected && RequestStatusButtonSelectionConverter == "Defected")  
                        || RequestStatusButtonSelectionConverter == "Ready" 
                        || RequestStatusButtonSelectionConverter == "Complete" ) { blnDefectReason = true;  }

                } else { blnDefectReason = true; }

                // Systems Required if Ready
                if (systems.Count > 0)
                {
                    foreach (FacToolItem item in systems)
                    {
                        if (item.IsSelected == true) { blnSystemsSelected = true; break; }
                    }
                    if ((blnSystemsSelected && RequestStatusButtonSelectionConverter == "Ready") 
                        || RequestStatusButtonSelectionConverter == "Defected" 
                        || RequestStatusButtonSelectionConverter == "Complete" ) { blnSystems = true; }
                } else { blnSystems = true; }

                // Reply Types
                if (replyTypes.Count > 0)
                {
                    foreach (FacToolItem item in replyTypes)
                    {
                        if (item.IsSelected == true) 
                        { 
                            if (item.Name == "Approve" || item.Name == "No Response") { if (SelectedState.BoxItem == "Pre-Approval") { SelectedState = stateTypes.ElementAt(1); } }
                            break; 
                        }
                    }
                }

                // Request Types
                if (requestTypes.Count > 0)
                {
                    foreach (FacToolItem item in requestTypes)
                    {
                        if (item.IsSelected == true) { break; }
                    }
                }

                // Final If / Else to Enable or Disable Generate Button
                if (blnTotalUsers && blnFormType && blnDefectReason && blnSystems)
                {
                    GenerateButtonEnabled = true;
                }
                else
                {
                    GenerateButtonEnabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Request Mode Change Config Function
        public void RequestModeChanged(ConfigurationProperties.RequestMode requestMode)
        {
            try
            {
                // If CurrentRequest == null, sets form fields. Otherwise does a RequestChanged
                if (FacilitatorMainViewModel.CurrentRequest == null)
                {
                    // New Mode Config
                    if (requestMode == ConfigurationProperties.RequestMode.New)
                    {
                        RequestNewEnabled = true;
                        RefTextBoxEnabled = false;
                        GetRequestButtonEnabled = false;
                        RequestStatusButtonSelectionConverter = "Ready";
                        RequestStatusDefectedEnabled = true;
                        RequestStatusReadyEnabled = true;
                        RequestStatusCompleteEnabled = true;
                        DefectListViewEnabled = false;
                        TotalUsersString = "1";
                        TotalUsersEnabled = true;
                        FormTypesEnabled = true;
                        LOBListViewEnabled = true;
                        SelectedLOBTerm = LOBTypes.ElementAt(0);
                        RequestTypesEnabled = true;
                        Xref1BoxEnabled = true;
                        Xref2BoxEnabled = true;
                        SystemsListViewEnabled = true;
                        ReplyListViewEnabled = true;
                        SelectedState = StateTypes.ElementAt(0);
                        SendBackEnabled = true;
                        ReceivedBackEnabled = true;
                        SelectedStateEnabled = true;
                        CommentsBoxEnabled = true;
                        GenerateTextBoxString = "Generate";
                    }

                    // Existing Mode Config
                    if (requestMode == ConfigurationProperties.RequestMode.Existing)
                    {
                        RequestNewEnabled = true;
                        RefTextBoxEnabled = true;
                        GetRequestButtonEnabled = false;
                        RequestStatusButtonSelectionConverter = "Ready";
                        DefectListViewEnabled = false;
                        TotalUsersEnabled = false;
                        FormTypesEnabled = false;
                        LOBListViewEnabled = true;
                        RequestTypesEnabled = false;
                        Xref1BoxEnabled = false;
                        xref2BoxEnabled = false;
                        SystemsListViewEnabled = false;
                        ReplyListViewEnabled = false;
                        SendBackEnabled = true;
                        ReceivedBackEnabled = true;
                        SelectedStateEnabled = true;
                        CommentsBoxEnabled = false;
                        GenerateTextBoxString = "Save";
                    }
                }
                else
                {
                    SetForm(requestMode);
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Request Mode Change Config Function
        public void RequestModeChangedButton(ConfigurationProperties.RequestMode requestMode)
        {
            try
            {
                // If CurrentRequest == null, sets form fields. Otherwise does a RequestChanged
                if (FacilitatorMainViewModel.CurrentRequest == null)
                {
                    // New Mode Config
                    if (requestMode == ConfigurationProperties.RequestMode.New)
                    {
                        RequestNewEnabled = true;
                        RefTextBoxEnabled = false;
                        GetRequestButtonEnabled = false;
                        RequestStatusButtonSelectionConverter = "Ready";
                        RequestStatusDefectedEnabled = true;
                        RequestStatusReadyEnabled = true;
                        RequestStatusCompleteEnabled = true;
                        DefectListViewEnabled = false;
                        TotalUsersString = "1";
                        TotalUsersEnabled = true;
                        FormTypesEnabled = true;
                        LOBListViewEnabled = true;
                        SelectedLOBTerm = LOBTypes.ElementAt(0);
                        RequestTypesEnabled = true;
                        Xref1BoxEnabled = true;
                        Xref2BoxEnabled = true;
                        SystemsListViewEnabled = true;
                        ReplyListViewEnabled = true;
                        SelectedState = StateTypes.ElementAt(0);
                        SendBackEnabled = true;
                        ReceivedBackEnabled = true;
                        SelectedStateEnabled = false;
                        CommentsBoxEnabled = true;
                        GenerateButtonString = "Generate";
                    }

                    // Existing Mode Config
                    if (requestMode == ConfigurationProperties.RequestMode.Existing)
                    {
                        RequestNewEnabled = true;
                        RefTextBoxEnabled = true;
                        GetRequestButtonEnabled = false;
                        RequestStatusButtonSelectionConverter = "Ready";
                        DefectListViewEnabled = false;
                        TotalUsersEnabled = false;
                        FormTypesEnabled = false;
                        LOBListViewEnabled = false;
                        RequestTypesEnabled = false;
                        Xref1BoxEnabled = false;
                        xref2BoxEnabled = false;
                        SystemsListViewEnabled = false;
                        ReplyListViewEnabled = false;
                        SendBackEnabled = true;
                        ReceivedBackEnabled = true;
                        SelectedStateEnabled = true;
                        CommentsBoxEnabled = false;
                        GenerateButtonString = "Save";
                    }
                }
                else
                {
                    SetForm(requestMode);
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Request Status Change Config Function
        public void RequestStatusChanged(string passedStatus)
        {
            try
            {
                // Defected Button Selected
                if (passedStatus == "Defected")
                {
                    if (NewForm == true)
                    {
                        ReferenceTextBoxString = "";
                        TotalUsersString = "1";
                        AdditionalCommentsTextBoxString = "";
                        GenerateTextBoxString = "";
                        Xref1String = "";
                        Xref2String = "";
                        SelectedFormTerm = formTypes.ElementAt(0);
                        SelectedRequestTerm = requestTypes.ElementAt(0);
                        SelectedLOBTerm = lOBTypes.ElementAt(0);
                    }

                    TotalUsersEnabled = true;
                    FormTypesEnabled = true;
                    LOBListViewEnabled = true;
                    DefectListViewEnabled = true;
                    RequestTypesEnabled = false;
                    ReplyListViewEnabled = false;
                    SystemsListViewEnabled = false;
                    
                    SelectedFormTerm = formTypes.ElementAt(0);
                    foreach (FacToolItem fI in systems)
                    {
                        fI.IsSelected = false;
                    }

                    foreach (FacToolItem fI in replyTypes)
                    {
                        fI.IsSelected = false;
                    }
                    SendBackEnabled = true;
                    ReceivedBackEnabled = true;

                   if (RequestButtonSelectionConverter == "New")
                    {
                        SelectedStateEnabled = false;
                    }
                   else
                    {
                        SelectedStateEnabled = true;
                    }
                    RequestChanged();
                }

                // Ready Button Selected
                if (passedStatus == "Ready")
                {

                    if (NewForm == true)
                    {
                        ReferenceTextBoxString = "";
                        TotalUsersString = "1";
                        AdditionalCommentsTextBoxString = "";
                        GenerateTextBoxString = "";
                        Xref1String = "";
                        Xref2String = "";
                        SelectedFormTerm = formTypes.ElementAt(0);
                        SelectedRequestTerm = requestTypes.ElementAt(0);
                        SelectedLOBTerm = lOBTypes.ElementAt(0);
                    }

                    TotalUsersEnabled = true;
                    FormTypesEnabled = true;
                    DefectListViewEnabled = false;
                    RequestTypesEnabled = true;
                    ReplyListViewEnabled = true;
                    SystemsListViewEnabled = true;
                    foreach (FacToolItem fI in defectReasons)
                    {
                        fI.IsSelected = false;
                    }

                    foreach (FacToolItem fI in replyTypes)
                    {
                        fI.IsSelected = false;
                    }
                    SendBackEnabled = true;
                    ReceivedBackEnabled = true;
                    SelectedStateEnabled = true;
                    RequestChanged();

                    if (RequestButtonSelectionConverter == "New")
                    {
                        SelectedStateEnabled = false;
                    }
                    else
                    {
                        SelectedStateEnabled = true;
                    }
                }

                // Complete Button Selected
                if (passedStatus == "Complete" || passedStatus == "Cancelled")
                {
                    RequestChanged();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Get Request Button Click Function
        public void GetRequestButton()
        {
            try
            {
                string requestnew = "";
                string requeststatus = "";
                string refnumber = ReferenceTextBoxString.ToUpper().Trim();
                ReferenceTextBoxString = refnumber;
                FacilitatorMainViewModel.ReferenceNumber = refnumber;
                GetRequest(FacilitatorMainViewModel.ReferenceNumber);

                if (FacilitatorMainViewModel.CurrentRequest != null)
                {
                    requestnew = FacilitatorMainViewModel.CurrentRequest.NewRequest;
                    requeststatus = FacilitatorMainViewModel.CurrentRequest.RequestStatus;

                    // Sets New as Existing on Second Save
                    if (requestnew == "New" && requeststatus != "Complete")
                    {
                        FacilitatorMainViewModel.CurrentRequest.NewRequest = "Existing";
                        SetForm(ConfigurationProperties.RequestMode.Existing);
                    }
                    // Sets pre-existing to existing
                    if (requestnew == "Pre-Existing" && requeststatus != "Complete")
                    {
                        FacilitatorMainViewModel.CurrentRequest.NewRequest = "Existing";
                        SetForm(ConfigurationProperties.RequestMode.PreExisting);
                    }
                    if (requestnew == "Existing" && requeststatus != "Complete")
                    {
                        SetForm(ConfigurationProperties.RequestMode.Existing);
                    }
                    if (requeststatus == "Complete")
                    {
                        SetForm(ConfigurationProperties.RequestMode.Complete);
                    }
                }

                
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
        
        // Get Request Search Main Function
        public void GetRequest(string refnumber)
        {
            try
            {
                string refNumber = refnumber;
                ReferenceTextBoxString = refNumber;

                ObservableCollection<FacToolRequest> results = new();
                DataTable tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestTable, "ReferenceNumber", refNumber);
                DataTable statusTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestStatus, "ReferenceNumber", refnumber);
                // Create a Collection of FacToolRequest items from DataTable results, Selecting the Highest Value Id Record as Current Request

                // One set of Fields if the Status Table Returns Empty
                if (tempTable.Rows.Count>0) 
                {
                    results.Add(new FacToolRequest
                    {
                        _id = Convert.ToInt32(tempTable.Rows[0][0]),
                        CreateDate = tempTable.Rows[0][1].ToString(),
                        CreateTick = tempTable.Rows[0][2].ToString(),
                        ModifiedDate = tempTable.Rows[0][3].ToString(),
                        ModifiedTick = tempTable.Rows[0][4].ToString(),
                        SamAccount = tempTable.Rows[0][5].ToString(),
                        DisplayName = tempTable.Rows[0][6].ToString(),
                        ReferenceNumber = tempTable.Rows[0][7].ToString(),
                        NewRequest = tempTable.Rows[0][8].ToString(),
                        TotalUsers = tempTable.Rows[0][9].ToString(),
                        RequestStatus = tempTable.Rows[0][10].ToString(),
                        FormType = tempTable.Rows[0][11].ToString(),
                        LOBType = tempTable.Rows[0][19].ToString(),
                        RequestType = tempTable.Rows[0][12].ToString().Split(','),
                        DefectReason = tempTable.Rows[0][13].ToString().Split(','),
                        Systems = tempTable.Rows[0][14].ToString().Split(','),
                        ReplyTypes = tempTable.Rows[0][15].ToString().Split(','),
                        Comments = tempTable.Rows[0][16].ToString(),
                        XREF1 = tempTable.Rows[0][17].ToString(),
                        XREF2 = tempTable.Rows[0][18].ToString()
                    });
                }

                // Fill Properties from the Request Status Table if there is a matching Reference Number
                if (statusTable.Rows.Count > 0 && results.Count > 0)
                {
                    results[0].ReqStatusID = Int32.Parse(statusTable.Rows[0][0].ToString());
                    results[0].SentStatus = statusTable.Rows[0][2].ToString();
                    results[0].RequestState = statusTable.Rows[0][3].ToString();
                    results[0].TouchPoints = statusTable.Rows[0][4].ToString();
                    results[0].TimesReturned = statusTable.Rows[0][5].ToString();
                    results[0].SLAStart = statusTable.Rows[0][6].ToString();
                    results[0].CompletionDate = statusTable.Rows[0][7].ToString();
                    results[0].SLACompletionTime = statusTable.Rows[0][8].ToString();
                    results[0].AgentComments = statusTable.Rows[0][9].ToString();
                    results[0].AgentsWorked = statusTable.Rows[0][10].ToString();
                }

                if (results.Count > 0)
                {
                    ActiveRequest = null;
                    FacilitatorMainViewModel.CurrentRequest = results[0];
                    StatusString = "Ref # " + FacilitatorMainViewModel.CurrentRequest.ReferenceNumber + " - Request found";
                    ActiveRequest = FacilitatorMainViewModel.CurrentRequest;
                }
                else
                {
                    ReferenceTextBoxString = refNumber;
                    StatusString = "Ref # " + refNumber + " - Request not found";
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Main Function to Generate Request
        public void GenerateRequest()
        {
            try
            {
                // Build Request Properties from UI Inputs
                string subject = "";
                string newRequest; if (RequestButtonSelectionConverter == "New") { newRequest = "New"; } else { newRequest = "Existing"; }
                string requestStatus; if (RequestStatusButtonSelectionConverter == "Ready") { requestStatus = "Ready to Process"; } 
                else if (RequestStatusButtonSelectionConverter == "Defected") { requestStatus = "Defected"; }
                else { requestStatus = "Complete"; }
                string selectedFormType; if (SelectedFormTerm != null) { selectedFormType = SelectedFormTerm.Name; } else { selectedFormType = ""; }
                string selectedLOBType; if (SelectedLOBTerm != null) { selectedLOBType = SelectedLOBTerm.Name; } else { selectedLOBType = ""; }

                List<string> selectedDefectReasons = new();
                foreach (FacToolItem fI in defectReasons)
                {
                    if (fI.IsSelected == true)
                    {
                        selectedDefectReasons.Add(fI.Name);
                    }
                }
                string joinedDefectReasons = string.Join(",", selectedDefectReasons);

                List<string> selectedSystems = new();
                foreach (FacToolItem fI in systems)
                {
                    if (fI.IsSelected == true)
                    {
                        selectedSystems.Add(fI.Name);
                    }
                }
                string joinedSystems = string.Join(",", selectedSystems);

                List<string> selectedReplys = new();
                foreach (FacToolItem fI in replyTypes)
                {
                    if (fI.IsSelected == true)
                    {
                        selectedReplys.Add(fI.Name);
                    }
                }
                string joinedReplyTypes = string.Join(",", selectedReplys);

                List<string> selectedRequests = new();
                foreach (FacToolItem fI in requestTypes)
                {
                    if (fI.IsSelected == true)
                    {
                        selectedRequests.Add(fI.Name);
                    }
                }
                string joinedRequestTypes = string.Join(",", selectedRequests);
                string totalUsersString = "(" + TotalUsersString + " users)";
                int intTotalUsers; if (!string.IsNullOrEmpty(TotalUsersString)) { intTotalUsers = Convert.ToInt32(TotalUsersString); } else { intTotalUsers = 1; }
                string initials = "{" + App.GlobalUserInfo.UserInitials + "}";
                string stringDate = DateTime.Now.ToString("MMddyy");
                int intSequence;
                string stringSequence;

                DataTable RefCheck = new();
                // Finds the Next Reference Number to Use for Current New Request
                FacToolRefNumber refNumber = new();
                if (RequestButtonSelectionConverter == "Existing")
                {
                    RefCheck = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestTable, "ReferenceNumber", ReferenceTextBoxString.ToUpper());
                    FacilitatorMainViewModel.ReferenceNumber = ReferenceTextBoxString.ToUpper();
                }
                else
                {
                    try
                    {
                        refNumber = GetNextReferenceNumber();
                        intSequence = Convert.ToInt32(refNumber.Sequence);
                        stringSequence = refNumber.ReferenceSequence;
                        FacilitatorMainViewModel.ReferenceNumber = refNumber.ReferenceNumber;
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                }

                string stringComments = AdditionalCommentsTextBoxString;
                string stringXref1 = Xref1String;
                string stringXref2 = Xref2String;

                // Generate Text Subject Line Created off List View Choices Made
                if (RequestButtonSelectionConverter == "Existing" && RequestStatusButtonSelectionConverter == "Ready")
                {
                    if (selectedFormType == "IT-020")
                    {
                        subject += "[EXTERNAL][" + joinedRequestTypes + "]";
                    }
                    else
                    {
                        subject += "[" + joinedRequestTypes + "]";
                    }

                    if (joinedReplyTypes.Length > 40)
                    {
                        subject += "([" + joinedReplyTypes[40..] + "]";
                    }
                    else
                    {
                        subject += "([" + joinedReplyTypes + "]";
                    }

                    if (joinedSystems.Length > 80)
                    {
                        subject += joinedSystems[80..] + ")";
                    }
                    else
                    {
                        subject += joinedSystems + ")";
                    }

                    subject += totalUsersString;
                    subject += initials;
                    subject += "- Ref # " + FacilitatorMainViewModel.ReferenceNumber;
                    subject += " SECUREMAIL - ";
                    subject += stringComments;
                }

                if (RequestButtonSelectionConverter == "New" && RequestStatusButtonSelectionConverter == "Ready")
                {
                    if (selectedFormType == "IT-020")
                    {
                        subject += "([EXTERNAL][" + joinedRequestTypes + "]";

                    }
                    else
                    {
                        subject += "([" + joinedRequestTypes + "]";
                    }

                    if (joinedReplyTypes.Length > 40)
                    {
                        subject += "([" + joinedReplyTypes[40..] + "]";
                    }
                    else
                    {
                        subject += "([" + joinedReplyTypes + "]";
                    }


                    if (joinedSystems.Length > 80)
                    {
                        subject += joinedSystems[80..] + ")";
                    }
                    else
                    {
                        subject += joinedSystems + ")";
                    }
                    subject += totalUsersString;
                    subject += initials;
                    subject += "- Ref # " + FacilitatorMainViewModel.ReferenceNumber;
                    subject += " SECUREMAIL - ";
                    subject += stringComments;
                }

                if (RequestStatusButtonSelectionConverter == "Defected")
                {
                    subject = "([" + joinedDefectReasons + "]";
                    subject += "[" + selectedFormType + "]";
                    subject += totalUsersString;
                    subject += initials;
                    subject += "- Ref # " + FacilitatorMainViewModel.ReferenceNumber;
                    subject += " SECUREMAIL - ";
                    subject += stringComments;
                }

                if (RequestStatusButtonSelectionConverter == "Complete")
                {
                    subject += "- Ref # " + FacilitatorMainViewModel.ReferenceNumber + "Marked as Completed.";
                }
                 
                    GenerateTextBoxString = subject;
                StatusString = "Generated Subject Ref # : " + FacilitatorMainViewModel.ReferenceNumber;
                DateTime createDate = new();

                createDate = DateTime.UtcNow;

                string sendStatus = "";
                if (ReceivedBackChecked == true) { sendStatus = "Received"; } else { sendStatus = "Sent"; }



                // Calculate Touch Points as System.Count * Total Users
                int systemCount = selectedSystems.Count;
                _ = int.TryParse(TotalUsersString, out int totalUsers);
                int touchPoints = systemCount * totalUsers; 

                string slaStartDate = " ";
                string slaStartCurrent = " ";

                try
                {
                    // Resets SLA-Start Time if Set back to Pre-Approval
                    slaStartDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    if (SelectedState.BoxItem == "Pre-Approval") { slaStartDate = "Pre-Approval"; }
                    if (SelectedState.BoxItem == "Pre-Approval") { slaStartCurrent = "Pre-Approval"; } 


                    if (SelectedState.BoxItem != "Pre-Approval")
                    {
                        if (FacilitatorMainViewModel.CurrentRequest != null)
                        {
                            if (FacilitatorMainViewModel.CurrentRequest.SLAStart == "Pre-Approval")
                            {
                                slaStartCurrent = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            else
                            {
                                slaStartCurrent = FacilitatorMainViewModel.CurrentRequest.SLAStart;
                            }    
                        }
                    }
                }

                catch (Exception ex)
                {
                    ExceptionOutput.Output(ex.ToString());
                }

                // If not Incomplete, and Box is set to Completed, marks the Completion Date 
                string completionDate = ""; if (SelectedState.BoxItem != "Completed" && SelectedState.BoxItem != "Cancelled") 
                { 
                    completionDate = "Incomplete"; 
                } 
                else if ((SelectedState.BoxItem == "Completed" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Incomplete") || (SelectedState.BoxItem == "Completed" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Cancelled"))
                {
                    completionDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
                }
                else if (SelectedState.BoxItem == "Cancelled" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Incomplete")
                {
                    completionDate = "Cancelled";
                }
                else
                {
                    completionDate = FacilitatorMainViewModel.CurrentRequest.CompletionDate;
                }

                // If not Incomplete, and Box is set to Completed, marks the Completion Time as Calculated between SLA Start and End Date
                string completionTime = ""; if (SelectedState.BoxItem != "Completed" && SelectedState.BoxItem != "Cancelled") 
                { 
                    completionTime = "Incomplete"; 
                } 
                else if ((SelectedState.BoxItem == "Completed" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Incomplete") || (SelectedState.BoxItem == "Completed" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Cancelled"))
                {
                    if (completionDate != "Incomplete" && slaStartDate != "Pre-Approval")
                    {
                        completionTime = CalculateCompletionTime(slaStartCurrent, completionDate);
                        requestStatus = "Complete";
                    }
                }
                else if (SelectedState.BoxItem == "Cancelled" && FacilitatorMainViewModel.CurrentRequest.CompletionDate == "Incomplete")
                {
                    if (completionDate != "Incomplete" && slaStartDate != "Pre-Approval")
                    {
                        completionTime = "Cancelled";
                        requestStatus = "Cancelled";
                    }
                }
                else
                { 
                    completionTime = FacilitatorMainViewModel.CurrentRequest.SLACompletionTime;
                }

                // If Update Request is Checked, will Update the Reference Number to Newest, and Insert New Request
                if (UpdateRequestChecked == true)
                {
                    try
                    {
                        if (RequestButtonSelectionConverter == "Existing")
                        {
                            FacToolRequest request = new()
                            {
                                CreateDate = FacilitatorMainViewModel.CurrentRequest.CreateDate,
                                CreateTick = FacilitatorMainViewModel.CurrentRequest.CreateTick,
                                ModifiedDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                                ModifiedTick = createDate.Ticks.ToString(),
                                SamAccount = App.GlobalUserInfo.SamAccountName,
                                DisplayName = App.GlobalUserInfo.DisplayName,
                                ReferenceNumber = FacilitatorMainViewModel.ReferenceNumber,
                                NewRequest = newRequest,
                                TotalUsers = intTotalUsers.ToString(),
                                RequestStatus = requestStatus,
                                FormType = selectedFormType,
                                RequestType = selectedRequests.ToArray(),
                                DefectReason = selectedDefectReasons.ToArray(),
                                Systems = selectedSystems.ToArray(),
                                ReplyTypes = selectedReplys.ToArray(),
                                Comments = stringComments,
                                XREF1 = stringXref1,
                                XREF2 = stringXref2,
                                LOBType = selectedLOBType,
                                SentStatus = sendStatus,
                                RequestState = SelectedState.BoxItem,
                                TouchPoints = touchPoints.ToString(),
                                TimesReturned = FacilitatorMainViewModel.CurrentRequest.TimesReturned,
                                SLAStart = slaStartCurrent,
                                CompletionDate = completionDate,
                                SLACompletionTime = completionTime,
                                AgentComments = FacilitatorMainViewModel.CurrentRequest.AgentComments,
                                AgentsWorked = FacilitatorMainViewModel.CurrentRequest.AgentsWorked
                            };

                            int checkDigit = DBConn.FacToolUpdateRequest(request, ConfigurationProperties.FactoolRequestTable);
                            // Function to Set Default Values for Records Pre-Dating Status Table Addition
                            DataTable checkTable = DBConn.GetSelectedRecord(ConfigurationProperties.FactoolRequestStatus, "ReferenceNumber", request.ReferenceNumber);
                            if (checkTable.Rows.Count > 0) { checkDigit = DBConn.FacToolUpdateRequestStatus(request, ConfigurationProperties.FactoolRequestStatus); }
                            else { DBConn.AddFactoolRequestStatus(request, ConfigurationProperties.FactoolRequestStatus); }
                            
                            if (checkDigit != -1)
                            {
                                GenerateButtonEnabled = false;
                                Clipboard.SetText(GenerateTextBoxString, TextDataFormat.Text);
                                StatusString = "Reference Number : " + FacilitatorMainViewModel.ReferenceNumber + " Updated.";
                                FactoolRequestHistory PassedHistory = new();
                                PassedHistory = HBuilder.HistoryBuilder(ActiveRequest, request, "Existing"); // Passing Original and Updated Request to Build History
                                DBConn.AddFactoolRequestHistory(PassedHistory,ConfigurationProperties.FactoolRequestHistory);
                            }
                            else
                            {
                                StatusString = "Reference Number Update FAILED";
                            }
                        }

                        else
                        {
                            FacToolRequest request = new()
                            {
                                CreateDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                                CreateTick = createDate.Ticks.ToString(),
                                ModifiedDate = createDate.ToString("yyyy/MM/dd HH:mm:ss.fff"),
                                ModifiedTick = createDate.Ticks.ToString(),
                                SamAccount = App.GlobalUserInfo.SamAccountName,
                                DisplayName = App.GlobalUserInfo.DisplayName,
                                ReferenceNumber = FacilitatorMainViewModel.ReferenceNumber,
                                NewRequest = newRequest,
                                TotalUsers = intTotalUsers.ToString(),
                                RequestStatus = requestStatus,
                                FormType = selectedFormType,
                                LOBType = selectedLOBType,
                                RequestType = selectedRequests.ToArray(),
                                DefectReason = selectedDefectReasons.ToArray(),
                                Systems = selectedSystems.ToArray(),
                                ReplyTypes = selectedReplys.ToArray(),
                                Comments = stringComments,
                                XREF1 = stringXref1,
                                XREF2 = stringXref2,
                                SentStatus = sendStatus,
                                RequestState = SelectedState.BoxItem,
                                TouchPoints = touchPoints.ToString(),
                                TimesReturned = "0",
                                SLAStart = slaStartDate,
                                CompletionDate = "Incomplete",
                                SLACompletionTime = "Incomplete",
                                AgentComments = "",
                                AgentsWorked = ""
                            };

                            int checkDigit = DBConn.AddFactoolRequest(request, ConfigurationProperties.FactoolRequestTable);
                            checkDigit = DBConn.FacToolUpdateRequestNumber(refNumber, ConfigurationProperties.FactoolRequestNumberTable);
                            checkDigit = DBConn.AddFactoolRequestStatus(request, ConfigurationProperties.FactoolRequestStatus);

                            if (checkDigit != -1)
                            {
                                GenerateButtonEnabled = false;
                                Clipboard.SetText(GenerateTextBoxString, TextDataFormat.Text);
                                StatusString = "Added Request Ref # : " + FacilitatorMainViewModel.ReferenceNumber;
                                FactoolRequestHistory PassedHistory = new();
                                PassedHistory = HBuilder.HistoryBuilder(ActiveRequest, request, "New"); // Passing Original and Updated Request to Build History
                                DBConn.AddFactoolRequestHistory(PassedHistory, ConfigurationProperties.FactoolRequestHistory);
                            }
                            else
                            {
                                StatusString = "Reference Number Add FAILED";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                }

                // Sets the Generate Textbox to Clipboard Copy
                Clipboard.SetText(GenerateTextBoxString);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Determines the new Reference Number to Generate Given Previous Number
        public FacToolRefNumber GetNextReferenceNumber()
        {
            FacToolRefNumber nextNumber = new();
            try
            {
                FacToolRefNumber returnNumber = new();
                string stringDate = DateTime.Now.ToString("MMddyy");
                string stringReferenceDate;
                int intSequence;
                string stringSequence;
                string stringNewRefNumber;
                ObservableCollection<FacToolRefNumber> refNumbers = new();

                DataTable tempTable = DBConn.GetTable(ConfigurationProperties.FactoolRequestNumberTable);

                foreach (DataRow row in tempTable.Rows)
                {
                    refNumbers.Add(new FacToolRefNumber
                    {
                        _id = Convert.ToInt32(row[0]),
                        ReferenceDate = row[1].ToString(),
                        Sequence = row[2].ToString(),
                        ReferenceSequence = row[3].ToString(),
                        ReferenceNumber = row[4].ToString(),
                        PreviousReferenceNumber = row[5].ToString(),
                    }
                   );
                }
                returnNumber = refNumbers.ElementAt(0);


                stringReferenceDate = returnNumber.ReferenceDate;
                intSequence = Convert.ToInt32(returnNumber.Sequence);

                if (stringDate == stringReferenceDate)
                {
                    if (intSequence < 999)
                    {
                        intSequence++;
                    }
                    else
                    {
                        intSequence = 1;
                    }
                }
                else
                {
                    stringReferenceDate = stringDate;
                    intSequence = 1;
                }

                stringSequence = intSequence.ToString().PadLeft(3, '0');
                nextNumber._id = returnNumber._id;
                nextNumber.ReferenceDate = stringReferenceDate;
                nextNumber.Sequence = intSequence.ToString();
                nextNumber.ReferenceSequence = stringSequence;
                stringNewRefNumber = "N" + stringReferenceDate + stringSequence + "P2";
                nextNumber.ReferenceNumber = stringNewRefNumber;
                nextNumber.PreviousReferenceNumber = returnNumber.ReferenceNumber;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }

            return nextNumber;
        }

        // Function to export Request Table to a CSV file
        public async void CSVExport()
        {
            try
            {
                if (MessageBox.Show("Export CSV? This takes time.", "Export CSV?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    IsLoadBool = true;
                    if (ExportDataTable.Rows.Count < 1)
                    {
                        var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                        ExportDataTable = dataTask.Copy();
                    }
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "CSV files|*.csv",
                        Title = "Save a CSV File",
                        FileName = "FactoolRequestDatabase.csv"
                    };
                    StreamWriter writer = null;

                    string saveCSV = DataTableToCSV.DataTableCSVConversion(ExportDataTable, '|');

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
                else
                {
                    // Do Nothing
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }
         
        // Clear Form Button Press Function
        public void ClearFormButton()
        {
            FacilitatorMainViewModel.CurrentRequest = null;
            SetListViews();
            ClearForm();
            SetForm(ConfigurationProperties.RequestMode.New);
        }
         
        // Uses Checkbox on Space Bar Press
       public void SpaceCheckbox(string ListBox)
        {
            switch(ListBox)
            {
                case "DefectReason":
                    foreach (FacToolItem facTool in defectReasons.Where(x => x._id == SelectedDefectReason._id))
                    {
                        if (facTool.IsSelected == true)
                        {
                            facTool.IsSelected = false;
                        }
                        else
                        {
                            facTool.IsSelected = true;
                        }
                    }
                    break;

                case "Systems":
                    foreach (FacToolItem facTool in systems.Where(x => x._id == SelectedSystemTerm._id))
                    {
                        if (facTool.IsSelected == true)
                        {
                            facTool.IsSelected = false;
                        }
                        else
                        {
                            facTool.IsSelected = true;
                        }
                    }
                    break;

                case "ReplyType":
                    foreach (FacToolItem facTool in replyTypes.Where(x => x._id == SelectedReplyTerm._id))
                    {
                        if (facTool.IsSelected == true)
                        {
                            facTool.IsSelected = false;
                        }
                        else
                        {
                            facTool.IsSelected = true;
                        }
                    }
                    break;

                case "RequestType":
                    foreach (FacToolItem facTool in requestTypes.Where(x => x._id == SelectedRequestTerm._id))
                    {
                        if (facTool.IsSelected == true)
                        {
                            facTool.IsSelected = false;
                        }
                        else
                        {
                            facTool.IsSelected = true;
                        }
                    }
                    break;
            }
        }

        // Function called on Reset Status Button, Sets a Complete Ticket back to Ready State
        public void ResetStatus()
        {
           // If CurrentRequest == null, sets form fields. Otherwise does a RequestChanged
                if (FacilitatorMainViewModel.CurrentRequest != null)
            {
                FacilitatorMainViewModel.CurrentRequest.RequestStatus = "Ready to Process";
                RequestStatusButtonSelectionConverter = "Ready";
                SetForm(ConfigurationProperties.RequestMode.Existing);
            }
        }

        //Export DataTable to Clipboard
        public async void ExportClipboard(string type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            try
            {
                if (MessageBox.Show("Export to Clipboard? This takes time.", "Export Clipboard?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var newline = System.Environment.NewLine;
                    var tab = "\t";
                    var clipboard_string = new StringBuilder();

                    DataTable exportTable = new();
                    IsLoadBool = true;

                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    exportTable = dataTask.Copy();

                    foreach (DataColumn dc in exportTable.Columns)
                    {
                        clipboard_string.Append(dc.ColumnName + tab);
                    }
                    clipboard_string.Append(newline);

                    foreach (DataRow row in exportTable.Rows)
                    {
                        foreach (DataColumn dc in exportTable.Columns)
                        {
                            clipboard_string.Append(row[dc].ToString() + tab);
                        }

                        clipboard_string.Append(newline);
                    }

                    Clipboard.SetText(clipboard_string.ToString());
                    IsLoadBool = false;
                }
                else
                {
                    // Do Nothing
                }
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

                if (MessageBox.Show("Export to HTML? This takes time.", "Export HTML?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string export = "";
                    DataTable ExportTable = new();
                    List<String> BuildList = new();
                    IsLoadBool = true;
                    var dataTask = await DBConn.GetTableAsync(ConfigurationProperties.FactoolRequestTable);
                    ExportTable = dataTask.Copy();

                    string[] columnNames = ExportTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
                    foreach (string s in columnNames)
                    {
                        BuildList.Add(s);
                    }

                    string built = string.Join(",", BuildList);
                    export = GetHTML(ExportTable, BuildList);
                    SaveFileDialog dlg = new()
                    {
                        FileName = "RequestExport",
                        DefaultExt = ".html",
                        Filter = "HTML Documents (.html)|*.html"
                    };

                    Nullable<bool> result = dlg.ShowDialog();

                    if (result == true)
                    {
                        File.WriteAllText(dlg.FileName, export);
                    }

                    IsLoadBool = false;
                }
                else
                {
                    // Do Nothing
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

        // Calculate the Completion time between SLA Start and Completion Dates
        public static string CalculateCompletionTime(string startTime, string endTime)
        {

            string completionTime = "";
            try
            {
                DateTime StartTime = Convert.ToDateTime(startTime);
                DateTime CompletionTime = Convert.ToDateTime(endTime);

                TimespanCalculation.DaySpan DS = TimespanCalculation.ComputeDaysDifference(StartTime, CompletionTime, false, false);

                completionTime = string.Format("{0} days, {1} hours, {2} minutes, {3} seconds", DS.Days.ToString(), DS.Hours.ToString(), DS.Minutes.ToString(), DS.Seconds.ToString());
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return completionTime;
        }
        #endregion Functions

        #region Messages
        // Listen to New Record for Database Reload
        public void TabMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                switch (passedMessage)
                {

                    case "Select FacTool Tab 1":
                        GetRequest(FacilitatorMainViewModel.ReferenceNumber);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Centrify.DirectControl.API;
using System.Linq;
using System.Data;
using System.Windows;
using System.Threading.Tasks;
using System.Security.Principal;
using MSG = GalaSoft.MvvmLight.Messaging;
using System.Reflection;
using SimpleImpersonation;
using System.DirectoryServices.AccountManagement;
using IAMHeimdall.Resources.Commands;
using System.Threading;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolGetUserViewModel : BaseViewModel
    {
        #region Delegates
        // Relay Commands
        public RelayCommand GetUsersCommand { get; set; }
        public RelayCommand ResetFormCommand { get; set; }
        public RelayCommand CopyUsersColumnCommand { get; set; }
        public RelayCommand CopyNoUsersColumnCommand { get; set; }
        #endregion

        #region Properties
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

        // Binds User Name Entry Box
        private string userEntryTextBox;
        public string UserEntryTextBox
        {
            get { return userEntryTextBox; }
            set { if (userEntryTextBox != value) 
                { userEntryTextBox = value; 
                    if (userEntryTextBox.Length > 5) 
                    { GetUsersEnabled = true; 
                    }
                    else 
                    {
                      GetUsersEnabled = false;
                    }
                    OnPropertyChanged(); 
                } }
        }

        // Binds User Button Enabled Bool
        private bool getUsersEnabled;
        public bool GetUsersEnabled
        {
            get { return getUsersEnabled; }
            set { if (getUsersEnabled != value) { getUsersEnabled = value; OnPropertyChanged(); } }
        }

        // Bool to starting Loading Animation
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }

        // Holds Data for Display Table
        private DataTable usersdisplayTable;
        public DataTable UsersDisplayTable
        {
            get { return usersdisplayTable; }
            set { usersdisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView usersdisplayView;
        public DataView UsersDisplayView
        {
            get { return usersdisplayView; }
            set { usersdisplayView = value; OnPropertyChanged(); }
        }

        // Holds data for Display Table
        private DataTable noUsersdisplayTable;
        public DataTable NoUsersDisplayTable
        {
            get { return noUsersdisplayTable; }
            set { noUsersdisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView nousersdisplayView;
        public DataView NoUsersDisplayView
        {
            get { return nousersdisplayView; }
            set { nousersdisplayView = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        public IAMToolGetUserViewModel()
        {
            IsLoadBool = false;
            GetUsersCommand = new RelayCommand(o => { GetUsersButtonMethod(); });
            ResetFormCommand = new RelayCommand(o => { ResetForm(); });
            CopyUsersColumnCommand = new RelayCommand(o => { CopyUsersClipboard(); });
            CopyNoUsersColumnCommand = new RelayCommand(o => { CopyNoUsersClipboard(); });
            UsersDisplayView = new();
            UsersDisplayTable = new("Temporary Table");
            UsersDisplayTable.Columns.Add("UDGUsername", typeof(String));
            UsersDisplayTable.Columns.Add("UDGUserdomain", typeof(String));
            UsersDisplayTable.Columns.Add("UDGUserzone", typeof(String));
            NoUsersDisplayTable = new("Temporary Table");
            NoUsersDisplayTable.Columns.Add("UDGUsername", typeof(String));
            MSG.Messenger.Default.Register<String>(this, SentMessage);

            foreach (string s in ConfigurationProperties.CfyChildZones) 
            {
                searchTerms.Add(new SearchTerms { SearchTerm = s });
            }
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            try
            {
                UsersDisplayTable = new("Temporary Table");
                UsersDisplayTable.Columns.Add("UDGUsername", typeof(String));
                UsersDisplayTable.Columns.Add("UDGUserdomain", typeof(String));
                UsersDisplayTable.Columns.Add("UDGUserzone", typeof(String));
                NoUsersDisplayTable = new("Temporary Table");
                NoUsersDisplayTable.Columns.Add("UDGUsername", typeof(String));
                IsLoadBool = false;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Clear and Reset Fields
        public void ResetForm()
        {
            UserEntryTextBox = "";
            SelectedTerm = SearchTerms.ElementAt(0);
            UsersDisplayTable.Clear();
            UsersDisplayView = UsersDisplayTable.DefaultView;
            NoUsersDisplayTable.Clear();
            NoUsersDisplayView = UsersDisplayTable.DefaultView;
        }

        // Get Users Method from Button Click
        public async void GetUsersButtonMethod()
        {
            try
            {
                if (UserEntryTextBox != null & UserEntryTextBox != "")
                {
                    ReturnValues returnInstance = new();
                    IsLoadBool = true;
                    UsersDisplayTable = new();
                    UsersDisplayTable.Clear();
                    NoUsersDisplayTable.Clear();
                    UsersDisplayView = UsersDisplayTable.DefaultView;
                    NoUsersDisplayView = NoUsersDisplayTable.DefaultView;
                    await Task.Run(() =>
                    {
                        try
                        {
                            returnInstance = GetUsersInfo();
                        }
                        catch (Exception ex)
                        {
                            ExceptionOutput.Output(ex.ToString());
                            throw;
                        }
                    });
                    UsersDisplayTable = returnInstance.table2;
                    NoUsersDisplayTable = returnInstance.table1;
                    UsersDisplayView = UsersDisplayTable.DefaultView;
                    NoUsersDisplayView = NoUsersDisplayTable.DefaultView;
                    IsLoadBool = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Search Users Function
        public ReturnValues GetUsersInfo()
        {
            ReturnValues returnInstance = new();
            returnInstance.table2 = new("Temporary Table 2");
            returnInstance.table2.Columns.Add("UDGUsername", typeof(String));
            returnInstance.table2.Columns.Add("UDGUserdomain", typeof(String));
            returnInstance.table2.Columns.Add("UDGUserzone", typeof(String));
            returnInstance.table1 = new("Temporary Table");
            returnInstance.table1.Columns.Add("UDGUsername", typeof(String));

            try
            {                string upperNames = UserEntryTextBox.ToUpper();
                string[] inputs = upperNames.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                for (var i = 0; i < inputs.Length; i++)
                {
                    // If there is a / in group string strips out all characters up to and including it.
                    if (inputs[i].ToLower().Contains('\\')) { string str = inputs[i][(inputs[i].IndexOf('\\') + 1)..]; inputs[i] = str; }
                }
                var input = inputs.Distinct();
                var rebuild = string.Join(Environment.NewLine, input);
                UserEntryTextBox = rebuild;

                ICims cims = new Cims();
                IZone zone;

                if (SelectedTerm == SearchTerms.ElementAt(0))
                {
                    zone = cims.GetZone(ConfigurationProperties.CfyRootZone);
                }
                else
                {
                    string selectedZone = SelectedTerm.SearchTerm.ToString();
                    zone = cims.GetZone(ConfigurationProperties.CfyRootZone + "/" + selectedZone);
                }

                foreach (string n in input)
                {
                    var unixuser = zone.GetUserUnixProfileByName(n);
                    if (unixuser == null)
                    {
                        if (n != "")
                        {
                            DataRow NewRow;
                            NewRow = returnInstance.table1.NewRow();
                            NewRow["UDGUsername"] = n;
                            returnInstance.table1.Rows.Add(NewRow);
                        }
                    }
                    else
                    {
                        string username, userdomain, userzone;
                        if (zone.IsHierarchical && !((Centrify.DirectControl.API.CDC50.UserUnixProfile)unixuser).IsNameDefined)
                        {
                            username = "<Empty>";
                        }
                        else
                        {
                            username = unixuser.Name;
                        }
                        String[] adsparse = unixuser.ADsPath.Split(',');
                        String[] domainparse = adsparse[0].Split('@');
                        userdomain = domainparse[1];
                        userzone = unixuser.Zone.ToString();
                        if (username.ToUpper() == n.ToUpper())
                        {
                            DataRow NewRow;
                            NewRow = returnInstance.table2.NewRow();
                            NewRow["UDGUsername"] = username;
                            NewRow["UDGUserdomain"] = userdomain;
                            NewRow["UDGUserzone"] = userzone;
                            returnInstance.table2.Rows.Add(NewRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return returnInstance;
        }

        // Copy Entire Users Column to Clipboard
        public void CopyUsersClipboard()
        {
            var clipboardString = "";
            foreach (DataRow row in UsersDisplayTable.Rows)
            {
                clipboardString += row[0].ToString() + Environment.NewLine;
            }
            Clipboard.SetText(clipboardString);
        }

        // Copy Entire No Users Column to Clipboard
        public void CopyNoUsersClipboard()
        {
            var clipboardString = "";
            foreach (DataRow row in NoUsersDisplayTable.Rows)
            {
                clipboardString += row[0].ToString() + Environment.NewLine;
            }
            Clipboard.SetText(clipboardString);
        }

        public struct ReturnValues
        {
            public DataTable table1;
            public DataTable table2;
        }
        #endregion

        #region Messages
        // Listen to New Record for Database Reload
        public void SentMessage(string passedMessage)
        {
            if (passedMessage != null)
            {
                if (passedMessage == "IAM Tool User Query Selected")
                {
                    UserEntryTextBox = IAMToolGroupQueryViewModel.SelectedUserQuery;
                }
            }
        }
        #endregion
    }
}

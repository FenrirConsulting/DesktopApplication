using IAMHeimdall.Core;
using IAMHeimdall.MVVM.IAMToolModule.Model;
using IAMHeimdall.MVVM.Model;
using IAMHeimdall.Resources.Commands;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolAddGroupViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand AddGroupsCommand { get; set; }
        public RelayCommand ClearFormCommand { get; set; }
        public RelayCommand DataGridChangedCommand { get; set; }
        #endregion

        #region Properties
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

        // Holds Data for Display Table
        private DataTable objectsDisplayTable;
        public DataTable ObjectsDisplayTable
        {
            get { return objectsDisplayTable; }
            set { objectsDisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView objectsDisplayView;
        public DataView ObjectsDisplayView
        {
            get { return objectsDisplayView; }
            set { 
                objectsDisplayView = value;
                OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<AddGroupObjectTypeModel> objectTypeTerms = new()
        {
            new AddGroupObjectTypeModel { ID = 1, ObjType = "USER" },
            new AddGroupObjectTypeModel { ID = 2, ObjType = "GROUP" },
            new AddGroupObjectTypeModel { ID = 3, ObjType = "COMPUTER" }
        };

        // Basis for Search Terms Collection
        public IEnumerable<AddGroupObjectTypeModel> ObjectTypeTerms
        {
            get { return objectTypeTerms; }
        }

        // Selected Term for ComboBox
        private AddGroupObjectTypeModel selectedObjectTypeTerm = new();
        public AddGroupObjectTypeModel SelectedObjectTypeTerm
        {
            get { return selectedObjectTypeTerm; }
            set { selectedObjectTypeTerm = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<AddGroupGroupDomainModel> groupDomainTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<AddGroupGroupDomainModel> GroupDomainTerms
        {
            get { return groupDomainTerms; }
        }

        // Selected Term for ComboBox
        private AddGroupGroupDomainModel selectedGroupDomainTerm = new();
        public AddGroupGroupDomainModel SelectedGroupDomainTerm
        {
            get { return selectedGroupDomainTerm; }
            set { selectedGroupDomainTerm = value; OnPropertyChanged(); }
        }

        // Search Terms for ComboBox
        private readonly ObservableCollection<AddGroupObjectDomainModel> objectDomainTerms = new()
        {

        };

        // Basis for Search Terms Collection
        public IEnumerable<AddGroupObjectDomainModel> ObjectDomainTerms
        {
            get { return objectDomainTerms; }
        }

        // Selected Term for ComboBox
        private AddGroupObjectDomainModel selectedObjectDomainTerm = new();
        public AddGroupObjectDomainModel SelectedObjectDomainTerm
        {
            get { return selectedObjectDomainTerm; }
            set { selectedObjectDomainTerm = value; OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable groupsDisplayTable;
        public DataTable GroupsDisplayTable
        {
            get { return groupsDisplayTable; }
            set { groupsDisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView groupsDisplayView;
        public DataView GroupsDisplayView
        {
            get { return groupsDisplayView; }
            set { 
                groupsDisplayView = value;
                OnPropertyChanged(); }
        }

        // Holds Data for Display Table
        private DataTable resultsDisplayTable;
        public DataTable ResultsDisplayTable
        {
            get { return resultsDisplayTable; }
            set { resultsDisplayTable = value; OnPropertyChanged(); }
        }

        // View Used to display DisplayTable
        private DataView resultsDisplayView;
        public DataView ResultsDisplayView
        {
            get { return resultsDisplayView; }
            set { resultsDisplayView = value; OnPropertyChanged(); }
        }

        // Binds Data Buttons Enabled Bool
        private bool addGroupsEnabled;
        public bool AddGroupsEnabled
        {
            get { return addGroupsEnabled; }
            set { if (addGroupsEnabled != value) { addGroupsEnabled = value; OnPropertyChanged(); } }
        }

        // Bool to starting Loading Animation
        private bool isLoadBool;
        public bool IsLoadBool
        {
            get { return isLoadBool; }
            set { if (isLoadBool != value) { isLoadBool = value; OnPropertyChanged(); } }
        }
        #endregion

        #region Methods
        public IAMToolAddGroupViewModel()
        {
            // Initialize Properties
            IsLoadBool = false;
            ObjectsDisplayTable = new("Objects Table");
            ObjectsDisplayTable.Columns.Add("ObjectName", typeof(String));
            ObjectsDisplayTable.Columns.Add("OType", typeof(String));
            ObjectsDisplayTable.Columns.Add("ODomain", typeof(String));
            GroupsDisplayTable = new("Groups Table");
            GroupsDisplayTable.Columns.Add("GName", typeof(String));
            GroupsDisplayTable.Columns.Add("GDomain", typeof(String));
            ResultsDisplayTable = new("Results Table");
            ResultsDisplayTable.Columns.Add("RObject", typeof(String));
            ResultsDisplayTable.Columns.Add("RTarget", typeof(String));
            ResultsDisplayTable.Columns.Add("RResult", typeof(String));
            AddGroupsEnabled = false;
            ObjectsDisplayView = ObjectsDisplayTable.DefaultView;
            GroupsDisplayView = GroupsDisplayTable.DefaultView;

            // Loads AD Domain List into Object and Group Models
            int localID = 0;
            foreach (string s in ConfigurationProperties.ADDomains)
            {
                localID++;
                objectDomainTerms.Add(new AddGroupObjectDomainModel { ID = localID, ObjDomain = s });
                groupDomainTerms.Add(new AddGroupGroupDomainModel { ID = localID, GrpDomain = s });
            }

            // Initialize Relay Commands
            AddGroupsCommand = new RelayCommand(o => { AddGroupsButton(); });
            ClearFormCommand = new RelayCommand(o => { ClearFormButton(); });
            DataGridChangedCommand = new RelayCommand(o => { EnableAddGroupCheck(); });

        }
        #endregion

        #region Functions
        // On Load Function
        public  void LoadData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        // Add Groups Button Method
        public async void AddGroupsButton()
        {
            try
            {
                DataTable tempTable = new();
                ResultsDisplayTable.Clear();
                ResultsDisplayView = ResultsDisplayTable.DefaultView;
                IsLoadBool = true;
                StatusString = "Working...";
                await Task.Run(() => {
                    try
                    {
                        Thread.CurrentPrincipal = new System.Security.Principal.WindowsPrincipal(WindowsIdentity.GetCurrent());
                        tempTable = AddObjectToGroup();
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                        throw;
                    }
                });
                IsLoadBool = false;
                ResultsDisplayTable = tempTable;
                if (ResultsDisplayTable.Rows.Count > 0)
                {
                    StatusString = "";
                }
                else
                {
                    StatusString = "No Records Updated.";
                }
                ResultsDisplayView = ResultsDisplayTable.DefaultView;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Clear Forms Button Method
        public void ClearFormButton()
        { 
            ObjectsDisplayTable.Clear();
            GroupsDisplayTable.Clear();
            ResultsDisplayTable.Clear();
            ObjectsDisplayView = ObjectsDisplayTable.DefaultView;
            GroupsDisplayView = GroupsDisplayTable.DefaultView;
            ResultsDisplayView = ResultsDisplayTable.DefaultView;
        }

        // On DataGrid Changes if Group and Object Tables are Greater than 0 Enables the Add Group Button
        public void EnableAddGroupCheck()
        {
            if (ObjectsDisplayView.Count > 0 && GroupsDisplayView.Count > 0)
            {
                AddGroupsEnabled = true;
            }
            else
            {
                AddGroupsEnabled = false;
            }
        }

        // Main Add to Group Function
        public DataTable AddObjectToGroup()
        {
            DataTable tempTable = new("Temporary Table");
            tempTable.Columns.Add("RObject", typeof(String));
            tempTable.Columns.Add("RTarget", typeof(String));
            tempTable.Columns.Add("RResult", typeof(String));
            try
            {
                DataTable tempObjTable = new();
                DataTable tempGrpTable = new();
                tempObjTable = ObjectsDisplayView.ToTable();
                tempGrpTable = GroupsDisplayView.ToTable();
                string objectSid = null;
                string objectDN = null;
                string groupDN = null;

                foreach (DataRow row in tempObjTable.Rows)
                {
                    string objectName;
                    string objectType;
                    string objectDomain;
                    string objectSAM;
                    string adObject;
                    string result;

                    if (row["ObjectName"].ToString() != null)
                    {
                        List<string> colList = new();
                        foreach (DataColumn column in tempObjTable.Columns)
                        {
                            colList.Add(column.ColumnName);
                        }
                        var finalList = string.Join(Environment.NewLine, colList);

                        objectName = row["ObjectName"].ToString().ToUpper();
                        objectType = row["OType"].ToString().ToUpper();
                        objectDomain = row["ODomain"].ToString().ToUpper();
                        adObject = objectDomain + "\\" + objectName;
                        objectDN = GetADObject(objectName, objectDomain, objectType, "DistinguishedName");
                        objectSAM = GetADObject(objectName, objectDomain, objectType, "SamAccountName");
                        objectSid = GetADObject(objectName, objectDomain, objectType, "Sid");
                        objectSid = string.Format("<SID={0}>", objectSid);

                        if (objectDN == null)
                        {
                            result = row["OType"].ToString() + " Object not Found";
                            DataRow NewRow;
                            NewRow = tempTable.NewRow();
                            NewRow["RObject"] = adObject;
                            NewRow["RTarget"] = "";
                            NewRow["RResult"] = result;
                            tempTable.Rows.Add(NewRow);
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }

                    foreach (DataRow grow in tempGrpTable.Rows)
                    {
                        String groupName;
                        String groupDomain;
                        String group;
                        String groupScope;
                        String adGroup;

                        if (grow["GName"].ToString() != null)
                        {
                            groupName = grow["GName"].ToString().ToUpper();
                            groupDomain = grow["GDomain"].ToString().ToUpper();
                            adGroup = groupDomain + "\\" + groupName;
                            groupDN = GetADObject(groupName, groupDomain, "GROUP", "DistinguishedName");
                            groupScope = GetADObject(groupName, groupDomain, "GROUP", "GroupScope");

                            if (groupDN == null)
                            {
                                result = "Group not Found";
                                DataRow NewRow;
                                NewRow = tempTable.NewRow();
                                NewRow["RObject"] = adObject;
                                NewRow["RTarget"] = adGroup;
                                NewRow["RResult"] = result;
                                tempTable.Rows.Add(NewRow);
                                break;
                            }

                            if (groupScope == "Global" && (objectDomain != groupDomain))
                            {
                                result = "Can't Add Foreign Object Principal to Global Group";
                                DataRow NewRow;
                                NewRow = tempTable.NewRow();
                                NewRow["RObject"] = adObject;
                                NewRow["RTarget"] = adGroup;
                                NewRow["RResult"] = result;
                                tempTable.Rows.Add(NewRow);
                                break;
                            }

                            if (IsGroupMember(objectName, groupName, groupDomain) == true)
                            {
                                result = "Object already in Group";
                                DataRow NewRow;
                                NewRow = tempTable.NewRow();
                                NewRow["RObject"] = adObject;
                                NewRow["RTarget"] = adGroup;
                                NewRow["RResult"] = result;
                                tempTable.Rows.Add(NewRow);
                                break;
                            }

                            group = groupDomain + "\\" + groupName;

                            if (AddMemberToGroup(objectSid, objectDomain, groupDN) == true)
                            {
                                result = "Add " + objectType + " Success";
                                DataRow NewRow;
                                NewRow = tempTable.NewRow();
                                NewRow["RObject"] = adObject;
                                NewRow["RTarget"] = group;
                                NewRow["RResult"] = result;
                                tempTable.Rows.Add(NewRow);
                            }
                            else
                            {
                                result = "Add " + objectType + " Failed - Check Application Logfile for details";
                                DataRow NewRow;
                                NewRow = tempTable.NewRow();
                                NewRow["RObject"] = adObject;
                                NewRow["RTarget"] = group;
                                NewRow["RResult"] = result;
                                tempTable.Rows.Add(NewRow);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return tempTable;
        }

        // Checks to see if Object exists in specified Object domain, returns principal object or null
        private static string GetADObject(string sam, string domain, string objecttype, string property)
        {
            string result = null;
            try 
            {
                switch (objecttype)
                {
                    case "USER":
                        using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
                        {
                            using var user = new UserPrincipal(domainContext);
                            user.SamAccountName = sam;
                            using var ps = new PrincipalSearcher();
                            ps.QueryFilter = user;
                            using PrincipalSearchResult<Principal> results = ps.FindAll();
                            if (results != null && results.Any())
                            {
                                foreach (UserPrincipal u in results)
                                {
                                    switch (property)
                                    {
                                        case "Sid":
                                            result = u.Sid.ToString();
                                            break;

                                        case "SamAccountName":
                                            result = u.SamAccountName;
                                            break;

                                        case "DistinguishedName":
                                            result = u.DistinguishedName;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }

                        break;

                    case "GROUP":

                        using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
                        {
                            using var group = new GroupPrincipal(domainContext);
                            group.SamAccountName = sam;
                            using var ps = new PrincipalSearcher();
                            ps.QueryFilter = group;
                            using PrincipalSearchResult<Principal> results = ps.FindAll();
                            if (results != null && results.Any())
                            {
                                foreach (GroupPrincipal g in results)
                                {
                                    switch (property)
                                    {
                                        case "Sid":
                                            result = g.Sid.ToString();
                                            break;

                                        case "SamAccountName":
                                            result = g.SamAccountName;
                                            break;

                                        case "DistinguishedName":
                                            result = g.DistinguishedName;
                                            break;

                                        case "GroupScope":
                                            result = g.GroupScope.ToString();
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                result = null;
                            }
                        }

                        break;

                    case "COMPUTER":

                        using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
                        {
                            using var computer = new ComputerPrincipal(domainContext);
                            computer.SamAccountName = sam + "$";
                            using var ps = new PrincipalSearcher();
                            ps.QueryFilter = computer;
                            using PrincipalSearchResult<Principal> results = ps.FindAll();
                            if (results != null && results.Any())
                            {
                                foreach (ComputerPrincipal c in results)
                                {
                                    switch (property)
                                    {
                                        case "Sid":
                                            result = c.Sid.ToString();
                                            break;

                                        case "SamAccountName":
                                            result = c.SamAccountName + "$";
                                            break;

                                        case "DistinguishedName":
                                            result = c.DistinguishedName;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                result = null;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            catch 
            {
                //ExceptionOutput.Output(ex.ToString());
            }
            return result;
        }

        // Check if Member is part of a Group before adding
        private static bool IsGroupMember(string sam, string groupname, string domain)
        {
            Boolean isMember = false;
            try 
            {
                using var domainContext = new PrincipalContext(ContextType.Domain, domain);
                GroupPrincipal group = GroupPrincipal.FindByIdentity(domainContext, groupname);
                if (group != null)
                {
                    // iterate over members
                    foreach (Principal p in group.GetMembers())
                    {
                        if (p.SamAccountName == sam)
                        {
                            isMember = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            return isMember;
        }

        // Add Object princiapl to Group Members
        private static Boolean AddMemberToGroup(String objectSid, String objectDomain, String groupDN)
        {
            try
            {
                DirectoryEntry dirEntry = new("LDAP://" + groupDN);
                dirEntry.Properties["member"].Add(objectSid);
                dirEntry.CommitChanges();
                dirEntry.Close();
                objectDomain += objectDomain;
                return true;
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                return false;
            }
        }
        #endregion
    }
}

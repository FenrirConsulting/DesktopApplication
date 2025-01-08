using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ADLookupMainViewModel : BaseViewModel
    {
        #region Delegates

        #endregion

        #region Properties
        // Get Employee Number Lookup View Tab
        private ADLookupEmpNumberViewModel employeeNumberLookupVm;
        public ADLookupEmpNumberViewModel EmployeeNumberLookupVm
        {
            get { return employeeNumberLookupVm; }
            set { employeeNumberLookupVm = value; OnPropertyChanged(); }
        }

        // Get Email Status Lookup View Tab
        private ADLookupEmailStatusViewModel employeEmailLookupVm;
        public ADLookupEmailStatusViewModel EmployeEmailLookupVm
        {
            get { return employeEmailLookupVm; }
            set { employeEmailLookupVm = value; OnPropertyChanged(); }
        }

        // Get Folder Permissions View Tab
        private ADLookupGetFolderViewModel employeFolderLookupVm;
        public ADLookupGetFolderViewModel EmployeeFolderLookupVm
        {
            get { return employeFolderLookupVm; }
            set { employeFolderLookupVm = value; OnPropertyChanged(); }
        }

        // Get Groups Lookup View Tab
        private ADLookupGroupsLookupViewModel groupsLookupVm;
        public ADLookupGroupsLookupViewModel GroupsLookupVm
        {
            get { return groupsLookupVm; }
            set { groupsLookupVm = value; OnPropertyChanged(); }
        }

        // Get Group Members View Tab
        private ADLookupGroupMembersViewModel groupMembersVm;
        public ADLookupGroupMembersViewModel GroupMembersVm
        {
            get { return groupMembersVm; }
            set { groupMembersVm = value; OnPropertyChanged(); }
        }

        // Get Hierarchy Search View Tab
        private ADLookupHierarchySearchViewModel hierarchySearchVm;
        public ADLookupHierarchySearchViewModel HierarchySearchVm
        {
            get { return hierarchySearchVm; }
            set { hierarchySearchVm = value; OnPropertyChanged(); }
        }

        // Get AD Export View Tab
        private ADLookupADExportViewModel aDExportVm;
        public ADLookupADExportViewModel ADExportVm
        {
            get { return aDExportVm; }
            set { aDExportVm = value; OnPropertyChanged(); }
        }

        public static string EMP_EmpNumberToQuery;
        public static string HS_UserDNSelectedForHierarchy;
        public static string HS_NameSelectedForHierarchy;
        public static string HS_ManagerSelectedForHierarchy;
        public static string UGL_CurrentDomain;
        public static string UGL_DomainString;
        public static string UGL_CurrentUsername;
        public static string UGL_CurrentName;
        public static string UGL_CurrentEmpNum;
        public static string MOG_CurrentDomain;
        public static string MOG_CurrentGroupQuery;
        public static string MOG_GroupDNSelectedForMembership;
        public static string MOG_GroupNameSelectedForMembership;

        public static DomainDetailModel MainModel = new();
        public static SearchResult Result;
        public static DirectorySearcher Searcher;
        public static DirectoryEntry RootEntry;
        

        #endregion

        #region Methods

        public ADLookupMainViewModel()
        {
            EmployeeNumberLookupVm = new();
            EmployeEmailLookupVm = new();
            EmployeeFolderLookupVm = new();
            GroupsLookupVm = new();
            GroupMembersVm = new();
            HierarchySearchVm = new();
            ADExportVm = new();
            MainModel = new();
        }
        #endregion

        #region Functions
        // Primary On Load Function
        public void LoadData()
        {

        }
        #endregion

    }
}

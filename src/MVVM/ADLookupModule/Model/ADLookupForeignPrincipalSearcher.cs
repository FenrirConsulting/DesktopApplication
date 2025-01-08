using IAMHeimdall.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;

namespace IAMHeimdall.MVVM.Model
{
    public class ADLookupForeignPrincipalSearcher : ObservableObject
    {
        #region Properties
        private string eMP_EmpNumberToQuery;
        public string EMP_EmpNumberToQuery
        {
            get
            {
                return eMP_EmpNumberToQuery;
            }
            set
            {
                if (eMP_EmpNumberToQuery != value)
                {
                    eMP_EmpNumberToQuery = value;
                    OnPropertyChanged();
                }
            }
        }

        private string hS_UserDNSelectedForHierarchy;
        public string HS_UserDNSelectedForHierarchy
        {
            get
            {
                return hS_UserDNSelectedForHierarchy;
            }
            set
            {
                if (hS_UserDNSelectedForHierarchy != value)
                {
                    hS_UserDNSelectedForHierarchy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string hS_NameSelectedForHierarchy;
        public string HS_NameSelectedForHierarchy
        {
            get
            {
                return hS_NameSelectedForHierarchy;
            }
            set
            {
                if (hS_NameSelectedForHierarchy != value)
                {
                    hS_NameSelectedForHierarchy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string hS_ManagerSelectedForHierarchy;
        public string HS_ManagerSelectedForHierarchy
        {
            get
            {
                return hS_ManagerSelectedForHierarchy;
            }
            set
            {
                if (hS_ManagerSelectedForHierarchy != value)
                {
                    hS_ManagerSelectedForHierarchy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string uGL_CurrentDomain;
        public string UGL_CurrentDomain
        {
            get
            {
                return uGL_CurrentDomain;
            }
            set
            {
                if (uGL_CurrentDomain != value)
                {
                    uGL_CurrentDomain = value;
                    OnPropertyChanged();
                }
            }
        }

        private string uGL_DomainString;
        public string UGL_DomainString
        {
            get
            {
                return uGL_DomainString;
            }
            set
            {
                if (uGL_DomainString != value)
                {
                    uGL_DomainString = value;
                    OnPropertyChanged();
                }
            }
        }

        private string uGL_CurrentUsername;
        public string UGL_CurrentUsername
        {
            get
            {
                return uGL_CurrentUsername;
            }
            set
            {
                if (uGL_CurrentUsername != value)
                {
                    uGL_CurrentUsername = value;
                    OnPropertyChanged();
                }
            }
        }

        private string uGL_CurrentName;
        public string UGL_CurrentName
        {
            get
            {
                return uGL_CurrentName;
            }
            set
            {
                if (uGL_CurrentName != value)
                {
                    uGL_CurrentName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string uGL_CurrentEmpNum;
        public string UGL_CurrentEmpNum
        {
            get
            {
                return uGL_CurrentEmpNum;
            }
            set
            {
                if (uGL_CurrentEmpNum != value)
                {
                    uGL_CurrentEmpNum = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mOG_CurrentDomain;
        public string MOG_CurrentDomain
        {
            get
            {
                return mOG_CurrentDomain;
            }
            set
            {
                if (mOG_CurrentDomain != value)
                {
                    mOG_CurrentDomain = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mOG_CurrentGroupQuery;
        public string MOG_CurrentGroupQuery
        {
            get
            {
                return mOG_CurrentGroupQuery;
            }
            set
            {
                if (mOG_CurrentGroupQuery != value)
                {
                    mOG_CurrentGroupQuery = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mOG_GroupDNSelectedForMembership;
        public string MOG_GroupDNSelectedForMembership
        {
            get
            {
                return mOG_GroupDNSelectedForMembership;
            }
            set
            {
                if (mOG_GroupDNSelectedForMembership != value)
                {
                    mOG_GroupDNSelectedForMembership = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mOG_GroupNameSelectedForMembership;
        public string MOG_GroupNameSelectedForMembership
        {
            get
            {
                return mOG_GroupNameSelectedForMembership;
            }
            set
            {
                if (mOG_GroupNameSelectedForMembership != value)
                {
                    mOG_GroupNameSelectedForMembership = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Object Collection ofDomain Detail
        private static readonly ObservableCollection<DomainDetailModel> domainList = new()
        {
            new DomainDetailModel
            {
                DomainFullName = "CORP.Company.COM",
                DomainShortName = "CORP",
                DomainSuffix = "DC=corp,DC=Company,DC=com",
                DomainObjectSidPrefix = "S-1-5-21-2702892183-608350096-274610827*",
                DomainNetBiosName = "CORP"
            },

            new DomainDetailModel
            {
                DomainFullName = "CORP.Company.COM",
                DomainShortName = "Company",
                DomainSuffix = "DC=Corp,DC=Company,DC=com",
                DomainObjectSidPrefix = "S-1-5-21-2130522478-1725188094-786498627*",
                DomainNetBiosName = "Company"
            },

            new DomainDetailModel
            {
                DomainFullName = "MINCLINIC.LOCAL",
                DomainShortName = "Minclinic",
                DomainSuffix = "DC=minclinic,DC=local",
                DomainObjectSidPrefix = "S-1-5-21-1808345313-2269461769-1081395210*",
                DomainNetBiosName = "MINCLINIC"
            },

            new DomainDetailModel
            {
                DomainFullName = "RX.NET",
                DomainShortName = "RX",
                DomainSuffix = "DC=rx,DC=net",
                DomainObjectSidPrefix = "S-1-5-21-4258776639-1271169360-244890835*",
                DomainNetBiosName = "RX"
            },

            new DomainDetailModel
            {
                DomainFullName = "PROCARE.PCARE.LTD",
                DomainShortName = "Procare",
                DomainSuffix = "DC=procare,DC=pcare,DC=ltd",
                DomainObjectSidPrefix = "S-1-5-21-1715567821-2111687655-682003330*",
                DomainNetBiosName = "PROCARE"
            },

            new DomainDetailModel
            {
                DomainFullName = "PHARMACARE.PCARE.LTD",
                DomainShortName = "Pharmacare",
                DomainSuffix = "DC=pharmacare,DC=pcare,DC=ltd",
                DomainObjectSidPrefix = "S-1-5-21-1139409664-453791681-928725530*",
                DomainNetBiosName = "PHARMACARE"
            },

            new DomainDetailModel
            {
                DomainFullName = "RIDC.PCARE.LTD",
                DomainShortName = "RIDC",
                DomainSuffix = "DC=ridc,DC=pcare,DC=ltd",
                DomainObjectSidPrefix = "S-1-5-21-238693470-80480483-1541874228*",
                DomainNetBiosName = "PITTSBURGH-EPS"
            },

            new DomainDetailModel
            {
                DomainFullName = "CORPORATE.MHRX.LOCAL",
                DomainShortName = "MHRX",
                DomainSuffix = "DC=corporate,DC=mhrx,DC=local",
                DomainObjectSidPrefix = "S-1-5-21-2396041630-3241229156-1003068976*",
                DomainNetBiosName = "MHRX"
            },

            new DomainDetailModel
            {
                DomainFullName = "COE.RX.NET",
                DomainShortName = "COE",
                DomainSuffix = "DC=coe,DC=rx,DC=net",
                DomainObjectSidPrefix = "S-1-5-21-2480620504-3585254612-2290200040*",
                DomainNetBiosName = "COE"
            },

            new DomainDetailModel
            {
                DomainFullName = "CORP.OMNICARE.COM",
                DomainShortName = "CORP (OMNI)",
                DomainSuffix = "DC=corp,DC=omnicare,DC=com",
                DomainObjectSidPrefix = "S-1-5-21-2326735829-1461535259-1004279218*",
                DomainNetBiosName = "CORP.OMNICARE.COM"
            },

            new DomainDetailModel
            {
                DomainFullName = "PCARE.LTD",
                DomainShortName = "PCare",
                DomainSuffix = "DC=pcare,DC=ltd",
                DomainObjectSidPrefix = "S-1-5-21-606747145-1303643608-682003330*",
                DomainNetBiosName = "PCARE"
            }
        };
        public static IEnumerable<DomainDetailModel> DomainList
        {
            get { return domainList; }
        }

        // Selected Term for Domain Detail List
        private DomainDetailModel selectedDomainList = new();
        public DomainDetailModel SelectedDomainList
        {
            get { return selectedDomainList; }
            set
            {
                selectedDomainList = value;
                OnPropertyChanged();
            }
        }


        public static DomainDetailModel MainModel = new();
        public static SearchResult Result;
        public static DirectorySearcher Searcher;
        public static DirectoryEntry RootEntry;
        public int DomainCOunter = 0;
        #endregion

        #region Functions

        public void PrincipalSearcher()
        {

        }

        #endregion
    }
}

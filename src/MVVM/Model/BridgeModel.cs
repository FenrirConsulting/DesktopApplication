using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class BridgeModel : ObservableObject
    {
        // List of AD group strings from ConfigurationProperties for IAM authorization

        #region Properties
        private List<string> factoolCompletionRole;
        public List<string> FactoolCompletionRole
        {
            get
            {
                return factoolCompletionRole;
            }
            set
            {
                if (factoolCompletionRole != value)
                {
                    factoolCompletionRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> facToolMinRole;
        public List<string> FacToolMinRole
        {
            get
            {
                return facToolMinRole;
            }
            set
            {
                if (facToolMinRole != value)
                {
                    facToolMinRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> iAMGroups;
        public List<string> IAMGroups
        {
            get
            {
                return iAMGroups;
            }
            set
            {
                if (iAMGroups != value)
                {
                    iAMGroups = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> cfyProvisioningRole;
        public List<string> CfyProvisioningRole
        {
            get
            {
                return cfyProvisioningRole;
            }
            set
            {
                if (cfyProvisioningRole != value)
                {
                    cfyProvisioningRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mgrRole;
        public string MgrRole
        {
            get
            {
                return mgrRole;
            }
            set
            {
                if (mgrRole != value)
                {
                    mgrRole = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<string> factoolUserRole;
        public List<string> FactoolUserRole
        {
            get
            {
                return factoolUserRole;
            }
            set
            {
                if (factoolUserRole != value)
                {
                    factoolUserRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> facToolMgrRole;
        public List<string> FacToolMgrRole
        {
            get
            {
                return facToolMgrRole;
            }
            set
            {
                if (facToolMgrRole != value)
                {
                    facToolMgrRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> facToolAdminRole;
        public List<string> FacToolAdminRole
        {
            get
            {
                return facToolAdminRole;
            }
            set
            {
                if (facToolAdminRole != value)
                {
                    facToolAdminRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> accessNowUserRole;
        public List<string> AccessNowUserRole
        {
            get
            {
                return accessNowUserRole;
            }
            set
            {
                if (accessNowUserRole != value)
                {
                    accessNowUserRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> accessNowMgrRole;
        public List<string> AccessNowMgrRole
        {
            get
            {
                return accessNowMgrRole;
            }
            set
            {
                if (accessNowMgrRole != value)
                {
                    accessNowMgrRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> accessNowUserRoleList;
        public List<string> AccessNowUserRoleList
        {
            get
            {
                return accessNowUserRoleList;
            }
            set
            {
                if (accessNowUserRoleList != value)
                {
                    accessNowUserRoleList = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> serviceCatalogAdminRole;
        public List<string> ServiceCatalogAdminRole
        {
            get
            {
                return serviceCatalogAdminRole;
            }
            set
            {
                if (serviceCatalogAdminRole != value)
                {
                    serviceCatalogAdminRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private string unixRole;
        public string UnixRole
        {
            get
            {
                return unixRole;
            }
            set
            {
                if (unixRole != value)
                {
                    unixRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> cfyAdminRole;
        public List<string> CfyAdminRole
        {
            get
            {
                return cfyAdminRole;
            }
            set
            {
                if (cfyAdminRole != value)
                {
                    cfyAdminRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> serviceCatalogUserRoleList;
        public List<string> ServiceCatalogUserRoleList
        {
            get
            {
                return serviceCatalogUserRoleList;
            }
            set
            {
                if (serviceCatalogUserRoleList != value)
                {
                    serviceCatalogUserRoleList = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> serviceCatalogUserRole;
        public List<string> ServiceCatalogUserRole
        {
            get
            {
                return serviceCatalogUserRole;
            }
            set
            {
                if (serviceCatalogUserRole != value)
                {
                    serviceCatalogUserRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> serviceCatalogMgrRole;
        public List<string> ServiceCatalogMgrRole
        {
            get
            {
                return serviceCatalogMgrRole;
            }
            set
            {
                if (serviceCatalogMgrRole != value)
                {
                    serviceCatalogMgrRole = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> devList;
        public List<string> DevList
        {
            get
            {
                return devList;
            }
            set
            {
                if (devList != value)
                {
                    devList = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> accessNowClosingRole;
        public List<string> AccessNowClosingRole
        {
            get
            {
                return accessNowClosingRole;
            }
            set
            {
                if (accessNowClosingRole != value)
                {
                    accessNowClosingRole = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

    }
}

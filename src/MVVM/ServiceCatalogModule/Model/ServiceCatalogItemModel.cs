using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class ServiceCatalogItemModel : ObservableObject
    {
        #region Properties
        private int iD;
        public int ID
        {
            get
            {
                return iD;
            }
            set
            {
                if (iD != value)
                {
                    iD = value;
                    OnPropertyChanged();
                }
            }
        }

        private string services;
        public string Services
        {
            get
            {
                return services;
            }
            set
            {
                if (services != value)
                {
                    services = value;
                    OnPropertyChanged();
                }
            }
        }

        private string subSystem;
        public string SubSystem
        {
            get
            {
                return subSystem;
            }
            set
            {
                if (subSystem != value)
                {
                    subSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestSource;
        public string RequestSource
        {
            get
            {
                return requestSource;
            }
            set
            {
                if (requestSource != value)
                {
                    requestSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private string assignmentGroup;
        public string AssignmentGroup
        {
            get
            {
                return assignmentGroup;
            }
            set
            {
                if (assignmentGroup != value)
                {
                    assignmentGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        private string expediteProcess;
        public string ExpediteProcess
        {
            get
            {
                return expediteProcess;
            }
            set
            {
                if (expediteProcess != value)
                {
                    expediteProcess = value;
                    OnPropertyChanged();
                }
            }
        }

        private string sla;
        public string SLA
        {
            get
            {
                return sla;
            }
            set
            {
                if (sla != value)
                {
                    sla = value;
                    OnPropertyChanged();
                }
            }
        }

        private string archerAppID;
        public string ArcherAppID
        {
            get
            {
                return archerAppID;
            }
            set
            {
                if (archerAppID != value)
                {
                    archerAppID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string iTPMAppID;
        public string ITPMAppID
        {
            get
            {
                return iTPMAppID;
            }
            set
            {
                if (iTPMAppID != value)
                {
                    iTPMAppID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string hints;
        public string Hints
        {
            get
            {
                return hints;
            }
            set
            {
                if (hints != value)
                {
                    hints = value;
                    OnPropertyChanged();
                }
            }
        }

        private string tasks;
        public string Tasks
        {
            get
            {
                return tasks;
            }
            set
            {
                if (tasks != value)
                {
                    tasks = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> domain;
        public List<string> Domain
        {
            get
            {
                return domain;
            }
            set
            {
                if (domain != value)
                {
                    domain = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> assignedDomains;
        public List<string> AssignedDomains
        {
            get
            {
                return assignedDomains;
            }
            set
            {
                if (assignedDomains != value)
                {
                    assignedDomains = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> availableDomains;
        public List<string> AvailableDomains
        {
            get
            {
                return availableDomains;
            }
            set
            {
                if (availableDomains != value)
                {
                    availableDomains = value;
                    OnPropertyChanged();
                }
            }
        }

        private string article;
        public string Article
        {
            get
            {
                return article;
            }
            set
            {
                if (article != value)
                {
                    article = value;
                    OnPropertyChanged();
                }
            }
        }

        private string provisioningTeam;
        public string ProvisioningTeam
        {
            get
            {
                return provisioningTeam;
            }
            set
            {
                if (provisioningTeam != value)
                {
                    provisioningTeam = value;
                    OnPropertyChanged();
                }
            }
        }

        private string url;
        public string URL
        {
            get
            {
                return url;
            }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged();
                }
            }
        }

        private string automationStatus;
        public string AutomationStatus
        {
            get
            {
                return automationStatus;
            }
            set
            {
                if (automationStatus != value)
                {
                    automationStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string environment;
        public string Environment
        {
            get
            {
                return environment;
            }
            set
            {
                if (environment != value)
                {
                    environment = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

    }
}

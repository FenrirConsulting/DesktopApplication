using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class DomainDetailModel : ObservableObject
    {
        #region Properties
        private string domainFullName;
        public string DomainFullName
        {
            get
            {
                return domainFullName;
            }
            set
            {
                if (domainFullName != value)
                {
                    domainFullName = value;
                    OnPropertyChanged();
                }
            }
        }


        private string domainSuffix;
        public string DomainSuffix
        {
            get
            {
                return domainSuffix;
            }
            set
            {
                if (domainSuffix != value)
                {
                    domainSuffix = value;
                    OnPropertyChanged();
                }
            }
        }

        private string domainShortName;
        public string DomainShortName
        {
            get
            {
                return domainShortName;
            }
            set
            {
                if (domainShortName != value)
                {
                    domainShortName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string domainObjectSidPrefix;
        public string DomainObjectSidPrefix
        {
            get
            {
                return domainObjectSidPrefix;
            }
            set
            {
                if (domainObjectSidPrefix != value)
                {
                    domainObjectSidPrefix = value;
                    OnPropertyChanged();
                }
            }
        }

        private string domainNetBiosName;
        public string DomainNetBiosName
        {
            get
            {
                return domainNetBiosName;
            }
            set
            {
                if (domainNetBiosName != value)
                {
                    domainNetBiosName = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

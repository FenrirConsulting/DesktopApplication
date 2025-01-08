using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class ServiceCatalogDomainTermModel : ObservableObject
    {
        #region Properties
        private string domainTerm;
        public string DomainTerm
        {
            get
            {
                return domainTerm;
            }
            set
            {
                if (domainTerm != value)
                {
                    domainTerm = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

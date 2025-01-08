using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class ServiceCatalogEnvironmentItemModel : ObservableObject
    {
        #region Properties
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestEnvironment;
        public string RequestEnvironment
        {
            get
            {
                return requestEnvironment;
            }
            set
            {
                if (requestEnvironment != value)
                {
                    requestEnvironment = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class ServiceCatalogSourceItemModel : ObservableObject
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
        #endregion
    }
}

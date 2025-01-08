using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class AccessNowHistoryItemModel : ObservableObject
    {
        #region Properties
        private int historyID;
        public int HistoryID
        {
            get
            {
                return historyID;
            }
            set
            {
                if (historyID != value)
                {
                    historyID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string modifiedBy;
        public string ModifiedBy
        {
            get
            {
                return modifiedBy;
            }
            set
            {
                if (modifiedBy != value)
                {
                    modifiedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string modifiedDate;
        public string ModifiedDate
        {
            get
            {
                return modifiedDate;
            }
            set
            {
                if (modifiedDate != value)
                {
                    modifiedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string changes;
        public string Changes
        {
            get
            {
                return changes;
            }
            set
            {
                if (changes != value)
                {
                    changes = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

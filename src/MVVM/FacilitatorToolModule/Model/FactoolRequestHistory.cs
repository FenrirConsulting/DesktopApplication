using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class FactoolRequestHistory : ObservableObject
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

        private string referenceNumber;
        public string ReferenceNumber
        {
            get
            {
                return referenceNumber;
            }
            set
            {
                if (referenceNumber != value)
                {
                    referenceNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string displayName;
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string dateModified;
        public string DateModified
        {
            get
            {
                return dateModified;
            }
            set
            {
                if (dateModified != value)
                {
                    dateModified = value;
                    OnPropertyChanged();
                }
            }
        }

        private string modifiedTick;
        public string ModifiedTick
        {
            get
            {
                return modifiedTick;
            }
            set
            {
                if (modifiedTick != value)
                {
                    modifiedTick = value;
                    OnPropertyChanged();
                }
            }
        }

        private string state;
        public string State
        {
            get
            {
                return state;
            }
            set
            {
                if (state != value)
                {
                    state = value;
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

using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class FactoolRequestApplicationHistoryModel : ObservableObject
    {
        #region Properties
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

        private string system;
        public string System
        {
            get
            {
                return system;
            }
            set
            {
                if (system != value)
                {
                    system = value;
                    OnPropertyChanged();
                }
            }
        }

        private string createdDate;
        public string CreatedDate
        {
            get
            {
                return createdDate;
            }
            set
            {
                if (createdDate != value)
                {
                    createdDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string slaStart;
        public string SLAStart
        {
            get
            {
                return slaStart;
            }
            set
            {
                if (slaStart != value)
                {
                    slaStart = value;
                    OnPropertyChanged();
                }
            }
        }

        private string completedDate;
        public string CompletedDate
        {
            get
            {
                return completedDate;
            }
            set
            {
                if (completedDate != value)
                {
                    completedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string completedBy;
        public string CompletedBy
        {
            get
            {
                return completedBy;
            }
            set
            {
                if (completedBy != value)
                {
                    completedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string completedFlag;
        public string CompletedFlag
        {
            get
            {
                return completedFlag;
            }
            set
            {
                if (completedFlag != value)
                {
                    completedFlag = value;
                    OnPropertyChanged();
                }
            }
        }

        private string users;
        public string Users
        {
            get
            {
                return users;
            }
            set
            {
                if (users != value)
                {
                    users = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

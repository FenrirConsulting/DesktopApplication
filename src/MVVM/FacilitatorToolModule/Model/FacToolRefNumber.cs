using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class FacToolRefNumber : ObservableObject
    {
        #region Properties
        // Ref Id
        private int id;
        public int _id
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

        // String date for Ref# MMddyy
        private string referenceDate;
        public string ReferenceDate
        {
            get
            {
                return referenceDate;
            }
            set
            {
                if (referenceDate != value)
                {
                    referenceDate = value;
                    OnPropertyChanged();
                }
            }
        }

        // In sequence # of Ref# for date 1-999
        private string sequence;
        public string Sequence
        {
            get
            {
                return sequence;
            }
            set
            {
                if (sequence != value)
                {
                    sequence = value;
                    OnPropertyChanged();
                }
            }
        }

        // string representation of sequence as 3-digit value
        private string referenceSequence;
        public string ReferenceSequence
        {
            get
            {
                return referenceSequence;
            }
            set
            {
                if (referenceSequence != value)
                {
                    referenceSequence = value;
                    OnPropertyChanged();
                }
            }
        }

        // Ref# in format PMMddyy000P2
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

        // Previous Ref# in format PMMddyy000P2
        private string previousReferenceNumber;
        public string PreviousReferenceNumber
        {
            get
            {
                return previousReferenceNumber;
            }
            set
            {
                if (previousReferenceNumber != value)
                {
                    previousReferenceNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

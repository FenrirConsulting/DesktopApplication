using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class AccessNowClosedTicketItemModel : ObservableObject 
    {
        #region Properties
        private int id;
        public int ID
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

        private string ticketNumber;
        public string TicketNumber
        {
            get
            {
                return ticketNumber;
            }
            set
            {
                if (ticketNumber != value)
                {
                    ticketNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string finishedState;
        public string FinishedState
        {
            get
            {
                return finishedState;
            }
            set
            {
                if (finishedState != value)
                {
                    finishedState = value;
                    OnPropertyChanged();
                }
            }
        }

        private string comments;
        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                if (comments != value)
                {
                    comments = value;
                    OnPropertyChanged();
                }
            }
        }

        private string closedBy;
        public string ClosedBy
        {
            get
            {
                return closedBy;
            }
            set
            {
                if (closedBy != value)
                {
                    closedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private string dateClosed;
        public string DateClosed
        {
            get
            {
                return dateClosed;
            }
            set
            {
                if (dateClosed != value)
                {
                    dateClosed = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

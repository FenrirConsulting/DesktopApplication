using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class AccessNowRelatedTicketsModel : ObservableObject
    {
        #region Properties
        private string requestSubmissionId;
        public string RequestSubmissionId
        {
            get
            {
                return requestSubmissionId;
            }
            set
            {
                if (requestSubmissionId != value)
                {
                    requestSubmissionId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestSubmissionFulfillmentStatus;
        public string RequestSubmissionFulfillmentStatus
        {
            get
            {
                return requestSubmissionFulfillmentStatus;
            }
            set
            {
                if (requestSubmissionFulfillmentStatus != value)
                {
                    requestSubmissionFulfillmentStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

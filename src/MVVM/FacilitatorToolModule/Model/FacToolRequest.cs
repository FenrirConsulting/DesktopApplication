using IAMHeimdall.Core;
using System;

namespace IAMHeimdall.MVVM.Model
{
    public class FacToolRequest : ObservableObject
    {
        #region Properties
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

        private int reqStatusID;
        public int ReqStatusID
        {
            get
            {
                return reqStatusID;
            }
            set
            {
                if (reqStatusID != value)
                {
                    reqStatusID = value;
                    OnPropertyChanged();
                }
            }
        }

        private string createDate;
        public string CreateDate
        {
            get
            {
                return createDate;
            }
            set
            {
                if (createDate != value)
                {
                    createDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string createTick;
        public string CreateTick
        {
            get
            {
                return createTick;
            }
            set
            {
                if (createTick != value)
                {
                    createTick = value;
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

        private string samAccount;
        public string SamAccount
        {
            get
            {
                return samAccount;
            }
            set
            {
                if (samAccount != value)
                {
                    samAccount = value;
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

        private string newRequest;
        public string NewRequest
        {
            get
            {
                return newRequest;
            }
            set
            {
                if (newRequest != value)
                {
                    newRequest = value;
                    OnPropertyChanged();
                }
            }
        }

        private string totalUsers;
        public string TotalUsers
        {
            get
            {
                return totalUsers;
            }
            set
            {
                if (totalUsers != value)
                {
                    totalUsers = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestStatus;
        public string RequestStatus
        {
            get
            {
                return requestStatus;
            }
            set
            {
                if (requestStatus != value)
                {
                    requestStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string formType;
        public string FormType
        {
            get
            {
                return formType;
            }
            set
            {
                if (formType != value)
                {
                    formType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string lOBType;
        public string LOBType
        {
            get
            {
                return lOBType;
            }
            set
            {
                if (lOBType != value)
                {
                    lOBType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string[] requestType;
        public string[] RequestType
        {
            get
            {
                return requestType;
            }
            set
            {
                if (requestType != value)
                {
                    requestType = value;
                    OnPropertyChanged();
                }
            }
        }

        private string[] defectReason;
        public string[] DefectReason
        {
            get
            {
                return defectReason;
            }
            set
            {
                if (defectReason != value)
                {
                    defectReason = value;
                    OnPropertyChanged();
                }
            }
        }

        private string[] systems;
        public string[] Systems
        {
            get
            {
                return systems;
            }
            set
            {
                if (systems != value)
                {
                    systems = value;
                    OnPropertyChanged();
                }
            }
        }

        private string[] replyTypes;
        public string[] ReplyTypes
        {
            get
            {
                return replyTypes;
            }
            set
            {
                if (replyTypes != value)
                {
                    replyTypes = value;
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

        private string xREF1;
        public string XREF1
        {
            get
            {
                return xREF1;
            }
            set
            {
                if (xREF1 != value)
                {
                    xREF1 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string xREF2;
        public string XREF2
        {
            get
            {
                return xREF2;
            }
            set
            {
                if (xREF2 != value)
                {
                    xREF2 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string sentStatus;
        public string SentStatus
        {
            get
            {
                return sentStatus;
            }
            set
            {
                if (sentStatus != value)
                {
                    sentStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestState;
        public string RequestState
        {
            get
            {
                return requestState;
            }
            set
            {
                if (requestState != value)
                {
                    requestState = value;
                    OnPropertyChanged();
                }
            }
        }

        private string touchPoints;
        public string TouchPoints
        {
            get
            {
                return touchPoints;
            }
            set
            {
                if (touchPoints != value)
                {
                    touchPoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private string timesReturned;
        public string TimesReturned
        {
            get
            {
                return timesReturned;
            }
            set
            {
                if (timesReturned != value)
                {
                    timesReturned = value;
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

        private string completionDate;
        public string CompletionDate
        {
            get
            {
                return completionDate;
            }
            set
            {
                if (completionDate != value)
                {
                    completionDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string slaCompletionTime;
        public string SLACompletionTime
        {
            get
            {
                return slaCompletionTime;
            }
            set
            {
                if (slaCompletionTime != value)
                {
                    slaCompletionTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private string agentComments;
        public string AgentComments
        {
            get
            {
                return agentComments;
            }
            set
            {
                if (agentComments != value)
                {
                    agentComments = value;
                    OnPropertyChanged();
                }
            }
        }

        private string agentsWorked;
        public string AgentsWorked
        {
            get
            {
                return agentsWorked;
            }
            set
            {
                if (agentsWorked != value)
                {
                    agentsWorked = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

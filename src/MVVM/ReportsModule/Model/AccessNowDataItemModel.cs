using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class AccessNowDataItemModel : ObservableObject
    {
        #region Properties
        private long request_id;
        public long Request_id
        {
            get
            {
                return request_id;
            }
            set
            {
                if (request_id != value)
                {
                    request_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requested_on;
        public string Requested_on
        {
            get
            {
                return requested_on;
            }
            set
            {
                if (requested_on != value)
                {
                    requested_on = value;
                    OnPropertyChanged();
                }
            }
        }

        private string request_type;
        public string Request_type
        {
            get
            {
                return request_type;
            }
            set
            {
                if (request_type != value)
                {
                    request_type = value;
                    OnPropertyChanged();
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        private string closed_on;
        public string Closed_on
        {
            get
            {
                return closed_on;
            }
            set
            {
                if (closed_on != value)
                {
                    closed_on = value;
                    OnPropertyChanged();
                }
            }
        }

        private string asset;
        public string Asset
        {
            get
            {
                return asset;
            }
            set
            {
                if (asset != value)
                {
                    asset = value;
                    OnPropertyChanged();
                }
            }
        }

        private string access;
        public string Access
        {
            get
            {
                return access;
            }
            set
            {
                if (access != value)
                {
                    access = value;
                    OnPropertyChanged();
                }
            }
        }

        private string sub_request_type;
        public string Sub_request_type
        {
            get
            {
                return sub_request_type;
            }
            set
            {
                if (sub_request_type != value)
                {
                    sub_request_type = value;
                    OnPropertyChanged();
                }
            }
        }

        private string account_name;
        public string Account_name
        {
            get
            {
                return account_name;
            }
            set
            {
                if (account_name != value)
                {
                    account_name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestor_name;
        public string Requestor_name
        {
            get
            {
                return requestor_name;
            }
            set
            {
                if (requestor_name != value)
                {
                    requestor_name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string requestor_employee_id;
        public string Requestor_employee_id
        {
            get
            {
                return requestor_employee_id;
            }
            set
            {
                if (requestor_employee_id != value)
                {
                    requestor_employee_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string ticket_id;
        public string Ticket_id
        {
            get
            {
                return ticket_id;
            }
            set
            {
                if (ticket_id != value)
                {
                    ticket_id = value;
                    OnPropertyChanged();
                }
            }
        }

        private string ticket_status;
        public string Ticket_status
        {
            get
            {
                return ticket_status;
            }
            set
            {
                if (ticket_status != value)
                {
                    ticket_status = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mail_to;
        public string Mail_to
        {
            get
            {
                return mail_to;
            }
            set
            {
                if (mail_to != value)
                {
                    mail_to = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mail_sent_on;
        public string Mail_sent_on
        {
            get
            {
                return mail_sent_on;
            }
            set
            {
                if (mail_sent_on != value)
                {
                    mail_sent_on = value;
                    OnPropertyChanged();
                }
            }
        }

        private string mail_sent_on_date;
        public string Mail_sent_on_date
        {
            get
            {
                return mail_sent_on_date;
            }
            set
            {
                if (mail_sent_on_date != value)
                {
                    mail_sent_on_date = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

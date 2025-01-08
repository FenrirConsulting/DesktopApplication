using IAMHeimdall.Core;
using System;
using System.Text.RegularExpressions;

namespace IAMHeimdall.MVVM.Model
{
    public class KerberosUserModel : ObservableObject
    {
        #region Properties
        private string _UidRef;
        public string UIDRef
        {
            get
            {
                return _UidRef;
            }
            set
            {
                if (_UidRef != value)
                {
                    _UidRef = value;
                    var numeric = Regex.Replace(_UidRef, @"[^\d]+", "\n").Trim();
                    _UidRef = numeric;
                    OnPropertyChanged();
                }
            }
        }

        private string _LogIDRef;
        public string LogIDRef
        {
            get
            {
                return _LogIDRef;
            }
            set
            {
                if (_LogIDRef != value)
                {
                    _LogIDRef = value;
                    if (_LogIDRef.Length > 0)
                    {
                        _LogIDRef = _LogIDRef.ToLower();
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _EmpIDRef;
        public string EmpIDRef
        {
            get
            {
                return _EmpIDRef;
            }
            set
            {
                if (_EmpIDRef != value)
                {
                    _EmpIDRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ADIDRef;
        public string ADIDRef
        {
            get
            {
                return _ADIDRef;
            }
            set
            {
                if (_ADIDRef != value)
                {
                    _ADIDRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FirstName;
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _LastName;
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _CommentsRef;
        public string CommentsRef
        {
            get
            {
                return _CommentsRef;
            }
            set
            {
                if (_CommentsRef != value)
                {
                    _CommentsRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ServersRef;
        public string ServersRef
        {
            get
            {
                return _ServersRef;
            }
            set
            {
                if (_ServersRef != value)
                {
                    _ServersRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _HistoryRef;
        public string HistoryRef
        {
            get
            {
                return _HistoryRef;
            }
            set
            {
                if (_HistoryRef != value)
                {
                    _HistoryRef = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

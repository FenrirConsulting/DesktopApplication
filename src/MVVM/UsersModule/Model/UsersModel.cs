using IAMHeimdall.Core;
using System.Text.RegularExpressions;

namespace IAMHeimdall.MVVM.Model
{
    public class UsersModel : ObservableObject
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

        private string _LogRef;
        public string LogRef
        {
            get
            {
                return _LogRef;
            }
            set
            {
                if (_LogRef != value)
                {
                    _LogRef = value;
                    if (_LogRef.Length > 0)
                    {
                        _LogRef = _LogRef.ToLower();
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _TypeRef;
        public string TypeRef
        {
            get
            {
                return _TypeRef;
            }
            set
            {
                if (_TypeRef != value)
                {
                    _TypeRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _QueueRef;
        public string QueueRef
        {
            get
            {
                return _QueueRef;
            }
            set
            {
                if (_QueueRef != value)
                {
                    _QueueRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _AdidRef;
        public string AdidRef
        {
            get
            {
                return _AdidRef;
            }
            set
            {
                if (_AdidRef != value)
                {
                    _AdidRef = value;
                    _AdidRef = _AdidRef.ToUpper();
                    OnPropertyChanged();
                }
            }
        }

        private string _EmpRef;
        public string EmpRef
        {
            get
            {
                return _EmpRef;
            }
            set
            {
                if (_EmpRef != value)
                {
                    _EmpRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _FirstRef;
        public string FirstRef
        {
            get
            {
                return _FirstRef;
            }
            set
            {
                if (_FirstRef != value)
                {
                    _FirstRef = value;
                    _FirstRef = _FirstRef.Replace(",", string.Empty);
                    OnPropertyChanged();
                }
            }
        }

        private string _LastRef;
        public string LastRef
        {
            get
            {
                return _LastRef;
            }
            set
            {
                if (_LastRef != value)
                {
                    _LastRef = value;
                    _LastRef = _LastRef.Replace(",", string.Empty);
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

using IAMHeimdall.Core;
using System.Text.RegularExpressions;

namespace IAMHeimdall.MVVM.Model
{
    public class KerberosGroupModel : ObservableObject
    {
        #region Properties
        private string _GidRef;
        public string GIDRef
        {
            get
            {
                return _GidRef;
            }
            set
            {
                if (_GidRef != value)
                {
                    _GidRef = value;
                    var numeric = Regex.Replace(_GidRef, @"[^\d]+", "\n").Trim();
                    _GidRef = numeric;
                    OnPropertyChanged();
                }
            }
        }

        private string _GroupRef;
        public string GroupRef
        {
            get
            {
                return _GroupRef;
            }
            set
            {
                if (_GroupRef != value)
                {
                    _GroupRef = value;
                    if (_GroupRef.Length > 0)
                    {
                        _GroupRef = _GroupRef.ToLower();
                    }
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

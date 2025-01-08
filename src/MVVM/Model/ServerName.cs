using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class ServerName : ObservableObject
    {
        #region Properties
        private string serverTerm;
        public string ServerTerm
        {
            get
            {
                return serverTerm;
            }
            set
            {
                if (serverTerm != value)
                {
                    serverTerm = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

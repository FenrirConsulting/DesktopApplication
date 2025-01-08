using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class SearchTerms : ObservableObject
    {
        #region Properties
        private string searchTerm;
        public string SearchTerm
        {
            get
            {
                return searchTerm;
            }
            set
            {
                if (searchTerm != value)
                {
                    searchTerm = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

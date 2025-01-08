using System;
using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class PaginationModel : ObservableObject
    {
        #region Methods
        public PaginationModel(int _totalitemscount, int _pageitemcount)
        {
            Initiation(_totalitemscount, _pageitemcount);
        }

        private void Initiation(int _totalitemscount, int _pageitemcount)
        {
            TotalItems = _totalitemscount;
            ItemsPerPage = _pageitemcount;

            Math.DivRem(TotalItems, ItemsPerPage, out int outValue);
            TotalPages = TotalItems / ItemsPerPage;
            if (outValue != 0) { TotalPages++; } //Increment by 1 to handle extra values
            CurrentPage = 1;
            MaxItemsPerPage = 15;
        }
        #endregion

        #region Properties
        public int MaxItemsPerPage { get; set; }

        private int _totalItems;
        public int TotalItems
        {
            get { return _totalItems; }
            set { _totalItems = value; OnPropertyChanged(); }
        }

        private int _itemsPerPage;
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set { _itemsPerPage = value; OnPropertyChanged(); }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value < _totalPages + 1 && value > 0)
                {
                    _currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            set { _totalPages = value; OnPropertyChanged(); }
        }
        #endregion
    }
}

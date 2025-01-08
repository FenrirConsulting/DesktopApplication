using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class ComboBoxListItem : ObservableObject
    {
        #region Properties
        private string boxItem;
        public string BoxItem
        {
            get
            {
                return boxItem;
            }
            set
            {
                if (boxItem != value)
                {
                    boxItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

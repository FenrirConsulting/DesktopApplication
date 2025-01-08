using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public class FacToolItem : ObservableObject
    {
        #region Properties
        // Factool Form Item Property
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

        // Factool Form Item Property
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        // Factool Form Item Property
        private string _value;
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        // Factool Form Item Property
        private string displayOrder;
        public string DisplayOrder
        {
            get
            {
                return displayOrder;
            }
            set
            {
                if (displayOrder != value)
                {
                    displayOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        // Factool Form Item Property
        private string modifiedBy;
        public string ModifiedBy
        {
            get
            {
                return modifiedBy;
            }
            set
            {
                if (modifiedBy != value)
                {
                    modifiedBy = value;
                    OnPropertyChanged();
                }
            }
        }

        // Factool Form Item Property
        private string modified;
        public string Modified
        {
            get
            {
                return modified;
            }
            set
            {
                if (modified != value)
                {
                    modified = value;
                    OnPropertyChanged();
                }
            }
        }

        // Factool Form Item Property
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

        // Factool Form Item Property
        private bool isCustom;
        public bool IsCustom
        {
            get
            {
                return isCustom;
            }
            set
            {
                if (isCustom != value)
                {
                    isCustom = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

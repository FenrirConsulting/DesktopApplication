using IAMHeimdall.Core;
using System.Collections.Generic;

namespace IAMHeimdall.MVVM.Model
{
    public class ReportModel : ObservableObject
    {
        #region Properties
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

        private string tableName;
        public string TableName
        {
            get
            {
                return tableName;
            }
            set
            {
                if (tableName != value)
                {
                    tableName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string joinedTable;
        public string JoinedTable
        {
            get
            {
                return joinedTable;
            }
            set
            {
                if (joinedTable != value)
                {
                    joinedTable = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> columnList;
        public List<string> ColumnList
        {
            get
            {
                return columnList;
            }
            set
            {
                if (columnList != value)
                {
                    columnList = value;
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

        private bool hasDateColumn;
        public bool HasDateColumn
        {
            get
            {
                return hasDateColumn;
            }
            set
            {
                if (hasDateColumn != value)
                {
                    hasDateColumn = value;
                    OnPropertyChanged();
                }
            }
        }

        private string dateColumnName;
        public string DateColumnName
        {
            get
            {
                return dateColumnName;
            }
            set
            {
                if (dateColumnName != value)
                {
                    dateColumnName = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

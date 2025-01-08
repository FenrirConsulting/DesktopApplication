using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.IAMToolModule.Model 
{
    public class AddGroupObjectTypeModel : ObservableObject
    {
        #region Properties
        private int iD;
        public int ID
        {
            get
            {
                return iD;
            }
            set
            {
                if (iD != value)
                {
                    iD = value;
                    OnPropertyChanged();
                }
            }
        }

        private string objType;
        public string ObjType
        {
            get
            {
                return objType;
            }
            set
            {
                if (objType != value)
                {
                    objType = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

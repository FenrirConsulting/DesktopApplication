using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.IAMToolModule.Model
{
    public class AddGroupObjectDomainModel : ObservableObject
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

        private string objDomain;
        public string ObjDomain
        {
            get
            {
                return objDomain;
            }
            set
            {
                if (objDomain != value)
                {
                    objDomain = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

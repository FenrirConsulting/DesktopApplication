using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.IAMToolModule.Model
{
    public class AddGroupGroupDomainModel : ObservableObject
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

        private string grpDomain;
        public string GrpDomain
        {
            get
            {
                return grpDomain;
            }
            set
            {
                if (grpDomain != value)
                {
                    grpDomain = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

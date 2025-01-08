using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IAMHeimdall.MVVM.ViewModel
{
    public class IAMToolUpdateModelViewModel : BaseViewModel
    {
        #region Delegates

        #endregion

        #region Methods
        public IAMToolUpdateModelViewModel()
        {

        }
        #endregion

        #region Functions
        public void LoadData()
        {
            try
            {
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;

namespace IAMHeimdall.Core
{
    public class UserInfo
    {
        #region Properties
        //Collects AD Information about current user
        public String CurrentUserRole { get; set; }
        public Boolean IsDev { get; set; }
        public String CurrentUserDomain { get; set; }
        public List<String> CurrentUserGroups = new();
        public String SamAccountName { get; set; }
        public String DisplayName { get; set; }
        public String GivenName { get; set; }
        public String DistinguishedName { get; set; }
        public String SurName { get; set; }
        public String UserInitials { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public bool FacToolMinRole { get; set; }
        public bool FactoolCompletionRole { get; set; }
        public bool ServiceCatalogRole { get; set; }
        public bool ServiceCatalogMgrRole { get; set; }
        public String Envrionment { get; set; }
        public String ConnectionString { get; set; }
        #endregion
    }
}
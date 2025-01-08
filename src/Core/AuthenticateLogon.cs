using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace IAMHeimdall.Core
{
    public class AuthenticateLogon : ObservableObject
    {
        #region Delegates
        // Set of Parameters controlling Windows Impersonation
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
        int dwLogonType, int dwLogonProvider, out SafeAccessTokenHandle phToken);
        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_INTERACTIVE = 9;
        public bool returnValue;
        #endregion

        #region Methods
        public AuthenticateLogon()
        {

        }
        #endregion

        #region Functions
        public static SafeAccessTokenHandle AuthenticateFunction(string username, string domain, string password,int LOGON32_LOGON_INTERACTIVE)
        {

            // On Validated Authentication Produces Token for Windows Impersonation
            _ = LogonUser(username, domain, password,
            LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
            out SafeAccessTokenHandle tempHandle);

            return tempHandle;
        }
        #endregion
    }
}

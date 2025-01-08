using System.Reflection;

namespace IAMHeimdall.Resources.Commands
{
    public class LoadAssemblies
    {
        #region Functions
        // Manual Assembly Reload. Used for some User Impersonation Functions
        public static void RunAssemblies()
        {
            Assembly.Load("util, Version=5.8.0.132, Culture=neutral, PublicKeyToken=1f9cb67793557b0b");
            Assembly.Load("centrifydc.api, Version=5.8.0.132, Culture=neutral, PublicKeyToken=1f9cb67793557b0b");
            Assembly.Load("Centrify.Cfw.Core, Version=5.8.0.132, Culture=neutral, PublicKeyToken=1f9cb67793557b0b");
            Assembly.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assembly.Load("System.Text.RegularExpressions, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assembly.Load("System.Diagnostics.TextWriterTraceListener, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Assembly.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            Assembly.Load("System.Security.AccessControl, Version = 5.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a");
            Assembly.Load("System.IO.FileSystem.AccessControl, Version = 5.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a");
            Assembly.Load("System.Web, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a");
        }
        #endregion
    }
}

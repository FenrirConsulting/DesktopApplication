using IAMHeimdall.Core;
using IAMHeimdall.MVVM.ViewModel;
using IAMHeimdall.MVVM.View;
using System.Windows;
using System;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using SimpleImpersonation;
using System.IO;

namespace IAMHeimdall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region App Delegates
        public static UserInfo GlobalUserInfo = new();
        #endregion

        #region Properties
        public static string AuthenticatedUsername { get; set; }
        public static string AuthenticatedPassword { get; set; }
        public static string AuthenticatedDomain { get; set; }
        public static string LogonChoice { get; set; }
        // Token Used for Authenticated Admin Functions
        public static SafeAccessTokenHandle SafeAccessTokenHandle { get; set; }
        #endregion

        #region Methods
        public App()
        {
            LogonChoice = "0";
            try
            {
                InitializeComponent();
                MainWindow window = new();
                Task authTask = Task.Run(() =>
                {
                    try
                    {
                        if (LogonChoice == "1") 
                        {
                            var credentials = new UserCredentials(AuthenticatedDomain, AuthenticatedUsername, AuthenticatedPassword);
                            Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
                            {
                                BridgeViewModel Bridge = new();
                                Bridge.LoadVM();
                            });
                        }
                        else 
                        {
                            BridgeViewModel Bridge = new();
                            Bridge.LoadVM();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionOutput.Output(ex.ToString());
                    }
                });

                authTask.Wait();
                authTask.ContinueWith((t) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            if (LogonChoice == "1")
                            {
                                var credentials = new UserCredentials(AuthenticatedDomain, AuthenticatedUsername, AuthenticatedPassword);
                                Impersonation.RunAsUser(credentials, LogonType.Interactive, () =>
                                {
                                    MainWindow window = new();
                                    window.Show();
                                });
                            }
                            else 
                            {
                                MainWindow window = new();
                                window.Show();
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionOutput.Output(ex.ToString());
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                Environment.Exit(-1);
                throw;
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}

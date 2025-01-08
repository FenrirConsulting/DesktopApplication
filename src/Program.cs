using System;
using Serilog.Exceptions;
using Serilog;
using System.Configuration;
using Serilog.Sinks.MSSqlServer;
using Serilog.Events;
using IAMHeimdall.Core;
using System.DirectoryServices.AccountManagement;
using System.Data;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Threading;
using IAMHeimdall.MVVM.Model;

namespace IAMHeimdall
{
    public class Program
    {
        #region Main Program Entry Point
        public static string ConnectionString = ConfigurationProperties.ConnectionString;

        [STAThread]
        static void Main(string[] args)
        {
            // Begins Serilog to SQL Database, Sets Columns and Data
            var connectionString = ConfigurationProperties.ConnectionString;
            // Check for connection to database. Makes up to 5 attempts and then exits on failure to connect.
            int connectionAttempts = 0;
            bool checkConnection = IsServerConnected(connectionString);
            while (checkConnection == false && connectionAttempts < 5)
            {
                Thread.Sleep(1000);
                checkConnection = IsServerConnected(connectionString);
                connectionAttempts++;
                if (connectionAttempts == 4) { MessageBox.Show("Cannot Reach Server. Exiting.");  }
            }

            var tableName = "IAMHLog";
            var columnOpts = new ColumnOptions();
            columnOpts.Store.Remove(StandardColumn.Properties);
            columnOpts.Store.Remove(StandardColumn.Exception);
            columnOpts.Store.Remove(StandardColumn.LogEvent);
            columnOpts.Store.Remove(StandardColumn.Level);
            columnOpts.Store.Remove(StandardColumn.MessageTemplate);
            columnOpts.LogEvent.DataLength = 2048;
            columnOpts.TimeStamp.NonClusteredIndex = true;
            columnOpts.AdditionalColumns = new Collection<SqlColumn>//Add custom column
                {
                    new SqlColumn { DataType = SqlDbType.NVarChar, DataLength = -1, ColumnName = "CurrentUser" }
                };

            #region Logger Configuration
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .MinimumLevel.Debug()
                         .Enrich.WithExceptionDetails()
                         .Enrich.FromLogContext()
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                         .Enrich.FromLogContext()
                          .Enrich.WithProperty("CurrentUser", UserPrincipal.Current.DisplayName.ToString())
                         .WriteTo.MSSqlServer(
                         connectionString: connectionString,
                         sinkOptions: new MSSqlServerSinkOptions { TableName = tableName },

                         columnOptions: columnOpts)
                         .CreateLogger();

            #endregion

            AppDomain.CurrentDomain.UnhandledException += AppUnhandledException;
            try
            {
                App app = new();
                app.Run();
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        #endregion

        #region Functions
        private static bool IsServerConnected(string connectionString)
        {
            using SqlConnection connection = new(connectionString);
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
        #endregion

        #region Event Handlers
        // Global unhandled exception Log
        private static void AppUnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            if (Log.Logger != null && ex.ExceptionObject is Exception exception)
            {
                UnhandledExceptions(exception);

                // It's not necessary to flush if the application isn't terminating.
                if (ex.IsTerminating)
                {
                    Log.CloseAndFlush();
                }
            }
        }

        private static void UnhandledExceptions(Exception ex)
        {
            ExceptionOutput.Output(ex.ToString());
        }
        #endregion
    }
}

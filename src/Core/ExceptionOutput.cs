using Serilog;

namespace IAMHeimdall.Core
{
    public class ExceptionOutput
    {
        #region Methods
        // Method to Handle Exporting Error Logs to MSSQL Sink
        public ExceptionOutput()
        {
        }

        public static void Output(string exception)
        {
            Log.Information(exception.ToString());
        }
        #endregion
    }
}

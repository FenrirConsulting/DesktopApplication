using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Data;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class ManualCompletionViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand SetEnvrionmentCommand { get; set; }
        public RelayCommand CheckConfigCommand { get; set; }
        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties

        #endregion

        #region Methods

        public ManualCompletionViewModel()
        {
            SetEnvrionmentCommand = new RelayCommand((box) => { SetEnvironment(box.ToString()); });
            CheckConfigCommand = new RelayCommand(o => { CheckConfig(); });
        }

        #endregion

        #region Functions
        // On Load Function
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

        public void SetEnvironment(string ENV)
        {
            try
            {
                switch (ENV)
                {
                    case "PROD":
                        Program.ConnectionString = ConfigurationProperties.ConnectionString;
                        break;

                    case "QA":
                        Program.ConnectionString = ConfigurationProperties.QAConnectionString;
                        break;

                    case "DEV":
                        Program.ConnectionString = ConfigurationProperties.DEVConnectionString;
                        break;

                    default:
                        break;
                }

                MessageBox.Show(Program.ConnectionString);
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
                throw;
            }
        }

        public void CheckConfig()
        {
            
            try
            {
                DataTable tempTable = new();
                tempTable = DBConn.GetSelectedRecord(ConfigurationProperties.IAMHConfig, "Setting", "FactoolUpdateCount");

                string checkReturn = tempTable.Rows[0][2].ToString();

                MessageBox.Show(checkReturn);
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

using IAMHeimdall.Core;
using IAMHeimdall.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IAMHeimdall.MVVM.ViewModel
{
    public class UserConfigurationViewModel : BaseViewModel
    {
        #region Delegates
        public RelayCommand TestButtonCommand { get; set; }
        public RelayCommand TestButtonCommand2 { get; set; }
        private readonly SQLMethods DBConn = new();
        #endregion

        #region Properties
        // Stored User Info 
        private UserInfo currentUserInfo;
        public UserInfo CurrentUserInfo
        {
            get { return currentUserInfo; }
            set { currentUserInfo = value; }
        }

        // Binds Decimal Value for Font Scale
        private double fontSliderDouble;
        public double FontSliderDouble
        {
            get { return fontSliderDouble; }
            set
            {
                if (fontSliderDouble != value)
                {
                    value = Math.Round(value, 2);
                    fontSliderDouble = value;
                    FontSliderLabelString = "Font Scale : " + fontSliderDouble.ToString("#0.##%");
                    OnPropertyChanged();
                }
            }
        }

        // Binds Font Slider Bar Label
        private string fontSliderLabelString;
        public string FontSliderLabelString
        {
            get { return fontSliderLabelString; }
            set
            {
                if (fontSliderLabelString != value)
                {
                    fontSliderLabelString = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        public UserConfigurationViewModel()
        {
            // Initialize Properties
            CurrentUserInfo = new UserInfo();

            // Initialize Relay Commands
            TestButtonCommand = new RelayCommand(o => { TestButton(); });
            TestButtonCommand2 = new RelayCommand(o => { TestButton2(); });
        }
        #endregion

        #region Functions
        // On Load Function
        public void LoadData()
        {
            CurrentUserInfo = App.GlobalUserInfo;
            SetFontSliderInt();
        }

        public static void TestButton()
        {
            try
            {
                MessageBox.Show(MainViewModel.UserConfigFontScale.ToString());
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        public void TestButton2()
        {
            try
            {
                DBConn.UpdateTableRecord(ConfigurationProperties.UserConfiguration, CurrentUserInfo.SamAccountName, "Username", "FontScale", fontSliderDouble.ToString());
                DataTable userCheck = DBConn.GetSelectedRecord(ConfigurationProperties.UserConfiguration, "Username", CurrentUserInfo.SamAccountName);
                if (userCheck.Rows.Count != 0)
                {
                    bool success = double.TryParse(userCheck.Rows[0]["FontScale"].ToString(), out double fontParse);
                    if (success == true) { MainViewModel.UserConfigFontScale = fontParse; }
                    SetFontSliderInt();
                }
            }
            catch (Exception ex)
            {
                ExceptionOutput.Output(ex.ToString());
            }
        }

        // Parse String for Possible Int to Set Slider To
        public void SetFontSliderInt()
        {
            double fontParse = MainViewModel.UserConfigFontScale;
            fontSliderDouble = fontParse;
        }
        #endregion
    }
}

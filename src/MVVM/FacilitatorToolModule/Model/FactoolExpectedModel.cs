using IAMHeimdall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IAMHeimdall.MVVM.Model
{
    public  class FactoolExpectedModel : ObservableObject
    {
        #region Properties
        private int _id;
        public int _iD
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        // String for Expected Class Name
        private string className;
        public string ClassName
        {
            get
            {
                return className;
            }
            set
            {
                if (className != value)
                {
                    className = value;
                    OnPropertyChanged();
                }
            }
        }

        // String holding the StartDate
        private string startDate;
        public string StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        // N Number String
        private string n_Number;
        public string N_Number
        {
            get
            {
                return n_Number;
            }
            set
            {
                if (n_Number != value)
                {
                    n_Number = value;
                    OnPropertyChanged();
                }
            }
        }

        // String holding the PERSONA 
        private string persona;
        public string PERSONA
        {
            get
            {
                return persona;
            }
            set
            {
                if (persona != value)
                {
                    persona = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds number of expected Users
        private string expectedUsers;
        public string ExpectedUsers
        {
            get
            {
                return expectedUsers;
            }
            set
            {
                if (expectedUsers != value)
                {
                    expectedUsers = value;
                    var numeric = Regex.Replace(expectedUsers, @"[^\d]+", "\n").Trim();
                    expectedUsers = numeric;
                    OnPropertyChanged();
                }
            }
        }

        // Holds number of expected Apps
        private string expectedApps;
        public string ExpectedApps
        {
            get
            {
                return expectedApps;
            }
            set
            {
                if (expectedApps != value)
                {
                    expectedApps = value;
                    var numeric = Regex.Replace(expectedApps, @"[^\d]+", "\n").Trim();
                    expectedApps = numeric;
                    OnPropertyChanged();
                }
            }
        }

        // Holds number of expected Touchpoints
        private string expectedTouchpoints;
        public string ExpectedTouchpoints
        {
            get
            {
                return expectedTouchpoints;
            }
            set
            {
                if (expectedTouchpoints != value)
                {
                    expectedTouchpoints = value;
                    OnPropertyChanged();
                }
            }
        }

        // Holds Class Owner Name
        private string classOwner;
        public string ClassOwner
        {
            get
            {
                return classOwner;
            }
            set
            {
                if (classOwner != value)
                {
                    classOwner = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAMHeimdall.Core;

namespace IAMHeimdall.MVVM.Model
{
    public class ExportColumns : ObservableObject
    {
        #region Properties
        private bool column1isChecked;
        public bool Column1IsChecked
        {
            get
            {
                return column1isChecked;
            }
            set
            {
                column1isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column1;
        public string Column1
        {
            get
            {
                return column1;
            }
            set
            {
                if (column1 != value)
                {
                    column1 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column2isChecked;
        public bool Column2IsChecked
        {
            get
            {
                return column2isChecked;
            }
            set
            {
                column2isChecked = value;
                OnPropertyChanged("IsCheck");
            }
        }

        private string column2;
        public string Column2
        {
            get
            {
                return column2;
            }
            set
            {
                if (column2 != value)
                {
                    column2 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column3isChecked;
        public bool Column3IsChecked
        {
            get
            {
                return column3isChecked;
            }
            set
            {
                column3isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column3;
        public string Column3
        {
            get
            {
                return column3;
            }
            set
            {
                if (column3 != value)
                {
                    column3 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column4isChecked;
        public bool Column4IsChecked
        {
            get
            {
                return column4isChecked;
            }
            set
            {
                column4isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column4;
        public string Column4
        {
            get
            {
                return column4;
            }
            set
            {
                if (column4 != value)
                {
                    column4 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column5isChecked;
        public bool Column5IsChecked
        {
            get
            {
                return column5isChecked;
            }
            set
            {
                column5isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column5;
        public string Column5
        {
            get
            {
                return column5;
            }
            set
            {
                if (column5 != value)
                {
                    column5 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column6isChecked;
        public bool Column6IsChecked
        {
            get
            {
                return column6isChecked;
            }
            set
            {
                column6isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column6;
        public string Column6
        {
            get
            {
                return column6;
            }
            set
            {
                if (column6 != value)
                {
                    column6 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column7isChecked;
        public bool Column7IsChecked
        {
            get
            {
                return column7isChecked;
            }
            set
            {
                column7isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column7;
        public string Column7
        {
            get
            {
                return column7;
            }
            set
            {
                if (column7 != value)
                {
                    column7 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column8isChecked;
        public bool Column8IsChecked
        {
            get
            {
                return column8isChecked;
            }
            set
            {
                column8isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column8;
        public string Column8
        {
            get
            {
                return column8;
            }
            set
            {
                if (column8 != value)
                {
                    column8 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column9isChecked;
        public bool Column9IsChecked
        {
            get
            {
                return column9isChecked;
            }
            set
            {
                column9isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column9;
        public string Column9
        {
            get
            {
                return column9;
            }
            set
            {
                if (column9 != value)
                {
                    column9 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column10isChecked;
        public bool Column10IsChecked
        {
            get
            {
                return column10isChecked;
            }
            set
            {
                column10isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column10;
        public string Column10
        {
            get
            {
                return column10;
            }
            set
            {
                if (column10 != value)
                {
                    column10 = value;
                    OnPropertyChanged();
                }
            }
        }

        private string column11;
        public string Column11
        {
            get
            {
                return column11;
            }
            set
            {
                if (column11 != value)
                {
                    column11 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column11isChecked;
        public bool Column11IsChecked
        {
            get
            {
                return column11isChecked;
            }
            set
            {
                column11isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column12;
        public string Column12
        {
            get
            {
                return column12;
            }
            set
            {
                if (column12 != value)
                {
                    column12 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column12isChecked;
        public bool Column12IsChecked
        {
            get
            {
                return column12isChecked;
            }
            set
            {
                column12isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column13;
        public string Column13
        {
            get
            {
                return column13;
            }
            set
            {
                if (column13 != value)
                {
                    column13 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column13isChecked;
        public bool Column13IsChecked
        {
            get
            {
                return column13isChecked;
            }
            set
            {
                column13isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column14;
        public string Column14
        {
            get
            {
                return column14;
            }
            set
            {
                if (column14 != value)
                {
                    column14 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column14isChecked;
        public bool Column14IsChecked
        {
            get
            {
                return column14isChecked;
            }
            set
            {
                column14isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column15;
        public string Column15
        {
            get
            {
                return column15;
            }
            set
            {
                if (column15 != value)
                {
                    column15 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column15isChecked;
        public bool Column15IsChecked
        {
            get
            {
                return column15isChecked;
            }
            set
            {
                column15isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column16;
        public string Column16
        {
            get
            {
                return column16;
            }
            set
            {
                if (column16 != value)
                {
                    column16 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column16isChecked;
        public bool Column16IsChecked
        {
            get
            {
                return column16isChecked;
            }
            set
            {
                column16isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column17;
        public string Column17
        {
            get
            {
                return column17;
            }
            set
            {
                if (column17 != value)
                {
                    column17 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column17isChecked;
        public bool Column17IsChecked
        {
            get
            {
                return column17isChecked;
            }
            set
            {
                column17isChecked = value;
                OnPropertyChanged();
            }
        }

        private string column18;
        public string Column18
        {
            get
            {
                return column18;
            }
            set
            {
                if (column18 != value)
                {
                    column18 = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool column18isChecked;
        public bool Column18IsChecked
        {
            get
            {
                return column18isChecked;
            }
            set
            {
                column18isChecked = value;
                OnPropertyChanged();
            }
        }

        private bool exportEverything;
        public bool ExportEverything
        {
            get
            {
                return exportEverything;
            }
            set
            {
                exportEverything = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}

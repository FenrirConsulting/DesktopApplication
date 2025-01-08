using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IAMHeimdall.Core
{
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Methods
        // Observable Object Resuable Class
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual void OnPropertyChangedName(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
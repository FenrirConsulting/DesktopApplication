using System;
using System.Windows.Input;

namespace IAMHeimdall.Core
{
    public class RelayCommand : ICommand
    {
        // Public Class Handling Relay Command Logic
        #region Delegates
        private Action<object> _execute;
        private Func<object, bool> _canExecute;
        #endregion

        #region Methods
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion
    }
}
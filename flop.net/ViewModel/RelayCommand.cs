using System;
using System.Diagnostics;
using System.Windows.Input;

namespace flop.net.ViewModel
{
    public class RelayCommand : ICommand
    {
        protected Action<object> execute;
        protected Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute"); 
            this.canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public void Execute(object parameter) 
        {
            execute(parameter);
        }
    }
}

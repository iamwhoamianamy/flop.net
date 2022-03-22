using System;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace flop.net.ViewModel
{
   public class RelayCommand : ICommand, INotifyPropertyChanged
   {
      protected Action<object> execute;
      protected Func<object, bool> canExecute;

      public event PropertyChangedEventHandler PropertyChanged;
      protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
      private bool isPressed = false;
      public bool IsPressed 
      { 
         get => isPressed;
         set
         {
            isPressed = value;
            OnPropertyChanged();
         }
      }
      public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
      {
         this.execute = execute ?? throw new ArgumentNullException("execute"); 
         this.canExecute = canExecute;
      }

      [DebuggerStepThrough]
      public bool CanExecute(object parameter)
      {
         return canExecute == null || canExecute(parameter);
         //return canExecute?.Invoke(parameter) ?? true;
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

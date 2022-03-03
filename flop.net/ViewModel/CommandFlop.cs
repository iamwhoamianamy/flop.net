using System;
using System.Diagnostics;
using System.Windows.Input;

namespace flop.net.ViewModel
{
    public class CommandFlop : RelayCommand
    {
        public string Label { get; set; }

        public CommandFlop(Action<object> action, Func<bool> canExecute = null, Action<object> unExecute = null, string name = null)
            : base(action, canExecute, unExecute)
        {
            Label = name;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> commandExecuteAction;
        private readonly Func<bool> commandCanExecute;
        private readonly Action<object> commandUnExecute;

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        /// <param name="execute">
        /// Действие, которое выполняется при вызове
        /// </param>
        /// <param name="canExecute">
        /// Функция, вызываемая для определения того, может ли команда выполнить действие
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если действие execute равно null
        /// </exception>
        public RelayCommand(Action<object> execute, Func<bool> canExecute = null, Action<object> unExecute = null)
        {
            commandExecuteAction = execute ?? throw new ArgumentNullException(nameof(execute));
            commandCanExecute = canExecute ?? (() => true);
            commandUnExecute = unExecute;
        }

        /// <summary>
        /// Возникает, когда происходят изменения, влияющие на возможность выполнения
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public event EventHandler ExecuteCommand;

        /// <summary>
        /// Определяет метод, который определяет, может ли команда выполняться в ее текущем состоянии
        /// </summary>
        /// <param name="parameter">
        /// Параметр, используемый командой
        /// </param>
        /// <returns>
        /// Возвращает значение, указывающее, может ли эта команда быть выполнена
        /// </returns>
        public bool CanExecute(object parameter = null)
        {
            try
            {
                return commandCanExecute();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Определяет метод, который будет вызываться при вызове команды
        /// </summary>
        /// <param name="parameter">
        /// Параметр, используемый командой
        /// </param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                commandExecuteAction(parameter);
                OnExecuteCommand();
            }
            catch
            {
                Debugger.Break();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExecuteCommand()
        {
            ExecuteCommand?.Invoke(this, EventArgs.Empty);
        }
    }
}
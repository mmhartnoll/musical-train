using System;
using System.Windows.Input;

namespace DisplayControl.ViewModels.Commands
{
    internal class RelayCommand<TParameter> : ICommand
    {
        private readonly Action<TParameter> execute;
        private readonly Func<TParameter, bool> canExecute;

        private bool isExecuting;

        private event EventHandler? ExecutionComplete;

        public event EventHandler? CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                ExecutionComplete += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                ExecutionComplete -= value;
            }
        }

        public RelayCommand(Action<TParameter> execute) : this(execute, _ => true) { }

        public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            return !isExecuting && canExecute.Invoke((TParameter)parameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));

            isExecuting = true;
            execute((TParameter)parameter);
            isExecuting = false;

            ExecutionComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}

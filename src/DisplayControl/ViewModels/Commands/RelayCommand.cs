using System;
using System.Windows.Input;

namespace DisplayControl.ViewModels.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

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

        public RelayCommand(Action execute) : this(execute, () => true) { }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) 
            => !isExecuting && canExecute.Invoke();

        public void Execute(object? parameter)
        {
            isExecuting = true;
            execute();
            isExecuting = false;

            ExecutionComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.Extensions;

namespace DisplayControl.ViewModels.Commands
{
    internal class AsyncRelayCommand : IAsyncCommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;
        private readonly Action<Exception> onError;
        private readonly Action? onCompletion;

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

        public AsyncRelayCommand(Func<Task> execute, Action<Exception> onError, Action? onCompletion = null)
            : this(execute, () => true, onError, onCompletion) { }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, Action<Exception> onError, Action? onCompletion = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.onError = onError;
            this.onCompletion = onCompletion;
        }

        public bool CanExecute()
            => !isExecuting && canExecute.Invoke(); 

        public bool CanExecute(object? parameter)
            => CanExecute();

        public void Execute(object? parameter)
            => ExecuteAsync().WithExceptionHandler(onError);

        public async Task ExecuteAsync()
        {
            if (CanExecute())
                try
                {
                    isExecuting = true;
                }
                finally
                {
                    await execute();
                    isExecuting = false;
                }
            ExecutionComplete?.Invoke(this, EventArgs.Empty);
            onCompletion?.Invoke();
        }
    }
}
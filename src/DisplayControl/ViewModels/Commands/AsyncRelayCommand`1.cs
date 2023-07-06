using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.Extensions;

namespace DisplayControl.ViewModels.Commands
{
    internal class AsyncRelayCommand<TParameter> : IAsyncCommand<TParameter>
    {
        private readonly Func<TParameter, Task> execute;
        private readonly Func<TParameter, bool> canExecute;
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

        public AsyncRelayCommand(Func<TParameter, Task> execute, Action<Exception> onError, Action? onCompletion = null)
            : this(execute, (_) => true, onError, onCompletion) { }

        public AsyncRelayCommand(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute, Action<Exception> onError, Action? onCompletion = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.onError = onError;
            this.onCompletion = onCompletion;
        }

        public bool CanExecute(TParameter parameter)
            => !isExecuting && canExecute.Invoke(parameter);

        public bool CanExecute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            return CanExecute((TParameter)parameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter is null) throw new ArgumentNullException(nameof(parameter));
            ExecuteAsync((TParameter)parameter).WithExceptionHandler(onError);
        }

        public async Task ExecuteAsync(TParameter parameter)
        {
            if (CanExecute(parameter))
                try
                {
                    isExecuting = true;
                }
                finally
                {
                    await execute(parameter);
                    isExecuting = false;
                }
            ExecutionComplete?.Invoke(this, EventArgs.Empty);
            onCompletion?.Invoke();
        }
    }
}

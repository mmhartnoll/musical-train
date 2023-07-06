using DisplayControl.ViewModels.Commands;
using System;
using System.Windows.Input;

namespace DisplayControl.ViewModels
{
    internal class DialogViewModel : ViewModel
    {
        private readonly IViewController viewController;
        private readonly Action<DialogResult>? onDialogResult;

        public string Header { get; private init; }
        public string Message { get; private init; }
        public DialogButtons DialogButtons { get; private init; }
        public bool IsError { get; private init; }

        public ICommand DialogCommand { get; private init; }

        public DialogViewModel(IViewController viewController, string header, string message, DialogButtons dialogButtons, bool isError = false, Action<DialogResult>? onDialogResult = null)
        {
            this.viewController = viewController;
            this.onDialogResult = onDialogResult;
            Header = header;
            Message = message;
            DialogButtons = dialogButtons;
            IsError = isError;

            DialogCommand = new RelayCommand<DialogResult>(ExecuteDialogCommand);
        }

        private void ExecuteDialogCommand(DialogResult result)
        {
            viewController.CloseViewModel(this);
            onDialogResult?.Invoke(result);
        }
    }
}
using System;

namespace DisplayControl
{
    internal interface IDialogService
    {
        void DisplayDialog(string header, string message, DialogButtons buttons = DialogButtons.OK, Action<DialogResult>? onDialogResult = null);

        void DisplayError(string header, string message, DialogButtons buttons = DialogButtons.OK, Action<DialogResult>? onDialogResult = null);
    }
}
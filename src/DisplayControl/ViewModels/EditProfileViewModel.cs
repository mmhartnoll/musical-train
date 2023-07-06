using DisplayControl.ViewModels.Commands;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Exceptions;
using System;
using System.Windows.Input;

namespace DisplayControl.ViewModels
{
    internal class EditProfileViewModel : ViewModel
    {
        private readonly IViewController viewController;
        private readonly IDialogService dialogService;
        private readonly IAsyncCommandService<SaveProfileCommand> saveProfileService;
        private readonly Action onSuccess;

        public SaveProfileCommand Command { get; } = new();

        public ICommand CancelCommand { get; private init; }
        public ICommand SaveCommand { get; private init; }

        public EditProfileViewModel(IViewController viewController, IDialogService dialogService, IAsyncCommandService<SaveProfileCommand> saveProfileService, DisplayProfile displayProfile, Action onSuccess)
        {
            this.viewController = viewController;
            this.dialogService = dialogService;
            this.saveProfileService = saveProfileService;
            this.onSuccess = onSuccess;

            Command.ProfileName = displayProfile.Name;
            Command.DisplayConfigurations = displayProfile.DisplayConfigurations;

            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand, () => Command.Errors.Count == 0);
        }

        private void ExecuteCancelCommand()
            => viewController.CloseViewModel(this);

        private void ExecuteSaveCommand()
        {
            dialogService.DisplayDialog("Confirmation", $"Save current profile as '{Command.ProfileName}'?", DialogButtons.YesNo, OnConfirmationResult);

            void OnConfirmationResult(DialogResult result)
            {
                if (result == DialogResult.Yes)
                    try
                    {
                        saveProfileService.Execute(Command);
                        dialogService.DisplayDialog("Success", $"Profile '{Command.ProfileName}' saved successfully.", DialogButtons.OK, (_) => 
                        {
                            onSuccess.Invoke();
                            viewController.CloseViewModel(this);
                        });
                    }
                    catch (UniqueKeyViolationException)
                    {
                        dialogService.DisplayDialog("Confirmation", "Overwrite existing profile?", DialogButtons.YesNo, (DialogResult result) =>
                        {
                            if (result == DialogResult.Yes)
                            {
                                Command.AllowOverwrite = true;
                                OnConfirmationResult(result);
                            }
                        });
                    }
                    catch (Exception)
                    {
                        dialogService.DisplayError("Failed", $"Failed to save profile '{Command.ProfileName}'", DialogButtons.OK);
                    }
            }
        }
    }
}
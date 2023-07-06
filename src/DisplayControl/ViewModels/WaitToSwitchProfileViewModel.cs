using DisplayControl.ViewModels.Commands;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Enumerations;
using DomainModel.Queries;
using DomainModel.QueryServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools.Extensions;

namespace DisplayControl.ViewModels
{
    internal class WaitToSwitchProfileViewModel : ViewModel
    {
        private readonly IViewController viewController;
        private readonly IEventManager<EventHandler> onDisplayChangedEventManager;
        private readonly IDialogService dialogService;
        private readonly IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>> getAvailableDisplayTargetsService;
        private readonly IAsyncCommandService<SwitchProfileCommand> switchProfileService;
        private readonly Action onSuccess;

        private DisplayProfile displayProfile;

        public DisplayProfile DisplayProfile
        {
            get => displayProfile;
            private set => SetProperty(ref displayProfile, value, nameof(DisplayProfile));
        }

        public ICommand CancelCommand { get; private init; }

        public WaitToSwitchProfileViewModel(
            IViewController viewController,
            IEventManager<EventHandler> onDisplayChangedEventManager,
            IDialogService dialogService,
            IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>> getAvailableDisplayTargetsService,
            IAsyncCommandService<SwitchProfileCommand> switchProfileService, 
            DisplayProfile displayProfile, 
            Action onSuccess)
        {
            this.viewController = viewController;
            this.onDisplayChangedEventManager = onDisplayChangedEventManager;
            this.dialogService = dialogService;
            this.getAvailableDisplayTargetsService = getAvailableDisplayTargetsService;
            this.switchProfileService = switchProfileService;
            this.displayProfile = displayProfile;
            this.onSuccess = onSuccess;

            this.onDisplayChangedEventManager.Subscribe(OnDisplayChanged);

            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void OnDisplayChanged(object? sender, EventArgs e)
        {
            RefreshProfileAndExecuteSwitch().WithExceptionHandler(OnUnexpectedException);

            void OnUnexpectedException(Exception ex)
                => dialogService.DisplayError("Unexpected Error", $"An unexpected error occured.\n{ex.Message}", DialogButtons.OK);
        }

        private async Task RefreshProfileAndExecuteSwitch()
        {
            IEnumerable<DisplayTarget> displayTargets = await getAvailableDisplayTargetsService.ExecuteAsync(new()).ConfigureAwait(false);
            DisplayProfile = DisplayProfile.WithTargetAvailabilityStatus(config =>
                config.WithTargetAvailabilityStatus(
                    displayTargets.Any(target => target.IsAvailable && target.Id == config.TargetId && target.DisplayName == config.DisplayName), true));

            if (DisplayProfile.StatusFlags.HasFlag(DisplayProfileStatusFlags.Available))
            {
                await switchProfileService.ExecuteAsync(new() { DisplayProfile = DisplayProfile });
                ExecuteCancelCommand();
            }
        }

        private void ExecuteCancelCommand()
        {
            viewController.CloseViewModel(this);
            onDisplayChangedEventManager.Unsubscribe(OnDisplayChanged);
        }
    }
}
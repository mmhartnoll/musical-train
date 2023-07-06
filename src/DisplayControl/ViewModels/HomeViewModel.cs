using DisplayControl.ViewModels.Commands;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Enumerations;
using DomainModel.Queries;
using DomainModel.QueryServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DisplayControl.ViewModels
{
    internal class HomeViewModel : ViewModel
    {
        private readonly IViewController viewController;
        private readonly IEventManager<EventHandler> onDisplayChangedEventManager;
        private readonly IDialogService dialogService;

        private readonly AppSettings appSettings;

        private readonly ICommandService<SaveAppSettingsCommand> saveAppSettingsService;

        private readonly IAsyncQueryService<GetActiveDisplayConfiguationsQuery, IEnumerable<DisplayConfiguration>> getActiveDisplayConfigurationsService;
        private readonly IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>> getAvailableDisplayTargetsService;
        private readonly IAsyncQueryService<GetSavedDisplayProfilesQuery, IEnumerable<DisplayProfile>> getSavedDisplayProfilesService;

        private readonly IAsyncCommandService<SwitchProfileCommand> switchProfileService;

        private bool isBusy = false;

        private ICollectionView displayProfiles = CollectionViewSource.GetDefaultView(new ObservableCollection<DisplayProfile>());

        public bool IsBusy
        {
            get => isBusy;
            private set => SetProperty(ref isBusy, value, nameof(IsBusy));
        }

        public AppSettings AppSettings { get; private init; }

        public ICollectionView DisplayProfiles
        {
            get => displayProfiles;
            private set => SetProperty(ref displayProfiles, value, nameof(DisplayProfiles));
        }

        public ICommand SelectPreviousProfileCommand { get; private init; }
        public ICommand SelectNextProfileCommand { get; private init; }

        public ICommand ToggleShowOnlyAvailableProfilesCommand { get; private init; }
        public ICommand SaveAppSettingsCommand { get; private init; }

        public ICommand RefreshCommand { get; private init; }
        public ICommand EditProfileCommand { get; private init; }
        public ICommand SwitchProfileCommand { get; private init; }
        public ICommand WaitToSwitchProfileCommand { get; private init; }

        public HomeViewModel(
            IViewController viewController,
            IEventManager<EventHandler> onDisplayChangedEventManager,
            IDialogService dialogService,
            AppSettings appSettings,
            ICommandService<SaveAppSettingsCommand> saveAppSettingsService,
            IAsyncQueryService<GetActiveDisplayConfiguationsQuery, IEnumerable<DisplayConfiguration>> getActiveDisplayConfigurationsService,
            IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>> getAvailableDisplayTargetsService,
            IAsyncQueryService<GetSavedDisplayProfilesQuery, IEnumerable<DisplayProfile>> getSavedDisplayProfilesService,
            IAsyncCommandService<SwitchProfileCommand> switchProfileService)
        {
            this.viewController = viewController;
            this.onDisplayChangedEventManager = onDisplayChangedEventManager;
            this.dialogService = dialogService;
            AppSettings = appSettings;
            this.saveAppSettingsService = saveAppSettingsService;
            this.getActiveDisplayConfigurationsService = getActiveDisplayConfigurationsService;
            this.getAvailableDisplayTargetsService = getAvailableDisplayTargetsService;
            this.getSavedDisplayProfilesService = getSavedDisplayProfilesService;
            this.switchProfileService = switchProfileService;

            SelectPreviousProfileCommand = new RelayCommand(ExecuteSelectPreviousProfileCommand, CanExecuteSelectPreviousProfileCommand);
            SelectNextProfileCommand = new RelayCommand(ExecuteSelectNextProfileCommand, CanExecuteSelectNextProfileCommand);

            ToggleShowOnlyAvailableProfilesCommand = new RelayCommand(ExecuteToggleShowOnlyAvailableProfilesCommand, () => !IsBusy);
            SaveAppSettingsCommand = new RelayCommand(ExecuteSaveAppSettingsCommand, () => !IsBusy);

            RefreshCommand = new AsyncRelayCommand(ExecuteRefresh, CanExecuteRefresh, OnUnexpectedException, InvalidateRequerySuggested);
            EditProfileCommand = new RelayCommand(ExecuteEditProfileCommand, CanExecuteEditProfileCommand);
            SwitchProfileCommand = new AsyncRelayCommand(ExecuteSwitchProfileCommand, CanExecuteSwitchProfileCommand, OnSwitchProfileException, InvalidateRequerySuggested);
            WaitToSwitchProfileCommand = new RelayCommand(ExecuteWaitToSwitchProfileCommand, CanExecuteWaitToSwitchProfileCommand);

            this.onDisplayChangedEventManager.Subscribe(OnDisplayChanged);

            RefreshCommand.Execute(null);

            void OnDisplayChanged(object? sender, EventArgs e)
                => RefreshCommand.Execute(null);
        }

        private async Task ExecuteRefresh()
        {
            IsBusy = true;

            Task<IEnumerable<DisplayConfiguration>> task1 = getActiveDisplayConfigurationsService.ExecuteAsync(new());
            Task<IEnumerable<DisplayTarget>> task2 = getAvailableDisplayTargetsService.ExecuteAsync(new());
            Task<IEnumerable<DisplayProfile>> task3 = getSavedDisplayProfilesService.ExecuteAsync(new());

            IEnumerable<DisplayConfiguration> activeDisplayConfigurations = (await task1.ConfigureAwait(false)).ToList();
            IEnumerable<DisplayTarget> displayTargets = (await task2.ConfigureAwait(false)).ToList();
            IList<DisplayProfile> displayProfiles = (await task3.ConfigureAwait(false)).ToList();

            displayProfiles = displayProfiles
                .Select(profile => profile.HasMatchngDisplayConfigurations(activeDisplayConfigurations) ? profile.AsActiveProfile() : profile)
                .Select(profile =>
                    profile.WithTargetAvailabilityStatus(
                        config => config.WithTargetAvailabilityStatus(
                            displayTargets.Any(target => target.IsAvailable && target.Id == config.TargetId && target.DisplayName == config.DisplayName))))
                .ToList();

            if (!displayProfiles.Any(profile => profile.StatusFlags.HasFlag(DisplayProfileStatusFlags.Active)))
            {
                DisplayProfile unsavedDisplayProfile = new DisplayProfile("Unsaved Profile", activeDisplayConfigurations, false)
                    .WithTargetAvailabilityStatus(config => config);

                displayProfiles.Add(unsavedDisplayProfile);
            }

            IOrderedEnumerable<DisplayProfile> orderedDisplayProfiles = displayProfiles
               .OrderByDescending(profile => profile.StatusFlags.HasFlag(DisplayProfileStatusFlags.Active))
               .ThenBy(profile => profile.Name);

            DisplayProfiles = CollectionViewSource.GetDefaultView(new ObservableCollection<DisplayProfile>(orderedDisplayProfiles));
            DisplayProfiles.Filter = FilterDisplayProfiles;

            IsBusy = false;
        }

        private bool CanExecuteRefresh()
            => !IsBusy;

        private void ExecuteSelectPreviousProfileCommand()
        {
            IsBusy = true;
            DisplayProfiles.MoveCurrentToPrevious();
            IsBusy = false;
        }

        private bool CanExecuteSelectPreviousProfileCommand()
            => !IsBusy && DisplayProfiles.CurrentPosition > 0;

        private void ExecuteSelectNextProfileCommand()
        {
            IsBusy = true;
            DisplayProfiles.MoveCurrentToNext();
            IsBusy = false;
        }

        private bool CanExecuteSelectNextProfileCommand()
            => !IsBusy && DisplayProfiles.CurrentPosition < DisplayProfiles.Cast<object>().Count() - 1;

        private void ExecuteToggleShowOnlyAvailableProfilesCommand()
        {
            SaveAppSettingsCommand.Execute(null);
            DisplayProfiles.Refresh();
        }

        private void ExecuteSaveAppSettingsCommand()
            => saveAppSettingsService.Execute(new() { AppSettings = AppSettings });

        private void OnUnexpectedException(Exception ex)
            => dialogService.DisplayError("Unexpected Error", $"An unexpected error occured.\n{ex.Message}", DialogButtons.OK, (_) => IsBusy = false);

        private void ExecuteEditProfileCommand()
            => viewController.DisplayEditProfileViewModel((DisplayProfile)DisplayProfiles.CurrentItem, () => RefreshCommand.Execute(null));

        private bool CanExecuteEditProfileCommand()
            => !IsBusy;

        private async Task ExecuteSwitchProfileCommand()
        {
            IsBusy = true;

            await Task.Delay(200).ConfigureAwait(false);
            await switchProfileService.ExecuteAsync(new() { DisplayProfile = (DisplayProfile)DisplayProfiles.CurrentItem }).ConfigureAwait(false);
            await ExecuteRefresh().ConfigureAwait(false);
        }

        private bool CanExecuteSwitchProfileCommand()
            => !IsBusy;

        private void OnSwitchProfileException(Exception ex)
            => dialogService.DisplayError("Failed to Switch Display Profile", ex.Message, DialogButtons.OK, (_) => IsBusy = false);

        private void ExecuteWaitToSwitchProfileCommand()
            => viewController.DisplayWaitToSwitchProfileViewModel((DisplayProfile)DisplayProfiles.CurrentItem, () => RefreshCommand.Execute(null));

        private bool CanExecuteWaitToSwitchProfileCommand()
            => !IsBusy;

        private void InvalidateRequerySuggested()
            => Dispatcher.CurrentDispatcher.Invoke(CommandManager.InvalidateRequerySuggested);

        private bool FilterDisplayProfiles(object value)
            => !AppSettings.ShowOnlyAvailableProfiles || ((DisplayProfile)value).StatusFlags.HasFlag(DisplayProfileStatusFlags.Available);
    }
}
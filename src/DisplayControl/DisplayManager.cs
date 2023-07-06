using DisplayControl.ViewModels;
using DomainModel.Entities;
using DomainModel.Enumerations;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DisplayControl
{
    internal class DisplayManager : IViewController, IEventManager<EventHandler>, IDialogService
    {
        private readonly CompositionManager compositionManager;
        private readonly Window window = new();
        private readonly NotifyIcon systemTrayIcon = new();
        private readonly AppSettings appSettings;
        private readonly ILoggingService loggingService;

        public event EventHandler? DisplayChanged;

        public DisplayManager(CompositionManager compositionManager)
        {
            this.compositionManager = compositionManager;
            
            appSettings = compositionManager
                .GetGetSavedAppSettingsService()
                .Execute(new());

            loggingService = compositionManager.LoggingService;
        }

        public void StartDisplay()
        {
            window.Title = "Display Control";
            window.Icon = BitmapFrame.Create(new Uri("C:\\Users\\mmhar\\Downloads\\icons8-monitor-96.ico"));
            window.Width = 800;
            window.Height = 600;
            window.StateChanged += WindowStateChanged;

            systemTrayIcon.Icon = new("C:\\Users\\mmhar\\Downloads\\icons8-monitor-96.ico");
            systemTrayIcon.MouseClick += SystemTrayIconClick;

            DisplayHomeViewModel();
            window.Show();

            if (appSettings.StartMinimized)
                window.WindowState = WindowState.Minimized;

            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            source.AddHook(WndProc);
        }

        public void CloseViewModel(ViewModel viewModel)
        {
            if (window.Content is not ViewModelFrame frame || frame.Current != viewModel)
            {
                string logMessage = "Current content is not the expected viewModel.";
                if (window.Content is not ViewModel currentViewModel)
                    logMessage += $"\nWindow content is not a view model. Content type is '{window.Content?.GetType().Name ?? null}'.";
                else
                {
                    logMessage += $"\nExpected '{viewModel.GetType().Name}' at top of frame stack.";
                    logMessage += $"\nActual frame stack:";
                    while (currentViewModel is ViewModelFrame currentFrame)
                    {
                        logMessage += $"\n\t{currentFrame.Current.GetType().Name}";
                        currentViewModel = currentFrame.Previous;
                    }
                    logMessage += $"\n\t{currentViewModel.GetType().Name}";
                }
                loggingService.LogAsync(LogLevel.Error, logMessage);
                throw new Exception("Current content is not the expected dialog.");
            }
            window.Content = frame.Previous;
        }

        public void DisplayHomeViewModel()
            => DisplayViewModel(
                new HomeViewModel(this, this, this,
                    appSettings,
                    compositionManager.GetSaveAppSettingsService(),
                    compositionManager.GetGetActiveDisplayConfigurationsService(),
                    compositionManager.GetGetAvailableDisplayTargetsService(),
                    compositionManager.GetGetAllProfilesService(),
                    compositionManager.GetSwitchProfileService()));

        public void DisplayEditProfileViewModel(DisplayProfile displayProfile, Action onSuccess)
            => DisplayViewModel(
                new EditProfileViewModel(this, this,
                    compositionManager.GetSaveProfileService(),
                    displayProfile,
                    onSuccess));

        public void DisplayWaitToSwitchProfileViewModel(DisplayProfile displayProfile, Action onSuccess)
            => DisplayViewModel(
                new WaitToSwitchProfileViewModel(this, this, this,
                    compositionManager.GetGetAvailableDisplayTargetsService(),
                    compositionManager.GetSwitchProfileService(),
                    displayProfile,
                    onSuccess));

        public void Subscribe(EventHandler handler)
            => DisplayChanged += handler;

        public void Unsubscribe(EventHandler handler)
            => DisplayChanged -= handler;

        public void DisplayDialog(string header, string message, DialogButtons buttons = DialogButtons.OK, Action<DialogResult>? onDialogResult = null)
            => DisplayViewModel(
                new DialogViewModel(this, header, message, buttons, false, onDialogResult));

        public void DisplayError(string header, string message, DialogButtons buttons = DialogButtons.OK, Action<DialogResult>? onDialogResult = null)
            => DisplayViewModel(
                new DialogViewModel(this, header, message, buttons, true, onDialogResult));

        private void DisplayViewModel(ViewModel viewModel)
        {
            if (window.Content is null)
                window.Content = viewModel;
            else
                window.Content = new ViewModelFrame(viewModel, (ViewModel)window.Content);
        }

        private void WindowStateChanged(object? sender, EventArgs args)
        {
            if (appSettings.MinimizeToTray)
                if (window.WindowState == WindowState.Minimized)
                {
                    systemTrayIcon.Visible = true;
                    window.ShowInTaskbar = false;
                }
                else if (window.WindowState == WindowState.Normal)
                {
                    systemTrayIcon.Visible = false;
                    window.ShowInTaskbar = true;
                }
        }

        private void SystemTrayIconClick(object? sender, EventArgs args)
        {
            window.WindowState = WindowState.Normal;
            window.Activate();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WmDisplayChange || msg == WmDeviceChange)
                DisplayChanged?.Invoke(this, new());

            return IntPtr.Zero;
        }

        private const int WmDisplayChange = 0x007e;
        private const int WmDeviceChange = 0x0219;
    }
}
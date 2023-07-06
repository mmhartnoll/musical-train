namespace DomainModel.Entities
{
    public class AppSettings : Entity
    {
        private bool showOnlyAvailableProfiles = true;
        private bool minimizeToTray = false;
        private bool startMinimized = false;

        public bool ShowOnlyAvailableProfiles
        {
            get => showOnlyAvailableProfiles;
            set => SetProperty(ref showOnlyAvailableProfiles, value, nameof(ShowOnlyAvailableProfiles));
        }

        public bool MinimizeToTray
        {
            get => minimizeToTray;
            set => SetProperty(ref minimizeToTray, value, nameof(MinimizeToTray));
        }

        public bool StartMinimized
        {
            get => startMinimized;
            set => SetProperty(ref startMinimized, value, nameof(StartMinimized));
        }
    }
}

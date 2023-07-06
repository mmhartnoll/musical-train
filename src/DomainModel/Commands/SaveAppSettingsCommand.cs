using DomainModel.Entities;

namespace DomainModel.Commands
{
    public class SaveAppSettingsCommand : ServiceCommand
    {
        private AppSettings appSettings = new();

        public AppSettings AppSettings
        {
            get => appSettings;
            set => SetProperty(ref appSettings, value, nameof(AppSettings));
        }
    }
}
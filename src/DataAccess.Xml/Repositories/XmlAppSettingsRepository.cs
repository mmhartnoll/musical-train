using DomainModel.Entities;
using DomainModel.Repositories;
using System.Xml.Linq;
using Tools.Extensions;

namespace DataAccess.Xml.Repositories
{
    public class XmlAppSettingsRepository : IAppSettingsRepository
    {
        private readonly XmlConfigurationFileFactory factory;

        public XmlAppSettingsRepository(XmlConfigurationFileFactory factory)
            => this.factory = factory;

        public AppSettings Get()
        {
            using XmlConfigurationFile configurationFile = factory.LoadConfigurationFile();
            XElement settingsElement = configurationFile.SettingsElement;

            AppSettings appSettings = new();

            if (settingsElement.TryGetAttributeValue(ShowOnlyAvailableProfiles, out bool showOnlyAvailableProfiles))
                appSettings.ShowOnlyAvailableProfiles = showOnlyAvailableProfiles;

            if (settingsElement.TryGetAttributeValue(MinimizeToTray, out bool minimizeToTray))
                appSettings.MinimizeToTray = minimizeToTray;

            if (settingsElement.TryGetAttributeValue(StartMinimized, out bool startMinimized))
                appSettings.StartMinimized = startMinimized;

            return appSettings;
        }

        public void Save(AppSettings appSettings)
        {
            using XmlConfigurationFile configurationFile = factory.LoadConfigurationFile();
            XElement settingsElement = configurationFile.SettingsElement;

            settingsElement.RemoveAttributes();
            settingsElement.SetAttributeValue(ShowOnlyAvailableProfiles, appSettings.ShowOnlyAvailableProfiles);
            settingsElement.SetAttributeValue(MinimizeToTray, appSettings.MinimizeToTray);
            settingsElement.SetAttributeValue(StartMinimized, appSettings.StartMinimized);

            configurationFile.SaveChanges();
        }

        private const string ShowOnlyAvailableProfiles = "showOnlyAvailableProfiles";
        private const string MinimizeToTray = "minimizeToTray";
        private const string StartMinimized = "startMinimized";
    }
}

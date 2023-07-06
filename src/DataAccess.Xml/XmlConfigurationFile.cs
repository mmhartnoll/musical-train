using System.Xml.Linq;

namespace DataAccess.Xml
{
    public class XmlConfigurationFile : IDisposable
    {
        private readonly XDocument document;
        private readonly Action<XDocument> saveMethod;

        private readonly Lazy<XElement> configurationElementLoader;
        private readonly Lazy<XElement> settingsElementLoader;
        private readonly Lazy<XElement> profilesElementLoader;

        private bool isDisposed = false;

        public XDocument Document => ThrowExceptionIfDisposed().document;
        public XElement ConfigurationElement => ThrowExceptionIfDisposed().configurationElementLoader.Value;
        public XElement SettingsElement => ThrowExceptionIfDisposed().settingsElementLoader.Value;
        public XElement ProfilesElement => ThrowExceptionIfDisposed().profilesElementLoader.Value;

        internal XmlConfigurationFile(XDocument document, Action<XDocument> saveMethod)
        {
            this.document = document;
            this.saveMethod = saveMethod;

            configurationElementLoader = new Lazy<XElement>(LoadConfigurationElement);
            settingsElementLoader = new Lazy<XElement>(LoadSettingsElement);
            profilesElementLoader = new Lazy<XElement>(LoadProfilesElement);
        }

        private XElement LoadConfigurationElement()
        {
            XElement? configurationElement = Document.Element("configuration");
            if (configurationElement is null)
            {
                configurationElement = new XElement("configuration");
                Document.Add(configurationElement);
            }
            return configurationElement;
        }

        public XElement LoadSettingsElement()
        {
            XElement configurationElement = ConfigurationElement;
            XElement? settingsElement = configurationElement.Element("settings");
            if (settingsElement is null)
            {
                settingsElement = new XElement("settings");
                configurationElement.Add(settingsElement);
            }
            return settingsElement;
        }

        public XElement LoadProfilesElement()
        {
            XElement configurationElement = ConfigurationElement;
            XElement? profilesElement = configurationElement.Element("profiles");
            if (profilesElement is null)
            {
                profilesElement = new XElement("profiles");
                configurationElement.Add(profilesElement);
            }
            return profilesElement;
        }

        public void SaveChanges()
            => saveMethod(Document);

        public void Dispose()
        {
            ThrowExceptionIfDisposed();
            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        protected XmlConfigurationFile ThrowExceptionIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            return this;
        }
    }
}
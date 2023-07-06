using System.Xml.Linq;

namespace DataAccess.Xml
{
    public class XmlConfigurationFileFactory
    {
        private readonly string configurationFilePath;

        public XmlConfigurationFileFactory(string configurationFilePath)
            => this.configurationFilePath = configurationFilePath;

        public XmlConfigurationFile LoadConfigurationFile()
        {
            XDocument document;
            try
            {
                document = XDocument.Load(configurationFilePath);
            }
            catch (FileNotFoundException)
            {
                document = new();
            }
            return new(document, Save);

            void Save(XDocument document) => document.Save(configurationFilePath, SaveOptions.None);
        }
    }
}
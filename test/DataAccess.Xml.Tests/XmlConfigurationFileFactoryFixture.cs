namespace DataAccess.Xml.Tests
{
    public class XmlConfigurationFileFactoryFixture : ITestFixture<XmlConfigurationFileFactory>, IDisposable
    {
        public XmlConfigurationFileFactoryFixture() { }

        public XmlConfigurationFileFactory CreateContext()
            => CreateContext(DefaultConfigurationTestFileName);

        public XmlConfigurationFileFactory CreateContext(string configurationFileName)
            => new(Path.Join(ConfigurationTestFileDirectory, configurationFileName));

        public XmlConfigurationFileFactory CreateContextAsCopy()
            => CreateContextAsCopy(DefaultConfigurationTestFileName);

        public XmlConfigurationFileFactory CreateContextAsCopy(string configurationFileName)
        {
            string existingFilePath = Path.Join(ConfigurationTestFileDirectory, configurationFileName);
            string tempDirectoryPath = Path.Join(ConfigurationTestFileDirectory, "Temp");

            if (!Directory.Exists(tempDirectoryPath))
                Directory.CreateDirectory(tempDirectoryPath);

            Guid id = Guid.NewGuid();
            string tempFilePath = Path.Join(tempDirectoryPath, $"{id}_{configurationFileName}");

            File.Copy(existingFilePath, tempFilePath);

            return new(tempFilePath);
        }

        public void Dispose()
        {
            string tempDirectoryPath = Path.Join(ConfigurationTestFileDirectory, "Temp");

            if (Directory.Exists(tempDirectoryPath))
                Directory.Delete(tempDirectoryPath, true);
        }

        private const string ConfigurationTestFileDirectory = @"..\..\..\SeedConfigurationFiles\";
        private const string DefaultConfigurationTestFileName = @"DefaultConfigurationFileFixture.xml";
    }

    [CollectionDefinition("XmlConfigurationFileFactory")]
    public class XmlConfigurationFileFactoryCollection : ICollectionFixture<XmlConfigurationFileFactoryFixture> { }
}
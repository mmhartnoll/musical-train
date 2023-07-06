using DataAccess.Xml.Repositories;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Repositories;
using FluentAssertions;
using System.Xml.Linq;

namespace DataAccess.Xml.Tests.CommandServices
{
    [Collection("XmlConfigurationFileFactory")]
    public class SaveAppSettingsServiceTests : IntegrationTest<XmlConfigurationFileFactory, XmlConfigurationFileFactoryFixture> 
    {
        public SaveAppSettingsServiceTests(XmlConfigurationFileFactoryFixture fixture)
            : base(fixture) { }

        [Fact]
        public void Command_should_create_new_settings_on_empty_configuration()
        {
            // Assert
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy(configurationFilePath);
            IAppSettingsRepository repository = new XmlAppSettingsRepository(context);
            ICommandService<SaveAppSettingsCommand> sut = new SaveAppSettingsService(repository);

            // Act
            AppSettings appSettings = new();
            sut.Execute(new() { AppSettings = appSettings });

            // Assert
            XDocument document = context.LoadConfigurationFile().Document;
            AssertCorrectSavedAppSettings(appSettings, document);
        }

        [Fact]
        public void Command_should_overwrite_existing_values()
        {
            // Assert
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy();
            IAppSettingsRepository repository = new XmlAppSettingsRepository(context);
            ICommandService<SaveAppSettingsCommand> sut = new SaveAppSettingsService(repository);

            // Act
            AppSettings appSettings = new()
            {
                ShowOnlyAvailableProfiles = true,
                MinimizeToTray = false,
                StartMinimized = false
            };
            sut.Execute(new() { AppSettings = appSettings });

            // Assert
            XDocument document = context.LoadConfigurationFile().Document;
            AssertCorrectSavedAppSettings(appSettings, document);
        }

        private static void AssertCorrectSavedAppSettings(AppSettings appSettings, XDocument document)
        {
            XElement? configurationElement = document.Element("configuration");
            configurationElement.Should().NotBeNull();
            if (configurationElement is null)
                return;

            XElement? settingsElement = configurationElement.Element("settings");
            settingsElement.Should().NotBeNull();
            if (settingsElement is null)
                return;

            settingsElement.Attributes().Count().Should().Be(3);

            AssertCorrectAttribute(settingsElement, "showOnlyAvailableProfiles", appSettings.ShowOnlyAvailableProfiles);
            AssertCorrectAttribute(settingsElement, "minimizeToTray", appSettings.MinimizeToTray);
            AssertCorrectAttribute(settingsElement, "startMinimized", appSettings.StartMinimized);

            static void AssertCorrectAttribute<T>(XElement element, string attributeName, T expectedValue)
            {
                XAttribute? attribute = element.Attribute(attributeName);
                attribute.Should().NotBeNull();
                if (attribute is null)
                    return;

                T attributeValue = (T)Convert.ChangeType(attribute.Value, typeof(T));

                attributeValue.Should().Be(expectedValue);
            }
        }
    }
}
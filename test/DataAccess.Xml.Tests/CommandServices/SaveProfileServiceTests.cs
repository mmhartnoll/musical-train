using DataAccess.Xml.Repositories;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Exceptions;
using DomainModel.Repositories;
using FluentAssertions;
using System.Xml.Linq;

namespace DataAccess.Xml.Tests.CommandServices
{
    [Collection("XmlConfigurationFileFactory")]
    public class SaveProfileServiceTests : IntegrationTest<XmlConfigurationFileFactory, XmlConfigurationFileFactoryFixture>
    {
        public SaveProfileServiceTests(XmlConfigurationFileFactoryFixture fixture)
            : base(fixture) { }

        [Fact]
        public void Command_should_create_new_profile_when_non_existing()
        {
            // Assert
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy(configurationFilePath);
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            IList<DisplayConfiguration> displayConfigurations = new List<DisplayConfiguration>
            {
                new(0, "Device #1", 10_000, "Display #1", 2560, 1440, 0, 0, 120, 1, true, false, false),
                new(1, "Device #2", 10_001, "Display #2", 3840, 2160, 2560, 0, 144, 1, true, true, false)
            };
            SaveProfileCommand command = new()
            {
                ProfileName = "New Profile",
                DisplayConfigurations = displayConfigurations
            };
            sut.Execute(command);

            // Assert
            XDocument document = context.LoadConfigurationFile().Document;
            AssertCorrectProfileSettings("New Profile", displayConfigurations, document);
        }

        [Fact]
        public void Command_should_overwrite_existing_profile_when_specified()
        {
            // Assert
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy();
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            IList<DisplayConfiguration> displayConfigurations = new List<DisplayConfiguration>
            {
                new(0, "Device #1", 10_000, "Display #1", 2560, 1440, 0, 0, 120, 1, true, false, false),
                new(1, "Device #2", 10_001, "Display #2", 3840, 2160, 2560, 0, 144, 1, true, true, false)
            };
            SaveProfileCommand command = new()
            {
                ProfileName = "Desktop",
                DisplayConfigurations = displayConfigurations,
                AllowOverwrite = true
            };
            sut.Execute(command);

            // Assert
            XDocument document = context.LoadConfigurationFile().Document;
            AssertCorrectProfileSettings("Desktop", displayConfigurations, document);
        }

        [Fact]
        public void Command_should_not_overwrite_existing_profile_when_not_specified()
        {
            // Assert
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy();
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            IList<DisplayConfiguration> displayConfigurations = new List<DisplayConfiguration>
            {
                new(0, "Device #1", 10_000, "Display #1", 2560, 1440, 0, 0, 120, 1, true, false, false),
                new(1, "Device #2", 10_001, "Display #2", 3840, 2160, 2560, 0, 144, 1, true, true, false)
            };
            SaveProfileCommand command = new()
            {
                ProfileName = "Desktop",
                DisplayConfigurations = displayConfigurations
            };
            Action Act = () => sut.Execute(command);

            // Assert
            Act.Should().Throw<UniqueKeyViolationException>()
                .Where(ex =>
                    ex.FieldValues.Count() == 1 &&
                    ex.FieldValues.ContainsKey("Name"));
        }

        [Fact]
        public void Command_should_fail_if_profile_name_is_blank()
        {
            // Assert
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy(configurationFilePath);
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            IList<DisplayConfiguration> displayConfigurations = new List<DisplayConfiguration>
            {
                new(0, "Device #1", 10_000, "Display #1", 2560, 1440, 0, 0, 120, 1, true, false, false),
                new(1, "Device #2", 10_001, "Display #2", 3840, 2160, 2560, 0, 144, 1, true, true, false)
            };
            SaveProfileCommand command = new()
            {
                ProfileName = string.Empty,
                DisplayConfigurations = displayConfigurations
            };
            Action Act = () => sut.Execute(command);

            // Assert
            Act.Should().Throw<InvalidCommandQueryException>()
                .Where(ex =>
                    ex.FieldErrors.Count() == 1 &&
                    ex.FieldErrors.ContainsKey(nameof(SaveProfileCommand.ProfileName)));
        }

        [Fact]
        public void Command_should_fail_if_profile_name_is_too_long()
        {
            // Assert
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy(configurationFilePath);
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            IList<DisplayConfiguration> displayConfigurations = new List<DisplayConfiguration>
            {
                new(0, "Device #1", 10_000, "Display #1", 2560, 1440, 0, 0, 120, 1, true, false, false),
                new(1, "Device #2", 10_001, "Display #2", 3840, 2160, 2560, 0, 144, 1, true, true, false)
            };
            SaveProfileCommand command = new()
            {
                ProfileName = new string('a', 25),
                DisplayConfigurations = displayConfigurations
            };
            Action Act = () => sut.Execute(command);

            // Assert
            Act.Should().Throw<InvalidCommandQueryException>()
                .Where(ex =>
                    ex.FieldErrors.Count() == 1 &&
                    ex.FieldErrors.ContainsKey(nameof(SaveProfileCommand.ProfileName)));
        }

        [Fact]
        public void Command_should_fail_if_profile_has_no_display_configurations()
        {
            // Assert
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContextAsCopy(configurationFilePath);
            IProfileRepository repository = new XmlProfileRepository(context);
            ICommandService<SaveProfileCommand> sut = new SaveProfileService(repository);

            // Act
            SaveProfileCommand command = new()
            {
                ProfileName = "Empty Profile"
            };
            Action Act = () => sut.Execute(command);

            // Assert
            Act.Should().Throw<InvalidCommandQueryException>()
                .Where(ex =>
                    ex.FieldErrors.Count() == 1 &&
                    ex.FieldErrors.ContainsKey(nameof(SaveProfileCommand.DisplayConfigurations)));
        }


        private static void AssertCorrectProfileSettings(string profileName, IEnumerable<DisplayConfiguration> displayConfigurations, XDocument document)
        {
            XElement? configurationElement = document.Element("configuration");
            configurationElement.Should().NotBeNull();
            if (configurationElement is null)
                return;

            XElement? profilesElement = configurationElement.Element("profiles");
            profilesElement.Should().NotBeNull();
            if (profilesElement is null)
                return;

            XElement? profileElement = profilesElement.Elements("profile")?
                .FirstOrDefault(profileElement => profileElement.Attribute("name")?.Value == profileName);
            profileElement.Should().NotBeNull();
            if (profileElement is null)
                return;

            XElement? displayConfigurationsElement = profileElement.Element("displayConfigurations");
            displayConfigurationsElement.Should().NotBeNull();
            if (displayConfigurationsElement is null)
                return;

            foreach (var displayConfiguration in displayConfigurations)
                AssertCorrectDisplayConfiguration(displayConfiguration, displayConfigurationsElement);

            static void AssertCorrectDisplayConfiguration(DisplayConfiguration displayConfiguration, XElement displayConfigurationsElement)
            {
                XElement? displayConfigurationElement = displayConfigurationsElement.Elements("displayConfiguration")?
                    .FirstOrDefault(displayConfigurationElement => displayConfigurationElement.Attribute("sourceId")?.Value == displayConfiguration.SourceId.ToString());
                displayConfigurationElement.Should().NotBeNull();
                if (displayConfigurationElement is null)
                    return;

                displayConfigurationElement.Attributes().Count().Should().Be(12);

                AssertCorrectAttribute(displayConfigurationElement, "sourceId", displayConfiguration.SourceId);
                AssertCorrectAttribute(displayConfigurationElement, "deviceName", displayConfiguration.DeviceName);
                AssertCorrectAttribute(displayConfigurationElement, "targetId", displayConfiguration.TargetId);
                AssertCorrectAttribute(displayConfigurationElement, "displayName", displayConfiguration.DisplayName);
                AssertCorrectAttribute(displayConfigurationElement, "resolutionWidth", displayConfiguration.ResolutionX);
                AssertCorrectAttribute(displayConfigurationElement, "resolutionHeight", displayConfiguration.ResolutionY);
                AssertCorrectAttribute(displayConfigurationElement, "positionX", displayConfiguration.PositionX);
                AssertCorrectAttribute(displayConfigurationElement, "positionY", displayConfiguration.PositionY);
                AssertCorrectAttribute(displayConfigurationElement, "vSyncNumerator", displayConfiguration.VSyncNumerator);
                AssertCorrectAttribute(displayConfigurationElement, "vSyncDenominator", displayConfiguration.VSyncDenominator);
                AssertCorrectAttribute(displayConfigurationElement, "isHdrSupported", displayConfiguration.IsHdrSupported);
                AssertCorrectAttribute(displayConfigurationElement, "isHdrEnabled", displayConfiguration.IsHdrEnabled);

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
}

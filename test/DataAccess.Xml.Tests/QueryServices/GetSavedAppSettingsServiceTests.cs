using DataAccess.Xml.Repositories;
using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.QueryServices;
using DomainModel.Repositories;
using FluentAssertions;

namespace DataAccess.Xml.Tests.QueryServices
{
    [Collection("XmlConfigurationFileFactory")]
    public class GetSavedAppSettingsServiceTests : IntegrationTest<XmlConfigurationFileFactory, XmlConfigurationFileFactoryFixture>
    {
        public GetSavedAppSettingsServiceTests(XmlConfigurationFileFactoryFixture fixture)
            : base(fixture) { }

        [Fact]
        public void Query_should_return_default_settings_on_empty_configuration_file()
        {
            // Arrange
            string configurationFilePath = @"EmptyConfigurationFileFixture.xml";
            XmlConfigurationFileFactory context = Fixture.CreateContext(configurationFilePath);
            IAppSettingsRepository repository = new XmlAppSettingsRepository(context);
            IQueryService<GetSavedAppSettingsQuery, AppSettings> sut = new GetSavedAppSettingsService(repository);

            // Act
            AppSettings result = sut.Execute(new());

            // Assert
            AppSettings defaultAppSettings = new();
            AssertCorrectAppSettings(result, defaultAppSettings);
        }


        [Fact]
        public void Query_returns_correct_app_settings()
        {
            // Arrange
            XmlConfigurationFileFactory context = Fixture.CreateContext();
            IAppSettingsRepository repository = new XmlAppSettingsRepository(context);
            IQueryService<GetSavedAppSettingsQuery, AppSettings> sut = new GetSavedAppSettingsService(repository);

            // Act
            AppSettings result = sut.Execute(new());

            // Assert
            AppSettings comparisonAppSettings = new()
            {
                ShowOnlyAvailableProfiles = false,
                MinimizeToTray = true,
                StartMinimized = true
            };
            AssertCorrectAppSettings(result, comparisonAppSettings);
        }

        private static void AssertCorrectAppSettings(AppSettings appSettings, AppSettings comparison)
        {
            // Fail the test if we are not comparing all of the properties found in AppSettings
            appSettings.GetType().GetProperties().Length.Should().Be(3);

            appSettings.ShowOnlyAvailableProfiles.Should().Be(comparison.ShowOnlyAvailableProfiles);
            appSettings.MinimizeToTray.Should().Be(comparison.MinimizeToTray);
            appSettings.StartMinimized.Should().Be(comparison.StartMinimized);
        } 
    }
}
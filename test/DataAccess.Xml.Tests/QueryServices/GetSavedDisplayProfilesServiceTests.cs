using DataAccess.Xml.Repositories;
using DomainModel.Entities;
using DomainModel.Queries;
using DomainModel.QueryServices;
using DomainModel.Repositories;
using FluentAssertions;

namespace DataAccess.Xml.Tests.QueryServices
{
    [Collection("XmlConfigurationFileFactory")]
    public class GetSavedDisplayProfilesServiceTests : IntegrationTest<XmlConfigurationFileFactory, XmlConfigurationFileFactoryFixture>
    {
        public GetSavedDisplayProfilesServiceTests(XmlConfigurationFileFactoryFixture fixture)
            : base(fixture) { }

        [Fact]
        public void Query_returns_correct_profiles()
        {
            // Arrange
            XmlConfigurationFileFactory context = Fixture.CreateContext();
            IProfileRepository repository = new XmlProfileRepository(context);
            IQueryService<GetSavedDisplayProfilesQuery, IEnumerable<DisplayProfile>> sut = new GetSavedDisplayProfilesService(repository);

            // Act
            IEnumerable<DisplayProfile> result = sut.Execute(new());

            // Assert
            result.Count().Should().Be(3);

            DisplayProfile? desktopProfile = result.SingleOrDefault(profile => profile.Name == "Desktop");
            desktopProfile.Should().NotBeNull();
            if (desktopProfile is null)
                return;

            DisplayProfile? primaryDisplayProfile = result.SingleOrDefault(profile => profile.Name == "Primary Display");
            primaryDisplayProfile.Should().NotBeNull();
            if (primaryDisplayProfile is null)
                return;

            DisplayProfile? simulatorProfile = result.SingleOrDefault(profile => profile.Name == "Simulator");
            simulatorProfile.Should().NotBeNull();
            if (simulatorProfile is null)
                return;

            desktopProfile.DisplayConfigurations.Count.Should().Be(2);
            primaryDisplayProfile.DisplayConfigurations.Count.Should().Be(1);
            simulatorProfile.DisplayConfigurations.Count.Should().Be(3);

            DisplayConfiguration? sampleConfiguration = desktopProfile.DisplayConfigurations.SingleOrDefault(configuration => configuration.DeviceName == @"\\.\DISPLAY1");
            sampleConfiguration.Should().NotBeNull();
            if (sampleConfiguration is null)
                return;

            sampleConfiguration.SourceId.Should().Be(0);
            sampleConfiguration.TargetId.Should().Be(24838);
            sampleConfiguration.DisplayName.Should().Be("LC32G7xT");
            sampleConfiguration.ResolutionX.Should().Be(2560);
            sampleConfiguration.ResolutionY.Should().Be(1440);
            sampleConfiguration.PositionX.Should().Be(0);
            sampleConfiguration.PositionY.Should().Be(0);
            sampleConfiguration.VSyncNumerator.Should().Be(239958);
            sampleConfiguration.VSyncDenominator.Should().Be(1000);
            sampleConfiguration.IsHdrSupported.Should().BeTrue();
            sampleConfiguration.IsHdrEnabled.Should().BeFalse();
        }
    }
}
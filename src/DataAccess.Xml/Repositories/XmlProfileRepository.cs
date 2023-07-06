using DomainModel.Entities;
using DomainModel.Exceptions;
using DomainModel.Repositories;
using System.Xml.Linq;
using Tools.Extensions;
using KeyNotFoundException = DomainModel.Exceptions.KeyNotFoundException;

namespace DataAccess.Xml.Repositories
{
    public class XmlProfileRepository : IProfileRepository
    {
        private readonly XmlConfigurationFileFactory factory;

        public XmlProfileRepository(XmlConfigurationFileFactory factory)
            => this.factory = factory;

        public IEnumerable<DisplayProfile> GetAll()
        {
            using XmlConfigurationFile configurationFile = factory.LoadConfigurationFile();

            foreach (var profileElement in configurationFile.ProfilesElement.Elements())
            {
                string profileName = profileElement.GetAttributeValue<string>("name");

                IEnumerable<DisplayConfiguration> displayConfigurations = profileElement
                    .Element("displayConfigurations")!
                    .Elements()
                    .Select(element => new DisplayConfiguration(
                        element.GetAttributeValue<uint>("sourceId"),
                        element.GetAttributeValue<string>("deviceName"),
                        element.GetAttributeValue<uint>("targetId"),
                        element.GetAttributeValue<string>("displayName"),
                        element.GetAttributeValue<uint>("resolutionWidth"),
                        element.GetAttributeValue<uint>("resolutionHeight"),
                        element.GetAttributeValue<int>("positionX"),
                        element.GetAttributeValue<int>("positionY"),
                        element.GetAttributeValue<uint>("vSyncNumerator"),
                        element.GetAttributeValue<uint>("vSyncDenominator"),
                        element.GetAttributeValue<bool>("isHdrSupported"),
                        element.GetAttributeValue<bool>("isHdrEnabled")))
                    .ToList();

                yield return new(profileName, displayConfigurations, true);
            }
        }

        public void Add(DisplayProfile profile)
        {
            using XmlConfigurationFile configurationFile = factory.LoadConfigurationFile();

            Add(configurationFile, profile);
            configurationFile.SaveChanges();
        }

        public void Update(DisplayProfile profile)
        {
            using XmlConfigurationFile configurationFile = factory.LoadConfigurationFile();

            XElement existingProfile = configurationFile
                .ProfilesElement
                .Elements("profile")
                .SingleOrDefault(p => p.Attribute("name")?.Value == profile.Name) ??
                    throw new KeyNotFoundException(nameof(DisplayProfile.Name), profile.Name);

            existingProfile.Remove();
            Add(configurationFile, profile);

            configurationFile.SaveChanges();
        }

        private static void Add(XmlConfigurationFile configurationFile, DisplayProfile profile)
        {
            bool profileNameExists = configurationFile
                .ProfilesElement
                .Elements("profile")
                .Any(p => p.Attribute("name")?.Value == profile.Name);

            if (profileNameExists)
                throw new UniqueKeyViolationException(nameof(DisplayProfile.Name), profile.Name);

            IEnumerable<XElement> displayConfigurationElements = profile.DisplayConfigurations
                .Select(dp => new XElement("displayConfiguration",
                    new XAttribute("deviceName", dp.DeviceName),
                    new XAttribute("sourceId", dp.SourceId),
                    new XAttribute("targetId", dp.TargetId),
                    new XAttribute("displayName", dp.DisplayName),
                    new XAttribute("resolutionWidth", dp.ResolutionX),
                    new XAttribute("resolutionHeight", dp.ResolutionY),
                    new XAttribute("positionX", dp.PositionX),
                    new XAttribute("positionY", dp.PositionY),
                    new XAttribute("vSyncNumerator", dp.VSyncNumerator),
                    new XAttribute("vSyncDenominator", dp.VSyncDenominator),
                    new XAttribute("isHdrSupported", dp.IsHdrSupported),
                    new XAttribute("isHdrEnabled", dp.IsHdrEnabled)));

            configurationFile.ProfilesElement
                .Add(new XElement("profile",
                    new XAttribute("name", profile.Name),
                    new XElement("displayConfigurations", displayConfigurationElements)));
        }
    }
}
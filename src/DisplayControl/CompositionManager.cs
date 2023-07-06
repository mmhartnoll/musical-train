using DataAccess.Xml.Repositories;
using DomainModel.Commands;
using DomainModel.CommandServices;
using DomainModel.Entities;
using DomainModel.Events;
using DomainModel.Queries;
using DomainModel.QueryServices;
using DomainModel.Repositories;
using System.Collections.Generic;

namespace DisplayControl
{
    internal class CompositionManager
    {
        public ILoggingService LoggingService { get; private init; } = new FileLogger(LogFilePath);

        public IQueryService<GetSavedAppSettingsQuery, AppSettings> GetGetSavedAppSettingsService()
            => new GetSavedAppSettingsService(GetAppSettingsRepository());

        public ICommandService<SaveAppSettingsCommand> GetSaveAppSettingsService()
            => new SaveAppSettingsService(GetAppSettingsRepository());

        public IAsyncQueryService<GetActiveDisplayConfiguationsQuery, IEnumerable<DisplayConfiguration>> GetGetActiveDisplayConfigurationsService()
            => new GetActiveDisplayConfigurationsService();

        public IAsyncQueryService<GetAvailableDisplayTargetsQuery, IEnumerable<DisplayTarget>> GetGetAvailableDisplayTargetsService()
            => new GetAvailableDisplayTargetsService();

        public IAsyncQueryService<GetSavedDisplayProfilesQuery, IEnumerable<DisplayProfile>> GetGetAllProfilesService()
            => new GetSavedDisplayProfilesService(GetProfileRepository());

        public IAsyncCommandService<SaveProfileCommand> GetSaveProfileService()
            => new SaveProfileService(GetProfileRepository());

        public IAsyncCommandService<SwitchProfileCommand> GetSwitchProfileService()
            => new SwitchProfileService(new(LogDomainLogEvent));

        private IAppSettingsRepository GetAppSettingsRepository()
            => new XmlAppSettingsRepository(new(ConfigurationFilePath));

        private IProfileRepository GetProfileRepository()
            => new XmlProfileRepository(new(ConfigurationFilePath));

        private void LogDomainLogEvent(DomainLogEvent domainLogEvent)
            => LoggingService.Log(domainLogEvent.Level, domainLogEvent.Message);

        private const string ConfigurationFilePath = @"C:\Users\mmhar\source\repos\mmhartnoll\musical-train\src\DisplayControl\Configuration.json";
        private const string LogFilePath = @"C:\Users\mmhar\OneDrive\Desktop\DisplayControl_Log.txt";
    }
}
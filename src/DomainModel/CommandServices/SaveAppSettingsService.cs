using DomainModel.Commands;
using DomainModel.Repositories;

namespace DomainModel.CommandServices
{
    public class SaveAppSettingsService : ICommandService<SaveAppSettingsCommand>
    {
        private readonly IAppSettingsRepository repository;

        public SaveAppSettingsService(IAppSettingsRepository repository)
            => this.repository = repository;

        public void Execute(SaveAppSettingsCommand command)
            => repository.Save(command.AppSettings);
    }
}

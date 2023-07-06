using DomainModel.Commands;
using DomainModel.Exceptions;
using DomainModel.Repositories;

namespace DomainModel.CommandServices
{
    public class SaveProfileService : IAsyncCommandService<SaveProfileCommand>
    {
        private readonly IProfileRepository repository;

        public SaveProfileService(IProfileRepository repository)
            => this.repository = repository;

        public void Execute(SaveProfileCommand command)
        {
            if (!command.TryValidate())
                throw new InvalidCommandQueryException(typeof(SaveProfileCommand), command.Errors);
            try
            {
                repository.Add(new(command.ProfileName, command.DisplayConfigurations, true));
            }
            catch (UniqueKeyViolationException)
            {
                if (!command.AllowOverwrite)
                    throw;
                repository.Update(new(command.ProfileName, command.DisplayConfigurations, true));
            }
        }

        public Task ExecuteAsync(SaveProfileCommand command)
            => Task.Run(() => Execute(command));
    }
}
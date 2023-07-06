using DomainModel.Commands;

namespace DomainModel.CommandServices
{
    public interface IAsyncCommandService<TCommand> : ICommandService<TCommand>
        where TCommand : ServiceCommand
    {
        Task ExecuteAsync(TCommand command);
    }
}
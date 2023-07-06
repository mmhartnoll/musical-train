using DomainModel.Commands;

namespace DomainModel.CommandServices
{
    public interface ICommandService<TCommand>
        where TCommand : ServiceCommand
    {
        void Execute(TCommand command);
    }
}
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisplayControl.ViewModels.Commands
{
    internal interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
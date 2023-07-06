using System.Threading.Tasks;
using System.Windows.Input;

namespace DisplayControl.ViewModels.Commands
{
    internal interface IAsyncCommand<TParameter> : ICommand
    {
        Task ExecuteAsync(TParameter parameter);
        bool CanExecute(TParameter parameter);
    }
}
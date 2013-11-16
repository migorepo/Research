using System.Windows.Input;

namespace Wp8.Framework.Utils.Interfaces
{
    public interface IRouteCommand : ICommand
    {
        void RaiseCanExecuteChanged(bool enabled = true);
    }
}

using System;
using System.Windows;
using Wp8.Framework.Utils.Interfaces;

namespace Wp8.Framework.Utils.Command
{
    class RouteCommand : IRouteCommand
    {
        public Action<object> ExecuteHandler { get; set; }
        public Func<object, bool> CanExecuteHandler { get; set; }

        private volatile bool _here;
        private readonly bool _autoDisable;
        private static readonly object SyncRoot = new object();

        public RouteCommand()
        {
        }

        public RouteCommand(bool autoDisable)
        {
            _autoDisable = autoDisable;
        }

        #region ICommandRoute Members

        public bool CanExecute(object parameter)
        {
            lock (SyncRoot)
            {
                if (_here) return false;
                return null == CanExecuteHandler || CanExecuteHandler(parameter);
            }
        }

        public void Execute(object parameter)
        {
            if (_here) return;

            lock (SyncRoot)
            {
                if (_here) return;
                _here = true;
                if (null == ExecuteHandler)
                {
                    _here = false;
                    return;
                }
                RaiseCanExecuteChanged(false);
                ExecuteHandler(parameter);
                if (!_autoDisable) RaiseCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged(bool enabled = true)
        {
            lock (SyncRoot)
            {
                _here = !enabled;
            }
            if (null != CanExecuteChanged)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => CanExecuteChanged(this, EventArgs.Empty));
            }
        }
        #endregion
    }
}

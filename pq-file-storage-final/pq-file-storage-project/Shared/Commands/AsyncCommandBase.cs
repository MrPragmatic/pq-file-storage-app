using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pq_file_storage_project.Shared.Commands
{
    public abstract class AsyncCommandBase : ICommand
    {
        private readonly Action<Exception> _onException;

        private bool _isExecuting;

        public bool isExecuting
        {
            get
            {
                return _isExecuting;
            }
            private set
            {
                _isExecuting = value;
                OnCanExecuteChanged();
            }
        }
        public event EventHandler CanExecuteChanged;

        public AsyncCommandBase(Action<Exception> onException = null)
        {
            _onException = onException;
        }

        public bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;

            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
            isExecuting = false;
        }

        protected abstract Task ExecuteAsync(object parameter);

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

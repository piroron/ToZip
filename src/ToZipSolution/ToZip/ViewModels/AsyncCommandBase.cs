using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZip.ViewModels {
    public abstract class AsyncCommandBase : IAsyncCommand {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) {
            throw new NotImplementedException();
        }

        public async void Execute(object parameter) {
            await ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object parameter) {
            throw new NotImplementedException();
        }
    }
}

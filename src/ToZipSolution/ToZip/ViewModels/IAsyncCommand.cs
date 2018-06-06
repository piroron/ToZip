using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToZip.ViewModels {
    /// <summary>
    /// 非同期で実行される Command を表します。
    /// </summary>
    public interface IAsyncCommand : ICommand {

        Task ExecuteAsync(object parameter);
    }
}

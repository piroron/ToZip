using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Forms = System.Windows.Forms;

using Livet;

namespace ToZip {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            DispatcherHelper.UIDispatcher = Dispatcher;
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        //集約エラーハンドラ
        //private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    //TODO:ロギング処理など
        //    MessageBox.Show(
        //        "不明なエラーが発生しました。アプリケーションを終了します。",
        //        "エラー",
        //        MessageBoxButton.OK,
        //        MessageBoxImage.Error);
        //
        //    Environment.Exit(1);
        //}

        /// <summary>
        /// フォルダ選択ダイアログを表示します。
        /// </summary>
        /// <returns></returns>
        public static string ShowFolderDialog() {

            using (Forms.FolderBrowserDialog dlg = new Forms.FolderBrowserDialog()) {

                if (dlg.ShowDialog() == Forms.DialogResult.OK) {
                    return dlg.SelectedPath;
                }
            }

            return string.Empty;
        }
    }
}

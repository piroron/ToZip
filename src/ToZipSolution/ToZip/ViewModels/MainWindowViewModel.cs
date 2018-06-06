using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using ToZip.Models;
using ToZip.Services;
using System.Threading;
using System.Threading.Tasks;

namespace ToZip.ViewModels {
    public class MainWindowViewModel : ViewModel {

        private FileCompressModel _model = new FileCompressModel();
        private CancellationTokenSource _cancelToken;

        public MainWindowViewModel() {
            SourceItems = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                _model.SourceItems,
                (child) => new FileSystemViewModel(child),
                DispatcherHelper.UIDispatcher);
        }

        public void Initialize() {
        }


        #region SourcePath変更通知プロパティ

        /// <summary>
        /// 圧縮開始ディレクトリのパスを取得または設定します。
        /// </summary>
        public string SourcePath {
            get { return _model.SourceRootPath; }
            set {
                if (_model.SourceRootPath == value)
                    return;
                _model.SourceRootPath = value;
                RaisePropertyChanged();

            }
        }
        #endregion

        #region DestinationPath変更通知プロパティ

        /// <summary>
        /// 圧縮ファイル格納フォルダを取得または設定します。
        /// </summary>
        public string DestinationPath {
            get { return _model.DestinationRootPath; }
            set {
                if (_model.DestinationRootPath == value)
                    return;
                _model.DestinationRootPath = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region SourceSelectCommand
        private ViewModelCommand _sourceSelectCommand;

        /// <summary>
        /// 圧縮開始フォルダを選択します。
        /// </summary>
        public ViewModelCommand SourceSelectCommand {
            get {
                if (_sourceSelectCommand == null) {
                    _sourceSelectCommand = new ViewModelCommand(SourceSelect, CanSourceSelect);
                }
                return _sourceSelectCommand;
            }
        }

        public bool CanSourceSelect() {
            return !IsBusyCompress;
        }

        public void SourceSelect() {
            string folder = SelectFolder();
            if (!string.IsNullOrEmpty(folder)) {
                SourcePath = folder;
            }
        }
        #endregion

        #region DestinationSelectCommand
        private ViewModelCommand _destinationSelectCommand;

        /// <summary>
        /// 圧縮ファイル格納フォルダを取得します。
        /// </summary>
        public ViewModelCommand DestinationSelectCommand {
            get {
                if (_destinationSelectCommand == null) {
                    _destinationSelectCommand = new ViewModelCommand(DestinationSelect, CanDestinationSelect);
                }
                return _destinationSelectCommand;
            }
        }

        private bool CanDestinationSelect() {
            return !IsBusyCompress;
        }

        private void DestinationSelect() {
            string folder = SelectFolder();
            if (!string.IsNullOrEmpty(folder)) {
                DestinationPath = folder;
            }
        }
        #endregion

        /// <summary>
        /// 圧縮対象として選択できる項目を表します。
        /// </summary>
        public IEnumerable<FileSystemViewModel> SourceItems {
            get;
            private set;
        }


        #region CompressCommand
        private ViewModelCommand _compressCommand;

        public ViewModelCommand CompressCommand {
            get {
                if (_compressCommand == null) {
                    _compressCommand = new ViewModelCommand(Compress, CanCompress);
                }
                return _compressCommand;
            }
        }

        public bool CanCompress() {
            return !IsBusyCompress;
        }

        /// <summary>
        /// 圧縮を実行します。
        /// </summary>
        public async void Compress() {
            IsBusyCompress = true;

            //キャンセル用トークンを用意

            if (_cancelToken != null) {
                _cancelToken = null;
            }

            _cancelToken = new CancellationTokenSource();

            try {
                await _model.CompressTaskAsync(_cancelToken);

                FileSystemService.Open(DestinationPath);

            } catch (AggregateException ex) {

                Messenger.Raise(new InformationMessage(ex.Message, "エラー", System.Windows.MessageBoxImage.Error, "Error"));
            } catch (OperationCanceledException) {

                Messenger.Raise(new InformationMessage("圧縮はキャンセルされました。", "確認", System.Windows.MessageBoxImage.Information, "Infomation"));
            } finally {
                IsBusyCompress = false;
            }



            //var task = _model.CompressTaskAsync(_cancelToken);

            //await task;

            //IsBusyCompress = false;

            //if (task.IsFaulted) {

            //    Messenger.Raise(new InformationMessage(task.Exception.Message, "エラー", System.Windows.MessageBoxImage.Error, "Error"));
            //} else if (task.IsCanceled) {

            //    //ファイル消す？
            //    Messenger.Raise(new InformationMessage("圧縮はキャンセルされました。", "確認", System.Windows.MessageBoxImage.Information, "Error"));
            //} else if (task.IsCompleted) {

            //    FileSystemService.Open(DestinationPath);
            //}

            //todo:非同期で圧縮、キャンセル
        }
        #endregion

        #region CancelCommand
        private ViewModelCommand _cancelCommand;

        /// <summary>
        /// 圧縮をキャンセルするコマンドです。
        /// </summary>
        public ViewModelCommand CancelCommand {
            get {
                if (_cancelCommand == null) {
                    _cancelCommand = new ViewModelCommand(Cancel, CanCancel);
                }
                return _cancelCommand;
            }
        }

        public bool CanCancel() {
            return IsBusyCompress;
        }

        public void Cancel() {
            if (_cancelToken != null) {
                _cancelToken.Cancel();
            }
        }
        #endregion

        #region IsBusyCompress変更通知プロパティ
        private bool _isBusyCompress;

        /// <summary>
        /// 圧縮処理を実行しているかどうか示す値を取得します。
        /// </summary>
        public bool IsBusyCompress {
            get { return _isBusyCompress; }
            set {
                if (_isBusyCompress == value)
                    return;
                _isBusyCompress = value;
                RaisePropertyChanged();

                CancelCommand.RaiseCanExecuteChanged();
                SourceSelectCommand.RaiseCanExecuteChanged();
                DestinationSelectCommand.RaiseCanExecuteChanged();
                CompressCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion


        private string SelectFolder() {
            return App.ShowFolderDialog();
        }
    }
}

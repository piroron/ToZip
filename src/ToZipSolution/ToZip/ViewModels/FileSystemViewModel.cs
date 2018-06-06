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
using System.Threading.Tasks;

namespace ToZip.ViewModels {
    public class FileSystemViewModel : ViewModel {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */


        private FileSystemModel _model;

        /// <summary>
        /// 当オブジェクトが扱う Model を取得します。
        /// </summary>
        /// <remarks>Modelは、ほとんどの場合、当クラスで生成されます。</remarks>
        public FileSystemModel Model {
            get {
                return _model;
            }
        }

        /// <summary>
        /// 当クラスが利用するModelを指定して、当クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="model"></param>
        public FileSystemViewModel(FileSystemModel model) {
            _model = model;

            Childs = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                _model.Childs,
                (child) => new FileSystemViewModel(child),
                DispatcherHelper.UIDispatcher);

            Childs.CollectionChanged += (sender, e) => {

                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
                    foreach (FileSystemViewModel item in e.NewItems) {
                        item.Parent = this;
                    }
                }
            };
        }

        public void Initialize() {
        }

        /// <summary>
        /// 当インスタンスの親を取得します。
        /// </summary>
        public FileSystemViewModel Parent { get; private set; }

        /// <summary>
        /// 子ディレクトリ等を取得します。
        /// </summary>
        public ReadOnlyDispatcherCollection<FileSystemViewModel> Childs { get; private set; }

        public string Name {
            get {
                return _model.Name;
            }
        }

        #region IsExpanded変更通知プロパティ
        private bool _isExpanded;

        /// <summary>
        /// 子要素が展開されているかどうか示す値を取得または設定します。
        /// </summary>
        public bool IsExpanded {
            get { return _isExpanded; }
            set {
                if (_isExpanded == value)
                    return;
                _isExpanded = value;
                RaisePropertyChanged();

                if (!_model.IsSearched) {

                    Search();
                }
            }
        }
        #endregion

        #region IsSelected変更通知プロパティ

        /// <summary>
        /// 当項目が選択状態かどうか示す値を取得または設定します。
        /// </summary>
        public bool IsSelected {
            get { return _model.IsCompressTarget; }
            set {
                if (_model.IsCompressTarget == value)
                    return;
                _model.IsCompressTarget = value;
                RaisePropertyChanged();

                if (value) {
                    RemoveCheckChild(this);
                    RemoveCheckParent(this);
                }
            }
        }
        #endregion


        #region SelectAllCommand
        private ViewModelCommand _selectAllCommand;

        /// <summary>
        /// 全ての子要素を選択状態にします。
        /// </summary>
        public ViewModelCommand SelectAllCommand {
            get {
                if (_selectAllCommand == null) {
                    _selectAllCommand = new ViewModelCommand(SelectAll, CanSelectAll);
                }
                return _selectAllCommand;
            }
        }

        public bool CanSelectAll() {
            return _model.IsSearched && _model.Childs.Count > 0;
        }

        public void SelectAll() {

            //検索していなくても、全選択は有効にしたほうが良いかも
            //→やらない。開いて確認してもらう

            this.IsSelected = false;

            Childs.AsParallel().ForAll(f => f.IsSelected = true);

            IsExpanded = true;
        }
        #endregion

        #region DeselectAllCommand
        private ViewModelCommand _deselectAllCommand;

        /// <summary>
        /// 全ての子要素を非選択状態にします。
        /// </summary>
        public ViewModelCommand DeselectAllCommand {
            get {
                if (_deselectAllCommand == null) {
                    _deselectAllCommand = new ViewModelCommand(DeselectAll, CanDeselectAll);
                }
                return _deselectAllCommand;
            }
        }

        public bool CanDeselectAll() {
            return _model.IsSearched && _model.Childs.Count > 0;
        }

        public void DeselectAll() {
            Childs.AsParallel().ForAll(f => f.IsSelected = false);
            //Childs.AsParallel().ForAll(f => f.DeselectAll());
            //孫も含めて全部やる？
        }
        #endregion

        /// <summary>
        /// 子供の検索（無駄にasync）
        /// </summary>
        private async void Search() {

            await _model.SearchChildDirectoryTaskAsync();


            SelectAllCommand.RaiseCanExecuteChanged();
            DeselectAllCommand.RaiseCanExecuteChanged();
        }

        private void RemoveCheckChild(FileSystemViewModel vm) {

            foreach (var item in vm.Childs) {
                item.IsSelected = false;
                RemoveCheckChild(item);
            }
        }

        private void RemoveCheckParent(FileSystemViewModel vm) {
            if (vm.Parent != null) {
                vm.Parent.IsSelected = false;
                RemoveCheckParent(vm.Parent);
            }
        }

    }
}

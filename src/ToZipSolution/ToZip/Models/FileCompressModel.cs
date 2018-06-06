using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Livet;
using ToZip.Services;

namespace ToZip.Models
{
    /// <summary>
    /// 圧縮処理を行います。
    /// </summary>
    public class FileCompressModel : NotificationObject
    {


        #region SourceRootPath変更通知プロパティ
        private string _sourceRootPath;

        /// <summary>
        /// 圧縮開始パスを取得または設定します。
        /// </summary>
        public string SourceRootPath {
            get { return _sourceRootPath; }
            set {
                if (_sourceRootPath == value)
                    return;
                _sourceRootPath = value;
                RaisePropertyChanged();

                _sourceItems.Clear();

                if (FileSystemService.Exists(value))
                {
                    var item = new DirectoryModel(value, null);
                    if (item != null)
                    {
                        _sourceItems.Add(item);
                    }
                }
            }
        }
        #endregion

        #region DestinationRootPath変更通知プロパティ
        private string _destinationRootPath;

        /// <summary>
        /// 圧縮ファイル出力先パスを取得または設定します。
        /// </summary>
        public string DestinationRootPath {
            get { return _destinationRootPath; }
            set {
                if (_destinationRootPath == value)
                    return;
                _destinationRootPath = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        private ObservableSynchronizedCollection<FileSystemModel> _sourceItems = new ObservableSynchronizedCollection<FileSystemModel>();

        /// <summary>
        /// 圧縮の対象となるフォルダを取得します。
        /// </summary>
        public ObservableSynchronizedCollection<FileSystemModel> SourceItems {
            get { return _sourceItems; }
        }

        /// <summary>
        /// 圧縮処理を非同期で行います。
        /// </summary>
        /// <returns></returns>
        public async Task CompressTaskAsync(CancellationTokenSource cancelToken)
        {
            var items = SourceItems.First().GetCompressTargets();
            //ParallelOptions opt = new ParallelOptions();
            //opt.CancellationToken = cancelToken.Token;
            var factory = new ZipFileFactory(SourceRootPath, DestinationRootPath);

            //var zips = items.AsParallel().WithCancellation(cancelToken.Token);

            //foreach (var zip in zips)
            //{
            //    factory.Create(zip.FullPath);
            //}

            await Task.WhenAll(items.Select(i => Task.Run(() => factory.Create(i.FullPath))));

            //await Task.Run(() => {

            //    var factory = new ZipFileFactory(SourceRootPath, DestinationRootPath);

            //    Parallel.ForEach(items, opt, (item) => {
            //        if (cancelToken.IsCancellationRequested) {

            //            return;
            //        }

            //        factory.Create(item.FullPath);
            //    });
            //}, cancelToken.Token);
        }

        public bool CanExecuteCompress {
            get {
                return
                    SourceItems.Any() &&
                    !string.IsNullOrEmpty(DestinationRootPath);
            }
        }
    }
}

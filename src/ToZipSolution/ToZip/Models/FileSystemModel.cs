using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Livet;
using ToZip.Services;

namespace ToZip.Models {
    /// <summary>
    /// ファイル、ディレクトリを表すオブジェクトの継承元です。
    /// </summary>
    public abstract class FileSystemModel : NotificationObject {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        /// <summary>
        /// 絶対パスを指定して、当クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="absolutePath">絶対パス</param>
        /// <remarks>存在しないファイルまたはディレクトリでも、インスタンスを生成できます。</remarks>
        public FileSystemModel(string absolutePath)
            : this(absolutePath, null) {
        }

        /// <summary>
        /// 絶対パス及び当クラスの親オブジェクトを指定して、当クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="absolutePath">絶対パス</param>
        /// <param name="parent">当クラスの親オブジェクト</param>
        /// <remarks>存在しないファイルまたはディレクトリでも、インスタンスを生成できます。</remarks>
        public FileSystemModel(string absolutePath, FileSystemModel parent) {
            FullPath = absolutePath;
            Name = FileSystemService.GetName(absolutePath);

            Childs = new ObservableSynchronizedCollection<FileSystemModel>();

            Parent = parent;
        }


        #region IsSearched変更通知プロパティ
        private bool _isSearched;

        /// <summary>
        /// 子孫ディレクトリを取得したかどうか判定した結果を取得します。
        /// </summary>
        public bool IsSearched {
            get { return _isSearched; }
            protected set {
                if (_isSearched == value)
                    return;
                _isSearched = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// ファイル名またはディレクトリ名を取得します。
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 絶対パスを取得します。
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// 当ディレクトリの親を取得します。
        /// </summary>
        public FileSystemModel Parent { get; private set; }


        #region IsCompressTarget変更通知プロパティ
        private bool _isCompressTarget;

        /// <summary>
        /// 圧縮の対象であるかどうか示す値を取得または設定します。
        /// </summary>
        public bool IsCompressTarget {
            get { return _isCompressTarget; }
            set {
                if (_isCompressTarget == value)
                    return;
                _isCompressTarget = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 当ディレクトリの子孫を取得します。
        /// </summary>
        public ObservableSynchronizedCollection<FileSystemModel> Childs { get; private set; }

        /// <summary>
        /// 当ディレクトリの子孫を検索します。
        /// </summary>
        public abstract Task<IEnumerable<FileSystemModel>> SearchChildDirectoryTaskAsync();

        /// <summary>
        /// 自身および自身の子孫で、圧縮対象とマークされている項目を取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FileSystemModel> GetCompressTargets() {
            var results = new List<FileSystemModel>();
            FillCompressTarget(results);
            return results;

        }

        private void FillCompressTarget(List<FileSystemModel> results) {

            if (IsCompressTarget) {
                results.Add(this);
            } else {
                foreach (var child in Childs) {
                    child.FillCompressTarget(results);
                }
            }
        }
    }
}

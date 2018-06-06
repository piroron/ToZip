using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace ToZip.Services {
    /// <summary>
    /// .zip形式のファイルを作成します。
    /// </summary>
    /// <remarks>パスの検査をつけたほうが良い（外部ツールからの実行を想定）</remarks>
    public class ZipFileFactory {

        #region フィールド
        private static readonly string _extension = ".zip";
        #endregion

        #region プロパティ

        /// <summary>
        /// 圧縮開始ディレクトリ名を取得します。
        /// </summary>
        public string SourceRootDirectoryName { get; private set; }

        /// <summary>
        /// 圧縮したファイルを格納するルートディレクトリ名を取得します。
        /// </summary>
        public string DestinationRootDirectoryName { get; private set; }

        #endregion

        /// <summary>
        /// 圧縮開始ディレクトリ、出力のルートディレクトリを指定して、当クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="sourceRoot">圧縮開始ディレクトリ</param>
        /// <param name="destinationRoot">圧縮ファイル格納ルートディレクトリ</param>
        public ZipFileFactory(string sourceRoot, string destinationRoot) {
            SourceRootDirectoryName = Path.GetFullPath(sourceRoot);
            DestinationRootDirectoryName = Path.GetFullPath(destinationRoot);
        }

        /// <summary>
        /// zipファイルを作成します。
        /// </summary>
        /// <param name="sourceDirectoryName">圧縮するディレクトリのパス</param>
        public void Create(string sourceDirectoryName) {

            string source = Path.GetFullPath(sourceDirectoryName);

            //ディレクトリ生成
            string destPath = CreateDirectory(source);

            string archivefilePath = Path.Combine(destPath, Path.GetFileNameWithoutExtension(source) + _extension);
            //string archivefilePath = Path.Combine(destinationArchiveDirectoryName, fileName + _extension);

            if (File.Exists(archivefilePath)) {
                //既に同名ファイルがある場合は消す
                File.Delete(archivefilePath);
            }

            ZipFile.CreateFromDirectory(source, archivefilePath);
        }

        /// <summary>
        /// ディレクトリの生成。既に存在する場合は生成しない。
        /// </summary>
        /// <param name="source">圧縮元ディレクトリ</param>
        /// <returns>生成を試行したディレクトリの絶対パス</returns>
        private string CreateDirectory(string source) {

            string path = DestinationRootDirectoryName;

            foreach (var name in GetRelatedPathNames(source)) {
                path = Path.Combine(path, name);
            }

            Debug.WriteLine(string.Format("DirectoryCreate Path={0}, Exists={1}", path, Directory.Exists(path)));

            Directory.CreateDirectory(path);

            return path;
        }


        /// <summary>
        /// 圧縮開始ディレクトリから、引数で指定したパスの一つ手前までの、相対パスディレクトリ名を取得する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<string> GetRelatedPathNames(string source) {
            //sourceRootからの相対パス取得
            string root = Path.GetFileNameWithoutExtension(SourceRootDirectoryName);

            yield return root;

            string relPath = source.Replace(SourceRootDirectoryName, string.Empty);

            var names = relPath.Split(Path.DirectorySeparatorChar);

            //末尾は取得しない
            foreach (var segment in names.Take(names.Length - 1)) {
                if (string.IsNullOrEmpty(segment)) {
                    continue;
                }
                yield return segment;
            }

            yield break;
        }
    }
}

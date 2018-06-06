using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace ToZip.Services {
    /// <summary>
    /// ファイルシステムへのアクセスを行う場合のメソッドを提供します。
    /// </summary>
    public static class FileSystemService {

        /// <summary>
        /// 指定ディレクトリの子孫となっているディレクトリを、絶対パスで取得します。
        /// </summary>
        /// <param name="dir">絶対パスのディレクトリ</param>
        /// <returns></returns>
        public static Task<IEnumerable<string>> GetDirectoriesAsync(string dir) {
            var result = new Func<IEnumerable<string>>(() => {
                if (!Directory.Exists(dir)) {
                    return Enumerable.Empty<string>();
                }

                return Directory.EnumerateDirectories(dir);
            });

            return Task.Run(result);
        }

        /// <summary>
        /// 指定したパスのファイル名またはフォルダ名を返します。
        /// </summary>
        /// <param name="path">対象パス</param>
        /// <returns>ファイル名またはディレクトリ名</returns>
        public static string GetName(string path) {
            //ファイルだろうがディレクトリだろうが、名前を返せる
            return Path.GetFileName(path);
        }

        /// <summary>
        /// 指定したパスの項目が存在するか判定します。
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static bool Exists(string dirName) {
            return Directory.Exists(dirName);
        }

        public static void Open(string path) {
            if (Exists(path)) {
                Process.Start(path);
            }

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using ToZip.Services;

namespace ToZip.Models {
    /// <summary>
    /// ディレクトリを表します。
    /// </summary>
    public class DirectoryModel : FileSystemModel {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        public DirectoryModel(string absolutePath, FileSystemModel parent)
            : base(absolutePath, parent) {

            Childs.Add(new EmptyFileSystemModel(absolutePath, this));
            IsSearched = false;
        }

        /// <summary>
        /// 当ディレクトリの子孫を検索します。
        /// </summary>
        public override async Task<IEnumerable<FileSystemModel>> SearchChildDirectoryTaskAsync() {

            if (IsSearched) {
                return Childs;
            }

            var dirs = await FileSystemService.GetDirectoriesAsync(FullPath);

            Childs.Clear();

            foreach (var path in dirs.OrderBy(f => f)) {
                Childs.Add(new DirectoryModel(path, this));
            }

            IsSearched = true;

            return Childs;
        }
    }
}

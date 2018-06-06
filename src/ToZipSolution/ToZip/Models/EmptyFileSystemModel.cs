using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace ToZip.Models {
    /// <summary>
    /// 空のファイル等を表します。ディレクトリの子孫を検索していない場合は、これが入っています。
    /// </summary>
    public class EmptyFileSystemModel : FileSystemModel {
        public EmptyFileSystemModel(string absolutePath, FileSystemModel parent)
            : base(Path.Combine(absolutePath, "検索中..."), parent) {

            //検索とかさせない
            IsSearched = true;
        }

        /// <summary>
        /// 実行されません。
        /// </summary>
        /// <returns></returns>
        public override Task<IEnumerable<FileSystemModel>> SearchChildDirectoryTaskAsync() {
            return null;
        }
    }
}

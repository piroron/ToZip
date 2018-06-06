using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZip.Services {
    /// <summary>
    /// 圧縮を実行することを表します。
    /// </summary>
    public interface ICompressor {

        /// <summary>
        /// 圧縮を実行します。
        /// </summary>
        /// <param name="targetPath">圧縮対象のパス</param>
        /// <param name="destinationPath">圧縮結果を出力するパス</param>
        void Compress(string targetPath, string destinationPath);
    }
}

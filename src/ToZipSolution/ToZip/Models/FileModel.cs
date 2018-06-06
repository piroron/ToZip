using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToZip.Models {
    public class FileModel : FileSystemModel {

        public FileModel(string absolutePath, FileSystemModel parent)
            : base(absolutePath, parent) {

            IsSearched = true;
        }

        public override async Task<IEnumerable<FileSystemModel>> SearchChildDirectoryTaskAsync() {
            return await Task.Run(() => Childs);
        }
    }
}

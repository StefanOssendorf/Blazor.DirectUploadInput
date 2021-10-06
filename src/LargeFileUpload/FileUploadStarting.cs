using System;
using System.Collections.Generic;
using System.Linq;

namespace LargeFileUpload {
    /// <summary>
    /// The container holding information about the files which will be uploaded.
    /// </summary>
    /// <param name="Files">The files being uploaded.</param>
    public record FileUploadStarting(IReadOnlyList<UploadingFile> Files) {

        private readonly Lazy<long> _totalSizeLazy = new(() => Files.Select(f => f.Size).DefaultIfEmpty(0).Sum(s => s));

        /// <summary>
        /// Gets the number of files being uploaded.
        /// </summary>
        public int FileCount => Files.Count;

        /// <summary>
        /// Gets the total size of the files being uploaded.
        /// </summary>
        public long TotalSize => _totalSizeLazy.Value;
    }
}

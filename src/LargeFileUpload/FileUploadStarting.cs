using System.Collections.Generic;

namespace LargeFileUpload {
    /// <summary>
    /// The container holding information about the files which will be uploaded.
    /// </summary>
    /// <param name="Files">The files being uploaded.</param>
    public record FileUploadStarting(IReadOnlyList<UploadingFile> Files) {

        /// <summary>
        /// Gets the number of files being uploaded.
        /// </summary>
        public int FileCount => Files.Count;
    }
}

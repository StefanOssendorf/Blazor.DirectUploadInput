using System.Collections.ObjectModel;

namespace LargeFileUpload.Interop {

    /// <summary>
    /// The js file upload starting container.
    /// </summary>
    public record JsFileUploadStarting {

        /// <summary>
        /// The information about files being uploaded.
        /// </summary>
        public Collection<JsFileInfo> Files { get; init; } = new();
    }
}

using System.Collections.ObjectModel;

namespace LargeFileUpload.Interop {
    public record JsFileUploadStarting {
        public Collection<JsFile> Files { get; init; } = new();
    }
}

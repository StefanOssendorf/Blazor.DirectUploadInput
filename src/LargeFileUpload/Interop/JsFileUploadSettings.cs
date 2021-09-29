using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.JSInterop;

namespace LargeFileUpload.Interop {
    internal record JsFileUploadSettings {
        public DotNetObjectReference<FileUpload> DotNetHelper { get; set; }

        public string UploadUrl { get; init; }

        public string HttpMethod { get; init; }

        public string FormName { get; init; } = "files";

        public Dictionary<string, string> Headers { get; init; } = new();

        public InteropCallbacks Callbacks { get; init; }
    }
}

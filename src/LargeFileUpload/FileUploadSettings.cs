using System.Collections.Generic;

namespace LargeFileUpload {
    public record FileUploadSettings {

        public string UploadUrl { get; init; }

        public string HttpMethod { get; init; }

        public string FormName { get; init; } = "files";

        public Dictionary<string, string> Headers { get; init; } = new();
    }

    
}

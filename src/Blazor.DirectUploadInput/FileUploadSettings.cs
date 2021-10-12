using System.Collections.Generic;

namespace StefanOssendorf.Blazor.DirectUploadInput {
    /// <summary>
    /// The settings to configure the file upload.
    /// </summary>
    public record FileUploadSettings {

        /// <summary>
        /// The url to upload to. Either relative or absolute.
        /// </summary>
        public string UploadUrl { get; init; } = string.Empty;

        /// <summary>
        /// The used http method for the file upload.
        /// </summary>
        public string HttpMethod { get; init; } = "POST";

        /// <summary>
        /// The name for form field to be used for uploading.
        /// </summary>
        public string FormName { get; init; } = "files";

        /// <summary>
        /// Headers to append to the file upload request.
        /// </summary>
        public Dictionary<string, string> Headers { get; init; } = new();

        /// <summary>
        /// Additional form data to add to the upload request.
        /// </summary>
        public Dictionary<string, string> FormData { get; init; } = new();
    }
}

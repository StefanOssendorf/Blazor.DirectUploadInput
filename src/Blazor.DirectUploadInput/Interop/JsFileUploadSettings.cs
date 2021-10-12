using System.Collections.Generic;
using Microsoft.JSInterop;

namespace StefanOssendorf.Blazor.DirectUploadInput.Interop {

    /// <summary>
    /// The js file upload settings container.
    /// </summary>
    internal record JsFileUploadSettings {

        /// <summary>
        /// The dotnet helper reference to call back into .net code.
        /// </summary>
        public DotNetObjectReference<FileUploadJsAdapter> DotNetHelper { get; set; } = null!;

        /// <summary>
        /// The upload url.
        /// </summary>
        public string UploadUrl { get; init; } = string.Empty;

        /// <summary>
        /// The http method to be used.
        /// </summary>
        public string HttpMethod { get; init; } = string.Empty;

        /// <summary>
        /// Whether to use strict accept mode.
        /// </summary>
        public bool StrictAccept { get; set; }

        /// <summary>
        /// The name of the form field.
        /// </summary>
        public string FormName { get; init; } = "files";

        /// <summary>
        /// The additional headers to be used for the request.
        /// </summary>
        public Dictionary<string, string> Headers { get; init; } = new();

        /// <summary>
        /// The additional form data for the request.
        /// </summary>
        public Dictionary<string, string> FormData { get; init; } = new();

        /// <summary>
        /// The callback names used by <see cref="DotNetHelper"/>.
        /// </summary>
        public InteropCallbacks Callbacks { get; init; } = null!;
    }
}

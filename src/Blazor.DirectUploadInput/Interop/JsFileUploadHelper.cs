using Microsoft.JSInterop;

namespace StefanOssendorf.Blazor.DirectUploadInput.Interop {
    /// <summary>
    /// The js file upload helper container for it's initial setup.
    /// </summary>
    internal record JsFileUploadHelper {

        /// <summary>
        /// The dotnet helper reference to call back into .net code.
        /// </summary>
        public DotNetObjectReference<FileUploadJsAdapter> DotNetHelper { get; set; } = null!;

        /// <summary>
        /// The callback names used by <see cref="DotNetHelper"/>.
        /// </summary>
        public InteropCallbacks Callbacks { get; init; } = null!;
    }
}

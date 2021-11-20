namespace StefanOssendorf.Blazor.DirectUploadInput.Interop {
    /// <summary>
    /// Container for the js callbacks.
    /// </summary>
    internal record InteropCallbacks {
        /// <summary>
        /// The callback name for about to begin upload.
        /// </summary>
        public string Starting { get; init; } = string.Empty;

        /// <summary>
        /// The callback name for upload finished.
        /// </summary>
        public string Finished { get; init; } = string.Empty;

        /// <summary>
        /// The callback name for error during request.
        /// </summary>
        public string Errored { get; init; } = string.Empty;

        /// <summary>
        /// The callback name for notification when an upload was cancelled.
        /// </summary>
        public string Canceled { get; init; } = string.Empty;

        /// <summary>
        /// The callback name for retrieving the file upload settings.
        /// </summary>
        public string GetSettings { get; init; } = string.Empty;
    }
}

namespace LargeFileUpload.Interop {
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
    }
}

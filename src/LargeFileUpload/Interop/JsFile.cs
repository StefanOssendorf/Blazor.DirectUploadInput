namespace LargeFileUpload.Interop {

    /// <summary>
    /// A js file information container.
    /// </summary>
    public record JsFileInfo {
        /// <summary>
        /// The file name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// The mime file type.
        /// </summary>
        public string Type { get; init; } = string.Empty;
        
        /// <summary>
        /// The file size in bytes.
        /// </summary>
        public long Size { get; init; }
    }
}

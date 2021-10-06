namespace LargeFileUpload.Interop {

    /// <summary>
    /// The JS error container.
    /// </summary>
    public record JsError {
        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// The error stack.
        /// </summary>
        public string Stack { get; init; } = string.Empty;

    }
}

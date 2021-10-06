namespace LargeFileUpload {
    /// <summary>
    /// Represents an error during the file upload request.
    /// </summary>
    /// <param name="Message">The message.</param>
    /// <param name="Stack">The stack.</param>
    public record FileUploadError(string Message, string Stack);
}

namespace StefanOssendorf.Blazor.DirectUploadInput {
    /// <summary>
    /// Information about a file being uploaded.
    /// </summary>
    /// <param name="Name">The name of the file.</param>
    /// <param name="Size">The file size.</param>
    /// <param name="Type">The (mime) type of the file.</param>
    public record UploadingFile(string Name, long Size, string Type);
}

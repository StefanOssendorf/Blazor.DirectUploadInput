using System.Collections.Generic;

namespace LargeFileUpload {
    /// <summary>
    /// The file upload result.
    /// </summary>
    /// <param name="Headers">The headers of the response.</param>
    /// <param name="RawBody">The raw body as text.</param>
    /// <param name="StatusCode">The status code of the response.</param>
    public record FileUploadResult(IReadOnlyDictionary<string, string> Headers, string RawBody, int StatusCode);
}

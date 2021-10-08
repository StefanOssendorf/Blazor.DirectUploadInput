using System.Collections.Generic;

namespace StefanOssendorf.Blazor.DirectUploadInput.Interop {

    /// <summary>
    /// The js response container.
    /// </summary>
    public record JsResponse {
        /// <summary>
        /// The header keys from response.
        /// </summary>
        public List<string> HeaderKeys { get; init; } = new();

        /// <summary>
        /// The header values from response.
        /// </summary>
        public List<string> HeaderValues { get; init; } = new();

        /// <summary>
        /// The response body in text form.
        /// </summary>
        public string Body { get; init; } = string.Empty;

        /// <summary>
        /// The response status code.
        /// </summary>
        public int StatusCode { get; init; }
    }
}

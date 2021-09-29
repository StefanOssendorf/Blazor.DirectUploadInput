using System.Collections.Generic;

namespace LargeFileUpload.Interop {
    public record JsResponse {

        public List<string> HeaderKeys { get; init; }
        public List<string> HeaderValues { get; init; }
        public string Body { get; init; }
        public int StatusCode { get; init; }
    }
}

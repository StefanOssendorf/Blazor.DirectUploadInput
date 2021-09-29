namespace LargeFileUpload.Interop {
    public record JsFile {
        public string Name { get; init; }
        public string Type { get; init; }
        public long Size { get; init; }
    }
}

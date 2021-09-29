namespace LargeFileUpload.Interop {
    internal record InteropCallbacks {
        public string Starting { get; init; }
        public string Finished { get; init; }
        public string Errored { get; init; }
    }
}

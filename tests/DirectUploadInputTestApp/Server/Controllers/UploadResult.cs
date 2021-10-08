namespace FileUploadTestApp.Server.Controllers;

public partial class UploadsController {
    public class UploadResult {
        public bool Uploaded { get; set; }
        public string FileName { get; set; }
        public string StoredFileName { get; set; }
        public int ErrorCode { get; set; }
    }
}

using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FileUploadTestApp.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public partial class UploadsController : ControllerBase {

    private readonly IWebHostEnvironment env;
    private readonly ILogger<UploadsController> logger;

    public UploadsController(IWebHostEnvironment env, ILogger<UploadsController> logger) {
        this.env = env;
        this.logger = logger;
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue, MultipartBoundaryLengthLimit = int.MaxValue)]
    public async Task<ActionResult<IList<UploadResult>>> PostFile(IEnumerable<IFormFile> files) {
        var maxAllowedFiles = 3;
        long maxFileSize = long.MaxValue;
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        List<UploadResult> uploadResults = new();

        var file = files.First();
        var uploadResult = new UploadResult();
        string trustedFileNameForFileStorage;
        var untrustedFileName = file.FileName;
        uploadResult.FileName = untrustedFileName;
        var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

        if(filesProcessed < maxAllowedFiles) {
            if(file.Length == 0) {
                logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                uploadResult.ErrorCode = 1;
            } else if(file.Length > maxFileSize) {
                logger.LogInformation("{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)", trustedFileNameForDisplay, file.Length, maxFileSize);
                uploadResult.ErrorCode = 2;
            } else {
                try {
                    trustedFileNameForFileStorage = Path.GetRandomFileName();
                    var dirPath = Path.Combine(env.ContentRootPath, env.EnvironmentName, "unsafe_uploads");
                    if(!Directory.Exists(dirPath)) {
                        Directory.CreateDirectory(dirPath);
                    }
                    var path = Path.Combine(dirPath, trustedFileNameForFileStorage);

                    await using FileStream fs = new(path, FileMode.Create);
                    await file.CopyToAsync(fs);

                    logger.LogInformation("{FileName} saved at {Path}", trustedFileNameForDisplay, path);
                    uploadResult.Uploaded = true;
                    uploadResult.StoredFileName = trustedFileNameForFileStorage;
                } catch(IOException ex) {
                    logger.LogError("{FileName} error on upload (Err: 3): {Message}", trustedFileNameForDisplay, ex.Message);
                    uploadResult.ErrorCode = 3;
                }
            }
        } else {
            logger.LogInformation("{FileName} not uploaded because the request exceeded the allowed {Count} of files (Err: 4)", trustedFileNameForDisplay, maxAllowedFiles);
            uploadResult.ErrorCode = 4;
        }

        uploadResults.Add(uploadResult);

        return new CreatedResult(resourcePath, uploadResults);
    }
}

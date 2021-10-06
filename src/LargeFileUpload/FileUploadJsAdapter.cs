using LargeFileUpload.Interop;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace LargeFileUpload {
    public class FileUploadJsAdapter {
        /// <summary>
        /// The file upload element.
        /// </summary>
        private readonly FileUpload _fileUpload;

        /// <summary>
        /// Initializes a new instance of <see cref="FileUploadJsAdapter"/>.
        /// </summary>
        /// <param name="fileUpload"></param>
        public FileUploadJsAdapter(FileUpload fileUpload) {
            this._fileUpload = fileUpload;
        }

        /// <summary>
        /// The javascript callback when the files have been selected and will be uploaded.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>void</returns>
        [JSInvokable(nameof(JsUploadStarting))]
        public Task JsUploadStarting(JsFileUploadStarting data) {
            _fileUpload.UploadStarting?.Invoke(new FileUploadStarting(data.Files.Select(f => new UploadingFile(Name: f.Name, Size: f.Size, Type: f.Type)).ToImmutableList()));
            return Task.CompletedTask;
        }

        /// <summary>
        /// The javascript callback when the file upload has been finished.
        /// </summary>
        /// <param name="response">The response data.</param>
        /// <returns>void</returns>
        [JSInvokable(nameof(JsUploadFinished))]
        public Task JsUploadFinished(JsResponse response) {

            Dictionary<string, string> headers = CreateHeaderDictionary(response);

            var uploadedFiles = new FileUploadResult(headers, response.Body, response.StatusCode);
            _ = _fileUpload.FilesUploaded?.Invoke(uploadedFiles);

            return Task.CompletedTask;

            static Dictionary<string, string> CreateHeaderDictionary(JsResponse response) {
                var headers = new Dictionary<string, string>();
                for(var i = 0; i < response.HeaderKeys.Count; i++) {
                    var headerKey = response.HeaderKeys[i];
                    if(headerKey is null) {
                        continue;
                    }

                    headers.Add(headerKey, response.HeaderValues[i]);
                }

                return headers;
            }
        }

        /// <summary>
        /// The javascript callback when anything did go wrong in the connection (not response).
        /// </summary>
        /// <param name="errorData">The error data.</param>
        /// <returns></returns>
        [JSInvokable(nameof(JsErroredUpload))]
        public Task JsErroredUpload(JsError errorData) {
            _ = _fileUpload.FileUploadErrored?.Invoke(new FileUploadError(errorData.Message, errorData.Stack));
            return Task.CompletedTask;
        }
    }
}

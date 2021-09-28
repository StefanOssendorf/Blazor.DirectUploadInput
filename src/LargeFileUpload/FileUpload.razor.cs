using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LargeFileUpload {

    /// <summary>
    /// The code behind file of <see cref="FileUpload"/> component.
    /// </summary>
    public partial class FileUpload : IAsyncDisposable {

        private ElementReference FileInput { get; set; }
        private DotNetObjectReference<FileUpload> FileInputJSReference { get; set; }

#if NET6_0_OR_GREATER
        [EditorRequired]
#endif
        [Parameter]
        public FileUploadSettings UploadSettings { get; set; }

        [Parameter]
        public string Name { get; set; } = string.Empty;

        [Parameter]
        public bool Multiple { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> AllOtherAttributes { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        private Lazy<Task<IJSObjectReference>> _moduleTask;

        /// <inheritdoc />
        protected override void OnInitialized() {

            _moduleTask = new Lazy<Task<IJSObjectReference>>(() => JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/LargeFileUpload/LargeFileUpload.js").AsTask());
            FileInputJSReference = DotNetObjectReference.Create(this);

            base.OnInitialized();
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(firstRender) {
                var headers = new Dictionary<string, string> {
                    { "A", "B" }
                };

                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("attachOnChangeListener", FileInput, UploadSettings
                    with {
                    DotNetHelper = FileInputJSReference,
                    Callbacks = new InteropCallbacks {
                        Starting = nameof(UploadStarting),
                        Finished = nameof(UploadFinished),
                        Errored = nameof(ErroredUpload)
                    },
                    Headers = new Dictionary<string, string> { { "A", "B" } }
                });
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync() {
            if(_moduleTask.IsValueCreated) {
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("removeOnChangeListener", FileInput);
                await module.DisposeAsync();
            }
            FileInputJSReference.Dispose();
        }

        [JSInvokable(nameof(UploadStarting))]
        public Task UploadStarting() { // TODO: Add count of files and maybe names?
            Console.WriteLine("Upload being started received in .Net!");
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(UploadFinished))]
        public Task UploadFinished(JsResponse data) {
            Console.WriteLine("Upload finished received in .Net!");
            Console.WriteLine($"Got data {data.StatusCode.ToString() ?? "<No data>"}");
            // HeaderKeys contain null, filter those out
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(ErroredUpload))]
        public Task ErroredUpload(JsError a) {
            Console.WriteLine("Errored with reason:" + a);
            return Task.CompletedTask;
        }
    }

    public record FileUploadSettings {

        public DotNetObjectReference<FileUpload> DotNetHelper { get; set; }

        public string UploadUrl { get; init; }

        public string HttpMethod { get; init; }

        public string FormName { get; init; } = "files";

        public Dictionary<string, string> Headers { get; init; } = new();

        public InteropCallbacks Callbacks { get; init; }
    }

    public record InteropCallbacks {
        public string Starting { get; init; }
        public string Finished { get; init; }
        public string Errored { get; init; }
    }

    public record JsResponse {
        
        public List<string> HeaderKeys { get; init; }
        public List<string> HeaderValues { get; init; }

        public string Body { get; init; }
        public int StatusCode { get; init;  }
    }

    public record JsError {
        public string Message { get; init; }
        public string Stack { get; init; }
    
    }
}

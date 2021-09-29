using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LargeFileUpload.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LargeFileUpload {

    /// <summary>
    /// The code behind file of <see cref="FileUpload"/> component.
    /// </summary>
    public partial class FileUpload : IAsyncDisposable {

        /// <summary>
        /// The html element reference of the file input.
        /// </summary>
        private ElementReference FileInput { get; set; }

        /// <summary>
        /// The reference to call the dotnet code of this component from javascript.
        /// </summary>
        private DotNetObjectReference<FileUpload> FileInputJSReference { get; set; } = null!;

        /// <summary>
        /// The lazily loaded javascript module.
        /// </summary>
        private Lazy<Task<IJSObjectReference>> _moduleTask = null!;

        /// <summary>
        /// Gets the javascript runtime.
        /// </summary>
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = null!;

        /// <summary>
        /// Sets the settings used by the file upload component.
        /// </summary>
#if NET6_0_OR_GREATER
        [EditorRequired]
#endif
        [Parameter]
        public FileUploadSettings UploadSettings { get; set; } = null!;

        /// <summary>
        /// Sets whether multiple files should be selectable by the file upload component or not.
        /// </summary>
        [Parameter]
        public bool Multiple { get; set; }

        /// <summary>
        /// Sets whether the file upload component is disabled or not.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// Captures all other attributes and passes them to the underlying input tag.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object>? AllOtherAttributes { get; set; }

        /// <summary>
        /// The callback to be notified when the upload is about to start.
        /// </summary>
        [Parameter]
        public Action<FileUploadStarting>? UploadStarting { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized() {

            _moduleTask = new Lazy<Task<IJSObjectReference>>(() => JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/LargeFileUpload/LargeFileUpload.js").AsTask());
            FileInputJSReference = DotNetObjectReference.Create(this);

            base.OnInitialized();
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(firstRender) {
                if(UploadSettings is null) {
                    throw new InvalidOperationException($"The parameter {nameof(UploadSettings)} must be set before the first rendering.");
                }

                IJSObjectReference module = await _moduleTask.Value;

                var jsSettings = new JsFileUploadSettings {
                    UploadUrl = UploadSettings.UploadUrl,
                    FormName = UploadSettings.FormName,
                    HttpMethod = UploadSettings.HttpMethod,
                    Headers = UploadSettings.Headers ?? new Dictionary<string, string>(),
                    DotNetHelper = FileInputJSReference,
                    Callbacks = new InteropCallbacks {
                        Starting = nameof(JsUploadStarting),
                        Finished = nameof(JsUploadFinished),
                        Errored = nameof(JsErroredUpload)
                    }
                };
                await module.InvokeVoidAsync("attachOnChangeListener", FileInput, jsSettings);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync() {
            if(_moduleTask.IsValueCreated) {
                IJSObjectReference module = await _moduleTask.Value;
                await module.InvokeVoidAsync("removeOnChangeListener", FileInput);
                await module.DisposeAsync();
            }

            FileInputJSReference.Dispose();
        }

        /// <summary>
        /// The javascript callback when the files have been selected and will be uploaded.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>void</returns>
        [JSInvokable(nameof(JsUploadStarting))]
        public Task JsUploadStarting(JsFileUploadStarting data) {
            UploadStarting?.Invoke(new FileUploadStarting(data.Files.Select(f => new UploadingFile(Name: f.Name, Size: f.Size, Type: f.Type)).ToImmutableList()));
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(JsUploadFinished))]
        public Task JsUploadFinished(JsResponse response) {
            Console.WriteLine("Upload finished received in .Net!");
            Console.WriteLine($"Got data {response.StatusCode.ToString() ?? "<No data>"}");
            // HeaderKeys contain null, filter those out
            return Task.CompletedTask;
        }

        [JSInvokable(nameof(JsErroredUpload))]
        public Task JsErroredUpload(JsError errorData) {
            Console.WriteLine("Errored with reason:" + errorData);
            return Task.CompletedTask;
        }
    }
}

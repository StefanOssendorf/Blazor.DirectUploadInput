using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using StefanOssendorf.Blazor.DirectUploadInput.Interop;

namespace StefanOssendorf.Blazor.DirectUploadInput {

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
        private DotNetObjectReference<FileUploadJsAdapter> FileInputJSReference { get; set; } = null!;

        /// <summary>
        /// The lazily loaded javascript module.
        /// </summary>
        private Lazy<Task<IJSObjectReference>> _moduleTask = null!;

        /// <summary>
        /// The effective value for strict accept.
        /// </summary>
        private bool EffecticeStrictAccept;

        /// <summary>
        /// Gets the javascript runtime.
        /// </summary>
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = null!;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private ILogger<FileUpload> Logger { get; set; } = null!;

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
        /// Sets the value for the accept attribute used for the upload control.
        /// </summary>
        [Parameter]
        public string? Accept { get; set; }

        /// <summary>
        /// Sets whether the upload is only allowed for file mime-types matching with <see cref="Accept"/>.
        /// </summary>
        [Parameter]
        public bool StrictAccept { get; set; }

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

        /// <summary>
        /// The callback to be notified when the upload has finished.
        /// </summary>
        [Parameter]
        public Func<FileUploadResult, Task>? FilesUploaded { get; set; }

        /// <summary>
        /// The callback when something with the upload request itself went wrong. This callback is <i>NOT</i> invoked when the response indicates failure (e.g. 3XX,4XX or 5XX).
        /// </summary>
        [Parameter]
        public Func<FileUploadError, Task>? FileUploadErrored { get; set; }

        /// <summary>
        /// The callback when the cancellation was done.
        /// </summary>
        [Parameter]
        public Func<Task>? FileUploadCanceled { get; set; }

        /// <inheritdoc />
        protected override void OnInitialized() {

            _moduleTask = new Lazy<Task<IJSObjectReference>>(() => JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/LargeFileUpload/LargeFileUpload.js").AsTask());

            if( StrictAccept && string.IsNullOrWhiteSpace(Accept) ) {
                Logger.LogWarning("You have configured the upload component to use the {StrictAccept} mode but did not provide a value for {Accept}. Strict accept setting will be ignored.", nameof(StrictAccept), nameof(Accept));
            }

            base.OnInitialized();
        }

        /// <inheritdoc />
        [MemberNotNull(nameof(FileInputJSReference))]
        protected override async Task OnAfterRenderAsync(bool firstRender) {

            FileInputJSReference ??= DotNetObjectReference.Create(new FileUploadJsAdapter(this));

            if( firstRender ) {
                if( UploadSettings is null ) {
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
                        Starting = nameof(FileUploadJsAdapter.JsUploadStarting),
                        Finished = nameof(FileUploadJsAdapter.JsUploadFinished),
                        Errored = nameof(FileUploadJsAdapter.JsErroredUpload),
                        Canceled = nameof(FileUploadJsAdapter.JsUploadCanceled)
                    }
                };
                await module.InvokeVoidAsync(InteropFunctionNames.AttachChangeListener, FileInput, jsSettings);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Cancels the current upload.
        /// </summary>
        /// <returns>void</returns>
        public async Task CancelUpload() {
            IJSObjectReference module = await _moduleTask.Value;
            await module.InvokeVoidAsync(InteropFunctionNames.CancelUpload, FileInput).ConfigureAwait(false);
        }

        /// <inheritdoc cref="IAsyncDisposable.DisposeAsync" />
        public async ValueTask DisposeAsync() {
            if( _moduleTask.IsValueCreated ) {
                IJSObjectReference module = await _moduleTask.Value;
                await module.InvokeVoidAsync(InteropFunctionNames.RemoveChangeListener, FileInput);
                await module.DisposeAsync();
            }

            FileInputJSReference.Dispose();
        }

        /// <summary>
        /// Helper type to define available js functions accessible from this component.
        /// </summary>
        private static class InteropFunctionNames {
            public const string AttachChangeListener = "attachOnChangeListener";
            public const string RemoveChangeListener = "removeOnChangeListener";
            public const string CancelUpload = "cancelCurrentUpload";
        }
    }
}

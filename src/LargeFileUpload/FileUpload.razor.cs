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

#if NET6_0_OR_GREATER
        [EditorRequired]
#endif
        [Parameter]
        public FileUploadSettings UploadSettings {  get; set; }

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

            base.OnInitialized();
        }

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(firstRender) {
                var module = await _moduleTask.Value;
                await module.InvokeVoidAsync("attachOnChangeListener", FileInput, UploadSettings);
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
        }
    }

    public record FileUploadSettings {
        public string UploadUrl { get; init; }

        public string HttpMethod { get; init; }

        //public InteropCallbacks Callbacks { get; init; }
    }

    public record InteropCallbacks {

    }
}

# DirectFileUpload
A Blazor (WASM) component designed/intended for directly uploading large files (>500MB) without going into the C# part.


## Why?
Since I needed to upload large files at my work with blazor to our servers I hit the problem that WASM Blazor does not support uploading large files through the C# part to the servers. This is due to the fact that the `HttpClient` buffers the whole file into an array. (For information see this [ASP.Net core issue](https://github.com/dotnet/aspnetcore/issues/35899))

## How to use?
The following is an example on how to use all features provided by the DirectFileUpload component.
You can find a locally running example in the tests-folder.
```csharp
@using StefanOssendorf.Blazor.DirectUploadInput

<DirectFileUpload @ref="@FileUpload" GetUploadSettings="@GetUploadSettings" Multiple="true" Accept="image/jpeg" StrictAccept="true" UploadStarting="@UploadingCallback" FilesUploaded="@UploadFinishedCallback" FileUploadErrored="@UploadeErrored" FileUploadCanceled="@UploadCanceled" />


@code {

	// Callback which will be invoked for every upload operation (but only once for an upload with multiple files) to get the configuration necessary to upload the files
	// If you have only one settings during the lifetime of your whole application cache it to improve performance.
	private Task<FileUploadSettings> GetUploadSettings() {
		return Task.FromResult(new FileUploadSettings {
			HttpMethod = "POST",
			UploadUrl = "api/uploads",
			FormName = "files",
			Headers = new Dictionary<string, string>(),
			FormData = {
				{
					"method", "Yeeehaa"
				}
			}
		});
	}

	private FileUploadStarting? _startUploading;
    // Callback which will be invoked immediately before the actual upload starts with information about the files being uploaded.
	private async void UploadingCallback(FileUploadStarting data) {
		_startUploading = data;

        // Used to reset the example UI
        _fileUploadCanceled = _cancelMessage = string.Empty;
		_fileUploadResult = null;
		_fileUploadError = null;

		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}

	private FileUploadResult? _fileUploadResult;
    // Callback which will be invoked when the upload response got back with the data as a pure string.
	private async Task UploadFinishedCallback(FileUploadResult uploadResult) {
		_fileUploadResult = uploadResult;
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}

	private string _cancelMessage = string.Empty;
	private DirectFileUpload FileUpload { get; set; } = null!;

    // Method to be invoked e.g. by user interaction to cancel a currently running file upload	
    private async Task CancelUpload(){
		await FileUpload.CancelUpload().ConfigureAwait(false);
		_cancelMessage = "Upload canceled";
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}

	private FileUploadError _fileUploadError;

    //	Callback which will be invoked when any kind of error occurred. 
    // ATTENTION: This does not include 3XX,4XX or 5XX responses. These will be handled by the "upload finished" callback!
    private async Task UploadeErrored(FileUploadError fileUploadError) {
		_fileUploadError = fileUploadError;
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}

	private string _fileUploadCanceled = string.Empty;
	
    // Callback which will be invoked when the cancellation was done.
    private async Task UploadCanceled() {
		_fileUploadCanceled = "Upload was canceled";
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);
	}
}
```
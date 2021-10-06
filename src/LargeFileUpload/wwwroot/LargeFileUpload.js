export function attachOnChangeListener(element, settings) {
    element.largeFileUploadFunc = function (ev) { uploadFileToServer(ev.target, settings); };
    addNewAbortController(element);
    element.addEventListener('change', element.largeFileUploadFunc, false);
}
async function uploadFileToServer(element, settings) {
    let abortSignal = element.largeFileUploadAbortController.signal;
    let files = element.files;
    let data = new FormData();
    let startingData = {
        files: []
    };
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        data.append(settings.formName, file);
        startingData.files.push({
            name: file.name,
            size: file.size,
            type: file.type
        });
    }
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.starting, startingData);
    await fetch(settings.uploadUrl, {
        method: settings.httpMethod,
        body: data,
        headers: settings.headers,
        signal: abortSignal
    }).then(response => filesToServerUploaded(response, settings), rejectedReason => uploadToServerFailed(rejectedReason, settings));
    element.value = '';
    addNewAbortController(element);
}
async function uploadToServerFailed(error, settings) {
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.errored, { message: error.message, stack: error.stack });
}
async function filesToServerUploaded(response, settings) {
    let headerKeys = [];
    let headerValues = [];
    response.headers.forEach((val, key) => {
        headerKeys.push(key);
        headerValues.push(val);
    });
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.finished, { headerKeys: headerKeys, headerValues: headerValues, body: await response.text(), statusCode: response.status });
}
export function removeOnChangeListener(element) {
    element.removeEventListener('change', element.largeFileUploadFunc);
    cancelCurrentUpload(element);
}
export function cancelCurrentUpload(element) {
    let controller = element.largeFileUploadAbortController;
    controller.abort();
    addNewAbortController(element);
}
function addNewAbortController(element) {
    element.largeFileUploadAbortController = new AbortController();
}
//# sourceMappingURL=LargeFileUpload.js.map
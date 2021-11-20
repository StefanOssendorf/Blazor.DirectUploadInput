export function attachOnChangeListener(element, settings) {
    element.largeFileUploadChangeFunc = function (ev) { uploadFileToServer(ev.target, settings); };
    addNewAbortController(element);
    element.addEventListener('change', element.largeFileUploadChangeFunc, false);
}
async function uploadFileToServer(element, dotnetBridge) {
    let abortSignal = element.largeFileUploadAbortController.signal;
    let settings = await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.getSettings);
    let files = element.files;
    let data = new FormData();
    let startingData = {
        files: []
    };
    let elementAccepts = element.accept.toLowerCase();
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (settings.strictAccept) {
            var fileType = file.type.toLowerCase();
            if (fileType === "" || !elementAccepts.includes(fileType)) {
                resetElement(element);
                return;
            }
        }
        data.append(settings.formName, file);
        startingData.files.push({
            name: file.name,
            size: file.size,
            type: file.type
        });
    }
    Object.entries(settings.formData).forEach(([k, v]) => {
        data.append(k, v);
    });
    await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.starting, startingData);
    await fetch(settings.uploadUrl, {
        method: settings.httpMethod,
        body: data,
        headers: settings.headers,
        signal: abortSignal
    }).then(response => filesToServerUploaded(response, dotnetBridge), rejectedReason => uploadToServerFailed(rejectedReason, dotnetBridge));
    resetElement(element);
}
async function uploadToServerFailed(error, dotnetBridge) {
    if (error instanceof DOMException && (error.code === DOMException.ABORT_ERR || error.name === "AbortError")) {
        await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.canceled);
    }
    else if (error instanceof Error) {
        await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.errored, { message: error.message, stack: error.stack });
    }
}
async function filesToServerUploaded(response, dotnetBridge) {
    let headerKeys = [];
    let headerValues = [];
    response.headers.forEach((val, key) => {
        headerKeys.push(key);
        headerValues.push(val);
    });
    await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.finished, { headerKeys: headerKeys, headerValues: headerValues, body: await response.text(), statusCode: response.status });
}
export function removeOnChangeListener(element) {
    element.removeEventListener('change', element.largeFileUploadChangeFunc);
    cancelCurrentUpload(element);
}
export function cancelCurrentUpload(element) {
    let controller = element.largeFileUploadAbortController;
    controller.abort();
    addNewAbortController(element);
}
function addNewAbortController(element) {
    let abortController = new AbortController();
    element.largeFileUploadAbortController = abortController;
}
function resetElement(element) {
    element.value = '';
    addNewAbortController(element);
}
//# sourceMappingURL=DirectUploadInput.js.map
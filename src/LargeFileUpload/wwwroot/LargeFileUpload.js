export function attachOnChangeListener(element, settings) {
    element.largeFileUploadFunc = function (ev) { uploadFileToServer(ev.target, settings); };
    element.addEventListener('change', element.lfuFunc, false);
}
async function uploadFileToServer(element, settings) {
    let files = element.files;
    let data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append(settings.formName, files[i]);
    }
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.starting);
    await fetch(settings.uploadUrl, {
        method: settings.httpMethod,
        body: data
    }).then(response => filesToServerUploaded(response, settings), rejectedReason => uploadToServerFailed(rejectedReason, settings));
    element.value = '';
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
    element.removeEventListener('change', element.lfuFuncn);
    console.log('Elements removed');
}
//# sourceMappingURL=LargeFileUpload.js.map
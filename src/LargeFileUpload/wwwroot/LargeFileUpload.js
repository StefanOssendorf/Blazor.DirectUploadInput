export function attachOnChangeListener(element, settings) {
    console.log('I am now attached.');
    element.addEventListener('change', function (ev) { uploadFileToServer(ev.target, settings); }, false);
}
async function uploadFileToServer(element, settings) {
    console.log("On change from input called.");
    console.log("You selected " + element.files.length + " files!");
    console.log("Settings with Method '" + settings.httpMethod + "' and url '" + settings.uploadUrl + "'");
    let files = element.files;
    let data = new FormData();
    if (files.length == 1) {
        data.set('file', files[0]);
    }
    else {
        for (var i = 0; i < files.length; i++) {
            data.set('file' + i, files[i]);
        }
    }
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.starting);
    await fetch(settings.uploadUrl, {
        method: settings.httpMethod,
        body: data
    }).then(response => filesToServerUploaded(response, settings), uploadToServerFailed);
    element.value = '';
}
async function uploadToServerFailed(error) {
    console.log('Fail!');
}
async function filesToServerUploaded(response, settings) {
    console.log('Success!');
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.finished, await response.text());
}
export function removeOnChangeListener(element) {
    console.log('I am now detached.');
}
class FileUploadSettings {
}
class InteropCallbacks {
}
//# sourceMappingURL=LargeFileUpload.js.map
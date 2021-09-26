export function attachOnChangeListener(element, settings) {
    console.log('I am now attached.');
    element.addEventListener('change', function (ev) { uploadFileToServer(ev.target, settings); }, false);
}
async function uploadFileToServer(element, settings) {
    console.log("On change from input called.");
    console.log("You selected " + element.files.length + " files!");
    console.log("Settings with Method '" + settings.httpMethod + "' and url '" + settings.uploadUrl + "'");
}
export function removeOnChangeListener(element) {
    console.log('I am now detached.');
}
class FileUploadSettings {
}
//# sourceMappingURL=LargeFileUpload.js.map
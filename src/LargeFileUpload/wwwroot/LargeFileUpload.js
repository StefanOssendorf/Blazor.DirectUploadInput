export function attachOnChangeListener(element) {
    console.log('I am now attached.');
    element.addEventListener('change', function (ev) { uploadFileToServer(ev.target); }, false);
}
async function uploadFileToServer(element) {
    console.log("On change from input with name '" + element.alt + "' called.");
    console.log("You selected " + element.files.length + " files!");
}
export function removeOnChangeListener(element) {
    console.log('I am now detached.');
}
//# sourceMappingURL=LargeFileUpload.js.map
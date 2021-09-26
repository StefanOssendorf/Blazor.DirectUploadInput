export function attachOnChangeListener(element: HTMLInputElement, settings: FileUploadSettings): void {
    console.log('I am now attached.');
    element.addEventListener('change', function (ev: Event) { uploadFileToServer(ev.target as HTMLInputElement, settings) }, false);
}

async function uploadFileToServer(element: HTMLInputElement, settings: FileUploadSettings) : Promise<void> {
    console.log("On change from input called.");
    console.log("You selected " + element.files.length + " files!");
    console.log("Settings with Method '" + settings.httpMethod + "' and url '" + settings.uploadUrl + "'");
}

export function removeOnChangeListener(element) {
    console.log('I am now detached.');
}

class FileUploadSettings {
    uploadUrl: string;
    httpMethod: string;
}

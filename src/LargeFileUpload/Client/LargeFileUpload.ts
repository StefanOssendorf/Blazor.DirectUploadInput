export function attachOnChangeListener(element: LFUInputElement, settings: FileUploadSettings): void {
    element.largeFileUploadFunc = function (ev: Event) { uploadFileToServer(ev.target as HTMLInputElement, settings) };
    element.addEventListener('change', element.largeFileUploadFunc, false);
}

async function uploadFileToServer(element: HTMLInputElement, settings: FileUploadSettings): Promise<void> {
    let files = element.files;
    let data = new FormData();

    let startingData: FileUploadStarting = {
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

    await fetch(settings.uploadUrl,
        {
            method: settings.httpMethod,
            body: data,
            headers: settings.headers
        }
    ).then(response => filesToServerUploaded(response, settings), rejectedReason => uploadToServerFailed(rejectedReason, settings));

    element.value = '';
}

async function uploadToServerFailed(error: Error, settings: FileUploadSettings) {
    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.errored, { message: error.message, stack: error.stack })
}

async function filesToServerUploaded(response: Response, settings: FileUploadSettings) {
    let headerKeys = [];
    let headerValues = [];
    response.headers.forEach((val, key) => {
        headerKeys.push(key);
        headerValues.push(val);
    });

    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.finished, { headerKeys: headerKeys, headerValues: headerValues, body: await response.text(), statusCode: response.status });
}

export function removeOnChangeListener(element: LFUInputElement) {
    element.removeEventListener('change', element.largeFileUploadFunc);
}

interface FileUploadSettings {
    uploadUrl: string;
    httpMethod: string;
    dotNetHelper: DotNetHelper;
    formName: string;
    callbacks: InteropCallbacks;
    headers: {[name: string]: string}
}

interface InteropCallbacks {
    starting: string;
    finished: string;
    errored: string;
}

interface DotNetHelper {
    invokeMethodAsync(callbackName: string, data?: any, data1?: any): PromiseLike<void>;
}

interface LFUInputElement extends HTMLInputElement {
    largeFileUploadFunc: any;
}

interface FileUploadStarting {
    files: File[];
}

interface File {
    name: string;
    type: string;
    size: number;
}
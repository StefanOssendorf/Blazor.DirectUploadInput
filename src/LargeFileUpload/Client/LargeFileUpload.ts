export function attachOnChangeListener(element: LFUInputElement, settings: FileUploadSettings): void {
    element.largeFileUploadFunc = function (ev: Event) { uploadFileToServer(ev.target as HTMLInputElement, settings) };
    element.addEventListener('change', element.largeFileUploadFunc, false);
}

async function uploadFileToServer(element: HTMLInputElement, settings: FileUploadSettings): Promise<void> {
    let files = element.files;
    let data = new FormData();

    for (var i = 0; i < files.length; i++) {
        data.append(settings.formName, files[i]);
    }

    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.starting);

    await fetch(settings.uploadUrl,
        {
            method: settings.httpMethod,
            body: data
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
    console.log('Elements removed');
}

interface FileUploadSettings {
    uploadUrl: string;
    httpMethod: string;
    dotNetHelper: DotNetHelper;
    formName: string;
    callbacks: InteropCallbacks;
    headers: 
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
export function attachOnChangeListener(element: LFUInputElement, settings: FileUploadSettings): void {
    element.largeFileUploadFunc = function (ev: Event) { uploadFileToServer(ev.target as LFUInputElement, settings) };
    addNewAbortController(element);
    element.addEventListener('change', element.largeFileUploadFunc, false);
}

async function uploadFileToServer(element: LFUInputElement, settings: FileUploadSettings): Promise<void> {
    let abortSignal = element.largeFileUploadAbortController.signal;

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
            headers: settings.headers,
            signal: abortSignal
        }
    ).then(response => filesToServerUploaded(response, settings), rejectedReason => uploadToServerFailed(rejectedReason, settings));

    element.value = '';
    addNewAbortController(element);
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

export function removeOnChangeListener(element: LFUInputElement): void {
    element.removeEventListener('change', element.largeFileUploadFunc);
    cancelCurrentUpload(element);
}

export function cancelCurrentUpload(element: LFUInputElement): void {
    let controller = element.largeFileUploadAbortController;
    controller.abort();
    addNewAbortController(element);
}

function addNewAbortController(element: LFUInputElement): void {
    element.largeFileUploadAbortController = new AbortController();
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
    largeFileUploadAbortController: AbortController;
}

interface FileUploadStarting {
    files: File[];
}

interface File {
    name: string;
    type: string;
    size: number;
}
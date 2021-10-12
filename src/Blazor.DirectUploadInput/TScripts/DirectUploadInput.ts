export function attachOnChangeListener(element: LFUInputElement, settings: FileUploadSettings): void {
    element.largeFileUploadChangeFunc = function (ev: Event) { uploadFileToServer(ev.target as LFUInputElement, settings) };
    addNewAbortController(element);
    element.addEventListener('change', element.largeFileUploadChangeFunc, false);
}

async function uploadFileToServer(element: LFUInputElement, settings: FileUploadSettings): Promise<void> {
    let abortSignal = element.largeFileUploadAbortController.signal;

    let files = element.files;
    let data = new FormData();

    let startingData: FileUploadStarting = {
        files: []
    };

    let elementAccepts = element.accept.toLowerCase();

    for (var i = 0; i < files.length; i++) {
        var file = files[i];

        if (settings.strictAccept) {
            if (!elementAccepts.includes(file.type.toLowerCase())) {
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

    await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.starting, startingData);

    await fetch(settings.uploadUrl,
        {
            method: settings.httpMethod,
            body: data,
            headers: settings.headers,
            signal: abortSignal
        }
    ).then(response => filesToServerUploaded(response, settings), rejectedReason => uploadToServerFailed(rejectedReason, settings));

    resetElement(element);
}

async function uploadToServerFailed(error: DOMException | Error, settings: FileUploadSettings) {
    if (error instanceof DOMException && (error.code === DOMException.ABORT_ERR || error.name === "AbortError")) {
        await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.canceled);
    } else if (error instanceof Error) {
        await settings.dotNetHelper.invokeMethodAsync(settings.callbacks.errored, { message: error.message, stack: error.stack })
    }
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
    element.removeEventListener('change', element.largeFileUploadChangeFunc);
    cancelCurrentUpload(element);
}

export function cancelCurrentUpload(element: LFUInputElement): void {
    let controller = element.largeFileUploadAbortController;
    controller.abort();
    addNewAbortController(element);
}

function addNewAbortController(element: LFUInputElement): void {
    let abortController = new AbortController();
    element.largeFileUploadAbortController = abortController;
}

function resetElement(element: LFUInputElement): void {
    element.value = '';
    addNewAbortController(element);
}

interface FileUploadSettings {
    uploadUrl: string;
    httpMethod: string;
    dotNetHelper: DotNetHelper;
    formName: string;
    strictAccept: boolean;
    callbacks: InteropCallbacks;
    headers: { [name: string]: string };
    formData: { [name: string]: string };
}

interface InteropCallbacks {
    starting: string;
    finished: string;
    errored: string;
    canceled: string;
}

interface DotNetHelper {
    invokeMethodAsync(callbackName: string, data?: any, data1?: any): PromiseLike<void>;
}

interface LFUInputElement extends HTMLInputElement {
    largeFileUploadChangeFunc: any;
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
export function attachOnChangeListener(element: LFUInputElement, settings: FileUploadDotNetBridge): void {
    if (element === null) {
        return;
    }
    element.largeFileUploadChangeFunc = function (ev: Event) { uploadFileToServer(ev.target as LFUInputElement, settings) };
    addNewAbortController(element);
    element.addEventListener('change', element.largeFileUploadChangeFunc, false);
}

async function uploadFileToServer(element: LFUInputElement, dotnetBridge: FileUploadDotNetBridge): Promise<void> {
    let abortSignal = element.largeFileUploadAbortController.signal;

    let settings: FileUploadSettings = await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.getSettings);

    let files = element.files;
    let data = new FormData();

    let startingData: FileUploadStarting = {
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

    await fetch(settings.uploadUrl,
        {
            method: settings.httpMethod,
            body: data,
            headers: settings.headers,
            signal: abortSignal
        }
    ).then(response => filesToServerUploaded(response, dotnetBridge), rejectedReason => uploadToServerFailed(rejectedReason, dotnetBridge));

    resetElement(element);
}

async function uploadToServerFailed(error: DOMException | Error, dotnetBridge: FileUploadDotNetBridge) {
    if (error instanceof DOMException && (error.code === DOMException.ABORT_ERR || error.name === "AbortError")) {
        await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.canceled);
    } else if (error instanceof Error) {
        await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.errored, { message: error.message, stack: error.stack })
    }
}

async function filesToServerUploaded(response: Response, dotnetBridge: FileUploadDotNetBridge) {
    let headerKeys = [];
    let headerValues = [];
    response.headers.forEach((val, key) => {
        headerKeys.push(key);
        headerValues.push(val);
    });

    await dotnetBridge.dotNetHelper.invokeMethodAsync(dotnetBridge.callbacks.finished, { headerKeys: headerKeys, headerValues: headerValues, body: await response.text(), statusCode: response.status });
}

export function removeOnChangeListener(element: LFUInputElement): void {
    if (element === null) {
        return;
    }
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

interface FileUploadDotNetBridge {
    dotNetHelper: DotNetHelper;
    callbacks: InteropCallbacks;
}

interface FileUploadSettings {
    uploadUrl: string;
    httpMethod: string;
    formName: string;
    strictAccept: boolean;
    headers: { [name: string]: string };
    formData: { [name: string]: string };
}

interface InteropCallbacks {
    starting: string;
    finished: string;
    errored: string;
    canceled: string;
    getSettings: string;
}

interface DotNetHelper {
    invokeMethodAsync(callbackName: string, data?: any, data1?: any): PromiseLike<any>;
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
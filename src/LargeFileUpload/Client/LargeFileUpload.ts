export function attachOnChangeListener(element: HTMLInputElement): void {
    console.log('I am now attached.');
    element.addEventListener('change', function (ev: Event) { uploadFileToServer(ev.target as HTMLInputElement) }, false);
}

async function uploadFileToServer(element: HTMLInputElement) : Promise<void> {
    console.log("On change from input called.");
    console.log("You selected " + element.files.length + " files!");
}

export function removeOnChangeListener(element) {
    console.log('I am now detached.');
}


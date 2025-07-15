//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
document.addEventListener("DOMContentLoaded", initialize);

declare const signalR: any;
let jobId: string;
let abortController: AbortController | null = null;
let isImportInProgress: boolean = false;

export function initialize(): void {
    const startImportButton = document.getElementById("importPlanetDataButton") as HTMLElement | null;
    const stopImportButton = document.getElementById("stopImportingPlanetDataButton") as HTMLElement | null;
    progresParHendler();
    startImport(startImportButton);
    stopImport(stopImportButton);
}


function progresParHendler() {
    jobId = crypto.randomUUID();
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`/progresshub?jobId=${jobId}`)
        .build();


    connection.start().catch(err => console.error(err.toString()));

    connection.on("ReceiveProgress", (progress: number) => {
        const progressBar = document.getElementById("importProgress") as HTMLElement;
        progressBar.style.width = progress + "%";
        progressBar.innerText = progress + "%";

        if (progress >= 100) {
            resetUI();
        }
    });
}

function startImport(btn: HTMLElement): void {
    if (btn) {
        btn.addEventListener("click", () => {
            if (isImportInProgress) return;
            isImportInProgress = true;
            abortController = new AbortController();

            const importButton = document.getElementById("importPlanetDataButton") as HTMLButtonElement;
            const progressContainer = document.getElementById("progressContainer") as HTMLElement;
            const stopButton = document.getElementById("stopImportingPlanetDataButton") as HTMLButtonElement;

            importButton.disabled = true;
            progressContainer.style.display = 'block';
            stopButton.disabled = false;
            stopButton.style.display = 'inline-block';

            fetch(`/Planetology/PlanetsCatalog/ImportData_OpenXml?jobId=${jobId}`, {
                method: 'POST',
                signal: abortController.signal
            }).then(response => {
                if (response.ok || response.status === 499) {
                    resetUI();
                }
            }).catch(error => {
                if (error.name === 'AbortError') {
                    console.log('Import aborted by user');
                } else {
                    console.error('Import error:', error);
                }
                resetUI();
            });
        });
    }
}

function stopImport(btn: HTMLElement): void {
    if (btn) {
        btn.addEventListener("click", () => {
            // To stop .fetch from startImport() to free up user resources
            if (abortController) {
                abortController.abort();
            }

            // To stop server process
            fetch(`/Planetology/PlanetsCatalog/CancelImport?jobId=${jobId}`, {
                method: 'POST'
            });

            resetUI();
        });
    }
}


function resetUI(): void {
    const stopButton = document.getElementById("stopImportingPlanetDataButton") as HTMLButtonElement;
    const importButton = document.getElementById("importPlanetDataButton") as HTMLButtonElement;
    const progressContainer = document.getElementById("progressContainer") as HTMLElement;
    const progressBar = document.getElementById("importProgress") as HTMLElement;

    importButton.disabled = false;
    progressContainer.style.display = 'none';
    progressBar.style.width = "0%";
    progressBar.innerText = "0%";
    stopButton.disabled = true;
    stopButton.style.display = 'none';

    isImportInProgress = false;
    abortController = null;

    fetch('/Planetology/PlanetsCatalog/Index', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            PageNumberValue: 1,
            RowOnPageCatalog: 30
        })
    })
        .then(response => response.json())
        .then(data => {
            const tableContainer = document.getElementById('planetsTableContainer') as HTMLElement;
            if (data.tableHtml) {
                tableContainer.innerHTML = data.tableHtml;
            }
        })
        .catch(error => console.error('Error updating table:', error));
}

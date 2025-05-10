//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
//import { distributeItemsIntoColumns } from "./metrics"

export function initialize(): void {
    astronomicCatalogFormHandler();
}

function astronomicCatalogFormHandler() {
    document.addEventListener("click", (e: MouseEvent) => {
        const target = e.target as HTMLElement;
        const paginationCell = target.classList.contains("paginationCell");
        const filterBtn = target.classList.contains("applyFiltersBtn");

        if (paginationCell || filterBtn) {
            const controller = target.getAttribute("data-controller");
            if (controller) {
                const pageButton = paginationCell ? target : null;
                updateCatalogData(controller, pageButton);
            }
        }
    });
}



function getCountChecked_fieldsCheckBoks_Checkbox(): number {
    const container = document.querySelector(".fieldsCheckBoks");
    if (!container) return 0;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    const checkedCount = Array.from(checkboxes).reduce((count, cb) => {
        return count + ((cb as HTMLInputElement).checked ? 1 : 0);
    }, 0);

    return checkedCount + 2; // Becouse some columns have not chackbox
}

function createSpinnerHTML(): string {
    const columnCount: number = getCountChecked_fieldsCheckBoks_Checkbox();
    const spinnerHTML = `
        <tr id="spinnerTr">
            <td colspan="${columnCount}" class="text-center align-items-center" style="height: 20rem;">
                <div id="spinner" class="spinner"> </div>
            </td>
        </tr>
    `;

    return spinnerHTML;
}

interface FormData {
    [key: string]: string | string[];
}

function serializeForm(rootElement: HTMLElement, pageNumber: string): FormData {
    const data: FormData = {};

    // Text inputs, selects (single & multiple), dropdowns
    const inputs = rootElement.querySelectorAll<HTMLInputElement | HTMLSelectElement>('input:not([type="hidden"]), select');


    inputs.forEach(input => {
        const name = input.name || input.id;
        if (!name) return;

        if (input instanceof HTMLSelectElement) {
            if (input.multiple) {
                let selectedValues = Array.from(input.selectedOptions).map(option => option.value);
                data[name] = selectedValues;
            } else {
                data[name] = input.value;
            }
        } else if (input instanceof HTMLInputElement) {
            if (input.type === 'checkbox') {
                if (input.checked) {
                    if (!Array.isArray(data[name])) {
                        data[name] = [];
                    }
                    (data[name] as string[]).push(input.value);
                }
            } else if (input.type === 'radio') {
                if (input.checked) {
                    data[name] = input.value;
                }
            } else {
                data[name] = input.value;
            }
        }
    });


    // Retrieves a boolean value from checkboxes.
    // Boolean values from checkboxes are used in the service that calls the stored procedure to generate a JSON list of selected the Object Types checkboxes.
    // This approach requires separate handling of single checkboxes and the selection of the Constellations parameter in the service that
    //   calls the stored procedure.
    for (const key in data) {
        // Clear arrays with a single element. 
        if (Array.isArray(data[key]) && data[key].length === 1) {
            data[key] = data[key][0];
        }

        // Remove duplicates from arrays.
        if (Array.isArray(data[key])) {
            const uniqueArray = Array.from(new Set(data[key] as string[]));
            data[key] = uniqueArray.length === 1 ? uniqueArray[0] : uniqueArray;
        }
    }

    data["PageNumberVaulue"] = pageNumber.match(/^\d+/)?.[0] ?? '1';
    console.log("serializeForm (after): ", data);
    return data;
}

async function submitFormAndUpdatePartial(form: HTMLElement, url: string, partialSelector: string, pageNumber: string = '1') {
    console.log("FUNCTION: submitFormAndUpdatePartial()");

    const tableHeader = document.querySelector(partialSelector);
    const paginationBodyBlock = document.getElementById("paginationBodyBlockContainer");
    let tableBody = document.getElementById('catalogTableBody') as HTMLTableSectionElement | null;

    // 1. Collect form data into JSON
    const json = serializeForm(form, pageNumber);


    try {
        // 2. Show spinner
        console.log(`Adding spinnerHTML.`);

        // Remove the old spinner, if it exists
        const existingSpinnerTr = document.getElementById("spinnerTr");
        if (existingSpinnerTr) {
            existingSpinnerTr.remove();
        }

        if (tableBody) {
            tableBody.insertAdjacentHTML('afterbegin', createSpinnerHTML());
        }

        // 3. Send request
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(json),
        });

        // 4. Check for success
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        // 5. Receive partial HTML
        const data = await response.json();
        console.log("Fetched data:", data);

        // 6. Insert it into the DOM
        if (tableHeader && data.tableHtml) {
            tableHeader.innerHTML = data.tableHtml;
        } else {
            console.error(`Partial container with selector "${partialSelector}" not found.`);
        }

        if (paginationBodyBlock && data.paginationHtml) {
            paginationBodyBlock.innerHTML = data.paginationHtml;
        } else {
            console.error(`Partial container with selector "paginationBodyBlockContainer" not found.`);
        }

    } catch (error) {
        console.error('Error submitting form or updating partial:', error);
    } finally {
        const spinnerTr = document.getElementById("spinnerTr");
        if (spinnerTr) {
            spinnerTr.remove();
        }
    }
}

function updateCatalogData(catalog: string, pageButton: HTMLElement) {
    const form = document.querySelector('.topMenuFiltersCatalogs') as HTMLElement;
    if (!form) return;
    console.log(form);

    let pageNumber = '1';

    if (pageButton)
        pageNumber = pageButton.textContent?.trim() || '1';

    if (catalog === 'NGCICOpendatasofts')
        submitFormAndUpdatePartial(form, '/Catalogs/NGCICOpendatasofts/Index', '#sizeFilterTable', pageNumber);

    if (catalog === 'CollinderCatalogs')
        submitFormAndUpdatePartial(form, '/Catalogs/CollinderCatalogs/Index', '#sizeFilterTable', pageNumber);
}

/// <reference types="jquery" />

let remInPixels: number;

document.addEventListener("DOMContentLoaded", () => {
    remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);





    /////////////////////////////////////////////
    /// DEBUGING
    /////////////////////////////////////////////
    console.log("GLOBAL SCOPE");

    document.addEventListener("click", (e) => {
        const el = e.target as HTMLElement;
        console.log(`AT ${new Date().toLocaleTimeString()} CLICKED ON:`, el);
        console.log("CLOSEST .restrictedContent:", el.closest(".restrictedContent"));
    });


    fixSelectBehavior();
    alertUnauthorizedAccess();
    //updateTableHeaderOffset();
});

// The metrics functions
export function setElemSize(): void {
    updateFixedColumnLeftOffset();
    adjustTableSize(remInPixels);
    distributeItemsIntoColumns(".responsiveColumnsContainer", remInPixels);
};

window.addEventListener('resize', () => {
    remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
    distributeItemsIntoColumns(".responsiveColumnsContainer", remInPixels);
    handleResize(remInPixels);
});



/////////////////////////////////////////////
// Style block
/////////////////////////////////////////////
// To solve problem with the background color of selected items in browser.
// Used in:
//      Admin:
//          Role: Create, Edit;
//          User: Create, Edit;
function fixSelectBehavior(): void {
    let selects: NodeListOf<HTMLSelectElement> = document.querySelectorAll("select.form-select"); // Can be used with "select.form-control"

    selects.forEach(select => {
        if (!select.multiple) return;

        let selectedValues: Set<string> = new Set();


        for (let option of Array.from(select.options)) {
            if (option.hasAttribute("selected") || option.selected) {
                selectedValues.add(option.value);
                option.selected = true;
                option.classList.add("selected-fix");
            }
        }

        select.addEventListener("change", function () {
            for (let option of Array.from(select.options)) {
                if (option.selected) {
                    selectedValues.add(option.value);
                    option.classList.add("selected-fix");
                } else {
                    selectedValues.delete(option.value);
                    option.classList.remove("selected-fix");
                }
            }
        });

        select.addEventListener("mousedown", function (event: MouseEvent) {
            event.preventDefault();

            let target = event.target as HTMLElement;

            if (target.tagName === "OPTION") {
                let option = target as HTMLOptionElement;

                if (selectedValues.has(option.value)) {
                    selectedValues.delete(option.value);
                    option.selected = false;
                    option.classList.remove("selected-fix");
                } else {
                    selectedValues.add(option.value);
                    option.selected = true;
                    option.classList.add("selected-fix");
                }
            }

            return false; // Prevents selected values ​​from "disappearing"
        });
    });
};



/////////////////////////////////////////////
// Access block
/////////////////////////////////////////////
// Prohibiting unauthorized access.
function alertUnauthorizedAccess(): void {
    console.log("FUNCTION: alertUnauthorizedAccess()");
    const restrictedElements = document.querySelectorAll<HTMLElement>(".restrictedWrapper");

    if (typeof (window as any).isAuthenticated !== "undefined" && !(window as any).isAuthenticated) {
        console.log(`FUNCTION: alertUnauthorizedAccess(): isAuthenticated = ${(window as any).isAuthenticated}`);

        restrictedElements.forEach(wrapper => {
            const mask = document.createElement("div");
            mask.classList.add("restrictedMask");

            mask.addEventListener("click", (e) => {
                e.preventDefault();
                e.stopPropagation();
                showAlert("Filters are unavailable for unregistered users. Please sign in.");
            });

            const buttons = wrapper.querySelectorAll("button, input, a");
            buttons.forEach(btn => {
                (btn as HTMLButtonElement).disabled = true;
            });

            wrapper.appendChild(mask);
        });
    }
}



/////////////////////////////////////////////
// Block allocation for the maximum number of columns
/////////////////////////////////////////////
function distributeItemsIntoColumns(containerSelector: string, remInPixels: number, minColumnWidth: number = (remInPixels * 26)) {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const items = Array.from(container.querySelectorAll("p"));
    if (items.length === 0) return;

    // Remove all <p> elements from the DOM before rebuilding.
    items.forEach(item => {
        if (item.parentNode) {
            item.parentNode.removeChild(item);
        }
    });

    // Determine the number of columns.
    const containerWidth = container.clientWidth;
    const columnCount = Math.floor(containerWidth / minColumnWidth) || 1;

    // Create a wrapper for the columns.
    const wrapper = document.createElement("div");
    wrapper.className = "responsive-columns-wrapper";

    // Create the columns.
    const columns: HTMLDivElement[] = [];
    for (let i = 0; i < columnCount; i++) {
        const col = document.createElement("div");
        col.className = "responsive-column";
        columns.push(col);
        wrapper.appendChild(col);
    }

    // Distribute the <p> elements into the columns in order.
    items.forEach((item, i) => {
        columns[i % columnCount].appendChild(item);
    });

    // Clear the container and insert the new structure.
    container.innerHTML = "";
    container.appendChild(wrapper);

    // Set the column widths taking the gap into account.
    const columnWidth = Math.floor((containerWidth) / columnCount - (remInPixels * 5));
    columns.forEach(col => {
        col.style.width = `${columnWidth}px`;
    });
}



/////////////////////////////////////////////
// Table size block
/////////////////////////////////////////////
// Set the margin for the table header row so that it does not overlap with the page header.
function updateTableHeaderOffset(): void {
    const header = document.querySelector('.toFixLayoutHeader') as HTMLElement | null;
    const tableHeader = document.querySelector('table.table-toFixRows-1.table-fixed-headers thead') as HTMLElement | null;

    if (header && tableHeader) {
        const headerHeight = header.offsetHeight; // Getting the height of the header
        tableHeader.style.top = `${headerHeight}px`;
    }
}

// Setting left offset for the 2nd and subsequent fixed columns:
function updateFixedColumnLeftOffset(): void {
    console.log("FUNCTION: updateFixedColumnLeftOffset()");
    const fixedColumns = 2;
    let offset = 0;

    // Getting the value of 1em in pixels.
    const emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
    const reductionStep = 0.1 * emInPixels; // 0.1em in pixels.
    let totalReduction = 0;

    for (let i = 1; i <= fixedColumns; i++) {
        const cells = document.querySelectorAll<HTMLElement>(`th:nth-child(${i}), td:nth-child(${i})`);
        if (cells.length > 0) {
            const width = cells[0].offsetWidth;
            offset += width;

            // Setting left for each column, subtracting cumulative reduction.
            cells.forEach(cell => {
                cell.style.left = `${offset - width - totalReduction}px`;
            });

            totalReduction += reductionStep; // Increasing the reduction for the next column.
        }
    }
}

// Table size adjustment function:
function adjustTableSize(remInPixels: number): void {
    console.log("FUNCTION: adjustTableSize()");
    const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
    if (!header) { console.log("ATTENTION!!!The header variable is null!") };
    const footer = document.querySelector("footer") as HTMLElement | null;
    if (!footer) { console.log("ATTENTION!!! The footer variable is null!") };
    const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;
    if (!main) { console.log("ATTENTION!!! The main variable is null!") };
    const tableContainer = document.querySelector(".table-set-size") as HTMLElement | null;
    if (!tableContainer) { console.log("ATTENTION!!! The tableContainer variable is null!") };
    const pageContent = document.querySelector(".catalogPagesHeader") as HTMLElement | null;
    if (!pageContent) { console.log("ATTENTION!!! The pageContent variable is null!") };

    if (!tableContainer || !header || !footer || !main || !pageContent) return;

    requestAnimationFrame(() => {
        const windowHeight = window.innerHeight;
        const windowWidth = window.innerWidth;

        const headerHeight = header.scrollHeight;
        const footerHeight = footer.scrollHeight;

        const mainStyles = getComputedStyle(main);
        const mainPaddingV = parseFloat(mainStyles.paddingTop) + parseFloat(mainStyles.paddingBottom);
        const mainMarginV = parseFloat(mainStyles.marginTop) + parseFloat(mainStyles.marginBottom);

        const mainPaddingH = parseFloat(mainStyles.paddingLeft) + parseFloat(mainStyles.paddingRight);
        const mainMarginH = parseFloat(mainStyles.marginLeft) + parseFloat(mainStyles.marginRight);

        const pageContentH = pageContent.scrollHeight;

        const extraHeight = 6 * remInPixels; // Unclear missing space.
        const extraPadding = 0 * remInPixels; // Backup margin.

        const tableHeight = windowHeight - (headerHeight + footerHeight + mainPaddingV + mainMarginV + pageContentH + extraPadding + extraHeight);
        const tableWidth = windowWidth - (mainPaddingH + mainMarginH + extraPadding);

        console.log({
            remInPixels,
            footerHeight,
            headerHeight,
            mainMarginH,
            mainMarginV,
            mainPaddingH,
            mainPaddingV,
            pageContentH,
            tableHeight,
            tableWidth,
            windowHeight,
            windowWidth,
        });

        tableContainer.style.height = `${tableHeight}px`;
        tableContainer.style.width = `${tableWidth}px`;
    });
}

// Executing on window resize.
function handleResize(remInPixels: number): void {
    updateFixedColumnLeftOffset();
    adjustTableSize(remInPixels);
}

// ***** Tracking size changes in header, footer, and main. *****
const resizeObserver = new ResizeObserver(() => adjustTableSize(remInPixels));

const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
const footer = document.querySelector("footer") as HTMLElement | null;
const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;

if (header) resizeObserver.observe(header);
if (footer) resizeObserver.observe(footer);
if (main) resizeObserver.observe(main);



/////////////////////////////////////////////
// Alert block
/////////////////////////////////////////////
declare const Swal: any;

function showAlert(message: string): void {
    console.log("FUNCTION: showAlert()");
    const isDarkTheme = localStorage.getItem("theme") === "dark";

    if (typeof Swal !== "undefined" && Swal.fire) {

        Swal.fire({
            title: "Attention!",
            text: message,
            icon: "info",
            background: isDarkTheme ? "#414243" : "white",
            color: isDarkTheme ? "red" : "black",
            confirmButtonColor: isDarkTheme ? "red" : "blue",
            confirmButtonText: "OK"
        });
    } else {

        alert(message);
    }
}



/////////////////////////////////////////////
// SignalR Progress Import block
/////////////////////////////////////////////
declare const signalR: any;

const jobId: string = crypto.randomUUID();
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`/progresshub?jobId=${jobId}`)
    .build();

let abortController: AbortController | null = null;
let isImportInProgress: boolean = false;

connection.start().catch(err => console.error(err.toString()));

connection.on("ReceiveProgress", (progress: number) => {
    const progressBar = document.getElementById("importProgress") as HTMLElement;
    progressBar.style.width = progress + "%";
    progressBar.innerText = progress + "%";

    if (progress >= 100) {
        resetUI();
    }
});

function startImport(): void {
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

    fetch(`/Planets/PlanetsCatalog/ImportData_OpenXml?jobId=${jobId}`, {
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
}

function stopImport(): void {
    // To stop .fetch from startImport() to free up user resources
    if (abortController) {
        abortController.abort();
    }

    // To stop server process
    fetch(`/Planets/PlanetsCatalog/CancelImport?jobId=${jobId}`, {
        method: 'POST'
    });

    resetUI();
}


(window as any).startImport = startImport;
(window as any).stopImport = stopImport;


function resetUI(): void {
    const importButton = document.getElementById("importPlanetDataButton") as HTMLButtonElement;
    const progressContainer = document.getElementById("progressContainer") as HTMLElement;
    const progressBar = document.getElementById("importProgress") as HTMLElement;
    const stopButton = document.getElementById("stopImportingPlanetDataButton") as HTMLButtonElement;

    importButton.disabled = false;
    progressContainer.style.display = 'none';
    progressBar.style.width = "0%";
    progressBar.innerText = "0%";
    stopButton.disabled = true;
    stopButton.style.display = 'none';

    isImportInProgress = false;
    abortController = null;

    fetch('/Planets/PlanetsCatalog/GetPlanetsTable')
        .then(response => response.text())
        .then(html => {
            const tableContainer = document.getElementById('planetsTableContainer') as HTMLElement;
            tableContainer.innerHTML = html;
        })
        .catch(error => console.error('Error updating table:', error));
}



/////////////////////////////////////////////
// Form-handler functions block
/////////////////////////////////////////////
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
}

(window as any).updateCatalogData = updateCatalogData;



/////////////////////////////////////////////
// Development block
/////////////////////////////////////////////
interface AjaxResponse {
    responseText?: string;
    status?: number;
    statusText?: string;
}


(window as any).fetchDateUsingADO = fetchDateUsingADO;
(window as any).fetchDateUsingEF = fetchDateUsingEF;
(window as any).executeCreateNewDateProcedure = executeCreateNewDateProcedure;



function fetchDateUsingADO(): void {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureADO',
        type: 'GET'
    })
        .done((data: any) => showAlert(data))
        .fail((data: AjaxResponse) => {
            showAlert("ADO. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
            console.log(data);
        })
        .always(() => console.log("Request execution completed!"));
}

function fetchDateUsingEF(): void {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureEF',
        type: 'GET'
    })
        .done((data: any) => showAlert(data))
        .fail((data: AjaxResponse) => {
            showAlert("EF. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
            console.log(data);
        });
}

function executeCreateNewDateProcedure(): void {
    $.ajax({
        url: '/Admin/HomeAdmin/CallCreateNewDateProcedure',
        type: 'GET'
    })
        .done((data: any) => {
            $("#dataTableContainer").html(data);
        })
        .fail(() => {
            showAlert("AN ERROR OCCURRED WHEN THE CallCreateNewDateProcedure WAS CALLED. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
        });
}















////////////////////////////////////////////////////////////////////////////////////////////
// Table column description messages
////////////////////////////////////////////////////////////////////////////////////////////
/// NGC2000 ///
document.addEventListener("click", function (event) {
    const target = event.target as HTMLElement;
    if (!target.classList) return;

    const alertMessages = new Map<string, string>([
        ["Name_UK", "Other unique name (taken from the NGC2000_UK catalog. Analogous to the Othernames field). THESE TWO FIELDS SHOULD BE MERGED EVENTUALLY!!!"],
        ["Comment", "Comment from the NameObject table."],
        ["SourceType", "Object type. See the SourceType table."],
        ["Other_names", "Name from other catalogs or historically established. In this case, from the Object field of the NameObject table."],
        ["LimitAngDiameter", "This field contains the character \"<\" if the object's size is an upper limit; otherwise, the field is empty."],
        ["AngDiameter", "This field contains the size of the object in arcminutes. It is the angular size measured along the largest dimension. It is taken from the NGC2000_UK catalog."],
        ["RA", "RA or LII – Galactic longitude of the NGC/IC object – right ascension (analogous to longitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 0.1 minutes of time in the original table. Measured in hours."],
        ["DEC", "DEC or BII – Galactic latitude of the NGC/IC object – declination (analogous to latitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 1 arcminute in the original table."],
        ["App_Mag", "This field contains the integrated (total) magnitude of the type specified in the app_mag_flag field. Accuracy varies."],
        ["App_Mag_Flag", "This field contains a flag indicating the magnitude type. It contains a space if the integrated magnitude is visual, or \"p\" if it is photographic (blue)."],
        ["b_mag", "This field contains the brightness of the object in the blue range (B-band, \"Blue\"), approximately 440 nm."],
        ["v_mag", "This field contains the stellar magnitude of the object in the visible range (V-band, \"Visual\"), around 550 nm."],
        ["j_mag", "This field contains the brightness of the object in the infrared range (J-band), with a wavelength of about 1.25 μm."],
        ["h_mag", "This field contains the brightness of the object in an even longer infrared range (H-band), around 1.65 μm."],
        ["k_mag", "This field contains the brightness of the object further into the infrared range (K-band), around 2.2 μm."],
        ["Surface_Brigthness", "It indicates the average brightness of an object per unit area: the brightness of the object distributed over a unit area in the sky (usually expressed in magnitudes per square arcsecond, e.g., mag/arcsec²). The lower the value, the brighter the object appears per unit area!"],
        ["Hubble_OnlyGalaxies", "Hubble type of the galaxy."],
        ["Cstar_UMag", "This field contains the brightness (stellar magnitude) of the nearest star in the ultraviolet range (U-band)."],
        ["Cstar_BMag", "This field contains the brightness of the nearest star in the blue range (B-band, \"Blue\")."],
        ["Cstar_VMag", "This field contains the brightness of the nearest star in visible light (V-band, \"Visual\")."],
        ["Cstar_Names", "This field contains the names or identifiers of the star specified in the fields: Cstar_UMag, Cstar_BMag, Cstar_VMag."]
    ]);

    for (const [className, message] of alertMessages.entries()) {
        if (target.classList.contains(className)) {
            showAlert(message);
            break;
        }
    }
});
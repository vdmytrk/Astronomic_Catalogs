//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
import { dataVisualization } from "./planetarySystemVisualization";
import { planetSysVisualizationTheme } from "./theme";
import { hideShowBlockDownAnime } from "./behavior";

//sessionStorage.getItem("isSysVisualization") === "true";

if (!(window as any).__formHandlerInitialized) {
    (window as any).__formHandlerInitialized = true;

    document.addEventListener("DOMContentLoaded", () => {
        sessionStorage.setItem("isSysVisualization", String(false));
        astronomicCatalogFormHandler();
    });
}

function astronomicCatalogFormHandler() {
    console.log('FUNCTION: astronomicCatalogFormHandler initialized');

    document.addEventListener("click", (e: MouseEvent) => {
        const target = e.target as HTMLElement;
        if (!target) return;

        console.log("############");
        console.log("FUNCTION: astronomicCatalogFormHandler - CLICK AT:");
        console.log(new Date().toLocaleString());

        let controller = target.getAttribute("data-controller");
        const isVisualization = sessionStorage.getItem("isSysVisualization") === "true"; // Current value
        console.log("🔁 isSysVisualization (before toggle):", isVisualization);

        const plSysVisualizationBtn = target.closest(".platetarySystemVisualization") as HTMLElement;
        const currentPageElement = document.querySelector(".paginationBodyBlock .currentPage") as HTMLElement | null;
        const paginationCell = target.classList.contains("paginationCell");
        const filterBtn = target.classList.contains("applyFiltersBtn");
        const pageButton = paginationCell ? target : currentPageElement;

        if (filterBtn || (paginationCell && !isVisualization)) {
            console.log("   ✅ filterBtn || (paginationCell && !isSysVisualization)");
            updateCatalogData(controller, pageButton);
        }

        if (plSysVisualizationBtn || (paginationCell && isVisualization)) {
            console.log("   ✅ plSysVisualizationBtn || (paginationCell && isSysVisualization)");

            const isPlSysVisBtn: boolean = !!plSysVisualizationBtn;

            let newIsVisualization: boolean;
            if (!paginationCell)
                newIsVisualization = toggleSysVisualization();
            console.log(`🔁 Toggle: isSysVisualization = ${newIsVisualization}`); // New value

            let parameters: FormData | null;
            if (newIsVisualization) {
                controller = controller + "_Visualization";
                parameters = parametersInitialization(isPlSysVisBtn, target);

                console.log(`Controller (if): ${controller}`);
                updateCatalogData(controller, pageButton, parameters);
            } else {
                console.log(`Controller (else): ${controller}`);
                updateCatalogData(controller, pageButton, parameters);
            }

            if (plSysVisualizationBtn) {
                const table = document.querySelector<HTMLSelectElement>('#planetsSystemTableContainer');
                console.log(` ⚠️ table = ${table}`);
                hideShowBlockDownAnime(table, false);
                planetSysVisualizationTheme(newIsVisualization);
            }
        }
    });
}

function toggleSysVisualization(): boolean {
    const current = sessionStorage.getItem("isSysVisualization") === "true";
    const next = !current;
    sessionStorage.setItem("isSysVisualization", String(next));

    return next;
}

interface FormData {
    [key: string]: string | string[];
}

function parametersInitialization(isPlSysVisBtn: boolean, target: HTMLElement | null): FormData | null {
    console.log('FUNCTION: parametersInitialization');
    let parameters = {};
    let dataParams = target?.getAttribute("data-parameters");
    console.log("data-parameters =", dataParams);


    if (isPlSysVisBtn) {
        console.log(`⚠️ First load of the _PlanetarySystemVisualizationBase page'`);
        const form = document.querySelector('.topMenuFiltersCatalogs') as HTMLElement | null;
        if (!form) return null;
        parameters = serializeForm(form, "0");
    } else if (dataParams) {
        try {
            parameters = JSON.parse(dataParams) as FormData;
            console.log(`✅ dataParams is valid.`);
        } catch (err) {
            console.error("⛔ Invalid JSON in data-parameters:", err);
        }
    }

    return parameters;
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

export function createSpinnerHTML(): string {
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

function showGlobalSpinner() {
    const spinner = document.getElementById("global-spinner-overlay");
    if (spinner) spinner.style.display = "flex";
}

function hideGlobalSpinner() {
    const spinner = document.getElementById("global-spinner-overlay");
    if (spinner) spinner.style.display = "none";
}

export function serializeForm(rootElement: HTMLElement | null, pageNumber: string): FormData {
    let data: FormData = {};

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

    const isVisualization = sessionStorage.getItem("isSysVisualization") === "true";
    if (isVisualization) {
        data = getPartialViewName(data, null);
    } else {
        const checkbox = document.querySelector("label.switchTableType input[type='checkbox']") as HTMLInputElement;
        if (checkbox)
            data = getPartialViewName(data, checkbox);
    }

    data["PageNumberValue"] = pageNumber.match(/^\d+/)?.[0] ?? '1';

    console.log("serializeForm (after): ", data);
    return data;
}

function getPartialViewName(data, checkbox?: HTMLInputElement): FormData {
    console.log("FUNCTION: getPartialViewName");

    const isVisualization = sessionStorage.getItem("isSysVisualization") === "true";
    if (isVisualization) {
        console.log(`  >>> IS VISUALIZATION: ${isVisualization}`);
        data["PartialViewName"] = "_PlanetarySystemVisualizationBase";
    } else if (checkbox) {
        console.log(`  >>> IS VISUALIZATION: ${isVisualization}`);
        data["PartialViewName"] = checkbox.checked ? "_PlanetarySystemTable" : "_PlanetarySystemTableInGroups";
    }
    console.log(`PartialViewName: ${data["PartialViewName"]}`);
    return data;
}

async function submitFormAndUpdatePartial(
    form: HTMLElement | null,
    url: string,
    partialSelector: string,
    pageNumber: string = '1',
    parameters?: FormData)
{
    console.log(`FUNCTOIN: submitFormAndUpdatePartial(form: ${form}, url: ${url}, partialSelector: ${partialSelector}, pageNumber: ${pageNumber}!!!`);
    const tableHeader = document.querySelector(partialSelector);
    const paginationBodyBlock = document.getElementById("paginationBodyBlockContainer");
    const plSysVisualizationBtn = document.querySelector(".platetarySystemVisualization") as HTMLElement;
    let json: FormData;

    // 1. Collect input data into JSON
    if (form) {
        console.log("  >>>  FORM IS NOT NULL");
        json = serializeForm(form, pageNumber);
    } else {
        console.log("  >>>  FORM IS NULL");
        const fallbackParams: FormData = {
            ...(parameters ?? {}) // If `parameters` is undefined — create an empty object.
        };

        fallbackParams["PartialViewName"] = "_PlanetarySystemVisualizationBase";
        fallbackParams["PageNumberValue"] = pageNumber.match(/^\d+/)?.[0] ?? '1';
        json = fallbackParams;
    }

    try {
        // 2. Show spinner
        showGlobalSpinner();

        console.log(`JSON argument: ${JSON.stringify(json)}`);

        // 3. Send request
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(json),
        });

        // 4. Check for success
        if (!response.ok) {
            console.log(`4. Check for success. response.ok = ${response.ok}`);
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        // 5. Receive partial HTML
        const data = await response.json();
        console.log("Fetched data:", data);

        // 6. Check if redirect is needed
        if (data.redirectTo) {
            console.log("Redirecting to:", data.redirectTo);
            window.location.href = data.redirectTo;
            return; // Exit function to prevent further execution
        }

        // 7. Insert received HTML into the DOM
        if (paginationBodyBlock)
            paginationBodyBlock.innerHTML = data.paginationHtml;
        else
            console.error(`Partial container with selector "paginationBodyBlockContainer" not found.`);

        if (tableHeader && data.tableHtml) {
            tableHeader.innerHTML = data.tableHtml;

            if (plSysVisualizationBtn) {
                plSysVisualizationBtn.setAttribute("data-parameters", JSON.stringify(json));
                dataVisualization();

                // ✅ ⬇️ Після всього — показати вміст
                const table = document.querySelector<HTMLElement>('#planetsSystemTableContainer');
                hideShowBlockDownAnime(table, true);
            }
        } else {
            console.error(`Partial container with selector "${partialSelector}" not found.`);
        }

    } catch (error) {
        console.error('Error submitting form or updating partial:', error);

        try {
            const errorMessage = "An unexpected error occurred on the server. The server did not return a response. Please try again later or contact support.";
            const encodedMessage = encodeURIComponent(errorMessage);

            const errorUrl = `/Error/Exception?errorMessage=${encodedMessage}`;
            window.location.href = errorUrl;
        }
        catch (error) {
            const container = document.body;
            container.innerHTML = `
                <div style="font-family: Arial, sans-serif; padding: 40px; text-align: center;">
                    <h1 style="color: #d9534f;">Oops! Something went wrong.</h1>
                    <p style="font-size: 18px;">An unexpected error occurred on the server.<br>
                    The server did not return a response.<br>
                    Please try again later or contact support.</p>
                    <button onclick="location.reload()">Try Again</button>
                </div>
            `;
        }
    } finally {
        hideGlobalSpinner();
    }
}

function updateCatalogData(catalog: string, pageButton: HTMLElement | null, parameters?: FormData) {
    const form = document.querySelector('.topMenuFiltersCatalogs') as HTMLElement;
    if (!form && !parameters) return;

    const pageNumber = pageButton?.textContent?.trim() || '1';

    console.log(`updateCatalogData(catalog = ${catalog}, form = ${form}, pageNumber = ${pageNumber}): `);

    if (catalog === 'NGCICOpendatasofts') {
        console.log("  >>> catalog === 'NGCICOpendatasofts'");
        submitFormAndUpdatePartial(form, '/Catalogs/NGCICOpendatasofts/Index', '#sizeFilterTable', pageNumber);
    }

    if (catalog === 'CollinderCatalogs') {
        console.log("  >>> catalog === 'CollinderCatalogs'");
        submitFormAndUpdatePartial(form, '/Catalogs/CollinderCatalogs/Index', '#sizeFilterTable', pageNumber);
    }

    if (catalog === 'PlanetsCatalog') {
        console.log("  >>> catalog === 'PlanetsCatalog'");
        submitFormAndUpdatePartial(form, '/Planetology/PlanetsCatalog/Index', '#sizeFilterTable', pageNumber);
    }

    if (catalog === 'PlanetarySystem') {
        console.log("  >>> catalog === 'PlanetarySystem'");
        submitFormAndUpdatePartial(form, '/Planetology/PlanetarySystem/Index', '#sizeFilterTable', pageNumber);
    }

    if (catalog === 'PlanetarySystem_Visualization') {
        console.log("  >>> catalog === 'PlanetarySystem_Visualization'");
        submitFormAndUpdatePartial(null, '/Planetology/PlanetarySystem/PlanetarySystemVisualization', '#sizeFilterTable', pageNumber, parameters);
    }
}

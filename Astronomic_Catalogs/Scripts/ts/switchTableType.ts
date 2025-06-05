//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
import * as formHandler from "./formHandler";
import { hideShowBlockDownAnime } from "./behavior";

document.addEventListener("DOMContentLoaded", () => {
    initialize();
});

function initialize(): void {
    console.log("FUNCTION: switchTable");
    const checkbox = document.querySelector("label.switchTableType input[type='checkbox']") as HTMLInputElement;

    if (!checkbox) return;

    checkbox.addEventListener("change", async () => {
        switchTable(checkbox);
    });
};

function switchTable(checkbox: HTMLInputElement): void {
    const container = document.getElementById("twoPlanetarySystemTable") as HTMLElement | null;
    if (!container) return;

    const form = document.querySelector('.topMenuFiltersCatalogs') as HTMLElement;
    if (!form) return;
    console.log(form);

    let pageNumber = '1';
    const currentPageElement = document.querySelector(".paginationBodyBlock .currentPage");
    if (currentPageElement) {
        pageNumber = currentPageElement.textContent?.trim() || pageNumber;
    }

    const showGrouped = checkbox.checked;
    const url = showGrouped
        ? "/Planetology/PlanetarySystem/GetFlatTable"
        : "/Planetology/PlanetarySystem/GetGroupedTable";

    submitFormAndUpdatePartialSwitchTable(form, url, container, showGrouped, pageNumber);

    const fieldsCheckBoksBlock = document.querySelector('.fieldsCheckBoksBlock') as HTMLElement | null;
    if (showGrouped)
        hideShowBlockDownAnime(fieldsCheckBoksBlock, true, 2);
    else
        hideShowBlockDownAnime(fieldsCheckBoksBlock, false, 2);

};

async function submitFormAndUpdatePartialSwitchTable(form: HTMLElement, url: string, container: HTMLElement, showGrouped: boolean, pageNumber: string = '1') {
    console.log(`FUNCTOIN: submitFormAndUpdatePartial (SwitchTable)_(form: ${form}, url: ${url}, container: ${container}, pageNumber: ${pageNumber}!!!`);
    let tableBody = document.getElementById('catalogTableBody') as HTMLTableSectionElement | null;

    // 1. Collect form data into JSON
    const json = formHandler.serializeForm(form, pageNumber);

    try {
        // 2. Show spinner
        console.log(`Adding spinnerHTML.`);

        // Remove the old spinner, if it exists
        const existingSpinnerTr = document.getElementById("spinnerTr");
        if (existingSpinnerTr) {
            existingSpinnerTr.remove();
        }

        if (tableBody) {
            tableBody.insertAdjacentHTML('afterbegin', formHandler.createSpinnerHTML());
        }

        // 3. Send request      TypeError: Failed to execute 'fetch' on: HEAD method cannot have body.
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(json),
        });
        //const response = await fetch(url);

        // 4. Check for success
        if (!response.ok)
            throw new Error(`HTTP error! Failed to load table. Status: ${response.status}`);

        // 5. Receive partial HTML
        const html = await response.text();

        // 6. Insert it into the DOM
        container.innerHTML = showGrouped
            ? `<div id="planetsSystemTableContainer" class="hideShowColumnTable p-3">${html}</div>`
            : `<div id="planetsInGroupsSystemTableContainer" class="p-3">${html}</div>`;

    } catch (error) {
        container.innerHTML = "<div class='text-danger p-3'>Error loading data</div>";
        console.error('Error submitting form or updating partial:', error);
    } finally {
        const spinnerTr = document.getElementById("spinnerTr");
        if (spinnerTr) {
            spinnerTr.remove();
        }
    }
}

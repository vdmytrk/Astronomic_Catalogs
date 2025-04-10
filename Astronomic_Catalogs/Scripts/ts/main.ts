document.addEventListener("DOMContentLoaded", () => {
    /////////////////////////////////////////////
    // Style block
    /////////////////////////////////////////////
    // To solve problem with the background color of selected items in browser.
    function FixSelectBehavior(): void {
        let selects: NodeListOf<HTMLSelectElement> = document.querySelectorAll("select.form-select"); // Can be used with "select.form-control"

        selects.forEach(select => {
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

                return false; 
            });
        });
    };



    /////////////////////////////////////////////
    // Access block
    /////////////////////////////////////////////
    // Prohibiting unauthorized access.
    function AlertUnauthorizedAccess(): void {
        const div = document.getElementById("restrictedDiv");
        if (!div) return;

        if (typeof (window as any).isAuthenticated !== "undefined" && !(window as any).isAuthenticated) {
            div.classList.add("disabled-container");
            document.addEventListener("click", HandleUnauthorizedClick);
        }
    }

    function HandleUnauthorizedClick(event: MouseEvent): void {
        const target = event.target as HTMLElement;
        if (target.closest("#restrictedDiv, #restrictedDiv *")) {
            showAlert("Filters are unavailable for unregistered users. Please sign in.");
        }
    }


    /////////////////////////////////////////////
    // Start outer functions
    /////////////////////////////////////////////
    FixSelectBehavior();
    AlertUnauthorizedAccess();
    updateFixedColumnLeftOffset();
    adjustTableSize();
    //updateTableHeaderOffset();
});



/////////////////////////////////////////////
// Table block
/////////////////////////////////////////////
// Set the margin for the table header row so that it does not overlap with the page header.
function updateTableHeaderOffset(): void {
    const header = document.querySelector('.toFixLayoutHeader') as HTMLElement | null;
    const tableHeader = document.querySelector('table.table-toFixRows.table-plantes thead') as HTMLElement | null;

    if (header && tableHeader) {
        const headerHeight = header.offsetHeight; // Getting the height of the header
        tableHeader.style.top = `${headerHeight}px`;
    }
}

// Setting left offset for the 2nd and subsequent fixed columns:
function updateFixedColumnLeftOffset(): void {
    const fixedColumns = 2; // Number of fixed columns.
    let offset = 0; // Left offset.

    // Getting the value of 1em in pixels.
    const emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
    const reductionStep = 0.1 * emInPixels; // 0.1em in pixels.
    let totalReduction = 0;

    for (let i = 1; i <= fixedColumns; i++) {
        const cells = document.querySelectorAll<HTMLElement>(`th:nth-child(${i}), td:nth-child(${i})`);
        if (cells.length > 0) {
            const width = cells[0].offsetWidth; // Getting the column width.
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
function adjustTableSize(): void {
    const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
    const footer = document.querySelector("footer") as HTMLElement | null;
    const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;
    const tableContainer = document.querySelector(".table-plantes") as HTMLElement | null;
    const pageContent = document.querySelector(".planetCatalogHeader") as HTMLElement | null;

    if (!tableContainer || !header || !footer || !main || !pageContent) return;

    requestAnimationFrame(() => {
        const windowHeight = window.innerHeight;
        const windowWidth = window.innerWidth;

        const headerHeight = header.offsetHeight;
        const footerHeight = footer.offsetHeight;

        const mainStyles = getComputedStyle(main);
        const mainPaddingV = parseFloat(mainStyles.paddingTop) + parseFloat(mainStyles.paddingBottom);
        const mainMarginV = parseFloat(mainStyles.marginTop) + parseFloat(mainStyles.marginBottom);

        const mainPaddingH = parseFloat(mainStyles.paddingLeft) + parseFloat(mainStyles.paddingRight);
        const mainMarginH = parseFloat(mainStyles.marginLeft) + parseFloat(mainStyles.marginRight);

        const pageContentH = pageContent.offsetHeight;

        const emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
        const extraHeight = 6 * emInPixels; // Unclear missing space.
        const extraPadding = 0 * emInPixels; // Backup margin.

        const tableHeight = windowHeight - (headerHeight + footerHeight + mainPaddingV + mainMarginV + pageContentH + extraPadding + extraHeight);
        const tableWidth = windowWidth - (mainPaddingH + mainMarginH + extraPadding);

        console.log({
            emInPixels,
            windowHeight,
            headerHeight,
            footerHeight,
            mainPaddingV,
            mainMarginV,
            mainPaddingH,
            mainMarginH,
            pageContentH,
            tableHeight,
            tableWidth
        });

        tableContainer.style.height = `${tableHeight}px`;
        tableContainer.style.width = `${tableWidth}px`;
    });
}

// Executing on window resize.
function handleResize(): void {
    updateFixedColumnLeftOffset();
    adjustTableSize();
}
window.addEventListener("resize", handleResize);

// ***** Tracking size changes in header, footer, and main. *****
const resizeObserver = new ResizeObserver(() => adjustTableSize());

const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
const footer = document.querySelector("footer") as HTMLElement | null;
const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;

if (header) resizeObserver.observe(header);
if (footer) resizeObserver.observe(footer);
if (main) resizeObserver.observe(main);



/////////////////////////////////////////////
// Store procedure block
/////////////////////////////////////////////
interface AjaxResponse {
    responseText?: string;
    status?: number;
    statusText?: string;
}


type AjaxFailHandler = (jqXHR: AjaxResponse) => void;
type AjaxDoneHandler = (data: any) => void;
type AjaxAlwaysHandler = () => void;
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



/////////////////////////////////////////////
// Alert block
/////////////////////////////////////////////
declare const Swal: any; 

function showAlert(message: string): void {
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






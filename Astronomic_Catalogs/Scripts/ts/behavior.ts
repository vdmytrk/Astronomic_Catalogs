//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
export function initialize(): void {
    setupShowTextBlockToggles();
    setupShowFilsersToggles();
    setColumnWidthAstroCatalogTable();
}



/////////////////////////////////////////////
// Hide/Show block
/////////////////////////////////////////////
function setupShowTextBlockToggles(): void {
    document.addEventListener("click", (e) => {
        const target = e.target as HTMLElement;
        if (target.classList.contains("showTextBlockBtn")) {
            const container = target.closest("div");
            if (!container) return;


            const outerContainer = container.parentElement;
            if (!outerContainer) return;

            const hiddenBlock = outerContainer.querySelector(".hiddenBlock") as HTMLElement | null;
            if (!hiddenBlock) return;

            const isCollapsed = hiddenBlock.classList.toggle("collapsed");
            target.textContent = isCollapsed ? "Show" : "Hide";
        };
    });
}

function setupShowFilsersToggles(): void {
    document.addEventListener("click", (e) => {
        const target = e.target as HTMLElement;
        if (target.classList.contains("hideFilters")) {
            const filtersBlock = document.querySelector(".allTopMenuFilters") as HTMLElement | null;
            if (!filtersBlock) return;

            const isCollapsed = filtersBlock.classList.toggle("collapsed");
            target.textContent = isCollapsed ? "Show filters" : "Hide filters";
        };
    });
}



/////////////////////////////////////////////
// Table property block
/////////////////////////////////////////////
function setColumnWidthAstroCatalogTable(): void {
    const checkbox = document.querySelector("label.switchButton input[type='checkbox']") as HTMLInputElement | null;
    if (!checkbox) return;

    checkbox.addEventListener("change", () => {
        const table = document.querySelector("table.astroCatalogTable") as HTMLElement | null;
        if (table) {
            console.log("######## switchButton");
            console.log("Before toggle:", table.classList.toString());
            table.classList.toggle("setColumnWidth", checkbox.checked);
            console.log("After toggle:", table.classList.toString());
        }
    });
}

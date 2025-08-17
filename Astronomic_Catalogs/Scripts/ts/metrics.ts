//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
let resizeObserver: ResizeObserver | null = null;
let isLayoutUpdating = false;
let resizeTimeout: number | null = null;
let previousColumnCount: number | null = null;

export function initialize(remInPixels: number): void {
    onWindowLoad(remInPixels);
    updateOnResizeWindow();
    updateOnResizeElement();
    adjustRestrictedBlockHeight(remInPixels);
}

function updateOnResizeElement() {
    let remInPixels: number = parseFloat(getComputedStyle(document.documentElement).fontSize);
    const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
    const footer = document.querySelector("footer") as HTMLElement | null;
    const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;

    if (!resizeObserver) {
        resizeObserver = new ResizeObserver(() => {
            if (isLayoutUpdating) return; // Block recursion.

            if (resizeTimeout !== null) clearTimeout(resizeTimeout);
            resizeTimeout = window.setTimeout(() => {
                isLayoutUpdating = true;
                handleTable(remInPixels);
                isLayoutUpdating = false;
            }, 100);
        });
    }

    [header, footer, main].forEach(el => {
        if (el) resizeObserver.observe(el);
    });
}

function updateOnResizeWindow() {
    window.addEventListener("resize", () => {
        let remInPixels: number = parseFloat(getComputedStyle(document.documentElement).fontSize);
        handleTable(remInPixels);
    });
}

function onWindowLoad(remInPixels: number) {
    window.addEventListener("load", () => {
        handleTable(remInPixels);
    });
}

function handleTable(remInPixels: number): void {
    updateFixedColumnLeftOffset();
    adjustTableSize(remInPixels);
    distributeItemsIntoColumns(".responsiveColumnsContainer", remInPixels);
}


/////////////////////////////////////////////
// Table block
/////////////////////////////////////////////
// Setting left offset for the 2nd and subsequent fixed columns:
function updateFixedColumnLeftOffset(): void {
    const fixedColumns = 2;
    let offset = 0;

    // Getting the value of 1em in pixels.
    const emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
    const reductionStep = 0.1 * emInPixels; // 0.1em in pixels.
    let totalReduction = 0;

    for (let i = 1; i <= fixedColumns; i++) {
        const cells = document.querySelectorAll<HTMLElement>(`.table-set-size th:nth-child(${i}), .table-set-size td:nth-child(${i})`);

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

function adjustTableSize(remInPixels: number): void {
    const header = document.querySelector(".toFixLayoutHeader") as HTMLElement | null;
    const footer = document.querySelector("footer") as HTMLElement | null;
    const main = document.querySelector(".main-RenderBody-container") as HTMLElement | null;
    const tableContainer = document.querySelector(".table-set-size") as HTMLElement | null;
    const pageContent = document.querySelector(".catalogPagesHeader") as HTMLElement | null;

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

// Set the margin for the table header row so that it does not overlap with the page header.
function updateTableHeaderOffset(): void {
    const header = document.querySelector('.toFixLayoutHeader') as HTMLElement | null;
    const tableHeader = document.querySelector('table.table-toFixRows-1.table-fixed-headers thead') as HTMLElement | null;

    if (header && tableHeader) {
        const headerHeight = header.offsetHeight;
        tableHeader.style.top = `${headerHeight}px`;
    }
}


/////////////////////////////////////////////
// Block allocation for the maximum number of columns
function distributeItemsIntoColumns(containerSelector: string, remInPixels: number, minColumnWidth: number = (remInPixels * 26)) {
    const container = document.querySelector(containerSelector);
    if (!container) return;

    const items = Array.from(container.querySelectorAll("p"));
    if (items.length === 0) return;

    // Determine the number of columns.
    const containerWidth = container.clientWidth;
    const columnCount = Math.floor(containerWidth / minColumnWidth) || 1;

    // If the number of columns is the same — skip.
    if (previousColumnCount === columnCount) return;
    previousColumnCount = columnCount;

    // Remove all <p> elements from the DOM before rebuilding.
    items.forEach(item => {
        if (item.parentNode) {
            item.parentNode.removeChild(item);
        }
    });

    // Create a wrapper for the columns.
    const wrapper = document.createElement("div");
    wrapper.className = "responsiveColumnsWrapper";

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
let isExpanded = false;
let originalHeight: number | null = null;

function adjustRestrictedBlockHeight(remInPixels: number): void {

    const setupAdjustHeightOnDropdownClick = () => {

        const restrictedBlock = document.querySelector('.allTopMenuFilters.restrictedBlock') as HTMLElement;
        const table = document.querySelector('.planetTypeSelect.table') as HTMLElement;

        if (!restrictedBlock || !table) return;

        if (!isExpanded) {
            const divRect = restrictedBlock.getBoundingClientRect();
            const tableRect = table.getBoundingClientRect();

            const divBottom = divRect.top + divRect.height + window.scrollY;
            const tableBottom = tableRect.top + tableRect.height + window.scrollY + remInPixels / 2;

            if (tableBottom > divBottom) {
                const requiredIncrease = tableBottom - divBottom;
                const currentComputedHeight = parseFloat(getComputedStyle(restrictedBlock).height);

                originalHeight = currentComputedHeight;

                restrictedBlock.style.height = (currentComputedHeight + requiredIncrease) + remInPixels / 2 + 'px';
                isExpanded = true;
            }
        } else {
            if (originalHeight !== null) {
                restrictedBlock.style.height = originalHeight + 'px';
                isExpanded = false;
            }
        }
    }

    const button = document.querySelector('.adjustHeightDropdownBtn.dropdown-toggle');

    if (button) {
        button.addEventListener('click', () => {
            setTimeout(() => {
                setupAdjustHeightOnDropdownClick();
            }, 2);
        });
    }
}



/////////////////////////////////////////////
export async function getElementRect(selector: string): Promise<DOMRect> {
    await new Promise(requestAnimationFrame); // To wait for a stable layout.
    const element = document.querySelector(selector) as HTMLElement;

    if (!element) {
        throw new Error(`❌ Element not found for selector: "${selector}". Make sure the selector is correct and the element exists in the DOM.`);
    }

    if (!(element instanceof HTMLElement)) {
        throw new Error(`❌ Element found for selector "${selector}" is not an instance of HTMLElement. It may be an SVG or unsupported node.`);
    }

    const rect = element.getBoundingClientRect();
    console.log(`📐 Element "${selector}" → Width: ${rect.width}px, Height: ${rect.height}px`);
    return rect;
}
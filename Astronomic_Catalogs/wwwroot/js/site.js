// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    /////////////////////////////////////////////
    // Theme block
    /////////////////////////////////////////////
    const themeToggle = document.getElementById("theme-toggle");
    const body = document.body;
    const inputs = document.querySelectorAll(".form-control");

    // Forced autofill update.
    function RefreshAutofillStyles() {
        console.log("🔄 Forced autofill update.");
        inputs.forEach(input => {
            const prevName = input.getAttribute("name"); // Saving the old name.
            input.setAttribute("name", prevName + "_tmp"); // Changing it so the browser forgets autofill.
            input.offsetHeight; // Trick for re-rendering.
            input.setAttribute("name", prevName); // Restoring the name.
        });
    }

    // Autofill fix after theme switching.
    function ForceAutofillFix() {
        console.log("🎨 Applied autofill fix.");
        inputs.forEach(input => {
            input.addEventListener("animationstart", (event) => {
                if (event.animationName.includes("autofill") ||
                    event.animationName === "autofill-fix" ||
                    event.animationName === "dark-autofill-fix"
                ) {
                    input.style.transition = "background-color 0.3s ease, color 0.3s ease";
                }
            });

            // Artificial autofill refresh via focus-blur.
            const value = input.value;
            input.value = "";
            setTimeout(() => {
                input.value = value;
            }, 10);
        });
    }

    // Loading theme from localStorage.
    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-theme");
    }

    // Click handler for theme switching.
    themeToggle.addEventListener("click", function () {
        body.classList.toggle("dark-theme");
        localStorage.setItem("theme", body.classList.contains("dark-theme") ? "dark" : "light");

        // Forcing autofill update.
        RefreshAutofillStyles();
        ForceAutofillFix();

        // Forcing autofill re-render.
        inputs.forEach(input => {
            input.classList.remove("force-repaint");
            void input.offsetWidth; // Trick for forced re-render.
            input.classList.add("force-repaint");
        });
    });

    // Updating autofill styles on page load.
    RefreshAutofillStyles();
    ForceAutofillFix();

    // Set a delay on page load.
    (function () {
        setTimeout(function () {
            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", function () {
                    document.body.style.display = "block";
                });
            } else {
                document.body.style.display = "block";
            }

            window.addEventListener("load", function () {
                document.body.classList.remove("loading");
            });

        }, 100);
    })();


    /////////////////////////////////////////////
    // Style block
    /////////////////////////////////////////////
    // To solve problem with the background color of selected items in browser.
    function FixSelectBehavior() {
        let selects = document.querySelectorAll("select.form-select"); // Can be used with "select.form-control"

        selects.forEach(select => {
            let selectedValues = new Set();

            for (let option of select.options) {
                if (option.hasAttribute("selected") || option.selected) {
                    selectedValues.add(option.value);
                    option.selected = true;
                    option.classList.add("selected-fix");
                }
            }

            select.addEventListener("change", function () {
                for (let option of select.options) {
                    if (option.selected) {
                        selectedValues.add(option.value);
                        option.classList.add("selected-fix");
                    } else {
                        selectedValues.delete(option.value);
                        option.classList.remove("selected-fix");
                    }
                }
            });

            select.addEventListener("mousedown", function (event) {
                event.preventDefault();

                let option = event.target;

                if (option.tagName === "OPTION") {
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
    function AlertUnauthorizedAccess() {
        var div = document.getElementById("restrictedDiv");
        if (!div) return;

        if (typeof window.isAuthenticated !== "undefined" && !window.isAuthenticated) {
            div.classList.add("disabled-container");

            document.addEventListener("click", HandleUnauthorizedClick);
        }
    }

    function HandleUnauthorizedClick(event) {
        if (event.target.closest("#restrictedDiv, #restrictedDiv *")) {
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
function updateTableHeaderOffset() {
    const header = document.querySelector('.toFixLayoutHeader');
    const tableHeader = document.querySelector('table.table-toFixRows.table-plantes thead');

    if (header && tableHeader) {
        const headerHeight = header.offsetHeight; // Getting the height of the header
        tableHeader.style.top = `${headerHeight}px`;
    }
}

// Setting left offset for the 2nd and subsequent fixed columns:
function updateFixedColumnLeftOffset() {
    const fixedColumns = 2; // Number of fixed columns.
    let offset = 0; // Left offset.

    // Getting the value of 1em in pixels.
    let emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
    let reductionStep = 0.1 * emInPixels; // 0.1em in pixels.
    let totalReduction = 0; 

    for (let i = 1; i <= fixedColumns; i++) {
        let cells = document.querySelectorAll(`th:nth-child(${i}), td:nth-child(${i})`);
        if (cells.length > 0) {
            let width = cells[0].offsetWidth; // Getting the column width.
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
function adjustTableSize() {
    const header = document.querySelector(".toFixLayoutHeader");
    const footer = document.querySelector("footer");
    const main = document.querySelector(".main-RenderBody-container");
    const tableContainer = document.querySelector(".table-plantes");
    const pageContent = document.querySelector(".planetCatalogHeader");

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

        let emInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
        const extreHeight = 6 * emInPixels; // Unclear missing space.
        const extraPadding = 0 * emInPixels; // Backup margin.

        const tableHeight = windowHeight - (headerHeight + footerHeight + mainPaddingV + mainMarginV + pageContentH + extraPadding + extreHeight);
        const tableWidth = windowWidth - (mainPaddingH + mainMarginH + extraPadding);

        //console.log({
        //    emInPixels,
        //    windowHeight,
        //    headerHeight,
        //    footerHeight,
        //    mainPaddingV,
        //    mainMarginV,
        //    mainPaddingH,
        //    mainMarginH,
        //    //otherContentHeight,
        //    pageContentH,
        //    tableHeight,
        //    tableWidth
        //});

        tableContainer.style.height = `${tableHeight}px`;
        tableContainer.style.width = `${tableWidth}px`;

    });
}


//// Executing on window resize.
function handleResize() {
    //updateTableHeaderOffset();
    updateFixedColumnLeftOffset();
    adjustTableSize();
}
window.addEventListener("resize", handleResize);

// ***** Tracking size changes in header, footer, and main. *****
const resizeObserver = new ResizeObserver(adjustTableSize);
resizeObserver.observe(document.querySelector(".toFixLayoutHeader"));
resizeObserver.observe(document.querySelector("footer"));
resizeObserver.observe(document.querySelector(".main-RenderBody-container"));








/////////////////////////////////////////////
// Store procedure block
/////////////////////////////////////////////
function fetchDateUsingADO() {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureADO',
        type: 'GET'
    }).done(function (data) {
        showAlert(data);
    }).fail(function (data) {
        showAlert("ADO. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
        console.log(data);
    }).always(function () {
        console.log("Request execution completed!");
    });
}

function fetchDateUsingEF() {
    $.ajax({
        url: '/Admin/HomeAdmin/GetDateFromProcedureEF',
        type: 'GET'
    }).done(function (data) {
        showAlert(data);
    }).fail(function (data) {
        showAlert("EF. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
        console.log(data);
    });
}

function executeCreateNewDateProcedure() {
    $.ajax({
        url: '/Admin/HomeAdmin/CallCreateNewDateProcedure',
        type: 'GET'
    }).done(function (data) {
        $("#dataTableContainer").html(data);
    }).fail(function () {
        showAlert("AN ARROR OCCURRED WHEN THE CallCreateNewDateProcedure WAS CALLED. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
    });
}


/////////////////////////////////////////////
// Alert block
/////////////////////////////////////////////
function showAlert(message) {
    if (localStorage.getItem("theme") === "dark") {
        Swal.fire({
            title: "Attention!",
            text: message,
            icon: "info",
            background: "#414243",
            color: "red",
            confirmButtonColor: "red",
            confirmButtonText: "OK"
        });
    } else {
        Swal.fire({
            title: "Attention!",
            text: message,
            icon: "info",
            background: "white",
            color: "black",
            confirmButtonColor: "blue",
            confirmButtonText: "OK"
        });
    }
}





//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
let inputs: NodeListOf<HTMLInputElement>;
let themeButton: HTMLElement | null;

export function initialize(body: HTMLElement): void {
    inputs = document.querySelectorAll(".form-control") as NodeListOf<HTMLInputElement>;
    themeButton = document.getElementById("theme-toggle") as HTMLElement | null;

    updateThemeBackground(inputs, body);
    themeToggle(themeButton, inputs, body);
    fixSelectBehavior();
}

function themeToggle(themeButton: HTMLElement, inputs: NodeListOf<HTMLInputElement>, body: HTMLElement): void {
    if (themeButton) {
        themeButton.addEventListener("click", () => {
            body.classList.toggle("dark-theme");
            localStorage.setItem("theme", body.classList.contains("dark-theme") ? "dark" : "light");

            updateThemeBackground(inputs, body);
        });
    };
}

function updateThemeBackground(inputs: NodeListOf<HTMLInputElement>, body: HTMLElement): void {
    console.log(" >>> FUNCTION: updateThemeBackground");
    const updateBodyBg = () => {
        console.log(" >>> FUNCTION: updateBodyBg");
        const isDark = localStorage.getItem("theme") === "dark";
        const bg = isDark ? "#121212" : "#FFF";

        if (isDark) {
            body.classList.add("dark-theme");
        }

        document.documentElement.style.backgroundColor = bg;
        body.style.backgroundColor = bg;
    };

    // Updating autofill styles on page load.
    if (inputs) {
        updateAutofill(inputs);
    }

    updateBodyBg();
}



/////////////////////////////////////////////
// Style for HTMLInputElement
/////////////////////////////////////////////
// Change the style of autofilled browser fields
function updateAutofill(inputs: NodeListOf<HTMLInputElement>): void {
    if (!inputs) return;
    refreshAutofillStyles(inputs);
    forceAutofillFix(inputs);
    forceAutofillReRender(inputs);
}

// Forced autofill update.
function refreshAutofillStyles(inputs: NodeListOf<HTMLInputElement>): void {
    console.log("🔄 Forced autofill update.");
    inputs.forEach(input => {
        const prevName = input.getAttribute("name"); // Saving the old name.
        if (prevName !== null) {
            input.setAttribute("name", prevName + "_tmp"); // Changing it so the browser forgets autofill.
            input.offsetHeight; // Trick for re-rendering.
            input.setAttribute("name", prevName); // Restoring the name.
        }
    });
}

// Autofill fix after theme switching.
function forceAutofillFix(inputs: NodeListOf<HTMLInputElement>): void {
    console.log("🎨 Applied autofill fix.");
    inputs.forEach(input => {
        input.addEventListener("animationstart", (event: AnimationEvent) => {
            if (
                event.animationName.includes("autofill") ||
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
        }, 100);
    });
}

// Forcing autofill re-render.
function forceAutofillReRender(inputs: NodeListOf<HTMLInputElement>): void {
    console.log("🔄 Forcing autofill re-render.");
    inputs.forEach(input => {
        input.classList.remove("force-repaint");
        void input.offsetWidth; // Trick for forced re-render.
        input.classList.add("force-repaint");
    });
}



/////////////////////////////////////////////
// Style for the selected element of the multiselect list.
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
// Style on _PlanetarySystemVisualization.
/////////////////////////////////////////////
export function planetSysVisualizationTheme(isVisualization: boolean) {
    console.log("FUNCTION: planetSysVisualizationTheme");
    const inputs = document.querySelectorAll(".form-control") as NodeListOf<HTMLInputElement>;
    const topMenu = document.querySelector(".toFixLayoutHeader") as HTMLElement;
    themeButton = document.getElementById("theme-toggle") as HTMLElement | null;

    const isDark = localStorage.getItem("theme") === "dark";

    if (!isDark) {
        document.body.classList.toggle("dark-theme");
    }

    if (isVisualization) {
        topMenu.classList.add("d-none");
        themeButton.classList.add("d-none");
        document.body.classList.add("planetary-system-visualization-style"); 
        updateThemeBackground(inputs, document.body);
        document.documentElement.style.backgroundColor = "#121212";
    } else {
        topMenu.classList.remove("d-none");
        themeButton.classList.remove("d-none");
        document.body.classList.remove("planetary-system-visualization-style");
        updateThemeBackground(inputs, document.body);
    }
}



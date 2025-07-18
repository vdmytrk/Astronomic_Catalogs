"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.initialize = initialize;
exports.planetSysVisualizationTheme = planetSysVisualizationTheme;
let inputs;
let themeButton;
function initialize(body) {
    inputs = document.querySelectorAll(".form-control");
    themeButton = document.getElementById("theme-toggle");
    updateThemeBackground(inputs, body);
    themeToggle(themeButton, inputs, body);
    fixSelectBehavior();
}
function themeToggle(themeButton, inputs, body) {
    if (themeButton) {
        themeButton.addEventListener("click", () => {
            body.classList.toggle("dark-theme");
            localStorage.setItem("theme", body.classList.contains("dark-theme") ? "dark" : "light");
            updateThemeBackground(inputs, body);
        });
    }
    ;
}
function updateThemeBackground(inputs, body) {
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
    if (inputs) {
        updateAutofill(inputs);
    }
    updateBodyBg();
}
function updateAutofill(inputs) {
    if (!inputs)
        return;
    refreshAutofillStyles(inputs);
    forceAutofillFix(inputs);
    forceAutofillReRender(inputs);
}
function refreshAutofillStyles(inputs) {
    console.log("🔄 Forced autofill update.");
    inputs.forEach(input => {
        const prevName = input.getAttribute("name");
        if (prevName !== null) {
            input.setAttribute("name", prevName + "_tmp");
            input.offsetHeight;
            input.setAttribute("name", prevName);
        }
    });
}
function forceAutofillFix(inputs) {
    console.log("🎨 Applied autofill fix.");
    inputs.forEach(input => {
        input.addEventListener("animationstart", (event) => {
            if (event.animationName.includes("autofill") ||
                event.animationName === "autofill-fix" ||
                event.animationName === "dark-autofill-fix") {
                input.style.transition = "background-color 0.3s ease, color 0.3s ease";
            }
        });
        const value = input.value;
        input.value = "";
        setTimeout(() => {
            input.value = value;
        }, 100);
    });
}
function forceAutofillReRender(inputs) {
    console.log("🔄 Forcing autofill re-render.");
    inputs.forEach(input => {
        input.classList.remove("force-repaint");
        void input.offsetWidth;
        input.classList.add("force-repaint");
    });
}
function fixSelectBehavior() {
    let selects = document.querySelectorAll("select.form-select");
    selects.forEach(select => {
        if (!select.multiple)
            return;
        let selectedValues = new Set();
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
                }
                else {
                    selectedValues.delete(option.value);
                    option.classList.remove("selected-fix");
                }
            }
        });
        select.addEventListener("mousedown", function (event) {
            event.preventDefault();
            let target = event.target;
            if (target.tagName === "OPTION") {
                let option = target;
                if (selectedValues.has(option.value)) {
                    selectedValues.delete(option.value);
                    option.selected = false;
                    option.classList.remove("selected-fix");
                }
                else {
                    selectedValues.add(option.value);
                    option.selected = true;
                    option.classList.add("selected-fix");
                }
            }
            return false;
        });
    });
}
;
function planetSysVisualizationTheme(isVisualization) {
    console.log("FUNCTION: planetSysVisualizationTheme");
    const inputs = document.querySelectorAll(".form-control");
    const topMenu = document.querySelector(".toFixLayoutHeader");
    themeButton = document.getElementById("theme-toggle");
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
    }
    else {
        topMenu.classList.remove("d-none");
        themeButton.classList.remove("d-none");
        document.body.classList.remove("planetary-system-visualization-style");
        updateThemeBackground(inputs, document.body);
    }
}
//# sourceMappingURL=theme.js.map
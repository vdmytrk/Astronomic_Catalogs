/// <reference types="jquery" />
import { setElemSize } from "./main";

document.addEventListener("DOMContentLoaded", function () {
    const body = document.body;
    body.style.display = "none"; 
    const themeToggle = document.getElementById("theme-toggle") as HTMLElement | null;
    const inputs = document.querySelectorAll(".form-control") as NodeListOf<HTMLInputElement>;

    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-theme");
    }

    // Set a delay on page load.
    window.addEventListener("load", function () {
        showBody(body);
    });

    // Click handler for theme switching.
    themeToggle?.addEventListener("click", function () {
        if (!body) {
            console.error(`The body page was not found!`);
            return;
        }

        body.style.display = "none";
        body.classList.toggle("dark-theme");
        localStorage.setItem("theme", body.classList.contains("dark-theme") ? "dark" : "light");

        
        if (localStorage.getItem("theme") === "dark") {
            document.documentElement.style.backgroundColor = "#121212";
            document.addEventListener("DOMContentLoaded", () => {
                document.body.style.backgroundColor = "#121212";
            });
            window.addEventListener("load", () => {
                document.body.style.backgroundColor = "#121212";
            });
        } else {
            document.documentElement.style.backgroundColor = "#FFFFFF";
            document.addEventListener("DOMContentLoaded", () => {
                document.body.style.backgroundColor = "#FFFFFF";
            });
            window.addEventListener("load", () => {
                document.body.style.backgroundColor = "#FFFFFF";
            });
        };

        showBody(body);

        // Style for HTMLInputElement
        if (!inputs) return;
        refreshAutofillStyles(inputs);
        forceAutofillFix(inputs);
        forceAutofillReRender(inputs);
    });

    // Updating autofill styles on page load.
    if (!inputs) return;
    refreshAutofillStyles(inputs);
    forceAutofillFix(inputs);
});


function showBody(body: HTMLElement) {
    setTimeout(() => {
        body.style.display = "block";
        document.documentElement.style.display = "block";

        setElemSize();
    }, 50);
}

/////////////////////////////////////////////
// Style for HTMLInputElement
/////////////////////////////////////////////
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

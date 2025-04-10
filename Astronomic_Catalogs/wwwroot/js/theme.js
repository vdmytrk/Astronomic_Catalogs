document.addEventListener("DOMContentLoaded", function () {
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
});





/// <reference types="jquery" />

document.addEventListener("DOMContentLoaded", () => {
    /////////////////////////////////////////////
    // Theme block
    /////////////////////////////////////////////
    const themeToggle = document.getElementById("theme-toggle") as HTMLElement | null;
    const body = document.body;
    const inputs = document.querySelectorAll<HTMLInputElement>(".form-control");

    // Forced autofill update.
    function forceAutofillStyleReset(): void {
        console.log("🔄 Forced autofill update.");
        inputs.forEach(input => {
            const originalName = input.name;
            input.name = `${originalName}_tmp`;
            input.offsetHeight; // Trigger reflow
            input.name = originalName;
        });
    }

    function applyAutofillFix(): void {
        console.log("🎨 Applied autofill fix.");
        inputs.forEach(input => {
            input.addEventListener("animationstart", (event: AnimationEvent) => {
                const autofillAnimations = ["autofill", "autofill-fix", "dark-autofill-fix"];
                if (autofillAnimations.includes(event.animationName)) {
                    input.style.transition = "background-color 0.3s ease, color 0.3s ease";
                }
            });

            // Artificial autofill refresh via focus-blur.
            // Force re-render to refresh autofill styles.
            const cachedValue = input.value;
            input.value = "";
            setTimeout(() => input.value = cachedValue, 10);
        });
    }

    function applyAutofillRepaint(): void {
        inputs.forEach(input => {
            input.classList.remove("force-repaint");
            void input.offsetWidth;
            input.classList.add("force-repaint");
        });
    }

    function applyThemeFromStorage(): void {
        if (localStorage.getItem("theme") === "dark") {
            body.classList.add("dark-theme");
        }
    }

    function toggleTheme(): void {
        body.classList.toggle("dark-theme");
        const isDark = body.classList.contains("dark-theme");
        localStorage.setItem("theme", isDark ? "dark" : "light");

        forceAutofillStyleReset();
        applyAutofillFix();
        applyAutofillRepaint();
    }

    function setupInitialTheme(): void {
        applyThemeFromStorage();
        body.style.display = "block";
        forceAutofillStyleReset();
        applyAutofillFix();
    }

    function setupThemeToggle(): void {
        themeToggle?.addEventListener("click", toggleTheme);
    }

    function setupLoadingHandlers(): void {
        setTimeout(() => {
            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", () => {
                    body.style.display = "block";
                });
            } else {
                body.style.display = "block";
            }

            window.addEventListener("load", () => {
                body.classList.remove("loading");
            });
        }, 100);
    }

    setupInitialTheme();
    setupThemeToggle();
    setupLoadingHandlers();
});

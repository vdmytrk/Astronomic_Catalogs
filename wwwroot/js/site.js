// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.getElementById("theme-toggle");
    const body = document.body;
    const inputs = document.querySelectorAll(".form-control");

    // Forced autofill update.
    function refreshAutofillStyles() {
        console.log("🔄 Примусове оновлення autofill");
        inputs.forEach(input => {
            const prevName = input.getAttribute("name"); // Saving the old name.
            input.setAttribute("name", prevName + "_tmp"); // Changing it so the browser forgets autofill.
            input.offsetHeight; // Trick for re-rendering.
            input.setAttribute("name", prevName); // Restoring the name.
        });
    }

    // Autofill fix after theme switching.
    function forceAutofillFix() {
        console.log("🎨 Застосування autofill фіксу");
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
        refreshAutofillStyles();
        forceAutofillFix();

        // Forcing autofill re-render.
        inputs.forEach(input => {
            input.classList.remove("force-repaint");
            void input.offsetWidth; // Trick for forced re-render.
            input.classList.add("force-repaint");
        });
    });

    // Updating autofill styles on page load.
    refreshAutofillStyles();
    forceAutofillFix();

    // On load pages
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

function CallStoreProcedureADO() {
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

function CallStoreProcedureEF() {
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

function CallStoreProcedure() {
    $.ajax({
        url: '/Admin/HomeAdmin/CallCreateNewDateProcedure',
        type: 'GET'
    }).done(function (data) {
            $("#dataTableContainer").html(data);
    }).fail(function () {
        showAlert("EF. THERE ARE SOME ISSUES ON THE SERVER! PLEASE CONTACT THE ADMINISTRATION.");
    });
}

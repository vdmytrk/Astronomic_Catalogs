//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
declare const Swal: any;

export function showAlert(message: string): void {
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
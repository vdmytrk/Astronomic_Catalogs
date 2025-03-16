function showAlert(message) {
    const isDarkTheme = localStorage.getItem("theme") === "dark";
    Swal.fire({
        title: "Attention!",
        text: message,
        icon: "info",
        background: isDarkTheme ? "#414243" : "white",
        color: isDarkTheme ? "red" : "black",
        confirmButtonColor: isDarkTheme ? "red" : "blue",
        confirmButtonText: "OK"
    });
}
showAlert("Це повідомлення з TypeScript!");
//# sourceMappingURL=main.js.map
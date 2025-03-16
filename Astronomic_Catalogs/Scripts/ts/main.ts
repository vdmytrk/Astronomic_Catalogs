declare const Swal: any; // Оголошуємо глобальну змінну, якщо бібліотека підключена через CDN

function showAlert(message: string): void {
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
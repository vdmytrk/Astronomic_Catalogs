//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
    export function calendarHandler(): void {
        const dateFrom = document.getElementById("dateFrom") as HTMLInputElement | null;
        const dateTo = document.getElementById("dateTo") as HTMLInputElement | null;

        if (!dateFrom || !dateTo) return;

        // Date "form"
        dateFrom.addEventListener("change", () => {
            const fromValue = dateFrom.value;
            if (fromValue) {
                dateTo.min = fromValue;

                if (dateTo.value && dateTo.value < fromValue) {
                    dateTo.value = "";
                }
            } else {
                dateTo.min = "1992-01-01";
            }
        });

        // Date "to"
        dateTo.addEventListener("change", () => {
            const toValue = dateTo.value;
            if (toValue) {
                dateFrom.max = toValue;

                if (dateFrom.value && dateFrom.value > toValue) {
                    dateFrom.value = "";
                }
            } else {
                dateFrom.max = new Date().toISOString().split("T")[0];
            }
        });
    }

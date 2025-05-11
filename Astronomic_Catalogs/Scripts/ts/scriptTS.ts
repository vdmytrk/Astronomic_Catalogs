import * as Behavior from "./behavior";
import * as FormHandler from "./formHandler";
import * as Main from "./main";
import * as ProgresImportBar from "./progresImportBar";

let remInPixels: number;

document.addEventListener("DOMContentLoaded", () => {
    remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);

    Main.initialize(remInPixels);
    Behavior.initialize();
    ProgresImportBar.initialize();
    FormHandler.initialize();

});

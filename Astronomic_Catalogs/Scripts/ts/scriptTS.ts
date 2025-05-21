import * as Behavior from "./behavior";
import * as Main from "./main";

let remInPixels: number;

document.addEventListener("DOMContentLoaded", () => {
    remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);

    Main.initialize(remInPixels);
    Behavior.initialize();
});

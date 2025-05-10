//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
import * as Theme from "./theme";
import * as Metrics from "./metrics";
import { showAlert } from "./alertOfSwal";

export function initialize(remInPixels: number): void {
    const body = document.body;
    if (!body) {
        console.error("Body not found!");
        return;
    }

    body.style.display = "none";
    Theme.initialize(body);
    Metrics.initialize(remInPixels);
    showBody(body, remInPixels);
    alertUnauthorizedAccess();
    tableHeaderNote_NGCICOpendatasofts();
}

function showBody(body: HTMLElement, remInPixels: number): void {
    window.addEventListener("load", () => {
        setTimeout(() => {
            body.style.display = "block";
            document.documentElement.style.display = "block";
        }, 50);
    });
}



/////////////////////////////////////////////
// Access block
/////////////////////////////////////////////
// Prohibiting unauthorized access.
function alertUnauthorizedAccess(): void {
    console.log("FUNCTION: alertUnauthorizedAccess()");
    const restrictedElements = document.querySelectorAll<HTMLElement>(".restrictedWrapper");

    if (typeof (window as any).isAuthenticated !== "undefined" && !(window as any).isAuthenticated) {
        console.log(`FUNCTION: alertUnauthorizedAccess(): isAuthenticated = ${(window as any).isAuthenticated}`);

        restrictedElements.forEach(wrapper => {
            const mask = document.createElement("div");
            mask.classList.add("restrictedMask");

            mask.addEventListener("click", (e) => {
                e.preventDefault();
                e.stopPropagation();
                showAlert("Filters are unavailable for unregistered users. Please sign in.");
            });

            const buttons = wrapper.querySelectorAll("button, input, a");
            buttons.forEach(btn => {
                (btn as HTMLButtonElement).disabled = true;
            });

            wrapper.appendChild(mask);
        });
    }
}



////////////////////////////////////////////////////////////////////////////////////////////
// Table column description messages block
////////////////////////////////////////////////////////////////////////////////////////////
/// NGCICOpendatasofts ///
function tableHeaderNote_NGCICOpendatasofts(): void {
    document.addEventListener("click", function (event) {
        const target = event.target as HTMLElement;
        if (!target.classList) return;

        const alertMessages = new Map<string, string>([
            ["Name_UK", "Other unique name (taken from the NGC2000_UK catalog. Analogous to the Othernames field). THESE TWO FIELDS SHOULD BE MERGED EVENTUALLY!!!"],
            ["Comment", "Comment from the NameObject table."],
            ["SourceType", "Object type. See the SourceType table."],
            ["Other_names", "Name from other catalogs or historically established. In this case, from the Object field of the NameObject table."],
            ["LimitAngDiameter", "This field contains the character \"<\" if the object's size is an upper limit; otherwise, the field is empty."],
            ["AngDiameter", "This field contains the size of the object in arcminutes. It is the angular size measured along the largest dimension. It is taken from the NGC2000_UK catalog."],
            ["RA", "RA or LII – Galactic longitude of the NGC/IC object – right ascension (analogous to longitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 0.1 minutes of time in the original table. Measured in hours."],
            ["DEC", "DEC or BII – Galactic latitude of the NGC/IC object – declination (analogous to latitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 1 arcminute in the original table."],
            ["App_Mag", "This field contains the integrated (total) magnitude of the type specified in the app_mag_flag field. Accuracy varies."],
            ["App_Mag_Flag", "This field contains a flag indicating the magnitude type. It contains a space if the integrated magnitude is visual, or \"p\" if it is photographic (blue)."],
            ["b_mag", "This field contains the brightness of the object in the blue range (B-band, \"Blue\"), approximately 440 nm."],
            ["v_mag", "This field contains the stellar magnitude of the object in the visible range (V-band, \"Visual\"), around 550 nm."],
            ["j_mag", "This field contains the brightness of the object in the infrared range (J-band), with a wavelength of about 1.25 μm."],
            ["h_mag", "This field contains the brightness of the object in an even longer infrared range (H-band), around 1.65 μm."],
            ["k_mag", "This field contains the brightness of the object further into the infrared range (K-band), around 2.2 μm."],
            ["Surface_Brigthness", "It indicates the average brightness of an object per unit area: the brightness of the object distributed over a unit area in the sky (usually expressed in magnitudes per square arcsecond, e.g., mag/arcsec²). The lower the value, the brighter the object appears per unit area!"],
            ["Hubble_OnlyGalaxies", "Hubble type of the galaxy."],
            ["Cstar_UMag", "This field contains the brightness (stellar magnitude) of the nearest star in the ultraviolet range (U-band)."],
            ["Cstar_BMag", "This field contains the brightness of the nearest star in the blue range (B-band, \"Blue\")."],
            ["Cstar_VMag", "This field contains the brightness of the nearest star in visible light (V-band, \"Visual\")."],
            ["Cstar_Names", "This field contains the names or identifiers of the star specified in the fields: Cstar_UMag, Cstar_BMag, Cstar_VMag."]
        ]);

        for (const [className, message] of alertMessages.entries()) {
            if (target.classList.contains(className)) {
                showAlert(message);
                break;
            }
        }
    });
};

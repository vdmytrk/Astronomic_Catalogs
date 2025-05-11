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
    tableHeaderNote();
    CollinderCatalogCommentNote();
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
function tableHeaderNote(): void {
    const ngcIcMessages = new Map<string, string>([
        ["Name_UK", "Other unique name (taken from the NGC2000_UK catalog. Analogous to the Othernames field). THESE TWO FIELDS SHOULD BE MERGED EVENTUALLY!!!"],
        ["Comment", "Comment from the NameObject table."],
        ["Other_names", "Name from other catalogs or historically established. In this case, from the Object field of the NameObject table."],
        ["LimitAngDiameter", "This field contains the character \"<\" if the object's size is an upper limit; otherwise, the field is empty."],
        ["AngDiameter", "This field contains the size of the object in arcminutes. It is the angular size measured along the largest dimension. It is taken from the NGC2000_UK catalog."],
        ["SourceType", "Object type. See the SourceType table."],
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

    const collinderMessages = new Map<string, string>([
        ["Declination", "Declination of the object. The exact day is unknown. Accuracy up to 1 arcsecond."],
        ["AppMag", "Apparent magnitude estimates of these clusters, both visual (v) and photographic (p), are taken from sources such as NSOG, the SEDS website, Archinal & Hynes, and The Historically Corrected New General Catalog (HCNGC) v.1.02. Magnitudes recorded by Collinder often differ from modern sources (see the original catalog)."],
        ["CountStars", "Number of stars photographed by Collinder. Or: Number of stars brighter than 6.0 magnitude."],
        ["AngDiameter", "Estimates of the apparent sizes of these clusters."],
        ["Comment", "Descriptive explanation of the differences between what Collinder observed and what is known today. Nearly all the notes below were gleaned from the book Star Clusters by Brent A. Archinal and Steven J. Hynes. Specific references to portions of the book are provided where they seemed especially useful."]
    ]);

    let activeMessages: Map<string, string> | null = null;

    if (document.querySelector("#NGCICOpendatasoftTableHeader")) {
        activeMessages = ngcIcMessages;
    } else if (document.querySelector("#CollinderCatalogTableHeader")) {
        activeMessages = collinderMessages;
    }

    if (!activeMessages) return;

    document.addEventListener("click", function (event) {
        const target = event.target as HTMLElement;
        if (!target.classList) return;

        for (const [className, message] of activeMessages.entries()) {
            if (target.classList.contains(className)) {
                showAlert(message);
                break;
            }
        }
    });
}

function CollinderCatalogCommentNote() {

    const table = document.getElementById("sizeFilterTable");
    if (!table) return;

    const clusterInfo: Record<number, string> = {
        1: "Cr.8: Collinder lists as the NGC designation for this cluster the number 281. However, NGC 281 is itself not a cluster, but a reflection nebula. The cluster Collinder studied here is properly designated IC 1590.",
        2: "Cr.21: Asterism.",
        3: "Cr.24 & Cr.25: The Double Cluster",
        4: "Cr.26: More likely to be found on charts as IC 1805.",
        5: "Cr.32/33/34: Essentially all parts of one complex, IC 1848 per A/H pg 134.",
        6: "Cr.39: Perseus Moving Cluster/Group (Mel 20).",
        7: "Cr.41: Often depicted elsewhere as a bright nebula without mention of the loose cluster involved.",
        8: "Cr.50: TMC = Taurus Moving Cluster.",
        9: "Cr.63: Associated with nebula IC 410.",
        10: "Cr.69: a.k.a. Lambda Orionis cluster.",
        11: "Cr.70: Orion belt cluster.",
        12: "Cr.81: Sometimes erroneously listed as a globular cluster.",
        13: "Cr.84: The NGC number listed by Collinder is properly assigned to the nebulosity surrounding Cr.84, NOT to the cluster itself (which may not actually be a cluster).",
        14: "Cr.109: Some references give an erroneous location of 06h 38.4m +01° 11' due to an original error by Collinder.",
        15: "Cr.156: According to A/H Collinder 156 = Mel 72 = Cr.467.",
        16: "Cr.182: It is possible that this is not an actual star cluster.",
        17: "Cr.191: The center of a super cluster (A/H p. 137); good binocular object.",
        18: "Cr.199 and 202:  something of a messy situation, with Cr.202 (Harvard 3) being included as a separate cluster by various authors, but with very different coordinates, even though Harv. 3 is actually “inside” of NGC 2669 (Cr.199); Cr.202 is apparently a central condensation of the larger cluster. (See A/H p. 168).",
        19: "Cr.206: an open cluster that actually includes a planetary nebula;  A/H argue that the planetary nebula should be given the NGC designation used by Collinder, with the cluster being identified as Cr.206.",
        20: "Cr.220: In assembling his catalog Collinder incorrectly identified this cluster as NGC 3247, when it was in fact a “new” cluster. Sold himself short. The NGC number is left in the spreadsheet because some references have apparently perpetuated this error.",
        21: "Cr.221: May not be a true star cluster.",
        22: "Cr.233 and 234: Both Collinder entries refer to the eta Carinae star cluster, with 234 applied by Collinder to the southern portion.The entire object is properly known as Trumpler 16.",
        23: "Cr.239 & 240: Another cluster in a cluster; 239 = NGC 3572, while 240 should be only a Cr. #.",
        24: "Cr.243(Tr.19): Described by Skiff as “not obvious in 0.35 degree field.”",
        25: "Cr.254: Probably not a true cluster.",
        26: "Cr.257 & 258: Apparently labeled incorrectly some places as Harv. 5 or Hogg 74.(A/ H p. 132 for details.)",
        27: "Cr.264: J.Herschel’s “Jewel Box.”",
        28: "Cr.265: Probably not a true cluster.",
        29: "Cr.267: This is actually a globular cluster. (A / H p.237).",
        30: "Cr.269: Probably not a true cluster.",
        31: "Cr.283: Probably not a true cluster.",
        32: "Cr.294: Probably not a true cluster.",
        33: "Cr.314: According to A / H Collinder was apparently incorrect in identifying this cluster with NGC 6222. No NSOG reference to this cluster under either designation.However, older atlases such as Norton’s 19th edition show NGC 6222 is the specified position, as does the newer HB Astroatlas.The HCNGC lists NGC 6222 and Cr. 314 as synonymous.",
        34: "Cr.316: Appears to be superposed on Tr. 24, and may be part of the same cluster.",
        35: "Cr.328: Collinder and his mentor apparently thought this might be a globular cluster.Later studies have verified that this is indeed the case.",
        36: "Cr.330: Another globular cluster caught up in Collinder’s survey.",
        37: "Cr.334 & 335: Duplicate listings; NGC 6374 is not a separate open cluster.",
        38: "Cr.339: Collinder provides NGC 6393 for this objects, which turns out to be a galaxy in Draco.The coordinates in Collinder’s catalog actually point to an object in Scorpius, and a check of the Historically Corrected New General Catalogue(HCNGC) Ver 1.02 shows NGC 6396 as the correct alias for Cr. 339. Data from the HCNGC has been used in the updated list.",
        39: "Cr.346: Another misidentified globular cluster.",
        40: "Cr.364: Another misidentified globular cluster.",
        41: "Cr.366: This really is, as Collinder and Lundmark believed, a globular cluster.",
        42: "Cr.368: Another globular cluster, which was not considered certain in Collinder’s time.",
        43: "Cr.371: The NGC number applied by Collinder is actually a reference to the nebula around the cluster.He discovered the cluster himself, but apparently did not know he was first to make the distinction.",
        44: "Cr.374: This cluster is embedded within M24.",
        45: "Cr.377: Cluster associated with M17.",
        46: "Cr.381: Yet another cluster considered a globular when Collinder assembled his catalog.It’s globular status has since been verified.",
        47: "Cr.387: May not be a real cluster.",
        48: "Cr.391: Collinder apparently considered M11 a globular cluster.",
        49: "Cr.395: A strange “object” that appears to be globular cluster NGC 6717(Palomar 9) very near IC 4802, an OC along the same line of sight.",
        50: "Cr.399: The ‘Coathanger’ asterism, which is not a true star cluster.",
        51: "Cr.404: May be an asterism associated with nebula NGC 6820.",
        52: "Cr.409: Globular cluster M71.",
        53: "Cr.414: Was considered a possible globular cluster when Collinder made the catalog.",
        54: "Cr.416 & 417: Possibly only a part of NGC 6885.",
        55: "Cr.425: Probably an asterism in a nebulous region.",
        56: "Cr.426:  It is thought that Collinder’s description of M73 is actually for M72, a globular cluster, and not the object he intended for Cr. 426.",
        57: "Cr.429: Possible cluster associated with NGC 7023; the NGC number does not actually refer to his cluster.",
        58: "Cr.456: May not be a true cluster.",
        59: "Cr.458: Another grouping that may not be a real cluster.",
        60: "Cr.471: Associated with the nebula identified by IC 5146; this number does not apply directly to the cluster."
    };

    //table.addEventListener("click", (event) => {
    //    const target = event.target as HTMLElement;

    //    if (target.tagName.toLowerCase() === "td") {
    //        const text = target.textContent?.trim() ?? "";
    //        const number = parseInt(text, 10);

    //        if (!isNaN(number) && clusterInfo[number]) {
    //            alert(clusterInfo[number]);
    //        }
    //    }
    //});

    table.addEventListener("click", (event) => {
        const target = event.target as HTMLElement;
        const parentTable = target.closest("table");

        if (parentTable && parentTable.id === "sizeFilterTable") {
            if (target.tagName.toLowerCase() === "td") {
                const text = target.textContent?.trim() ?? "";
                const number = parseInt(text, 10);

                if (!isNaN(number) && clusterInfo[number]) {
                    showAlert(clusterInfo[number]);
                }
            }
        }
    });

    //table.addEventListener("click", (event) => {
    //    const table = document.getElementById("sizeFilterTable") as HTMLTableElement;

    //    //if (table) {
    //    const target = event.target as HTMLElement;

    //    // Перевірка: клік був по <td> в межах tbody цієї конкретної таблиці
    //    if (target.tagName.toLowerCase() === "td" && table.contains(target)) {
    //        const parentTable = target.closest("table");
    //        if (parentTable?.id === "sizeFilterTable") {
    //            const text = target.textContent?.trim() ?? "";
    //            const number = parseInt(text, 10);

    //            if (!isNaN(number) && clusterInfo[number]) {
    //                showAlert(clusterInfo[number]);
    //            }
    //        }
    //    }
    //});


}

//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
import { showAlert } from "./alertOfSwal";

document.addEventListener("DOMContentLoaded", () => {
    const button = document.querySelector(".CalculateTelescopeView") as HTMLButtonElement;
    if (button) {
        button.addEventListener("click", calculateTelescopeView);
    }
});

function calculateTelescopeView(): void {
    const getInputValue = (selector: string): number | null => {
        const input = document.querySelector(selector) as HTMLInputElement | null;
        if (!input) return null;

        const value = input.value.trim();
        const number = parseFloat(value);

        if (value === "" || isNaN(number)) {
            input.classList.add("is-invalid");
            return null;
        }

        input.classList.remove("is-invalid");
        return number;
    };

    const getDropdownValue = (selector: string): number => {
        const el = document.querySelector(selector) as HTMLSelectElement | null;
        if (selector)
            return parseFloat(el.value);
    };

    const setOutputText = (selector: string, value: string): void => {
        const el = document.querySelector<HTMLElement>(selector);
        if (el) el.textContent = value;
    };


    const aperture = getInputValue(".text-Aperture");
    const focalLengthTelescope = getInputValue(".text-FocalLengthT");
    const focuserDiameterInches = getDropdownValue(".text-FocuserDiameter");
    const focalLengthEyepiece = getInputValue(".text-FocalLengthE");
    const fieldViewEyepiece = getInputValue(".text-FieldViewE");
    const barlowLens = getDropdownValue(".barlowLens");


    if (
        aperture === null ||
        focalLengthTelescope === null ||
        focalLengthEyepiece === null ||
        fieldViewEyepiece === null
    ) {
        showAlert("Please fill in all fields with valid numbers.");
        return;
    }

    // Calculations
    const moonDistanceKm = 384400;

    const focuserDiameterMM = focuserDiameterInches * 25.399;
    const focalLengthTelescopeBL  = focalLengthTelescope * barlowLens;
    const magnification = (focalLengthTelescopeBL / focalLengthEyepiece);
    const exitPupil = aperture / magnification;

    const focalRatioBL = focalLengthTelescopeBL  / aperture;
    const focalRatio = focalLengthTelescope / aperture;
    const resolvingPowerArcSec = 116 / aperture; // Dawes limit (where 116 is a constant factor in angular seconds)

    //===========================================================================================================================
    // limitingStellarMagnitude
    //===========================================================================================================================
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    const limitingStellarMagnitude = 2 + 5 * Math.log10(aperture);
    // Ця формула найчастіше використовується в астрономії аматорського рівня.
    //    Число 2 є емпіричною константою для невеликих телескопів(10 - 20 см).
    //    Враховує, що людське око може бачити зорі приблизно до 6 - ї зоряної величини неозброєним оком.
    //const limitingStellarMagnitude = 7.5 + 5 * Math.log10(aperture);
    // Ця формула найчастіше використовується в професійній астрономії.
    //    Число 7.5 вказує на теоретичний максимум зоряної величини для більш чутливих інструментів.
    //    Ця формула враховує не тільки розмір апертури, а й більш складні фактори, наприклад, втрати світла в оптиці.


    //===========================================================================================================================
    // fieldOfView
    //===========================================================================================================================
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //const maximumFieldOfView = fieldViewE / magnification;
    // Ця формула використовує безпосередні параметри окуляра та збільшення.
    const maximumFieldOfView = (focuserDiameterMM / focalLengthTelescope) * (180 / Math.PI);
    // Ця формула враховує діаметр фокусера та фокусну відстань телескопа, а також перетворює радіани в градуси.


    const fieldOfViewE = fieldViewEyepiece / magnification;
    const fieldOfView = Math.min(fieldOfViewE, maximumFieldOfView);

    let formatDegreesToDMS = (decimalDegrees: number): string => {
        const degrees = Math.floor(decimalDegrees);
        const minutesDecimal = (decimalDegrees - degrees) * 60;
        const minutes = Math.floor(minutesDecimal);
        const seconds = Math.round((minutesDecimal - minutes) * 60);

        return `${degrees}° ${minutes}' ${seconds}"`;
    }
    const maximumFieldOfViewFormatted = formatDegreesToDMS(maximumFieldOfView);
    const maximumFieldOfViewFormattedE = formatDegreesToDMS(fieldOfView);

    let formatDegreesToMinutesOnly = (decimalDegrees: number): string => {
        const totalMinutes = decimalDegrees * 60;
        return `${totalMinutes.toFixed(2)}`;
    };
    const maximumFieldOfViewInMinutes = formatDegreesToMinutesOnly(maximumFieldOfView);
    const maximumFieldOfViewInMinutesE = formatDegreesToMinutesOnly(fieldOfView);


    //===========================================================================================================================
    // minMoonCrater
    //===========================================================================================================================
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //const minMoonCrater = 200 / aperture;
    // Ця формула використовує простий емпіричний підхід для аматорських спостережень.
    //    Число 200 є емпіричним значенням, яке використовується для аматорських телескопів з апертурою в сантиметрах. Воно
    //    базується на роздільній здатності оптичної системи та середній дистанції до Місяця.
    //const minMoonCrater = (3474 / (aperture * 2));
    // Ця формула більш точна, оскільки враховує реальний діаметр Місяця
    //    Число 3474 — це діаметр Місяця в кілометрах.
    //const minMoonCrater = 3423 / magnification;
    // Ця формула пов’язана з безпосереднім збільшенням телескопа. Добре підходить для практичних спостережень, коли збільшення відоме.
    //    Число 3423 — це похідне значення, яке враховує кутову роздільну здатність телескопа та середню відстань до Місяця. Яке розраховується так:
    //    3474 * 206265 / 384400 ≈ 3423
    //    Де:
    //      3474 км — діаметр Місяця.
    //      384400 км — середня відстань від Землі до Місяця.
    //      206265 — кількість кутових секунд в одному радіані.
    // Calculation of the smallest crater on the Moon that the telescope can resolve, in kilometers.
    const minMoonCrater = 2 * Math.PI * moonDistanceKm * (resolvingPowerArcSec / 3600) / 360;
    ///////////////
    ///// Висновок:
    // Остання формула та формула з числом 3474 найбільш точні, оскільки вони ґрунтується на реальному діаметрі Місяця та оптичній роздільній 
    // здатності телескопа. Інші формули дають лише наближені значення.


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Мінімальний розмір кратера, який людське око зможе побачити при цьому збільшенні
    // (поправка на людське сприйняття, розмір повинен "потрапити" в 1 arcmin)
    // Для цієї формули важливо щоб minMoonCrater розраховувася так: 2 * Math.PI * moonDistanceKm * (resolvingPowerArcSec / 3600) / 360;
    const minMoonCraterWithMagnificatoin  = minMoonCrater * (60 / magnification);

    const minCraterPerceived = Math.max(minMoonCrater, minMoonCraterWithMagnificatoin);
    //===========================================================================================================================

    setOutputText(".magnification", magnification.toFixed(1) + "×");
    setOutputText(".fieldView", maximumFieldOfViewFormattedE + "   (" + fieldOfView.toFixed(2) + "° | " + maximumFieldOfViewInMinutesE + "\")");
    setOutputText(".exitPupil", exitPupil.toFixed(1) + " mm");

    setOutputText(".focalRatioE", "f/" + focalRatioBL.toFixed(1));
    setOutputText(".focalRatio", "f/" + focalRatio.toFixed(2));
    setOutputText(".resolvingPower", resolvingPowerArcSec .toFixed(3) + " arcsec");
    setOutputText(".limitingStellarMagnitude", limitingStellarMagnitude.toFixed(1));
    setOutputText(".maximumFieldOfView", maximumFieldOfViewFormatted + "   (" + maximumFieldOfView.toFixed(3) + "° | " + maximumFieldOfViewInMinutes + "\")");
    setOutputText(".minimumMoonCrater", minMoonCrater.toFixed(1) + " km"); 
    setOutputText(".minimumMoonCraterWithEyepiece", minCraterPerceived.toFixed(1) + " km"); 

    const updateTypicalMagnification = (
        magn: number,
        valueSelector: string,
        eyepieceSelector: string
    ): void => {
        const eyepieceFocalLength = focalLengthTelescope / magn;
        console.log(`$$$$ aperture2: ${aperture}, focalLengthTelescope2: ${focalLengthTelescope}, 
                        magn: ${magn.toFixed(1)}, valueSelector: ${valueSelector}, eyepieceSelector: ${eyepieceSelector}, 
                        eyepieceFocalLength: ${eyepieceFocalLength.toFixed(0)}.`);
        setOutputText(valueSelector, magn.toFixed(1));
        setOutputText(eyepieceSelector, "× (with the eyepiece: " + eyepieceFocalLength.toFixed(0) + "mm )");
    };

    // Typical magnifications (D/5, D/3, D/2, ...)
    const magnifications = [
        { ratio:   aperture / 5, val: ".typicalMagnificationsD5value" , ep: ".eyepieceSizeD5" },
        { ratio:   aperture / 3, val: ".typicalMagnificationsD3value" , ep: ".eyepieceSizeD3" },
        { ratio:   aperture / 2, val: ".typicalMagnificationsD2value" , ep: ".eyepieceSizeD2" },
        { ratio: 0.7 * aperture, val: ".typicalMagnifications07Dvalue", ep: ".eyepieceSize07D" },
        { ratio:       aperture, val: ".typicalMagnificationsDvalue"  , ep: ".eyepieceSizeD" },
        { ratio: 1.4 * aperture, val: ".typicalMagnifications14Dvalue", ep: ".eyepieceSize14D" },
        { ratio:   2 * aperture, val: ".typicalMagnifications2Dvalue" , ep: ".eyepieceSize2D" },
    ];

    magnifications.forEach(({ ratio, val, ep }) =>
        updateTypicalMagnification(ratio, val, ep)
    );

}


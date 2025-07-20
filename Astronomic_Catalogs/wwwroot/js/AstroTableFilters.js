"use strict";
//Object.defineProperty(exports, "__esModule", { value: true }); // для використання мінімізованого файлу. ЗАКОМІТЬ ДЛЯ ВІДЛАДКИ!
/// <reference types="jquery" />
document.addEventListener("DOMContentLoaded", function () {
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///   Н Е   Р О Б И   Н А Д Т О   У Н І В Е Р С А Л Ь Н И Х   Ф У Н К Ц І Й   -   З А Д О В Б А Є Ш С Я   В І Д Л А Г О Д Ж У В А Т И ,   ///
    ///                     А Д Ж Е   J S   В И К О Н У Ю Т Ь   Б Р А У З Е Р И ,   А   В   Н И Х   П О В Н О   Б А Г І В                     ///
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////
    /// ФЛЬТРИ АСТРО КАТАЛОГІВ
    ////////////////////////////////////////////////////////////////////////////////////////////
    /// Чекбокси
    // Встановлення атрибуту який зберігає занчення за замовчуванням
    [".fieldsCheckBoks", ".topMenuFiltersCatalogs", ".additionalBlock"].forEach(selector => {
        const container = document.querySelector(selector);
        if (!container) return;

        container.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
            checkbox.setAttribute("data-initial", checkbox.checked.toString());
        });
    });

    // Кнопки
    const selectAllFieldsCheckBox = document.querySelector(".selectAllFieldsCheckBox");
    const deselectAllFieldsCheckBox = document.querySelector(".deselectAllFieldsCheckBox");
    const setDefaultValueForAllFieldsCheckBox = document.querySelector(".setDefaultValueForAllFieldsCheckBox");

    selectAllFieldsCheckBox?.addEventListener("click", () => {
        toggleCheckboxes(true);
    });
    deselectAllFieldsCheckBox?.addEventListener("click", () => {
        toggleCheckboxes(false);
    });
    setDefaultValueForAllFieldsCheckBox?.addEventListener("click", () => {
        restoreDefaultCheckboxes();
    });

    // Після завантаження одразу оновити стовпці згідно з чекбоксами
    updateAllColumnVisibility();


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // ВСТАНОВЛЕННЯ ОБРОБНИКІВ КЛІКІВ ТА ЗМІН
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    addAngDiameterEventListeners();
    addRaEventListeners();
    addDecEventListeners();
    addPoleEventListener();
    addPlanetsCountEventListeners();
    addDistanceToStarEventListeners();
    setColumnWidthAstroCatalogTable();
});



////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ВСТАНОВЛЕННЯ ЗАЛЕЖНОСТІ ДОСТУПНИХ ЗНАЧЕНЬ МІЖ РІЗНИМИ @Html.DropDownList()
////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Константи. Ці константи необхідні коли допустимі значення вже змінені але потрібно встановити допустимий діапазон за замовчуванням.
const allStaticOptions = {
    "Ang_Diameter_min": generateRangeSingleValue(0, 300),
    "Ang_Diameter_max": generateRangeSingleValue(0, 300),
    "Ang_Diameter_New_min": generateRangeSingleValue(0, 350),
    "Ang_Diameter_New_max": generateRangeSingleValue(0, 350),
    "RA_From_Hours": generateRangedDoubleValue(0, 23),
    "RA_To_Hours": generateRangedDoubleValue(0, 23),
    "RA_From_Minutes": generateRangedDoubleValue(0, 59),
    "RA_To_Minutes": generateRangedDoubleValue(0, 59),
    "RA_From_Seconds": generateRangedDoubleValue(0, 59),
    "RA_To_Seconds": generateRangedDoubleValue(0, 59),
    "Dec_From_Degrees": generateRangedDoubleValue(0, 89),
    "Dec_To_Degrees": generateRangedDoubleValue(0, 89),
    "Dec_From_Minutes": generateRangedDoubleValue(0, 59),
    "Dec_To_Minutes": generateRangedDoubleValue(0, 59),
    "Dec_From_Seconds": generateRangedDoubleValue(0, 59),
    "Dec_To_Seconds": generateRangedDoubleValue(0, 59),
    "discoveredPlenetCountFom": generateRangeSingleValue(1, 8),
    "discoveredPlenetCountTo": generateRangeSingleValue(1, 8),
    "distanceToStarFrom": generateSmartRangeSingleValue(20000),
    "distanceToStarTo": generateSmartRangeSingleValue(20000),
};

function generateRangedDoubleValue(from, to) {
    const result = [];
    for (let i = from; i <= to; i++) {
        const val = i.toString().padStart(2, "0");
        result.push({ value: val, text: val });
    }
    return result;
}

function generateRangeSingleValue(from, to) {
    const result = [];
    for (let i = from; i <= to; i++) {
        const val = i.toString();
        result.push({ value: val, text: val });
    }
    return result;
}

function generateSmartRangeSingleValue(maxValue) {
    var result = [];

    // 0–100 (всі числа)
    for (var i = 0; i <= Math.min(100, maxValue); i++) {
        var val = i.toString();
        result.push({ value: val, text: val });
    }

    // 200–900 (лише сотні), 100 вже додано
    for (var i = 200; i < 1000 && i <= maxValue; i += 100) {
        var val = i.toString();
        result.push({ value: val, text: val });
    }

    // 1000–maxValue (лише тисячі)
    for (var i = 1000; i <= maxValue; i += 1000) {
        var val = i.toString();
        result.push({ value: val, text: val });
    }

    return result;
}

//// Для фільтрів: Apparent dimensions, Right Ascension (RA), Declination (DEC), PlanetsCount, Distance To Star ////
//--  Apparent dimensions  --------------------------------------------------------------------------

function addAngDiameterEventListeners() {
    const elDiameter = [
        ["Ang_Diameter_min", "Ang_Diameter_max"],
        ["Ang_Diameter_New_min", "Ang_Diameter_New_max"]
    ];

    elDiameter.forEach(([minId, maxId]) => {
        [minId, maxId].forEach(id => {
            const el = document.getElementById(id);
            if (el) {
                el.addEventListener("change", () => initializeRangeElementsNatural(minId, maxId, true));
            }
        });
    });
}
//--  Right Ascension (RA)  --------------------------------------------------------------------------
function addRaEventListeners() {
    const elFieldsRA = [
        "RA_From_Hours", "RA_To_Hours",
        "RA_From_Minutes", "RA_To_Minutes",
        "RA_From_Seconds", "RA_To_Seconds"
    ];

    elFieldsRA.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener("change", handleRaDependencies);
        }
    });
}
function handleRaDependencies() {
    let hoursEqual = false;
    let minutesEqual = false;
    let conditionForSeconds = false;
    console.log(`######## FUNCTION: handleRaDependencies`);

    // Hours: обмежуємо завжди
    initializeRangeElementsNatural("RA_From_Hours", "RA_To_Hours", true);

    // Minutes: обмежуємо, тільки якщо години однакові
    hoursEqual = getCondition("RA_From_Hours", "RA_To_Hours");
    initializeRangeElementsNatural("RA_From_Minutes", "RA_To_Minutes", hoursEqual,
        () => {
            minutesEqual = getCondition("RA_From_Minutes", "RA_To_Minutes"); // Тому, що значення цих полів може бути змінене setLowerLimitOptions() або setUpperLimitOptions()!
            conditionForSeconds = hoursEqual && minutesEqual;
            console.log(`MINUTES: conditionForSeconds(${conditionForSeconds}) = hoursEqual(${hoursEqual}) && minutesEqual(${minutesEqual})!`);
            initializeRangeElementsNatural("RA_From_Seconds", "RA_To_Seconds", conditionForSeconds);
        }
    );

    // Seconds: обмежуємо, якщо - години і хвилини однакові
    hoursEqual = getCondition("RA_From_Hours", "RA_To_Hours");
    minutesEqual = getCondition("RA_From_Minutes", "RA_To_Minutes");
    conditionForSeconds = hoursEqual && minutesEqual;
    console.log(`SECONDS: conditionForSeconds(${conditionForSeconds}) = hoursEqual(${hoursEqual}) && minutesEqual(${minutesEqual})!`);
    initializeRangeElementsNatural("RA_From_Seconds", "RA_To_Seconds", conditionForSeconds);

}
//--  Declination (DEC)  --------------------------------------------------------------------------
function addDecEventListeners() {
    const elFieldsDEC = [
        "Dec_From_Degrees", "Dec_To_Degrees",
        "Dec_From_Minutes", "Dec_To_Minutes",
        "Dec_From_Seconds", "Dec_To_Seconds"
    ];

    elFieldsDEC.forEach(id => {
        const el = document.getElementById(id);
        if (el) {
            el.addEventListener("change", handleDecDependencies);
        }
    });
}
function handleDecDependencies() {
    console.log("FUNCTION: handleDecDependencies");
    let polesEqual = false;
    let degreesEqual = false;
    let minutesEqual = false;
    let conditionForMinutes = false;
    let conditionForSeconds = false;
    console.log(`######## FUNCTION: handleDecDependencies`);

    // Degrees: обмежуємо тільки, якщо полюcи одинакові
    // Не додав callbeck функцію оскільки на зміну полюсів неробиш жодних змін!
    polesEqual = getCondition("Dec_From_Pole", "Dec_To_Pole");
    initializeRangeElementsIntegers("Dec_From_Degrees", "Dec_To_Degrees", polesEqual);

    // Minutes: обмежуємо, якщо полюcи та градуси однакові
    polesEqual = getCondition("Dec_From_Pole", "Dec_To_Pole");
    degreesEqual = getCondition("Dec_From_Degrees", "Dec_To_Degrees");
    conditionForMinutes = polesEqual && degreesEqual;
    initializeRangeElementsIntegers("Dec_From_Minutes", "Dec_To_Minutes", conditionForMinutes,
        () => {
            minutesEqual = getCondition("Dec_From_Minutes", "Dec_To_Minutes"); // Тому, що значення цих полів може бути змінене setLowerLimitOptions() або setUpperLimitOptions()!
            conditionForSeconds = polesEqual && degreesEqual && minutesEqual;
            console.log(`MINUTES: conditionForSeconds(${conditionForSeconds}) = hoursEqual(${degreesEqual}) && minutesEqual(${minutesEqual})!`);
            initializeRangeElementsIntegers("Dec_From_Seconds", "Dec_To_Seconds", conditionForSeconds);
        }
    );

    // Seconds: обмежуємо, якщо - градуси і хвилини однакові
    polesEqual = getCondition("Dec_From_Pole", "Dec_To_Pole");
    degreesEqual = getCondition("Dec_From_Degrees", "Dec_To_Degrees");
    minutesEqual = getCondition("Dec_From_Minutes", "Dec_To_Minutes");
    conditionForSeconds = polesEqual && degreesEqual && minutesEqual;
    console.log(`SECONDS: conditionForSeconds(${conditionForSeconds}) = hoursEqual(${degreesEqual}) && minutesEqual(${minutesEqual})!`);
    initializeRangeElementsIntegers("Dec_From_Seconds", "Dec_To_Seconds", conditionForSeconds);
}
//--  PlanetsCount  --------------------------------------------------------------------------
function addPlanetsCountEventListeners() {
    const elFields = ["discoveredPlenetCountFom", "discoveredPlenetCountTo"];

    elFields.forEach(id => {
        const el = document.getElementById(id);
        if (el)
            el.addEventListener("change", () => initializeRangeElementsNatural(elFields[0], elFields[1], true));
    });
}
//--  Distance To Star  --------------------------------------------------------------------------
function addDistanceToStarEventListeners() {
    const elFields = ["distanceToStarFrom", "distanceToStarTo"];

    elFields.forEach(id => {
        const el = document.getElementById(id);
        if (el)
            el.addEventListener("change", () => initializeRangeElementsNatural(elFields[0], elFields[1], true));
    });
}
//--  Switch button to fix column width  --------------------------------------------------------------------------
function setColumnWidthAstroCatalogTable() {
    const checkbox = document.querySelector("label.switchButtonFixColumnWidth input[type='checkbox']");
    if (!checkbox) return;

    checkbox.addEventListener("change", () => {
        setAstroCatalogColumnWidth(checkbox);
    });
}
//----------------------------------------------------------------------------
function getCondition(fromId, toId) {
    const fromEl = document.getElementById(fromId);
    const toEl = document.getElementById(toId);
    if (!fromEl || !toEl) return false;

    let relativeCondition = fromEl.value === toEl.value;
    return relativeCondition;
}

function initializeRangeElementsIntegers(fromId, toId, relativeCondition = false, onChangeCallback = null) {
    console.log(`******** initializeRangeElementsIntegers FOR ${fromId} AND ${toId}: WITH Condition: ${relativeCondition}`);
    const fromEl = document.getElementById(fromId);
    const toEl = document.getElementById(toId);
    console.log(`****  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
    let fromPole = document.getElementById("Dec_From_Pole");
    let toPole = document.getElementById("Dec_To_Pole");
    if (!fromEl || !toEl || !fromPole || !toPole) return;

    // Дістаємо весь діапазон опцій для вибраного списку
    const allOptions = allStaticOptions[fromId];
    const isDescendingFromPole = fromPole.value === "-";
    const isDescendingToPole = toPole.value === "-";

    if (relativeCondition) { // Якщо true — встановити обрізані випадаючі списки
        // НЕ МІНЯЙ ПОРЯДОК ВИКЛИКУ setLowerLimit ІЗ setUpperLimit - ІНАКШЕ НЕ ПРАЦЮВАТИМЕ ПРАВЕЛЬНЕ АВТОВСТАНОВЛЕННЯ ЗНАЧЕНЬ В ПОЛЯ!
        setLowerLimitOptionsIntegers(fromEl, toEl, allOptions, isDescendingToPole);
        console.log(`**  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
        setUpperLimitOptionsIntegers(fromEl, toEl, allOptions, isDescendingFromPole);
        console.log(`*  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
    } else { // При знятті обмежень — відновлюємо повний список
        console.log("#### relativeCondition = false");
        setOptions_Sortig(fromEl, allOptions, isDescendingFromPole);
        setOptions_Sortig(toEl, allOptions, isDescendingToPole);
    }

    if (onChangeCallback) onChangeCallback();
}
// Функції обрізки максимального та мінімального значень
function setLowerLimitOptionsIntegers(minSelectObject, maxSelectObject, allOptions, isDescending) {
    console.log(`FUNCTION: setLowerLimitOptionsIntegers WITH maxSelectObject-value = ${maxSelectObject.value} AND minSelectObject.value = ${minSelectObject.value} AND isDescending = ${isDescending}`);
    const minValue = parseInt(minSelectObject.value);
    let validOptions;

    if (isDescending) {
        validOptions = allOptions.filter(opt => parseInt(opt.value) <= minValue);
    } else {
        validOptions = allOptions.filter(opt => parseInt(opt.value) >= minValue);
    }
    console.log(`validOptions(minSelectObject).lengh = ${Array.from(validOptions).length}`)

    setOptions_Sortig(maxSelectObject, validOptions, isDescending);
}
function setUpperLimitOptionsIntegers(minSelectObject, maxSelectObject, allOptions, isDescending) {
    console.log(`FUNCTION: setUpperLimitOptionsIntegers WITH minSelectObject.value = ${minSelectObject.value} AND maxSelectObject-value = ${maxSelectObject.value} AND isDescending = ${isDescending}`);
    const maxValue = parseInt(maxSelectObject.value);
    let validOptions;

    if (isDescending) {
        validOptions = allOptions.filter(opt => parseInt(opt.value) >= maxValue);
    } else {
        validOptions = allOptions.filter(opt => parseInt(opt.value) <= maxValue);
    }
    console.log(`validOptions(maxSelectObject).lengh = ${Array.from(validOptions).length}`)

    setOptions_Sortig(minSelectObject, validOptions, isDescending);
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function initializeRangeElementsNatural(fromId, toId, relativeCondition = false, onChangeCallback = null) {
    console.log(`******** initializeRangeElementsNatural FOR ${fromId} AND ${toId}: WITH Condition: ${relativeCondition}`);
    const fromEl = document.getElementById(fromId);
    const toEl = document.getElementById(toId);
    console.log(`****  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
    if (!fromEl || !toEl) return;

    // Дістаємо весь діапазон опцій для вибраного списку
    const allOptions = allStaticOptions[fromId];
    console.log(`fromId = ${fromId} | toId = ${toId}`);

    if (relativeCondition) { // Якщо true — встановити обрізані випадаючі списки
        //var list = Array.from(validOptions).join(',');
        console.log(`***  relativeCondition IS TRUE, SO: fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`);
        // НЕ МІНЯЙ ПОРЯДОК ВИКЛИКУ setLowerLimit ІЗ setUpperLimit - ІНАКШЕ НЕ ПРАЦЮВАТИМЕ ПРАВЕЛЬНЕ АВТОВСТАНОВЛЕННЯ ЗНАЧЕНЬ В ПОЛЯ!
        setLowerLimitOptionsNatural(fromEl, toEl, allOptions);
        console.log(`**  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
        setUpperLimitOptionsNatural(fromEl, toEl, allOptions);
        console.log(`*  fromEl = ${fromEl.value} AND toEl = ${toEl.value}!!!`)
    } else { // При знятті обмежень — відновлюємо повний список
        console.log("#### relativeCondition = false");
        setOptions(fromEl, allOptions);
        setOptions(toEl, allOptions);
    }

    if (onChangeCallback) onChangeCallback();
}

// Функції обрізки максимального та мінімального значень
function setLowerLimitOptionsNatural(minSelectObject, maxSelectObject, allOptions) {
    console.log(`FUNCTION: setLowerLimitOptionsNatural WITH minSelectObject.value = ${minSelectObject.value} AND maxSelectObject-value = ${maxSelectObject.value}`);
    const minValue = parseInt(minSelectObject.value);
    const validOptions = allOptions.filter(opt => parseInt(opt.value) >= minValue);
    //const ForDebug = Array.from(validOptions).length;
    console.log(`validOptions(maxSelectObject).lengh = ${Array.from(validOptions).length}`)
    setOptions(maxSelectObject, validOptions);
}
function setUpperLimitOptionsNatural(minSelectObject, maxSelectObject, allOptions) {
    console.log(`FUNCTION: setUpperLimitOptionsNatural WITH maxSelectObject-value = ${maxSelectObject.value} AND minSelectObject.value = ${minSelectObject.value}`);
    const maxValue = parseInt(maxSelectObject.value);
    const validOptions = allOptions.filter(opt => parseInt(opt.value) <= maxValue);
    //const ForDebug = Array.from(validOptions).length;
    console.log(`validOptions(minSelectObject).lengh = ${Array.from(validOptions).length}`)
    setOptions(minSelectObject, validOptions);
}

// Функція безпосереднього встановлення значеннь в блоки HTML
function setOptions(selectEl, validOptions) {
    console.log(`FUNCTION: setOptions WITH selectEl-value = ${selectEl.value} AND validOptions.length = ${Array.from(validOptions).length}`);
    const currentValue = selectEl.value;
    selectEl.innerHTML = "";
    console.log(`FUNCTION setOptions: currentValue = ${currentValue}`)

    // Використання тут setTimeout() необхідне для вирішення проблеми з мікротаймінгами та синхронізацією DOM при роботі з
    //   select - елементами та динамічним оновленням їхніх option.
    //
    // ПОЯСНЕННЯ ChatGPT:
    //   🔍 Що відбувається:
    //   Коли ти вперше змінюєш Ang_Diameter_min, викликається initializeRangeElementsXxx, який змінює innerHTML списку select, і в той же момент
    //      браузер ще обробляє подію change, пов’язану з твоїм вибором.
    //   Проблема виникає через те, що ти зчитуєш select.value, а потім перезаписуєш innerHTML(тобто повністю викидаєш старі < option >), і при
    //      цьому намагаєшся залишити вибране значення. Але — якщо такого значення немає серед нових option, то браузер бере перше доступне.
    //
    //   ❗Чому console.log(...) «фіксить» проблему:
    //   Це класичний race condition / timing issue.Коли додається console.log, JavaScript трохи «зупиняється» (виконується повільніше), що може
    //      вплинути на синхронізацію між change - подією і зміною innerHTML.Браузер просто встигає правильно все обробити до того, як опції
    //      перезаписуються.
    //
    // ПРИБРАВ ЦЮ ФУНКЦІЮ АДЖЕ ДОДАТКОВА ЗАТРИМКА ПОРУШУЄ РОБОТУ ІНШОГО КОДУ, ЗОКРЕМА handleRaDependencies ТА handleDecDependencies.
    var forDevelopment = "To separate the previous and next comments.";
    //setTimeout(() => {
    validOptions.forEach(opt => {
        const option = document.createElement("option");
        option.value = opt.value;
        option.text = opt.text;
        if (opt.value === currentValue) {
            option.selected = true;
        }
        selectEl.appendChild(option);
    });
    //}, 0);
    console.log(`"""""""" selectEl-value = ${selectEl.value}`);
}


//// Для полів: Pole ////
function addPoleEventListener() {
    let fromPole = document.getElementById("Dec_From_Pole");
    let toPole = document.getElementById("Dec_To_Pole");
    if (!fromPole || !toPole) return;

    fromPole.addEventListener("change", function () {
        toPole = document.getElementById("Dec_To_Pole");
        const animateBloksToId = ["Dec_To_Pole", "Dec_To_Degrees", "Dec_To_Minutes", "Dec_To_Seconds",];

        if (this.value === "+" && toPole.value === "-") {
            toPole.value = "+";
            animateFieldBackground(animateBloksToId);
        }
        if (this.value === toPole.value) {
            console.log(`fromPole = ${fromPole.value} === toPole = ${toPole.value}`);
            handleDecDependencies();
        }
        else {
            updateDecDropdownOrder("From", this.value);
            updateDecDropdownOrder("To", toPole.value);
        }
    });

    toPole.addEventListener("change", function () {
        fromPole = document.getElementById("Dec_From_Pole");
        const animateBloksFromId = ["Dec_From_Pole", "Dec_From_Degrees", "Dec_From_Minutes", "Dec_From_Seconds",];

        if (this.value === "-" && fromPole.value === "+") {
            fromPole.value = "-";
            animateFieldBackground(animateBloksFromId);
        }

        if (this.value === fromPole.value) {
            console.log(`fromPole = ${fromPole.value} === toPole = ${toPole.value}`);
            handleDecDependencies();
        }
        else {
            updateDecDropdownOrder("From", fromPole.value);
            updateDecDropdownOrder("To", this.value);
        }
    });

    // Ініціалізуємо порядок при завантаженні
    updateDecDropdownOrder("From", fromPole.value);
    updateDecDropdownOrder("To", toPole.value);
}

function animateFieldBackground(animateBloksId) {
    const animateBloks = animateBloksId.map(id => document.getElementById(id))
        .filter(b => b); // лишаємо тільки існуючі елементи

    if (animateBloks.length === 0) return; // Не помішало б додати перевірку саме на наявність Dec_To_Pole і Dec_From_Pole

    animateBloks.forEach(blok => {
        blok.classList.remove("pole-flash"); // На випадок подвійного кліку
        void blok.offsetWidth; // форсуємо ререндер, щоб браузер "перезарядив" анімацію
        blok.classList.add("pole-flash");
    });

    // Після завершення анімації — видаляємо клас та обробник
    const firstEl = animateBloks[0];
    const handler = () => {
        animateBloks.forEach(blok => blok.classList.remove("pole-flash"));
        firstEl.removeEventListener("animationend", handler);
    };
    firstEl.addEventListener("animationend", handler);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
function updateDecDropdownOrder(type, poleValue) {
    const suffix = type === "From" ? "From" : "To";
    const isDescending = poleValue === "-";
    console.log(`suffix = ${suffix} | isDescending = ${isDescending}`)

    const degreesId = `Dec_${suffix}_Degrees`;
    const minutesId = `Dec_${suffix}_Minutes`;
    const secondsId = `Dec_${suffix}_Seconds`;

    const degreesEl = document.getElementById(degreesId);
    const minutesEl = document.getElementById(minutesId);
    const secondsEl = document.getElementById(secondsId);
    if (!degreesEl || !minutesEl || !secondsEl) return;

    // ПІСЛЯ ВІДЛАДКИ ВИКОРИСТОВУЙ ФУНКЦІЮ setOptions()!!! -   В   Ж О Д Н О М У   Р А З І !!!  ДИВ. ЗАГОЛОВОК ЦЬОГО ФАЙЛУ ПРО НАДМІРНО УНІВЕРСАЛЬНІ ФУНКЦІЇ!!!
    setOptions_Sortig(degreesEl, allStaticOptions[degreesId], isDescending);
    setOptions_Sortig(minutesEl, allStaticOptions[minutesId], isDescending);
    setOptions_Sortig(secondsEl, allStaticOptions[secondsId], isDescending);
}

function setOptions_Sortig(selectEl, validOptions, isDescending = false) {
    console.log(`FUNCTION: setOptions_Sortig WITH selectEl-value = ${selectEl.value} AND validOptions.length = ${Array.from(validOptions).length} AND isDescending = ${isDescending}`);
    const currentValue = selectEl.value;
    selectEl.innerHTML = "";
    const sortedOptions = isDescending ? [...validOptions].sort((a, b) => parseInt(b.value) - parseInt(a.value)) : validOptions;

    sortedOptions.forEach(opt => {
        const option = document.createElement("option");
        option.value = opt.value;
        option.text = isDescending ? `-${opt.text}` : opt.text;
        if (opt.value === currentValue) { // ЯКЩО НЕ ЗНАХОДИТЬ, ТО ВСТАНОВЛЮЄ МІНІМАЛЬНЕ: 00
            option.selected = true;
        }
        selectEl.appendChild(option);
    });
    console.log(`"""""""" selectEl-value = ${selectEl.value}`);
}



////////////////////////////////////////////////////////////////////////////////////////////
/// ФЛЬТРИ КОЛОНОК ТАБЛИЦЬ АСТРО КАТАЛОГІВ
////////////////////////////////////////////////////////////////////////////////////////////
// Чекбокси
window.toggleCheckbox = toggleCheckbox;

function toggleCheckbox(checkboxElement, className) {
    updateColumnVisibility(checkboxElement.checked, className);
}

//// Універсальна функція: приховує або показує стовпець по класу ////
function updateColumnVisibility(visible, className) {
    console.log(`FUNCTION: updateColumnVisibility(${visible}, ${className})`);
    // Знайти всі заголовки стовпців
    const headers = document.querySelectorAll(".hideShowColumnTable table.table thead tr th");
    //УВАГА!!! Функція querySelectorAll(), навідміну від querySelector(), ЗАВЖДИ повертає NodeList тому перевірка має бути така: 
    if (headers.length === 0) return;

    let columnIndex = -1;

    // Знайти індекс стовпця, який має потрібний клас
    headers.forEach((th, index) => {
        /*console.log(`columnIndex = ${columnIndex} | className = ${className}`)*/
        if (th.classList.contains(className)) {
            columnIndex = index + 1; // +1 бо nth-child індексується з 1

            // Показати/сховати стовпець //
            // th
            document.querySelectorAll(`.hideShowColumnTable table.table thead th:nth-child(${columnIndex})`).forEach(th => {
                toggleColumnWithAnimation(th, visible);
            });

            // td
            document.querySelectorAll(`.hideShowColumnTable table.table tbody tr td:nth-child(${columnIndex})`).forEach(td => {
                toggleColumnWithAnimation(td, visible);
            });
        }
    });

    if (columnIndex === -1) {
        console.warn("Стовпець з класом " + className + " не знайдено.");
        return;
    }

}

//// Додати/зняти анімаційний клас ////
function toggleColumnWithAnimation(element, show) {
    element.classList.add("fade-column");

    // Якщо потрібно відобразити — показати плавно
    if (show) {
        // Встановлюємо ширину перед видаленням класу, щоб уникнути "auto"
        element.style.width = element.scrollWidth + "px";
        requestAnimationFrame(() => {
            element.classList.remove("hidden-column");
        });
    } else {
        // Фіксуємо поточну ширину, щоб анімація знала з чого починати
        element.style.width = element.offsetWidth + "px";
        // Дати можливість браузеру застосувати стиль
        requestAnimationFrame(() => {
            element.classList.add("hidden-column");
        });
    }
    // затримка зроблена щоб перед приховування стовпців дати час на виконання анімації, і ця затримка визначається в стилях в класі .fade-column:
    setTimeout(() => element.hidden = !show, 400);
}

/// Встановлення максимальної/мінімальної ширини колонок
function setAstroCatalogColumnWidth(checkbox) {
    console.log("FUNCTION: setAstroCatalogColumnWidth");
    const table = document.querySelector("table.astroCatalogTable");

    if (!checkbox || !table) return;

    const isChecked = checkbox.checked;
    console.log(`@@@@@@ Checkbox is ${isChecked}!`);

    console.log("Before toggle:", table.classList.toString());
    table.classList.toggle("setColumnWidth", isChecked);
    console.log("After toggle:", table.classList.toString());
}

//// Показати/приховати ВСІ колонки на основі прапорця ////
function toggleCheckboxes(checked) {
    const container = document.querySelector(".fieldsCheckBoks");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        checkbox.checked = checked;
    });

    updateAllColumnVisibility();
}

//// Відновити початковий стан усіх чекбоксів які вдображають/приховують поля таблиці ////
function restoreDefaultCheckboxes() {
    const container = document.querySelector(".fieldsCheckBoks");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        const initial = checkbox.getAttribute("data-initial");
        if (initial !== null) {
            checkbox.checked = initial === "true";
        }
    });

    updateAllColumnVisibility();
    setTimeout(() => {
        setAstroCatalogColumnWidth();
    }, 2);
}

//// Відновити початковий стан усіх чекбоксів з Object filters та Additional блоків ////
function restoreDefaultObjectAdditionalCheckboxes() {
    const container = document.querySelector(".topMenuFiltersCatalogs");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        const initial = checkbox.getAttribute("data-initial");
        if (initial !== null) {
            const newValue = initial === "true";
            if (checkbox.checked !== newValue) {
                checkbox.checked = newValue;
                checkbox.dispatchEvent(new Event('change', { bubbles: true }));
            }
        }
    });

}

// Оновити відображення всіх колонок згідно з поточним станом чекбоксів
function updateAllColumnVisibility() {
    const container = document.querySelector(".fieldsCheckBoks");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(checkbox => {
        // Отримуємо ім'я класу з класу типу `chb-SubObject` => `SubObject`
        const classes = checkbox.className.split(' ');
        const className = classes.find(cls => cls.startsWith('chb-'))?.replace('chb-', '');

        if (className) {
            updateColumnVisibility(checkbox.checked, className);
        }
    });
}



////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ФІЛЬТРИ ОБ'ЄКТІВ
////////////////////////////////////////////////////////////////////////////////////////////////////////////
window.resetObjectSelection = resetObjectSelection;
window.resetTypeCatalogSelection = resetTypeCatalogSelection;
window.resetAllFilters = resetAllFilters;

function resetObjectSelection() {
    const container = document.querySelector(".responsiveColumnsContainer");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(cb => {
        cb.checked = false;
    });
}

function resetTypeCatalogSelection() {
    const container = document.querySelector(".typeCatalogFilters");
    if (!container) return;

    const checkboxes = container.querySelectorAll('input[type="checkbox"]');
    checkboxes.forEach(cb => {
        cb.checked = true;
    });
}

function resetFiltersToDefault() {
    const selects = document.querySelectorAll('select[data-default]');
    //УВАГА!!! Функція querySelectorAll(), навідміну від querySelector(), ЗАВЖДИ повертає NodeList тому перевірка має бути така: 
    if (selects.length === 0) return;

    selects.forEach(select => {
        select.value = select.getAttribute('data-default');
    });


    /// ВСТАНОВЛЕННЯ ВСЬОГО(початкового) ДІАПАЗОНУ ВИБОРУ
    const doropDounlListNaturalFields = [
        ["Ang_Diameter_min", "Ang_Diameter_max"],
        ["Ang_Diameter_New_min", "Ang_Diameter_New_max"],
        ["RA_From_Hours", "RA_To_Hours"],
        ["RA_From_Minutes", "RA_To_Minutes"],
        ["RA_From_Seconds", "RA_To_Seconds"],
        ["discoveredPlenetCountFom", "discoveredPlenetCountTo"],
        ["distanceToStarFrom", "distanceToStarTo"]
    ];
    // ТуреЕггоп e.forEach is not a function
    doropDounlListNaturalFields.forEach(([minId, maxId]) => {
        // Використовуєш лише перший елемент, оскільки у функцію initializeRangeElementsNatural передаються відразу обидва, а значення останнього 
        //   параметру false - тому ця функція встановлює повний діаназон обидвом елементам!
        console.log(`**** RESORE RANGE FOR Natural: ${minId} AND ${maxId}`);
        const minEl = document.getElementById(minId);
        console.log(`minEl is ${minEl}`);
        if (minEl) {
            console.log("ForEach before -> Natural");
            initializeRangeElementsNatural(minId, maxId, false);
        }
    })

    const doropDounlListIntegerFields = [
        ["Dec_From_Degrees", "Dec_To_Degrees"],
        ["Dec_From_Minutes", "Dec_To_Minutes"],
        ["Dec_From_Seconds", "Dec_To_Seconds"]
    ];
    doropDounlListIntegerFields.forEach(([minId, maxId]) => {
        // Використовуєш лише перший елемент, оскільки у функцію initializeRangeElementsNatural передаються відразу обидва, а значення останнього
        //   параметру false - тому ця функція встановлює повний діаназон обидвом елементам!
        console.log(`**** RESORE RANGE FOR Integers: ${minId} AND ${maxId}`);
        const minEl = document.getElementById(minId);
        console.log(`minEl is ${minEl}`);
        if (minEl) {
            console.log("ForEach before -> Integers");
            initializeRangeElementsIntegers(minId, maxId, false);
        }
    })
}

function resetConstellationSelection() {
    // ВАДИЛИ ОДИН З НАСТУПНИХ БЛОКІВ ПІСЛЯ ТОГО ЯК ВИЗНАЧИШСЯ ЯКИМ ІНСТРУМЕНТОМ ХОЧЕШ ВИБИРАТИ СУЗІР'Я!
    const select = document.getElementById('constellationSelect');
    reselectMultyselection(select);

    const checkboxes = document.querySelectorAll('.constellationSelectTable input[type="checkbox"]');
    if (checkboxes.length === 0) return;

    checkboxes.forEach(checkbox => {
        checkbox.checked = false;
    });
}

function resetMultipleSelection() {
    const classNames = ["planetTypeSelect", "exoplanetTelescope", "exoplanetPlanetName", "exoplanetDiscoveryMethod"];
    if (classNames.length === 0) return;

    classNames.forEach(className => {
        const elements = document.querySelectorAll(`.` + className);
        elements.forEach(element => {
            reselectMultyselection(element);
        });
    });
}

function reselectMultyselection(select) {
    if (select && select.options) {
        Array.from(select.options).forEach(option => {
            option.selected = false;
            option.classList.remove("selected-fix"); // Цей клас також використовується у файлі theme.ts -->  fixSelectBehavior()!
        });
    }
}

function clearTextboxesByClass(className) {
    const selects = document.querySelectorAll(`input[type="text"].${className}`);
    if (selects.length === 0) return;

    selects.forEach(input => {
        input.value = "";
    });
}

/// Функція по встановленю взаємних обмежень календаря визначена у файлі: calendarHandler.ts
function resetDateFilters() {
    console.log("FUNCTION: resetDateFilters");
    const discoveredDateBlock = document.querySelectorAll('.discoveredDateBlock');
    if (discoveredDateBlock.length === 0) return;

    const dateFrom = document.getElementById("dateFrom");
    const dateTo = document.getElementById("dateTo");
    const today = new Date().toISOString().split("T")[0];

    if (dateFrom) {
        dateFrom.value = "";
        dateFrom.min = "1992-01-01";
        dateFrom.max = today;
    }

    if (dateTo) {
        dateTo.value = "";
        dateTo.min = "1992-01-01";
        dateTo.max = today;
    }
}

function resetAllFilters() {
    restoreDefaultCheckboxes();
    restoreDefaultObjectAdditionalCheckboxes();
    resetObjectSelection();
    resetTypeCatalogSelection();
    resetFiltersToDefault();
    resetConstellationSelection();
    resetMultipleSelection();
    resetDateFilters();
    clearTextboxesByClass("text-Name");
}


////////////////////////////////////////////////////////////////////////////////////////////
// С П Л И В А Ю Ч І   П І Д К А З К И
////////////////////////////////////////////////////////////////////////////////////////////
/// NGC2000 ///
//// 1. Оголошуємо Map з усіма іменами класів і відповідними повідомленнями
//var alertMessages = new Map([
//    ["Comment", "Comment from the NameObject table."],
//    ["SourceType", "Object type. See the SourceType table."],
//    ["Other_names", "Name from other catalogs or historically established. In this case, from the Object field of the NameObject table."],
//    ["Name_UK", "Other unique name (taken from the NGC2000_UK catalog. Analogous to the Othernames field). THESE TWO FIELDS SHOULD BE MERGED EVENTUALLY!!!"],
//    ["LimitAngDiameter", "This field contains the character \"<\" if the object's size is an upper limit; otherwise, the field is empty."],
//    ["AngDiameter", "This field contains the size of the object in arcminutes. It is the angular size measured along the largest dimension. It is taken from the NGC2000_UK catalog."],
//    ["RA", "RA or LII – Galactic longitude of the NGC/IC object – right ascension (analogous to longitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 0.1 minutes of time in the original table. Measured in hours."],
//    ["DEC", "DEC or BII – Galactic latitude of the NGC/IC object – declination (analogous to latitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 1 arcminute in the original table."],
//    ["App_Mag", "This field contains the integrated (total) magnitude of the type specified in the app_mag_flag field. Accuracy varies."],
//    ["App_Mag_Flag", "This field contains a flag indicating the magnitude type. It contains a space if the integrated magnitude is visual, or \"p\" if it is photographic (blue)."],
//    ["b_mag", "This field contains the brightness of the object in the blue range (B-band, \"Blue\"), approximately 440 nm."],
//    ["v_mag", "This field contains the stellar magnitude of the object in the visible range (V-band, \"Visual\"), around 550 nm."],
//    ["j_mag", "This field contains the brightness of the object in the infrared range (J-band), with a wavelength of about 1.25 μm."],
//    ["h_mag", "This field contains the brightness of the object in an even longer infrared range (H-band), around 1.65 μm."],
//    ["k_mag", "This field contains the brightness of the object further into the infrared range (K-band), around 2.2 μm."],
//    ["Surface_Brigthness", "It indicates the average brightness of an object per unit area: the brightness of the object distributed over a unit area in the sky (usually expressed in magnitudes per square arcsecond, e.g., mag/arcsec²). The lower the value, the brighter the object appears per unit area!"],
//    ["Hubble_OnlyGalaxies", "Hubble type of the galaxy."],
//    ["Cstar_UMag", "This field contains the brightness (stellar magnitude) of the nearest star in the ultraviolet range (U-band)."],
//    ["Cstar_BMag", "This field contains the brightness of the nearest star in the blue range (B-band, \"Blue\")."],
//    ["Cstar_VMag", "This field contains the brightness of the nearest star in visible light (V-band, \"Visual\")."],
//    ["Cstar_Names", "This field contains the names or identifiers of the star specified in the fields: Cstar_UMag, Cstar_BMag, Cstar_VMag."]
//]);

//// 2. Додаємо обробник кліку
//document.addEventListener("click", function (event) {
//    var target = event.target;
//    if (!target.classList) return;

//    for (var i = 0; i < target.classList.length; i++) {
//        var cls = target.classList[i];
//        var message = alertMessages.get(cls);
//        if (message) {
//            alert(message);
//            break; // зупинити після першого збігу
//        }
//    }
//});





/// Старий код:
//$(".Name_UK").on("click", function () {
//    alert("Other unique name (taken from the NGC2000_UK catalog. Analogous to the Othernames field). THESE TWO FIELDS SHOULD BE MERGED EVENTUALLY!!!");
//})
//$(".Comment").on("click", function () {
//    alert("Comment from the NameObject table.");
//    //alert("Коментар з вкладки NameObject.");
//})
//$(".SourceType").on("click", function () {
//    alert("Object type. See the SourceType table.");
//})
//$(".Other_names").on("click", function () {
//    alert("Name from other catalogs or historically established. In this case, from the Object field of the NameObject table.");
//})
//$(".LimitAngDiameter").on("click", function () {
//    alert("This field contains the character &quot;<&quot; if the object's size is an upper limit; otherwise, the field is empty.");
//    //alert("Це поле містить символ «<», якщо розмір об’єкта є верхньою межею, інакше це поле пусте.");
//})
//$(".AngDiameter").on("click", function () {
//    alert("This field contains the size of the object in arcminutes. It is the angular size measured along the largest dimension. It is taken from the NGC2000_UK catalog.");
//    //alert("Це поле містить розмір об’єкта в дугових хвилинах. Це кутовий розмір, виміряний уздовж найбільшого виміру. Взято з каталогу NGC2000_UK.");
//})
//$(".RA").on("click", function () {
//    alert("RA or LII – Galactic longitude of the NGC/IC object – right ascension (analogous to longitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 0.1 minutes of time in the original table. Measured in hours.");
//    //alert("Розбирися з цим і напиши тут та в Collinder catalog нормальний комент.");
//})
//$(".DEC").on("click", function () {
//    alert("DEC or BII – Galactic latitude of the NGC/IC object – declination (analogous to latitude on Earth's surface) of the NGC/IC object on the selected equinox date. Provided in J2000 coordinates, with an accuracy of 1 arcminute in the original table.");
//    //alert("Схилення об’єкта NGC/IC у вибраний день рівнодення. Це було дано в координатах 2000 з точністю до 1 кутової хвилини в оригінальній таблиці.");
//})
//$(".App_Mag").on("click", function () {
//    alert("This field contains the integrated (total) magnitude of the type specified in the app_mag_flag field. Accuracy varies.");
//    //alert("Це поле містить інтегровану (загальну) величину типу, зазначеного в полі app_mag_flag. Точність різна.");
//})
//$(".App_Mag_Flag").on("click", function () {
//    alert("This field contains a flag indicating the magnitude type. It contains a space if the integrated magnitude is visual, or &quot;p&quot; if it is photographic (blue).");
//    //alert("Це поле містить прапорець, який вказує тип величини. Він містить пробіл, якщо інтегрована величина є візуальною, «p», якщо це фотографічна(синя) величина.");
//})
//$(".b_mag").on("click", function () {
//    alert("This field contains the brightness of the object in the blue range (B-band, \"Blue\"), approximately 440 nm.");
//    //alert("Це поле містить яскравість об'єкту в синьому діапазоні (B-band, \"Blue\"), приблизно 440 нм.");
//})
//$(".v_mag").on("click", function () {
//    alert("This field contains the stellar magnitude of the object in the visible range (V-band, \"Visual\"), around 550 nm.");
//    //alert("Це поле містить зоряну величину об'єкту у видимому діапазоні (V-band, \"Visual\"), близько 550 нм.");
//})
//$(".j_mag").on("click", function () {
//    alert("This field contains the brightness of the object in the infrared range (J-band), with a wavelength of about 1.25 μm.");
//    //alert("Це поле містить яскравість об'єкту в інфрачервоному діапазоні (J-band), довжина хвилі близько 1,25 мкм.");
//})
//$(".h_mag").on("click", function () {
//    alert("This field contains the brightness of the object in an even longer infrared range (H-band), around 1.65 μm.");
//    //alert("Це поле містить яскравість об'єкту в ще довшому інфрачервоному діапазоні (H-band), близько 1,65 мкм.");
//})
//$(".k_mag").on("click", function () {
//    alert("This field contains the brightness of the object further into the infrared range (K-band), around 2.2 μm.");
//    //alert("Це поле містить яскравість об'єкту в  ще далі в ІЧ-діапазоні (K-band), близько 2,2 мкм.");
//})
//$(".Surface_Brigthness").on("click", function () {
//    alert("It indicates the average brightness of an object per unit area: the brightness of the object distributed over a unit area in the sky (usually expressed in magnitudes per square arcsecond, e.g., mag/arcsec²). The lower the value, the brighter the object appears per unit area!");
//    //alert("Вказує середню яскравість об'єкта на одиницю площі: яскравість об'єкта, розподілена на одиницю площі на небі (зазвичай вказується у величинах на квадратну дугову секунду, наприклад, mag/arcsec²). Чим менше значення, тим яскравіший об'єкт виглядає на одиниці площі!");
//})
//$(".Hubble_OnlyGalaxies").on("click", function () {
//    alert("Hubble type of the galaxy.");
//})
//$(".Cstar_UMag").on("click", function () {
//    alert("This field contains the brightness (stellar magnitude) of the nearest star in the ultraviolet range (U-band).");
//    //alert("Це поле містить яскравість (зоряна величина) найближчої зорі в ультрафіолетовому діапазоні (U-band).");
//})
//$(".Cstar_BMag").on("click", function () {
//    alert("This field contains the brightness of the nearest star in the blue range (B-band, \"Blue\").");
//    //alert("Це поле містить яскравість найближчої зорі у синьому діапазоні (B-band, \"Blue\").");
//})
//$(".Cstar_VMag").on("click", function () {
//    alert("This field contains the brightness of the nearest star in visible light (V-band, \"Visual\").");
//    //alert("Це поле містить яскравість найближчої зорі у видимому світлі (V-band, \"Visual\").");
//})
//$(".Cstar_Names").on("click", function () {
//    alert("This field contains the names or identifiers of the star specified in the fields: Cstar_UMag, Cstar_BMag, Cstar_VMag.");
//    //alert("Це поле містить імена або ідентифікатори зорі зазначеної в полях: Cstar_UMag, Cstar_BMag, Cstar_VMag.");
//})

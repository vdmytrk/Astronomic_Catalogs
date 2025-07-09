"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || (function () {
    var ownKeys = function(o) {
        ownKeys = Object.getOwnPropertyNames || function (o) {
            var ar = [];
            for (var k in o) if (Object.prototype.hasOwnProperty.call(o, k)) ar[ar.length] = k;
            return ar;
        };
        return ownKeys(o);
    };
    return function (mod) {
        if (mod && mod.__esModule) return mod;
        var result = {};
        if (mod != null) for (var k = ownKeys(mod), i = 0; i < k.length; i++) if (k[i] !== "default") __createBinding(result, mod, k[i]);
        __setModuleDefault(result, mod);
        return result;
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.dataVisualization = dataVisualization;
//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
const d3 = __importStar(require("d3"));
const metrics_1 = require("./metrics");
let rect;
let zeroX;
let width;
let margin;
let spacing; // Простір між планетарними системами.
let endRange;
let planetGroup;
let remInPixels;
let radiusEarth; // Предсталяє діаметер для зменшення розмірів планет.
let preferRight; // Якщо орбіта невизначена або дуже маленька — примусово праворуч
let starRadiusPx;
let maxTickOrder = 4;
let maxPlanetRadius; // У коді ти візуалізував пларенти за їхні радіусом як за діаметром, тож не дивуйся.
let lablePlSysSpace; // Визначає не лише висоту підписів планет, але й висоту зсуву меж систем (setPlSysBorder), щоб утворився верхній відступ для першої системи та текстової сітки (drawGridOverlay)
let leftRightPlanet;
let correctionFactor;
let clearBaseSysHeight;
let topBottomPlSysSpace; // Додає зсув для утворення верхнього та нижнього відступів між планетами та підписами планет.
let correctionFactorForAdditionWidth;
const borderLineColor = "#F5C946";
let planetDescColor = "#E4E4E4";
let allSystems = [];
let allTicks;
let offsets = [];
let axesX;
let svg;
let svgDesc;
let svgWidth;
// Режими вдмальовування технічної графіки:
// 0 - жодна тенічна графіка не відмальовується
// 1 - Підписи планет отримуют фон
// 2 - Промальовується текстова сітка
// 11 - Засвітлення зайнятих планетами клітинок 
// 12 - Засвітлення зайнятих клітинок також виконується функцією markOccupiedCellsAroundLabel
// 20 - Обмеження кількості планет для який працює функція applyGridLabelLayout до к-сті зазначеній поруч з міткою: #Обмеження_к_сті_планетарних_сис
// 30 - Лінія від підписів до планет змінює колір на червоний
const testingLevel = 0; // ← увімкнути тестове промальовування
const allowedPlLetters = ["d"]; // "b", "c", "d", "e", "f", "g", "h", "i" | "Io", "Callisto"
const grid = {
    colWidth: 0,
    rowHeight: 0,
    cols: 0,
    rows: 0,
    occupied: new Set(), // сітка зайнятих клітинок "col,row"
    centralPlanetX: 0
};
function clearData() {
    rect = undefined;
    zeroX = undefined;
    width = undefined;
    margin = undefined;
    spacing = undefined;
    endRange = undefined;
    remInPixels = undefined;
    radiusEarth = undefined;
    starRadiusPx = undefined;
    maxTickOrder = 4;
    maxPlanetRadius = undefined;
    lablePlSysSpace = undefined;
    leftRightPlanet = undefined;
    correctionFactor = undefined;
    clearBaseSysHeight = undefined;
    topBottomPlSysSpace = undefined;
    correctionFactorForAdditionWidth = undefined;
    planetDescColor = "#E4E4E4";
    allSystems = [];
    allTicks = [];
    offsets = [];
    axesX = undefined;
    svg = undefined;
    svgDesc = undefined;
    svgWidth = undefined;
    grid.colWidth = 0;
    grid.rowHeight = 0;
    grid.cols = 0;
    grid.rows = 0;
    grid.occupied.clear();
    grid.centralPlanetX = 0;
}
function gridInitialisation(systemHeight) {
    grid.colWidth = remInPixels * 1;
    grid.rowHeight = remInPixels * 0.5;
    grid.cols = Math.floor(svgWidth / grid.colWidth);
    grid.rows = Math.floor(systemHeight / grid.rowHeight);
}
/// ЯКЩО ПОТРІБНО ЗМІНИТИ ШИРИНУ, ТО Є 4 КІНЦЕВІ ПАРАМЕТРИ:
/// endRange - Визначається у функції setEndRange()
/// currentWidth - Визначається у рядку: const currentWidth = Math.max(rect.width - 7 * remInPixels, 1000);
/// svg.width - Визначається в рядку: .attr("width", width + ...)
/// svgDesc.width - Визначається в рядку: .attr("width", width + ...)
function dataVisualization() {
    return __awaiter(this, void 0, void 0, function* () {
        console.log(" FUNCTION dataVisualization >>>>>>>>>>>>> ");
        clearData();
        rect = yield (0, metrics_1.getElementRect)("#planetsSystemTableContainer");
        remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
        lablePlSysSpace = 3 * remInPixels;
        clearBaseSysHeight = 4 * remInPixels;
        topBottomPlSysSpace = 1 * remInPixels;
        maxPlanetRadius = 11 * remInPixels; // 11 - це максимальний радіус планети, щоб планети не вилазили за шкалу.
        spacing = 2 * remInPixels;
        radiusEarth = remInPixels / 2; // Поділив на два, тому радіус планети візуалізується як діаметр, щоб зменшити розмір планет у візуалізації.
        margin = {
            top: remInPixels,
            right: 3 * remInPixels,
            bottom: remInPixels,
            left: 3 * remInPixels
        };
        const container = document.querySelector('.planetary-systems-visualization'); // Одержання   Б А З О В О Г О   Б Л О К У   В І З У А Л І З А Ц І Ї   в якому міститиметься вся решта візуалізації
        if (!container) {
            console.warn("No container for planetary system background");
            return;
        }
        svg = d3.select(container).select("svg.planetary-axis");
        svgDesc = d3.select(container).select("svg.planetary-axis-description");
        addData();
        calculateSystemsPosition();
        maximumOrder();
        //maxTickOrder = 3; // Т Е Х Н І Ч Н А   В І Д Л А Д К А. Від -2 до 4
        // Визначення базових порядків (лог-шкала)
        // TickOrder(порядок):         -2   -1   0   1   2     3     4
        //  const baseTicks = [0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000];
        //console.log(` 👉 MAXORDER: ${maxTickOrder}`);
        const nonZeroCoefficient = 4;
        // Math.pow(value, 1 / n) 🔔 Примітка: Якщо value від’ємне, а n парне — результат буде NaN, бо парний корінь з від’ємного числа не є дійсним числом.
        // Тому, 
        //   або використовуюй не парні: 3 чи 5 - чомусь не працює!
        //   або додавай умовний коефіцієнт: maxTickOrder + 3, щоб при maxTickOrder: -2 отримати 1.
        correctionFactorForAdditionWidth = Math.pow(maxTickOrder + nonZeroCoefficient, 1 / 4.6); // ⁴√maxTickOrder | ⁴ - це юнікод в діапазоні від U+2070 до U+2079
        // Встановлення ширини базового блоку 
        const orderCount = maxTickOrder + nonZeroCoefficient - 1; // - 1 оскільки -2 повинно дорівнювати 1.
        const minWidth = orderCount * 200;
        width = Math.max(rect.width - 11 * remInPixels * correctionFactorForAdditionWidth, minWidth, 1000);
        // ЗАСТОСУВАННЯ У ФУНКЦІЇ dataVisualization
        //// [1.2, 1.0, 0.6] - початковий варіант
        ////[992, 1146, 1351, 1639, 1874, 2070, 2310, 2609, 2789, 3149, 4227],
        ////[1.20, 1.16, 1.13, 1.10, 1.08, 1.00, 0.95, 0.87, 0.92, 0.80, 0.60],
        ////[0.60, 0.80, 0.87, 0.92, 0.95, 1.00, 1.08, 1.10, 1.13, 1.16, 1.20],
        getCorrectionFactorForWidth(// ⚠️ЗАЛЕЖНІСТЬ №0: Змінюючи тут - змінюй у ВІДЛАДКА КОЕФІЦІЄНТУ
        width, [900, 2070, 4227], [0.35, 1.0, 2.5], // ⚠️ЗАЛЕЖНІСТЬ №1: Зменшуючи тут - збільшуй width в svg.attr("width", ТУТ)!
        1); // Дає дуже невеликий ефект
        renderBackgroundAxis();
        renderSystemCharts();
        logingDataVisualization();
    });
}
;
function addData() {
    const charts = document.querySelectorAll('.planetary-system-data');
    //console.log(`  %%% THE charts ARE FUOUND AND charts.length IS ${charts.length}!!!`);
    allSystems = Array.from(charts).map(chartElement => {
        return JSON.parse(chartElement.getAttribute('data-system') || '{}');
    });
    const showSolarSysCheckbox = document.querySelector("label.showSolarSystem input[type='checkbox']");
    if (showSolarSysCheckbox.checked) {
        allSystems.unshift(solarSystem);
    }
    const showJupiterSystemCheckbox = document.querySelector("label.showJupiterSystem input[type='checkbox']");
    if (showJupiterSystemCheckbox.checked) {
        allSystems.unshift(jupiterSystem);
    }
    const showEarthSystemCheckbox = document.querySelector("label.showEarthSystem input[type='checkbox']");
    if (showEarthSystemCheckbox.checked) {
        allSystems.unshift(earthSystem);
    }
}
function renderBackgroundAxis() {
    console.log("FUNCTION renderBackground");
    //console.log("Width:", rect.width);
    let totalHeight = maxPlanetRadius;
    /////////////////////////////////////////////////////////////////////////
    // Розрахунок висоти ліній та блоку візуалізації:
    //const offsets = calculateSystemsPosition();
    //
    // 📌 Пояснення наступного рядка коду:
    // 🔹 offsets.reduce((sum, o) => sum + o.height, 0)
    // Це метод reduce() масиву offsets.
    //
    // Він обчислює суму висот усіх блоків планетарних систем.
    //   sum — це акумулятор(накопичувач).
    //   o.height — це висота поточного блоку.
    //   0 — початкове значення акумулятора sum.
    //
    // 🔍 Приклад:
    //   offsets = [{ height: 120 }, { height: 90 }, { height: 140 }];
    //   sum = 0
    //   sum = 0 + 120 = 120
    //   sum = 120 + 90 = 210
    //   sum = 210 + 140 = 350
    //
    //Після reduce(...) отримаємо 350
    //const totalHeight = offsets.reduce((sum, o) => sum + o.height, 0) + spacing * (systems.length - 1);
    /////////////////////////////////////////////////////////////////////////
    //Попередній рядок замінив на цей:
    if (offsets.length > 0) {
        totalHeight = offsets[0].height + offsets.at(-1).offsetY + offsets.at(-1).height;
        rect.height = totalHeight; // Оновлення rect.height для подальшого використання
    }
    // ⚠️ЗАЛЕЖНІСТЬ №0: Змінюючи тут - змінюй у ВІДЛАДКА КОЕФІЦІЄНТУ
    // ⚠️ЗАЛЕЖНІСТЬ №1: Зменшуючи "width" - збільшуй перший елемент параметру range у d3.scalePow()
    svg
        .attr("width", width + 10 * remInPixels * correctionFactorForAdditionWidth)
        .attr("height", totalHeight + maxPlanetRadius); // Додав maxPlanetRadius щоб планети не вилазили за шкалу
    // ⚠️ЗАЛЕЖНІСТЬ №0: Змінюючи тут - змінюй у ВІДЛАДКА КОЕФІЦІЄНТУ
    svgDesc
        .attr("width", width + 11 * remInPixels)
        .attr("height", 2.5 * remInPixels);
    // Визначення базових порядків (лог-шкала)
    // TickOrder(порядок):         -2   -1   0   1   2     3     4
    //  const baseTicks = [0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000];
    const minTickOrder = -3;
    const baseTicks = [];
    for (let i = minTickOrder; i <= maxTickOrder; i++) {
        baseTicks.push(Math.pow(10, i));
    }
    //console.log(` 👉 BASE_TICK_ORDER: ${baseTicks}`);
    const tickSet = new Set(baseTicks); // для унікальності
    allTicks = [...baseTicks];
    // Логарифмічна шкала відстані в AU
    const lastOrder = baseTicks.at(-1); // Аналог: baseTicks[baseTicks.length - 1]
    //console.log(` 👉 LAST_ORDER: ${lastOrder}`);
    setEndRange();
    axesX = d3.scaleLog()
        .domain([0.001, lastOrder])
        .range([margin.left, endRange]); // ТУТ ВИЗНАЧАЄШ ЗАЛЕЖНІСТЬ МІЖ ПОРЯДКОМ ТА НЕОБХІДНИМ ЗМЕНШИННЯМ ШИРИНИ!!!
    // Визначення суб-порядків (лог-шкала)
    for (let i = -3; i <= maxTickOrder; i++) {
        const base = Math.pow(10, i);
        for (let m = 1; m <= 9; m++) {
            const tick = base * m;
            if (tick >= 0.001 && tick <= lastOrder && !tickSet.has(tick)) {
                allTicks.push(tick);
            }
        }
    }
    // ТОБІ ПОТРІБНО ПРИ ПОТОЧНІЙ ШИРИНІ svg.attr("width", width + 7 * remInPixels) ЗМУСИТИ d3 МАЛЮВАТИ РОЗМІТКУ ШКАЛИ ТАК НІМБИ МІЦЯ В НЬОГО МЕНШЕ.
    // АБО ЗБІЛЬШИТИ ШИРИНУ БЛОКУ ПІСЯ ВІДМАЛЮВАННЯ ШКАЛИ.
    const axesLableY = 1.3;
    // Н У Л Ь О В А   В І Д М І Т К А
    svg.append("line")
        .attr("x1", 0)
        .attr("y1", 0) // margin.top - для безкоштовного доступу, щоб було менше місця
        .attr("x2", 0)
        .attr("y2", totalHeight - margin.bottom)
        .attr("stroke", "#888")
        .attr("stroke-width", 0.15 * remInPixels);
    svgDesc.append("text")
        .attr("x", 1.5 * remInPixels)
        .attr("y", axesLableY * remInPixels)
        .attr("text-anchor", "middle")
        .attr("class", "logarithmicScale")
        .text("0 AU");
    // Визначення к-сті порядків у шаклі
    let tickCount = 0;
    allTicks.forEach(tick => {
        const isBase = baseTicks.includes(tick);
        if (isBase)
            tickCount++;
    });
    let baseIndex = 0;
    allTicks.sort((a, b) => a - b).forEach(tick => {
        const x = axesX(tick);
        const isBase = baseTicks.includes(tick);
        const orderLength = (axesX(0.01) - axesX(0.001));
        const lastDescCorrectionFactor = Math.min(3 * remInPixels / correctionFactor, 2 * remInPixels); // Визначення зсуву вліво для підпису останнього порядку
        zeroX = orderLength * (1 / 5 + 1 / 6); // Визначення ширини першої шкали від 0 до 0.001.
        let lineWidth = baseIndex == 0 ? 0.02 : 0.05; // Індивідуальне встановлення стилів для позначки 0.001 
        svg.append("line")
            .attr("x1", x + zeroX)
            .attr("y1", 0) // margin.top - для безконтовного доступу, щоб було менше місця
            .attr("x2", x + zeroX)
            .attr("y2", totalHeight - margin.bottom)
            .attr("stroke", "#888")
            .attr("class", isBase ? "BASE_TICK" : "SUB_TICK")
            .attr("stroke-width", isBase ? lineWidth * remInPixels : 0.02 * remInPixels);
        if (isBase) {
            svgDesc.append("text")
                .attr("x", baseIndex + 1 === tickCount ? x + zeroX - lastDescCorrectionFactor : x + zeroX) // Встановлення зсуву вліво для підпису останнього порядку
                .attr("y", axesLableY * remInPixels)
                .attr("text-anchor", "middle")
                .attr("class", "logarithmicScale")
                .text(baseIndex === 0 ? "0.001 AU" : `${formatTick(tick)} AU`); // baseIndex === 0 - щоб визначати чи варто показувати підпис для "0.001"
            baseIndex++;
        }
    });
    //console.log(`     >>>> TICK COUNT: ${tickCount}`);
}
function renderSystemCharts() {
    console.log(" FUNCTION renderAllSystemCharts >>>>>>>>>>>>> ");
    allSystems.forEach((system, index) => {
        if (!offsets[index]) {
            console.error(`No offset data for index ${index}`);
            return;
        }
        // ВСТАНОВИ offsetY = maxPlanetRadius - ЩОБ ЗЛИТИ ВСІ СИСТЕМИ В ОДИН РЯДОК!!!
        const offsetY = offsets[index].offsetY +
            offsets[index].height / 2; // Центрування блоку по Y через додававння висоти поділеної на 2
        const safeOffsetY = Number(offsetY);
        if (isNaN(safeOffsetY)) {
            console.error(`offsetY is NaN!`, { index, offsetY, offsets });
        }
        if (system.exoplanets.length > 1)
            system.exoplanets.sort((a, b) => a.plOrbsmax - b.plOrbsmax);
        renderPlanetChart(svg, system, offsetY, index);
    });
}
function renderPlanetChart(svg, system, offsetY, index) {
    //console.log(`🚀🚀🚀🚀🚀 FUNCTION: renderSystemChart for planetary system: ${system.hostname}`);
    //console.log(`   >>>  offsetY: ${offsetY}`);
    //if (system.hostname === 'Kepler-75899')
    //    console.log(JSON.stringify(system, null, 2));
    var _a;
    planetGroup = undefined;
    planetGroup = svg.append("g")
        .attr("transform", `translate(0, ${offsetY})`)
        .attr("class", "planetGroupSvg");
    const systemHeight = offsets[index].height;
    svgWidth = +svg.attr("width") || ((_a = svg.node()) === null || _a === void 0 ? void 0 : _a.getBoundingClientRect().width) || rect.width || 0;
    gridInitialisation(systemHeight);
    if (testingLevel >= 2) {
        drawGridOverlay(svg, offsetY, systemHeight);
    }
    grid.occupied.clear(); // очищення на кожному кроці
    starRadiusPx = renderStar(system, offsetY, index);
    setPlSysBorder(svg, offsetY, systemHeight);
    renderEndShadowBlock(system, planetGroup, systemHeight);
    renderHabitablZone(system, planetGroup, systemHeight);
    renderPlanets(system, planetGroup, index);
}
function renderStar(system, offsetY, index) {
    const isEJ = ["Earth", "Jupiter"].includes(system.hostname);
    const offsetYStar = offsetY + lablePlSysSpace / 2;
    const hasNotRadius = +system.stRad == 0.00;
    const starGroup = svg.append("g")
        .attr("class", "starGroup")
        .lower(); // Помістити під усі інші елементи
    // Радіус зірки в AU
    const solarRadiusInAU = 0.00465047; // 1 R☉ = 0.00465047 AU
    let starRadiusAU = hasNotRadius ? 0.103 * solarRadiusInAU : system.stRad * solarRadiusInAU; // 0.103 - мін. радіус зірки на 2025 р.
    //console.log(`  📐✔ starRadiusAU: ${starRadiusAU}`); // 14 Her
    // Обчислення радіуса в пікселях по шкалі
    let pixelX;
    if (starRadiusAU >= 0.001)
        pixelX = axesX(starRadiusAU) + zeroX;
    else {
        const pseudoAxesX = createPseudoLogScale([0, 10000], [margin.left, endRange]);
        pixelX = pseudoAxesX(starRadiusAU);
    }
    //console.log(`  📐✔✔ pixelX: ${pixelX} = axesX(starRadiusAU)(${axesX(starRadiusAU)}) + zeroX(${zeroX})`);
    const starRadiusPx = Math.abs(pixelX); // Відстань від 0.001 до радіуса зірки // - pixelX
    //console.log(`  📐✔✔✔ starRadiusPx: ${starRadiusPx}`);
    /// 🔲 ЗОРЯНА КОРОНА
    // Градієнт для зоряної корони
    const gradientId = `starCoronaGradient-${system.hostname.replace(/\W/g, "")}`;
    const defs = svg.select("defs").empty() ? svg.insert("defs", ":first-child") : svg.select("defs");
    const stellarCoronaColor = system.stTeff < 100 ? "rgba(148, 148, 148, 1)" : temperatureToColor(system.stTeff); // "rgba(169, 169, 169, 1)"
    if (!isEJ) {
        const gradient = defs.append("radialGradient")
            .attr("id", gradientId)
            .attr("cx", "50%")
            .attr("cy", "50%")
            .attr("r", "50%")
            .attr("fx", "50%")
            .attr("fy", "50%");
        gradient.append("stop")
            .attr("offset", "0%")
            .attr("stop-color", "white")
            .attr("stop-opacity", 1);
        gradient.append("stop")
            .attr("offset", "20%")
            .attr("stop-color", "white")
            .attr("stop-opacity", 0.8);
        gradient.append("stop")
            .attr("offset", "24%") // ⚠️ЗАЛЕЖНІСТЬ №5 - Оскільки розмір корони залежить від радіусу зірки, то радіус зірки має стабільний відсоток у R_корони!
            .attr("stop-color", stellarCoronaColor)
            .attr("stop-opacity", 0.8);
        gradient.append("stop")
            .attr("offset", "100%")
            .attr("stop-color", stellarCoronaColor)
            .attr("stop-opacity", 0);
    }
    // Радіус корони
    const coronaRadiusPx = starRadiusPx * 3; // ⚠️ЗАЛЕЖНІСТЬ №5
    // Зоряна корона
    starGroup.append("circle")
        .attr("cx", 0)
        .attr("cy", offsetYStar)
        .attr("r", coronaRadiusPx)
        .attr("fill", `url(#${gradientId})`);
    /// 🎯 ЗІРКА
    // Колір тіла зірки
    const fadeColorStr = d3.color(stellarCoronaColor);
    fadeColorStr.opacity = 0.9;
    const starFill = isEJ ? "rgba(255, 0, 0, 0.35)" : fadeColorStr.toString();
    // Малюємо зірку
    starGroup.append("circle")
        .attr("cx", 0)
        .attr("cy", offsetYStar)
        .attr("r", starRadiusPx)
        .attr("fill", starFill);
    // Пунктирне коло навколо зірки
    if (hasNotRadius) {
        starGroup.append("circle")
            .attr("cx", 0)
            .attr("cy", offsetYStar)
            .attr("r", starRadiusPx)
            .attr("fill", "none")
            .attr("stroke", "rgba(10, 10, 10, 0.5")
            .attr("stroke-width", remInPixels * 0.1)
            .attr("stroke-dasharray", `${remInPixels * 0.4}, ${remInPixels * 0.2}`) // шаблон пунктиру
            .attr("pointer-events", "none"); // не перешкоджає взаємодії
    }
    // ✅ Обмеження видимості зірки та корони межами системи
    const clipId = createStarClipPath(system.hostname, offsetY, offsets[index].height);
    starGroup.attr("clip-path", `url(#${clipId})`);
    /// 📄 Опис системи
    // Колір підписів
    planetDescColor = "#E4E4E4";
    const sysHeaderFill = isEJ ? planetDescColor : "#D01818";
    const sysDescFill = /*hasNotRadius ? "rgba(228, 228, 228, 1)" :*/ "#F61C1C";
    // Назва системи
    svg.append("text")
        .attr("x", remInPixels)
        .attr("y", -remInPixels)
        .attr("transform", `translate(0, ${offsetY})`)
        .text(system.hostname)
        .attr("font-size", "1.3rem")
        .style("font-weight", "bold")
        .attr("fill", sysHeaderFill);
    // Додати підпис з характеристиками зірки
    const whiteSysDescFill = system.stTeff >= 300 && system.stTeff <= 1000 ? "white" : sysDescFill;
    const x = remInPixels;
    const labelSys = svg.append("g")
        .attr("class", "systemLabelDesc")
        .attr("transform", `translate(${x}, ${offsetY + remInPixels * 0.2})`)
        .attr("fill", whiteSysDescFill);
    if (!isEJ) {
        const formatT = (value) => {
            if (value === "0.00" || value === 0 || value === 0.0) {
                return "?";
            }
            return value.toFixed(0);
        };
        const stTeff = formatT(+system.stTeff);
        const format = (v) => (v.toFixed(2) === "0.00" || v.toFixed(2) === "0" ? "?" : v.toFixed(2));
        const stRad = format(+system.stRad);
        const stMass = format(+system.stMass);
        const formatA = (value) => {
            if (value === "0.00" || value === 0 || value === 0.0) {
                return "?";
            }
            let result;
            if (value <= 1)
                result = `${value * 1000} Myr`;
            else
                result = `${value} Gyr`;
            return result;
        };
        const stAge = formatA(+system.stAge);
        let stLum;
        const formatStellarLuminosity = (value) => {
            if (value === "0.00" || value === 0 || value === 0.0) {
                return "?";
            }
            // Пробуємо перетворити на число
            const num = typeof value === "number"
                ? value
                : typeof value === "string"
                    ? parseFloat(value)
                    : NaN;
            // Якщо число валідне, повертаємо 10^num
            let result;
            if (!isNaN(num)) {
                const lum = Math.pow(10, num);
                if (lum <= 0.0001)
                    result = lum.toFixed(6);
                else if (lum <= 0.001)
                    result = lum.toFixed(5);
                else if (lum <= 0.01)
                    result = lum.toFixed(4);
                else if (lum <= 0.1)
                    result = lum.toFixed(3);
                else
                    result = lum.toFixed(2);
            }
            else
                result = "value";
            return result;
        };
        if (["Sun"].includes(system.hostname))
            stLum = "1"; // Для Сонця
        else
            stLum = formatStellarLuminosity(system.stLum); // ТЕСТ: Barnard's star
        const stRadEarth = hasNotRadius ? "?" : (system.stRad * 109.076).toFixed(2);
        const stRadJup = hasNotRadius ? "?" : (system.stRad * 9.731).toFixed(2);
        const stMassEarth = system.stMass != 0.00 ? (system.stMass * 332946).toFixed(2) : "?";
        const stMassJup = system.stMass != 0.00 ? (system.stMass * 1047.57).toFixed(2) : "?";
        labelSys.append("text").text(`${stLum} L☉ | ${stTeff} K`).attr("y", remInPixels * 0.0);
        labelSys.append("text").text(`${stRad} R☉`).attr("y", remInPixels * 1.2); // ${stRadEarth} R⊕ | ${stRadJup} R♃ | 
        labelSys.append("text").text(`${stMass} M☉`).attr("y", remInPixels * 2.4); // ${stMassEarth} M⊕ | ${stMassJup} M♃ | 
        labelSys.append("text").text(`Age: ${stAge}`).attr("y", remInPixels * 3.6);
    }
    return starRadiusPx;
}
//===================================================================================================================
/// ФУНКЦІЇ РОЗРАХУНКІВ РОЗМІРУ МАТЕРИНСЬКИХ ТІЛ СИСТЕМ З РАДІУСОМ МЕНШЕ 0.0001 АО.
//
// ПІДХІД №1: Використати логарифмічну шкалу з "зсувом" (pseudolog або log1p).
// Ідея: застосовувати не log(x), а log(x + ε), де ε = 0.0005 або подібне.
//    Переваги:
//      * Зберігає логарифмічну природу.
//      * Уникає проблеми з log(0).
//    Недоліки:
//      * Не підтримується d3.scaleLog напряму — потрібно створити кастомну шкалу.
//      * Потрібен особливий підхід до позначок осі(ticks).
function createPseudoLogScale(domain, range, epsilon = 0.0005) {
    const [minDomain, maxDomain] = domain;
    const [minRange, maxRange] = range;
    const logMin = Math.log(minDomain + epsilon);
    const logMax = Math.log(maxDomain + epsilon);
    const scale = d3.scaleLinear()
        .domain([logMin, logMax])
        .range([minRange, maxRange]);
    return (x) => {
        const adjusted = Math.log(x + epsilon);
        return scale(adjusted);
    };
}
//===================================================================================================================
// Обмеження видимості тіла зірки
function createStarClipPath(hostname, offsetY, systemHeight) {
    const clipId = `starClip-${hostname.replace(/\W/g, "")}`;
    const defs = svg.select("defs").empty()
        ? svg.insert("defs", ":first-child")
        : svg.select("defs");
    // Межі системи по Y:
    const topY = offsetY - systemHeight / 2 + lablePlSysSpace / 2;
    const bottomY = offsetY + systemHeight / 2 + lablePlSysSpace / 2;
    // Створення clipPath
    defs.append("clipPath")
        .attr("id", clipId)
        .append("rect")
        .attr("x", 0) // нульова відмітка — межа по X
        .attr("y", topY)
        .attr("width", svgWidth) // повна ширина, але зліва обрізка по x = 0
        .attr("height", bottomY - topY);
    return clipId;
}
// Вибілення кольору із вказанням фактору factor який означає: 0.4 - зробити на 40% білішим.
function lightenColor(r, g, b, factor) {
    const newR = Math.round(r + (255 - r) * factor);
    const newG = Math.round(g + (255 - g) * factor);
    const newB = Math.round(b + (255 - b) * factor);
    return `rgba(${newR}, ${newG}, ${newB}, 1)`;
}
function temperatureToColor(tempK) {
    // Обмеження температури в межах 300 - 50000 K
    tempK = Math.max(300, Math.min(tempK, 50000));
    // Для нормалізованої температури (в логарифмічній шкалі)
    const t = tempK / 100;
    let r, g, b;
    // Основний діапазон — класичний колір тіла Чорного
    if (tempK >= 600) {
        // Red
        r = t <= 66
            ? 255
            : 329.698727446 * Math.pow(t - 60, -0.1332047592);
        r = Math.min(Math.max(r, 0), 255);
        // Green
        g = t <= 66
            ? 99.4708025861 * Math.log(t) - 161.1195681661
            : 288.1221695283 * Math.pow(t - 60, -0.0755148492);
        g = Math.min(Math.max(g, 0), 255);
        // Blue
        b = t >= 66
            ? 255
            : t <= 19
                ? 0
                : 138.5177312231 * Math.log(t - 10) - 305.0447927307;
        b = Math.min(Math.max(b, 0), 255);
    }
    else {
        // Додатковий діапазон 300–600K — інтерполяція до чорного
        // Вага, що показує наскільки ми близько до 600K
        const factor = (tempK - 300) / (600 - 300); // від 0 до 1
        // Базовий колір для 600K (червонувато-коричневий)
        const baseR = 255;
        const baseG = 56;
        const baseB = 0;
        r = baseR * factor;
        g = baseG * factor;
        b = baseB * factor;
    }
    // Перетворення в hex
    const toHex = (c) => Math.round(c).toString(16).padStart(2, '0');
    return `#${toHex(r)}${toHex(g)}${toHex(b)}`;
}
function renderHabitablZone(system, planetGroup, systemHeight) {
    const borderThickness = 0.1 * remInPixels; // Якщо видалиш і будеш використовувати однойменну глобальну змінну - перестане працюваати
    if (system.habitablZone && (system.stLum != 0.00 || system.hostname == "Sun")) {
        const hzCenter = +system.habitablZone;
        // Межі HZ на основі пропорцій для Сонця
        const sunMin = 0.75;
        const sunMax = 1.77;
        const sunCenter = 1.26;
        const hzMin = hzCenter * (sunMin * 0.75);
        const hzMax = hzCenter * (sunMax * 1.25);
        const hzX1 = axesX(hzMin) + zeroX;
        const hzX2 = axesX(hzMax) + zeroX;
        // Додаємо defs з градієнтом лише один раз:
        let defs = svg.select("defs");
        if (defs.empty()) {
            defs = svg.append("defs");
        }
        const gradientId = `habitable-gradient-${system.hostname.replace(/\W/g, '')}`;
        const gradient = defs.append("linearGradient")
            .attr("id", gradientId)
            .attr("x1", "0%")
            .attr("x2", "100%")
            .attr("y1", "0%")
            .attr("y2", "0%");
        const logHzMin = Math.log10(hzMin);
        const logHzMax = Math.log10(hzMax);
        const logHzCenter = Math.log10(hzCenter);
        //const leftPercent = ((logHzCenter - logHzMin) / (logHzMax - logHzMin)) * 100;
        //const rightPercent = ((logHzMax - logHzCenter) / (logHzMax - logHzMin)) * 100;
        // Це відносна позиція центру в діапазоні [hzMin..hzMax] у відсотках (від 0 до 100)
        const centerPercent = ((logHzCenter - logHzMin) / (logHzMax - logHzMin)) * 100;
        //console.log(` ⚠️  ⚠️  ⚠️  system.habitablZone = ${system.habitablZone}`);
        //console.log(` ⚠️  ⚠️  ⚠️  centerPercent = ${centerPercent}`);
        gradient.append("stop").attr("offset", "0%").attr("stop-color", "rgba(0, 255, 25, 0"); //.attr("stop-color", "rgba(0, 255, 25, 100");
        gradient.append("stop").attr("offset", `${centerPercent - 35}%`).attr("stop-color", "rgba(0, 177, 25, 0.1)");
        gradient.append("stop").attr("offset", `${centerPercent - 20}%`).attr("stop-color", "rgba(0, 177, 25, 0.8)");
        gradient.append("stop").attr("offset", `${centerPercent}%`).attr("stop-color", "rgba(0, 255, 25, 0.67)");
        gradient.append("stop").attr("offset", `${centerPercent + 20}%`).attr("stop-color", "rgba(0, 177, 25, 0.8)");
        gradient.append("stop").attr("offset", `${centerPercent + 40}%`).attr("stop-color", "rgba(0, 177, 25, 0.1)");
        gradient.append("stop").attr("offset", "100%").attr("stop-color", "rgba(0, 255, 25, 0"); //.attr("stop-color", "rgba(0, 255, 25, 100");
        planetGroup.append("rect")
            .attr("x", hzX1)
            .attr("y", -systemHeight / 2 + lablePlSysSpace / 2 + borderThickness / 2)
            .attr("width", hzX2 - hzX1)
            .attr("height", systemHeight - borderThickness)
            .attr("fill", `url(#${gradientId})`);
    }
}
let count = 0;
function renderPlanets(system, planetGroup, index) {
    const jupiterRadiusHalf = 0.5;
    const jupiterMassHalf = 0.5;
    if (Array.isArray(system.exoplanets)) {
        for (const planet of system.exoplanets) {
            const rawDistance = +planet.plOrbsmax;
            const distance = (rawDistance > 0.001 && !isNaN(rawDistance)) ? rawDistance : 0.001; // 0.001 AU якщо не зазначено
            const currentRadius = (planet.plRade || 1) * radiusEarth;
            const radius = Math.min(currentRadius, maxPlanetRadius);
            const planetName = planet.plLetter || "?";
            const hasRadius = !!planet.plRade;
            const x = axesX(distance) + zeroX;
            const y = 0;
            // Планета
            planetGroup.append("circle")
                .attr("cx", x)
                .attr("cy", y)
                .attr("r", radius)
                .attr("fill", hasRadius ? "rgba(255, 0, 0, 0.35)" : "rgba(94, 94, 94)"); // "rgba(169, 169, 169, 0.5)");
            occupyGridForPlanetDisk(x, y, radius, planetGroup, system, planet);
            // Опис до планети
            const labelY = radius + remInPixels;
            const label = planetGroup.append("g")
                .attr("class", "planet-label-group")
                .attr("transform", `translate(${x}, ${labelY})`)
                .attr("text-anchor", "middle") // ➕ ЦЕНТРУЄ текст
                .attr("alignment-baseline", "middle") // ➕ ЦЕНТРУЄ текст
                .attr("checked", 0)
                .attr("data-xp", x)
                .attr("data-yl", labelY) // Зберігаємо абсолютні координати.
                .attr("font-size", "1rem")
                .attr("fill", "#E4E4E4"); // Зміни кольору в залежності від позиціонування реалізовано в applyGridLabelLayout.
            // Підпис (буква або "?")
            label.append("text")
                .text(`${planetName}`)
                .attr("y", 0)
                .raise(); // НЕПРАЦЮЄ! Щоб реалізувати - потрібно відмальовувати підписи планет окремим циклом післі відмальовування планет!
            // Характеристики планет
            const format = (v) => (v.toFixed(2) === "0.00" ? "?" : v.toFixed(2));
            const rade = format(planet.plRade);
            const radj = format(planet.plRadJ);
            const masse = format(planet.plMasse);
            const massj = format(planet.plMassJ);
            // якщо радіус і маса менше половини Юпітера
            if ((typeof planet.plRadJ === "number" && planet.plRadJ < jupiterRadiusHalf) &&
                (typeof planet.plMassJ === "number" && planet.plMassJ < jupiterMassHalf)) {
                label.append("text").text(`${rade} R⊕`).attr("y", remInPixels);
                label.append("text").text(`${masse} M⊕`).attr("y", remInPixels * 2);
            }
            else {
                label.append("text").text(`${rade} R⊕ | ${radj} R♃`).attr("y", remInPixels);
                label.append("text").text(`${masse} M⊕ | ${massj} M♃`).attr("y", remInPixels * 2);
            }
            // Додатковий підпис поверх планети, якщо немає радіуса
            if (!hasRadius) {
                planetGroup.append("text")
                    .attr("x", x)
                    .attr("y", y + remInPixels * 0.15) // трохи нижче, щоб візуально по центру круга applyGridLabelLayout()
                    .attr("text-anchor", "middle")
                    .attr("alignment-baseline", "middle")
                    .attr("class", "planetMissingRadius")
                    .text("?");
            }
        }
    }
    else {
        console.warn("exoplanets is not an array:", system.exoplanets);
    }
    if (testingLevel >= 20) {
        if (count < 4) { // #Обмеження_к_сті_планетарних_сис
            applyGridLabelLayout(planetGroup, system, index);
            count++;
        }
    }
    else
        applyGridLabelLayout(planetGroup, system, index);
}
function occupyGridForPlanetDisk(cx, cy, r, planetGroup, system, planet) {
    //console.log(`FUNCTION occupyGridForPlanetDisk FOR SYSTEM: ${system.hostname} FOR PLANET: ${planet.plLetter}`);
    const minX = cx - r;
    const maxX = cx + r;
    const minY = cy - r;
    const maxY = cy + r;
    const startCol = Math.floor(minX / grid.colWidth);
    const endCol = Math.ceil(maxX / grid.colWidth);
    const startRow = Math.floor(minY / grid.rowHeight);
    const endRow = Math.ceil(maxY / grid.rowHeight);
    let markedAny = false; // Чи позначено хоч одну клітинку
    for (let col = startCol; col <= endCol; col++) {
        for (let row = startRow; row <= endRow; row++) {
            // Визначення геометричного центру кожної клітинки в пікселях, щоб перевірити, чи вона попадає всередину кола
            const cellX = col * grid.colWidth + grid.colWidth / 2;
            const cellY = row * grid.rowHeight + grid.rowHeight / 2;
            // 	Відстань від центру планети (cx, cy) до центру клітинки
            const dx = cellX - cx;
            const dy = cellY - cy;
            const dist = Math.sqrt(dx * dx + dy * dy); // dist = √(dx² + dy²) → Стандартна формула для відстані між двома точками: від центру клітинки до центру планети.
            // Якщо dist ≤ r → центр клітинки знаходиться всередині кола радіусом r
            if (dist <= r) {
                grid.occupied.add(`${col},${row}`);
                markedAny = true;
                if (testingLevel >= 11)
                    // 🔲 Візуалізація зайнятої клітинки
                    planetGroup
                        .append("rect")
                        .attr("x", col * grid.colWidth)
                        .attr("y", row * grid.rowHeight)
                        .attr("width", grid.colWidth)
                        .attr("height", grid.rowHeight)
                        .attr("fill", "red")
                        .attr("opacity", 0.5);
            }
        }
    }
    // Якщо не було позначено жодної клітинки — позначаємо найближчі верхню і нижню
    if (!markedAny) {
        const centerCol = Math.floor(cx / grid.colWidth);
        const centerRow = Math.floor(cy / grid.rowHeight);
        const rowsToMark = [centerRow - 1, centerRow];
        rowsToMark.forEach(row => {
            const key = `${centerCol},${row}`;
            grid.occupied.add(key);
            if (testingLevel >= 11) {
                planetGroup
                    .append("rect")
                    .attr("x", centerCol * grid.colWidth)
                    .attr("y", row * grid.rowHeight)
                    .attr("width", grid.colWidth)
                    .attr("height", grid.rowHeight)
                    .attr("fill", "orange")
                    .attr("opacity", 0.5);
            }
        });
    }
}
function setPlSysBorder(planetGroup, offsetY, systemHeight) {
    var _a;
    const borderThickness = 0.1 * remInPixels;
    svgWidth = +svg.attr("width") || ((_a = svg.node()) === null || _a === void 0 ? void 0 : _a.getBoundingClientRect().width) || rect.width || 0;
    // Верхня межа
    planetGroup.append("line")
        .attr("x1", 0)
        .attr("y1", offsetY - systemHeight / 2 + lablePlSysSpace / 2)
        .attr("x2", svgWidth)
        .attr("y2", offsetY - systemHeight / 2 + lablePlSysSpace / 2)
        .attr("stroke", borderLineColor)
        .attr("stroke-width", borderThickness)
        .lower(); // Відправляє цей елемент під усе, що вже є
    if (testingLevel >= 1)
        // Центр системи по Y
        planetGroup.append("line")
            .attr("x1", 0)
            .attr("y1", offsetY)
            .attr("x2", svgWidth)
            .attr("y2", offsetY)
            .attr("stroke", "white")
            .attr("stroke-width", 0.5)
            .lower();
    // Нижня межа
    planetGroup.append("line")
        .attr("x1", 0)
        .attr("y1", offsetY + systemHeight / 2 + lablePlSysSpace / 2)
        .attr("x2", svgWidth)
        .attr("y2", offsetY + systemHeight / 2 + lablePlSysSpace / 2)
        .attr("stroke", borderLineColor)
        .attr("stroke-width", borderThickness)
        .lower();
}
function renderEndShadowBlock(system, planetGroup, systemHeight) {
    let shadowStartX = endRange + 1 * remInPixels;
    const borderThickness = 0.2 * remInPixels; // Якщо видалиш і будеш використовувати однойменну глобальну змінну - перестане працюваати 
    // Додаємо defs з градієнтом лише один раз:
    let defs = svg.select("defs");
    if (defs.empty()) {
        defs = svg.append("defs");
    }
    const gradientId = `end-shadow-gradient-${system.hostname.replace(/\W/g, '')}`;
    const gradient = defs.append("linearGradient")
        .attr("id", gradientId)
        //.attr("gradientUnits", "userSpaceOnUse") // ← ВАЖЛИВО! ⚠️
        // 📝 Пояснення
        // |----------------------------------------------------------|-------------------------------------------|
        // | Властивість	                                          | Що робить                                 |
        // |----------------------------------------------------------|-------------------------------------------|
        // | gradientUnits = "objectBoundingBox"(за замовчуванням)	  | координати відносно об’єкта(<rect>)       |
        // | gradientUnits = "userSpaceOnUse"	                      | координати у системі координат всього SVG |
        // |----------------------------------------------------------|-------------------------------------------|
        .attr("x1", "0%").attr("x2", "100%")
        .attr("y1", "0%").attr("y2", "0%");
    gradient.append("stop").attr("offset", "0%").attr("stop-color", "rgba(18,18,18,   0)");
    gradient.append("stop").attr("offset", "20%").attr("stop-color", "rgba(18,18,18,  0.3)");
    gradient.append("stop").attr("offset", "100%").attr("stop-color", "rgba(18,18,18, 1)");
    planetGroup.append("rect")
        .attr("id", "end-shadow-rect") // ← ОБОВ’ЯЗКОВО для функції updateShadowBlockHeight
        .attr("x", shadowStartX + zeroX / 2)
        .attr("y", -systemHeight / 2 + lablePlSysSpace / 2 - borderThickness)
        .attr("width", svgWidth - shadowStartX - zeroX / 2 + borderThickness)
        .attr("height", systemHeight + borderThickness * 2)
        .attr("fill", `url(#${gradientId})`);
}
function maximumOrder() {
    const logTicks = [0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000];
    let maxPlanetDistance = 0;
    let countOfSystem = 0; // технічна змінна
    let maxPlanetDistanceInSystem = 0; // технічна змінна
    allSystems.forEach(system => {
        countOfSystem++;
        maxPlanetDistanceInSystem = 0;
        if (Array.isArray(system.exoplanets)) {
            for (const planet of system.exoplanets) {
                maxPlanetDistance = Math.max(maxPlanetDistance, planet.plOrbsmax);
                maxPlanetDistanceInSystem = Math.max(maxPlanetDistanceInSystem, planet.plOrbsmax);
                //console.log(`The planet ${planet.plLetter} have orbit: ${planet.plOrbsmax}`);
            }
        }
        //console.log(` >>>> In planet system ${system.hostname} № ${countOfSystem} the distance to the outmost remout planet is:`);
        //console.log(`${maxPlanetDistanceInSystem}.`);
        //console.log(` >>>> So in this sysetm order is: ${Math.log10(logTicks.find(tick => tick >= maxPlanetDistanceInSystem))}`);
    });
    const roundedUpTick = logTicks.find(tick => tick >= maxPlanetDistance);
    const checkbox = document.querySelector("label.dynamicScalingAxis input[type='checkbox']");
    maxTickOrder = checkbox.checked ? Math.log10(roundedUpTick) : 4;
    //console.log(` >>>> So MAX order is: ${maxTickOrder}`);
}
// Функція корекції ширини блоку візуалізації в залежності від ширини екрану
function setEndRange() {
    // Додай коефіцієнт масштабування ширини екрану за принципом:  Менша ширина - збільшуй width
    switch (maxTickOrder) {
        case -2:
            endRange = width - margin.right - 32 * remInPixels * correctionFactor;
            break;
        case -1:
            endRange = width - margin.right - 17 * remInPixels * correctionFactor;
            break;
        case 0:
            endRange = width - margin.right - 11 * remInPixels * correctionFactor;
            break;
        case 1:
            endRange = width - margin.right - 8 * remInPixels * correctionFactor;
            break;
        case 2:
            endRange = width - margin.right - 6 * remInPixels * correctionFactor;
            break;
        case 3:
            endRange = width - margin.right - 4 * remInPixels * correctionFactor;
            break;
        case 4:
            endRange = width - margin.right - 2 * remInPixels * correctionFactor; // Можна було б поставити 3, але це і так вже 7 порядків і планети дуже близько.
            break;
    }
}
/// О П И С
/// Повертає масив Y-зсуву та висоти для кожного блоку планетраних систем:
/// const offsets = [
///     { "offsetY": 0,        "height": 268.5167 },
///     { "offsetY": 297.3167, "height": 297.3167 },
///     { "offsetY": 487.5263, "height": 232.4304 },
///     { "offsetY": 748.7568, "height": 211.4496 },
///     ...
/// ];
function calculateSystemsPosition() {
    let cumulativeY = 0;
    const maxSysHeight = maxPlanetRadius * 2 + lablePlSysSpace + topBottomPlSysSpace;
    for (const system of allSystems) {
        let maxPlanetRadiusInSystem = 0;
        let baseSysHeight = clearBaseSysHeight + lablePlSysSpace + topBottomPlSysSpace; // Визначаєш базову (мінімальну) висоту системи
        if (Array.isArray(system.exoplanets)) {
            for (const planet of system.exoplanets) {
                const plRadius = (planet.plRade || 1) * remInPixels;
                const sysHeight = plRadius + lablePlSysSpace + topBottomPlSysSpace;
                if (plRadius > maxPlanetRadiusInSystem)
                    maxPlanetRadiusInSystem = plRadius;
                if (sysHeight > baseSysHeight) {
                    baseSysHeight = sysHeight;
                }
                if (baseSysHeight > maxSysHeight)
                    baseSysHeight = maxSysHeight;
                //console.log(`📐 New maximum height of all planetary system is → ${height}px`);
                //console.log(`📐 Current maximum height of all planetary system ${system.hostname} is → ${height}px`);
            }
        }
        offsets.push({
            offsetY: cumulativeY,
            height: baseSysHeight,
            maxPlanetRadiusInSystem: Math.min(maxPlanetRadiusInSystem, maxPlanetRadius),
        });
        //console.log(`📌 Parameters of planetary system ${system.hostname} is →`);
        //console.log(`  📐 System offset: ${cumulativeY}px; height: ${baseSysHeight}px`);
        cumulativeY += baseSysHeight + spacing;
    }
    //console.log(">>>>> offsets (JSON):", JSON.stringify(offsets, null, 2));    
}
function formatTick(tick) {
    if (tick < 1) {
        return tick.toString(); // або tick.toPrecision(3)
    }
    return d3.format(",.0f")(tick).replace(/,/g, ' ');
}
/// П О Я С Н Е Н Н Я
/// 📈 Як працює інтерполяція
///   Наприклад:
///     const correctionFactor = correctionScale(1500);
///     1500 — між 992 і 2070. D3 сам вирахує, що це приблизно 40 % шляху від 992 до 2070:
///     2070 − 992 = 1078 → повний інтервал
///     1500 − 992 = 508 → скільки пройдено
///     508 / 1078 ≈ 0.47 → прогрес між першими двома domain'ами
///
///   І аналогічно застосує це до range:
///     0.6 + 0.47 × (1.0 − 0.6) = 0.6 + 0.188 ≈ 0.788
///
/// ✅ Що повертає correctionScale(screenWidth)
///   Воно повертає:
///     📏 0.6, якщо ширина екрана ≤ 992
///     📏 1.0, якщо екран = 2070
///     📏 1.2, якщо екран ≥ 4227
///     📏 🔄 Проміжне значення, якщо ширина екрана між цими числами
///
/// 🧮 Як ти його використовуєш
///   Потім ти множиш ширину елемента на цей коефіцієнт:
///     const correctedWidth = rect.width * correctionFactor;
///
///   Це означає:
///     На маленьких екранах(992px) — зменшити ширину svg(× 0.6)
///     На середніх екранах(2070px) — нічого не змінювати(× 1.0)
///     На великих екранах(4227px) — трохи збільшити(× 1.2)
///
/// В А Ж Л И В О
/// В domain та range, функцій scalePow та scaleLiner у d3, можна передавати декілька аргументів:
///   scale.domain([x0, x1, x2, ..., xn])
///       .range([y0, y1, y2, ..., yn])
///
/// 🔧 Приклад(для scaleLinear)
///       const scale = d3.scaleLinear()
///           .domain([0, 100, 200, 400])
///           .range([0, 50, 60, 100]);
///
///       console.log(scale(50));   // 25
///       console.log(scale(150));  // 55 (між 100 і 200: від 50 до 60)
///       console.log(scale(300));  // 80 (між 200 і 400: від 60 до 100)
///
/// 🧪 Приклад(для scalePow)
///    const powScale = d3.scalePow()
///        .exponent(0.6)
///        .domain([100, 1000, 5000, 10000])
///        .range([0.5, 1.0, 1.1, 1.2]);
///
///    console.log(powScale(700));   // масштабування між 100–1000
///    console.log(powScale(3000));  // між 1000–5000
///
/// ⚠️ Умови:
///      * Кількість елементів у.domain() і.range() має бути однакова.
///      * Значення в domain повинні йти у зростаючому порядку(або спадному, якщо хочеш інверсію);
///      * Значення в range інтерполюються відповідно до відповідного проміжку.
function getCorrectionFactorForWidth(input, domain, range, exponent) {
    // 1. Масштаб помилки (емпіричний, на основі твоїх спостережень)
    // d3.scaleLinear() - При використанні цієї функції не використовується .exponent()
    // d3.scalePow()    - Але scalePow().exponent(0.6) — це найгнучкіший варіант, легко "гнути" криву.
    const correctionScale = d3.scaleLinear()
        //.exponent(exponent)               // або 0.5, 0.7 — підбирай експериментально - ДАЄ ДУЖЕ НЕВЕЛИКЕ ЗМІЩЕННЯ
        .domain(domain) // взяв три ключові точки. Можливо: [x0, x1, x2, ..., xn]
        .range(range); // тобто 60% (недостатньо), 100% (норм), 120% (надлишок). Можливо: [y0, y1, y2, ..., yn]
    // 2. Поправка
    correctionFactor = correctionScale(input);
}
function logingDataVisualization() {
    //console.log(" 📐 📐 📐 ");
    //console.log(` 👉 MAXORDER: ${maxTickOrder}`);
    //console.log("width used for scale =", width);
    //console.log("scale domain =", axesX.domain());
    //console.log("scale range =", axesX.range());
    //console.log(" 📐 📐 📐");
    //console.log(`  remInPixels: ${remInPixels}px`);
    //console.log(`  Current width: ${rect.width}px`);
    //console.log(`  Correction factor: ${correctionFactor}`);
    //console.log(`  Total correction factor: ${correctionFactorForAdditionWidth}`);
    //console.log(`  CorrectionFactorForAdditionWidth = ${correctionFactorForAdditionWidth}`);
    //console.log(`  Final svg width: ${width + 8 * remInPixels * correctionFactorForAdditionWidth}px`);
    //console.log(`  Final svgDesc width: ${width + 11 * remInPixels}px`);
    // ВІДЛАДКА КОЕФІЦІЄНТУ КОРЕКЦІЇ ШИРИНИ:
    //const myArray = [1000, 1500, 2000, 2500, 3000, 3500, 4000];
    //for (let i = 0; i < myArray.length; i++) {
    //    getCorrectionFactorForWidth(
    //        myArray[i],
    //        [900, 2070, 4227],
    //        [0.35, 1.0, 2.5],
    //        1);
    //    //console.log(`🎯 👉  Correction factor for ${myArray[i]}: ${correctionFactor}`);
    //}
}
const solarSystem = {
    hostname: "Sun",
    hdName: "",
    hipName: "",
    ticId: "",
    gaiaId: "",
    stSpectype: "G2V",
    stTeff: 5778,
    stRad: 1.0,
    stMass: 1.0,
    stMet: 0.0122,
    stMetratio: "[Fe/H]",
    stLum: 0,
    stAge: 4.6,
    syDist: 0,
    stLumSunAbsol: 1.0,
    habitablZone: 1.0,
    exoplanets: [
        { id: 1, hostname: "Sun", plLetter: "Mercury", plRade: 0.3829, plRadJ: 0.0346, plMasse: 0.0553, plMassJ: 0.00017, plOrbsmax: 0.387 },
        { id: 2, hostname: "Sun", plLetter: "Venus", plRade: 0.9499, plRadJ: 0.0858, plMasse: 0.815, plMassJ: 0.00256, plOrbsmax: 0.723 },
        { id: 3, hostname: "Sun", plLetter: "Earth", plRade: 1.0, plRadJ: 0.0892, plMasse: 1.0, plMassJ: 0.00315, plOrbsmax: 1.0 },
        { id: 4, hostname: "Sun", plLetter: "Mars", plRade: 0.5320, plRadJ: 0.0474, plMasse: 0.107, plMassJ: 0.00034, plOrbsmax: 1.524 },
        { id: 5, hostname: "Sun", plLetter: "Jupiter", plRade: 11.209, plRadJ: 1.0, plMasse: 317.8, plMassJ: 1.0, plOrbsmax: 5.204 },
        { id: 6, hostname: "Sun", plLetter: "Saturn", plRade: 9.449, plRadJ: 0.843, plMasse: 95.16, plMassJ: 0.299, plOrbsmax: 9.582 },
        { id: 7, hostname: "Sun", plLetter: "Uranus", plRade: 4.007, plRadJ: 0.358, plMasse: 14.54, plMassJ: 0.046, plOrbsmax: 19.2 },
        { id: 8, hostname: "Sun", plLetter: "Neptune", plRade: 3.883, plRadJ: 0.346, plMasse: 17.15, plMassJ: 0.054, plOrbsmax: 30.1 }
    ]
};
const jupiterSystem = {
    hostname: "Jupiter",
    hdName: "",
    hipName: "",
    ticId: "",
    gaiaId: "",
    stSpectype: "",
    stTeff: 0,
    stRad: 0.10045, // 1 Jupiter radius ≈ 0.10045 Solar radii
    stMass: 0.0009546, // 1 Jupiter mass ≈ 0.0009546 Solar masses
    stMet: null,
    stMetratio: "",
    stLum: 0, // Не світиться
    stAge: 4.6,
    syDist: 0,
    stLumSunAbsol: 0,
    habitablZone: null,
    exoplanets: [
        { id: 1, hostname: "Jupiter", plLetter: "Io", plRade: 0.286, plRadJ: 0.0255, plMasse: 0.015, plMassJ: 0.000047, plOrbsmax: 0.0028 }, // ~421,800 км ≈ 0.0028 AU       
        { id: 2, hostname: "Jupiter", plLetter: "Europa", plRade: 0.245, plRadJ: 0.0218, plMasse: 0.008, plMassJ: 0.000025, plOrbsmax: 0.0045 }, // ~671,100 км ≈ 0.0045 AU
        { id: 3, hostname: "Jupiter", plLetter: "Ganymede", plRade: 0.413, plRadJ: 0.037, plMasse: 0.025, plMassJ: 0.000079, plOrbsmax: 0.0072 }, // ~1,070,400 км ≈ 0.0072 AU
        { id: 4, hostname: "Jupiter", plLetter: "Callisto", plRade: 0.378, plRadJ: 0.0339, plMasse: 0.018, plMassJ: 0.000057, plOrbsmax: 0.0126 } // ~1,882,700 км ≈ 0.0126 AU
    ]
};
const earthSystem = {
    hostname: "Earth",
    hdName: "",
    hipName: "",
    ticId: "",
    gaiaId: "",
    stSpectype: "", // Земля — не зірка
    stTeff: 0,
    stRad: 0.009155, // 1 Earth radius ≈ 0.009155 Solar radii
    stMass: 0.000003003, // 1 Earth mass ≈ 3.003e-6 Solar masses
    stMet: null,
    stMetratio: "",
    stLum: 0, // не світиться
    stAge: 4.543,
    syDist: 0,
    stLumSunAbsol: 0,
    habitablZone: null,
    exoplanets: [
        // радіус відносно Землі (~1,737 км)
        // маса Місяця ≈ 0.0123 земної
        // 384,400 км ≈ 0.00257 AU
        { id: 1, hostname: "Earth", plLetter: "Moon", plRade: 0.273, plRadJ: 0.0245, plMasse: 0.0123, plMassJ: 0.000039, plOrbsmax: 0.00257 }
    ]
};
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ✅ Основна функція
function applyGridLabelLayout(planetGroup, system, systemindex) {
    //console.log("📐📐📐📐  FUNCTION: applyGridLabelLayout");
    const labelsGroup = planetGroup.selectAll(".planet-label-group");
    const labels = labelsGroup.nodes();
    const plSysParameters = {
        systemHeight: offsets[systemindex].height,
        maxPlRadius: offsets[systemindex].maxPlanetRadiusInSystem,
    };
    const { order, center } = getCenterOutwardIndices(labels.length);
    //console.log(` 🎯🎯🎯 For ${system.hostname} order is: ${order}, and its center is: ${center}`);
    const labelNodeCenter = labels[center];
    const labelCenter = d3.select(labelNodeCenter);
    grid.centralPlanetX = parseFloat(labelCenter.attr("data-xp")); // Центр планети (по X);
    order.forEach(i => {
        var _a, _b, _c, _d, _e, _f;
        // Для labels = ["A", "B", "C"] - length = 3, а і для масиву: 2, 3, 1, 4, 0 буде мати такі значення:
        // 1-й прохід: i === 2
        // 2-й прохід: i === 3
        // 3-й прохід: i === 1
        // 4-й прохід: i === 4
        // 5-й прохід: i === 0
        const labelNode = labels[i];
        const labelSVGGEl = d3.select(labelNode);
        const label = labelSVGGEl.node();
        leftRightPlanet = i === center ? 0 : i >= center ? 1 : -1;
        // Використовуємо збережені абсолютні координати
        const xPlanet = parseFloat(labelSVGGEl.attr("data-xp")); // Центр планети (по X)
        const yLabel = parseFloat(labelSVGGEl.attr("data-yl")); // Y для підпису = radius + remInPixels; - відносно центру всіх планет по Y
        if (testingLevel >= 1 && allowedPlLetters.includes((_a = system.exoplanets) === null || _a === void 0 ? void 0 : _a[i].plLetter)) {
            console.log(` 📌📌📌 ПОТОЧНЕ МІСЦЕ ДЛЯ ПІДПИСУ: `);
            console.log(`         For planet ${(_b = system.exoplanets) === null || _b === void 0 ? void 0 : _b[i].plLetter}[${i}] in svg:  X = ${Math.floor(xPlanet)}, Y: ${Math.floor(yLabel)}`);
            console.log(`         centralPlanetX = ${Math.floor(grid.centralPlanetX)}`);
            console.log(`         leftRightPlanet: ${leftRightPlanet} = ${i} >= center(${center}) ? 1 : -1;`);
        }
        let cell;
        cell = findFreeCellNear(xPlanet, yLabel, label, labels, i, leftRightPlanet, (_c = system.exoplanets) === null || _c === void 0 ? void 0 : _c[i], plSysParameters);
        if (!cell) {
            const PlName = labelSVGGEl.select(".planetNameLabel");
            if (!PlName) {
                console.error("❌ Label не має .planetNameLabel або він null", labelSVGGEl);
            }
            else {
                console.warn(` ⚠️⚠️⚠️ Не вдалося знайти вільну клітинку для планети [${PlName.text()}] => [${system.exoplanets[i].plLetter}]`);
                occupyGridAt(xPlanet, yLabel, label, "red", false);
            }
            return;
        }
        const bbox = label.getBBox();
        const newX = cell.newX;
        let newY = cell.newY;
        // Якщо орбіта невизначена або дуже маленька — примусово праворуч   ТЕСТ: K2-384
        planetDescColor = "#E4E4E4";
        preferRight = !((_d = system.exoplanets) === null || _d === void 0 ? void 0 : _d[i].plOrbsmax) || +((_e = system.exoplanets) === null || _e === void 0 ? void 0 : _e[i].plOrbsmax) <= 0.001;
        if (preferRight) {
            //console.log(`  ⚠️⚠️⚠ PLANET: ${system.exoplanets?.[i].plLetter}`);
            //console.log(`  🧪 starRadiusPx(${starRadiusPx}) > newX(${newX})`);
            //console.log(`  🧪 newX - bbox.width / 2(${bbox.width / 2}) = ${newX - bbox.width / 2} < ${starRadiusPx * 2} = starRadiusPx * 2`);
            planetDescColor = starRadiusPx > newX || newX - bbox.width / 2 < starRadiusPx * 1.4 ? "#2C2C2C" : planetDescColor;
        }
        // Зміщення текстів
        if (cell.isMoved) {
            labelSVGGEl
                .attr("transform", `translate(${newX}, ${newY})`)
                // .attr("transform", ...) - не зсуває, а встановлює нові координати відносно батківського елементу.
                // .attr("x", ...) та .attr("transform", ...) — це взаємовиключні способи позиціювання, залежно від того, який елемент ти відображаєш (text, g, circle).
                //  * Якщо label — це <g>(SVGGElement), то він не має x / y, тому.attr("x", ...) не дасть ефекту.
                //  * Якщо label — це <text>, тоді x / y працює напряму, і transform не обов'язковий.
                .attr("data-xp", newX) // оновлюємо координати
                .attr("data-yl", newY)
                .attr("fill", planetDescColor);
        }
        // Лінія від підпису до планети 
        //if (testingLevel >= 1)
        //    console.log(`   🧪🧪🧪 newX = ${newX}, newY = ${newY}`);
        if (cell.isMoved) {
            newY = cell.newY < 0 + remInPixels / 2 ? cell.newY + remInPixels * 0.3 : cell.newY - remInPixels * 0.3;
            // Додав leftRightLabel оскільки leftRightPlanet працює некоректно коли код розміщує label лівої планети праворуч, або правої планети ліворуч від планети.
            const leftRightLabel = newX < xPlanet ? -1 : newX > xPlanet ? 1 : 0;
            const offcetXLineEndpoint = leftRightLabel === 1 || preferRight
                ? -remInPixels / 3
                : leftRightLabel === -1
                    ? remInPixels / 3
                    : 0;
            const isLow = newY + bbox.height / 2 + remInPixels >= 0;
            const YLineEndpoint = isLow ? newY - remInPixels / 2 : newY + bbox.height / 2 + remInPixels * 0.5;
            const XLineEndpoint = isLow ? newX + offcetXLineEndpoint : newX;
            // Центр планети (всередині групи, тому y = 0)
            const planetX = xPlanet;
            const planetY = 0;
            // Центр підпису (label)
            const labelX = XLineEndpoint;
            const labelY = YLineEndpoint;
            // Напрямок вектора від label до планети
            const dx = planetX - labelX;
            const dy = planetY - labelY;
            const vectorLength = Math.sqrt(dx * dx + dy * dy);
            // Радіус планети + невеликий відступ
            const planetRadius = Math.min(((((_f = system.exoplanets) === null || _f === void 0 ? void 0 : _f[i].plRade) || 1) * remInPixels / 2), maxPlanetRadius);
            const lineOffset = planetRadius + 0.3 * remInPixels;
            // Новий кінець лінії — ближче до планети, але не торкається
            const endX = planetX - (dx / vectorLength) * lineOffset;
            const endY = planetY - (dy / vectorLength) * lineOffset;
            // Лінія від центру label до краю планети (зазор)
            let pointerLineColor = "yellow";
            if (preferRight)
                pointerLineColor = starRadiusPx > xPlanet ? "rgba(39, 54, 222, 0.5)" : pointerLineColor;
            planetGroup.append("line")
                .attr("x1", labelX) // Початок біля label
                .attr("y1", labelY)
                .attr("x2", endX) // Вкахує на центр планети
                .attr("y2", endY)
                .attr("stroke", testingLevel >= 30 ? "red" : pointerLineColor)
                .attr("stroke-width", 1);
        }
        // Засвітлення зайнятих клітинок. Переніс у функцію findFreeCellNear, але для відладки залишив 
        if (testingLevel >= 110)
            markOccupiedCellsAroundLabel(xPlanet, yLabel, labelSVGGEl.node(), "#FFFF80");
    });
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// ⚠️ ПОТРІБНО ДОПРАЦЮВАТИ:
//  1. Функція overlapsAnyOther повинна враховувати нові положення блоків.
//  2. Пропрацювати пошуковий радіус: searchRadiusX та searchRadiusY так, щоб блоки не виходи за межі планетарної системи та X < 0.001 AU
function findFreeCellNear(xAbs, yAbs, label, labels, planetIndex, leftRightPlanet, planetData, plSysParams) {
    //console.log("  ⚠️⚠️ FUNCTION: findFreeCellNear");
    const bbox = label.getBBox();
    const zeroX1 = zeroX + bbox.width / 2;
    // Якщо орбіта невизначена або дуже маленька — примусово праворуч
    preferRight = !(planetData === null || planetData === void 0 ? void 0 : planetData.plOrbsmax) || +planetData.plOrbsmax <= 0.001; // Хоча вперше змінна ініціалізується в renderPlanets(), та без цього рядка код падає
    //  🎯  === Central planet - Не зміщуємо ===
    // 🔹 0. Спроба покласти label у поточне місце центральної планети
    if (leftRightPlanet === 0) {
        let xAbsPreferRight = xAbs;
        let isMoved = false;
        if (preferRight) {
            xAbsPreferRight += zeroX1;
            isMoved = true; // HIP 41378
        }
        return occupyGridAt(xAbsPreferRight, yAbs, label, "rgba(0, 255, 0, 1)", isMoved);
    }
    // Параметри системи
    const planetCount = labels.length;
    const systemHeight = plSysParams.systemHeight;
    //const maxOffsetYToAbsY = plSysParams.maxPlRadius / 2 + topBottomPlSysSpace / 2;
    //const minOffsetYToAbsY = clearBaseSysHeight / 2 - topBottomPlSysSpace / 2; // Мінімальни верхній відступ для групи планет у системах з мінімальною висотою.
    // Абсолютні координати блоку
    const absX = xAbs;
    const absY = yAbs; // Y для підпису = radius + remInPixels; - відносно центру всіх планет по Y
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 🔹 1. Спроба покласти label у поточне місце інших планет
    if (!preferRight && absY >= 0 && !overlapsAnyOther(label, absX, absY)) {
        //console.log(`  ⚠️⚠️ 🔹 1. Спроба покласти label планети ${planetData.hostname}.${planetData.plLetter}[${planetIndex}] у поточне місце ⚠️⚠️`);
        //markOccupiedCellsAroundLabel(absX, absY, label, "blue");
        return occupyGridAt(absX, absY, label, "rgba(0, 0, 255, 1)", false); // "blue" = rgba(0, 0, 255, 1)
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 🔹 2. Пошук нового місця
    return searchFreeSpace();
    function searchFreeSpace() {
        //console.log(`  ⚠️⚠️ 🔹 2. Пошук нового місця для планети ${planetData.hostname}.${planetData.plLetter}[${planetIndex}] ⚠️⚠️`);
        //const centraPlanetCellX = grid.centralPlanetX / grid.colWidth; - Призначенна для пошуку місця по X лівіше або правіше від центру планети не залежно від її позиціонування (ліва/права), але з умовою, що підпис не перетинає координату X центральної планети. Поки відмовився від цієї ідеї.
        // Визначення типу планети
        const isFirstOrLast = planetIndex === 0 || planetIndex === planetCount - 1; // якщо в планети невизначена орбіта або менша 0.001, то початкова координата підпису по X = 0
        const isSecondOrPenultimate = planetIndex === 1 || planetIndex === planetCount - 2;
        // Встановлення діапазону X
        const dxStep = leftRightPlanet === 1 ? grid.colWidth : -grid.colWidth;
        const dxLimit = leftRightPlanet === 1 ? svgWidth : -svgWidth; // НЕ ЗАМІНЮЙ НА 0.
        // Встановлення діапазону Y
        // === Для planetGroupY1/2 враховуємо offset для ліній меж системи ===
        const planetGroupY1 = -systemHeight / 2 + lablePlSysSpace / 2;
        const planetGroupY2 = +systemHeight / 2 + lablePlSysSpace / 2;
        //  🎯  === Орбіта відсутня чи 0.001 — розміщення примусово справа ===
        if (preferRight) {
            let dx = 0;
            while (true) {
                const testX = zeroX1 + dx;
                const testY = absY;
                if (testX + bbox.width / 2 > svgWidth)
                    break; // вихід за межі
                if (!overlapsAnyOther(label, testX, testY)) {
                    return occupyGridAt(testX, testY, label, "white", true);
                }
                dx += grid.colWidth;
            }
            return null;
        }
        // В І Д Л А Д К А:
        // ПЛАНЕТИ НА ВІДСТАНІ ВІД 100 ДО 1000 AU І ЛИШЕ 2 ПЛАНЕТИ: TYC 8998-760-1
        // ПЛАНЕТИ НА ВІДСТАНІ ВІД 10 ДО 100 AU І ЛИШЕ 2 ПЛАНЕТИ:14 Her
        // Тип планет які логуються:
        // 0 - Звичайна планета
        // 1 - Передкрайня планета
        // 2 - Крайня планета
        const logTypePlanet = 1;
        //  🎯  === Звичайна планета ===
        if (!isFirstOrLast && !isSecondOrPenultimate) {
            //console.log(`  🎯 if (!isFirstOrLast && !isSecondOrPenultimate)  №0`);
            // absY має бути поза межами планети
            const dyStart = 0;
            const dyLimit = planetGroupY2;
            const newAbsY = absY - remInPixels / 2; // *КОСТИЛЬ: чомуь виходить дуже великий радіус, хоча всі label розміщуються правильно.
            for (let dx = 0; leftRightPlanet === 1 ? dx <= dxLimit : dx >= dxLimit; dx += dxStep) {
                const testX = absX + dx;
                if (testX - bbox.width / 2 < zeroX1 || testX + bbox.width / 2 > svgWidth)
                    continue;
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`  absX(${Math.floor(absX)}) + dx(${Math.floor(dx)}) = testX(${Math.floor(testX)}) ><= ` +
                        `dxLimit(${Math.floor(dxLimit)}) + dxStep(${Math.floor(dxStep)})`);
                // Поку місця нижче absY
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`нижче: newAbsY(${Math.floor(newAbsY)}) + dy(${Math.floor(dyStart)}) + bbox.height(${Math.floor(bbox.height)} / 2)` +
                        ` == ${Math.floor(newAbsY + dyStart + bbox.height / 2)} <= dyLimit(${Math.floor(dyLimit)})`);
                for (let dy = dyStart; newAbsY + dy + bbox.height / 2 <= dyLimit; dy += grid.rowHeight) {
                    // ⚠️ЗАЛЕЖНІСТЬ №3.  + remInPixels / 3 - щоб лінія від планети йшла вниз, а не горизонтально:                    
                    const testY1 = newAbsY + dy; // Тест на: KOI-351 
                    if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`нижче: newAbsY(${Math.floor(newAbsY)}) + dy(${Math.floor(dyStart)}) + bbox.height(${Math.floor(bbox.height)} / 2)` +
                            ` == ${Math.floor(newAbsY + dy + bbox.height / 2)} <= dyLimit(${Math.floor(dyLimit)})| ${Math.floor(testY1)}`);
                    if (!overlapsAnyOther(label, testX, testY1)) {
                        return occupyGridAt(testX, testY1, label, "white", true);
                    }
                }
                // Пошук місця вище absY
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`вище: newAbsY(${Math.floor(-newAbsY)}) - dy(${Math.floor(dyStart)}) == ${Math.floor(-newAbsY - dyStart)} >= ` +
                        `planetGroupY1(${Math.floor(planetGroupY1)})`);
                for (let dy = dyStart; -newAbsY - dy >= planetGroupY1; dy += grid.rowHeight) {
                    let testY = Math.max(-newAbsY - dy, planetGroupY1); // Тест на: HD 10180, Kepler-20
                    if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`вище: newAbsY(${Math.floor(-newAbsY)}) - dy(${Math.floor(dy)}) = ${Math.floor(-newAbsY - dy)} >= ` +
                            `planetGroupY1(${Math.floor(planetGroupY1)})| ${Math.floor(testY)}`);
                    if (!overlapsAnyOther(label, testX, testY)) {
                        return occupyGridAt(testX, testY, label, "white", true);
                    }
                }
            }
        }
        //  🎯  === Передкрайня планета ===
        else if (isSecondOrPenultimate) {
            //console.log(`  🎯🎯 for else if (isSecondOrPenultimate) №1`);
            const yStartTop = -absY + bbox.height; // зміщуємось над центром. *КОСТИЛЬ: Мало б бути лише: -absY - але так блоки вилазять за межі 
            const yStartBottom = absY + bbox.height - Math.max(planetData.plRade, remInPixels);
            let testX;
            // Пошук місця вище absY
            const tryAbove = () => {
                if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`вище: absY(${Math.floor(-absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartTop(${Math.floor(yStartTop)}) ` +
                        `- dy(${Math.floor(0)}) == ${Math.floor(yStartTop - 0)} >= planetGroupY1(${Math.floor(planetGroupY1)})`);
                //console.log(`ABOVE`);
                for (let dy = 0; yStartTop - dy >= planetGroupY1; dy += grid.rowHeight) {
                    const testY = yStartTop - dy;
                    if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`вище: absY(${Math.floor(-absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartTop(${Math.floor(yStartTop)}) ` +
                            `- dy(${Math.floor(dy)}) == ${Math.floor(yStartTop - dy)} >= ` +
                            `planetGroupY1(${Math.floor(planetGroupY1)})| ${Math.floor(testY)}`);
                    if (testY > 0)
                        continue; // тіло label не повинно опускатися нижче центру
                    if (!overlapsAnyOther(label, testX, testY)) {
                        return occupyGridAt(testX, testY, label, "yellow", true);
                    }
                }
            };
            // Поку місця нижче absY
            const tryBelow = () => {
                if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`нижче: absY(${Math.floor(absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartBottom(${Math.floor(yStartBottom)}) + ` +
                        `dy(${Math.floor(grid.rowHeight)}) == ${Math.floor(yStartBottom + grid.rowHeight)} <= planetGroupY2(${Math.floor(planetGroupY2)})`);
                //console.log(`BELOW`);
                for (let dy = grid.rowHeight; yStartBottom + dy <= planetGroupY2; dy += grid.rowHeight) {
                    const testY = yStartBottom + dy - bbox.height;
                    if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`нижче: absY(${Math.floor(absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartBottom(${Math.floor(yStartBottom)}) + ` +
                            `dy(${Math.floor(dy)}) == ${Math.floor(yStartBottom + dy)} <= planetGroupY2(${Math.floor(planetGroupY2)}) | ${Math.floor(testY)}`);
                    if (!overlapsAnyOther(label, testX, testY)) {
                        return occupyGridAt(testX, testY, label, "yellow", true);
                    }
                }
            };
            for (let dx = 0; leftRightPlanet === 1 ? dx <= dxLimit : dx >= dxLimit; dx += dxStep) {
                testX = absX + dx;
                if (testX - bbox.width / 2 < zeroX1)
                    continue;
                if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter)) {
                    console.log(`    🎯 absX(${Math.floor(absX)}) + dx(${Math.floor(dx)}) = testX(${Math.floor(testX)}) ><= ` +
                        `dxLimit(${Math.floor(dxLimit)}) + dxStep(${Math.floor(dxStep)}) `);
                    console.log(`  🎯 planetData.plRade / 2 * remInPixels(${planetData.plRade / 2 * remInPixels}) > ${lablePlSysSpace + topBottomPlSysSpace * 2} = ` +
                        `lablePlSysSpace(${lablePlSysSpace}) + topBottomPlSysSpace * 2(${topBottomPlSysSpace * 2})`);
                }
                const first = planetData.plRade / 2 * remInPixels > lablePlSysSpace + topBottomPlSysSpace * 2
                    ? tryAbove
                    : tryBelow; // K2-285
                const second = first === tryAbove ? tryBelow : tryAbove;
                // Перший напрямок
                const result1 = first();
                if (result1)
                    return result1;
                // Другий напрямок
                const result2 = second();
                if (result2)
                    return result2;
            }
        }
        //  🎯  === Крайня планета ===
        else if (isFirstOrLast) {
            //console.log(`  🎯🎯🎯 for else if (isFirstOrLast) №2`);
            const startDy = 0 + lablePlSysSpace / 4;
            const dyStep = grid.rowHeight;
            const dxDirection = leftRightPlanet === 1 ? 1 : -1;
            const dxOffset = (remInPixels + dxDirection) * 5 + planetData.plRade / 2; // ⚠️ЗАЛЕЖНІСТЬ №4. + dxDirection - збільшує відступ для правих планет і зменшує для ліих.
            // === Етап 1: збільшуємо dy вниз і dx (вліво або вправо) ===
            for (let dy = 0;; dy += dyStep) {
                const testY1 = startDy + dy;
                const testY2 = testY1 + bbox.height;
                if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`етап №1: testY2(${Math.floor(testY2)}) > planetGroupY2(${Math.floor(planetGroupY2)}) == ${testY2 > planetGroupY2}` +
                        `. testY1(${Math.floor(testY1)}). dxOffset(${Math.floor(dxOffset)})`);
                if (testY2 > planetGroupY2)
                    break; // нижня межа
                for (let dx = 0; dx <= svgWidth; dx += dxStep) {
                    let testX = absX + dx + dxOffset;
                    if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter)) {
                        console.log(` testX(${Math.floor(testX)}) = absX(${Math.floor(absX)}) + (dx(${Math.floor(dx)}) + ` +
                            `dxOffset(${Math.floor(dxOffset)}) ) * dxDirection(${Math.floor(dxDirection)})`);
                        console.log(` testX(${Math.floor(testX)}) - bbox.width/2(${Math.floor(bbox.width / 2)}) < zeroX1(${Math.floor(zeroX1)})` +
                            ` == ${testX < zeroX1}`);
                    }
                    if (testX < zeroX1) {
                        testX += remInPixels; // ⚠️ЗАЛЕЖНІСТЬ №4
                        if (testX < zeroX1)
                            break;
                    }
                    if (!overlapsAnyOther(label, testX, testY1)) {
                        return occupyGridAt(testX, testY1, label, "orange", true);
                    }
                }
            }
            // === Етап 2: зменшуємо dy вгору і dx (вліво або вправо) ===
            for (let dy = dyStep;; dy += dyStep) {
                const testY1 = startDy - dy;
                if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`етап №2: testY1(${Math.floor(testY1)}) < planetGroupY2(${Math.floor(planetGroupY2)}) == ${testY1 > planetGroupY2}` +
                        `. dxOffset(${Math.floor(dxOffset)})`);
                if (testY1 < planetGroupY1)
                    break; // верхня межа
                for (let dx = 0; dx <= svgWidth; dx += dxStep) {
                    let testX = absX + dx + dxOffset;
                    if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter)) {
                        console.log(` testX(${Math.floor(testX)}) - bbox.width/2(${Math.floor(bbox.width / 2)}) < zeroX1(${Math.floor(zeroX1)})` +
                            ` == ${testX < zeroX1}`);
                    }
                    if (testX < zeroX1) {
                        testX += remInPixels; // ⚠️ЗАЛЕЖНІСТЬ №4
                        if (testX < zeroX1)
                            break;
                    }
                    if (!overlapsAnyOther(label, testX, testY1)) {
                        return occupyGridAt(testX, testY1, label, "orange", true);
                    }
                }
            }
        }
        return null;
    }
    //==========================================================================================================================
    //==========================================================================================================================
    // Функція перевірки перетину поточного підпису з іншими підписами поточної планетарної системи
    function overlapsAnyOther(label, testX, testY) {
        //console.log("   FUNCTION: overlapsAnyOther ===========================");
        const labelSpacing = remInPixels * 0.3;
        // Визначення координат розміру підпису поточної планети
        const currentBox = {
            x1: testX - labelSpacing,
            y1: testY - labelSpacing,
            x2: testX + bbox.width + labelSpacing,
            y2: testY + bbox.height + labelSpacing
        };
        if (testingLevel >= 1 && allowedPlLetters.includes(planetData.plLetter)) {
            console.log(`   📐 testX = ${Math.floor(testX)}`);
            console.log(`   📐 testY = ${Math.floor(testY)}`);
            console.log(`   📐 bbox.width = ${Math.floor(bbox.width)}`);
            console.log(`   📐 bbox.height = ${Math.floor(bbox.height)}`);
            console.log(`   📐 currentBox.x1 = ${Math.floor(currentBox.x1)}`);
            console.log(`   📐 currentBox.y1 = ${Math.floor(currentBox.y1)}`);
            console.log(`   📐 currentBox.x2 = ${Math.floor(currentBox.x2)}`);
            console.log(`   📐 currentBox.y2 = ${Math.floor(currentBox.y2)}`);
        }
        // 📌 Перевірка на зайняті клітинки (до перевірки з іншими labels)
        const absXL = testX - bbox.width / 2; // ⚠️ЗАЛЕЖНІСТЬ №2. "- bbox.width / 2" - оскільтки текст центрований по горизонталі
        const absYL = testY - remInPixels; // ⚠️ЗАЛЕЖНІСТЬ №3. *КОСТИЛЬ: remInPixels
        const startCol = Math.floor(absXL / grid.colWidth);
        const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
        const startRow = Math.floor(absYL / grid.rowHeight);
        const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
        for (let c = startCol; c <= endCol; c++) {
            for (let r = startRow; r <= endRow; r++) {
                const key = `${c},${r}`;
                if (grid.occupied.has(key)) {
                    // Якщо хоча б одна клітинка зайнята — перериваємо
                    return true;
                }
            }
        }
        // Позначення поточного label як уже перевіреного
        d3.select(label).attr("checked", "1");
        let testIndex = 0;
        // 📌 Перевірка на перетин із іншими labels
        return labels.some(other => {
            //console.log("   Method: some ---------------------------");
            testIndex++;
            if (other === label)
                return false;
            const isChecked = parseFloat(other.getAttribute("checked") || "0");
            if (testingLevel >= 1 && allowedPlLetters.includes(planetData.plLetter))
                console.log(`   📐 label[${testIndex}] isChecked === ${isChecked}`);
            if (isChecked === 0)
                return false;
            const otherBBox = other.getBBox();
            const otherX = parseFloat(other.getAttribute("data-xp") || "0"); // Центр планети (по X)
            const otherY = parseFloat(other.getAttribute("data-yl") || "0"); // Y для підпису = radius + remInPixels; - відносно центру всіх планет по Y
            const otherBox = {
                // Визначення координат розміру інших підписів в циклі методу some()
                x1: otherX, // Центр планети (по X)
                y1: otherY, // Y для підпису = radius + remInPixels; - відносно центру всіх планет по Y
                x2: otherX + otherBBox.width,
                y2: otherY + otherBBox.height
            };
            if (testingLevel >= 1 && allowedPlLetters.includes(planetData.plLetter)) {
                console.log("   +++++++++++++++++++++++++++");
                console.log(`   📐 otherX = ${Math.floor(otherX)}`);
                console.log(`   📐 otherY = ${Math.floor(otherY)}`);
                console.log(`   📐 otherBBox.width = ${Math.floor(otherBBox.width)}`);
                console.log(`   📐 otherBBox.height = ${Math.floor(otherBBox.height)}`);
                console.log("   ===========================");
            }
            // У змінну isOverlaps записується true, якщо прямокутники currentBox і otherBox перетинаються, і false, якщо вони не перетинаються.
            // Два прямокутники перетинаються, якщо НЕ виконується жодна з умов "розділення":
            const isOverlaps = !(currentBox.x1 > otherBox.x2 || currentBox.x2 < otherBox.x1 || currentBox.y1 > otherBox.y2 || currentBox.y2 < otherBox.y1);
            //console.log(`   📐📐 currentBox.x1(${Math.floor(currentBox.x1)}) > otherBox.x2(${Math.floor(otherBox.x2)}) = ${currentBox.x1 > otherBox.x2}`);
            //console.log(`   📐📐 currentBox.x2(${Math.floor(currentBox.x2)}) < otherBox.x1(${Math.floor(otherBox.x1)}) = ${currentBox.x2 < otherBox.x1}`);
            //console.log(`   📐📐 currentBox.y1(${Math.floor(currentBox.y1)}) > otherBox.y2(${Math.floor(otherBox.y2)}) = ${currentBox.y1 > otherBox.y2}`);
            //console.log(`   📐📐 currentBox.y2(${Math.floor(currentBox.y2)}) < otherBox.y1(${Math.floor(otherBox.y1)}) = ${currentBox.y2 < otherBox.y1}`);
            //console.log(`   📐✔ isOverlaps = ${isOverlaps}(!${!isOverlaps})`);
            return isOverlaps;
        });
    }
}
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ✅ Позначення зайнятих клітинок
// Повертає зміщені x та y настільки щоб вони відповідали верхньому лівому краю верхньої лівої клітинки, тобто її початку
function occupyGridAt(x, y, label, color, isMoved) {
    //console.log("    📌🔲  FUNCTION: occupyGridAt");
    // Межі текстового блоку (локальні координати)
    const bbox = label.getBBox();
    const absXL = x - bbox.width / 2; // ⚠️ЗАЛЕЖНІСТЬ №2. "- bbox.width / 2" - оскільтки текст центрований по горизонталі
    const absYL = y - remInPixels; // ⚠️ЗАЛЕЖНІСТЬ №3. *КОСТИЛЬ: remInPixels
    //console.log(`    📐 bbox.x = ${bbox.x}, bbox.y = ${bbox.y}`);
    //console.log(`    📐 x: ${Math.floor(x)}, y: ${Math.floor(y)}`);
    //console.log(`    📐 absXL = ${absXL}, absYL = ${absYL}`);
    //console.log(`    📐 bbox.height = ${bbox.height}, bbox.width = ${bbox.width}`);
    // Визначення колонок
    const startCol = Math.floor(absXL / grid.colWidth);
    const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
    const startRow = Math.floor(absYL / grid.rowHeight);
    const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
    //console.log(`    🎯 startCol = ${startCol}, endCol = ${endCol}`);
    //console.log(`    🎯 startRow = ${startRow}, endRow = ${endRow}`);
    // Позначаємо клітинки як зайняті
    for (let c = startCol; c <= endCol; c++) {
        for (let r = startRow; r <= endRow; r++) {
            const key = `${c},${r}`;
            grid.occupied.add(key);
            if (testingLevel >= 1)
                // 🔲 Візуалізація зайнятої клітинки
                d3.select(label.parentNode)
                    .append("rect")
                    .attr("x", c * grid.colWidth)
                    .attr("y", r * grid.rowHeight)
                    .attr("width", grid.colWidth)
                    .attr("height", grid.rowHeight)
                    .attr("fill", color)
                    .attr("opacity", 0.3);
        }
    }
    // Відкат до початкових значень щоб зайняті та замальовані клітинки відповідали розташування тексту:
    const newX = x;
    const newY = y;
    return { newX, newY, isMoved };
}
/// 🔲 Замальовування фону зайнятих клітин
// Залишив, оскільки .getBBox() з width та height також трохи глючить.
function markOccupiedCellsAroundLabel(newX, newY, label, color) {
    //console.log("  📌🔲  FUNCTION: markOccupiedCellsAroundLabel - ПРАЦЮЄ ДОБРЕ");
    // Властивості bbox: top, bottom, left, right - undefined
    const bbox = label.getBBox(); // межі текстового блоку (локальні координати)
    const absXL = newX - bbox.width / 2; // ⚠️ЗАЛЕЖНІСТЬ №2. "- bbox.width / 2" - оскільтки текст центрований по горизонталі
    const absYL = newY - remInPixels; // ⚠️ЗАЛЕЖНІСТЬ №3. *КОСТИЛЬ: remInPixels
    //console.log(`  📐 bbox.x = ${bbox.x}, bbox.y = ${bbox.y}`);
    //console.log(`  📐 x: ${Math.floor(newX)}, y: ${Math.floor(newY)}`);
    //console.log(`  📐 absXL = ${absXL}, absYL = ${absYL}`);
    //console.log(`  📐 bbox.height = ${bbox.height}, bbox.width = ${bbox.width}`);
    // Інші властивості:
    const startCol = Math.floor(absXL / grid.colWidth);
    const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
    const startRow = Math.floor(absYL / grid.rowHeight);
    const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
    //console.log(`  🎯 startCol = ${startCol}, endCol = ${endCol}`);
    //console.log(`  🎯 startRow = ${startRow}, endRow = ${endRow}`);
    for (let c = startCol; c <= endCol; c++) {
        for (let r = startRow; r <= endRow; r++) {
            // 🔲 👉 Візуалізація клітинок
            d3.select(label.parentNode)
                .append("rect")
                .attr("x", c * grid.colWidth)
                .attr("y", r * grid.rowHeight)
                .attr("width", grid.colWidth)
                .attr("height", grid.rowHeight)
                .attr("fill", color)
                .attr("opacity", 0.3);
        }
    }
}
/// ✅ Візуалізація сітки
function drawGridOverlay(svg, offsetY, systemHeight) {
    //console.log("FUNCTION: drawGridOverlay");
    //console.log(` grid.colWidth = ${grid.colWidth}`);
    //console.log(` grid.rowHeight = ${grid.rowHeight}`);
    //console.log(` grid.cols = ${grid.cols}`);
    //console.log(` grid.rows = ${grid.rows}`);
    const gridGroup = svg.append("g").attr("class", "grid-overlay");
    // Вертикальні лінії
    for (let col = 0; col <= grid.cols; col++) {
        if (col < 1)
            continue;
        const x = col * grid.colWidth;
        const y1 = -systemHeight / 2 + lablePlSysSpace / 2;
        const y2 = systemHeight / 2 + lablePlSysSpace / 2;
        gridGroup.append("line")
            .attr("x1", x)
            .attr("y1", y1)
            .attr("transform", `translate(0, ${offsetY})`)
            .attr("x2", x)
            .attr("y2", y2)
            .attr("stroke", "rgba(0, 0, 255, 0.6)")
            .attr("stroke-dasharray", "2,2");
    }
    // Горизонтальні лінії
    for (let row = 0; row <= grid.rows; row++) {
        if (row < 1)
            continue;
        const y = row * grid.rowHeight - systemHeight / 2 + lablePlSysSpace / 2;
        gridGroup.append("line")
            .attr("x1", 0)
            .attr("y1", y)
            .attr("transform", `translate(0, ${offsetY})`)
            .attr("x2", svgWidth)
            .attr("y2", y)
            .attr("stroke", "rgba(0, 0, 255, 0.6)")
            .attr("stroke-dasharray", "2,2");
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// ✅ getTranslateX(...) – безпечне отримання значення x з атрибуту, або повернення 0
// getTranslateX(next.descGroup.attr("transform"));
function getTranslateX(transform) {
    if (!transform)
        return 0;
    const match = transform.match(/translate\(([-\d.]+),/);
    return match ? parseFloat(match[1]) : 0;
}
/// ✅  Сортування масиву за принципами:
//   * Від центру до країв.
//   * При цьому спочатку в праву сторону від центру, а потім в ліву сторону.
//   * А також якщо к-сть labels парна, то за центральну вважати лівішу від центру масиву.
//
//  Тобто, для:
//  парногого масиву: [0, 1, 2, 3, 4, 5], то порядок сортування такий: 2, 3, 1, 4, 0, 5. 
//  парногого масиву: [0, 1, 2, 3, 4, 5, 6, 7], то порядок сортування такий: 3, 4, 2, 5, 1, 6, 0, 7. 
//  непарного масиву: [0, 1, 2, 3, 4] порядок сортування такий: 2, 3, 1, 4, 0.
function getCenterOutwardIndices(length) {
    // Even or odd number of planets.
    const center = Math.floor((length - 1) / 2); // Тут вже потрібна планета: середня для непарних, або перша ліва для парних
    const order = [center]; // додаємо центр одразу
    for (let offset = 1; order.length < length; offset++) {
        const right = center + offset;
        const left = center - offset;
        if (right < length) {
            order.push(right); // першим —  правий
        }
        if (left >= 0) {
            order.push(left); // потім — лівий
        }
    }
    return { order: order, center: center };
}
//# sourceMappingURL=planetarySystemVisualization.js.map
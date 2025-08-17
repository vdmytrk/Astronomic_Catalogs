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
const d3 = __importStar(require("d3"));
const metrics_1 = require("./metrics");
let rect;
let zeroX;
let width;
let margin;
let spacing;
let endRange;
let planetGroup;
let remInPixels;
let radiusEarth;
let preferRight;
let starRadiusPx;
let maxTickOrder = 4;
let maxPlanetRadius;
let lablePlSysSpace;
let leftRightPlanet;
let correctionFactor;
let clearBaseSysHeight;
let topBottomPlSysSpace;
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
const testingLevel = 0;
const allowedPlLetters = ["d"];
const grid = {
    colWidth: 0,
    rowHeight: 0,
    cols: 0,
    rows: 0,
    occupied: new Set(),
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
function dataVisualization() {
    return __awaiter(this, void 0, void 0, function* () {
        console.log(" FUNCTION dataVisualization >>>>>>>>>>>>> ");
        clearData();
        rect = yield (0, metrics_1.getElementRect)("#planetsSystemTableContainer");
        remInPixels = parseFloat(getComputedStyle(document.documentElement).fontSize);
        lablePlSysSpace = 3 * remInPixels;
        clearBaseSysHeight = 4 * remInPixels;
        topBottomPlSysSpace = 1 * remInPixels;
        maxPlanetRadius = 11 * remInPixels;
        spacing = 2 * remInPixels;
        radiusEarth = remInPixels / 2;
        margin = {
            top: remInPixels,
            right: 3 * remInPixels,
            bottom: remInPixels,
            left: 3 * remInPixels
        };
        const container = document.querySelector('.planetarySystemsVisualization');
        if (!container) {
            console.warn("No container for planetary system background");
            return;
        }
        svg = d3.select(container).select("svg.planetaryAxis");
        svgDesc = d3.select(container).select("svg.planetaryAxisDescription");
        addData();
        calculateSystemsPosition();
        maximumOrder();
        const nonZeroCoefficient = 4;
        correctionFactorForAdditionWidth = Math.pow(maxTickOrder + nonZeroCoefficient, 1 / 4.6);
        const orderCount = maxTickOrder + nonZeroCoefficient - 1;
        const minWidth = orderCount * 200;
        width = Math.max(rect.width - 11 * remInPixels * correctionFactorForAdditionWidth, minWidth, 1000);
        getCorrectionFactorForWidth(width, [900, 2070, 4227], [0.35, 1.0, 2.5], 1);
        renderBackgroundAxis();
        renderSystemCharts();
        logingDataVisualization();
    });
}
;
function addData() {
    const charts = document.querySelectorAll('.planetary-system-data');
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
    let totalHeight = maxPlanetRadius;
    if (offsets.length > 0) {
        totalHeight = offsets[0].height + offsets.at(-1).offsetY + offsets.at(-1).height;
        rect.height = totalHeight;
    }
    svg
        .attr("width", width + 10 * remInPixels * correctionFactorForAdditionWidth)
        .attr("height", totalHeight + maxPlanetRadius);
    svgDesc
        .attr("width", width + 11 * remInPixels)
        .attr("height", 2.5 * remInPixels);
    const minTickOrder = -3;
    const baseTicks = [];
    for (let i = minTickOrder; i <= maxTickOrder; i++) {
        baseTicks.push(Math.pow(10, i));
    }
    const tickSet = new Set(baseTicks);
    allTicks = [...baseTicks];
    const lastOrder = baseTicks.at(-1);
    setEndRange();
    axesX = d3.scaleLog()
        .domain([0.001, lastOrder])
        .range([margin.left, endRange]);
    for (let i = -3; i <= maxTickOrder; i++) {
        const base = Math.pow(10, i);
        for (let m = 1; m <= 9; m++) {
            const tick = base * m;
            if (tick >= 0.001 && tick <= lastOrder && !tickSet.has(tick)) {
                allTicks.push(tick);
            }
        }
    }
    const axesLableY = 1.3;
    svg.append("line")
        .attr("x1", 0)
        .attr("y1", 0)
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
        const lastDescCorrectionFactor = Math.min(3 * remInPixels / correctionFactor, 2 * remInPixels);
        zeroX = orderLength * (1 / 5 + 1 / 6);
        let lineWidth = baseIndex == 0 ? 0.02 : 0.05;
        svg.append("line")
            .attr("x1", x + zeroX)
            .attr("y1", 0)
            .attr("x2", x + zeroX)
            .attr("y2", totalHeight - margin.bottom)
            .attr("stroke", "#888")
            .attr("class", isBase ? "BASE_TICK" : "SUB_TICK")
            .attr("stroke-width", isBase ? lineWidth * remInPixels : 0.02 * remInPixels);
        if (isBase) {
            svgDesc.append("text")
                .attr("x", baseIndex + 1 === tickCount ? x + zeroX - lastDescCorrectionFactor : x + zeroX)
                .attr("y", axesLableY * remInPixels)
                .attr("text-anchor", "middle")
                .attr("class", "logarithmicScale")
                .text(baseIndex === 0 ? "0.001 AU" : `${formatTick(tick)} AU`);
            baseIndex++;
        }
    });
}
function renderSystemCharts() {
    console.log(" FUNCTION renderAllSystemCharts >>>>>>>>>>>>> ");
    allSystems.forEach((system, index) => {
        if (!offsets[index]) {
            console.error(`No offset data for index ${index}`);
            return;
        }
        const offsetY = offsets[index].offsetY +
            offsets[index].height / 2;
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
    grid.occupied.clear();
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
        .lower();
    const solarRadiusInAU = 0.00465047;
    let starRadiusAU = hasNotRadius ? 0.103 * solarRadiusInAU : system.stRad * solarRadiusInAU;
    let pixelX;
    if (starRadiusAU >= 0.001)
        pixelX = axesX(starRadiusAU) + zeroX;
    else {
        const pseudoAxesX = createPseudoLogScale([0, 10000], [margin.left, endRange]);
        pixelX = pseudoAxesX(starRadiusAU);
    }
    const starRadiusPx = Math.abs(pixelX);
    const gradientId = `starCoronaGradient-${system.hostname.replace(/\W/g, "")}`;
    const defs = svg.select("defs").empty() ? svg.insert("defs", ":first-child") : svg.select("defs");
    const stellarCoronaColor = system.stTeff < 100 ? "rgba(148, 148, 148, 1)" : temperatureToColor(system.stTeff);
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
            .attr("offset", "24%")
            .attr("stop-color", stellarCoronaColor)
            .attr("stop-opacity", 0.8);
        gradient.append("stop")
            .attr("offset", "100%")
            .attr("stop-color", stellarCoronaColor)
            .attr("stop-opacity", 0);
    }
    const coronaRadiusPx = starRadiusPx * 3;
    starGroup.append("circle")
        .attr("cx", 0)
        .attr("cy", offsetYStar)
        .attr("r", coronaRadiusPx)
        .attr("fill", `url(#${gradientId})`);
    const fadeColorStr = d3.color(stellarCoronaColor);
    fadeColorStr.opacity = 0.9;
    const starFill = isEJ ? "rgba(255, 0, 0, 0.35)" : fadeColorStr.toString();
    starGroup.append("circle")
        .attr("cx", 0)
        .attr("cy", offsetYStar)
        .attr("r", starRadiusPx)
        .attr("fill", starFill);
    if (hasNotRadius) {
        starGroup.append("circle")
            .attr("cx", 0)
            .attr("cy", offsetYStar)
            .attr("r", starRadiusPx)
            .attr("fill", "none")
            .attr("stroke", "rgba(10, 10, 10, 0.5")
            .attr("stroke-width", remInPixels * 0.1)
            .attr("stroke-dasharray", `${remInPixels * 0.4}, ${remInPixels * 0.2}`)
            .attr("pointer-events", "none");
    }
    const clipId = createStarClipPath(system.hostname, offsetY, offsets[index].height);
    starGroup.attr("clip-path", `url(#${clipId})`);
    planetDescColor = "#E4E4E4";
    const sysHeaderFill = isEJ ? planetDescColor : "#D01818";
    const sysDescFill = "#F61C1C";
    svg.append("text")
        .attr("x", remInPixels)
        .attr("y", -remInPixels)
        .attr("transform", `translate(0, ${offsetY})`)
        .text(system.hostname)
        .attr("font-size", "1.3rem")
        .style("font-weight", "bold")
        .attr("fill", sysHeaderFill);
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
            const num = typeof value === "number"
                ? value
                : typeof value === "string"
                    ? parseFloat(value)
                    : NaN;
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
            stLum = "1";
        else
            stLum = formatStellarLuminosity(system.stLum);
        const stRadEarth = hasNotRadius ? "?" : (system.stRad * 109.076).toFixed(2);
        const stRadJup = hasNotRadius ? "?" : (system.stRad * 9.731).toFixed(2);
        const stMassEarth = system.stMass != 0.00 ? (system.stMass * 332946).toFixed(2) : "?";
        const stMassJup = system.stMass != 0.00 ? (system.stMass * 1047.57).toFixed(2) : "?";
        labelSys.append("text").text(`${stLum} L☉ | ${stTeff} K`).attr("y", remInPixels * 0.0);
        labelSys.append("text").text(`${stRad} R☉`).attr("y", remInPixels * 1.2);
        labelSys.append("text").text(`${stMass} M☉`).attr("y", remInPixels * 2.4);
        labelSys.append("text").text(`Age: ${stAge}`).attr("y", remInPixels * 3.6);
    }
    return starRadiusPx;
}
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
function createStarClipPath(hostname, offsetY, systemHeight) {
    const clipId = `starClip-${hostname.replace(/\W/g, "")}`;
    const defs = svg.select("defs").empty()
        ? svg.insert("defs", ":first-child")
        : svg.select("defs");
    const topY = offsetY - systemHeight / 2 + lablePlSysSpace / 2;
    const bottomY = offsetY + systemHeight / 2 + lablePlSysSpace / 2;
    defs.append("clipPath")
        .attr("id", clipId)
        .append("rect")
        .attr("x", 0)
        .attr("y", topY)
        .attr("width", svgWidth)
        .attr("height", bottomY - topY);
    return clipId;
}
function lightenColor(r, g, b, factor) {
    const newR = Math.round(r + (255 - r) * factor);
    const newG = Math.round(g + (255 - g) * factor);
    const newB = Math.round(b + (255 - b) * factor);
    return `rgba(${newR}, ${newG}, ${newB}, 1)`;
}
function temperatureToColor(tempK) {
    tempK = Math.max(300, Math.min(tempK, 50000));
    const t = tempK / 100;
    let r, g, b;
    if (tempK >= 600) {
        r = t <= 66
            ? 255
            : 329.698727446 * Math.pow(t - 60, -0.1332047592);
        r = Math.min(Math.max(r, 0), 255);
        g = t <= 66
            ? 99.4708025861 * Math.log(t) - 161.1195681661
            : 288.1221695283 * Math.pow(t - 60, -0.0755148492);
        g = Math.min(Math.max(g, 0), 255);
        b = t >= 66
            ? 255
            : t <= 19
                ? 0
                : 138.5177312231 * Math.log(t - 10) - 305.0447927307;
        b = Math.min(Math.max(b, 0), 255);
    }
    else {
        const factor = (tempK - 300) / (600 - 300);
        const baseR = 255;
        const baseG = 56;
        const baseB = 0;
        r = baseR * factor;
        g = baseG * factor;
        b = baseB * factor;
    }
    const toHex = (c) => Math.round(c).toString(16).padStart(2, '0');
    return `#${toHex(r)}${toHex(g)}${toHex(b)}`;
}
function renderHabitablZone(system, planetGroup, systemHeight) {
    const borderThickness = 0.1 * remInPixels;
    if (system.habitablZone && (system.stLum != 0.00 || system.hostname == "Sun")) {
        const hzCenter = +system.habitablZone;
        const sunMin = 0.75;
        const sunMax = 1.77;
        const sunCenter = 1.26;
        const hzMin = hzCenter * (sunMin * 0.75);
        const hzMax = hzCenter * (sunMax * 1.25);
        const hzX1 = axesX(hzMin) + zeroX;
        const hzX2 = axesX(hzMax) + zeroX;
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
        const centerPercent = ((logHzCenter - logHzMin) / (logHzMax - logHzMin)) * 100;
        gradient.append("stop").attr("offset", "0%").attr("stop-color", "rgba(0, 255, 25, 0");
        gradient.append("stop").attr("offset", `${centerPercent - 35}%`).attr("stop-color", "rgba(0, 177, 25, 0.1)");
        gradient.append("stop").attr("offset", `${centerPercent - 20}%`).attr("stop-color", "rgba(0, 177, 25, 0.8)");
        gradient.append("stop").attr("offset", `${centerPercent}%`).attr("stop-color", "rgba(0, 255, 25, 0.67)");
        gradient.append("stop").attr("offset", `${centerPercent + 20}%`).attr("stop-color", "rgba(0, 177, 25, 0.8)");
        gradient.append("stop").attr("offset", `${centerPercent + 40}%`).attr("stop-color", "rgba(0, 177, 25, 0.1)");
        gradient.append("stop").attr("offset", "100%").attr("stop-color", "rgba(0, 255, 25, 0");
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
            const distance = (rawDistance > 0.001 && !isNaN(rawDistance)) ? rawDistance : 0.001;
            const currentRadius = (planet.plRade || 1) * radiusEarth;
            const radius = Math.min(currentRadius, maxPlanetRadius);
            const planetName = planet.plLetter || "?";
            const hasRadius = !!planet.plRade;
            const x = axesX(distance) + zeroX;
            const y = 0;
            planetGroup.append("circle")
                .attr("cx", x)
                .attr("cy", y)
                .attr("r", radius)
                .attr("fill", hasRadius ? "rgba(255, 0, 0, 0.35)" : "rgba(94, 94, 94)");
            occupyGridForPlanetDisk(x, y, radius, planetGroup, system, planet);
            const labelY = radius + remInPixels;
            const label = planetGroup.append("g")
                .attr("class", "planet-label-group")
                .attr("transform", `translate(${x}, ${labelY})`)
                .attr("text-anchor", "middle")
                .attr("alignment-baseline", "middle")
                .attr("checked", 0)
                .attr("data-xp", x)
                .attr("data-yl", labelY)
                .attr("font-size", "1rem")
                .attr("fill", "#E4E4E4");
            label.append("text")
                .text(`${planetName}`)
                .attr("y", 0)
                .raise();
            const format = (v) => (v.toFixed(2) === "0.00" ? "?" : v.toFixed(2));
            const rade = format(planet.plRade);
            const radj = format(planet.plRadJ);
            const masse = format(planet.plMasse);
            const massj = format(planet.plMassJ);
            if ((typeof planet.plRadJ === "number" && planet.plRadJ < jupiterRadiusHalf) &&
                (typeof planet.plMassJ === "number" && planet.plMassJ < jupiterMassHalf)) {
                label.append("text").text(`${rade} R⊕`).attr("y", remInPixels);
                label.append("text").text(`${masse} M⊕`).attr("y", remInPixels * 2);
            }
            else {
                label.append("text").text(`${rade} R⊕ | ${radj} R♃`).attr("y", remInPixels);
                label.append("text").text(`${masse} M⊕ | ${massj} M♃`).attr("y", remInPixels * 2);
            }
            if (!hasRadius) {
                planetGroup.append("text")
                    .attr("x", x)
                    .attr("y", y + remInPixels * 0.15)
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
        if (count < 4) {
            applyGridLabelLayout(planetGroup, system, index);
            count++;
        }
    }
    else
        applyGridLabelLayout(planetGroup, system, index);
}
function occupyGridForPlanetDisk(cx, cy, r, planetGroup, system, planet) {
    const minX = cx - r;
    const maxX = cx + r;
    const minY = cy - r;
    const maxY = cy + r;
    const startCol = Math.floor(minX / grid.colWidth);
    const endCol = Math.ceil(maxX / grid.colWidth);
    const startRow = Math.floor(minY / grid.rowHeight);
    const endRow = Math.ceil(maxY / grid.rowHeight);
    let markedAny = false;
    for (let col = startCol; col <= endCol; col++) {
        for (let row = startRow; row <= endRow; row++) {
            const cellX = col * grid.colWidth + grid.colWidth / 2;
            const cellY = row * grid.rowHeight + grid.rowHeight / 2;
            const dx = cellX - cx;
            const dy = cellY - cy;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist <= r) {
                grid.occupied.add(`${col},${row}`);
                markedAny = true;
                if (testingLevel >= 11)
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
    planetGroup.append("line")
        .attr("x1", 0)
        .attr("y1", offsetY - systemHeight / 2 + lablePlSysSpace / 2)
        .attr("x2", svgWidth)
        .attr("y2", offsetY - systemHeight / 2 + lablePlSysSpace / 2)
        .attr("stroke", borderLineColor)
        .attr("stroke-width", borderThickness)
        .lower();
    if (testingLevel >= 1)
        planetGroup.append("line")
            .attr("x1", 0)
            .attr("y1", offsetY)
            .attr("x2", svgWidth)
            .attr("y2", offsetY)
            .attr("stroke", "white")
            .attr("stroke-width", 0.5)
            .lower();
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
    const borderThickness = 0.2 * remInPixels;
    let defs = svg.select("defs");
    if (defs.empty()) {
        defs = svg.append("defs");
    }
    const gradientId = `end-shadow-gradient-${system.hostname.replace(/\W/g, '')}`;
    const gradient = defs.append("linearGradient")
        .attr("id", gradientId)
        .attr("x1", "0%").attr("x2", "100%")
        .attr("y1", "0%").attr("y2", "0%");
    gradient.append("stop").attr("offset", "0%").attr("stop-color", "rgba(18,18,18,   0)");
    gradient.append("stop").attr("offset", "20%").attr("stop-color", "rgba(18,18,18,  0.3)");
    gradient.append("stop").attr("offset", "100%").attr("stop-color", "rgba(18,18,18, 1)");
    planetGroup.append("rect")
        .attr("id", "end-shadow-rect")
        .attr("x", shadowStartX + zeroX / 2)
        .attr("y", -systemHeight / 2 + lablePlSysSpace / 2 - borderThickness)
        .attr("width", svgWidth - shadowStartX - zeroX / 2 + borderThickness)
        .attr("height", systemHeight + borderThickness * 2)
        .attr("fill", `url(#${gradientId})`);
}
function maximumOrder() {
    const logTicks = [0.001, 0.01, 0.1, 1, 10, 100, 1000, 10000];
    let maxPlanetDistance = 0;
    let countOfSystem = 0;
    let maxPlanetDistanceInSystem = 0;
    allSystems.forEach(system => {
        countOfSystem++;
        maxPlanetDistanceInSystem = 0;
        if (Array.isArray(system.exoplanets)) {
            for (const planet of system.exoplanets) {
                maxPlanetDistance = Math.max(maxPlanetDistance, planet.plOrbsmax);
                maxPlanetDistanceInSystem = Math.max(maxPlanetDistanceInSystem, planet.plOrbsmax);
            }
        }
    });
    const roundedUpTick = logTicks.find(tick => tick >= maxPlanetDistance);
    const checkbox = document.querySelector("label.dynamicScalingAxis input[type='checkbox']");
    maxTickOrder = checkbox.checked ? Math.log10(roundedUpTick) : 4;
}
function setEndRange() {
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
            endRange = width - margin.right - 2 * remInPixels * correctionFactor;
            break;
    }
}
function calculateSystemsPosition() {
    let cumulativeY = 0;
    const maxSysHeight = maxPlanetRadius * 2 + lablePlSysSpace + topBottomPlSysSpace;
    for (const system of allSystems) {
        let maxPlanetRadiusInSystem = 0;
        let baseSysHeight = clearBaseSysHeight + lablePlSysSpace + topBottomPlSysSpace;
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
            }
        }
        offsets.push({
            offsetY: cumulativeY,
            height: baseSysHeight,
            maxPlanetRadiusInSystem: Math.min(maxPlanetRadiusInSystem, maxPlanetRadius),
        });
        cumulativeY += baseSysHeight + spacing;
    }
}
function formatTick(tick) {
    if (tick < 1) {
        return tick.toString();
    }
    return d3.format(",.0f")(tick).replace(/,/g, ' ');
}
function getCorrectionFactorForWidth(input, domain, range, exponent) {
    const correctionScale = d3.scaleLinear()
        .domain(domain)
        .range(range);
    correctionFactor = correctionScale(input);
}
function logingDataVisualization() {
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
    stRad: 0.10045,
    stMass: 0.0009546,
    stMet: null,
    stMetratio: "",
    stLum: 0,
    stAge: 4.6,
    syDist: 0,
    stLumSunAbsol: 0,
    habitablZone: null,
    exoplanets: [
        { id: 1, hostname: "Jupiter", plLetter: "Io", plRade: 0.286, plRadJ: 0.0255, plMasse: 0.015, plMassJ: 0.000047, plOrbsmax: 0.0028 },
        { id: 2, hostname: "Jupiter", plLetter: "Europa", plRade: 0.245, plRadJ: 0.0218, plMasse: 0.008, plMassJ: 0.000025, plOrbsmax: 0.0045 },
        { id: 3, hostname: "Jupiter", plLetter: "Ganymede", plRade: 0.413, plRadJ: 0.037, plMasse: 0.025, plMassJ: 0.000079, plOrbsmax: 0.0072 },
        { id: 4, hostname: "Jupiter", plLetter: "Callisto", plRade: 0.378, plRadJ: 0.0339, plMasse: 0.018, plMassJ: 0.000057, plOrbsmax: 0.0126 }
    ]
};
const earthSystem = {
    hostname: "Earth",
    hdName: "",
    hipName: "",
    ticId: "",
    gaiaId: "",
    stSpectype: "",
    stTeff: 0,
    stRad: 0.009155,
    stMass: 0.000003003,
    stMet: null,
    stMetratio: "",
    stLum: 0,
    stAge: 4.543,
    syDist: 0,
    stLumSunAbsol: 0,
    habitablZone: null,
    exoplanets: [
        { id: 1, hostname: "Earth", plLetter: "Moon", plRade: 0.273, plRadJ: 0.0245, plMasse: 0.0123, plMassJ: 0.000039, plOrbsmax: 0.00257 }
    ]
};
function applyGridLabelLayout(planetGroup, system, systemindex) {
    const labelsGroup = planetGroup.selectAll(".planet-label-group");
    const labels = labelsGroup.nodes();
    const plSysParameters = {
        systemHeight: offsets[systemindex].height,
        maxPlRadius: offsets[systemindex].maxPlanetRadiusInSystem,
    };
    const { order, center } = getCenterOutwardIndices(labels.length);
    const labelNodeCenter = labels[center];
    const labelCenter = d3.select(labelNodeCenter);
    grid.centralPlanetX = parseFloat(labelCenter.attr("data-xp"));
    order.forEach(i => {
        var _a, _b, _c, _d, _e, _f;
        const labelNode = labels[i];
        const labelSVGGEl = d3.select(labelNode);
        const label = labelSVGGEl.node();
        leftRightPlanet = i === center ? 0 : i >= center ? 1 : -1;
        const xPlanet = parseFloat(labelSVGGEl.attr("data-xp"));
        const yLabel = parseFloat(labelSVGGEl.attr("data-yl"));
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
        planetDescColor = "#E4E4E4";
        preferRight = !((_d = system.exoplanets) === null || _d === void 0 ? void 0 : _d[i].plOrbsmax) || +((_e = system.exoplanets) === null || _e === void 0 ? void 0 : _e[i].plOrbsmax) <= 0.001;
        if (preferRight) {
            planetDescColor = starRadiusPx > newX || newX - bbox.width / 2 < starRadiusPx * 1.4 ? "#2C2C2C" : planetDescColor;
        }
        if (cell.isMoved) {
            labelSVGGEl
                .attr("transform", `translate(${newX}, ${newY})`)
                .attr("data-xp", newX)
                .attr("data-yl", newY)
                .attr("fill", planetDescColor);
        }
        if (cell.isMoved) {
            newY = cell.newY < 0 + remInPixels / 2 ? cell.newY + remInPixels * 0.3 : cell.newY - remInPixels * 0.3;
            const leftRightLabel = newX < xPlanet ? -1 : newX > xPlanet ? 1 : 0;
            const offcetXLineEndpoint = leftRightLabel === 1 || preferRight
                ? -remInPixels / 3
                : leftRightLabel === -1
                    ? remInPixels / 3
                    : 0;
            const isLow = newY + bbox.height / 2 + remInPixels >= 0;
            const YLineEndpoint = isLow ? newY - remInPixels / 2 : newY + bbox.height / 2 + remInPixels * 0.5;
            const XLineEndpoint = isLow ? newX + offcetXLineEndpoint : newX;
            const planetX = xPlanet;
            const planetY = 0;
            const labelX = XLineEndpoint;
            const labelY = YLineEndpoint;
            const dx = planetX - labelX;
            const dy = planetY - labelY;
            const vectorLength = Math.sqrt(dx * dx + dy * dy);
            const planetRadius = Math.min(((((_f = system.exoplanets) === null || _f === void 0 ? void 0 : _f[i].plRade) || 1) * remInPixels / 2), maxPlanetRadius);
            const lineOffset = planetRadius + 0.3 * remInPixels;
            const endX = planetX - (dx / vectorLength) * lineOffset;
            const endY = planetY - (dy / vectorLength) * lineOffset;
            let pointerLineColor = "yellow";
            if (preferRight)
                pointerLineColor = starRadiusPx > xPlanet ? "rgba(39, 54, 222, 0.5)" : pointerLineColor;
            planetGroup.append("line")
                .attr("x1", labelX)
                .attr("y1", labelY)
                .attr("x2", endX)
                .attr("y2", endY)
                .attr("stroke", testingLevel >= 30 ? "red" : pointerLineColor)
                .attr("stroke-width", 1);
        }
        if (testingLevel >= 110)
            markOccupiedCellsAroundLabel(xPlanet, yLabel, labelSVGGEl.node(), "#FFFF80");
    });
}
function findFreeCellNear(xAbs, yAbs, label, labels, planetIndex, leftRightPlanet, planetData, plSysParams) {
    const bbox = label.getBBox();
    const zeroX1 = zeroX + bbox.width / 2;
    preferRight = !(planetData === null || planetData === void 0 ? void 0 : planetData.plOrbsmax) || +planetData.plOrbsmax <= 0.001;
    if (leftRightPlanet === 0) {
        let xAbsPreferRight = xAbs;
        let isMoved = false;
        if (preferRight) {
            xAbsPreferRight += zeroX1;
            isMoved = true;
        }
        return occupyGridAt(xAbsPreferRight, yAbs, label, "rgba(0, 255, 0, 1)", isMoved);
    }
    const planetCount = labels.length;
    const systemHeight = plSysParams.systemHeight;
    const absX = xAbs;
    const absY = yAbs;
    if (!preferRight && absY >= 0 && !overlapsAnyOther(label, absX, absY)) {
        return occupyGridAt(absX, absY, label, "rgba(0, 0, 255, 1)", false);
    }
    return searchFreeSpace();
    function searchFreeSpace() {
        const isFirstOrLast = planetIndex === 0 || planetIndex === planetCount - 1;
        const isSecondOrPenultimate = planetIndex === 1 || planetIndex === planetCount - 2;
        const dxStep = leftRightPlanet === 1 ? grid.colWidth : -grid.colWidth;
        const dxLimit = leftRightPlanet === 1 ? svgWidth : -svgWidth;
        const planetGroupY1 = -systemHeight / 2 + lablePlSysSpace / 2;
        const planetGroupY2 = +systemHeight / 2 + lablePlSysSpace / 2;
        if (preferRight) {
            let dx = 0;
            while (true) {
                const testX = zeroX1 + dx;
                const testY = absY;
                if (testX + bbox.width / 2 > svgWidth)
                    break;
                if (!overlapsAnyOther(label, testX, testY)) {
                    return occupyGridAt(testX, testY, label, "white", true);
                }
                dx += grid.colWidth;
            }
            return null;
        }
        const logTypePlanet = 1;
        if (!isFirstOrLast && !isSecondOrPenultimate) {
            const dyStart = 0;
            const dyLimit = planetGroupY2;
            const newAbsY = absY - remInPixels / 2;
            for (let dx = 0; leftRightPlanet === 1 ? dx <= dxLimit : dx >= dxLimit; dx += dxStep) {
                const testX = absX + dx;
                if (testX - bbox.width / 2 < zeroX1 || testX + bbox.width / 2 > svgWidth)
                    continue;
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`  absX(${Math.floor(absX)}) + dx(${Math.floor(dx)}) = testX(${Math.floor(testX)}) ><= ` +
                        `dxLimit(${Math.floor(dxLimit)}) + dxStep(${Math.floor(dxStep)})`);
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`нижче: newAbsY(${Math.floor(newAbsY)}) + dy(${Math.floor(dyStart)}) + bbox.height(${Math.floor(bbox.height)} / 2)` +
                        ` == ${Math.floor(newAbsY + dyStart + bbox.height / 2)} <= dyLimit(${Math.floor(dyLimit)})`);
                for (let dy = dyStart; newAbsY + dy + bbox.height / 2 <= dyLimit; dy += grid.rowHeight) {
                    const testY1 = newAbsY + dy;
                    if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`нижче: newAbsY(${Math.floor(newAbsY)}) + dy(${Math.floor(dyStart)}) + bbox.height(${Math.floor(bbox.height)} / 2)` +
                            ` == ${Math.floor(newAbsY + dy + bbox.height / 2)} <= dyLimit(${Math.floor(dyLimit)})| ${Math.floor(testY1)}`);
                    if (!overlapsAnyOther(label, testX, testY1)) {
                        return occupyGridAt(testX, testY1, label, "white", true);
                    }
                }
                if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`вище: newAbsY(${Math.floor(-newAbsY)}) - dy(${Math.floor(dyStart)}) == ${Math.floor(-newAbsY - dyStart)} >= ` +
                        `planetGroupY1(${Math.floor(planetGroupY1)})`);
                for (let dy = dyStart; -newAbsY - dy >= planetGroupY1; dy += grid.rowHeight) {
                    let testY = Math.max(-newAbsY - dy, planetGroupY1);
                    if (testingLevel >= 1 && logTypePlanet === 0 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`вище: newAbsY(${Math.floor(-newAbsY)}) - dy(${Math.floor(dy)}) = ${Math.floor(-newAbsY - dy)} >= ` +
                            `planetGroupY1(${Math.floor(planetGroupY1)})| ${Math.floor(testY)}`);
                    if (!overlapsAnyOther(label, testX, testY)) {
                        return occupyGridAt(testX, testY, label, "white", true);
                    }
                }
            }
        }
        else if (isSecondOrPenultimate) {
            const yStartTop = -absY + bbox.height;
            const yStartBottom = absY + bbox.height - Math.max(planetData.plRade, remInPixels);
            let testX;
            const tryAbove = () => {
                if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`вище: absY(${Math.floor(-absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartTop(${Math.floor(yStartTop)}) ` +
                        `- dy(${Math.floor(0)}) == ${Math.floor(yStartTop - 0)} >= planetGroupY1(${Math.floor(planetGroupY1)})`);
                for (let dy = 0; yStartTop - dy >= planetGroupY1; dy += grid.rowHeight) {
                    const testY = yStartTop - dy;
                    if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                        console.log(`вище: absY(${Math.floor(-absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartTop(${Math.floor(yStartTop)}) ` +
                            `- dy(${Math.floor(dy)}) == ${Math.floor(yStartTop - dy)} >= ` +
                            `planetGroupY1(${Math.floor(planetGroupY1)})| ${Math.floor(testY)}`);
                    if (testY > 0)
                        continue;
                    if (!overlapsAnyOther(label, testX, testY)) {
                        return occupyGridAt(testX, testY, label, "yellow", true);
                    }
                }
            };
            const tryBelow = () => {
                if (testingLevel >= 1 && logTypePlanet === 1 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`нижче: absY(${Math.floor(absY)}) + bbox.height(${Math.floor(bbox.height)}) = yStartBottom(${Math.floor(yStartBottom)}) + ` +
                        `dy(${Math.floor(grid.rowHeight)}) == ${Math.floor(yStartBottom + grid.rowHeight)} <= planetGroupY2(${Math.floor(planetGroupY2)})`);
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
                    : tryBelow;
                const second = first === tryAbove ? tryBelow : tryAbove;
                const result1 = first();
                if (result1)
                    return result1;
                const result2 = second();
                if (result2)
                    return result2;
            }
        }
        else if (isFirstOrLast) {
            const startDy = 0 + lablePlSysSpace / 4;
            const dyStep = grid.rowHeight;
            const dxDirection = leftRightPlanet === 1 ? 1 : -1;
            const dxOffset = (remInPixels + dxDirection) * 5 + planetData.plRade / 2;
            for (let dy = 0;; dy += dyStep) {
                const testY1 = startDy + dy;
                const testY2 = testY1 + bbox.height;
                if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`етап №1: testY2(${Math.floor(testY2)}) > planetGroupY2(${Math.floor(planetGroupY2)}) == ${testY2 > planetGroupY2}` +
                        `. testY1(${Math.floor(testY1)}). dxOffset(${Math.floor(dxOffset)})`);
                if (testY2 > planetGroupY2)
                    break;
                for (let dx = 0; dx <= svgWidth; dx += dxStep) {
                    let testX = absX + dx + dxOffset;
                    if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter)) {
                        console.log(` testX(${Math.floor(testX)}) = absX(${Math.floor(absX)}) + (dx(${Math.floor(dx)}) + ` +
                            `dxOffset(${Math.floor(dxOffset)}) ) * dxDirection(${Math.floor(dxDirection)})`);
                        console.log(` testX(${Math.floor(testX)}) - bbox.width/2(${Math.floor(bbox.width / 2)}) < zeroX1(${Math.floor(zeroX1)})` +
                            ` == ${testX < zeroX1}`);
                    }
                    if (testX < zeroX1) {
                        testX += remInPixels;
                        if (testX < zeroX1)
                            break;
                    }
                    if (!overlapsAnyOther(label, testX, testY1)) {
                        return occupyGridAt(testX, testY1, label, "orange", true);
                    }
                }
            }
            for (let dy = dyStep;; dy += dyStep) {
                const testY1 = startDy - dy;
                if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter))
                    console.log(`етап №2: testY1(${Math.floor(testY1)}) < planetGroupY2(${Math.floor(planetGroupY2)}) == ${testY1 > planetGroupY2}` +
                        `. dxOffset(${Math.floor(dxOffset)})`);
                if (testY1 < planetGroupY1)
                    break;
                for (let dx = 0; dx <= svgWidth; dx += dxStep) {
                    let testX = absX + dx + dxOffset;
                    if (testingLevel >= 1 && logTypePlanet === 2 && allowedPlLetters.includes(planetData.plLetter)) {
                        console.log(` testX(${Math.floor(testX)}) - bbox.width/2(${Math.floor(bbox.width / 2)}) < zeroX1(${Math.floor(zeroX1)})` +
                            ` == ${testX < zeroX1}`);
                    }
                    if (testX < zeroX1) {
                        testX += remInPixels;
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
    function overlapsAnyOther(label, testX, testY) {
        const labelSpacing = remInPixels * 0.3;
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
        const absXL = testX - bbox.width / 2;
        const absYL = testY - remInPixels;
        const startCol = Math.floor(absXL / grid.colWidth);
        const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
        const startRow = Math.floor(absYL / grid.rowHeight);
        const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
        for (let c = startCol; c <= endCol; c++) {
            for (let r = startRow; r <= endRow; r++) {
                const key = `${c},${r}`;
                if (grid.occupied.has(key)) {
                    return true;
                }
            }
        }
        d3.select(label).attr("checked", "1");
        let testIndex = 0;
        return labels.some(other => {
            testIndex++;
            if (other === label)
                return false;
            const isChecked = parseFloat(other.getAttribute("checked") || "0");
            if (testingLevel >= 1 && allowedPlLetters.includes(planetData.plLetter))
                console.log(`   📐 label[${testIndex}] isChecked === ${isChecked}`);
            if (isChecked === 0)
                return false;
            const otherBBox = other.getBBox();
            const otherX = parseFloat(other.getAttribute("data-xp") || "0");
            const otherY = parseFloat(other.getAttribute("data-yl") || "0");
            const otherBox = {
                x1: otherX,
                y1: otherY,
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
            const isOverlaps = !(currentBox.x1 > otherBox.x2 || currentBox.x2 < otherBox.x1 || currentBox.y1 > otherBox.y2 || currentBox.y2 < otherBox.y1);
            return isOverlaps;
        });
    }
}
function occupyGridAt(x, y, label, color, isMoved) {
    const bbox = label.getBBox();
    const absXL = x - bbox.width / 2;
    const absYL = y - remInPixels;
    const startCol = Math.floor(absXL / grid.colWidth);
    const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
    const startRow = Math.floor(absYL / grid.rowHeight);
    const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
    for (let c = startCol; c <= endCol; c++) {
        for (let r = startRow; r <= endRow; r++) {
            const key = `${c},${r}`;
            grid.occupied.add(key);
            if (testingLevel >= 1)
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
    const newX = x;
    const newY = y;
    return { newX, newY, isMoved };
}
function markOccupiedCellsAroundLabel(newX, newY, label, color) {
    const bbox = label.getBBox();
    const absXL = newX - bbox.width / 2;
    const absYL = newY - remInPixels;
    const startCol = Math.floor(absXL / grid.colWidth);
    const endCol = Math.floor((absXL + bbox.width) / grid.colWidth);
    const startRow = Math.floor(absYL / grid.rowHeight);
    const endRow = Math.ceil((absYL + bbox.height - grid.rowHeight) / grid.rowHeight);
    for (let c = startCol; c <= endCol; c++) {
        for (let r = startRow; r <= endRow; r++) {
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
function drawGridOverlay(svg, offsetY, systemHeight) {
    const gridGroup = svg.append("g").attr("class", "grid-overlay");
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
function getTranslateX(transform) {
    if (!transform)
        return 0;
    const match = transform.match(/translate\(([-\d.]+),/);
    return match ? parseFloat(match[1]) : 0;
}
function getCenterOutwardIndices(length) {
    const center = Math.floor((length - 1) / 2);
    const order = [center];
    for (let offset = 1; order.length < length; offset++) {
        const right = center + offset;
        const left = center - offset;
        if (right < length) {
            order.push(right);
        }
        if (left >= 0) {
            order.push(left);
        }
    }
    return { order: order, center: center };
}
//# sourceMappingURL=planetarySystemVisualization.js.map
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
//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
const formHandler = __importStar(require("./formHandler"));
//import { serializeForm  } from "./formHandler";
//import { createSpinnerHTML } from "./formHandler";
const behavior_1 = require("./behavior");
document.addEventListener("DOMContentLoaded", () => {
    initialize();
});
function initialize() {
    console.log("FUNCTION: switchTable");
    const checkbox = document.querySelector("label.switchTableType input[type='checkbox']");
    if (!checkbox)
        return;
    checkbox.addEventListener("change", () => __awaiter(this, void 0, void 0, function* () {
        switchTable(checkbox);
    }));
}
;
function switchTable(checkbox) {
    var _a;
    const container = document.getElementById("twoPlanetarySystemTable");
    if (!container)
        return;
    const form = document.querySelector('.topMenuFiltersCatalogs');
    if (!form)
        return;
    console.log(form);
    let pageNumber = '1';
    const currentPageElement = document.querySelector(".paginationBodyBlock .currentPage");
    if (currentPageElement) {
        pageNumber = ((_a = currentPageElement.textContent) === null || _a === void 0 ? void 0 : _a.trim()) || '1';
    }
    const showGrouped = checkbox.checked;
    const url = showGrouped
        ? "/Planetology/PlanetarySystem/GetFlatTable"
        : "/Planetology/PlanetarySystem/GetGroupedTable";
    submitFormAndUpdatePartialSwitchTable(form, url, container, showGrouped, pageNumber);
    const fieldsCheckBoksBlock = document.querySelector('.fieldsCheckBoksBlock');
    if (showGrouped)
        (0, behavior_1.hideShowBlockDownAnime)(fieldsCheckBoksBlock, true, 2);
    else
        (0, behavior_1.hideShowBlockDownAnime)(fieldsCheckBoksBlock, false, 2);
}
;
function submitFormAndUpdatePartialSwitchTable(form_1, url_1, container_1, showGrouped_1) {
    return __awaiter(this, arguments, void 0, function* (form, url, container, showGrouped, pageNumber = '1') {
        console.log(`FUNCTOIN: submitFormAndUpdatePartial (SwitchTable)_(form: ${form}, url: ${url}, container: ${container}, pageNumber: ${pageNumber}!!!`);
        let tableBody = document.getElementById('catalogTableBody');
        // 1. Collect form data into JSON
        const json = formHandler.serializeForm(form, pageNumber);
        try {
            // 2. Show spinner
            console.log(`Adding spinnerHTML.`);
            // Remove the old spinner, if it exists
            const existingSpinnerTr = document.getElementById("spinnerTr");
            if (existingSpinnerTr) {
                existingSpinnerTr.remove();
            }
            if (tableBody) {
                tableBody.insertAdjacentHTML('afterbegin', formHandler.createSpinnerHTML());
            }
            // 3. Send request      TypeError: Failed to execute 'fetch' on: HEAD method cannot have body.
            const response = yield fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(json),
            });
            //const response = await fetch(url);
            // 4. Check for success
            if (!response.ok)
                throw new Error(`HTTP error! Failed to load table. Status: ${response.status}`);
            // 5. Receive partial HTML
            const html = yield response.text();
            // 6. Insert it into the DOM
            container.innerHTML = showGrouped
                ? `<div id="planetsSystemTableContainer" class="hideShowColumnTable p-3">${html}</div>`
                : `<div id="planetsInGroupsSystemTableContainer" class="p-3">${html}</div>`;
        }
        catch (error) {
            container.innerHTML = "<div class='text-danger p-3'>Error loading data</div>";
            console.error('Error submitting form or updating partial:', error);
        }
        finally {
            const spinnerTr = document.getElementById("spinnerTr");
            if (spinnerTr) {
                spinnerTr.remove();
            }
        }
    });
}
//# sourceMappingURL=switchTableType.js.map
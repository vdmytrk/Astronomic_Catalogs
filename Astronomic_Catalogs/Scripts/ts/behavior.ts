//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
import { DeviceUtils } from './deviceUtils';

export function initialize(): void {
    setupShowTextBlockToggles();
    setupShowFiltersToggles();
    handleHabitableZoneDependency();
    handlePlanetSyzeDependency();
    selectAllPlanetSize();
    syncCheckboxWithSelectedOptions();
    applyDeviceResponsiveUI();
}



/////////////////////////////////////////////
// Hide/Show block
/////////////////////////////////////////////
function setupShowTextBlockToggles(): void {
    document.addEventListener("click", (e) => {
        const target = e.target as HTMLElement;
        if (target.classList.contains("showTextBlockBtn")) {
            const container = target.closest("div");
            if (!container) return;


            const outerContainer = container.parentElement;
            if (!outerContainer) return;

            const hiddenBlock = outerContainer.querySelector(".hiddenBlock") as HTMLElement | null;
            if (!hiddenBlock) return;

            const isCollapsed = hiddenBlock.classList.toggle("collapsed");
            target.textContent = isCollapsed ? "Show" : "Hide";
        };
    });
}

export function setupShowFiltersToggles(): void {
    document.addEventListener("click", (e) => {
        console.log("FUNCTION: setupShowFiltersToggles");
        const target = e.target as HTMLElement;
        const isVisualization = sessionStorage.getItem("isSysVisualization") === "true";
        const filtersBlock = document.querySelector(".allTopMenuFilters") as HTMLElement | null;
        const filtersBtn = document.querySelector(".hideFilters") as HTMLElement | null;

        if (!filtersBlock || !filtersBtn) return;

        const toggleFilters = () => {
            const isCollapsed = filtersBlock.classList.toggle("collapsed");
            if (filtersBtn) {
                filtersBtn.textContent = isCollapsed ? "Show filters" : "Hide filters";
            }
        };

        const toggleVisualization = () => {
            console.log(`🔁 setupShowFiltersToggles: isVisualization = ${isVisualization}`);
            filtersBtn.classList.toggle("d-none", !isVisualization);
            filtersBlock.classList.toggle("collapsed", !isVisualization);

            const isCollapsed = filtersBlock.classList.contains("collapsed");
            console.log(`isCollapsed = ${isCollapsed}`);

            target.textContent = isCollapsed
                ? "HIDE PLANETARY SYSTEMS VISUALIZATION"
                : "SHOW PLANETARY SYSTEMS VISUALIZATION";
        };

        if (target.classList.contains("hideFilters")) {
            toggleFilters();
        } else if (target.classList.contains("platetarySystemVisualization")) {
            toggleVisualization();
        }
    });
}

export function hideShowBlockDownAnime(target: HTMLElement, show: boolean, delay: number = 0): void {
    if (!target) return;
    const toggle = () => target.classList.toggle("collapsed", !show);
    delay > 0 ? setTimeout(toggle, delay) : toggle();
}

function handlePlanetSyzeDependency(): void {
    const planetSyzeCheckbox = document.querySelector<HTMLInputElement>('.chb-PlanetWithSize');
    const choosePlanetSizes = document.querySelectorAll<HTMLElement>('.chooseWhichPlanetSizes');
    const planetSizeSelects = document.querySelectorAll<HTMLSelectElement>('.planetTypeEnabled');
    const planetTypeCheckboxes = document.querySelectorAll<HTMLInputElement>('input[type="checkbox"][name="PlanetType"]');

    if (!planetSyzeCheckbox || (planetSizeSelects.length === 0 &&  choosePlanetSizes.length === 0 && planetTypeCheckboxes.length === 0)) return;

    const updateChoosePlanetSizesAccessibility = () => {
        const isChecked = planetSyzeCheckbox.checked;

        choosePlanetSizes.forEach(block => {
            block.classList.toggle('disabled-opacity', !isChecked);
        });

        planetSizeSelects.forEach(select => {
            select.disabled = !isChecked;

            if (!isChecked) {
                select.selectedIndex = -1;

                const options = select.querySelectorAll<HTMLOptionElement>('option');
                options.forEach(option => option.classList.remove('selected-fix'));
            }
        });

        if (!isChecked) {
            planetTypeCheckboxes.forEach(checkbox => {
                checkbox.checked = false;
            });
        } else {
            planetTypeCheckboxes.forEach(checkbox => {
                checkbox.checked = true;
            });
        }
    };

    updateChoosePlanetSizesAccessibility();

    planetSyzeCheckbox.addEventListener('change', updateChoosePlanetSizesAccessibility);
}


function handleHabitableZoneDependency(): void {
    const habitableZoneCheckbox = document.querySelector<HTMLInputElement>('.chb-habitableZonePlanets');
    const terrestrialCheckbox = document.querySelector<HTMLInputElement>('.chb-terrestrialHabitableZonePlanets');

    if (!habitableZoneCheckbox || !terrestrialCheckbox) return;

    const updateTerrestrialCheckboxState = () => {
        if (habitableZoneCheckbox.checked) {
            terrestrialCheckbox.disabled = false;
            terrestrialCheckbox.classList.remove('disabled-opacity');
        } else {
            terrestrialCheckbox.disabled = true;
            terrestrialCheckbox.checked = false;
            terrestrialCheckbox.classList.add('disabled-opacity');
        }
    };

    updateTerrestrialCheckboxState();

    habitableZoneCheckbox.addEventListener('change', updateTerrestrialCheckboxState);
}


function selectAllPlanetSize(): void {
    const planetSyzeCheckbox = document.querySelector<HTMLInputElement>('.chb-PlanetAllSize');
    const planetSizeSelect = document.querySelector<HTMLSelectElement>('.planetTypeEnabled');

    if (!planetSyzeCheckbox || !planetSizeSelect) return;

    const updateChoosePlanetSizesAccessibility = () => {
        const options = planetSizeSelect.querySelectorAll<HTMLOptionElement>('option');
        if (planetSyzeCheckbox.checked) {
            options.forEach(option => {
                option.classList.add('selected-fix');
                option.selected = true;
            });
        } else {
            options.forEach(option => {
                option.classList.remove('selected-fix')
                option.selected = false;
            });
        }
    };

    updateChoosePlanetSizesAccessibility();

    planetSyzeCheckbox.addEventListener('change', updateChoosePlanetSizesAccessibility);
}

function syncCheckboxWithSelectedOptions(): void {
    const planetSyzeCheckbox = document.querySelector<HTMLInputElement>('.chb-PlanetAllSize');
    const planetSizeSelect = document.querySelector<HTMLSelectElement>('.planetTypeEnabled');

    if (!planetSyzeCheckbox || !planetSizeSelect) return;

    const checkIfAllSelected = () => {
        const options = planetSizeSelect.querySelectorAll<HTMLOptionElement>('option');
        const allSelected = Array.from(options).every(option => option.selected);
        planetSyzeCheckbox.checked = allSelected;
    };

    planetSizeSelect.addEventListener('mouseup', () => {
        setTimeout(checkIfAllSelected, 0);
    });
}


function applyDeviceResponsiveUI(): void {
    const applyResponsiveChanges = () => {
        const deviceType = DeviceUtils.getDeviceType();      // 'mobile' | 'tablet' | 'desktop' | 'unknown'
        const platform = DeviceUtils.getDevicePlatform();    // 'windows' | 'macos' | 'android' | 'ios' | 'linux' | 'unknown'
        console.log(` ⚠️ 📐 DEVICE CHARACTERISTICS: deviceType: ${deviceType}, platform: ${platform}`);


        const tableSelectBlocks = document.querySelectorAll<HTMLElement>('.tableSelect');
        const multipleSelectBlocks = document.querySelectorAll<HTMLElement>('.multipleSelect');

        if (deviceType === 'desktop' && platform != 'Android' && platform != 'MacOS')
            tableSelectBlocks.forEach(e => e.remove());
        else
            multipleSelectBlocks.forEach(e => e.remove());
    };
    
    window.addEventListener("DOMContentLoaded", applyResponsiveChanges);
}
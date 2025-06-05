//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
export function initialize(): void {
    setupShowTextBlockToggles();
    setupShowFiltersToggles();
    handleHabitableZoneDependency();
    handlePlanetSyzeDependency();
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

function setupShowFiltersToggles(): void {
    document.addEventListener("click", (e) => {
        const target = e.target as HTMLElement;
        if (target.classList.contains("hideFilters")) {
            const filtersBlock = document.querySelector(".allTopMenuFilters") as HTMLElement | null;
            if (!filtersBlock) return;

            const isCollapsed = filtersBlock.classList.toggle("collapsed");
            target.textContent = isCollapsed ? "Show filters" : "Hide filters";
        };
    });
}

export function hideShowBlockDownAnime(target: HTMLElement, show: boolean, delay: number = 0): void {
    if (!target) return;

    setTimeout(() => {
        if (show)
            target.classList.remove("collapsed");
        else
            target.classList.add("collapsed");
    }, delay);
}

function handlePlanetSyzeDependency(): void {
    const planetSyzeCheckbox = document.querySelector<HTMLInputElement>('.chb-PlanetWithSize');
    const planetSizeSelect = document.querySelector<HTMLSelectElement>('#planetTypeSelect');
    const choosePlanetSizes = document.querySelector<HTMLElement>('.chooseWhichPlanetSizes');

    if (!planetSyzeCheckbox || !planetSizeSelect || !choosePlanetSizes) return;

    const updateChoosePlanetSizesAccessibility = () => {
        if (planetSyzeCheckbox.checked) {
            planetSizeSelect.disabled = false;
            choosePlanetSizes.classList.remove('disabled-opacity');
        } else {
            planetSizeSelect.disabled = true;
            planetSizeSelect.selectedIndex = -1;
            const options = planetSizeSelect.querySelectorAll<HTMLOptionElement>('option');
            options.forEach(option => option.classList.remove('selected-fix'));
            choosePlanetSizes.classList.add('disabled-opacity');
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
        } else {
            terrestrialCheckbox.disabled = true;
            terrestrialCheckbox.checked = false;
        }
    };

    updateTerrestrialCheckboxState();

    habitableZoneCheckbox.addEventListener('change', updateTerrestrialCheckboxState);
}

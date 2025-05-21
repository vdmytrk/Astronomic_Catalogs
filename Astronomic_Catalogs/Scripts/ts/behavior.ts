//////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////// 
export function initialize(): void {
    setupShowTextBlockToggles();
    setupShowFiltersToggles();
    handleHabitableZoneDependency();
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

    // Початковий стан
    updateTerrestrialCheckboxState();

    // Обробник зміни
    habitableZoneCheckbox.addEventListener('change', updateTerrestrialCheckboxState);
}

"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////////////
document.addEventListener('DOMContentLoaded', () => {
    const charts = document.querySelectorAll('.planetary-system-chart');
    charts.forEach(chartElement => {
        const systemData = JSON.parse(chartElement.getAttribute('data-system') || '{}');
        renderSystemChart(chartElement, systemData);
    });
});
function renderSystemChart(container, system) {
    //const width = 1000;
    //const height = 80;
    //const margin = { top: 20, right: 20, bottom: 20, left: 60 };
    //const svg = d3.select(container)
    //    .append("svg")
    //    .attr("width", width)
    //    .attr("height", height);
    //// Логарифмічна шкала відстані в AU
    //const x = d3.scaleLog()
    //    .domain([0.001, 100])
    //    .range([margin.left, width - margin.right]);
    //// Назва системи
    //svg.append("text")
    //    .attr("x", margin.left)
    //    .attr("y", 15)
    //    .text(system.Hostname)
    //    .attr("class", "system-name");
    //const planetGroup = svg.append("g")
    //    .attr("transform", `translate(0, ${height / 2})`);
    //for (const planet of system.Exoplanets) {
    //    const distance = +planet.PlOrbsmax || 0.01; // Мінімум 0.01 AU
    //    const radius = 6; // постійний розмір кружечка
    //    // Планета
    //    planetGroup.append("circle")
    //        .attr("cx", x(distance))
    //        .attr("cy", 0)
    //        .attr("r", radius)
    //        .attr("fill", "red");
    //    // Підпис
    //    const label = planet.PlLetter || "";
    //    planetGroup.append("text")
    //        .attr("x", x(distance))
    //        .attr("y", 20)
    //        .attr("text-anchor", "middle")
    //        .attr("class", "planet-label")
    //        .text(label);
    //}
    //// Вертикальні лінії (лог-шкала)
    //const ticks = [0.001, 0.01, 0.1, 1, 10, 100];
    //for (const tick of ticks) {
    //    const xPos = x(tick);
    //    svg.append("line")
    //        .attr("x1", xPos)
    //        .attr("y1", margin.top)
    //        .attr("x2", xPos)
    //        .attr("y2", height - margin.bottom)
    //        .attr("stroke", "#888")
    //        .attr("stroke-width", tick === 1 || tick === 10 ? 1.5 : 0.5)
    //        .attr("stroke-dasharray", tick < 1 ? "2,2" : "none");
    //    svg.append("text")
    //        .attr("x", xPos)
    //        .attr("y", height - 2)
    //        .attr("text-anchor", "middle")
    //        .attr("font-size", "10px")
    //        .text(`${tick} AU`);
    //}
}
//# sourceMappingURL=planetarySystemVisualization.js.map
document.addEventListener("DOMContentLoaded", () => {

    const toggleBtn = document.getElementById("toggleFilters");
    const panel = document.getElementById("filterPanel");

    if (!toggleBtn || !panel) return;

    let isCollapsed = false;

    panel.style.overflow = "hidden";
    panel.style.transition = "max-height 0.35s ease, opacity 0.25s ease";
    panel.style.maxHeight = panel.scrollHeight + "px";
    panel.style.opacity = "1";

    toggleBtn.addEventListener("click", function () {
        isCollapsed = !isCollapsed;

        if (isCollapsed) {
            panel.style.maxHeight = "0";
            panel.style.opacity = "0";
            toggleBtn.innerText = "Expand";
        } else {
            panel.style.maxHeight = panel.scrollHeight + "px";
            panel.style.opacity = "1";
            toggleBtn.innerText = "Collapse";
        }
    });

    window.onDatePresetChange = function (select) {
        if (select.value !== "Custom") {
            select.form.submit();
        }
    };
});
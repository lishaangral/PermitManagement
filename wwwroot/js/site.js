// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", () => {

    /* TOAST */

    const toastEl = document.querySelector(".toast");
    if (toastEl) {
        setTimeout(() => {
            toastEl.classList.remove("show");
        }, 3500);
    }

    const root = document.documentElement;

    /* SIDEBAR COLLAPSE (HAMBURGER ONLY) */

    const sidebarToggle = document.getElementById("sidebarToggle");

    sidebarToggle?.addEventListener("click", () => {
        root.classList.toggle("sidebar-collapsed");
        localStorage.setItem(
            "sidebar-collapsed",
            root.classList.contains("sidebar-collapsed")
        );
    });

    /* PARENT EXPAND / COLLAPSE (ARROW ONLY) */

    document.querySelectorAll(".parent-tab .arrow-col").forEach(arrow => {

        arrow.addEventListener("click", (e) => {
            e.stopPropagation();
            e.preventDefault();

            if (root.classList.contains("sidebar-collapsed")) return;

            const group = arrow.closest(".sidebar-group");
            if (!group) return;

            const controller = group.dataset.firstChildController;
            const action = group.dataset.firstChildAction;
            if (!controller || !action) return;

            const key = `sidebar-group-${controller}-${action}`;

            group.classList.add("user-toggled");
            group.classList.toggle("expanded");
            if (group.classList.contains("expanded")) {
                localStorage.setItem(key, "user");
            } else {
                localStorage.removeItem(key);
            }

        });
    });

    /* PARENT ICON / TEXT → NAVIGATE TO FIRST CHILD) */

    document.querySelectorAll(".parent-tab .icon-col, .parent-tab .text-col")
        .forEach(el => {

            el.addEventListener("click", (e) => {
                e.stopPropagation();
                e.preventDefault();

                const group = el.closest(".sidebar-group");
                if (!group) return;

                const controller = group.dataset.firstChildController;
                const action = group.dataset.firstChildAction;
                if (!controller || !action) return;

                window.location.href = `/${controller}/${action}`;
            });
        });

    document.querySelectorAll('.navbar-collapse a, .navbar-collapse button')
        .forEach(el => {
            el.addEventListener('click', () => {
                document.querySelector('.navbar-collapse')
                    ?.classList.remove('show');
            });
        });

});


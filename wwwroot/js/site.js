// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
    const toastEl = document.querySelector(".toast");
    if (toastEl) {
        setTimeout(() => {
            toastEl.classList.remove("show");
        }, 3500);
    }
});

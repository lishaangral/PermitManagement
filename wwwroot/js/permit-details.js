document.addEventListener("DOMContentLoaded", () => {

    window.openPreview = function (url) {
        const img = document.getElementById("modalPreviewImage");
        img.src = url;

        const modal = new bootstrap.Modal(
            document.getElementById("imagePreviewModal")
        );

        modal.show();
    };

});

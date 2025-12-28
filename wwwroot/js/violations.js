const selected = [];
document.addEventListener("DOMContentLoaded", () => {

    /* ELEMENT REFERENCES */

    const searchBox = document.getElementById("violationSearch");
    const listItems = document.querySelectorAll(".violation-item");
    const tabs = document.querySelectorAll(".filter-tab");

    const selectedBox = document.getElementById("selectedViolations");
    const emptyText = document.getElementById("emptySelected");
    const detailsSection = document.getElementById("detailsSection");
    const detailsCards = document.getElementById("detailsCards");

    if (!searchBox || listItems.length === 0) {
        console.error("Violations DOM not loaded correctly");
        return;
    }

    /* FILTER STATE */

    let activeCategories = new Set();
    let activeSeverities = new Set();

    /* SEARCH */

    searchBox.addEventListener("input", applyFilters);

    /* FILTER TABS */

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            const type = tab.dataset.type;
            const value = tab.dataset.value;

            if (type === "all") {
                activeCategories.clear();
                activeSeverities.clear();
                tabs.forEach(t => t.classList.remove("active"));
                tab.classList.add("active");
            } else {
                document.querySelector('[data-type="all"]').classList.remove("active");
                tab.classList.toggle("active");

                const set = type === "category" ? activeCategories : activeSeverities;
                set.has(value) ? set.delete(value) : set.add(value);

                if (activeCategories.size === 0 && activeSeverities.size === 0) {
                    document.querySelector('[data-type="all"]').classList.add("active");
                }
            }

            applyFilters();
        });
    });

    /* APPLY FILTERS */

    function applyFilters() {
        const term = searchBox.value.toLowerCase();

        listItems.forEach(item => {
            const name = item.dataset.name;
            const category = item.dataset.category;
            const severity = item.dataset.severity;

            const matchSearch = name.includes(term);
            const matchCategory = activeCategories.size === 0 || activeCategories.has(category);
            const matchSeverity = activeSeverities.size === 0 || activeSeverities.has(severity);

            item.style.display =
                matchSearch && matchCategory && matchSeverity
                    ? "block"
                    : "none";
        });
    }

    /* SELECTION */

    document.querySelectorAll(".add-violation").forEach(btn => {
        btn.addEventListener("click", () => {

            const id = Number(btn.dataset.id);
            if (selected.some(v => v.id === id)) return;

            selected.push({
                id,
                name: btn.dataset.name,
                files: new DataTransfer()
            });

            const item = btn.closest(".violation-item");
            item.classList.add("disabled");

            btn.disabled = true;
            btn.classList.add("disabled");

            renderSelected();
        });
    });

    function renderSelected() {
        selectedBox.innerHTML = "";

        if (selected.length === 0) {
            emptyText.style.display = "block";
            detailsSection.classList.add("d-none");
            return;
        }

        emptyText.style.display = "none";

        selected.forEach(v => {
            selectedBox.innerHTML += `
                <div class="selected-chip">
                    ${v.name}
                    <button type="button" class="chip" onclick="removeViolation(${v.id})">✕</button>
                </div>
            `;
        });

        selectedBox.innerHTML += `
            <button type="button" class="btn btn-orange w-100 mt-2" onclick="showDetails()">
                Add Details
            </button>
        `;
    }

    window.removeViolation = function (id) {
        const idx = selected.findIndex(v => v.id === id);
        if (idx === -1) return;

        selected.splice(idx, 1);

        const btn = document.querySelector(`.add-violation[data-id="${id}"]`);
        if (btn) {
            btn.disabled = false;
            btn.classList.remove("disabled");
            const item = btn.closest(".violation-item");
            item.classList.remove("disabled");
        }

        const card = document.getElementById(`detail-${id}`);
        if (card) card.remove();

        renderSelected();
    };

    window.showDetails = function () {
        detailsCards.innerHTML = "";

        selected.forEach((v, i) => {
            detailsCards.innerHTML += `
            <div class="card mb-3" id="detail-${v.id}">
                <div class="card-body">
                    <h6>${v.name}</h6>

                    <!-- REQUIRED FOR MODEL BINDING -->
                    <input type="hidden" name="Violations.Index" value="${i}" />

                    <input type="hidden"
                           name="Violations[${i}].ViolationId"
                           value="${v.id}" />

                    <label>Remarks</label>
                    <textarea name="Violations[${i}].Remarks"
                              class="form-control mb-2"></textarea>

                    <label>Action Taken</label>
                    <textarea name="Violations[${i}].ActionTaken"
                              class="form-control mb-2"></textarea>

                    <label>Images</label>
                    <input type="file"
                           name="Violations[${i}].Images"
                           class="form-control"
                           multiple
                           accept=".jpg,.jpeg,.png,.webp"
                           onchange="handleFiles(event, ${i})"/>

                    <small class="text-muted">
                        Max 5 images · 5MB each
                    </small>

                    <div id="preview-${i}" class="image-preview mt-2"></div>
                </div>
            </div>
        `;
        });

        detailsSection.classList.remove("d-none");
        detailsSection.scrollIntoView({ behavior: "smooth" });
    };

    window.handleFiles = function (e, index) {
        const input = e.target;
        const files = Array.from(input.files);
        const v = selected[index];
        v.hasInvalidFile = false;

        for (let file of files) {

            if (file.size > 5 * 1024 * 1024) {
                showToast(`${file.name} exceeds 5 MB`);
                v.hasInvalidFile = true;
                continue;
            }

            if (v.files.items.length >= 5) {
                showToast("Maximum 5 images allowed per violation.");
                break;
            }

            v.files.items.add(file);
        }

        input.files = v.files.files;
        renderPreview(index);
    };

    function renderPreview(index) {
        const preview = document.getElementById(`preview-${index}`);
        preview.innerHTML = "";

        const v = selected[index];

        Array.from(v.files.files).forEach((file, i) => {
            const url = URL.createObjectURL(file);

            preview.innerHTML += `
            <div class="image-thumb">
                <img src="${url}" onclick="openPreview('${url}')"/>
                <button type="button"
                        class="remove-img"
                        onclick="removeImage(${index}, ${i})">×</button>
            </div>
        `;
        });
    }

    window.removeImage = function (violationIndex, imageIndex) {
        const v = selected[violationIndex];
        const dt = new DataTransfer();

        Array.from(v.files.files).forEach((file, i) => {
            if (i !== imageIndex) dt.items.add(file);
        });

        v.files = dt;

        const input = document.querySelector(
            `#detail-${v.id} input[type="file"]`
        );
        input.files = dt.files;

        renderPreview(violationIndex);
    };

    window.openPreview = function (url) {
        document.getElementById("modalPreviewImage").src = url;
        new bootstrap.Modal(document.getElementById("imagePreviewModal")).show();
    };

    function showToast(message, type = "error") {
        const toast = document.createElement("div");
        toast.className = `toast align-items-center text-bg-${type === "error" ? "danger" : "success"} border-0 show`;
        toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
        </div>
    `;
        document.querySelector(".toast-container")?.appendChild(toast);
        setTimeout(() => toast.remove(), 4000);
    }

    const searchBtn = document.getElementById("searchBtn");
    const clearBtn = document.getElementById("clearSearchBtn");

    /* Prevent ENTER from submitting form */
    searchBox.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            e.preventDefault();
            applyFilters();
        }
    });

    /* Search button */
    searchBtn.addEventListener("click", () => {
        applyFilters();
    });

    /* Clear button */
    clearBtn.addEventListener("click", () => {
        searchBox.value = "";

        activeCategories.clear();
        activeSeverities.clear();

        tabs.forEach(t => t.classList.remove("active"));
        document.querySelector('[data-type="all"]')?.classList.add("active");

        applyFilters();
    });
});


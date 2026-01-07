document.addEventListener("DOMContentLoaded", function () {

    const canvas = document.getElementById("reportChart");
    if (!canvas) return;

    const labels = JSON.parse(canvas.dataset.labels || "[]");
    const permits = JSON.parse(canvas.dataset.permits || "[]");
    const violations = JSON.parse(canvas.dataset.violations || "[]");

    new Chart(canvas, {
        type: "bar",
        data: {
            labels: labels,
            datasets: [
                {
                    label: "Permits",
                    data: permits,
                    backgroundColor: "#213875"
                },
                {
                    label: "Violations",
                    data: violations,
                    backgroundColor: "#f37021"
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,   
            resizeDelay: 200,             
            plugins: {
                legend: {
                    position: "bottom"
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            }
        }
    });
});

document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".reports-nav").classList.add("active");

    // Ensure Major Violations is active when the page loads
    $(".major-violations").addClass("active");

    // Fetch and render Major Violations report immediately
    fetchData("MajorViolations");
});

$(document).ready(function () {
    var hiddenColleges = new Set(); // Store hidden colleges
    var collegeColors = {
        "CBAA": "#FFD700", // Yellow
        "CCJE": "#FF69B4", // Pink
        "CEAT": "#228B22", // Green
        "CICS": "#808080", // Gray
        "CLAC": "#FFFFFF", // White
        "COED": "#0000FF", // Blue
        "COS": "#FF0000", // Red
        "CTHM": "#800080"  // Purple
    };

    function fetchData(violationType) {
        console.log("Fetching data for:", violationType);

        $.ajax({
            type: "POST",
            url: "/Reports/GetCollegeReports",
            data: JSON.stringify({ violationType: violationType }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Server Response:", response);

                if (!response || response.error) {
                    console.error("Invalid response:", response);
                    alert("Error loading chart data.");
                    return;
                }

                if (!Array.isArray(response.labels) || !Array.isArray(response.violationNumbers)) {
                    console.error("Unexpected data format:", response);
                    return;
                }

                updateChart(response.labels, response.violationNumbers, violationType, response.totalViolations);
                generateLegend(response.labels);
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", status, error);
                alert("Failed to load chart data.");
            }
        });
    }

    function updateChart(labels, data, violationType, totalViolations) {
        $("#total-violations").text("Total Violations: " + totalViolations);

        var ctx = document.getElementById("reportsChart").getContext("2d");

        if (window.reportsChart instanceof Chart) {
            window.reportsChart.destroy();
        }

        window.reportsChart = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [{
                    label: violationType + " by College",
                    backgroundColor: labels.map(college => collegeColors[college] || "gray"),
                    borderColor: "black",
                    borderWidth: 1,
                    data: data
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    title: {
                        display: true,
                        text: violationType + " by College"
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: "Number of Violations"
                        }
                    },
                    x: {
                        title: {
                            display: true,
                            text: "College"
                        }
                    }
                }
            }
        });
    }

    function generateLegend(labels) {
        let legendContainer = $("#chartLegend");
        legendContainer.empty();

        labels.forEach(college => {
            let color = collegeColors[college] || "gray";

            let legendItem = $(`<button class="legend-item" data-college="${college}" style="background-color: ${color}; border: 1px solid black; padding: 5px 10px; margin: 2px; color: black;">${college}</button>`);

            legendItem.click(function () {
                let selectedCollege = $(this).data("college");

                if (hiddenColleges.has(selectedCollege)) {
                    hiddenColleges.delete(selectedCollege);
                    $(this).css("opacity", "1");
                } else {
                    hiddenColleges.add(selectedCollege);
                    $(this).css("opacity", "0.5");
                }

                filterChartData();
            });

            legendContainer.append(legendItem);
        });
    }

    function filterChartData() {
        let chart = window.reportsChart;
        if (!chart) return;

        chart.data.datasets.forEach(dataset => {
            dataset.data = dataset.data.map((value, index) =>
                hiddenColleges.has(chart.data.labels[index]) ? 0 : value
            );
        });

        chart.update();
    }

    // When clicking the Reports tab, ensure Major Violations is loaded
    $(".reports-nav").click(function () {
        $(".button-group button").removeClass("active");
        $(".major-violations").addClass("active");
        fetchData("MajorViolations");
    });

    // When clicking any violation button, update the active class and fetch data
    $(".button-group button").click(function () {
        let violationType = $(this).val();
        if (!violationType) return;

        $(".button-group button").removeClass("active");
        $(this).addClass("active");

        fetchData(violationType);
    });

    // Fetch Major Violations by default on page load
    fetchData("MajorViolations");
});

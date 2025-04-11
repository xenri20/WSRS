document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".reports-nav").classList.add("active");

    // Ensure Major Violations is active when the page loads
    $(".major-violations").addClass("active");

    // Fetch and render Major Violations report immediately
    fetchData("MajorViolations");
});

$(document).ready(function () {
    let selectedViolationType = "MajorViolations"; // Store selected violation type
    let hiddenColleges = new Set(); // Track hidden colleges
    let collegeColors = {
        "CBAA": "#FFD700", // Yellow
        "CCJE": "#FF69B4", // Pink
        "CEAT": "#228B22", // Green
        "CICS": "#808080", // Gray
        "CLAC": "#FFFFFF", // White
        "COED": "#0000FF", // Blue
        "COS": "#FF0000", // Red
        "CTHM": "#800080"  // Purple
    };

    document.getElementById("generateReportForm").addEventListener("submit", function (event) {
        event.preventDefault();

        let fileName = document.getElementById("fileName")?.value || "Report";
        let startDate = document.getElementById("reportStartDate")?.value.trim();
        let endDate = document.getElementById("reportEndDate")?.value.trim();

        if (!startDate || !endDate) {
            alert("Please select both start and end dates.");
            return;
        }

        if (new Date(startDate) > new Date(endDate)) {
            alert("Start date cannot be later than end date.");
            return;
        }

        console.log(`Generating report: ${fileName}, Start: ${startDate}, End: ${endDate}`);

        window.location.href = `/Reports/GenerateReport?fileName=${encodeURIComponent(fileName)}&startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`;
    });
    document.addEventListener("DOMContentLoaded", function () {
        const form = document.getElementById("generateReportForm");
        const fileNameInput = document.getElementById("fileName");
        const startDateInput = document.getElementById("reportStartDate");
        const endDateInput = document.getElementById("reportEndDate"); 
        const submitButton = form.querySelector("button");

        function validateForm() {
            let fileNameValid = fileNameInput.value.trim() !== "";
            let startDateValid = startDateInput.value !== "";
            let endDateValid = endDateInput.value !== "";

            submitButton.disabled = !(fileNameValid && startDateValid && endDateValid);
        }

        let today = new Date().toISOString().split("T")[0];
        startDateInput.setAttribute("max", today);
        endDateInput.setAttribute("max", today);

        [fileNameInput, startDateInput, endDateInput].forEach(input => {
            input.addEventListener("input", validateForm);
        });

        form.addEventListener("submit", function (event) {
            event.preventDefault();

            let fileName = fileNameInput.value.trim();
            let startDate = startDateInput.value;
            let endDate = endDateInput.value;

            if (new Date(startDate) > new Date(endDate)) {
                alert("Start date cannot be later than end date.");
                return;
            }

            window.location.href = `/Reports/GenerateReport?fileName=${encodeURIComponent(fileName)}&startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`;
        });

        validateForm();
    });


    function fetchData(violationType, startDate = null, endDate = null) {
        console.log("Fetching Data:", { violationType, startDate, endDate });

        $.ajax({
            type: "POST",
            url: "/Reports/GetCollegeReports",
            data: JSON.stringify({
                violationType: violationType,
                startDate: startDate || null,
                endDate: endDate || null
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Server Response:", response);

                if (!response || response.error) {
                    console.error("Invalid response:", response);
                    alert("Error loading chart data.");
                    return;
                }

                updateChart(response.labels, response.violationNumbers, violationType, response.totalViolations);
                generateLegend(response.labels);
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", xhr.responseText);
                alert("Failed to load chart data.");
            }
        });
    }

    function updateChart(labels, data, violationType, totalViolations) {
        $("#total-violations").text("Total Violations: " + totalViolations);

        let ctx = document.getElementById("reportsChart").getContext("2d");

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
        selectedViolationType = "MajorViolations";
        fetchData(selectedViolationType);
    });

    // Handle violation button clicks
    $(".button-group button").click(function () {
        let violationType = $(this).val();
        if (!violationType) return;

        selectedViolationType = violationType;
        $(".button-group button").removeClass("active");
        $(this).addClass("active");

        let startDate = $("#startDate").val();
        let endDate = $("#endDate").val();

        fetchData(selectedViolationType, startDate, endDate);
    });

    $("#applyFilterBtn").click(function () {
        let startDate = $("#startDate").val();
        let endDate = $("#endDate").val();

        if (!startDate || !endDate) {
            alert("Please select both start and end dates.");
            return;
        }

        let start = new Date(startDate);
        let end = new Date(endDate);

        if (start > end) {
            alert("Start date cannot be later than end date.");
            return;
        }

        fetchData(selectedViolationType, startDate, endDate);
    });

    $("#clearFilterBtn").click(function () {
        $("#startDate").val(""); // Clear start date
        $("#endDate").val("");   // Clear end date
        fetchData(selectedViolationType, null, null); 
    });

  
    fetchData(selectedViolationType);
});

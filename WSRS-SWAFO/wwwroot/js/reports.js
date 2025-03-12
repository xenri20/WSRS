document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".reports-nav").classList.add("active");
    document.querySelector(".major-violations").classList.add("active");
});

$(document).ready(function () {
    var chart; // To store the Chart instance

    // Define a color mapping for colleges
    var collegeColors = {
        "CBAA": "yellow",
        "CCJE": "pink",
        "CEAT": "green",
        "CICS": "gray",
        "CLAC": "white",
        "COED": "blue",
        "COS": "red",
        "CTHM": "purple"
    };

    // Function to fetch data and update the chart
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
        // Destroy existing chart instance if it exists
        if (window.reportsChart instanceof Chart) {
            window.reportsChart.destroy();
        }

        // Create a new chart instance
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
    $("#category-function button").click(function () {
        let violationType = $(this).val();

        if (!violationType) {
            console.error("No violation type found for button!");
            return;
        }

        console.log("Button clicked:", violationType);

        $("#category-function button").removeClass("active");
        $(this).addClass("active");

        fetchData(violationType);
    });

    $(".button-group button").click(function () {
        let violationType = $(this).val();

        if (!violationType) {
            console.error("No violation type found for button!");
            return;
        }

        console.log("Button clicked:", violationType);

        $(".button-group button").removeClass("active");
        $(this).addClass("active");

        fetchData(violationType);
    });

});
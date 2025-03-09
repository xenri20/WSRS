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
        $.ajax({
            type: "POST",
            url: "/Reports/GetCollegeReports",
            data: JSON.stringify({ violationType: violationType }),  // Send the selected violation type to the server
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.error) {
                    console.error(response.error);
                    alert("Error loading chart data.");
                    return;
                }

                // Extract chart labels, data, and total count
                var chartLabels = response.labels;
                var chartData = response.violationNumbers;
                var totalViolations = response.totalViolations;

                // Update the total violation count on the page
                $("#total-violations").text("Total Violations: " + totalViolations);

                // Map college names to their corresponding colors
                var chartColors = chartLabels.map(function (college) {
                    return collegeColors[college] || "gray"; // Default to gray if no color is found
                });

                // If chart exists, destroy it before creating a new one to avoid duplicates
                if (chart) {
                    chart.destroy();
                }

                // Initialize the bar chart
                var ctx = document.getElementById("reportsChart").getContext("2d");
                chart = new Chart(ctx, {
                    type: "bar",
                    data: {
                        labels: chartLabels,
                        datasets: [{
                            label: violationType + " by College",
                            backgroundColor: chartColors,
                            borderColor: "black",
                            borderWidth: 1,
                            data: chartData
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false, // Allow chart to resize properly
                        plugins: {
                            legend: {
                                display: false // Hide legend since bars are labeled
                            },
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
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data:", error);
                alert("Failed to load chart data.");
            }
        });
    }

    // Fetch data for default violation type on page load
    fetchData('MajorViolations');

    // Handle button click events
    $("#category-function button").click(function () {
        let violationType = $(this).val();

        // Remove 'active' class from all buttons and add it to the clicked one
        $("#category-function button").removeClass("active");
        $(this).addClass("active");

        // Fetch new data based on the selected violation type
        fetchData(violationType);
    });
});

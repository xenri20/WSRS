document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".encode-nav").classList.add("active");
});

var btnLinks = document.querySelectorAll("#category-function a")
for (btnLink of btnLinks) {
    btnLink.addEventListener("click", (e) => {
        for (eachCategory of btnLinks) {
            eachCategory.classList.remove("active");
        }
        e.currentTarget.classList.add("active");
    });
}

$(document).ready(function () {
    const hearingDate = $('#hearingDate');
    const offense = $('#offenseNature'); // Dropdown to populate 
    hearingDate.hide();
    offense.attr("disabled", false);
    $('#offenseClassification').change(function () {
        const classification = $(this).val(); // Get selected classification
        offense.removeAttr("disabled", true);
        // Perform AJAX request
        $.ajax({
            url: '/Encode/GetOffenseNature',
            type: 'GET',
            data: { classification: classification }, // Pass classification as data
            dataType: 'json',
            success: function (data) {
                // Clear the dropdown first
                offense.empty();
                offense.append('<option disabled hidden selected>-- Select an offense --</option>');

                // Populate the dropdown with new data
                data.forEach(function (item) {
                    offense.append(`<option value="${item.id}">${item.nature}</option>`);
                });
                if (classification === "0") {
                    hearingDate.children().first().val('');
                    hearingDate.hide(); // Use jQuery to hide
                } else if (classification === "1") {
                    hearingDate.show(); // Use jQuery to show
                } else {
                    hearingDate.hide(); // Default case
                }
            },
            error: function () {
                alert('Failed to retrieve offenses.');
            }
        });
    });
});

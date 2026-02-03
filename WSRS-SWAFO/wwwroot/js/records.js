document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".records-nav").classList.add("active");

    loadDropdowns();
});

function loadDropdowns() {
    const urlParams = new URLSearchParams(window.location.search);

    const college = urlParams.get('college');
    const classification = urlParams.get('classification');
    const statusOfSanction = urlParams.get('statusOfSanction');

    const collegeDropdown = document.getElementById('collegeOptions');
    const classificationDropdown = document.getElementById('classificationOptions');
    const statusOfSanctionDropdown = document.getElementById('statusOptions');

    if (college) collegeDropdown.value = college;
    if (classification) classificationDropdown.value = classification;
    if (statusOfSanction) statusOfSanctionDropdown.value = statusOfSanction;
}

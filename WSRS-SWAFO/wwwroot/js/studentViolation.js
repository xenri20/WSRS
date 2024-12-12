document.addEventListener("DOMContentLoaded", function () {
    document.querySelector(".student-encode").classList.add("active");
});

// Stay
document.querySelector("#create-new").addEventListener("submit", function (e) {
    if (!this.checkValidity()) {
        e.preventDefault();
        return alert("Please fill out all required fields before proceeding.");
    } 
    alert("Student Data Created Successfully!");
    this.style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
});

// STAY
document.querySelector(".btn-select").addEventListener("click", function (e) {
    e.preventDefault(); // Prevent default link behavior

    // Hide the first page and show the record violation section
    $('#first-page').hide();
    $('#record-violation').show();

    // Optionally, call fetchStudentData() here if needed
     //fetchStudentData();
});

if($('.btn-create').is(':visible')){
    document.querySelector("#first-page .btn-create").addEventListener("click", function () {
        //document.querySelector("#first-page").style.display = "none";
        //document.querySelector("#create-new").style.display = "inline-flex";
        $('#first-page').hide();
        $('#create-new').show();
    });
};


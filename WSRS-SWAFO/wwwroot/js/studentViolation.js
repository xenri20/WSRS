document.addEventListener("DOMContentLoaded", function () {
    document.querySelector(".student-encode").classList.add("active");
});

document.querySelector("#create-new").addEventListener("submit", function (e) {
    if (!this.checkValidity()) {
        e.preventDefault();
        return alert("Please fill out all required fields before proceeding.");
    } 
    alert("Student Data Created Successfully!");
    this.style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
});

document.getElementById('create-new').addEventListener('submit', function (event) {
    event.preventDefault();

    var studentNumber = document.querySelector('[name="StudentNumber"]').value;
    var firstName = document.querySelector('[name="FirstName"]').value;
    var lastName = document.querySelector('[name="LastName"]').value;

    var formAction = '/Encode/EncodeStudentViolation';
    formAction += `?studentNumber=${studentNumber}&firstName=${firstName}&lastName=${lastName}`;

    window.location.href = formAction;
});

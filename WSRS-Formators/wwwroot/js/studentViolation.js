document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".student-encode").classList.add("active");
});

document.querySelector("#create-new").addEventListener("submit", (e) => {
    if (!this.checkValidity()) {
        e.preventDefault();
        return alert("Please fill out all required fields before proceeding.");
    } 
    alert("Student Data Created Successfully!");
    this.style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
});
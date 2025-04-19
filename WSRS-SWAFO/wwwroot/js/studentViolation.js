document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".student-encode")?.classList.add("active");

    const createForm = document.querySelector("#create-new");
    if (createForm) {
        createForm.addEventListener("submit", function (e) {
            if (!this.checkValidity()) {
                e.preventDefault();
                alert("Please fill out all required fields before proceeding.");
                return;
            }
            alert("Student Data Created Successfully!");
            this.style.display = "none";
            const violationForm = document.querySelector("#record-violation");
            if (violationForm) {
                violationForm.style.display = "inline-flex";
            }
        });
    }
});
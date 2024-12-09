var btnLinks = document.querySelectorAll("#category-function a")
for (btnLink of btnLinks) {
    btnLink.addEventListener("click", function (e) {
        for (eachCategory of btnLinks) {
            eachCategory.classList.remove("active");
        }
        e.currentTarget.classList.add("active");
    });
}

document.addEventListener("DOMContentLoaded", function () {
    document.querySelector(".encode-nav").classList.add("active");
    document.querySelector(".student-encode").classList.add("active");
    document.querySelector("#first-page").style.display = "none";
    document.querySelector("#create-new").style.display = "inline-flex";
    document.querySelector("#record-violation").style.display = "none";
});

document.querySelectorAll("#preview-records tbody tr").forEach(row => {
    row.addEventListener("click", function () {
        const isSelected = this.classList.contains("selected");
        document.querySelectorAll("#preview-records tbody tr").forEach(r => r.classList.remove("selected")); 
        if (!isSelected) {
            this.classList.add("selected");  
        }
    });
});

document.querySelector("#first-page .btn-next").addEventListener("click", function () {
    if (document.querySelector("#preview-records").classList.contains(".selected")){
        document.querySelector("#first-page").style.display = "none";
        document.querySelector("#create-new").style.display = "inline-flex";
        return;
    }
    alert('Select a student first!')
});

document.querySelector("#create-new .btn-back").addEventListener("click", function () {
    document.querySelector("#first-page").style.display = "flex";
    document.querySelector("#create-new").style.display = "none";
})

document.querySelector("#create-new").addEventListener("submit", function (e) {
    if (!this.checkValidity()) {
        e.preventDefault();
        return alert("Please fill out all required fields before proceeding.");
    } 
    alert("Student Data Created Successfully!");
    this.style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
});

document.querySelector("#record-violation .btn-back").addEventListener("click", function () {
    document.querySelector("#create-new").style.display = "inline-flex";
    document.querySelector("#record-violation").style.display = "none";
})
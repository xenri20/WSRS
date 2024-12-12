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
});
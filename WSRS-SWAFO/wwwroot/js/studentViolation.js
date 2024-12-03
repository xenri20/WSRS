function formInitializer() {
    document.querySelector("#first-page").style.display = "none";
    document.querySelector("#create-new").style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
}

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

document.querySelector("#create-new .btn-next").addEventListener("click", function () {
    document.querySelector("#create-new").style.display = "none";
    document.querySelector("#record-violation").style.display = "inline-flex";
})

document.querySelector("#record-violation .btn-back").addEventListener("click", function () {
    document.querySelector("#create-new").style.display = "inline-flex";
    document.querySelector("#record-violation").style.display = "none";
})
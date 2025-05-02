document.addEventListener('DOMContentLoaded', () => {
    const toastElement = document.getElementById('toast');

    if (toastElement) {
        const toast = bootstrap.Toast.getOrCreateInstance(toastElement);
        toast.show();
    }
});
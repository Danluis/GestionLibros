function mostrarToast(tipo, summary, detail, duration) {
    const clases = {
        success: "text-bg-success",
        error: "text-bg-danger",
        warning: "text-bg-warning"
    };

    const claseColor = clases[tipo] || "text-bg-secondary";

    const toastEl = document.createElement("div");
    toastEl.className = `toast align-items-center ${claseColor} border-0`;
    toastEl.setAttribute("role", "alert");
    toastEl.setAttribute("aria-live", "assertive");
    toastEl.setAttribute("aria-atomic", "true");

    toastEl.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                <strong>${summary}</strong><br />${detail}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

    document.getElementById("toastContainer").appendChild(toastEl);

    const toast = new bootstrap.Toast(toastEl, { delay: duration });
    toast.show();

    toastEl.addEventListener("hidden.bs.toast", () => toastEl.remove());
}
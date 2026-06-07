(function () {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    const purchaseDeletedToastKey = "purchaseDeletedToastMessage";

    const serverMessageTranslations = new Map([
        ["Request failed.", "Запит не виконано."],
        ["Purchase was not found.", "Покупку не знайдено."],
        ["Purchase list was not found.", "Список покупок не знайдено."],
        ["Purchase was removed.", "Покупку видалено."]
    ]);

    function getAntiForgeryToken() {
        return tokenInput ? tokenInput.value : "";
    }

    function translateServerMessage(message) {
        if (!message) {
            return "Сталася невідома помилка.";
        }

        return serverMessageTranslations.get(message) || message;
    }

    function escapeHtml(value) {
        return String(value)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#039;");
    }

    function ensureToastContainer() {
        let container = document.getElementById("purchasesToastContainer");

        if (container) {
            return container;
        }

        container = document.createElement("div");
        container.id = "purchasesToastContainer";
        container.className = "toast-container position-fixed bottom-0 end-0 p-3";
        container.style.zIndex = "1080";

        document.body.appendChild(container);

        return container;
    }

    function getToastHeaderClass(type) {
        switch (type) {
            case "success":
                return "bg-success text-white";
            case "warning":
                return "bg-warning text-dark";
            case "danger":
                return "bg-danger text-white";
            default:
                return "bg-primary text-white";
        }
    }

    function getToastBodyClass(type) {
        switch (type) {
            case "success":
                return "border-success";
            case "warning":
                return "border-warning";
            case "danger":
                return "border-danger";
            default:
                return "border-primary";
        }
    }

    function getToastCloseButtonClass(type) {
        return type === "warning" ? "" : "btn-close-white";
    }

    function getToastIcon(type) {
        switch (type) {
            case "success":
                return "bi-check-circle-fill";
            case "warning":
                return "bi-exclamation-triangle-fill";
            case "danger":
                return "bi-x-circle-fill";
            default:
                return "bi-info-circle-fill";
        }
    }

    function getToastTitle(type) {
        switch (type) {
            case "success":
                return "Успішно";
            case "warning":
                return "Увага";
            case "danger":
                return "Помилка";
            default:
                return "Повідомлення";
        }
    }

    function showAlert(type, message) {
        const container = ensureToastContainer();
        const toast = document.createElement("div");

        toast.className = "toast border-0 shadow";
        toast.setAttribute("role", "alert");
        toast.setAttribute("aria-live", "assertive");
        toast.setAttribute("aria-atomic", "true");

        toast.innerHTML = `
            <div class="toast-header ${getToastHeaderClass(type)}">
                <i class="bi ${getToastIcon(type)} me-2"></i>
                <strong class="me-auto">${getToastTitle(type)}</strong>
                <button type="button"
                        class="btn-close ${getToastCloseButtonClass(type)} ms-2 mb-1"
                        data-bs-dismiss="toast"
                        aria-label="Закрити"></button>
            </div>
            <div class="toast-body bg-white border-start border-4 ${getToastBodyClass(type)}">
                ${escapeHtml(message)}
            </div>`;

        container.appendChild(toast);

        if (window.bootstrap && bootstrap.Toast) {
            const bootstrapToast = new bootstrap.Toast(toast, { delay: 4500 });

            toast.addEventListener("hidden.bs.toast", function () {
                toast.remove();
            });

            bootstrapToast.show();
            return;
        }

        toast.classList.add("show");

        setTimeout(function () {
            toast.remove();
        }, 4500);
    }

    function hideAlert() {
        const oldPageAlert = document.getElementById("purchasesAlert");

        if (oldPageAlert) {
            oldPageAlert.classList.add("d-none");
            oldPageAlert.classList.remove("alert-success", "alert-danger", "alert-warning");
            oldPageAlert.textContent = "";
        }
    }

    function ensureConfirmModal() {
        let modal = document.getElementById("purchasesConfirmModal");

        if (modal) {
            return modal;
        }

        modal = document.createElement("div");
        modal.id = "purchasesConfirmModal";
        modal.className = "modal fade";
        modal.tabIndex = -1;
        modal.setAttribute("aria-hidden", "true");

        modal.innerHTML = `
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content shadow">
                    <div class="modal-header">
                        <h5 class="modal-title">Підтвердження дії</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Закрити"></button>
                    </div>
                    <div class="modal-body">
                        <p class="mb-0" id="purchasesConfirmMessage"></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-outline-secondary" data-confirm-result="false">
                            Скасувати
                        </button>
                        <button type="button" class="btn btn-danger" data-confirm-result="true">
                            Підтвердити
                        </button>
                    </div>
                </div>
            </div>`;

        document.body.appendChild(modal);

        return modal;
    }

    function confirmAction(message, confirmButtonClass) {
        return new Promise(function (resolve) {
            const modalElement = ensureConfirmModal();
            const messageElement = modalElement.querySelector("#purchasesConfirmMessage");
            const confirmButton = modalElement.querySelector('[data-confirm-result="true"]');
            const cancelButton = modalElement.querySelector('[data-confirm-result="false"]');

            let resolved = false;

            messageElement.textContent = message;
            confirmButton.className = `btn ${confirmButtonClass || "btn-danger"}`;

            function cleanup(result) {
                if (resolved) {
                    return;
                }

                resolved = true;

                confirmButton.removeEventListener("click", onConfirm);
                cancelButton.removeEventListener("click", onCancel);
                modalElement.removeEventListener("hidden.bs.modal", onHidden);

                resolve(result);
            }

            function hideModal() {
                if (window.bootstrap && bootstrap.Modal) {
                    bootstrap.Modal.getOrCreateInstance(modalElement).hide();
                    return;
                }

                modalElement.classList.remove("show");
                modalElement.style.display = "none";
            }

            function onConfirm() {
                cleanup(true);
                hideModal();
            }

            function onCancel() {
                cleanup(false);
                hideModal();
            }

            function onHidden() {
                cleanup(false);
            }

            confirmButton.addEventListener("click", onConfirm);
            cancelButton.addEventListener("click", onCancel);
            modalElement.addEventListener("hidden.bs.modal", onHidden);

            if (window.bootstrap && bootstrap.Modal) {
                bootstrap.Modal.getOrCreateInstance(modalElement).show();
                return;
            }

            modalElement.style.display = "block";
            modalElement.classList.add("show");
        });
    }

    function switchToEmptyStateIfNeeded() {
        const remainingCards = $(".purchase-card").length;

        if (remainingCards > 0) {
            return;
        }

        $("#purchasesList").remove();
        $("#toggleAllReceiptsBtn").remove();
        $("nav[aria-label='Пагінація покупок']").remove();
        $("nav[aria-label='Purchases pagination']").remove();

        const emptyStateHtml = `
            <div id="emptyPurchasesState" class="card border-0 shadow-sm">
                <div class="card-body text-center py-5">
                    <i class="bi bi-receipt display-5 text-muted"></i>
                    <h2 class="h5 mt-3">Покупок не знайдено</h2>
                    <p class="text-muted mb-0">
                        Збережіть поточний список продуктів як покупку, щоб побачити його тут.
                    </p>
                </div>
            </div>`;

        $("#purchasesAlert").after(emptyStateHtml);
    }

    async function sendDelete(url) {
        const response = await fetch(url, {
            method: "DELETE",
            headers: {
                "RequestVerificationToken": getAntiForgeryToken()
            }
        });

        const contentType = response.headers.get("content-type");

        const result = contentType && contentType.includes("application/json")
            ? await response.json()
            : null;

        if (!response.ok) {
            const message = result && result.message
                ? result.message
                : "Request failed.";

            throw new Error(translateServerMessage(message));
        }

        return result;
    }

    function setReceiptButtonText(button, expanded) {
        const text = expanded ? "Сховати чек" : "Показати чек";

        $(button)
            .attr("aria-expanded", expanded ? "true" : "false")
            .html(`<i class="bi bi-list-ul"></i> ${text}`);
    }

    function setToggleAllButtonText(expanded) {
        const button = $("#toggleAllReceiptsBtn");

        if (!button.length) {
            return;
        }

        const text = expanded ? "Сховати всі чеки" : "Показати всі чеки";

        button
            .data("expanded", expanded)
            .html(`<i class="bi bi-list-ul"></i> ${text}`);
    }

    function updateToggleAllButtonState() {
        const receipts = $(".receipt-collapse");

        if (!receipts.length) {
            setToggleAllButtonText(false);
            return;
        }

        const allExpanded = receipts.toArray().every(function (receipt) {
            return $(receipt).hasClass("show");
        });

        setToggleAllButtonText(allExpanded);
    }

    $("#toggleAllReceiptsBtn").on("click", function () {
        const button = $(this);
        const shouldExpand = button.data("expanded") !== true;

        $(".receipt-collapse").each(function () {
            const collapseElement = this;

            if (window.bootstrap && bootstrap.Collapse) {
                const collapse = bootstrap.Collapse.getOrCreateInstance(collapseElement, {
                    toggle: false
                });

                if (shouldExpand) {
                    collapse.show();
                } else {
                    collapse.hide();
                }

                return;
            }

            $(collapseElement).toggleClass("show", shouldExpand);
        });

        $(".toggle-receipt-btn").each(function () {
            setReceiptButtonText(this, shouldExpand);
        });

        setToggleAllButtonText(shouldExpand);
    });

    $(document).on("shown.bs.collapse", ".receipt-collapse", function () {
        const receiptId = this.id;
        const button = $(`.toggle-receipt-btn[aria-controls="${receiptId}"]`);

        if (button.length) {
            setReceiptButtonText(button, true);
        }

        updateToggleAllButtonState();
    });

    $(document).on("hidden.bs.collapse", ".receipt-collapse", function () {
        const receiptId = this.id;
        const button = $(`.toggle-receipt-btn[aria-controls="${receiptId}"]`);

        if (button.length) {
            setReceiptButtonText(button, false);
        }

        updateToggleAllButtonState();
    });

    function showPendingPurchaseToastIfNeeded() {
        const message = sessionStorage.getItem(purchaseDeletedToastKey);

        if (!message) {
            return;
        }

        sessionStorage.removeItem(purchaseDeletedToastKey);
        showAlert("success", message);
    }

    $(document).on("click", ".remove-purchase-btn", async function () {
        hideAlert();

        const card = $(this).closest(".purchase-card");
        const purchaseId = card.data("purchase-id");

        const confirmed = await confirmAction(
            "Видалити цю покупку? Цю дію неможливо буде скасувати.",
            "btn-danger"
        );

        if (!confirmed) {
            return;
        }

        const button = $(this);
        button.prop("disabled", true);

        try {
            await sendDelete(`/purchases/${purchaseId}`);

            sessionStorage.setItem(purchaseDeletedToastKey, "Покупку видалено.");
            window.location.reload();
        } catch (error) {
            showAlert("danger", error.message);
            button.prop("disabled", false);
        }
    });

    showPendingPurchaseToastIfNeeded();
})();
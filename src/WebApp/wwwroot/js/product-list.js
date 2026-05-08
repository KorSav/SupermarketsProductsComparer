(function () {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    const serverMessageTranslations = new Map([
        ["Request failed.", "Запит не виконано."],
        ["Amount must be greater than zero.", "Кількість має бути більшою за нуль."],
        ["Product list entry was not found.", "Товар у списку не знайдено."],
        ["Product was not found.", "Товар не знайдено."],
        ["Product list was not found.", "Список товарів не знайдено."],
        ["Product list is empty.", "Список товарів порожній."]
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
        let container = document.getElementById("productListToastContainer");

        if (container) {
            return container;
        }

        container = document.createElement("div");
        container.id = "productListToastContainer";
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
        const oldPageAlert = document.getElementById("productListAlert");

        if (oldPageAlert) {
            oldPageAlert.classList.add("d-none");
            oldPageAlert.classList.remove("alert-success", "alert-danger", "alert-warning");
            oldPageAlert.textContent = "";
        }
    }

    function ensureConfirmModal() {
        let modal = document.getElementById("productListConfirmModal");

        if (modal) {
            return modal;
        }

        modal = document.createElement("div");
        modal.id = "productListConfirmModal";
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
                        <p class="mb-0" id="productListConfirmMessage"></p>
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
            const messageElement = modalElement.querySelector("#productListConfirmMessage");
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

    function parseDecimal(value) {
        if (value === undefined || value === null) {
            return NaN;
        }

        return Number(String(value).replace(",", ".").replace(/[^0-9.-]/g, ""));
    }

    function formatMoney(value) {
        const number = Number(value);

        if (Number.isNaN(number)) {
            return "0,00";
        }

        return number.toFixed(2);
    }

    function formatAmount(value) {
        const number = Number(value);

        if (!Number.isFinite(number)) {
            return "0";
        }

        return number.toFixed(3).replace(/\.?0+$/, "");
    }

    function getFinalMeasureElement(row) {
        return row
            .find(".entry-final-measure, .final-measure, .calculated-amount, .product-final-amount")
            .first();
    }

    function rememberInitialRowValues(row) {
        const amountInput = row.find(".product-amount-input");
        const initialAmount = parseDecimal(amountInput.val());

        if (Number.isFinite(initialAmount) && initialAmount > 0) {
            row.data("initial-entry-amount", initialAmount);
        }

        const finalMeasureElement = getFinalMeasureElement(row);

        if (finalMeasureElement.length) {
            const rawText = finalMeasureElement.text().trim();
            const initialFinalAmount = parseDecimal(rawText);

            if (Number.isFinite(initialFinalAmount) && initialFinalAmount > 0) {
                row.data("initial-final-amount", initialFinalAmount);
            }

            const unitMatch = rawText.match(/[^\d\s.,-]+.*$/);

            if (unitMatch) {
                row.data("final-measure-unit", unitMatch[0].trim());
            }
        }
    }

    function initializeInitialRowValues() {
        $("tr[data-entry-id]").each(function () {
            rememberInitialRowValues($(this));
        });
    }

    function updateFinalMeasureAfterSuccessfulAmountUpdate(row, newAmount) {
        const finalMeasureElement = getFinalMeasureElement(row);

        if (!finalMeasureElement.length) {
            return;
        }

        const initialEntryAmount = Number(row.data("initial-entry-amount"));
        const initialFinalAmount = Number(row.data("initial-final-amount"));
        const finalMeasureUnit = row.data("final-measure-unit") || "";

        if (
            !Number.isFinite(initialEntryAmount) ||
            initialEntryAmount <= 0 ||
            !Number.isFinite(initialFinalAmount) ||
            initialFinalAmount <= 0
        ) {
            return;
        }

        const updatedFinalAmount = initialFinalAmount * newAmount / initialEntryAmount;
        const formattedFinalAmount = formatAmount(updatedFinalAmount);

        finalMeasureElement.text(
            finalMeasureUnit
                ? `${formattedFinalAmount} ${finalMeasureUnit}`
                : formattedFinalAmount
        );
    }

    function getShopHeaderForEntryRow(entryRow) {
        return entryRow.prevAll("tr.table-secondary").first();
    }

    function removeShopHeaderIfShopBecameEmpty(shopHeaderRow) {
        if (!shopHeaderRow || !shopHeaderRow.length) {
            return;
        }

        const productRowsInSameShop = shopHeaderRow.nextUntil("tr.table-secondary", "tr[data-entry-id]");

        if (productRowsInSameShop.length === 0) {
            shopHeaderRow.remove();
        }
    }

    function switchToEmptyState() {
        $("#productListContent").addClass("d-none");
        $("#emptyProductListState").removeClass("d-none");
        $("#productListTotal").text("0,00");
    }

    async function sendJson(url, method, body) {
        const response = await fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": getAntiForgeryToken()
            },
            body: body === undefined ? undefined : JSON.stringify(body)
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

    $(document).on("click", ".update-entry-amount-btn", async function () {
        hideAlert();

        const row = $(this).closest("tr");
        const entryId = row.data("entry-id");
        const input = row.find(".product-amount-input");
        const amount = parseDecimal(input.val());

        if (!Number.isFinite(amount) || amount <= 0) {
            showAlert("warning", "Кількість має бути більшою за нуль.");
            input.trigger("focus");
            return;
        }

        const button = $(this);
        button.prop("disabled", true);

        try {
            const result = await sendJson(
                `/product-list/entries/${entryId}/amount`,
                "PATCH",
                { amount: amount }
            );

            row.find(".entry-total").text(formatMoney(result.entryTotal));
            $("#productListTotal").text(formatMoney(result.listTotal));

            updateFinalMeasureAfterSuccessfulAmountUpdate(row, amount);

            showAlert("success", "Кількість товару оновлено.");
        } catch (error) {
            showAlert("danger", error.message);
        } finally {
            button.prop("disabled", false);
        }
    });

    $(document).on("click", ".remove-entry-btn", async function () {
        hideAlert();

        const row = $(this).closest("tr");
        const entryId = row.data("entry-id");

        const confirmed = await confirmAction(
            "Видалити цей товар із поточного списку?",
            "btn-danger"
        );

        if (!confirmed) {
            return;
        }

        const button = $(this);
        button.prop("disabled", true);

        try {
            const result = await sendJson(
                `/product-list/entries/${entryId}`,
                "DELETE"
            );

            const shopHeaderRow = getShopHeaderForEntryRow(row);

            row.remove();
            removeShopHeaderIfShopBecameEmpty(shopHeaderRow);

            $("#productListTotal").text(formatMoney(result.listTotal));

            if (result.isEmpty) {
                switchToEmptyState();
            } else {
                showAlert("success", "Товар видалено зі списку.");
            }
        } catch (error) {
            showAlert("danger", error.message);
            button.prop("disabled", false);
        }
    });

    $("#storeProductListBtn").on("click", async function () {
        hideAlert();

        const confirmed = await confirmAction(
            "Зберегти поточний список як покупку? Після цього список буде очищено.",
            "btn-success"
        );

        if (!confirmed) {
            return;
        }

        const button = $(this);
        button.prop("disabled", true);

        try {
            const result = await sendJson(
                "/product-list/store",
                "POST"
            );

            showAlert("success", "Покупку збережено.");

            if (result.redirectUrl) {
                window.location.href = result.redirectUrl;
                return;
            }

            switchToEmptyState();
        } catch (error) {
            showAlert("danger", error.message);
            button.prop("disabled", false);
        }
    });

    initializeInitialRowValues();
})();
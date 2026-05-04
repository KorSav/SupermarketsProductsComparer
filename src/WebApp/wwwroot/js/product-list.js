(function () {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    function getAntiForgeryToken() {
        return tokenInput ? tokenInput.value : "";
    }

    function showAlert(type, message) {
        const alert = $("#productListAlert");

        alert
            .removeClass("d-none alert-success alert-danger alert-warning")
            .addClass(`alert-${type}`)
            .text(message);
    }

    function hideAlert() {
        $("#productListAlert")
            .addClass("d-none")
            .removeClass("alert-success alert-danger alert-warning")
            .text("");
    }

    function formatMoney(value) {
        const number = Number(value);

        if (Number.isNaN(number)) {
            return "0.00";
        }

        return number.toFixed(2);
    }

    function switchToEmptyState() {
        $("#productListContent").addClass("d-none");
        $("#emptyProductListState").removeClass("d-none");
        $("#productListTotal").text("0.00");
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

            throw new Error(message);
        }

        return result;
    }

    $(document).on("click", ".update-entry-amount-btn", async function () {
        hideAlert();

        const row = $(this).closest("tr");
        const entryId = row.data("entry-id");
        const input = row.find(".product-amount-input");
        const amount = Number(input.val());

        if (!Number.isFinite(amount) || amount <= 0) {
            showAlert("warning", "Amount must be greater than zero.");
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

            showAlert("success", "Amount was updated.");
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

        const confirmed = confirm("Remove this product from your current list?");

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

            row.remove();

            $("#productListTotal").text(formatMoney(result.listTotal));

            if (result.isEmpty) {
                switchToEmptyState();
            } else {
                showAlert("success", "Product was removed from the list.");
            }
        } catch (error) {
            showAlert("danger", error.message);
            button.prop("disabled", false);
        }
    });

    $("#storeProductListBtn").on("click", async function () {
        hideAlert();

        const confirmed = confirm("Save current product list as a purchase? Current list will be cleared.");

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

            showAlert("success", "Purchase was saved.");

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
})();
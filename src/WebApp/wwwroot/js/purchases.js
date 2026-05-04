(function () {
    const tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');

    function getAntiForgeryToken() {
        return tokenInput ? tokenInput.value : "";
    }

    function showAlert(type, message) {
        const alert = $("#purchasesAlert");

        alert
            .removeClass("d-none alert-success alert-danger alert-warning")
            .addClass(`alert-${type}`)
            .text(message);
    }

    function hideAlert() {
        $("#purchasesAlert")
            .addClass("d-none")
            .removeClass("alert-success alert-danger alert-warning")
            .text("");
    }

    function switchToEmptyStateIfNeeded() {
        const remainingCards = $(".purchase-card").length;

        if (remainingCards > 0) {
            return;
        }

        $("#purchasesList").remove();

        const emptyStateHtml = `
            <div id="emptyPurchasesState" class="card border-0 shadow-sm">
                <div class="card-body text-center py-5">
                    <i class="bi bi-receipt display-5 text-muted"></i>
                    <h2 class="h5 mt-3">No purchases found</h2>
                    <p class="text-muted mb-0">
                        Save your current product list as a purchase to see it here.
                    </p>
                </div>
            </div>`;

        $("nav[aria-label='Purchases pagination']").remove();
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

            throw new Error(message);
        }

        return result;
    }

    $(document).on("click", ".remove-purchase-btn", async function () {
        hideAlert();

        const card = $(this).closest(".purchase-card");
        const purchaseId = card.data("purchase-id");

        const confirmed = confirm("Remove this purchase?");

        if (!confirmed) {
            return;
        }

        const button = $(this);
        button.prop("disabled", true);

        try {
            await sendDelete(`/purchases/${purchaseId}`);

            card.remove();

            showAlert("success", "Purchase was removed.");
            switchToEmptyStateIfNeeded();
        } catch (error) {
            showAlert("danger", error.message);
            button.prop("disabled", false);
        }
    });
})();
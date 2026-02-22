$(document).ready(function () {
    var billingMasterId = $("#BillingMasterId").val();

    // Add new item
    $("#addItemBtn").click(function () {
        var itemName = $("#ItemName").val();
        var price = $("#Price").val();
        var quantity = $("#Quantity").val();

        $.post("/BillingItems/CreateAjax", {
            BillingMasterId: billingMasterId,
            ItemName: itemName,
            Price: price,
            Quantity: quantity
        }).done(function (data) {
            if (data.success) {
                var item = data.item;
                $("#itemsTable tbody").append(`
                    <tr id="itemRow_${item.id}">
                        <td><input type="text" value="${item.itemName}" class="form-control itemName" data-id="${item.id}" /></td>
                        <td><input type="number" value="${item.price}" class="form-control itemPrice" data-id="${item.id}" /></td>
                        <td><input type="number" value="${item.quantity}" class="form-control itemQuantity" data-id="${item.id}" /></td>
                        <td>
                            <button class="btn btn-success btn-sm editItemBtn" data-id="${item.id}">Save</button>
                            <button class="btn btn-danger btn-sm deleteItemBtn" data-id="${item.id}">Delete</button>
                        </td>
                    </tr>
                `);
                $("#ItemName, #Price, #Quantity").val("");
            } else {
                alert("Error adding item.");
            }
        });
    });

    // Edit item
    $(document).on("click", ".editItemBtn", function () {
        var id = $(this).data("id");
        var row = $("#itemRow_" + id);
        var itemName = row.find(".itemName").val();
        var price = row.find(".itemPrice").val();
        var quantity = row.find(".itemQuantity").val();

        $.post("/BillingItems/EditAjax", {
            Id: id,
            ItemName: itemName,
            Price: price,
            Quantity: quantity
        }).done(function (data) {
            if (!data.success) alert("Error updating item.");
        });
    });

    // Delete item
    $(document).on("click", ".deleteItemBtn", function () {
        var id = $(this).data("id");

        $.post("/BillingItems/DeleteAjax", { id: id })
            .done(function (data) {
                if (data.success) {
                    $("#itemRow_" + id).remove();
                } else {
                    alert("Error deleting item.");
                }
            });
    });
});

let productsInList = [];
$(document).ready(function () {
    $('#productSearch').select2({
        theme: "classic",
        ajax:
        {
            delay: 200,
            url: '/Orders/GetProductsByName',
            dataType: 'json',
            type: 'get',
            contentType: "application/json; charset=utf-8",
            data: function (params) {
                return {
                    search: params.term
                };
            },
            processResults: function (data) {
                return {
                    results: data.map((item) => ({
                        id: item.id,
                        text: item.name,
                        price: item.sellingPrice,
                        quantity: item.quantity,
                        totalPrice: item.totalPrice
                    }))
                };
            },
        },
        placeholder: 'Search Product...',
        minimumInputLength: 0,
        templateResult: formatResults
    });
});

function formatResults(data) {

    if (data.loading)
        return data.text;

    var container = $(
        `    <table class="table table-bordered" style="margin:0;" width="100%">
                <thead>
                    <th class="w-50">Name</th>
                    <th class="w-25">Price</th>
                    <th>Quantity</th>
                </thead>
                <tr>
                     <td>
                         <p>${data.text}</p>
                     </td>
                     <td>
                        <p>₹${data.price}</p>
                     </td>
                     <td>
                         <p>${data.quantity}</p>
                     </td>
                 </tr>
              </table>`
    );

    return container;
}

$('#addBtn').on('click', function () {

    const product = $('#productSearch').select2('data')[0];

    const quantity = parseFloat($('#quantityInput').val());
    if (isNaN(quantity) || quantity === null || quantity < 1) {
        alert("Please enter a valid quantity!");
        return;
    }

    var quantityInList = getQuantityFromListById(product.id);

    if (product.quantity < quantity + quantityInList) {
        alert("Invalid quantity");
        return;
    }

    const price = parseFloat(product.price);
    if (quantityInList > 0) {
        let index = productsInList.findIndex((item) => item.id === product.id);
        if (index !== -1) {
            productsInList[index].quantity += quantity;
            productsInList[index].totalPrice = productsInList[index].quantity * price;
        }
    }
    else {

        let listProduct = { pId: createUniqueId(), id: product.id, name: product.text, quantity: quantity, price: price, totalPrice: price * quantity };

        if (listProduct.pId == null) {
            alert("error while adding product");
        }
        else {
            productsInList.push(listProduct);
        }
    }
    showProductsInList();
    updatePrice();
});

function showProductsInList() {
    let no = 0;
    let tbody = $('.table tbody');
    tbody.html("");
    productsInList.forEach((item) => {
        no = no + 1;
        tbody.append(
            $('<tr>').append(
                $('<td>').text(no),
                $('<td>').text(item.name),
                $('<td>').text(item.quantity),
                $('<td>').text('₹' + item.price),
                $('<td>').text('₹' + item.totalPrice),
                $('<td>').append($('<i>').addClass('remove-btn c-pointer fa fa-trash').data("uId", item.pId)),
            )
        );
    });

    $('.remove-btn').on('click', function () {
        const id = $(this).data("uId");
        productsInList = productsInList.filter((item) => item.pId != id);
        showProductsInList();
        updatePrice();
    });
}

function getQuantityFromListById(id) {
    const quantity = productsInList.filter((item) => item.id == id).reduce((acc, item) => { return acc + item.quantity }, 0);
    return quantity;
}

function createUniqueId() {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
}

function updatePrice() {
    const price = productsInList.reduce((acc, item) => { return acc + item.totalPrice }, 0);
    $('#totalPrice').val(price);
    calculateBillPrice();
}

$('#discountInput').on('keyup change', function () {
    const value = $('#discountInput').val();
    if (isValidNumber(value)) {
        const number = parseFloat(value);
        if (value < 0) {
            $('#discountInput').val(0);
        }
        const totalPrice = $('#totalPrice').val();
        if (isValidNumber(totalPrice)) {
            const disCountPercent = (number / parseFloat(totalPrice)) * 100;
            if (disCountPercent > 20) {
                const newDiscount = (0.2) * parseFloat(totalPrice);
                $('#discountInput').val(newDiscount);
                alert("The discount cannot be more than 20%");
            }
        }
        calculateBillPrice();
        correctPaidPriceValue();
    } else {
        $('#discountInput').val('');
    }
});

function isValidNumber(number) {
    return !isNaN(parseFloat(number) && isFinite(number));
}

$('#customerPrice').on('keyup change', correctPaidPriceValue);

function maxDiscountPossible() {
    const percentage = 20;

}

function correctPaidPriceValue() {
    const thisValue = parseFloat($('#customerPrice').val());
    const billValue = parseFloat($('#billPrice').val());

    if (isValidNumber(thisValue) && isValidNumber(billValue) && (thisValue > billValue)) {
        $('#customerPrice').val(billValue);
    }
}

function calculateBillPrice() {
    const billPrice = parseFloat($('#totalPrice').val()) - parseFloat($('#discountInput').val());
    if (billPrice >= 0) {
        $('#billPrice').val(billPrice);
    }
}

$('#saleBtn').on('click', function () {
    const orderModel = PrepareOrderModelToSubmit();
    PostOrder(orderModel);
});

function PrepareOrderModelToSubmit() {
    let products = [];

    productsInList.forEach((item) => {
        products.push({ productId: item.id, quantity: item.quantity });
    });

    let orderModel = { customerName: $("#nameInput").val(), phoneNumber: $("#phoneInput").val(), fullAddress: $("#addInput").val(), emailAddress: $("#emailInput").val(), discount: parseFloat($("#discountInput").val()), employee: $("#empInput").val(), products: products, paidByCustomer: $('#customerPrice').val() };

    return orderModel;
}

function PostOrder(orderModel) {
    $.ajax({
        url: '/Orders/SaveSaleOrder',
        dataType: 'json',
        type: 'post',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(orderModel),
        success: function (result) {
            if (result.success) {
                window.location.href = result.redirectUrl;
            } else {
                showMessageToUser(result.responseText);
            }
        },
        error: function (result) {
            showMessageToUser(result.responseText);
        }
    });
}

function showMessageToUser(errorMessage) {
    if (typeof (errorMessage) === 'string') {
        const div = $('<div>').addClass('mt-2 alert alert-warning alert-dismissible fade show').attr('role', 'alert').text(errorMessage);
        var btn = $('<button>', {
            type: 'button',
            class: 'btn-close',
            'data-bs-dismiss': 'alert',
            'aria-label': 'Close'
        });

        div.append(btn);
        $('#show-error-below').after(div);
    }
}
var obj = @Json.Serialize(Model);


function dateFormatter(data, row, index) {
    const date = new Date(Date.parse(data));
    return date.toDateString();
}
function priceFormatter(data, row, index) {
    return '₹' + data;
}
$('#ordersTable').bootstrapTable({
    columns:
        [{
            field: "orderNumber",
            title: "Order#",
            sortable: true,
            align: 'right'
        }, {
            field: "employee",
            title: "Employee",
            sortable: true,
            align: 'right'
        }, {
            field: "price",
            title: "Price",
            formatter: "priceFormatter",
            sortable: true,
            align: 'right'
        }, {
            field: "discount",
            title: "Discount",
            sortable: true,
            formatter: "priceFormatter",
            align: 'right'
        }, {
            field: "billPrice",
            title: "Bill Price",
            formatter: "priceFormatter",
            sortable: true,
            align: 'right'
        }, {
            field: "paidByCustomer",
            title: "Paid",
            cellStyle: "redCell",
            sortable: true,
            formatter: "priceFormatter",
            align: 'right'
        }, {
            field: "orderDate",
            title: "Order date",
            sortable: true,
            formatter: "dateFormatter",
            align: 'right'
        }],
    data: obj,
    search: true,
    pagination: true,
    sortName: 'orderDate',
    sortOrder: 'desc',
    detailView: true,
    detailFormatter: 'productListFormatter'
});


function productListFormatter(index, row) {
    var tableRows = [];
    console.log(row);
    $.each(row.products, function (i, data) {

        tableRows.push(`<tr>
                        <th scope="row">${i + 1}</th>
                        <td>${data.name}</td>
                        <td>${data.quantity}</td>
                        <td>${data.unitPrice}</td>
                        <td>${data.sellingPrice}</td>
                        <td>${data.totalPrice}</td>
                    </tr>`);

    });
    var rows = tableRows.join('');
    var html = `<div class="row p-3">
                      <div class="col-12 d-flex justify-content-between table-border p-3">
                        <p>Name: <strong>${row.customerName}</strong></p>
                        <p>Phone: <strong>${row.customerPhone}</strong></p>
                        <p>Address: <strong>${row.customerAddress}</strong></p>
                        <p class="text-red" >Pending: <strong>₹${row.billPrice - row.paidByCustomer}</strong></p>
                      </div>
                      <div class="col-12 mt-4 p-3 table-border">
                        <table class="table">
                          <thead>
                            <tr>
                              <th scope="col">#</th>
                              <th scope="col">Name</th>
                              <th scope="col">Quantity</th>
                              <th scope="col">Unit Price</th>
                              <th scope="col">Selling Price</th>
                              <th scope="col">Total</th>
                            </tr>
                          </thead>
                          <tbody>
                            ${rows}
                          </tbody>
                        </table>
                      </div>
                    </div>`;

    console.log(html);
    return html;
}

function redCell() {
    return {
        css: {
            color: 'red'
        }
    }
}
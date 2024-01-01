var obj = @Json.Serialize(Model);

console.log(obj);
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
            field: "orderDate",
            title: "Order date",
            sortable: true,
            formatter: "dateFormatter",
            align: 'right'
        }
        ],
    data: obj,
    search: true,
    pagination: true,
    sortName: 'orderDate',
    sortOrder: 'desc',
    detailView: true,
    detailFormatter

});

function dateFormatter(data, row, index) {
    const date = new Date(Date.parse(data));
    return date.toDateString();
}
function priceFormatter(data, row, index) {
    return '₹' + data;
}
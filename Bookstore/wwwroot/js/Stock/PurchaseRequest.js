var dataTablePurchaseRequest;
$(document).ready(function () {
    loadDataTablePurchaseRequest();
});
function refreshDataTable() {
    dataTablePurchaseRequest.ajax.url('/Stockkeeper/Stock/GetTablePurchaseRequest').load();
}
function loadDataTablePurchaseRequest() {
    dataTablePurchaseRequest = $('#purchaseRequest').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/GetTablePurchaseRequest' },
        "columns": [
            { data: 'productId', "width": "10%" },
            {
                data: 'titleProduct',
                render: function (data, type, row) {
                    return '<a href="/Customer/Home/Details?productId=' + row.productId + '">' + data + '</a>';
                },
                "width": "40%"
            },
            { data: 'totalProduct', "width": "25%" },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <input type="number" required/>
                    </div>`;
                }, "width": "15%"
            },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a onClick="deleteFromPurchaseRequest('${data.productId}')" class="btn btn-warning"><i class="bi bi-check2-square"></i></a>
                    </div>`;
                }, "width": "10%"
            }
        ]
    });
}

function deleteFromPurchaseRequest(productId) {
    $.ajax({
        url: '/Stockkeeper/Stock/SelectProductToPurchase' + "?productId=" + productId,
        type: 'POST',
        data: productId,
        success: function (response) {
            refreshDataTable();
            resolve(response);
        },
        error: function (error) {
            reject(error);
        }
    });
}             
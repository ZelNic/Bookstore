//var dataTablePurchaseRequest;
//$(document).ready(function () {
//    loadDataTable();
//});

//function loadDataTableStock() {
//    dataTableStock = $('#purchaseRequest').DataTable({
//        "ajax": { url: '/Stockkeeper/Stock/GetPurchaseRequest' },
//        "columns": [
//            { data: 'id', "width": "5%" },
//            { data: 'productId', "width": "5%" },
//            {
//                data: 'nameProduct',
//                render: function (data, type, row) {
//                    return '<a href="/Customer/Home/Details?productId=' + row.productId + '">' + data + '</a>';
//                },
//                "width": "40%"
//            },
//            { data: 'count', "width": "20%" },
//            { data: 'shelfNumber', "width": "15%" },
//            {
//                data: null,
//                render: function (data) {
//                    return `<div class="w-100 btn-group" role="group">
//                    <a onClick="editShelfProduct('${data.id}&${data.productId}&${data.nameProduct}&${data.count}&${data.shelfNumber}&${data.isOrder}')" class="btn bg-secondary"><i class="bi bi-pencil-square"></i></a>
//                    </div>`;
//                }, "width": "5%"
//            },
//            {
//                data: null,
//                render: function (data) {

//                    if (data.isOrder == true) {
//                        return `<div class="w-100 btn-group" role="group">
//                    <a onClick="selectProductToPurchase('${data.productId}&${data.isOrder}')" class="btn btn-warning"><i class="bi bi-check2-square"></i></a>
//                    </div>`;
//                    }
//                    else {
//                        return `<div class="w-100 btn-group" role="group">
//                    <a onClick="selectProductToPurchase('${data.productId}&${data.isOrder}')" class="btn btn-dark"><i class="bi bi-dash-square"></i></a>
//                    </div>`;
//                    }

//                }, "width": "5%"
//            },
//            { data: 'totalProduct', "width": "5%" }
//        ]
//    });
//}
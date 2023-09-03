
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#stock').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/getStock' },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'productId', "width": "5%" },
            {
                data: 'nameProduct',
                render: function (data, type, row) {
                    return '<a href="/Customer/Home/Details?productId=' + row.productId + '">' + data + '</a>';
                },
                "width": "40%"
            },
            { data: 'count', "width": "20%" },
            { data: 'shelfNumber', "width": "20%" },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a onClick="editShelf('${data.id}&${data.productId}&${data.nameProduct}&${data.count}&${data.shelfNumber}&${data.isOrder}')" class="btn bg-secondary"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                }, "width": "5%"
            },
            {
                data: null,
                render: function (data) {

                    if (data.isOrder == true) {
                        return `<div class="w-100 btn-group" role="group">
                    <a onClick="selectProductToPurchase('${data.productId}&${data.isOrder}')" class="btn btn-warning"><i class="bi bi-check2-square"></i></a>
                    </div>`;
                    }
                    else {
                        return `<div class="w-100 btn-group" role="group">
                    <a onClick="selectProductToPurchase('${data.productId}&${data.isOrder}')" class="btn btn-dark"><i class="bi bi-dash-square"></i></a>
                    </div>`;
                    }

                }, "width": "5%"
            }
        ]
    });
}

function selectProductToPurchase(productData) {
    var response = productData.split('&');
    var id = response[0];
    var isOrder = response[1];
    $.ajax({
        url: '/Stockkeeper/Stock/SelectProductToPurchase' + "?productId=" + id + "&isOrder" + isOrder,
        type: 'POST',
        data: productData,
        success: function (response) {
            refreshDataTable();
            resolve(response);
        },
        error: function (error) {
            reject(error);
        }
    });
}


function editShelf(productData) {
    console.log(productData)
    var response = productData;
    var values = response.split('&');
    var recordId = values[0];
    var productId = values[1];
    var nameProduct = values[2];
    var count = values[3];
    var shelfNumber = values[4];

    Swal.fire({
        title: nameProduct,
        input: 'range',
        inputLabel: 'Количество книг',
        inputAttributes: {
            min: 1,
            max: count,
            step: 1
        },
        inputValue: 1,
        html: '<input type="number" id="shelfNumber" placeholder="Номер полки" class="swal2-input">',
        showCancelButton: true,
        confirmButtonText: 'Сменить',
        cancelButtonText: 'Отмена',
    }).then(function (result) {
        if (result.isConfirmed) {
            var productCount = result.value;
            var newShelfNumber = document.getElementById('shelfNumber').value;

            return new Promise((resolve, reject) => {
                $.ajax({
                    url: '/Stockkeeper/Stock/ChangeShelfProduct?recordId=' + recordId + "&productCount=" + productCount + "&newShelfNumber=" + newShelfNumber,
                    type: 'POST',
                    success: function (response) {
                        refreshDataTable();
                        resolve(response);
                    },
                    error: function (error) {
                        reject(error);
                    }
                });
            });
        }
    });

}

function refreshDataTable() {
    dataTable.ajax.url('/Stockkeeper/Stock/GetStock').load();
}

function enterIdProduct(url) {
    Swal.fire({
        title: 'Добавить новый товар',
        html: '<input type="number" id="productId" placeholder="Код товара" class="swal2-input">' +
            '<input type="number" id="productCount" placeholder="Количество" class="swal2-input">' +
            '<input type="number" id="shelfNumber" placeholder="Номер полки" class="swal2-input bg">',
        showCancelButton: true,
        confirmButtonText: 'Подтвердить',
        showLoaderOnConfirm: true,

    }).then((result) => {
        if (result.isConfirmed) {
            const productId = document.getElementById('productId').value;
            const productCount = document.getElementById('productCount').value;
            const numberShelf = document.getElementById('shelfNumber').value;

            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url + "&productId=" + productId + "&numberShelf=" + numberShelf + "&productCount=" + productCount,
                    type: 'POST',
                    success: function (response) {
                        resolve(response);
                    },
                    error: function (error) {
                        reject(error);
                    }
                });
            });
        }
        else {
            return;
        }
    }).then((response) => {
        Swal.fire({
            title: 'Товар добавлен',
            text: response,
            icon: 'success'
        });
        refreshDataTable();

    }).catch((error) => {
        Swal.fire({
            title: 'Ошибка при выполнении запроса',
            text: error,
            icon: 'error'
        });
    });
}




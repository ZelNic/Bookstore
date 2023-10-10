
let dataTable;
$(document).ready(function () {
    loadDataTableStock();
});

function loadDataTableStock() {
    dataTable = $('#stock').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/getStock' },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'productId', "width": "5%" },
            {
                data: 'time', "width": "15%"
            },
            {
                data: 'operation', "width": "15%"
            },
            {
                data: 'nameProduct',
                render: function (data, type, row) {
                    return '<a href="/Customer/Home/Details?productId=' + row.productId + '">' + data + '</a>';
                },
                "width": "35%"
            },
            { data: 'count', "width": "5%" },
            { data: 'shelfNumber', "width": "5%" },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a onClick="editShelfProduct('${data.id}&${data.productId}&${data.nameProduct}&${data.count}&${data.shelfNumber}&${data.isOrder}')" class="btn bg-secondary"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                }, "width": "5%"
            },
            {
                data: null,
                render: function (data) {

                    if (data.isOrder == true) {
                        return `<div class="w-100 btn-group" role="group">
                    <a onClick="selectProductToPurchase('${data.productId}')" class="btn btn-warning"><i class="bi bi-check2-square"></i></a>
                    </div>`;
                    }
                    else {
                        return `<div class="w-100 btn-group" role="group">
                    <a onClick="selectProductToPurchase('${data.productId}')" class="btn btn-dark"><i class="bi bi-dash-square"></i></a>
                    </div>`;
                    }

                }, "width": "5%"
            },
            { data: 'totalProduct', "width": "5%" }
        ]
    });
}
function selectProductToPurchase(productId) {
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
function editShelfProduct(productData) {
    let response = productData;
    let values = response.split('&');
    let recordId = values[0];
    let productId = values[1];
    let nameProduct = values[2];
    let count = values[3];
    let shelfNumber = values[4];

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
            let productCount = result.value;
            let newShelfNumber = document.getElementById('shelfNumber').value;

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
function addProductInStock(url) {
    Swal.fire({
        title: 'Добавить новый товар',
        html: '<input type="number" id="productId" placeholder="Код товара" required class="swal2-input">' +
            '<input type="number" id="productCount" placeholder="Количество" required class="swal2-input">' +
            '<input type="number" id="shelfNumber" placeholder="Номер полки" required class="swal2-input bg">',
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
                    text: error.responseJSON.error,
                    icon: 'error'
                });
            });
        } else {
            let timerInterval
            Swal.fire({
                title: 'Отмена операции',
                timer: 500,
                timerProgressBar: true,
                didOpen: () => {
                    Swal.showLoading()
                    const b = Swal.getHtmlContainer().querySelector('b')
                    timerInterval = setInterval(() => {
                        b.textContent = Swal.getTimerLeft()
                    }, 100)
                },
                willClose: () => {
                    clearInterval(timerInterval)
                }
            })
        }
    });
}





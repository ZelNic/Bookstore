
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#stock').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/getStock' },
        "columns": [
            { data: 'productId', "width": "5%" },
            { data: 'nameProduct', "width": "35%" },
            { data: 'count', "width": "20%" },
            { data: 'shelfNumber', "width": "20%" },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                         <a onClick="editShelf('${data.productId}&${data.nameProduct}&${data.count}&${data.shelfNumber}')" class="btn btn-primary mx-1">Сменить полку<i class="bi bi-pencil-square"></i></a>
                        </div>`;
                },
                "width": "20%"
            }
        ]
    });

}

function editShelf(productData) {
    var response = productData;
    var values = response.split('&');
    var productId = values[0];
    var nameProduct = values[1];
    var count = values[2];
    var shelfNumber = values[3];

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
        showCancelButton: true,
        confirmButtonText: 'Сменить',
        cancelButtonText: 'Отмена',
    }).then(function (result) {
        if (result.isConfirmed) {
            var selectedValue = result.value;
            console.log(selectedValue);
            // Дальнейшая обработка выбранного значения
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
        if (response) {
            Swal.fire({
                title: 'Товар добавлен',
                icon: 'success'
            });
            refreshDataTable();
        }
    }).catch((error) => {
        Swal.fire({
            title: 'Ошибка при выполнении запроса',
            text: 'a',
            icon: 'error'
        });
    });
}




//{






//const searchContainer = document.querySelector('.hidden');
//const hide = document.getElementById('hide');

//openSearch.addEventListener('click', function () {
//    searchContainer.classList.remove('hidden');
//    searchContainer.classList.add('visible');
//    hide.classList.add('visible');
//});

//hide.addEventListener('click', function () {
//    searchContainer.classList.remove('visible');
//    searchContainer.classList.add('hidden');
//    hide.classList.remove('visible');
//});








//document.getElementById("myForm").addEventListener("submit", function (event) {
//    event.preventDefault();

//    var title = document.getElementById("searchTitle").value;
//    var id = document.getElementById("searchId").value;
//    var result = findProduct(title,id);
//});

//function findProduct(title, id) {
//    fetch(`/Stockkeeper/Stock/GetProductAsync?nameProduct=${title}&productId=${id}`, { method: 'POST' })
//        .then(response => {
//        })
//        .catch(error => {
//        });
//}

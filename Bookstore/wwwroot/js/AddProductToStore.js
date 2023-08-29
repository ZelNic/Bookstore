
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#stock').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/getStock' },
        "columns": [
            { data: 'productId', "width": "25%" },
            { data: 'nameProduct', "width": "25%" },
            { data: 'count', "width": "25%" },
            { data: 'shelfNumber', "width": "25%" },
            
        ]
    });
}


function EnterIdProduct(url) {
    Swal.fire({
        title: 'Добавить новый товар',
        html: '<input type="number" id="productId" placeholder="Код товара" class="swal2-input">' +
            '<input type="number" id="productCount" placeholder="Количество" class="swal2-input">'+
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
        } else {
            return;
        }
    }).then((response) => {
        if (response) {
            Swal.fire({
                title: 'Товар добавлен',
                icon: 'success'
            });
        }
        else {
            Swal.fire({
                title: 'Ошибка',
                icon: 'error'
            });
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
            //    data: 'id',
            //    "render": function (data) {
            //        return `<div class="w-100 btn-group" role="group">
            //         <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-1"> <i class="bi bi-pencil-square"></i> Edit</a>
            //         <a onClick=Delete('/admin/product/delete/${data}') class="btn btn-danger mx-1"> <i class="bi bi-trash-fill"></i> Delete</a>
            //        </div>`
            //    },
            //    "width": "20%"
            //}





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

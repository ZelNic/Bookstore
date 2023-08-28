function EnterIdProduct(url) {
    Swal.fire({
        title: 'Добавить новый товар',
        html: '<input type="number" id="productCode" placeholder="Код товара" class="swal2-input">' +
            '<input type="number" id="productCount" placeholder="Количество" class="swal2-input">'+
            '<input type="number" id="shelfNumber" placeholder="Номер полки" class="swal2-input bg">',
        showCancelButton: true,
        confirmButtonText: 'Подтвердить',
        showLoaderOnConfirm: true,

    }).then((result) => {
        if (result.isConfirmed) {
            const productCode = document.getElementById('productCode').value;
            const productCount = document.getElementById('productCount').value;
            const numberShelf = document.getElementById('shelfNumber').value;

            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url + "&productId=" + productCode + "&numberShelf=" + numberShelf + "&productCount=" + productCount,
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
    }).catch((error) => {
        Swal.fire({
            title: 'Ошибка при выполнении запроса',
            text: error.message,
            icon: 'error'
        });
    });
}










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

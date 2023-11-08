



function updateOrAddProduct() {

    const formData = new FormData(document.getElementById('dataProduct'));
    const jsonObject = {};

    for (const [key, value] of formData.entries()) {
        jsonObject[key] = value;
    }

    const jsonDataProduct = JSON.stringify(jsonObject);

    $.ajax({
        url: '/Admin/Product/UpdateOrAdd?dataProduct=' + jsonDataProduct,
        type: 'POST',
        success: function (response) {
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Удаление прошло успешно',
                showConfirmButton: false,
                timer: 1500
            })
            window.location.href = '/Admin/Product/index';
        },

        error: function (error) {
            Swal.fire({
                icon: 'error',
                text: error.responseText
            })
        },
    });
}

function deleteProduct(productId) {

    $.ajax({
        url: '/Admin/Product/Delete?productId=' + productId,
        type: 'POST',
        success: function (response) {
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Удаление прошло успешно',
                showConfirmButton: false,
                timer: 1500
            })
        },

        error: function (error) {
            Swal.fire({
                icon: 'error',
                text: error.responseText
            })
        },
    });
}


$(document).ready(function () {
    getWishList();
});


function getWishList() {
    var wishList = document.getElementById("wishList");
    $.ajax({
        url: '/Customer/WishList/GetWishList',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            wishList.innerHTML = `
                <h1>Отложенные товары</h1>
                <div class="row row-cols-2">
                    <div class="container col-10">
                        <div class="row row-cols-2 row-cols-md-6 mb-5">
                        ${generateHTML(response)}
                        </div>
                    </div>
                </div>
            `;
        },
        error: function (error) {
            wishList.innerHTML = `
                <h1>${error.responseJSON.error}</h1>                
            `;
        }
    });
}
function generateHTML(response) {

    var html = "";

    for (var key in response.data) {
        html += '<div class="col-2 mb-3">' +
            '<div class="card card-subtitle h-100 shadow pt-1 border-0">' +
            '<img src="' + response.data[key].image + '" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 50%; height: 150px;" />' +
            '<div class="card-body mt-0 pb-0">' +
            response.data[key].nameProduct +
            '</div><div class="card-body mt-0 pb-0">' +
            response.data[key].author +
            '</div><div class="card-body mt-0 pb-0">' +
            response.data[key].category +
            '</div>' +
            '<div class="card-body mt-0 pb-0">' +
            response.data[key].price +
                '</div>' +
                    '<div class="card card-footer bg-transparent border-0">' +
                       '<form method="post">' +
                            '<button onclick="removeFromWishlist(event,' + response.data[key].productId + ',true)" type="submit" class="btn btn-danger bi bi-x-circle"></button>' +
                            '<button onclick="addToShoppingBasket(event,' + response.data[key].productId + ',true)" type="submit" class="btn btn-success border-0 bi bi-bag-plus"></button>' +
                       '</form>' +
                    '</div>' +
                '</div>' +
            '</div>';
    };

    return html;
}

function addToWishlist(event, id) {
    event.preventDefault();
    $.ajax({
        url: '/Customer/WishList/AddWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            var btnWishList = document.getElementById(`btnWishList_${id}`);
            btnWishList.innerHTML = `<button type="submit" onclick="removeFromWishlist(event,${id})" class="btn btn-outline-danger bi-heart-fill" style="width: 56px; height: 40px;"></button>`;
        },
        error: function (error) {
        }
    });
}

function removeFromWishlist(event, id, isFromWishList = false) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/WishList/RemoveFromWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            if (isFromWishList == false) {
                var btnWishList = document.getElementById(`btnWishList_${id}`);
                btnWishList.innerHTML = `<button type="submit" onclick="addToWishlist(event,${id})" class="btn bi-heart" style="width: 56px; height: 40px;"></button>`;
            }
            getWishList();
        },
        error: function (error) {
            reject(error);
        }
    });
}
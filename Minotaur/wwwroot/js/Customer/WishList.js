
$(document).ready(function () {
    getWishList();
});

var divWishList = document.getElementById("wishList");
var wishList;

function getWishList() {
    $.ajax({
        url: '/Customer/WishList/GetWishList',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            wishList = response.data;
            divWishList.innerHTML = `
                <h1>Отложенные товары</h1>
                <div class="row row-cols-2">
                    <div class="container col-10">
                        <div class="row row-cols-2 row-cols-md-6 mb-5">
                        ${generateHTML(wishList)}
                        </div>
                    </div>
                </div>
            `;
        },
        error: function (error) {
            divWishList.innerHTML = `<h1>${error.responseJSON.error}</h1>`;
        }
    });
}
function generateHTML(wishList) {

    var html = "";

    for (var key in wishList) {
        html +=
            `<div class="mb-3 p-1">
                    <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                        <img src="${wishList[key].image}" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 70%;" />
                        <div class="card-body mt-0 pb-0">
                            <div class="fs-6">
                                ${wishList[key].nameProduct}
                            </div>
                        </div>
                        <div class="card card-footer bg-transparent border-0">
                            <hr />
                            <div class="text-center">
                                ${wishList[key].author}
                                ${wishList[key].category}
                            </div>
                            <hr />
                            <div class="text-center fs-5">
                                ${wishList[key].price} ₽
                            </div>
                            <hr />
                            <div class="mx-auto pb-1">
                                 <button onclick="removeFromWishlist(event, ${wishList[key].productId}, true)" type="submit" class="btn btn-outline-danger border-0 bi bi-x-circle"></button>
                                 <button onclick="addToShoppingBasket(event, ${wishList[key].productId})" type="submit" class="btn btn-outline-success border-0 bi bi-bag-plus"></button>
                            </div>                            
                        </div>
                    </div>
            </div>`;
    };

    return html;
}



function addToWishlist(event, id) {
    event.preventDefault();
    $.ajax({
        url: '/Customer/WishList/AddWishList' + "?newProductId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            var btnWishList = document.getElementById(`btnWishList_${id}`);
            btnWishList.innerHTML = `<button type="submit" onclick="removeFromWishlist(event,${id})" class="btn btn-outline-danger border-0 bi-heart-fill" style="width: 56px; height: 40px;"></button>`;
            getWishList();
        },
        error: function (error) {
        }
    });
}

function removeFromWishlist(event, id, isFromWishlist = false) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/WishList/RemoveFromWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            if (isFromWishlist == false) {
                var btnWishList = document.getElementById(`btnWishList_${id}`);
                btnWishList.innerHTML = `<button type="submit" onclick="addToWishlist(event,${id})" class="btn border-0 bi-heart" style="width: 56px; height: 40px;"></button>`;
            }
            getWishList();
        },
        error: function (error) {
            reject(error);
        }
    });
}
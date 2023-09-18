
$(document).ready(function () {
    getShoppingBasket();
});

var countProduct = 0;
var totalPrice = 0;

function getShoppingBasket() {
    var shoppingBasket = document.getElementById("shoppingBasket");
    var orderingInformation = document.getElementById("orderingInformation");
    $.ajax({
        url: '/Customer/ShoppingBasket/GetShoppingBasket',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            shoppingBasket.innerHTML = `
                <h1>Корзина</h1>
                <div class="row row-cols-1">
                    <div class="container col-10">
                        <div class="row row-cols-2 row-cols-md-6 mb-5">
                        ${generateHTML(response)}
                        </div>
                    </div>
                </div>
            `;

            if (countProduct == 1) {
                orderingInformation.innerHTML = `<div>В вашей корзине ${countProduct} позиция</div><h5>Итого: ${sumPurchase} ₽</h5>`;
            }
            else {
                orderingInformation.innerHTML = `<div>Всего ${countProduct} позиций в вашей корзине.</div><h5>Итого: ${sumPurchase} ₽</h5>`;
            }
        },
        error: function (error) {
            shoppingBasket.innerHTML = `
                <h1>${error.responseJSON.error}</h1>                
            `;
        }
    });
}
function generateHTML(response) {

    var html = "";

    for (var key in response.data) {

        totalPrice += response.data[key].price * response.data[key].count;
        countProduct += response.data[key].count;

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
            response.data[key].price + ` ₽` +
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







function addToShoppingBasket(event, id, isFromWishList = false) {
    event.preventDefault();
    $.ajax({
        url: '/Customer/ShoppingBasket/AddBasket' + "?productId=" + id + "&isWishList=" + isFromWishList,
        type: 'POST',
        data: id, isFromWishList,
        success: function (response) {
            var btnShoppingBasket = document.getElementById(`btnShoppingBasket_${id}`);
            btnShoppingBasket.innerHTML = `<button type="submit" onclick="removeFromShoppingBasket(event,${id})" class="btn btn-outline-success bi-cart-check" style="width: 56px; height: 40px;"></button>`;
        },
        error: function (error) {
        }
    });
}
function removeFromShoppingBasket(event, id, isFromWishList = false) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/ShoppingBasket/RemoveFromBasket' + "?productId=" + id + "&isWishList=" + isFromWishList,
        type: 'POST',
        data: id, isFromWishList,
        success: function (response) {
            if (isFromWishList == false) {
                var btnShoppingBasket = document.getElementById(`btnShoppingBasket_${id}`);
                btnShoppingBasket.innerHTML = `<button type="submit" onclick="addToShoppingBasket(event,${id})" class="btn bi-cart" style="width: 56px; height: 50px;"></button>`;
            }
            getWishList();
        },
        error: function (error) {
            reject(error);
        }
    });
}
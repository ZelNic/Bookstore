



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
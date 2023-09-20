
$(document).ready(function () {
    getShoppingBasket();
});

var dataShoppingBasket;
var countProduct;
var totalPrice;
var countSelectedProduct;
var orderingInformation = document.getElementById("orderingInformation");
var shoppingBasket = document.getElementById("shoppingBasket");
function getShoppingBasket() {

    countProduct = 0;
    totalPrice = 0;


    $.ajax({
        url: '/Customer/ShoppingBasket/GetShoppingBasket',
        type: 'GET',
        dataType: 'json',
        success: function (response) {

            dataShoppingBasket = response.data;

            shoppingBasket.innerHTML = `
                <h1>Корзина</h1>
                <div class="row row-cols-1">
                    <div class="container col-10">
                        <div class="row row-cols-2 row-cols-md-6 mb-5">
                        ${generateHTML()}
                        </div>
                    </div>
                </div>
            `;

            if (countProduct == 1) {
                orderingInformation.innerHTML = `<div>В вашей корзине ${countProduct} позиция</div><h5>Итого: ${totalPrice} ₽</h5>`;
            }
            else {
                orderingInformation.innerHTML = `<div>Всего ${countProduct} позиций в вашей корзине.</div><h5>Итого: ${totalPrice} ₽</h5>`;
            }


        },
        error: function (error) {
            shoppingBasket.innerHTML = `
                <h1>${error.responseJSON.error}</h1>                
            `;
        }
    });
}
function generateHTML() {

    var html = "";

    for (var key in dataShoppingBasket) {

        totalPrice += dataShoppingBasket[key].price * dataShoppingBasket[key].count;
        countProduct += dataShoppingBasket[key].count;

        html +=
            `<div class="mb-3 p-1">
                    <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                        <img src="${dataShoppingBasket[key].image}" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 70%;" />
                        <div class="card-body mt-0 pb-0">
                            <div class="fs-6">
                                ${dataShoppingBasket[key].nameProduct}
                            </div>
                        </div>
                        <div class="card card-footer bg-transparent border-0">
                            <hr />
                            <div class="text-center">
                                ${dataShoppingBasket[key].author}
                            </div>
                            <hr />
                            <div class="text-center fs-5">
                                ${dataShoppingBasket[key].price} ₽
                            </div>
                            <hr />
                            <div class="mx-auto pb-1">
                                <button onclick="removeFromShoppingBasket(event, ${dataShoppingBasket[key].productId})" type="submit"
                                        class="btn btn-outline-danger border-0 bi bi-x-circle"></button>

                                <button id="selector_${dataShoppingBasket[key].productId}" onclick="selectProduct(${key},${dataShoppingBasket[key].productId},true)" type="submit"
                                        class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>
                            </div>
                            <div class="card-group">
                                <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                                    <div class="row border border-0">
                                        <div class="mx-auto">
                                            <button class="btn bi bi-dash-circle">                                                
                                            </button>                       
                                                ${dataShoppingBasket[key].count}                        
                                            <button  type="submit" class="btn bi bi-plus-circle opacity-75">                                               
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                `;
    };

    return html;
}

function selectProduct(key, id, isSelect) {
    var selector = document.getElementById(`selector_${id}`);
    var boxSelect = document.getElementById("boxSelect");
    countSelectedProduct = 0;

    if (isSelect == true) {
        selector.outerHTML =
            `<button id="selector_${id}" onclick="selectProduct(${key}, ${id}, ${false})" type="submit"
            class="btn btn-outline-success border-0 bi bi-check-circle-fill"></button>`;
        dataShoppingBasket[key].isSelect = true;
    }
    else {
        selector.outerHTML =
            `<button id="selector_${id}" onclick="selectProduct(${key},${id}, ${true})" type="submit"
            class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>`;
        dataShoppingBasket[key].isSelect = false;
    }

    for (var key in dataShoppingBasket) {
        if (dataShoppingBasket[key].isSelect == true) {
            countSelectedProduct++;
        }
    }

    if (countSelectedProduct > 0) {
        boxSelect.innerHTML = `<div class="mt-5 pt-5">Выбрано ${countSelectedProduct} позиций в вашей корзине.</div>                
                <div>
                <button type="submit" onclick="removeFromShoppingBasket(event, ${dataShoppingBasket[key].productId})" class="btn btn-outline-warning border-2 bi bi-trash-fill"></button>
                <button type="submit" onclick="addToWishlist(event,${dataShoppingBasket[key].productId}); removeFromShoppingBasket(event,${dataShoppingBasket[key].productId}) " class="btn btn-outline-danger border-2 bi bi-heart-fill" style="width: 56px; height: 40px;"></button>
                <div>`;
    }
    else {
        boxSelect.innerHTML = "";
    }


}


function addToShoppingBasket(event, id) {
    event.preventDefault();
    $.ajax({
        url: '/Customer/ShoppingBasket/AddBasket' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            var btnShoppingBasket = document.getElementById(`btnShoppingBasket_${id}`);
            btnShoppingBasket.innerHTML = `<button type="submit" onclick="removeFromShoppingBasket(event,${id})" class="btn btn-outline-success bi-cart-check" style="width: 56px; height: 40px;"></button>`;
        },
        error: function (error) {
        }
    });
}
function removeFromShoppingBasket(event, id) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/ShoppingBasket/RemoveFromBasket' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            getShoppingBasket();
        },
        error: function (error) {
            reject(error);
        }
    });
}
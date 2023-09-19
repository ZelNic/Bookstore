
$(document).ready(function () {
    getShoppingBasket();
});

var dataShoppingBasket;
var countProduct;
var totalPrice;
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
            dataShoppingBasket = response;
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
function generateHTML(response) {

    var html = "";

    for (var key in response.data) {

        totalPrice += response.data[key].price * response.data[key].count;
        countProduct += response.data[key].count;

        html +=
            `<div class="mb-3 p-1">
                    <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                        <img src="${response.data[key].image}" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 70%;" />
                        <div class="card-body mt-0 pb-0">
                            <div class="fs-6">
                                ${response.data[key].nameProduct}
                            </div>
                        </div>
                        <div class="card card-footer bg-transparent border-0">
                            <hr />
                            <div class="text-center">
                                ${response.data[key].author}
                            </div>
                            <hr />
                            <div class="text-center fs-5">
                                ${response.data[key].price} ₽
                            </div>
                            <hr />
                            <div class="mx-auto pb-1">
                                <button onclick="removeFromShoppingBasket(event, ${response.data[key].productId})" type="submit"
                                        class="btn btn-outline-danger border-0 bi bi-x-circle"></button>

                                <button id="selector_${response.data[key].productId}" onclick="selectProduct(event,${response.data[key].productId},true)" type="submit"
                                        class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>
                            </div>
                            <div class="card-group">
                                <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                                    <div class="row border border-0">
                                        <div class="mx-auto">
                                            <button class="btn bi bi-dash-circle">                                                
                                            </button>                       
                                                ${response.data[key].count}                        
                                            <button  type="submit" class="btn bi bi-plus-circle opacity-75">                                               
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                `
    };

    return html;
}

function selectProduct(event, id, isSelect) {
    var selectror = document.getElementById(`selector_${id}`);

    if (isSelect == true) {
        selectror.innerHTML = `
        <button id="selector_${id}" onclick="selectProduct(event, ${id},false)" type="submit"
        class="btn btn-outline-secondary border-0 bi bi-check-circle-fill"></button>
    `;
    } else {
        selectror.innerHTML = 
        `
            <button id="selector_${id}" onclick="selectProduct(event,${id},true)" type="submit"
            class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>`;
    }


    dataShoppingBasket.data[key].isSelect = true;
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
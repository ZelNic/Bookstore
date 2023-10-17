
$(document).ready(function () {
    getShoppingBasket();
});

let dataShoppingBasket;
let countProduct;
let totalPrice;
let countSelectedProduct;
let orderingInformation = document.getElementById("orderingInformation");
let boxSelect = document.getElementById("boxSelect");
let checkout = document.getElementById("checkout");
let productArray = [];
let isSave = false;

function getShoppingBasket() {

    countProduct = 0;
    countSelectedProduct = 0;
    totalPrice = 0;
    productArray = [];

    $.ajax({
        url: '/Customer/ShoppingBasket/GetShoppingBasket',
        type: 'GET',
        dataType: 'json',
        success: function (response) {

            let shoppingBasket = document.getElementById("shoppingBasket");
            if (shoppingBasket == null) {
                return;
            }

            dataShoppingBasket = response.data;

            shoppingBasket.innerHTML =
                `
                    <h1>Корзина</h1>          
                    <div class="row row-cols-1">
                        <div class="container col-10">
                            <div class="row row-cols-2 row-cols-md-6 mb-5">
                                ${generateCardProduct()}
                            </div>
                        </div>                    
                    </div>
                `;



            showBtnSaveChange();
            showTotal();
            activeSelectBox();
        },
        error: function (error) {
            shoppingBasket.innerHTML = `<h1>${error.responseText}</h1>`
            orderingInformation.innerHTML = ``;
            checkout.innerHTML = ``;
            boxSelect.innerHTML = ``;
        }
    });
}
function generateCardProduct() {

    let html = "";

    for (let key in dataShoppingBasket) {

        totalPrice += dataShoppingBasket[key].price * dataShoppingBasket[key].count;
        countProduct += dataShoppingBasket[key].count;

        html +=
            `<div class="mb-3 p-1">
                    <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                        <img src="${dataShoppingBasket[key].image}" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 80%;" />
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
                                            <button onclick="changeCountProduct(event, ${key}, 'minus')" type="submit" class="btn bi bi-dash-circle opacity-100"></button> 
                                            <input onblur="changeCountProduct(event, ${key}, 'input', document.getElementById('countProduct_${key}').value)"
                                                id="countProduct_${key}"  type="number" min="1" max="50" name="count" value="${dataShoppingBasket[key].count}" style="object-fit: width: 4px; height: 20px; " required/>
                                            <button onclick="changeCountProduct(event, ${key},'plus')" type="submit" class="btn bi bi-plus-circle opacity-100"></button>   
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

function showBtnSaveChange() {
    let btnIsSave = document.getElementById("isSave");
    if (isSave == false) {
        btnIsSave.setAttribute('hidden', '');
    }
    else {
        btnIsSave.removeAttribute('hidden');
    }
}
function showTotal() {
    checkout.innerHTML = `<button onclick="doCheckout()" class="btn btn-success bi bi-bag-fill" type="submit"> Оформить</button>`;

    if (countProduct == 1) {
        orderingInformation.innerHTML = `<div>В вашей корзине ${countProduct} позиция</div><h5>Итого: ${totalPrice} ₽</h5>`;
    }
    else {
        orderingInformation.innerHTML = `<div>Всего ${countProduct} позиций в вашей корзине.</div><h5>Итого: ${totalPrice} ₽</h5>`;
    }
}

function doCheckout() {
    $.ajax({
        url: '/Purchase/Purchase/FillDeliveryDate',
        type: 'GET',
        success: function (response) {
            window.location.href = '/Purchase/Purchase/FillDeliveryDate';
        },
        error: function (error) {
            console.log();
        }
    });
}

function activeSelectBox(key = null) {
    if (productArray.length > 0) {

        boxSelect.innerHTML = `<div class="mt-5 pt-5">Выбрано ${productArray.length} позиций в вашей корзине.</div>                
                <div>
                <button type="submit" onclick="removeFromShoppingBasket(event, '${productArray}')" class="btn btn-outline-warning border-2 bi bi-trash-fill"></button>
                <button type="submit" onclick="addToWishlist(event,'${productArray}'); removeFromShoppingBasket(event,'${productArray}') " class="btn btn-outline-danger border-2 bi bi-heart-fill" style="width: 56px; height: 40px;"></button>
                <div>`;
    }
    else {
        boxSelect.innerHTML = "";
    }
}
function selectProduct(key, id, isSelect) {

    let selector = document.getElementById(`selector_${id}`);
    productArray = [];

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

    for (let key in dataShoppingBasket) {
        if (dataShoppingBasket[key].isSelect == true) {
            productArray.push(dataShoppingBasket[key].productId);
        }
    }

    activeSelectBox(key);
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
function removeFromShoppingBasket(event, productId) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/ShoppingBasket/RemoveFromBasket?productsId=' + productId,
        type: 'POST',
        data: productId,
        success: function (response) {
            getShoppingBasket();
        },
        error: function (error) {
            getShoppingBasket();
            reject(error);
        }
    });
}

function changeCountProduct(event, key, operation, count = 1) {

    if (parseInt(count) == 0) {
        removeFromShoppingBasket(event, dataShoppingBasket[key].productId);
        isSave = false;
        showBtnSaveChange();
        return;
    }

    isSave = true;
    showBtnSaveChange();

    let indicateCountProduct = document.getElementById(`countProduct_${key}`);


    switch (operation) {
        case "minus":
            dataShoppingBasket[key].count--;
            countProduct--;
            totalPrice -= dataShoppingBasket[key].price
            if (dataShoppingBasket[key].count <= 0) {
                isSave = false;
                showBtnSaveChange();
                removeFromShoppingBasket(event, dataShoppingBasket[key].productId)
            }
            break;
        case "plus":
            dataShoppingBasket[key].count++;
            countProduct++;
            totalPrice += dataShoppingBasket[key].price;
            break;
        case "input":
            count = parseInt(count);
            countProduct -= dataShoppingBasket[key].count;
            totalPrice -= dataShoppingBasket[key].count * dataShoppingBasket[key].price;

            dataShoppingBasket[key].count = count;
            countProduct += dataShoppingBasket[key].count;
            totalPrice += dataShoppingBasket[key].count * dataShoppingBasket[key].price;
            break
        default:
            Swal.fire({
                icon: 'error',
                text: 'Произошла ошибка. Обновите страницу.'
            })
            break;
    }



    if (!productArray.includes(key)) {
        productArray.push(key);
    }

    indicateCountProduct.setAttribute("value", dataShoppingBasket[key].count);
    showTotal();
}

function confirmChangeCount(event) {
    event.preventDefault();

    let productData = "";

    for (var i = 0; i < productArray.length; i++) {
        productData += dataShoppingBasket[productArray[i]].productId + ":" + dataShoppingBasket[productArray[i]].count;
        if (i + 1 < productArray.length) {
            productData += '|';
        }
    }

    $.ajax({
        url: '/Customer/ShoppingBasket/ChangeCountProduct?productData=' + `${productData}`,
        type: 'POST',
        data: productData,
        success: function (response) {
            isSave = false;
            showBtnSaveChange();
            getShoppingBasket();
        },
        error: function (error) {
            getShoppingBasket();
            reject(error);
        }
    });
}
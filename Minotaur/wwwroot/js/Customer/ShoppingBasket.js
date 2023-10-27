
$(document).ready(function () {
    getShoppingBasket();
});

let selectedProductArray = [];

let countProduct;
let totalPrice;
let countSelectedProduct;

let dataShoppingBasket;

let orderingInformation = document.getElementById("orderingInformation");
let checkout = document.getElementById("checkout");
let boxSelect;
let shoppingBasket;

function getShoppingBasket() {

    countProduct = 0;
    countSelectedProduct = 0;
    totalPrice = 0;
    selectedProductArray = [];

    shoppingBasket = document.getElementById("shoppingBasket");
    boxSelect = document.getElementById("boxSelect");

    $.ajax({
        url: '/Customer/ShoppingBasket/GetShoppingBasket',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            dataShoppingBasket = response.data;

            shoppingBasket.innerHTML =
                `
                    <h1>Корзина</h1>          
                    <div class="row row-cols-1">
                        <div class="container col-10">
                            <div class="row row-cols-2 row-cols-md-6 mb-5">
                                ${generateCardProducts()}
                            </div>
                        </div>                    
                    </div>
                `;


            showTotal();
        },
        error: function (error) {
            shoppingBasket.innerHTML = `<h1>${error.responseText}</h1>`;
        }
    });
}
function generateCardProducts() {

    let html = "";

    for (let [index, product] of dataShoppingBasket.entries()) {

        totalPrice += product.price * product.count;
        countProduct += product.count;

        html += ` <div class="mb-3" >
                    <div class="card card-subtitle h-100 shadow pt-1 border-0 m-0 p-0">
                        <img src="${product.image}" class="card-img-top mx-auto rounded-1" alt="Product Image" style="object-fit: cover; width: 70%;" />
                        <div class="card-body mt-0 pb-0">
                            <div class="fs-6">
                                ${product.nameProduct}
                            </div>
                        </div>
                        <div class="card card-footer bg-transparent border-0">
                            <hr />
                            <div class="text-center">
                                ${product.author}
                            </div>
                            <hr />
                            <div class="text-center fs-5">
                                ${product.price} ₽
                            </div>
                            <div>
                            <button onclick="removeFromShoppingBasket(${product.productId})" class="btn btn-outline-danger border-0 bi bi-x-circle"></button>
                            <button id="selector_${product.productId}" onclick="selectProduct(${index}, ${true})" class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>
                            </div>
                            <hr />
                            <div class="mx-auto pb-1">
                                <button onclick="changeCountProduct(${product}, 'minus')" type="submit" class="btn bi bi-dash-circle opacity-100"></button>
                                <input onblur="changeCountProduct( ${product}, 'input', document.getElementById('countProduct_${product}').value)"
                                    id="countProduct_${product.productId}" type="number" min="1" max="50" name="count" value="${product.count}" style="object-fit: width: 4px; height: 20px; " required />
                                <button onclick="changeCountProduct( ${product},'plus')" type="submit" class="btn bi bi-plus-circle opacity-100"></button>
                            </div>
                        </div>
                    </div>
                </div >`;
    };

    return html;
}
function showBtnSaveChange() {
    let btnIsSave = document.getElementById("isSave");
    if (btnIsSave.hasAttribute('hidden')) {
        btnIsSave.removeAttribute('hidden');
    }
    else {
        btnIsSave.setAttribute('hidden', 'true');
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
            Swal.fire({
                icon: 'error',
                text: error.responseText,
            })
        }
    });
}
function activeSelectBox() {
    if (selectedProductArray.length > 0) {

        boxSelect.removeAttribute("hidden");

        boxSelect.innerHTML =
            `<div class="mt-5 pt-5">Выбрано ${selectedProductArray.length} позиций в вашей корзине.</div>                
                <div>
                    <button type="submit" onclick="removeFromShoppingBasket('${selectedProductArray}')" class="btn btn-outline-warning border-2 bi bi-trash-fill"></button>
                    <button type="submit" onclick="addToWishlist('${selectedProductArray}'); removeFromShoppingBasket('${selectedProductArray}') " class="btn btn-outline-danger border-2 bi bi-heart-fill" style="width: 56px; height: 40px;"></button>
                <div>`;
    }
    else {
        boxSelect.setAttribute("hidden", "true");
    }
}
function selectProduct(index, isSelect) {

    let selector = document.getElementById(`selector_${dataShoppingBasket[index].productId}`);
    selectedProductArray = [];

    if (isSelect == true) {
        selector.outerHTML =
            `<button id="selector_${dataShoppingBasket[index].productId}" onclick="selectProduct(${index}, ${false})"class="btn btn-outline-success border-0 bi bi-check-circle-fill"></button>`;
        dataShoppingBasket[index].isSelect = true;
    }
    else {
        selector.outerHTML =
            `<button id="selector_${dataShoppingBasket[index].productId}" onclick="selectProduct(${index},${true})" type="submit"
            class="btn btn-outline-secondary border-0 bi bi-check-circle"></button>`;
        dataShoppingBasket[index].isSelect = false;
    }

    for (let index in dataShoppingBasket) {
        if (dataShoppingBasket[index].isSelect == true) {
            selectedProductArray.push(dataShoppingBasket[index].productId);
        }
    }

    activeSelectBox();
}

function removeFromShoppingBasket(productId) {
    $.ajax({
        url: '/Customer/ShoppingBasket/RemoveFromBasket?productsId=' + productId,
        type: 'POST',
        data: productId,

        success: function (responce) {
            selectedProductArray = [];
            getShoppingBasket();
            showTotal();
            activeSelectBox();
        },
        error: function (error) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'info',
                title: error.responseText
            })

            selectedProductArray = [];
            getShoppingBasket();
            showTotal();
            activeSelectBox();
        }
    });
}
function changeCountProduct(product, operation, count = 1) {

    if (parseInt(count) == 0) {
        removeFromShoppingBasket(dataShoppingBasket[product].productId);
        showBtnSaveChange();
        return;
    }

    showBtnSaveChange();

    let indicateCountProduct = document.getElementById(`countProduct_${product.productId}`);

    switch (operation) {
        case "minus":
            dataShoppingBasket[product].count--;
            countProduct--;
            totalPrice -= dataShoppingBasket[product].price
            if (dataShoppingBasket[product].count <= 0) {
                showBtnSaveChange();
                removeFromShoppingBasket(dataShoppingBasket[product].productId)
            }
            break;
        case "plus":
            dataShoppingBasket[product].count++;
            countProduct++;
            totalPrice += dataShoppingBasket[product].price;
            break;
        case "input":
            count = parseInt(count);
            countProduct -= dataShoppingBasket[product].count;
            totalPrice -= dataShoppingBasket[product].count * dataShoppingBasket[product].price;

            dataShoppingBasket[product].count = count;
            countProduct += dataShoppingBasket[product].count;
            totalPrice += dataShoppingBasket[product].count * dataShoppingBasket[product].price;
            break
        default:
            Swal.fire({
                icon: 'error',
                text: 'Произошла ошибка. Обновите страницу.'
            })
            break;
    }


    if (!selectedProductArray.includes(product)) {
        selectedProductArray.push(product);
    }

    indicateCountProduct.setAttribute("value", dataShoppingBasket[product].count);
    showTotal();
}
function confirmChangeCount() {

    let productData = "";

    for (var i = 0; i < selectedProductArray.length; i++) {
        productData += dataShoppingBasket[selectedProductArray[i]].productId + ":" + dataShoppingBasket[selectedProductArray[i]].count;
        if (i + 1 < selectedProductArray.length) {
            productData += '|';
        }
    }

    $.ajax({
        url: '/Customer/ShoppingBasket/ChangeCountProduct?productData=' + `${productData}`,
        type: 'POST',
        data: productData,
        success: function (response) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'success',
                title: 'Сохранено'
            })

            showBtnSaveChange();
            getShoppingBasket();
        },
        error: function (error) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'error',
                title: error.responseText
            })

            getShoppingBasket();
        }
    });
}
function addToWishlist(selectedProductArray) {
    $.ajax({
        url: '/Customer/WishList/AddWishList?productIds=' + selectedProductArray,
        type: 'POST',
        success: function (response) {

            selectedProductArray = [];

            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'info',
                title: 'Товары перемещены в список желаний'
            })
        },
        error: function (error) {

            selectedProductArray = [];

            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'info',
                title: error.responseText
            })
        }
    });

    activeSelectBox()
}
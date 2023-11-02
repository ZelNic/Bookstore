let productsCardData;
let totalPages;
let userId;

$(document).ready(function () {
    getProductsCard();
});

function getProductsCard(numberPage = 1) {
    $.ajax({
        url: '/Customer/Home/GetProductsData?numberPage=' + numberPage,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            productsCardData = response.data;
            totalPages = response.totalPages;
            userId = response.id

            let divProductCards = document.getElementById("productCards");
            let divNumberPages = document.getElementById("numberPages");

            divProductCards.innerHTML = `${generateProductsCard()}`;
            divNumberPages.innerHTML = `${genarateBtnNumberPages()}`;
        },
    });
}
function genarateBtnNumberPages() {
    let btnNumberPages = ``;

    for (let i = 0; i < totalPages; i++) {
        btnNumberPages += `
            <button onclick="getProductsCard(${i + 1})" class="btn btn-secondary">${i + 1}</button>
        `;
    }

    return btnNumberPages;
}
function generateProductsCard() {

    let cardsProduct = ``;

    for (let [index, product] of productsCardData.entries()) {

        let cardBtn = ``;

        if (userId == null) {
            cardBtn += `<button onclick="goToLogin()" class="btn btn-outline-danger border-0 bi bi-heart">`;
            cardBtn += `<button onclick="goToLogin()" class="btn btn-outline-success border-0 bi bi-cart"></button>`
        }
        else {
            cardBtn += `${product.isInWishList ? `<button id="btnWishList_${index}" onclick="removeFromWishList(${index})" class="btn btn-outline-danger border-0 bi bi-heart-fill"></button>`
                : `<button id="btnWishList_${index}" onclick="addToWishList(${index})" class="btn btn-outline-danger border-0 bi bi-heart"></button>`}`;

            cardBtn += `${product.isInShoppingBasket ? `<button id="btnShoppingBasket_${index}" onclick="goToShoppingBasket(${index})" class="btn btn-outline-success border-0 bi bi-cart-check"></button>`
                : `<button id="btnShoppingBasket_${index}" onclick="addToShoppingBasket(${index})" class="btn btn-outline-success border-0 bi bi-cart"></button>`}`;
        }

        cardsProduct += `           
                        <div class="col-lg-2 col-sm-1 px-3">
                            <div class="col-10">
                                <div class="card card-deck shadow h-100">
                                    <img src="${product.imageURL}" class="card-img-top img-fluid" style="object-fit: cover;" />
                                    <div class="card-body d-flex flex-column">
                                        <div class="card-body mt-auto mt-2">
                                            <div>
                                                <p class="card-title text-black"><b>${product.name}</b></p>
                                            </div>
                                            <div>
                                                <p class="card-title text-dark opacity-75 fs-6"><b>${product.author}</b></p>
                                            </div>
                                            <div>
                                                <p class="card-title text-dark opacity-75 fs-6"><b>${product.category}</b></p>
                                            </div>
                                            <hr />
                                            <div class="text-center text-muted fs-3 fw-bold">
                                                ${product.price} ₽
                                            </div>
                                        </div>
                                        <div class="card-footer bg-transparent mx-auto mt-auto p-1">
                                            <div>
                                                <button onclick="getDetails(${product.productId})" class="btn btn-outline-primary border-0 bi bi-three-dots"></button>
                                                ${cardBtn}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        `;

    }
    return cardsProduct;
}
function goToLogin() {
    window.location.href = '/Identity/Account/Login'
}
function goToWishList() {
    window.location.href = '/customer/wishlist/index'
}
function goToShoppingBasket() {
    window.location.href = '/customer/shoppingbasket/index'
}

function getDetails(productId) {
    window.location.href = `/Customer/Home/Details?id=${productId}`
}

function addToWishList(index) {
    $.ajax({
        url: '/Customer/WishList/AddWishList?productIds=' + String(productsCardData[index].productId),
        method: 'POST',
        success: function (response) {

            let btnWishList = document.getElementById(`btnWishList_${index}`);
            btnWishList.removeAttribute("onclick");
            btnWishList.setAttribute("onclick", `removeFromWishList(${index})`);

            if (btnWishList.classList.contains("bi-heart-fill")) {
                btnWishList.classList.remove("bi-heart-fill");
                btnWishList.classList.add("bi-heart");
            } else {
                btnWishList.classList.remove("bi-heart");
                btnWishList.classList.add("bi-heart-fill");
            }

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
                title: 'Добавлено в список желаемого',
            })
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
                title: 'Не удалось добавить товар в список желаемого',
            })
        }
    });
}

function removeFromWishList(index) {
    $.ajax({
        url: '/Customer/WishList/RemoveFromWishList?productId=' + productsCardData[index].productId,
        method: 'POST',
        success: function (response) {

            let btnWishList = document.getElementById(`btnWishList_${index}`);
            btnWishList.removeAttribute("onclick");
            btnWishList.setAttribute("onclick", `addToWishList(${index})`);

            if (btnWishList.classList.contains("bi-heart-fill")) {
                btnWishList.classList.remove("bi-heart-fill");
                btnWishList.classList.add("bi-heart");
            } else {
                btnWishList.classList.remove("bi-heart");
                btnWishList.classList.add("bi-heart-fill");
            }


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
                title: 'Товар удален из списка желаемого',
            })
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
                title: error.responseText,
            })
        }
    });
}

function addToShoppingBasket(index) {
    $.ajax({
        url: '/Customer/shoppingBasket/AddToBasketProduct?productId=' + productsCardData[index].productId,
        method: 'POST',
        success: function (response) {

            let btnShoppingBasket = document.getElementById(`btnShoppingBasket_${index}`);
            btnShoppingBasket.removeAttribute("onclick");
            btnShoppingBasket.setAttribute("onclick", `goToShoppingBasket(${index})`);

            if (btnShoppingBasket.classList.contains("bi-cart")) {
                btnShoppingBasket.classList.remove("bi-cart");
                btnShoppingBasket.classList.add("bi-cart-check");
            }

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
                title: 'Добавлено в корзину',
            })
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
                title: error.responseText,
            })
        }
    });
}
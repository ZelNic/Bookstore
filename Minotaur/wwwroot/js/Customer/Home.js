
$(document).ready(function () {
    getProductsCard();
});

let productsCardData;
let totalPages;

function getProductsCard(numberPage = 1) {
    $.ajax({
        url: '/Customer/Home/GetProductsData?numberPage=' + numberPage,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            productsCardData = response.data;
            totalPages = response.totalPages;

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
        cardsProduct += `     
            <div class="col-lg-2 col-sm-1">
                <div class="row p-1 mt-1" style="height: 100%">
                    <div class="col-12">
                        <div class="card card-deck border-0 p-1 shadow mx-auto h-100">
                            <img src="${product.imageURL}" class="card-img" />
                            <div class="card-body border-light">
                                <div class="card-body mt-auto mt-2">
                                    <div>
                                        <p class="card-title text-black"><b>${product.name}</b></p>
                                    </div>
                                    <div>
                                        <p class="card-title text-dark opacity-75"><b>${product.author}</b></p>
                                    </div>
                                    <div>
                                        <p class="card-title text-dark opacity-75"><b>${product.category}</b></p>
                                    </div>
                                    <hr/>
                                     <div>
                                        <p class="card-title text-black"><b>${product.price} ₽</b></p>
                                    </div>                                    
                                </div>
                                <div class="card-footer bg-transparent mt-auto p-1 ">
                                    <div>
                                    <button onclick="getDetails(${product.productId})" class="btn btn-outline-primary border-0 bi bi-three-dots"></button>
                                    ${product.isInWishList ? `<button id="btnWishList_${product.productId}" onclick="addToWishList(${index})" class="btn btn-outline-danger border-0 bi bi-heart-fill"></button>` : `<button id="btnWishList_${product.productId}" onclick="addToWishList(${index})" class="btn btn-outline-danger border-0 bi bi-heart">`}
                                    ${product.isInShoppingBasket ? `<button onclick="addToShoppingBasket(${index})" class="btn btn-outline-success border-0  bi-cart-check"></button>` : `<button onclick="addToShoppingBasket(${index})" class="btn btn-outline-success border-0 bi bi-cart"></button>`}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;
    }

    return cardsProduct;
}

function getDetails(productId) {
    $.ajax({
        url: '/customer/home/details?productId=' + productId,
        method: 'get',
        success: function (response) {
            window.location.href = '/customer/home/details?productId=' + productId
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

function goToWishList() {
    window.location.href = '/customer/wishlist/indes'
}

function addToWishList(index) {
    $.ajax({
        url: '/customer/wishlist/addWishList?newProductId' + productsCardData[index].productId,
        method: 'POST',
        success: function (response) {

            let btnWishList = document.getElementById(`btnWishList_${productsCardData[index].productId}`);
            btnWishList.removeAttribute("onclick");
            btnWishList.setAttribute("onclick", "goToWishList()");

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

function addToShoppingBasket(index) {

}
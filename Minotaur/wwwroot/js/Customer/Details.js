
function changeFunctionButtonWishlist(id) {
     let btnWishlist = document.getElementById(`btnWishList_${id}`);

            btnWishlist.addEventListener('click', function () {
                if (btnWishlist.getAttribute('onclick') === `addToWishlist(${id})`) {
                    btnWishlist.removeAttribute('onclick');
                    btnWishlist.addEventListener('click', function () {
                        removeFromWishlist(id);
                    });
                    btnWishlist.classList.remove('bi-heart');
                    btnWishlist.classList.add('bi-heart-fill');
                } else {
                    btnWishlist.removeAttribute('onclick');
                    btnWishlist.addEventListener('click', function () {
                        addToWishlist(id);
                    });
                    btnWishlist.classList.remove('bi-heart-fill');
                    btnWishlist.classList.add('bi-heart');
                }
            });
}

function addToWishlist(id) {
    $.ajax({
        url: '/Customer/WishList/AddWishList?newProductId=' + id,
        type: 'POST',
        success: function (response) {

            changeFunctionButtonWishlist(id);

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
                title: 'Товар добавлен в список желаний'
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
                icon: 'info',
                title: error.responseText
            })
        }
    });
}
function removeFromWishlist(id) {
    $.ajax({
        url: '/Customer/WishList/RemoveFromWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            changeFunctionButtonWishlist(id);
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
        }
    });
}

function changeFunctionButtonShoppingBasket(id) {
    let btnShoppingBasket = document.getElementById(`btnShoppingBasket_${id}`);

    btnShoppingBasket.addEventListener('click', function () {
        if (btnShoppingBasket.getAttribute('onclick') === `addToShoppingBasket(${id})`) {
            btnShoppingBasket.removeAttribute('onclick');
            btnShoppingBasket.addEventListener('click', function () {
                removeFromWishlist(id);
            });
            btnShoppingBasket.classList.remove('bi bi-bag');
            btnShoppingBasket.classList.add('bi bi-bag-check-fill');
        } else {
            btnShoppingBasket.removeAttribute('onclick');
            btnShoppingBasket.addEventListener('click', function () {
                addToShoppingBasket(id);
            });
            btnShoppingBasket.classList.remove('bi bi-bag-check-fill');
            btnShoppingBasket.classList.add('bi bi-bag');
        }
    });
}


function addToShoppingBasket(id) {
    $.ajax({
        url: '/Customer/ShoppingBasket/AddToBasketProduct?productId=' + id,
        type: 'POST',
        data: id,
        success: function (response) {

            changeFunctionButtonShoppingBasket(id);

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
                title: "Товар добавлен в корзину"
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
                title: error.responseText
            })
        }
    });

}
function removeFromShoppingBasket(id) {
    $.ajax({
        url: '/Customer/ShoppingBasket/RemoveFromBasket?productsId=' + id,
        type: 'POST',
        data: id,

        success: function (responce) {
            changeFunctionButtonShoppingBasket(id);

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
                title: "Товар убран из корзины"})
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

        }
    });
}

function informAboutNeedToLogin() {
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
        title: "Необходимо войти в учетную запись"
    })

}
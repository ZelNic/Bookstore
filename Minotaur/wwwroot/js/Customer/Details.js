

function changeFunctionButtonWishlist(id, isAdd) {
    let btnWishlist = document.getElementById(`btnWishList_${id}`);

    if (isAdd == true) {
        btnWishlist.innerHTML = `<button onclick = "removeFromWishlist(${id})" class="btn btn-outline-danger border-1 btn bi bi-heart-fill mt-1 mb-2" style = "width:100%; height: 40px;"></button >`;
    }
    else {
        btnWishlist.innerHTML = `
        <button onclick="addToWishlist(${id})" class="btn btn-outline-danger border-1 btn bi-heart mt-1 mb-2" style="width: 100%; height: 40px;"></button>`;
    }
}
function addToWishlist(id) {
    $.ajax({
        url: '/Customer/WishList/AddWishList?newProductId=' + id,
        type: 'POST',
        success: function (response) {

            changeFunctionButtonWishlist(id, true);

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
            changeFunctionButtonWishlist(id, false);
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
                title: 'Товар убран из списка желаемого'
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
function changeFunctionButtonShoppingBasket(id, isAdd) {
    let btnShoppingBasket = document.getElementById(`btnShoppingBasket_${id}`);

    if (isAdd == true) {
        btnShoppingBasket.innerHTML = ` 
        <button onclick = "removeFromShoppingBasket(${id})" class="btn btn-outline-success border-1 bi bi-bag-check-fill mt-1 mb-2" style = "width:100%; height: 40px;"></button >
        `;
    }
    else {
        btnShoppingBasket.innerHTML = `
       <button onclick="addToShoppingBasket(${id})" class="btn btn-outline-success border-1 btn bi bi-bag mt-1 mb-2" style="width: 100%; height: 40px;"></button>`;
    }    
}
function addToShoppingBasket(id) {
    $.ajax({
        url: '/Customer/ShoppingBasket/AddToBasketProduct?productId=' + id,
        type: 'POST',
        data: id,
        success: function (response) {

            changeFunctionButtonShoppingBasket(id, true);

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
            changeFunctionButtonShoppingBasket(id, false);

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
                title: "Товар убран из корзины"
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
function informAboutNeedToLogin() {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 1000,
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
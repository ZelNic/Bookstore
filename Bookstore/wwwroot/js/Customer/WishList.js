
var dataTable;
$(document).ready(function () {
    getWishList();
});


function getWishList() {
    $.ajax({
        url: '/Customer/WishList/GetWishList',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            var wishList = document.getElementById("wishList");
            wishList.innerHTML = `
                <h1>Отложенные товары</h1>
                <div class="row row-cols-2">
                    <div class="container col-10">
                        <div class="row row-cols-2 row-cols-md-6 mb-5">
                        ${generateHTML(response)}
                        </div>
                    </div>
                </div>
            `;
        },
        error: function (error) {

        }
    });
}
function generateHTML(response) {

    var html = "";

    var product = JSON.parse(response);
    
    for (var key in product) {        
        console.log(key);

        console.log(key.productId);
        console.log(key.nameProduct);
        console.log(key.category);

        html += `
        <div>${key.productId}</div>
        <div>${key.nameProduct}</div>
        <div>${key.category}</div>
    `;
    }
}

//console.log(product.nameProduct);
//console.log(product.category);

//html += `
//        <div>${product.productId}</div>
//        <div>${product.nameProduct}</div>
//        <div>${product.category}</div>
//        `;








function addToWishlist(event, id) {
    event.preventDefault();
    $.ajax({
        url: '/Customer/WishList/AddWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            var btnWishList = document.getElementById("btnWishList");
            btnWishList.innerHTML = '<button type="submit" onclick="removeFromWishlist(event,@book.BookId)" class="btn"><i class="bi bi-heart-fill"></i></button>';
        },
        error: function (error) {
        }
    });
}
function removeFromWishlist(event, id, isFromWishList) {
    event.preventDefault();

    $.ajax({
        url: '/Customer/WishList/RemoveFromWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            if (isFromWishList == false) {
                var btnWishList = document.getElementById("btnWishList");
                btnWishList.innerHTML = '<button type="submit" onclick="addToWishlist(event,@book.BookId)" class="btn><i class="bi bi-heart"></i></button>';
            }
        },
        error: function (error) {
            reject(error);
        }
    });
}






         //${data.map(product => `
                            //    <div class="col-2 mb-3">
                            //        <div class="card card-subtitle h-100 shadow pt-1 border-0">
                            //            <img src="${product.image}" class="card-img-top mx-auto rounded-1" style="object-fit: cover; width: 50%; height: 150px;" />
                            //            <div class="card-body mt-0 pb-0">
                            //                ${product.nameProduct}
                            //            </div>
                            //            <div class="card-body mt-0 pb-0">
                            //                ${product.author} p
                            //            </div>
                            //            <div class="card-body mt-0 pb-0">
                            //                ${product.category} p
                            //            </div>
                            //            <div class="card-body mt-0 pb-0">
                            //                ${product.price} p
                            //            </div>
                            //            <div class="card card-footer bg-transparent border-0">
                            //                <form method="post">
                            //                    <button onclick="removeFromWishlist(event, ${product.productId}, true)" type="submit" class="btn btn-danger">
                            //                        <i class="bi bi-x-circle"></i>
                            //                    </button>
                            //                    <button type="submit" asp-area="Customer" asp-controller="ShoppingBasket" asp-action="AddBasket" asp-route-productId="${product.productId}" asp-route-isWishList="true" class="btn btn-success border-0">
                            //                        <i class="bi bi-bag-plus"></i>
                            //                    </button>
                            //                </form>
                            //            </div>
                            //        </div>
                            //    </div>
                            //`).join('')}

function addToWishlist(event, id) {
    event.preventDefault(); // Предотвращает отправку стандартной формы

    $.ajax({
        url: '/Customer/WishList/AddWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {
            resolve(response);
        },
        error: function (error) {
            reject(error);
        }
    });
}


//function removeFromWishlist(id) {
//    $.ajax{
        
//    }
//}







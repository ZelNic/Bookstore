
const btnWishlist = document.getElementById("btnWishlist");

document.getElementById('addToWishlist').addEventListener('submit', function (event) {
    event.preventDefault(); 
    addToWishlist();
});


function addToWishlist(id) {
    $.ajax({
        url: '/Customer/WishList/AddWishList' + "?productId=" + id,
        type: 'POST',
        data: id,
        success: function (response) {          
            if (

            )
            resolve(response);
        },
        error: function (error) {
            reject(error);
        }
    });
}

function removeFromWishlist(id) {
    $.ajax{
        
    }
}




<button type="submit" asp-area="Customer" asp-controller="Wishlist" asp-action="AddWishList"
    asp-route-productId="@book.BookId" class="btn btn-warning border-0">
    <i class="bi bi-bookmark-check-fill"></i>
</button>




let reviews;

function goToReview() {
    window.location.href = '/Custmomer/Review/Index';
}
function getReviewsOnProduct(productId) {
    $.ajax({
        url: `/Customer/Review/GetReviewsOnProduct?productId=${productId}`,
        method: 'GET',
        dataTypes: 'json',
        success: function (response) {
            reviews = response.data;
            generateCardReviews();
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
function generateCardReviews() {
    let cardsReviews = ``;

    let sumOfRatings = 0;
    let countReviews = 0;
    let totalRating = 0;
    let divRating = document.getElementById("productRating");

    for (let [index, review] of reviews.entries()) {
        let image = ``;

        sumOfRatings += review.rating;
        totalRating = sumOfRatings / reviews.length;
        divRating.innerHTML = `<div class="fs-5 fw-bolder">Оценка: ${totalRating} <i class="bi bi-star-fill text-warning fs-4"></i></div>`

        for (let img of review.photo) {
            image += `<img src="/fileStorage/productReviewFiles/${img}" alt="Не найдено" class="mb-2 mx-1 rounded rounded-1" style="width: 20%; height: 20%;">`;
        }

        cardsReviews +=
            `
              <div class="card mb-2">
                <div class="card-body">
                    <p class="card-text">${review.nameUser}</p>
                    <hr />
                    <h5 class="card-title">Оценка: ${review.rating} <i class="bi bi-star-fill"></i></h5>
                    <p class="card-text">${review.productReviewText}</p>
                    <div class="d-flex flex-wrap">
                        ${image}
                    </div>
                </div>
                <div class="mx-2 mt-2 mb-2 row align-items-center">
                    <div class="col-auto p-0">
                        <button id="like_${review.id}" onclick="rateReview('${review.id}', true)" class="btn btn-outline-danger border border-0 bi bi-plus-circle-fill"></button>
                    </div>
                    <div class="col-auto p-1">
                        <div class="row m-0">
                            <div id="countLike_${review.id}" class="col text-danger p-1">${review.countLike}</div>
                            <div id="countDislike_${review.id}" class="col text-primary p-1">${review.countDislike}</div>
                        </div>
                    </div>
                    <div class="col-auto p-0">
                        <button id="dislike_${review.id}" onclick="rateReview('${review.id}', false)" class="btn btn-outline-primary border border-0 bi bi-dash-circle-fill"></button>
                    </div>
                </div>
            </div>
      `;
    }

    let divReviews = document.getElementById("reviewsCard");
    divReviews.innerHTML = cardsReviews;

}
function rateReview(reviewId, isLiked) {
    $.ajax({
        url: `/Customer/Review/RateReview?reviewId=${reviewId}&isLiked=${isLiked}`,
        method: 'POST',
        success: function (response) {
            getRateReview(reviewId);
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
                title: 'Успешно'
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
                icon: 'success',
                title: error.responseText
            })
        }
    });
}
function getRateReview(reviewId) {
    $.ajax({
        url: `/Customer/Review/GetRatingReview?id=${reviewId}`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            let countLike = document.getElementById(`countLike_${reviewId}`);
            let countDislike = document.getElementById(`countDislike_${reviewId}`);
            console.log(response.data);
            console.log(response.data.countLike);
            console.log(response.data.countDislike);

            countLike.textContent = response.data.countLike;
            countDislike.textContent = response.data.countDislike;
        }
    });
}
function reviewProductHandler(indexOrder, indexProduct) {
    $.ajax({
        url: `/Customer/Review/CheckForRefeedback?orderId=${ordersData[indexOrder].orderId}&productId=${ordersData[indexOrder].shippedProducts[indexProduct].id}`,
        method: 'GET',
        data: 'json',
        success: function (response) {
            let formProductReview = `
                   <form id="formReviewProduct" enctype="multipart/form-data">
                          <div class="form-row">    

                          <input name="OrderId" value="${ordersData[indexOrder].orderId}" hidden>   
                          <input name="ProductId" value="${ordersData[indexOrder].shippedProducts[indexProduct].id}" hidden>   
                          <input name="UserId" value="${ordersData[indexOrder].userId}" hidden>   

                            <div class="form-group col-md-12 mb-1">
                              <label for="productRating">Оценка товара:</label>
                              <select name="Rating" class="form-control" id="productRating" required>
                                <option selected disabled>Выбрать</option>
                                <option value="1">1</option>
                                <option value="2">2</option>
                                <option value="3">3</option>
                                <option value="4">4</option>
                                <option value="5">5</option>
                              </select>
                            </div>      
                          </div>
                          <div class="form-row">
                            <div class="form-group col-md-12 mb-1">
                              <label>Отзыв:</label>
                              <textarea name="ProductReviewText" class="form-control" id="review" ></textarea>
                            </div>
                              <label for="photo">Фото:</label>
                            <div class="form-group col-md-12 mb-1">
                              <div id="placeForCelslForPhoto">
                              <input type="file" class="form-control-file" id="photos" name="Photos" multiple>
                              </div>
                              <label class="opacity-50 fs-6">*до 10 снимков</label>
                            </div>
                            <label for="isAnonymous">Опубликовать анонимно?</label>
                            <input type="checkbox" id="isAnonymous" name="IsAnonymous" value="false">
                          </div>
                        </form>                              
                        `;
            Swal.fire({
                title: `Отзыв на ${ordersData[indexOrder].shippedProducts[indexProduct].productName}`,
                html: formProductReview,
                showCancelButton: true,
                confirmButtonText: 'Сохранить',
                cancelButtonText: 'Отмена',
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        let form = document.getElementById("formReviewProduct");
                        let isAnonymousCheckbox = document.getElementById("isAnonymous");

                        if (isAnonymousCheckbox.checked == true) {
                            isAnonymousCheckbox.value = true;
                        } else {
                            isAnonymousCheckbox.value = false;
                        }

                        let formData = new FormData(form);
                        console.log(formData);

                        $.ajax({
                            url: `/Customer/Review/PostReview`,
                            type: 'POST',
                            data: formData,
                            processData: false,
                            contentType: false,
                            success: function (response) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Отзыв поступил на проверку модерации',
                                });
                            },
                            error: function (error) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Ошибка...',
                                    text: error.responseText,
                                });
                            }
                        });
                    });
                },
                allowOutsideClick: true,
                allowEscapeKey: true,
                didOpen: () => {
                    document.getElementById("photos").addEventListener("change", checkFileCountAndSize);
                }
            });
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
                icon: 'success',
                title: error.responseText
            })
        }
    });
}

//TODO: НЕ РАБОТАЕТ
function checkFileCountAndSize() {
    let files = document.getElementById("photos").files;
    let maxFiles = 10; // Максимальное количество файлов
    let maxSizeInBytes = 5 * 1024 * 1024; // Максимальный размер файла в байтах (5 МБ)

    if (files.length > maxFiles) {
        alert("Максимальное количество файлов: " + maxFiles);
        files = Array.from(files).slice(0, maxFiles);
        return;
    }

    for (let i = 0; i < files.length; i++) {
        if (files[i].size > maxSizeInBytes) {
            alert("Размер файла превышает допустимый предел: " + (maxSizeInBytes / (1024 * 1024)) + " МБ");
            // Очистите выбранный файл или предупредите пользователя
            return;
        }
    }

    // Продолжайте с обработкой выбранных файлов
}

function reviewOrderHandler(orderId) {
    let formOrderReview = `
                        <form id="formOrderReview" enctype="multipart/form-data">
                        <div class="form-row">
                            <div class="form-group col-md-12 mb-1">
                                <label for="deliveryRating">Оценка доставки:</label>
                                <select class="form-control" name="DeliveryRating" required>
                                    <option selected disabled>Выбрать</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                    <option value="4">4</option>
                                    <option value="5">5</option>
                                </select>
                                <div class="form-group col-md-12 mb-1">
                                    <label for="review">Отзыв доставку:</label>
                                    <textarea class="form-control" ></textarea>
                                </div>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                                <label for="productRating">Оценка пункта выдачи:</label>
                                <select class="form-control"  required>
                                    <option selected disabled>Выбрать</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                    <option value="4">4</option>
                                    <option value="5">5</option>
                                </select>
                                <div class="form-group col-md-12 mb-1">
                                    <label for="review">Отзыв на пункт выдачи:</label>
                                    <textarea class="form-control" ></textarea>
                                </div>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                                <label for="productRating">Оценка сотрудника:</label>
                                <select class="form-control"  required>
                                    <option selected disabled>Выбрать</option>
                                    <option value="1">1</option>
                                    <option value="2">2</option>
                                    <option value="3">3</option>
                                    <option value="4">4</option>
                                    <option value="5">5</option>
                                </select>
                                <div class="form-group col-md-12 mb-1">
                                    <label for="review">Отзыв на сотрудника:</label>
                                    <textarea class="form-control"></textarea>
                                </div>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-12 mb-1">
                                <label for="review">Отзыв:</label>
                                <textarea class="form-control" ></textarea>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                                <label for="photo">Фотография:</label>
                                <input type="file" class="form-control-file">
                            </div>
                        </div>
                    </form>
                        `;

    Swal.fire({
        title: `Отзыв на заказ`,
        html: formOrderReview,
        showCancelButton: true,
        confirmButtonText: 'Сохранить',
        cancelButtonText: 'Отмена',
        preConfirm: () => {
            return new Promise((resolve, reject) => {
                const formData = new FormData(document.getElementById('formOrderReview'));
                const reviewObject = {};

                for (const [key, value] of formData.entries()) {
                    reviewObject[key] = value;
                }

                const reviewJson = JSON.stringify(reviewObject);

                $.ajax({
                    url: `=${reviewJson}`,
                    type: 'POST',
                    success: function (response) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Отзыв поступил на проверку модерации',
                        });
                    },
                    error: function (error) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Ошибка...',
                            text: 'Отзыв не опубликован',
                        });
                    }
                });
            });
        },
        allowOutsideClick: false,
        allowEscapeKey: false
    });
}
//

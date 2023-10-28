
let ordersData;
let productNameData;

$(document).ready(function () {
    getOrders();
});

function getOrders() {
    let orders = document.getElementById("orders");

    $.ajax({
        url: '/Customer/Orders/GetOrders',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            ordersData = response.data;
            productNameData = response.prodIdAndName;
            orders.innerHTML = `${generateOrderCards()}`;
        },
    });
}

function generateOrderCards() {
    let html = ``;

    for (var order of ordersData) {

        let countProduct = 0;
        let sumPrice = 0;
        var tableOrder = ``;
        var showProduct;

        if (order.shippedProducts != null) {
            showProduct = order.shippedProducts;
        }
        else {
            showProduct = order.orderedProducts;
        }

        for (var product of showProduct) {
            tableOrder += `
            <tbody class="table-group-divider">
                <tr>
                    <td>${product.id}</td>
                    <td id="nameProduct_${product.id}">
                        <a href="/Customer/Home/Details?productId=${product.id}">${productNameData[product.id]}</a>
                    </td>
                    <td>${product.price} ₽ </td>
                    <td>${product.count}</td>
                    <td><button  onclick="reviewProductHandler('${product.id}', '${productNameData[product.id]}')" class="btn btn-success">Отзыв</button></td>
                </tr>
            </tbody>
            `;

            countProduct += product.count;
            sumPrice += product.count * product.price;
        }


        html += `
                    <div class="mb-5">
                <div class="form-group border border-1 rounded rounded-3 p-4">
                    <div class="d-inline-flex align-items-center">
                        <span class="mr-3">Номер заказа: </span>
                        <div class="rouden rounded-1 text-bg-primary m-1">
                            ${order.orderId}
                        </div>
                    </div>

                    <div class="d-inline-flex align-items-center">
                        <span class="mr-3">Статус заказа: </span>
                        <div class="rouden rounded-1 text-bg-primary m-1">${order.orderStatus}</div>
                    </div>

                    <div class="">
                        <label>Дата и время заказа: ${order.purchaseDate} по МСК</label>
                    </div>

                    <table class="table" id="tableProduct_${order.orderId} style="display: none;"">
                        <thead>
                            <tr>
                                <th scope="col">Id</th>
                                <th scope="col">Название</th>
                                <th scope="col">Цена</th>
                                <th scope="col">Количество</th>
                                <th scope="col"></th>
                            </tr>
                        </thead>
                        ${tableOrder}
                    </table>
                    <div>
                        <label>Количество товара: ${countProduct}</label>
                        <label class="m-3">Стоимость: ${sumPrice} ₽ </label>
                    </div>
                    <div class="form-group">
                        <label>Оплачено:</label>
                        <label>${order.purchaseAmount} ₽ </label>
                        ${order.refundAmount != 0 ? `<label class="bg-warning rounded rounded-3">Сумма возврата: ${order.refundAmount} ₽</label>` : ``}
                    </div>
                    <div>
                        Доставка:
                        ${order.isCourierDelivery ? "Курьером" : "В пункт выдачи"}
                        <div class="form-group">
                            ${order.isCourierDelivery ? `Адрес доставки: ${order.city}, ${order.street} ${order.houseNumber}` : ""}
                            <div class="form-group">
                                Данные получателя:
                                ${order.receiverName}
                                ${order.receiverLastName}
                                ${order.phoneNumber}
                            </div>
                        </div>
                    </div>
                    <hr/>
                    <button onclick="reviewOrderHandler('${order.orderId}')" class="btn btn-success">Отзыв о заказе</button>
                </div>
            </div>
        `;
    }
    return html;
}

function reviewProductHandler(productId, productName) {

    let formProductReview = `
                   <form id="formReviewProduct" enctype="multipart/form-data">
                          <div class="form-row">                            
                            <div class="form-group col-md-12 mb-1">
                              <label for="productRating">Оценка товара:</label>
                              <select class="form-control" id="productRating" name="ProductRating" required>
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
                              <label for="review">Отзыв:</label>
                              <textarea class="form-control" id="review" name="Review" required></textarea>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                              <label for="photo">Фотография:</label>
                              <input type="file" class="form-control-file" id="photo" name="Photo">
                            </div>
                          </div>
                        </form>
                        `;

    Swal.fire({
        title: `Отзыв на ${productName}`,
        html: formProductReview,
        showCancelButton: true,
        confirmButtonText: 'Сохранить',
        cancelButtonText: 'Отмена',
        preConfirm: () => {
            return new Promise((resolve, reject) => {
                const formData = new FormData(document.getElementById('formReviewProduct'));
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




$(document).ready(function () {
    generateDataOrder();
});


function generateDataOrder() {

    const dataOrders = document.getElementById("dataOrder");

    let cardOrder = ``;
    let productData = ``;

    $.ajax({
        url: '/Picker/OrderPicking/GetAssemblyOrders',
        type: 'GET',
        dataType: 'json',

        success: function (response) {
            let dataOrder = response.data;

            for (let order of dataOrder) {
                for (let product of order.products) {
                    productData +=
                        `
                            <td>${product.productId}</td>
                            <td>${product.name}</td>
                            <td>${product.author}</td>
                            <td>${product.count}</td>
                            <td>
                                <button class="btn btn-success">Button 1</button>
                                <button class="btn btn-success">Button 2</button>
                            </td>
                        `;
                }

                cardOrder +=
                    `
                          <div class="container m-4">
                            <div class="form-group border border-1 rounded rounded-1 p-4">
                                <div class="d-inline-flex align-items-center">
                                    <span class="mr-3">Номер заказа: </span>
                                    <div class="rouden rounded-1 text-bg-primary m-1">
                                        ${order.orderId}
                                    </div>
                                </div>
                                <div>
                                    <label>
                                        Дата и время заказа: ${order.purchaseDate} по МСК
                                    </label>
                                </div>
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th scope="col">Id</th>
                                            <th scope="col">Название</th>
                                            <th scope="col">Автор</th>
                                            <th scope="col">Количество</th>
                                            <th scope="col">Собрано</th>
                                        </tr>
                                                                              
                                    </thead>
                                    <tbody>
                                     ${productData} 
                                    </tbody>
                                </table>
                                <div>
                                   <button class="btn btn-success">Заказ полностью собран</button>
                                   <button class="btn btn-success">Не все товары есть в наличии</button>
                                </div>
                            </div>
                            <hr class="mt-5 mb-5"/>
                        </div>
                    `;
            }

            dataOrders.innerHTML = `${cardOrder}`
        },

        error: function (error) {
            Swal.fire({
                icon: 'error',
            })
        }
    })
}
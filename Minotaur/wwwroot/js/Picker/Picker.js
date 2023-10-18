
$(document).ready(function () {
    generateDataOrder();
});

let dataOrder;

function generateDataOrder() {

    const dataOrders = document.getElementById("dataOrder");

    $.ajax({
        url: '/Picker/OrderPicking/GetAssemblyOrders',
        type: 'GET',
        dataType: 'json',

        success: function (response) {
            dataOrder = response.data;
            let cardOrder = ``;


            for (let order of dataOrder) {
                let productData = ``;
                let stockData = ``;

                for (let product of order.products) {

                    let stockData = ``;

                    for (let dataProduct of order.dataStock) {
                        if (dataProduct.productId === product.productId) {
                            stockData += `
                                Полка: ${dataProduct.shelfNumber}
                                Количество: ${dataProduct.count}
                                <br>
                            `;
                        }
                    }

                    productData +=
                        `
                            <tr>
                                <td>${product.productId}</td>
                                <td>${product.name}</td>
                                <td>${product.author}</td>
                                <td>${product.count}</td>
                                <td>
                                    <button onclick="addToOrderAssembly('${order.orderId}', '${product.productId}')" id="selectBtn_${order.orderId}_${product.productId}" class="btn btn-outline-success border-0 bi bi-square"></button>
                                </td>
                                <td> 
                                   ${stockData}
                                </td>
                            </tr>
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
                                            <th scope="col">Требуемое количество</th>
                                            <th scope="col">Собрано</th>
                                            <th scrope="col">На складе</th>ббб
                                        </tr>
                                                                              
                                    </thead>
                                    <tbody>
                                     ${productData} 
                                    </tbody>
                                </table>
                                <div>
                                   <button onclick="confirmOrderReadiness('${order.orderId}')" class="btn btn-success">Подтвердить готовность товара</button>
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

function addToOrderAssembly(orderId, productId) {
    const order = dataOrder.find(order => order.orderId === orderId).products;
    if (order == null) {
        Swal.fire('Заказ не найден. Товар отметить нельзя.')
        return;
    }

    const product = order.find(product => product.productId === parseInt(productId));
    if (product == null) {
        Swal.fire('Товар не найден')
        return;
    }

    product.isChecked = true;

    let btnProductChecked = document.getElementById(`selectBtn_${orderId}_${productId}`);

    if (btnProductChecked.classList.contains('bi-square')) {
        btnProductChecked.classList.remove('bi-square');
        btnProductChecked.classList.add('bi-check-square');
    }
    else {
        btnProductChecked.classList.remove('bi-check-square');
        btnProductChecked.classList.add('bi-square');
    }
}

function confirmOrderReadiness(orderId) {

    const order = dataOrder.find(order => order.orderId === orderId).products;

    if (order == null) {
        Swal.fire('Заказ не найден.');
        return;
    }

    let flagFullReadyOrder = true;

    let missingProduct = [];

    for (let product of order) {
        if (product.isChecked != true) {
            flagFullReadyOrder = false;
            missingProduct.push({
                ProductId: product.productId,
                Count: product.count,
                ProductName: product.name
            });
        }
    }
    jsonMissingProduct = JSON.stringify(missingProduct);

    console.log(jsonMissingProduct);

    if (flagFullReadyOrder == true) {
        Swal.fire({
            title: 'Заказ полностью укомплектован',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: 'Отправить пункт отправки',
            denyButtonText: `Отмена`,
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire('Оправлено!', '', 'success');
                $.ajax({
                    url: '/Picker/OrderPicking/SendCollectedOrder?orderId=' + orderId,
                    type: 'POST',

                    success: function (response) {
                        Swal.fire('Операция прошла успешно. Передайте заказ в отдел отправки.');
                    },
                    error: function (error) {
                        Swal.fire(`${error.responseText}`);
                    },
                });
            }
        });
    }

    if (flagFullReadyOrder == false) {
        Swal.fire({
            title: 'Заказ не укомплентован',
            text: 'Будет отправлено уведомлению пользователю на согласие об получение заказа в неукомлентованом состоянии.',
            showDenyButton: true,
            confirmButtonText: 'Отправить уведомление',
            denyButtonText: `Отмена`,
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Picker/OrderPicking/SendCollectedOrder?orderId=' + orderId + '&missingProduct=' + jsonMissingProduct,
                    type: 'POST',
                    success: function (response) {
                        Swal.fire('Уведомление отправлено');
                    },
                    error: function (error) {
                        Swal.fire(`${error.responseText}`);
                    },
                });
            }
        });
    }
}

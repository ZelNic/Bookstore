
$(document).ready(function () {
    generateDataOrder();
});

let dataOrder;

const StatusApproved_1 = "Одобренный";
const StatusInProcess_2 = "Сборка заказа";
const BuyerAgreesNeedSend_8 = "Покупатель согласен на получение неполного заказа. Ожидается передача в отдел отправки";
function generateDataOrder() {

    const dataOrders = document.getElementById("dataOrder");

    $.ajax({
        url: '/Picker/OrderPicking/GetOrders',
        type: 'GET',
        dataType: 'json',

        success: function (response) {
            dataOrder = response.data;
            let cardOrder = ``;


            for (let [index, order] of dataOrder.entries()) {
                let productData = ``;
                let stockData = ``;

                let functionForOrder = ``;

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

                    switch (order.orderStatus) {
                        case BuyerAgreesNeedSend_8:
                            functionForOrder = `
                             <div id="waitingAssemblyDiv_${order.orderId}">
                                <button onclick="takeOrderOnAssembly('${order.orderId}','${index}')" class="btn btn-success">Взять заказ на сборку</button>
                             </div>
                             <div id="onAssembly_${order.orderId}" hidden>
                                <button onclick="confirmOrderReadiness('${order.orderId}', '${index}')" class="btn btn-success">Подтвердить готовность товара</button>
                                <button onclick="cancelAssemblyOrder('${order.orderId}','${index}')" class="btn btn-success">Отменить сборку заказа</button>
                             </div>`;
                            break;

                        case StatusApproved_1:
                            functionForOrder = `
                             <div id="waitingAssemblyDiv_${order.orderId}">
                                <button onclick="takeOrderOnAssembly('${order.orderId}','${index}')" class="btn btn-success">Взять заказ на сборку</button>
                             </div>
                             <div id="onAssembly_${order.orderId}" hidden>
                                <button onclick="confirmOrderReadiness('${order.orderId}', '${index}')" class="btn btn-success">Подтвердить готовность товара</button>
                                <button onclick="cancelAssemblyOrder('${order.orderId}','${index}')" class="btn btn-success">Отменить сборку заказа</button>
                             </div>`;
                            break;
                        case StatusInProcess_2:
                            functionForOrder = ` 
                             <div id="onAssembly_${order.orderId}">
                                <button onclick="confirmOrderReadiness('${order.orderId}', '${index}')" class="btn btn-success">Подтвердить готовность товара</button>
                                <button onclick="cancelAssemblyOrder('${order.orderId}','${index}')" class="btn btn-success">Отменить сборку заказа</button>
                             </div>`;
                            break;
                    }

                    productData +=
                        `
                            <tr>
                                <td>${product.productId}</td>
                                <td>${product.name}</td>
                                <td>${product.author}</td>
                                <td>${product.count}</td>
                                <td>
                                    <input type="number" value="0" id="inputCount_${order.orderId}_${product.productId}" style="width: 70px; height: 30px;">
                                </td>
                                <td>
                                    <button onclick="addToOrderAssembly('${order.orderId}', '${product.productId}', document.getElementById('inputCount_${order.orderId}_${product.productId}').value)" 
                                     id="selectBtn_${order.orderId}_${product.productId}" class="btn btn-outline-success border-0 bi bi-square"></button>
                                </td>
                                <td> 
                                    ${stockData}
                                </td>
                            </tr>
                        `;
                }

                cardOrder +=
                    `
                          <div class="mt-4 mb-4">
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
                                    <label id="status_${index}">
                                        Статус заказа: ${order.orderStatus}
                                    </label>
                                </div>
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th scope="col">Id</th>
                                            <th scope="col">Название</th>
                                            <th scope="col">Автор</th>
                                            <th scope="col">Требуемое кол-во</th>
                                            <th scrope="col">В посылке</th>
                                            <th scope="col">Собрано</th>
                                            <th scrope="col">На складе</th>
                                        </tr>
                                                                              
                                    </thead>
                                    <tbody>
                                     ${productData} 
                                    </tbody>
                                </table>
                                <div>
                                   ${functionForOrder}                                
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
                text: error.responseText,
            })
        }
    })
}

function cancelAssemblyOrder(orderId, index) {
    $.ajax({
        url: '/Picker/OrderPicking/CancelAsseblyOrder?orderId=' + orderId,
        type: 'GET',
        success: function (response) {
            let divWaiting = document.getElementById(`waitingAssemblyDiv_${orderId}`);
            let onAssembly = document.getElementById(`onAssembly_${orderId}`);
            let labelStatus = document.getElementById(`status_${index}`)

            dataOrder[index].orderStatus = "Одобренный";
            labelStatus.textContent = `Статус заказа: ${dataOrder[index].orderStatus}`;
            divWaiting.removeAttribute("hidden");
            onAssembly.setAttribute("hidden", "true");
        },
        error: function (error) {
            Swal.fire(error.responseText)
        }
    })
}

function takeOrderOnAssembly(orderId, index) {
    $.ajax({
        url: '/Picker/OrderPicking/TakeOrderOnAssebly?orderId=' + orderId,
        type: 'GET',
        success: function (response) {
            let divWaiting = document.getElementById(`waitingAssemblyDiv_${orderId}`);
            let onAssembly = document.getElementById(`onAssembly_${orderId}`);
            let labelStatus = document.getElementById(`status_${index}`)

            dataOrder[index].orderStatus = "Сборка заказа";
            labelStatus.textContent = `Статус заказа: ${dataOrder[index].orderStatus}`;
            divWaiting.setAttribute("hidden", "true");
            onAssembly.removeAttribute("hidden");
        },
        error: function (error) {
            Swal.fire(error.responseText)
        }
    })
}

function addToOrderAssembly(orderId, productId, count) {

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

    let btnProductChecked = document.getElementById(`selectBtn_${orderId}_${productId}`);

    if (count > product.count || count < 0) {
        let inputCount = document.getElementById(`inputCount_${orderId}_${productId}`);
        inputCount.value = product.count;
    }


    if (product.isChecked == true) {
        product.isChecked = false;
        btnProductChecked.classList.remove('bi-check-square');
        btnProductChecked.classList.add('bi-square');
    }
    else {
        product.isChecked = true;
        btnProductChecked.classList.remove('bi-square');
        btnProductChecked.classList.add('bi-check-square');
    }
}

function confirmOrderReadiness(orderId, index) {

    const order = dataOrder.find(order => order.orderId === orderId).products;

    if (order == null) {
        Swal.fire('Заказ не найден.');
        return;
    }

    let flagFullReadyOrder = true;

    let missingProduct = [];


    for (let product of order) {
        let inputCount = document.getElementById(`inputCount_${orderId}_${product.productId}`);

        if (product.isChecked != true || inputCount.value != product.count) {
            flagFullReadyOrder = false;
            missingProduct.push({
                Id: product.productId,
                ProductName: product.name,
                Price: product.price,
                Count: inputCount.value,
            });
        }
    }

    let jsonMissingProduct;
    if (missingProduct.length > 0) {
        jsonMissingProduct = JSON.stringify(missingProduct);
    }


    if (flagFullReadyOrder == true) {
        Swal.fire({
            title: 'Заказ полностью укомплектован',
            showCancelButton: true,
            confirmButtonText: 'Отправить пункт отправки',
            cancelButtonText: `Отмена`,
        }).then((result) => {
            if (result.isConfirmed) {
                Swal.fire('Оправлено!', '', 'success');
                $.ajax({
                    url: `/Picker/OrderPicking/SendCollectedOrder?orderId=${orderId}&missingProduct=Отсутствуют`,
                    type: 'POST',

                    success: function (response) {
                        Swal.fire('Операция прошла успешно. Передайте заказ в отдел отправки.');
                        generateDataOrder();
                    },
                    error: function (error) {
                        Swal.fire(error.responseText);
                    },
                });
            }
        });
    }
    else {

        if (dataOrder[index].missingItems != null) {
            Swal.fire({
                title: 'Заказ укомплектован',
                showCancelButton: true,
                confirmButtonText: 'Отправить пункт отправки',
                cancelButtonText: `Отмена`,
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire('Оправлено!', '', 'success');
                    $.ajax({
                        url: '/Picker/OrderPicking/SendCollectedOrder?orderId=' + orderId + '&missingProduct=' + jsonMissingProduct,
                        type: 'POST',

                        success: function (response) {
                            Swal.fire('Операция прошла успешно. Передайте заказ в отдел отправки.');
                            generateDataOrder();
                        },
                        error: function (error) {
                            Swal.fire(error.responseText);
                        },
                    });
                }
            });
        }
        else {
            Swal.fire({
                title: 'Заказ не укомплентован',
                text: 'Будет отправлено уведомление пользователю о согласии на получение заказа в некомплектном состоянии.',
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
                            generateDataOrder();
                        },
                        error: function (error) {
                            Swal.fire(error.responseText);
                        },
                    });
                }
            });
        }
    }
}

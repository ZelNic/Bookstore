
$(document).ready(function () {
    getInfomationAboutBuyer();
});

let orderData;
let listPickupPoints;
let personalWalletAndPurchaseAmount;

function getInfomationAboutBuyer() {
    let infomationData = document.getElementById("infomationData");

    $.ajax({
        url: '/Purchase/Purchase/GetInfomationAboutBuyer',
        type: 'GET',
        dataType: 'json',
        success: function (response) {

            orderData = response.data;

            infomationData.innerHTML = `
                        <div class="form-group p-3">
                            <h6>Данные получателя</h6>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5">Имя</label>
                                <input id="receiverName" value="${orderData.receiverName}" onblur="changeDataBuyer('receiverName', document.getElementById('receiverName').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5">Фамилия</label>
                                <input id="receiverLastName" value="${orderData.receiverLastName}" onblur="changeDataBuyer('receiverLastName', document.getElementById('receiverLastName').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5">Номер телефона</label>
                                <input id="phoneNumber" value="${orderData.phoneNumber}" onblur="changeDataBuyer('phoneNumber', document.getElementById('phoneNumber').value)" class="border border-1 rounded rounded-1" type="number" />
                            </div>
                            <hr />
                            <h6>Данные доствки</h6>
                            <div>
                                <label>Способ доставки</label>
                                <select id="mySelect" class="rounded" onchange="setTypeDelivery(this.value)" required>
                                    <option value="" selected disabled>Выбрать</option>
                                    <option value="false">Пункт выдачи</option>
                                    <option value="true">Курьером</option>
                                </select>
                                <div id="deliveryData"></div>                                
                                <div id="btnPayment"><div>
                            </div>
                            
                        </div>
            `
        },
        error: function (error) {
            console.log(error.responseJSON.error);
        }
    });
}

function getOrderPickupPoint() {

    var orderPickupPoint = document.getElementById("orderPickupPoint");
    $.ajax({
        url: '/API/API/GetOrderPickupPoint',
        type: 'GET',
        dataType: 'json',

        success: function (response) {
            listPickupPoints = response.data;

            for (var pickupPoint of listPickupPoints) {
                var option = document.createElement("option");
                option.value = pickupPoint.pointId;
                option.textContent = pickupPoint.city + ", " + pickupPoint.street + " " + pickupPoint.buildingNumber;
                orderPickupPoint.appendChild(option);
            }
        }
    });
}

function getPersonalWalletAndPurchaseAmount(callback) {
    $.ajax({
        url: '/Purchase/Purchase/GetPersonalWalletAndPurchaseAmount',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            callback(null, response.data);
        },
        error: function (error) {
            callback(error);
        },
    });
}
function makePayment() {
    getPersonalWalletAndPurchaseAmount(function (error, personalWalletAndPurchaseAmount) {
        if (error) {
            Swal.fire({
                icon: 'error',
                title: 'Ошибка. Обратитесь в поддержку.',
            });
            return;
        }

        Swal.fire({
            title: 'Оплатить?',
            icon: 'question',
            text: `На балансе ${personalWalletAndPurchaseAmount.sumOnWallet}. Стоимость заказа ${personalWalletAndPurchaseAmount.purchaseAmount}.`,
            showCancelButton: true,
            confirmButtonText: 'Оплатить',
            cancelButtonText: 'Отмена операции',
            showDenyButton: true,
            denyButtonText: 'Пополнить счет',
        }).then((result) => {
            if (result.isConfirmed) {
                orderData.purchaseAmount = personalWalletAndPurchaseAmount.purchaseAmount;
                let orderD = JSON.stringify(orderData);

                $.ajax({
                    url: '/Purchase/Purchase/Payment?dataDelivery=' + orderD,
                    type: 'POST',
                    success: function (response) {

                        let timerInterval
                        Swal.fire({
                            title: 'Оплата прошла успешно',
                            timer: 2000,
                            timerProgressBar: true,
                            didOpen: () => {
                                Swal.showLoading()
                                const b = Swal.getHtmlContainer().querySelector('b')
                                timerInterval = setInterval(() => {
                                    b.textContent = Swal.getTimerLeft()
                                }, 100)
                            },
                            willClose: () => {
                                clearInterval(timerInterval)
                            }
                        }).then((result) => {

                            if (result.dismiss === Swal.DismissReason.timer) {
                                window.location.href = '/Customer/Orders/Index';
                            }
                        })
                    },
                    error: function (error) {
                        Swal.fire(error.responseJSON.error);
                    }
                });
            } else if (result.isDenied) {
                $.ajax({
                    url: '/Purchase/Purchase/AddMoneyOnWallet?sum=' + personalWalletAndPurchaseAmount.purchaseAmount,
                    type: 'POST',
                    success: function (response) {
                        Swal.fire('Счет успешно пополнен');
                    },
                    error: function (error) {
                        Swal.fire('Пополнение не прошло');
                    }
                });
            } else {
                console.log('Отмена операции');
            }
        });
    });
}


function checkData() {

    let propertyBuyer = [orderData.receiverName, orderData.receiverLastName, orderData.phoneNumber]
    let propertyDataCourier = [orderData.city, orderData.street, orderData.houseNumber]
    for (var p of propertyBuyer) {
        if (p.length === 0) {
            Swal.fire('Необходимо заполнить все поля');
            return;
        }
    }

    if (orderData.isCourierDelivery == false) {
        if (orderData.OrderPickupPointId == null) {
            Swal.fire('Необходимо заполнить все поля');
            return;
        }
    } else {
        for (let p of propertyDataCourier) {
            if (p.length === 0) {
                Swal.fire('Необходимо заполнить все поля');
                return;
            }
        }
    }
    makePayment();
}

function activeBtnPayment(isActive) {
    let btnPayment = document.getElementById("btnPayment");

    if (isActive == false) {
        btnPayment.innerHTML = ``;
    }
    else {
        btnPayment.innerHTML = `<button class="btn btn-success mt-1" onclick="checkData()">Оплатить</button>`;
    }
}

function setOrderPickupPoint(ppId) {
    orderData.OrderPickupPointId = ppId;
}

function setTypeDelivery(type) {

    activeBtnPayment(false);

    let deliveryData = document.getElementById("deliveryData");
    if (type === "false") {
        orderData.isCourierDelivery = false;
        deliveryData.innerHTML = `
            <div class="mt-2">
                <label>Выберите пункт выдачи</label>
                <select id="orderPickupPoint" class="rounded" onchange="setOrderPickupPoint(document.getElementById('orderPickupPoint').value); activeBtnPayment(true)" required>
                    <option value="" selected disabled>Выбрать</option>
                </select>
            </div>
            <hr />
        `;
        getOrderPickupPoint();
    } else if (type === "true") {

        orderData.isCourierDelivery = true;
        activeBtnPayment(true);

        if ((orderData.city == null) || (orderData.street == null) || (orderData.houseNumber == null)) {
            orderData.city = "";
            orderData.street = "";
            orderData.houseNumber = "";
        }

        deliveryData.innerHTML = `       
            <div>
                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Город</label>
                <input id="city" value="${orderData.city}" onblur="changeDataBuyer('city', document.getElementById('city').value)" class="border border-1 rounded rounded-1" type="text" required />
            </div>
            <div>
                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Улица</label>
                <input id="street" value="${orderData.street}" onblur="changeDataBuyer('street', document.getElementById('street').value)" class="border border-1 rounded rounded-1" type="text" required/>
            </div>                           
            <div>
                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Номер дома</label>
                <input id="houseNumber" value="${orderData.houseNumber}" onblur="changeDataBuyer('houseNumber', document.getElementById('houseNumber').value)" class="border border-1 rounded rounded-1" type="text" required/>
            </div>
            <hr />
        `;
    }
    else { return; }
}

function changeDataBuyer(attribute, value) {
    let property = document.getElementById(attribute);

    orderData[attribute] = value;

    property.setAttribute("value", orderData[attribute]);
}

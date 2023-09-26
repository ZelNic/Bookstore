
$(document).ready(function () {
    getInfomationAboutBuyer();
});
var buyerData;
function getInfomationAboutBuyer() {
    var infomationData = document.getElementById("infomationData");

    $.ajax({
        url: '/Purchase/Purchase/GetInfomationAboutBuyer',
        type: 'GET',
        dataType: 'json',
        success: function (response) {

            buyerData = response.data;

            infomationData.innerHTML = `
                        <div class="form-group p-3">
                            <h6>Данные получателя</h6>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Имя</label>
                                <input id="receiverName" value="${buyerData.receiverName}" onblur="change('firstName', document.getElementById('receiverName').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Фамилия</label>
                                <input id="receiverLastName" value="${buyerData.receiverLastName}" onblur="change('lastName', document.getElementById('receiverLastName').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Номер телефона</label>
                                <input id="phoneNumber" value="${buyerData.phoneNumber}" onblur="change('phoneNumber', document.getElementById('phoneNumber').value)" class="border border-1 rounded rounded-1" type="number" />
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
                                <div id="deliveryData">
                                </div>
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
    var html = "";
    $.ajax({
        url: '/Purchase/Purchase/GetInfomationAboutBuyer',
        type: 'GET',
        dataType: 'json',



        success: function (response) {

            orderPickupPoint.innerHTML = `
                <select id="orderPickupPoint2" class="rounded" onchange="setTypeDelivery(this.value)" required>
                    <option value="" selected disabled>Выбрать</option>
                    
                </select>
            `;
        }
    });

    return html
}


function setTypeDelivery(type) {
    var deliveryData = document.getElementById("deliveryData");
    if (type === "false") {
        buyerData.isCourierDelivery = false;
        deliveryData.innerHTML = `
                            <div>
                                ${getOrderPickupPoint()};
                            </div>
                            <hr />
        `;
    } else if (type === "true") {
        buyerData.isCourierDelivery = true;

        if ((buyerData.city == null) || (buyerData.street == null) || (buyerData.houseNumber == null)) {
            buyerData.city = "";
            buyerData.street = "";
            buyerData.houseNumber = "";
        }

        deliveryData.innerHTML = `       
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Город</label>
                                <input id="city" value="${buyerData.city}" onblur="change('city', document.getElementById('city').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Улица</label>
                                <input id="street" value="${buyerData.street}" onblur="change('street', document.getElementById('street').value)" class="border border-1 rounded rounded-1" type="text" />
                            </div>                           
                            <div>
                                <label class="ms-1 my-1 text-dark fs-5" style="display: block;">Номер дома</label>
                                <input id="houseNumber" value="${buyerData.houseNumber}" onblur="change('houseNumber', document.getElementById('houseNumber').value)" class="border border-1 rounded rounded-1" type="number" />
                            </div>
                            <hr />
        `;
    }
    else { return; }
}



function changeDataBuyer(attribute, value) {
    var property = document.getElementById(attribute);

    switch (attribute) {
        case ("receiverName"):
            buyerData.receiverName = value;
            break;
        case ("receiverLastName"):
            buyerData.receiverLastName = value;
            break;
        case ("phoneNumber"):
            buyerData.phoneNumber = value;
            break;
        default: console.log("Error")
            break;
    }
    property.setAttribute("value", buyerData.attribute);
}

function sendBuyerData() {

    $.ajax({
        url: '/Purchase/Purchase/FillDeliveryDate?buyerData=' + buyerData,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            window.location.href = '/Purchase/Purchase/FillDeliveryDate';
        }

    });
}
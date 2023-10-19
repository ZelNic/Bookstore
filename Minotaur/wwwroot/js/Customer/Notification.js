
let notificationData;
let divNotifData;

const NotificationIncompleteOrderType = "Неполный заказ. Требуется подтверждение.";

$(document).ready(function () {
    getDataNotification();
});


function getDataNotification() {
    divNotifData = document.getElementById("notificationData");
    $.ajax({
        url: '/Customer/NotificationCustomer/GetDataNotification',
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            notificationData = response.data;
            divNotifData.innerHTML = generateCardNotitfications();
        },
        error: function (error) {
            divNotifData.innerHTML = `${error.responseText}`;
        },
    });
}

function generateCardNotitfications() {
    let cardsNotification = "";
    for (let notification of notificationData) {
        cardsNotification += `
       <div class="border border-1 rounded rounded-1 p-3 col-7 m-2">
            <style>
                .mr {
                    margin-right: 5px;
                }
            </style>
            <div class="d-inline-flex align-items-center position-relative">
                <label class="">Номер заказа: </label>
                <div class="rouden rounded-1 text-bg-primary col-auto mr">
                    ${notification.orderId}
                </div>
                <div class="rouden rounded-1 text-bg-warning col-auto">
                    ${notification.sendingTime.toString()} по МСК
                </div>
            </div>
            <div class="col-1">
            </div>
            <div>
                ${notification.text}
            </div>            
                ${notification.typeNotification == NotificationIncompleteOrderType ? `
                <button onclick="answerOrder('${notification.id}', false)" class="btn btn-sm btn-danger bi bi-x-square-fill"> Отказ </button>
                <button onclick="answerOrder('${notification.id}', true)" class="btn btn-sm btn-success bi bi-check-square-fill"> Согласие</button>
                <button onclick="goToOrder()" class="btn btn-sm btn-warning">Перейти к заказу</button>
                ` : `
                <button class="btn btn-sm btn-secondary bi bi-x-square"></button>
                <button onclick="goToOrder()" class="btn btn-sm btn-warning">Перейти к заказу</button>
                <button class="btn btn-sm btn-success">Получить код выдачи</button>
                `}
        </div>
        `;
    }
    return cardsNotification;
}

function goToOrder() {
    window.location.href = '/Customer/Orders/Index';
}
function answerOrder(notificationId, answer) {
    $.ajax({
        url: '/Customer/NotificationCustomer/SendReplyIncompleteOrder?notificationId=' + notificationId + "&isAgree=" + answer,
        method: 'POST',
        success: function (response) {
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
                title: 'Signed in successfully'
            })
            getDataNotification();
        },
        error: function (error) {
            Swal.fire({
                icon: 'error',
                title: 'Ошибка',
                text: error.responseText,
                footer: 'Обновите страницу'
            })
        }
    })
}
function hiddenNotification(notificationId) {
    $.ajax({
        url: '/Customer/NotificationCustomer/HideNotification?notificationId=' + notificationId,
        method: 'POST',
        success: function (response) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'success',
                title: 'Уведомление скрыто'
            })
        },
        error: function (error) {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer)
                    toast.addEventListener('mouseleave', Swal.resumeTimer)
                }
            })

            Toast.fire({
                icon: 'error',
                title: error.responseText,
            })
        }
    });


}
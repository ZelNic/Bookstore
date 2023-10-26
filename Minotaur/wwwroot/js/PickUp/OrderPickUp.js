
let dataByOrder;

function findOrder() {

    let idOrder = document.getElementById("orderSearchString").value;

    $.ajax({
        url: `/PickUp/Order/GetDataByOrder?id=` + idOrder,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            dataByOrder = response.data;
            generateCardOrder();
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
                title: error.responseText,
            })
        }
    });

}
function generateCardOrder() {
    let divOrderData = document.getElementById("orderData");

    let countProduct = 0;

    let functionBtn = ``;

    let dataProducts = ``;

    for (var product of dataByOrder.products) {
        dataProducts += `
                <tbody>
                    <td scope="col">${product.id}</td>
                    <td scope="col">${product.productName}</td>
                    <td scope="col">${product.count}</td>
                    </tbody>
                `
        countProduct += product.count
    }

    switch (dataByOrder.status) {

        case "Отправленный":
            functionBtn = `  <button onclick="confirmOrderArrival('${dataByOrder.id}')" class="btn btn-success mt-2">
                                     Подтвердить прибытие заказа в пункт выдачи*
                                     </button>
                                     <h8 class="opacity-50">*отправка уведомления клиенту о прибитые посылки</h8>`;
            break

        case "Доставлен в пункт выдачи":
            functionBtn = `<div>
                            <button onclick="sendConfirmationCode('${dataByOrder.id}')"
                                class="btn btn-success mt-2">
                                Отправить код подтверждения
                            </button>
                            </div>
                            <div>
                                <button onclick="enterConfirmationCode('${dataByOrder.id}')"
                                    class="btn btn-success mt-2">
                                    Ввести код подтверждения
                                </button>
                            </div> `;
            break;

        default: break;

    }

    divOrderData.innerHTML = `     
                                <div class="container m-4">
                                    <div class="form-group border border-1 rounded rounded-1 p-4">
                                        <div class="d-inline-flex align-items-center">
                                            <span class="mr-3">Номер заказа: </span>
                                            <div class="rouden rounded-1 text-bg-primary m-1">
                                                ${dataByOrder.id}
                                            </div>
                                        </div>
                                        <div class="d-inline-flex align-items-center">
                                            <span class="mr-3">Статус заказа: </span>
                                            <div class="rouden rounded-1 bg-@colorStatus m-1">${dataByOrder.status}</div>
                                        </div>
                                        <div>
                                            <label>
                                                Дата и время заказа:
                                                ${dataByOrder.time}
                                                по МСК
                                            </label>
                                        </div>   
                                        <table class="table">
                                            <thead>
                                                <tr>
                                                    <th scope="col">Id</th>
                                                    <th scope="col">Название</th>
                                                    <th scope="col">Количество</th>
                                                </tr>
                                            </thead>
                                           ${dataProducts}
                                        </table>
                                        <div>
                                            <label>Количество товаров: ${countProduct}</label>
                                        </div>
                                        <div class="form-group">
                                        </div>                                        
                                        <div>
                                        <label>Данные получателя: ${dataByOrder.receiverData}</label>                                        
                                        </div>
                                        <div>
                                        ${functionBtn}
                                        </div>
                                    </div>
                                    <hr class="mt-5 mb-5" />
                                </div>`;
}

function confirmOrderArrival(id) {
    $.ajax({
        url: `/PickUp/Order/ConfirmOrderArrival?id=${id}`,
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
                title: 'Уведомление отправлено получателю'
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
                icon: 'error',
                title: error.responseText
            })
        }
    });
}

function sendConfirmationCode(id) {
    $.ajax({
        url: `/PickUp/Order/SendConfirmationCode?id=${id}`,
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
                title: 'Код отправлен получателю'
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
                icon: 'error',
                title: error.responseText
            })
        }
    });
}

async function enterConfirmationCode(id) {
    const { value: code } = await Swal.fire({
        title: 'Введите код получения',
        input: 'number',
        inputPlaceholder: 'Код получения'
    })

    if (code) {
        $.ajax({
            url: `/PickUp/Order/CheckVerificationCode?id=${id}&confirmationCode=${code}`,
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
                    title: 'Код верный. Заказ завершен.'
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
                    icon: 'error',
                    title: error.responseText
                })
            }
        });
    }
   
}
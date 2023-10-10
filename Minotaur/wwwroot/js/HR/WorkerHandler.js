
let dataWorkerTable;
let dataWorker;

$(document).ready(function () {
    getDataByWorker();
});

function getDataByWorker() {
    dataWorkerTable = $('#tableWorker').DataTable({
        "ajax": { url: '/HR/Worker/GetDataByWorkers' },
        "columns": [
            { data: 'userId', "width": "10%" },
            { data: 'status', "width": "5%" },
            { data: 'lfs', "width": "25%" },
            { data: 'post', "width": "15%" },
            { data: 'officeName', "width": "15%" },
            { data: 'email', "width": "15%" },
            {
                data: 'workerId',
                render: function (data, type, row) {
                    return `<button onclick="editDataNewWorker(${data})" class="btn btn-warning">Редактировать</button>`;
                },
                "width": "15%"
            }
        ]
    });
}

async function enterEmailNewWorker() {

    const { value: email } = await Swal.fire({
        title: 'Добавить сотрудника',
        input: 'email',
        inputLabel: 'Электронная почта нового сотрудника',
        inputPlaceholder: 'Введите электронную почту нового сотрудника'
    })

    if (email) {
        editEnterDataUserByNewWorker(email);
    }
}


function editEnterDataUserByNewWorker(email) {

    $.ajax({
        url: '/HR/Worker/FindUserForHiring?email=' + email,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            let formUserData = `
                <form id="dataUser">     
                    <input type="text" value="${response.data.id}" name="Id" placeholder="Id" class="m-1" hidden>        
                    <input type="text" value="${response.data.firstName !== null ? response.data.firstName : ''}" name="FirstName" placeholder="Имя" class="m-1">                         
                    <input type="text" value="${response.data.lastName !== null ? response.data.lastName : ''}" name="LastName" placeholder="Фамилия" class="m-1">   
                    <input type="text" value="${response.data.surname !== null ? response.data.surname : ''}" name="Surname" placeholder="Отчество" class="m-1">                          
                    <input type="date" value="${response.data.dateofBirth !== null ? response.data.dateofBirth : ''}" name="DateofBirth" placeholder="Дата рождения" style="width: 218px;" class="m-1">                            
                    <input type="text" value="${response.data.region !== null ? response.data.region : ''}" name="Region" placeholder="Регион" class="m-1">                           
                    <input type="text" value="${response.data.city !== null ? response.data.city : ''}" name="City" placeholder="Город" class="m-1">                            
                    <input type="text" value="${response.data.street !== null ? response.data.street : ''}" name="Street" placeholder="Улица" class="m-1">                          
                    <input type="text" value="${response.data.houseNumber !== null ? response.data.houseNumber : ''}" name="HouseNumber" placeholder="Номер дома" class="m-1" >                            
                </form>
                `;

            Swal.fire({
                title: 'Данные пользователя',
                html: formUserData,
                showCancelButton: true,
                confirmButtonText: 'Сохранить',
                cancelButtonText: 'Отмена',
                preConfirm: () => {
                    return new Promise((resolve, reject) => {

                        const formData = new FormData(document.getElementById('dataUser'));
                        const jsonObject = {};

                        for (const [key, value] of formData.entries()) {
                            jsonObject[key] = value;
                        }

                        const jsonDataUser = JSON.stringify(jsonObject);

                        $.ajax({
                            url: '/HR/Worker/SaveDataUser?dataUser=' + jsonDataUser,
                            type: 'POST',
                            success: function (response) {

                                let timerInterval
                                Swal.fire({
                                    title: 'Данные сохранены',
                                    html: 'Переход к другой форме через <b></b>.',
                                    timer: 1500,
                                    timerProgressBar: true,
                                    didOpen: () => {
                                        Swal.showLoading()
                                        const b = Swal.getHtmlContainer().querySelector('b')
                                        timerInterval = setInterval(() => {
                                            b.textContent = Swal.getTimerLeft()
                                        }, 100)
                                    },
                                    willClose: () => {
                                        editDataNewWorker();
                                    }
                                }).then((result) => {
                                    if (result.dismiss === Swal.DismissReason.timer) {
                                        editDataNewWorker();
                                    }
                                });
                            },
                            error: function (error) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Ошибка...',
                                    text: 'Изменения не сохранились',
                                });
                            }
                        });
                    });
                }
            });
        },
        error: function (error) {
            Swal.fire({
                icon: 'error',
                title: 'Ошибка...',
                text: 'Не удалось найти пользователя',
            });
        }
    });
}


function editDataNewWorker(workerId = null) {
    console.log("воркер = ");
    console.log("воркер = " + workerId);
    $.ajax({
        url: '/HR/Worker/GetDataByWorkers' + workerId,
        type: 'GET',
        dataType: 'json',
        success: function (response) {


            let formWorkerData = `
            <form id="dataUser">     
                <input type="text" value="${response.data.userId}" name="UserId" hidden>        
                <input type="text" value="${response.data.status !== null ? response.data.status : ''}" name="Status" placeholder="Статус" class="m-1">                         
                <input type="text" value="${response.data.officeId !== null ? response.data.officeId : ''}" name="OfficeId" placeholder="Id Офиса" class="m-1">   
                <input type="text" value="${response.data.officeName !== null ? response.data.officeName : ''}" name="OfficeName" placeholder="Название офиса" class="m-1">                          
                <input type="text" value="${response.data.post !== null ? response.data.post : ''}" name="Post" placeholder="Должен" class="m-1">                            
                <input type="text" value="${response.data.admissionOrder !== null ? response.data.admissionOrder : ''}" name="AdmissionOrder" placeholder="Номер приказа о приеме на работу" class="m-1">                           
                <input type="text" value="${response.data.orderDismissal !== null ? response.data.orderDismissal : ''}" name="OrderDismissal" placeholder="Номер приказа об увольнения на работу" class="m-1"> 
            </form>`;

            Swal.fire({
                title: 'Данные работника',
                html: formWorkerData,
                showCancelButton: true,
                confirmButtonText: 'Сохранить',
                cancelButtonText: 'Отмена',
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        const formData = new FormData(document.getElementById('dataUser'));
                        const jsonObject = {};

                        for (const [key, value] of formData.entries()) {
                            jsonObject[key] = value;
                        }

                        const jsonDataUser = JSON.stringify(jsonObject);

                        $.ajax({
                            url: '/HR/Worker/RegisterNewWorker?dataUser=' + jsonDataUser,
                            type: 'POST',
                            success: function (response) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Операция прошла успешно',
                                    text: 'Работник добавлен в базу данных',
                                });
                            },
                            error: function (error) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Ошибка...',
                                    text: 'Изменения не сохранились',
                                });
                            }
                        });
                    });
                },
                allowOutsideClick: false,
                allowEscapeKey: false
            });
        }
    });
}

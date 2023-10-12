
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
                    return `<button onclick="editDataWorker('${data}')" class="btn btn-warning">Редактировать</button>`;
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
        editEnterWorkerDataUser(email);
    }
}


function editEnterWorkerDataUser(email) {

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
                            url: '/HR/Worker/UpsertEmployeeUserData?dataUser=' + jsonDataUser,
                            type: 'POST',
                            dataType: 'json',
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
                                        editDataWorker(response.data.workerId);
                                    }
                                }).then((result) => {
                                    if (result.dismiss === Swal.DismissReason.timer) {
                                        editDataWorker(response.data.workerId);
                                    }
                                });
                            },
                            error: function (error) {
                                Swal.fire({
                                    icon: 'error',
                                    title: 0,
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
                title: 0,
                text: 'Не удалось найти пользователя',
            });
        }
    });
}


function editDataWorker(workerId = null) {
    $.ajax({
        url: '/HR/Worker/GetDataByWorkers?workerId=' + workerId,
        type: 'GET',
        dataType: 'json',
        success: function (response) {
                        
            let worderData = response.data;
            let statusesWorker = response.status;
            let offices = response.offices;
            let selectorStatus;
            let selectorOffices;


            for (let status of statusesWorker) {
                selectorStatus += `<option value="${status}">${status}</option>`;
            }

            for (let office of offices) {
                selectorOffices += `<option value="${office.id}">${office.name}, ${office.type}</option>`;
            }



            let formWorkerData = `
                    <form id="dataUser" enctype="multipart/form-data">
                          <div class="form-row">                           
                              <input type="text" class="form-control" id="workerId" value="${worderData.workerId}" name="WorkerId" hidden>                           
                              <input type="text" class="form-control" id="accessRights" value="${worderData.accessRights}" name="AccessRights" hidden>                           
                              <input type="text" class="form-control" id="userId" value="${worderData.userId}" name="UserId" hidden>                           
                            <div class="form-group col-md-12 mb-1">
                              <label for="status">Статус:</label>
                              <select class="form-control" id="status" name="Status" required>
                              <option selected disabled>${worderData.status !== null ? worderData.status : 'Выбрать'}</option>
                                ${selectorStatus}
                              </select>
                            </div>
                          </div>
                          <div class="form-row">
                            <div class="form-group col-md-12 mb-1">
                              <label for="officeName">Место работы:</label>
                              <select class="form-control" id="officeName" name="OfficeId" required>
                                <option selected disabled>${worderData.officeName !== null ? worderData.officeName : 'Выбрать'}</option>
                                ${selectorOffices}
                              </select>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                              <label for="post">Должность:</label>
                              <input type="text" class="form-control" id="post" value="${worderData.post !== null ? worderData.post : ''}" name="Post" required>
                            </div>
                          </div>
                          <div class="form-row">
                            <div class="form-group col-md-12 mb-1">
                              <label for="admissionOrder">Номер приказа о приеме:</label>
                              <input type="number" class="form-control" id="admissionOrder" value="${worderData.admissionOrder !== 0 ? worderData.admissionOrder : ''}" name="AdmissionOrder" required>
                            </div>
                            <div class="form-group col-md-12 mb-1">
                              <label for="orderDismissal">Номер приказа об увольнении:</label>
                              <input type="number" class="form-control" id="orderDismissal" value="${worderData.orderDismissal !== 0 ? worderData.orderDismissal : '0'}" name="OrderDismissal" required>
                            </div>
                          </div>
                        </form>
                        `;

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
                            url: '/HR/Worker/UpsertWorkerData?dataWorker=' + jsonDataUser,
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

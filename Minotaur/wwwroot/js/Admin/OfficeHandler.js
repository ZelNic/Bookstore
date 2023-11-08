
let typeOffice;
let statusOffice;
let tableOffice;

$(document).ready(function () {
    getTableOffice();
});

function getTableOffice() {
    tableOffice = $('#tableOffice').DataTable({
        "ajax": { url: '/Admin/Office/GetDataOffice' },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'type', "width": "10%" },
            { data: 'status', "width": "10%" },
            {
                data: function (row) {
                    return row.city + ' ' + row.street + ' ' + row.buildingNumber;
                },
                "width": "15%"
            },
            { data: 'workingHours', "width": "10%" },
            { data: 'supervisorId', "width": "10%" },
            { data: 'workload', "width": "10%" },
            { data: 'notes', "width": "10%" },
            {
                data: 'id',
                render: function (data, type, row) {
                    return '<button class="btn btn-warning">Редактировать</button>';
                },
                "width": "15%"
            }
        ]
    });
}

function updateDataTable() {
    getTableOffice();
    tableOffice.ajax.url('/Admin/Office/GetDataOffice').load();
}

function getDataForFormNewOffice() {
    tableOffice = document.getElementById("tableOffice");
    $.ajax({
        url: '/Admin/Office/GetDataForFormNewOffice',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            typeOffice = response.officeTypes;
            statusOffice = response.officeStatus;
            addParameterForForm();
        },
    });
}
function addParameterForForm() {
    let formNewOffice = document.getElementById("formNewOffice")
    let selectTypeOffice = "";
    let stOffice = "";

    for (let type of typeOffice) {

        if (type.includes('_')) {
            type = type.replace('_', ' ');
        }

        selectTypeOffice += `<option value="${type}">${type}</option>`
    }

    for (let status of statusOffice) {

        if (status.includes('_')) {
            status = status.replace('_', ' ');
        }

        stOffice += `<option value="${status}">${status}</option>`
    }

    formNewOffice.innerHTML = `
                <form>
                <div class="mb-1 col">
                    <label for="name" class="form-label">Название</label>
                    <input type="text" class="form-control" id="name" name="name" required>
                </div>
                <div class="mb-1 col">
                    <label for="name" class="form-label">Тип</label>
                    <select class="form-control" id="type" name="type" required>
                         <option selected disabled>Выбрать</option>
                        ${selectTypeOffice}
                    </select>
                </div>
                <div class="mb-1">
                    <label for="status" class="form-label">Статус</label>
                    <select class="form-control" id="status" name="status" required>
                         <option selected disabled>Выбрать</option>
                        ${stOffice}
                    </select>                    
                </div>
                <div class="mb-1">
                    <label for="city" class="form-label">Город</label>
                    <input type="text" class="form-control" id="city" name="city" required>
                </div>
                <div class="mb-1">
                    <label for="street" class="form-label">Улица</label>
                    <input type="text" class="form-control" id="street" name="street" required>
                </div>
                <div class="mb-1">
                    <label for="buildingNumber" class="form-label">Номер здания</label>
                    <input type="text" class="form-control" id="buildingNumber" name="buildingNumber" required>
                </div>
                <div class="mb-1">
                    <label for="workingHours" class="form-label">Режим работы</label>
                    <input type="text" class="form-control" id="workingHours" name="workingHours" required>
                </div>
                <div class="mb-1">
                    <label for="supervisorId" class="form-label">ID Ответственного</label>
                    <input type="text" class="form-control" id="supervisorId" name="supervisorId" required>
                </div>                
                <div class="mb-1">
                    <label for="notes" class="form-label">Заметки</label>
                    <textarea class="form-control" id="notes" name="notes"></textarea>
                </div>
                <button type="submit" onclick="sendDataNewOffice()" class="btn btn-primary">Добавить</button>
                </form>
        `;
}
function activeFormNewOffice(isActive) {
    let formNewOffice = document.getElementById("formNewOffice");
    let btnActive = document.getElementById("btnActive");
    let btnClose = document.getElementById("btnClose");
    getDataForFormNewOffice();

    if (isActive == true) {
        formNewOffice.removeAttribute("hidden");
        btnActive.setAttribute("hidden", true);
        btnClose.removeAttribute("hidden");
    }
    else {
        formNewOffice.setAttribute("hidden", true);
        btnActive.removeAttribute("hidden");
        btnClose.setAttribute("hidden", true);
    }
}

function sendDataNewOffice() {
    // Получение значений полей формы
    let name = document.getElementById('name').value;
    let type = document.getElementById('type').value;
    let status = document.getElementById('status').value;
    let city = document.getElementById('city').value;
    let street = document.getElementById('street').value;
    let buildingNumber = document.getElementById('buildingNumber').value;
    let workingHours = document.getElementById('workingHours').value;
    let supervisorId = document.getElementById('supervisorId').value;
    let notes = document.getElementById('notes').value;

    let data = {
        Name: name,
        Type: type,
        Status: status,
        City: city,
        Street: street,
        BuildingNumber: buildingNumber,
        WorkingHours: workingHours,
        SupervisorId: supervisorId,
        Notes: notes
    };

    for (let prop in data) {
        if (data[prop] == '' && prop !== "Notes") {
            return;
        }
    }


    let jsonDataNewOffice = JSON.stringify(data);

    $.ajax({
        url: '/Admin/Office/AddOffice?dataOffice=' + jsonDataNewOffice,
        type: 'POST',

        success: function (response) {
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Новый офис добавлен в базу',
                showConfirmButton: false,
                timer: 1500
            })
        },
        error: function (error) {
            Swal.fire({
                position: 'top-end',
                icon: 'error',
                title: 'Ошибка',
                showConfirmButton: false,
                timer: 1500
            })
        }
    });
    updateDataTable();
}
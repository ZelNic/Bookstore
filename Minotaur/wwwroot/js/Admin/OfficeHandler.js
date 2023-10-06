

var typeOffice;
var statusOffice;
var tableOffice;

$(document).ready(function () {
    getTableOffice();
});



function getTableOffice() {
    $.ajax({
        url: '/Admin/Office/GetDataOffice',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            dataForOffice = response.offices;
            typeOffice = response.officeTypes;
            statusOffice = response.officeStatus;
            addFormForAddOffice();

            console.log(dataForOffice);

            $('#tableOffice').DataTable({
                "columns": [
                    {
                        data: 'name',
                        "width": "10%"
                    },
                    {
                        data: 'type',
                        "width": "10%"
                    },
                    {
                        data: 'status',
                        "width": "10%"
                    },
                    {
                        data: function (row) {
                            return row.city + ' ' + row.street + ' ' + row.buildingNumber;
                        },
                        "width": "15%"
                    },
                    {
                        data: 'workingHours',
                        "width": "10%"
                    },
                    {
                        data: 'supervisorId',
                        "width": "10%"
                    },
                    {
                        data: 'workload',
                        "width": "10%"
                    },
                    {
                        data: 'notes',
                        "width": "10%"
                    },
                    {
                        data: 'id',
                        render: function (data, type, row) {
                            return '<a href="/Customer/Home/Details?productId=' + data + '">' + data + '</a>';
                        },
                        "width": "15%"
                    }
                ]
            });
        }
    });
}



function addFormForAddOffice() {
    var formNewOffice = document.getElementById("formNewOffice")
    var selectTypeOffice = "";
    var stOffice = "";


    for (var type of typeOffice) {

        if (type.includes('_')) {
            type = type.replace('_', ' ');
        }

        selectTypeOffice += `<option value="${type}">${type}</option>`
    }

    for (var status of statusOffice) {

        if (status.includes('_')) {
            status = status.replace('_', ' ');
        }

        stOffice += `<option value="${status}">${status}</option>`
    }

    formNewOffice.innerHTML = `
                <div class="mb-1 col">
                    <label for="name" class="form-label">Название</label>
                    <input type="text" class="form-control" id="name" name="name">
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
                    <input type="text" class="form-control" id="workingHours" name="workingHours">
                </div>
                <div class="mb-1">
                    <label for="supervisorId" class="form-label">ID Ответственного</label>
                    <input type="text" class="form-control" id="supervisorId" name="supervisorId">
                </div>                
                <div class="mb-1">
                    <label for="notes" class="form-label">Заметки</label>
                    <textarea class="form-control" id="notes" name="notes"></textarea>
                </div>
                <button type="submit" onclick="sendDataNewOffice()" class="btn btn-primary">Добавить</button>
        `;
}

function activeFormNewOffice(isActive) {
    var formNewOffice = document.getElementById("formNewOffice");
    var btnActive = document.getElementById("btnActive");
    var btnClose = document.getElementById("btnClose");

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
    var name = document.getElementById('name').value;
    var type = document.getElementById('type').value;
    var status = document.getElementById('status').value;
    var city = document.getElementById('city').value;
    var street = document.getElementById('street').value;
    var buildingNumber = document.getElementById('buildingNumber').value;
    var workingHours = document.getElementById('workingHours').value;
    var supervisorId = document.getElementById('supervisorId').value;
    var notes = document.getElementById('notes').value;

    var data = {
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

    var jsonDataNewOffice = JSON.stringify(data);

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
    })

}
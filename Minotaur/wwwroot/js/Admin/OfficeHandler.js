
$(document).ready(function () {

});


function getTableOffice() {
    var tableOffice = document.getElementById("tableOffice");
}

function activeFormNewOffice(isActive) {
    var dataNewOffice = document.getElementById("dataNewOffice");
    var btnActive = document.getElementById("btnActive");
    var btnClose = document.getElementById("btnClose");

    if (isActive == true) {
        dataNewOffice.removeAttribute("hidden");
        btnActive.setAttribute("hidden", true);
        btnClose.removeAttribute("hidden");
    }
    else {
        dataNewOffice.setAttribute("hidden", true);
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
    var workload = document.getElementById('workload').value;
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
        Workload: workload,
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
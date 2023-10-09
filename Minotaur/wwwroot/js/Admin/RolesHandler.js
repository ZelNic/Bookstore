
var dataWorker;
$(document).ready(function () {
    getDataUserRoles();
});

function getDataUserRoles() {
    dataWorker = $('#tableWorker').DataTable({
        "ajax": { url: '/Admin/Roles/GetDataUserRoles' },
        "columns": [
            { data: 'userId', "width": "15%" },
            { data: 'status', "width": "5%" },
            { data: 'lfs', "width": "15%" },
            { data: 'post', "width": "10%" },
            { data: 'officeName', "width": "15%" },
            { data: 'email', "width": "10%" },
            {
                data: 'userId',
                render: function (data, type, row) {
                    var userId = row.userId;
                    var roleArray;
                    if (row.accessRights.includes('|')) {
                        var roleArray = row.accessRights.split("|");
                    } else {
                        roleArray = [row.accessRights];
                    }
                    return `<div>
                                <input type="checkbox" class="role-checkbox" value="Admin" ${roleArray.includes("Admin") ? 'checked' : ''} userId="${userId}" /> Админ<br>
                                <input type="checkbox" class="role-checkbox" value="OrderPicker" ${roleArray.includes("OrderPicker") ? 'checked' : ''} userId="${userId}"/> Сборщик заказов<br>
                                <input type="checkbox" class="role-checkbox" value="WorkerOrderPickupPoint" ${roleArray.includes("WorkerOrderPickupPoint") ? 'checked' : ''} userId="${userId}"/> Сотрудник пункта выдачи<br>
                                <input type="checkbox" class="role-checkbox" value="Customer" ${roleArray.includes("Customer") ? 'checked' : ''} userId="${userId}"/> Пользователь<br>
                                <input type="checkbox" class="role-checkbox" value="HR" ${roleArray.includes("HR") ? 'checked' : ''} userId="${userId}"/> HR<br>
                                <input type="checkbox" class="role-checkbox" value="Stockkeeper" ${roleArray.includes("Stockkeeper") ? 'checked' : ''} userId="${userId}"/> Кладовщик<br>                        
                            </div>`;
                },
                "width": "30%"
            }
        ]
    });

    $(document).on('change', '.role-checkbox', function () {
        var userId = $(this).attr('userId');
        var role = $(this).attr('value');

        var isChecked = $(this).is(':checked');

        var urlMethod;
        var messageResponse;

        if (isChecked == true) {
            urlMethod = `/Admin/Roles/SetRoleWorker?userId= + ${userId} + &role= + ${role}`
            messageResponse = "Роль успешно назначена"
        } else {
            urlMethod = `/Admin/Roles/RemoveRoleWorker?userId= + ${userId} + &role= + ${role}`
            messageResponse = "Роль успешно снята"
        }

        $.ajax({
            url: urlMethod,
            type: 'POST',
            data: {
                userId: userId,
                role: role,
            },
            success: function (response) {
                Swal.fire(messageResponse);
            },
            error: function (error) {
                Swal.fire("Ошибка");
            }
        });
    });
}




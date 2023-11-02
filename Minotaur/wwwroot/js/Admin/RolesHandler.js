
let dataWorker;
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
                    let userId = row.userId;
                    let roleArray;

                    if (row.accessRights == null) {
                        roleArray = [row.accessRights];
                    }
                    else if (row.accessRights.includes('|')) {
                        roleArray = row.accessRights.split("|");
                    } else {
                        roleArray = [row.accessRights];
                    }

                    return `<div>
                                <input type="checkbox" class="role-checkbox" value="Admin" ${roleArray.includes("Admin") ? 'checked' : ''} userId="${userId}" /> Администратор сайта<br>
                                <input type="checkbox" class="role-checkbox" value="OrderPicker" ${roleArray.includes("OrderPicker") ? 'checked' : ''} userId="${userId}"/> Сборщик заказов<br>
                                <input type="checkbox" class="role-checkbox" value="WorkerOrderPickupPoint" ${roleArray.includes("WorkerOrderPickupPoint") ? 'checked' : ''} userId="${userId}"/> Сотрудник пункта выдачи<br>
                                <input type="checkbox" class="role-checkbox" value="Customer" ${roleArray.includes("Customer") ? 'checked' : ''} userId="${userId}"/> Пользователь<br>
                                <input type="checkbox" class="role-checkbox" value="HR" ${roleArray.includes("HR") ? 'checked' : ''} userId="${userId}"/> HR<br>
                                <input type="checkbox" class="role-checkbox" value="Stockkeeper" ${roleArray.includes("Stockkeeper") ? 'checked' : ''} userId="${userId}"/> Кладовщик<br>   
                                <input type="checkbox" class="role-checkbox" value="Operator" ${roleArray.includes("Operator") ? 'checked' : ''} userId="${userId}"/> Оператор<br>  
                            </div>`;
                },
                "width": "30%"
            }
        ]
    });

    $(document).on('change', '.role-checkbox', function () {
        let userId = $(this).attr('userId');
        let role = $(this).attr('value');
        let isChecked = $(this).is(':checked');

        let urlMethod;
        let messageResponse;

        if (isChecked == true) {
            urlMethod = `/Admin/Roles/SetRoleWorker?userId= + ${userId} + &role= + ${role}`
            messageResponse = "Роль успешно назначена"
        } else {
            urlMethod = `/Admin/Roles/RemoveRoleWorker?userId= + ${userId} + &role= + ${role}`
            messageResponse = "Роль успешно снята"
        }

        //var result = await ConfirmActionAsync(userId);
        //if (result == true) {

        //}
        //else {
        //    Swal.fire("Неверный пароль");
        //}

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



// IDEA: сделать проверку на админа
async function сonfirmActionAsync(userId) {
    const { value: password } = await Swal.fire({
        title: 'Введите пароль',
        input: 'password',
        inputLabel: 'Password',
        inputPlaceholder: '',
        inputAttributes: {
            maxlength: 50,
            autocapitalize: 'off',
            autocorrect: 'off'
        }
    })

    if (password) {
        const operation = "Изменение роли";
        $.ajax({
            url: '/Admin/AuthenticationAdmin/ConfirmAction?password=' + password + "&operation=" + operation + "&userId" + userId,
            type: 'GET',
            success: function (response) {
                Swal.fire("Успешно");
            },
            error: function (error) {
                Swal.fire("Доступ отказан");
            }
        });
    }
}

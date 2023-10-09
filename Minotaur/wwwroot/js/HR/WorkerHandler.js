
var dataWorker;
$(document).ready(function () {
    getDataByWorker();
});

function getDataByWorker() {
    dataWorker = $('#tableWorker').DataTable({
        "ajax": { url: '/HR/Worker/GetDataByWorker' },
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
                    return `<button class="btn btn-warning">Редактировать</button>`;
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




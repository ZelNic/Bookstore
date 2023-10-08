
var dataWorker;
$(document).ready(function () {
    getDataUserRoles();
});

function getDataUserRoles() {
    dataWorker = $('#tableWorker').DataTable({
        "ajax": { url: '/HR/Worker/GetDataByWorker' },
        "columns": [
            { data: 'workerId', "width": "10%" },
            { data: 'userId', "width": "10%" },
            { data: 'status', "width": "10%" },            
            { data: 'officeId', "width": "10%" },
            { data: 'role', "width": "10%" },
            { data: 'admissionOrder', "width": "10%" },
            { data: 'orderDismissal', "width": "10%" },
            {
                data: 'id',
                render: function (data, type, row) {
                    return '<button class="btn btn-warning">edit</button>';
                },
                "width": "15%"
            }
        ]
    });
}


var dataWorker;
$(document).ready(function () {
    getDataUserRoles();
});

function getDataUserRoles() {
    dataWorker = $('#tableWorker').DataTable({
        "ajax": { url: '/Admin/Roles/GetDataUserRoles' },
        "columns": [
            { data: 'workerId', "width": "10%" },
            { data: 'userId', "width": "10%" },
            {
                data: 'lfs', "width": "15%"
            },
            { data: 'status', "width": "10%" },
            {
                data: 'officeId',
                render: function (data, type, row) {
                    var office = offices.find(function (o) {
                        return o.id === data;
                    });
                    return office ? office.name : '';
                },
            },
            { data: 'role', "width": "15%" },
            {
                data: 'userId',
                render: function (data, type, row) {
                    return 
                    `<button class="btn btn-warning">edit</button>
                     <button class="btn btn-warning" > edit</button >
                     <button class="btn btn-warning">edit</button>
                    `;
                },
                "width": "30%"
            }
        ]
    });
}

function getDataOffice() {
    
}

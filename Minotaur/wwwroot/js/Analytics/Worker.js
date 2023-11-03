$(document).ready(function () {
    get();
});

function get() {
    dataTable = $('#dataWorker').DataTable({
        "ajax": { url: '/Analytics/WorkerAnalysis/Get' },
        "columns": [
            { data: 'workerId', width: "20%" },
            { data: 'officeName', "width": "20%" },
            { data: 'post', "width": "5%" },
            { data: 'rating', "width": "5%" },
            { data: 'workerReviewText', "width": "50%" },
        ]
    });
}
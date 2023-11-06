$(document).ready(function () {
    get();
});

function get() {
    dataTable = $('#pickUpDataTable').DataTable({
        "ajax": { url: '/Analytics/PickUpAnalysis/Get' },
        "columns": [
            { data: 'orderId', width: "20%" },
            { data: 'pickUpRating', "width": "20%" },
            { data: 'pickUpReviewText', "width": "25%" },
        ]
    });
}
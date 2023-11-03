$(document).ready(function () {
    get();
});

function get() {
    dataTable = $('#deliveryWorker').DataTable({
        "ajax": { url: '/Analytics/DeliveryAnalysis/Get' },
        "columns": [
            { data: 'orderId', width: "20%" },
            { data: 'rating', "width": "20%" },
            { data: 'dispatchTime', "width": "5%" },
            { data: 'timeOfReceiving', "width": "5%" },
            { data: 'historyMovement', "width": "25%" },       
            { data: 'deliveryReviewText', "width": "25%" },     
            
        ]
    });
}
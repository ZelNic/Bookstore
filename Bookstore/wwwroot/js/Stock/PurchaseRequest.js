var dataTablePurchaseRequest;
$(document).ready(function () {
    loadDataTablePurchaseRequest();
});
function refreshDataTable() {
    dataTablePurchaseRequest.ajax.url('/Stockkeeper/Stock/GetTablePurchaseRequest').load();
}
function loadDataTablePurchaseRequest() {
    dataTablePurchaseRequest = $('#purchaseRequest').DataTable({
        "ajax": { url: '/Stockkeeper/Stock/GetTablePurchaseRequest' },
        "columns": [
            { data: 'productId', "width": "10%" },
            {
                data: 'titleProduct',
                render: function (data, type, row) {
                    return '<a href="/Customer/Home/Details?productId=' + row.productId + '">' + data + '</a>';
                },
                "width": "40%"
            },
            { data: 'totalProduct', "width": "25%" },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                                <input type="number" name="Count" value="${parseInt(data.totalProduct)}" required/>
                                <input type="number" name="ProductId" value="${data.productId}" hidden/> 
                                <input name="ProductName" value="${data.titleProduct}" hidden/> 
                            </div>`;
                }, "width": "15%"
            },
            {
                data: null,
                render: function (data) {
                    return `<div class="w-100 btn-group" role="group">
                    <a onClick="deleteFromPurchaseRequest('${data.productId}')" class="btn btn-warning"><i class="bi bi-check2-square"></i></a>
                    </div>`;
                }, "width": "10%"
            }
        ]
    });
}

function sendTable() {

    var models = [];

    // Получить все элементы ввода моделей
    var countInputs = document.querySelectorAll('input[name="Count"]');
    var productIdInputs = document.querySelectorAll('input[name="ProductId"]');
    var productNameInputs = document.querySelectorAll('input[name="ProductName"]');

    // Пройти по каждому элементу ввода и создать модель
    for (var i = 0; i < countInputs.length; i++) {
        var count = parseInt(countInputs[i].value);
        var productId = productIdInputs[i].value;
        var productName = productNameInputs[i].value;

        var model = {
            Count: count,
            ProductId: productId,
            productName: productName
        };

        models.push(model);
        console.log(model)
    }


    var jsonData = JSON.stringify(models)
    
    var url = "/Stockkeeper/Stock/OrderProducts";

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: jsonData
    })
        .then(function (response) {
            if (response.ok) {
                return response.text();
            } else {
                throw new Error("Ошибка при отправке данных на сервер");
            }
        })
        .then(function (data) {
            // Обработка успешного ответа от сервера
            console.log(data);
        })
        .catch(function (error) {
            // Обработка ошибки
            console.log(error);
        });

}



function deleteFromPurchaseRequest(productId) {
    $.ajax({
        url: '/Stockkeeper/Stock/SelectProductToPurchase' + "?productId=" + productId,
        type: 'POST',
        data: productId,
        success: function (response) {
            refreshDataTable();
            resolve(response);
        },
        error: function (error) {
            reject(error);
        }
    });
}             
let dataTablePurchaseRequest;
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

document.getElementById('purchaseRequestSend').addEventListener('submit', function (event) {
    event.preventDefault();
    sendTable();
});

function sendTable() {
    let models = [];

    let countInputs = document.querySelectorAll('input[name="Count"]');
    let productIdInputs = document.querySelectorAll('input[name="ProductId"]');
    let productNameInputs = document.querySelectorAll('input[name="ProductName"]');

    for (var i = 0; i < countInputs.length; i++) {
        let count = parseInt(countInputs[i].value);
        let productId = productIdInputs[i].value;
        let productName = productNameInputs[i].value;

        var model = {
            Count: count,
            ProductId: productId,
            ProductName: productName
        };

        models.push(model);
        console.log(model);
    }

    let jsonData = JSON.stringify(models);

    const URL = "/Stockkeeper/Stock/OrderProducts";

    fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: jsonData
    }).then(function (response) {
        if (response.ok) {
            return response.blob();
        } else {
            throw new Error("Ошибка при отправке данных на сервер");
        }
    })
        .then(function (blob) {
            let fileUrl = URL.createObjectURL(blob);

            let downloadLink = document.createElement("a");
            downloadLink.href = fileUrl;
            downloadLink.download = "Заявление.docx";

            downloadLink.click();
        })
        .catch(function (error) {
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




function findOrder() {

    let idOrder = document.getElementById("orderSearchString").value;
    $.ajax({
        url: `PickUp/Order/GetDataByOrder?id=${idOrder}`,
        method: 'GET',
        dataType: 'json',
        success: function (response) {
            let divOrderData = document.getElementById("orderData");
            let data = response.data;

            let countProduct = 0;
            let sumPrice = 0;

            divOrderData.innerHTML = `     
                                <div class="container m-4">
                                    <div class="form-group border border-1 rounded rounded-1 p-4">
                                        <div class="d-inline-flex align-items-center">
                                            <span class="mr-3">Номер заказа: </span>
                                            <div class="rouden rounded-1 text-bg-primary m-1">
                                                ${data.orderId}
                                            </div>
                                        </div>
                                        <div class="d-inline-flex align-items-center">
                                            <span class="mr-3">Статус заказа: </span>
                                            <div class="rouden rounded-1 bg-@colorStatus m-1">${data.orderStatus}</div>
                                        </div>


                                        <div>
                                            <label>
                                                Дата заказа:
                                                ${data.purchaseDate}
                                                по МСК
                                            </label>
                                        </div>       

                                        <table class="table">
                                            <thead>
                                                <tr>
                                                    <th scope="col">Id</th>
                                                    <th scope="col">Название</th>
                                                    <th scope="col">Цена</th>
                                                    <th scope="col">Количество</th>
                                                </tr>
                                            </thead>
                                            <tbody class="table-group-divider">
                                                <tr>
                                                    <td>@product.Id</td>
                                                    <td>
                                                        <a asp-area="Customer" asp-controller="Home" asp-action="Details" asp-route-productId="${product.Id">
                                                            @db.Products.Find(product.Id).Name
                                                        </a>
                                                    </td>
                                                    <td>@product.Price</td>
                                                    <td>@product.Count</td>
                                                    @{
                                                        countProduct += product.Count;
                                                    sumPrice += product.Count * product.Price;

                                                </tr>
                                            </tbody>
                                        </table>
                                        <div>
                                            <label>Количество товаров: @countProduct</label>
                                            <label class="m-3">Стоимость: @sumPrice</label>
                                        </div>
                                        <div class="form-group">
                                            <label>Оплачено:</label>
                                            <label>@Model.Order.PurchaseAmount</label>
                                        </div>
                                        <div>
                                         data by delivery
                                        </div>
                                        <div>
                                        data by recpemt
                                        </div>
                                    </div>
                                    <hr class="mt-5 mb-5" />
                                </div>`;

        },
        error: function (xhr, status, error) {
            // Обработка ошибки
        }
    });

}




namespace Bookstore.Models.SD
{
    public static class SD
    {
        public const string RoleAdmin = "Admin";
        public const string RoleWorkerOrderPickupPoint = "WorkerOrderPickupPoint";
        public const string RoleOrderPicker = "OrderPicker";
        public const string RoleStorekeeper = "Storekeeper";

        public const string StatusPending_0 = "В ожидании";
        public const string StatusApproved_1 = "Одобренный";
        public const string StatusInProcess_2 = "Обработка";
        public const string StatusShipped_3 = "Отправленный";
        public const string StatusDelivered_4 = "Доставлен";
        public const string StatusCancelled_5 = "Отменено";
        public const string StatusRefunded_6 = "Возмещено";

        public const string PaymentStatusPending = "В ожидании";
        public const string PaymentStatusApproved = "Одобренный";
        public const string PaymentStatusDelayedPayment = "Утверждено для задержанного платежа";
        public const string PaymentStatusRejected = "Отклоненный";

        public const string ViewOrderDetails = "Проверить состояние";
        public const string AcceptOrder = "Принять заказ";
        public const string IssueOrder = "Выдать заказ";


        public const string NotFoundUser = "Пользователь не найден.";
        public const string NotFoundOrder = "Заказ не найден.";
    }
}

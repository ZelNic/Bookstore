namespace Minotaur.Models.SD
{
    public static class StatusByOrder
    {
        public const string Pending = "В ожидании";
        public const string Approved = "Одобренный";
        public const string InProcess = "Сборка заказа";
        public const string Shipped = "Отправленный";
        public const string Delivered = "Доставлен";
        public const string Cancelled = "Отменено";
        public const string Refunded = "Возмещено";
        public const string DeliveredToPickUp = "Доставлен в пункт выдачи";
        public const string Сompleted = "Завершен";

        public const string AwaitingConfirmationForIncompleteOrder = "Ожидает согласия от покупателя на отправку неполного заказа";
        public const string BuyerAgreesNeedSend = "Покупатель согласен на получение неполного заказа. Ожидается передача в отдел отправки";
        public const string BuyerDontAgreesNeedRefunded = "Покупатель не согласен на получение неполного заказа. Ожидается возврат средств";
        public const string StatusCancelled = "Заказ отменен, ожидается возврат средств";


        public const string PaymentStatusPending = "В ожидании";
        public const string PaymentStatusApproved = "Одобренный";
        public const string PaymentStatusDelayedPayment = "Утверждено для задержанного платежа";
        public const string PaymentStatusRejected = "Отклоненный";
    }
}

using Humanizer;

namespace Minotaur.Models.SD
{
    public static class StatusByOrder
    {
        public const string StatusPending_0 = "В ожидании";
        public const string StatusApproved_1 = "Одобренный";
        public const string StatusInProcess_2 = "Сборка заказа";
        public const string StatusShipped_3 = "Отправленный";
        public const string StatusDelivered_4 = "Доставлен";
        public const string StatusCancelled_5 = "Отменено";
        public const string StatusRefunded_6 = "Возмещено";

        public const string AwaitingConfirmationForIncompleteOrder_7 = "Ожидает согласия от покупателя на отправку неполного заказа";
        public const string BuyerAgreesNeedSend_8 = "Покупатель согласен на получение неполного заказа. Ожидается передача в отдел отправки";
        public const string BuyerDontAgreesNeedRefunded_9 = "Покупатель не согласен на получение неполного заказа. Ожидается возврат средств";
        public const string StatusCancelled_10 = "Заказ отменен, ожидается возврат средств";


        public const string PaymentStatusPending = "В ожидании";
        public const string PaymentStatusApproved = "Одобренный";
        public const string PaymentStatusDelayedPayment = "Утверждено для задержанного платежа";
        public const string PaymentStatusRejected = "Отклоненный";
    }
}

namespace Bookstore.Models.SD
{
    public static class SD
    {        
        public const string Role_Admin = "Admin";

        public const string StatusPending = "В ожидании";
        public const string StatusApproved = "Одобренный";
        public const string StatusInProcess = "Обработка";
        public const string StatusShipped = "Отправленный";
        public const string StatusCancelled = "Отменено";
        public const string StatusRefunded = "Возмещено";

        public const string PaymentStatusPending = "В ожидании";
        public const string PaymentStatusApproved = "Одобренный";
        public const string PaymentStatusDelayedPayment = "Утверждено для задержанного платежа";
        public const string PaymentStatusRejected = "Отклоненный";
    }
}

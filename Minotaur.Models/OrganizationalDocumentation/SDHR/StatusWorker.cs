namespace Minotaur.Models.OrganizationalDocumentation.SDHR
{
    public class StatusWorker
    {
        public const string Confirmation_Required = "Необходимо подтверждение";
        public const string Works = "Работает";
        public const string SickLeave = "На больничном";
        public const string Vacation = "Отпуск";
        public const string Decree = "Отпуск по беременности и родам";
        public const string Pension = "На пенсии";
        public const string Fired = "Уволен";


        public static string[] GetStatus()
        {
            string[] status = new string[] { Confirmation_Required, Works, SickLeave, Vacation, Decree, Pension, Fired };

            return status;
        }
    }
}

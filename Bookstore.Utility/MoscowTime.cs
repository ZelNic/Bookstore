namespace Minotaur.Utility
{
    public static class MoscowTime
    {
        public static DateTime GetTime()
        {
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            DateTime currentTimeInMoscow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowTimeZone);
            return currentTimeInMoscow;
        }
    }
}
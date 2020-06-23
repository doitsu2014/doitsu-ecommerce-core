namespace System
{
    public static class TimeUtils
    {

        public static DateTime ToVietnamDateTime(this DateTime time)
        {
            var now = DateTime.UtcNow;
            var vietnam = now.AddHours(7);
            return vietnam;
        }

        /// <summary>
        /// Pass the gmt time and get the time code
        /// </summary>
        /// <param name="gmt"></param>
        /// <returns></returns>
        public static string GetTimeCode(int gmt)
        {
            var now = DateTime.UtcNow;
            var trueGmt = now.AddHours(gmt);
            string timeCode = trueGmt.ToString("ddMMyyyyHHmmssfff");
            return timeCode;
        }

        public static DateTime StartOfDay(this DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(this DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }
    }
}

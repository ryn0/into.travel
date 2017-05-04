using System;

namespace IntoTravel.Web.Helpers
{
    public class UrlBuilder
    {
        public static string BlogUrlPath(string key, DateTime blogPublishDateTimeUtc)
        {
            return string.Format("/blog/{0}/{1}/{2}/{3}",
                blogPublishDateTimeUtc.Year.ToString("0000"),
                blogPublishDateTimeUtc.Month.ToString("00"),
                blogPublishDateTimeUtc.Day.ToString("00"),
                key);
        }

        public static string BlogPreviewUrlPath(string key)
        {
            return string.Format("/blogmanagement/preview/{0}",
                key);
        }
    }
}

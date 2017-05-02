using IntoTravel.Data.Models;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Helpers
{
    public class ModelConverter
    {
        public static BlogEntrytDisplay Convert(BlogEntry blogEntry)
        {
            return new BlogEntrytDisplay()
            {
                BlogPublishDateTimeUtc = blogEntry.BlogPublishDateTimeUtc,
                Content = blogEntry.Content,
                Key = blogEntry.Key,
                Title = blogEntry.Title,
                UrlPath = string.Format("/{0}/{1}/{2}/{3}", 
                            blogEntry.BlogPublishDateTimeUtc.Year, 
                            blogEntry.BlogPublishDateTimeUtc.Month, 
                            blogEntry.BlogPublishDateTimeUtc.Day,  
                            blogEntry.Key),

                // todo: other props
            };
        }
    }
}

using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System.Linq;

namespace IntoTravel.Web.Helpers
{
    public class ModelConverter
    {
        public static BlogEntryDisplayModel Convert(BlogEntry blogEntry)
        {
      
          
            var model =  new BlogEntryDisplayModel()
            {
                BlogPublishDateTimeUtc = blogEntry.BlogPublishDateTimeUtc,
                Content = blogEntry.Content,
                Key = blogEntry.Key,
                Title = blogEntry.Title,
                UrlPath = string.Format("/{0}/{1}/{2}/{3}", 
                            blogEntry.BlogPublishDateTimeUtc.Year.ToString("0000"), 
                            blogEntry.BlogPublishDateTimeUtc.Month.ToString("00"), 
                            blogEntry.BlogPublishDateTimeUtc.Day.ToString("00"),  
                            blogEntry.Key),
                DefaulPhotoUrl = blogEntry.Photos.FirstOrDefault(x => x.IsDefault == true).PhotoUrl
                // todo: other props
            };

            return model;
            

        }
    }
}

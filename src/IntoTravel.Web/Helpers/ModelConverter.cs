using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                // todo: other props
            };
        }
    }
}

using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace IntoTravel.Web.Helpers
{
    public class ModelConverter
    {
        const string BlobPreix = "https://intotravel.blob.core.windows.net";
        const string CdnPrefix = "http://cdn.into.travel";

        public static BlogEntryDisplayModel Convert(BlogEntry blogEntry)
        {
            var defaultPhotoUrl = blogEntry.Photos.FirstOrDefault(x => x.IsDefault == true);

            var model = new BlogEntryDisplayModel()
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
                Photos = AddBlogPhotos(blogEntry.Photos),
                DefaultPhotoUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoUrl : null
            };

            return model;
        }

        private static List<BlogPhotoModel> AddBlogPhotos(List<BlogEntryPhoto> photos)
        {
            photos = photos.OrderBy(x => x.Rank).ToList();

            var photoList = new List<BlogPhotoModel>();

            foreach(var photo in photos)
            {
                photoList.Add(new BlogPhotoModel
                {
                    BlogEntryPhotoId = photo.BlogEntryPhotoId,
                    Description = photo.Description,
                    IsDefault = photo.IsDefault,
                    PhotoUrl = photo.PhotoUrl,
                    Title = photo.Title,
                    PhotoCdnUrl = photo.PhotoUrl.Replace(BlobPreix, CdnPrefix) // todo: replace in a better way
                });
            }

            return photoList;
        }
    }
}

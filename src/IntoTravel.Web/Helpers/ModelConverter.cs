using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System.Linq;
using System.Collections.Generic;
using System;

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
                UrlPath = UrlBuilder.BlogUrlPath(blogEntry.Key, blogEntry.BlogPublishDateTimeUtc),
                Photos = AddBlogPhotos(blogEntry.Photos),
                DefaultPhotoUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoUrl : null,
                MetaDescription = blogEntry.MetaDescription
            };

            if (blogEntry.BlogEntryTags != null)
            {
                foreach (var blogEntryTag in blogEntry.BlogEntryTags)
                {
                    model.Tags.Add(blogEntryTag.Tag.Name);
                }
            }

            return model;
        }



        private static BlogEntryDisplayListModel ConvertToListModel(List<BlogEntry> blogEntries)
        {
            var model = new BlogEntryDisplayListModel();

            foreach (var blog in blogEntries)
            {
                model.Items.Add(ModelConverter.Convert(blog));
            }

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


        public static BlogEntryDisplayListModel BlogPage(List<BlogEntry> blogEntries, int pageNumber, int amountPerPage, int total)
        {
            var model = ConvertToListModel(blogEntries);
            model.Total = total;
            model.CurrentPageNumber = pageNumber;
            model.QuantityPerPage = amountPerPage;
            var pageCount = (double)model.Total / model.QuantityPerPage;
            model.PageCount = (int)Math.Ceiling(pageCount);

            return model;
        }

    }
}

using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using IntoTravel.Core.Constants;

namespace IntoTravel.Web.Helpers
{
    public class ModelConverter
    {
        public static BlogEntryDisplayModel ConvertToBlogDisplayModel(BlogEntry blogEntry)
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
                DefaultPhotoThumbUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoThumbUrl : null,
                DefaultPhotoThumbCdnUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoThumbUrl.Replace(StringConstants.BlobPreix, StringConstants.CdnPrefix) : null,
                DefaultPhotoUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoUrl : null,
                DefaultPhotoCdnUrl = (defaultPhotoUrl != null) ? defaultPhotoUrl.PhotoUrl.Replace(StringConstants.BlobPreix, StringConstants.CdnPrefix) : null,
                MetaDescription = blogEntry.MetaDescription
            };

            if (blogEntry.BlogEntryTags != null)
            {
                foreach (var blogEntryTag in blogEntry.BlogEntryTags)
                {
                    model.Tags.Add(blogEntryTag.Tag.Name);
                }

                model.Tags = model.Tags.OrderBy(x => x).ToList();
            }

            return model;
        }


        private static BlogEntryDisplayListModel ConvertToListModel(List<BlogEntry> blogEntries)
        {
            var model = new BlogEntryDisplayListModel();

            foreach (var blog in blogEntries)
            {
                model.Items.Add(ConvertToBlogDisplayModel(blog));
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
                    Title = photo.Title,
                    PhotoUrl = photo.PhotoUrl,
                    PhotoCdnUrl = photo.PhotoUrl.Replace(StringConstants.BlobPreix, StringConstants.CdnPrefix),
                    PhotoThumbUrl = photo.PhotoThumbUrl,
                    PhotoThumbCdnUrl = photo.PhotoThumbUrl.Replace(StringConstants.BlobPreix, StringConstants.CdnPrefix)
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


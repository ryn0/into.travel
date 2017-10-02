using IntoTravel.Data.Models;
using IntoTravel.Web.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using IntoTravel.Data.Constants;

namespace IntoTravel.Web.Helpers
{
    public class ModelConverter
    {
        public static BlogEntryDisplayModel ConvertToBlogDisplayModel(BlogEntry current, BlogEntry previous, BlogEntry next)
        {
            var defaultPhotoUrl = current?.Photos.FirstOrDefault(x => x.IsDefault == true);
            var previousPhotoUrl = previous?.Photos.FirstOrDefault(x => x.IsDefault == true);
            var nextPhotoUrl = next?.Photos.FirstOrDefault(x => x.IsDefault == true);

            var model = new BlogEntryDisplayModel()
            {
                BlogPublishDateTimeUtc = current.BlogPublishDateTimeUtc,
                Content = current.Content,
                Key = current.Key,
                Title = current.Title,
                UrlPath = UrlBuilder.BlogUrlPath(current.Key, current.BlogPublishDateTimeUtc),

                PreviousName = previous?.Title,
                PreviousUrlPath = (previous != null) ? UrlBuilder.BlogUrlPath(previous.Key, previous.BlogPublishDateTimeUtc) : null,
                DefaultPreviousPhotoThumbCdnUrl = ConvertBlobToCdnUrl(previousPhotoUrl?.PhotoThumbUrl),

                NextName = next?.Title,
                NextUrlPath = (next != null) ? UrlBuilder.BlogUrlPath(next.Key, next.BlogPublishDateTimeUtc) : null,
                DefaultNextPhotoThumbCdnUrl = ConvertBlobToCdnUrl(nextPhotoUrl?.PhotoThumbUrl),

                Photos = AddBlogPhotos(current.Photos),

                DefaultPhotoThumbUrl = defaultPhotoUrl?.PhotoThumbUrl,
                DefaultPhotoThumbCdnUrl = ConvertBlobToCdnUrl(defaultPhotoUrl?.PhotoThumbUrl),

                DefaultPhotoUrl = defaultPhotoUrl?.PhotoPreviewUrl,
                DefaultPhotoCdnUrl = ConvertBlobToCdnUrl(defaultPhotoUrl?.PhotoPreviewUrl),

                MetaDescription = current.MetaDescription
            };

            if (current.BlogEntryTags != null)
            {
                foreach (var blogEntryTag in current.BlogEntryTags)
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
                model.Items.Add(ConvertToBlogDisplayModel(blog, null, null));
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
                    PhotoCdnUrl = ConvertBlobToCdnUrl(photo?.PhotoFullScreenUrl),

                    PhotoThumbUrl = photo.PhotoThumbUrl,
                    PhotoThumbCdnUrl = ConvertBlobToCdnUrl(photo?.PhotoThumbUrl),

                    PhotoPreviewUrl = photo.PhotoPreviewUrl,
                    PhotoPreviewCdnUrl = ConvertBlobToCdnUrl(photo?.PhotoPreviewUrl),
                });
            }

            return photoList;
        }


        private static string ConvertBlobToCdnUrl(string blobUrl)
        {
            if (string.IsNullOrWhiteSpace(blobUrl))
                return null;

            if (BoolConstants.EnableSsl)
            {
                return blobUrl.Replace(StringConstants.BlobPrefix, StringConstants.CdnHttpsPrefix);
            }
            else
            {
                return blobUrl.Replace(StringConstants.BlobPrefix, StringConstants.CdnHttpPrefix);
            }
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


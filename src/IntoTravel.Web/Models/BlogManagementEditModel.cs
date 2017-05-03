using System;
using System.Collections.Generic;

namespace IntoTravel.Web.Models
{
    public class BlogManagementEditModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime BlogPublishDateTimeUtc { get; set; } = DateTime.UtcNow;

        public int BlogEntryId { get;  set; }

        public bool IsLive { get;   set; }

        public List<BlogPhotoModel> BlogPhotos { get; set; } = new List<BlogPhotoModel>();
    }

    public class BlogPhotoModel
    {

        public int BlogEntryPhotoId { get; set; }

        public string PhotoUrl { get; set; }

        public bool IsDefault { get; set; }
    }
}

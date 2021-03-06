﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntoTravel.Web.Models
{
    public class BlogManagementEditModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime BlogPublishDateTimeUtc { get; set; } = DateTime.UtcNow;

        public int BlogEntryId { get;  set; }

        public bool IsLive { get;   set; }

        public string LiveUrlPath { get; set; }
        public string PreviewUrlPath { get; set; }

        public List<BlogPhotoModel> BlogPhotos { get; set; } = new List<BlogPhotoModel>();

        public List<string> BlogTags { get; set; } = new List<string>();

        public string Tags { get; set; }

        [StringLength(160)]
        public string MetaDescription { get; set; }
    }
}

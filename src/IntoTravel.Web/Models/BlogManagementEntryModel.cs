using System;

namespace IntoTravel.Web.Models
{
    public class BlogManagementEntryModel
    {
        public string Title { get; set; }


        public string Content { get; set; }

        public DateTime BlogPublishDateTimeUtc { get; set; } = DateTime.UtcNow;

        public int BlogEntryId { get;  set; }

        public bool IsLive { get;   set; }
    }
}

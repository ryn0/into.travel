using System;

namespace IntoTravel.Web.Models
{
    public class BlogManagementEntryModel
    {
        public string Title { get; set; }


        public string Content { get; set; }
        public DateTime BlogPublishDateTimeUtc { get;   set; }
        public int BlogEntryId { get;  set; }
    }
}

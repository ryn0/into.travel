using System;
using System.Collections.Generic;

namespace IntoTravel.Web.Models
{
    public class BlogEntrytDisplay
    {
        public string Title { get; set; }

        public string Key { get; set; }

        public string Content { get; set; }
 
        public DateTime BlogPublishDateTimeUtc { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}

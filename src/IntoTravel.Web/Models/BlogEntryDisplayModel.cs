using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTravel.Web.Models
{
    public class BlogEntryDisplayModel
    {
        public string Title { get; set; }

        public string Key { get; set; }

        public string UrlPath { get; set; }

        public string Content { get; set; }
 
        public DateTime BlogPublishDateTimeUtc { get; set; }

        public string FriendlyDateDisplay
        {
            get
            {
                var dt = BlogPublishDateTimeUtc;

                string suffix;

                switch (dt.Day)
                {
                    case 1:
                    case 21:
                    case 31:
                        suffix = "st";
                        break;
                    case 2:
                    case 22:
                        suffix = "nd";
                        break;
                    case 3:
                    case 23:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }

                return string.Format("{0:MMMM} {1}{2}, {0:yyyy}", dt, dt.Day, suffix);
            }
        }

        public string DefaulPhotoUrl { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public List<string> PhotoUrls { get; set; } = new List<string>();
    }
}

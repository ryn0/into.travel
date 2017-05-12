namespace IntoTravel.Web.Models
{

    public class BlogPhotoModel
    {
        public int BlogEntryPhotoId { get; set; }

        public string PhotoUrl { get; set; }
 
        public string PhotoThumbUrl { get; set; }

        public string PhotoCdnUrl { get; set; }

        public string PhotoThumbCdnUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }
    }
}

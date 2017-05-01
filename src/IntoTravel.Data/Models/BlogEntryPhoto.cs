using IntoTravel.Data.DbModels.BaseDbModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class BlogEntryPhoto : StateInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogEntryPhotoId { get; set; }

        public int BlogEntryId { get; set; }

        public string PhotoUrl { get; set; }

        public virtual BlogEntry  BlogEntry  { get; set; }

    }
}

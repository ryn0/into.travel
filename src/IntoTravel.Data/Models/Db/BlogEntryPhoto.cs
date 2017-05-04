using IntoTravel.Data.DbModels.BaseDbModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class BlogEntryPhoto : StateInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogEntryPhotoId { get; set; }

        [Required]
        [StringLength(255)]
        public string PhotoUrl { get; set; }

        public bool IsDefault { get; set; }

        public int Rank { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public int BlogEntryId { get; set; }

        public virtual BlogEntry BlogEntry  { get; set; }
    }
}

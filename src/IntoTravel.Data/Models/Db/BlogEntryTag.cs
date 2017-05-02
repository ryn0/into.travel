using IntoTravel.Data.DbModels.BaseDbModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class BlogEntryTag : StateInfo
    {
        [Column(Order = 0), Key, ForeignKey("BlogEntry")]
        public int BlogEntryId { get; set; }

        [Column(Order = 1), Key, ForeignKey("Tag")]
        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }
    }
}

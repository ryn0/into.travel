using IntoTravel.Data.DbModels.BaseDbModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class BlogEntryTag : StateInfo
    {
        [Column(Order = 0), Key, ForeignKey("BlogEntry")]
        public int BlogEntryId { get; set; }

        public virtual BlogEntry BlogEntry { get; set; }

        [Column(Order = 1), Key, ForeignKey("Tag")]
        public int TagId { get; set; }
  
        public Tag Tag { get; set; }
    }
}

using IntoTravel.Data.DbModels.BaseDbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class BlogEntry : StateInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogEntryId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Key { get; set; }

        public string Content { get; set; }

        public bool IsLive { get; set; }

        public DateTime BlogPublishDateTimeUtc { get; set; }

        [StringLength(160)]
        public string MetaDescription { get; set; }

        public virtual ICollection<BlogEntryTag> BlogEntryTags { get; set; }

        public virtual List<BlogEntryPhoto> Photos { get; set; } = new List<BlogEntryPhoto>();
    }
}

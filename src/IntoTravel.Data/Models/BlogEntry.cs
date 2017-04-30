using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntoTravel.Data.Models
{
    
    public class BlogEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogEntryId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
    }
     
}

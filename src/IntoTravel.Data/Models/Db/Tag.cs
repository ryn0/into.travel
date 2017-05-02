using IntoTravel.Data.DbModels.BaseDbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IntoTravel.Data.Models
{
    public class Tag : StateInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagId { get; set; }

        public string Name { get; set; }

    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]

        [StringLength(36)]
        public override string UserId
        {
            get
            {
                return base.UserId;
            }

            set
            {
                base.UserId = value;
            }
        }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public override string RoleId
        {
            get
            {
                return base.RoleId;
            }

            set
            {
                base.RoleId = value;
            }
        }
    }
}

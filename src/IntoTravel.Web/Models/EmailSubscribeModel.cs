using System.ComponentModel.DataAnnotations;

namespace IntoTravel.Web.Models
{
    public class EmailSubscribeModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
    }
}

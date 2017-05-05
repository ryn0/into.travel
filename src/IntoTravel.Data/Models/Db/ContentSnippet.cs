using IntoTravel.Data.DbModels.BaseDbModels;
using IntoTravel.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntoTravel.Data.Models.Db
{
    public class ContentSnippet : StateInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContentSnippetId { get; set; }

        public SnippetType SnippetType { get; set; }

        public string Content { get; set; }
    }
}

using IntoTravel.Data.Enums;
using System.Collections.Generic;

namespace IntoTravel.Services.Models
{
    public class ContentSnippetModel
    {
        public int ContentSnippetId { get; set; }

        public SnippetType SnippetType { get; set; }

        public string Content { get; set; }

    }

    public class ContentSnippetListModel
    {
        public List<ContentSnippetModel> Items { get; set; } = new List<ContentSnippetModel>();
    }
}

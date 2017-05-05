using IntoTravel.Data.Enums;
using System.Collections.Generic;

namespace IntoTravel.Web.Models
{
    public class ContentSnippetEditModel
    {
        public int ContentSnippetId { get; set; }

        public SnippetType SnippetType { get; set; }

        public string Content { get; set; }
         
    }

    public class ContentSnippetEditListModel
    {
        public List<ContentSnippetEditModel> Items { get; set; } = new List<ContentSnippetEditModel>();
    }
}

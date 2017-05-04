using System;
using System.Collections.Generic;

namespace IntoTravel.Web.Models
{
    public class BlogManagementListModel
    {
        public int PageCount { get; set; }

        public int Total { get; set; }

        public int CurrentPageNumber { get; set; }

        public int QuantityPerPage { get; set; }

        public List<BlogManagementEntryItemModel> Items { get; set; } = new List<BlogManagementEntryItemModel>();
    }

    public class BlogManagementEntryItemModel
    {
        public int BlogEntryId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }

        public string Key { get; set; }

        public bool IsLive { get;  set; }
        public string LiveUrlPath { get; set; }
        public string PreviewUrlPath { get; set; }
    }
}

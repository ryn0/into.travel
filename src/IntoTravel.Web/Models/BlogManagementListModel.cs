using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntoTravel.Web.Models
{
    public class BlogManagementListModel
    {
        public List<BlogManagementEntryItemModel> Items { get; set; } = new List<BlogManagementEntryItemModel>();
    }

    public class BlogManagementEntryItemModel
    {
        public int BlogEntryId { get; set; }

        public DateTime CreateDate { get; set; }

        public string Title { get; set; }
    }
}

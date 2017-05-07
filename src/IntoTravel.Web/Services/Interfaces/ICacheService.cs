using IntoTravel.Data.Enums;
using IntoTravel.Web.Models;

namespace IntoTravel.Web.Services.Interfaces
{

    public interface ICacheService
    {
        ContentSnippetDisplayModel GetSnippet(SnippetType snippetType);

        void ClearSnippetCache(SnippetType snippetType);
    }
}

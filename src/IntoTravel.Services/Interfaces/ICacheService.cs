using IntoTravel.Data.Enums;
using IntoTravel.Services.Models;

namespace IntoTravel.Services.Interfaces
{

    public interface ICacheService
    {
        ContentSnippetModel GetSnippet(SnippetType snippetType);

        void ClearSnippetCache(SnippetType snippetType);
    }
}

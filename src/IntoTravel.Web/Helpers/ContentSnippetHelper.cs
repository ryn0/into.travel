using IntoTravel.Data.Enums;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IntoTravel.Web.Helpers
{
    public class ContentSnippetHelper : IContentSnippetHelper
    {
        const string CachePrefix = "snippet";
        IMemoryCache _memoryCache;

        private readonly IContentSnippetRepository _contentSnippetRepository;

        public ContentSnippetHelper(
            IMemoryCache memoryCache, 
            IContentSnippetRepository contentSnippetRepository)
        {
            _memoryCache = memoryCache;
            _contentSnippetRepository = contentSnippetRepository;
        }

        public void ClearCache(SnippetType snippetType)
        {
            var cacheKey = CachePrefix + snippetType.ToString();

            _memoryCache.Remove(cacheKey);
        }

        public ContentSnippetDisplayModel GetSnippet(SnippetType snippetType)
        {
            var cacheKey = CachePrefix + snippetType.ToString();

            if (_memoryCache.TryGetValue(cacheKey, out ContentSnippetDisplayModel snippet))
            {
                return new ContentSnippetDisplayModel()
                {
                    Content = snippet.Content,
                    SnippetType = snippet.SnippetType
                };
            }
            else
            {
                var dbModel = _contentSnippetRepository.Get(snippetType);

                if (dbModel == null)
                    return new ContentSnippetDisplayModel();

                var model = new ContentSnippetDisplayModel()
                {
                    Content = dbModel.Content,
                    SnippetType = dbModel.SnippetType
                };

                _memoryCache.Set(cacheKey, model);

                return model;
            }
        }
    }
    
    public interface IContentSnippetHelper
    {
        ContentSnippetDisplayModel GetSnippet(SnippetType snippetType);

        void ClearCache(SnippetType snippetType);
    }
}

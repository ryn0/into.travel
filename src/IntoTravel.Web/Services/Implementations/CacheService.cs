using IntoTravel.Data.Enums;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Web.Models;
using IntoTravel.Web.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IntoTravel.Web.Services.Implementations
{
    public class CacheService : ICacheService
    {
        const string SnippetCachePrefix = "snippet";
        IMemoryCache _memoryCache;

        private readonly IContentSnippetRepository _contentSnippetRepository;

        public CacheService(
            IMemoryCache memoryCache,
            IContentSnippetRepository contentSnippetRepository)
        {
            _memoryCache = memoryCache;
            _contentSnippetRepository = contentSnippetRepository;
        }

        public void ClearSnippetCache(SnippetType snippetType)
        {
            var cacheKey = SnippetCachePrefix + snippetType.ToString();

            _memoryCache.Remove(cacheKey);
        }

        public ContentSnippetDisplayModel GetSnippet(SnippetType snippetType)
        {
            var cacheKey = SnippetCachePrefix + snippetType.ToString();

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
}

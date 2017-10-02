using IntoTravel.Data.Enums;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Services.Interfaces;
using IntoTravel.Services.Models;
using Microsoft.Extensions.Caching.Memory;

namespace IntoTravel.Services.Implementations
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

        public ContentSnippetModel GetSnippet(SnippetType snippetType)
        {
            var cacheKey = SnippetCachePrefix + snippetType.ToString();

            if (_memoryCache.TryGetValue(cacheKey, out ContentSnippetModel snippet))
            {
                return new ContentSnippetModel()
                {
                    Content = snippet.Content,
                    SnippetType = snippet.SnippetType
                };
            }
            else
            {
                var dbModel = _contentSnippetRepository.Get(snippetType);

                if (dbModel == null)
                    return new ContentSnippetModel();

                var model = new ContentSnippetModel()
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

using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class StoriesService : IStoriesService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public StoriesService(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClient = httpClientFactory.CreateClient();
        }

        public List<Story> GetStories()
        {
            return new List<Story> { new() };
        }
    }
}

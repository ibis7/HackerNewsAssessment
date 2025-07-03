using HackerNewsAPI.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAPI.Services
{
    public class SearchService(IMemoryCache cache, IStoriesService storiesService, ILogger<SearchService> logger) : ISearchService
    {
        private const string CachedStories = "NewestStories";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

        public async Task<List<Story>> GetFilteredStoriesAsync(SearchRequest searchRequest)
        {
            var baseStories = await GetNewestStoriesAsync();

            return FilterStories(searchRequest, baseStories);
        }

        private async Task<List<Story>> GetNewestStoriesAsync()
        {
            //Implement caching
            //Bypass cache when searching?
            if (cache.TryGetValue(CachedStories, out List<Story>? cachedStories) && cachedStories != null)
            {
                return cachedStories;
            }
            else
            {
                var storyIds = await storiesService.GetNewestStoryIdsAsync();

                //Make this in paralel
                //Only get the ones we are going to show??

                var storyTasks = storyIds.Select(id => storiesService.GetStoryDetailsAsync(id));
                var storyResults = await Task.WhenAll(storyTasks);

                var stories = storyResults
                    .OfType<Story>()
                    .ToList() ?? [];

                cache.Set(CachedStories, stories, CacheDuration);

                return stories;
            }
        }

        private static List<Story> FilterStories(SearchRequest searchRequest, List<Story> storiesUnfiltered)
        {
            var query = storiesUnfiltered.AsQueryable();

            if (searchRequest.IsSearching())
            {
                //Search for other fields?
                var searchTerm = searchRequest.SearchTerm!.Trim();
                query = query.Where(x => (!string.IsNullOrEmpty(x.Url) && x.Url.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
                    || (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)));
            }


            if (searchRequest.IsSorting())
            {
                var isAsc = searchRequest.IsSortingAscending!.Value;

                switch (searchRequest.SortedBy!.ToLower())
                {
                    case "url":
                        query = isAsc ? query.OrderBy(x => x.Url) : query.OrderByDescending(x => x.Url);
                        break;
                    case "title":
                        query = isAsc ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title);
                        break;
                    default:
                        break;
                }

            }

            query = query
                .Skip(searchRequest.PageSize * searchRequest.PageNumber)
                .Take(searchRequest.PageSize);

            return query.ToList();
        }
    }
}

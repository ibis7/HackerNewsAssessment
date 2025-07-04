using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services
{
    public class SearchService(ICachingService cachingService, ILogger<SearchService> logger) : ISearchService
    {
        public async Task<SearchResponse> GetFilteredNewestStoriesAsync(SearchRequest searchRequest)
        {
            var baseStories = await cachingService.GetNewestStoriesAsync();

            if (searchRequest.IsSearching())
            {
                baseStories = FilterStories(searchRequest, baseStories);
            }

            var orderedStories = OrderStories(searchRequest, baseStories);

            return new SearchResponse
            {
                Stories = orderedStories,
                TotalLength = baseStories.Count
            };
        }

        private static List<Story> FilterStories(SearchRequest searchRequest, List<Story> storiesUnfiltered)
        {
            var searchTerm = searchRequest.SearchTerm!.Trim();

            return storiesUnfiltered.Where(x =>
                (!string.IsNullOrEmpty(x.Url) && x.Url.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }

        private static List<Story> OrderStories(SearchRequest searchRequest, List<Story> stories)
        {
            var query = stories.AsQueryable();

            if (searchRequest.IsSorting())
            {
                var isAsc = searchRequest.IsSortingAscending!.Value;

                query = searchRequest.SortedBy! switch
                {
                    SortableColumnsEnum.Url => isAsc ? query.OrderBy(x => x.Url) : query.OrderByDescending(x => x.Url),
                    SortableColumnsEnum.Title => isAsc ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title),
                    _ => query
                };
            }

            query = query
                .Skip(searchRequest.PageSize * searchRequest.PageNumber)
                .Take(searchRequest.PageSize);

            return query.ToList();
        }
    }
}

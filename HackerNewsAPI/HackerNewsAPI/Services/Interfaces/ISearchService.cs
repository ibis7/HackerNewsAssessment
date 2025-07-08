using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services.Interfaces;

public interface ISearchService
{
    Task<SearchResponse> GetFilteredNewestStoriesAsync(SearchRequest searchRequest);
}

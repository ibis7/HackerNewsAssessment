using HackerNewsAPI.Models;

namespace HackerNewsAPI.Services.Interfaces;

public interface ICachingService
{
    Task<List<Story>> GetNewestStoriesAsync();
}

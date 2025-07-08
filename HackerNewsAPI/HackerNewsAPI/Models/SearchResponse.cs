namespace HackerNewsAPI.Models;

public class SearchResponse
{
    public List<Story> Stories { get; set; } = [];
    public int TotalLength { get; set; }
}


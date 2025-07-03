namespace HackerNewsAPI.Models
{
    public class SearchRequest
    {
        public string? SearchTerm { get; set; }

        public string? SortedBy { get; set; }
        public bool? IsSortingAscending { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public bool IsSorting()
        {
            return SortedBy != null && IsSortingAscending != null;
        }

        public bool IsSearching()
        {
            return !string.IsNullOrWhiteSpace(SearchTerm);
        }
    }
}

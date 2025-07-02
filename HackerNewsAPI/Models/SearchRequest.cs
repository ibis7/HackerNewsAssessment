namespace HackerNewsAPI.Models
{
    public class SearchRequest
    {
        public string? SearchTerm { get; set; }

        public SortableColumnsEnum? SortedBy { get; set; }
        public bool? IsSortingAscending { get; set; }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}

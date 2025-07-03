namespace HackerNewsAPI.Models
{
    public class Story
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }

        public bool HasVisibleData()
        {
            return !string.IsNullOrEmpty(Url) || !string.IsNullOrEmpty(Title);
        }
    }
}

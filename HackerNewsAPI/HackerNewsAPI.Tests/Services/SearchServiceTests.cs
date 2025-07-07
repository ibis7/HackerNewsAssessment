using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using HackerNewsAPI.Services.Interfaces;
using Moq;

namespace HackerNewsAPI.Tests.Services
{
    public class SearchServiceTests
    {
        private SearchService? _searchService;
        private Mock<ICachingService>? _cachingServiceMock;

        [SetUp]
        public void Setup()
        {
            _cachingServiceMock = new Mock<ICachingService>();
            _searchService = new SearchService(_cachingServiceMock.Object);
        }

        [Test]
        public async Task GetFilteredNewestStoriesAsync_ReturnsAllStories_WhenNoSearchOrSort()
        {
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "test1", Url = "http://test.com" },
                new() { Id = 2, Title = "test2", Url = "http://test.com" }
            };
            _cachingServiceMock!.Setup(x => x.GetNewestStoriesAsync()).ReturnsAsync(stories);

            var request = new SearchRequest
            {
                PageSize = 10,
                PageNumber = 0
            };

            var result = await _searchService!.GetFilteredNewestStoriesAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Stories, Has.Count.EqualTo(2));
                Assert.That(result.TotalLength, Is.EqualTo(2));
                Assert.That(result.Stories.Any(s => s.Title == "test1"), Is.True);
                Assert.That(result.Stories.Any(s => s.Title == "test2"), Is.True);
            });
        }

        [Test]
        public async Task GetFilteredNewestStoriesAsync_FiltersBySearchTerm()
        {
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "test1", Url = "http://test.com" },
                new() { Id = 2, Title = "test2", Url = "http://test.com" }
            };
            _cachingServiceMock!.Setup(x => x.GetNewestStoriesAsync()).ReturnsAsync(stories);

            var request = new SearchRequest
            {
                SearchTerm = "test1",
                PageSize = 10,
                PageNumber = 0
            };

            var result = await _searchService!.GetFilteredNewestStoriesAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Stories, Has.Count.EqualTo(1));
                Assert.That(result.TotalLength, Is.EqualTo(1));
                Assert.That(result.Stories[0].Title, Is.EqualTo("test1"));
            });
        }

        [Test]
        public async Task GetFilteredNewestStoriesAsync_SortsByTitleDescending()
        {
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "test1", Url = "http://test.com" },
                new() { Id = 2, Title = "test2", Url = "http://test.com" }
            };
            _cachingServiceMock!.Setup(x => x.GetNewestStoriesAsync()).ReturnsAsync(stories);

            var request = new SearchRequest
            {
                SortedBy = SortableColumnsEnum.Title,
                IsSortingAscending = false,
                PageSize = 10,
                PageNumber = 0
            };

            var result = await _searchService!.GetFilteredNewestStoriesAsync(request);

            Assert.Multiple(() =>
            {
                Assert.That(result.Stories, Has.Count.EqualTo(2));
                Assert.That(result.Stories[0].Title, Is.EqualTo("test2"));
                Assert.That(result.Stories[1].Title, Is.EqualTo("test1"));
            });
        }

        [Test]
        public async Task GetFilteredNewestStoriesAsync_SortsByUrlAscending()
        {
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "test1", Url = "http://b.com" },
                new() { Id = 2, Title = "test2", Url = "http://a.com" }
            };
            _cachingServiceMock!.Setup(x => x.GetNewestStoriesAsync()).ReturnsAsync(stories);
            var request = new SearchRequest
            {
                SortedBy = SortableColumnsEnum.Url,
                IsSortingAscending = true,
                PageSize = 10,
                PageNumber = 0
            };
            var result = await _searchService!.GetFilteredNewestStoriesAsync(request);
            Assert.Multiple(() =>
            {
                Assert.That(result.Stories, Has.Count.EqualTo(2));
                Assert.That(result.Stories[0].Url, Is.EqualTo("http://a.com"));
                Assert.That(result.Stories[1].Url, Is.EqualTo("http://b.com"));
            });
        }

        [Test]
        public async Task GetFilteredNewestStoriesAsync_PaginatesResults()
        {
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "test1", Url = "http://test.com" },
                new() { Id = 2, Title = "test2", Url = "http://test.com" },
                new() { Id = 3, Title = "test3", Url = "http://test.com" }
            };
            _cachingServiceMock!.Setup(x => x.GetNewestStoriesAsync()).ReturnsAsync(stories);

            var request = new SearchRequest
            {
                PageSize = 1,
                PageNumber = 1
            };

            var result = await _searchService!.GetFilteredNewestStoriesAsync(request);

            Assert.Multiple(() =>
            {

                Assert.That(result.Stories, Has.Count.EqualTo(1));
                Assert.That(result.Stories[0].Title, Is.EqualTo("test2"));
                Assert.That(result.TotalLength, Is.EqualTo(3));
            });
        }
    }
}
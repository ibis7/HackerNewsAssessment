using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace HackerNewsAPI.Tests.Services
{
    public class CachingServiceTests
    {
        private CachingService? _service;
        private Mock<IMemoryCache>? _cacheMock;
        private Mock<IThirdPartyService>? _thirdPartyServiceMock;
        private Mock<ILogger<CachingService>>? _loggerMock;

        [SetUp]
        public void Setup()
        {
            _cacheMock = new Mock<IMemoryCache>();
            _cacheMock!.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            _thirdPartyServiceMock = new Mock<IThirdPartyService>();
            _loggerMock = new Mock<ILogger<CachingService>>();
            _service = new CachingService(
                _cacheMock.Object,
                _thirdPartyServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetNewestStoriesAsync_WhenCacheIsAvailable_ReturnsCachedStories()
        {
            var cachedStories = new List<Story> { new() { Id = 1, Title = "Test", Url = "http://test.com" } };
            object? outStories = cachedStories;

            _cacheMock!.Setup(c => c.TryGetValue(It.IsAny<object>(), out outStories)).Returns(true);

            _thirdPartyServiceMock!.Setup(s => s.GetNewestStoryIdsAsync()).ReturnsAsync(cachedStories.Select(s => s.Id).ToList());

            var result = await _service!.GetNewestStoriesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(1));
            _thirdPartyServiceMock.Verify(s => s.GetStoryDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task GetNewestStoriesAsync_WhenCacheIsUnavailable_ReturnsNewStories()
        {
            object? outObj = null;
            _cacheMock!.Setup(c => c.TryGetValue(It.IsAny<object>(), out outObj)).Returns(false);

            var newStoryIds = new List<int> { 2 };
            var newStories = new List<Story> { new() { Id = 2, Title = "New", Url = "http://new.com" } };

            _thirdPartyServiceMock!.Setup(s => s.GetNewestStoryIdsAsync()).ReturnsAsync(newStoryIds);
            _thirdPartyServiceMock.Setup(s => s.GetStoryDetailsAsync(It.IsAny<int>())).ReturnsAsync((int id) => newStories.First(s => s.Id == id));

            var result = await _service!.GetNewestStoriesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(2));
        }

        [Test]
        public async Task GetNewestStoriesAsync_WhenSomeAreUncached_AppendsNewStories()
        {
            var cachedStories = new List<Story> { new() { Id = 1, Title = "Cached", Url = "http://cached.com" } };
            var newestStoryIds = new List<int> { 1, 2 };

            object? outStories = cachedStories;

            _cacheMock!.Setup(c => c.TryGetValue(It.IsAny<object>(), out outStories)).Returns(true);

            _thirdPartyServiceMock!.Setup(s => s.GetNewestStoryIdsAsync()).ReturnsAsync(newestStoryIds);

            var newStory = new Story { Id = 2, Title = "Uncached", Url = "http://uncached.com" };
            _thirdPartyServiceMock.Setup(s => s.GetStoryDetailsAsync(2)).ReturnsAsync(newStory);

            var result = await _service!.GetNewestStoriesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.Any(s => s.Id == 2), Is.True);
                Assert.That(result.Any(s => s.Id == 1), Is.True);
            });
        }

        [Test]
        public async Task GetNewestStoriesAsync_CachesAndReturnsStories()
        {
            var newStoryIds = new List<int> { 10, 20, 30 };
            var newStories = new List<Story>
            {
                new() { Id = 10, Title = "Story 10", Url = "http://10.com" },
                new() { Id = 20, Title = "Story 20", Url = "http://20.com" },
                new() { Id = 30, Title = "Story 30", Url = "http://30.com" }
            };

            _thirdPartyServiceMock!.Setup(s => s.GetNewestStoryIdsAsync()).ReturnsAsync(newStoryIds);
            _thirdPartyServiceMock.Setup(s => s.GetStoryDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => newStories.First(s => s.Id == id));

            var result = await _service!.GetNewestStoriesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result.Select(s => s.Id), Is.EquivalentTo(newStoryIds));
            _cacheMock!.Verify(c => c.CreateEntry(It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task GetNewestStoriesAsync_ReturnsVisibleStoriesOnly()
        {
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Story>
            {
                new() { Id = 1, Title = "Visible", Url = "http://1.com" },
                new() { Id = 2 },
                new() { Id = 3, Title = "Visible2", Url = "http://3.com" }
            };

            _thirdPartyServiceMock!.Setup(s => s.GetNewestStoryIdsAsync()).ReturnsAsync(storyIds);
            _thirdPartyServiceMock!.Setup(s => s.GetStoryDetailsAsync(1)).ReturnsAsync(stories[0]);
            _thirdPartyServiceMock.Setup(s => s.GetStoryDetailsAsync(2)).ReturnsAsync(stories[1]);
            _thirdPartyServiceMock.Setup(s => s.GetStoryDetailsAsync(3)).ReturnsAsync(stories[2]);

            var result = await _service!.GetNewestStoriesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!, Has.Count.EqualTo(2));
                Assert.That(result.Any(s => s.Id == 2), Is.False);
                Assert.That(result.Any(s => s.Id == 1), Is.True);
                Assert.That(result.Any(s => s.Id == 3), Is.True);
            });
        }
    }
}
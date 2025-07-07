using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace HackerNewsAPI.Tests.Services
{
    public class ThirdPartyServiceTests
    {
        private ThirdPartyService? _service;
        private Mock<IHttpClientFactory>? _httpClientFactoryMock;
        private Mock<HttpMessageHandler>? _handlerMock;

        [SetUp]
        public void Setup()
        {
            _handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            _httpClientFactoryMock.Setup(f => f.CreateClient("HackerNews")).Returns(httpClient);

            _service = new ThirdPartyService(
                _httpClientFactoryMock.Object,
                new Mock<ILogger<ThirdPartyService>>().Object
            );
        }

        [Test]
        public async Task GetStoryDetailsAsync_WhenApiReturnsStory_ReturnsStory()
        {
            var storyId = 1;
            var expectedStory = new Story { Id = storyId, Title = "test", Url = "http://test" };

            _handlerMock!.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains($"item/{storyId}.json")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = System.Net.Http.Json.JsonContent.Create(expectedStory)
                });

            var result = await _service!.GetStoryDetailsAsync(storyId);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result!.Id, Is.EqualTo(expectedStory.Id));
                Assert.That(result.Title, Is.EqualTo(expectedStory.Title));
                Assert.That(result.Url, Is.EqualTo(expectedStory.Url));
            });

            _handlerMock!.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task GetStoryDetailsAsync_WhenApiThrowsException_ReturnsNull()
        {
            _handlerMock!.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Error"));

            var result = await _service!.GetStoryDetailsAsync(99);

            Assert.That(result, Is.Null);
            _handlerMock!.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task GetNewestStoryIdsAsync_WhenApiReturnsIds_ReturnsListOfIds()
        {
            var expectedIds = new List<int> { 1, 2, 3, 4 };

            _handlerMock!.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("newstories.json")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = System.Net.Http.Json.JsonContent.Create(expectedIds)
                });

            var result = await _service!.GetNewestStoryIdsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EquivalentTo(expectedIds));

            _handlerMock!.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Test]
        public async Task GetNewestStoryIdsAsync_WhenApiThrowsException_ReturnsEmptyList()
        {
            _handlerMock!.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Error"));

            var result = await _service!.GetNewestStoryIdsAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);

            _handlerMock!.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

    }
}
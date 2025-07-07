using HackerNewsAPI.Controllers;
using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HackerNewsAPI.Tests.Controllers
{
    public class StoriesControllerTests
    {
        private StoriesController? _controller;
        private Mock<ISearchService>? _searchServiceMock;

        [SetUp]
        public void Setup()
        {
            _searchServiceMock = new Mock<ISearchService>();
            _controller = new StoriesController(_searchServiceMock.Object);
        }

        [Test]
        public async Task ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var searchRequest = new SearchRequest();
            _controller!.ModelState.AddModelError("PageNumber", "Cannot be negative");

            var result = await _controller.GetNewestStoriesAsync(searchRequest);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task ReturnsOkWithStories_WhenModelStateIsValid()
        {
            var searchRequest = new SearchRequest { PageNumber = 1, PageSize = 10 };
            var response = new SearchResponse
            {
                Stories = [],
                TotalLength = 0
            };

            _searchServiceMock!
                .Setup(s => s.GetFilteredNewestStoriesAsync(searchRequest))
                .ReturnsAsync(response);

            var result = await _controller!.GetNewestStoriesAsync(searchRequest);

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult!.Value, Is.EqualTo(response));
        }
    }
}
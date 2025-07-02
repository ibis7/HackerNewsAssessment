using HackerNewsAPI.Models;
using HackerNewsAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController(IStoriesService storiesService, ILogger<StoriesController> logger) : ControllerBase
    {
        private readonly IStoriesService _storiesService = storiesService;

        [HttpPost("newest-stories")]
        public async Task<IActionResult> GetNewestStoriesAsync([FromBody] SearchRequest searchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stories = await _storiesService.GetFilteredStoriesAsync(searchRequest);
            return Ok(stories);
        }
    }
}

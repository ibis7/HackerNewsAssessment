using HackerNewsAPI.Models;
using HackerNewsAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoriesController(ISearchService searchService) : ControllerBase
    {
        [HttpPost("newest-stories")]
        public async Task<IActionResult> GetNewestStoriesAsync([FromBody] SearchRequest searchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stories = await searchService.GetFilteredNewestStoriesAsync(searchRequest);
            return Ok(stories);
        }
    }
}

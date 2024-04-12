using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Microsoft.Extensions.Logging;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    public ContentController(ILogger<ContentController> logger, IContentsManager manager)
    {
        _manager = manager;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        _logger.LogInformation("Requesting all contents...");
        
        try
        {
            var contents = await _manager.GetManyContents().ConfigureAwait(false);

            if (!contents.Any())
            {
                _logger.LogWarning($"Returned {contents.Count()} contents.");
                return NotFound();
            }
            
            _logger.LogInformation($"Returned {contents.Count()} contents.");
            return Ok(contents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting contents.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        try
        {
            _logger.LogInformation($"Attempting to retrieve content with ID: {id}");

            var content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogWarning($"Content with ID: {id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Successfully retrieved content with ID: {id}");
            return Ok(content);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while retrieving content with ID: {id}. Error: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        _logger.LogInformation("Received request to create content.");

        try
        {
            var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);

            if (createdContent == null)
            {
                _logger.LogWarning("Failed to create content. Null content returned.");
                return Problem();
            }

            _logger.LogInformation("Content created successfully.");
            return Ok(createdContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating content.");
            return Problem("An error occurred while processing your request.", statusCode: 500);
        }
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        try
        {
            _logger.LogInformation($"Attempting to update content with ID: {id}");

            var updatedContent = await _manager.UpdateContent(id, content.ToDto());

            if (updatedContent == null)
            {
                _logger.LogWarning($"Content with ID: {id} not found");
                return NotFound();
            }

            _logger.LogInformation($"Content with ID: {id} updated successfully");

            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating content with ID: {id}: {ex.Message}");
            return StatusCode(500, "An error occurred while updating the content.");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        _logger.LogInformation($"Deleting content with ID: {id}");

        try
        {
            var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
            _logger.LogInformation($"Content with ID {deletedId} deleted successfully.");
            return Ok(deletedId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while deleting content with ID {id}: {ex.Message}");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        try
        {
            _logger.LogInformation($"Adding genres to content with id {id}");
            
            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
                return NotFound();

            var newGenres = new List<string>();
            foreach (var gen in genre)
            {
                if (!content.GenreList.Contains(gen))
                    newGenres.Add(gen);
                else
                {
                    _logger.LogWarning($"Genre '{gen}' already exists in content with id {id}");
                    return BadRequest(new MessageOutput { Message = "Genre already exists" });
                }
            }

            var updatedContentDto = new ContentDto
            (
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                content.GenreList.Concat(newGenres).ToList()
            );

            var updatedContent = await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);
            _logger.LogInformation($"Genres added successfully to content with id {id}");

            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while adding genres to content with id {id}: {ex.Message}");
            return StatusCode(500, new MessageOutput { Message = "An error occurred while adding genres" });
        }
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        try
        {
            var content = await _manager.GetContent(id).ConfigureAwait(false);
            if (content == null)
            {
                _logger.LogWarning($"Content with id '{id}' not found.");
                return NotFound();
            }

            var genreList = content.GenreList.ToList();

            genreList.RemoveAll(genre.Contains);

            var updatedContent = await _manager.UpdateContent(id, new ContentDto
            (
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                genreList
            )).ConfigureAwait(false);

            _logger.LogInformation($"Genres removed from content with id '{id}'.");
            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while removing genres from content with id '{id}'.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
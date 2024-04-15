using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Exceptions;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;
    private readonly ILogger<ContentController> _logger;
    private readonly ICacheService<Content> _cacheService;

    public ContentController(ILogger<ContentController> logger, IContentsManager manager,
        ICacheService<Content> cacheService)
    {
        _manager = manager;
        _logger = logger;
        _cacheService = cacheService;
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
            
            var cachedContent = await _cacheService.GetAsync(id).ConfigureAwait(false);
            if (cachedContent != null)
            {
                _logger.LogInformation($"Content with ID: {id} found in cache");
                return Ok(cachedContent);
            }

            var content = await _manager.GetContent(id).ConfigureAwait(false);

            if (content == null)
            {
                _logger.LogWarning($"Content with ID: {id} not found");
                return NotFound();
            }
            
            await _cacheService.SetAsync(content.Id, content).ConfigureAwait(false);
            _logger.LogInformation($"Content with ID: {id} retrieved from database and cached successfully");

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
            
            await _cacheService.SetAsync(createdContent.Id, createdContent).ConfigureAwait(false);

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
            
            await _cacheService.SetAsync(id, updatedContent);

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
            await _cacheService.RemoveAsync(id);
            
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
    public async Task<IActionResult> AddGenres(Guid id, [FromBody] IEnumerable<string> genres)
    {
        try
        {
            var content = await GetContentAsync(id);
            if (content == null)
                return NotFound();

            foreach (var genre in genres)
            {
                try
                {
                    content = content.AddGenre(genre);
                }
                catch (GenreAlreadyExistsException ex)
                {
                    _logger.LogWarning(ex.Message);
                    return BadRequest(new MessageOutput { Message = ex.Message });
                }
            }

            await UpdateContentAsync(id, content);

            _logger.LogInformation($"Genres added successfully to content with id {id}");

            return Ok(content);
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
            var content = await _cacheService.GetAsync(id).ConfigureAwait(false);
            
            if (content == null)
            {
                content = await _manager.GetContent(id).ConfigureAwait(false);
                if (content == null)
                {
                    _logger.LogWarning($"Content with id '{id}' not found.");
                    return NotFound();
                }
            }

            var genreList = content.GenreList.ToList();

            genreList.RemoveAll(genre.Contains);

            await _cacheService.RemoveAsync(content.Id);

            var updatedContentDto = await _manager.UpdateContent(id, new ContentDto
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

            var updatedContent = new Content
            (
                content.Id,
                content.Title,
                content.SubTitle,
                content.Description,
                content.ImageUrl,
                content.Duration,
                content.StartTime,
                content.EndTime,
                genreList
            );
                
            await _cacheService.SetAsync(id, updatedContent);

            _logger.LogInformation($"Genres removed from content with id '{id}'.");
            return Ok(updatedContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while removing genres from content with id '{id}'.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
    
    private async Task<Content> GetContentAsync(Guid id)
    {
        var content = await _cacheService.GetAsync(id);
        if (content == null)
            content = await _manager.GetContent(id).ConfigureAwait(false);

        return content;
    }

    private async Task UpdateContentAsync(Guid id, Content content)
    {
        await _cacheService.SetAsync(id, content);

        var updatedContentDto = new ContentDto
        (
            content.Title,
            content.SubTitle,
            content.Description,
            content.ImageUrl,
            content.Duration,
            content.StartTime,
            content.EndTime,
            content.GenreList
        );

        await _manager.UpdateContent(id, updatedContentDto).ConfigureAwait(false);
    }
}
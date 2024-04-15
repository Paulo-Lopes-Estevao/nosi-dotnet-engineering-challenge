using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NOS.Engineering.Challenge.API.Controllers;
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Moq;
using NOS.Engineering.Challenge.API.Models;

namespace NOS.Engineering.Challenge.Tests;

public class ContentControllerTests
{
    private readonly Mock<IContentsManager> _contentsManagerMock;
    private readonly Mock<ILogger<ContentController>> _loggerMock;
    private readonly Mock<ICacheService<Content>> _cacheServiceMock;
    
    public ContentControllerTests()
    {
        _contentsManagerMock = new Mock<IContentsManager>();
        _loggerMock = new Mock<ILogger<ContentController>>();
        _cacheServiceMock = new Mock<ICacheService<Content>>();
    }
    
    [Fact]
        public async Task CreateContent_ValidInput_Success()
        {
            // Arrange
            var contentInput = new ContentInput
            {
                Title = "Test Title",
                SubTitle = "Test Subtitle",
                Description = "Test Description",
                ImageUrl = "https://example.com/test.jpg",
                Duration = 120,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(2),
                //GenreList = new List<string> { "Action", "Adventure" }
            };

            var contentDto = contentInput.ToDto();
            var createdContent = new Content
            (
                Guid.NewGuid(),
                contentDto.Title!,
                contentDto.SubTitle!,
                contentDto.Description!,
                contentDto.ImageUrl!,
                contentDto.Duration.GetValueOrDefault(),
                contentDto.StartTime.GetValueOrDefault(),
                contentDto.EndTime.GetValueOrDefault(),
                contentDto.GenreList.ToList()
            );

            _contentsManagerMock.Setup(manager => manager.CreateContent(contentDto))
                .ReturnsAsync(createdContent);

            var controller = new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.CreateContent(contentInput) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var resultContent = result.Value as Content;
            Assert.NotNull(resultContent);
            Assert.Equal(createdContent.Id, resultContent.Id);
            Assert.Equal(createdContent.Title, resultContent.Title);
            Assert.Equal(createdContent.SubTitle, resultContent.SubTitle);
            Assert.Equal(createdContent.Description, resultContent.Description);
            Assert.Equal(createdContent.ImageUrl, resultContent.ImageUrl);
            Assert.Equal(createdContent.Duration, resultContent.Duration);
            Assert.Equal(createdContent.StartTime, resultContent.StartTime);
            Assert.Equal(createdContent.EndTime, resultContent.EndTime);
            Assert.Equal(createdContent.GenreList, resultContent.GenreList);
        }
        
        [Fact]
        public async Task GetContent_ContentFound_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var expectedContent = new Content(
                contentId,
                "Test Content",
                "Test Subtitle",
                "Test Description",
                "https://example.com/test.jpg",
                120,
                DateTime.UtcNow,
                DateTime.UtcNow,
                new List<string> { "Action", "Adventure" }
            );

            _cacheServiceMock.Setup(service => service.GetAsync(contentId))
                .ReturnsAsync(expectedContent);

            var controller = new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.GetContent(contentId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var resultContent = result.Value as Content;
            Assert.NotNull(resultContent);
            Assert.Equal(expectedContent.Id, resultContent.Id);
            Assert.Equal(expectedContent.Title, resultContent.Title);
            Assert.Equal(expectedContent.SubTitle, resultContent.SubTitle);
            Assert.Equal(expectedContent.Description, resultContent.Description);
            Assert.Equal(expectedContent.ImageUrl, resultContent.ImageUrl);
            Assert.Equal(expectedContent.Duration, resultContent.Duration);
            Assert.Equal(expectedContent.StartTime, resultContent.StartTime);
            Assert.Equal(expectedContent.EndTime, resultContent.EndTime);
            Assert.Equal(expectedContent.GenreList, resultContent.GenreList);
        }
        
        [Fact]
        public async Task UpdateContent_ContentExists_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var inputContent = new ContentInput
            {
                Title = "Updated Title",
                SubTitle = "Updated Subtitle",
                Description = "Updated Description",
                ImageUrl = "https://example.com/updated.jpg",
                Duration = 150,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow,
            };

            var updatedContent = new Content(
                contentId,
                "Test Content",
                "Test Subtitle",
                "Test Description",
                "https://example.com/test.jpg",
                120,
                DateTime.UtcNow,
                DateTime.UtcNow,
                new List<string> { "Action", "Adventure" }
            );

            _contentsManagerMock.Setup(manager => manager.UpdateContent(contentId, inputContent.ToDto()))
                .ReturnsAsync(updatedContent);

            var controller = new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.UpdateContent(contentId, inputContent) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var resultContent = result.Value as Content;
            Assert.NotNull(resultContent);
            Assert.Equal(updatedContent.Id, resultContent.Id);
            Assert.Equal(updatedContent.Title, resultContent.Title);
            Assert.Equal(updatedContent.SubTitle, resultContent.SubTitle);
            Assert.Equal(updatedContent.Description, resultContent.Description);
            Assert.Equal(updatedContent.ImageUrl, resultContent.ImageUrl);
            Assert.Equal(updatedContent.Duration, resultContent.Duration);
            Assert.Equal(updatedContent.StartTime, resultContent.StartTime);
            Assert.Equal(updatedContent.EndTime, resultContent.EndTime);
            Assert.Equal(updatedContent.GenreList, resultContent.GenreList);
        }

    
        [Fact]
        public async Task DeleteContent_ContentExists_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var deletedId = contentId;

            _contentsManagerMock.Setup(manager => manager.DeleteContent(contentId))
                .ReturnsAsync(deletedId);

            var controller = new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.DeleteContent(contentId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deletedId, result.Value);
        }

        [Fact]
        public async Task AddGenres_ContentExistsAndGenresAreAdded_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var genresToAdd = new List<string> { "Drama", "Action" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now,
                DateTime.Now.AddHours(2), new List<string>());

            _cacheServiceMock.Setup(service => service.GetAsync(contentId)).ReturnsAsync(content);
            _contentsManagerMock.Setup(manager => manager.GetContent(contentId)).ReturnsAsync(content);
            _contentsManagerMock.Setup(manager => manager.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()))
                .ReturnsAsync(content);

            var controller =
                new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.AddGenres(contentId, genresToAdd) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var updatedContent = result.Value as Content;
            Assert.NotNull(updatedContent);
        }
        
        [Fact]
        public async Task RemoveGenres_ContentExistsAndGenresAreRemoved_Success()
        {
            // Arrange
            var contentId = Guid.NewGuid();
            var existingGenres = new List<string> { "Drama", "Action", "Comedy" };
            var genresToRemove = new List<string> { "Drama", "Action" };
            var content = new Content(contentId, "Title", "SubTitle", "Description", "ImageUrl", 120, DateTime.Now, DateTime.Now.AddHours(2), existingGenres);

            _cacheServiceMock.Setup(service => service.GetAsync(contentId)).ReturnsAsync(content);
            _contentsManagerMock.Setup(manager => manager.GetContent(contentId)).ReturnsAsync(content);
            _contentsManagerMock.Setup(manager => manager.UpdateContent(It.IsAny<Guid>(), It.IsAny<ContentDto>()))
                .ReturnsAsync(content);

            var controller = new ContentController(_loggerMock.Object, _contentsManagerMock.Object, _cacheServiceMock.Object);

            // Act
            var result = await controller.RemoveGenres(contentId, genresToRemove) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var updatedContent = result.Value as Content;
            Assert.NotNull(updatedContent);
            Assert.True(updatedContent.GenreList.SequenceEqual(new List<string> { "Comedy" }));
        }
}
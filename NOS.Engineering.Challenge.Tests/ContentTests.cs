using NOS.Engineering.Challenge.Exceptions;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Tests;

public class ContentTests
{
    // AddGenre
    [Fact]
    public void AddGenre_SuccessfullyAddsGenre_WhenGenreDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var newGenre = "Drama";

        // Act
        var updatedContent = content.AddGenre(newGenre);

        // Assert
        Assert.Contains(newGenre, updatedContent.GenreList);
    }
    
    [Fact]
    public void AddGenre_ThrowsException_WhenGenreAlreadyExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var existingGenre = "Action";

        // Act & Assert
        var exception = Assert.Throws<GenreAlreadyExistsException>(() => content.AddGenre(existingGenre));
        Assert.Equal($"Genre '{existingGenre}' already exists.", exception.Message);
    }
    
    [Fact]
    public void AddGenre_ThrowsArgumentException_WhenGenreIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        string newGenre = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => content.AddGenre(newGenre));
    }
    
    // RemoveGenre
    [Fact]
    public void RemoveGenre_SuccessfullyRemovesGenre_WhenGenreExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var genreToRemove = "Action";

        // Act
        var updatedContent = content.RemoveGenre(genreToRemove);

        // Assert
        Assert.DoesNotContain(genreToRemove, updatedContent.GenreList);
    }

    [Fact]
    public void RemoveGenre_ThrowsArgumentException_WhenGenreIsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        string genreToRemove = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => content.RemoveGenre(genreToRemove));
    }
    
    [Fact]
    public void RemoveGenre_ThrowsGenreNotFoundException_WhenGenreDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var genreToRemove = "Drama";

        // Act & Assert
        var exception = Assert.Throws<GenreNotFoundException>(() => content.RemoveGenre(genreToRemove));
        Assert.Equal($"Genre '{genreToRemove}' not found in the list of genres.", exception.Message);
    }
    
    // IsValid
    [Fact]
    public void IsValid_ReturnsTrue_WhenAllRequiredPropertiesAreSet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        // Act
        var isValid = content.IsValid();

        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void IsValid_ReturnsFalse_WhenTitleIsNullOrWhiteSpace()
    {
        // Arrange
        var id = Guid.NewGuid();
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, null, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        // Act
        var isValid = content.IsValid();

        // Assert
        Assert.False(isValid);
    }
    
    // CalculateAge
    [Fact]
    public void CalculateAge_ReturnsCorrectAge_WhenStartTimeIsSet()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today.AddYears(-5); // 5 years ago
        var endTime = DateTime.Today;
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        // Act
        var age = content.CalculateAge();

        // Assert
        Assert.Equal(5, age);
    }
    
    // HasGenre
    [Fact]
    public void HasGenre_ReturnsTrue_WhenGenreExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var genreToCheck = "Action";

        // Act
        var hasGenre = content.HasGenre(genreToCheck);

        // Assert
        Assert.True(hasGenre);
    }
    
    [Fact]
    public void HasGenre_ReturnsFalse_WhenGenreDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Title";
        var subTitle = "SubTitle";
        var description = "Description";
        var imageUrl = "ImageUrl";
        var duration = 120;
        var startTime = DateTime.Today;
        var endTime = DateTime.Today.AddDays(1);
        var genreList = new List<string> { "Action", "Adventure" };
        var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genreList);

        var genreToCheck = "Drama";

        // Act
        var hasGenre = content.HasGenre(genreToCheck);

        // Assert
        Assert.False(hasGenre);
    }
}
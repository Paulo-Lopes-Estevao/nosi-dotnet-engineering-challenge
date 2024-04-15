using System.ComponentModel.DataAnnotations;
using NOS.Engineering.Challenge.Exceptions;

namespace NOS.Engineering.Challenge.Models;

public class Content
{
    [Key]
    public Guid Id { get; }
    public string Title { get; }
    public string SubTitle { get; }
    public string Description { get; }
    public string ImageUrl { get; }
    public int Duration { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public IEnumerable<string> GenreList { get; }

    public Content(Guid id, string title, string subTitle, string description, string imageUrl, int duration, DateTime startTime, DateTime endTime, IEnumerable<string> genreList)
    {
        Id = id;
        Title = title;
        SubTitle = subTitle;
        Description = description;
        ImageUrl = imageUrl;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        GenreList = genreList;
    }

    public Content()
    {
        
    }
    
    public Content CreateContent(Content content)
    {
        if (string.IsNullOrWhiteSpace(content.Title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(content.Title));

        if (string.IsNullOrWhiteSpace(content.SubTitle))
            throw new ArgumentException("SubTitle cannot be null or empty.", nameof(content.SubTitle));

        if (string.IsNullOrWhiteSpace(content.Description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(content.Description));
        
        if(int.IsNegative(content.Duration))
            throw new ArgumentException("Duration must be greater than zero.", nameof(content.Duration));

        if (!ContentValidator.IsValidImageUrl(content.ImageUrl))
            throw new ArgumentException("Invalid image URL.", nameof(content.ImageUrl));
        
        if (content.StartTime >= content.EndTime)
            throw new ArgumentException("End time must be greater than start time.", nameof(content.EndTime));

        return new Content(content.Id, content.Title, content.SubTitle, content.Description, content.ImageUrl, content.Duration, content.StartTime, content.EndTime, content.GenreList ?? Enumerable.Empty<string>());
    }
    
    public Content AddGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be null or empty.", nameof(genre));

        if (HasGenre(genre))
        {
            throw new GenreAlreadyExistsException($"Genre '{genre}' already exists.");
        }

        var updatedGenres = GenreList.Append(genre);
        return new Content(Id, Title, SubTitle, Description, ImageUrl, Duration, StartTime, EndTime, updatedGenres);
    }
    
    public Content RemoveGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException("Genre cannot be null or empty.", nameof(genre));
        
        if (!HasGenre(genre))
        {
            throw new GenreNotFoundException($"Genre '{genre}' not found in the list of genres.");
        }

        var updatedGenres = GenreList.ToList();
        updatedGenres.RemoveAll(g => string.Equals(g, genre, StringComparison.OrdinalIgnoreCase));

        return new Content(Id, Title, SubTitle, Description, ImageUrl, Duration, StartTime, EndTime, updatedGenres);
    }
    
    public bool HasGenre(string genre)
    {
        return GenreList.Any(g => string.Equals(g, genre, StringComparison.OrdinalIgnoreCase));
    }
    
    public int CalculateAge()
    {
        return (DateTime.Today - StartTime).Days / 365;
    }
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(SubTitle) && !string.IsNullOrWhiteSpace(Description);
    }
}
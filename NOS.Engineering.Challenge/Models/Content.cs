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
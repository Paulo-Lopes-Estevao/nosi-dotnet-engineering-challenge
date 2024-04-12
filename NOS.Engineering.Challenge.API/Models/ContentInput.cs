using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.API.Models;

public class ContentInput
{
    public string? Title { get; set; }
    public string? SubTitle { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? Duration { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public ContentDto ToDto()
    {
        return new ContentDto(
            Title,
            SubTitle,
            Description,
            ImageUrl,
            Duration,
            StartTime,
            EndTime,
            new List<string>()
        );
    }
}

public class MessageOutput
{
    public string Message { get; set; }
}
using NOS.Engineering.Challenge.Cache;
using NOS.Engineering.Challenge.Models;

namespace NOS.Engineering.Challenge.Database;

public class MockData : IMockData<Content>
{
    private readonly ICacheService<Content> _cache;
    private readonly ICacheService<Content> _cacheService;

    public MockData(ICacheService<Content> cache, ICacheService<Content> cacheService)
    {
        _cache = cache;
        _cacheService = cacheService;
    }
    
    public IDictionary<Guid, Content> GenerateMocks()
    {
        IDictionary<Guid, Content> mockContents = new Dictionary<Guid, Content>();
        for (int i = 0; i < 10; i++)
        {
            Guid id = Guid.NewGuid();
            string title = GetMovieTitle(i + 1);
            string subTitle = "Subtitle for " + title;
            string description = "Description for " + title;
            string imageUrl = "https://example.com/" + title.Replace(' ', '_') + ".jpg";
            int duration = 120 + i;
            DateTime startTime = DateTime.Now.AddDays(i);
            DateTime endTime = DateTime.Now.AddDays(i).AddHours(2);
            List<string> genres = GetRandomGenres();

            var content = new Content(id, title, subTitle, description, imageUrl, duration, startTime, endTime, genres);
            
            _cache.SetAsync(id, content);
            
            mockContents.Add(id, content);
        }

        return mockContents;
    }
    
    private static string GetMovieTitle(int index)
    {
        // Sample movie titles
        string[] titles = {
            "Inception",
            "Interstellar",
            "The Dark Knight",
            "Pulp Fiction",
            "The Shawshank Redemption",
            "Forrest Gump",
            "The Godfather",
            "Fight Club",
            "Gladiator",
            "The Lord of the Rings: The Fellowship of the Ring"
        };

        return titles[index % titles.Length];
    }

    private static List<string> GetRandomGenres()
    {
        string[] genres = { "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Horror", "Mystery", "Romance", "Sci-Fi", "Thriller" };
        List<string> randomGenres = new List<string>();

        Random rand = new Random();
        int numGenres = rand.Next(1, 4); // Randomly choose 1 to 3 genres

        for (int i = 0; i < numGenres; i++)
        {
            int index = rand.Next(0, genres.Length);
            randomGenres.Add(genres[index]);
        }

        return randomGenres;
    }
}
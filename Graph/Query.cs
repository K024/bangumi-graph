
namespace Bangumi.Graph;

public class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Character> GetCharacters([Service] AppDbContext dbContext)
        => dbContext.Character;

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Episode> GetEpisodes([Service] AppDbContext dbContext)
        => dbContext.Episode;

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Subject> GetSubjects([Service] AppDbContext dbContext)
        => dbContext.Subject;

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Person> GetPersons([Service] AppDbContext dbContext)
        => dbContext.Person;
}

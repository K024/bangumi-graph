
namespace Bangumi.Graph;

public class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Character> GetCharacters([Service] AppDbContext dbContext)
        => dbContext.Character;

    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<Character> GetCharacterById(long id, [Service] AppDbContext dbContext)
        => dbContext.Character.Where(c => c.id == id);

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Episode> GetEpisodes([Service] AppDbContext dbContext)
        => dbContext.Episode;

    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<Episode> GetEpisodeById(long id, [Service] AppDbContext dbContext)
        => dbContext.Episode.Where(e => e.id == id);

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Subject> GetSubjects([Service] AppDbContext dbContext)
        => dbContext.Subject;

    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<Subject> GetSubjectById(long id, [Service] AppDbContext dbContext)
        => dbContext.Subject.Where(s => s.id == id);

    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Person> GetPersons([Service] AppDbContext dbContext)
        => dbContext.Person;

    [UseSingleOrDefault]
    [UseProjection]
    public IQueryable<Person> GetPersonById(long id, [Service] AppDbContext dbContext)
        => dbContext.Person.Where(p => p.id == id);
}

using Microsoft.EntityFrameworkCore;

namespace Bangumi.Graph;


public class AppDbContext : DbContext
{
    private bool has_nsfw_access;
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
        : base(options)
    {
        if (httpContextAccessor?.HttpContext is not null)
        {
            var user = httpContextAccessor.HttpContext.User;
            has_nsfw_access = user?.Identity?.IsAuthenticated ?? false;
        }
        else
        {
            has_nsfw_access = true;
            Console.WriteLine("warning: implicit has_nsfw_access");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!has_nsfw_access)
        {
            modelBuilder.Entity<Subject>()
                .HasQueryFilter(s => s.nsfw == false);
        }
    }

    public DbSet<Character> Character { get; set; } = null!;
    public DbSet<Episode> Episode { get; set; } = null!;
    public DbSet<Subject> Subject { get; set; } = null!;
    public DbSet<Person> Person { get; set; } = null!;

    public DbSet<Person_Character> Person_Characters { get; set; } = null!;
    public DbSet<Subject_Character> Subject_Characters { get; set; } = null!;
    public DbSet<Subject_Person> Subject_Persons { get; set; } = null!;
    public DbSet<Subject_Relation> Subject_Relations { get; set; } = null!;
}

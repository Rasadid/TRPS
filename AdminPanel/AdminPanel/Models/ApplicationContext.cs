using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Genre> Genre { get; set; } = null!;
    public DbSet<Evaluation> Evaluation { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LiterBotDB;Username=postgres;Password=feGEg46hSSgthntd");
        optionsBuilder.EnableSensitiveDataLogging();
    }
    
    
    public ApplicationContext(DbContextOptions options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
        modelBuilder.Entity<Book>()
            .HasMany(u => u.Genres)
            .WithMany(c => c.Books)
            .UsingEntity(j => j.ToTable("BookGenre"));
        */
    }
}
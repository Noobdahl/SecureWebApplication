using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Define a DbSet for each table in your database
    public DbSet<Product> Products { get; set; }
}
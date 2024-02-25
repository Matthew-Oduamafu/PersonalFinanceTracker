using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Image> Images => Set<Image>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // query filters
        builder.Entity<AppUser>().HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // get environment\
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // build config
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../PersonalFinanceTracker.Api"))
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true)
            .AddEnvironmentVariables()
            .Build();

        // get connection string
        var connectionString = config.GetConnectionString("DbConnection");
        // dbContext builder 
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // use mysql
        builder.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString));
        // return dbContext
        return new ApplicationDbContext(builder.Options);
    }
}
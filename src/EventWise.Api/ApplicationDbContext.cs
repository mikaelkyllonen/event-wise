namespace EventWise.Api;

using EventWise.Api.Events;

using Microsoft.EntityFrameworkCore;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<BaseEvent> Events { get; set; } = default!;
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<EventParticipant> EventParticipants { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
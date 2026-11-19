using Microsoft.EntityFrameworkCore;
using FitForge.Api.Models;

namespace FitForge.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<MuscleGroup> MuscleGroups => Set<MuscleGroup>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutPlan> WorkoutPlans => Set<WorkoutPlan>();
    public DbSet<WorkoutPlanItem> WorkoutPlanItems => Set<WorkoutPlanItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL wymaga cudzysłowów dla nazw z wielkimi literami, więc wszystko zapisujemy małymi literami
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToLowerInvariant());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToLowerInvariant());

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName()!.ToLowerInvariant());

            foreach (var foreignKey in entity.GetForeignKeys())
                foreignKey.SetConstraintName(foreignKey.GetConstraintName()!.ToLowerInvariant());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName()!.ToLowerInvariant());
        }
    }
}

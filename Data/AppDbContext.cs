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

        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.MuscleGroup)
            .WithMany(m => m.Exercises)
            .HasForeignKey(e => e.MuscleGroupId);

        modelBuilder.Entity<Exercise>()
            .HasMany(e => e.SecondaryMuscleGroups)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "ExerciseSecondaryMuscleGroups",
                j => j.HasOne<MuscleGroup>().WithMany().HasForeignKey("MuscleGroupId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Exercise>().WithMany().HasForeignKey("ExerciseId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("ExerciseId", "MuscleGroupId");
                    j.HasData(
                        new { ExerciseId = 1, MuscleGroupId = 4 },
                        new { ExerciseId = 1, MuscleGroupId = 6 },
                        new { ExerciseId = 2, MuscleGroupId = 7 },
                        new { ExerciseId = 2, MuscleGroupId = 10 },
                        new { ExerciseId = 2, MuscleGroupId = 11 },
                        new { ExerciseId = 3, MuscleGroupId = 10 },
                        new { ExerciseId = 3, MuscleGroupId = 11 },
                        new { ExerciseId = 3, MuscleGroupId = 13 },
                        new { ExerciseId = 4, MuscleGroupId = 5 },
                        new { ExerciseId = 4, MuscleGroupId = 13 },
                        new { ExerciseId = 5, MuscleGroupId = 6 },
                        new { ExerciseId = 5, MuscleGroupId = 7 },
                        new { ExerciseId = 6, MuscleGroupId = 13 },
                        new { ExerciseId = 8, MuscleGroupId = 5 },
                        new { ExerciseId = 8, MuscleGroupId = 13 },
                        new { ExerciseId = 9, MuscleGroupId = 10 },
                        new { ExerciseId = 9, MuscleGroupId = 11 },
                        new { ExerciseId = 10, MuscleGroupId = 4 },
                        new { ExerciseId = 10, MuscleGroupId = 11 }
                    );
                });

        modelBuilder.Entity<MuscleGroup>().HasData(
            new MuscleGroup { Id = 1, Name = "Klatka piersiowa" },
            new MuscleGroup { Id = 3, Name = "Plecy" },
            new MuscleGroup { Id = 4, Name = "Barki" },
            new MuscleGroup { Id = 5, Name = "Biceps" },
            new MuscleGroup { Id = 6, Name = "Triceps" },
            new MuscleGroup { Id = 7, Name = "Brzuch" },
            new MuscleGroup { Id = 8, Name = "Cardio" },
            new MuscleGroup { Id = 9, Name = "Czworogłowe uda" },
            new MuscleGroup { Id = 10, Name = "Dwugłowe uda" },
            new MuscleGroup { Id = 11, Name = "Biodra" },
            new MuscleGroup { Id = 12, Name = "Łydki" },
            new MuscleGroup { Id = 13, Name = "Przedramiona" }
        );

        modelBuilder.Entity<Exercise>().HasData(
            new Exercise
            {
                Id = 1,
                MuscleGroupId = 1,
                Name = "Wyciskanie sztangi na ławce poziomej",
                Description = "Połóż się na ławce, ściągnij łopatki i wypchnij sztangę nad klatkę, kontrolując ruch w dół.",
                EquipmentNeeded = "Sztanga, ławka"
            },
            new Exercise
            {
                Id = 2,
                MuscleGroupId = 9,
                Name = "Przysiady ze sztangą na plecach",
                Description = "Sztanga oparta na górnej części pleców. Zejdź do kąta prostego w kolanach i wstań, trzymając pięty na podłodze.",
                EquipmentNeeded = "Sztanga, stojaki"
            },
            new Exercise
            {
                Id = 3,
                MuscleGroupId = 3,
                Name = "Martwy ciąg klasyczny",
                Description = "Chwyt na szerokość barków, plecy proste. Unieś sztangę, prostując biodra i kolana jednocześnie.",
                EquipmentNeeded = "Sztanga"
            },
            new Exercise
            {
                Id = 4,
                MuscleGroupId = 3,
                Name = "Podciąganie na drążku nachwytem",
                Description = "Zwis na drążku, przyciągnij klatkę do drążka, ściągając łopatki. Opuszczaj się do pełnego zwisu.",
                EquipmentNeeded = "Drążek"
            },
            new Exercise
            {
                Id = 5,
                MuscleGroupId = 4,
                Name = "Wyciskanie żołnierskie",
                Description = "Stojąc, wypchnij sztangę znad obojczyków nad głowę i opuść ją kontrolowanie do brody.",
                EquipmentNeeded = "Sztanga"
            },
            new Exercise
            {
                Id = 6,
                MuscleGroupId = 5,
                Name = "Uginanie przedramion z hantlami",
                Description = "Łokcie przy tułowiu, ugnij przedramiona i powoli opuść hantle. Nie bujaj ciałem.",
                EquipmentNeeded = "Hantle"
            },
            new Exercise
            {
                Id = 7,
                MuscleGroupId = 6,
                Name = "Wyciskanie francuskie",
                Description = "Leżąc na ławce, opuść sztangę łamaną do czoła, zginając tylko łokcie, a następnie wyprostuj ramiona.",
                EquipmentNeeded = "Sztanga łamana, ławka"
            },
            new Exercise
            {
                Id = 8,
                MuscleGroupId = 3,
                Name = "Wiosłowanie sztangą w opadzie",
                Description = "Tułów prawie równolegle do podłogi, przyciągnij sztangę do brzucha i opuść ją bez zaokrąglania pleców.",
                EquipmentNeeded = "Sztanga"
            },
            new Exercise
            {
                Id = 9,
                MuscleGroupId = 9,
                Name = "Przysiad bułgarski",
                Description = "Tylna stopa na ławce, zejdź w dół na nodze wykrocznej i wróć do pozycji startowej.",
                EquipmentNeeded = "Hantle, ławka"
            },
            new Exercise
            {
                Id = 10,
                MuscleGroupId = 7,
                Name = "Deska",
                Description = "Oprzyj się na przedramionach, ciało w jednej linii. Napnij brzuch i utrzymuj pozycję bez opadania bioder.",
                EquipmentNeeded = "Mata"
            }
        );

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

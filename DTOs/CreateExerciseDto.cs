namespace FitForge.Api.DTOs;

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? EquipmentNeeded { get; set; }
    public int MuscleGroupId { get; set; }
    public List<int> SecondaryMuscleGroupIds { get; set; } = new();
}

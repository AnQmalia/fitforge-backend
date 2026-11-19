namespace FitForge.Api.DTOs;

public class CreateWorkoutPlanDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<WorkoutPlanItemDto> Items { get; set; } = new();
}

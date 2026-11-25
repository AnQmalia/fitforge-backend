namespace FitForge.Api.DTOs;

public class WorkoutPlanItemDto
{
    public int ExerciseId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int TargetReps { get; set; }
    public decimal Weight { get; set; }
    public int SetNumber { get; set; }
    public int ExercisesOrder { get; set; }
}
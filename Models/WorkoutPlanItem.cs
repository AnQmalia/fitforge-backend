namespace FitForge.Api.Models;

public class WorkoutPlanItem
{
    public int Id { get; set; }
    
    public int WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;
    
    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
    
    public string DayOfWeek { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int TargetReps { get; set; }
    public int ExerciseOrder {get; set;}
}
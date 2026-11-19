namespace FitForge.Api.Models;

public class WorkoutPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive {get; set;} = true;
    
    public ICollection<WorkoutPlanItem> WorkoutPlanItems { get; set; } = new List<WorkoutPlanItem>();
}
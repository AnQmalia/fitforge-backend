namespace FitForge.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FitnessGoal { get; set; }  = string.Empty;
    public string ExperienceLevel { get; set; }   = string.Empty;
    public string EquipmentAvailable { get; set; }  = string.Empty;
    
    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
}
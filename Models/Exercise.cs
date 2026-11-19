namespace FitForge.Api.Models;

public class Exercise
{
    public int Id { get; set; }
    public int MuscleGroupId { get; set; }
    public MuscleGroup? MuscleGroup { get; set; }
    
    public string Name { get; set; }  = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string EquipmentNeeded { get; set; }  = string.Empty;
    
    public ICollection<WorkoutPlanItem> WorkoutPlanItems { get; set; } = new List<WorkoutPlanItem>();
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitForge.Api.DTOs;
using FitForge.Api.Data;
using FitForge.Api.Models;
using Npgsql;
using System.Data;

namespace FitForge.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WorkoutPlansController: ControllerBase
{
    private readonly AppDbContext _context;

    public WorkoutPlansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkoutPlans()
    {
        var plans = await _context.WorkoutPlans
            .AsNoTracking()
            .Include(plan => plan.WorkoutPlanItems)
            .ThenInclude(item => item.Exercise)
            .OrderByDescending(plan => plan.CreatedAt)
            .ToListAsync();

        return Ok(plans.Select(plan => new
        {
            plan.Id,
            plan.Name,
            plan.CreatedAt,
            plan.IsActive,
            Items = plan.WorkoutPlanItems
                .OrderBy(item => item.ExerciseOrder)
                .ThenBy(item => item.SetNumber)
                .Select(item => new
                {
                    item.Id,
                    item.ExerciseId,
                    ExerciseName = item.Exercise?.Name ?? "Usunięte ćwiczenie",
                    item.DayOfWeek,
                    item.Sets,
                    item.TargetReps,
                    item.Weight,
                    item.SetNumber,
                    item.ExerciseOrder
                })
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkoutPlan([FromBody] CreateWorkoutPlanDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Items.Count == 0)
            return BadRequest(new { Message = "Plan musi mieć nazwę i co najmniej jedno ćwiczenie." });

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            int generatedPlanId = 0;
            var userId = await ResolveUserIdAsync(dto.UserId);

            var userIdParam = new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer)
                { Value = userId };
            var nameParam = new NpgsqlParameter("p_name", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dto.Name };
            var planIdParam = new NpgsqlParameter("p_plan_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.InputOutput, Value = DBNull.Value
            };

            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_insert_workout_plan(@p_user_id, @p_name, @p_plan_id)",
                userIdParam, nameParam, planIdParam);

            generatedPlanId = Convert.ToInt32(planIdParam.Value);

            foreach (var itemDto in dto.Items)
            {
                var item = new WorkoutPlanItem
                {
                    WorkoutPlanId = generatedPlanId,
                    ExerciseId = itemDto.ExerciseId,
                    DayOfWeek = string.IsNullOrWhiteSpace(itemDto.DayOfWeek) ? "" : itemDto.DayOfWeek,
                    Sets = 1,
                    TargetReps = itemDto.TargetReps,
                    Weight = itemDto.Weight,
                    SetNumber = itemDto.SetNumber < 1 ? 1 : itemDto.SetNumber,
                    ExerciseOrder = itemDto.ExercisesOrder
                };

                _context.WorkoutPlanItems.Add(item);
            }

            await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { Message = "Plan treningowy został pomyślnie zapisany", PlanId = generatedPlanId });
                
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Error", error = ex.Message });
            }
    }

    private async Task<int> ResolveUserIdAsync(int userId)
    {
        if (userId > 0 && await _context.Users.AnyAsync(user => user.Id == userId))
            return userId;

        var anonymous = await _context.Users.FirstOrDefaultAsync(user => user.Username == "anonim");
        if (anonymous != null)
            return anonymous.Id;

        anonymous = new User
        {
            Username = "anonim",
            FitnessGoal = "Niepodany",
            ExperienceLevel = "Niepodany",
            EquipmentAvailable = "Niepodany"
        };
        _context.Users.Add(anonymous);
        await _context.SaveChangesAsync();
        return anonymous.Id;
    }
}
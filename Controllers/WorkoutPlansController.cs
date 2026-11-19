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

    [HttpPost]
    public async Task<IActionResult> CreateWorkoutPlan([FromBody] CreateWorkoutPlanDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            int generatedPlanId = 0;

            var userIdParam = new NpgsqlParameter("p_user_id", NpgsqlTypes.NpgsqlDbType.Integer)
                { Value = dto.UserId };
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
                    DayOfWeek = itemDto.DayOfWeek,
                    Sets = itemDto.Sets,
                    TargetReps = itemDto.TargetReps,
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
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitForge.Api.Data;
using FitForge.Api.DTOs;
using FitForge.Api.Models;

namespace FitForge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExercisesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExercisesController(AppDbContext context)
    {
        _context= context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
    {
        var exercises = await _context.Exercises.Include(e => e.MuscleGroup).ToListAsync();
        return Ok(exercises);
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> CreateExercise([FromBody] CreateExerciseDto dto)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Podaj nazwę ćwiczenia.");

        var groupExists = await _context.MuscleGroups.AnyAsync(g => g.Id == dto.MuscleGroupId);
        if (!groupExists)
            return BadRequest("Wybierz partię mięśniową.");

        var exercise = new Exercise
        {
            Name = name,
            Description = dto.Description?.Trim() ?? string.Empty,
            EquipmentNeeded = dto.EquipmentNeeded?.Trim() ?? string.Empty,
            MuscleGroupId = dto.MuscleGroupId
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();
        await _context.Entry(exercise).Reference(e => e.MuscleGroup).LoadAsync();

        return CreatedAtAction(nameof(GetExercises), new { id = exercise.Id }, exercise);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);
        if (exercise == null)
            return NotFound();

        var usedInPlan = await _context.WorkoutPlanItems.AnyAsync(item => item.ExerciseId == id);
        if (usedInPlan)
        {
            return new ContentResult
            {
                StatusCode = StatusCodes.Status409Conflict,
                Content = "To ćwiczenie jest w zapisanym planie.",
                ContentType = "text/plain; charset=utf-8"
            };
        }

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}
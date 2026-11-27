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
        var exercises = await _context.Exercises
            .AsNoTracking()
            .Include(e => e.MuscleGroup)
            .Include(e => e.SecondaryMuscleGroups)
            .OrderBy(e => e.Id)
            .ToListAsync();
        return Ok(exercises);
    }

    [HttpPost]
    public async Task<ActionResult<Exercise>> CreateExercise([FromBody] CreateExerciseDto dto)
    {
        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Podaj nazwę ćwiczenia.");

        var group = await _context.MuscleGroups.FindAsync(dto.MuscleGroupId);
        if (group == null)
            return BadRequest("Wybierz partię mięśniową.");

        var secondaryIds = (dto.SecondaryMuscleGroupIds ?? new List<int>())
            .Where(gid => gid != group.Id)
            .Distinct()
            .ToList();

        var secondaryGroups = secondaryIds.Count > 0
            ? await _context.MuscleGroups.Where(g => secondaryIds.Contains(g.Id)).ToListAsync()
            : new List<MuscleGroup>();

        var exercise = new Exercise
        {
            Name = name,
            Description = dto.Description?.Trim() ?? string.Empty,
            EquipmentNeeded = dto.EquipmentNeeded?.Trim() ?? string.Empty,
            MuscleGroupId = group.Id,
            MuscleGroup = group,
            SecondaryMuscleGroups = secondaryGroups
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExercises), new { id = exercise.Id }, exercise);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Exercise>> UpdateExercise(int id, [FromBody] CreateExerciseDto dto)
    {
        var exercise = await _context.Exercises
            .Include(e => e.SecondaryMuscleGroups)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (exercise == null)
            return NotFound();

        var name = dto.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Podaj nazwę ćwiczenia.");

        var group = await _context.MuscleGroups.FindAsync(dto.MuscleGroupId);
        if (group == null)
            return BadRequest("Wybierz partię mięśniową.");

        var secondaryIds = (dto.SecondaryMuscleGroupIds ?? new List<int>())
            .Where(gid => gid != group.Id)
            .Distinct()
            .ToList();

        var secondaryGroups = secondaryIds.Count > 0
            ? await _context.MuscleGroups.Where(g => secondaryIds.Contains(g.Id)).ToListAsync()
            : new List<MuscleGroup>();

        exercise.Name = name;
        exercise.Description = dto.Description?.Trim() ?? string.Empty;
        exercise.EquipmentNeeded = dto.EquipmentNeeded?.Trim() ?? string.Empty;
        exercise.MuscleGroupId = group.Id;
        exercise.MuscleGroup = group;

        exercise.SecondaryMuscleGroups.Clear();
        foreach (var secondary in secondaryGroups)
            exercise.SecondaryMuscleGroups.Add(secondary);

        await _context.SaveChangesAsync();

        return Ok(exercise);
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
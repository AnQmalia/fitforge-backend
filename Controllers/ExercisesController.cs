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

}
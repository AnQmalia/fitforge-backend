using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitForge.Api.Data;

namespace FitForge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MuscleGroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MuscleGroupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> GetMuscleGroups()
    {
        var groups = await _context.MuscleGroups
            .OrderBy(g => g.Name)
            .Select(g => new { g.Id, g.Name })
            .ToListAsync();

        return Ok(groups);
    }
}

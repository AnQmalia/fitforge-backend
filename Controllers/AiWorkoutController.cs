using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitForge.Api.Data;
using FitForge.Api.DTOs;
using FitForge.Api.Models;
using FitForge.Api.Services;

namespace FitForge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiWorkoutController : ControllerBase
{
    private readonly OpenAiService _openAiService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AiWorkoutController(
        OpenAiService openAiService,
        AppDbContext context,
        IConfiguration configuration)
    {
        _openAiService = openAiService;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GeneratePlan([FromBody] AiPromptDto dto)
    {
        var goal = dto.FitnessGoal.Trim();
        var level = dto.ExperienceLevel.Trim();
        var equipment = dto.EquipmentAvailable.Trim();

        if (string.IsNullOrWhiteSpace(goal) ||
            string.IsNullOrWhiteSpace(level) ||
            string.IsNullOrWhiteSpace(equipment))
        {
            return BadRequest(new { Message = "Uzupełnij cel, poziom doświadczenia i dostępny sprzęt." });
        }

        try
        {
            var hasApiKey = !string.IsNullOrWhiteSpace(_configuration["OpenAi:ApiKey"]);
            if (hasApiKey)
            {
                var generatedPlan = await _openAiService.GenerateWorkoutPlanTextAsync(
                    goal,
                    level,
                    equipment);

                return Ok(new
                {
                    Message = "Git",
                    PlanContent = generatedPlan,
                    Source = "openai"
                });
            }

            var exercises = await _context.Exercises
                .AsNoTracking()
                .Include(e => e.MuscleGroup)
                .OrderBy(e => e.Id)
                .ToListAsync();

            var localPlan = BuildLocalPlan(exercises, goal, level, equipment);

            return Ok(new
            {
                Message = "Git",
                PlanContent = localPlan,
                Source = "local"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error", Error = ex.Message });
        }
    }

    private static string BuildLocalPlan(
        List<Exercise> allExercises,
        string goal,
        string level,
        string equipment)
    {
        var (setsReps, restNote) = goal switch
        {
            "Zwiększyć siłę" => ("4 serie × 4–6 powtórzeń", "Przerwa 2–3 min między seriami."),
            "Zbudować masę mięśniową" => ("4 serie × 8–12 powtórzeń", "Przerwa 90 s między seriami."),
            "Schudnąć" => ("3 serie × 12–15 powtórzeń", "Przerwa 45–60 s, utrzymuj sprawne tempo."),
            "Poprawić kondycję" => ("3 serie × 15–20 powtórzeń", "Przerwa 30–45 s między seriami."),
            _ => ("3 serie × 10–12 powtórzeń", "Przerwa 60–90 s między seriami.")
        };

        var pool = FilterByEquipment(allExercises, equipment);
        var sb = new StringBuilder();
        sb.AppendLine($"Cel: {goal}");
        sb.AppendLine($"Poziom: {level}");
        sb.AppendLine($"Sprzęt: {equipment}");
        sb.AppendLine();

        var days = level switch
        {
            "Początkujący" => BuildFullBodyDays(pool),
            "Zaawansowany" => BuildPushPullLegsDays(pool),
            _ => BuildUpperLowerDays(pool)
        };

        foreach (var (dayTitle, dayExercises) in days)
        {
            sb.AppendLine(dayTitle);
            foreach (var name in dayExercises)
            {
                var prescription = name.Contains("Deska", StringComparison.OrdinalIgnoreCase)
                    ? "3 serie × 45–60 s"
                    : setsReps;
                sb.AppendLine($"- {name} — {prescription}");
            }
            sb.AppendLine();
        }

        sb.Append($"Wskazówka: {restNote} Zadbaj o 5–10 min rozgrzewki przed każdym treningiem.");
        return sb.ToString();
    }

    private static List<Exercise> FilterByEquipment(List<Exercise> exercises, string equipment)
    {
        var normalized = equipment.ToLowerInvariant();
        if (normalized.Contains("pełna siłownia") || normalized.Contains("siłownia"))
            return exercises;

        if (normalized.Contains("brak sprzętu") || normalized.Contains("w domu"))
        {
            var matched = exercises
                .Where(e => string.IsNullOrWhiteSpace(e.EquipmentNeeded) ||
                            e.EquipmentNeeded.Contains("mata", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var bodyweight = new List<Exercise>(matched);
            AddFallbackIfMissing(bodyweight, "Pompki klasyczne", "Klatka piersiowa");
            AddFallbackIfMissing(bodyweight, "Przysiady bez obciążenia", "Czworogłowe uda");
            AddFallbackIfMissing(bodyweight, "Wykroki naprzemienne", "Dwugłowe uda");
            AddFallbackIfMissing(bodyweight, "Pompki w podporze tyłem (dipy na krześle)", "Triceps");
            AddFallbackIfMissing(bodyweight, "Superman (unoszenie tułowia w leżeniu)", "Plecy");
            AddFallbackIfMissing(bodyweight, "Deska", "Brzuch");
            return bodyweight;
        }

        var keywords = new List<string> { "mata" };
        if (normalized.Contains("hantl")) keywords.Add("hantl");
        if (normalized.Contains("ławk")) keywords.Add("ławk");
        if (normalized.Contains("sztang")) keywords.Add("sztang");
        if (normalized.Contains("drąż")) keywords.Add("drąż");

        var filtered = exercises
            .Where(e => string.IsNullOrWhiteSpace(e.EquipmentNeeded) ||
                        keywords.Any(k => e.EquipmentNeeded.Contains(k, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        return filtered.Count >= 4 ? filtered : exercises;
    }

    private static void AddFallbackIfMissing(List<Exercise> list, string name, string groupName)
    {
        if (list.Any(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return;

        list.Add(new Exercise
        {
            Name = name,
            MuscleGroup = new MuscleGroup { Name = groupName }
        });
    }

    private static List<(string Title, List<string> Exercises)> BuildFullBodyDays(List<Exercise> pool)
    {
        var dayA = PickByGroups(pool, new[] { "Czworogłowe uda", "Klatka piersiowa", "Plecy", "Brzuch" }, 0);
        var dayB = PickByGroups(pool, new[] { "Czworogłowe uda", "Barki", "Plecy", "Biceps", "Triceps" }, 1);
        var dayC = PickByGroups(pool, new[] { "Dwugłowe uda", "Klatka piersiowa", "Plecy", "Brzuch" }, 2);

        return new List<(string, List<string>)>
        {
            ("Poniedziałek — Trening całego ciała (FBW A)", dayA),
            ("Środa — Trening całego ciała (FBW B)", dayB),
            ("Piątek — Trening całego ciała (FBW C)", dayC)
        };
    }

    private static List<(string Title, List<string> Exercises)> BuildUpperLowerDays(List<Exercise> pool)
    {
        var upperPool = pool.Where(e => !e.Name.Contains("Martwy ciąg", StringComparison.OrdinalIgnoreCase)).ToList();
        var lowerPool = pool
            .Where(e => IsLowerOrCore(e) || e.Name.Contains("Martwy ciąg", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var upperA = PickByGroups(upperPool, new[] { "Klatka piersiowa", "Plecy", "Barki", "Biceps" }, 0, upperOnly: true);
        var lowerA = PickByGroups(lowerPool, new[] { "Czworogłowe uda", "Dwugłowe uda", "Plecy", "Łydki", "Brzuch" }, 0, upperOnly: false);
        var upperB = PickByGroups(upperPool, new[] { "Plecy", "Klatka piersiowa", "Barki", "Triceps" }, 1, upperOnly: true);
        var lowerB = PickByGroups(lowerPool, new[] { "Czworogłowe uda", "Czworogłowe uda", "Biodra", "Brzuch" }, 1, upperOnly: false);

        return new List<(string, List<string>)>
        {
            ("Poniedziałek — Góra ciała A", upperA),
            ("Wtorek — Dół ciała + Brzuch A", lowerA),
            ("Czwartek — Góra ciała B", upperB),
            ("Piątek — Dół ciała + Brzuch B", lowerB)
        };
    }

    private static List<(string Title, List<string> Exercises)> BuildPushPullLegsDays(List<Exercise> pool)
    {
        var upperPool = pool.Where(e => !e.Name.Contains("Martwy ciąg", StringComparison.OrdinalIgnoreCase)).ToList();
        var lowerPool = pool
            .Where(e => IsLowerOrCore(e) || e.Name.Contains("Martwy ciąg", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var push = PickByGroups(upperPool, new[] { "Klatka piersiowa", "Barki", "Triceps", "Brzuch" }, 0, upperOnly: true);
        var pull = PickByGroups(pool, new[] { "Plecy", "Plecy", "Biceps", "Przedramiona" }, 0, upperOnly: true);
        var legs = PickByGroups(lowerPool, new[] { "Czworogłowe uda", "Czworogłowe uda", "Dwugłowe uda", "Łydki", "Brzuch" }, 0, upperOnly: false);
        var upper = PickByGroups(upperPool, new[] { "Klatka piersiowa", "Plecy", "Barki", "Biceps", "Triceps" }, 1, upperOnly: true);

        return new List<(string, List<string>)>
        {
            ("Poniedziałek — Push (Klatka, Barki, Triceps)", push),
            ("Wtorek — Pull (Plecy, Biceps)", pull),
            ("Czwartek — Nogi + Brzuch", legs),
            ("Piątek — Góra ciała (uzupełniający)", upper)
        };
    }

    private static bool IsLowerOrCore(Exercise exercise)
    {
        var group = exercise.MuscleGroup?.Name ?? string.Empty;
        return group is "Czworogłowe uda" or "Dwugłowe uda" or "Biodra" or "Łydki" or "Brzuch" or "Cardio";
    }

    private static List<string> PickByGroups(
        List<Exercise> pool,
        string[] desiredGroups,
        int offset,
        bool? upperOnly = null)
    {
        var chosen = new List<string>();
        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var group in desiredGroups)
        {
            var candidates = pool
                .Where(e => string.Equals(e.MuscleGroup?.Name, group, StringComparison.OrdinalIgnoreCase) &&
                            !usedNames.Contains(e.Name))
                .ToList();

            if (candidates.Count > 0)
            {
                var pick = candidates[offset % candidates.Count];
                chosen.Add(pick.Name);
                usedNames.Add(pick.Name);
            }
        }

        if (upperOnly is null)
        {
            foreach (var fallback in pool)
            {
                if (chosen.Count >= 4) break;
                if (usedNames.Add(fallback.Name))
                    chosen.Add(fallback.Name);
            }
        }

        return chosen;
    }
}

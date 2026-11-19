using Microsoft.AspNetCore.Mvc;
using FitForge.Api.DTOs;
using FitForge.Api.Services;

namespace FitForge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AiWorkoutController: ControllerBase
{
    private readonly OpenAiService _openAiService;

    public AiWorkoutController(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [HttpPost("generate")]
        public async Task<IActionResult> GeneratePlan([FromBody] AiPromptDto dto){
            try
            {
                var generatedPlan = await _openAiService.GenerateWorkoutPlanTextAsync(
                    dto.FitnessGoal,
                    dto.ExperienceLevel,
                    dto.EquipmentAvailable);

                return Ok(new
                {
                    Message = "Git",
                    PlanContent = generatedPlan
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{Message = "Error", Error = ex.Message});
            }
    }
}
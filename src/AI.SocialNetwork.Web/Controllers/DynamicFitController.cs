using AI.SocialNetwork.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

/// <summary>
/// Динамическая оценка пригодности кадра (DFS) — отбор по площади потерь эффективности.
/// Модель: диссертация, гл. 2, рис. 1–5, с. 64–68.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DynamicFitController : ControllerBase
{
    private readonly IDynamicFitService _dynamicFit;

    public DynamicFitController(IDynamicFitService dynamicFit)
    {
        _dynamicFit = dynamicFit;
    }

    /// <summary>Переходный процесс и потери одного кандидата.</summary>
    [HttpPost("evaluate")]
    public ActionResult<DynamicFitResult> Evaluate([FromBody] DynamicFitInput input)
    {
        return Ok(_dynamicFit.Evaluate(input));
    }

    /// <summary>Ранжирование кандидатов на заданном горизонте планирования.</summary>
    [HttpPost("rank")]
    public ActionResult<IReadOnlyList<RankedCandidate>> Rank(
        [FromBody] IEnumerable<CandidateProfile> candidates,
        [FromQuery] double horizonMonths = 36)
    {
        return Ok(_dynamicFit.Rank(candidates, horizonMonths));
    }

    /// <summary>Денежная стоимость ротации с учётом разрыва первого рода.</summary>
    [HttpPost("rotation-cost")]
    public ActionResult<decimal> RotationCost(
        [FromBody] DynamicFitInput input,
        [FromQuery] decimal vacancyMonths = 0m,
        [FromQuery] decimal actingEfficiency = 0.3m,
        [FromQuery] decimal monthlyValue = 0m)
    {
        return Ok(_dynamicFit.RotationCost(input, vacancyMonths, actingEfficiency, monthlyValue));
    }
}

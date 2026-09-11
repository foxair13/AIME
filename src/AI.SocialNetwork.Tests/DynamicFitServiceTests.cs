using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Application.Services;

namespace AI.SocialNetwork.Tests;

/// <summary>
/// Проверка динамической модели отбора (DFS) против графиков диссертации (рис. 4–5, с. 67).
/// </summary>
public class DynamicFitServiceTests
{
    private readonly DynamicFitService _dfs = new();

    [Fact]
    public void SteadyState_ReachesRequiredLevel_DueToIntegralAction()
    {
        // Интегральная составляющая (Kкомпет/p) устраняет статическую ошибку.
        var r = _dfs.Evaluate(new DynamicFitInput(Competence: 0.9, Motivation: 3.0, HorizonMonths: 99));
        Assert.InRange(r.SteadyStateLevel, 0.98, 1.02);
    }

    [Theory]
    // Рис. 4 (с. 67): при росте Tмотив перерегулирование монотонно падает.
    [InlineData(1.0, 3.0)]
    [InlineData(3.0, 5.0)]
    [InlineData(5.0, 10.0)]
    public void HigherMotivation_ReducesOvershoot(double lowMotivation, double highMotivation)
    {
        var low = _dfs.Evaluate(new DynamicFitInput(0.9, lowMotivation, HorizonMonths: 99));
        var high = _dfs.Evaluate(new DynamicFitInput(0.9, highMotivation, HorizonMonths: 99));
        Assert.True(high.OvershootPercent < low.OvershootPercent,
            $"Tм={highMotivation} дал перерегулирование {high.OvershootPercent}%, что не меньше {low.OvershootPercent}% при Tм={lowMotivation}");
    }

    [Theory]
    // Рис. 5 (с. 67): при падении Kкомпет время регулирования растёт.
    [InlineData(0.9, 0.4)]
    [InlineData(0.4, 0.1)]
    public void LowerCompetence_IncreasesTimeToCompetence(double highCompetence, double lowCompetence)
    {
        var high = _dfs.Evaluate(new DynamicFitInput(highCompetence, 3.0, HorizonMonths: 99));
        var low = _dfs.Evaluate(new DynamicFitInput(lowCompetence, 3.0, HorizonMonths: 99));
        Assert.True(low.TimeToCompetenceMonths > high.TimeToCompetenceMonths,
            $"Kк={lowCompetence}: t={low.TimeToCompetenceMonths}, Kк={highCompetence}: t={high.TimeToCompetenceMonths}");
    }

    [Fact]
    public void Ranking_PrefersFastAdapter_OverHigherRawCompetence()
    {
        // Главный эффект метода: статический балл ставит «звезду» первой,
        // а по площади потерь она проигрывает мотивированному середняку.
        var candidates = new[]
        {
            new CandidateProfile(1, "Звезда, инертный", new DynamicFitInput(0.90, 1.0)),
            new CandidateProfile(2, "Середняк, мотивированный", new DynamicFitInput(0.55, 6.0)),
        };

        var ranked = _dfs.Rank(candidates, horizonMonths: 36);

        Assert.Equal(2, ranked[0].CandidateId);
        Assert.True(ranked[0].Result.LossArea < ranked[1].Result.LossArea);
    }

    [Fact]
    public void Ranking_IsStableAndOrdered()
    {
        var candidates = new[]
        {
            new CandidateProfile(1, "A", new DynamicFitInput(0.90, 1.0)),
            new CandidateProfile(2, "B", new DynamicFitInput(0.55, 6.0)),
            new CandidateProfile(3, "C", new DynamicFitInput(0.25, 12.0)),
        };

        var ranked = _dfs.Rank(candidates, 36);

        Assert.Equal(3, ranked.Count);
        Assert.Equal([1, 2, 3], ranked.Select(r => r.Rank).ToArray());
        for (int i = 1; i < ranked.Count; i++)
        {
            Assert.True(ranked[i - 1].Result.LossArea <= ranked[i].Result.LossArea);
        }
    }

    [Fact]
    public void ShortHorizon_FavoursFastOnboarding()
    {
        // На горизонте 12 месяцев быстрый старт важнее потолка компетентности.
        var slowStarter = new DynamicFitInput(0.90, 1.0);
        var fastStarter = new DynamicFitInput(0.25, 12.0);

        var slow = _dfs.Evaluate(slowStarter with { HorizonMonths = 12 });
        var fast = _dfs.Evaluate(fastStarter with { HorizonMonths = 12 });

        Assert.True(fast.LossArea < slow.LossArea);
        Assert.True(fast.TimeToCompetenceMonths < slow.TimeToCompetenceMonths);
    }

    [Fact]
    public void RotationCost_GrowsWithVacancyLength()
    {
        var input = new DynamicFitInput(0.7, 3.0);
        var shortGap = _dfs.RotationCost(input, vacancyMonths: 1m, actingEfficiency: 0.3m, monthlyValue: 10_000m);
        var longGap = _dfs.RotationCost(input, vacancyMonths: 6m, actingEfficiency: 0.3m, monthlyValue: 10_000m);

        Assert.True(longGap > shortGap);
        Assert.True(shortGap > 0m);
    }

    [Fact]
    public void Evaluate_ClampsDegenerateInput_AndDoesNotThrow()
    {
        var r = _dfs.Evaluate(new DynamicFitInput(Competence: 0, Motivation: 0, HorizonMonths: 0));
        Assert.True(r.LossArea >= 0);
        Assert.NotEmpty(r.Trajectory);
    }
}

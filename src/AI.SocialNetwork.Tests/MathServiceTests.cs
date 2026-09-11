using AI.SocialNetwork.Application.Services;

namespace AI.SocialNetwork.Tests;

public class MathServiceTests
{
    private readonly MathService _math = new();

    [Fact]
    public void Normalize_ScalersToUnitRange()
    {
        Assert.Equal(0m, _math.Normalize(0m, 0m, 10m));
        Assert.Equal(0.5m, _math.Normalize(5m, 0m, 10m));
        Assert.Equal(1m, _math.Normalize(10m, 0m, 10m));
    }

    [Fact]
    public void Normalize_EqualMaxMin_ReturnsOne()
    {
        Assert.Equal(1m, _math.Normalize(5m, 5m, 5m));
    }

    [Fact]
    public void RoleFit_EqualWeights_ReturnsWeightedSumOfNormalizedScores()
    {
        // (3.1): RoleFit = Σ(Vi·xi) = 1·0.5 + 1·1.0 = 1.5
        decimal[] weights = [1m, 1m];
        decimal[] scores = [5m, 10m];
        decimal[] maxScores = [10m, 10m];
        Assert.Equal(1.5m, _math.RoleFit(weights, scores, maxScores));
    }

    [Fact]
    public void RoleFit_ZeroSkill_ReturnsContributionOfOtherWeightOnly()
    {
        decimal[] weights = [0.8m, 0.2m];
        decimal[] scores = [0m, 10m];
        decimal[] maxScores = [10m, 10m];
        Assert.Equal(0.2m, _math.RoleFit(weights, scores, maxScores));
    }

    [Fact]
    public void RoleFit_MismatchedLengths_ReturnsZero()
    {
        Assert.Equal(0m, _math.RoleFit([1m], [1m, 1m], [10m]));
    }

    [Fact]
    public void Normalize_ClampsOutOfRangeValues()
    {
        // Была живая ошибка: Normalize(10,0,5) возвращал 2.0 и ломал (3.1).
        Assert.Equal(1m, _math.Normalize(10m, 0m, 5m));
        Assert.Equal(0m, _math.Normalize(-3m, 0m, 5m));
    }

    [Fact]
    public void NormalizeDescending_InvertsScale()
    {
        Assert.Equal(1m, _math.NormalizeDescending(0m, 0m, 10m));
        Assert.Equal(0.5m, _math.NormalizeDescending(5m, 0m, 10m));
        Assert.Equal(0m, _math.NormalizeDescending(10m, 0m, 10m));
    }

    [Fact]
    public void RoleFit_NormalizesWeights_SoResultStaysInUnitRange()
    {
        // Веса не образуют полную группу — результат всё равно должен быть в [0..1].
        decimal[] weights = [2m, 2m];
        decimal[] scores = [10m, 10m];
        decimal[] maxScores = [10m, 10m];
        Assert.Equal(1m, _math.RoleFit(weights, scores, maxScores));
    }

    [Fact]
    public void LearningCurve_GrowsTowardCeiling()
    {
        var day0 = _math.LearningCurve(10m, 2m, 0.05m, 0m);
        var day30 = _math.LearningCurve(10m, 2m, 0.05m, 30m);
        var day365 = _math.LearningCurve(10m, 2m, 0.05m, 365m);

        Assert.Equal(2m, day0);
        Assert.True(day30 > day0);
        Assert.True(day365 > day30);
        Assert.InRange(day365, 9.9m, 10m);
    }

    [Fact]
    public void DaysToReach_IsInverseOfLearningCurve()
    {
        var days = _math.DaysToReach(10m, 2m, 6m, 0.05m);
        var achieved = _math.LearningCurve(10m, 2m, 0.05m, days);
        Assert.InRange(achieved, 5.95m, 6.05m);
    }

    [Fact]
    public void DecayedLevel_FallsWithoutConfirmation()
    {
        var baseTime = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);

        Assert.Equal(8m, _math.DecayedLevel(8m, baseTime, 0.05m, baseTime));

        var after30 = _math.DecayedLevel(8m, baseTime, 0.05m, baseTime.AddDays(30));
        var after90 = _math.DecayedLevel(8m, baseTime, 0.05m, baseTime.AddDays(90));

        Assert.True(after30 < 8m, "уровень обязан падать без подтверждения");
        Assert.True(after90 < after30);
        Assert.True(after90 >= 0m);
    }

    [Fact]
    public void DecayedLevel_RespectsResidualFloor()
    {
        var baseTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var level = _math.DecayedLevel(8m, baseTime, 0.05m, baseTime.AddDays(3650), floorLevel: 3m);
        Assert.InRange(level, 3m, 3.01m);
    }

    [Fact]
    public void Haversine_SamePoint_ReturnsZero()
    {
        Assert.Equal(0.0, _math.HaversineDistance(55.7558, 37.6173, 55.7558, 37.6173), 5);
    }

    [Fact]
    public void Haversine_MoscowToPetersburg_Approx630Km()
    {
        var dist = _math.HaversineDistance(55.7558, 37.6173, 59.9343, 30.3351);
        Assert.InRange(dist, 600, 700);
    }

    [Fact]
    public void KMeans_SeparatesThreeDistinctGroups()
    {
        // Три чётких кластера (по 2 точки рядом)
        double[][] points =
        [
            [0.0, 0.0], [0.1, 0.1],
            [10.0, 10.0], [10.1, 10.0],
            [20.0, 20.0], [20.0, 20.1],
        ];
        var labels = _math.KMeansCluster(points, 3);
        Assert.Equal(6, labels.Length);
        Assert.Equal(labels[0], labels[1]);
        Assert.Equal(labels[2], labels[3]);
        Assert.Equal(labels[4], labels[5]);
        Assert.NotEqual(labels[0], labels[2]);
        Assert.NotEqual(labels[0], labels[4]);
        Assert.NotEqual(labels[2], labels[4]);
    }

    [Fact]
    public void KMeans_NoPoints_ReturnsEmpty()
    {
        Assert.Empty(_math.KMeansCluster([], 3));
    }

    [Fact]
    public void Knapsack_PicksMaxValueWithinCapacity()
    {
        // Элементы: вес/ценность 2/5, 3/8, 4/9, 5/10, вместимость 8
        double[] weights = [2, 3, 4, 5];
        double[] values = [5, 8, 9, 10];
        var result = _math.BranchAndBoundKnapsack(weights, values, 8.0);
        // Оптимально: {3,5} = 18 (вес 8)
        Assert.Equal(18.0, result.TotalValue, 3);
        Assert.Equal(8.0, result.TotalWeight, 3);
        Assert.Equal([1, 3], result.SelectedIndices);
    }

    [Fact]
    public void Knapsack_EmptyCapacity_ReturnsEmptySet()
    {
        var result = _math.BranchAndBoundKnapsack([1, 2], [5, 6], 0);
        Assert.Empty(result.SelectedIndices);
    }
}
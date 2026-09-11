using AI.SocialNetwork.Application.Contracts;

namespace AI.SocialNetwork.Application.Services;

/// <summary>
/// Динамическая оценка пригодности кадра (Dynamic Fit Score, DFS).
///
/// Основана на модели контура управления из диссертации (гл. 2, с. 64–68, рис. 1–5):
/// сотрудник рассматривается не как набор баллов, а как звено системы регулирования,
/// имеющее переходный процесс при вхождении в должность.
///
/// Структурная схема (рис. 3, с. 66):
///     e = r − y                       рассогласование «требуется — фактически»
///     u = Tm·e + Kk·∫e dt             мотивация (пропорц.) + компетентность (интегр.)
///     y1' = (u − y1)/Te               инерция экономики/среды
///     y2' = (y1 − y2)/Tob             инерция объекта управления (коллектива)
///     y   = Kob·y2                    фактическая эффективность деятельности
///
/// Ключевое отличие от всех известных методов отбора: оценивается не «уровень» кандидата,
/// а ПЛОЩАДЬ ПОТЕРЬ эффективности за горизонт планирования (заштрихованная область на рис. 1),
/// то есть интегральный недобор J = ∫ max(r − y(t), 0) dt.
/// Побеждает не самый компетентный, а тот, кто даёт наименьшие потери за нужный срок.
/// </summary>
public sealed class DynamicFitService : IDynamicFitService
{
    // Шаг интегрирования (месяцы). 0.01 даёт погрешность < 0.1 % при Te >= 0.5.
    private const double Step = 0.01;

    public DynamicFitResult Evaluate(DynamicFitInput input)
    {
        var horizon = input.HorizonMonths <= 0 ? 36.0 : input.HorizonMonths;
        var required = input.RequiredLevel <= 0 ? 1.0 : input.RequiredLevel;

        var competence = Clamp(input.Competence, 0.01, 1.0);   // Kкомпет: интегральный коэф.
        var motivation = Clamp(input.Motivation, 0.1, 20.0);   // Tмотив: пропорц. коэф.
        var envInertia = input.EnvironmentInertia <= 0 ? 0.5 : input.EnvironmentInertia;   // Tэ
        var objInertia = input.ObjectInertia <= 0 ? 15.0 : input.ObjectInertia;            // Тоб
        var gain = input.ObjectGain <= 0 ? 1.0 : input.ObjectGain;                         // Kоб

        int n = (int)(horizon / Step);
        double integral = 0.0, y1 = 0.0, y2 = 0.0;
        double loss = 0.0, peak = 0.0;
        double timeToCompetence = -1.0;
        var trajectory = new List<double>(Math.Min(n, 512));
        int sampleEvery = Math.Max(1, n / 512);

        for (int i = 0; i < n; i++)
        {
            double y = gain * y2;
            double e = required - y;

            integral += competence * e * Step;
            double u = motivation * e + integral;

            y1 += (u - y1) / envInertia * Step;
            y2 += (y1 - y2) / objInertia * Step;

            // Площадь недобора эффективности (рис. 1: «потери из-за некомпетентности»).
            if (e > 0)
            {
                loss += e * Step;
            }
            if (y > peak)
            {
                peak = y;
            }
            // Момент выхода на требуемый уровень (вхождение в должность).
            if (timeToCompetence < 0 && y >= 0.95 * required)
            {
                timeToCompetence = i * Step;
            }
            if (i % sampleEvery == 0)
            {
                trajectory.Add(Math.Round(y, 4));
            }
        }

        double steady = gain * y2;
        double overshoot = steady > 0 ? (peak - steady) / steady * 100.0 : 0.0;

        return new DynamicFitResult(
            LossArea: Math.Round(loss, 4),
            TimeToCompetenceMonths: timeToCompetence < 0 ? horizon : Math.Round(timeToCompetence, 2),
            SteadyStateLevel: Math.Round(steady, 4),
            OvershootPercent: Math.Round(overshoot, 2),
            Trajectory: trajectory);
    }

    /// <summary>
    /// Полная стоимость назначения с учётом разрыва первого рода при ротации (с. 68).
    /// Диссертация: наличие разрыва и этапа вхождения в должность снижает качество
    /// управления в переходном режиме на 10–30 %.
    /// </summary>
    public decimal RotationCost(DynamicFitInput input, decimal vacancyMonths, decimal actingEfficiency, decimal monthlyValue)
    {
        var fit = Evaluate(input);
        var required = input.RequiredLevel <= 0 ? 1.0m : (decimal)input.RequiredLevel;

        // Потери за время вакансии: и.о. держит лишь часть требуемого уровня.
        var gap = vacancyMonths <= 0m ? 0m : vacancyMonths * (required - Clamp(actingEfficiency, 0m, required));
        // Потери на вхождение в должность — площадь недобора из модели.
        var onboarding = (decimal)fit.LossArea;

        return Math.Round((gap + onboarding) * monthlyValue, 2);
    }

    /// <summary>
    /// Ранжирование кандидатов: наименьшая площадь потерь за горизонт — лучший.
    /// При равных потерях приоритет у того, кто быстрее входит в должность.
    /// </summary>
    public IReadOnlyList<RankedCandidate> Rank(IEnumerable<CandidateProfile> candidates, double horizonMonths)
    {
        var scored = new List<RankedCandidate>();
        foreach (var c in candidates)
        {
            var input = c.Input with { HorizonMonths = horizonMonths };
            var r = Evaluate(input);
            scored.Add(new RankedCandidate(c.CandidateId, c.DisplayName, r, 0));
        }

        return scored
            .OrderBy(s => s.Result.LossArea)
            .ThenBy(s => s.Result.TimeToCompetenceMonths)
            .Select((s, i) => s with { Rank = i + 1 })
            .ToList();
    }

    private static double Clamp(double v, double lo, double hi) => v < lo ? lo : (v > hi ? hi : v);

    private static decimal Clamp(decimal v, decimal lo, decimal hi) => v < lo ? lo : (v > hi ? hi : v);
}

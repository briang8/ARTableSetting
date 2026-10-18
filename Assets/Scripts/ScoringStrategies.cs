using UnityEngine;

public interface IScoringStrategy
{
    int CalculateScore(int correct, int total, int mistakes, float timeLeft, float timeLimit);
}

public class BasicScoring : IScoringStrategy
{
    public int CalculateScore(int correct, int total, int mistakes, float timeLeft, float timeLimit)
    {
        float score = 100f * correct / total - 5f * mistakes;
        return Mathf.Max(0, Mathf.RoundToInt(score));
    }
}

public class TimeBonusScoring : IScoringStrategy
{
    public int CalculateScore(int correct, int total, int mistakes, float timeLeft, float timeLimit)
    {
        float bonus = 10f * Mathf.Clamp01(timeLeft / Mathf.Max(timeLimit, 0.01f));
        float score = 100f * correct / total - 5f * mistakes + bonus;
        return Mathf.Max(0, Mathf.RoundToInt(score));
    }
}
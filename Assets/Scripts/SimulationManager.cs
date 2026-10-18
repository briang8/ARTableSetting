using System;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private TableSettingLevel level;
    private IScoringStrategy scoring;
    private List<TablewareItem> items = new List<TablewareItem>();
    private float timeRemaining;
    private int mistakes;
    private int placedCount;
    private int score;
    private bool running;

    public int Score => score;
    public int Mistakes => mistakes;
    public int PlacedCount => placedCount;
    public int TotalItems => items.Count;
    public float TimeRemaining => timeRemaining;

    private void OnEnable() => GameEvents.OnItemDropped += HandleItemDropped;
    private void OnDisable() => GameEvents.OnItemDropped -= HandleItemDropped;

    public void StartLevel(TableSettingLevel newLevel, List<TablewareItem> levelItems)
    {
        level = newLevel;
        items = levelItems;
        scoring = CreateStrategy(level.scoringType);
        timeRemaining = level.timeLimit;
        mistakes = 0;
        placedCount = 0;
        score = 0;
        running = true;

        GameEvents.RaiseLevelStarted();
        GameEvents.RaiseScoreChanged(score);
        GameEvents.RaiseTimerChanged(timeRemaining);
    }

    private void Update()
    {
        if (!running) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            GameEvents.RaiseTimerChanged(timeRemaining);
            EndLevel();
            return;
        }
        GameEvents.RaiseTimerChanged(timeRemaining);
    }

    public void HandleItemDropped(TablewareItem item)
    {
        if (!running) return;

        if (item.IsPlacedCorrectly())
        {
            item.LockInSlot();
            placedCount++;
            GameEvents.RaiseItemPlaced(item, true);
        }
        else
        {
            mistakes++;
            item.ReturnToTray();
            GameEvents.RaiseItemPlaced(item, false);
        }

        score = CalculateCurrentScore();
        GameEvents.RaiseScoreChanged(score);

        if (placedCount >= items.Count) EndLevel();
    }

    private void EndLevel()
    {
        running = false;
        score = CalculateCurrentScore();
        GameEvents.RaiseScoreChanged(score);

        SessionData data = new SessionData
        {
            sessionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            levelName = level.levelName,
            score = score,
            timeTaken = level.timeLimit - timeRemaining,
            mistakes = mistakes,
            result = score >= level.passMark ? "Passed" : "Failed"
        };
        // Saving through the session repository is added in the persistence step.
        GameEvents.RaiseLevelCompleted(data);
    }

    private int CalculateCurrentScore()
    {
        return scoring.CalculateScore(placedCount, items.Count, mistakes, timeRemaining, level.timeLimit);
    }

    private IScoringStrategy CreateStrategy(string type)
    {
        switch (type)
        {
            case "TimeBonus":
                return new TimeBonusScoring();
            default:
                return new BasicScoring();
        }
    }
}
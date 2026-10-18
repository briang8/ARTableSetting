using System;

public static class GameEvents
{
    public static event Action OnLevelStarted;
    public static event Action<TablewareItem> OnItemDropped;
    public static event Action<TablewareItem> OnItemPickedUp;
    public static event Action<TablewareItem, bool> OnItemPlaced;
    public static event Action<int> OnScoreChanged;
    public static event Action<float> OnTimerChanged;
    public static event Action<SessionData> OnLevelCompleted;

    public static void RaiseLevelStarted() => OnLevelStarted?.Invoke();
    public static void RaiseItemDropped(TablewareItem item) => OnItemDropped?.Invoke(item);
    public static void RaiseItemPickedUp(TablewareItem item) => OnItemPickedUp?.Invoke(item);
    public static void RaiseItemPlaced(TablewareItem item, bool correct) => OnItemPlaced?.Invoke(item, correct);
    public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void RaiseTimerChanged(float timeLeft) => OnTimerChanged?.Invoke(timeLeft);
    public static void RaiseLevelCompleted(SessionData data) => OnLevelCompleted?.Invoke(data);
}
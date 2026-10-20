using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameScreen { Start, Setup, Simulation, Completion, History }

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject setupPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private GameObject historyPanel;

    [Header("Start screen")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button basicButton;
    [SerializeField] private Button standardButton;
    [SerializeField] private Button historyButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text selectedLevelText;

    [Header("HUD")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text itemsText;
    [SerializeField] private TMP_Text mistakesText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private Button rotateButton;

    [Header("Completion screen")]
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text resultScoreText;
    [SerializeField] private TMP_Text resultTimeText;
    [SerializeField] private TMP_Text resultMistakesText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("History screen")]
    [SerializeField] private TMP_Text historyText;
    [SerializeField] private Button backButton;

    public event Action StartPressed;
    public event Action HistoryPressed;
    public event Action BackPressed;
    public event Action RotatePressed;
    public event Action RestartPressed;
    public event Action MenuPressed;
    public event Action QuitPressed;

    public int SelectedLevelIndex { get; private set; }

    private string hudLevelName = "";
    private int totalItems;
    private int placedCount;
    private int mistakeCount;
    private float hintTimer;

    private void Awake()
    {
        startButton.onClick.AddListener(() => StartPressed?.Invoke());
        historyButton.onClick.AddListener(() => HistoryPressed?.Invoke());
        backButton.onClick.AddListener(() => BackPressed?.Invoke());
        if (quitButton != null) quitButton.onClick.AddListener(() => QuitPressed?.Invoke());
        rotateButton.onClick.AddListener(() => RotatePressed?.Invoke());
        restartButton.onClick.AddListener(() => RestartPressed?.Invoke());
        menuButton.onClick.AddListener(() => MenuPressed?.Invoke());
        basicButton.onClick.AddListener(() => SelectLevel(0));
        standardButton.onClick.AddListener(() => SelectLevel(1));
        SelectLevel(0);
    }

    private void OnEnable()
    {
        GameEvents.OnLevelStarted += HandleLevelStarted;
        GameEvents.OnItemPlaced += HandleItemPlaced;
        GameEvents.OnScoreChanged += HandleScoreChanged;
        GameEvents.OnTimerChanged += HandleTimerChanged;
        GameEvents.OnLevelCompleted += ShowResults;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelStarted -= HandleLevelStarted;
        GameEvents.OnItemPlaced -= HandleItemPlaced;
        GameEvents.OnScoreChanged -= HandleScoreChanged;
        GameEvents.OnTimerChanged -= HandleTimerChanged;
        GameEvents.OnLevelCompleted -= ShowResults;
    }

    private void Update()
    {
        if (hintTimer <= 0f) return;
        hintTimer -= Time.deltaTime;
        if (hintTimer <= 0f) hintText.text = "";
    }

    public void ShowScreen(GameScreen screen)
    {
        startPanel.SetActive(screen == GameScreen.Start);
        setupPanel.SetActive(screen == GameScreen.Setup);
        hudPanel.SetActive(screen == GameScreen.Simulation);
        completionPanel.SetActive(screen == GameScreen.Completion);
        historyPanel.SetActive(screen == GameScreen.History);
    }

    // Called once when a level starts so the HUD knows the level name and item total.
    public void ConfigureHud(string levelName, int total)
    {
        hudLevelName = levelName;
        totalItems = total;
        placedCount = 0;
        mistakeCount = 0;
        RefreshCounts();
    }

    public void ShowResults(SessionData data)
    {
        outcomeText.text = data.result == "Passed" ? "Passed!" : "Failed";
        resultScoreText.text = "Score: " + data.score;
        resultTimeText.text = "Time taken: " + data.timeTaken.ToString("0.0") + " s";
        resultMistakesText.text = "Mistakes: " + data.mistakes;
    }

    public void ShowHistory(List<SessionData> sessions)
    {
        if (sessions == null || sessions.Count == 0)
        {
            historyText.text = "No sessions yet.";
            return;
        }

        string text = "";
        int count = Mathf.Min(5, sessions.Count);
        for (int i = 0; i < count; i++)
        {
            SessionData s = sessions[i];
            text += s.sessionDate + "   " + s.levelName + "   " + s.score + " pts   "
                  + s.timeTaken.ToString("0") + " s   " + s.mistakes + (s.mistakes == 1 ? " mistake   " : " mistakes   ") + s.result + "\n";
        }
        historyText.text = text;
    }

    private void SelectLevel(int index)
    {
        SelectedLevelIndex = index;
        selectedLevelText.text = "Selected level: " + (index == 0 ? "Basic" : "Standard");
    }

    private void HandleLevelStarted()
    {
        placedCount = 0;
        mistakeCount = 0;
        hintText.text = "";
        hintTimer = 0f;
        RefreshCounts();
    }

    private void HandleItemPlaced(TablewareItem item, bool correct)
    {
        if (correct)
        {
            placedCount++;
            ShowHint(item.ItemName + " placed correctly!");
        }
        else
        {
            mistakeCount++;
            ShowHint("Not quite. " + item.GetHint());
        }
        RefreshCounts();
    }

    private void HandleScoreChanged(int score)
    {
        scoreText.text = score.ToString();
    }

    private void HandleTimerChanged(float timeLeft)
    {
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
    }

    private void ShowHint(string message)
    {
        hintText.text = message;
        hintTimer = 4f;
    }

    private void RefreshCounts()
    {
        levelText.text = "Level: " + hudLevelName;
        itemsText.text = placedCount + " / " + totalItems;
        mistakesText.text = mistakeCount.ToString();
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARPlacementManager arManager;
    [SerializeField] private SimulationManager simulationManager;
    [SerializeField] private TablewareFactory factory;
    [SerializeField] private AssetLoader assetLoader;
    [SerializeField] private UIManager ui;
    [SerializeField] private TableSettingLevel basicLevel;
    [SerializeField] private TableSettingLevel standardLevel;

    public ARSession ArSession => arSession;
    public ARPlacementManager Ar => arManager;
    public SimulationManager Simulation => simulationManager;
    public TablewareFactory Factory => factory;
    public AssetLoader Loader => assetLoader;
    public UIManager Ui => ui;
    public TableSettingLevel SelectedLevel => ui.SelectedLevelIndex == 1 ? standardLevel : basicLevel;

    private GameStateMachine machine;
    private readonly ISessionRepository repository = new JsonSessionRepository();
    private StartState startState;
    private SetupState setupState;
    private SimulationState simulationState;
    private CompletionState completionState;

    private void Start()
    {
        machine = new GameStateMachine();
        startState = new StartState(this);
        setupState = new SetupState(this);
        simulationState = new SimulationState(this);
        completionState = new CompletionState(this);

        machine.ChangeState(startState);
    }

    private void OnEnable()
    {
        ui.StartPressed += HandleStartPressed;
        ui.HistoryPressed += HandleHistoryPressed;
        ui.BackPressed += HandleBackPressed;
        ui.RotatePressed += HandleRotatePressed;
        ui.RestartPressed += HandleRestartPressed;
        ui.MenuPressed += HandleMenuPressed;
        ui.QuitPressed += HandleQuitPressed;
        arManager.BeginPressed += HandleBeginPressed;
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        ui.StartPressed -= HandleStartPressed;
        ui.HistoryPressed -= HandleHistoryPressed;
        ui.BackPressed -= HandleBackPressed;
        ui.RotatePressed -= HandleRotatePressed;
        ui.RestartPressed -= HandleRestartPressed;
        ui.MenuPressed -= HandleMenuPressed;
        ui.QuitPressed -= HandleQuitPressed;
        arManager.BeginPressed -= HandleBeginPressed;
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandleStartPressed() => machine.ChangeState(setupState);
    private void HandleBeginPressed() => machine.ChangeState(simulationState);
    private void HandleRestartPressed() => machine.ChangeState(simulationState);
    private void HandleMenuPressed() => machine.ChangeState(startState);
    private void HandleLevelCompleted(SessionData data) => machine.ChangeState(completionState);

    private void HandleHistoryPressed()
    {
        ui.ShowHistory(repository.LoadRecent(5));
        ui.ShowScreen(GameScreen.History);
    }

    private void HandleBackPressed() => ui.ShowScreen(GameScreen.Start);

    private void HandleQuitPressed()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    private void HandleRotatePressed()
    {
        if (TablewareItem.Selected != null) TablewareItem.Selected.Rotate();
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameState
{
    void Enter();
    void Exit();
}

public class GameStateMachine
{
    private IGameState currentState;

    public void ChangeState(IGameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
}

public class StartState : IGameState
{
    private readonly GameController c;
    public StartState(GameController controller) { c = controller; }

    public void Enter()
    {
        // Back to a clean menu: release items, assets and the table.
        c.Factory.DestroyAll();
        c.Loader.ReleaseLevelAssets();
        c.Ar.ClearTable();
        c.Ar.EndSetup();
        c.ArSession.enabled = false;
        c.Ui.ShowScreen(GameScreen.Start);
    }

    public void Exit() { }
}

public class SetupState : IGameState
{
    private readonly GameController c;
    public SetupState(GameController controller) { c = controller; }

    public void Enter()
    {
        c.ArSession.enabled = true;
        c.Ar.BeginSetup();
        c.Ui.ShowScreen(GameScreen.Setup);
    }

    public void Exit()
    {
        c.Ar.EndSetup();
    }
}

public class SimulationState : IGameState
{
    private readonly GameController c;
    public SimulationState(GameController controller) { c = controller; }

    public void Enter()
    {
        c.Ui.ShowScreen(GameScreen.Simulation);
        _ = StartLevelAsync();
    }

    private async Task StartLevelAsync()
    {
        try
        {
            TableSettingLevel level = c.SelectedLevel;
            await c.Loader.LoadLevelAssets(level);

            Transform table = c.Ar.TableTransform;
            if (table == null)
            {
                Debug.LogError("No table placed.");
                return;
            }

            List<TablewareItem> items = c.Factory.CreateItems(level, table);
            c.Ui.ConfigureHud(level.levelName, items.Count);
            c.Simulation.StartLevel(level, items);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void Exit() { }
}

public class CompletionState : IGameState
{
    private readonly GameController c;
    public CompletionState(GameController controller) { c = controller; }

    public void Enter()
    {
        c.Ui.ShowScreen(GameScreen.Completion);
    }

    public void Exit() { }
}
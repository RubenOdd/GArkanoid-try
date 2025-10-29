// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System.Collections.Generic;
using GA.Common.Godot;
using GA.GArkanoid.States;
using Godot;
using GodotPlugins.Game;

/*
GameManager

- Switch scenes
- Score keeping (DONE)
- Lives (DONE)
- Centralized access point to other managers / systems (TODO)
*/
namespace GA.GArkanoid.Systems;

/// <summary>
/// The singleton which controls the game related functions like loading scenes
/// </summary>
public partial class GameManager : Singleton<GameManager>
{
    [Signal]
    public delegate void LivesChangedEventHandler(int lives);

    [Signal]
    public delegate void ScoreChangedEventHandler(int score);

    [Signal]
    public delegate void GameResetEventHandler();

    private int _score = 0;
    private int _lives = 0;
    private SceneTree _sceneTree = null;
    private List<GameStateBase> _gameStates =
    [
        new GameState(),
        new MainMenuState(),
        new SettingsState(),
        new GameOverState(),
        // TODO: Add all the rest
    ];

    private Stack<GameStateBase> _loadedStates = [];

    public GameStateBase ActiveState =>
        _loadedStates.Count > 0 ? _loadedStates.Peek() : null;

    public int Score
    {
        get { return _score; }
        private set
        {
            _score = Mathf.Clamp(value, 0, int.MaxValue);
            EmitSignal(SignalName.ScoreChanged, _score);

            GD.Print($"Score is {Score}");
        }
    }

    public int Lives
    {
        get { return _lives; }
        set
        {
            _lives = Mathf.Clamp(value, 0, Config.MaxLives);
            EmitSignal(SignalName.LivesChanged, _lives);

            GD.Print($"Lives left {Lives}");
        }
    }

    public int LevelIndex
    {
        get;
        private set;
    } = 1;

    public SceneTree SceneTree
    {
        get
        {
            // Lazy load
            if (_sceneTree == null)
            {
                _sceneTree = GetTree();
            }

            return _sceneTree;
        }
    }

    protected override void Initialize()
    {
        GD.Print("GameManager initialized!");
    }

    public void Reset()
    {
        Lives = Config.InitialLives;
        Score = Config.InitialScore;
        LevelIndex = 1;
        EmitSignal(SignalName.GameReset);
    }

    public void AddScore(int score)
    {
        if (score < 0)
        {
            GD.PrintErr("Added score can't be negative!");
            return;
        }

        Score += score;
    }

    public void SubtractScore(int score)
    {
        if (score < 0)
        {
            GD.PrintErr("Added score can't be negative!");
            return;
        }

        Score -= score;
    }

    public void IncreaseLives()
    {
        Lives++;
    }

    public void DecreaseLives()
    {
        Lives--;
    }


    #region State Machine

    public bool ChangeState(StateType stateType)
    {
        if (ActiveState == null)
        {
            GD.PushWarning("No state is currently active. Is this a bug?");
        }

        if (ActiveState != null && !ActiveState.CanTransitionTo(stateType))
        {
            GD.PrintErr($"Invalid state transition from {ActiveState.StateType} to {stateType}");
            return false;
        }

        GameStateBase nextState = GetState(stateType);
        if (nextState == null)
        {
            GD.PrintErr($"The target state of type {stateType} does not exist");
            return false;
        }

        GameStateBase previousState = ActiveState;
        bool keepPrevious = nextState.IsAdditive;

        if (!keepPrevious && ActiveState != null)
        {
            _loadedStates.Pop();
        }

        // ? - Works great in Godot, but is broken in Unity
        previousState?.OnExit();

        if (ActiveState != nextState)
        {
            if (ActiveState != null && !nextState.IsAdditive)
            {
                _loadedStates.Clear();
            }

            _loadedStates.Push(nextState);
        }

        nextState.OnEnter();

        return true;
    }

    public bool ActivatePreviousState()
    {
        if (_loadedStates.Count < 2)
        {
            GD.PrintErr("No previous state to transtion to.");
            return false;
        }

        GameStateBase currentState = _loadedStates.Pop();
        currentState.OnExit();

        ActiveState.OnEnter();

        return true;
    }

    private GameStateBase GetState(StateType stateType)
    {
        foreach (GameStateBase state in _gameStates)
        {
            if (state.StateType == stateType)
            {
                return state;
            }
        }

        return null;
    }

    #endregion State Machine
}
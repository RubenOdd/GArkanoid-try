// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System;
using System.IO;
using System.Collections.Generic;
using GA.Common;
using GA.Common.Godot;
using GA.GArkanoid.Data;
using GA.GArkanoid.States;
using Godot;
using Godot.Collections;

/*
GameManager

- Switch scenes
- Score keeping (DONE)
- Lives (DONE)
- Centralized access point to other managers / systems (DONE)
*/
namespace GA.GArkanoid.Systems;

/// <summary>
/// The singleton which controls the game related functions like loading scenes
/// </summary>
public partial class GameManager : Singleton<GameManager>
{
    public static Vector2I MinWindowSize { get; private set; }

    [Signal]
    public delegate void LivesChangedEventHandler(int lives, int previous);

    [Signal]
    public delegate void ScoreChangedEventHandler(int score);

    [Signal]
    public delegate void GameResetEventHandler();
    private SceneTree _sceneTree = null;
    private AudioStreamPlayer _musicPlayer = null;
    private MusicData _musicData = null;
    private List<GameStateBase> _gameStates =
    [
        new GameState(),
        new MainMenuState(),
        new SettingsState(),
        new GameOverState(),
        new PauseState(),
        new WinState(),
    ];

    private Stack<GameStateBase> _loadedStates = [];

    public GameStateBase ActiveState =>
        _loadedStates.Count > 0 ? _loadedStates.Peek() : null;

    public int Score
    {
        get { return CurrentPlayerData.Score; }
        private set
        {
            CurrentPlayerData.Score = value;
			EmitSignal(SignalName.ScoreChanged, Score);

            GD.Print($"Score is {Score}");
        }
    }

    public int Lives
    {
        get { return CurrentPlayerData.Lives; }
        set
        {
            int previous = CurrentPlayerData.Lives;
            CurrentPlayerData.Lives = value;
			EmitSignal(SignalName.LivesChanged, Lives, previous);

            GD.Print($"Lives left {Lives}");
        }
    }

    public int LevelIndex
    {
        get { return CurrentPlayerData.LevelIndex; }
        set { CurrentPlayerData.LevelIndex = value; }
    }

    public SceneTree SceneTree
    {
        get
        {
            // Lazy load
            _sceneTree ??= GetTree();

            return _sceneTree;
        }
    }

    public PlayerData DefaultPlayerData { get; private set; } = null;
	public PlayerData CurrentPlayerData { get; private set; } = null;

    // TODO: A bit hacky solution. Figure out something better.
	public Dictionary LoadedLevelData { get; set; }

    protected override void Initialize()
    {
        GD.Print("GameManager initialized!");
        MinWindowSize = GetWindow().Size;

        // Initialize audio
        _musicPlayer = new AudioStreamPlayer();
        _musicPlayer.Bus = Config.MusicBusName;
        _musicPlayer.ProcessMode = ProcessModeEnum.Always; // Make sure audio can be played while paused
        AddChild(_musicPlayer);

        _musicData = GD.Load<MusicData>(Config.MusicDataPath);

        DefaultPlayerData = GD.Load<PlayerData>(Config.DefaultPlayerDataPath);
    }

    public void Reset()
    {
        CurrentPlayerData = DefaultPlayerData.Duplicate<PlayerData>(deepCopy: true);
        EmitSignal(SignalName.GameReset);
    }

    public void AddScore(int score)
    {
        if (score < 0)
        {
            GD.PrintErr("Changed score can't be negative!");
            return;
        }

        Score += score;
    }

    public void SubtractScore(int score)
    {
        if (score < 0)
        {
            GD.PrintErr("Changed score can't be negative!");
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

    public void TogglePause()
    {
        SceneTree.Paused = !SceneTree.Paused;
    }

    /// <summary>
    /// Plays the appropriate music track for the given state.
    /// </summary>
    /// <param name="stateType">The state to play music for.</param>
    public void PlayMusic(StateType stateType)
    {
        if (_musicPlayer == null || _musicData == null)
        {
            // Can't play anything.
            return;
        }

        AudioStream audioStream = _musicData.GetMusicForState(stateType);
        if (audioStream == null)
        {
            // No music defined for the state.
            return;
        }

        if (_musicPlayer.Stream == audioStream && _musicPlayer.Playing)
        {
            // The correct track is already playing.7
            return;
        }

        _musicPlayer.Stream = audioStream;
        _musicPlayer.Play();
    }

    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (_musicPlayer == null)
        {
            return;
        }

        _musicPlayer.Stop();
    }

    #region Save
    public void Save(string saveSlotName)
    {
        string savePath = Config.GetSaveFolderPath();

        // Form the save data.
        Dictionary saveData = [];
        Dictionary playerData = CurrentPlayerData.Save();
        Dictionary levelData = LevelManager.Active.Save();

        saveData[Config.PlayerDataKey] = playerData;
        saveData[Config.LevelDataKey] = levelData;

        string jsonData = Json.Stringify(saveData);

        if (SaveToFile(savePath, saveSlotName, Config.SaveFileExtension, jsonData))
        {
            GD.Print($"File saved to {savePath}");
        }
        else
        {
            GD.PrintErr("Saving failed!");
        }
    }

    public void SaveSettings(string saveslotName,ConfigFile config)
    {
        string savePath = Config.GetSaveFolderPath();

        if (SaveSettingsToFile(savePath, saveslotName, Config.SaveSettingsFileExtension, config))
        {
            GD.Print($"Config file for settings has been created at {savePath}");
        }
        else
        {
            GD.PrintErr("Saving settings failed!");
        }
    }

    public bool Load(string saveSlotName)
    {
        string savePath = Config.GetSaveFolderPath();
        Json jsonLoader = new();

        string json = LoadFromFile(savePath, saveSlotName, Config.SaveFileExtension);

        Error error = jsonLoader.Parse(json);
        if (error != Error.Ok)
        {
            GD.PrintErr("Error parsing JSON file: " + error);
            return false;
        }

        Dictionary loadedData = (Dictionary)jsonLoader.Data;
        Dictionary playerData = (Dictionary)loadedData[Config.PlayerDataKey];
        CurrentPlayerData = PlayerData.Deserialize(playerData);

        LoadedLevelData = (Dictionary)loadedData[Config.LevelDataKey];

        return true;
    }

    public ConfigFile LoadSettingsFile(string saveFolder, string saveFile, string saveExtension)
    {
        string saveFilePath = Path.Combine(saveFolder, saveFile + saveExtension);
        ConfigFile config = new();

        try
        {
            config.Load($"{saveFilePath}");
        }
        catch (Exception e)
        {
            GD.PushError($"Something went wrong while loading the settings. Message: {e.Message}");
        }

        return config;
    }

    private string LoadFromFile(string saveFolder, string saveFile, string saveExtension)
    {
        string saveFilePath = Path.Combine(saveFolder, saveFile + saveExtension);
        if (!File.Exists(saveFilePath))
        {
            return null;
        }

        try
        {
            return File.ReadAllText(saveFilePath);
        }
        catch (Exception e)
        {
            GD.PushError($"Something went wrong reading the save file. Message: {e.Message}");
            return null;
        }
    }

    private bool SaveToFile(string saveFolder, string saveFile, string saveExtension, string jsonData)
    {
        if (!Directory.Exists(saveFolder))
        {
            try
            {
                Directory.CreateDirectory(saveFolder);
            }
            catch (ArgumentNullException e)
            {
                GD.PushError($"Argument is null. Message: {e.Message}");
                return false;
            }
            catch (PathTooLongException e)
            {
                GD.PushError($"Save folder path is too long. Message: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                GD.PushError($"Something went wrong! Message: {e.Message}");
                return false;
            }
        }

        string saveFilePath = Path.Combine(saveFolder, saveFile + saveExtension);

        try
        {
            File.WriteAllText(saveFilePath, jsonData);
        }
        catch (Exception e)
        {
            GD.PushError(e.Message);
            return false;
        }

        return true;
    }

    private bool SaveSettingsToFile(string saveFolder, string saveFile, string saveExtension, ConfigFile config)
    {
        if (!Directory.Exists(saveFolder))
        {
            try
            {
                Directory.CreateDirectory(saveFolder);
            }
            catch (ArgumentNullException e)
            {
                GD.PushError($"Argument is null. Message: {e.Message}");
                return false;
            }
            catch (PathTooLongException e)
            {
                GD.PushError($"Save folder path is too long. Message: {e.Message}");
                return false;
            }
            catch (Exception e)
            {
                GD.PushError($"Something went wrong! Message: {e.Message}");
                return false;
            }
        }

        string saveFilePath = Path.Combine(saveFolder, saveFile + saveExtension);

        config.Save($"{saveFilePath}");

        if (!File.Exists(saveFilePath)) GD.Print("Initial settings file does not exist");

        return true;
    }
    #endregion

    #region State Machine

    public bool ChangeState(StateType stateType, bool forceLoad = false)
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

        bool keepPrevious = nextState.IsAdditive;
        GameStateBase previousState = keepPrevious || ActiveState == null ? ActiveState : _loadedStates.Pop();

        // the operators ?? and ? - Works great in Godot, but are broken in Unity
        // ? - checks wheter the left operand is null or not, and executes the right operand if it's not null.
        previousState?.OnExit(keepPrevious);

        while (!keepPrevious && ActiveState != null)
        {
            previousState = _loadedStates.Pop();
            previousState?.OnExit(keepPrevious);
        }

        if (ActiveState != nextState)
        {
            if (ActiveState != null && !nextState.IsAdditive)
            {
                _loadedStates.Clear();
            }

            _loadedStates.Push(nextState);
        }

        nextState.OnEnter(forceLoad);

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
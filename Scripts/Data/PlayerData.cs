using System;
using System.Collections.Generic;
using GA.GArkanoid.Save;
using Godot;
using Godot.Collections;

namespace GA.GArkanoid.Data;

[GlobalClass]
public partial class PlayerData : Resource, ISave
{
    /// <summary>
    ///  A factory method, which creates a new PlayerData object and populates its data with ones loaded from the dictionary.
    /// </summary>
    /// <param name="data">Dictionary containing the data.</param>
    /// <returns></returns>
    public static PlayerData Deserialize(Dictionary data)
    {
        PlayerData result = new();
        result.Load(data);
        return result;
    }

    [Signal] public delegate void LivesChangedEventHandler(int lives);
    [Signal] public delegate void ScoreChangedEventHandler(int score);

    [Export] private int _lives = 0;

    private int _score = 0;

    [Export] public float BallSpeed { get; set; } = 50f;
    [Export] public Vector2 LaunchDirection { get; set; } = new Vector2(1f, 1f);
    [Export] public float PaddleWidth { get; set; } = 1f;
    [Export] public float PaddleSpeed { get; set; } = 50f;

    public int LevelIndex { get; set; } = 1;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = Mathf.Clamp(value, 0, int.MaxValue);
            EmitSignal(SignalName.ScoreChanged, _score);
        }
    }

    public int Lives
    {
        get { return _lives; }
        set
        {
            _lives = Mathf.Clamp(value, 0, Config.MaxLives);
            EmitSignal(SignalName.LivesChanged, _lives);
        }
    }

    public bool IsAlive => Lives > 0;

    public void DeductLife()
    {
        if (Lives > 0)
        {
            Lives--;
        }
    }

    /// <summary>
    /// Serializes the data for storing it.
    /// </summary>
    /// <returns>The dictionary containing the data for this object.</returns>
    public Dictionary Save()
    {
        return new Dictionary
        {
            ["Lives"] = Lives,
            ["Score"] = Score,
            ["BallSpeed"] = BallSpeed,
            ["PaddleWidth"] = PaddleWidth,
            ["PaddleSpeed"] = PaddleSpeed,
            ["LevelIndex"] = LevelIndex,
        };
    }

    public void Load(Dictionary data)
    {
        // TODO: Read default values from the resource file.
        Lives = (int)data.GetValueOrDefault("Lives", 3);
        Score = (int)data.GetValueOrDefault("Score", 0);
        BallSpeed = (float)data.GetValueOrDefault("BallSpeed", 50f);
        PaddleSpeed = (float)data.GetValueOrDefault("PaddleSpeed", 50f);
        PaddleWidth = (float)data.GetValueOrDefault("PaddleWidth", 1f);
        LevelIndex = (int)data.GetValueOrDefault("LevelIndex", 1);
    }
}
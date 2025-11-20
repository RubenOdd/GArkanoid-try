using System.Collections;
using System.Collections.Generic;
using GA.Common;
using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using GA.GArkanoid.Save;
using Godot;
using Godot.Collections;
using System.IO;

namespace GA.GArkanoid;
public partial class LevelManager : Node2D, ISave
{
    private const string LevelContentPath = "res://Scenes/LevelContent/";
    private const string LevelContentName = "Level";
    private const string LevelContentExtension = ".tscn";
    private int _blockCount = 0;
    [Export]
    public Ball CurrentBall { get; private set; } = null;
    [Export]
    protected Paddle CurrentPaddle { get; private set; } = null;

    // TODO: When is this initialized?
    public static LevelManager Active { get; private set; } = null;
    [Export] public EffectPlayer EffectPlayer { get; private set; }

    public override void _Ready()
    {
        Active = this;
        
        LoadLevel(GameManager.Instance.LevelIndex);

        if (EffectPlayer == null)
        {
            EffectPlayer = this.GetNode<EffectPlayer>();
        }

        IList<Block> blocks = this.GetNodesInChildren<Block>();

			// Load the level
        if (GameManager.Instance.LoadedLevelData != null)
        {
            Load(GameManager.Instance.LoadedLevelData);
            GameManager.Instance.LoadedLevelData = null;
        }

        // Initialize Blocks
        foreach (Block block in blocks)
        {
            if (block.IsEnabled)
            {
                _blockCount++;
                block.BlockDestroyed += OnBlockDestroyed;
            }
        }
    }

    public override void _EnterTree()
    {
        GameManager.Instance.LivesChanged += OnLivesChanged;
        GameManager.Instance.ScoreChanged += OnScoreChanged;
    }

    public override void _ExitTree()
    {
        GameManager.Instance.LivesChanged -= OnLivesChanged;
        GameManager.Instance.ScoreChanged -= OnScoreChanged;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(Config.PauseAction))
        {
            GameManager.Instance.ChangeState(StateType.Pause);
        }

        if (Input.IsActionJustPressed(Config.QuickSaveAction))
        {
            GameManager.Instance.Save(Config.QuickSaveName);
        }

        if (Input.IsActionJustPressed("AutoWin"))
        {
            GD.Print("HEARD");
            foreach (Block block in this.GetNodesInChildren<Block>())
            {
                block.Hit();
            }
        }
    }

    public Dictionary Save()
    {
        Dictionary ballData = CurrentBall.Save();
        Dictionary paddleData = CurrentPaddle.Save();

        Dictionary blockData = [];
        foreach (Block block in this.GetNodesInChildren<Block>(recursive: true))
        {
            blockData[block.GUID] = block.IsEnabled;
        }

        Dictionary levelData = [];
        levelData["Ball"] = ballData;
        levelData["Paddle"] = paddleData;
        levelData["Blocks"] = blockData;

        return levelData;
    }

    public void Load(Dictionary data)
    {
        Dictionary blockData = (Dictionary)data["Blocks"];
        IList<Block> blocks = this.GetNodesInChildren<Block>();
        // Restore level data.
        foreach (Block block in blocks)
        {
            bool isEnabled = blockData.TryGetValue(block.GUID, out Variant value) && (bool)value;
            block.IsEnabled = isEnabled;
        }

        CurrentBall.Load((Dictionary)data["Ball"]);
        CurrentPaddle.Load((Dictionary)data["Paddle"]);
    }


    public static string GetLevelcContentPath(int levelIndex)
    {
        return $"{LevelContentPath}/{LevelContentName}{levelIndex}{LevelContentExtension}";
    }

    private bool LoadLevel(int levelIndex)
    {
        string levelContentPath = GetLevelcContentPath(levelIndex);
        PackedScene levelContentScene = GD.Load<PackedScene>(levelContentPath);

        if (levelContentScene == null)
        {
            GD.PrintErr($"Cannot load a level at path {levelContentPath}");
            return false;
        }

        Node2D levelContentNode = levelContentScene.Instantiate<Node2D>();
        if (levelContentNode == null)
        {
            GD.PrintErr($"Level scene at the path {levelContentPath} cannot be loaded.");
            return false;
        }

        AddChild(levelContentNode);
        return true;
    }

    public void OnLivesChanged(int lives)
    {
        if (lives > 0)
        {
            CurrentBall.ResetBall();
        }
        else
        {
            // Game over
            // TODO: DOn't sctually destroy
            CurrentBall.QueueFree();
            CurrentBall = null;
            GameManager.Instance.ChangeState(StateType.GameOver);
            GD.Print("GAme Over");
        }
    }

    private void OnScoreChanged(int score)
    {
        int blocksLeft = GameManager.Instance.SceneTree.Root.GetNode<Node2D>("Level/LevelLayout").GetChildCount() - 1;

        GD.Print($"{blocksLeft} block left");

        if (blocksLeft <= 0)
        {
            GameManager.Instance.ChangeState(StateType.Win);
        }
    }

    private void OnBlockDestroyed(Block block)
    {
        block.BlockDestroyed -= OnBlockDestroyed;
        _blockCount--;

        // Gave up on furthering the level
        if (_blockCount <= 0 && !File.Exists(GetLevelcContentPath(GameManager.Instance.LevelIndex + 1)))
        {
            GameManager.Instance.ChangeState(StateType.Win);
        }
        else
        {
            LoadLevel(GameManager.Instance.LevelIndex + 1);
        }
    }
}
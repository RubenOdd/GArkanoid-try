using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;

namespace GA.GArkanoid;
public partial class LevelManager : Node2D
{
    private const string LevelContentPath = "res://Scenes/LevelContent/";
    private const string LevelContentName = "Level";
    private const string LevelContentExtension = ".tscn";
    [Export]
    public Ball CurrentBall { get; private set; } = null;
    [Export]
    protected Paddle CurrentPaddle { get; private set; } = null;

    // TODO: When is this initialized?
    public static LevelManager Active { get; private set; } = null;

    public override void _Ready()
    {
        Active = this;
        // TODO: Will this work when loading a new level
        GameManager.Instance.Reset();
        GameManager.Instance.LivesChanged += OnLivesChanged;
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
}
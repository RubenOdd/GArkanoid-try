// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;
using Godot;
using System;

namespace GA.GArkanoid.UI;

public partial class UILevel : Control
{
    [Export] private Label _scoreLabel = null;
    [Export] private Label _livesLabel = null;

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

    public override void _Ready()
    {
        OnLivesChanged(GameManager.Instance.Lives, GameManager.Instance.Lives);
        OnScoreChanged(GameManager.Instance.Score);
    }



    private void OnScoreChanged(int score)
    {
        _scoreLabel.Text = $"Score: {score}";
    }


    private void OnLivesChanged(int lives, int previous)
    {
        _livesLabel.Text = $"Lives: {lives}";
    }

}

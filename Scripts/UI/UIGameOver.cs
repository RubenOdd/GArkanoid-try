// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.States;
using GA.GArkanoid.Systems;
using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UIGameOver : Control
{
    [Export] private Button _closeButton = null;
    [Export] private Label _scoreText = null;

    public override void _Ready()
    {
        _scoreText.Text = $"Score: {GameManager.Instance.Score}";

        _closeButton.Pressed += OnClose;
    }

    private void OnClose()
    {
        GameManager.Instance.ChangeState(StateType.MainMenu);
    }

}

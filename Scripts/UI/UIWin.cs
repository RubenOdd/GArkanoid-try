using Godot;
using GA.GArkanoid.States;
using System;
using GA.GArkanoid.Systems;

namespace GA.GArkanoid.UI;

public partial class UIWin : Control
{
    [Export] private Button _restartButton = null;
    [Export] private Button _menuButton = null;

    public override void _EnterTree()
    {
        _restartButton.Pressed += OnRestart;
        _menuButton.Pressed += OnMenu;
    }

    public override void _ExitTree()
    {
        _restartButton.Pressed -= OnRestart;
        _menuButton.Pressed -= OnMenu;
    }

    private void OnRestart()
    {
        GameManager.Instance.Reset();
        GameManager.Instance.ChangeState(StateType.Game);
    }


    private void OnMenu()
    {
        GameManager.Instance.ChangeState(StateType.MainMenu);
    }

}

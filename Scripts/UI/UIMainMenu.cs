// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;
using GA.GArkanoid.States;
using Godot;
using System;

namespace GA.GArkanoid.UI;
public partial class UIMainMenu : Control
{
    [Export] private Button _newGameButton = null;
    [Export] private Button _settingsButtons = null;
    [Export] private Button _quitButton = null;
    [Export] private Button _loadButton = null;

    public override void _Ready()
    {
        _newGameButton.Pressed += OnNewGame;
        _settingsButtons.Pressed += OnSettings;
        _quitButton.Pressed += OnQuit;
        _loadButton.Pressed += OnLoadGame;
    }

    private void OnLoadGame()
    {
        if (GameManager.Instance.Load(Config.QuickSaveName))
        {
            GameManager.Instance.ChangeState(StateType.Game);
        }
        else
        {
            GD.PrintErr("Error while loading game");
        }
    }


    private void OnQuit()
    {
        GameManager.Instance.SceneTree.Quit();
    }


    private void OnSettings()
    {
        GameManager.Instance.ChangeState(StateType.Settings);
    }


    private void OnNewGame()
    {
        GameManager.Instance.Reset();
        GameManager.Instance.ChangeState(StateType.Game);
    }
}

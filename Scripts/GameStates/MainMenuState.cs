// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;

namespace GA.GArkanoid.States;

public class MainMenuState : GameStateBase
{
    public override StateType StateType => StateType.MainMenu;
    public override string ScenePath => "res://Scenes/UI/MainMenu.tscn";

    public MainMenuState()
    {
        AddTargetState(StateType.Game);
        AddTargetState(StateType.Settings);
    }

    public override void OnEnter(bool forceLoad = false)
    {
        base.OnEnter(forceLoad);

        GameManager.Instance.PlayMusic(StateType);
    }
}
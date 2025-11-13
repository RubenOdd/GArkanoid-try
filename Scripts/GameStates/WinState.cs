// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;

namespace GA.GArkanoid.States;

public class WinState : GameStateBase
{
    public override StateType StateType => StateType.Win;
    public override bool IsAdditive => true;
    public override string ScenePath => "res://Scenes/UI/Win.tscn";

    public WinState()
    {
        AddTargetState(StateType.Game);
        AddTargetState(StateType.MainMenu);
    }

    public override void OnEnter(bool forceLoad = false)
    {
        // Important to have in this case, or the scene load will break
        base.OnEnter(forceLoad);

        GameManager.Instance.TogglePause();
    }

    public override void OnExit(bool keepLoaded = false)
    {
        base.OnExit(keepLoaded);

        GameManager.Instance.TogglePause();
    }
}
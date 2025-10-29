// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

namespace GA.GArkanoid.States;

public class SettingsState : GameStateBase
{
    public override StateType StateType => StateType.Settings;
    public override bool IsAdditive => true;
    public override string ScenePath => "res://Scenes/UI/Settings.tscn";

    public SettingsState()
    {
        AddTargetState(StateType.MainMenu);
    }
}
// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

namespace GA.GArkanoid.States;

public class GameOverState : GameStateBase
{
    public override StateType StateType => StateType.GameOver;
    public override string ScenePath => "res://Scenes/UI/GameOver.tscn";

    public GameOverState()
    {
        AddTargetState(StateType.MainMenu);
    }
}
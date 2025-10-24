// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

namespace GA.GArkanoid.States;

public class GameState : GameStateBase
{
    public override StateType StateType => StateType.Game;
    public override string ScenePath => "res://Scenes/Level.tscn";

    public GameState()
    {
        AddTargetState(StateType.Pause);
        AddTargetState(StateType.GameOver);
        AddTargetState(StateType.Win);
    }
}
// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using GA.GArkanoid.Systems;

namespace GA.GArkanoid.States;
public class GameOverState : GameStateBase
{
    public override StateType StateType => StateType.GameOver;
    public override string ScenePath => "res://Scenes/UI/GameOver.tscn";

    public GameOverState()
    {
        AddTargetState(StateType.Game);
        AddTargetState(StateType.MainMenu);
    }

    public override void OnEnter(bool forceLoad = false)
    {
        base.OnEnter(forceLoad);

        GameManager.Instance.StopMusic();
    }
}
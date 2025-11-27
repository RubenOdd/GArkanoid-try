using Godot;

namespace GA.GArkanoid;
[GlobalClass]
public partial class GhostBall : PowerUpEffect
{
    public override PowerUpType PowerUpType => PowerUpType.GhostBall;

    public override void Activate()
    {
        base.Activate();
        LevelManager.Active.CurrentBall.IsGhost = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        LevelManager.Active.CurrentBall.IsGhost = false;
    }
}
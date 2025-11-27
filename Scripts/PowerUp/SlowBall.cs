using GA.GArkanoid.Systems;
using Godot;

namespace GA.GArkanoid;

[GlobalClass]
public partial class SlowBall : PowerUpEffect
{
    public override PowerUpType PowerUpType => PowerUpType.SlowBall;

    public override void Activate()
    {
        base.Activate();
        LevelManager.Active.CurrentBall.Speed = LevelManager.Active.CurrentBall.Speed / 2;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        LevelManager.Active.CurrentBall.Speed = LevelManager.Active.CurrentBall.Speed * 2;
    }
}
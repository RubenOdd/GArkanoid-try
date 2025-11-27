using Godot;

namespace GA.GArkanoid;
[GlobalClass]
public partial class ExtendPaddle : PowerUpEffect
{
    [Export] private float _multiplyer = 2f;
    public override PowerUpType PowerUpType => PowerUpType.ExtendPaddle;

    public override void Activate()
    {
        base.Activate();
        LevelManager.Active.CurrentPaddle.Expand(_multiplyer);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        LevelManager.Active.CurrentPaddle.Shrink(_multiplyer);
    }
}
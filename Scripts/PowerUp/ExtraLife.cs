using GA.GArkanoid.Systems;
using Godot;

namespace GA.GArkanoid;

[GlobalClass]
public partial class ExtraLife : PowerUpEffect
{
    public override PowerUpType PowerUpType => PowerUpType.ExtraLife;

    public override void Activate()
    {
        base.Activate();

        GameManager.Instance.Lives++;
        Deactivate();
    }
}
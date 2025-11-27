using GA.GArkanoid.Systems;
using Godot;

namespace GA.GArkanoid;

[GlobalClass]
public partial class LoseLife : PowerUpEffect
{
    public override PowerUpType PowerUpType => PowerUpType.LoseLife;

    public override void Activate()
    {
        base.Activate();

        GameManager.Instance.Lives--;
        Deactivate();
    }
}
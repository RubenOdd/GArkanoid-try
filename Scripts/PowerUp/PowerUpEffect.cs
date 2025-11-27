using Godot;

namespace GA.GArkanoid;

public abstract partial class PowerUpEffect : Resource, IPowerUpEffect
{
    [Export] public float Duration {get; protected set;}

    public float TimeRemaining { get; set; } = 0f;

    public bool IsActive {get; protected set;} = false;

    public abstract PowerUpType PowerUpType { get; }

    /// <summary>
    /// Applies the power up effect when collected
    /// </summary>
    public virtual void Activate()
    {
        IsActive = true;
        TimeRemaining = Duration;
    }

    /// <summary>
    /// Removes the power up effect when it expires or is manually deactivated
    /// </summary>
    public virtual void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Updates the power up effect each frame (for times effects)
    /// </summary>
    /// <param name="deltaTime">Time pieace</param>
    /// <returns>Wheter the effect is still active or not.</returns>
    public virtual bool Update(float deltaTime)
    {
        if (Duration > 0)
        {
            TimeRemaining -= deltaTime;
            if (TimeRemaining <= 0)
            {
                Deactivate();
                return false;
            }
        }

        return IsActive;
    }
}
namespace GA.GArkanoid;

public interface IPowerUpEffect
{
    float Duration {get;}
    float TimeRemaining {get; set;}
    bool IsActive {get;}
    PowerUpType PowerUpType { get; }
    
    void Activate();
    bool Update(float deltaTime);
    void Deactivate();
}
using Godot;
using GA.Common;
using System;
using System.Collections.Generic;

namespace GA.GArkanoid;

public partial class PowerUpManager : Node2D
{
    [Signal] public delegate void PowerUpActivatedEventHandler (PowerUpType type);
    [Signal] public delegate void PowerUpDeactivatedEventHandler (PowerUpType type);
    [Export] private PowerUpSceneMap[] _powerUpSceneMaps = null;
    [Export] private PowerUpEffect[] _powerUpEffectTemplates = null;
    private List<PowerUpEffect> _activePowerUps = [];

    #region PowerUp Logic

    public override void _Process(double delta)
    {
        for (int i = _activePowerUps.Count - 1; i >= 0; i--)
        {
            PowerUpEffect powerUpEffect = _activePowerUps[i];
            if (!powerUpEffect.Update((float)delta))
            {
                _activePowerUps.RemoveAt(i);
                EmitSignal(SignalName.PowerUpDeactivated, (int)powerUpEffect.PowerUpType);
            }
        }
    }


    public void ActivatePowerUp(PowerUpType powerUpType)
    {
        PowerUpEffect effect = GetActiveEffect(powerUpType);
        if (effect != null)
        {
            HandleStacking(effect);
            return;
        }

        effect = CreatePowerUpEffect(powerUpType);
        if (effect != null)
        {
            effect.Activate();
            _activePowerUps.Add(effect);
            EmitSignal(SignalName.PowerUpActivated, (int)powerUpType);
        }
    }

    public void DeactivatePowerUp(PowerUpType powerUpType)
    {
        PowerUpEffect activeEffect = GetActiveEffect(powerUpType);
        if (activeEffect != null)
        {
            activeEffect.Deactivate();
            _activePowerUps.Remove(activeEffect);
            EmitSignal(SignalName.PowerUpDeactivated, (int)powerUpType);
        }
    }

    private void HandleStacking(PowerUpEffect existing)
    {
        switch (existing.PowerUpType)
        {
            case PowerUpType.ExtendPaddle:
            case PowerUpType.GhostBall:
                existing.TimeRemaining += existing.Duration;
                break;
            default:
                break;
        }
    }

    private PowerUpEffect GetActiveEffect(PowerUpType powerUpType)
    {
        foreach (var powerUpEffect in _activePowerUps)
        {
            if (powerUpEffect.PowerUpType == powerUpType)
            {
                return powerUpEffect;
            }
        }

        return null;
    }


    private PowerUpEffect CreatePowerUpEffect(PowerUpType type)
    {
        foreach (var powerUpEffect in _powerUpEffectTemplates)
        {
            if (powerUpEffect.PowerUpType == type)
            {
                return powerUpEffect.Duplicate<PowerUpEffect>();
            }
        }

        return null;
    }

    #endregion PowerUp Logic

    #region Virtual Implementation
    public void SpawnPowerUp(PowerUpType type, Vector2 position)
    {
        PowerUp powerUp = null;
        PackedScene powerUpScene = GetPowerUpSceneByType(type);

        if (powerUpScene != null)
        {
            powerUp = powerUpScene.Instantiate<PowerUp>();
            powerUp.GlobalPosition = position;
            AddChild(powerUp);

            powerUp.PowerUpCollected += OnPowerUpCollected;
            powerUp.PowerUpExpired += OnPowerUpExpired;
        }
    }

    private void OnPowerUpExpired(PowerUp expired)
    {
        expired.PowerUpCollected -= OnPowerUpCollected;
        expired.PowerUpExpired -= OnPowerUpExpired;
    }


    private void OnPowerUpCollected(PowerUp collected)
    {
        OnPowerUpExpired(collected);

        ActivatePowerUp(collected.Type);
    }


    private PackedScene GetPowerUpSceneByType(PowerUpType powerUpType)
    {
        foreach (var powerUpSceneMap in _powerUpSceneMaps)
        {
            if (powerUpSceneMap.PowerUpType == powerUpType)
            {
                return powerUpSceneMap.PowerUpScene;
            }
        }

        return null;
    }
    #endregion Virtual Implementation
}
